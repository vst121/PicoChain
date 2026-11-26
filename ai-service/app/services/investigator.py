from uuid import UUID

import httpx

from app.models.investigation import InvestigationResult
from app.services.settings import Settings


class ShipmentInvestigator:
    """Coordinates backend tools and the configured LLM; it never writes business state."""

    def __init__(self, settings: Settings) -> None:
        self.settings = settings

    async def investigate(self, shipment_id: UUID) -> InvestigationResult:
        async with httpx.AsyncClient(base_url=self.settings.backend_url, timeout=15) as client:
            response = await client.get(f"/internal/agent-gateway/shipments/{shipment_id}/context")
            response.raise_for_status()
            context = response.json()

        if self.settings.llm_provider == "none":
            raise RuntimeError("No LLM provider configured; set PICOCHAIN_LLM_PROVIDER for real execution")

        # The provider adapter is intentionally isolated here so agent prompts and model
        # clients can evolve without leaking into API contracts or business state.
        raise NotImplementedError(f"LLM provider '{self.settings.llm_provider}' is not configured yet")
