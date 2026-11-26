from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    model_config = SettingsConfigDict(env_prefix="PICOCHAIN_", env_file=".env", extra="ignore")

    backend_url: str = "http://localhost:5000"
    llm_provider: str = "none"
    llm_model: str = "openai:gpt-4o-mini"
    openai_api_key: str | None = None


settings = Settings()
