# Unit test checklist — theo Function (UTC thực)

Use this as quick marking guide in each Function tab:
- Mark O in Condition row shown for each UTCID.
- Mark O in Return row shown for each UTCID.
- Type/P-F/Date fill: use listed values (Date = **04/04/2026** for Function31+ and extended rows; earlier functions may show 20/03/2026).

## Quy tắc đếm (tester)

- **Mỗi Function** = một nhóm nghiệp vụ / lớp được test; **mỗi dòng UTC** trong bảng = một **kịch bản checklist có mô tả** (Condition / Return).
- **Tổng UTC thực** = chỉ đếm các dòng có kịch bản (cột Condition không rỗng và **không** phải ô đệm). **Không** dùng hàng “N/A toàn dòng” để tính coverage; các hàng đó đã **bỏ khỏi file**.
- **P** — Có unit test tương ứng, chạy pass (một dòng UTC có thể map **nhiều** `[Fact]` trong cùng một file / cùng luồng).
- **NT** — Kịch bản checklist có ý nghĩa nhưng **chưa** có unit test (SMTP thật, Google login, v.v.).
- **Function2–50** — API, Application, Domain cốt lõi, Infrastructure dịch vụ. **Function51–59** — bổ sung **từng file** test Domain entity / validator / handler còn lại (mỗi UTC = toàn bộ suite trong file `*Tests.cs` ghi trong Condition).
- Trong thư mục `tests/` hiện có **105** file `*Tests.cs` (class test), map vào **59** Function checklist (F2–F60) — xem **Phụ lục A** (đủ từng file). File hỗ trợ `Application.UnitTests/TestSupport/AsyncQueryable.cs` **không** phải `*Tests.cs`, không tính vào 105.

## Đối chiếu solution ↔ Function (snapshot)

| Chỉ số | Giá trị | Ghi chú |
|--------|---------|---------|
| **File class test trong `tests/`** | **105** | `*Tests.cs`: API **3**, Application **44**, Domain **44**, Infrastructure **14** |
| **Số Function trong checklist** | **59** | Function2 → Function60 (không có Function0/1) |
| **Tổng lần chạy test (xUnit)** | **1053** | `dotnet test` — Domain **507** + Application **351** + Infrastructure **144** + API **51** |
| **105 file vs 59 Function** | Nhiều–một | Nhiều file test cùng một Function (ví dụ F46–50 chung `ScheduleTemplateCommandAndQueryHandlerTests`) |

### Phụ lục A — Đủ 105 file `tests/**/*Tests.cs` → Function

#### `tests/API.UnitTests` (3)

| File | Function |
|------|----------|
| `Controllers/BaseApiControllerTests.cs` | **F2** |
| `Controllers/PatientProfileControllerTests.cs` | **F3** |
| `Controllers/WalletsControllerTests.cs` | **F4** |

#### `tests/Application.UnitTests` (44)

| File | Function |
|------|----------|
| `Behaviors/LoggingBehaviorTests.cs` | **F8** |
| `Behaviors/PerformanceBehaviorTests.cs` | **F9** |
| `Behaviors/ValidationBehaviorTests.cs` | **F12** |
| `Models/ResultTests.cs` | **F10** |
| `Handlers/CreateVerificationSessionCommandHandlerTests.cs` | **F5** |
| `Handlers/GetAuditLogsQueryHandlerTests.cs` | **F7** |
| `Handlers/CreateAiScreeningSessionCommandHandlerTests.cs` | **F43** |
| `Handlers/SaveAiScreeningResultsCommandHandlerTests.cs` | **F44** |
| `Handlers/CompleteAiScreeningCommandHandlerTests.cs` | **F45** |
| `Handlers/ScheduleTemplateCommandAndQueryHandlerTests.cs` | **F46–F50** (cùng file: Create/Update/Delete/Get template & list) |
| `Handlers/CreateDepositCommandHandlerTests.cs` | **F59** |
| `Handlers/CreateWithdrawalRequestCommandHandlerTests.cs` | **F59** |
| `Handlers/VerifyPaymentCommandHandlerTests.cs` | **F59** |
| `Handlers/SubmitVerificationReportCommandHandlerTests.cs` | **F59** |
| `Validators/CreateVerificationSessionCommandValidatorTests.cs` | **F6** |
| `Validators/UpdatePatientProfileCommandValidatorTests.cs` | **F11** |
| `Validators/AgreeScreeningConsentCommandValidatorTests.cs` | **F55** |
| `Validators/BuyAiQuotaCommandValidatorTests.cs` | **F55** |
| `Validators/CancelSessionCommandValidatorTests.cs` | **F55** |
| `Validators/ChangePasswordCommandValidatorTests.cs` | **F55** |
| `Validators/CompleteAiScreeningCommandValidatorTests.cs` | **F55** |
| `Validators/ConfirmReservationCommandValidatorTests.cs` | **F55** |
| `Validators/CreateAiScreeningSessionCommandValidatorTests.cs` | **F55** |
| `Validators/CreateAppointmentSlotCommandValidatorTests.cs` | **F55** |
| `Validators/CreateContractCommandValidatorTests.cs` | **F56** |
| `Validators/CreateContractTemplateCommandValidatorTests.cs` | **F56** |
| `Validators/CreateDepositCommandValidatorTests.cs` | **F56** |
| `Validators/CreateOphthalmologistCommandValidatorTests.cs` | **F56** |
| `Validators/CreateOphthalmologistFeedbackCommandValidatorTests.cs` | **F56** |
| `Validators/CreateOrganisationFeedbackCommandValidatorTests.cs` | **F56** |
| `Validators/CreatePermissionCommandValidatorTests.cs` | **F56** |
| `Validators/CreateScheduleTemplateCommandValidatorTests.cs` | **F56** |
| `Validators/CreateVideoCallSessionCommandValidatorTests.cs` | **F57** |
| `Validators/CreateWebsiteFeedbackCommandValidatorTests.cs` | **F57** |
| `Validators/CreateWithdrawalRequestCommandValidatorTests.cs` | **F57** |
| `Validators/EndSessionCommandValidatorTests.cs` | **F57** |
| `Validators/GenerateSlotsCommandValidatorTests.cs` | **F57** |
| `Validators/ReserveSlotCommandValidatorTests.cs` | **F57** |
| `Validators/SendMessageCommandValidatorTests.cs` | **F57** |
| `Validators/SubmitVerificationReportCommandValidatorTests.cs` | **F57** |
| `Validators/UpdateOphthalmologistCommandValidatorTests.cs` | **F58** |
| `Validators/UpdatePermissionCommandValidatorTests.cs` | **F58** |
| `Validators/UpdateScheduleTemplateCommandValidatorTests.cs` | **F58** |
| `Validators/VerifyPaymentCommandValidatorTests.cs` | **F58** |

#### `tests/Domain.UnitTests` (44)

