# Infrastructure 105 Functions - Sheet Copy README (Test Accurate)

This file is generated from the checklist and the current generated test files.
Condition labels are humanized, and Return / Exception / Log entries are taken from the test assertions.

## Quick Index

- F001 - ApplicationUser.Deactivate
- F002 - ApplicationUser.Activate
- F003 - ApplicationUser.SoftDelete
- F004 - ApplicationUser.UpdateLastLogin
- F005 - AuthService.RegisterPatientAsync
- F006 - AuthService.RegisterOphthalmologistAsync
- F007 - AuthService.RegisterOrganisationAsync
- F008 - AuthService.GoogleLoginAsync
- F009 - AuthService.LoginAsync
- F010 - AuthService.VerifyTwoFactorLoginAsync
- F011 - AuthService.RefreshTokenAsync
- F012 - AuthService.LogoutAsync
- F013 - AuthService.LogoutAllAsync
- F014 - AuthService.ConfirmEmailAsync
- F015 - AuthService.ForgotPasswordAsync
- F016 - AuthService.ResetPasswordAsync
- F017 - AuthService.GetCurrentUserAsync
- F018 - AuthService.ResendConfirmationAsync
- F019 - IdentityService.CheckPasswordAsync
- F020 - IdentityService.GetUserByEmailAsync
- F021 - IdentityService.GetUserByIdAsync
- F022 - IdentityService.IsPhoneNumberInUseByOrganizationAsync
- F023 - IdentityService.IsEmailConfirmedAsync
- F024 - IdentityService.IsUserActiveAsync
- F025 - IdentityService.GenerateEmailConfirmationTokenAsync
- F026 - IdentityService.GeneratePasswordResetTokenAsync
- F027 - IdentityService.GetUserRolesAsync
- F028 - IdentityService.IsInRoleAsync
- F029 - IdentityService.GetUserIdsByRoleAndOrganizationAsync
- F030 - IdentityService.UpdateLastLoginAsync
- F031 - IdentityService.IsTwoFactorEnabledAsync
- F032 - IdentityService.GetAuthenticatorKeyAsync
- F033 - IdentityService.GetOrCreateAuthenticatorKeyAsync
- F034 - IdentityService.VerifyTwoFactorCodeAsync
- F035 - IdentityService.GenerateNewRecoveryCodesAsync
- F036 - IdentityService.GetRecoveryCodesCountAsync
- F037 - IdentityService.GenerateAuthenticatorUri
- F038 - IdentityService.FormatAuthenticatorKey
- F039 - IdentityService.GetUserMetricsAsync
- F040 - IdentityService.GetUsersInRoleCountAsync
- F041 - IdentityService.GetPendingApprovalsCountAsync
- F042 - IdentityService.GetUserDetailsAsync
- F043 - RefreshToken.Create
- F044 - RefreshToken.MarkAsUsed
- F045 - RefreshToken.Revoke
- F046 - RefreshTokenService.CreateRefreshTokenAsync
- F047 - RefreshTokenService.GetByTokenHashAsync
- F048 - RefreshTokenService.RotateRefreshTokenAsync
- F049 - RefreshTokenService.RevokeTokenAsync
- F050 - RefreshTokenService.RevokeAllUserTokensAsync
- F051 - RefreshTokenService.RevokeTokenFamilyAsync
- F052 - RefreshTokenService.CleanupExpiredTokensAsync
- F053 - TokenService.GenerateAccessTokenAsync
- F054 - TokenService.GenerateRefreshToken
- F055 - TokenService.ValidateToken
- F056 - TokenService.GetUserIdFromToken
- F057 - TokenService.GetJtiFromToken
- F058 - TokenService.HashToken
- F059 - AdminQueryService.GetOphthalmologistsAsync
- F060 - AdminQueryService.GetPatientsAsync
- F061 - AdminQueryService.GetAuditLogsAsync
- F062 - AiQuotaService.GetQuotaAsync
- F063 - AiQuotaService.HasAvailableQuotaAsync
- F064 - AiQuotaService.DeductQuotaAsync
- F065 - AiQuotaService.AddPurchasedQuotaAsync
- F066 - BetterStackHeartbeatService.GetEmbedUrl
- F067 - BetterStackHeartbeatService.GetMonitorDescriptors
- F068 - BetterStackHeartbeatService.NotifyStartedAsync
- F069 - BetterStackHeartbeatService.NotifySucceededAsync
- F070 - BetterStackHeartbeatService.NotifyFailedAsync
- F071 - DailyQuotaResetJob.ExecuteAsync
- F072 - DashboardMetricsService.GetSystemAdminMetricsAsync
- F073 - DashboardMetricsService.GetRecentScreeningsAsync
- F074 - DashboardMetricsService.GetScreeningVolumeTrendsAsync
- F075 - DashboardMetricsService.GetPopulationRiskAnalysisAsync
- F076 - DashboardMetricsService.GetSystemHealthAsync
- F077 - DashboardMetricsService.GetOphthalmologistMetricsAsync
- F078 - DashboardMetricsService.GetOrganisationMetricsAsync
- F079 - DashboardMetricsService.GetPatientMetricsAsync
- F080 - DateTimeService.Now (property)
- F081 - DateTimeService.UtcNow (property)
- F082 - EmailService.SendEmailConfirmationAsync
- F083 - EmailService.SendPasswordResetAsync
- F084 - EmailService.SendWelcomeEmailAsync
- F085 - EmailService.SendAsync
- F086 - GoogleMeetService.CreateMeetingAsync
- F087 - GoogleMeetService.DeleteMeetingAsync
- F088 - GoogleMeetService.Dispose
- F089 - NotificationService.SendAsync (typed)
- F090 - NotificationService.SendAsync (legacy)
- F091 - OrganisationOnboardingService.SubmitRequestAsync
- F092 - OrganisationOnboardingService.GetRequestsAsync
- F093 - OrganisationOnboardingService.ApproveRequestAsync
- F094 - PatientRoadmapGenerationService.GenerateFromDiagnosisAsync
- F095 - PayOSService.CreatePaymentLinkAsync
- F096 - PayOSService.GetPaymentStatusAsync
- F097 - PayOSService.VerifyWebhookSignatureAsync
- F098 - PayOSService.CancelPaymentAsync
- F099 - SlotMaintenanceJob.ExpireUnusedSlotsAsync
- F100 - SupabaseStorageService.SaveFileAsync
- F101 - SupabaseStorageService.DeleteFile
- F102 - SupabaseStorageService.FileExists
- F103 - SystemSettingService.GetSettingAsync
- F104 - SystemSettingService.GetAllSettingsAsync
- F105 - SystemSettingService.UpdateSettingsAsync

---

## F001 - ApplicationUser.Deactivate

### 1) Function Header

| Field            | Value                                                                                                                                 |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F001                                                                                                                                  |
| Function Name    | Deactivate                                                                                                                            |
| Class Name       | ApplicationUser                                                                                                                       |
| Method           | Deactivate                                                                                                                            |
| Requirement      | Deactivate user                                                                                                                       |
| Description      | Validate 'Deactivate user' in ApplicationUser.Deactivate, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                     |
| Passed           | 7                                                                                                                                     |
| Failed           | 0                                                                                                                                     |
| Untested         | 0                                                                                                                                     |
| N/A/B            | N=7, A=0, B=0                                                                                                                         |

### 2) Condition + Precondition

- Precondition: Valid ApplicationUser instance is initialized with the required active/inactive/deleted state.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC001-01 |
| Method Should Exist                                         | UTC001-02 |
| Source Should Contain Method Declaration                    | UTC001-03 |
| Return Case Set Should Be Valid                             | UTC001-04 |
| Log Message Case Set Should Be Valid                        | UTC001-05 |
| When Logger Used Should Follow Log Message Convention       | UTC001-06 |
| When Result Response Used Should Follow Response Convention | UTC001-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC001-01 | N                 | Type Should Exist                                           |
| UTC001-02 | N                 | Method Should Exist                                         |
| UTC001-03 | N                 | Source Should Contain Method Declaration                    |
| UTC001-04 | N                 | Return Case Set Should Be Valid                             |
| UTC001-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC001-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC001-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F001_ApplicationUser_Deactivate_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F002 - ApplicationUser.Activate

### 1) Function Header

| Field            | Value                                                                                                                             |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F002                                                                                                                              |
| Function Name    | Activate                                                                                                                          |
| Class Name       | ApplicationUser                                                                                                                   |
| Method           | Activate                                                                                                                          |
| Requirement      | Activate user                                                                                                                     |
| Description      | Validate 'Activate user' in ApplicationUser.Activate, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                 |
| Passed           | 7                                                                                                                                 |
| Failed           | 0                                                                                                                                 |
| Untested         | 0                                                                                                                                 |
| N/A/B            | N=7, A=0, B=0                                                                                                                     |

### 2) Condition + Precondition

- Precondition: Valid ApplicationUser instance is initialized with the required active/inactive/deleted state.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC002-01 |
| Method Should Exist                                         | UTC002-02 |
| Source Should Contain Method Declaration                    | UTC002-03 |
| Return Case Set Should Be Valid                             | UTC002-04 |
| Log Message Case Set Should Be Valid                        | UTC002-05 |
| When Logger Used Should Follow Log Message Convention       | UTC002-06 |
| When Result Response Used Should Follow Response Convention | UTC002-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC002-01 | N                 | Type Should Exist                                           |
| UTC002-02 | N                 | Method Should Exist                                         |
| UTC002-03 | N                 | Source Should Contain Method Declaration                    |
| UTC002-04 | N                 | Return Case Set Should Be Valid                             |
| UTC002-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC002-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC002-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F002_ApplicationUser_Activate_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F003 - ApplicationUser.SoftDelete

### 1) Function Header

| Field            | Value                                                                                                                                  |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F003                                                                                                                                   |
| Function Name    | SoftDelete                                                                                                                             |
| Class Name       | ApplicationUser                                                                                                                        |
| Method           | SoftDelete                                                                                                                             |
| Requirement      | Soft delete user                                                                                                                       |
| Description      | Validate 'Soft delete user' in ApplicationUser.SoftDelete, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                      |
| Passed           | 7                                                                                                                                      |
| Failed           | 0                                                                                                                                      |
| Untested         | 0                                                                                                                                      |
| N/A/B            | N=7, A=0, B=0                                                                                                                          |

### 2) Condition + Precondition

- Precondition: Valid ApplicationUser instance is initialized with the required active/inactive/deleted state.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC003-01 |
| Method Should Exist                                         | UTC003-02 |
| Source Should Contain Method Declaration                    | UTC003-03 |
| Return Case Set Should Be Valid                             | UTC003-04 |
| Log Message Case Set Should Be Valid                        | UTC003-05 |
| When Logger Used Should Follow Log Message Convention       | UTC003-06 |
| When Result Response Used Should Follow Response Convention | UTC003-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC003-01 | N                 | Type Should Exist                                           |
| UTC003-02 | N                 | Method Should Exist                                         |
| UTC003-03 | N                 | Source Should Contain Method Declaration                    |
| UTC003-04 | N                 | Return Case Set Should Be Valid                             |
| UTC003-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC003-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC003-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F003_ApplicationUser_SoftDelete_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F004 - ApplicationUser.UpdateLastLogin

### 1) Function Header

| Field            | Value                                                                                                                                        |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F004                                                                                                                                         |
| Function Name    | UpdateLastLogin                                                                                                                              |
| Class Name       | ApplicationUser                                                                                                                              |
| Method           | UpdateLastLogin                                                                                                                              |
| Requirement      | Update last login                                                                                                                            |
| Description      | Validate 'Update last login' in ApplicationUser.UpdateLastLogin, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                            |
| Passed           | 7                                                                                                                                            |
| Failed           | 0                                                                                                                                            |
| Untested         | 0                                                                                                                                            |
| N/A/B            | N=7, A=0, B=0                                                                                                                                |

### 2) Condition + Precondition

- Precondition: Valid ApplicationUser instance is initialized with the required active/inactive/deleted state.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC004-01 |
| Method Should Exist                                         | UTC004-02 |
| Source Should Contain Method Declaration                    | UTC004-03 |
| Return Case Set Should Be Valid                             | UTC004-04 |
| Log Message Case Set Should Be Valid                        | UTC004-05 |
| When Logger Used Should Follow Log Message Convention       | UTC004-06 |
| When Result Response Used Should Follow Response Convention | UTC004-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC004-01 | N                 | Type Should Exist                                           |
| UTC004-02 | N                 | Method Should Exist                                         |
| UTC004-03 | N                 | Source Should Contain Method Declaration                    |
| UTC004-04 | N                 | Return Case Set Should Be Valid                             |
| UTC004-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC004-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC004-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F004_ApplicationUser_UpdateLastLogin_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F005 - AuthService.RegisterPatientAsync

### 1) Function Header

| Field            | Value                                                                                                                                                |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F005                                                                                                                                                 |
| Function Name    | RegisterPatientAsync                                                                                                                                 |
| Class Name       | AuthService                                                                                                                                          |
| Method           | RegisterPatientAsync                                                                                                                                 |
| Requirement      | Register patient account                                                                                                                             |
| Description      | Validate 'Register patient account' in AuthService.RegisterPatientAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                    |
| Passed           | 7                                                                                                                                                    |
| Failed           | 0                                                                                                                                                    |
| Untested         | 0                                                                                                                                                    |
| N/A/B            | N=7, A=0, B=0                                                                                                                                        |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC005-01 |
| Method Should Exist                                         | UTC005-02 |
| Source Should Contain Method Declaration                    | UTC005-03 |
| Return Case Set Should Be Valid                             | UTC005-04 |
| Log Message Case Set Should Be Valid                        | UTC005-05 |
| When Logger Used Should Follow Log Message Convention       | UTC005-06 |
| When Result Response Used Should Follow Response Convention | UTC005-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result<RegisterResponse>.Failure("A user with this email already exists")

#### Exception

- No dedicated exception case asserted.

#### Log message

- Patient registered: {Email}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC005-01 | N                 | Type Should Exist                                           |
| UTC005-02 | N                 | Method Should Exist                                         |
| UTC005-03 | N                 | Source Should Contain Method Declaration                    |
| UTC005-04 | N                 | Return Case Set Should Be Valid                             |
| UTC005-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC005-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC005-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F005_AuthService_RegisterPatientAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F006 - AuthService.RegisterOphthalmologistAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F006                                                                                                                                                                 |
| Function Name    | RegisterOphthalmologistAsync                                                                                                                                         |
| Class Name       | AuthService                                                                                                                                                          |
| Method           | RegisterOphthalmologistAsync                                                                                                                                         |
| Requirement      | Register ophthalmologist account                                                                                                                                     |
| Description      | Validate 'Register ophthalmologist account' in AuthService.RegisterOphthalmologistAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                    |
| Passed           | 7                                                                                                                                                                    |
| Failed           | 0                                                                                                                                                                    |
| Untested         | 0                                                                                                                                                                    |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                        |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC006-01 |
| Method Should Exist                                         | UTC006-02 |
| Source Should Contain Method Declaration                    | UTC006-03 |
| Return Case Set Should Be Valid                             | UTC006-04 |
| Log Message Case Set Should Be Valid                        | UTC006-05 |
| When Logger Used Should Follow Log Message Convention       | UTC006-06 |
| When Result Response Used Should Follow Response Convention | UTC006-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result<RegisterResponse>.Failure("At least one credential is required")

#### Exception

- No dedicated exception case asserted.

#### Log message

- Ophthalmologist registered: {Email}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC006-01 | N                 | Type Should Exist                                           |
| UTC006-02 | N                 | Method Should Exist                                         |
| UTC006-03 | N                 | Source Should Contain Method Declaration                    |
| UTC006-04 | N                 | Return Case Set Should Be Valid                             |
| UTC006-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC006-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC006-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F006_AuthService_RegisterOphthalmologistAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F007 - AuthService.RegisterOrganisationAsync

### 1) Function Header

| Field            | Value                                                                                                                                                          |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F007                                                                                                                                                           |
| Function Name    | RegisterOrganisationAsync                                                                                                                                      |
| Class Name       | AuthService                                                                                                                                                    |
| Method           | RegisterOrganisationAsync                                                                                                                                      |
| Requirement      | Register organisation account                                                                                                                                  |
| Description      | Validate 'Register organisation account' in AuthService.RegisterOrganisationAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                              |
| Passed           | 7                                                                                                                                                              |
| Failed           | 0                                                                                                                                                              |
| Untested         | 0                                                                                                                                                              |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                  |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC007-01 |
| Method Should Exist                                         | UTC007-02 |
| Source Should Contain Method Declaration                    | UTC007-03 |
| Return Case Set Should Be Valid                             | UTC007-04 |
| Log Message Case Set Should Be Valid                        | UTC007-05 |
| When Logger Used Should Follow Log Message Convention       | UTC007-06 |
| When Result Response Used Should Follow Response Convention | UTC007-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC007-01 | N                 | Type Should Exist                                           |
| UTC007-02 | N                 | Method Should Exist                                         |
| UTC007-03 | N                 | Source Should Contain Method Declaration                    |
| UTC007-04 | N                 | Return Case Set Should Be Valid                             |
| UTC007-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC007-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC007-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F007_AuthService_RegisterOrganisationAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F008 - AuthService.GoogleLoginAsync

### 1) Function Header

| Field            | Value                                                                                                                                |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F008                                                                                                                                 |
| Function Name    | GoogleLoginAsync                                                                                                                     |
| Class Name       | AuthService                                                                                                                          |
| Method           | GoogleLoginAsync                                                                                                                     |
| Requirement      | Google login                                                                                                                         |
| Description      | Validate 'Google login' in AuthService.GoogleLoginAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                    |
| Passed           | 7                                                                                                                                    |
| Failed           | 0                                                                                                                                    |
| Untested         | 0                                                                                                                                    |
| N/A/B            | N=7, A=0, B=0                                                                                                                        |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC008-01 |
| Method Should Exist                                         | UTC008-02 |
| Source Should Contain Method Declaration                    | UTC008-03 |
| Return Case Set Should Be Valid                             | UTC008-04 |
| Log Message Case Set Should Be Valid                        | UTC008-05 |
| When Logger Used Should Follow Log Message Convention       | UTC008-06 |
| When Result Response Used Should Follow Response Convention | UTC008-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result<LoginResponse>.Unauthorized("Invalid Google token")

#### Exception

- No dedicated exception case asserted.

#### Log message

- 2FA required for Google user: {Email}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC008-01 | N                 | Type Should Exist                                           |
| UTC008-02 | N                 | Method Should Exist                                         |
| UTC008-03 | N                 | Source Should Contain Method Declaration                    |
| UTC008-04 | N                 | Return Case Set Should Be Valid                             |
| UTC008-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC008-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC008-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F008_AuthService_GoogleLoginAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F009 - AuthService.LoginAsync

### 1) Function Header

| Field            | Value                                                                                                                                 |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F009                                                                                                                                  |
| Function Name    | LoginAsync                                                                                                                            |
| Class Name       | AuthService                                                                                                                           |
| Method           | LoginAsync                                                                                                                            |
| Requirement      | Login with password                                                                                                                   |
| Description      | Validate 'Login with password' in AuthService.LoginAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 9                                                                                                                                     |
| Passed           | 9                                                                                                                                     |
| Failed           | 0                                                                                                                                     |
| Untested         | 0                                                                                                                                     |
| N/A/B            | N=2, A=7, B=0                                                                                                                         |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item                                                                        | UTC Ref   |
| ------------------------------------------------------------------------------------- | --------- |
| MethodSignature Should Exist                                                          | UTC009-01 |
| Condition UserNotFoundOrDeleted Should ReturnUnauthorized InvalidEmailOrPassword      | UTC009-02 |
| Condition UserInactive Should ReturnUnauthorized DeactivatedMessage                   | UTC009-03 |
| Condition UserLockedOutOrSignInLockedOut Should ReturnUnauthorized LockMessage        | UTC009-04 |
| Condition EmailNotConfirmed Should ReturnUnauthorized NotAllowedMessage               | UTC009-05 |
| Condition OphthalmologistRejected Should ReturnUnauthorized CredentialRejectedMessage | UTC009-06 |
| Condition TwoFactorRequired Should LogInformation And ReturnTwoFactorRequired         | UTC009-07 |
| Condition SuccessfulPasswordLogin Should ReturnLoginResponseSuccess                   | UTC009-08 |
| Exception Should LogError And ReturnFailureMessage                                    | UTC009-09 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result<LoginResponse>.Unauthorized("Invalid email or password")
- Result<LoginResponse>.Unauthorized("Account is deactivated. Please contact support.")
- Result<LoginResponse>.Unauthorized("Account is temporarily locked due to multiple failed attempts. Please try again later.")
- Result<LoginResponse>.Unauthorized("Please confirm your email before logging in.")
- Result<LoginResponse>.Unauthorized("Your credential verification has been rejected. Please contact support for more information.")
- Result<LoginResponse>.Success(LoginResponse.TwoFactorRequired(user.Id))
- Result<LoginResponse>.Success(LoginResponse.Success(authResponse))
- Result<LoginResponse>.Failure("An error occurred during login")

#### Exception

- Exception Should LogError And ReturnFailureMessage

#### Log message

- \_logger.LogInformation("2FA required for user: {Email}", request.Email);
- \_logger.LogError(ex, "Error during login: {Email}", request.Email);

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                                           |
| --------- | ----------------- | ------------------------------------------------------------------------------------- |
| UTC009-01 | N                 | MethodSignature Should Exist                                                          |
| UTC009-02 | A                 | Condition UserNotFoundOrDeleted Should ReturnUnauthorized InvalidEmailOrPassword      |
| UTC009-03 | A                 | Condition UserInactive Should ReturnUnauthorized DeactivatedMessage                   |
| UTC009-04 | A                 | Condition UserLockedOutOrSignInLockedOut Should ReturnUnauthorized LockMessage        |
| UTC009-05 | A                 | Condition EmailNotConfirmed Should ReturnUnauthorized NotAllowedMessage               |
| UTC009-06 | A                 | Condition OphthalmologistRejected Should ReturnUnauthorized CredentialRejectedMessage |
| UTC009-07 | A                 | Condition TwoFactorRequired Should LogInformation And ReturnTwoFactorRequired         |
| UTC009-08 | N                 | Condition SuccessfulPasswordLogin Should ReturnLoginResponseSuccess                   |
| UTC009-09 | A                 | Exception Should LogError And ReturnFailureMessage                                    |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, A, A, A, A, A, A, N, A
- Passed/Failed: P, P, P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F009_AuthService_LoginAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F010 - AuthService.VerifyTwoFactorLoginAsync

