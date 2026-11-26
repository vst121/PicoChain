import json
import logging

import aio_pika
import httpx

from app.services.settings import Settings

logger = logging.getLogger(__name__)


async def consume_shipment_delays(settings: Settings, stop_event) -> None:
    connection = await aio_pika.connect_robust(settings.rabbitmq_url)
    channel = await connection.channel()
    exchange = await channel.declare_exchange(settings.rabbitmq_exchange, aio_pika.ExchangeType.TOPIC, durable=True)
    queue = await channel.declare_queue(settings.rabbitmq_queue, durable=True)
    await queue.bind(exchange, routing_key="ShipmentDelayed.v1")

    async with queue.iterator() as messages:
        async for message in messages:
            if stop_event.is_set():
                break
            async with message.process(requeue=True):
                event = json.loads(message.body)
                async with httpx.AsyncClient(base_url=settings.backend_url, timeout=30) as client:
                    response = await client.post(
                        "/api/v1/copilot/investigate",
                        json={
                            "shipmentId": event["ShipmentId"],
                            "correlationId": event.get("CorrelationId", event["EventId"]),
                        },
                    )
                    response.raise_for_status()
                logger.info("Processed ShipmentDelayed.v1 for shipment %s", event["ShipmentId"])

    await connection.close()
