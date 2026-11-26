from uuid import uuid4

import httpx
import pytest

from app.tools.shipment_tools import ShipmentContextTool


@pytest.mark.asyncio
async def test_shipment_context_tool_calls_backend_gateway(monkeypatch):
    requested = {}

    class MockTransport(httpx.AsyncBaseTransport):
        async def handle_async_request(self, request):
            requested["path"] = request.url.path
            return httpx.Response(200, json={"shipmentId": str(uuid4())})

    original_client = httpx.AsyncClient

    def client_factory(*args, **kwargs):
        kwargs["transport"] = MockTransport()
        return original_client(*args, **kwargs)

    monkeypatch.setattr(httpx, "AsyncClient", client_factory)
    await ShipmentContextTool("http://backend").get(uuid4())

    assert requested["path"].startswith("/internal/agent-gateway/shipments/")