| File | Function |
|------|----------|
| `Common/BaseEntityTests.cs` | **F14** |
| `Common/ConcurrencyExceptionTests.cs` | **F15** |
| `Common/DomainEventTests.cs` | **F18** |
| `ValueObjects/AddressTests.cs` | **F13** |
| `ValueObjects/MoneyTests.cs` | **F19** |
| `Entities/ConsultationSessionTests.cs` | **F16, F17** |
| `Entities/OphthalmologistTests.cs` | **F20** |
| `Entities/OrderTests.cs` | **F21** |
| `Entities/PatientTests.cs` | **F22** |
| `Entities/WalletTests.cs` | **F23** |
| `Entities/ProfessionalPostTests.cs` | **F34** |
| `Entities/PostCommentTests.cs` | **F35** |
| `Entities/PostReactionTests.cs` | **F36** |
| `Entities/SavedPostTests.cs` | **F37** |
| `Entities/PostAttachmentTests.cs` | **F38** |
| `Entities/AiScreeningTests.cs` | **F51** |
| `Entities/AppointmentSlotTests.cs` | **F51** |
| `Entities/AppointmentTests.cs` | **F51** |
| `Entities/AuditLogTests.cs` | **F51** |
| `Entities/CertificateTests.cs` | **F52** |
| `Entities/ChatMessageTests.cs` | **F52** |
| `Entities/ConsentTests.cs` | **F52** |
| `Entities/ContractTemplateTests.cs` | **F52** |
| `Entities/ContractTests.cs` | **F52** |
| `Entities/ConversationTests.cs` | **F52** |
| `Entities/DepositRequestTests.cs` | **F52** |
| `Entities/MedicalDiagnosisTests.cs` | **F53** |
| `Entities/NotificationTests.cs` | **F53** |
| `Entities/OphthalmologistFeedbackTests.cs` | **F53** |
| `Entities/OrganisationFeedbackTests.cs` | **F53** |
| `Entities/OrganisationOnboardingRequestTests.cs` | **F53** |
| `Entities/OrganisationTests.cs` | **F53** |
| `Entities/PatientRoadmapTests.cs` | **F53** |
| `Entities/PaymentTests.cs` | **F53** |
| `Entities/PermissionTests.cs` | **F53** |
| `Entities/RetinalImageTests.cs` | **F53** |
| `Entities/RolePermissionTests.cs` | **F53** |
| `Entities/ScheduleTemplateTests.cs` | **F53** |
| `Entities/ScreeningResultTests.cs` | **F53** |
| `Entities/SystemSettingTests.cs` | **F54** |
| `Entities/UserPermissionTests.cs` | **F54** |
| `Entities/WalletTransactionTests.cs` | **F54** |
| `Entities/WebsiteFeedbackTests.cs` | **F54** |
| `Entities/WithdrawalRequestTests.cs` | **F54** |

#### `tests/Infrastructure.UnitTests` (14)

| File | Function |
|------|----------|
| `Identity/ApplicationUserTests.cs` | **F33** |
| `Identity/AuthServiceRegistrationTests.cs` | **F39** |
| `Identity/JwtSettingsTests.cs` | **F25** (binding JwtSettings; song song `AllSettingsTests`) |
| `Identity/RefreshTokenServiceTests.cs` | **F31** |
| `Identity/RefreshTokenTests.cs` | **F32** |
| `Identity/TokenServiceTests.cs` | **F29, F30** |
| `Services/DateTimeServiceTests.cs` | **F24** |
| `Services/DashboardMetricsServiceTests.cs` | **F42** |
| `Services/EmailServiceValidationTests.cs` | **F41** |
| `Services/EmailTemplatesTests.cs` | **F41** |
| `Services/NotificationServiceTests.cs` | **F40** |
| `Services/SystemSettingServiceTests.cs` | **F60** |
| `Settings/AllSettingsTests.cs` | **F25–F28** (PayOS, SMTP, Supabase, Jwt gộp section) |
| `Settings/SettingsTests.cs` | **F25–F28** |

*Ghi chú Infrastructure:* `SystemSettingServiceTests` bổ sung cho `ISystemSettingService`; F42 tập trung `DashboardMetricsService`. Nếu cần tách Function riêng cho file này, có thể thêm **F60** sau.

#### Tóm tắt tên Function (F2–F59)

**F2** BaseApiController · **F3** PatientProfile · **F4** Wallets · **F5** CreateVerificationSession handler · **F6** CreateVerificationSession validator · **F7** GetAuditLogs · **F8–F9, F12** behaviors · **F10** Result · **F11** UpdatePatientProfile validator · **F13–F23** Domain value/entity cốt lõi · **F24** DateTimeService · **F25–F28** settings (Jwt, PayOS, SMTP, Supabase + All/Settings tests) · **F29–F30** TokenService · **F31–F32** RefreshToken service/entity · **F33** ApplicationUser · **F34–F38** social posts · **F39** AuthService registration · **F40–F42** Notification, Email, Dashboard · **F43–F50** AI screening + ScheduleTemplate · **F51–F54** Domain entity batch · **F55–F58** validator batch · **F59** handler deposit/withdrawal/verify/submit report · **F60** `SystemSettingServiceTests`.

## Function2 - BaseApiController
- Header: Passed=20, Failed=0, Untested=0, N=14, A=6, B=0, Total=20 — khớp `BaseApiControllerTests` (51 test API = 20 BaseApi + 12 PatientProfile + 19 Wallets)
- Pre-Condition: Controller instance initialized with HttpContext; request context available; input Result/Request object prepared.
- Date for all executed UTCID: 20/03/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | HandleResult Generic Success | ReturnOk200 | N | P | 20/03/2026 |
| UTCID02 | HandleResult Generic Success WithData | ResponseContainsData | N | P | 20/03/2026 |
| UTCID03 | HandleResult Generic Unauthorized | Return401 | A | P | 20/03/2026 |
| UTCID04 | HandleResult Generic Forbidden | Return403 | A | P | 20/03/2026 |
| UTCID05 | HandleResult Generic NotFound | Return404 | A | P | 20/03/2026 |
| UTCID06 | HandleResult Generic Conflict | Return409 | A | P | 20/03/2026 |
| UTCID07 | HandleResult Generic Failure | Return400 | A | P | 20/03/2026 |
| UTCID08 | HandleResult Generic Failure MultipleErrors | Return400 | A | P | 20/03/2026 |
| UTCID09 | HandleResult NonGeneric Success | ReturnOk200 | N | P | 20/03/2026 |
| UTCID10 | HandleResult NonGeneric Unauthorized | Return401 | A | P | 20/03/2026 |
| UTCID11 | HandleResult NonGeneric Forbidden | Return403 | A | P | 20/03/2026 |
| UTCID12 | HandleResult NonGeneric NotFound | Return404 | A | P | 20/03/2026 |
| UTCID13 | HandleResult NonGeneric Conflict | Return409 | A | P | 20/03/2026 |
| UTCID14 | HandleResult NonGeneric Failure | Return400 | A | P | 20/03/2026 |
| UTCID15 | OkResponse WithData | ReturnOk200 | N | P | 20/03/2026 |
| UTCID16 | OkResponse WithoutData | ReturnOk200 | N | P | 20/03/2026 |
| UTCID17 | ErrorResponse Default | Return400 | N | P | 20/03/2026 |
| UTCID18 | ErrorResponse CustomStatusCode | ReturnSpecifiedCode | N | P | 20/03/2026 |
| UTCID19 | InternalError | Return500 | N | P | 20/03/2026 |
| UTCID20 | InternalError CustomMessage | Return500WithMessage | N | P | 20/03/2026 |

## Function3 - PatientProfileController
- Header: Passed=12, Failed=0, Untested=0, N=6, A=4, B=0, Total=12
- Pre-Condition: Controller instance initialized with HttpContext; request context available; input Result/Request object prepared.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | GetProfile WhenUserNotAuthenticated | Return401 | N | P | 20/03/2026 |
| UTCID02 | GetProfile WhenAuthenticated | SendQuery | N | P | 20/03/2026 |
| UTCID03 | GetProfile WhenNotFound | Return404 | A | P | 20/03/2026 |
| UTCID04 | UpdateProfile WhenUserNotAuthenticated | Return401 | N | P | 20/03/2026 |
| UTCID05 | UploadAvatar WhenUserNotAuthenticated | Return401 | N | P | 20/03/2026 |
| UTCID06 | UploadAvatar WhenEmptyFile | Return400 | A | P | 20/03/2026 |
| UTCID07 | UploadAvatar WhenInvalidFileType | Return400 | A | P | 20/03/2026 |
| UTCID08 | UploadAvatar WhenFileTooLarge | Return400 | A | P | 20/03/2026 |
| UTCID09 | UploadAvatar WhenValidFile | SendCommand | N | P | 20/03/2026 |
| UTCID10 | UpdateProfile WhenAuthenticated | SendCommand | N | P | 20/03/2026 |
| UTCID11 | ChangePassword WhenUserNotAuthenticated | Return401 | N | P | 20/03/2026 |
| UTCID12 | ChangePassword WhenAuthenticated | SendCommand | N | P | 20/03/2026 |

