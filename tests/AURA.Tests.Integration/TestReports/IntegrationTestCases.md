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

## GetMyNotifications_AfterWalletDeposit_ShouldContainNotification

Feature: Notifications
Description: User notification feed includes wallet deposit success notification after payment verification.
Command / Endpoint: GET /api/notifications after GET /api/wallets/payment-status/{orderCode}
Expected Result: HTTP 200 with success true, non-empty items array, and unreadCount greater than 0.

## MarkAsRead_AndMarkAllAsRead_ShouldUpdateUnreadCount

Feature: Notifications
Description: User can mark one notification as read and then mark all notifications as read.
Command / Endpoint: POST /api/notifications/{id}/mark-read, POST /api/notifications/mark-all-read, GET /api/notifications/unread-count
Expected Result: Both mark endpoints return HTTP 200 and unreadCount becomes 0.

## GetStatus_WithToken_ShouldReturnTwoFactorStatus

Feature: Two-Factor Authentication
Description: Authenticated user can get current two-factor authentication status.
Command / Endpoint: GET /api/two-factor/status
Expected Result: HTTP 200 with success true and status object fields.

## Setup_Enable_LoginVerify_WithTotpAndRecoveryCode_ShouldWorkEndToEnd

Feature: Two-Factor Authentication
Description: User can setup 2FA, enable with valid TOTP, login through verify-2fa, and exercise recovery-code path.
Command / Endpoint: POST /api/two-factor/setup, POST /api/two-factor/enable, POST /api/auth/login, POST /api/auth/login/verify-2fa
Expected Result: Setup and enable succeed, login returns requiresTwoFactor, verify-2fa with TOTP succeeds.

## Enable_WithInvalidCode_ShouldReturnBadRequest

Feature: Two-Factor Authentication Validation
Description: Enabling 2FA with invalid TOTP code is rejected.
Command / Endpoint: POST /api/two-factor/enable
Expected Result: HTTP 400 BadRequest.

## GenerateRecoveryCodes_AndDisable_WithValidPassword_ShouldSucceed

Feature: Two-Factor Authentication Security
Description: User can rotate recovery codes and disable 2FA using valid password.
Command / Endpoint: POST /api/two-factor/recovery-codes and POST /api/two-factor/disable
Expected Result: Both endpoints return HTTP 200 and status reflects disabled 2FA.

## RegisterOphthalmologist_WithMultipartForm_ShouldSucceed

Feature: Authentication (Advanced Registration)
Description: Ophthalmologist registration accepts multipart form-data payload.
Command / Endpoint: POST /api/auth/register/ophthalmologist
Expected Result: HTTP 200 with success true.

## ForgotPassword_AndResendConfirmation_ShouldAlwaysReturnOk

Feature: Authentication (Account Recovery)
Description: Forgot-password and resend-confirmation responses remain generic to avoid account enumeration.
Command / Endpoint: POST /api/auth/forgot-password and POST /api/auth/resend-confirmation
Expected Result: Both endpoints return HTTP 200.

## ConfirmEmail_AndResetPassword_WithInvalidToken_ShouldFail

Feature: Authentication (Token Validation)
Description: Invalid confirmation/reset tokens are rejected.
Command / Endpoint: GET /api/auth/confirm-email and POST /api/auth/reset-password
Expected Result: HTTP 400 or HTTP 404 depending on token/user resolution path.

## GoogleLogin_WithInvalidCredential_ShouldFail

Feature: Authentication (Federated Login)
Description: Invalid Google ID token cannot be used to authenticate.
Command / Endpoint: POST /api/auth/google-login
Expected Result: HTTP 400 or HTTP 401.

## WebsiteFeedback_CreateAndGet_ShouldSucceedAfterAiUsage

Feature: Feedback (Website)
Description: Patient can submit website feedback after at least one processed AI screening and fetch it by ID.
Command / Endpoint: POST /api/feedback/website then GET /api/feedback/website/{feedbackId}
Expected Result: Create returns HTTP 201 and get returns HTTP 200 with expected rating.

## OrganisationFeedback_CreateListGetRating_ShouldSucceed

Feature: Feedback (Organisation)
Description: Patient can submit organisation feedback for completed clinic appointment, then list/detail/rating endpoints return data.
Command / Endpoint: POST /api/feedback/organisations/{organisationId}, GET /items, GET /items/{feedbackId}, GET /rating
Expected Result: Create returns HTTP 201, query endpoints return HTTP 200 with non-empty results.

## VerificationLifecycle_CreateReportMessageEnd_ShouldSucceed

Feature: Consultation Session Lifecycle
Description: Verification session can be created by patient, reported by verified ophthalmologist, messaged by patient, and ended by doctor.
Command / Endpoint: POST /api/consultation-sessions/verification, POST /verification-report, POST /messages, POST /end
Expected Result: Create returns HTTP 201, subsequent lifecycle operations return HTTP 200.

## CancelSession_WithValidPatientRequest_ShouldSucceed

Feature: Consultation Session Lifecycle
Description: Patient can cancel pending verification consultation session.
Command / Endpoint: POST /api/consultation-sessions/{sessionId}/cancel
Expected Result: HTTP 200.

## ScheduleTemplate_CreateGetUpdateListDelete_ShouldSucceed

Feature: Scheduling (Schedule Templates)
Description: Ophthalmologist can perform full CRUD flow on schedule templates.
Command / Endpoint: POST/GET/PUT/DELETE /api/schedule-templates and GET list
Expected Result: Create returns HTTP 201, other operations return HTTP 200.

## AppointmentSlot_CreateManage_Delete_ShouldSucceedForOphthalmologist

Feature: Scheduling (Appointment Slots Management)
Description: Ophthalmologist can create, update, patch cost, block/unblock, and delete slots.
Command / Endpoint: POST/GET/PUT/PATCH/DELETE and block/unblock under /api/appointment-slots
Expected Result: Create returns HTTP 201 and all management operations return HTTP 200.

## AppointmentSlot_ReserveAndConfirm_ShouldCreateConsultationSession

Feature: Scheduling (Reservation Confirmation)
Description: Patient reserves slot and confirms reservation to create consultation session.
Command / Endpoint: POST /api/appointment-slots/{slotId}/reserve then POST /confirm
Expected Result: Both endpoints return HTTP 200 and confirm response contains consultationSessionId.

## ClinicAppointment_CreateCheckInStartComplete_ShouldSucceed

Feature: Scheduling (Clinic Appointments)
Description: Patient creates clinic appointment, org admin executes check-in, start, and complete transitions.
Command / Endpoint: POST /api/clinic-appointments, PUT /check-in, PUT /start, PUT /complete
Expected Result: All endpoints return success (HTTP 200).

## ClinicAppointment_CancelAndNoShow_ShouldSucceed

Feature: Scheduling (Clinic Appointments)
Description: Patient can cancel own appointment and org admin can mark another appointment as no-show.
Command / Endpoint: DELETE /api/clinic-appointments/{id} and PUT /api/clinic-appointments/{id}/no-show
Expected Result: Both flows return HTTP 200.
