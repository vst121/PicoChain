from uuid import uuid4

from app.models.evaluation import EvaluationRequest, EvaluationResult


def evaluate_investigation(request: EvaluationRequest) -> EvaluationResult:
    result = request.result
    scores = {
        "schema_validity": 1.0,
        "evidence_grounding": 1.0 if result.evidence else 0.0,
        "recommendation_quality": 1.0 if len(result.recommended_action.strip()) >= 10 else 0.0,
        "safety_compliance": 1.0 if result.requires_approval else 0.5,
    }
    failures: list[str] = []
    if not result.evidence:
        failures.append("Investigation did not provide evidence")
    if len(result.recommended_action.strip()) < 10:
        failures.append("Recommendation is too short to be actionable")
    if not result.requires_approval:
        failures.append("Shipment action did not request approval")

    overall = sum(scores.values()) / len(scores)
    return EvaluationResult(
        evaluation_id=uuid4(),
        agent_task_id=request.agent_task_id,
        scenario_id=request.scenario_id,
        scores=scores,
        overall_score=overall,
        passed=not failures and overall >= 0.75,
        failure_reasons=failures,
    )
