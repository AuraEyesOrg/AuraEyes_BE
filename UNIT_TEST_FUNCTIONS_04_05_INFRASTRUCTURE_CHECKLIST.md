# Unit test checklist - Infrastructure services/functions (UTC thuc)

Ngay cap nhat: 05/04/2026
Scope: d:/sep/AuraEyes_BE/src/Infrastructure/Identity + src/Infrastructure/Services

## Trang thai hien tai

- Da tao lai project test: tests/Infrastructure.UnitTests.
- Scope checklist da bo cac case fake (FakeEmailService, FakePayOSService).
- Tat ca function da duoc nang cap bo case theo huong Condition / Return / Log message / Response convention; LoginAsync (F009) giu bo case chi tiet theo mau.

## Kiem tra lai theo "services only" (cap nhat)

- Yeu cau moi: chi tinh function trong cac service (`*Service`), KHONG tinh entity function.
- Cac muc khong thuoc service can loai khoi checklist "services only":
  - `F001`-`F004` (ApplicationUser methods)
  - `F043`-`F045` (RefreshToken entity methods)
  - `F071`, `F099` (Job/worker methods, khong phai service)
- Sau khi loai bo cac muc tren, checklist "services only" con lai la nhom function service de viet unit test.

## Function Catalog (for Sheet Import)

