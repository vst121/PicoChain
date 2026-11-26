# PicoChain - Codex Master Prompt

## 1. Your Role

Act as a **Senior Software Architect, Senior AI Engineer, and Senior AI Product Engineer**.

You are responsible for designing and implementing PicoChain as a production-oriented, AI-native logistics intelligence and orchestration platform.

Your responsibilities include:

- Software architecture and technical decision-making.
- Enterprise backend engineering with C# and .NET.
- Agentic AI engineering with Python.
- Frontend engineering with Angular.
- Distributed systems and event-driven architecture.
- AI observability, evaluation, monitoring, and reliability.
- Security, testing, maintainability, and deployment architecture.

Do not approach this project as a simple CRUD application or an AI chatbot demo.

The objective is to demonstrate how modern Agentic AI can perform meaningful enterprise logistics work within a reliable, observable, and controlled software architecture.

---

# 2. Product Vision

## What is PicoChain?

PicoChain is a lightweight, independent, AI-native Logistics Intelligence and Orchestration Platform.

It is conceptually inspired by modern logistics integration platforms such as ecovium SimpleChain, but it is not intended to be a full clone.

The main purpose is to demonstrate how AI agents can intelligently investigate logistics operations, identify problems, recommend actions, and execute approved business operations.

PicoChain connects logistics business processes, enterprise services, and AI agents through a unified domain model and controlled execution architecture.

### Core product capabilities

- Logistics data integration and normalization.
- Order and shipment management.
- Shipment event processing.
- Operational exception detection.
- AI-powered investigation and decision support.
- Agent-based workflow orchestration.
- Human-in-the-loop approvals.
- Controlled execution of business actions.
- AI-powered operational assistance.
- Integration mapping and data validation.
- End-to-end observability and AI evaluation.

### Core architectural principles

**Principle 1: AI provides intelligence, not business control.**

Agents provide intelligence and perform tasks. The application owns business state, rules, permissions, and execution.

**Principle 2: Business state belongs to the backend.**

Python agents must never directly modify the business database. All business mutations must go through the C# backend and its domain/application layers.

**Principle 3: Every AI decision and business action must be observable, traceable, measurable, and evaluable.**

**Principle 4: Human approval is required for sensitive or business-critical actions.**

**Principle 5: Use real AI capabilities.**

Do not simulate agent execution through hardcoded responses. Use actual LLM calls and tool execution. If an external dependency is unavailable, provide an explicitly labelled development mode rather than pretending that real AI execution occurred.

---

# 3. Primary MVP Scenario

## AI-Powered Shipment Delay Investigation and Resolution

This is the primary end-to-end business scenario.

A shipment becomes delayed, and PicoChain automatically investigates the problem, identifies the probable cause, recommends an appropriate action, and requests approval before executing the resolution.

### Workflow

1. A shipment delay event is generated or received.
2. The C# backend receives the event.
3. The backend publishes a `ShipmentDelayed` event through RabbitMQ.
4. RabbitMQ delivers the event to the Python AI service.
5. The Orchestrator Agent receives the task and delegates it to the Shipment Agent.
6. The Shipment Agent retrieves relevant business context through explicitly defined tools.
7. It investigates shipment history, order information, customer information, carrier details, and available shipment events.
8. The agent analyzes the evidence and identifies the probable cause.
9. The agent generates a structured investigation result and recommendation.
10. The Python AI service sends the recommendation to the C# backend.
11. The C# backend validates the recommendation and applies business policies.
12. An action proposal and approval request are created.
13. The Angular frontend receives the approval request.
14. A user reviews the investigation, evidence, and proposed action.
15. After approval, the C# backend executes the business operation.
16. The backend verifies the result.
17. The backend publishes the corresponding business event.
18. The Angular frontend receives real-time updates through Server-Sent Events.
19. All business operations, agent executions, decisions, and results are recorded.

The same investigation must also be possible through the AI Copilot interface.

The architecture must support both event-triggered and user-triggered agent workflows.

---

# 4. Technology Stack

Use the following technologies unless a technical limitation justifies a documented alternative.

## 4.1 Frontend

- Angular 18
- TypeScript
- Angular Material
- Angular Standalone Components
- Angular Signals
- Angular Router
- Dependency Injection
- Reactive Forms where appropriate
- Native Fetch API and Promises for application HTTP communication
- Server-Sent Events (SSE) for server-to-client real-time updates

