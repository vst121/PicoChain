from uuid import UUID

from pydantic import BaseModel, Field

from .investigation import InvestigationResult


class EvaluationRequest(BaseModel):
    agent_task_id: UUID
    scenario_id: str = Field(min_length=1)
    result: InvestigationResult
    agent_version: str = "shipment-agent-v1"
    prompt_version: str = "shipment-investigation-v1"
    model_identifier: str = "unknown"
    input_tokens: int = 0
    output_tokens: int = 0
    estimated_cost: float = 0.0


class EvaluationResult(BaseModel):
    evaluation_id: UUID
    agent_task_id: UUID
    scenario_id: str
    evaluator_type: str = "deterministic"
    scores: dict[str, float]
    overall_score: float = Field(ge=0, le=1)
    passed: bool
    failure_reasons: list[str]
    agent_version: str = "shipment-agent-v1"
    prompt_version: str = "shipment-investigation-v1"
    model_identifier: str = "unknown"
    input_tokens: int = 0
    output_tokens: int = 0
    estimated_cost: float = 0.0