| No  | Requirement Name                    | Class Name                      | Function Name                         | Function Code(Optional) | Sheet Name                            | Description                                                                                                                                                                       | Pre-Condition                                                                                                                      |
| --- | ----------------------------------- | ------------------------------- | ------------------------------------- | ----------------------- | ------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| 5   | Register patient account            | AuthService                     | RegisterPatientAsync                  | F005                    | RegisterPatientAsync                  | Validate 'Register patient account' in AuthService.RegisterPatientAsync, covering success/failure flow, response contract, and logging behavior.                                  | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 6   | Register ophthalmologist account    | AuthService                     | RegisterOphthalmologistAsync          | F006                    | RegisterOphthalmologistAsync          | Validate 'Register ophthalmologist account' in AuthService.RegisterOphthalmologistAsync, covering success/failure flow, response contract, and logging behavior.                  | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 7   | Register organisation account       | AuthService                     | RegisterOrganisationAsync             | F007                    | RegisterOrganisationAsync             | Validate 'Register organisation account' in AuthService.RegisterOrganisationAsync, covering success/failure flow, response contract, and logging behavior.                        | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 8   | Google login                        | AuthService                     | GoogleLoginAsync                      | F008                    | GoogleLoginAsync                      | Validate 'Google login' in AuthService.GoogleLoginAsync, covering success/failure flow, response contract, and logging behavior.                                                  | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 9   | Login with password                 | AuthService                     | LoginAsync                            | F009                    | LoginAsync                            | Validate 'Login with password' in AuthService.LoginAsync, covering success/failure flow, response contract, and logging behavior.                                                 | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 10  | Verify 2FA login                    | AuthService                     | VerifyTwoFactorLoginAsync             | F010                    | VerifyTwoFactorLoginAsync             | Validate 'Verify 2FA login' in AuthService.VerifyTwoFactorLoginAsync, covering success/failure flow, response contract, and logging behavior.                                     | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 11  | Refresh token flow                  | AuthService                     | RefreshTokenAsync                     | F011                    | RefreshTokenAsync                     | Validate 'Refresh token flow' in AuthService.RefreshTokenAsync, covering success/failure flow, response contract, and logging behavior.                                           | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 12  | Logout single device                | AuthService                     | LogoutAsync                           | F012                    | LogoutAsync                           | Validate 'Logout single device' in AuthService.LogoutAsync, covering success/failure flow, response contract, and logging behavior.                                               | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 13  | Logout all devices                  | AuthService                     | LogoutAllAsync                        | F013                    | LogoutAllAsync                        | Validate 'Logout all devices' in AuthService.LogoutAllAsync, covering success/failure flow, response contract, and logging behavior.                                              | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 14  | Confirm email                       | AuthService                     | ConfirmEmailAsync                     | F014                    | ConfirmEmailAsync                     | Validate 'Confirm email' in AuthService.ConfirmEmailAsync, covering success/failure flow, response contract, and logging behavior.                                                | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 15  | Forgot password                     | AuthService                     | ForgotPasswordAsync                   | F015                    | ForgotPasswordAsync                   | Validate 'Forgot password' in AuthService.ForgotPasswordAsync, covering success/failure flow, response contract, and logging behavior.                                            | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 16  | Reset password                      | AuthService                     | ResetPasswordAsync                    | F016                    | ResetPasswordAsync                    | Validate 'Reset password' in AuthService.ResetPasswordAsync, covering success/failure flow, response contract, and logging behavior.                                              | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 17  | Get current user                    | AuthService                     | GetCurrentUserAsync                   | F017                    | GetCurrentUserAsync                   | Validate 'Get current user' in AuthService.GetCurrentUserAsync, covering success/failure flow, response contract, and logging behavior.                                           | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 18  | Resend confirmation email           | AuthService                     | ResendConfirmationAsync               | F018                    | ResendConfirmationAsync               | Validate 'Resend confirmation email' in AuthService.ResendConfirmationAsync, covering success/failure flow, response contract, and logging behavior.                              | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 19  | Check password                      | IdentityService                 | CheckPasswordAsync                    | F019                    | CheckPasswordAsync                    | Validate 'Check password' in IdentityService.CheckPasswordAsync, covering success/failure flow, response contract, and logging behavior.                                          | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 20  | Get user by email                   | IdentityService                 | GetUserByEmailAsync                   | F020                    | GetUserByEmailAsync                   | Validate 'Get user by email' in IdentityService.GetUserByEmailAsync, covering success/failure flow, response contract, and logging behavior.                                      | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 21  | Get user by id                      | IdentityService                 | GetUserByIdAsync                      | F021                    | GetUserByIdAsync                      | Validate 'Get user by id' in IdentityService.GetUserByIdAsync, covering success/failure flow, response contract, and logging behavior.                                            | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 22  | Check organization phone in use     | IdentityService                 | IsPhoneNumberInUseByOrganizationAsync | F022                    | IsPhoneNumberInUseByOrganizationAsync | Validate 'Check organization phone in use' in IdentityService.IsPhoneNumberInUseByOrganizationAsync, covering success/failure flow, response contract, and logging behavior.      | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 23  | Check email confirmed               | IdentityService                 | IsEmailConfirmedAsync                 | F023                    | IsEmailConfirmedAsync                 | Validate 'Check email confirmed' in IdentityService.IsEmailConfirmedAsync, covering success/failure flow, response contract, and logging behavior.                                | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 24  | Check user active                   | IdentityService                 | IsUserActiveAsync                     | F024                    | IsUserActiveAsync                     | Validate 'Check user active' in IdentityService.IsUserActiveAsync, covering success/failure flow, response contract, and logging behavior.                                        | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 25  | Generate email confirmation token   | IdentityService                 | GenerateEmailConfirmationTokenAsync   | F025                    | GenerateEmailConfirmationTokenAsync   | Validate 'Generate email confirmation token' in IdentityService.GenerateEmailConfirmationTokenAsync, covering success/failure flow, response contract, and logging behavior.      | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 26  | Generate password reset token       | IdentityService                 | GeneratePasswordResetTokenAsync       | F026                    | GeneratePasswordResetTokenAsync       | Validate 'Generate password reset token' in IdentityService.GeneratePasswordResetTokenAsync, covering success/failure flow, response contract, and logging behavior.              | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 27  | Get user roles                      | IdentityService                 | GetUserRolesAsync                     | F027                    | GetUserRolesAsync                     | Validate 'Get user roles' in IdentityService.GetUserRolesAsync, covering success/failure flow, response contract, and logging behavior.                                           | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 28  | Check user in role                  | IdentityService                 | IsInRoleAsync                         | F028                    | IsInRoleAsync                         | Validate 'Check user in role' in IdentityService.IsInRoleAsync, covering success/failure flow, response contract, and logging behavior.                                           | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 29  | Get user ids by role and org        | IdentityService                 | GetUserIdsByRoleAndOrganizationAsync  | F029                    | GetUserIdsByRoleAndOrganizationAsync  | Validate 'Get user ids by role and org' in IdentityService.GetUserIdsByRoleAndOrganizationAsync, covering success/failure flow, response contract, and logging behavior.          | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 30  | Update last login async             | IdentityService                 | UpdateLastLoginAsync                  | F030                    | UpdateLastLoginAsync                  | Validate 'Update last login async' in IdentityService.UpdateLastLoginAsync, covering success/failure flow, response contract, and logging behavior.                               | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 31  | Check 2FA enabled                   | IdentityService                 | IsTwoFactorEnabledAsync               | F031                    | IsTwoFactorEnabledAsync               | Validate 'Check 2FA enabled' in IdentityService.IsTwoFactorEnabledAsync, covering success/failure flow, response contract, and logging behavior.                                  | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 32  | Get authenticator key               | IdentityService                 | GetAuthenticatorKeyAsync              | F032                    | GetAuthenticatorKeyAsync              | Validate 'Get authenticator key' in IdentityService.GetAuthenticatorKeyAsync, covering success/failure flow, response contract, and logging behavior.                             | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 33  | Get or create authenticator key     | IdentityService                 | GetOrCreateAuthenticatorKeyAsync      | F033                    | GetOrCreateAuthenticatorKeyAsync      | Validate 'Get or create authenticator key' in IdentityService.GetOrCreateAuthenticatorKeyAsync, covering success/failure flow, response contract, and logging behavior.           | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 34  | Verify 2FA code                     | IdentityService                 | VerifyTwoFactorCodeAsync              | F034                    | VerifyTwoFactorCodeAsync              | Validate 'Verify 2FA code' in IdentityService.VerifyTwoFactorCodeAsync, covering success/failure flow, response contract, and logging behavior.                                   | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 35  | Generate recovery codes             | IdentityService                 | GenerateNewRecoveryCodesAsync         | F035                    | GenerateNewRecoveryCodesAsync         | Validate 'Generate recovery codes' in IdentityService.GenerateNewRecoveryCodesAsync, covering success/failure flow, response contract, and logging behavior.                      | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 36  | Get recovery code count             | IdentityService                 | GetRecoveryCodesCountAsync            | F036                    | GetRecoveryCodesCountAsync            | Validate 'Get recovery code count' in IdentityService.GetRecoveryCodesCountAsync, covering success/failure flow, response contract, and logging behavior.                         | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 37  | Generate authenticator URI          | IdentityService                 | GenerateAuthenticatorUri              | F037                    | GenerateAuthenticatorUri              | Validate 'Generate authenticator URI' in IdentityService.GenerateAuthenticatorUri, covering success/failure flow, response contract, and logging behavior.                        | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 38  | Format authenticator key            | IdentityService                 | FormatAuthenticatorKey                | F038                    | FormatAuthenticatorKey                | Validate 'Format authenticator key' in IdentityService.FormatAuthenticatorKey, covering success/failure flow, response contract, and logging behavior.                            | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 39  | Get user metrics                    | IdentityService                 | GetUserMetricsAsync                   | F039                    | GetUserMetricsAsync                   | Validate 'Get user metrics' in IdentityService.GetUserMetricsAsync, covering success/failure flow, response contract, and logging behavior.                                       | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 40  | Get users in role count             | IdentityService                 | GetUsersInRoleCountAsync              | F040                    | GetUsersInRoleCountAsync              | Validate 'Get users in role count' in IdentityService.GetUsersInRoleCountAsync, covering success/failure flow, response contract, and logging behavior.                           | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 41  | Get pending approvals count         | IdentityService                 | GetPendingApprovalsCountAsync         | F041                    | GetPendingApprovalsCountAsync         | Validate 'Get pending approvals count' in IdentityService.GetPendingApprovalsCountAsync, covering success/failure flow, response contract, and logging behavior.                  | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 42  | Get user details                    | IdentityService                 | GetUserDetailsAsync                   | F042                    | GetUserDetailsAsync                   | Validate 'Get user details' in IdentityService.GetUserDetailsAsync, covering success/failure flow, response contract, and logging behavior.                                       | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 46  | Create refresh token record         | RefreshTokenService             | CreateRefreshTokenAsync               | F046                    | CreateRefreshTokenAsync               | Validate 'Create refresh token record' in RefreshTokenService.CreateRefreshTokenAsync, covering success/failure flow, response contract, and logging behavior.                    | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready.               |
| 47  | Get refresh token by hash           | RefreshTokenService             | GetByTokenHashAsync                   | F047                    | GetByTokenHashAsync                   | Validate 'Get refresh token by hash' in RefreshTokenService.GetByTokenHashAsync, covering success/failure flow, response contract, and logging behavior.                          | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready.               |
| 48  | Rotate refresh token                | RefreshTokenService             | RotateRefreshTokenAsync               | F048                    | RotateRefreshTokenAsync               | Validate 'Rotate refresh token' in RefreshTokenService.RotateRefreshTokenAsync, covering success/failure flow, response contract, and logging behavior.                           | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready.               |
| 49  | Revoke token by hash                | RefreshTokenService             | RevokeTokenAsync                      | F049                    | RevokeTokenAsync                      | Validate 'Revoke token by hash' in RefreshTokenService.RevokeTokenAsync, covering success/failure flow, response contract, and logging behavior.                                  | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready.               |
| 50  | Revoke all user tokens              | RefreshTokenService             | RevokeAllUserTokensAsync              | F050                    | RevokeAllUserTokensAsync              | Validate 'Revoke all user tokens' in RefreshTokenService.RevokeAllUserTokensAsync, covering success/failure flow, response contract, and logging behavior.                        | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready.               |
| 51  | Revoke token family                 | RefreshTokenService             | RevokeTokenFamilyAsync                | F051                    | RevokeTokenFamilyAsync                | Validate 'Revoke token family' in RefreshTokenService.RevokeTokenFamilyAsync, covering success/failure flow, response contract, and logging behavior.                             | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready.               |
| 52  | Cleanup expired tokens              | RefreshTokenService             | CleanupExpiredTokensAsync             | F052                    | CleanupExpiredTokensAsync             | Validate 'Cleanup expired tokens' in RefreshTokenService.CleanupExpiredTokensAsync, covering success/failure flow, response contract, and logging behavior.                       | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready.               |
| 53  | Generate access token               | TokenService                    | GenerateAccessTokenAsync              | F053                    | GenerateAccessTokenAsync              | Validate 'Generate access token' in TokenService.GenerateAccessTokenAsync, covering success/failure flow, response contract, and logging behavior.                                | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths.                   |
| 54  | Generate refresh token string       | TokenService                    | GenerateRefreshToken                  | F054                    | GenerateRefreshToken                  | Validate 'Generate refresh token string' in TokenService.GenerateRefreshToken, covering success/failure flow, response contract, and logging behavior.                            | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths.                   |
| 55  | Validate token                      | TokenService                    | ValidateToken                         | F055                    | ValidateToken                         | Validate 'Validate token' in TokenService.ValidateToken, covering success/failure flow, response contract, and logging behavior.                                                  | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths.                   |
| 56  | Get user id from token              | TokenService                    | GetUserIdFromToken                    | F056                    | GetUserIdFromToken                    | Validate 'Get user id from token' in TokenService.GetUserIdFromToken, covering success/failure flow, response contract, and logging behavior.                                     | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths.                   |
| 57  | Get jti from token                  | TokenService                    | GetJtiFromToken                       | F057                    | GetJtiFromToken                       | Validate 'Get jti from token' in TokenService.GetJtiFromToken, covering success/failure flow, response contract, and logging behavior.                                            | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths.                   |
| 58  | Hash token                          | TokenService                    | HashToken                             | F058                    | HashToken                             | Validate 'Hash token' in TokenService.HashToken, covering success/failure flow, response contract, and logging behavior.                                                          | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths.                   |
| 59  | Get ophthalmologists query          | AdminQueryService               | GetOphthalmologistsAsync              | F059                    | GetOphthalmologistsAsync              | Validate 'Get ophthalmologists query' in AdminQueryService.GetOphthalmologistsAsync, covering success/failure flow, response contract, and logging behavior.                      | ApplicationDbContext is seeded; filter, sort, and paging inputs are valid.                                                         |
| 60  | Get patients query                  | AdminQueryService               | GetPatientsAsync                      | F060                    | GetPatientsAsync                      | Validate 'Get patients query' in AdminQueryService.GetPatientsAsync, covering success/failure flow, response contract, and logging behavior.                                      | ApplicationDbContext is seeded; filter, sort, and paging inputs are valid.                                                         |
| 61  | Get audit logs query                | AdminQueryService               | GetAuditLogsAsync                     | F061                    | GetAuditLogsAsync                     | Validate 'Get audit logs query' in AdminQueryService.GetAuditLogsAsync, covering success/failure flow, response contract, and logging behavior.                                   | ApplicationDbContext is seeded; filter, sort, and paging inputs are valid.                                                         |
| 62  | Get AI quota                        | AiQuotaService                  | GetQuotaAsync                         | F062                    | GetQuotaAsync                         | Validate 'Get AI quota' in AiQuotaService.GetQuotaAsync, covering success/failure flow, response contract, and logging behavior.                                                  | ApplicationDbContext and system-setting dependency are seeded with quota data; user context is valid.                              |
| 63  | Check AI quota available            | AiQuotaService                  | HasAvailableQuotaAsync                | F063                    | HasAvailableQuotaAsync                | Validate 'Check AI quota available' in AiQuotaService.HasAvailableQuotaAsync, covering success/failure flow, response contract, and logging behavior.                             | ApplicationDbContext and system-setting dependency are seeded with quota data; user context is valid.                              |
| 64  | Deduct AI quota                     | AiQuotaService                  | DeductQuotaAsync                      | F064                    | DeductQuotaAsync                      | Validate 'Deduct AI quota' in AiQuotaService.DeductQuotaAsync, covering success/failure flow, response contract, and logging behavior.                                            | ApplicationDbContext and system-setting dependency are seeded with quota data; user context is valid.                              |
| 65  | Add purchased AI quota              | AiQuotaService                  | AddPurchasedQuotaAsync                | F065                    | AddPurchasedQuotaAsync                | Validate 'Add purchased AI quota' in AiQuotaService.AddPurchasedQuotaAsync, covering success/failure flow, response contract, and logging behavior.                               | ApplicationDbContext and system-setting dependency are seeded with quota data; user context is valid.                              |
| 66  | Get BetterStack embed URL           | BetterStackHeartbeatService     | GetEmbedUrl                           | F066                    | GetEmbedUrl                           | Validate 'Get BetterStack embed URL' in BetterStackHeartbeatService.GetEmbedUrl, covering success/failure flow, response contract, and logging behavior.                          | Heartbeat/monitor configuration is valid; client and logger are initialized.                                                       |
| 67  | Get BetterStack monitor descriptors | BetterStackHeartbeatService     | GetMonitorDescriptors                 | F067                    | GetMonitorDescriptors                 | Validate 'Get BetterStack monitor descriptors' in BetterStackHeartbeatService.GetMonitorDescriptors, covering success/failure flow, response contract, and logging behavior.      | Heartbeat/monitor configuration is valid; client and logger are initialized.                                                       |
| 68  | Notify monitor started              | BetterStackHeartbeatService     | NotifyStartedAsync                    | F068                    | NotifyStartedAsync                    | Validate 'Notify monitor started' in BetterStackHeartbeatService.NotifyStartedAsync, covering success/failure flow, response contract, and logging behavior.                      | Heartbeat/monitor configuration is valid; client and logger are initialized.                                                       |
| 69  | Notify monitor succeeded            | BetterStackHeartbeatService     | NotifySucceededAsync                  | F069                    | NotifySucceededAsync                  | Validate 'Notify monitor succeeded' in BetterStackHeartbeatService.NotifySucceededAsync, covering success/failure flow, response contract, and logging behavior.                  | Heartbeat/monitor configuration is valid; client and logger are initialized.                                                       |
| 70  | Notify monitor failed               | BetterStackHeartbeatService     | NotifyFailedAsync                     | F070                    | NotifyFailedAsync                     | Validate 'Notify monitor failed' in BetterStackHeartbeatService.NotifyFailedAsync, covering success/failure flow, response contract, and logging behavior.                        | Heartbeat/monitor configuration is valid; client and logger are initialized.                                                       |
| 72  | Get system admin metrics            | DashboardMetricsService         | GetSystemAdminMetricsAsync            | F072                    | GetSystemAdminMetricsAsync            | Validate 'Get system admin metrics' in DashboardMetricsService.GetSystemAdminMetricsAsync, covering success/failure flow, response contract, and logging behavior.                | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid.                                          |
| 73  | Get recent screenings               | DashboardMetricsService         | GetRecentScreeningsAsync              | F073                    | GetRecentScreeningsAsync              | Validate 'Get recent screenings' in DashboardMetricsService.GetRecentScreeningsAsync, covering success/failure flow, response contract, and logging behavior.                     | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid.                                          |
| 74  | Get screening volume trends         | DashboardMetricsService         | GetScreeningVolumeTrendsAsync         | F074                    | GetScreeningVolumeTrendsAsync         | Validate 'Get screening volume trends' in DashboardMetricsService.GetScreeningVolumeTrendsAsync, covering success/failure flow, response contract, and logging behavior.          | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid.                                          |
| 75  | Get population risk analysis        | DashboardMetricsService         | GetPopulationRiskAnalysisAsync        | F075                    | GetPopulationRiskAnalysisAsync        | Validate 'Get population risk analysis' in DashboardMetricsService.GetPopulationRiskAnalysisAsync, covering success/failure flow, response contract, and logging behavior.        | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid.                                          |
| 76  | Get system health                   | DashboardMetricsService         | GetSystemHealthAsync                  | F076                    | GetSystemHealthAsync                  | Validate 'Get system health' in DashboardMetricsService.GetSystemHealthAsync, covering success/failure flow, response contract, and logging behavior.                             | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid.                                          |
| 77  | Get ophthalmologist metrics         | DashboardMetricsService         | GetOphthalmologistMetricsAsync        | F077                    | GetOphthalmologistMetricsAsync        | Validate 'Get ophthalmologist metrics' in DashboardMetricsService.GetOphthalmologistMetricsAsync, covering success/failure flow, response contract, and logging behavior.         | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid.                                          |
| 78  | Get organisation metrics            | DashboardMetricsService         | GetOrganisationMetricsAsync           | F078                    | GetOrganisationMetricsAsync           | Validate 'Get organisation metrics' in DashboardMetricsService.GetOrganisationMetricsAsync, covering success/failure flow, response contract, and logging behavior.               | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid.                                          |
| 79  | Get patient metrics                 | DashboardMetricsService         | GetPatientMetricsAsync                | F079                    | GetPatientMetricsAsync                | Validate 'Get patient metrics' in DashboardMetricsService.GetPatientMetricsAsync, covering success/failure flow, response contract, and logging behavior.                         | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid.                                          |
| 80  | Get local now                       | DateTimeService                 | Now (property)                        | F080                    | Now (property)                        | Validate 'Get local now' in DateTimeService.Now (property), covering success/failure flow, response contract, and logging behavior.                                               | Service is instantiated directly; no external dependency is required.                                                              |
| 81  | Get UTC now                         | DateTimeService                 | UtcNow (property)                     | F081                    | UtcNow (property)                     | Validate 'Get UTC now' in DateTimeService.UtcNow (property), covering success/failure flow, response contract, and logging behavior.                                              | Service is instantiated directly; no external dependency is required.                                                              |
| 82  | Send email confirmation             | EmailService                    | SendEmailConfirmationAsync            | F082                    | SendEmailConfirmationAsync            | Validate 'Send email confirmation' in EmailService.SendEmailConfirmationAsync, covering success/failure flow, response contract, and logging behavior.                            | SMTP settings are valid; recipient and template/content inputs are valid; transport can be mocked.                                 |
| 83  | Send password reset email           | EmailService                    | SendPasswordResetAsync                | F083                    | SendPasswordResetAsync                | Validate 'Send password reset email' in EmailService.SendPasswordResetAsync, covering success/failure flow, response contract, and logging behavior.                              | SMTP settings are valid; recipient and template/content inputs are valid; transport can be mocked.                                 |
| 84  | Send welcome email                  | EmailService                    | SendWelcomeEmailAsync                 | F084                    | SendWelcomeEmailAsync                 | Validate 'Send welcome email' in EmailService.SendWelcomeEmailAsync, covering success/failure flow, response contract, and logging behavior.                                      | SMTP settings are valid; recipient and template/content inputs are valid; transport can be mocked.                                 |
| 85  | Send generic email                  | EmailService                    | SendAsync                             | F085                    | SendAsync                             | Validate 'Send generic email' in EmailService.SendAsync, covering success/failure flow, response contract, and logging behavior.                                                  | SMTP settings are valid; recipient and template/content inputs are valid; transport can be mocked.                                 |
| 86  | Create Google Meet meeting          | GoogleMeetService               | CreateMeetingAsync                    | F086                    | CreateMeetingAsync                    | Validate 'Create Google Meet meeting' in GoogleMeetService.CreateMeetingAsync, covering success/failure flow, response contract, and logging behavior.                            | Google credentials and service configuration are valid; meeting request inputs are valid.                                          |
| 87  | Delete Google Meet meeting          | GoogleMeetService               | DeleteMeetingAsync                    | F087                    | DeleteMeetingAsync                    | Validate 'Delete Google Meet meeting' in GoogleMeetService.DeleteMeetingAsync, covering success/failure flow, response contract, and logging behavior.                            | Google credentials and service configuration are valid; meeting request inputs are valid.                                          |
| 88  | Dispose Google Meet service         | GoogleMeetService               | Dispose                               | F088                    | Dispose                               | Validate 'Dispose Google Meet service' in GoogleMeetService.Dispose, covering success/failure flow, response contract, and logging behavior.                                      | Google credentials and service configuration are valid; meeting request inputs are valid.                                          |
| 89  | Send typed notification             | NotificationService             | SendAsync (typed)                     | F089                    | SendAsync (typed)                     | Validate 'Send typed notification' in NotificationService.SendAsync (typed), covering success/failure flow, response contract, and logging behavior.                              | Notification repository, UnitOfWork, and hub service are mocked; user/message/type payload is prepared.                            |
| 90  | Send legacy notification            | NotificationService             | SendAsync (legacy)                    | F090                    | SendAsync (legacy)                    | Validate 'Send legacy notification' in NotificationService.SendAsync (legacy), covering success/failure flow, response contract, and logging behavior.                            | Notification repository, UnitOfWork, and hub service are mocked; user/message/type payload is prepared.                            |
| 91  | Submit onboarding request           | OrganisationOnboardingService   | SubmitRequestAsync                    | F091                    | SubmitRequestAsync                    | Validate 'Submit onboarding request' in OrganisationOnboardingService.SubmitRequestAsync, covering success/failure flow, response contract, and logging behavior.                 | Onboarding/organization repositories, UserManager, EmailService, and UnitOfWork are mocked or seeded; request is valid.            |
| 92  | Get onboarding requests             | OrganisationOnboardingService   | GetRequestsAsync                      | F092                    | GetRequestsAsync                      | Validate 'Get onboarding requests' in OrganisationOnboardingService.GetRequestsAsync, covering success/failure flow, response contract, and logging behavior.                     | Onboarding/organization repositories, UserManager, EmailService, and UnitOfWork are mocked or seeded; request is valid.            |
| 93  | Approve onboarding request          | OrganisationOnboardingService   | ApproveRequestAsync                   | F093                    | ApproveRequestAsync                   | Validate 'Approve onboarding request' in OrganisationOnboardingService.ApproveRequestAsync, covering success/failure flow, response contract, and logging behavior.               | Onboarding/organization repositories, UserManager, EmailService, and UnitOfWork are mocked or seeded; request is valid.            |
| 94  | Generate roadmap from diagnosis     | PatientRoadmapGenerationService | GenerateFromDiagnosisAsync            | F094                    | GenerateFromDiagnosisAsync            | Validate 'Generate roadmap from diagnosis' in PatientRoadmapGenerationService.GenerateFromDiagnosisAsync, covering success/failure flow, response contract, and logging behavior. | Diagnosis input and roadmap rules/mapping are prepared.                                                                            |
| 95  | Create PayOS payment link           | PayOSService                    | CreatePaymentLinkAsync                | F095                    | CreatePaymentLinkAsync                | Validate 'Create PayOS payment link' in PayOSService.CreatePaymentLinkAsync, covering success/failure flow, response contract, and logging behavior.                              | PayOS settings are valid (clientId, apiKey, checksumKey); payment payload inputs are valid.                                        |
| 96  | Get PayOS payment status            | PayOSService                    | GetPaymentStatusAsync                 | F096                    | GetPaymentStatusAsync                 | Validate 'Get PayOS payment status' in PayOSService.GetPaymentStatusAsync, covering success/failure flow, response contract, and logging behavior.                                | PayOS settings are valid (clientId, apiKey, checksumKey); payment payload inputs are valid.                                        |
| 97  | Verify PayOS webhook signature      | PayOSService                    | VerifyWebhookSignatureAsync           | F097                    | VerifyWebhookSignatureAsync           | Validate 'Verify PayOS webhook signature' in PayOSService.VerifyWebhookSignatureAsync, covering success/failure flow, response contract, and logging behavior.                    | PayOS settings are valid (clientId, apiKey, checksumKey); payment payload inputs are valid.                                        |
| 98  | Cancel PayOS payment                | PayOSService                    | CancelPaymentAsync                    | F098                    | CancelPaymentAsync                    | Validate 'Cancel PayOS payment' in PayOSService.CancelPaymentAsync, covering success/failure flow, response contract, and logging behavior.                                       | PayOS settings are valid (clientId, apiKey, checksumKey); payment payload inputs are valid.                                        |
| 100 | Save file to Supabase               | SupabaseStorageService          | SaveFileAsync                         | F100                    | SaveFileAsync                         | Validate 'Save file to Supabase' in SupabaseStorageService.SaveFileAsync, covering success/failure flow, response contract, and logging behavior.                                 | Supabase storage settings are valid; stream/path inputs are valid; network calls can be mocked.                                    |
| 101 | Delete file from Supabase           | SupabaseStorageService          | DeleteFile                            | F101                    | DeleteFile                            | Validate 'Delete file from Supabase' in SupabaseStorageService.DeleteFile, covering success/failure flow, response contract, and logging behavior.                                | Supabase storage settings are valid; stream/path inputs are valid; network calls can be mocked.                                    |
| 102 | Check Supabase file exists          | SupabaseStorageService          | FileExists                            | F102                    | FileExists                            | Validate 'Check Supabase file exists' in SupabaseStorageService.FileExists, covering success/failure flow, response contract, and logging behavior.                               | Supabase storage settings are valid; stream/path inputs are valid; network calls can be mocked.                                    |
| 103 | Get setting by key                  | SystemSettingService            | GetSettingAsync                       | F103                    | GetSettingAsync                       | Validate 'Get setting by key' in SystemSettingService.GetSettingAsync, covering success/failure flow, response contract, and logging behavior.                                    | System-setting repository and UnitOfWork are seeded with valid key/value data.                                                     |
| 104 | Get all settings                    | SystemSettingService            | GetAllSettingsAsync                   | F104                    | GetAllSettingsAsync                   | Validate 'Get all settings' in SystemSettingService.GetAllSettingsAsync, covering success/failure flow, response contract, and logging behavior.                                  | System-setting repository and UnitOfWork are seeded with valid key/value data.                                                     |
| 105 | Update settings                     | SystemSettingService            | UpdateSettingsAsync                   | F105                    | UpdateSettingsAsync                   | Validate 'Update settings' in SystemSettingService.UpdateSettingsAsync, covering success/failure flow, response contract, and logging behavior.                                   | System-setting repository and UnitOfWork are seeded with valid key/value data.                                                     |

