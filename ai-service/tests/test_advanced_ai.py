from uuid import uuid4

from app.models.evaluation import EvaluationResult
from app.observability.costs import calculate_usage_cost
from app.services.evaluator import compare_evaluations
from app.services.settings import Settings


def test_cost_tracking_is_deterministic():
    usage = calculate_usage_cost(Settings(model_cost_per_1k_input_tokens=0.01, model_cost_per_1k_output_tokens=0.02), 1000, 500)
    assert usage.estimated_cost == 0.02


def test_evaluation_comparison_reports_average_and_pass_rate():
    results = [
        EvaluationResult(evaluation_id=uuid4(), agent_task_id=uuid4(), scenario_id="a", scores={}, overall_score=1, passed=True, failure_reasons=[]),
        EvaluationResult(evaluation_id=uuid4(), agent_task_id=uuid4(), scenario_id="b", scores={}, overall_score=0.5, passed=False, failure_reasons=["x"]),
    ]
    comparison = compare_evaluations(results)
    assert comparison["average_score"] == 0.75
    assert comparison["pass_rate"] == 0.5
