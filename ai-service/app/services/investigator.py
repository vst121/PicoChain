from uuid import UUID
import os

from pydantic_ai import Agent

from app.models.investigation import InvestigationResult
from app.services.settings import Settings
from app.tools.shipment_tools import ShipmentContextTool


class ShipmentInvestigator:
    """Coordinates backend tools and the configured LLM; it never writes business state."""

    def __init__(self, settings: Settings) -> None:
        self.settings = settings
        self.agent: Agent | None = None
        self.context_tool = ShipmentContextTool(settings.backend_url)

    async def investigate(self, shipment_id: UUID) -> InvestigationResult:
        if self.settings.llm_provider == "none":
            raise RuntimeError("No LLM provider configured; set PICOCHAIN_LLM_PROVIDER for real execution")

        context = await self.context_tool.get(shipment_id)

        if self.agent is None:
            if self.settings.openai_api_key:
                os.environ.setdefault("OPENAI_API_KEY", self.settings.openai_api_key)
            self.agent = Agent(
                self.settings.llm_model,
                output_type=InvestigationResult,
                system_prompt=(
                    "You investigate logistics shipment delays. Use only the supplied backend context. "
                    "Return concise evidence-based conclusions. Never claim an action was executed. "
                    "Sensitive or business-changing actions always require approval."
                ),
                name="shipment-agent",
            )

        prompt = (
            f"Investigate shipment {shipment_id}. Backend context JSON follows:\n"
            f"{context}\n\n"
            "Identify the probable cause, cite concrete evidence fields, and recommend the next action."
        )
        result = await self.agent.run(prompt)
        return result.output