### 1) Function Header

| Field            | Value                                                                                                                                             |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F010                                                                                                                                              |
| Function Name    | VerifyTwoFactorLoginAsync                                                                                                                         |
| Class Name       | AuthService                                                                                                                                       |
| Method           | VerifyTwoFactorLoginAsync                                                                                                                         |
| Requirement      | Verify 2FA login                                                                                                                                  |
| Description      | Validate 'Verify 2FA login' in AuthService.VerifyTwoFactorLoginAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                 |
| Passed           | 7                                                                                                                                                 |
| Failed           | 0                                                                                                                                                 |
| Untested         | 0                                                                                                                                                 |
| N/A/B            | N=7, A=0, B=0                                                                                                                                     |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC010-01 |
| Method Should Exist                                         | UTC010-02 |
| Source Should Contain Method Declaration                    | UTC010-03 |
| Return Case Set Should Be Valid                             | UTC010-04 |
| Log Message Case Set Should Be Valid                        | UTC010-05 |
| When Logger Used Should Follow Log Message Convention       | UTC010-06 |
| When Result Response Used Should Follow Response Convention | UTC010-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result<AuthResponse>.Unauthorized("User not found or inactive")

#### Exception

- No dedicated exception case asserted.

#### Log message

- Recovery code used for user: {UserId}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC010-01 | N                 | Type Should Exist                                           |
| UTC010-02 | N                 | Method Should Exist                                         |
| UTC010-03 | N                 | Source Should Contain Method Declaration                    |
| UTC010-04 | N                 | Return Case Set Should Be Valid                             |
| UTC010-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC010-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC010-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F010_AuthService_VerifyTwoFactorLoginAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F011 - AuthService.RefreshTokenAsync

### 1) Function Header

| Field            | Value                                                                                                                                       |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F011                                                                                                                                        |
| Function Name    | RefreshTokenAsync                                                                                                                           |
| Class Name       | AuthService                                                                                                                                 |
| Method           | RefreshTokenAsync                                                                                                                           |
| Requirement      | Refresh token flow                                                                                                                          |
| Description      | Validate 'Refresh token flow' in AuthService.RefreshTokenAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                           |
| Passed           | 7                                                                                                                                           |
| Failed           | 0                                                                                                                                           |
| Untested         | 0                                                                                                                                           |
| N/A/B            | N=7, A=0, B=0                                                                                                                               |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC011-01 |
| Method Should Exist                                         | UTC011-02 |
| Source Should Contain Method Declaration                    | UTC011-03 |
| Return Case Set Should Be Valid                             | UTC011-04 |
| Log Message Case Set Should Be Valid                        | UTC011-05 |
| When Logger Used Should Follow Log Message Convention       | UTC011-06 |
| When Result Response Used Should Follow Response Convention | UTC011-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result<AuthResponse>.Unauthorized("Invalid access token")

#### Exception

- No dedicated exception case asserted.

#### Log message

- Refresh token reuse detected for user {UserId}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC011-01 | N                 | Type Should Exist                                           |
| UTC011-02 | N                 | Method Should Exist                                         |
| UTC011-03 | N                 | Source Should Contain Method Declaration                    |
| UTC011-04 | N                 | Return Case Set Should Be Valid                             |
| UTC011-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC011-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC011-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F011_AuthService_RefreshTokenAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F012 - AuthService.LogoutAsync

### 1) Function Header

| Field            | Value                                                                                                                                   |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F012                                                                                                                                    |
| Function Name    | LogoutAsync                                                                                                                             |
| Class Name       | AuthService                                                                                                                             |
| Method           | LogoutAsync                                                                                                                             |
| Requirement      | Logout single device                                                                                                                    |
| Description      | Validate 'Logout single device' in AuthService.LogoutAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                       |
| Passed           | 7                                                                                                                                       |
| Failed           | 0                                                                                                                                       |
| Untested         | 0                                                                                                                                       |
| N/A/B            | N=7, A=0, B=0                                                                                                                           |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC012-01 |
| Method Should Exist                                         | UTC012-02 |
| Source Should Contain Method Declaration                    | UTC012-03 |
| Return Case Set Should Be Valid                             | UTC012-04 |
| Log Message Case Set Should Be Valid                        | UTC012-05 |
| When Logger Used Should Follow Log Message Convention       | UTC012-06 |
| When Result Response Used Should Follow Response Convention | UTC012-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result.Success()

#### Exception

- No dedicated exception case asserted.

#### Log message

- User logged out

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC012-01 | N                 | Type Should Exist                                           |
| UTC012-02 | N                 | Method Should Exist                                         |
| UTC012-03 | N                 | Source Should Contain Method Declaration                    |
| UTC012-04 | N                 | Return Case Set Should Be Valid                             |
| UTC012-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC012-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC012-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F012_AuthService_LogoutAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F013 - AuthService.LogoutAllAsync

### 1) Function Header

| Field            | Value                                                                                                                                    |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F013                                                                                                                                     |
| Function Name    | LogoutAllAsync                                                                                                                           |
| Class Name       | AuthService                                                                                                                              |
| Method           | LogoutAllAsync                                                                                                                           |
| Requirement      | Logout all devices                                                                                                                       |
| Description      | Validate 'Logout all devices' in AuthService.LogoutAllAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                        |
| Passed           | 7                                                                                                                                        |
| Failed           | 0                                                                                                                                        |
| Untested         | 0                                                                                                                                        |
| N/A/B            | N=7, A=0, B=0                                                                                                                            |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC013-01 |
| Method Should Exist                                         | UTC013-02 |
| Source Should Contain Method Declaration                    | UTC013-03 |
| Return Case Set Should Be Valid                             | UTC013-04 |
| Log Message Case Set Should Be Valid                        | UTC013-05 |
| When Logger Used Should Follow Log Message Convention       | UTC013-06 |
| When Result Response Used Should Follow Response Convention | UTC013-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result.Success()

#### Exception

- No dedicated exception case asserted.

#### Log message

- All tokens revoked for user: {UserId}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC013-01 | N                 | Type Should Exist                                           |
| UTC013-02 | N                 | Method Should Exist                                         |
| UTC013-03 | N                 | Source Should Contain Method Declaration                    |
| UTC013-04 | N                 | Return Case Set Should Be Valid                             |
| UTC013-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC013-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC013-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F013_AuthService_LogoutAllAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F014 - AuthService.ConfirmEmailAsync

### 1) Function Header

| Field            | Value                                                                                                                                  |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F014                                                                                                                                   |
| Function Name    | ConfirmEmailAsync                                                                                                                      |
| Class Name       | AuthService                                                                                                                            |
| Method           | ConfirmEmailAsync                                                                                                                      |
| Requirement      | Confirm email                                                                                                                          |
| Description      | Validate 'Confirm email' in AuthService.ConfirmEmailAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                      |
| Passed           | 7                                                                                                                                      |
| Failed           | 0                                                                                                                                      |
| Untested         | 0                                                                                                                                      |
| N/A/B            | N=7, A=0, B=0                                                                                                                          |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC014-01 |
| Method Should Exist                                         | UTC014-02 |
| Source Should Contain Method Declaration                    | UTC014-03 |
| Return Case Set Should Be Valid                             | UTC014-04 |
| Log Message Case Set Should Be Valid                        | UTC014-05 |
| When Logger Used Should Follow Log Message Convention       | UTC014-06 |
| When Result Response Used Should Follow Response Convention | UTC014-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result.Failure("Invalid user ID")

#### Exception

- No dedicated exception case asserted.

#### Log message

- Email confirmed for user: {UserId}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC014-01 | N                 | Type Should Exist                                           |
| UTC014-02 | N                 | Method Should Exist                                         |
| UTC014-03 | N                 | Source Should Contain Method Declaration                    |
| UTC014-04 | N                 | Return Case Set Should Be Valid                             |
| UTC014-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC014-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC014-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F014_AuthService_ConfirmEmailAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F015 - AuthService.ForgotPasswordAsync

### 1) Function Header

| Field            | Value                                                                                                                                      |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F015                                                                                                                                       |
| Function Name    | ForgotPasswordAsync                                                                                                                        |
| Class Name       | AuthService                                                                                                                                |
| Method           | ForgotPasswordAsync                                                                                                                        |
| Requirement      | Forgot password                                                                                                                            |
| Description      | Validate 'Forgot password' in AuthService.ForgotPasswordAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                          |
| Passed           | 7                                                                                                                                          |
| Failed           | 0                                                                                                                                          |
| Untested         | 0                                                                                                                                          |
| N/A/B            | N=7, A=0, B=0                                                                                                                              |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC015-01 |
| Method Should Exist                                         | UTC015-02 |
| Source Should Contain Method Declaration                    | UTC015-03 |
| Return Case Set Should Be Valid                             | UTC015-04 |
| Log Message Case Set Should Be Valid                        | UTC015-05 |
| When Logger Used Should Follow Log Message Convention       | UTC015-06 |
| When Result Response Used Should Follow Response Convention | UTC015-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- success to prevent email enumeration return Result.Success()

#### Exception

- No dedicated exception case asserted.

#### Log message

- Password reset requested for: {Email}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC015-01 | N                 | Type Should Exist                                           |
| UTC015-02 | N                 | Method Should Exist                                         |
| UTC015-03 | N                 | Source Should Contain Method Declaration                    |
| UTC015-04 | N                 | Return Case Set Should Be Valid                             |
| UTC015-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC015-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC015-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F015_AuthService_ForgotPasswordAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F016 - AuthService.ResetPasswordAsync

### 1) Function Header

| Field            | Value                                                                                                                                    |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F016                                                                                                                                     |
| Function Name    | ResetPasswordAsync                                                                                                                       |
| Class Name       | AuthService                                                                                                                              |
| Method           | ResetPasswordAsync                                                                                                                       |
| Requirement      | Reset password                                                                                                                           |
| Description      | Validate 'Reset password' in AuthService.ResetPasswordAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                        |
| Passed           | 7                                                                                                                                        |
| Failed           | 0                                                                                                                                        |
| Untested         | 0                                                                                                                                        |
| N/A/B            | N=7, A=0, B=0                                                                                                                            |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC016-01 |
| Method Should Exist                                         | UTC016-02 |
| Source Should Contain Method Declaration                    | UTC016-03 |
| Return Case Set Should Be Valid                             | UTC016-04 |
| Log Message Case Set Should Be Valid                        | UTC016-05 |
| When Logger Used Should Follow Log Message Convention       | UTC016-06 |
| When Result Response Used Should Follow Response Convention | UTC016-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result.Failure("Invalid user ID")

#### Exception

- No dedicated exception case asserted.

#### Log message

- Password reset for user: {UserId}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC016-01 | N                 | Type Should Exist                                           |
| UTC016-02 | N                 | Method Should Exist                                         |
| UTC016-03 | N                 | Source Should Contain Method Declaration                    |
| UTC016-04 | N                 | Return Case Set Should Be Valid                             |
| UTC016-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC016-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC016-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F016_AuthService_ResetPasswordAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F017 - AuthService.GetCurrentUserAsync

### 1) Function Header

| Field            | Value                                                                                                                                       |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F017                                                                                                                                        |
| Function Name    | GetCurrentUserAsync                                                                                                                         |
| Class Name       | AuthService                                                                                                                                 |
| Method           | GetCurrentUserAsync                                                                                                                         |
| Requirement      | Get current user                                                                                                                            |
| Description      | Validate 'Get current user' in AuthService.GetCurrentUserAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                           |
| Passed           | 7                                                                                                                                           |
| Failed           | 0                                                                                                                                           |
| Untested         | 0                                                                                                                                           |
| N/A/B            | N=7, A=0, B=0                                                                                                                               |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC017-01 |
| Method Should Exist                                         | UTC017-02 |
| Source Should Contain Method Declaration                    | UTC017-03 |
| Return Case Set Should Be Valid                             | UTC017-04 |
| Log Message Case Set Should Be Valid                        | UTC017-05 |
| When Logger Used Should Follow Log Message Convention       | UTC017-06 |
| When Result Response Used Should Follow Response Convention | UTC017-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result<UserInfoResponse>.Unauthorized("User not found")

#### Exception

- No dedicated exception case asserted.

#### Log message

- Error getting current user: {UserId}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC017-01 | N                 | Type Should Exist                                           |
| UTC017-02 | N                 | Method Should Exist                                         |
| UTC017-03 | N                 | Source Should Contain Method Declaration                    |
| UTC017-04 | N                 | Return Case Set Should Be Valid                             |
| UTC017-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC017-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC017-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F017_AuthService_GetCurrentUserAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F018 - AuthService.ResendConfirmationAsync

### 1) Function Header

| Field            | Value                                                                                                                                                    |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F018                                                                                                                                                     |
| Function Name    | ResendConfirmationAsync                                                                                                                                  |
| Class Name       | AuthService                                                                                                                                              |
| Method           | ResendConfirmationAsync                                                                                                                                  |
| Requirement      | Resend confirmation email                                                                                                                                |
| Description      | Validate 'Resend confirmation email' in AuthService.ResendConfirmationAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                        |
| Passed           | 7                                                                                                                                                        |
| Failed           | 0                                                                                                                                                        |
| Untested         | 0                                                                                                                                                        |
| N/A/B            | N=7, A=0, B=0                                                                                                                                            |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC018-01 |
| Method Should Exist                                         | UTC018-02 |
| Source Should Contain Method Declaration                    | UTC018-03 |
| Return Case Set Should Be Valid                             | UTC018-04 |
| Log Message Case Set Should Be Valid                        | UTC018-05 |
| When Logger Used Should Follow Log Message Convention       | UTC018-06 |
| When Result Response Used Should Follow Response Convention | UTC018-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result.Success()

#### Exception

- No dedicated exception case asserted.

#### Log message

- Confirmation email resent to: {Email}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC018-01 | N                 | Type Should Exist                                           |
| UTC018-02 | N                 | Method Should Exist                                         |
| UTC018-03 | N                 | Source Should Contain Method Declaration                    |
| UTC018-04 | N                 | Return Case Set Should Be Valid                             |
| UTC018-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC018-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC018-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F018_AuthService_ResendConfirmationAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F019 - IdentityService.CheckPasswordAsync

### 1) Function Header

| Field            | Value                                                                                                                                        |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F019                                                                                                                                         |
| Function Name    | CheckPasswordAsync                                                                                                                           |
| Class Name       | IdentityService                                                                                                                              |
| Method           | CheckPasswordAsync                                                                                                                           |
| Requirement      | Check password                                                                                                                               |
| Description      | Validate 'Check password' in IdentityService.CheckPasswordAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                            |
| Passed           | 7                                                                                                                                            |
| Failed           | 0                                                                                                                                            |
| Untested         | 0                                                                                                                                            |
| N/A/B            | N=7, A=0, B=0                                                                                                                                |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC019-01 |
| Method Should Exist                                         | UTC019-02 |
| Source Should Contain Method Declaration                    | UTC019-03 |
| Return Case Set Should Be Valid                             | UTC019-04 |
| Log Message Case Set Should Be Valid                        | UTC019-05 |
| When Logger Used Should Follow Log Message Convention       | UTC019-06 |
| When Result Response Used Should Follow Response Convention | UTC019-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- false

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC019-01 | N                 | Type Should Exist                                           |
| UTC019-02 | N                 | Method Should Exist                                         |
| UTC019-03 | N                 | Source Should Contain Method Declaration                    |
| UTC019-04 | N                 | Return Case Set Should Be Valid                             |
| UTC019-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC019-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC019-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F019_IdentityService_CheckPasswordAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F020 - IdentityService.GetUserByEmailAsync

### 1) Function Header

| Field            | Value                                                                                                                                            |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F020                                                                                                                                             |
| Function Name    | GetUserByEmailAsync                                                                                                                              |
| Class Name       | IdentityService                                                                                                                                  |
| Method           | GetUserByEmailAsync                                                                                                                              |
| Requirement      | Get user by email                                                                                                                                |
| Description      | Validate 'Get user by email' in IdentityService.GetUserByEmailAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                |
| Passed           | 7                                                                                                                                                |
| Failed           | 0                                                                                                                                                |
| Untested         | 0                                                                                                                                                |
| N/A/B            | N=7, A=0, B=0                                                                                                                                    |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC020-01 |
| Method Should Exist                                         | UTC020-02 |
| Source Should Contain Method Declaration                    | UTC020-03 |
| Return Case Set Should Be Valid                             | UTC020-04 |
| Log Message Case Set Should Be Valid                        | UTC020-05 |
| When Logger Used Should Follow Log Message Convention       | UTC020-06 |
| When Result Response Used Should Follow Response Convention | UTC020-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- user == null ? null : MapToDto(user)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC020-01 | N                 | Type Should Exist                                           |
| UTC020-02 | N                 | Method Should Exist                                         |
| UTC020-03 | N                 | Source Should Contain Method Declaration                    |
| UTC020-04 | N                 | Return Case Set Should Be Valid                             |
| UTC020-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC020-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC020-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F020_IdentityService_GetUserByEmailAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F021 - IdentityService.GetUserByIdAsync

### 1) Function Header

| Field            | Value                                                                                                                                      |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F021                                                                                                                                       |
| Function Name    | GetUserByIdAsync                                                                                                                           |
| Class Name       | IdentityService                                                                                                                            |
| Method           | GetUserByIdAsync                                                                                                                           |
| Requirement      | Get user by id                                                                                                                             |
| Description      | Validate 'Get user by id' in IdentityService.GetUserByIdAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                          |
| Passed           | 7                                                                                                                                          |
| Failed           | 0                                                                                                                                          |
| Untested         | 0                                                                                                                                          |
| N/A/B            | N=7, A=0, B=0                                                                                                                              |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC021-01 |
| Method Should Exist                                         | UTC021-02 |
| Source Should Contain Method Declaration                    | UTC021-03 |
| Return Case Set Should Be Valid                             | UTC021-04 |
| Log Message Case Set Should Be Valid                        | UTC021-05 |
| When Logger Used Should Follow Log Message Convention       | UTC021-06 |
| When Result Response Used Should Follow Response Convention | UTC021-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- user == null ? null : MapToDto(user)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC021-01 | N                 | Type Should Exist                                           |
| UTC021-02 | N                 | Method Should Exist                                         |
| UTC021-03 | N                 | Source Should Contain Method Declaration                    |
| UTC021-04 | N                 | Return Case Set Should Be Valid                             |
| UTC021-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC021-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC021-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F021_IdentityService_GetUserByIdAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F022 - IdentityService.IsPhoneNumberInUseByOrganizationAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                            |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F022                                                                                                                                                                             |
| Function Name    | IsPhoneNumberInUseByOrganizationAsync                                                                                                                                            |
| Class Name       | IdentityService                                                                                                                                                                  |
| Method           | IsPhoneNumberInUseByOrganizationAsync                                                                                                                                            |
| Requirement      | Check organization phone in use                                                                                                                                                  |
| Description      | Validate 'Check organization phone in use' in IdentityService.IsPhoneNumberInUseByOrganizationAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                                |
| Passed           | 7                                                                                                                                                                                |
| Failed           | 0                                                                                                                                                                                |
| Untested         | 0                                                                                                                                                                                |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                                    |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC022-01 |
| Method Should Exist                                         | UTC022-02 |
| Source Should Contain Method Declaration                    | UTC022-03 |
| Return Case Set Should Be Valid                             | UTC022-04 |
| Log Message Case Set Should Be Valid                        | UTC022-05 |
| When Logger Used Should Follow Log Message Convention       | UTC022-06 |
| When Result Response Used Should Follow Response Convention | UTC022-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- false

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC022-01 | N                 | Type Should Exist                                           |
| UTC022-02 | N                 | Method Should Exist                                         |
| UTC022-03 | N                 | Source Should Contain Method Declaration                    |
| UTC022-04 | N                 | Return Case Set Should Be Valid                             |
| UTC022-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC022-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC022-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F022_IdentityService_IsPhoneNumberInUseByOrganizationAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F023 - IdentityService.IsEmailConfirmedAsync

### 1) Function Header

| Field            | Value                                                                                                                                                  |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F023                                                                                                                                                   |
| Function Name    | IsEmailConfirmedAsync                                                                                                                                  |
| Class Name       | IdentityService                                                                                                                                        |
| Method           | IsEmailConfirmedAsync                                                                                                                                  |
| Requirement      | Check email confirmed                                                                                                                                  |
| Description      | Validate 'Check email confirmed' in IdentityService.IsEmailConfirmedAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                      |
| Passed           | 7                                                                                                                                                      |
| Failed           | 0                                                                                                                                                      |
| Untested         | 0                                                                                                                                                      |
| N/A/B            | N=7, A=0, B=0                                                                                                                                          |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC023-01 |
| Method Should Exist                                         | UTC023-02 |
| Source Should Contain Method Declaration                    | UTC023-03 |
| Return Case Set Should Be Valid                             | UTC023-04 |
| Log Message Case Set Should Be Valid                        | UTC023-05 |
| When Logger Used Should Follow Log Message Convention       | UTC023-06 |
| When Result Response Used Should Follow Response Convention | UTC023-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- user != null && user.EmailConfirmed

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC023-01 | N                 | Type Should Exist                                           |
| UTC023-02 | N                 | Method Should Exist                                         |
| UTC023-03 | N                 | Source Should Contain Method Declaration                    |
| UTC023-04 | N                 | Return Case Set Should Be Valid                             |
| UTC023-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC023-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC023-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F023_IdentityService_IsEmailConfirmedAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F024 - IdentityService.IsUserActiveAsync

### 1) Function Header

