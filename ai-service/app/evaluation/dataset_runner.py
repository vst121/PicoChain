import json
from pathlib import Path
from typing import Iterable

from app.models.evaluation import EvaluationRequest, EvaluationResult
from app.services.evaluator import evaluate_investigation


def load_scenarios(path: Path | None = None) -> list[dict[str, object]]:
    scenario_path = path or Path(__file__).parents[2] / "datasets" / "shipment_scenarios.json"
    return json.loads(scenario_path.read_text(encoding="utf-8"))


def evaluate_dataset(requests: Iterable[EvaluationRequest]) -> list[EvaluationResult]:
    return [evaluate_investigation(request) for request in requests]
