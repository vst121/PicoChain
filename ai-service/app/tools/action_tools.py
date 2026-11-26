from typing import Any
import json
from uuid import UUID

import httpx


class ActionProposalTool:
    """Submits agent recommendations to the governed backend gateway."""

    def __init__(self, backend_url: str, timeout: float = 15.0) -> None:
        self.backend_url = backend_url
        self.timeout = timeout

    async def submit_shipment_status_update(
        self,
        agent_task_id: UUID,
        shipment_id: UUID,
        new_status: str,
        evidence_summary: str,
    ) -> dict[str, Any]:
        payload = {
            "agentTaskId": str(agent_task_id),
            "actionType": "UpdateShipmentStatus",
            "parametersJson": json.dumps({"shipmentId": str(shipment_id), "newStatus": new_status}),
            "evidenceSummary": evidence_summary,
        }
        async with httpx.AsyncClient(base_url=self.backend_url, timeout=self.timeout) as client:
            response = await client.post("/internal/agent-gateway/recommendations", json=payload)
            response.raise_for_status()
            return response.json()
