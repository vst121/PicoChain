from uuid import UUID
import os

from pydantic_ai import Agent

from app.models.investigation import InvestigationResult
from app.services.settings import Settings
from app.tools.shipment_tools import ShipmentContextTool
from app.observability.langfuse import LangfuseClient
from app.observability.costs import calculate_usage_cost


class ShipmentInvestigator:
    """Coordinates backend tools and the configured LLM; it never writes business state."""

    def __init__(self, settings: Settings) -> None:
        self.settings = settings
        self.agent: Agent | None = None
        self.context_tool = ShipmentContextTool(settings.backend_url)
        self.langfuse = LangfuseClient(settings)

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
        usage = result.usage() if callable(getattr(result, "usage", None)) else None
        input_tokens = int(getattr(usage, "input_tokens", 0) or 0)
        output_tokens = int(getattr(usage, "output_tokens", 0) or 0)
        cost = calculate_usage_cost(self.settings, input_tokens, output_tokens)
        try:
            await self.langfuse.record_generation(
                name="shipment-investigation",
                input_data={"shipment_id": str(shipment_id), "context": context},
                output_data=result.output.model_dump(mode="json"),
                model=self.settings.llm_model,
                prompt_version=self.settings.prompt_version,
                usage={"input": input_tokens, "output": output_tokens},
                metadata={
                    "agent_version": self.settings.agent_version,
                    "estimated_cost": cost.estimated_cost,
                },
            )
        except Exception:
            # Observability must never prevent a business investigation from completing.
            pass
        return result.output
