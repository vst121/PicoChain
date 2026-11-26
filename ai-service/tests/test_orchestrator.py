from uuid import uuid4

from app.models.investigation import InvestigationResult
from app.services.orchestrator import Orchestrator


class StubShipmentAgent:
    async def investigate(self, shipment_id):
        return InvestigationResult(
            shipment_id=shipment_id,
            probable_cause="traffic",
            evidence=["delay event"],
            recommended_action="contact carrier",
            decision_summary="Evidence supports a traffic delay.",
            confidence=0.9,
        )


def test_orchestrator_delegates_to_shipment_agent():
    import asyncio

    shipment_id = uuid4()
    result = asyncio.run(Orchestrator(StubShipmentAgent()).investigate_shipment_delay(shipment_id))

    assert result.shipment_id == shipment_id
    assert result.probable_cause == "traffic"