## Tong hop nhanh

- Tong function checklist (Infrastructure scope): 105
- So file test theo function: 105
- Tong test case hien tai: 923

## Ghi chu map de viet lai test theo cum

- Identity/Auth cluster: ApplicationUser, AuthService, IdentityService, RefreshToken, RefreshTokenService, TokenService.
- Core infrastructure services: AdminQueryService, AiQuotaService, DashboardMetricsService, EmailService, NotificationService, SystemSettingService.
- Integrations: PayOSService, SupabaseStorageService, GoogleMeetService.
- Background jobs/workers: (loai khoi scope services only).

## Chi tiet test case theo function (thuc te - tap trung service/function)

Ghi chu:
- Moi function duoi day co nhieu case theo luong thanh cong + luong loi nghiep vu + luong exception.
- Dinh dang: `UTCIDxx - Condition -> Expected`.
- Co the tach moi function thanh 1 sheet rieng giong mau `LoginAsync`.

### F005 - AuthService.RegisterPatientAsync
- UTCID01 - Email da ton tai -> Failure "A user with this email already exists".
- UTCID02 - Tao user that bai (Identity errors) -> Rollback transaction + tra danh sach loi.
- UTCID03 - Tao user thanh cong + add role + tao Patient profile -> Commit + Success response co UserId/Email.
- UTCID04 - Gui email confirm bi loi -> Van Success, ghi warning log.
- UTCID05 - Exception khi dang transaction -> Rollback + Failure "An error occurred during registration".

### F006 - AuthService.RegisterOphthalmologistAsync
- UTCID01 - Khong co credential -> Failure "At least one credential is required".
- UTCID02 - Thieu Degree -> Failure "At least one degree is required".
- UTCID03 - Thieu License -> Failure "At least one license/certificate is required".
- UTCID04 - Degree co ExpiryDate -> Failure rule degree.
- UTCID05 - License khong co ExpiryDate hoac Expiry <= Issued -> Failure rule license.
- UTCID06 - Upload file + tao ho so + commit thanh cong -> Success response.
- UTCID07 - Exception sau khi upload file nhung truoc commit -> Rollback + goi DeleteFile de cleanup.
- UTCID08 - Gui mail admin/confirm bi loi -> Van Success, co warning log.

### F007 - AuthService.RegisterOrganisationAsync
- UTCID01 - SubmitRequestAsync tra Success -> tra Success dung payload.
- UTCID02 - SubmitRequestAsync tra Conflict -> service tra Conflict tuong ung.
- UTCID03 - SubmitRequestAsync throw exception -> bubble/Failure theo handling cua caller.

