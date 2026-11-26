import json
import logging
import asyncio

import aio_pika
import httpx

from app.services.settings import Settings

logger = logging.getLogger(__name__)


async def consume_shipment_delays(settings: Settings, stop_event) -> None:
    while not stop_event.is_set():
        connection = None
        try:
            connection = await aio_pika.connect_robust(settings.rabbitmq_url)
            channel = await connection.channel()
            exchange = await channel.declare_exchange(settings.rabbitmq_exchange, aio_pika.ExchangeType.TOPIC, durable=True)
            dead_letter_exchange = await channel.declare_exchange(f"{settings.rabbitmq_exchange}.dlx", aio_pika.ExchangeType.TOPIC, durable=True)
            dead_letter_queue = await channel.declare_queue(f"{settings.rabbitmq_queue}.dead-letter", durable=True)
            await dead_letter_queue.bind(dead_letter_exchange, routing_key="#")
            queue = await channel.declare_queue(settings.rabbitmq_queue, durable=True, arguments={"x-dead-letter-exchange": dead_letter_exchange.name})
            await queue.bind(exchange, routing_key="ShipmentDelayed.v1")
            async with queue.iterator() as messages:
                async for message in messages:
                    if stop_event.is_set():
                        break
                    async with message.process(requeue=False):
                        event = json.loads(message.body)
                        shipment_id = event.get("shipmentId") or event.get("ShipmentId")
                        event_id = event.get("eventId") or event.get("EventId")
                        correlation_id = event.get("correlationId") or event.get("CorrelationId") or event_id
                        if not shipment_id:
                            raise ValueError("ShipmentDelayed.v1 is missing shipmentId")
                        async with httpx.AsyncClient(base_url=settings.backend_url, timeout=30) as client:
                            response = await client.post(
                                "/api/v1/copilot/investigate",
                                json={"shipmentId": shipment_id, "correlationId": correlation_id},
                            )
                            response.raise_for_status()
                        logger.info("Processed ShipmentDelayed.v1 for shipment %s", shipment_id)
        except asyncio.CancelledError:
            raise
        except Exception:
            logger.exception("RabbitMQ consumer disconnected; retrying in 5 seconds")
            await asyncio.sleep(5)
        finally:
            if connection:
                await connection.close()
