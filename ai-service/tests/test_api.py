from uuid import uuid4

from fastapi.testclient import TestClient

from app.main import app


client = TestClient(app)


def test_health_returns_service_status() -> None:
    response = client.get("/health")

    assert response.status_code == 200
    assert response.json() == {"status": "ok", "service": "picochain-ai-service"}


def test_investigation_requires_correlation_id() -> None:
    response = client.post(
        "/api/v1/investigations",
        json={"shipment_id": str(uuid4())},
    )

    assert response.status_code == 422


def test_investigation_is_unavailable_without_llm_provider() -> None:
    response = client.post(
        "/api/v1/investigations",
        json={"shipment_id": str(uuid4()), "correlation_id": "test-correlation"},
    )

    # Development mode must be explicit; it must not pretend an AI decision occurred.
    assert response.status_code == 503
    assert "No LLM provider configured" in response.json()["detail"]