### Frontend architectural requirements

Use a modern standalone Angular architecture.

Prefer Signals-first state management:

- `signal`
- `computed`
- `effect`

Use feature-level state services.

Avoid using RxJS as the primary application programming model.

Use native Fetch API, async/await, and AbortController for application-level asynchronous operations where appropriate.

Angular 18 still depends on RxJS internally, and Angular HttpClient exposes Observable-based APIs. Do not attempt to remove framework dependencies artificially. The objective is to avoid unnecessary Observable pipelines in application feature code.

### Real-time communication

Do not introduce SignalR in the initial implementation.

Use SSE for:

- Agent execution progress.
- Shipment status updates.
- Operational task updates.
- Approval notifications.
- Agent task completion and failure.
- Investigation results.

Use Fetch with `ReadableStream` when POST-based streaming, custom headers, or authentication requirements make native EventSource unsuitable.

SSE is sufficient for the initial one-way server-to-client communication requirements.

Introduce WebSockets or SignalR only if a demonstrated future requirement justifies them.

### Frontend independence

The Angular application must be independently buildable, testable, and deployable.

It communicates exclusively with the C# backend through documented, versioned HTTP APIs and SSE endpoints.

The frontend must never communicate directly with the Python AI service.

Frontend configuration must be externalized.

Do not duplicate business rules or authorization logic in the frontend.

---

## 4.2 Backend

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- FluentValidation where appropriate
- Clean Architecture
- Domain-Driven Design
- Modular architecture

### Backend responsibilities

The C# backend is the authoritative owner of:

- Logistics domain entities.
- Business state.
- Business rules and validations.
- Database access and transactions.
- REST APIs.
- Authentication and authorization.
- Business event publishing.
- Workflow execution.
- Human approval processes.
- Agent Execution Gateway.
- Action validation and execution.
- Audit trails.
- Business telemetry.

Python agents must never directly access or modify the business database.

All business operations must be executed through explicit backend application services and domain rules.

Example:

`update_shipment_status(shipment_id, new_status)`

The backend must validate:

- Whether the requested status transition is valid.
- Whether the initiating user or workflow is authorized.
- Whether the operation satisfies business policies.
- Whether human approval is required.
- Whether the operation has already been executed.

Only then may the backend execute the operation.

---

## 4.3 Python AI Service

- Python
- FastAPI
- Pydantic
- Pydantic AI
- uv for dependency and project management

Use Pydantic AI as the primary agent framework.

Implement a configurable LLM provider abstraction.

The AI service must support:

- Structured LLM outputs.
- Typed agent inputs and outputs.
- Explicit tool definitions.
- Agent orchestration.
- Tool execution.
- Context retrieval.
- RAG where appropriate.
- Prompt and model configuration.
- AI observability.
- Agent evaluation.
- Token and cost tracking.
- Error handling and retry policies.

### Agent architecture

Implement four agents.

**1. Orchestrator Agent**

Responsibilities:

- Understand incoming requests and events.
- Determine the appropriate workflow.
- Delegate tasks to specialized agents.
- Coordinate multi-step operations.
- Aggregate agent results.
- Manage task progression.
- Escalate unresolved problems.

**2. Shipment Agent**

Responsibilities:

- Investigate shipment delays.
- Retrieve shipment and order context.
- Analyze shipment events.
- Investigate carrier-related issues.
- Identify probable root causes.
- Generate evidence-based recommendations.
- Prepare customer notifications where appropriate.

**3. Order Agent**

Responsibilities:

- Validate incoming orders.
- Detect missing or inconsistent data.
- Identify business rule violations.
- Recommend corrections.
- Initiate approved order updates.

**4. Integration Agent**

Responsibilities:

- Analyze external API specifications.
- Analyze partner data formats and sample payloads.
- Map external schemas to the PicoChain canonical model.
- Generate transformation definitions.
- Generate validation rules.
- Generate test cases for integration mappings.

### Implementation strategy

Do not implement all four agents immediately.

Start with:

- Orchestrator Agent.
- Shipment Agent.

Add Order Agent and Integration Agent after the first complete workflow is working.

Avoid unnecessary multi-agent complexity.