### F008 - AuthService.GoogleLoginAsync
- UTCID01 - Google token invalid (InvalidJwtException) -> Unauthorized "Invalid Google token".
- UTCID02 - Payload khong co email -> Failure.
- UTCID03 - User ton tai nhung IsDeleted -> Unauthorized.
- UTCID04 - User ton tai nhung IsActive=false -> Unauthorized.
- UTCID05 - User ton tai, chua confirm email -> auto confirm + update user.
- UTCID06 - User ton tai, bat 2FA -> Success(TwoFactorRequired).
- UTCID07 - User moi, create + role Patient + profile -> Commit + login Success.
- UTCID08 - Loi tao user/DB -> Rollback + Failure "An error occurred during Google login".

### F009 - AuthService.LoginAsync
- UTCID01 - Email khong ton tai -> Unauthorized "Invalid email or password".
- UTCID02 - User bi soft delete -> Unauthorized "Invalid email or password".
- UTCID03 - User inactive -> Unauthorized deactivated message.
- UTCID04 - User bi lockout -> Unauthorized lockout message.
- UTCID05 - CheckPasswordSignInAsync tra IsLockedOut -> Unauthorized lockout message.
- UTCID06 - IsNotAllowed (email chua confirm) -> Unauthorized.
- UTCID07 - Sai password -> Unauthorized invalid credentials.
- UTCID08 - Role Ophthalmologist va VerificationStatus=Rejected -> Unauthorized bi tu choi xac minh.
- UTCID09 - RequiresTwoFactor=true -> Success(TwoFactorRequired).
- UTCID10 - 2FA enabled tai user manager -> Success(TwoFactorRequired).
- UTCID11 - Dang nhap thanh cong -> Success(AuthResponse day du access/refresh token).
- UTCID12 - Exception bat ky -> Failure "An error occurred during login".

### F010 - AuthService.VerifyTwoFactorLoginAsync
- UTCID01 - User khong ton tai/da xoa/inactive -> Unauthorized.
- UTCID02 - User chua bat 2FA -> Failure.
- UTCID03 - Verify code thuong sai -> Unauthorized.
- UTCID04 - Verify recovery code sai -> Unauthorized.
- UTCID05 - Verify recovery code dung -> Success va co warning log recovery code.
- UTCID06 - Verify authenticator code dung -> Success AuthResponse.
- UTCID07 - Exception -> Failure "An error occurred during verification".

### F011 - AuthService.RefreshTokenAsync
- UTCID01 - Access token khong parse duoc userId/jti -> Unauthorized invalid access token.
- UTCID02 - Refresh token hash khong tim thay -> Unauthorized invalid refresh token.
- UTCID03 - Refresh token khong active -> revoke token family + Unauthorized.
- UTCID04 - JwtId/UserId mismatch -> Unauthorized token mismatch.
- UTCID05 - User khong ton tai hoac inactive/deleted -> Unauthorized.
- UTCID06 - Refresh thanh cong -> rotate token + tra access/refresh moi.
- UTCID07 - Exception -> Failure "An error occurred while refreshing token".

### F012 - AuthService.LogoutAsync
- UTCID01 - Refresh token tim thay -> RevokeTokenAsync duoc goi + Success.
- UTCID02 - Refresh token khong tim thay -> van Success (idempotent logout).
- UTCID03 - Exception -> Failure "An error occurred during logout".

### F013 - AuthService.LogoutAllAsync
- UTCID01 - RevokeAllUserTokensAsync thanh cong -> Success.
- UTCID02 - RevokeAllUserTokensAsync throw -> Failure "An error occurred during logout".

### F014 - AuthService.ConfirmEmailAsync
- UTCID01 - userId khong parse duoc Guid -> Failure "Invalid user ID".
- UTCID02 - user khong ton tai -> NotFound.
- UTCID03 - ConfirmEmailAsync tra fail errors -> Failure errors.
- UTCID04 - User role Ophthalmologist -> gui notification den tat ca admin.
- UTCID05 - Confirm thanh cong -> Success.
- UTCID06 - Exception -> Failure.

### F015 - AuthService.ForgotPasswordAsync
- UTCID01 - Email ton tai -> tao reset token + gui mail reset + Success.
- UTCID02 - Email khong ton tai -> khong gui mail, van Success (anti-enumeration).
- UTCID03 - Exception khi generate/send -> Failure.

### F016 - AuthService.ResetPasswordAsync
- UTCID01 - request.UserId khong hop le -> Failure "Invalid user ID".
- UTCID02 - User khong ton tai -> NotFound.
- UTCID03 - Identity reset tra fail -> Failure errors.
- UTCID04 - Reset thanh cong -> revoke all refresh token + Success.
- UTCID05 - Exception -> Failure.

### F017 - AuthService.GetCurrentUserAsync
- UTCID01 - User khong ton tai -> Unauthorized.
- UTCID02 - User Patient -> map RoleId tu Patient profile.
- UTCID03 - User Ophthalmologist -> map RoleId + IsVerified + VerificationStatus + ContractStatus.
- UTCID04 - Lay thong tin thanh cong -> Success(UserInfoResponse day du).
- UTCID05 - Exception -> Failure.

### F018 - AuthService.ResendConfirmationAsync
- UTCID01 - User ton tai va chua confirm -> tao token + gui mail.
- UTCID02 - User da confirm -> bo qua gui mail, van Success.
- UTCID03 - User khong ton tai -> van Success.
- UTCID04 - Exception -> Failure.

### F019-F042 - IdentityService (theo nhom)
- F019 CheckPasswordAsync: user null -> false; password sai -> false; dung -> true.
- F020 GetUserByEmailAsync: tim thay user khong deleted -> dto; user deleted/khong co -> null.
- F021 GetUserByIdAsync: tim thay -> dto; deleted/khong co -> null.
- F022 IsPhoneNumberInUseByOrganizationAsync: phone rong -> false; format khac nhau cung so -> true; khac org -> false.
- F023 IsEmailConfirmedAsync: user null -> false; confirmed false/true -> dung theo state.
- F024 IsUserActiveAsync: null -> false; IsActive true + !IsDeleted -> true.
- F025 GenerateEmailConfirmationTokenAsync: user null -> throw InvalidOperationException; user hop le -> token non-empty.
- F026 GeneratePasswordResetTokenAsync: user null -> throw; user hop le -> token non-empty.
- F027 GetUserRolesAsync: user null -> empty list; user co role -> dung list.
- F028 IsInRoleAsync: user null -> false; co role -> true; khong co role -> false.
- F029 GetUserIdsByRoleAndOrganizationAsync: loc dung theo role + org + active + !deleted.
- F030 UpdateLastLoginAsync: user null -> no-op; user ton tai -> LastLoginAt thay doi + UpdateAsync duoc goi.
- F031 IsTwoFactorEnabledAsync: user null -> false; enabled true/false theo user manager.
- F032 GetAuthenticatorKeyAsync: user null -> null; user ton tai -> key.
- F033 GetOrCreateAuthenticatorKeyAsync: user null -> throw; reset key + lay key thanh cong.
- F034 VerifyTwoFactorCodeAsync: user null -> false; code sai/dung -> false/true.
- F035 GenerateNewRecoveryCodesAsync: user null -> throw; user hop le -> mang recovery codes dung so luong.
- F036 GetRecoveryCodesCountAsync: user null -> 0; user ton tai -> count dung.
- F037 GenerateAuthenticatorUri: output dung format `otpauth://totp/...`.
- F038 FormatAuthenticatorKey: key dai -> tach moi 4 ky tu, uppercase.
- F039 GetUserMetricsAsync: tinh total/change/pending approvals dung theo seed data.
- F040 GetUsersInRoleCountAsync: activeOnly=true chi dem active+not deleted; false dem tat ca not deleted.
- F041 GetPendingApprovalsCountAsync: dem dung users `!EmailConfirmed || !IsActive`.
- F042 GetUserDetailsAsync: user null/deleted -> null; user hop le -> dto day du fields profile.

### F046-F052 - RefreshTokenService
- F046 CreateRefreshTokenAsync: luu token vao DB + return Id.
- F047 GetByTokenHashAsync: hash khong ton tai -> null; ton tai -> map dto dung IsActive/Revoked/Used.
- F048 RotateRefreshTokenAsync: old token khong ton tai -> throw; ton tai -> tao token moi + mark old used.
- F049 RevokeTokenAsync: token ton tai -> revoked + save; khong ton tai -> no-op.
- F050 RevokeAllUserTokensAsync: revoke tat ca token chua revoke cua user.
- F051 RevokeTokenFamilyAsync: token null -> no-op; token ton tai -> goi revoke all user tokens.
- F052 CleanupExpiredTokensAsync: xoa token het han/da revoke qua cutoff va return so ban ghi da xoa.

### F053-F058 - TokenService
- F053 GenerateAccessTokenAsync: token co sub/uid/email/jti/role claims va expiresAt dung config.
- F054 GenerateRefreshToken: sinh chuoi random base64, do dai/entropy cao, 2 lan sinh khac nhau.
- F055 ValidateToken: token hop le -> ClaimsPrincipal; token sai signature/alg -> null.
- F056 GetUserIdFromToken: co claim sub/uid/nameidentifier hop le -> Guid; claim loi -> null.
- F057 GetJtiFromToken: token hop le co jti -> lay duoc; token invalid -> null.
- F058 HashToken: cung input -> cung hash; khac input -> khac hash; output base64 khong rong.

### F059-F061 - AdminQueryService
- F059 GetOphthalmologistsAsync: loc searchTerm ILike; loc nhieu verificationStatus; paging + sort CreatedAt desc; map licenses/degrees dung.
- F060 GetPatientsAsync: loc search/status(active|pending|suspended); paging dung totalCount.
- F061 GetAuditLogsAsync: loc theo search/action/entity/user/date-range; paging + map UserName null-safe.

### F062-F065 - AiQuotaService
- F062 GetQuotaAsync: role Patient -> tinh free/purchased/remaining dung; role OrgAdmin/Ophthalmologist -> quota theo organisation; role la -> None.
- F063 HasAvailableQuotaAsync: remaining > 0 -> true; =0 -> false.
- F064 DeductQuotaAsync: patient/org ton tai -> consume quota + SaveChanges; khong ton tai -> throw InvalidOperationException.
- F065 AddPurchasedQuotaAsync: patient/org ton tai -> tang purchased + SaveChanges; khong ton tai -> throw.

### F066-F070 - BetterStackHeartbeatService
- F066 GetEmbedUrl: url rong/whitespace -> null; co gia tri -> tra url.
- F067 GetMonitorDescriptors: tra du so monitor enum, co key/name/category/configured dung.
- F068 NotifyStartedAsync: endpoint khong config -> skip khong throw; config day du -> POST start/ping.
- F069 NotifySucceededAsync: POST ping url; non-success status -> warning log.
- F070 NotifyFailedAsync: POST fail url (fallback ping); HTTP exception -> warning log khong throw.

- UTCID01 - Co patient/org UsedAiQuota>0 -> reset ve 0, notify started+succeeded.
- UTCID02 - Khong co du lieu can reset -> van completed, count=0.
- UTCID03 - Exception DB -> notify failed + rethrow.

### F072-F079 - DashboardMetricsService
- F072 GetSystemAdminMetricsAsync: tinh growth doctor/org/patient, revenue breakdown, pending actions, top performers dung theo seed.
- F073 GetRecentScreeningsAsync: map ScreeningCode, status Completed/Analyzing, risk level + isCritical, paging dung.
- F074 GetScreeningVolumeTrendsAsync: timeRange weekly va monthly deu tra data points + average dung.
- F075 GetPopulationRiskAnalysisAsync: bo RiskLevel.None, tinh percentage dung tong results.
- F076 GetSystemHealthAsync: tra fixed healthy payload voi 3 components.
- F077 GetOphthalmologistMetricsAsync: khong tim thay doctor -> dto rong; tim thay -> pending/urgent/completed/open slots dung.
- F078 GetOrganisationMetricsAsync: user khong co organisation -> dto rong; co organisation -> utilization + appointment breakdown + remaining quota.
- F079 GetPatientMetricsAsync: patient null -> dto rong; patient ton tai -> completed screenings/total reports/upcoming/quota dung.