## Function4 - WalletsController
- Header: Passed=19, Failed=0, Untested=0, N=12, A=5, B=2, Total=19
- Pre-Condition: Controller instance initialized with HttpContext; request context available; input Result/Request object prepared.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | GetWallet WhenUserNotAuthenticated | Return401 | N | P | 20/03/2026 |
| UTCID02 | GetWallet WhenAuthenticated | SendQuery | N | P | 20/03/2026 |
| UTCID03 | GetWallet WhenNotFound | Return404 | A | P | 20/03/2026 |
| UTCID04 | GetTransactions WhenUserNotAuthenticated | Return401 | N | P | 20/03/2026 |
| UTCID05 | GetTransactions WhenAuthenticated | SendQueryWithPagination | B | P | 20/03/2026 |
| UTCID06 | GetDepositHistory WhenUserNotAuthenticated | Return401 | N | P | 20/03/2026 |
| UTCID07 | GetDepositHistory WhenAuthenticated | SendQueryWithPagination | N | P | 20/03/2026 |
| UTCID08 | GetDepositRequest WhenUserNotAuthenticated | Return401 | N | P | 20/03/2026 |
| UTCID09 | GetDepositRequest WhenAuthenticatedAndOwner | ReturnOk | N | P | 20/03/2026 |
| UTCID10 | GetDepositRequest WhenAuthenticatedButNotOwner | Return403 | A | P | 20/03/2026 |
| UTCID11 | CreateDeposit WhenUserNotAuthenticated | Return401 | N | P | 20/03/2026 |
| UTCID12 | CreateDeposit WhenAuthenticated | SendCommandWithPayOSDefault | N | P | 20/03/2026 |
| UTCID13 | VerifyPayment WhenUserNotAuthenticated | Return401 | N | P | 20/03/2026 |
| UTCID14 | VerifyPayment WhenAuthenticated | SendCommand | N | P | 20/03/2026 |
| UTCID15 | PayOSWebhook WhenNotSuccessful | ReturnOk | N | P | 20/03/2026 |
| UTCID16 | PayOSWebhook WhenDataIsNull | ReturnOk | A | P | 20/03/2026 |
| UTCID17 | PayOSWebhook WhenSuccessfulAndPaymentVerified | ReturnOk | N | P | 20/03/2026 |
| UTCID18 | GetWithdrawalRequests WhenAuthenticated | SendQuery | N | P | 20/03/2026 |
| UTCID19 | CreateWithdrawalRequest WhenAuthenticated | SendCommand | N | P | 20/03/2026 |

## Function5 - CreateVerificationSessionCommandHandler
- Header: Passed=2, Failed=0, Untested=0, N=1, A=1, B=0, Total=2
- Pre-Condition: Handler initialized with mocked dependencies; command/query object prepared; repository/service mocks set up.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Handle ValidCommand | CreateSessionAndReturnId | N | P | 20/03/2026 |
| UTCID02 | Handle WithoutDoctor | CreateSessionWithNullDoctor | A | P | 20/03/2026 |

## Function6 - CreateVerificationSessionCommandValidator
- Header: Passed=6, Failed=0, Untested=0, N=2, A=3, B=1, Total=6
- Pre-Condition: Validator instance initialized; command/query object prepared with target field values.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Validate ValidCommand | Pass | N | P | 20/03/2026 |
| UTCID02 | Validate EmptyPatientId | Fail | A | P | 20/03/2026 |
| UTCID03 | Validate EmptyAiScreeningId | Fail | A | P | 20/03/2026 |
| UTCID04 | Validate NegativePrice | Fail | A | P | 20/03/2026 |
| UTCID05 | Validate ZeroPrice | Pass | B | P | 20/03/2026 |
| UTCID06 | Validate OptionalOphthalmologistId | Pass | N | P | 20/03/2026 |

## Function7 - GetAuditLogsQueryHandler
- Header: Passed=5, Failed=0, Untested=0, N=3, A=1, B=1, Total=5
- Pre-Condition: Handler initialized with mocked dependencies; command/query object prepared; repository/service mocks set up.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Handle NoFilters ReturnsAllLogs | Expected per test method | N | P | 20/03/2026 |
| UTCID02 | Handle FilterByEntityName ReturnsFilteredLogs | Expected per test method | N | P | 20/03/2026 |
| UTCID03 | Handle FilterByAction ReturnsOnlyMatchingActions | Expected per test method | N | P | 20/03/2026 |
| UTCID04 | Handle EmptyResult ReturnsEmptyPagedResult | Expected per test method | A | P | 20/03/2026 |
| UTCID05 | Handle PaginationParameters PassedCorrectly | Expected per test method | B | P | 20/03/2026 |

## Function8 - LoggingBehavior
- Header: Passed=2, Failed=0, Untested=0, N=2, A=0, B=0, Total=2
- Pre-Condition: Pipeline behavior initialized; mocked next delegate and logger (if any) configured.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Handle | LogBeforeAndAfterExecution | N | P | 20/03/2026 |
| UTCID02 | Handle | ReturnResponseFromNext | N | P | 20/03/2026 |

## Function9 - PerformanceBehavior
- Header: Passed=2, Failed=0, Untested=0, N=2, A=0, B=0, Total=2
- Pre-Condition: Pipeline behavior initialized; mocked next delegate and logger (if any) configured.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Handle FastRequest | NotLogWarning | N | P | 20/03/2026 |
| UTCID02 | Handle SlowRequest | LogWarning | N | P | 20/03/2026 |

## Function10 - Result
- Header: Passed=13, Failed=0, Untested=0, N=2, A=11, B=0, Total=13
- Pre-Condition: Application.Common.Models.Result input values prepared for each status scenario.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Success | CreateSuccessResult | N | P | 20/03/2026 |
| UTCID02 | Failure WithMessage | CreateFailureResult | A | P | 20/03/2026 |
| UTCID03 | Failure WithMultipleErrors | JoinErrors | A | P | 20/03/2026 |
| UTCID04 | Unauthorized | SetFlag | A | P | 20/03/2026 |
| UTCID05 | Forbidden | SetFlag | A | P | 20/03/2026 |
| UTCID06 | NotFound | SetFlag | A | P | 20/03/2026 |
| UTCID07 | Conflict | SetFlag | A | P | 20/03/2026 |
| UTCID08 | NonGeneric Success | BeSuccessful | N | P | 20/03/2026 |
| UTCID09 | NonGeneric Failure | NotBeSuccessful | A | P | 20/03/2026 |
| UTCID10 | NonGeneric Unauthorized | SetFlag | A | P | 20/03/2026 |
| UTCID11 | NonGeneric Forbidden | SetFlag | A | P | 20/03/2026 |
| UTCID12 | NonGeneric NotFound | SetFlag | A | P | 20/03/2026 |
| UTCID13 | NonGeneric Conflict | SetFlag | A | P | 20/03/2026 |

## Function11 - UpdatePatientProfileCommandValidator
- Header: Passed=13, Failed=0, Untested=0, N=5, A=8, B=0, Total=13
- Pre-Condition: Validator instance initialized; command/query object prepared with target field values.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Validate ValidCommand | Pass | N | P | 20/03/2026 |
| UTCID02 | Validate EmptyUserId | Fail | A | P | 20/03/2026 |
| UTCID03 | Validate EmptyFullName | Fail | A | P | 20/03/2026 |
| UTCID04 | Validate FullNameTooLong | Fail | A | P | 20/03/2026 |
| UTCID05 | Validate PhoneTooLong | Fail | A | P | 20/03/2026 |
| UTCID06 | Validate InvalidDateOfBirth | Fail | A | P | 20/03/2026 |
| UTCID07 | Validate InvalidGender | Fail | A | P | 20/03/2026 |
| UTCID08 | Validate ValidGender | Pass | N | P | 20/03/2026 |
| UTCID09 | Validate AddressTooLong | Fail | A | P | 20/03/2026 |
| UTCID10 | Validate NullOptionalFields | Pass | A | P | 20/03/2026 |