Each agent must have a clear responsibility and explicit typed contracts.

---

# 5. Logistics Domain Model

Start with the following domain entities:

- Customer
- Product
- Order
- OrderItem
- Shipment
- Carrier
- ShipmentEvent
- OperationalTask
- AgentTask
- ActionProposal
- ApprovalRequest
- AuditRecord

Define meaningful relationships between entities.

Use a canonical logistics model.

Create realistic seed data containing:

- Normal shipments.
- Delayed shipments.
- Incomplete shipments.
- Inconsistent shipment statuses.
- Multiple customers.
- Multiple carriers.
- Orders with missing mandatory information.
- Different shipment histories.
- Conflicting operational events.

Avoid unrealistic or arbitrary test data.

The domain model must support future expansion without unnecessary abstractions.

---

# 6. Messaging and Communication Architecture

## 6.1 Synchronous communication

Use HTTP APIs for synchronous interactions.

Expected communication:

- Angular → C# backend.
- C# backend → Python AI service.
- Python AI service → C# backend for context, tools, and proposals.
- C# backend → Angular through SSE.

Angular must never call the Python service directly.

## 6.2 Asynchronous communication

Use RabbitMQ for asynchronous events and background workflows.

Initial event contracts:

- `OrderReceived`
- `OrderValidationFailed`
- `ShipmentDelayed`
- `ShipmentDelivered`
- `ActionProposalCreated`
- `ActionApproved`
- `ActionRejected`
- `AgentTaskStarted`
- `AgentTaskCompleted`
- `AgentTaskFailed`
- `AgentEvaluationCompleted`

Requirements:

- Versioned event contracts.
- Correlation IDs.
- W3C Trace Context propagation.
- Idempotent message processing.
- Retry policies.
- Dead-letter handling.
- Clear error handling.
- Reliable event publishing.

Use the transactional outbox pattern where appropriate to avoid inconsistent database and message-publishing operations.

Do not introduce Kafka or unnecessary messaging infrastructure for the MVP.

---

# 7. Observability, Telemetry, and Monitoring

Observability is a first-class architectural capability.

It must be implemented from the beginning, not added at the end.

The objective is to make the complete execution of every important business operation and AI task visible.

## 7.1 Agreed observability technology stack

Use:

**OpenTelemetry**

The common instrumentation and telemetry standard across the system.

**.NET Aspire Dashboard**

The primary local development diagnostics dashboard for inspecting logs, metrics, and distributed traces.

It can be used independently without adopting .NET Aspire as the application's orchestration framework.

**SigNoz**

The unified, self-hosted observability and monitoring platform.

Use SigNoz for:

- Application logs.
- Infrastructure and application metrics.
- Distributed traces.
- Dashboards.
- Alerts.
- Service health monitoring.
- Performance monitoring.

**Langfuse**

The AI-specific observability and evaluation platform.

Use Langfuse for:

- Agent execution traces.
- LLM generations.
- Prompt versions.
- Model parameters.
- Token usage.
- Model latency.
- Estimated LLM costs.
- Tool executions.
- AI evaluations.

**Serilog + Microsoft.Extensions.Logging**

Use structured logging throughout the .NET backend.

Integrate Serilog with the Microsoft logging abstractions and OpenTelemetry where appropriate.

### Important architectural distinction

OpenTelemetry is the instrumentation and telemetry standard.

SigNoz is the persistent, unified observability platform.

.NET Aspire Dashboard is primarily for local development diagnostics.

Langfuse provides specialized AI observability.

These products have complementary responsibilities.

Do not introduce Prometheus or Grafana into this architecture.

Do not create application dependencies on a particular observability backend.

Use OpenTelemetry APIs and OTLP so monitoring platforms can be changed without rewriting application instrumentation.

## 7.2 Distributed tracing

Implement end-to-end distributed tracing.

A complete trace must cover:

1. Angular request.
2. C# API request.
3. Database operations.
4. RabbitMQ event publishing.
5. RabbitMQ message consumption.
6. Python AI service request.
7. Orchestrator Agent execution.
8. Shipment Agent execution.
9. LLM calls.
10. Agent tool calls.
11. C# Agent Execution Gateway.
12. Business database transaction.
13. Result verification.
14. Final event publication.

Propagate W3C Trace Context across HTTP and RabbitMQ.

