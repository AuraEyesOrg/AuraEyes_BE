# Patient Roadmap Implementation Summary

## 1. Domain Separation

### Source of truth: MedicalDiagnosis
- `MedicalDiagnosis` remains the doctor-owned clinical/legal record.
- No roadmap fields were added into `MedicalDiagnosis`.

### Derived layer: PatientRoadmap
- New aggregate: `PatientRoadmap` in `Domain.Entities.Screening`.
- Roadmap stores only patient-facing guidance derived from doctor diagnosis + AI screening context.
- Roadmap does not override diagnosis content and does not duplicate diagnosis fields as first-class columns.

## 2. Implemented Backend Architecture

### 2.1 New Domain Entity
- File: `src/Domain/Entities/Screening/PatientRoadmap.cs`
- Fields:
  - `Id`
  - `PatientId`
  - `MedicalDiagnosisId`
  - `RiskLevel`
  - `Summary`
  - `NextStepsJson`
  - `LifestyleAdviceJson`
  - `WarningSignsJson`
  - `FollowUpNeeded`
  - `FollowUpTimeframe`
  - `RawAiResponse` (optional)
  - `Source` (`AI` / `DOCTOR_OVERRIDE`)
  - `GeneratedAt`

### 2.2 Persistence + DB
- Added `DbSet<PatientRoadmap>` in `ApplicationDbContext`.
- Added EF config: `PatientRoadmapConfiguration`.
- Constraints:
  - Required key roadmap fields
  - FK to `Patients` (`PatientId`) with `Restrict`
  - FK to `MedicalDiagnoses` (`MedicalDiagnosisId`) with `Restrict`
  - Indexes on `PatientId`, `GeneratedAt`
  - Unique index on `MedicalDiagnosisId`

### 2.3 Migration (generated only)
- Migration: `20260329201835_AddPatientRoadmapFromAiDiagnosis`
- Table created: `PatientRoadmaps`
- Database update was **not** applied.

## 3. AI Integration (Backend-only)

### 3.1 New Service Interface
- `IPatientRoadmapGenerationService`
- Method:
  - `GenerateFromDiagnosisAsync(PatientRoadmapGenerationInput input, CancellationToken ct)`

### 3.2 New Infrastructure Service
- File: `src/Infrastructure/Services/PatientRoadmapGenerationService.cs`
- Calls Google AI Studio from backend only.
- Uses strict prompt requiring JSON-only output.
- Uses retry policy: 2 retries (3 total attempts).

### 3.3 Prompt Contract Enforced
Prompt includes required schema:

```json
{
  "risk_level": "LOW | MEDIUM | HIGH | CRITICAL",
  "summary": "short explanation",
  "next_steps": ["step 1", "step 2"],
  "lifestyle_advice": ["advice 1", "advice 2"],
  "follow_up": {
    "needed": true,
    "timeframe": "string"
  },
  "warning_signs": ["symptom 1", "symptom 2"]
}
```

### 3.4 Validation Rules Implemented
- AI payload must be valid JSON.
- Required fields:
  - `risk_level`
  - `summary`
- Arrays must be non-null:
  - `next_steps`
  - `lifestyle_advice`
  - `warning_signs`
- `follow_up` must exist.
- `follow_up.timeframe` required when `follow_up.needed = true`.
- Invalid responses trigger retry; final failure returns controlled error.

### 3.5 Mapping Implemented
AI JSON to `PatientRoadmap`:
- `risk_level` -> `RiskLevel`
- `summary` -> `Summary`
- `next_steps` -> `NextStepsJson`
- `lifestyle_advice` -> `LifestyleAdviceJson`
- `warning_signs` -> `WarningSignsJson`
- `follow_up.needed` -> `FollowUpNeeded`
- `follow_up.timeframe` -> `FollowUpTimeframe`

`RawAiResponse` is stored only after successful parse/validation.

## 4. Roadmap Creation Flow

## Trigger point
- Existing doctor finalization flow in `SubmitVerificationReportCommandHandler`.

## Updated flow
1. Doctor submits verification report.
2. Backend loads linked `AiScreening` + diagnosis data.
3. Backend calls AI generation service.
4. Backend validates and maps AI response.
5. Backend opens transaction.
6. Save `MedicalDiagnosis`.
7. Save `PatientRoadmap` linked by `MedicalDiagnosisId`.
8. Commit transaction.
9. Continue existing notification flow.

If roadmap generation fails, request returns controlled failure and persistence does not proceed.

## 5. Patient Read API

### New Controller
- `src/API/Controllers/PatientRoadmapsController.cs`
- Route: `/api/patient/roadmaps`
- Policy: `PatientOnly`

### Endpoints
- `GET /api/patient/roadmaps`
- `GET /api/patient/roadmaps/{roadmapId}`

### DTO returned to FE
- `PatientRoadmapDto` includes:
  - identifiers
  - risk level
  - summary
  - deserialized arrays (`nextSteps`, `lifestyleAdvice`, `warningSigns`)
  - follow-up object
  - source
  - generated time
- `RawAiResponse` is intentionally excluded from API response.

## 6. Frontend Integration

### Contract update
- Updated `HealthRoadmap` type to patient-facing roadmap model:
  - `medicalDiagnosisId`
  - `riskLevel`
  - `summary`
  - `nextSteps`
  - `lifestyleAdvice`
  - `warningSigns`
  - `followUp`
  - `source`
  - `generatedAt`

### Page update
- `src/features/patient/pages/roadmap.tsx`
- Replaced mock data UI with API-driven rendering using `react-query` + `getRoadmaps()`.
- Added:
  - loading state
  - error state
  - empty state
  - patient-facing sections for next steps, lifestyle advice, warning signs, follow-up
- Added UI note clarifying diagnosis authority remains with doctors.

### API client update
- `patient.api.ts` roadmap endpoints aligned to available backend operations (list/get).

## 7. Configuration

### New settings model
- `GoogleAiStudioSettings`
  - `ApiKey`
  - `BaseUrl`
  - `Model`
  - `TimeoutSeconds`
  - `MaxRetries`
  - `InitialBackoffMs`

### DI wiring
- Added options binding in `Infrastructure.DependencyInjection`.
- Registered `IPatientRoadmapGenerationService` implementation.

### App settings sections added
- `GoogleAiStudio` block in:
  - `src/API/appsettings.json`
  - `src/API/appsettings.Development.json`

## 8. Verification Performed

### Backend
- `dotnet build AuraEyes_BE.sln --configuration Release --no-restore` succeeded.

### Frontend
- `npm run build` in `AuraEyes_FE` succeeded.

### Migration
- `dotnet ef migrations add AddPatientRoadmapFromAiDiagnosis --project src/Infrastructure --startup-project src/API` succeeded.
- No database update executed.

## 9. Important Guardrails Satisfied

- AI call is backend-only.
- AI output is schema-validated before persistence.
- No direct raw AI payload exposure to frontend.
- Roadmap is mapped into internal model first.
- MedicalDiagnosis remains source of truth.

## 10. Future Extension (prepared)

- `PatientRoadmap.Source` already supports `DOCTOR_OVERRIDE`.
- `PatientRoadmap.OverrideByDoctor(...)` exists for controlled override path in future workflows.
