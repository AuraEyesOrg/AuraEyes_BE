# Integration Test Case List

## RegisterPatient_ShouldSucceed

Feature: Authentication
Description: Patient can register a new account through Auth API.
Command / Endpoint: POST /api/auth/register/patient
Expected Result: HTTP 200 with success true and registration response payload.

## Login_ShouldReturnJwt

Feature: Authentication
Description: Seeded patient account can login and receive JWT token.
Command / Endpoint: POST /api/auth/login
Expected Result: HTTP 200 with AccessToken and RefreshToken in response data.

## Login_WithInvalidPassword_ShouldFail

Feature: Authentication
Description: Invalid credentials are rejected.
Command / Endpoint: POST /api/auth/login
Expected Result: HTTP 401 (or 400 based on validation) and success false.

## GetMe_WithoutToken_ShouldReturnUnauthorized

Feature: Authorization
Description: Protected endpoint rejects unauthenticated access.
Command / Endpoint: GET /api/auth/me
Expected Result: HTTP 401 Unauthorized.

## GetMe_WithToken_ShouldReturnCurrentUser

Feature: Authorization
Description: Authenticated patient can fetch profile from the me endpoint.
Command / Endpoint: GET /api/auth/me
Expected Result: HTTP 200 with success true and patient email payload.

## Refresh_ShouldReturnNewTokenPair

Feature: Authentication Token Lifecycle
Description: A valid access-refresh pair can be used to rotate and issue a new token pair.
Command / Endpoint: POST /api/auth/refresh
Expected Result: HTTP 200 with new AccessToken and new RefreshToken.

## Logout_ShouldRevokeRefreshToken

Feature: Authentication Token Revocation
Description: Logging out revokes current refresh token and prevents reuse.
Command / Endpoint: POST /api/auth/logout then POST /api/auth/refresh
Expected Result: Logout returns HTTP 200 and refresh using revoked token returns HTTP 401.

## LogoutAll_ShouldRevokeAllRefreshTokens

Feature: Authentication Session Management
Description: Logout-all revokes all active refresh tokens for the authenticated user.
Command / Endpoint: POST /api/auth/logout-all then POST /api/auth/refresh
Expected Result: Logout-all returns HTTP 200 and refresh using old token returns HTTP 401.

## UploadEyeImage_ShouldPersistMetadata_AndTriggerAiRequest

Feature: AI Screening
Description: Retinal image metadata is persisted and AI screening request is triggered to mocked AI endpoint.
Command / Endpoint: Aggregate persistence + POST /mock/ai/screenings
Expected Result: Retinal image saved with screening link and WireMock receives AI request.

## RequestConsultation_ShouldCreateAggregate_AndAssignOphthalmologist

Feature: Consultation Request
Description: Patient requests verification consultation and system creates consultation aggregate with assigned ophthalmologist.
Command / Endpoint: POST /api/consultation-sessions/verification
Expected Result: HTTP 201 and consultation session persisted with ophthalmologist assignment.

## PaymentFlow_ShouldCreateDeposit_AndMarkAsPaid

Feature: Payment
Description: Deposit is initiated and payment verification marks it as paid and credits wallet balance.
Command / Endpoint: POST /api/wallets/deposit, POST /api/wallets/verify-payment
Expected Result: Deposit created, payment status completed, wallet balance increased.

## CompleteConsultation_ShouldUpdateStatusCorrectly

Feature: Consultation Lifecycle
Description: Ending consultation updates aggregate status and chat lifecycle.
Command / Endpoint: EndSessionCommand via IMediator
Expected Result: Session status becomes Completed and chat status becomes Archived.

## SubmitFeedback_ShouldLinkToConsultation

Feature: Feedback
Description: Patient submits ophthalmologist feedback for completed consultation.
Command / Endpoint: POST /api/feedback/ophthalmologists/{ophthalmologistId}
Expected Result: HTTP 201 and feedback row linked to consultation session.

## SubmitFeedback_WithInvalidRating_ShouldBeRejected

Feature: Feedback Validation
Description: Invalid rating value is rejected by validation.
Command / Endpoint: POST /api/feedback/ophthalmologists/{ophthalmologistId}
Expected Result: HTTP 400 BadRequest.

## SubmitFeedback_BeforeConsultationCompletion_ShouldFail

Feature: Feedback Business Rules
Description: Feedback cannot be submitted before consultation is completed.
Command / Endpoint: POST /api/feedback/ophthalmologists/{ophthalmologistId}
Expected Result: HTTP 400 and message indicating consultation must be completed first.

## GetRecentScreenings_ShouldReturnStoredAiAnalysis

Feature: Screening Query
Description: System admin can query recent screening results from persisted AI screening data.
Command / Endpoint: GET /api/system-admin/dashboard/recent-screenings
Expected Result: HTTP 200 with non-empty screening list.

## GetConsultation_ShouldReturnSessionById

Feature: Consultation Query
Description: Session participant can retrieve consultation details.
Command / Endpoint: GET /api/consultation-sessions/{sessionId}
Expected Result: HTTP 200 with session data matching requested ID.

## GetPatientHistory_ShouldReturnClinicAppointmentHistory

Feature: Patient History Query
Description: Patient retrieves own clinic appointment history.
Command / Endpoint: GET /api/patients/{patientId}/clinic-appointments
Expected Result: HTTP 200 and history array payload.

## GetPaymentStatus_ShouldReturnStatusForExistingOrderCode

Feature: Wallet Payment Query
Description: Public payment-status endpoint returns status for an existing deposit order code.
Command / Endpoint: GET /api/wallets/payment-status/{orderCode}
Expected Result: HTTP 200 with success true and payment status payload for the order code.

## PayOSWebhook_WithFailurePayload_ShouldReturnAcknowledged

Feature: Wallet Webhook
Description: PayOS webhook endpoint acknowledges failure payloads without crashing.
Command / Endpoint: POST /api/wallets/webhook/payos
Expected Result: HTTP 200 with acknowledgement body containing success true.

## GetMetrics_ShouldReturnDashboardMetrics

Feature: System Admin Dashboard
Description: System admin retrieves top-level dashboard metrics.
Command / Endpoint: GET /api/system-admin/dashboard/metrics
Expected Result: HTTP 200 with success true and metrics object.

## GetRiskAnalysis_ShouldReturnPopulationRiskBreakdown

Feature: System Admin Dashboard
Description: System admin retrieves population risk distribution.
Command / Endpoint: GET /api/system-admin/dashboard/risk-analysis
Expected Result: HTTP 200 with success true and non-empty riskCategories array.

## GetSystemHealth_ShouldReturnComponentHealthStatus

Feature: System Admin Dashboard
Description: System admin retrieves overall system health and component statuses.
Command / Endpoint: GET /api/system-admin/dashboard/system-health
Expected Result: HTTP 200 with success true and non-empty components array.