Capture:

- Trace ID.
- Span ID.
- Correlation ID.
- Workflow ID.
- Agent Task ID.
- User or system initiator.
- Service name.
- Operation name.
- Execution status.
- Start timestamp.
- End timestamp.
- Duration.

Connect AI-specific traces with distributed application traces wherever possible.

Never expose hidden chain-of-thought or private model reasoning.

Instead, record concise decision summaries, evidence references, tool activity, structured explanations, and final decisions.

Sensitive information, credentials, and personal data must be redacted.

## 7.3 Agent execution telemetry

Record structured agent lifecycle events:

- `AgentTaskCreated`
- `AgentTaskStarted`
- `AgentThinkingSummary`
- `AgentToolCallStarted`
- `AgentToolCallCompleted`
- `AgentToolCallFailed`
- `AgentDelegationStarted`
- `AgentDelegationCompleted`
- `AgentRecommendationGenerated`
- `AgentApprovalRequested`
- `AgentTaskCompleted`
- `AgentTaskFailed`

`AgentThinkingSummary` must contain a safe, concise execution summary, never private chain-of-thought.

Capture:

- Agent name and version.
- Model provider.
- Model identifier.
- Prompt/configuration version.
- Input and output schemas.
- Tool names and results.
- Token usage.
- Estimated model cost.
- Latency.
- Retries.
- Errors.
- Final outcome.

## 7.4 Application and infrastructure monitoring

Monitor:

- API request rate.
- API latency.
- HTTP errors.
- Database performance.
- RabbitMQ queue depth.
- Message processing failures.
- Dead-letter queue activity.
- AI service availability.
- Agent execution duration.
- LLM provider failures.
- Token consumption.
- Estimated AI cost.
- Concurrent agent tasks.
- Resource consumption.

Use SigNoz dashboards and alerts.

All displayed monitoring data must come from real telemetry.

Never hardcode fake live metrics.

## 7.5 Centralized structured logging

Use structured logs with:

- Timestamp.
- Service.
- Environment.
- Log level.
- Trace ID.
- Correlation ID.
- Workflow ID.
- Agent Task ID.
- Operation.
- Status.

It must be possible to navigate from a failed business operation to its complete distributed trace and associated AI execution.

---

# 8. AI Evaluation Framework

AI evaluation is independent of normal agent execution.

Do not treat successful execution as proof of a correct AI decision.

Create a dedicated evaluation module inside the Python AI service.

## 8.1 Evaluation dimensions

Evaluate:

**Task Completion**

- Did the agent complete the requested task?
- Did it produce the expected result?
- Were required steps completed?

**Tool Selection and Execution**

- Did the agent select the correct tools?
- Were tool arguments valid?
- Were unnecessary tool calls made?
- Were expected results obtained?

**Contextual Accuracy**

- Did the agent use the correct shipment, order, and customer?
- Were conclusions supported by available evidence?
- Did the agent invent information?

**Recommendation Quality**

- Was the probable cause evidence-based?
- Was the recommended action appropriate?
- Were alternative actions considered where relevant?

**Safety and Policy Compliance**

- Were permissions respected?
- Were approvals requested when necessary?
- Did the agent avoid unauthorized execution?
- Were business constraints respected?

**Operational Efficiency**

- Execution duration.
- Number of LLM calls.
- Number of tool calls.
- Token consumption.
- Estimated cost.
- Retries.

## 8.2 Evaluation techniques

Use a combination of:

- Deterministic assertions.
- Schema validation.
- Rule-based evaluation.
- Ground-truth datasets.
- LLM-as-judge where appropriate.
- Human review.

Never rely exclusively on LLM-as-judge.

## 8.3 Evaluation scenarios

Create repeatable scenarios such as:

- Traffic-related shipment delay.
- Carrier has not collected the shipment.
- Shipment delivered without proof of delivery.
- Missing mandatory order information.
- Conflicting shipment statuses.
- Unauthorized action request.
- Insufficient evidence to determine the cause.

## 8.4 Evaluation records

Store:

- Evaluation ID.
- Agent Task ID.
- Scenario ID.
- Timestamp.
- Evaluator type.
- Individual metric scores.
- Overall score.
- Pass/fail status.
- Failure reasons.
- Supporting evidence.

