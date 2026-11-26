# PicoChain

**AI-Native Logistics Intelligence & Orchestration Platform**

PicoChain is an independent, AI-native logistics platform that uses specialized AI agents to investigate operational problems, analyze logistics data, recommend actions, and execute approved business workflows.

The project demonstrates production-oriented Agentic AI engineering combined with enterprise software architecture, event-driven systems, and end-to-end observability.

## Core Capabilities

- **Logistics Management:** Customers, orders, shipments, carriers, and shipment events.
- **AI Investigation:** Identify shipment delays, investigate root causes, and generate evidence-based recommendations.
- **Agent Orchestration:** Coordinate specialized agents through controlled tools and workflows.
- **AI Copilot:** Interact conversationally with logistics operations.
- **Human-in-the-Loop:** Review, approve, or reject proposed business actions.
- **Event-Driven Processing:** Asynchronous workflows powered by RabbitMQ.
- **Observability:** Monitor application activity, distributed traces, agent executions, and system health.
- **AI Evaluation:** Measure agent accuracy, task completion, policy compliance, performance, and cost.

## Architecture & Technology Stack

| Component                     | Technology                                        |
| ----------------------------- | ------------------------------------------------- |
| Frontend                      | Angular 18, TypeScript, Angular Material, Signals |
| Backend                       | C#, .NET 10, ASP.NET Core                         |
| Database                      | PostgreSQL, Entity Framework Core                 |
| AI Service                    | Python, FastAPI, Pydantic AI                      |
| Messaging                     | RabbitMQ                                          |
| Real-time Updates             | Server-Sent Events (SSE)                          |
| Telemetry                     | OpenTelemetry                                     |
| Development Diagnostics       | .NET Aspire Dashboard                             |
| Application Monitoring        | SigNoz                                            |
| AI Observability & Evaluation | Langfuse                                          |
| Logging                       | Serilog, Microsoft.Extensions.Logging             |
| Infrastructure                | Docker Compose                                    |

## AI Agents

PicoChain is designed around four specialized agents:

- **Orchestrator Agent:** Coordinates tasks and delegates work.
- **Shipment Agent:** Investigates shipment issues and recommends resolutions.
- **Order Agent:** Validates orders and identifies inconsistencies.
- **Integration Agent:** Analyzes external integrations and generates mapping and validation definitions.

The initial implementation focuses on the Orchestrator and Shipment Agents.

## Primary Workflow

**AI-Powered Shipment Delay Investigation and Resolution**

1. A shipment delay event is received.
2. The backend publishes an event through RabbitMQ.
3. AI agents investigate the shipment using controlled tools.
4. The agent analyzes evidence and recommends a resolution.
5. The backend validates the proposed action.
6. A user reviews and approves the proposal.
7. The .NET backend executes and verifies the operation.
8. The result is recorded and communicated to the frontend.

## Architectural Principles

- The C# backend owns business state, rules, permissions, and execution.
- AI agents provide intelligence, not unrestricted business control.
- Agents access business capabilities through explicitly defined tools and APIs.
- Sensitive operations require appropriate authorization and human approval.
- Every important operation must be traceable, observable, and auditable.
- AI quality must be evaluated independently of successful execution.
- Frontend, backend, and AI service maintain independent architectural boundaries.

## Observability & Evaluation

OpenTelemetry provides unified instrumentation across services.

- **Aspire Dashboard:** Local development diagnostics.
- **SigNoz:** Centralized logs, metrics, distributed traces, dashboards, and alerts.
- **Langfuse:** Agent traces, LLM generations, token usage, costs, and AI evaluations.

The objective is complete visibility from frontend requests and backend operations to agent execution, tool calls, and final business actions.

## Project Structure

```text
PicoChain/
├── backend/          # .NET application and domain
├── ai-service/       # Python agents and AI evaluation
├── frontend/         # Angular application
├── infrastructure/   # Docker and observability
├── docs/             # Architecture and project documentation
├── docker-compose.yml
└── README.md
```

## Development Roadmap

1. Foundation and infrastructure.
2. Logistics domain and backend APIs.
3. AI agent foundation.
4. Shipment investigation workflow.
5. Angular application and real-time updates.
6. Observability and AI evaluation.
7. Advanced agents and autonomous workflows.

## Project Objective

PicoChain is not simply a chatbot connected to a logistics database.

It demonstrates how specialized AI agents can investigate real business problems, collaborate through controlled tools, make evidence-based recommendations, and participate in enterprise workflows under explicit application governance.

**Build reliable software. Give AI meaningful capabilities. Keep business execution under control.**

## Enable real AI execution

```powershell
Copy-Item ai-service/.env.example ai-service/.env
# Edit ai-service/.env and set OPENAI_API_KEY.
cd ai-service
uv run uvicorn app.main:app --reload --port 8000
```

The frontend calls only the C# API. The C# API calls the AI service and remains the authority for action proposals, approvals, and business-state changes. Without provider credentials, the AI service fails explicitly instead of fabricating an investigation.
# PicoChain

AI-native logistics investigation and orchestration demo.

## Demo startup

Prerequisites:

- Docker Desktop with Compose
- An OpenAI-compatible API key for real agent execution

Start the complete local environment:

```powershell
$env:PICOCHAIN_LLM_PROVIDER = "openai"
$env:OPENAI_API_KEY = "your-key"
$env:PICOCHAIN_LANGFUSE_PUBLIC_KEY = "your-langfuse-public-key"
$env:PICOCHAIN_LANGFUSE_SECRET_KEY = "your-langfuse-secret-key"
docker compose up --build
```

Open:

- Frontend: http://localhost:4200
- Backend health: http://localhost:5000/health
- AI service health: http://localhost:8000/health
- RabbitMQ management: http://localhost:15672

Demo flow:

1. Open Shipments and mark a shipment delayed.
2. Open AI Copilot and investigate that shipment, or wait for the event-triggered workflow.
3. Review the evidence and recommendation in Approval Center.
4. Approve the proposal.
5. Verify the shipment status, agent task, audit record, and dashboard counters.

Automated smoke check:

```powershell
.\scripts\demo-smoke.ps1
```

If no LLM provider is configured, the AI service runs in explicitly labelled development mode and real investigations return an unavailable-provider response.

Langfuse is optional. When its keys are configured, generation traces, prompt versions, token usage, model metadata, and estimated costs are sent to the configured Langfuse host.
