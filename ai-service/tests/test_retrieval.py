from app.services.retrieval import retrieve_relevant_context


def test_retrieval_prioritizes_matching_operational_evidence():
    context = {
        "shipmentId": "s1",
        "events": [
            {"type": "label_created", "description": "Label created"},
            {"type": "delay", "description": "Regional traffic disruption"},
        ],
    }

    result = retrieve_relevant_context(context, "investigate traffic delay")

    assert result["events"][0]["type"] == "delay"