Support comparison between:

- Agent versions.
- Prompt versions.
- Model configurations.
- Evaluation datasets.

Evaluations must be reproducible.

---

# 9. Angular Application Features

Implement the following feature modules:

- `dashboard`
- `shipments`
- `orders`
- `ai-copilot`
- `agent-control`
- `approvals`
- `observability`
- `evaluations`

## 9.1 Dashboard

Display:

- Shipment overview.
- Delayed shipments.
- Open operational tasks.
- Pending approvals.
- Recent AI investigations.
- Agent execution summaries.
- Operational statistics.

## 9.2 AI Copilot

Provide a conversational interface for logistics operations.

Users should be able to ask questions such as:

- Why is shipment SHP-1001 delayed?
- Investigate the shipment and identify the probable cause.
- What is the recommended resolution?
- Which other shipments are affected by this carrier?
- Prepare a customer notification.

The Copilot must use real AI agents and backend tools.

It must not simply generate answers from hardcoded examples.

## 9.3 Agent Control Center

Display:

- Active agents.
- Running tasks.
- Completed tasks.
- Failed tasks.
- Execution timeline.
- Tool history.
- Agent delegation.
- Decision summaries.
- Duration.
- Token usage.
- Estimated cost.

Provide detailed inspection of individual agent tasks.

## 9.4 Approval Center

Display:

- Pending action proposals.
- Investigation results.
- Supporting evidence.
- Recommended actions.
- Risk information.
- Approval and rejection controls.
- Execution status.

The C# backend must enforce all approval and authorization rules.

## 9.5 Distributed Trace Explorer

Provide a hierarchical execution timeline showing:

- Frontend request.
- Backend API.
- RabbitMQ publishing.
- Python AI service.
- Agent execution.
- LLM calls.
- Tool calls.
- Business action.
- Final result.

Show latency and status for each operation.

Support filtering by:

- Trace ID.
- Correlation ID.
- Agent Task ID.
- Service.
- Date.
- Status.

Use actual telemetry from the observability platform.

## 9.6 AI Evaluation Dashboard

Display:

- Agent success rate.
- Task completion rate.
- Recommendation quality.
- Tool accuracy.
- Policy compliance.
- Average execution duration.
- Token consumption.
- Estimated cost per task.
- Evaluation pass/fail distribution.
- Agent and model comparisons.
- Evaluation history.
- Individual failure reasons.

## 9.7 System Monitoring Dashboard

Display:

- API health.
- AI service health.
- Database health.
- RabbitMQ health.
- Queue depth.
- Error rates.
- Response times.
- Agent workload.
- LLM provider availability.
- Resource consumption.

Use real SigNoz telemetry and health information.

Do not simulate production monitoring data.

---

# 10. Repository Structure

Use the following structure as the initial architectural guideline.

```text
PicoChain/
│
├── backend/
│   ├── PicoChain.Api/
│   ├── PicoChain.Application/
│   ├── PicoChain.Domain/
│   ├── PicoChain.Infrastructure/
│   ├── PicoChain.Contracts/
│   └── tests/
│
├── ai-service/
│   ├── app/
│   │   ├── agents/
│   │   ├── tools/
│   │   ├── orchestration/
│   │   ├── models/
│   │   ├── services/
│   │   ├── api/
│   │   ├── evaluation/
│   │   └── observability/
│   │
│   ├── datasets/
│   ├── tests/
│   └── pyproject.toml
│
├── frontend/
│   └── src/app/
│       ├── core/
│       ├── shared/
│       ├── features/
│       │   ├── dashboard/
│       │   ├── shipments/
│       │   ├── orders/
│       │   ├── ai-copilot/
│       │   ├── agent-control/
│       │   ├── approvals/
│       │   ├── observability/
│       │   └── evaluations/
│       └── layout/
│
├── infrastructure/
│   ├── docker/
│   ├── rabbitmq/
│   ├── observability/
│   │   ├── otel/
│   │   ├── signoz/
│   │   ├── aspire/
│   │   └── langfuse/
│   └── configuration/
│
├── docs/
│   ├── 01-Project-Vision.md
│   ├── 02-Requirements.md
│   ├── 03-Architecture.md
│   ├── 04-Agent-Architecture.md
│   ├── 05-Observability-Strategy.md
│   ├── 06-Evaluation-Strategy.md
│   └── 07-Development-Roadmap.md
│
├── docker-compose.yml
└── README.md
```