| Field            | Value                                                                                                                                          |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F024                                                                                                                                           |
| Function Name    | IsUserActiveAsync                                                                                                                              |
| Class Name       | IdentityService                                                                                                                                |
| Method           | IsUserActiveAsync                                                                                                                              |
| Requirement      | Check user active                                                                                                                              |
| Description      | Validate 'Check user active' in IdentityService.IsUserActiveAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                              |
| Passed           | 7                                                                                                                                              |
| Failed           | 0                                                                                                                                              |
| Untested         | 0                                                                                                                                              |
| N/A/B            | N=7, A=0, B=0                                                                                                                                  |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC024-01 |
| Method Should Exist                                         | UTC024-02 |
| Source Should Contain Method Declaration                    | UTC024-03 |
| Return Case Set Should Be Valid                             | UTC024-04 |
| Log Message Case Set Should Be Valid                        | UTC024-05 |
| When Logger Used Should Follow Log Message Convention       | UTC024-06 |
| When Result Response Used Should Follow Response Convention | UTC024-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- user is { IsActive: true, IsDeleted: false }

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC024-01 | N                 | Type Should Exist                                           |
| UTC024-02 | N                 | Method Should Exist                                         |
| UTC024-03 | N                 | Source Should Contain Method Declaration                    |
| UTC024-04 | N                 | Return Case Set Should Be Valid                             |
| UTC024-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC024-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC024-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F024_IdentityService_IsUserActiveAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F025 - IdentityService.GenerateEmailConfirmationTokenAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                            |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F025                                                                                                                                                                             |
| Function Name    | GenerateEmailConfirmationTokenAsync                                                                                                                                              |
| Class Name       | IdentityService                                                                                                                                                                  |
| Method           | GenerateEmailConfirmationTokenAsync                                                                                                                                              |
| Requirement      | Generate email confirmation token                                                                                                                                                |
| Description      | Validate 'Generate email confirmation token' in IdentityService.GenerateEmailConfirmationTokenAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                                |
| Passed           | 7                                                                                                                                                                                |
| Failed           | 0                                                                                                                                                                                |
| Untested         | 0                                                                                                                                                                                |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                                    |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC025-01 |
| Method Should Exist                                         | UTC025-02 |
| Source Should Contain Method Declaration                    | UTC025-03 |
| Return Case Set Should Be Valid                             | UTC025-04 |
| Log Message Case Set Should Be Valid                        | UTC025-05 |
| When Logger Used Should Follow Log Message Convention       | UTC025-06 |
| When Result Response Used Should Follow Response Convention | UTC025-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- await \_userManager.GenerateEmailConfirmationTokenAsync(user)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC025-01 | N                 | Type Should Exist                                           |
| UTC025-02 | N                 | Method Should Exist                                         |
| UTC025-03 | N                 | Source Should Contain Method Declaration                    |
| UTC025-04 | N                 | Return Case Set Should Be Valid                             |
| UTC025-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC025-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC025-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F025_IdentityService_GenerateEmailConfirmationTokenAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F026 - IdentityService.GeneratePasswordResetTokenAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                    |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F026                                                                                                                                                                     |
| Function Name    | GeneratePasswordResetTokenAsync                                                                                                                                          |
| Class Name       | IdentityService                                                                                                                                                          |
| Method           | GeneratePasswordResetTokenAsync                                                                                                                                          |
| Requirement      | Generate password reset token                                                                                                                                            |
| Description      | Validate 'Generate password reset token' in IdentityService.GeneratePasswordResetTokenAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                        |
| Passed           | 7                                                                                                                                                                        |
| Failed           | 0                                                                                                                                                                        |
| Untested         | 0                                                                                                                                                                        |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                            |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC026-01 |
| Method Should Exist                                         | UTC026-02 |
| Source Should Contain Method Declaration                    | UTC026-03 |
| Return Case Set Should Be Valid                             | UTC026-04 |
| Log Message Case Set Should Be Valid                        | UTC026-05 |
| When Logger Used Should Follow Log Message Convention       | UTC026-06 |
| When Result Response Used Should Follow Response Convention | UTC026-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- await \_userManager.GeneratePasswordResetTokenAsync(user)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC026-01 | N                 | Type Should Exist                                           |
| UTC026-02 | N                 | Method Should Exist                                         |
| UTC026-03 | N                 | Source Should Contain Method Declaration                    |
| UTC026-04 | N                 | Return Case Set Should Be Valid                             |
| UTC026-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC026-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC026-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F026_IdentityService_GeneratePasswordResetTokenAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F027 - IdentityService.GetUserRolesAsync

### 1) Function Header

| Field            | Value                                                                                                                                       |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F027                                                                                                                                        |
| Function Name    | GetUserRolesAsync                                                                                                                           |
| Class Name       | IdentityService                                                                                                                             |
| Method           | GetUserRolesAsync                                                                                                                           |
| Requirement      | Get user roles                                                                                                                              |
| Description      | Validate 'Get user roles' in IdentityService.GetUserRolesAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                           |
| Passed           | 7                                                                                                                                           |
| Failed           | 0                                                                                                                                           |
| Untested         | 0                                                                                                                                           |
| N/A/B            | N=7, A=0, B=0                                                                                                                               |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC027-01 |
| Method Should Exist                                         | UTC027-02 |
| Source Should Contain Method Declaration                    | UTC027-03 |
| Return Case Set Should Be Valid                             | UTC027-04 |
| Log Message Case Set Should Be Valid                        | UTC027-05 |
| When Logger Used Should Follow Log Message Convention       | UTC027-06 |
| When Result Response Used Should Follow Response Convention | UTC027-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Array.Empty<string>()

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC027-01 | N                 | Type Should Exist                                           |
| UTC027-02 | N                 | Method Should Exist                                         |
| UTC027-03 | N                 | Source Should Contain Method Declaration                    |
| UTC027-04 | N                 | Return Case Set Should Be Valid                             |
| UTC027-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC027-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC027-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F027_IdentityService_GetUserRolesAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F028 - IdentityService.IsInRoleAsync

### 1) Function Header

| Field            | Value                                                                                                                                       |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F028                                                                                                                                        |
| Function Name    | IsInRoleAsync                                                                                                                               |
| Class Name       | IdentityService                                                                                                                             |
| Method           | IsInRoleAsync                                                                                                                               |
| Requirement      | Check user in role                                                                                                                          |
| Description      | Validate 'Check user in role' in IdentityService.IsInRoleAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                           |
| Passed           | 7                                                                                                                                           |
| Failed           | 0                                                                                                                                           |
| Untested         | 0                                                                                                                                           |
| N/A/B            | N=7, A=0, B=0                                                                                                                               |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC028-01 |
| Method Should Exist                                         | UTC028-02 |
| Source Should Contain Method Declaration                    | UTC028-03 |
| Return Case Set Should Be Valid                             | UTC028-04 |
| Log Message Case Set Should Be Valid                        | UTC028-05 |
| When Logger Used Should Follow Log Message Convention       | UTC028-06 |
| When Result Response Used Should Follow Response Convention | UTC028-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- false

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC028-01 | N                 | Type Should Exist                                           |
| UTC028-02 | N                 | Method Should Exist                                         |
| UTC028-03 | N                 | Source Should Contain Method Declaration                    |
| UTC028-04 | N                 | Return Case Set Should Be Valid                             |
| UTC028-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC028-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC028-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F028_IdentityService_IsInRoleAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F029 - IdentityService.GetUserIdsByRoleAndOrganizationAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                        |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F029                                                                                                                                                                         |
| Function Name    | GetUserIdsByRoleAndOrganizationAsync                                                                                                                                         |
| Class Name       | IdentityService                                                                                                                                                              |
| Method           | GetUserIdsByRoleAndOrganizationAsync                                                                                                                                         |
| Requirement      | Get user ids by role and org                                                                                                                                                 |
| Description      | Validate 'Get user ids by role and org' in IdentityService.GetUserIdsByRoleAndOrganizationAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                            |
| Passed           | 7                                                                                                                                                                            |
| Failed           | 0                                                                                                                                                                            |
| Untested         | 0                                                                                                                                                                            |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                                |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC029-01 |
| Method Should Exist                                         | UTC029-02 |
| Source Should Contain Method Declaration                    | UTC029-03 |
| Return Case Set Should Be Valid                             | UTC029-04 |
| Log Message Case Set Should Be Valid                        | UTC029-05 |
| When Logger Used Should Follow Log Message Convention       | UTC029-06 |
| When Result Response Used Should Follow Response Convention | UTC029-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- usersInRole .Where(u => u.OrganizationId == organizationId && u.IsActive && !u.IsDeleted) .Select(u => u.Id) .ToList()

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC029-01 | N                 | Type Should Exist                                           |
| UTC029-02 | N                 | Method Should Exist                                         |
| UTC029-03 | N                 | Source Should Contain Method Declaration                    |
| UTC029-04 | N                 | Return Case Set Should Be Valid                             |
| UTC029-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC029-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC029-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F029_IdentityService_GetUserIdsByRoleAndOrganizationAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F030 - IdentityService.UpdateLastLoginAsync

### 1) Function Header

| Field            | Value                                                                                                                                                   |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F030                                                                                                                                                    |
| Function Name    | UpdateLastLoginAsync                                                                                                                                    |
| Class Name       | IdentityService                                                                                                                                         |
| Method           | UpdateLastLoginAsync                                                                                                                                    |
| Requirement      | Update last login async                                                                                                                                 |
| Description      | Validate 'Update last login async' in IdentityService.UpdateLastLoginAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                       |
| Passed           | 7                                                                                                                                                       |
| Failed           | 0                                                                                                                                                       |
| Untested         | 0                                                                                                                                                       |
| N/A/B            | N=7, A=0, B=0                                                                                                                                           |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC030-01 |
| Method Should Exist                                         | UTC030-02 |
| Source Should Contain Method Declaration                    | UTC030-03 |
| Return Case Set Should Be Valid                             | UTC030-04 |
| Log Message Case Set Should Be Valid                        | UTC030-05 |
| When Logger Used Should Follow Log Message Convention       | UTC030-06 |
| When Result Response Used Should Follow Response Convention | UTC030-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC030-01 | N                 | Type Should Exist                                           |
| UTC030-02 | N                 | Method Should Exist                                         |
| UTC030-03 | N                 | Source Should Contain Method Declaration                    |
| UTC030-04 | N                 | Return Case Set Should Be Valid                             |
| UTC030-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC030-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC030-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F030_IdentityService_UpdateLastLoginAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F031 - IdentityService.IsTwoFactorEnabledAsync

### 1) Function Header

| Field            | Value                                                                                                                                                |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F031                                                                                                                                                 |
| Function Name    | IsTwoFactorEnabledAsync                                                                                                                              |
| Class Name       | IdentityService                                                                                                                                      |
| Method           | IsTwoFactorEnabledAsync                                                                                                                              |
| Requirement      | Check 2FA enabled                                                                                                                                    |
| Description      | Validate 'Check 2FA enabled' in IdentityService.IsTwoFactorEnabledAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                    |
| Passed           | 7                                                                                                                                                    |
| Failed           | 0                                                                                                                                                    |
| Untested         | 0                                                                                                                                                    |
| N/A/B            | N=7, A=0, B=0                                                                                                                                        |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC031-01 |
| Method Should Exist                                         | UTC031-02 |
| Source Should Contain Method Declaration                    | UTC031-03 |
| Return Case Set Should Be Valid                             | UTC031-04 |
| Log Message Case Set Should Be Valid                        | UTC031-05 |
| When Logger Used Should Follow Log Message Convention       | UTC031-06 |
| When Result Response Used Should Follow Response Convention | UTC031-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- user != null && await \_userManager.GetTwoFactorEnabledAsync(user)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC031-01 | N                 | Type Should Exist                                           |
| UTC031-02 | N                 | Method Should Exist                                         |
| UTC031-03 | N                 | Source Should Contain Method Declaration                    |
| UTC031-04 | N                 | Return Case Set Should Be Valid                             |
| UTC031-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC031-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC031-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F031_IdentityService_IsTwoFactorEnabledAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F032 - IdentityService.GetAuthenticatorKeyAsync

### 1) Function Header

| Field            | Value                                                                                                                                                     |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F032                                                                                                                                                      |
| Function Name    | GetAuthenticatorKeyAsync                                                                                                                                  |
| Class Name       | IdentityService                                                                                                                                           |
| Method           | GetAuthenticatorKeyAsync                                                                                                                                  |
| Requirement      | Get authenticator key                                                                                                                                     |
| Description      | Validate 'Get authenticator key' in IdentityService.GetAuthenticatorKeyAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                         |
| Passed           | 7                                                                                                                                                         |
| Failed           | 0                                                                                                                                                         |
| Untested         | 0                                                                                                                                                         |
| N/A/B            | N=7, A=0, B=0                                                                                                                                             |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC032-01 |
| Method Should Exist                                         | UTC032-02 |
| Source Should Contain Method Declaration                    | UTC032-03 |
| Return Case Set Should Be Valid                             | UTC032-04 |
| Log Message Case Set Should Be Valid                        | UTC032-05 |
| When Logger Used Should Follow Log Message Convention       | UTC032-06 |
| When Result Response Used Should Follow Response Convention | UTC032-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- null

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC032-01 | N                 | Type Should Exist                                           |
| UTC032-02 | N                 | Method Should Exist                                         |
| UTC032-03 | N                 | Source Should Contain Method Declaration                    |
| UTC032-04 | N                 | Return Case Set Should Be Valid                             |
| UTC032-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC032-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC032-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F032_IdentityService_GetAuthenticatorKeyAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F033 - IdentityService.GetOrCreateAuthenticatorKeyAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                       |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F033                                                                                                                                                                        |
| Function Name    | GetOrCreateAuthenticatorKeyAsync                                                                                                                                            |
| Class Name       | IdentityService                                                                                                                                                             |
| Method           | GetOrCreateAuthenticatorKeyAsync                                                                                                                                            |
| Requirement      | Get or create authenticator key                                                                                                                                             |
| Description      | Validate 'Get or create authenticator key' in IdentityService.GetOrCreateAuthenticatorKeyAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                           |
| Passed           | 7                                                                                                                                                                           |
| Failed           | 0                                                                                                                                                                           |
| Untested         | 0                                                                                                                                                                           |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                               |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC033-01 |
| Method Should Exist                                         | UTC033-02 |
| Source Should Contain Method Declaration                    | UTC033-03 |
| Return Case Set Should Be Valid                             | UTC033-04 |
| Log Message Case Set Should Be Valid                        | UTC033-05 |
| When Logger Used Should Follow Log Message Convention       | UTC033-06 |
| When Result Response Used Should Follow Response Convention | UTC033-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- key ?? throw new InvalidOperationException("Failed to generate authenticator key")

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC033-01 | N                 | Type Should Exist                                           |
| UTC033-02 | N                 | Method Should Exist                                         |
| UTC033-03 | N                 | Source Should Contain Method Declaration                    |
| UTC033-04 | N                 | Return Case Set Should Be Valid                             |
| UTC033-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC033-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC033-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F033_IdentityService_GetOrCreateAuthenticatorKeyAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F034 - IdentityService.VerifyTwoFactorCodeAsync

### 1) Function Header

| Field            | Value                                                                                                                                               |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F034                                                                                                                                                |
| Function Name    | VerifyTwoFactorCodeAsync                                                                                                                            |
| Class Name       | IdentityService                                                                                                                                     |
| Method           | VerifyTwoFactorCodeAsync                                                                                                                            |
| Requirement      | Verify 2FA code                                                                                                                                     |
| Description      | Validate 'Verify 2FA code' in IdentityService.VerifyTwoFactorCodeAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                   |
| Passed           | 7                                                                                                                                                   |
| Failed           | 0                                                                                                                                                   |
| Untested         | 0                                                                                                                                                   |
| N/A/B            | N=7, A=0, B=0                                                                                                                                       |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC034-01 |
| Method Should Exist                                         | UTC034-02 |
| Source Should Contain Method Declaration                    | UTC034-03 |
| Return Case Set Should Be Valid                             | UTC034-04 |
| Log Message Case Set Should Be Valid                        | UTC034-05 |
| When Logger Used Should Follow Log Message Convention       | UTC034-06 |
| When Result Response Used Should Follow Response Convention | UTC034-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- false

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC034-01 | N                 | Type Should Exist                                           |
| UTC034-02 | N                 | Method Should Exist                                         |
| UTC034-03 | N                 | Source Should Contain Method Declaration                    |
| UTC034-04 | N                 | Return Case Set Should Be Valid                             |
| UTC034-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC034-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC034-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F034_IdentityService_VerifyTwoFactorCodeAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F035 - IdentityService.GenerateNewRecoveryCodesAsync

### 1) Function Header

| Field            | Value                                                                                                                                                            |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F035                                                                                                                                                             |
| Function Name    | GenerateNewRecoveryCodesAsync                                                                                                                                    |
| Class Name       | IdentityService                                                                                                                                                  |
| Method           | GenerateNewRecoveryCodesAsync                                                                                                                                    |
| Requirement      | Generate recovery codes                                                                                                                                          |
| Description      | Validate 'Generate recovery codes' in IdentityService.GenerateNewRecoveryCodesAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                |
| Passed           | 7                                                                                                                                                                |
| Failed           | 0                                                                                                                                                                |
| Untested         | 0                                                                                                                                                                |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                    |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC035-01 |
| Method Should Exist                                         | UTC035-02 |
| Source Should Contain Method Declaration                    | UTC035-03 |
| Return Case Set Should Be Valid                             | UTC035-04 |
| Log Message Case Set Should Be Valid                        | UTC035-05 |
| When Logger Used Should Follow Log Message Convention       | UTC035-06 |
| When Result Response Used Should Follow Response Convention | UTC035-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- codes?.ToArray() ?? Array.Empty<string>()

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC035-01 | N                 | Type Should Exist                                           |
| UTC035-02 | N                 | Method Should Exist                                         |
| UTC035-03 | N                 | Source Should Contain Method Declaration                    |
| UTC035-04 | N                 | Return Case Set Should Be Valid                             |
| UTC035-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC035-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC035-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F035_IdentityService_GenerateNewRecoveryCodesAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F036 - IdentityService.GetRecoveryCodesCountAsync

### 1) Function Header

| Field            | Value                                                                                                                                                         |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F036                                                                                                                                                          |
| Function Name    | GetRecoveryCodesCountAsync                                                                                                                                    |
| Class Name       | IdentityService                                                                                                                                               |
| Method           | GetRecoveryCodesCountAsync                                                                                                                                    |
| Requirement      | Get recovery code count                                                                                                                                       |
| Description      | Validate 'Get recovery code count' in IdentityService.GetRecoveryCodesCountAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                             |
| Passed           | 7                                                                                                                                                             |
| Failed           | 0                                                                                                                                                             |
| Untested         | 0                                                                                                                                                             |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                 |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC036-01 |
| Method Should Exist                                         | UTC036-02 |
| Source Should Contain Method Declaration                    | UTC036-03 |
| Return Case Set Should Be Valid                             | UTC036-04 |
| Log Message Case Set Should Be Valid                        | UTC036-05 |
| When Logger Used Should Follow Log Message Convention       | UTC036-06 |
| When Result Response Used Should Follow Response Convention | UTC036-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- 0

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC036-01 | N                 | Type Should Exist                                           |
| UTC036-02 | N                 | Method Should Exist                                         |
| UTC036-03 | N                 | Source Should Contain Method Declaration                    |
| UTC036-04 | N                 | Return Case Set Should Be Valid                             |
| UTC036-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC036-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC036-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F036_IdentityService_GetRecoveryCodesCountAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F037 - IdentityService.GenerateAuthenticatorUri

### 1) Function Header

| Field            | Value                                                                                                                                                          |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F037                                                                                                                                                           |
| Function Name    | GenerateAuthenticatorUri                                                                                                                                       |
| Class Name       | IdentityService                                                                                                                                                |
| Method           | GenerateAuthenticatorUri                                                                                                                                       |
| Requirement      | Generate authenticator URI                                                                                                                                     |
| Description      | Validate 'Generate authenticator URI' in IdentityService.GenerateAuthenticatorUri, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                              |
| Passed           | 7                                                                                                                                                              |
| Failed           | 0                                                                                                                                                              |
| Untested         | 0                                                                                                                                                              |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                  |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC037-01 |
| Method Should Exist                                         | UTC037-02 |
| Source Should Contain Method Declaration                    | UTC037-03 |
| Return Case Set Should Be Valid                             | UTC037-04 |
| Log Message Case Set Should Be Valid                        | UTC037-05 |
| When Logger Used Should Follow Log Message Convention       | UTC037-06 |
| When Result Response Used Should Follow Response Convention | UTC037-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- $"otpauth://totp/{UrlEncoder.Default.Encode(AuthenticatorIssuer)}:{UrlEncoder.Default.Encode(email)}" + $"?secret={sharedKey}" + $"&issuer={UrlEncoder.Default.Encode(AuthenticatorIssuer)}" + "&digits=6"

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC037-01 | N                 | Type Should Exist                                           |
| UTC037-02 | N                 | Method Should Exist                                         |
| UTC037-03 | N                 | Source Should Contain Method Declaration                    |
| UTC037-04 | N                 | Return Case Set Should Be Valid                             |
| UTC037-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC037-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC037-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F037_IdentityService_GenerateAuthenticatorUri_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F038 - IdentityService.FormatAuthenticatorKey

### 1) Function Header