## Function12 - ValidationBehavior
- Header: Passed=3, Failed=0, Untested=0, N=2, A=1, B=0, Total=3
- Pre-Condition: Pipeline behavior initialized; mocked next delegate and logger (if any) configured.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Handle NoValidators | CallNext | N | P | 20/03/2026 |
| UTCID02 | Handle ValidRequest | CallNext | N | P | 20/03/2026 |
| UTCID03 | Handle InvalidRequest | ThrowValidationException | A | P | 20/03/2026 |

## Function13 - Address
- Header: Passed=11, Failed=0, Untested=0, N=5, A=6, B=0, Total=11
- Pre-Condition: Domain object constructor inputs/state prepared; no external infrastructure dependency required.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Constructor ValidInput | CreateAddress | N | P | 20/03/2026 |
| UTCID02 | Constructor EmptyStreet | Throw | A | P | 20/03/2026 |
| UTCID03 | Constructor EmptyCity | Throw | A | P | 20/03/2026 |
| UTCID04 | Equals SameValues | BeEqual | N | P | 20/03/2026 |
| UTCID05 | Equals DifferentValues | NotBeEqual | N | P | 20/03/2026 |
| UTCID06 | ToString | FormatCorrectly | N | P | 20/03/2026 |
| UTCID07 | GetHashCode SameValues | BeSame | N | P | 20/03/2026 |

## Function14 - BaseEntity
- Header: Passed=5, Failed=0, Untested=0, N=5, A=0, B=0, Total=5
- Pre-Condition: Domain object constructor inputs/state prepared; no external infrastructure dependency required.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Constructor Default | GenerateId | N | P | 20/03/2026 |
| UTCID02 | Constructor WithId | UseProvidedId | N | P | 20/03/2026 |
| UTCID03 | AddDomainEvent | AddToCollection | N | P | 20/03/2026 |
| UTCID04 | RemoveDomainEvent | RemoveFromCollection | N | P | 20/03/2026 |
| UTCID05 | ClearDomainEvents | RemoveAllEvents | N | P | 20/03/2026 |

## Function15 - ConcurrencyException
- Header: Passed=3, Failed=0, Untested=0, N=3, A=0, B=0, Total=3
- Pre-Condition: Domain object constructor inputs/state prepared; no external infrastructure dependency required.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | DefaultConstructor | HaveDefaultMessage | N | P | 20/03/2026 |
| UTCID02 | Constructor WithMessage | SetMessage | N | P | 20/03/2026 |
| UTCID03 | Constructor WithMessageAndInnerException | SetBoth | N | P | 20/03/2026 |

## Function16 - ConsultationSession (Part 1/2)
- Header: Passed=15, Failed=0, Untested=0, N=9, A=6, B=0, Total=15
- Pre-Condition: Domain object constructor inputs/state prepared; no external infrastructure dependency required.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | CreateVerification | CreateWithCorrectDefaults | N | P | 20/03/2026 |
| UTCID02 | CreateVerification WithDoctor | AssignDoctor | N | P | 20/03/2026 |
| UTCID03 | CreateVideoCall | CreateWithMemoOnlyChat | N | P | 20/03/2026 |
| UTCID04 | CreateVideoCall PastAppointment | Throw | A | P | 20/03/2026 |
| UTCID05 | CreateClinicBooking | CreateWithLockedChat | N | P | 20/03/2026 |
| UTCID06 | CreateClinicBooking PastAppointment | Throw | A | P | 20/03/2026 |
| UTCID07 | Confirm PendingSession | Confirm | N | P | 20/03/2026 |
| UTCID08 | Confirm NonPendingSession | Throw | A | P | 20/03/2026 |
| UTCID09 | EndSession AssignedDoctor | Complete | N | P | 20/03/2026 |
| UTCID10 | EndSession WrongDoctor | Throw | A | P | 20/03/2026 |
| UTCID11 | Cancel PendingSession | Cancel | N | P | 20/03/2026 |
| UTCID12 | Cancel CompletedSession | Throw | A | P | 20/03/2026 |
| UTCID13 | OpenChat LockedSession | Open | N | P | 20/03/2026 |
| UTCID14 | OpenChat ArchivedSession | Throw | A | P | 20/03/2026 |
| UTCID15 | AssignDoctor | SetOphthalmologistId | N | P | 20/03/2026 |

## Function17 - ConsultationSession (Part 2/2)
- Header: Passed=5, Failed=0, Untested=0, N=3, A=2, B=0, Total=5
- Pre-Condition: Domain object constructor inputs/state prepared; no external infrastructure dependency required.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | SetMeetingInfo ValidLink | Set | N | P | 20/03/2026 |
| UTCID02 | SetMeetingInfo EmptyLink | Throw | A | P | 20/03/2026 |
| UTCID03 | ClearMeetingInfo | Nullify | A | P | 20/03/2026 |
| UTCID04 | RecordActivity | UpdateLastActivityAt | N | P | 20/03/2026 |
| UTCID05 | RecordReminderSent | SetLastReminderSentAt | N | P | 20/03/2026 |

## Function18 - DomainEvent
- Header: Passed=2, Failed=0, Untested=0, N=2, A=0, B=0, Total=2
- Pre-Condition: Domain object constructor inputs/state prepared; no external infrastructure dependency required.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | DomainEvent | SetOccurredOnToUtcNow | N | P | 20/03/2026 |
| UTCID02 | DomainEvent | GenerateUniqueEventId | N | P | 20/03/2026 |

## Function19 - Money
- Header: Passed=15, Failed=0, Untested=0, N=8, A=6, B=1, Total=15
- Pre-Condition: Domain object constructor inputs/state prepared; no external infrastructure dependency required.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Constructor ValidInput | CreateMoney | N | P | 20/03/2026 |
| UTCID02 | Constructor NegativeAmount | Throw | A | P | 20/03/2026 |
| UTCID03 | Constructor ZeroAmount | Succeed | B | P | 20/03/2026 |
| UTCID04 | Constructor EmptyCurrency | Throw | A | P | 20/03/2026 |
| UTCID05 | Currency | BeUpperCase | N | P | 20/03/2026 |
| UTCID06 | Addition SameCurrency | Add | N | P | 20/03/2026 |
| UTCID07 | Addition DifferentCurrency | Throw | A | P | 20/03/2026 |
| UTCID08 | Subtraction SameCurrency | Subtract | N | P | 20/03/2026 |
| UTCID09 | Subtraction DifferentCurrency | Throw | A | P | 20/03/2026 |
| UTCID10 | Equals SameValues | BeEqual | N | P | 20/03/2026 |
| UTCID11 | Equals DifferentAmount | NotBeEqual | N | P | 20/03/2026 |
| UTCID12 | Equals DifferentCurrency | NotBeEqual | N | P | 20/03/2026 |
| UTCID13 | ToString | FormatCorrectly | N | P | 20/03/2026 |

## Function20 - Ophthalmologist
- Header: Passed=10, Failed=0, Untested=0, N=8, A=2, B=0, Total=10
- Pre-Condition: Domain object constructor inputs/state prepared; no external infrastructure dependency required.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Constructor | CreateWithPendingVerification | N | P | 20/03/2026 |
| UTCID02 | Constructor DefaultValues | UseDefaults | N | P | 20/03/2026 |
| UTCID03 | UpdateProfile ValidInput | Update | N | P | 20/03/2026 |
| UTCID04 | UpdateProfile NegativeYears | Throw | A | P | 20/03/2026 |
| UTCID05 | Verify | SetApproved | N | P | 20/03/2026 |
| UTCID06 | Reject | SetRejected | N | P | 20/03/2026 |
| UTCID07 | Unverify | ResetToPending | N | P | 20/03/2026 |
| UTCID08 | VerificationLifecycle Verify Unverify Reject | Expected per test method | N | P | 20/03/2026 |
| UTCID09 | UpdateCredentialFiles | UpdateNonNullValues | A | P | 20/03/2026 |
| UTCID10 | UpdateCredentialFiles BothValues | UpdateBoth | N | P | 20/03/2026 |

