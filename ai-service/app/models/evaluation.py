from uuid import UUID

from pydantic import BaseModel, Field

from .investigation import InvestigationResult


class EvaluationRequest(BaseModel):
    agent_task_id: UUID
    scenario_id: str = Field(min_length=1)
    result: InvestigationResult


class EvaluationResult(BaseModel):
    evaluation_id: UUID
    agent_task_id: UUID
    scenario_id: str
    evaluator_type: str = "deterministic"
    scores: dict[str, float]
    overall_score: float = Field(ge=0, le=1)
    passed: bool
    failure_reasons: list[str]