| Field            | Value                                                                                                                                                      |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F038                                                                                                                                                       |
| Function Name    | FormatAuthenticatorKey                                                                                                                                     |
| Class Name       | IdentityService                                                                                                                                            |
| Method           | FormatAuthenticatorKey                                                                                                                                     |
| Requirement      | Format authenticator key                                                                                                                                   |
| Description      | Validate 'Format authenticator key' in IdentityService.FormatAuthenticatorKey, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                          |
| Passed           | 7                                                                                                                                                          |
| Failed           | 0                                                                                                                                                          |
| Untested         | 0                                                                                                                                                          |
| N/A/B            | N=7, A=0, B=0                                                                                                                                              |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC038-01 |
| Method Should Exist                                         | UTC038-02 |
| Source Should Contain Method Declaration                    | UTC038-03 |
| Return Case Set Should Be Valid                             | UTC038-04 |
| Log Message Case Set Should Be Valid                        | UTC038-05 |
| When Logger Used Should Follow Log Message Convention       | UTC038-06 |
| When Result Response Used Should Follow Response Convention | UTC038-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- result.ToString().ToUpperInvariant()

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC038-01 | N                 | Type Should Exist                                           |
| UTC038-02 | N                 | Method Should Exist                                         |
| UTC038-03 | N                 | Source Should Contain Method Declaration                    |
| UTC038-04 | N                 | Return Case Set Should Be Valid                             |
| UTC038-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC038-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC038-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F038_IdentityService_FormatAuthenticatorKey_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F039 - IdentityService.GetUserMetricsAsync

### 1) Function Header