## Function21 - Order
- Header: Passed=12, Failed=0, Untested=0, N=6, A=6, B=0, Total=12
- Pre-Condition: Domain object constructor inputs/state prepared; no external infrastructure dependency required.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Constructor | CreatePendingOrder | N | P | 20/03/2026 |
| UTCID02 | FullLifecycle Pending Confirmed Processing Completed | Expected per test method | N | P | 20/03/2026 |
| UTCID03 | Refund CompletedOrder | Refund | N | P | 20/03/2026 |
| UTCID04 | Confirm NonPendingOrder | Throw | A | P | 20/03/2026 |
| UTCID05 | StartProcessing NonConfirmedOrder | Throw | A | P | 20/03/2026 |
| UTCID06 | Complete NonProcessingOrder | Throw | A | P | 20/03/2026 |
| UTCID07 | Cancel CompletedOrder | Throw | A | P | 20/03/2026 |
| UTCID08 | Cancel RefundedOrder | Throw | A | P | 20/03/2026 |
| UTCID09 | Refund NonCompletedOrder | Throw | A | P | 20/03/2026 |
| UTCID10 | Cancel PendingOrder | Succeed | N | P | 20/03/2026 |
| UTCID11 | Cancel ConfirmedOrder | Succeed | N | P | 20/03/2026 |
| UTCID12 | Cancel ProcessingOrder | Succeed | N | P | 20/03/2026 |

## Function22 - Patient
- Header: Passed=4, Failed=0, Untested=0, N=2, A=2, B=0, Total=4
- Pre-Condition: Domain object constructor inputs/state prepared; no external infrastructure dependency required.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Constructor | CreatePatient | N | P | 20/03/2026 |
| UTCID02 | Constructor DefaultValues | BeNull | A | P | 20/03/2026 |
| UTCID03 | UpdateProfile | UpdateFields | N | P | 20/03/2026 |
| UTCID04 | UpdateProfile NullValues | ClearFields | A | P | 20/03/2026 |

## Function23 - Wallet
- Header: Passed=14, Failed=0, Untested=0, N=3, A=9, B=2, Total=14
- Pre-Condition: Domain object constructor inputs/state prepared; no external infrastructure dependency required.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Constructor ValidInput | CreateWallet | N | P | 20/03/2026 |
| UTCID02 | Constructor DefaultBalance | BeZero | B | P | 20/03/2026 |
| UTCID03 | Constructor EmptyOwnerType | Throw | A | P | 20/03/2026 |
| UTCID04 | Constructor NegativeBalance | Throw | A | P | 20/03/2026 |
| UTCID05 | Deposit PositiveAmount | IncreaseBalance | N | P | 20/03/2026 |
| UTCID06 | Deposit NonPositiveAmount | Throw | A | P | 20/03/2026 |
| UTCID07 | Withdraw SufficientBalance | DecreaseBalance | N | P | 20/03/2026 |
| UTCID08 | Withdraw InsufficientBalance | Throw | A | P | 20/03/2026 |
| UTCID09 | Withdraw NonPositiveAmount | Throw | A | P | 20/03/2026 |
| UTCID10 | Withdraw ExactBalance | ResultInZero | B | P | 20/03/2026 |

## Function24 - DateTimeService
- Header: Passed=6, Failed=0, Untested=0, N=4, A=0, B=2, Total=6
- Pre-Condition: DateTimeService instance initialized; no external dependency required.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Now | ReturnCurrentLocalTime | N | P | 20/03/2026 |
| UTCID02 | UtcNow | ReturnCurrentUtcTime | N | P | 20/03/2026 |
| UTCID03 | Now | ReturnLocalKind | N | P | 20/03/2026 |
| UTCID04 | UtcNow | ReturnUtcKind | N | P | 20/03/2026 |
| UTCID05 | Now ConsecutiveCalls | BeNonDecreasing | B | P | 20/03/2026 |
| UTCID06 | UtcNow ConsecutiveCalls | BeNonDecreasing | B | P | 20/03/2026 |

## Function25 - JwtSettings
- Header: Passed=3, Failed=0, Untested=0, N=3, A=0, B=0, Total=3
- Pre-Condition: Settings object initialized with default/custom values to verify binding behavior.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | SectionName | BeJwtSettings | N | P | 20/03/2026 |
| UTCID02 | DefaultValues | BeCorrect | N | P | 20/03/2026 |
| UTCID03 | Properties | BeSettable | N | P | 20/03/2026 |

## Function26 - PayOSSettings
- Header: Passed=3, Failed=0, Untested=0, N=2, A=1, B=0, Total=3
- Pre-Condition: Settings object initialized with default/custom values to verify binding behavior.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | SectionName | BePayOS | N | P | 20/03/2026 |
| UTCID02 | DefaultValues | BeEmpty | A | P | 20/03/2026 |
| UTCID03 | Properties | BeSettable | N | P | 20/03/2026 |

## Function27 - SmtpSettings
- Header: Passed=3, Failed=0, Untested=0, N=3, A=0, B=0, Total=3
- Pre-Condition: Settings object initialized with default/custom values to verify binding behavior.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | SectionName | BeSmtp | N | P | 20/03/2026 |
| UTCID02 | DefaultValues | BeCorrect | N | P | 20/03/2026 |
| UTCID03 | Properties | BeSettable | N | P | 20/03/2026 |

## Function28 - SupabaseStorageSettings
- Header: Passed=3, Failed=0, Untested=0, N=2, A=1, B=0, Total=3
- Pre-Condition: Settings object initialized with default/custom values to verify binding behavior.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | SectionName | BeSupabaseStorage | N | P | 20/03/2026 |
| UTCID02 | DefaultValues | BeEmpty | A | P | 20/03/2026 |
| UTCID03 | Properties | BeSettable | N | P | 20/03/2026 |

## Function29 - TokenService (Part 1/2)
- Header: Passed=15, Failed=0, Untested=0, N=10, A=4, B=1, Total=15
- Pre-Condition: TokenService initialized with valid JwtSettings and required services; user/claims test data prepared.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | GenerateAccessTokenAsync | ReturnValidToken | N | P | 20/03/2026 |
| UTCID02 | GenerateAccessTokenAsync | ContainCorrectClaims | N | P | 20/03/2026 |
| UTCID03 | GenerateAccessTokenAsync WithMultipleRoles | ContainAllRoles | N | P | 20/03/2026 |
| UTCID04 | GenerateAccessTokenAsync WithAdditionalClaims | IncludeThem | N | P | 20/03/2026 |
| UTCID05 | GenerateAccessTokenAsync ExpiresAt | MatchSettings | B | P | 20/03/2026 |
| UTCID06 | GenerateAccessTokenAsync Jti | BeUniquePerCall | N | P | 20/03/2026 |
| UTCID07 | GenerateAccessTokenAsync TokenFormat | BeValidJwt | N | P | 20/03/2026 |
| UTCID08 | GenerateRefreshToken | ReturnNonEmptyString | A | P | 20/03/2026 |
| UTCID09 | GenerateRefreshToken | ReturnBase64String | N | P | 20/03/2026 |
| UTCID10 | GenerateRefreshToken | Return64BytesWhenDecoded | N | P | 20/03/2026 |
| UTCID11 | GenerateRefreshToken | GenerateUniqueTokens | N | P | 20/03/2026 |
| UTCID12 | ValidateToken WithValidToken | ReturnPrincipal | N | P | 20/03/2026 |
| UTCID13 | ValidateToken WithInvalidToken | ReturnNull | A | P | 20/03/2026 |
| UTCID14 | ValidateToken WithEmptyString | ReturnNull | A | P | 20/03/2026 |
| UTCID15 | ValidateToken WithTamperedToken | ReturnNull | A | P | 20/03/2026 |