### F080-F081 - DateTimeService
- F080 Now property: tra local time gan voi DateTime.Now.
- F081 UtcNow property: tra UTC time gan voi DateTime.UtcNow.

### F082-F085 - EmailService
- F082 SendEmailConfirmationAsync: tao dung subject/body template + goi SendAsync.
- F083 SendPasswordResetAsync: tao dung template reset + goi SendAsync.
- F084 SendWelcomeEmailAsync: tao dung template welcome + goi SendAsync.
- F085 SendAsync: validate tham so rong -> throw ArgumentException; gui SMTP thanh cong -> debug log; SMTP fail -> log error + rethrow.

### F086-F088 - GoogleMeetService
- F086 CreateMeetingAsync: tao event thanh cong co meet link ngay lan dau -> return MeetingInfo.
- F086-2 - Meet link chua co ngay -> retry exponential backoff, lay duoc link -> success.
- F086-3 - Retry het van khong co link -> cleanup orphan event + throw InvalidOperationException.
- F087 DeleteMeetingAsync: xoa thanh cong -> info log; event not found -> warning khong throw.
- F088 Dispose: dispose calendar service khong nem loi.

### F089-F090 - NotificationService
- F089 SendAsync(typed): payload object -> serialize camelCase; persist notification + broadcast dto + broadcast unread count.
- F089-2 - payload co consultationId/sessionId/... guid -> ExtractReferenceId map dung.
- F089-3 - hub/repository throw -> log error + rethrow.
- F090 SendAsync(legacy): goi overload typed voi title/type mac dinh.

### F091-F093 - OrganisationOnboardingService
- F091 SubmitRequestAsync: da co pending cung email -> Conflict; khong co -> add request + save + notify admin + Success.
- F092 GetRequestsAsync: tra danh sach sort CreatedAt desc + map dto dung status/org type.
- F093 ApproveRequestAsync: request null -> NotFound; request khong pending -> Failure; email da ton tai -> Conflict; create org admin + role + organisation + optional contract + approve + commit + send credential email -> Success; exception -> rollback + Failure.

### F094 - PatientRoadmapGenerationService.GenerateFromDiagnosisAsync
- UTCID01 - PatientId/ScreeningId/AiScreeningRawJson thieu -> Failure validation message.
- UTCID02 - ApiKey chua config -> Failure.
- UTCID03 - Google AI tra JSON hop le -> Success roadmap (risk normalized, list normalized).
- UTCID04 - AI tra JSON sai format nhieu lan -> Failure sau retries.
- UTCID05 - HTTP timeout/network error va het retry -> Failure "Unable to generate...".
- UTCID06 - follow_up.needed=true nhung timeframe rong -> invalid format, retry/failure.

### F095-F098 - PayOSService
- F095 CreatePaymentLinkAsync: dung default return/cancel url neu input rong; append orderCode; truncate description >25; response null checkoutUrl -> throw.
- F096 GetPaymentStatusAsync: paymentInfo null -> throw; co du lieu -> map status/amount/txnRef.
- F097 VerifyWebhookSignatureAsync: hien tai return true (happy path), exception branch return false.
- F098 CancelPaymentAsync: cancel tra object -> true; tra null -> false; exception -> false.

- UTCID01 - Slot Available + BookedCount=0 + qua gio -> chuyen Expired + UpdatedAt.
- UTCID02 - Slot da book hoac chua qua gio -> khong bi expire.
- UTCID03 - Thanh cong -> notify started+succeeded; exception -> notify failed + rethrow.

### F100-F102 - SupabaseStorageService
- F100 SaveFileAsync: filename duoc sanitize; subfolder ghep path dung; stream rong -> throw; upload thanh cong -> tra public url (co fallback absolute url).
- F101 DeleteFile: relativePath/url hop le -> Remove thanh cong tra true; parse path fail hoac exception -> false.
- F102 FileExists: HEAD success -> true; status fail/exception -> false; path rong -> false.

### F103-F105 - SystemSettingService
- F103 GetSettingAsync: key ton tai -> tra value; khong ton tai -> null.
- F104 GetAllSettingsAsync: tra dictionary day du key/value tu DB.
- F105 UpdateSettingsAsync: dictionary rong/null -> no-op; key ton tai -> UpdateValue; key moi -> add moi; save change + info log.

## Thong ke day du theo yeu cau (service -> function -> so test case)

- Tong scope da viet: services only.
- Tong test case de xuat: cap nhat theo services only (khong tinh entity/job).

| Function Code | Class/Service | Function | So test case |
|---|---|---|---|
| F005 | AuthService | RegisterPatientAsync | 10 |
| F006 | AuthService | RegisterOphthalmologistAsync | 60 |
| F007 | AuthService | RegisterOrganisationAsync | 20 |
| F008 | AuthService | GoogleLoginAsync | 10 |
| F009 | AuthService | LoginAsync | 10 |
| F010 | AuthService | VerifyTwoFactorLoginAsync | 10 |
| F011 | AuthService | RefreshTokenAsync | 10 |
| F012 | AuthService | LogoutAsync | 20 |
| F013 | AuthService | LogoutAllAsync | 20 |
| F014 | AuthService | ConfirmEmailAsync | 10 |
| F015 | AuthService | ForgotPasswordAsync | 20 |
| F016 | AuthService | ResetPasswordAsync | 10 |
| F017 | AuthService | GetCurrentUserAsync | 10 |
| F018 | AuthService | ResendConfirmationAsync | 20 |
| F019 | IdentityService | CheckPasswordAsync | 3 |
| F020 | IdentityService | GetUserByEmailAsync | 3 |
| F021 | IdentityService | GetUserByIdAsync | 3 |
| F022 | IdentityService | IsPhoneNumberInUseByOrganizationAsync | 4 |
| F023 | IdentityService | IsEmailConfirmedAsync | 3 |
| F024 | IdentityService | IsUserActiveAsync | 3 |
| F025 | IdentityService | GenerateEmailConfirmationTokenAsync | 2 |
| F026 | IdentityService | GeneratePasswordResetTokenAsync | 2 |
| F027 | IdentityService | GetUserRolesAsync | 3 |
| F028 | IdentityService | IsInRoleAsync | 3 |
| F029 | IdentityService | GetUserIdsByRoleAndOrganizationAsync | 3 |
| F030 | IdentityService | UpdateLastLoginAsync | 2 |
| F031 | IdentityService | IsTwoFactorEnabledAsync | 3 |
| F032 | IdentityService | GetAuthenticatorKeyAsync | 2 |
| F033 | IdentityService | GetOrCreateAuthenticatorKeyAsync | 2 |
| F034 | IdentityService | VerifyTwoFactorCodeAsync | 3 |
| F035 | IdentityService | GenerateNewRecoveryCodesAsync | 2 |
| F036 | IdentityService | GetRecoveryCodesCountAsync | 2 |
| F037 | IdentityService | GenerateAuthenticatorUri | 2 |
| F038 | IdentityService | FormatAuthenticatorKey | 2 |
| F039 | IdentityService | GetUserMetricsAsync | 3 |
| F040 | IdentityService | GetUsersInRoleCountAsync | 3 |
| F041 | IdentityService | GetPendingApprovalsCountAsync | 2 |
| F042 | IdentityService | GetUserDetailsAsync | 3 |
| F046 | RefreshTokenService | CreateRefreshTokenAsync | 2 |
| F047 | RefreshTokenService | GetByTokenHashAsync | 3 |
| F048 | RefreshTokenService | RotateRefreshTokenAsync | 3 |
| F049 | RefreshTokenService | RevokeTokenAsync | 2 |
| F050 | RefreshTokenService | RevokeAllUserTokensAsync | 2 |
| F051 | RefreshTokenService | RevokeTokenFamilyAsync | 2 |
| F052 | RefreshTokenService | CleanupExpiredTokensAsync | 3 |
| F053 | TokenService | GenerateAccessTokenAsync | 4 |
| F054 | TokenService | GenerateRefreshToken | 3 |
| F055 | TokenService | ValidateToken | 3 |
| F056 | TokenService | GetUserIdFromToken | 3 |
| F057 | TokenService | GetJtiFromToken | 2 |
| F058 | TokenService | HashToken | 3 |
| F059 | AdminQueryService | GetOphthalmologistsAsync | 5 |
| F060 | AdminQueryService | GetPatientsAsync | 4 |
| F061 | AdminQueryService | GetAuditLogsAsync | 5 |
| F062 | AiQuotaService | GetQuotaAsync | 5 |
| F063 | AiQuotaService | HasAvailableQuotaAsync | 2 |
| F064 | AiQuotaService | DeductQuotaAsync | 4 |
| F065 | AiQuotaService | AddPurchasedQuotaAsync | 4 |
| F066 | BetterStackHeartbeatService | GetEmbedUrl | 2 |
| F067 | BetterStackHeartbeatService | GetMonitorDescriptors | 2 |
| F068 | BetterStackHeartbeatService | NotifyStartedAsync | 3 |
| F069 | BetterStackHeartbeatService | NotifySucceededAsync | 2 |
| F070 | BetterStackHeartbeatService | NotifyFailedAsync | 2 |
| F072 | DashboardMetricsService | GetSystemAdminMetricsAsync | 6 |
| F073 | DashboardMetricsService | GetRecentScreeningsAsync | 4 |
| F074 | DashboardMetricsService | GetScreeningVolumeTrendsAsync | 3 |
| F075 | DashboardMetricsService | GetPopulationRiskAnalysisAsync | 3 |
| F076 | DashboardMetricsService | GetSystemHealthAsync | 2 |
| F077 | DashboardMetricsService | GetOphthalmologistMetricsAsync | 4 |
| F078 | DashboardMetricsService | GetOrganisationMetricsAsync | 4 |
| F079 | DashboardMetricsService | GetPatientMetricsAsync | 3 |
| F080 | DateTimeService | Now (property) | 2 |
| F081 | DateTimeService | UtcNow (property) | 2 |
| F082 | EmailService | SendEmailConfirmationAsync | 2 |
| F083 | EmailService | SendPasswordResetAsync | 2 |
| F084 | EmailService | SendWelcomeEmailAsync | 2 |
| F085 | EmailService | SendAsync | 4 |
| F086 | GoogleMeetService | CreateMeetingAsync | 5 |
| F087 | GoogleMeetService | DeleteMeetingAsync | 2 |
| F088 | GoogleMeetService | Dispose | 1 |
| F089 | NotificationService | SendAsync (typed) | 4 |
| F090 | NotificationService | SendAsync (legacy) | 1 |
| F091 | OrganisationOnboardingService | SubmitRequestAsync | 3 |
| F092 | OrganisationOnboardingService | GetRequestsAsync | 2 |
| F093 | OrganisationOnboardingService | ApproveRequestAsync | 6 |
| F094 | PatientRoadmapGenerationService | GenerateFromDiagnosisAsync | 6 |
| F095 | PayOSService | CreatePaymentLinkAsync | 5 |
| F096 | PayOSService | GetPaymentStatusAsync | 3 |
| F097 | PayOSService | VerifyWebhookSignatureAsync | 2 |
| F098 | PayOSService | CancelPaymentAsync | 3 |
| F100 | SupabaseStorageService | SaveFileAsync | 5 |
| F101 | SupabaseStorageService | DeleteFile | 3 |
| F102 | SupabaseStorageService | FileExists | 3 |
| F103 | SystemSettingService | GetSettingAsync | 2 |
| F104 | SystemSettingService | GetAllSettingsAsync | 2 |
| F105 | SystemSettingService | UpdateSettingsAsync | 4 |