| Field            | Value                                                                                                                                           |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F039                                                                                                                                            |
| Function Name    | GetUserMetricsAsync                                                                                                                             |
| Class Name       | IdentityService                                                                                                                                 |
| Method           | GetUserMetricsAsync                                                                                                                             |
| Requirement      | Get user metrics                                                                                                                                |
| Description      | Validate 'Get user metrics' in IdentityService.GetUserMetricsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                               |
| Passed           | 7                                                                                                                                               |
| Failed           | 0                                                                                                                                               |
| Untested         | 0                                                                                                                                               |
| N/A/B            | N=7, A=0, B=0                                                                                                                                   |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC039-01 |
| Method Should Exist                                         | UTC039-02 |
| Source Should Contain Method Declaration                    | UTC039-03 |
| Return Case Set Should Be Valid                             | UTC039-04 |
| Log Message Case Set Should Be Valid                        | UTC039-05 |
| When Logger Used Should Follow Log Message Convention       | UTC039-06 |
| When Result Response Used Should Follow Response Convention | UTC039-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new UserMetricsDto( totalUsers, Math.Round(totalUsersChange, 1), activeDoctors, 0, // Would need historical data for change 0, // Would need screening data - passed separately 0, // Would need screening data pendingApprovals )

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC039-01 | N                 | Type Should Exist                                           |
| UTC039-02 | N                 | Method Should Exist                                         |
| UTC039-03 | N                 | Source Should Contain Method Declaration                    |
| UTC039-04 | N                 | Return Case Set Should Be Valid                             |
| UTC039-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC039-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC039-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F039_IdentityService_GetUserMetricsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F040 - IdentityService.GetUsersInRoleCountAsync

### 1) Function Header

| Field            | Value                                                                                                                                                       |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F040                                                                                                                                                        |
| Function Name    | GetUsersInRoleCountAsync                                                                                                                                    |
| Class Name       | IdentityService                                                                                                                                             |
| Method           | GetUsersInRoleCountAsync                                                                                                                                    |
| Requirement      | Get users in role count                                                                                                                                     |
| Description      | Validate 'Get users in role count' in IdentityService.GetUsersInRoleCountAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                           |
| Passed           | 7                                                                                                                                                           |
| Failed           | 0                                                                                                                                                           |
| Untested         | 0                                                                                                                                                           |
| N/A/B            | N=7, A=0, B=0                                                                                                                                               |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC040-01 |
| Method Should Exist                                         | UTC040-02 |
| Source Should Contain Method Declaration                    | UTC040-03 |
| Return Case Set Should Be Valid                             | UTC040-04 |
| Log Message Case Set Should Be Valid                        | UTC040-05 |
| When Logger Used Should Follow Log Message Convention       | UTC040-06 |
| When Result Response Used Should Follow Response Convention | UTC040-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- activeOnly ? usersInRole.Count(u => u.IsActive && !u.IsDeleted) : usersInRole.Count(u => !u.IsDeleted)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC040-01 | N                 | Type Should Exist                                           |
| UTC040-02 | N                 | Method Should Exist                                         |
| UTC040-03 | N                 | Source Should Contain Method Declaration                    |
| UTC040-04 | N                 | Return Case Set Should Be Valid                             |
| UTC040-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC040-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC040-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F040_IdentityService_GetUsersInRoleCountAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F041 - IdentityService.GetPendingApprovalsCountAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F041                                                                                                                                                                 |
| Function Name    | GetPendingApprovalsCountAsync                                                                                                                                        |
| Class Name       | IdentityService                                                                                                                                                      |
| Method           | GetPendingApprovalsCountAsync                                                                                                                                        |
| Requirement      | Get pending approvals count                                                                                                                                          |
| Description      | Validate 'Get pending approvals count' in IdentityService.GetPendingApprovalsCountAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                    |
| Passed           | 7                                                                                                                                                                    |
| Failed           | 0                                                                                                                                                                    |
| Untested         | 0                                                                                                                                                                    |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                        |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC041-01 |
| Method Should Exist                                         | UTC041-02 |
| Source Should Contain Method Declaration                    | UTC041-03 |
| Return Case Set Should Be Valid                             | UTC041-04 |
| Log Message Case Set Should Be Valid                        | UTC041-05 |
| When Logger Used Should Follow Log Message Convention       | UTC041-06 |
| When Result Response Used Should Follow Response Convention | UTC041-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- await \_userManager.Users .CountAsync(u => !u.IsDeleted && (!u.EmailConfirmed || !u.IsActive), cancellationToken)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC041-01 | N                 | Type Should Exist                                           |
| UTC041-02 | N                 | Method Should Exist                                         |
| UTC041-03 | N                 | Source Should Contain Method Declaration                    |
| UTC041-04 | N                 | Return Case Set Should Be Valid                             |
| UTC041-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC041-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC041-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F041_IdentityService_GetPendingApprovalsCountAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F042 - IdentityService.GetUserDetailsAsync

### 1) Function Header

| Field            | Value                                                                                                                                           |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F042                                                                                                                                            |
| Function Name    | GetUserDetailsAsync                                                                                                                             |
| Class Name       | IdentityService                                                                                                                                 |
| Method           | GetUserDetailsAsync                                                                                                                             |
| Requirement      | Get user details                                                                                                                                |
| Description      | Validate 'Get user details' in IdentityService.GetUserDetailsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                               |
| Passed           | 7                                                                                                                                               |
| Failed           | 0                                                                                                                                               |
| Untested         | 0                                                                                                                                               |
| N/A/B            | N=7, A=0, B=0                                                                                                                                   |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC042-01 |
| Method Should Exist                                         | UTC042-02 |
| Source Should Contain Method Declaration                    | UTC042-03 |
| Return Case Set Should Be Valid                             | UTC042-04 |
| Log Message Case Set Should Be Valid                        | UTC042-05 |
| When Logger Used Should Follow Log Message Convention       | UTC042-06 |
| When Result Response Used Should Follow Response Convention | UTC042-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- null

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC042-01 | N                 | Type Should Exist                                           |
| UTC042-02 | N                 | Method Should Exist                                         |
| UTC042-03 | N                 | Source Should Contain Method Declaration                    |
| UTC042-04 | N                 | Return Case Set Should Be Valid                             |
| UTC042-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC042-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC042-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F042_IdentityService_GetUserDetailsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F043 - RefreshToken.Create

### 1) Function Header

| Field            | Value                                                                                                                                      |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F043                                                                                                                                       |
| Function Name    | Create                                                                                                                                     |
| Class Name       | RefreshToken                                                                                                                               |
| Method           | Create                                                                                                                                     |
| Requirement      | Create refresh token entity                                                                                                                |
| Description      | Validate 'Create refresh token entity' in RefreshToken.Create, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                          |
| Passed           | 7                                                                                                                                          |
| Failed           | 0                                                                                                                                          |
| Untested         | 0                                                                                                                                          |
| N/A/B            | N=7, A=0, B=0                                                                                                                              |

### 2) Condition + Precondition

- Precondition: RefreshToken entity is initialized with valid TokenHash, JwtId, and UserId.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC043-01 |
| Method Should Exist                                         | UTC043-02 |
| Source Should Contain Method Declaration                    | UTC043-03 |
| Return Case Set Should Be Valid                             | UTC043-04 |
| Log Message Case Set Should Be Valid                        | UTC043-05 |
| When Logger Used Should Follow Log Message Convention       | UTC043-06 |
| When Result Response Used Should Follow Response Convention | UTC043-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new RefreshToken { UserId = userId, TokenHash = tokenHash, JwtId = jwtId, ExpiresAt = DateTime.UtcNow.AddDays(expiryDays), DeviceInfo = deviceInfo, IpAddress = ipAddress }

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC043-01 | N                 | Type Should Exist                                           |
| UTC043-02 | N                 | Method Should Exist                                         |
| UTC043-03 | N                 | Source Should Contain Method Declaration                    |
| UTC043-04 | N                 | Return Case Set Should Be Valid                             |
| UTC043-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC043-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC043-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F043_RefreshToken_Create_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F044 - RefreshToken.MarkAsUsed

### 1) Function Header

| Field            | Value                                                                                                                                      |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F044                                                                                                                                       |
| Function Name    | MarkAsUsed                                                                                                                                 |
| Class Name       | RefreshToken                                                                                                                               |
| Method           | MarkAsUsed                                                                                                                                 |
| Requirement      | Mark refresh token used                                                                                                                    |
| Description      | Validate 'Mark refresh token used' in RefreshToken.MarkAsUsed, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                          |
| Passed           | 7                                                                                                                                          |
| Failed           | 0                                                                                                                                          |
| Untested         | 0                                                                                                                                          |
| N/A/B            | N=7, A=0, B=0                                                                                                                              |

### 2) Condition + Precondition

- Precondition: RefreshToken entity is initialized with valid TokenHash, JwtId, and UserId.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC044-01 |
| Method Should Exist                                         | UTC044-02 |
| Source Should Contain Method Declaration                    | UTC044-03 |
| Return Case Set Should Be Valid                             | UTC044-04 |
| Log Message Case Set Should Be Valid                        | UTC044-05 |
| When Logger Used Should Follow Log Message Convention       | UTC044-06 |
| When Result Response Used Should Follow Response Convention | UTC044-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC044-01 | N                 | Type Should Exist                                           |
| UTC044-02 | N                 | Method Should Exist                                         |
| UTC044-03 | N                 | Source Should Contain Method Declaration                    |
| UTC044-04 | N                 | Return Case Set Should Be Valid                             |
| UTC044-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC044-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC044-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F044_RefreshToken_MarkAsUsed_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F045 - RefreshToken.Revoke

### 1) Function Header

| Field            | Value                                                                                                                               |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F045                                                                                                                                |
| Function Name    | Revoke                                                                                                                              |
| Class Name       | RefreshToken                                                                                                                        |
| Method           | Revoke                                                                                                                              |
| Requirement      | Revoke refresh token                                                                                                                |
| Description      | Validate 'Revoke refresh token' in RefreshToken.Revoke, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                   |
| Passed           | 7                                                                                                                                   |
| Failed           | 0                                                                                                                                   |
| Untested         | 0                                                                                                                                   |
| N/A/B            | N=7, A=0, B=0                                                                                                                       |

### 2) Condition + Precondition

- Precondition: RefreshToken entity is initialized with valid TokenHash, JwtId, and UserId.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC045-01 |
| Method Should Exist                                         | UTC045-02 |
| Source Should Contain Method Declaration                    | UTC045-03 |
| Return Case Set Should Be Valid                             | UTC045-04 |
| Log Message Case Set Should Be Valid                        | UTC045-05 |
| When Logger Used Should Follow Log Message Convention       | UTC045-06 |
| When Result Response Used Should Follow Response Convention | UTC045-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC045-01 | N                 | Type Should Exist                                           |
| UTC045-02 | N                 | Method Should Exist                                         |
| UTC045-03 | N                 | Source Should Contain Method Declaration                    |
| UTC045-04 | N                 | Return Case Set Should Be Valid                             |
| UTC045-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC045-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC045-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F045_RefreshToken_Revoke_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F046 - RefreshTokenService.CreateRefreshTokenAsync

### 1) Function Header

| Field            | Value                                                                                                                                                              |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F046                                                                                                                                                               |
| Function Name    | CreateRefreshTokenAsync                                                                                                                                            |
| Class Name       | RefreshTokenService                                                                                                                                                |
| Method           | CreateRefreshTokenAsync                                                                                                                                            |
| Requirement      | Create refresh token record                                                                                                                                        |
| Description      | Validate 'Create refresh token record' in RefreshTokenService.CreateRefreshTokenAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                  |
| Passed           | 7                                                                                                                                                                  |
| Failed           | 0                                                                                                                                                                  |
| Untested         | 0                                                                                                                                                                  |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                      |

### 2) Condition + Precondition

- Precondition: Refresh-token repository or DbContext is seeded; token hash, jwtId, and userId are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC046-01 |
| Method Should Exist                                         | UTC046-02 |
| Source Should Contain Method Declaration                    | UTC046-03 |
| Return Case Set Should Be Valid                             | UTC046-04 |
| Log Message Case Set Should Be Valid                        | UTC046-05 |
| When Logger Used Should Follow Log Message Convention       | UTC046-06 |
| When Result Response Used Should Follow Response Convention | UTC046-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- refreshToken.Id

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC046-01 | N                 | Type Should Exist                                           |
| UTC046-02 | N                 | Method Should Exist                                         |
| UTC046-03 | N                 | Source Should Contain Method Declaration                    |
| UTC046-04 | N                 | Return Case Set Should Be Valid                             |
| UTC046-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC046-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC046-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F046_RefreshTokenService_CreateRefreshTokenAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F047 - RefreshTokenService.GetByTokenHashAsync

### 1) Function Header

| Field            | Value                                                                                                                                                        |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F047                                                                                                                                                         |
| Function Name    | GetByTokenHashAsync                                                                                                                                          |
| Class Name       | RefreshTokenService                                                                                                                                          |
| Method           | GetByTokenHashAsync                                                                                                                                          |
| Requirement      | Get refresh token by hash                                                                                                                                    |
| Description      | Validate 'Get refresh token by hash' in RefreshTokenService.GetByTokenHashAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                            |
| Passed           | 7                                                                                                                                                            |
| Failed           | 0                                                                                                                                                            |
| Untested         | 0                                                                                                                                                            |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                |

### 2) Condition + Precondition

- Precondition: Refresh-token repository or DbContext is seeded; token hash, jwtId, and userId are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC047-01 |
| Method Should Exist                                         | UTC047-02 |
| Source Should Contain Method Declaration                    | UTC047-03 |
| Return Case Set Should Be Valid                             | UTC047-04 |
| Log Message Case Set Should Be Valid                        | UTC047-05 |
| When Logger Used Should Follow Log Message Convention       | UTC047-06 |
| When Result Response Used Should Follow Response Convention | UTC047-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- null

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC047-01 | N                 | Type Should Exist                                           |
| UTC047-02 | N                 | Method Should Exist                                         |
| UTC047-03 | N                 | Source Should Contain Method Declaration                    |
| UTC047-04 | N                 | Return Case Set Should Be Valid                             |
| UTC047-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC047-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC047-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F047_RefreshTokenService_GetByTokenHashAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F048 - RefreshTokenService.RotateRefreshTokenAsync

### 1) Function Header

| Field            | Value                                                                                                                                                       |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F048                                                                                                                                                        |
| Function Name    | RotateRefreshTokenAsync                                                                                                                                     |
| Class Name       | RefreshTokenService                                                                                                                                         |
| Method           | RotateRefreshTokenAsync                                                                                                                                     |
| Requirement      | Rotate refresh token                                                                                                                                        |
| Description      | Validate 'Rotate refresh token' in RefreshTokenService.RotateRefreshTokenAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                           |
| Passed           | 7                                                                                                                                                           |
| Failed           | 0                                                                                                                                                           |
| Untested         | 0                                                                                                                                                           |
| N/A/B            | N=7, A=0, B=0                                                                                                                                               |

### 2) Condition + Precondition

- Precondition: Refresh-token repository or DbContext is seeded; token hash, jwtId, and userId are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC048-01 |
| Method Should Exist                                         | UTC048-02 |
| Source Should Contain Method Declaration                    | UTC048-03 |
| Return Case Set Should Be Valid                             | UTC048-04 |
| Log Message Case Set Should Be Valid                        | UTC048-05 |
| When Logger Used Should Follow Log Message Convention       | UTC048-06 |
| When Result Response Used Should Follow Response Convention | UTC048-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- newToken.Id

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC048-01 | N                 | Type Should Exist                                           |
| UTC048-02 | N                 | Method Should Exist                                         |
| UTC048-03 | N                 | Source Should Contain Method Declaration                    |
| UTC048-04 | N                 | Return Case Set Should Be Valid                             |
| UTC048-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC048-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC048-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F048_RefreshTokenService_RotateRefreshTokenAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F049 - RefreshTokenService.RevokeTokenAsync

### 1) Function Header

| Field            | Value                                                                                                                                                |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F049                                                                                                                                                 |
| Function Name    | RevokeTokenAsync                                                                                                                                     |
| Class Name       | RefreshTokenService                                                                                                                                  |
| Method           | RevokeTokenAsync                                                                                                                                     |
| Requirement      | Revoke token by hash                                                                                                                                 |
| Description      | Validate 'Revoke token by hash' in RefreshTokenService.RevokeTokenAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                    |
| Passed           | 7                                                                                                                                                    |
| Failed           | 0                                                                                                                                                    |
| Untested         | 0                                                                                                                                                    |
| N/A/B            | N=7, A=0, B=0                                                                                                                                        |

### 2) Condition + Precondition

- Precondition: Refresh-token repository or DbContext is seeded; token hash, jwtId, and userId are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC049-01 |
| Method Should Exist                                         | UTC049-02 |
| Source Should Contain Method Declaration                    | UTC049-03 |
| Return Case Set Should Be Valid                             | UTC049-04 |
| Log Message Case Set Should Be Valid                        | UTC049-05 |
| When Logger Used Should Follow Log Message Convention       | UTC049-06 |
| When Result Response Used Should Follow Response Convention | UTC049-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC049-01 | N                 | Type Should Exist                                           |
| UTC049-02 | N                 | Method Should Exist                                         |
| UTC049-03 | N                 | Source Should Contain Method Declaration                    |
| UTC049-04 | N                 | Return Case Set Should Be Valid                             |
| UTC049-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC049-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC049-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F049_RefreshTokenService_RevokeTokenAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F050 - RefreshTokenService.RevokeAllUserTokensAsync

### 1) Function Header

| Field            | Value                                                                                                                                                          |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F050                                                                                                                                                           |
| Function Name    | RevokeAllUserTokensAsync                                                                                                                                       |
| Class Name       | RefreshTokenService                                                                                                                                            |
| Method           | RevokeAllUserTokensAsync                                                                                                                                       |
| Requirement      | Revoke all user tokens                                                                                                                                         |
| Description      | Validate 'Revoke all user tokens' in RefreshTokenService.RevokeAllUserTokensAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                              |
| Passed           | 7                                                                                                                                                              |
| Failed           | 0                                                                                                                                                              |
| Untested         | 0                                                                                                                                                              |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                  |

### 2) Condition + Precondition

- Precondition: Refresh-token repository or DbContext is seeded; token hash, jwtId, and userId are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC050-01 |
| Method Should Exist                                         | UTC050-02 |
| Source Should Contain Method Declaration                    | UTC050-03 |
| Return Case Set Should Be Valid                             | UTC050-04 |
| Log Message Case Set Should Be Valid                        | UTC050-05 |
| When Logger Used Should Follow Log Message Convention       | UTC050-06 |
| When Result Response Used Should Follow Response Convention | UTC050-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC050-01 | N                 | Type Should Exist                                           |
| UTC050-02 | N                 | Method Should Exist                                         |
| UTC050-03 | N                 | Source Should Contain Method Declaration                    |
| UTC050-04 | N                 | Return Case Set Should Be Valid                             |
| UTC050-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC050-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC050-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F050_RefreshTokenService_RevokeAllUserTokensAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F051 - RefreshTokenService.RevokeTokenFamilyAsync

### 1) Function Header

| Field            | Value                                                                                                                                                     |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F051                                                                                                                                                      |
| Function Name    | RevokeTokenFamilyAsync                                                                                                                                    |
| Class Name       | RefreshTokenService                                                                                                                                       |
| Method           | RevokeTokenFamilyAsync                                                                                                                                    |
| Requirement      | Revoke token family                                                                                                                                       |
| Description      | Validate 'Revoke token family' in RefreshTokenService.RevokeTokenFamilyAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                         |
| Passed           | 7                                                                                                                                                         |
| Failed           | 0                                                                                                                                                         |
| Untested         | 0                                                                                                                                                         |
| N/A/B            | N=7, A=0, B=0                                                                                                                                             |

### 2) Condition + Precondition

- Precondition: Refresh-token repository or DbContext is seeded; token hash, jwtId, and userId are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC051-01 |
| Method Should Exist                                         | UTC051-02 |
| Source Should Contain Method Declaration                    | UTC051-03 |
| Return Case Set Should Be Valid                             | UTC051-04 |
| Log Message Case Set Should Be Valid                        | UTC051-05 |
| When Logger Used Should Follow Log Message Convention       | UTC051-06 |
| When Result Response Used Should Follow Response Convention | UTC051-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC051-01 | N                 | Type Should Exist                                           |
| UTC051-02 | N                 | Method Should Exist                                         |
| UTC051-03 | N                 | Source Should Contain Method Declaration                    |
| UTC051-04 | N                 | Return Case Set Should Be Valid                             |
| UTC051-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC051-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC051-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F051_RefreshTokenService_RevokeTokenFamilyAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F052 - RefreshTokenService.CleanupExpiredTokensAsync

### 1) Function Header

| Field            | Value                                                                                                                                                           |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F052                                                                                                                                                            |
| Function Name    | CleanupExpiredTokensAsync                                                                                                                                       |
| Class Name       | RefreshTokenService                                                                                                                                             |
| Method           | CleanupExpiredTokensAsync                                                                                                                                       |
| Requirement      | Cleanup expired tokens                                                                                                                                          |
| Description      | Validate 'Cleanup expired tokens' in RefreshTokenService.CleanupExpiredTokensAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                               |
| Passed           | 7                                                                                                                                                               |
| Failed           | 0                                                                                                                                                               |
| Untested         | 0                                                                                                                                                               |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                   |

### 2) Condition + Precondition

- Precondition: Refresh-token repository or DbContext is seeded; token hash, jwtId, and userId are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC052-01 |
| Method Should Exist                                         | UTC052-02 |
| Source Should Contain Method Declaration                    | UTC052-03 |
| Return Case Set Should Be Valid                             | UTC052-04 |
| Log Message Case Set Should Be Valid                        | UTC052-05 |
| When Logger Used Should Follow Log Message Convention       | UTC052-06 |
| When Result Response Used Should Follow Response Convention | UTC052-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- tokensToDelete.Count

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC052-01 | N                 | Type Should Exist                                           |
| UTC052-02 | N                 | Method Should Exist                                         |
| UTC052-03 | N                 | Source Should Contain Method Declaration                    |
| UTC052-04 | N                 | Return Case Set Should Be Valid                             |
| UTC052-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC052-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC052-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F052_RefreshTokenService_CleanupExpiredTokensAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F053 - TokenService.GenerateAccessTokenAsync

### 1) Function Header

| Field            | Value                                                                                                                                                  |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F053                                                                                                                                                   |
| Function Name    | GenerateAccessTokenAsync                                                                                                                               |
| Class Name       | TokenService                                                                                                                                           |
| Method           | GenerateAccessTokenAsync                                                                                                                               |
| Requirement      | Generate access token                                                                                                                                  |
| Description      | Validate 'Generate access token' in TokenService.GenerateAccessTokenAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                      |
| Passed           | 7                                                                                                                                                      |
| Failed           | 0                                                                                                                                                      |
| Untested         | 0                                                                                                                                                      |
| N/A/B            | N=7, A=0, B=0                                                                                                                                          |

### 2) Condition + Precondition

- Precondition: JWT settings and token inputs are valid for both success and failure paths.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC053-01 |
| Method Should Exist                                         | UTC053-02 |
| Source Should Contain Method Declaration                    | UTC053-03 |
| Return Case Set Should Be Valid                             | UTC053-04 |
| Log Message Case Set Should Be Valid                        | UTC053-05 |
| When Logger Used Should Follow Log Message Convention       | UTC053-06 |
| When Result Response Used Should Follow Response Convention | UTC053-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Task.FromResult(new TokenResult(accessToken, jti, expiresAt))

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC053-01 | N                 | Type Should Exist                                           |
| UTC053-02 | N                 | Method Should Exist                                         |
| UTC053-03 | N                 | Source Should Contain Method Declaration                    |
| UTC053-04 | N                 | Return Case Set Should Be Valid                             |
| UTC053-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC053-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC053-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F053_TokenService_GenerateAccessTokenAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F054 - TokenService.GenerateRefreshToken

### 1) Function Header

| Field            | Value                                                                                                                                                      |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F054                                                                                                                                                       |
| Function Name    | GenerateRefreshToken                                                                                                                                       |
| Class Name       | TokenService                                                                                                                                               |
| Method           | GenerateRefreshToken                                                                                                                                       |
| Requirement      | Generate refresh token string                                                                                                                              |
| Description      | Validate 'Generate refresh token string' in TokenService.GenerateRefreshToken, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                          |
| Passed           | 7                                                                                                                                                          |
| Failed           | 0                                                                                                                                                          |
| Untested         | 0                                                                                                                                                          |
| N/A/B            | N=7, A=0, B=0                                                                                                                                              |

### 2) Condition + Precondition

- Precondition: JWT settings and token inputs are valid for both success and failure paths.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC054-01 |
| Method Should Exist                                         | UTC054-02 |
| Source Should Contain Method Declaration                    | UTC054-03 |
| Return Case Set Should Be Valid                             | UTC054-04 |
| Log Message Case Set Should Be Valid                        | UTC054-05 |
| When Logger Used Should Follow Log Message Convention       | UTC054-06 |
| When Result Response Used Should Follow Response Convention | UTC054-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Convert.ToBase64String(randomBytes)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC054-01 | N                 | Type Should Exist                                           |
| UTC054-02 | N                 | Method Should Exist                                         |
| UTC054-03 | N                 | Source Should Contain Method Declaration                    |
| UTC054-04 | N                 | Return Case Set Should Be Valid                             |
| UTC054-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC054-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC054-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F054_TokenService_GenerateRefreshToken_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F055 - TokenService.ValidateToken

### 1) Function Header

| Field            | Value                                                                                                                                |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F055                                                                                                                                 |
| Function Name    | ValidateToken                                                                                                                        |
| Class Name       | TokenService                                                                                                                         |
| Method           | ValidateToken                                                                                                                        |
| Requirement      | Validate token                                                                                                                       |
| Description      | Validate 'Validate token' in TokenService.ValidateToken, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                    |
| Passed           | 7                                                                                                                                    |
| Failed           | 0                                                                                                                                    |
| Untested         | 0                                                                                                                                    |
| N/A/B            | N=7, A=0, B=0                                                                                                                        |

### 2) Condition + Precondition

- Precondition: JWT settings and token inputs are valid for both success and failure paths.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC055-01 |
| Method Should Exist                                         | UTC055-02 |
| Source Should Contain Method Declaration                    | UTC055-03 |
| Return Case Set Should Be Valid                             | UTC055-04 |
| Log Message Case Set Should Be Valid                        | UTC055-05 |
| When Logger Used Should Follow Log Message Convention       | UTC055-06 |
| When Result Response Used Should Follow Response Convention | UTC055-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- null

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC055-01 | N                 | Type Should Exist                                           |
| UTC055-02 | N                 | Method Should Exist                                         |
| UTC055-03 | N                 | Source Should Contain Method Declaration                    |
| UTC055-04 | N                 | Return Case Set Should Be Valid                             |
| UTC055-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC055-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC055-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F055_TokenService_ValidateToken_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F056 - TokenService.GetUserIdFromToken

### 1) Function Header

| Field            | Value                                                                                                                                             |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F056                                                                                                                                              |
| Function Name    | GetUserIdFromToken                                                                                                                                |
| Class Name       | TokenService                                                                                                                                      |
| Method           | GetUserIdFromToken                                                                                                                                |
| Requirement      | Get user id from token                                                                                                                            |
| Description      | Validate 'Get user id from token' in TokenService.GetUserIdFromToken, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                 |
| Passed           | 7                                                                                                                                                 |
| Failed           | 0                                                                                                                                                 |
| Untested         | 0                                                                                                                                                 |
| N/A/B            | N=7, A=0, B=0                                                                                                                                     |

### 2) Condition + Precondition

- Precondition: JWT settings and token inputs are valid for both success and failure paths.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC056-01 |
| Method Should Exist                                         | UTC056-02 |
| Source Should Contain Method Declaration                    | UTC056-03 |
| Return Case Set Should Be Valid                             | UTC056-04 |
| Log Message Case Set Should Be Valid                        | UTC056-05 |
| When Logger Used Should Follow Log Message Convention       | UTC056-06 |
| When Result Response Used Should Follow Response Convention | UTC056-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- null

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC056-01 | N                 | Type Should Exist                                           |
| UTC056-02 | N                 | Method Should Exist                                         |
| UTC056-03 | N                 | Source Should Contain Method Declaration                    |
| UTC056-04 | N                 | Return Case Set Should Be Valid                             |
| UTC056-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC056-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC056-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F056_TokenService_GetUserIdFromToken_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F057 - TokenService.GetJtiFromToken

### 1) Function Header

| Field            | Value                                                                                                                                      |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F057                                                                                                                                       |
| Function Name    | GetJtiFromToken                                                                                                                            |
| Class Name       | TokenService                                                                                                                               |
| Method           | GetJtiFromToken                                                                                                                            |
| Requirement      | Get jti from token                                                                                                                         |
| Description      | Validate 'Get jti from token' in TokenService.GetJtiFromToken, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                          |
| Passed           | 7                                                                                                                                          |
| Failed           | 0                                                                                                                                          |
| Untested         | 0                                                                                                                                          |
| N/A/B            | N=7, A=0, B=0                                                                                                                              |

### 2) Condition + Precondition

- Precondition: JWT settings and token inputs are valid for both success and failure paths.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC057-01 |
| Method Should Exist                                         | UTC057-02 |
| Source Should Contain Method Declaration                    | UTC057-03 |
| Return Case Set Should Be Valid                             | UTC057-04 |
| Log Message Case Set Should Be Valid                        | UTC057-05 |
| When Logger Used Should Follow Log Message Convention       | UTC057-06 |
| When Result Response Used Should Follow Response Convention | UTC057-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC057-01 | N                 | Type Should Exist                                           |
| UTC057-02 | N                 | Method Should Exist                                         |
| UTC057-03 | N                 | Source Should Contain Method Declaration                    |
| UTC057-04 | N                 | Return Case Set Should Be Valid                             |
| UTC057-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC057-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC057-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F057_TokenService_GetJtiFromToken_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F058 - TokenService.HashToken

### 1) Function Header

| Field            | Value                                                                                                                        |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F058                                                                                                                         |
| Function Name    | HashToken                                                                                                                    |
| Class Name       | TokenService                                                                                                                 |
| Method           | HashToken                                                                                                                    |
| Requirement      | Hash token                                                                                                                   |
| Description      | Validate 'Hash token' in TokenService.HashToken, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                            |
| Passed           | 7                                                                                                                            |
| Failed           | 0                                                                                                                            |
| Untested         | 0                                                                                                                            |
| N/A/B            | N=7, A=0, B=0                                                                                                                |

### 2) Condition + Precondition

- Precondition: JWT settings and token inputs are valid for both success and failure paths.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC058-01 |
| Method Should Exist                                         | UTC058-02 |
| Source Should Contain Method Declaration                    | UTC058-03 |
| Return Case Set Should Be Valid                             | UTC058-04 |
| Log Message Case Set Should Be Valid                        | UTC058-05 |
| When Logger Used Should Follow Log Message Convention       | UTC058-06 |
| When Result Response Used Should Follow Response Convention | UTC058-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Convert.ToBase64String(hash)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC058-01 | N                 | Type Should Exist                                           |
| UTC058-02 | N                 | Method Should Exist                                         |
| UTC058-03 | N                 | Source Should Contain Method Declaration                    |
| UTC058-04 | N                 | Return Case Set Should Be Valid                             |
| UTC058-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC058-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC058-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F058_TokenService_HashToken_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F059 - AdminQueryService.GetOphthalmologistsAsync

### 1) Function Header

| Field            | Value                                                                                                                                                            |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F059                                                                                                                                                             |
| Function Name    | GetOphthalmologistsAsync                                                                                                                                         |
| Class Name       | AdminQueryService                                                                                                                                                |
| Method           | GetOphthalmologistsAsync                                                                                                                                         |
| Requirement      | Get ophthalmologists query                                                                                                                                       |
| Description      | Validate 'Get ophthalmologists query' in AdminQueryService.GetOphthalmologistsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                |
| Passed           | 7                                                                                                                                                                |
| Failed           | 0                                                                                                                                                                |
| Untested         | 0                                                                                                                                                                |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                    |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext is seeded; filter, sort, and paging inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC059-01 |
| Method Should Exist                                         | UTC059-02 |
| Source Should Contain Method Declaration                    | UTC059-03 |
| Return Case Set Should Be Valid                             | UTC059-04 |
| Log Message Case Set Should Be Valid                        | UTC059-05 |
| When Logger Used Should Follow Log Message Convention       | UTC059-06 |
| When Result Response Used Should Follow Response Convention | UTC059-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new OphthalmologistListDto { Id = row.OphthalmologistId, UserId = row.UserId, FullName = row.FullName, Email = row.Email, Phone = row.Phone, Bio = row.Bio, YearsOfExperience = row.YearsOfExperience, EmploymentType = row.EmploymentType, WorkingHoursPerWeek = row.WorkingHoursPerWeek, ExpectedMonthlySalary = row.ExpectedMonthlySalary, CommissionRate = row.CommissionRate, ActualMonthlySalary = row.ActualMonthlySalary, VerificationStatus = row.VerificationStatus, IsVerified = row.IsVerified, LicenseUrl = row.LicenseUrl, DegreeUrl = row.DegreeUrl, Licenses = licenses, Degrees = degrees, RejectionReason = row.RejectionReason, OrganisationName = row.OrganisationName, IsActive = row.IsActive, CreatedAt = row.CreatedAt }

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC059-01 | N                 | Type Should Exist                                           |
| UTC059-02 | N                 | Method Should Exist                                         |
| UTC059-03 | N                 | Source Should Contain Method Declaration                    |
| UTC059-04 | N                 | Return Case Set Should Be Valid                             |
| UTC059-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC059-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC059-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F059_AdminQueryService_GetOphthalmologistsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F060 - AdminQueryService.GetPatientsAsync

### 1) Function Header

| Field            | Value                                                                                                                                            |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F060                                                                                                                                             |
| Function Name    | GetPatientsAsync                                                                                                                                 |
| Class Name       | AdminQueryService                                                                                                                                |
| Method           | GetPatientsAsync                                                                                                                                 |
| Requirement      | Get patients query                                                                                                                               |
| Description      | Validate 'Get patients query' in AdminQueryService.GetPatientsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                |
| Passed           | 7                                                                                                                                                |
| Failed           | 0                                                                                                                                                |
| Untested         | 0                                                                                                                                                |
| N/A/B            | N=7, A=0, B=0                                                                                                                                    |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext is seeded; filter, sort, and paging inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC060-01 |
| Method Should Exist                                         | UTC060-02 |
| Source Should Contain Method Declaration                    | UTC060-03 |
| Return Case Set Should Be Valid                             | UTC060-04 |
| Log Message Case Set Should Be Valid                        | UTC060-05 |
| When Logger Used Should Follow Log Message Convention       | UTC060-06 |
| When Result Response Used Should Follow Response Convention | UTC060-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new PagedResult<PatientListDto>( items, totalCount, pageNumber, pageSize)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC060-01 | N                 | Type Should Exist                                           |
| UTC060-02 | N                 | Method Should Exist                                         |
| UTC060-03 | N                 | Source Should Contain Method Declaration                    |
| UTC060-04 | N                 | Return Case Set Should Be Valid                             |
| UTC060-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC060-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC060-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F060_AdminQueryService_GetPatientsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F061 - AdminQueryService.GetAuditLogsAsync

### 1) Function Header

| Field            | Value                                                                                                                                               |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F061                                                                                                                                                |
| Function Name    | GetAuditLogsAsync                                                                                                                                   |
| Class Name       | AdminQueryService                                                                                                                                   |
| Method           | GetAuditLogsAsync                                                                                                                                   |
| Requirement      | Get audit logs query                                                                                                                                |
| Description      | Validate 'Get audit logs query' in AdminQueryService.GetAuditLogsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                   |
| Passed           | 7                                                                                                                                                   |
| Failed           | 0                                                                                                                                                   |
| Untested         | 0                                                                                                                                                   |
| N/A/B            | N=7, A=0, B=0                                                                                                                                       |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext is seeded; filter, sort, and paging inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC061-01 |
| Method Should Exist                                         | UTC061-02 |
| Source Should Contain Method Declaration                    | UTC061-03 |
| Return Case Set Should Be Valid                             | UTC061-04 |
| Log Message Case Set Should Be Valid                        | UTC061-05 |
| When Logger Used Should Follow Log Message Convention       | UTC061-06 |
| When Result Response Used Should Follow Response Convention | UTC061-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new PagedResult<AuditLogDto>(items, totalCount, pageNumber, pageSize)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC061-01 | N                 | Type Should Exist                                           |
| UTC061-02 | N                 | Method Should Exist                                         |
| UTC061-03 | N                 | Source Should Contain Method Declaration                    |
| UTC061-04 | N                 | Return Case Set Should Be Valid                             |
| UTC061-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC061-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC061-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F061_AdminQueryService_GetAuditLogsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F062 - AiQuotaService.GetQuotaAsync

### 1) Function Header

| Field            | Value                                                                                                                                |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F062                                                                                                                                 |
| Function Name    | GetQuotaAsync                                                                                                                        |
| Class Name       | AiQuotaService                                                                                                                       |
| Method           | GetQuotaAsync                                                                                                                        |
| Requirement      | Get AI quota                                                                                                                         |
| Description      | Validate 'Get AI quota' in AiQuotaService.GetQuotaAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                    |
| Passed           | 7                                                                                                                                    |
| Failed           | 0                                                                                                                                    |
| Untested         | 0                                                                                                                                    |
| N/A/B            | N=7, A=0, B=0                                                                                                                        |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext and setting dependency are seeded with quota data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC062-01 |
| Method Should Exist                                         | UTC062-02 |
| Source Should Contain Method Declaration                    | UTC062-03 |
| Return Case Set Should Be Valid                             | UTC062-04 |
| Log Message Case Set Should Be Valid                        | UTC062-05 |
| When Logger Used Should Follow Log Message Convention       | UTC062-06 |
| When Result Response Used Should Follow Response Convention | UTC062-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- await GetPatientQuotaAsync(userId, cancellationToken)

#### Exception

- No dedicated exception case asserted.

#### Log message

- [AiQuotaService] GetQuotaAsync called â€” UserId: {UserId}, Role: {Role}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC062-01 | N                 | Type Should Exist                                           |
| UTC062-02 | N                 | Method Should Exist                                         |
| UTC062-03 | N                 | Source Should Contain Method Declaration                    |
| UTC062-04 | N                 | Return Case Set Should Be Valid                             |
| UTC062-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC062-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC062-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F062_AiQuotaService_GetQuotaAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F063 - AiQuotaService.HasAvailableQuotaAsync

### 1) Function Header

| Field            | Value                                                                                                                                                     |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F063                                                                                                                                                      |
| Function Name    | HasAvailableQuotaAsync                                                                                                                                    |
| Class Name       | AiQuotaService                                                                                                                                            |
| Method           | HasAvailableQuotaAsync                                                                                                                                    |
| Requirement      | Check AI quota available                                                                                                                                  |
| Description      | Validate 'Check AI quota available' in AiQuotaService.HasAvailableQuotaAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                         |
| Passed           | 7                                                                                                                                                         |
| Failed           | 0                                                                                                                                                         |
| Untested         | 0                                                                                                                                                         |
| N/A/B            | N=7, A=0, B=0                                                                                                                                             |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext and setting dependency are seeded with quota data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC063-01 |
| Method Should Exist                                         | UTC063-02 |
| Source Should Contain Method Declaration                    | UTC063-03 |
| Return Case Set Should Be Valid                             | UTC063-04 |
| Log Message Case Set Should Be Valid                        | UTC063-05 |
| When Logger Used Should Follow Log Message Convention       | UTC063-06 |
| When Result Response Used Should Follow Response Convention | UTC063-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- quota.RemainingQuota > 0

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC063-01 | N                 | Type Should Exist                                           |
| UTC063-02 | N                 | Method Should Exist                                         |
| UTC063-03 | N                 | Source Should Contain Method Declaration                    |
| UTC063-04 | N                 | Return Case Set Should Be Valid                             |
| UTC063-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC063-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC063-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F063_AiQuotaService_HasAvailableQuotaAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F064 - AiQuotaService.DeductQuotaAsync

### 1) Function Header

| Field            | Value                                                                                                                                      |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F064                                                                                                                                       |
| Function Name    | DeductQuotaAsync                                                                                                                           |
| Class Name       | AiQuotaService                                                                                                                             |
| Method           | DeductQuotaAsync                                                                                                                           |
| Requirement      | Deduct AI quota                                                                                                                            |
| Description      | Validate 'Deduct AI quota' in AiQuotaService.DeductQuotaAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                          |
| Passed           | 7                                                                                                                                          |
| Failed           | 0                                                                                                                                          |
| Untested         | 0                                                                                                                                          |
| N/A/B            | N=7, A=0, B=0                                                                                                                              |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext and setting dependency are seeded with quota data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC064-01 |
| Method Should Exist                                         | UTC064-02 |
| Source Should Contain Method Declaration                    | UTC064-03 |
| Return Case Set Should Be Valid                             | UTC064-04 |
| Log Message Case Set Should Be Valid                        | UTC064-05 |
| When Logger Used Should Follow Log Message Convention       | UTC064-06 |
| When Result Response Used Should Follow Response Convention | UTC064-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC064-01 | N                 | Type Should Exist                                           |
| UTC064-02 | N                 | Method Should Exist                                         |
| UTC064-03 | N                 | Source Should Contain Method Declaration                    |
| UTC064-04 | N                 | Return Case Set Should Be Valid                             |
| UTC064-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC064-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC064-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F064_AiQuotaService_DeductQuotaAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F065 - AiQuotaService.AddPurchasedQuotaAsync

### 1) Function Header

| Field            | Value                                                                                                                                                   |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F065                                                                                                                                                    |
| Function Name    | AddPurchasedQuotaAsync                                                                                                                                  |
| Class Name       | AiQuotaService                                                                                                                                          |
| Method           | AddPurchasedQuotaAsync                                                                                                                                  |
| Requirement      | Add purchased AI quota                                                                                                                                  |
| Description      | Validate 'Add purchased AI quota' in AiQuotaService.AddPurchasedQuotaAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                       |
| Passed           | 7                                                                                                                                                       |
| Failed           | 0                                                                                                                                                       |
| Untested         | 0                                                                                                                                                       |
| N/A/B            | N=7, A=0, B=0                                                                                                                                           |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext and setting dependency are seeded with quota data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC065-01 |
| Method Should Exist                                         | UTC065-02 |
| Source Should Contain Method Declaration                    | UTC065-03 |
| Return Case Set Should Be Valid                             | UTC065-04 |
| Log Message Case Set Should Be Valid                        | UTC065-05 |
| When Logger Used Should Follow Log Message Convention       | UTC065-06 |
| When Result Response Used Should Follow Response Convention | UTC065-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC065-01 | N                 | Type Should Exist                                           |
| UTC065-02 | N                 | Method Should Exist                                         |
| UTC065-03 | N                 | Source Should Contain Method Declaration                    |
| UTC065-04 | N                 | Return Case Set Should Be Valid                             |
| UTC065-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC065-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC065-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F065_AiQuotaService_AddPurchasedQuotaAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F066 - BetterStackHeartbeatService.GetEmbedUrl

### 1) Function Header

| Field            | Value                                                                                                                                                        |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F066                                                                                                                                                         |
| Function Name    | GetEmbedUrl                                                                                                                                                  |
| Class Name       | BetterStackHeartbeatService                                                                                                                                  |
| Method           | GetEmbedUrl                                                                                                                                                  |
| Requirement      | Get BetterStack embed URL                                                                                                                                    |
| Description      | Validate 'Get BetterStack embed URL' in BetterStackHeartbeatService.GetEmbedUrl, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                            |
| Passed           | 7                                                                                                                                                            |
| Failed           | 0                                                                                                                                                            |
| Untested         | 0                                                                                                                                                            |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                |

### 2) Condition + Precondition

- Precondition: Heartbeat configuration is valid and the client/logger are initialized.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC066-01 |
| Method Should Exist                                         | UTC066-02 |
| Source Should Contain Method Declaration                    | UTC066-03 |
| Return Case Set Should Be Valid                             | UTC066-04 |
| Log Message Case Set Should Be Valid                        | UTC066-05 |
| When Logger Used Should Follow Log Message Convention       | UTC066-06 |
| When Result Response Used Should Follow Response Convention | UTC066-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC066-01 | N                 | Type Should Exist                                           |
| UTC066-02 | N                 | Method Should Exist                                         |
| UTC066-03 | N                 | Source Should Contain Method Declaration                    |
| UTC066-04 | N                 | Return Case Set Should Be Valid                             |
| UTC066-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC066-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC066-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F066_BetterStackHeartbeatService_GetEmbedUrl_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F067 - BetterStackHeartbeatService.GetMonitorDescriptors

### 1) Function Header

| Field            | Value                                                                                                                                                                            |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F067                                                                                                                                                                             |
| Function Name    | GetMonitorDescriptors                                                                                                                                                            |
| Class Name       | BetterStackHeartbeatService                                                                                                                                                      |
| Method           | GetMonitorDescriptors                                                                                                                                                            |
| Requirement      | Get BetterStack monitor descriptors                                                                                                                                              |
| Description      | Validate 'Get BetterStack monitor descriptors' in BetterStackHeartbeatService.GetMonitorDescriptors, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                                |
| Passed           | 7                                                                                                                                                                                |
| Failed           | 0                                                                                                                                                                                |
| Untested         | 0                                                                                                                                                                                |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                                    |

### 2) Condition + Precondition

- Precondition: Heartbeat configuration is valid and the client/logger are initialized.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC067-01 |
| Method Should Exist                                         | UTC067-02 |
| Source Should Contain Method Declaration                    | UTC067-03 |
| Return Case Set Should Be Valid                             | UTC067-04 |
| Log Message Case Set Should Be Valid                        | UTC067-05 |
| When Logger Used Should Follow Log Message Convention       | UTC067-06 |
| When Result Response Used Should Follow Response Convention | UTC067-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Enum.GetValues<BetterStackMonitor>() .Select(monitor => { var meta = MonitorMeta[monitor]

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC067-01 | N                 | Type Should Exist                                           |
| UTC067-02 | N                 | Method Should Exist                                         |
| UTC067-03 | N                 | Source Should Contain Method Declaration                    |
| UTC067-04 | N                 | Return Case Set Should Be Valid                             |
| UTC067-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC067-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC067-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F067_BetterStackHeartbeatService_GetMonitorDescriptors_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F068 - BetterStackHeartbeatService.NotifyStartedAsync

### 1) Function Header

| Field            | Value                                                                                                                                                            |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F068                                                                                                                                                             |
| Function Name    | NotifyStartedAsync                                                                                                                                               |
| Class Name       | BetterStackHeartbeatService                                                                                                                                      |
| Method           | NotifyStartedAsync                                                                                                                                               |
| Requirement      | Notify monitor started                                                                                                                                           |
| Description      | Validate 'Notify monitor started' in BetterStackHeartbeatService.NotifyStartedAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                |
| Passed           | 7                                                                                                                                                                |
| Failed           | 0                                                                                                                                                                |
| Untested         | 0                                                                                                                                                                |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                    |

### 2) Condition + Precondition

- Precondition: Heartbeat configuration is valid and the client/logger are initialized.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC068-01 |
| Method Should Exist                                         | UTC068-02 |
| Source Should Contain Method Declaration                    | UTC068-03 |
| Return Case Set Should Be Valid                             | UTC068-04 |
| Log Message Case Set Should Be Valid                        | UTC068-05 |
| When Logger Used Should Follow Log Message Convention       | UTC068-06 |
| When Result Response Used Should Follow Response Convention | UTC068-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC068-01 | N                 | Type Should Exist                                           |
| UTC068-02 | N                 | Method Should Exist                                         |
| UTC068-03 | N                 | Source Should Contain Method Declaration                    |
| UTC068-04 | N                 | Return Case Set Should Be Valid                             |
| UTC068-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC068-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC068-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F068_BetterStackHeartbeatService_NotifyStartedAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F069 - BetterStackHeartbeatService.NotifySucceededAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F069                                                                                                                                                                 |
| Function Name    | NotifySucceededAsync                                                                                                                                                 |
| Class Name       | BetterStackHeartbeatService                                                                                                                                          |
| Method           | NotifySucceededAsync                                                                                                                                                 |
| Requirement      | Notify monitor succeeded                                                                                                                                             |
| Description      | Validate 'Notify monitor succeeded' in BetterStackHeartbeatService.NotifySucceededAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                    |
| Passed           | 7                                                                                                                                                                    |
| Failed           | 0                                                                                                                                                                    |
| Untested         | 0                                                                                                                                                                    |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                        |

### 2) Condition + Precondition

- Precondition: Heartbeat configuration is valid and the client/logger are initialized.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC069-01 |
| Method Should Exist                                         | UTC069-02 |
| Source Should Contain Method Declaration                    | UTC069-03 |
| Return Case Set Should Be Valid                             | UTC069-04 |
| Log Message Case Set Should Be Valid                        | UTC069-05 |
| When Logger Used Should Follow Log Message Convention       | UTC069-06 |
| When Result Response Used Should Follow Response Convention | UTC069-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC069-01 | N                 | Type Should Exist                                           |
| UTC069-02 | N                 | Method Should Exist                                         |
| UTC069-03 | N                 | Source Should Contain Method Declaration                    |
| UTC069-04 | N                 | Return Case Set Should Be Valid                             |
| UTC069-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC069-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC069-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F069_BetterStackHeartbeatService_NotifySucceededAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F070 - BetterStackHeartbeatService.NotifyFailedAsync

### 1) Function Header

| Field            | Value                                                                                                                                                          |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F070                                                                                                                                                           |
| Function Name    | NotifyFailedAsync                                                                                                                                              |
| Class Name       | BetterStackHeartbeatService                                                                                                                                    |
| Method           | NotifyFailedAsync                                                                                                                                              |
| Requirement      | Notify monitor failed                                                                                                                                          |
| Description      | Validate 'Notify monitor failed' in BetterStackHeartbeatService.NotifyFailedAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                              |
| Passed           | 7                                                                                                                                                              |
| Failed           | 0                                                                                                                                                              |
| Untested         | 0                                                                                                                                                              |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                  |

### 2) Condition + Precondition

- Precondition: Heartbeat configuration is valid and the client/logger are initialized.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC070-01 |
| Method Should Exist                                         | UTC070-02 |
| Source Should Contain Method Declaration                    | UTC070-03 |
| Return Case Set Should Be Valid                             | UTC070-04 |
| Log Message Case Set Should Be Valid                        | UTC070-05 |
| When Logger Used Should Follow Log Message Convention       | UTC070-06 |
| When Result Response Used Should Follow Response Convention | UTC070-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC070-01 | N                 | Type Should Exist                                           |
| UTC070-02 | N                 | Method Should Exist                                         |
| UTC070-03 | N                 | Source Should Contain Method Declaration                    |
| UTC070-04 | N                 | Return Case Set Should Be Valid                             |
| UTC070-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC070-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC070-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F070_BetterStackHeartbeatService_NotifyFailedAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F071 - DailyQuotaResetJob.ExecuteAsync

### 1) Function Header

| Field            | Value                                                                                                                                                |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F071                                                                                                                                                 |
| Function Name    | ExecuteAsync                                                                                                                                         |
| Class Name       | DailyQuotaResetJob                                                                                                                                   |
| Method           | ExecuteAsync                                                                                                                                         |
| Requirement      | Execute daily quota reset                                                                                                                            |
| Description      | Validate 'Execute daily quota reset' in DailyQuotaResetJob.ExecuteAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                    |
| Passed           | 7                                                                                                                                                    |
| Failed           | 0                                                                                                                                                    |
| Untested         | 0                                                                                                                                                    |
| N/A/B            | N=7, A=0, B=0                                                                                                                                        |

### 2) Condition + Precondition

- Precondition: Job context and quota dependencies are initialized.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC071-01 |
| Method Should Exist                                         | UTC071-02 |
| Source Should Contain Method Declaration                    | UTC071-03 |
| Return Case Set Should Be Valid                             | UTC071-04 |
| Log Message Case Set Should Be Valid                        | UTC071-05 |
| When Logger Used Should Follow Log Message Convention       | UTC071-06 |
| When Result Response Used Should Follow Response Convention | UTC071-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- Starting daily AI quota reset job at {Time} UTC

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC071-01 | N                 | Type Should Exist                                           |
| UTC071-02 | N                 | Method Should Exist                                         |
| UTC071-03 | N                 | Source Should Contain Method Declaration                    |
| UTC071-04 | N                 | Return Case Set Should Be Valid                             |
| UTC071-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC071-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC071-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F071_DailyQuotaResetJob_ExecuteAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F072 - DashboardMetricsService.GetSystemAdminMetricsAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                  |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F072                                                                                                                                                                   |
| Function Name    | GetSystemAdminMetricsAsync                                                                                                                                             |
| Class Name       | DashboardMetricsService                                                                                                                                                |
| Method           | GetSystemAdminMetricsAsync                                                                                                                                             |
| Requirement      | Get system admin metrics                                                                                                                                               |
| Description      | Validate 'Get system admin metrics' in DashboardMetricsService.GetSystemAdminMetricsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                      |
| Passed           | 7                                                                                                                                                                      |
| Failed           | 0                                                                                                                                                                      |
| Untested         | 0                                                                                                                                                                      |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                          |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext is seeded with metrics data; time-range inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC072-01 |
| Method Should Exist                                         | UTC072-02 |
| Source Should Contain Method Declaration                    | UTC072-03 |
| Return Case Set Should Be Valid                             | UTC072-04 |
| Log Message Case Set Should Be Valid                        | UTC072-05 |
| When Logger Used Should Follow Log Message Convention       | UTC072-06 |
| When Result Response Used Should Follow Response Convention | UTC072-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new PaymentMethodRevenueDto { PaymentMethod = item.PaymentMethod.ToString(), Amount = amount, Percentage = totalDepositAmount <= 0m ? 0m : Math.Round(amount / totalDepositAmount \* 100m, 1) }

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC072-01 | N                 | Type Should Exist                                           |
| UTC072-02 | N                 | Method Should Exist                                         |
| UTC072-03 | N                 | Source Should Contain Method Declaration                    |
| UTC072-04 | N                 | Return Case Set Should Be Valid                             |
| UTC072-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC072-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC072-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F072_DashboardMetricsService_GetSystemAdminMetricsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F073 - DashboardMetricsService.GetRecentScreeningsAsync

### 1) Function Header

| Field            | Value                                                                                                                                                             |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F073                                                                                                                                                              |
| Function Name    | GetRecentScreeningsAsync                                                                                                                                          |
| Class Name       | DashboardMetricsService                                                                                                                                           |
| Method           | GetRecentScreeningsAsync                                                                                                                                          |
| Requirement      | Get recent screenings                                                                                                                                             |
| Description      | Validate 'Get recent screenings' in DashboardMetricsService.GetRecentScreeningsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                 |
| Passed           | 7                                                                                                                                                                 |
| Failed           | 0                                                                                                                                                                 |
| Untested         | 0                                                                                                                                                                 |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                     |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext is seeded with metrics data; time-range inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC073-01 |
| Method Should Exist                                         | UTC073-02 |
| Source Should Contain Method Declaration                    | UTC073-03 |
| Return Case Set Should Be Valid                             | UTC073-04 |
| Log Message Case Set Should Be Valid                        | UTC073-05 |
| When Logger Used Should Follow Log Message Convention       | UTC073-06 |
| When Result Response Used Should Follow Response Convention | UTC073-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new PagedResult<RecentScreeningDto>(items, totalCount, pageNumber, pageSize)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC073-01 | N                 | Type Should Exist                                           |
| UTC073-02 | N                 | Method Should Exist                                         |
| UTC073-03 | N                 | Source Should Contain Method Declaration                    |
| UTC073-04 | N                 | Return Case Set Should Be Valid                             |
| UTC073-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC073-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC073-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F073_DashboardMetricsService_GetRecentScreeningsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F074 - DashboardMetricsService.GetScreeningVolumeTrendsAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                        |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F074                                                                                                                                                                         |
| Function Name    | GetScreeningVolumeTrendsAsync                                                                                                                                                |
| Class Name       | DashboardMetricsService                                                                                                                                                      |
| Method           | GetScreeningVolumeTrendsAsync                                                                                                                                                |
| Requirement      | Get screening volume trends                                                                                                                                                  |
| Description      | Validate 'Get screening volume trends' in DashboardMetricsService.GetScreeningVolumeTrendsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                            |
| Passed           | 7                                                                                                                                                                            |
| Failed           | 0                                                                                                                                                                            |
| Untested         | 0                                                                                                                                                                            |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                                |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext is seeded with metrics data; time-range inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC074-01 |
| Method Should Exist                                         | UTC074-02 |
| Source Should Contain Method Declaration                    | UTC074-03 |
| Return Case Set Should Be Valid                             | UTC074-04 |
| Log Message Case Set Should Be Valid                        | UTC074-05 |
| When Logger Used Should Follow Log Message Convention       | UTC074-06 |
| When Result Response Used Should Follow Response Convention | UTC074-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- date.Date.AddDays(-offset)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC074-01 | N                 | Type Should Exist                                           |
| UTC074-02 | N                 | Method Should Exist                                         |
| UTC074-03 | N                 | Source Should Contain Method Declaration                    |
| UTC074-04 | N                 | Return Case Set Should Be Valid                             |
| UTC074-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC074-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC074-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F074_DashboardMetricsService_GetScreeningVolumeTrendsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F075 - DashboardMetricsService.GetPopulationRiskAnalysisAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                          |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F075                                                                                                                                                                           |
| Function Name    | GetPopulationRiskAnalysisAsync                                                                                                                                                 |
| Class Name       | DashboardMetricsService                                                                                                                                                        |
| Method           | GetPopulationRiskAnalysisAsync                                                                                                                                                 |
| Requirement      | Get population risk analysis                                                                                                                                                   |
| Description      | Validate 'Get population risk analysis' in DashboardMetricsService.GetPopulationRiskAnalysisAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                              |
| Passed           | 7                                                                                                                                                                              |
| Failed           | 0                                                                                                                                                                              |
| Untested         | 0                                                                                                                                                                              |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                                  |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext is seeded with metrics data; time-range inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC075-01 |
| Method Should Exist                                         | UTC075-02 |
| Source Should Contain Method Declaration                    | UTC075-03 |
| Return Case Set Should Be Valid                             | UTC075-04 |
| Log Message Case Set Should Be Valid                        | UTC075-05 |
| When Logger Used Should Follow Log Message Convention       | UTC075-06 |
| When Result Response Used Should Follow Response Convention | UTC075-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new PopulationRiskAnalysisDto { TotalPatients = totalPatients, RiskCategories = grouped .Where(item => item.Key != RiskLevel.None) .OrderBy(item => item.Key) .Select(item => new RiskCategoryDto { RiskLevel = item.Key.ToString(), Count = item.Count, Percentage = totalResults == 0 ? 0 : Math.Round((decimal)item.Count / totalResults \* 100m, 1) }) .ToList() }

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC075-01 | N                 | Type Should Exist                                           |
| UTC075-02 | N                 | Method Should Exist                                         |
| UTC075-03 | N                 | Source Should Contain Method Declaration                    |
| UTC075-04 | N                 | Return Case Set Should Be Valid                             |
| UTC075-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC075-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC075-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F075_DashboardMetricsService_GetPopulationRiskAnalysisAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F076 - DashboardMetricsService.GetSystemHealthAsync

### 1) Function Header

| Field            | Value                                                                                                                                                     |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F076                                                                                                                                                      |
| Function Name    | GetSystemHealthAsync                                                                                                                                      |
| Class Name       | DashboardMetricsService                                                                                                                                   |
| Method           | GetSystemHealthAsync                                                                                                                                      |
| Requirement      | Get system health                                                                                                                                         |
| Description      | Validate 'Get system health' in DashboardMetricsService.GetSystemHealthAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                         |
| Passed           | 7                                                                                                                                                         |
| Failed           | 0                                                                                                                                                         |
| Untested         | 0                                                                                                                                                         |
| N/A/B            | N=7, A=0, B=0                                                                                                                                             |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext is seeded with metrics data; time-range inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC076-01 |
| Method Should Exist                                         | UTC076-02 |
| Source Should Contain Method Declaration                    | UTC076-03 |
| Return Case Set Should Be Valid                             | UTC076-04 |
| Log Message Case Set Should Be Valid                        | UTC076-05 |
| When Logger Used Should Follow Log Message Convention       | UTC076-06 |
| When Result Response Used Should Follow Response Convention | UTC076-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Task.FromResult(new SystemHealthDto { AllSystemsOperational = true, Components = new List<ComponentHealthDto> { new() { ComponentName = "Database", Status = "Connected", IsHealthy = true, LatencyMs = 25, UptimePercentage = 100, LastCheckedAt = DateTime.UtcNow }, new() { ComponentName = "AI Service", Status = "Online", IsHealthy = true, LatencyMs = 120, UptimePercentage = 100, LastCheckedAt = DateTime.UtcNow }, new() { ComponentName = "Notifications", Status = "Operational", IsHealthy = true, LatencyMs = 40, UptimePercentage = 100, LastCheckedAt = DateTime.UtcNow } } })

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC076-01 | N                 | Type Should Exist                                           |
| UTC076-02 | N                 | Method Should Exist                                         |
| UTC076-03 | N                 | Source Should Contain Method Declaration                    |
| UTC076-04 | N                 | Return Case Set Should Be Valid                             |
| UTC076-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC076-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC076-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F076_DashboardMetricsService_GetSystemHealthAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F077 - DashboardMetricsService.GetOphthalmologistMetricsAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                         |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F077                                                                                                                                                                          |
| Function Name    | GetOphthalmologistMetricsAsync                                                                                                                                                |
| Class Name       | DashboardMetricsService                                                                                                                                                       |
| Method           | GetOphthalmologistMetricsAsync                                                                                                                                                |
| Requirement      | Get ophthalmologist metrics                                                                                                                                                   |
| Description      | Validate 'Get ophthalmologist metrics' in DashboardMetricsService.GetOphthalmologistMetricsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                             |
| Passed           | 7                                                                                                                                                                             |
| Failed           | 0                                                                                                                                                                             |
| Untested         | 0                                                                                                                                                                             |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                                 |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext is seeded with metrics data; time-range inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC077-01 |
| Method Should Exist                                         | UTC077-02 |
| Source Should Contain Method Declaration                    | UTC077-03 |
| Return Case Set Should Be Valid                             | UTC077-04 |
| Log Message Case Set Should Be Valid                        | UTC077-05 |
| When Logger Used Should Follow Log Message Convention       | UTC077-06 |
| When Result Response Used Should Follow Response Convention | UTC077-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new OphthalmologistDashboardMetricsDto()

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC077-01 | N                 | Type Should Exist                                           |
| UTC077-02 | N                 | Method Should Exist                                         |
| UTC077-03 | N                 | Source Should Contain Method Declaration                    |
| UTC077-04 | N                 | Return Case Set Should Be Valid                             |
| UTC077-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC077-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC077-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F077_DashboardMetricsService_GetOphthalmologistMetricsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F078 - DashboardMetricsService.GetOrganisationMetricsAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                   |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F078                                                                                                                                                                    |
| Function Name    | GetOrganisationMetricsAsync                                                                                                                                             |
| Class Name       | DashboardMetricsService                                                                                                                                                 |
| Method           | GetOrganisationMetricsAsync                                                                                                                                             |
| Requirement      | Get organisation metrics                                                                                                                                                |
| Description      | Validate 'Get organisation metrics' in DashboardMetricsService.GetOrganisationMetricsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                       |
| Passed           | 7                                                                                                                                                                       |
| Failed           | 0                                                                                                                                                                       |
| Untested         | 0                                                                                                                                                                       |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                           |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext is seeded with metrics data; time-range inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC078-01 |
| Method Should Exist                                         | UTC078-02 |
| Source Should Contain Method Declaration                    | UTC078-03 |
| Return Case Set Should Be Valid                             | UTC078-04 |
| Log Message Case Set Should Be Valid                        | UTC078-05 |
| When Logger Used Should Follow Log Message Convention       | UTC078-06 |
| When Result Response Used Should Follow Response Convention | UTC078-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new OrganisationDashboardMetricsDto()

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC078-01 | N                 | Type Should Exist                                           |
| UTC078-02 | N                 | Method Should Exist                                         |
| UTC078-03 | N                 | Source Should Contain Method Declaration                    |
| UTC078-04 | N                 | Return Case Set Should Be Valid                             |
| UTC078-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC078-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC078-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F078_DashboardMetricsService_GetOrganisationMetricsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F079 - DashboardMetricsService.GetPatientMetricsAsync

### 1) Function Header

| Field            | Value                                                                                                                                                         |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F079                                                                                                                                                          |
| Function Name    | GetPatientMetricsAsync                                                                                                                                        |
| Class Name       | DashboardMetricsService                                                                                                                                       |
| Method           | GetPatientMetricsAsync                                                                                                                                        |
| Requirement      | Get patient metrics                                                                                                                                           |
| Description      | Validate 'Get patient metrics' in DashboardMetricsService.GetPatientMetricsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                             |
| Passed           | 7                                                                                                                                                             |
| Failed           | 0                                                                                                                                                             |
| Untested         | 0                                                                                                                                                             |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                 |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext is seeded with metrics data; time-range inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC079-01 |
| Method Should Exist                                         | UTC079-02 |
| Source Should Contain Method Declaration                    | UTC079-03 |
| Return Case Set Should Be Valid                             | UTC079-04 |
| Log Message Case Set Should Be Valid                        | UTC079-05 |
| When Logger Used Should Follow Log Message Convention       | UTC079-06 |
| When Result Response Used Should Follow Response Convention | UTC079-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new PatientDashboardMetricsDto()

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC079-01 | N                 | Type Should Exist                                           |
| UTC079-02 | N                 | Method Should Exist                                         |
| UTC079-03 | N                 | Source Should Contain Method Declaration                    |
| UTC079-04 | N                 | Return Case Set Should Be Valid                             |
| UTC079-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC079-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC079-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F079_DashboardMetricsService_GetPatientMetricsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F080 - DateTimeService.Now (property)

### 1) Function Header

| Field            | Value                                                                                                                                   |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F080                                                                                                                                    |
| Function Name    | Now (property)                                                                                                                          |
| Class Name       | DateTimeService                                                                                                                         |
| Method           | Now (property)                                                                                                                          |
| Requirement      | Get local now                                                                                                                           |
| Description      | Validate 'Get local now' in DateTimeService.Now (property), covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 4                                                                                                                                       |
| Passed           | 4                                                                                                                                       |
| Failed           | 0                                                                                                                                       |
| Untested         | 0                                                                                                                                       |
| N/A/B            | N=4, A=0, B=0                                                                                                                           |

### 2) Condition + Precondition

- Precondition: Service is instantiated directly with no external dependency.

| Condition Item                             | UTC Ref   |
| ------------------------------------------ | --------- |
| Type Should Exist                          | UTC080-01 |
| Property Should Exist                      | UTC080-02 |
| Source Should Contain Property Declaration | UTC080-03 |
| Property Should Be Readable                | UTC080-04 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                |
| --------- | ----------------- | ------------------------------------------ |
| UTC080-01 | N                 | Type Should Exist                          |
| UTC080-02 | N                 | Property Should Exist                      |
| UTC080-03 | N                 | Source Should Contain Property Declaration |
| UTC080-04 | N                 | Property Should Be Readable                |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N
- Passed/Failed: P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F080_DateTimeService_Now_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F081 - DateTimeService.UtcNow (property)

### 1) Function Header

| Field            | Value                                                                                                                                    |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F081                                                                                                                                     |
| Function Name    | UtcNow (property)                                                                                                                        |
| Class Name       | DateTimeService                                                                                                                          |
| Method           | UtcNow (property)                                                                                                                        |
| Requirement      | Get UTC now                                                                                                                              |
| Description      | Validate 'Get UTC now' in DateTimeService.UtcNow (property), covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 4                                                                                                                                        |
| Passed           | 4                                                                                                                                        |
| Failed           | 0                                                                                                                                        |
| Untested         | 0                                                                                                                                        |
| N/A/B            | N=4, A=0, B=0                                                                                                                            |

### 2) Condition + Precondition

- Precondition: Service is instantiated directly with no external dependency.

| Condition Item                             | UTC Ref   |
| ------------------------------------------ | --------- |
| Type Should Exist                          | UTC081-01 |
| Property Should Exist                      | UTC081-02 |
| Source Should Contain Property Declaration | UTC081-03 |
| Property Should Be Readable                | UTC081-04 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                |
| --------- | ----------------- | ------------------------------------------ |
| UTC081-01 | N                 | Type Should Exist                          |
| UTC081-02 | N                 | Property Should Exist                      |
| UTC081-03 | N                 | Source Should Contain Property Declaration |
| UTC081-04 | N                 | Property Should Be Readable                |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N
- Passed/Failed: P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F081_DateTimeService_UtcNow_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F082 - EmailService.SendEmailConfirmationAsync

### 1) Function Header

| Field            | Value                                                                                                                                                      |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F082                                                                                                                                                       |
| Function Name    | SendEmailConfirmationAsync                                                                                                                                 |
| Class Name       | EmailService                                                                                                                                               |
| Method           | SendEmailConfirmationAsync                                                                                                                                 |
| Requirement      | Send email confirmation                                                                                                                                    |
| Description      | Validate 'Send email confirmation' in EmailService.SendEmailConfirmationAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                          |
| Passed           | 7                                                                                                                                                          |
| Failed           | 0                                                                                                                                                          |
| Untested         | 0                                                                                                                                                          |
| N/A/B            | N=7, A=0, B=0                                                                                                                                              |

### 2) Condition + Precondition

- Precondition: SMTP settings are valid; recipient and template inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC082-01 |
| Method Should Exist                                         | UTC082-02 |
| Source Should Contain Method Declaration                    | UTC082-03 |
| Return Case Set Should Be Valid                             | UTC082-04 |
| Log Message Case Set Should Be Valid                        | UTC082-05 |
| When Logger Used Should Follow Log Message Convention       | UTC082-06 |
| When Result Response Used Should Follow Response Convention | UTC082-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC082-01 | N                 | Type Should Exist                                           |
| UTC082-02 | N                 | Method Should Exist                                         |
| UTC082-03 | N                 | Source Should Contain Method Declaration                    |
| UTC082-04 | N                 | Return Case Set Should Be Valid                             |
| UTC082-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC082-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC082-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F082_EmailService_SendEmailConfirmationAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F083 - EmailService.SendPasswordResetAsync

### 1) Function Header

| Field            | Value                                                                                                                                                    |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F083                                                                                                                                                     |
| Function Name    | SendPasswordResetAsync                                                                                                                                   |
| Class Name       | EmailService                                                                                                                                             |
| Method           | SendPasswordResetAsync                                                                                                                                   |
| Requirement      | Send password reset email                                                                                                                                |
| Description      | Validate 'Send password reset email' in EmailService.SendPasswordResetAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                        |
| Passed           | 7                                                                                                                                                        |
| Failed           | 0                                                                                                                                                        |
| Untested         | 0                                                                                                                                                        |
| N/A/B            | N=7, A=0, B=0                                                                                                                                            |

### 2) Condition + Precondition

- Precondition: SMTP settings are valid; recipient and template inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC083-01 |
| Method Should Exist                                         | UTC083-02 |
| Source Should Contain Method Declaration                    | UTC083-03 |
| Return Case Set Should Be Valid                             | UTC083-04 |
| Log Message Case Set Should Be Valid                        | UTC083-05 |
| When Logger Used Should Follow Log Message Convention       | UTC083-06 |
| When Result Response Used Should Follow Response Convention | UTC083-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC083-01 | N                 | Type Should Exist                                           |
| UTC083-02 | N                 | Method Should Exist                                         |
| UTC083-03 | N                 | Source Should Contain Method Declaration                    |
| UTC083-04 | N                 | Return Case Set Should Be Valid                             |
| UTC083-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC083-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC083-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F083_EmailService_SendPasswordResetAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F084 - EmailService.SendWelcomeEmailAsync

### 1) Function Header

| Field            | Value                                                                                                                                            |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F084                                                                                                                                             |
| Function Name    | SendWelcomeEmailAsync                                                                                                                            |
| Class Name       | EmailService                                                                                                                                     |
| Method           | SendWelcomeEmailAsync                                                                                                                            |
| Requirement      | Send welcome email                                                                                                                               |
| Description      | Validate 'Send welcome email' in EmailService.SendWelcomeEmailAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                |
| Passed           | 7                                                                                                                                                |
| Failed           | 0                                                                                                                                                |
| Untested         | 0                                                                                                                                                |
| N/A/B            | N=7, A=0, B=0                                                                                                                                    |

### 2) Condition + Precondition

- Precondition: SMTP settings are valid; recipient and template inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC084-01 |
| Method Should Exist                                         | UTC084-02 |
| Source Should Contain Method Declaration                    | UTC084-03 |
| Return Case Set Should Be Valid                             | UTC084-04 |
| Log Message Case Set Should Be Valid                        | UTC084-05 |
| When Logger Used Should Follow Log Message Convention       | UTC084-06 |
| When Result Response Used Should Follow Response Convention | UTC084-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC084-01 | N                 | Type Should Exist                                           |
| UTC084-02 | N                 | Method Should Exist                                         |
| UTC084-03 | N                 | Source Should Contain Method Declaration                    |
| UTC084-04 | N                 | Return Case Set Should Be Valid                             |
| UTC084-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC084-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC084-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F084_EmailService_SendWelcomeEmailAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F085 - EmailService.SendAsync

### 1) Function Header

| Field            | Value                                                                                                                                |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F085                                                                                                                                 |
| Function Name    | SendAsync                                                                                                                            |
| Class Name       | EmailService                                                                                                                         |
| Method           | SendAsync                                                                                                                            |
| Requirement      | Send generic email                                                                                                                   |
| Description      | Validate 'Send generic email' in EmailService.SendAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                    |
| Passed           | 7                                                                                                                                    |
| Failed           | 0                                                                                                                                    |
| Untested         | 0                                                                                                                                    |
| N/A/B            | N=7, A=0, B=0                                                                                                                        |

### 2) Condition + Precondition

- Precondition: SMTP settings are valid; recipient and template inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC085-01 |
| Method Should Exist                                         | UTC085-02 |
| Source Should Contain Method Declaration                    | UTC085-03 |
| Return Case Set Should Be Valid                             | UTC085-04 |
| Log Message Case Set Should Be Valid                        | UTC085-05 |
| When Logger Used Should Follow Log Message Convention       | UTC085-06 |
| When Result Response Used Should Follow Response Convention | UTC085-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- Failed to send email - To: {To}, Subject: {Subject}, Error: {Error}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC085-01 | N                 | Type Should Exist                                           |
| UTC085-02 | N                 | Method Should Exist                                         |
| UTC085-03 | N                 | Source Should Contain Method Declaration                    |
| UTC085-04 | N                 | Return Case Set Should Be Valid                             |
| UTC085-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC085-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC085-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F085_EmailService_SendAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F086 - GoogleMeetService.CreateMeetingAsync

### 1) Function Header

| Field            | Value                                                                                                                                                      |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F086                                                                                                                                                       |
| Function Name    | CreateMeetingAsync                                                                                                                                         |
| Class Name       | GoogleMeetService                                                                                                                                          |
| Method           | CreateMeetingAsync                                                                                                                                         |
| Requirement      | Create Google Meet meeting                                                                                                                                 |
| Description      | Validate 'Create Google Meet meeting' in GoogleMeetService.CreateMeetingAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                          |
| Passed           | 7                                                                                                                                                          |
| Failed           | 0                                                                                                                                                          |
| Untested         | 0                                                                                                                                                          |
| N/A/B            | N=7, A=0, B=0                                                                                                                                              |

### 2) Condition + Precondition

- Precondition: Google credentials and service configuration are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC086-01 |
| Method Should Exist                                         | UTC086-02 |
| Source Should Contain Method Declaration                    | UTC086-03 |
| Return Case Set Should Be Valid                             | UTC086-04 |
| Log Message Case Set Should Be Valid                        | UTC086-05 |
| When Logger Used Should Follow Log Message Convention       | UTC086-06 |
| When Result Response Used Should Follow Response Convention | UTC086-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new MeetingInfo(meetLink, createdEvent.Id)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC086-01 | N                 | Type Should Exist                                           |
| UTC086-02 | N                 | Method Should Exist                                         |
| UTC086-03 | N                 | Source Should Contain Method Declaration                    |
| UTC086-04 | N                 | Return Case Set Should Be Valid                             |
| UTC086-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC086-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC086-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F086_GoogleMeetService_CreateMeetingAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F087 - GoogleMeetService.DeleteMeetingAsync

### 1) Function Header

| Field            | Value                                                                                                                                                      |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F087                                                                                                                                                       |
| Function Name    | DeleteMeetingAsync                                                                                                                                         |
| Class Name       | GoogleMeetService                                                                                                                                          |
| Method           | DeleteMeetingAsync                                                                                                                                         |
| Requirement      | Delete Google Meet meeting                                                                                                                                 |
| Description      | Validate 'Delete Google Meet meeting' in GoogleMeetService.DeleteMeetingAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                          |
| Passed           | 7                                                                                                                                                          |
| Failed           | 0                                                                                                                                                          |
| Untested         | 0                                                                                                                                                          |
| N/A/B            | N=7, A=0, B=0                                                                                                                                              |

### 2) Condition + Precondition

- Precondition: Google credentials and service configuration are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC087-01 |
| Method Should Exist                                         | UTC087-02 |
| Source Should Contain Method Declaration                    | UTC087-03 |
| Return Case Set Should Be Valid                             | UTC087-04 |
| Log Message Case Set Should Be Valid                        | UTC087-05 |
| When Logger Used Should Follow Log Message Convention       | UTC087-06 |
| When Result Response Used Should Follow Response Convention | UTC087-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- Deleted Calendar event {EventId}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC087-01 | N                 | Type Should Exist                                           |
| UTC087-02 | N                 | Method Should Exist                                         |
| UTC087-03 | N                 | Source Should Contain Method Declaration                    |
| UTC087-04 | N                 | Return Case Set Should Be Valid                             |
| UTC087-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC087-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC087-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F087_GoogleMeetService_DeleteMeetingAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F088 - GoogleMeetService.Dispose

### 1) Function Header

| Field            | Value                                                                                                                                            |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F088                                                                                                                                             |
| Function Name    | Dispose                                                                                                                                          |
| Class Name       | GoogleMeetService                                                                                                                                |
| Method           | Dispose                                                                                                                                          |
| Requirement      | Dispose Google Meet service                                                                                                                      |
| Description      | Validate 'Dispose Google Meet service' in GoogleMeetService.Dispose, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                |
| Passed           | 7                                                                                                                                                |
| Failed           | 0                                                                                                                                                |
| Untested         | 0                                                                                                                                                |
| N/A/B            | N=7, A=0, B=0                                                                                                                                    |

### 2) Condition + Precondition

- Precondition: Google credentials and service configuration are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC088-01 |
| Method Should Exist                                         | UTC088-02 |
| Source Should Contain Method Declaration                    | UTC088-03 |
| Return Case Set Should Be Valid                             | UTC088-04 |
| Log Message Case Set Should Be Valid                        | UTC088-05 |
| When Logger Used Should Follow Log Message Convention       | UTC088-06 |
| When Result Response Used Should Follow Response Convention | UTC088-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC088-01 | N                 | Type Should Exist                                           |
| UTC088-02 | N                 | Method Should Exist                                         |
| UTC088-03 | N                 | Source Should Contain Method Declaration                    |
| UTC088-04 | N                 | Return Case Set Should Be Valid                             |
| UTC088-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC088-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC088-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F088_GoogleMeetService_Dispose_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F089 - NotificationService.SendAsync (typed)

### 1) Function Header

| Field            | Value                                                                                                                                                    |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F089                                                                                                                                                     |
| Function Name    | SendAsync (typed)                                                                                                                                        |
| Class Name       | NotificationService                                                                                                                                      |
| Method           | SendAsync (typed)                                                                                                                                        |
| Requirement      | Send typed notification                                                                                                                                  |
| Description      | Validate 'Send typed notification' in NotificationService.SendAsync (typed), covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                        |
| Passed           | 7                                                                                                                                                        |
| Failed           | 0                                                                                                                                                        |
| Untested         | 0                                                                                                                                                        |
| N/A/B            | N=7, A=0, B=0                                                                                                                                            |

### 2) Condition + Precondition

- Precondition: Notification repository, UnitOfWork, and hub service are mocked.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC089-01 |
| Method Should Exist                                         | UTC089-02 |
| Source Should Contain Method Declaration                    | UTC089-03 |
| Return Case Set Should Be Valid                             | UTC089-04 |
| Log Message Case Set Should Be Valid                        | UTC089-05 |
| When Logger Used Should Follow Log Message Convention       | UTC089-06 |
| When Result Response Used Should Follow Response Convention | UTC089-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- Failed to send notification to User {UserId}: {Message}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC089-01 | N                 | Type Should Exist                                           |
| UTC089-02 | N                 | Method Should Exist                                         |
| UTC089-03 | N                 | Source Should Contain Method Declaration                    |
| UTC089-04 | N                 | Return Case Set Should Be Valid                             |
| UTC089-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC089-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC089-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F089_NotificationService_SendAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F090 - NotificationService.SendAsync (legacy)

### 1) Function Header

| Field            | Value                                                                                                                                                      |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F090                                                                                                                                                       |
| Function Name    | SendAsync (legacy)                                                                                                                                         |
| Class Name       | NotificationService                                                                                                                                        |
| Method           | SendAsync (legacy)                                                                                                                                         |
| Requirement      | Send legacy notification                                                                                                                                   |
| Description      | Validate 'Send legacy notification' in NotificationService.SendAsync (legacy), covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                          |
| Passed           | 7                                                                                                                                                          |
| Failed           | 0                                                                                                                                                          |
| Untested         | 0                                                                                                                                                          |
| N/A/B            | N=7, A=0, B=0                                                                                                                                              |

### 2) Condition + Precondition

- Precondition: Notification repository, UnitOfWork, and hub service are mocked.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC090-01 |
| Method Should Exist                                         | UTC090-02 |
| Source Should Contain Method Declaration                    | UTC090-03 |
| Return Case Set Should Be Valid                             | UTC090-04 |
| Log Message Case Set Should Be Valid                        | UTC090-05 |
| When Logger Used Should Follow Log Message Convention       | UTC090-06 |
| When Result Response Used Should Follow Response Convention | UTC090-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC090-01 | N                 | Type Should Exist                                           |
| UTC090-02 | N                 | Method Should Exist                                         |
| UTC090-03 | N                 | Source Should Contain Method Declaration                    |
| UTC090-04 | N                 | Return Case Set Should Be Valid                             |
| UTC090-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC090-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC090-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F090_NotificationService_SendAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F091 - OrganisationOnboardingService.SubmitRequestAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                 |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F091                                                                                                                                                                  |
| Function Name    | SubmitRequestAsync                                                                                                                                                    |
| Class Name       | OrganisationOnboardingService                                                                                                                                         |
| Method           | SubmitRequestAsync                                                                                                                                                    |
| Requirement      | Submit onboarding request                                                                                                                                             |
| Description      | Validate 'Submit onboarding request' in OrganisationOnboardingService.SubmitRequestAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                     |
| Passed           | 7                                                                                                                                                                     |
| Failed           | 0                                                                                                                                                                     |
| Untested         | 0                                                                                                                                                                     |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                         |

### 2) Condition + Precondition

- Precondition: Onboarding repositories, UserManager, EmailService, and UnitOfWork are mocked or seeded.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC091-01 |
| Method Should Exist                                         | UTC091-02 |
| Source Should Contain Method Declaration                    | UTC091-03 |
| Return Case Set Should Be Valid                             | UTC091-04 |
| Log Message Case Set Should Be Valid                        | UTC091-05 |
| When Logger Used Should Follow Log Message Convention       | UTC091-06 |
| When Result Response Used Should Follow Response Convention | UTC091-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result<OrganisationRegistrationResponse>.Conflict( "A pending organisation onboarding request already exists for this email.")

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC091-01 | N                 | Type Should Exist                                           |
| UTC091-02 | N                 | Method Should Exist                                         |
| UTC091-03 | N                 | Source Should Contain Method Declaration                    |
| UTC091-04 | N                 | Return Case Set Should Be Valid                             |
| UTC091-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC091-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC091-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F091_OrganisationOnboardingService_SubmitRequestAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F092 - OrganisationOnboardingService.GetRequestsAsync

### 1) Function Header

| Field            | Value                                                                                                                                                             |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F092                                                                                                                                                              |
| Function Name    | GetRequestsAsync                                                                                                                                                  |
| Class Name       | OrganisationOnboardingService                                                                                                                                     |
| Method           | GetRequestsAsync                                                                                                                                                  |
| Requirement      | Get onboarding requests                                                                                                                                           |
| Description      | Validate 'Get onboarding requests' in OrganisationOnboardingService.GetRequestsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                 |
| Passed           | 7                                                                                                                                                                 |
| Failed           | 0                                                                                                                                                                 |
| Untested         | 0                                                                                                                                                                 |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                     |

### 2) Condition + Precondition

- Precondition: Onboarding repositories, UserManager, EmailService, and UnitOfWork are mocked or seeded.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC092-01 |
| Method Should Exist                                         | UTC092-02 |
| Source Should Contain Method Declaration                    | UTC092-03 |
| Return Case Set Should Be Valid                             | UTC092-04 |
| Log Message Case Set Should Be Valid                        | UTC092-05 |
| When Logger Used Should Follow Log Message Convention       | UTC092-06 |
| When Result Response Used Should Follow Response Convention | UTC092-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result<IReadOnlyList<OrganisationOnboardingRequestDto>>.Success(items)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC092-01 | N                 | Type Should Exist                                           |
| UTC092-02 | N                 | Method Should Exist                                         |
| UTC092-03 | N                 | Source Should Contain Method Declaration                    |
| UTC092-04 | N                 | Return Case Set Should Be Valid                             |
| UTC092-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC092-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC092-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F092_OrganisationOnboardingService_GetRequestsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F093 - OrganisationOnboardingService.ApproveRequestAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                   |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F093                                                                                                                                                                    |
| Function Name    | ApproveRequestAsync                                                                                                                                                     |
| Class Name       | OrganisationOnboardingService                                                                                                                                           |
| Method           | ApproveRequestAsync                                                                                                                                                     |
| Requirement      | Approve onboarding request                                                                                                                                              |
| Description      | Validate 'Approve onboarding request' in OrganisationOnboardingService.ApproveRequestAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                       |
| Passed           | 7                                                                                                                                                                       |
| Failed           | 0                                                                                                                                                                       |
| Untested         | 0                                                                                                                                                                       |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                           |

### 2) Condition + Precondition

- Precondition: Onboarding repositories, UserManager, EmailService, and UnitOfWork are mocked or seeded.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC093-01 |
| Method Should Exist                                         | UTC093-02 |
| Source Should Contain Method Declaration                    | UTC093-03 |
| Return Case Set Should Be Valid                             | UTC093-04 |
| Log Message Case Set Should Be Valid                        | UTC093-05 |
| When Logger Used Should Follow Log Message Convention       | UTC093-06 |
| When Result Response Used Should Follow Response Convention | UTC093-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result<ApproveOrganisationOnboardingResult>.NotFound( "Organisation onboarding request not found.")

#### Exception

- No dedicated exception case asserted.

#### Log message

- Failed to approve organisation onboarding request {RequestId}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC093-01 | N                 | Type Should Exist                                           |
| UTC093-02 | N                 | Method Should Exist                                         |
| UTC093-03 | N                 | Source Should Contain Method Declaration                    |
| UTC093-04 | N                 | Return Case Set Should Be Valid                             |
| UTC093-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC093-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC093-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F093_OrganisationOnboardingService_ApproveRequestAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F094 - PatientRoadmapGenerationService.GenerateFromDiagnosisAsync

### 1) Function Header

| Field            | Value                                                                                                                                                                                 |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F094                                                                                                                                                                                  |
| Function Name    | GenerateFromDiagnosisAsync                                                                                                                                                            |
| Class Name       | PatientRoadmapGenerationService                                                                                                                                                       |
| Method           | GenerateFromDiagnosisAsync                                                                                                                                                            |
| Requirement      | Generate roadmap from diagnosis                                                                                                                                                       |
| Description      | Validate 'Generate roadmap from diagnosis' in PatientRoadmapGenerationService.GenerateFromDiagnosisAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                                     |
| Passed           | 7                                                                                                                                                                                     |
| Failed           | 0                                                                                                                                                                                     |
| Untested         | 0                                                                                                                                                                                     |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                                         |

### 2) Condition + Precondition

- Precondition: Diagnosis input and roadmap mapping rules are prepared.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC094-01 |
| Method Should Exist                                         | UTC094-02 |
| Source Should Contain Method Declaration                    | UTC094-03 |
| Return Case Set Should Be Valid                             | UTC094-04 |
| Log Message Case Set Should Be Valid                        | UTC094-05 |
| When Logger Used Should Follow Log Message Convention       | UTC094-06 |
| When Result Response Used Should Follow Response Convention | UTC094-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result<GeneratedPatientRoadmap>.Failure("Patient ID is required for roadmap generation.")

#### Exception

- No dedicated exception case asserted.

#### Log message

- Patient roadmap generation attempt {Attempt}/{MaxAttempts} failed

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC094-01 | N                 | Type Should Exist                                           |
| UTC094-02 | N                 | Method Should Exist                                         |
| UTC094-03 | N                 | Source Should Contain Method Declaration                    |
| UTC094-04 | N                 | Return Case Set Should Be Valid                             |
| UTC094-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC094-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC094-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F094_PatientRoadmapGenerationService_GenerateFromDiagnosisAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F095 - PayOSService.CreatePaymentLinkAsync

### 1) Function Header

| Field            | Value                                                                                                                                                    |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F095                                                                                                                                                     |
| Function Name    | CreatePaymentLinkAsync                                                                                                                                   |
| Class Name       | PayOSService                                                                                                                                             |
| Method           | CreatePaymentLinkAsync                                                                                                                                   |
| Requirement      | Create PayOS payment link                                                                                                                                |
| Description      | Validate 'Create PayOS payment link' in PayOSService.CreatePaymentLinkAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                        |
| Passed           | 7                                                                                                                                                        |
| Failed           | 0                                                                                                                                                        |
| Untested         | 0                                                                                                                                                        |
| N/A/B            | N=7, A=0, B=0                                                                                                                                            |

### 2) Condition + Precondition

- Precondition: PayOS settings and payment payload inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC095-01 |
| Method Should Exist                                         | UTC095-02 |
| Source Should Contain Method Declaration                    | UTC095-03 |
| Return Case Set Should Be Valid                             | UTC095-04 |
| Log Message Case Set Should Be Valid                        | UTC095-05 |
| When Logger Used Should Follow Log Message Convention       | UTC095-06 |
| When Result Response Used Should Follow Response Convention | UTC095-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- (response.checkoutUrl, orderCode.ToString())

#### Exception

- No dedicated exception case asserted.

#### Log message

- Calling PayOS SDK to create payment link

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC095-01 | N                 | Type Should Exist                                           |
| UTC095-02 | N                 | Method Should Exist                                         |
| UTC095-03 | N                 | Source Should Contain Method Declaration                    |
| UTC095-04 | N                 | Return Case Set Should Be Valid                             |
| UTC095-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC095-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC095-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F095_PayOSService_CreatePaymentLinkAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F096 - PayOSService.GetPaymentStatusAsync

### 1) Function Header

| Field            | Value                                                                                                                                                  |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F096                                                                                                                                                   |
| Function Name    | GetPaymentStatusAsync                                                                                                                                  |
| Class Name       | PayOSService                                                                                                                                           |
| Method           | GetPaymentStatusAsync                                                                                                                                  |
| Requirement      | Get PayOS payment status                                                                                                                               |
| Description      | Validate 'Get PayOS payment status' in PayOSService.GetPaymentStatusAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                      |
| Passed           | 7                                                                                                                                                      |
| Failed           | 0                                                                                                                                                      |
| Untested         | 0                                                                                                                                                      |
| N/A/B            | N=7, A=0, B=0                                                                                                                                          |

### 2) Condition + Precondition

- Precondition: PayOS settings and payment payload inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC096-01 |
| Method Should Exist                                         | UTC096-02 |
| Source Should Contain Method Declaration                    | UTC096-03 |
| Return Case Set Should Be Valid                             | UTC096-04 |
| Log Message Case Set Should Be Valid                        | UTC096-05 |
| When Logger Used Should Follow Log Message Convention       | UTC096-06 |
| When Result Response Used Should Follow Response Convention | UTC096-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- (status, amount, txnRef)

#### Exception

- No dedicated exception case asserted.

#### Log message

- Querying PayOS payment status for OrderCode={OrderCode}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC096-01 | N                 | Type Should Exist                                           |
| UTC096-02 | N                 | Method Should Exist                                         |
| UTC096-03 | N                 | Source Should Contain Method Declaration                    |
| UTC096-04 | N                 | Return Case Set Should Be Valid                             |
| UTC096-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC096-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC096-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F096_PayOSService_GetPaymentStatusAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F097 - PayOSService.VerifyWebhookSignatureAsync

### 1) Function Header

| Field            | Value                                                                                                                                                              |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F097                                                                                                                                                               |
| Function Name    | VerifyWebhookSignatureAsync                                                                                                                                        |
| Class Name       | PayOSService                                                                                                                                                       |
| Method           | VerifyWebhookSignatureAsync                                                                                                                                        |
| Requirement      | Verify PayOS webhook signature                                                                                                                                     |
| Description      | Validate 'Verify PayOS webhook signature' in PayOSService.VerifyWebhookSignatureAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                                  |
| Passed           | 7                                                                                                                                                                  |
| Failed           | 0                                                                                                                                                                  |
| Untested         | 0                                                                                                                                                                  |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                      |

### 2) Condition + Precondition

- Precondition: PayOS settings and payment payload inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC097-01 |
| Method Should Exist                                         | UTC097-02 |
| Source Should Contain Method Declaration                    | UTC097-03 |
| Return Case Set Should Be Valid                             | UTC097-04 |
| Log Message Case Set Should Be Valid                        | UTC097-05 |
| When Logger Used Should Follow Log Message Convention       | UTC097-06 |
| When Result Response Used Should Follow Response Convention | UTC097-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Task.FromResult(true)

#### Exception

- No dedicated exception case asserted.

#### Log message

- Verifying PayOS webhook signature

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC097-01 | N                 | Type Should Exist                                           |
| UTC097-02 | N                 | Method Should Exist                                         |
| UTC097-03 | N                 | Source Should Contain Method Declaration                    |
| UTC097-04 | N                 | Return Case Set Should Be Valid                             |
| UTC097-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC097-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC097-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F097_PayOSService_VerifyWebhookSignatureAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F098 - PayOSService.CancelPaymentAsync

### 1) Function Header

| Field            | Value                                                                                                                                           |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F098                                                                                                                                            |
| Function Name    | CancelPaymentAsync                                                                                                                              |
| Class Name       | PayOSService                                                                                                                                    |
| Method           | CancelPaymentAsync                                                                                                                              |
| Requirement      | Cancel PayOS payment                                                                                                                            |
| Description      | Validate 'Cancel PayOS payment' in PayOSService.CancelPaymentAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                               |
| Passed           | 7                                                                                                                                               |
| Failed           | 0                                                                                                                                               |
| Untested         | 0                                                                                                                                               |
| N/A/B            | N=7, A=0, B=0                                                                                                                                   |

### 2) Condition + Precondition

- Precondition: PayOS settings and payment payload inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC098-01 |
| Method Should Exist                                         | UTC098-02 |
| Source Should Contain Method Declaration                    | UTC098-03 |
| Return Case Set Should Be Valid                             | UTC098-04 |
| Log Message Case Set Should Be Valid                        | UTC098-05 |
| When Logger Used Should Follow Log Message Convention       | UTC098-06 |
| When Result Response Used Should Follow Response Convention | UTC098-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- true

#### Exception

- No dedicated exception case asserted.

#### Log message

- Cancelling PayOS payment: OrderCode={OrderCode}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC098-01 | N                 | Type Should Exist                                           |
| UTC098-02 | N                 | Method Should Exist                                         |
| UTC098-03 | N                 | Source Should Contain Method Declaration                    |
| UTC098-04 | N                 | Return Case Set Should Be Valid                             |
| UTC098-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC098-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC098-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F098_PayOSService_CancelPaymentAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F099 - SlotMaintenanceJob.ExpireUnusedSlotsAsync

### 1) Function Header

| Field            | Value                                                                                                                                                         |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F099                                                                                                                                                          |
| Function Name    | ExpireUnusedSlotsAsync                                                                                                                                        |
| Class Name       | SlotMaintenanceJob                                                                                                                                            |
| Method           | ExpireUnusedSlotsAsync                                                                                                                                        |
| Requirement      | Execute slot maintenance                                                                                                                                      |
| Description      | Validate 'Execute slot maintenance' in SlotMaintenanceJob.ExpireUnusedSlotsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                             |
| Passed           | 7                                                                                                                                                             |
| Failed           | 0                                                                                                                                                             |
| Untested         | 0                                                                                                                                                             |
| N/A/B            | N=7, A=0, B=0                                                                                                                                                 |

### 2) Condition + Precondition

- Precondition: Slot data is seeded with time-based scenarios and the job context is ready.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC099-01 |
| Method Should Exist                                         | UTC099-02 |
| Source Should Contain Method Declaration                    | UTC099-03 |
| Return Case Set Should Be Valid                             | UTC099-04 |
| Log Message Case Set Should Be Valid                        | UTC099-05 |
| When Logger Used Should Follow Log Message Convention       | UTC099-06 |
| When Result Response Used Should Follow Response Convention | UTC099-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC099-01 | N                 | Type Should Exist                                           |
| UTC099-02 | N                 | Method Should Exist                                         |
| UTC099-03 | N                 | Source Should Contain Method Declaration                    |
| UTC099-04 | N                 | Return Case Set Should Be Valid                             |
| UTC099-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC099-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC099-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F099_SlotMaintenanceJob_ExpireUnusedSlotsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F100 - SupabaseStorageService.SaveFileAsync

### 1) Function Header

| Field            | Value                                                                                                                                                 |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F100                                                                                                                                                  |
| Function Name    | SaveFileAsync                                                                                                                                         |
| Class Name       | SupabaseStorageService                                                                                                                                |
| Method           | SaveFileAsync                                                                                                                                         |
| Requirement      | Save file to Supabase                                                                                                                                 |
| Description      | Validate 'Save file to Supabase' in SupabaseStorageService.SaveFileAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                     |
| Passed           | 7                                                                                                                                                     |
| Failed           | 0                                                                                                                                                     |
| Untested         | 0                                                                                                                                                     |
| N/A/B            | N=7, A=0, B=0                                                                                                                                         |

### 2) Condition + Precondition

- Precondition: Supabase settings are valid; stream and path inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC100-01 |
| Method Should Exist                                         | UTC100-02 |
| Source Should Contain Method Declaration                    | UTC100-03 |
| Return Case Set Should Be Valid                             | UTC100-04 |
| Log Message Case Set Should Be Valid                        | UTC100-05 |
| When Logger Used Should Follow Log Message Convention       | UTC100-06 |
| When Result Response Used Should Follow Response Convention | UTC100-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- publicUrl

#### Exception

- No dedicated exception case asserted.

#### Log message

- Uploaded to Supabase Storage: {Url}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC100-01 | N                 | Type Should Exist                                           |
| UTC100-02 | N                 | Method Should Exist                                         |
| UTC100-03 | N                 | Source Should Contain Method Declaration                    |
| UTC100-04 | N                 | Return Case Set Should Be Valid                             |
| UTC100-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC100-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC100-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F100_SupabaseStorageService_SaveFileAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F101 - SupabaseStorageService.DeleteFile

### 1) Function Header

| Field            | Value                                                                                                                                                  |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F101                                                                                                                                                   |
| Function Name    | DeleteFile                                                                                                                                             |
| Class Name       | SupabaseStorageService                                                                                                                                 |
| Method           | DeleteFile                                                                                                                                             |
| Requirement      | Delete file from Supabase                                                                                                                              |
| Description      | Validate 'Delete file from Supabase' in SupabaseStorageService.DeleteFile, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                      |
| Passed           | 7                                                                                                                                                      |
| Failed           | 0                                                                                                                                                      |
| Untested         | 0                                                                                                                                                      |
| N/A/B            | N=7, A=0, B=0                                                                                                                                          |

### 2) Condition + Precondition

- Precondition: Supabase settings are valid; stream and path inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC101-01 |
| Method Should Exist                                         | UTC101-02 |
| Source Should Contain Method Declaration                    | UTC101-03 |
| Return Case Set Should Be Valid                             | UTC101-04 |
| Log Message Case Set Should Be Valid                        | UTC101-05 |
| When Logger Used Should Follow Log Message Convention       | UTC101-06 |
| When Result Response Used Should Follow Response Convention | UTC101-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- false

#### Exception

- No dedicated exception case asserted.

#### Log message

- Deleted file from Supabase Storage: {Path}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC101-01 | N                 | Type Should Exist                                           |
| UTC101-02 | N                 | Method Should Exist                                         |
| UTC101-03 | N                 | Source Should Contain Method Declaration                    |
| UTC101-04 | N                 | Return Case Set Should Be Valid                             |
| UTC101-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC101-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC101-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F101_SupabaseStorageService_DeleteFile_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F102 - SupabaseStorageService.FileExists

### 1) Function Header

| Field            | Value                                                                                                                                                   |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F102                                                                                                                                                    |
| Function Name    | FileExists                                                                                                                                              |
| Class Name       | SupabaseStorageService                                                                                                                                  |
| Method           | FileExists                                                                                                                                              |
| Requirement      | Check Supabase file exists                                                                                                                              |
| Description      | Validate 'Check Supabase file exists' in SupabaseStorageService.FileExists, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                       |
| Passed           | 7                                                                                                                                                       |
| Failed           | 0                                                                                                                                                       |
| Untested         | 0                                                                                                                                                       |
| N/A/B            | N=7, A=0, B=0                                                                                                                                           |

### 2) Condition + Precondition

- Precondition: Supabase settings are valid; stream and path inputs are valid.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC102-01 |
| Method Should Exist                                         | UTC102-02 |
| Source Should Contain Method Declaration                    | UTC102-03 |
| Return Case Set Should Be Valid                             | UTC102-04 |
| Log Message Case Set Should Be Valid                        | UTC102-05 |
| When Logger Used Should Follow Log Message Convention       | UTC102-06 |
| When Result Response Used Should Follow Response Convention | UTC102-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- false

#### Exception

- No dedicated exception case asserted.

#### Log message

- Error checking file existence: {Path}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC102-01 | N                 | Type Should Exist                                           |
| UTC102-02 | N                 | Method Should Exist                                         |
| UTC102-03 | N                 | Source Should Contain Method Declaration                    |
| UTC102-04 | N                 | Return Case Set Should Be Valid                             |
| UTC102-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC102-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC102-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F102_SupabaseStorageService_FileExists_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F103 - SystemSettingService.GetSettingAsync

### 1) Function Header

| Field            | Value                                                                                                                                              |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F103                                                                                                                                               |
| Function Name    | GetSettingAsync                                                                                                                                    |
| Class Name       | SystemSettingService                                                                                                                               |
| Method           | GetSettingAsync                                                                                                                                    |
| Requirement      | Get setting by key                                                                                                                                 |
| Description      | Validate 'Get setting by key' in SystemSettingService.GetSettingAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                  |
| Passed           | 7                                                                                                                                                  |
| Failed           | 0                                                                                                                                                  |
| Untested         | 0                                                                                                                                                  |
| N/A/B            | N=7, A=0, B=0                                                                                                                                      |

### 2) Condition + Precondition

- Precondition: System-setting repository and UnitOfWork are seeded with valid key/value data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC103-01 |
| Method Should Exist                                         | UTC103-02 |
| Source Should Contain Method Declaration                    | UTC103-03 |
| Return Case Set Should Be Valid                             | UTC103-04 |
| Log Message Case Set Should Be Valid                        | UTC103-05 |
| When Logger Used Should Follow Log Message Convention       | UTC103-06 |
| When Result Response Used Should Follow Response Convention | UTC103-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- setting?.Value

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC103-01 | N                 | Type Should Exist                                           |
| UTC103-02 | N                 | Method Should Exist                                         |
| UTC103-03 | N                 | Source Should Contain Method Declaration                    |
| UTC103-04 | N                 | Return Case Set Should Be Valid                             |
| UTC103-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC103-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC103-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F103_SystemSettingService_GetSettingAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F104 - SystemSettingService.GetAllSettingsAsync

### 1) Function Header

| Field            | Value                                                                                                                                                |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F104                                                                                                                                                 |
| Function Name    | GetAllSettingsAsync                                                                                                                                  |
| Class Name       | SystemSettingService                                                                                                                                 |
| Method           | GetAllSettingsAsync                                                                                                                                  |
| Requirement      | Get all settings                                                                                                                                     |
| Description      | Validate 'Get all settings' in SystemSettingService.GetAllSettingsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                    |
| Passed           | 7                                                                                                                                                    |
| Failed           | 0                                                                                                                                                    |
| Untested         | 0                                                                                                                                                    |
| N/A/B            | N=7, A=0, B=0                                                                                                                                        |

### 2) Condition + Precondition

- Precondition: System-setting repository and UnitOfWork are seeded with valid key/value data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC104-01 |
| Method Should Exist                                         | UTC104-02 |
| Source Should Contain Method Declaration                    | UTC104-03 |
| Return Case Set Should Be Valid                             | UTC104-04 |
| Log Message Case Set Should Be Valid                        | UTC104-05 |
| When Logger Used Should Follow Log Message Convention       | UTC104-06 |
| When Result Response Used Should Follow Response Convention | UTC104-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- settingsList.ToDictionary(s => s.Key, s => s.Value)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC104-01 | N                 | Type Should Exist                                           |
| UTC104-02 | N                 | Method Should Exist                                         |
| UTC104-03 | N                 | Source Should Contain Method Declaration                    |
| UTC104-04 | N                 | Return Case Set Should Be Valid                             |
| UTC104-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC104-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC104-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F104_SystemSettingService_GetAllSettingsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---

## F105 - SystemSettingService.UpdateSettingsAsync

### 1) Function Header

| Field            | Value                                                                                                                                               |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F105                                                                                                                                                |
| Function Name    | UpdateSettingsAsync                                                                                                                                 |
| Class Name       | SystemSettingService                                                                                                                                |
| Method           | UpdateSettingsAsync                                                                                                                                 |
| Requirement      | Update settings                                                                                                                                     |
| Description      | Validate 'Update settings' in SystemSettingService.UpdateSettingsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7                                                                                                                                                   |
| Passed           | 7                                                                                                                                                   |
| Failed           | 0                                                                                                                                                   |
| Untested         | 0                                                                                                                                                   |
| N/A/B            | N=7, A=0, B=0                                                                                                                                       |

### 2) Condition + Precondition

- Precondition: System-setting repository and UnitOfWork are seeded with valid key/value data.

| Condition Item                                              | UTC Ref   |
| ----------------------------------------------------------- | --------- |
| Type Should Exist                                           | UTC105-01 |
| Method Should Exist                                         | UTC105-02 |
| Source Should Contain Method Declaration                    | UTC105-03 |
| Return Case Set Should Be Valid                             | UTC105-04 |
| Log Message Case Set Should Be Valid                        | UTC105-05 |
| When Logger Used Should Follow Log Message Convention       | UTC105-06 |
| When Result Response Used Should Follow Response Convention | UTC105-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- System settings updated for keys: {Keys}

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                 |
| --------- | ----------------- | ----------------------------------------------------------- |
| UTC105-01 | N                 | Type Should Exist                                           |
| UTC105-02 | N                 | Method Should Exist                                         |
| UTC105-03 | N                 | Source Should Contain Method Declaration                    |
| UTC105-04 | N                 | Return Case Set Should Be Valid                             |
| UTC105-05 | N                 | Log Message Case Set Should Be Valid                        |
| UTC105-06 | N                 | When Logger Used Should Follow Log Message Convention       |
| UTC105-07 | N                 | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F105_SystemSettingService_UpdateSettingsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md