## Function30 - TokenService (Part 2/2)
- Header: Passed=9, Failed=0, Untested=0, N=6, A=3, B=0, Total=9
- Pre-Condition: TokenService initialized with valid JwtSettings and required services; user/claims test data prepared.
- Date for all executed UTCID: 20/03/2026
- Exception row: leave blank (N/A)
- Log message row: leave blank (N/A)

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | GetUserIdFromToken WithValidToken | ReturnUserId | N | P | 20/03/2026 |
| UTCID02 | GetUserIdFromToken WithInvalidToken | ReturnNull | A | P | 20/03/2026 |
| UTCID03 | GetJtiFromToken WithValidToken | ReturnJti | N | P | 20/03/2026 |
| UTCID04 | GetJtiFromToken WithInvalidToken | ReturnNull | A | P | 20/03/2026 |
| UTCID05 | HashToken | ReturnNonEmptyString | A | P | 20/03/2026 |
| UTCID06 | HashToken SameInput | ReturnSameHash | N | P | 20/03/2026 |
| UTCID07 | HashToken DifferentInputs | ReturnDifferentHashes | N | P | 20/03/2026 |
| UTCID08 | HashToken | ReturnBase64String | N | P | 20/03/2026 |
| UTCID09 | HashToken | Return32BytesWhenDecoded | N | P | 20/03/2026 |

## Function31 - RefreshTokenService
- Header: Passed=18 xUnit cases / 9 checklist scenarios, Failed=0, N=5, A=3, B=1 (`RefreshTokenServiceTests.cs`)
- Pre-Condition: Service initialized with DbContext and seeded refresh token data; cancellation token available.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | CreateRefreshTokenAsync ValidInput | PersistTokenAndReturnId | N | P | 04/04/2026 |
| UTCID02 | GetByTokenHashAsync ExistingHash | ReturnRefreshTokenDto | N | P | 04/04/2026 |
| UTCID03 | GetByTokenHashAsync MissingHash | ReturnNull | N | P | 04/04/2026 |
| UTCID04 | RotateRefreshTokenAsync ValidOldToken | MarkOldUsedAndCreateNewToken | A | P | 04/04/2026 |
| UTCID05 | RotateRefreshTokenAsync MissingOldToken | ThrowInvalidOperationException | A | P | 04/04/2026 |
| UTCID06 | RevokeTokenAsync ExistingToken | MarkRevoked | N | P | 04/04/2026 |
| UTCID07 | RevokeAllUserTokensAsync ActiveTokens | RevokeAllMatchingTokens | B | P | 04/04/2026 |
| UTCID08 | RevokeTokenFamilyAsync ExistingToken | RevokeUserTokenFamily | A | P | 04/04/2026 |
| UTCID09 | CleanupExpiredTokensAsync ExpiredAndRevokedTokens | DeleteAndReturnCount | N | P | 04/04/2026 |

## Function32 - RefreshToken
- Header: Passed=6, Failed=0, Untested=0, N=4, A=1, B=1, Total=6
- Pre-Condition: RefreshToken entity state prepared with expiry and revocation scenarios.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Constructor Default | SetIdAndCreatedAt | N | P | 04/04/2026 |
| UTCID02 | Create Factory | SetTokenDataAndExpiry | N | P | 04/04/2026 |
| UTCID03 | IsExpired PastExpiry | ReturnTrue | A | P | 04/04/2026 |
| UTCID04 | IsActive FreshToken | ReturnTrue | N | P | 04/04/2026 |
| UTCID05 | MarkAsUsed | SetUsedAtAndReplacement | B | P | 04/04/2026 |
| UTCID06 | Revoke | SetRevokedAtAndReason | N | P | 04/04/2026 |

## Function33 - ApplicationUser
- Header: Passed=5, Failed=0, Untested=0, N=3, A=1, B=1, Total=5
- Pre-Condition: User entity initialized; profile and status fields available for mutation.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Constructor Default | SetDefaultStatusAndCreatedAt | N | P | 04/04/2026 |
| UTCID02 | Deactivate | SetInactiveAndUpdatedAt | N | P | 04/04/2026 |
| UTCID03 | Activate | SetActiveAndUpdatedAt | N | P | 04/04/2026 |
| UTCID04 | SoftDelete | SetDeletedFlagsAndTimestamps | A | P | 04/04/2026 |
| UTCID05 | UpdateLastLogin | SetLastLoginAt | B | P | 04/04/2026 |

## Function34 - ProfessionalPost
- Header: Passed=12+, Failed=0, Untested=0, N=6, A=4, B=2, Total=12+ (`ProfessionalPostTests.cs`)
- Pre-Condition: Post entity initialized with author, content, category, and optional repost metadata.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Constructor ValidInput | CreateOriginalPost | N | P | 04/04/2026 |
| UTCID02 | Constructor EmptyContent | ThrowArgumentException | A | P | 04/04/2026 |
| UTCID03 | CreateRepost ValidOriginalPost | CopyOriginalAndMarkAsRepost | N | P | 04/04/2026 |
| UTCID04 | UpdateContent ValidInput | SetContentAndUpdatedAt | N | P | 04/04/2026 |
| UTCID05 | UpdateContent EmptyContent | ThrowArgumentException | A | P | 04/04/2026 |
| UTCID06 | UpdateAllowComments | ToggleFlag | B | P | 04/04/2026 |
| UTCID07 | IncrementReactionCount | IncreaseCount | N | P | 04/04/2026 |
| UTCID08 | DecrementReactionCount AtZero | StayZero | A | P | 04/04/2026 |
| UTCID09 | IncrementCommentCount | IncreaseCount | N | P | 04/04/2026 |
| UTCID10 | DecrementCommentCount AtZero | StayZero | A | P | 04/04/2026 |
| UTCID11 | Hide WithReason | TrimAndStoreReason | A | P | 04/04/2026 |
| UTCID12 | SetClinicalCaseMetadata ValidInput | SetInternalCaseMetadata | B | P | 04/04/2026 |

## Function35 - PostComment
- Header: Passed=6+, Failed=0, Untested=0, N=4, A=2, B=0, Total=6+ (`PostCommentTests.cs`)
- Pre-Condition: Comment entity initialized with post, author, and optional parent comment data.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Constructor ValidInput | CreateComment | N | P | 04/04/2026 |
| UTCID02 | Constructor EmptyContent | ThrowArgumentException | A | P | 04/04/2026 |
| UTCID03 | UpdateContent ValidInput | SetContentAndUpdatedAt | N | P | 04/04/2026 |
| UTCID04 | UpdateContent EmptyContent | ThrowArgumentException | A | P | 04/04/2026 |
| UTCID05 | IncrementReplyCount | IncreaseCount | N | P | 04/04/2026 |
| UTCID06 | DecrementLikeCount AtZero | StayZero | B | P | 04/04/2026 |

## Function36 - PostReaction
- Header: Passed=4+, Failed=0, Untested=0, N=2, A=1, B=0, Total=4+ (`PostReactionTests.cs`)
- Pre-Condition: Reaction entity initialized with post, user, and reaction type.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Constructor ValidInput | CreateReaction | N | P | 04/04/2026 |
| UTCID02 | ChangeType ValidType | UpdateTypeAndUpdatedAt | N | P | 04/04/2026 |
| UTCID03 | ChangeType SameType | KeepConsistentState | A | P | 04/04/2026 |

## Function37 - SavedPost
- Header: Passed=3+, Failed=0, Untested=0, N=2, A=1, B=0, Total=3+ (`SavedPostTests.cs`)
- Pre-Condition: Saved post entity initialized with user, post, and optional collection name.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Constructor ValidInput | CreateSavedPost | N | P | 04/04/2026 |
| UTCID02 | MoveToCollection ValidName | UpdateCollectionAndTimestamp | N | P | 04/04/2026 |
| UTCID03 | MoveToCollection NullName | ClearCollection | A | P | 04/04/2026 |

## Function38 - PostAttachment
- Header: Passed=4+, Failed=0, Untested=0, N=2, A=2, B=0, Total=4+ (`PostAttachmentTests.cs`)
- Pre-Condition: Attachment entity initialized with post, file metadata, and display order.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Constructor ValidInput | CreateAttachment | N | P | 04/04/2026 |
| UTCID02 | Constructor EmptyFileName | ThrowArgumentException | A | P | 04/04/2026 |
| UTCID03 | Constructor EmptyFileUrl | ThrowArgumentException | A | P | 04/04/2026 |
| UTCID04 | UpdateDisplayOrder | SetOrderAndUpdatedAt | B | P | 04/04/2026 |

