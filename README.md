# PicoChain

PicoChain is an AI-native logistics intelligence and orchestration platform.

## Local development

Start the required infrastructure:

```sh
docker compose up -d
```

Run the backend from `backend/PicoChain.Api` and the AI service from `ai-service` with:

```sh
uv run uvicorn app.main:app --reload --port 8000
```

Run the Angular application from `frontend` with:

```sh
pnpm install
pnpm start
```

The current development flow requires `PICOCHAIN_LLM_PROVIDER` and the corresponding provider credentials for real AI investigations. Without them, the AI service explicitly returns a configuration error and does not fabricate a result.
