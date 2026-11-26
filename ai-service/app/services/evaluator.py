from uuid import uuid4

from app.models.evaluation import EvaluationRequest, EvaluationResult


def compare_evaluations(results: list[EvaluationResult]) -> dict[str, object]:
    """Produces a compact comparison for agent/model/prompt experiment runs."""
    if not results:
        return {"count": 0, "average_score": 0.0, "pass_rate": 0.0}
    return {
        "count": len(results),
        "average_score": round(sum(item.overall_score for item in results) / len(results), 4),
        "pass_rate": round(sum(item.passed for item in results) / len(results), 4),
        "scenarios": sorted({item.scenario_id for item in results}),
        "agent_versions": sorted({item.agent_version for item in results}),
        "prompt_versions": sorted({item.prompt_version for item in results}),
        "models": sorted({item.model_identifier for item in results}),
        "estimated_cost": round(sum(item.estimated_cost for item in results), 8),
    }


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
        agent_version=request.agent_version,
        prompt_version=request.prompt_version,
        model_identifier=request.model_identifier,
        input_tokens=request.input_tokens,
        output_tokens=request.output_tokens,
        estimated_cost=request.estimated_cost,
    )