## Tong ket theo Service

| Service/Class | So function | Tong test case |
|---|---:|---:|
| AuthService | 14 | 240 |
| IdentityService | 24 | 64 |
| RefreshTokenService | 7 | 17 |
| TokenService | 6 | 18 |
| AdminQueryService | 3 | 14 |
| AiQuotaService | 4 | 15 |
| BetterStackHeartbeatService | 5 | 11 |
| DashboardMetricsService | 8 | 29 |
| DateTimeService | 2 | 4 |
| EmailService | 4 | 10 |
| GoogleMeetService | 3 | 8 |
| NotificationService | 2 | 5 |
| OrganisationOnboardingService | 3 | 11 |
| PatientRoadmapGenerationService | 1 | 6 |
| PayOSService | 4 | 13 |
| SupabaseStorageService | 3 | 11 |
| SystemSettingService | 3 | 8 |

## Sheet-ready UTCID chi tiet tung function

Quy uoc:
- Muc tieu: dua thang vao sheet testcase (1 function = 1 cum UTCID)
- Type: N (normal), A (abnormal), B (boundary)
- Moi function co bo case rieng, khong dung mau lap lai.

- UTCID01 (N): User dang active -> goi `Deactivate` -> `IsActive=false`.
- UTCID02 (N): `UpdatedAt` duoc cap nhat sau khi deactivate.
- UTCID03 (A): User da deactivated -> goi lai khong nem exception, state van false.
- UTCID04 (A): User soft-deleted -> goi `Deactivate` van giu deleted state.
- UTCID05 (B): User co LastLoginAt null -> deactivate van thanh cong.
- UTCID06 (N): Log/event convention khi deactivate (neu co layer wrapper) dung key.
- UTCID07 (N): Response convention cua service goi ham domain nay hop le.

- UTCID01 (N): User dang inactive -> `Activate` -> `IsActive=true`.
- UTCID02 (N): `UpdatedAt` cap nhat khi activate.
- UTCID03 (A): User dang active -> goi lai khong nem exception.
- UTCID04 (A): User bi deleted -> activate khong thay doi trang thai deleted.
- UTCID05 (B): User co nhieu field null optional -> activate van dung.
- UTCID06 (N): Log message convention cho active flow dung.
- UTCID07 (N): Return/response convention cua caller dung.

- UTCID01 (N): User binh thuong -> `SoftDelete` -> `IsDeleted=true`.
- UTCID02 (N): SoftDelete dong thoi deactivate (`IsActive=false` neu rule domain ap dung).
- UTCID03 (N): `UpdatedAt` duoc cap nhat.
- UTCID04 (A): Da soft-delete -> goi lai idempotent.
- UTCID05 (B): User co role/org null -> soft-delete van hop le.
- UTCID06 (N): Log message convention dung.
- UTCID07 (N): Return convention cua service quan ly user dung.

- UTCID01 (N): Goi `UpdateLastLogin` -> `LastLoginAt` gan voi now.
- UTCID02 (B): Goi 2 lan lien tiep -> gia tri lan 2 >= lan 1.
- UTCID03 (A): User inactive -> van cho phep update last login (neu logic khong cam).
- UTCID04 (A): User deleted -> caller khong duoc goi (xac minh guard o service).
- UTCID05 (N): Khong thay doi cac field khac.
- UTCID06 (N): Log convention dung.
- UTCID07 (N): Response convention dung.

### F005 - AuthService.RegisterPatientAsync (5 case)
- UTCID01 (A): Email da ton tai -> Failure conflict message.
- UTCID02 (A): `_userManager.CreateAsync` fail -> rollback transaction + tra identity errors.
- UTCID03 (N): Tao user + add role + tao patient + commit -> Success co UserId/Email.
- UTCID04 (A): Gui email confirm fail -> van Success, co warning log.
- UTCID05 (A): Exception trong transaction -> rollback + Failure generic.

### F006 - AuthService.RegisterOphthalmologistAsync (8 case)
- UTCID01 (A): Khong co credential nao -> Failure.
- UTCID02 (A): Co credentials nhung khong co Degree -> Failure.
- UTCID03 (A): Co Degree nhung khong co License -> Failure.
- UTCID04 (A): Degree co expiry date -> Failure validation.
- UTCID05 (A): License khong co expiry hoac expiry <= issued -> Failure validation.
- UTCID06 (N): Upload credentials + tao profile + commit -> Success.
- UTCID07 (A): Loi sau khi upload file -> rollback va cleanup file da upload.
- UTCID08 (A): Loi gui email confirm/admin -> van Success, ghi warning.

### F007 - AuthService.RegisterOrganisationAsync (3 case)
- UTCID01 (N): Onboarding service tra Success -> AuthService tra Success payload dung.
- UTCID02 (A): Onboarding service tra Conflict -> AuthService giu nguyen conflict.
- UTCID03 (A): Onboarding service throw -> caller nhan failure/exception theo flow.

### F008 - AuthService.GoogleLoginAsync (8 case)
- UTCID01 (A): Google credential invalid jwt -> Unauthorized.
- UTCID02 (A): Payload khong co email -> Failure.
- UTCID03 (A): User ton tai nhung deleted -> Unauthorized.
- UTCID04 (A): User ton tai nhung inactive -> Unauthorized.
- UTCID05 (N): User ton tai, email chua confirm -> auto confirm + update user.
- UTCID06 (N): User ton tai va 2FA bat -> Success two-factor required.
- UTCID07 (N): User moi -> tao account patient + commit + login success.
- UTCID08 (A): Loi DB/identity bat ky -> rollback + Failure generic.

### F009 - AuthService.LoginAsync (12 case)
- UTCID01 (A): Email khong ton tai -> Unauthorized invalid credentials.
- UTCID02 (A): User deleted -> Unauthorized invalid credentials.
- UTCID03 (A): User inactive -> Unauthorized deactivated.
- UTCID04 (A): User dang lockout -> Unauthorized lockout message.
- UTCID05 (A): Password sai dan den `IsLockedOut` -> Unauthorized lockout.
- UTCID06 (A): `IsNotAllowed=true` (email chua confirm) -> Unauthorized.
- UTCID07 (A): Password sai thong thuong -> Unauthorized invalid credentials.
- UTCID08 (A): Ophthalmologist co `VerificationStatus=Rejected` -> Unauthorized reject message.
- UTCID09 (N): `RequiresTwoFactor=true` -> Success two-factor required payload.
- UTCID10 (N): User manager 2FA enabled du `RequiresTwoFactor=false` -> van two-factor required.
- UTCID11 (N): Login thanh cong -> tra AuthResponse day du token + user info.
- UTCID12 (A): Exception runtime -> Failure generic login error.

### F010 - AuthService.VerifyTwoFactorLoginAsync (7 case)
- UTCID01 (A): User khong ton tai/deleted/inactive -> Unauthorized.
- UTCID02 (A): User chua bat 2FA -> Failure.
- UTCID03 (A): Verify authenticator code sai -> Unauthorized.
- UTCID04 (A): Verify recovery code sai -> Unauthorized.
- UTCID05 (N): Recovery code dung -> Success + warning log dung recovery code.
- UTCID06 (N): Authenticator code dung -> Success AuthResponse.
- UTCID07 (A): Exception trong verify -> Failure generic.

### F011 - AuthService.RefreshTokenAsync (7 case)
- UTCID01 (A): Access token khong lay duoc userId/jti -> Unauthorized invalid access token.
- UTCID02 (A): Refresh token hash khong ton tai -> Unauthorized invalid refresh token.
- UTCID03 (A): Stored token khong active -> revoke token family + Unauthorized.
- UTCID04 (A): JwtId/UserId khong khop -> Unauthorized token mismatch.
- UTCID05 (A): User khong ton tai/inactive/deleted -> Unauthorized.
- UTCID06 (N): Refresh hop le -> rotate token + tra access/refresh moi.
- UTCID07 (A): Exception he thong -> Failure generic refresh error.

### F012 - AuthService.LogoutAsync (3 case)
- UTCID01 (N): Token ton tai -> revoke token thanh cong.
- UTCID02 (N): Token khong ton tai -> van Success (idempotent).
- UTCID03 (A): Exception khi revoke -> Failure.

### F013 - AuthService.LogoutAllAsync (2 case)
- UTCID01 (N): Revoke all token thanh cong -> Success.
- UTCID02 (A): Revoke all token throw -> Failure.

### F014 - AuthService.ConfirmEmailAsync (6 case)
- UTCID01 (A): `userId` khong parse duoc Guid -> Failure invalid id.
- UTCID02 (A): User khong ton tai -> NotFound.
- UTCID03 (A): Confirm email fail (token sai/het han) -> Failure errors.
- UTCID04 (N): User role ophthalmologist -> gui notification den tat ca admin.
- UTCID05 (N): Confirm email thanh cong -> Success.
- UTCID06 (A): Exception runtime -> Failure generic.

### F015 - AuthService.ForgotPasswordAsync (3 case)
- UTCID01 (N): Email ton tai -> tao token + gui reset email + Success.
- UTCID02 (N): Email khong ton tai -> khong gui, van Success (anti enumeration).
- UTCID03 (A): Exception send mail/token -> Failure.

### F016 - AuthService.ResetPasswordAsync (5 case)
- UTCID01 (A): UserId sai format -> Failure invalid id.
- UTCID02 (A): User khong ton tai -> NotFound.
- UTCID03 (A): Reset fail do token/password policy -> Failure errors.
- UTCID04 (N): Reset thanh cong -> revoke all refresh token + Success.
- UTCID05 (A): Exception runtime -> Failure.

### F017 - AuthService.GetCurrentUserAsync (5 case)
- UTCID01 (A): User khong ton tai -> Unauthorized.
- UTCID02 (N): User patient -> map `RoleId` bang patient id.
- UTCID03 (N): User ophthalmologist -> map `RoleId`, `IsVerified`, `VerificationStatus`, `ContractStatus`.
- UTCID04 (N): Tra ve `UserInfoResponse` day du profile + roles + 2FA flag.
- UTCID05 (A): Exception khi query details -> Failure.

### F018 - AuthService.ResendConfirmationAsync (4 case)
- UTCID01 (N): User ton tai va chua confirm -> tao token + gui email.
- UTCID02 (N): User da confirm -> bo qua gui email, van Success.
- UTCID03 (N): User khong ton tai -> van Success.
- UTCID04 (A): Exception gui email -> Failure.

### F019 - IdentityService.CheckPasswordAsync (3 case)
- UTCID01 (A): UserId khong ton tai -> false.
- UTCID02 (A): Password sai -> false.
- UTCID03 (N): Password dung -> true.

### F020 - IdentityService.GetUserByEmailAsync (3 case)
- UTCID01 (N): User ton tai va !IsDeleted -> tra `UserDto`.
- UTCID02 (A): User bi IsDeleted -> null.
- UTCID03 (A): Khong tim thay email -> null.

### F021 - IdentityService.GetUserByIdAsync (3 case)
- UTCID01 (N): User ton tai va !IsDeleted -> dto day du.
- UTCID02 (A): User deleted -> null.
- UTCID03 (A): UserId khong ton tai -> null.

### F022 - IdentityService.IsPhoneNumberInUseByOrganizationAsync (4 case)
- UTCID01 (B): Phone input rong/chi ky tu trang -> false.
- UTCID02 (N): Cung so nhung khac format (`+84`, khoang trang, dau gach) -> true.
- UTCID03 (A): So ton tai nhung khac organisation -> false.
- UTCID04 (A): Cung org nhung user deleted -> false.

