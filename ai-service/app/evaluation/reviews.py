from datetime import datetime, timezone
from typing import Literal
from uuid import UUID, uuid4

from pydantic import BaseModel, Field


class HumanReview(BaseModel):
    review_id: UUID = Field(default_factory=uuid4)
    evaluation_id: UUID
    reviewer: str = Field(min_length=1)
    decision: Literal["approved", "rejected", "needs_revision"]
    notes: str = ""
    reviewed_at: datetime = Field(default_factory=lambda: datetime.now(timezone.utc))
