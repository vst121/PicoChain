from typing import Any
from uuid import UUID

import httpx


class ShipmentContextTool:
    """Retrieves canonical shipment context through the backend gateway."""

    def __init__(self, backend_url: str, timeout: float = 15.0) -> None:
        self.backend_url = backend_url
        self.timeout = timeout

    async def get(self, shipment_id: UUID) -> dict[str, Any]:
        async with httpx.AsyncClient(base_url=self.backend_url, timeout=self.timeout) as client:
            response = await client.get(f"/internal/agent-gateway/shipments/{shipment_id}/context")
            response.raise_for_status()
            return response.json()
