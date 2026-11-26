import base64
from datetime import datetime, timezone
from typing import Any
from uuid import uuid4

import httpx

from app.services.settings import Settings


class LangfuseClient:
    """Small optional Langfuse ingestion adapter with safe no-op behavior."""

    def __init__(self, settings: Settings) -> None:
        self.settings = settings

    @property
    def enabled(self) -> bool:
        return bool(self.settings.langfuse_public_key and self.settings.langfuse_secret_key)

    async def record_generation(
        self,
        *,
        name: str,
        input_data: Any,
        output_data: Any,
        model: str,
        prompt_version: str,
        usage: dict[str, int] | None = None,
        metadata: dict[str, Any] | None = None,
    ) -> str:
        trace_id = str(uuid4())
        if not self.enabled:
            return trace_id

        auth = base64.b64encode(
            f"{self.settings.langfuse_public_key}:{self.settings.langfuse_secret_key}".encode()
        ).decode()
        event = {
            "batch": [{
                "id": str(uuid4()),
                "type": "generation-create",
                "timestamp": datetime.now(timezone.utc).isoformat(),
                "body": {
                    "id": trace_id,
                    "traceId": trace_id,
                    "name": name,
                    "model": model,
                    "input": input_data,
                    "output": output_data,
                    "promptName": prompt_version,
                    "usage": usage or {},
                    "metadata": metadata or {},
                },
            }]}
        async with httpx.AsyncClient(timeout=10) as client:
            response = await client.post(
                f"{self.settings.langfuse_host.rstrip('/')}/api/public/ingestion",
                headers={"Authorization": f"Basic {auth}"},
                json=event,
            )
            response.raise_for_status()
        return trace_id