## Function39 - AuthService
- Header: Passed=5, Failed=0, NotTestedYet=10, N=8, A=5, B=2, Total=15 — **partial** (`AuthServiceRegistrationTests.cs`)
- Pre-Condition: Auth service initialized with mocked identity, token, refresh-token, email, notification, repository, and unit-of-work dependencies.
- Date: **P** rows 04/04/2026; **NT** rows — (chưa có test).

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | RegisterPatientAsync ExistingEmail | ReturnFailure | N | P | 04/04/2026 |
| UTCID02 | RegisterPatientAsync ValidInput | CreatePatientAndReturnSuccess | N | NT | — |
| UTCID03 | RegisterPatientAsync CreateUserFails | RollbackAndReturnFailure | A | NT | — |
| UTCID04 | RegisterOphthalmologistAsync NoCredentials | ReturnFailure | A | P | 04/04/2026 |
| UTCID05 | RegisterOphthalmologistAsync MissingDegree | ReturnFailure | A | P | 04/04/2026 |
| UTCID06 | RegisterOphthalmologistAsync MissingLicense | ReturnFailure | A | P | 04/04/2026 |
| UTCID07 | RegisterOphthalmologistAsync ValidInput | UploadFilesAndReturnSuccess | N | NT | — |
| UTCID08 | RegisterOrganisationAsync ValidInput | DelegateToOnboardingService | N | P | 04/04/2026 |
| UTCID09 | GoogleLoginAsync ValidGoogleToken | ReturnSuccess | N | NT | — |
| UTCID10 | GoogleLoginAsync InvalidGoogleToken | ReturnFailure | A | NT | — |
| UTCID11 | LoginAsync RequiresTwoFactor | ReturnTwoFactorRequired | B | NT | — |
| UTCID12 | LoginAsync ValidPassword | ReturnAuthResponse | N | NT | — |
| UTCID13 | RefreshTokenAsync ValidTokens | ReturnNewAuthResponse | N | NT | — |
| UTCID14 | LogoutAsync ValidRefreshToken | RevokeTokenAndReturnSuccess | N | NT | — |
| UTCID15 | ResetPasswordAsync InvalidToken | ReturnFailure | A | NT | — |

## Function40 - NotificationService
- Header: Passed=5, Failed=0, Untested=0, N=3, A=2, B=0, Total=5 (`NotificationServiceTests.cs` + InMemory `ApplicationDbContext`)
- Pre-Condition: Notification service with real `Repository<Notification>` and `ApplicationDbContext` (in-memory); hub mocked.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | SendAsync WithPayload | SerializePayloadAndPersist | N | P | 04/04/2026 |
| UTCID02 | SendAsync WithReferenceIdPayload | ExtractReferenceId | N | P | 04/04/2026 |
| UTCID03 | SendAsync WithoutPayload | PersistWithoutPayload | N | P | 04/04/2026 |
| UTCID04 | SendAsync NestedPayloadNoGuidKeys | IgnoreReferenceId | A | P | 04/04/2026 |
| UTCID05 | LegacySendAsync | RouteToTypedNotification | B | P | 04/04/2026 |

## Function41 - EmailService
- Header: UTC thực=6, Passed=6 (3 template + 3 `SendAsync` guards), Failed=0, NT=2, N=3, A=3, B=0
- Pre-Condition: `EmailTemplatesTests` (subject + HTML bodies); `EmailServiceValidationTests` (empty to/subject/body → `ArgumentException`). Gửi SMTP thật / lỗi mạng → **NT** hoặc integration test.

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | SendEmailConfirmationAsync | BuildTemplateAndSend | N | P | 04/04/2026 |
| UTCID02 | SendPasswordResetAsync | BuildTemplateAndSend | N | P | 04/04/2026 |
| UTCID03 | SendWelcomeEmailAsync | BuildTemplateAndSend | N | P | 04/04/2026 |
| UTCID04 | SendAsync PlainText | SendPlainTextBody | A | NT | — |
| UTCID05 | SendAsync EmptyTo | ThrowArgumentException | A | P | 04/04/2026 |
| UTCID06 | SendAsync SMTPFailure | LogAndRethrow | A | NT | — |

*Note: UTCID05 covers empty-to guard; **P** cũng cho empty subject và empty body (`EmailServiceValidationTests`). UTCID01–03 **P** map `EmailTemplatesTests` (build nội dung/subject trước khi gọi `SendAsync`).*

## Function42 - DashboardMetricsService
- Header: Passed=7, Failed=0, Untested=0, N=5, A=2, B=1, Total=8 (`DashboardMetricsServiceTests.cs`)
- Pre-Condition: Service with `ApplicationDbContext` (in-memory) and mocked `IAiQuotaService`.
- Date: **P** rows 04/04/2026.

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | GetSystemAdminMetricsAsync EmptyDb | ReturnZeroGrowthMetrics | N | P | 04/04/2026 |
| UTCID02 | GetSystemAdminMetricsAsync NoDeposits | ReturnZeroRevenueBreakdown | A | P | 04/04/2026 |
| UTCID03 | GetRecentScreeningsAsync PageOne | ReturnPagedResults | N | P | 04/04/2026 |
| UTCID04 | GetScreeningVolumeTrendsAsync MonthlyRange | ReturnMonthlySeries | N | P | 04/04/2026 |
| UTCID05 | GetScreeningVolumeTrendsAsync InvalidRange | DefaultToMonthly | A | P | 04/04/2026 |
| UTCID06 | GetPopulationRiskAnalysisAsync EmptyDb | ReturnDto | N | P | 04/04/2026 |
| UTCID07 | GetSystemHealthAsync | ReturnStatus | B | P | 04/04/2026 |
| UTCID08 | GetOphthalmologistMetricsAsync WithOphthalId | ReturnDto | N | P | 04/04/2026 |

## Function43 - CreateAiScreeningSessionCommandHandler
- Header: Passed=5, Failed=0, Untested=0, N=3, A=2, B=0, Total=5
- Pre-Condition: Handler initialized with mocked repositories, current user service, and unit of work.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Handle UnauthenticatedUser | ReturnUnauthorized | A | P | 04/04/2026 |
| UTCID02 | Handle MissingPatientProfile | ReturnNotFound | A | P | 04/04/2026 |
| UTCID03 | Handle ValidCommandWithoutImages | CreateSession | N | P | 04/04/2026 |
| UTCID04 | Handle ValidCommandWithImages | SaveImagesAndReturnDtos | N | P | 04/04/2026 |
| UTCID05 | Handle RepositoryFailure | ReturnFailure | B | P | 04/04/2026 |

## Function44 - SaveAiScreeningResultsCommandHandler
- Header: Passed=6, Failed=0, Untested=0, N=3, A=3, B=0, Total=6
- Pre-Condition: Handler + `AsyncQueryable` mock for `IRepository.Query()` + EF async.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Handle InvalidConfidenceScore | ReturnFailure | A | P | 04/04/2026 |
| UTCID02 | Handle MissingScreening | ReturnNotFound | A | P | 04/04/2026 |
| UTCID03 | Handle MissingPatient | ReturnNotFound | A | P | 04/04/2026 |
| UTCID04 | Handle MissingConsent | ReturnFailure | N | P | 04/04/2026 |
| UTCID05 | Handle ValidConsentAndResults | SaveResultAndReturnSuccess | N | P | 04/04/2026 |
| UTCID06 | Handle TransactionFailure | RollbackAndReturnFailure | B | P | 04/04/2026 |

## Function45 - CompleteAiScreeningCommandHandler
- Header: Passed=5, Failed=0, Untested=0, N=3, A=2, B=0, Total=5
- Pre-Condition: Handler initialized with mocked repositories, notification service, and unit of work.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Handle MissingScreening | ReturnNotFound | A | P | 04/04/2026 |
| UTCID02 | Handle AlreadyProcessedScreening | ReturnFailure | A | P | 04/04/2026 |
| UTCID03 | Handle ValidScreeningWithPatient | ProcessAndNotify | N | P | 04/04/2026 |
| UTCID04 | Handle ValidScreeningWithoutPatient | ProcessWithoutNotification | N | P | 04/04/2026 |
| UTCID05 | Handle NormalStatus | UseNormalNotificationTemplate | B | P | 04/04/2026 |