This structure is a guideline, not an instruction to create empty folders or unnecessary projects.

Adjust it when there is a clear architectural reason.

Keep boundaries explicit and avoid overengineering.

---

# 11. Infrastructure and Local Development

Use Docker Compose for local infrastructure.

Initial infrastructure:

- PostgreSQL.
- RabbitMQ.
- SigNoz and its required dependencies.
- Langfuse and its required dependencies.

Run the .NET Aspire Dashboard for local development diagnostics.

Keep the development setup straightforward and reproducible.

Use externalized environment configuration.

Do not commit credentials, API keys, or secrets.

Do not introduce Kubernetes, Kafka, or other infrastructure that is not justified by the current requirements.

Make sure the infrastructure can be started and stopped independently of the application services.

Document resource requirements and startup dependencies, particularly for SigNoz and Langfuse.

---

# 12. Security and Reliability

Implement security as part of the architecture.

Requirements include:

- Authentication and authorization.
- Role-based or policy-based access control.
- Explicit agent tool permissions.
- Server-side validation of every business action.
- Human approval for sensitive operations.
- Input validation.
- Secure secret management.
- PII redaction.
- Audit logging.
- Idempotent business operations.
- Timeout handling.
- Retry policies.
- Dead-letter handling.
- Protection against unauthorized agent tool execution.

AI agents must not be trusted as authorization authorities.

The C# backend must independently validate every requested action.

Do not expose database access or unrestricted internal APIs to agents.

---

# 13. Development Roadmap

Work incrementally.

Do not jump between phases before the current phase has a tested, working outcome.

## Phase 1: Foundation

- Inspect the repository.
- Establish project structure.
- Create architecture documentation.
- Create the .NET solution.
- Create the Python FastAPI project.
- Create the Angular application.
- Configure PostgreSQL.
- Configure RabbitMQ.
- Configure Docker Compose.
- Establish the initial domain model.
- Introduce OpenTelemetry instrumentation.
- Configure Aspire Dashboard for local diagnostics.
- Prepare SigNoz and Langfuse infrastructure configuration.

**Completion criteria:** All services can start independently, infrastructure is operational, and basic telemetry can be inspected.

## Phase 2: Logistics Domain

- Implement domain entities.
- Implement database migrations.
- Implement application services.
- Implement REST APIs.
- Implement domain validation.
- Create realistic seed data.
- Add unit tests.
- Add integration tests.

**Completion criteria:** The backend can manage customers, orders, shipments, carriers, and shipment events with validated business rules.

## Phase 3: Agent Foundation

- Configure LLM provider abstraction.
- Integrate Pydantic AI.
- Define typed tool contracts.
- Implement Orchestrator Agent.
- Implement Shipment Agent.
- Implement agent execution tracking.
- Integrate AI observability with Langfuse.
- Add OpenTelemetry instrumentation.
- Add token and cost tracking.

**Completion criteria:** Agents can execute real tasks, invoke backend tools, produce structured results, and generate traceable execution records.

## Phase 4: First Complete Workflow

Implement the shipment delay investigation scenario.

- Receive shipment delay event.
- Publish event through RabbitMQ.
- Start agent workflow.
- Investigate shipment.
- Retrieve business context through tools.
- Generate structured recommendation.
- Create action proposal.
- Request human approval.
- Execute approved operation through C#.
- Verify business result.
- Publish result event.
- Record complete audit history.

**Completion criteria:** A complete end-to-end shipment investigation and resolution workflow works without hardcoded AI responses.

## Phase 5: Frontend

Implement:

- Dashboard.
- Shipment management.
- AI Copilot.
- Agent Control Center.
- Approval Center.
- SSE integration.
- Real-time updates.

**Completion criteria:** Users can interact with the complete shipment investigation workflow through Angular.

## Phase 6: Observability and Evaluation

- Complete distributed tracing.
- Integrate SigNoz.
- Configure centralized structured logging.
- Create operational monitoring dashboards.
- Configure relevant alerts.
- Complete Langfuse integration.
- Implement evaluation framework.
- Create evaluation datasets.
- Implement repeatable evaluation runs.
- Build Trace Explorer.
- Build AI Evaluation Dashboard.
- Build System Monitoring Dashboard.
- Add error, latency, token, and cost monitoring.