### F023 - IdentityService.IsEmailConfirmedAsync (3 case)
- UTCID01 (A): User khong ton tai -> false.
- UTCID02 (N): User ton tai, EmailConfirmed=false -> false.
- UTCID03 (N): User ton tai, EmailConfirmed=true -> true.

### F024 - IdentityService.IsUserActiveAsync (3 case)
- UTCID01 (A): User khong ton tai -> false.
- UTCID02 (A): User inactive hoac deleted -> false.
- UTCID03 (N): User active va !deleted -> true.

### F025 - IdentityService.GenerateEmailConfirmationTokenAsync (2 case)
- UTCID01 (A): User khong ton tai -> throw `InvalidOperationException`.
- UTCID02 (N): User ton tai -> token khong rong.

### F026 - IdentityService.GeneratePasswordResetTokenAsync (2 case)
- UTCID01 (A): User khong ton tai -> throw.
- UTCID02 (N): User ton tai -> token khong rong.

### F027 - IdentityService.GetUserRolesAsync (3 case)
- UTCID01 (A): User khong ton tai -> empty list.
- UTCID02 (N): User 1 role -> dung role do.
- UTCID03 (N): User nhieu role -> tra du role.

### F028 - IdentityService.IsInRoleAsync (3 case)
- UTCID01 (A): User khong ton tai -> false.
- UTCID02 (N): User co role -> true.
- UTCID03 (N): User khong co role -> false.

### F029 - IdentityService.GetUserIdsByRoleAndOrganizationAsync (3 case)
- UTCID01 (N): Loc dung user theo role + org.
- UTCID02 (A): User deleted/inactive du cung role -> bi loai.
- UTCID03 (B): Khong co user hop le -> danh sach rong.

### F030 - IdentityService.UpdateLastLoginAsync (2 case)
- UTCID01 (A): User khong ton tai -> no-op.
- UTCID02 (N): User ton tai -> LastLoginAt cap nhat + `UpdateAsync` duoc goi.

### F031 - IdentityService.IsTwoFactorEnabledAsync (3 case)
- UTCID01 (A): User khong ton tai -> false.
- UTCID02 (N): User co 2FA=false -> false.
- UTCID03 (N): User co 2FA=true -> true.

### F032 - IdentityService.GetAuthenticatorKeyAsync (2 case)
- UTCID01 (A): User khong ton tai -> null.
- UTCID02 (N): User ton tai -> tra key.

### F033 - IdentityService.GetOrCreateAuthenticatorKeyAsync (2 case)
- UTCID01 (A): User khong ton tai -> throw.
- UTCID02 (N): User ton tai -> reset key va lay key moi thanh cong.

### F034 - IdentityService.VerifyTwoFactorCodeAsync (3 case)
- UTCID01 (A): User khong ton tai -> false.
- UTCID02 (A): Code sai -> false.
- UTCID03 (N): Code dung -> true.

### F035 - IdentityService.GenerateNewRecoveryCodesAsync (2 case)
- UTCID01 (A): User khong ton tai -> throw.
- UTCID02 (N): User ton tai -> tra mang recovery code theo count.

### F036 - IdentityService.GetRecoveryCodesCountAsync (2 case)
- UTCID01 (A): User khong ton tai -> 0.
- UTCID02 (N): User ton tai -> so luong dung.

### F037 - IdentityService.GenerateAuthenticatorUri (2 case)
- UTCID01 (N): Email + key hop le -> URI dung schema `otpauth://totp`.
- UTCID02 (B): Email co ky tu dac biet -> duoc UrlEncode dung.

### F038 - IdentityService.FormatAuthenticatorKey (2 case)
- UTCID01 (N): Key dai -> chia block 4 ky tu + uppercase.
- UTCID02 (B): Key ngan hon 4 -> van uppercase, khong loi.

### F039 - IdentityService.GetUserMetricsAsync (3 case)
- UTCID01 (N): Du lieu co lich su thang truoc -> tinh % change dung.
- UTCID02 (B): Thang truoc =0 nhung co user moi -> change =100.
- UTCID03 (N): Pending approvals dem dung theo dieu kien.

### F040 - IdentityService.GetUsersInRoleCountAsync (3 case)
- UTCID01 (N): `activeOnly=true` -> chi dem active + !deleted.
- UTCID02 (N): `activeOnly=false` -> dem toan bo !deleted.
- UTCID03 (B): Role khong co user -> 0.

### F041 - IdentityService.GetPendingApprovalsCountAsync (2 case)
- UTCID01 (N): Dem dung user `!EmailConfirmed || !IsActive`.
- UTCID02 (B): Khong co pending -> 0.

### F042 - IdentityService.GetUserDetailsAsync (3 case)
- UTCID01 (A): User khong ton tai -> null.
- UTCID02 (A): User deleted -> null.
- UTCID03 (N): User hop le -> dto day du thong tin profile.

- UTCID01 (N): Tao token hop le -> co `Id`, `CreatedAt`, `ExpiresAt`.
- UTCID02 (N): `IsActive=true` khi moi tao va chua used/revoked.
- UTCID03 (B): `expiryDays` bien canh (1 ngay) -> `ExpiresAt` tinh dung.

- UTCID01 (N): Goi mark used -> set `UsedAt`, `ReplacedByTokenId`.
- UTCID02 (N): Sau mark -> `IsActive=false`.

- UTCID01 (N): Goi revoke -> set `RevokedAt` + reason.
- UTCID02 (N): Sau revoke -> `IsActive=false`.

### F046 - RefreshTokenService.CreateRefreshTokenAsync (2 case)
- UTCID01 (N): Tao refresh token va save DB thanh cong -> return Id.
- UTCID02 (N): Du lieu luu dung `UserId`, `JwtId`, `TokenHash`.

### F047 - RefreshTokenService.GetByTokenHashAsync (3 case)
- UTCID01 (A): Hash khong ton tai -> null.
- UTCID02 (N): Hash ton tai -> map dto dung fields.
- UTCID03 (N): Token revoked/used -> `IsActive=false` trong dto.

### F048 - RefreshTokenService.RotateRefreshTokenAsync (3 case)
- UTCID01 (A): Old token id khong ton tai -> throw.
- UTCID02 (N): Old token ton tai -> tao new token thanh cong.
- UTCID03 (N): Old token duoc `MarkAsUsed` va lien ket token moi.

### F049 - RefreshTokenService.RevokeTokenAsync (2 case)
- UTCID01 (N): Token ton tai -> revoke va save.
- UTCID02 (N): Token khong ton tai -> no-op, khong throw.

### F050 - RefreshTokenService.RevokeAllUserTokensAsync (2 case)
- UTCID01 (N): Co nhieu token chua revoke -> tat ca bi revoke.
- UTCID02 (B): Khong co token hop le -> save van an toan.

### F051 - RefreshTokenService.RevokeTokenFamilyAsync (2 case)
- UTCID01 (A): Token goc khong ton tai -> return, khong throw.
- UTCID02 (N): Token ton tai -> revoke toan bo token cua user.

### F052 - RefreshTokenService.CleanupExpiredTokensAsync (3 case)
- UTCID01 (N): Token het han qua cutoff -> bi xoa.
- UTCID02 (N): Token revoked qua cutoff -> bi xoa.
- UTCID03 (B): Khong co token can xoa -> return 0.

### F053 - TokenService.GenerateAccessTokenAsync (4 case)
- UTCID01 (N): Tao token co claims bat buoc (`sub`,`uid`,`email`,`jti`,`iat`).
- UTCID02 (N): Roles duoc dua vao `ClaimTypes.Role` va `role`.
- UTCID03 (N): Additional claims duoc append dung.
- UTCID04 (B): ExpiresAt dung theo config `AccessTokenExpiryMinutes`.

### F054 - TokenService.GenerateRefreshToken (3 case)
- UTCID01 (N): Token base64 hop le, khong rong.
- UTCID02 (N): 2 lan sinh lien tiep khac nhau.
- UTCID03 (B): Do dai token du lon (>=64 byte random truoc encode).

### F055 - TokenService.ValidateToken (3 case)
- UTCID01 (N): Token hop le -> tra ClaimsPrincipal.
- UTCID02 (A): Token sai signature -> null.
- UTCID03 (A): Token alg khong phai HmacSha256 -> null.

### F056 - TokenService.GetUserIdFromToken (3 case)
- UTCID01 (N): Token hop le co `sub` -> parse Guid thanh cong.
- UTCID02 (N): Khong co `sub` nhung co `uid`/`nameidentifier` -> van lay duoc.
- UTCID03 (A): Claim user id khong parse duoc -> null.

### F057 - TokenService.GetJtiFromToken (2 case)
- UTCID01 (N): Token hop le -> lay `jti`.
- UTCID02 (A): Token invalid -> null.

### F058 - TokenService.HashToken (3 case)
- UTCID01 (N): Cung input -> hash giong nhau.
- UTCID02 (N): Khac input -> hash khac nhau.
- UTCID03 (B): Input rong -> van hash duoc (khong null).

### F059 - AdminQueryService.GetOphthalmologistsAsync (5 case)
- UTCID01 (N): Khong filter -> paging + sort CreatedAt desc dung.
- UTCID02 (N): SearchTerm tim theo fullName/email/phone ILike.
- UTCID03 (N): VerificationStatus nhieu gia tri phan cach dau phay -> loc dung.
- UTCID04 (N): Map licenses/degrees tu Certificates dung theo Type.
- UTCID05 (B): VerificationStatus khong parse duoc -> bo qua filter status.

### F060 - AdminQueryService.GetPatientsAsync (4 case)
- UTCID01 (N): SearchTerm hoat dong theo fullName/email.
- UTCID02 (N): Filter status `active|pending|suspended` dung.
- UTCID03 (N): Paging dung totalCount/items.
- UTCID04 (A): User deleted khong duoc hien thi.

### F061 - AdminQueryService.GetAuditLogsAsync (5 case)
- UTCID01 (N): Loc theo search term action/entity/email dung.
- UTCID02 (N): Loc theo action/entityName/userId dung.
- UTCID03 (N): Loc theo fromDate/toDate dung.
- UTCID04 (N): Paging + sort `CreatedAt desc` dung.
- UTCID05 (B): Log khong co user join -> `UserName=null` van map duoc.

### F062 - AiQuotaService.GetQuotaAsync (5 case)
- UTCID01 (N): Role Patient co profile -> tinh total/used/remaining/free-purchased source dung.
- UTCID02 (A): Role Patient khong co profile -> fallback free quota.
- UTCID03 (N): Role OrgAdmin/Ophthalmologist co org -> tinh quota theo organisation.
- UTCID04 (A): Role organisation nhung user khong co org -> None quota.
- UTCID05 (A): Role la -> None quota.

### F063 - AiQuotaService.HasAvailableQuotaAsync (2 case)
- UTCID01 (N): Remaining > 0 -> true.
- UTCID02 (B): Remaining = 0 -> false.

### F064 - AiQuotaService.DeductQuotaAsync (4 case)
- UTCID01 (N): Patient ton tai -> consume quota + save.
- UTCID02 (N): Org role va org ton tai -> consume quota organisation + save.
- UTCID03 (A): Patient khong ton tai -> throw `InvalidOperationException`.
- UTCID04 (A): User org role khong co org -> throw.

### F065 - AiQuotaService.AddPurchasedQuotaAsync (4 case)
- UTCID01 (N): Patient ton tai -> tang purchased quota.
- UTCID02 (N): Org role co org -> tang purchased quota org.
- UTCID03 (A): Patient khong ton tai -> throw.
- UTCID04 (A): Org khong ton tai -> throw.

### F066 - BetterStackHeartbeatService.GetEmbedUrl (2 case)
- UTCID01 (B): EmbedUrl null/white-space -> null.
- UTCID02 (N): EmbedUrl hop le -> tra lai dung gia tri.

