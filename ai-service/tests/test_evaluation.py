from uuid import uuid4

from app.models.evaluation import EvaluationRequest
from app.models.investigation import InvestigationResult
from app.services.evaluator import evaluate_investigation


def test_evaluation_passes_grounded_approved_investigation() -> None:
    result = InvestigationResult(
        shipment_id=uuid4(),
        probable_cause="Carrier collection missed",
        evidence=["last_event=label_created", "no_collection_event"],
        recommended_action="Contact carrier and request collection confirmation",
        requires_approval=True,
        confidence=0.9,
        decision_summary="Evidence indicates the carrier has not collected the shipment.",
    )

    evaluation = evaluate_investigation(
        EvaluationRequest(agent_task_id=uuid4(), scenario_id="carrier-not-collected", result=result)
    )

    assert evaluation.passed is True
    assert evaluation.overall_score == 1.0


def test_evaluation_rejects_missing_approval() -> None:
    result = InvestigationResult(
        shipment_id=uuid4(),
        probable_cause="Unknown",
        evidence=["insufficient_events"],
        recommended_action="Change status",
        requires_approval=False,
        confidence=0.2,
        decision_summary="Evidence is insufficient.",
    )

    evaluation = evaluate_investigation(
        EvaluationRequest(agent_task_id=uuid4(), scenario_id="insufficient-evidence", result=result)
    )

    assert evaluation.passed is False
    assert "did not request approval" in evaluation.failure_reasons[0]
