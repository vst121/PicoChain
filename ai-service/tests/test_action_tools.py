from uuid import uuid4

import httpx
import pytest

from app.tools.action_tools import ActionProposalTool


@pytest.mark.asyncio
async def test_action_proposal_tool_posts_structured_status_update(monkeypatch):
    captured = {}

    class MockTransport(httpx.AsyncBaseTransport):
        async def handle_async_request(self, request):
            captured["payload"] = request.content
            return httpx.Response(201, json={"status": "Proposed"})

    original_client = httpx.AsyncClient

    def client_factory(*args, **kwargs):
        kwargs["transport"] = MockTransport()
        return original_client(*args, **kwargs)

    monkeypatch.setattr(httpx, "AsyncClient", client_factory)
    await ActionProposalTool("http://backend").submit_shipment_status_update(
        uuid4(), uuid4(), "InTransit", "Traffic evidence")

    assert b'"actionType":"UpdateShipmentStatus"' in captured["payload"]
    assert b'newStatus' in captured["payload"]
    assert b'InTransit' in captured["payload"]
