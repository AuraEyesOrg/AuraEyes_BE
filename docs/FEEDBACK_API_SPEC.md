# Feedback API Specification

Version: 1.0
Date: 2026-03-13
Architecture: Clean Architecture + DDD + CQRS

## 1. Scope

This specification defines feedback APIs for three scenarios:

1. Website feedback (platform-level)
2. Organisation feedback (clinic visit appointment)
3. Ophthalmologist feedback (online consultation session)

All write endpoints require authenticated patient access.

## 2. Domain Model and Tables

### 2.1 WebsiteFeedback

- id: uuid
- patient_id: uuid (FK -> Patients.Id)
- rating: int (1-5)
- category: enum string (BUG | UX | SUGGESTION | OTHER)
- comment: string nullable
- created_at: timestamptz

Business rule:

- Patient can submit only after at least one AI analytic usage (AiScreening with processed result).

### 2.2 OrganisationFeedback

- id: uuid
- patient_id: uuid (FK -> Patients.Id)
- organisation_id: uuid (FK -> Organisations.Id)
- appointment_id: uuid (FK -> Appointments.Id)
- rating: int (1-5)
- comment: string nullable
- created_at: timestamptz

Business rules:

- Appointment.type must be ClinicVisit.
- Appointment.status must be Completed.
- appointment.patient_id must equal current patient.
- appointment.organisation_id must equal request organisation_id.
- unique(patient_id, appointment_id).

### 2.3 OphthalmologistFeedback

- id: uuid
- patient_id: uuid (FK -> Patients.Id)
- ophthalmologist_id: uuid (FK -> Ophthalmologists.Id)
- consultation_session_id: uuid (FK -> ConsultationSessions.Id)
- rating: int (1-5)
- comment: string nullable
- created_at: timestamptz

Business rules:

- consultation_session.status must be Completed.
- consultation_session.patient_id must equal current patient.
- consultation_session.ophthalmologist_id must equal request ophthalmologist_id.
- unique(patient_id, consultation_session_id).

## 3. Authentication and Authorization

- Create endpoints: `Authorize(Policy = PatientOnly)`.
- Read/list/rating endpoints: `Authorize` (authenticated), optionally can be public in a later version.
- Patient identity source: JWT claim `profile_id` mapped to `Patient.Id`.
- If JWT missing/invalid or not patient role for create: return 401/403.

## 4. CQRS Mapping

Commands:

- `CreateWebsiteFeedbackCommand`
- `CreateOrganisationFeedbackCommand`
- `CreateOphthalmologistFeedbackCommand`

Queries:

- `GetWebsiteFeedbackQuery`
- `GetOrganisationFeedbackQuery`
- `GetOphthalmologistFeedbackQuery`
- `ListOrganisationFeedbackQuery`
- `ListOphthalmologistFeedbackQuery`
- `GetOrganisationRatingSummaryQuery`
- `GetOphthalmologistRatingSummaryQuery`

## 5. REST Endpoints

Base route: `/api/feedback`

### 5.1 Create Website Feedback

- Method: `POST`
- Path: `/api/feedback/website`
- Auth: Patient required

Request body:

```json
{
	"rating": 5,
	"category": "UX",
	"comment": "The flow is smooth and easy to understand"
}
```

Validation:

- rating: required, integer, min 1, max 5
- category: required, one of BUG|UX|SUGGESTION|OTHER
- comment: optional, max length 2000
- current patient must have at least one processed AI screening

Success response (201):

```json
{
	"success": true,
	"message": "Website feedback created successfully.",
	"data": {
		"id": "4baf7673-56c4-432f-b6bd-0b2a3b3fc74a",
		"patientId": "dc5ea58d-6575-4dcb-b8af-96fa5624d5ab",
		"rating": 5,
		"category": "UX",
		"comment": "The flow is smooth and easy to understand",
		"createdAt": "2026-03-13T04:00:00Z"
	}
}
```

### 5.2 Create Organisation Feedback

- Method: `POST`
- Path: `/api/feedback/organisations/{organisationId}`
- Auth: Patient required

Request body:

```json
{
	"appointmentId": "0f5e74ed-2baf-467b-b8c4-425f2a3aef1d",
	"rating": 4,
	"comment": "Clinic staff was helpful"
}
```

Validation:

- organisationId: required UUID route param
- appointmentId: required UUID
- rating: required int 1-5
- comment: optional, max 2000
- appointment must exist and belong to authenticated patient
- appointment.type = ClinicVisit
- appointment.status = Completed
- appointment.organisation_id = organisationId
- no existing feedback for (patient_id, appointment_id)

Success response (201): same shape as website feedback, with `organisationId` and `appointmentId`.

### 5.3 Create Ophthalmologist Feedback

- Method: `POST`
- Path: `/api/feedback/ophthalmologists/{ophthalmologistId}`
- Auth: Patient required

Request body:

```json
{
	"consultationSessionId": "97590a9b-34b4-45f4-9863-88de9f271112",
	"rating": 5,
	"comment": "Clear explanation and good follow-up advice"
}
```

Validation:

- ophthalmologistId: required UUID route param
- consultationSessionId: required UUID
- rating: required int 1-5
- comment: optional, max 2000
- consultation session must exist and belong to authenticated patient
- consultation_session.status = Completed
- consultation_session.ophthalmologist_id = ophthalmologistId
- no existing feedback for (patient_id, consultation_session_id)

