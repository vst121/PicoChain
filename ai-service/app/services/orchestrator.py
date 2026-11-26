from uuid import UUID

from app.models.investigation import InvestigationResult
from app.services.investigator import ShipmentInvestigator


class Orchestrator:
    """Routes business requests to the specialized logistics agent."""

    def __init__(self, shipment_agent: ShipmentInvestigator) -> None:
        self.shipment_agent = shipment_agent

    async def investigate_shipment_delay(self, shipment_id: UUID) -> InvestigationResult:
        return await self.shipment_agent.investigate(shipment_id)