## Function46 - CreateScheduleTemplateCommandHandler
- Header: Passed=2, Failed=0, Untested=0, N=1, A=1, B=0, Total=2
- Pre-Condition: Handler initialized with repository and unit of work mocks; schedule template command prepared.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Handle NoOverlap | CreateAndReturnId | N | P | 04/04/2026 |
| UTCID02 | Handle OverlapExists | ReturnConflict | A | P | 04/04/2026 |

## Function47 - UpdateScheduleTemplateCommandHandler
- Header: Passed=3, Failed=0, Untested=0, N=2, A=1, B=0, Total=3
- Pre-Condition: Handler initialized with repository and unit of work mocks; existing template or null result prepared.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Handle TemplateNotFound | ReturnNotFound | A | P | 04/04/2026 |
| UTCID02 | Handle NoOverlap | UpdateAndReturnSuccess | N | P | 04/04/2026 |
| UTCID03 | Handle OverlapExists | ReturnConflict | N | P | 04/04/2026 |

## Function48 - DeleteScheduleTemplateCommandHandler
- Header: Passed=3, Failed=0, Untested=0, N=2, A=1, B=0, Total=3
- Pre-Condition: Handler initialized with repository, unit of work, and logger mocks; template state prepared (reflection injects slot for active-slot case).
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Handle TemplateNotFound | ReturnNotFound | A | P | 04/04/2026 |
| UTCID02 | Handle NoActiveSlots | DeleteAndReturnSuccess | N | P | 04/04/2026 |
| UTCID03 | Handle ActiveSlotsExist | ReturnFailure | N | P | 04/04/2026 |

## Function49 - GetScheduleTemplateQueryHandler
- Header: Passed=2, Failed=0, Untested=0, N=1, A=1, B=0, Total=2
- Pre-Condition: Handler initialized with repository mock and template query prepared.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Handle TemplateFound | ReturnMappedDto | N | P | 04/04/2026 |
| UTCID02 | Handle TemplateNotFound | ReturnNotFound | A | P | 04/04/2026 |

## Function50 - GetScheduleTemplatesQueryHandler
- Header: Passed=2, Failed=0, Untested=0, N=1, A=1, B=0, Total=2
- Pre-Condition: Handler initialized with repository mock and paged query prepared.
- Date for all executed UTCID: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | Handle PopulatedPage | ReturnPagedDtoList | N | P | 04/04/2026 |
| UTCID02 | Handle EmptyPage | ReturnEmptyPagedResult | A | P | 04/04/2026 |

## Function51 - Domain entity tests (batch A — chưa có Function riêng ở trên)
- Header: UTC thực=8, Passed=8, Failed=0, NT=0 — mỗi UTC = toàn bộ test trong file `tests/Domain.UnitTests/Entities/`.
- Date: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | `AiScreeningTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID02 | `AppointmentSlotTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID03 | `AppointmentTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID04 | `AuditLogTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID05 | `CertificateTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID06 | `ChatMessageTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID07 | `ConsentTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID08 | `ContractTemplateTests.cs` | All facts pass | N | P | 04/04/2026 |

## Function52 - Domain entity tests (batch B)
- Header: UTC thực=8, Passed=8, Failed=0, NT=0
- Date: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | `ContractTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID02 | `ConversationTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID03 | `DepositRequestTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID04 | `MedicalDiagnosisTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID05 | `NotificationTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID06 | `OrganisationFeedbackTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID07 | `OrganisationOnboardingRequestTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID08 | `OrganisationTests.cs` | All facts pass | N | P | 04/04/2026 |

## Function53 - Domain entity tests (batch C)
- Header: UTC thực=8, Passed=8, Failed=0, NT=0
- Date: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | `OphthalmologistFeedbackTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID02 | `PatientRoadmapTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID03 | `PaymentTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID04 | `PermissionTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID05 | `RetinalImageTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID06 | `RolePermissionTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID07 | `ScreeningResultTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID08 | `ScheduleTemplateTests.cs` | All facts pass | N | P | 04/04/2026 |

## Function54 - Domain entity tests (batch D)
- Header: UTC thực=5, Passed=5, Failed=0, NT=0
- Date: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | `SystemSettingTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID02 | `UserPermissionTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID03 | `WebsiteFeedbackTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID04 | `WithdrawalRequestTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID05 | `WalletTransactionTests.cs` | All facts pass | N | P | 04/04/2026 |

## Function55 - Application validators (batch A — ngoài Function6 / Function11)
- Header: UTC thực=8, Passed=8, Failed=0, NT=0 — `tests/Application.UnitTests/Validators/`
- Date: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | `AgreeScreeningConsentCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID02 | `BuyAiQuotaCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID03 | `CancelSessionCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID04 | `ChangePasswordCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID05 | `CompleteAiScreeningCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID06 | `ConfirmReservationCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID07 | `CreateAiScreeningSessionCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID08 | `CreateAppointmentSlotCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |

## Function56 - Application validators (batch B)
- Header: UTC thực=8, Passed=8, Failed=0, NT=0
- Date: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | `CreateContractCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID02 | `CreateContractTemplateCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID03 | `CreateDepositCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID04 | `CreateOphthalmologistCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID05 | `CreateOphthalmologistFeedbackCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID06 | `CreateOrganisationFeedbackCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID07 | `CreatePermissionCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID08 | `CreateScheduleTemplateCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |

## Function57 - Application validators (batch C)
- Header: UTC thực=8, Passed=8, Failed=0, NT=0
- Date: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | `CreateVideoCallSessionCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID02 | `CreateWebsiteFeedbackCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID03 | `CreateWithdrawalRequestCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID04 | `EndSessionCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID05 | `GenerateSlotsCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID06 | `ReserveSlotCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID07 | `SendMessageCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID08 | `SubmitVerificationReportCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |

## Function58 - Application validators (batch D)
- Header: UTC thực=4, Passed=4, Failed=0, NT=0
- Date: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | `UpdateOphthalmologistCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID02 | `UpdatePermissionCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID03 | `UpdateScheduleTemplateCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID04 | `VerifyPaymentCommandValidatorTests.cs` | All facts pass | N | P | 04/04/2026 |

## Function59 - Application command handlers (bổ sung — ngoài Function5,7,43–50)
- Header: UTC thực=4, Passed=4, Failed=0, NT=0 — `tests/Application.UnitTests/Handlers/`
- Date: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | `CreateDepositCommandHandlerTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID02 | `CreateWithdrawalRequestCommandHandlerTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID03 | `VerifyPaymentCommandHandlerTests.cs` | All facts pass | N | P | 04/04/2026 |
| UTCID04 | `SubmitVerificationReportCommandHandlerTests.cs` | All facts pass | N | P | 04/04/2026 |

## Function60 - SystemSettingService (Infrastructure)
- Header: UTC thực=1, Passed=1, Failed=0, NT=0 — `tests/Infrastructure.UnitTests/Services/SystemSettingServiceTests.cs` (map đủ file test; không gộp vào F42 Dashboard).
- Date: 04/04/2026

| UTCID | O at Condition row | O at Return row | Type | P/F | Date |
|---|---|---|---|---|---|
| UTCID01 | `SystemSettingServiceTests.cs` | All facts pass | N | P | 04/04/2026 |

### Tổng hợp UTC thực (cập nhật theo repo)

- **~453** dòng `| UTCID` trong file này (đếm bằng tìm kiếm `^\| UTCID` — chạy lại khi sửa checklist).
- **1053** test trong solution (`dotnet test`): vẫn **≥** số UTC vì một dòng có thể gộp nhiều `[Fact]` (Domain/Application batch) hoặc **một test = một UTC** (API Function2–4 sau khi bổ sung).

