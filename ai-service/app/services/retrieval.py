from typing import Any


def retrieve_relevant_context(context: dict[str, Any], query: str, limit: int = 5) -> dict[str, Any]:
    """Ranks canonical shipment events by relevance to the investigation query."""
    events = context.get("events", context.get("Events", []))
    terms = {term.lower() for term in query.split() if len(term) > 3}

    def score(event: dict[str, Any]) -> int:
        text = " ".join(str(value) for value in event.values()).lower()
        return sum(term in text for term in terms)

    ranked = sorted(events, key=score, reverse=True)[:limit]
    return {
        "shipmentId": context.get("shipmentId", context.get("ShipmentId")),
        "shipmentNumber": context.get("shipmentNumber", context.get("ShipmentNumber")),
        "shipmentStatus": context.get("shipmentStatus", context.get("ShipmentStatus")),
        "orderId": context.get("orderId", context.get("OrderId")),
        "carrierId": context.get("carrierId", context.get("CarrierId")),
        "events": ranked,
    }
