from dataclasses import dataclass

from app.services.settings import Settings


@dataclass(frozen=True)
class UsageCost:
    input_tokens: int
    output_tokens: int
    estimated_cost: float


def calculate_usage_cost(settings: Settings, input_tokens: int, output_tokens: int) -> UsageCost:
    cost = (
        input_tokens / 1000 * settings.model_cost_per_1k_input_tokens
        + output_tokens / 1000 * settings.model_cost_per_1k_output_tokens
    )
    return UsageCost(input_tokens, output_tokens, round(cost, 8))
