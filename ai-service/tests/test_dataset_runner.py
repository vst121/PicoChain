from app.evaluation.dataset_runner import load_scenarios


def test_shipment_dataset_contains_repeatable_scenarios():
    scenarios = load_scenarios()

    assert len(scenarios) >= 5
    assert {item["scenario_id"] for item in scenarios} >= {
        "traffic-delay",
        "carrier-not-collected",
        "insufficient-evidence",
    }
