from fastapi import FastAPI, HTTPException
import httpx

from app.models.investigation import InvestigationRequest
from app.models.evaluation import EvaluationRequest
from app.services.evaluator import evaluate_investigation
from app.services.investigator import ShipmentInvestigator
from app.services.settings import settings

app = FastAPI(title="PicoChain AI Service", version="0.1.0")
investigator = ShipmentInvestigator(settings)


@app.get("/health")
async def health() -> dict[str, str]:
    return {"status": "ok", "service": "picochain-ai-service"}


@app.post("/api/v1/investigations", status_code=202)
async def investigate(request: InvestigationRequest) -> dict[str, object]:
    try:
        result = await investigator.investigate(request.shipment_id)
    except httpx.HTTPError as error:
        raise HTTPException(status_code=502, detail="Backend context tool unavailable") from error
    except RuntimeError as error:
        raise HTTPException(status_code=503, detail=str(error)) from error
    except NotImplementedError as error:
        raise HTTPException(status_code=501, detail=str(error)) from error
    return result.model_dump(mode="json")


@app.post("/api/v1/evaluations", status_code=200)
async def evaluate(request: EvaluationRequest) -> dict[str, object]:
    return evaluate_investigation(request).model_dump(mode="json")