**Completion criteria:** The entire execution path is observable, AI quality is measurable, and system health is visible using real telemetry.

## Phase 7: Advanced Capabilities

- Implement Order Agent.
- Implement Integration Agent.
- Introduce RAG where appropriate.
- Add agent version comparisons.
- Add prompt version comparisons.
- Expand evaluation datasets.
- Introduce additional autonomous workflows.
- Improve operational reliability.

Do not begin Phase 7 until the previous phases have working, tested outcomes.

---

# 14. Engineering Standards

Follow these engineering principles throughout implementation.

## Code quality

- Use clear naming.
- Follow SOLID principles where appropriate.
- Prefer composition over unnecessary inheritance.
- Use dependency injection.
- Keep responsibilities focused.
- Avoid unnecessary abstractions.
- Avoid speculative architecture.
- Keep business rules in the domain/application layers.
- Keep infrastructure concerns outside the domain.

## Testing

Implement:

- Unit tests.
- Integration tests.
- API contract tests where appropriate.
- Agent tool tests.
- Agent evaluation tests.
- End-to-end workflow tests.

Do not rely only on manual testing.

## Documentation

Keep documentation synchronized with implementation.

Update architecture decisions when they change.

Document API contracts, event contracts, configuration, local development, and operational procedures.

## Observability

Every important business operation and AI task must be traceable.

Propagate trace and correlation identifiers.

Record meaningful telemetry.

Do not create fake production metrics.

## AI engineering

Use structured outputs.

Validate agent results.

Use typed tools.

Enforce backend authorization.

Keep agent execution separate from business execution.

Evaluate AI output independently of successful task execution.

Do not expose private chain-of-thought.

## Maintainability

Prefer small, verifiable increments.

Avoid unnecessary boilerplate.

Do not create empty abstractions, placeholder services, or TODO-only implementations.

Do not change the agreed technology stack silently.

---

# 15. Instructions for Working with Codex

Before writing code:

1. Inspect the existing repository.
2. Read the existing documentation.
3. Understand the current implementation.
4. Identify missing decisions and dependencies.
5. Check whether the proposed work conflicts with existing architecture.
6. Present a concise implementation plan.

During implementation:

1. Work in small, verifiable increments.
2. Explain important technical decisions.
3. Preserve architectural boundaries.
4. Do not change the stack without discussing the reason.
5. Avoid unnecessary boilerplate.
6. Build and test services independently.
7. Verify API and event contracts.
8. Keep documentation synchronized.
9. Introduce telemetry from the beginning.
10. Propagate trace and correlation IDs.
11. Use real metrics and actual AI execution.
12. Create reproducible evaluations.

After implementation:

1. Run relevant tests.
2. Verify builds.
3. Verify service startup.
4. Verify integration points.
5. Report completed work.
6. Report test results.
7. Identify known limitations.
8. Identify the next logical step.

Never claim that something works unless it has been tested or otherwise verified.

Do not move to the next development phase until the current phase has a demonstrably working outcome.

---

# 16. Definition of Success

PicoChain is successful when a user can:

1. Open an independently running Angular application.
2. View logistics operations and shipment information.
3. Request an AI-powered shipment investigation.
4. See the Orchestrator and Shipment Agent execute a real task.
5. Inspect agent activity and tool calls.
6. Follow the complete distributed trace.
7. Review evidence and the AI recommendation.
8. Approve or reject a proposed action.
9. See the C# backend execute and verify an approved business operation.
10. Inspect the resulting audit history.
11. Review AI evaluation results.
12. Compare agent performance and quality.
13. Monitor application health and performance.
14. Inspect execution latency, token usage, and estimated AI costs.

The system must demonstrate real integration between enterprise software architecture, event-driven systems, and Agentic AI.

## Final Product Objective

**PicoChain is not an AI chatbot connected to a logistics database. It is an AI-native logistics platform where specialized agents investigate operational problems, collaborate through controlled tools, recommend actions, and help execute business workflows under explicit application governance.**

The primary objective is to demonstrate production-oriented Agentic AI engineering with realistic enterprise logistics scenarios, robust software architecture, observability, evaluation, and operational reliability.
