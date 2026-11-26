from pydantic import Field
from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    model_config = SettingsConfigDict(env_prefix="PICOCHAIN_", env_file=".env", extra="ignore")

    backend_url: str = "http://localhost:5000"
    llm_provider: str = "none"
    llm_model: str = "openai:gpt-4o-mini"
    openai_api_key: str | None = Field(default=None, validation_alias="OPENAI_API_KEY")
    rabbitmq_url: str = "amqp://picochain:development-only@localhost:5672/"
    rabbitmq_exchange: str = "picochain.events"
    rabbitmq_queue: str = "picochain.ai.shipment-delays"
    otlp_endpoint: str = "http://localhost:4317"
    langfuse_host: str = "http://localhost:3000"
    langfuse_public_key: str | None = None
    langfuse_secret_key: str | None = None
    prompt_version: str = "shipment-investigation-v1"
    agent_version: str = "shipment-agent-v1"
    model_cost_per_1k_input_tokens: float = 0.0
    model_cost_per_1k_output_tokens: float = 0.0


settings = Settings()
