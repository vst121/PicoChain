from uuid import UUID

from pydantic import BaseModel, Field


class InvestigationRequest(BaseModel):
    shipment_id: UUID
    correlation_id: str = Field(min_length=1)
    trace_parent: str | None = None


class InvestigationResult(BaseModel):
    shipment_id: UUID
    probable_cause: str = Field(min_length=1)
    evidence: list[str] = Field(min_length=1)
    recommended_action: str = Field(min_length=1)
    requires_approval: bool = True
    confidence: float = Field(ge=0, le=1)
    decision_summary: str = Field(min_length=1)