Success response (201): same shape with `ophthalmologistId` and `consultationSessionId`.

### 5.4 Get Feedback by Id

- Method: `GET`
- Path: `/api/feedback/website/{feedbackId}`
- Path: `/api/feedback/organisations/{organisationId}/items/{feedbackId}`
- Path: `/api/feedback/ophthalmologists/{ophthalmologistId}/items/{feedbackId}`
- Auth: authenticated user

Response (200):

```json
{
	"success": true,
	"message": "Operation completed successfully",
	"data": {
		"id": "...",
		"rating": 4,
		"comment": "...",
		"createdAt": "2026-03-13T04:00:00Z"
	}
}
```

### 5.5 List Feedback for Organisation

- Method: `GET`
- Path: `/api/feedback/organisations/{organisationId}/items?pageNumber=1&pageSize=20`
- Auth: authenticated user

Response (200): paged list

```json
{
	"success": true,
	"message": "Operation completed successfully",
	"data": {
		"items": [
			{
				"id": "...",
				"patientId": "...",
				"appointmentId": "...",
				"rating": 4,
				"comment": "...",
				"createdAt": "2026-03-13T04:00:00Z"
			}
		],
		"totalCount": 56,
		"pageNumber": 1,
		"pageSize": 20,
		"totalPages": 3
	}
}
```

### 5.6 List Feedback for Ophthalmologist

- Method: `GET`
- Path: `/api/feedback/ophthalmologists/{ophthalmologistId}/items?pageNumber=1&pageSize=20`
- Auth: authenticated user

Response shape: same as organisation list with `consultationSessionId`.

### 5.7 Get Average Rating for Organisation

- Method: `GET`
- Path: `/api/feedback/organisations/{organisationId}/rating`
- Auth: authenticated user

Response (200):

```json
{
	"success": true,
	"message": "Operation completed successfully",
	"data": {
		"entityId": "42e91a57-41f4-4e53-93da-1b5434515df4",
		"ratingAverage": 4.42,
		"ratingCount": 120,
		"distribution": {
			"1": 2,
			"2": 4,
			"3": 18,
			"4": 40,
			"5": 56
		}
	}
}
```

### 5.8 Get Average Rating for Ophthalmologist

- Method: `GET`
- Path: `/api/feedback/ophthalmologists/{ophthalmologistId}/rating`
- Auth: authenticated user

Response shape: same as organisation rating summary.

## 6. Error Handling

Standard wrapper: `ApiResponseFactory` with `Result` mapping.

### 6.1 Duplicate Feedback

- HTTP: `409 Conflict`
- Cases:
	- Existing organisation feedback for (patient_id, appointment_id)
	- Existing ophthalmologist feedback for (patient_id, consultation_session_id)
- Message example: `Feedback already exists for this appointment.`

### 6.2 Appointment/Session Not Completed

- HTTP: `400 Bad Request`
- Cases:
	- Appointment.status != Completed
	- ConsultationSession.status != Completed
- Message example: `Feedback can only be submitted after completion.`

### 6.3 Invalid Rating

- HTTP: `400 Bad Request`
- Case: rating < 1 or rating > 5
- Message example: `Rating must be between 1 and 5.`

### 6.4 Unauthorized User

- HTTP: `401 Unauthorized`
- Cases:
	- Missing/invalid token
	- Missing profile_id claim

### 6.5 Forbidden User

- HTTP: `403 Forbidden`
- Cases:
	- Authenticated but not patient role for create endpoints

## 7. Efficient Rating Aggregation Strategy

Recommended approach: maintain denormalized counters on `Ophthalmologists` and `Organisations`.

Fields to add:

- `rating_average` numeric(4,2) default 0
- `rating_count` int default 0
- Optional: `rating_1_count` ... `rating_5_count` int default 0

Write path:

1. Insert feedback in transaction.
2. Update aggregate counters atomically:
	 - `rating_count = rating_count + 1`
	 - `rating_average = ((rating_average * old_count) + new_rating) / (old_count + 1)`
3. Commit.

Read path:

- Rating endpoints read from cached fields (O(1)).
- List endpoints still read from feedback tables with indexes.

Consistency and repair:

- Nightly reconciliation job recomputes counters from source feedback tables.
- On mismatch, overwrite cached values and emit monitoring alert.

Indexing guidance:

- `OrganisationFeedback(organisation_id, created_at desc)`
- `OphthalmologistFeedback(ophthalmologist_id, created_at desc)`
- Keep unique indexes for idempotency constraints.

## 8. Migration Status

- EF migration created: `SplitFeedbackTables`
- EF migration created: `AddRatingAggregatesToOrganisationAndOphthalmologist`
- Action completed: scaffolded migration only
- Action not performed: `dotnet ef database update`

## 9. Implementation Status

- Feedback CQRS handlers implemented (create/get/list/rating summary)
- Feedback controller implemented at `/api/feedback`
- Rating aggregate fields implemented on Organisation and Ophthalmologist:
	- `RatingAverage`
	- `RatingCount`

## 10. UI/UX and FE-BE Handoff

- Detailed handoff document: `docs/FEEDBACK_UI_FE_BE_FLOW.md`
- Includes:
	- UI flows for Website/Organisation/Ophthalmologist feedback
	- Page layout and component structure
	- FE-BE endpoint mapping and interaction states
	- Manual test scenarios and release checklist