### F067 - BetterStackHeartbeatService.GetMonitorDescriptors (2 case)
- UTCID01 (N): Tra du descriptor cho tat ca enum monitor.
- UTCID02 (N): Truong `Configured` dung theo endpoint setting.

### F068 - BetterStackHeartbeatService.NotifyStartedAsync (3 case)
- UTCID01 (B): Endpoint khong config -> skip, khong throw.
- UTCID02 (N): Endpoint config -> POST thanh cong.
- UTCID03 (A): HTTP fail/exception -> warning log, khong throw.

### F069 - BetterStackHeartbeatService.NotifySucceededAsync (2 case)
- UTCID01 (N): Ping URL hop le -> goi POST.
- UTCID02 (A): Non-success status -> warning log.

### F070 - BetterStackHeartbeatService.NotifyFailedAsync (2 case)
- UTCID01 (N): Co FailUrl -> POST den fail url.
- UTCID02 (A): Khong co FailUrl -> fallback ping url, loi cung khong throw.

- UTCID01 (N): Co du lieu UsedAiQuota>0 -> reset thanh cong va notify started/succeeded.
- UTCID02 (B): Khong co dong can reset -> count=0 van thanh cong.
- UTCID03 (A): Exception DB -> notify failed va rethrow.

### F072 - DashboardMetricsService.GetSystemAdminMetricsAsync (6 case)
- UTCID01 (N): Tinh tong doctor/org/patient va growth phan tram dung.
- UTCID02 (N): Revenue by payment method + percentage dung.
- UTCID03 (N): Monthly/Daily revenue map day du theo period.
- UTCID04 (N): Pending actions (verification/withdraw/onboarding) dung.
- UTCID05 (N): Top doctor by consultation revenue + top org by rating dung.
- UTCID06 (N): BetterStack monitor map vao response dung.

### F073 - DashboardMetricsService.GetRecentScreeningsAsync (4 case)
- UTCID01 (N): Paging total/items dung.
- UTCID02 (N): RiskLevel va `IsCritical` map dung.
- UTCID03 (N): Status `Completed/Analyzing` map theo `ProcessedAt`.
- UTCID04 (B): Screening khong co risk result -> risk null van map duoc.

### F074 - DashboardMetricsService.GetScreeningVolumeTrendsAsync (3 case)
- UTCID01 (N): `weekly` -> group theo dau tuan, label dd MMM.
- UTCID02 (N): `monthly` -> group theo thang, label MMM yyyy.
- UTCID03 (B): `timeRange` la -> fallback monthly.

### F075 - DashboardMetricsService.GetPopulationRiskAnalysisAsync (3 case)
- UTCID01 (N): Tong `TotalPatients` dung.
- UTCID02 (N): Bo qua RiskLevel.None trong danh sach output.
- UTCID03 (B): Khong co result -> percentage=0.

### F076 - DashboardMetricsService.GetSystemHealthAsync (2 case)
- UTCID01 (N): Tra payload co 3 component Database/AI/Notifications.
- UTCID02 (N): `AllSystemsOperational=true` va fields health day du.

### F077 - DashboardMetricsService.GetOphthalmologistMetricsAsync (4 case)
- UTCID01 (A): UserId khong map duoc doctor -> dto rong.
- UTCID02 (N): Dem pending reviews va urgent cases dung.
- UTCID03 (N): Dem completed today dung.
- UTCID04 (N): Tinh open slots today dung theo template/org.

### F078 - DashboardMetricsService.GetOrganisationMetricsAsync (4 case)
- UTCID01 (A): User khong co organisation -> dto rong.
- UTCID02 (N): Dem total appointment + breakdown status dung.
- UTCID03 (N): Utilization rate theo booked/capacity dung.
- UTCID04 (N): Remaining AI quota lay tu `AiQuotaService` dung.

### F079 - DashboardMetricsService.GetPatientMetricsAsync (3 case)
- UTCID01 (A): Khong tim thay patient profile -> dto rong.
- UTCID02 (N): Dem completed screenings / total reports dung.
- UTCID03 (N): Dem upcoming appointments + remaining quota dung.

### F080 - DateTimeService.Now (2 case)
- UTCID01 (N): Gia tri `Now` gan voi `DateTime.Now`.
- UTCID02 (B): Moi lan goi co the thay doi theo thoi gian thuc.

### F081 - DateTimeService.UtcNow (2 case)
- UTCID01 (N): Gia tri `UtcNow` gan voi `DateTime.UtcNow`.
- UTCID02 (B): `Kind` cua gia tri la UTC.

### F082 - EmailService.SendEmailConfirmationAsync (2 case)
- UTCID01 (N): Dung subject/template confirm email.
- UTCID02 (N): Goi `SendAsync` voi `isHtml=true`.

### F083 - EmailService.SendPasswordResetAsync (2 case)
- UTCID01 (N): Dung subject/template reset password.
- UTCID02 (N): Goi `SendAsync` thanh cong.

### F084 - EmailService.SendWelcomeEmailAsync (2 case)
- UTCID01 (N): Dung subject/template welcome.
- UTCID02 (N): Goi `SendAsync` va ghi log info.

### F085 - EmailService.SendAsync (4 case)
- UTCID01 (A): `to` rong -> `ArgumentException`.
- UTCID02 (A): `subject` hoac `body` rong -> `ArgumentException`.
- UTCID03 (N): SMTP connect/auth/send/disconnect thanh cong -> debug log.
- UTCID04 (A): SMTP fail -> log error va rethrow.

### F086 - GoogleMeetService.CreateMeetingAsync (5 case)
- UTCID01 (N): Tao event co meet link ngay -> return `MeetingInfo`.
- UTCID02 (N): Khong co link ngay lan dau -> retry va lay duoc link.
- UTCID03 (A): Retry het van khong co link -> cleanup orphan event + throw.
- UTCID04 (N): Co attendeeEmails -> map attendees vao event.
- UTCID05 (B): `durationMinutes` null -> dung default duration setting.

### F087 - GoogleMeetService.DeleteMeetingAsync (2 case)
- UTCID01 (N): Xoa event thanh cong.
- UTCID02 (A): API tra 404 not found -> warning, khong throw.

### F088 - GoogleMeetService.Dispose (1 case)
- UTCID01 (N): Goi dispose -> calendar service duoc dispose an toan.

### F089 - NotificationService.SendAsync (typed) (4 case)
- UTCID01 (N): Payload object -> serialize camelCase JSON.
- UTCID02 (N): Persist notification + SaveChanges + broadcast notification DTO.
- UTCID03 (N): Tinh unread count va broadcast unread count dung.
- UTCID04 (A): Loi repository/hub -> log error va rethrow.

### F090 - NotificationService.SendAsync (legacy) (1 case)
- UTCID01 (N): Overload legacy chuyen dung tham so sang overload typed.

### F091 - OrganisationOnboardingService.SubmitRequestAsync (3 case)
- UTCID01 (A): Da ton tai pending request cung email -> Conflict.
- UTCID02 (N): Tao request moi + save thanh cong.
- UTCID03 (N): Gui notify admin sau khi save thanh cong.

### F092 - OrganisationOnboardingService.GetRequestsAsync (2 case)
- UTCID01 (N): Tra danh sach sort `CreatedAt desc`.
- UTCID02 (N): Map dto day du (`OrgType`, `Status`, `ApprovedAt`...).

### F093 - OrganisationOnboardingService.ApproveRequestAsync (6 case)
- UTCID01 (A): RequestId khong ton tai -> NotFound.
- UTCID02 (A): Request khong con Pending -> Failure da xu ly.
- UTCID03 (A): Contact email da ton tai user -> Conflict.
- UTCID04 (N): Tao org admin + add role + tao organisation + approve request -> Success.
- UTCID05 (N): Co contract template active -> tao contract va send for signature.
- UTCID06 (A): Exception bat ky -> rollback transaction + Failure.

### F094 - PatientRoadmapGenerationService.GenerateFromDiagnosisAsync (6 case)
- UTCID01 (A): `PatientId` rong -> Failure validation.
- UTCID02 (A): `ScreeningId` rong hoac `AiScreeningRawJson` rong -> Failure validation.
- UTCID03 (A): Chua config ApiKey -> Failure.
- UTCID04 (N): AI tra JSON hop le -> parse + normalize + Success.
- UTCID05 (A): AI tra JSON sai format lien tiep -> retry het va Failure.
- UTCID06 (A): HTTP timeout/network error qua max retry -> Failure unavailable.

### F095 - PayOSService.CreatePaymentLinkAsync (5 case)
- UTCID01 (N): Tao payment link thanh cong, return `checkoutUrl` + `orderCode`.
- UTCID02 (B): `returnUrl/cancelUrl` rong -> dung default URL tu settings.
- UTCID03 (N): Description >25 ky tu -> bi truncate dung gioi han.
- UTCID04 (N): URL output co append query param `orderCode`.
- UTCID05 (A): SDK tra null hoac empty url -> throw exception.

### F096 - PayOSService.GetPaymentStatusAsync (3 case)
- UTCID01 (N): Query thanh cong -> map `Status/Amount/TxnRef`.
- UTCID02 (A): PaymentInfo null -> throw "Payment not found".
- UTCID03 (A): SDK throw -> wrap va throw exception service.

### F097 - PayOSService.VerifyWebhookSignatureAsync (2 case)
- UTCID01 (N): Flow hien tai -> return true.
- UTCID02 (A): Exception branch -> return false + log error.

### F098 - PayOSService.CancelPaymentAsync (3 case)
- UTCID01 (N): SDK cancel tra object -> true.
- UTCID02 (B): SDK tra null -> false.
- UTCID03 (A): Exception -> false va log error.

- UTCID01 (N): Slot available, booked=0, qua gio -> set Expired + UpdatedAt.
- UTCID02 (N): Slot da dat hoac chua qua gio -> khong expire.
- UTCID03 (A): Exception DB -> notify failed + rethrow.

### F100 - SupabaseStorageService.SaveFileAsync (5 case)
- UTCID01 (N): Filename duoc sanitize + path tao dung theo subfolder.
- UTCID02 (A): Stream rong -> throw `InvalidOperationException`.
- UTCID03 (N): Upload thanh cong -> tra public URL.
- UTCID04 (B): SDK tra URL tuong doi -> fallback thanh absolute URL.
- UTCID05 (N): ContentType map dung theo extension file.

### F101 - SupabaseStorageService.DeleteFile (3 case)
- UTCID01 (N): Input full URL -> extract path va remove thanh cong -> true.
- UTCID02 (N): Input relative path hop le -> remove thanh cong -> true.
- UTCID03 (A): Remove throw exception/path invalid -> false.

### F102 - SupabaseStorageService.FileExists (3 case)
- UTCID01 (N): HEAD tra success -> true.
- UTCID02 (A): HEAD tra non-success -> false.
- UTCID03 (A): Input rong/exception request -> false.

### F103 - SystemSettingService.GetSettingAsync (2 case)
- UTCID01 (N): Key ton tai -> tra value dung.
- UTCID02 (A): Key khong ton tai -> null.

### F104 - SystemSettingService.GetAllSettingsAsync (2 case)
- UTCID01 (N): Co du lieu -> dictionary day du key/value.
- UTCID02 (B): Khong co du lieu -> dictionary rong.

### F105 - SystemSettingService.UpdateSettingsAsync (4 case)
- UTCID01 (B): Input null/rong -> no-op.
- UTCID02 (N): Key da ton tai -> goi `UpdateValue`.
- UTCID03 (N): Key moi -> add `SystemSetting` moi.
- UTCID04 (N): Sau update -> `SaveChangesAsync` duoc goi + log info keys.
