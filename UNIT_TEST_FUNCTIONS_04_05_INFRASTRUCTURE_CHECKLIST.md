# Unit test checklist - Infrastructure services/functions (UTC thuc)

Ngay cap nhat: 13/04/2026
Scope: d:/sep/AuraEyes_BE/src/Infrastructure/Identity + src/Infrastructure/Services

## Huong dan: doc checklist + copy sang sheet (de hieu)

**Hai file dung chung ma F001–F096 (96 function).**

| File | Vai tro |
|------|---------|
| **CHECKLIST (file nay)** | Nguon chi tiet tung **UTCID**: dong `UTCIDxx - dieu kien -> ket qua`. Phan sau `->` la y de ghi vao cot **Expected** tren sheet. |
| **UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_SHEET_LAYOUT.md** | Layout giong Excel: moi `## Fxxx` la 1 function; bang **Result Matrix** = **1 hang = 1 UTCID** (de dien va copy). |

**Quy tac don gian**

1. Tim **Function Code** (Fxxx) trong bang **Function Catalog** ben duoi.
2. Keo xuong muc `### Fxxx - ...` trong file nay: moi bullet **UTCID01, UTCID02, ...** tuong ung **mot hang** trong Result Matrix cua cung Fxxx trong file SHEET_LAYOUT.
3. **Condition Matrix** trong SHEET_LAYOUT chi la **tom tat** (nhieu UTCID co the gop 1 dong). Checklist + Result Matrix la **khung**; cot Expected tren sheet chi **dien co chung cu** tu assert trong test (xem quy tac o tren).

**Luu y:** Unit test trong `tests/Infrastructure.UnitTests` dung `Assert`/`Verify` trong code; file MD khong tu dong dong bo — ban cap nhat tay khi doi spec.

**Quy tac Expected (return / exception / log):** chi ghi noi dung khi **co assert hoac Verify tuong ung** trong test, hoac ban da doi chieu ro va ghi ten test. **Khong** dien theo suy doan / doc code ma test khong kiem tra — de trong hoac `n/a (chua assert trong test)`.

## Trang thai hien tai

- **Sheet Result Matrix:** tung co lan dien hang loat bang `scripts/fill_all_sheet_expected.py` (gom ca phan suy dien tu contract cho cho chua co test). **Chuan dung:** chi giu / sua cot Expected khi **co chung cu tu test**; phan con lai de trong hoac danh dau `n/a` — xem huong dan dau file SHEET_LAYOUT.
- Da tao lai project test: tests/Infrastructure.UnitTests.
- Scope checklist da bo cac case fake (FakeEmailService, FakePayOSService).
- Tat ca function da duoc nang cap bo case theo huong Condition / Return / Log message / Response convention; LoginAsync (F005) giu bo case chi tiet theo mau.

## Kiem tra lai theo "services only" (cap nhat)

- Chi tinh method trong cac class `*Service` trong bang **Function Catalog** (96 function, ma **F001-F096** lien tuc).
- **Khong** nam trong bang nay: method tren entity (vd. `ApplicationUser`), method tren entity `RefreshToken`, job/worker — trace rieng, **khong** dung lai ma F001-F096 o day.

## Function Catalog (for Sheet Import)

| No  | Requirement Name                    | Class Name                      | Function Name                         | Function Code(Optional) | Sheet Name                            | Description                                                                                                                                                                       | Pre-Condition                                                                                                                      |
| --- | ----------------------------------- | ------------------------------- | ------------------------------------- | ----------------------- | ------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| No  | Requirement Name                    | Class Name                      | Function Name                         | Function Code(Optional) | Sheet Name                            | Description                                                                                                                                                                       | Pre-Condition                                                                                                                      |
| --- | ----------------------------------- | ------------------------------- | ------------------------------------- | ----------------------- | ------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| 1   | Register patient account            | AuthService                     | RegisterPatientAsync                  | F001                    | RegisterPatientAsync                  | Validate 'Register patient account' in AuthService.RegisterPatientAsync, covering success/failure flow, response contract, and logging behavior.                                  | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 2   | Lookup account by citizen id        | AuthService                     | LookupAccountByCitizenIdAsync         | F002                    | LookupAccountByCitizenIdAsync         | Validate 'Lookup account by citizen id' in AuthService.LookupAccountByCitizenIdAsync, covering exists/not exists and masked email logic.                                          | UserManager and repositories are mocked; citizenId input is valid or invalid.                                                      |
| 3   | Google login                        | AuthService                     | GoogleLoginAsync                      | F003                    | GoogleLoginAsync                      | Validate 'Google login' in AuthService.GoogleLoginAsync, covering success/failure flow, response contract, and logging behavior.                                                  | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 4   | Login with password                 | AuthService                     | LoginAsync                            | F004                    | LoginAsync                            | Validate 'Login with password' in AuthService.LoginAsync, covering success/failure flow, response contract, and logging behavior.                                                 | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 5   | Verify 2FA login                    | AuthService                     | VerifyTwoFactorLoginAsync             | F005                    | VerifyTwoFactorLoginAsync             | Validate 'Verify 2FA login' in AuthService.VerifyTwoFactorLoginAsync, covering success/failure flow, response contract, and logging behavior.                                     | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 6   | Refresh token flow                  | AuthService                     | RefreshTokenAsync                     | F006                    | RefreshTokenAsync                     | Validate 'Refresh token flow' in AuthService.RefreshTokenAsync, covering success/failure flow, response contract, and logging behavior.                                           | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 7   | Logout single device                | AuthService                     | LogoutAsync                           | F007                    | LogoutAsync                           | Validate 'Logout single device' in AuthService.LogoutAsync, covering success/failure flow, response contract, and logging behavior.                                               | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 8   | Logout all devices                  | AuthService                     | LogoutAllAsync                        | F008                    | LogoutAllAsync                        | Validate 'Logout all devices' in AuthService.LogoutAllAsync, covering success/failure flow, response contract, and logging behavior.                                              | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 9   | Confirm email                       | AuthService                     | ConfirmEmailAsync                     | F009                    | ConfirmEmailAsync                     | Validate 'Confirm email' in AuthService.ConfirmEmailAsync, covering success/failure flow, response contract, and logging behavior.                                                | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 10  | Forgot password                     | AuthService                     | ForgotPasswordAsync                   | F010                    | ForgotPasswordAsync                   | Validate 'Forgot password' in AuthService.ForgotPasswordAsync, covering success/failure flow, response contract, and logging behavior.                                            | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 11  | Reset password                      | AuthService                     | ResetPasswordAsync                    | F011                    | ResetPasswordAsync                    | Validate 'Reset password' in AuthService.ResetPasswordAsync, covering success/failure flow, response contract, and logging behavior.                                              | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 12  | Get current user                    | AuthService                     | GetCurrentUserAsync                   | F012                    | GetCurrentUserAsync                   | Validate 'Get current user' in AuthService.GetCurrentUserAsync, covering success/failure flow and ClinicStaff sub-role mapping.                                                   | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 13  | Resend confirmation email           | AuthService                     | ResendConfirmationAsync               | F013                    | ResendConfirmationAsync               | Validate 'Resend confirmation email' in AuthService.ResendConfirmationAsync, covering success/failure flow, response contract, and logging behavior.                              | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. |
| 14  | Check password                      | IdentityService                 | CheckPasswordAsync                    | F014                    | CheckPasswordAsync                    | Validate 'Check password' in IdentityService.CheckPasswordAsync, covering success/failure flow, response contract, and logging behavior.                                          | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 15  | Get user by email                   | IdentityService                 | GetUserByEmailAsync                   | F015                    | GetUserByEmailAsync                   | Validate 'Get user by email' in IdentityService.GetUserByEmailAsync, covering success/failure flow, response contract, and logging behavior.                                      | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 16  | Get user by id                      | IdentityService                 | GetUserByIdAsync                      | F016                    | GetUserByIdAsync                      | Validate 'Get user by id' in IdentityService.GetUserByIdAsync, covering success/failure flow, response contract, and logging behavior.                                            | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 17  | Check email confirmed               | IdentityService                 | IsEmailConfirmedAsync                 | F017                    | IsEmailConfirmedAsync                 | Validate 'Check email confirmed' in IdentityService.IsEmailConfirmedAsync, covering success/failure flow, response contract, and logging behavior.                                | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 18  | Check user active                   | IdentityService                 | IsUserActiveAsync                     | F018                    | IsUserActiveAsync                     | Validate 'Check user active' in IdentityService.IsUserActiveAsync, covering success/failure flow, response contract, and logging behavior.                                        | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 19  | Generate email confirmation token   | IdentityService                 | GenerateEmailConfirmationTokenAsync   | F019                    | GenerateEmailConfirmationTokenAsync   | Validate 'Generate email confirmation token' in IdentityService.GenerateEmailConfirmationTokenAsync, covering success/failure flow, response contract, and logging behavior.      | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 20  | Generate password reset token       | IdentityService                 | GeneratePasswordResetTokenAsync       | F020                    | GeneratePasswordResetTokenAsync       | Validate 'Generate password reset token' in IdentityService.GeneratePasswordResetTokenAsync, covering success/failure flow, response contract, and logging behavior.              | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 21  | Get user roles                      | IdentityService                 | GetUserRolesAsync                     | F021                    | GetUserRolesAsync                     | Validate 'Get user roles' in IdentityService.GetUserRolesAsync, covering success/failure flow, response contract, and logging behavior.                                           | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 22  | Check user in role                  | IdentityService                 | IsInRoleAsync                         | F022                    | IsInRoleAsync                         | Validate 'Check user in role' in IdentityService.IsInRoleAsync, covering success/failure flow, response contract, and logging behavior.                                           | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 23  | Update last login async             | IdentityService                 | UpdateLastLoginAsync                  | F023                    | UpdateLastLoginAsync                  | Validate 'Update last login async' in IdentityService.UpdateLastLoginAsync, covering success/failure flow, response contract, and logging behavior.                               | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 24  | Check 2FA enabled                   | IdentityService                 | IsTwoFactorEnabledAsync               | F024                    | IsTwoFactorEnabledAsync               | Validate 'Check 2FA enabled' in IdentityService.IsTwoFactorEnabledAsync, covering success/failure flow, response contract, and logging behavior.                                  | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 25  | Get authenticator key               | IdentityService                 | GetAuthenticatorKeyAsync              | F025                    | GetAuthenticatorKeyAsync              | Validate 'Get authenticator key' in IdentityService.GetAuthenticatorKeyAsync, covering success/failure flow, response contract, and logging behavior.                             | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 26  | Get or create authenticator key     | IdentityService                 | GetOrCreateAuthenticatorKeyAsync      | F026                    | GetOrCreateAuthenticatorKeyAsync      | Validate 'Get or create authenticator key' in IdentityService.GetOrCreateAuthenticatorKeyAsync, covering success/failure flow, response contract, and logging behavior.           | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 27  | Verify 2FA code                     | IdentityService                 | VerifyTwoFactorCodeAsync              | F027                    | VerifyTwoFactorCodeAsync              | Validate 'Verify 2FA code' in IdentityService.VerifyTwoFactorCodeAsync, covering success/failure flow, response contract, and logging behavior.                                   | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 28  | Generate recovery codes             | IdentityService                 | GenerateNewRecoveryCodesAsync         | F028                    | GenerateNewRecoveryCodesAsync         | Validate 'Generate recovery codes' in IdentityService.GenerateNewRecoveryCodesAsync, covering success/failure flow, response contract, and logging behavior.                      | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 29  | Get recovery code count             | IdentityService                 | GetRecoveryCodesCountAsync            | F029                    | GetRecoveryCodesCountAsync            | Validate 'Get recovery code count' in IdentityService.GetRecoveryCodesCountAsync, covering success/failure flow, response contract, and logging behavior.                         | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 30  | Generate authenticator URI          | IdentityService                 | GenerateAuthenticatorUri              | F030                    | GenerateAuthenticatorUri              | Validate 'Generate authenticator URI' in IdentityService.GenerateAuthenticatorUri, covering success/failure flow, response contract, and logging behavior.                        | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 31  | Format authenticator key            | IdentityService                 | FormatAuthenticatorKey                | F031                    | FormatAuthenticatorKey                | Validate 'Format authenticator key' in IdentityService.FormatAuthenticatorKey, covering success/failure flow, response contract, and logging behavior.                            | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 32  | Get user metrics                    | IdentityService                 | GetUserMetricsAsync                   | F032                    | GetUserMetricsAsync                   | Validate 'Get user metrics' in IdentityService.GetUserMetricsAsync, covering success/failure flow, response contract, and logging behavior.                                       | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 33  | Get users in role count             | IdentityService                 | GetUsersInRoleCountAsync              | F033                    | GetUsersInRoleCountAsync              | Validate 'Get users in role count' in IdentityService.GetUsersInRoleCountAsync, covering success/failure flow, response contract, and logging behavior.                           | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 34  | Get pending approvals count         | IdentityService                 | GetPendingApprovalsCountAsync         | F034                    | GetPendingApprovalsCountAsync         | Validate 'Get pending approvals count' in IdentityService.GetPendingApprovalsCountAsync, covering success/failure flow, response contract, and logging behavior.                  | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 35  | Get user details                    | IdentityService                 | GetUserDetailsAsync                   | F035                    | GetUserDetailsAsync                   | Validate 'Get user details' in IdentityService.GetUserDetailsAsync, covering success/failure flow, response contract, and logging behavior.                                       | UserManager and RoleManager are mocked with user/role seed data matching each scenario.                                            |
| 36  | Create refresh token record         | RefreshTokenService             | CreateRefreshTokenAsync               | F036                    | CreateRefreshTokenAsync               | Validate 'Create refresh token record' in RefreshTokenService.CreateRefreshTokenAsync, covering success/failure flow, response contract, and logging behavior.                    | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready.               |
| 37  | Get refresh token by hash           | RefreshTokenService             | GetByTokenHashAsync                   | F037                    | GetByTokenHashAsync                   | Validate 'Get refresh token by hash' in RefreshTokenService.GetByTokenHashAsync, covering success/failure flow, response contract, and logging behavior.                          | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready.               |
| 38  | Rotate refresh token                | RefreshTokenService             | RotateRefreshTokenAsync               | F038                    | RotateRefreshTokenAsync               | Validate 'Rotate refresh token' in RefreshTokenService.RotateRefreshTokenAsync, covering success/failure flow, response contract, and logging behavior.                           | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready.               |
| 39  | Revoke token by hash                | RefreshTokenService             | RevokeTokenAsync                      | F039                    | RevokeTokenAsync                      | Validate 'Revoke token by hash' in RefreshTokenService.RevokeTokenAsync, covering success/failure flow, response contract, and logging behavior.                                  | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready.               |
| 40  | Revoke all user tokens              | RefreshTokenService             | RevokeAllUserTokensAsync              | F040                    | RevokeAllUserTokensAsync              | Validate 'Revoke all user tokens' in RefreshTokenService.RevokeAllUserTokensAsync, covering success/failure flow, response contract, and logging behavior.                        | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready.               |
| 41  | Revoke token family                 | RefreshTokenService             | RevokeTokenFamilyAsync                | F041                    | RevokeTokenFamilyAsync                | Validate 'Revoke token family' in RefreshTokenService.RevokeTokenFamilyAsync, covering success/failure flow, response contract, and logging behavior.                             | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready.               |
| 42  | Cleanup expired tokens              | RefreshTokenService             | CleanupExpiredTokensAsync             | F042                    | CleanupExpiredTokensAsync             | Validate 'Cleanup expired tokens' in RefreshTokenService.CleanupExpiredTokensAsync, covering success/failure flow, response contract, and logging behavior.                       | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready.               |
| 43  | Generate access token               | TokenService                    | GenerateAccessTokenAsync              | F043                    | GenerateAccessTokenAsync              | Validate 'Generate access token' in TokenService.GenerateAccessTokenAsync, covering success/failure flow, response contract, and logging behavior.                                | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths.                   |
| 44  | Generate refresh token string       | TokenService                    | GenerateRefreshToken                  | F044                    | GenerateRefreshToken                  | Validate 'Generate refresh token string' in TokenService.GenerateRefreshToken, covering success/failure flow, response contract, and logging behavior.                            | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths.                   |
| 45  | Validate token                      | TokenService                    | ValidateToken                         | F045                    | ValidateToken                         | Validate 'Validate token' in TokenService.ValidateToken, covering success/failure flow, response contract, and logging behavior.                                                  | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths.                   |
| 46  | Get user id from token              | TokenService                    | GetUserIdFromToken                    | F046                    | GetUserIdFromToken                    | Validate 'Get user id from token' in TokenService.GetUserIdFromToken, covering success/failure flow, response contract, and logging behavior.                                     | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths.                   |
| 47  | Get jti from token                  | TokenService                    | GetJtiFromToken                       | F047                    | GetJtiFromToken                       | Validate 'Get jti from token' in TokenService.GetJtiFromToken, covering success/failure flow, response contract, and logging behavior.                                            | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths.                   |
| 48  | Hash token                          | TokenService                    | HashToken                             | F048                    | HashToken                             | Validate 'Hash token' in TokenService.HashToken, covering success/failure flow, response contract, and logging behavior.                                                          | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths.                   |
| 49  | Get ophthalmologists query          | AdminQueryService               | GetOphthalmologistsAsync              | F049                    | GetOphthalmologistsAsync              | Validate 'Get ophthalmologists query' in AdminQueryService.GetOphthalmologistsAsync, covering success/failure flow, response contract, and logging behavior.                      | ApplicationDbContext is seeded; filter, sort, and paging inputs are valid.                                                         |
| 50  | Get patients query                  | AdminQueryService               | GetPatientsAsync                      | F050                    | GetPatientsAsync                      | Validate 'Get patients query' in AdminQueryService.GetPatientsAsync, covering success/failure flow, response contract, and logging behavior.                                      | ApplicationDbContext is seeded; filter, sort, and paging inputs are valid.                                                         |
| 51  | Get audit logs query                | AdminQueryService               | GetAuditLogsAsync                     | F051                    | GetAuditLogsAsync                     | Validate 'Get audit logs query' in AdminQueryService.GetAuditLogsAsync, covering success/failure flow, response contract, and logging behavior.                                   | ApplicationDbContext is seeded; filter, sort, and paging inputs are valid.                                                         |
| 52  | Get AI quota                        | AiQuotaService                  | GetQuotaAsync                         | F052                    | GetQuotaAsync                         | Validate 'Get AI quota' in AiQuotaService.GetQuotaAsync, covering success/failure flow, response contract, and logging behavior.                                                  | ApplicationDbContext and system-setting dependency are seeded with quota data; user context is valid.                              |
| 53  | Check AI quota available            | AiQuotaService                  | HasAvailableQuotaAsync                | F053                    | HasAvailableQuotaAsync                | Validate 'Check AI quota available' in AiQuotaService.HasAvailableQuotaAsync, covering success/failure flow, response contract, and logging behavior.                             | ApplicationDbContext and system-setting dependency are seeded with quota data; user context is valid.                              |
| 54  | Deduct AI quota                     | AiQuotaService                  | DeductQuotaAsync                      | F054                    | DeductQuotaAsync                      | Validate 'Deduct AI quota' in AiQuotaService.DeductQuotaAsync, covering success/failure flow, response contract, and logging behavior.                                            | ApplicationDbContext and system-setting dependency are seeded with quota data; user context is valid.                              |
| 55  | Add purchased AI quota              | AiQuotaService                  | AddPurchasedQuotaAsync                | F055                    | AddPurchasedQuotaAsync                | Validate 'Add purchased AI quota' in AiQuotaService.AddPurchasedQuotaAsync, covering success/failure flow, response contract, and logging behavior.                               | ApplicationDbContext and system-setting dependency are seeded with quota data; user context is valid.                              |
| 56  | Get BetterStack embed URL           | BetterStackHeartbeatService     | GetEmbedUrl                           | F056                    | GetEmbedUrl                           | Validate 'Get BetterStack embed URL' in BetterStackHeartbeatService.GetEmbedUrl, covering success/failure flow, response contract, and logging behavior.                          | Heartbeat/monitor configuration is valid; client and logger are initialized.                                                       |
| 57  | Get BetterStack monitor descriptors | BetterStackHeartbeatService     | GetMonitorDescriptors                 | F057                    | GetMonitorDescriptors                 | Validate 'Get BetterStack monitor descriptors' in BetterStackHeartbeatService.GetMonitorDescriptors, covering success/failure flow, response contract, and logging behavior.      | Heartbeat/monitor configuration is valid; client and logger are initialized.                                                       |
| 58  | Notify monitor started              | BetterStackHeartbeatService     | NotifyStartedAsync                    | F058                    | NotifyStartedAsync                    | Validate 'Notify monitor started' in BetterStackHeartbeatService.NotifyStartedAsync, covering success/failure flow, response contract, and logging behavior.                      | Heartbeat/monitor configuration is valid; client and logger are initialized.                                                       |
| 59  | Notify monitor succeeded            | BetterStackHeartbeatService     | NotifySucceededAsync                  | F059                    | NotifySucceededAsync                  | Validate 'Notify monitor succeeded' in BetterStackHeartbeatService.NotifySucceededAsync, covering success/failure flow, response contract, and logging behavior.                  | Heartbeat/monitor configuration is valid; client and logger are initialized.                                                       |
| 60  | Notify monitor failed               | BetterStackHeartbeatService     | NotifyFailedAsync                     | F060                    | NotifyFailedAsync                     | Validate 'Notify monitor failed' in BetterStackHeartbeatService.NotifyFailedAsync, covering success/failure flow, response contract, and logging behavior.                        | Heartbeat/monitor configuration is valid; client and logger are initialized.                                                       |
| 61  | Get system admin metrics            | DashboardMetricsService         | GetSystemAdminMetricsAsync            | F061                    | GetSystemAdminMetricsAsync            | Validate 'Get system admin metrics' in DashboardMetricsService.GetSystemAdminMetricsAsync, covering success/failure flow, response contract, and logging behavior.                | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid.                                          |
| 62  | Get recent screenings               | DashboardMetricsService         | GetRecentScreeningsAsync              | F062                    | GetRecentScreeningsAsync              | Validate 'Get recent screenings' in DashboardMetricsService.GetRecentScreeningsAsync, covering success/failure flow, response contract, and logging behavior.                     | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid.                                          |
| 63  | Get screening volume trends         | DashboardMetricsService         | GetScreeningVolumeTrendsAsync         | F063                    | GetScreeningVolumeTrendsAsync         | Validate 'Get screening volume trends' in DashboardMetricsService.GetScreeningVolumeTrendsAsync, covering success/failure flow, response contract, and logging behavior.          | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid.                                          |
| 64  | Get population risk analysis        | DashboardMetricsService         | GetPopulationRiskAnalysisAsync        | F064                    | GetPopulationRiskAnalysisAsync        | Validate 'Get population risk analysis' in DashboardMetricsService.GetPopulationRiskAnalysisAsync, covering success/failure flow, response contract, and logging behavior.        | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid.                                          |
| 65  | Get system health                   | DashboardMetricsService         | GetSystemHealthAsync                  | F065                    | GetSystemHealthAsync                  | Validate 'Get system health' in DashboardMetricsService.GetSystemHealthAsync, covering success/failure flow, response contract, and logging behavior.                             | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid.                                          |
| 66  | Get ophthalmologist metrics         | DashboardMetricsService         | GetOphthalmologistMetricsAsync        | F066                    | GetOphthalmologistMetricsAsync        | Validate 'Get ophthalmologist metrics' in DashboardMetricsService.GetOphthalmologistMetricsAsync, covering success/failure flow, response contract, and logging behavior.         | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid.                                          |
| 67  | Get patient metrics                 | DashboardMetricsService         | GetPatientMetricsAsync                | F067                    | GetPatientMetricsAsync                | Validate 'Get patient metrics' in DashboardMetricsService.GetPatientMetricsAsync, covering success/failure flow, response contract, and logging behavior.                         | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid.                                          |
| 68  | Get local now                       | DateTimeService                 | Now (property)                        | F068                    | Now (property)                        | Validate 'Get local now' in DateTimeService.Now (property), covering success/failure flow, response contract, and logging behavior.                                               | Service is instantiated directly; no external dependency is required.                                                              |
| 69  | Get UTC now                         | DateTimeService                 | UtcNow (property)                     | F069                    | UtcNow (property)                     | Validate 'Get UTC now' in DateTimeService.UtcNow (property), covering success/failure flow, response contract, and logging behavior.                                              | Service is instantiated directly; no external dependency is required.                                                              |
| 70  | Send email confirmation             | EmailService                    | SendEmailConfirmationAsync            | F070                    | SendEmailConfirmationAsync            | Validate 'Send email confirmation' in EmailService.SendEmailConfirmationAsync, covering success/failure flow, response contract, and logging behavior.                            | SMTP settings are valid; recipient and template/content inputs are valid; transport can be mocked.                                 |
| 71  | Send password reset email           | EmailService                    | SendPasswordResetAsync                | F071                    | SendPasswordResetAsync                | Validate 'Send password reset email' in EmailService.SendPasswordResetAsync, covering success/failure flow, response contract, and logging behavior.                              | SMTP settings are valid; recipient and template/content inputs are valid; transport can be mocked.                                 |
| 72  | Send welcome email                  | EmailService                    | SendWelcomeEmailAsync                 | F072                    | SendWelcomeEmailAsync                 | Validate 'Send welcome email' in EmailService.SendWelcomeEmailAsync, covering success/failure flow, response contract, and logging behavior.                                      | SMTP settings are valid; recipient and template/content inputs are valid; transport can be mocked.                                 |
| 73  | Send generic email                  | EmailService                    | SendAsync                             | F073                    | SendAsync                             | Validate 'Send generic email' in EmailService.SendAsync, covering success/failure flow, response contract, and logging behavior.                                                  | SMTP settings are valid; recipient and template/content inputs are valid; transport can be mocked.                                 |
| 74  | Create Google Meet meeting          | GoogleMeetService               | CreateMeetingAsync                    | F074                    | CreateMeetingAsync                    | Validate 'Create Google Meet meeting' in GoogleMeetService.CreateMeetingAsync, covering success/failure flow, response contract, and logging behavior.                            | Google credentials and service configuration are valid; meeting request inputs are valid.                                          |
| 75  | Delete Google Meet meeting          | GoogleMeetService               | DeleteMeetingAsync                    | F075                    | DeleteMeetingAsync                    | Validate 'Delete Google Meet meeting' in GoogleMeetService.DeleteMeetingAsync, covering success/failure flow, response contract, and logging behavior.                            | Google credentials and service configuration are valid; meeting request inputs are valid.                                          |
| 76  | Dispose Google Meet service         | GoogleMeetService               | Dispose                               | F076                    | Dispose                               | Validate 'Dispose Google Meet service' in GoogleMeetService.Dispose, covering success/failure flow, response contract, and logging behavior.                                      | Google credentials and service configuration are valid; meeting request inputs are valid.                                          |
| 77  | Send typed notification             | NotificationService             | SendAsync (typed)                     | F077                    | SendAsync (typed)                     | Validate 'Send typed notification' in NotificationService.SendAsync (typed), covering success/failure flow, response contract, and logging behavior.                              | Notification repository, UnitOfWork, and hub service are mocked; user/message/type payload is prepared.                            |
| 78  | Send legacy notification            | NotificationService             | SendAsync (legacy)                    | F078                    | SendAsync (legacy)                    | Validate 'Send legacy notification' in NotificationService.SendAsync (legacy), covering success/failure flow, response contract, and logging behavior.                            | Notification repository, UnitOfWork, and hub service are mocked; user/message/type payload is prepared.                            |
| 79  | Generate roadmap from diagnosis     | PatientRoadmapGenerationService | GenerateFromDiagnosisAsync            | F079                    | GenerateFromDiagnosisAsync            | Validate 'Generate roadmap from diagnosis' in PatientRoadmapGenerationService.GenerateFromDiagnosisAsync, covering success/failure flow, response contract, and logging behavior. | Diagnosis input and roadmap rules/mapping are prepared.                                                                            |
| 80  | Create PayOS payment link           | PayOSService                    | CreatePaymentLinkAsync                | F080                    | CreatePaymentLinkAsync                | Validate 'Create PayOS payment link' in PayOSService.CreatePaymentLinkAsync, covering success/failure flow, response contract, and logging behavior.                              | PayOS settings are valid (clientId, apiKey, checksumKey); payment payload inputs are valid.                                        |
| 81  | Get PayOS payment status            | PayOSService                    | GetPaymentStatusAsync                 | F081                    | GetPaymentStatusAsync                 | Validate 'Get PayOS payment status' in PayOSService.GetPaymentStatusAsync, covering success/failure flow, response contract, and logging behavior.                                | PayOS settings are valid (clientId, apiKey, checksumKey); payment payload inputs are valid.                                        |
| 82  | Verify PayOS webhook signature      | PayOSService                    | VerifyWebhookSignatureAsync           | F082                    | VerifyWebhookSignatureAsync           | Validate 'Verify PayOS webhook signature' in PayOSService.VerifyWebhookSignatureAsync, covering success/failure flow, response contract, and logging behavior.                    | PayOS settings are valid (clientId, apiKey, checksumKey); payment payload inputs are valid.                                        |
| 83  | Cancel PayOS payment                | PayOSService                    | CancelPaymentAsync                    | F083                    | CancelPaymentAsync                    | Validate 'Cancel PayOS payment' in PayOSService.CancelPaymentAsync, covering success/failure flow, response contract, and logging behavior.                                       | PayOS settings are valid (clientId, apiKey, checksumKey); payment payload inputs are valid.                                        |
| 84  | Save file to Supabase               | SupabaseStorageService          | SaveFileAsync                         | F084                    | SaveFileAsync                         | Validate 'Save file to Supabase' in SupabaseStorageService.SaveFileAsync, covering success/failure flow, response contract, and logging behavior.                                 | Supabase storage settings are valid; stream/path inputs are valid; network calls can be mocked.                                    |
| 85  | Delete file from Supabase           | SupabaseStorageService          | DeleteFile                            | F085                    | DeleteFile                            | Validate 'Delete file from Supabase' in SupabaseStorageService.DeleteFile, covering success/failure flow, response contract, and logging behavior.                                | Supabase storage settings are valid; stream/path inputs are valid; network calls can be mocked.                                    |
| 86  | Check Supabase file exists          | SupabaseStorageService          | FileExists                            | F086                    | FileExists                            | Validate 'Check Supabase file exists' in SupabaseStorageService.FileExists, covering success/failure flow, response contract, and logging behavior.                               | Supabase storage settings are valid; stream/path inputs are valid; network calls can be mocked.                                    |
| 87  | Get setting by key                  | SystemSettingService            | GetSettingAsync                       | F087                    | GetSettingAsync                       | Validate 'Get setting by key' in SystemSettingService.GetSettingAsync, covering success/failure flow, response contract, and logging behavior.                                    | System-setting repository and UnitOfWork are seeded with valid key/value data.                                                     |
| 88  | Get all settings                    | SystemSettingService            | GetAllSettingsAsync                   | F088                    | GetAllSettingsAsync                   | Validate 'Get all settings' in SystemSettingService.GetAllSettingsAsync, covering success/failure flow, response contract, and logging behavior.                                  | System-setting repository and UnitOfWork are seeded with valid key/value data.                                                     |
| 89  | Update settings                     | SystemSettingService            | UpdateSettingsAsync                   | F089                    | UpdateSettingsAsync                   | Validate 'Update settings' in SystemSettingService.UpdateSettingsAsync, covering success/failure flow, response contract, and logging behavior.                                   | System-setting repository and UnitOfWork are seeded with valid key/value data.                                                     |
| 90  | Process payment completion          | ClinicVisitService              | ProcessPaymentCompletionAsync         | F090                    | ProcessPaymentCompletionAsync         | Validate 'Process payment completion' in ClinicVisitService.ProcessPaymentCompletionAsync, covering appointment completion and session creation.                                  | Repositories and UnitOfWork are mocked; order and payment data are valid.                                                          |
| 91  | Create PayOS payout                 | PayOSPayoutService              | CreatePayoutAsync                     | F091                    | CreatePayoutAsync                     | Validate 'Create PayOS payout' in PayOSPayoutService.CreatePayoutAsync, covering request validation, signature generation, and API call.                                          | PayOS settings and HTTP client are prepared; request payload is valid.                                                             |
| 92  | Get PayOS payout details            | PayOSPayoutService              | GetPayoutAsync                        | F092                    | GetPayoutAsync                        | Validate 'Get PayOS payout details' in PayOSPayoutService.GetPayoutAsync, covering response mapping and error handling.                                                           | PayOS settings and HTTP client are prepared; payout ID is valid.                                                                   |
| 93  | Get all PayOS payouts               | PayOSPayoutService              | GetPayoutsAsync                       | F093                    | GetPayoutsAsync                       | Validate 'Get all PayOS payouts' in PayOSPayoutService.GetPayoutsAsync, covering pagination and list mapping.                                                                     | PayOS settings and HTTP client are prepared.                                                                                       |
| 94  | Estimate payout credit              | PayOSPayoutService              | EstimateCreditAsync                   | F094                    | EstimateCreditAsync                   | Validate 'Estimate payout credit' in PayOSPayoutService.EstimateCreditAsync, covering calculation logic.                                                                          | PayOS settings and HTTP client are prepared; amount is valid.                                                                      |
| 95  | Get payout account balance          | PayOSPayoutService              | GetPayoutAccountBalanceAsync          | F095                    | GetPayoutAccountBalanceAsync          | Validate 'Get payout account balance' in PayOSPayoutService.GetPayoutAccountBalanceAsync, covering response mapping.                                                               | PayOS settings and HTTP client are prepared.                                                                                       |
| 96  | Generate patient screening PDF      | PatientScreeningPdfService      | GenerateScreeningReportPdf            | F096                    | GenerateScreeningReportPdf            | Validate 'Generate patient screening PDF' in PatientScreeningPdfService.GenerateScreeningReportPdf, covering PDF layout and data population.                                      | QuestPDF library is initialized; screening data is valid.                                                                          |

## Tong hop nhanh

- Tong function checklist (Infrastructure scope, services only): **96** — ma **F001** den **F096** (danh so lien tuc, trung voi cot **No** 1-96 trong bang catalog).
- So class test trong `tests/Infrastructure.UnitTests`: **16** file `*Tests.cs` (gom theo service, khong phai 1 file / 1 function).
- Tong test case theo matrix sheet (bang "So test case" duoi day + `UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_SHEET_LAYOUT.md`): **432**.
- So test method xUnit (`[Fact]` + `[Theory]`) trong project hien tai: **249** (co the gom nhieu data row trong `[Theory]`, khong bang 484 slot tren sheet neu tach tung UTCID).

## Ghi chu map de viet lai test theo cum

- Identity/Auth cluster: ApplicationUser, AuthService, IdentityService, RefreshToken, RefreshTokenService, TokenService.
- Core infrastructure services: AdminQueryService, AiQuotaService, DashboardMetricsService, EmailService, NotificationService, SystemSettingService.
- Integrations: PayOSService, SupabaseStorageService, GoogleMeetService.
- Background jobs/workers: (loai khoi scope services only).

## Chi tiet test case theo function (thuc te - tap trung service/function)

Ghi chu:
- **Chuan dem so test case / trace matrix:** dung bang "Thong ke day du theo yeu cau" (cot `So test case`) va `UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_SHEET_LAYOUT.md` (hang `Total Test Cases` tren moi function). Tong **432** slot UTCID; **96** function.
- Doan bullet `### F001` … `### F014` ngay duoi day la **tom tat luong chinh** (it hon so dong tren sheet). Vi du F001 o day liet ke 5 y chinh; sheet/matrix va bang thong ke dung **10** case. Khong lay doan tom tat lam so lieu tong hop.
- Moi function duoi day co nhieu case theo luong thanh cong + luong loi nghiep vu + luong exception.
- Dinh dang: `UTCIDxx - Condition -> Expected`.
- **Return / exception / log message:** O day expected outcome gop vao cuoi moi bullet (`-> ...`). File `UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_SHEET_LAYOUT.md` co them 3 cot trong **Result Matrix**: `Expected return`, `Expected exception`, `Expected log message` (dien tay cho tung UTCID, map voi vung **Confirm** tren Excel: Excel dung hang Return/Exception/Log x cot UTCID; markdown dung 1 hang = 1 UTCID). Trong code `tests/Infrastructure.UnitTests`, ket qua kiem tra nam trong **assert/Verify** (khong tu dong sync tu file MD).
- Co the tach moi function thanh 1 sheet rieng giong mau `LoginAsync`.

### F001 - AuthService.RegisterPatientAsync
- UTCID01 - Email da ton tai -> Failure "A user with this email already exists".
- UTCID02 - Tao user that bai (Identity errors) -> Rollback transaction + tra danh sach loi.
- UTCID03 - Tao user thanh cong + add role + tao Patient profile -> Commit + Success response co UserId/Email.
- UTCID04 - Gui email confirm bi loi -> Van Success, ghi warning log.
- UTCID05 - Exception khi dang transaction -> Rollback + Failure "An error occurred during registration".

### F002 - AuthService.LookupAccountByCitizenIdAsync
- UTCID01 - citizenId ton tai -> Success + Masked Email (e.g. r***@gmail.com).
- UTCID02 - citizenId khong ton tai -> Failure "Account not found".
- UTCID03 - Validation citizenId failed -> Failure + Validation errors.

### F003 - AuthService.GoogleLoginAsync
- UTCID01 - Google token invalid (InvalidJwtException) -> Unauthorized "Invalid Google token".
- UTCID02 - Payload khong co email -> Failure.
- UTCID03 - User ton tai nhung IsDeleted -> Unauthorized.
- UTCID04 - User ton tai nhung IsActive=false -> Unauthorized.
- UTCID05 - User ton tai, chua confirm email -> auto confirm + update user.
- UTCID06 - User ton tai, bat 2FA -> Success(TwoFactorRequired).
- UTCID07 - User moi, create + role Patient + profile -> Commit + login Success.
- UTCID08 - Loi tao user/DB -> Rollback + Failure "An error occurred during Google login".

### F004 - AuthService.LoginAsync
- UTCID01 - Email khong ton tai -> Unauthorized "Invalid email or password".
- UTCID02 - User bi soft delete -> Unauthorized "Invalid email or password".
- UTCID03 - User inactive -> Unauthorized deactivated message.
- UTCID04 - User bi lockout -> Unauthorized lockout message.
- UTCID05 - CheckPasswordSignInAsync tra IsLockedOut -> Unauthorized lockout message.
- UTCID06 - IsNotAllowed (email chua confirm) -> Unauthorized.
- UTCID07 - Sai password -> Unauthorized invalid credentials.
- UTCID08 - Role ClinicStaff va VerificationStatus=Rejected -> Unauthorized bi tu choi xac minh.
- UTCID09 - RequiresTwoFactor=true -> Success(TwoFactorRequired).
- UTCID10 - 2FA enabled tai user manager -> Success(TwoFactorRequired).
- UTCID11 - Dang nhap thanh cong -> Success(AuthResponse day du access/refresh token).
- UTCID12 - Exception bat ky -> Failure "An error occurred during login".

### F005 - AuthService.VerifyTwoFactorLoginAsync
- UTCID01 - User khong ton tai/da xoa/inactive -> Unauthorized.
- UTCID02 - User chua bat 2FA -> Failure.
- UTCID03 - Verify code thuong sai -> Unauthorized.
- UTCID04 - Verify recovery code sai -> Unauthorized.
- UTCID05 - Verify recovery code dung -> Success va co warning log recovery code.
- UTCID06 - Verify authenticator code dung -> Success AuthResponse.
- UTCID07 - Exception -> Failure "An error occurred during verification".

### F006 - AuthService.RefreshTokenAsync
- UTCID01 - Access token khong parse duoc userId/jti -> Unauthorized invalid access token.
- UTCID02 - Refresh token hash khong tim thay -> Unauthorized invalid refresh token.
- UTCID03 - Refresh token khong active -> revoke token family + Unauthorized.
- UTCID04 - JwtId/UserId mismatch -> Unauthorized token mismatch.
- UTCID05 - User khong ton tai hoac inactive/deleted -> Unauthorized.
- UTCID06 - Refresh thanh cong -> rotate token + tra access/refresh moi.
- UTCID07 - Exception -> Failure "An error occurred while refreshing token".

### F007 - AuthService.LogoutAsync
- UTCID01 - Refresh token tim thay -> RevokeTokenAsync duoc goi + Success.
- UTCID02 - Refresh token khong tim thay -> van Success (idempotent logout).
- UTCID03 - Exception -> Failure "An error occurred during logout".

### F008 - AuthService.LogoutAllAsync
- UTCID01 - RevokeAllUserTokensAsync thanh cong -> Success.
- UTCID02 - RevokeAllUserTokensAsync throw -> Failure "An error occurred during logout".

### F009 - AuthService.ConfirmEmailAsync
- UTCID01 - userId khong parse duoc Guid -> Failure "Invalid user ID".
- UTCID02 - user khong ton tai -> NotFound.
- UTCID03 - ConfirmEmailAsync tra fail errors -> Failure errors.
- UTCID04 - User role ClinicStaff -> gui notification den tat ca admin.
- UTCID05 - Confirm thanh cong -> Success.
- UTCID06 - Exception -> Failure.

### F010 - AuthService.ForgotPasswordAsync
- UTCID01 - Email ton tai -> tao reset token + gui mail reset + Success.
- UTCID02 - Email khong ton tai -> khong gui mail, van Success (anti-enumeration).
- UTCID03 - Exception khi generate/send -> Failure.

### F011 - AuthService.ResetPasswordAsync
- UTCID01 - request.UserId khong hop le -> Failure "Invalid user ID".
- UTCID02 - User khong ton tai -> NotFound.
- UTCID03 - Identity reset tra fail -> Failure errors.
- UTCID04 - Reset thanh cong -> revoke all refresh token + Success.
- UTCID05 - Exception -> Failure.

### F012 - AuthService.GetCurrentUserAsync
- UTCID01 - User khong ton tai -> Unauthorized.
- UTCID02 - User Patient -> map RoleId tu Patient profile.
- UTCID03 - User ClinicStaff -> map RoleId + StaffSubRole + Permissions.
- UTCID04 - Lay thong tin thanh cong -> Success(UserInfoResponse day du).
- UTCID05 - Exception -> Failure.

### F013 - AuthService.ResendConfirmationAsync
- UTCID01 - User ton tai va chua confirm -> tao token + gui mail.
- UTCID02 - User da confirm -> bo qua gui mail, van Success.
- UTCID03 - User khong ton tai -> van Success.
- UTCID04 - Exception -> Failure.

### F015-F038 - IdentityService (theo nhom)
- F015 CheckPasswordAsync: user null -> false; password sai -> false; dung -> true.
- F016 GetUserByEmailAsync: tim thay user khong deleted -> dto; user deleted/khong co -> null.
- F017 GetUserByIdAsync: tim thay -> dto; deleted/khong co -> null.
- F018 IsPhoneNumberInUseByOrganizationAsync: phone rong -> false; format khac nhau cung so -> true; khac org -> false.
- F019 IsEmailConfirmedAsync: user null -> false; confirmed false/true -> dung theo state.
- F020 IsUserActiveAsync: null -> false; IsActive true + !IsDeleted -> true.
- F021 GenerateEmailConfirmationTokenAsync: user null -> throw InvalidOperationException; user hop le -> token non-empty.
- F022 GeneratePasswordResetTokenAsync: user null -> throw; user hop le -> token non-empty.
- F023 GetUserRolesAsync: user null -> empty list; user co role -> dung list.
- F024 IsInRoleAsync: user null -> false; co role -> true; khong co role -> false.
- F025 GetUserIdsByRoleAndOrganizationAsync: loc dung theo role + org + active + !deleted.
### F014-F035 - IdentityService (theo nhom)
- F014 CheckPasswordAsync: user null -> false; password sai -> false; dung -> true.
- F015 GetUserByEmailAsync: tim thay user khong deleted -> dto; user deleted/khong co -> null.
- F016 GetUserByIdAsync: tim thay -> dto; deleted/khong co -> null.
- F017 IsEmailConfirmedAsync: user null -> false; confirmed false/true -> dung theo state.
- F018 IsUserActiveAsync: null -> false; IsActive true + !IsDeleted -> true.
- F019 GenerateEmailConfirmationTokenAsync: user null -> throw InvalidOperationException; user hop le -> token non-empty.
- F020 GeneratePasswordResetTokenAsync: user null -> throw; user hop le -> token non-empty.
- F021 GetUserRolesAsync: user null -> empty list; user co role -> dung list.
- F022 IsInRoleAsync: user null -> false; co role -> true; khong co role -> false.
- F023 UpdateLastLoginAsync: user null -> no-op; user ton tai -> LastLoginAt thay doi + UpdateAsync duoc goi.
- F024 IsTwoFactorEnabledAsync: user null -> false; enabled true/false theo user manager.
- F025 GetAuthenticatorKeyAsync: user null -> null; user ton tai -> key.
- F026 GetOrCreateAuthenticatorKeyAsync: user null -> throw; reset key + lay key thanh cong.
- F027 VerifyTwoFactorCodeAsync: user null -> false; code sai/dung -> false/true.
- F028 GenerateNewRecoveryCodesAsync: user null -> throw; user hop le -> mang recovery codes dung so luong.
- F029 GetRecoveryCodesCountAsync: user null -> 0; user ton tai -> count dung.
- F030 GenerateAuthenticatorUri: output dung format `otpauth://totp/...`.
- F031 FormatAuthenticatorKey: key dai -> tach moi 4 ky tu, uppercase.
- F032 GetUserMetricsAsync: tinh total/change/pending approvals dung theo seed data.
- F033 GetUsersInRoleCountAsync: activeOnly=true chi dem active+not deleted; false dem tat ca not deleted.
- F034 GetPendingApprovalsCountAsync: dem dung users `!EmailConfirmed || !IsActive`.
- F035 GetUserDetailsAsync: user null/deleted -> null; user hop le -> dto day du fields profile.

### F036-F042 - RefreshTokenService
- F036 CreateRefreshTokenAsync: luu token vao DB + return Id.
- F037 GetByTokenHashAsync: hash khong ton tai -> null; ton tai -> map dto dung IsActive/Revoked/Used.
- F038 RotateRefreshTokenAsync: old token khong ton tai -> throw; ton tai -> tao token moi + mark old used.
- F039 RevokeTokenAsync: token ton tai -> revoked + save; khong ton tai -> no-op.
- F040 RevokeAllUserTokensAsync: revoke tat ca token chua revoke cua user.
- F041 RevokeTokenFamilyAsync: token null -> no-op; token ton tai -> goi revoke all user tokens.
- F042 CleanupExpiredTokensAsync: xoa token het han/da revoke qua cutoff va return so ban ghi da xoa.

### F043-F048 - TokenService
- F043 GenerateAccessTokenAsync: token co sub/uid/email/jti/role claims va expiresAt dung config.
- F044 GenerateRefreshToken: sinh chuoi random base64, do dai/entropy cao, 2 lan sinh khac nhau.
- F045 ValidateToken: token hop le -> ClaimsPrincipal; token sai signature/alg -> null.
- F046 GetUserIdFromToken: co claim sub/uid/nameidentifier hop le -> Guid; claim loi -> null.
- F047 GetJtiFromToken: token hop le co jti -> lay duoc; token invalid -> null.
- F048 HashToken: cung input -> cung hash; khac input -> khac hash; output base64 khong rong.

### F049-F051 - AdminQueryService
- F049 GetOphthalmologistsAsync: loc searchTerm ILike; loc nhieu verificationStatus; paging + sort CreatedAt desc; map licenses/degrees dung.
- F050 GetPatientsAsync: loc search/status(active|pending|suspended); paging dung totalCount.
- F051 GetAuditLogsAsync: loc theo search/action/entity/user/date-range; paging + map UserName null-safe.

### F052-F055 - AiQuotaService
- F052 GetQuotaAsync: role Patient -> tinh free/purchased/remaining dung; role ClinicStaff -> quota theo clinic; role la -> None.
- F053 HasAvailableQuotaAsync: remaining > 0 -> true; =0 -> false.
- F054 DeductQuotaAsync: patient/clinic ton tai -> consume quota + SaveChanges; khong ton tai -> throw InvalidOperationException.
- F055 AddPurchasedQuotaAsync: patient/clinic ton tai -> tang purchased + SaveChanges; khong ton tai -> throw.

### F056-F060 - BetterStackHeartbeatService
- F056 GetEmbedUrl: url rong/whitespace -> null; co gia tri -> tra url.
- F057 GetMonitorDescriptors: tra du so monitor enum, co key/name/category/configured dung.
- F058 NotifyStartedAsync: endpoint khong config -> skip khong throw; config day du -> POST start/ping.
- F059 NotifySucceededAsync: POST ping url; non-success status -> warning log.
- F060 NotifyFailedAsync: POST fail url (fallback ping); HTTP exception -> warning log khong throw.

- UTCID01 - Co patient/org UsedAiQuota>0 -> reset ve 0, notify started+succeeded.
- UTCID02 - Khong co du lieu can reset -> van completed, count=0.
- UTCID03 - Exception DB -> notify failed + rethrow.

### F061-F067 - DashboardMetricsService
- F061 GetSystemAdminMetricsAsync: tinh growth doctor/clinic/patient, revenue breakdown, pending actions, top performers dung theo seed.
- F062 GetRecentScreeningsAsync: map ScreeningCode, status Completed/Analyzing, risk level + isCritical, paging dung.
- F063 GetScreeningVolumeTrendsAsync: timeRange weekly va monthly deu tra data points + average dung.
- F064 GetPopulationRiskAnalysisAsync: bo RiskLevel.None, tinh percentage dung tong results.
- F065 GetSystemHealthAsync: tra fixed healthy payload voi 3 components.
- F066 GetOphthalmologistMetricsAsync: khong tim thay doctor -> dto rong; tim thay -> pending/urgent/completed/open slots dung.
- F067 GetPatientMetricsAsync: patient null -> dto rong; patient ton tai -> completed screenings/total reports/upcoming/quota dung.

### F068-F069 - DateTimeService
- F068 Now property: tra local time gan voi DateTime.Now.
- F069 UtcNow property: tra UTC time gan voi DateTime.UtcNow.

### F070-F073 - EmailService
- F070 SendEmailConfirmationAsync: tao dung subject/body template + goi SendAsync.
- F071 SendPasswordResetAsync: tao dung template reset + goi SendAsync.
- F072 SendWelcomeEmailAsync: tao dung template welcome + goi SendAsync.
- F073 SendAsync: validate tham so rong -> throw ArgumentException; gui SMTP thanh cong -> debug log; SMTP fail -> log error + rethrow.

### F074-F076 - GoogleMeetService
- F074 CreateMeetingAsync: tao event thanh cong co meet link ngay lan dau -> return MeetingInfo.
  - (sub) Meet link chua co ngay -> retry exponential backoff, lay duoc link -> success.
  - (sub) Retry het van khong co link -> cleanup orphan event + throw InvalidOperationException.
- F075 DeleteMeetingAsync: xoa thanh cong -> info log; event not found -> warning khong throw.
- F076 Dispose: dispose calendar service khong nem loi.

### F077-F078 - NotificationService
- F077 SendAsync(typed): payload object -> serialize camelCase; persist notification + broadcast dto + broadcast unread count.
  - (sub) Payload co consultationId/sessionId/... guid -> ExtractReferenceId map dung.
  - (sub) Hub/repository throw -> log error + rethrow.
- F078 SendAsync(legacy): goi overload typed voi title/type mac dinh.

### F079 - PatientRoadmapGenerationService.GenerateFromDiagnosisAsync
- UTCID01 - PatientId/ScreeningId/AiScreeningRawJson thieu -> Failure validation message.
- UTCID02 - ApiKey chua config -> Failure.
- UTCID03 - Google AI tra JSON hop le -> Success roadmap (risk normalized, list normalized).
- UTCID04 - AI tra JSON sai format nhieu lan -> Failure sau retries.
- UTCID05 - HTTP timeout/network error va het retry -> Failure "Unable to generate...".
- UTCID06 - follow_up.needed=true nhung timeframe rong -> invalid format, retry/failure.

### F080-F083 - PayOSService
- F080 CreatePaymentLinkAsync: dung default return/cancel url neu input rong; append orderCode; truncate description >25; response null checkoutUrl -> throw.
- F081 GetPaymentStatusAsync: paymentInfo null -> throw; co du lieu -> map status/amount/txnRef.
- F082 VerifyWebhookSignatureAsync: hien tai return true (happy path), exception branch return false.
- F083 CancelPaymentAsync: cancel tra object -> true; tra null -> false; exception -> false.

- UTCID01 - Slot Available + BookedCount=0 + qua gio -> chuyen Expired + UpdatedAt.
- UTCID02 - Slot da book hoac chua qua gio -> khong bi expire.
- UTCID03 - Thanh cong -> notify started+succeeded; exception -> notify failed + rethrow.

### F084-F086 - SupabaseStorageService
- F084 SaveFileAsync: filename duoc sanitize; subfolder ghep path dung; stream rong -> throw; upload thanh cong -> tra public url (co fallback absolute url).
- F085 DeleteFile: relativePath/url hop le -> Remove thanh cong tra true; parse path fail hoac exception -> false.
- F086 FileExists: HEAD success -> true; status fail/exception -> false; path rong -> false.

### F087-F089 - SystemSettingService
- F087 GetSettingAsync: key ton tai -> tra value; khong ton tai -> null.
- F088 GetAllSettingsAsync: tra dictionary day du key/value tu DB.
- F089 UpdateSettingsAsync: dictionary rong/null -> no-op; key ton tai -> UpdateValue; key moi -> add moi; save change + info log.

### F090 - ClinicVisitService
- F090 ProcessPaymentCompletionAsync: validate order -> complete appointment + generate results + notify staff.

### F091-F095 - PayOSPayoutService
- F091 CreatePayoutAsync: check balance -> create disbursement request -> return payout data.
- F092 GetPayoutAsync: query payout status tu PayOS API.
- F093 GetPayoutsAsync: fetch list payouts co pagination.
- F094 EstimateCreditAsync: tinh toan phi va credit thuc nhan.
- F095 GetPayoutAccountBalanceAsync: lay so du tai khoan thanh toan.

### F096 - PatientScreeningPdfService
- F096 GenerateScreeningReportPdf: map screening data -> render PDF bytes (QuestPDF).

## Thong ke day du theo yeu cau (service -> function -> so test case)

- Tong scope da viet: services only.
- Tong test case de xuat: cap nhat theo services only (khong tinh entity/job).

| Function Code | Class/Service | Function | So test case |
|---|---|---|---|
| F001 | AuthService | RegisterPatientAsync | 10 |
| F002 | AuthService | LookupAccountByCitizenIdAsync | 3 |
| F003 | AuthService | GoogleLoginAsync | 10 |
| F004 | AuthService | LoginAsync | 12 |
| F005 | AuthService | VerifyTwoFactorLoginAsync | 10 |
| F006 | AuthService | RefreshTokenAsync | 10 |
| F007 | AuthService | LogoutAsync | 3 |
| F008 | AuthService | LogoutAllAsync | 2 |
| F009 | AuthService | ConfirmEmailAsync | 6 |
| F010 | AuthService | ForgotPasswordAsync | 3 |
| F011 | AuthService | ResetPasswordAsync | 5 |
| F012 | AuthService | GetCurrentUserAsync | 5 |
| F013 | AuthService | ResendConfirmationAsync | 4 |
| F014 | IdentityService | CheckPasswordAsync | 3 |
| F015 | IdentityService | GetUserByEmailAsync | 3 |
| F016 | IdentityService | GetUserByIdAsync | 3 |
| F017 | IdentityService | IsEmailConfirmedAsync | 3 |
| F018 | IdentityService | IsUserActiveAsync | 3 |
| F019 | IdentityService | GenerateEmailConfirmationTokenAsync | 2 |
| F020 | IdentityService | GeneratePasswordResetTokenAsync | 2 |
| F021 | IdentityService | GetUserRolesAsync | 3 |
| F022 | IdentityService | IsInRoleAsync | 3 |
| F023 | IdentityService | UpdateLastLoginAsync | 2 |
| F024 | IdentityService | IsTwoFactorEnabledAsync | 3 |
| F025 | IdentityService | GetAuthenticatorKeyAsync | 2 |
| F026 | IdentityService | GetOrCreateAuthenticatorKeyAsync | 2 |
| F027 | IdentityService | VerifyTwoFactorCodeAsync | 3 |
| F028 | IdentityService | GenerateNewRecoveryCodesAsync | 2 |
| F029 | IdentityService | GetRecoveryCodesCountAsync | 2 |
| F030 | IdentityService | GenerateAuthenticatorUri | 2 |
| F031 | IdentityService | FormatAuthenticatorKey | 2 |
| F032 | IdentityService | GetUserMetricsAsync | 3 |
| F033 | IdentityService | GetUsersInRoleCountAsync | 3 |
| F034 | IdentityService | GetPendingApprovalsCountAsync | 2 |
| F035 | IdentityService | GetUserDetailsAsync | 3 |
| F036 | RefreshTokenService | CreateRefreshTokenAsync | 3 |
| F037 | RefreshTokenService | GetByTokenHashAsync | 3 |
| F038 | RefreshTokenService | RotateRefreshTokenAsync | 3 |
| F039 | RefreshTokenService | RevokeTokenAsync | 2 |
| F040 | RefreshTokenService | RevokeAllUserTokensAsync | 2 |
| F041 | RefreshTokenService | RevokeTokenFamilyAsync | 2 |
| F042 | RefreshTokenService | CleanupExpiredTokensAsync | 3 |
| F043 | TokenService | GenerateAccessTokenAsync | 4 |
| F044 | TokenService | GenerateRefreshToken | 3 |
| F045 | TokenService | ValidateToken | 3 |
| F046 | TokenService | GetUserIdFromToken | 3 |
| F047 | TokenService | GetJtiFromToken | 2 |
| F048 | TokenService | HashToken | 3 |
| F049 | AdminQueryService | GetOphthalmologistsAsync | 5 |
| F050 | AdminQueryService | GetPatientsAsync | 4 |
| F051 | AdminQueryService | GetAuditLogsAsync | 5 |
| F052 | AiQuotaService | GetQuotaAsync | 5 |
| F053 | AiQuotaService | HasAvailableQuotaAsync | 2 |
| F054 | AiQuotaService | DeductQuotaAsync | 4 |
| F055 | AiQuotaService | AddPurchasedQuotaAsync | 4 |
| F056 | BetterStackHeartbeatService | GetEmbedUrl | 2 |
| F057 | BetterStackHeartbeatService | GetMonitorDescriptors | 2 |
| F058 | BetterStackHeartbeatService | NotifyStartedAsync | 3 |
| F059 | BetterStackHeartbeatService | NotifySucceededAsync | 2 |
| F060 | BetterStackHeartbeatService | NotifyFailedAsync | 2 |
| F061 | DashboardMetricsService | GetSystemAdminMetricsAsync | 6 |
| F062 | DashboardMetricsService | GetRecentScreeningsAsync | 4 |
| F063 | DashboardMetricsService | GetScreeningVolumeTrendsAsync | 3 |
| F064 | DashboardMetricsService | GetPopulationRiskAnalysisAsync | 3 |
| F065 | DashboardMetricsService | GetSystemHealthAsync | 2 |
| F066 | DashboardMetricsService | GetOphthalmologistMetricsAsync | 4 |
| F067 | DashboardMetricsService | GetPatientMetricsAsync | 3 |
| F068 | DateTimeService | Now (property) | 2 |
| F069 | DateTimeService | UtcNow (property) | 2 |
| F070 | EmailService | SendEmailConfirmationAsync | 2 |
| F071 | EmailService | SendPasswordResetAsync | 2 |
| F072 | EmailService | SendWelcomeEmailAsync | 2 |
| F073 | EmailService | SendAsync | 4 |
| F074 | GoogleMeetService | CreateMeetingAsync | 5 |
| F075 | GoogleMeetService | DeleteMeetingAsync | 2 |
| F076 | GoogleMeetService | Dispose | 1 |
| F077 | NotificationService | SendAsync (typed) | 4 |
| F078 | NotificationService | SendAsync (legacy) | 1 |
| F079 | PatientRoadmapGenerationService | GenerateFromDiagnosisAsync | 6 |
| F080 | PayOSService | CreatePaymentLinkAsync | 5 |
| F081 | PayOSService | GetPaymentStatusAsync | 3 |
| F082 | PayOSService | VerifyWebhookSignatureAsync | 2 |
| F083 | PayOSService | CancelPaymentAsync | 3 |
| F084 | SupabaseStorageService | SaveFileAsync | 5 |
| F085 | SupabaseStorageService | DeleteFile | 3 |
| F086 | SupabaseStorageService | FileExists | 3 |
| F087 | SystemSettingService | GetSettingAsync | 2 |
| F088 | SystemSettingService | GetAllSettingsAsync | 2 |
| F089 | SystemSettingService | UpdateSettingsAsync | 4 |
| F090 | ClinicVisitService | ProcessPaymentCompletionAsync | 4 |
| F091 | PayOSPayoutService | CreatePayoutAsync | 3 |
| F092 | PayOSPayoutService | GetPayoutAsync | 3 |
| F093 | PayOSPayoutService | GetPayoutsAsync | 3 |
| F094 | PayOSPayoutService | EstimateCreditAsync | 3 |
| F095 | PayOSPayoutService | GetPayoutAccountBalanceAsync | 3 |
| F096 | PatientScreeningPdfService | GenerateScreeningReportPdf | 3 |

## Tong ket theo Service

| Service/Class | So function | Tong test case |
|---|---:|---:|
| AuthService | 13 | 87 |
| IdentityService | 22 | 58 |
| RefreshTokenService | 7 | 18 |
| TokenService | 6 | 18 |
| AdminQueryService | 3 | 14 |
| AiQuotaService | 4 | 15 |
| BetterStackHeartbeatService | 5 | 11 |
| DashboardMetricsService | 7 | 25 |
| DateTimeService | 2 | 4 |
| EmailService | 4 | 10 |
| GoogleMeetService | 3 | 8 |
| NotificationService | 2 | 5 |
| PatientRoadmapGenerationService | 1 | 6 |
| PayOSService | 4 | 13 |
| SupabaseStorageService | 3 | 11 |
| SystemSettingService | 3 | 8 |
| ClinicVisitService | 1 | 4 |
| PayOSPayoutService | 5 | 15 |
| PatientScreeningPdfService | 1 | 3 |

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

### F001 - AuthService.RegisterPatientAsync (5 case)
- UTCID01 (A): Email da ton tai -> Failure conflict message.
- UTCID02 (A): `_userManager.CreateAsync` fail -> rollback transaction + tra identity errors.
- UTCID03 (N): Tao user + add role + tao patient + commit -> Success co UserId/Email.
- UTCID04 (A): Gui email confirm fail -> van Success, co warning log.
- UTCID05 (A): Exception trong transaction -> rollback + Failure generic.

### F002 - AuthService.LookupAccountByCitizenIdAsync (3 case)
- UTCID01 (N): citizenId hop le va co account -> Success + Masked Email.
- UTCID02 (A): citizenId hop le nhung khong co account -> Failure "Account not found".
- UTCID03 (A): citizenId sai format -> Failure + Validation errors.

### F003 - AuthService.GoogleLoginAsync (10 case)
- UTCID01 (A): Google credential invalid jwt -> Unauthorized.
- UTCID02 (A): Payload khong co email -> Failure.
- UTCID03 (A): User ton tai nhung deleted -> Unauthorized.
- UTCID04 (A): User ton tai nhung inactive -> Unauthorized.
- UTCID05 (N): User ton tai, email chua confirm -> auto confirm + update user.
- UTCID06 (N): User ton tai va 2FA bat -> Success two-factor required.
- UTCID07 (N): User moi -> tao account patient + commit + login success.
- UTCID08 (A): Loi DB/identity bat ky -> rollback + Failure generic.
- UTCID09 (B): Token Google het han -> Unauthorized.
- UTCID10 (A): Khong the lien ket Google account voi user ton tai cung email (security policy) -> Failure.

### F004 - AuthService.LoginAsync (12 case)
- UTCID01 (A): Email khong ton tai -> Unauthorized invalid credentials.
- UTCID02 (A): User deleted -> Unauthorized invalid credentials.
- UTCID03 (A): User inactive -> Unauthorized deactivated.
- UTCID04 (A): User dang lockout -> Unauthorized lockout message.
- UTCID05 (A): Password sai dan den `IsLockedOut` -> Unauthorized lockout.
- UTCID06 (A): `IsNotAllowed=true` (email chua confirm) -> Unauthorized.
- UTCID07 (A): Password sai thong thuong -> Unauthorized invalid credentials.
- UTCID08 (A): ClinicStaff co `VerificationStatus=Rejected` -> Unauthorized reject message.
- UTCID09 (N): `RequiresTwoFactor=true` -> Success two-factor required payload.
- UTCID10 (N): User manager 2FA enabled du `RequiresTwoFactor=false` -> van two-factor required.
- UTCID11 (N): Login thanh cong -> tra AuthResponse day du token + user info.
- UTCID12 (A): Exception runtime -> Failure generic login error.

### F005 - AuthService.VerifyTwoFactorLoginAsync (10 case)
- UTCID01 (A): User khong ton tai/deleted/inactive -> Unauthorized.
- UTCID02 (A): User chua bat 2FA -> Failure.
- UTCID03 (A): Verify authenticator code sai -> Unauthorized.
- UTCID04 (A): Verify recovery code sai -> Unauthorized.
- UTCID05 (N): Recovery code dung -> Success + warning log dung recovery code.
- UTCID06 (N): Authenticator code dung -> Success AuthResponse.
- UTCID07 (A): Exception trong verify -> Failure generic.
- UTCID08 (B): Code dung nhung het han (time window) -> Unauthorized.
- UTCID09 (B): Code rong -> Failure.
- UTCID10 (N): 2FA thanh cong set user info vao response dung.

### F006 - AuthService.RefreshTokenAsync (10 case)
- UTCID01 (A): Access token khong lay duoc userId/jti -> Unauthorized invalid access token.
- UTCID02 (A): Refresh token hash khong tim thay -> Unauthorized invalid refresh token.
- UTCID03 (A): Stored token khong active -> revoke token family + Unauthorized.
- UTCID04 (A): JwtId/UserId khong khop -> Unauthorized token mismatch.
- UTCID05 (A): User khong ton tai/inactive/deleted -> Unauthorized.
- UTCID06 (N): Refresh hop le -> rotate token + tra access/refresh moi.
- UTCID07 (A): Exception he thong -> Failure generic refresh error.
- UTCID08 (A): Refresh token da used (reuse attack) -> revoke family + Unauthorized.
- UTCID09 (A): Refresh token da expired -> Unauthorized.
- UTCID10 (N): Family revocation works across all devices of user.

- UTCID01 (A): Access token khong lay duoc userId/jti -> Unauthorized invalid access token.
- UTCID02 (A): Refresh token hash khong ton tai -> Unauthorized invalid refresh token.
- UTCID03 (A): Stored token khong active -> revoke token family + Unauthorized.
- UTCID04 (A): JwtId/UserId khong khop -> Unauthorized token mismatch.
- UTCID05 (A): User khong ton tai/inactive/deleted -> Unauthorized.
- UTCID06 (N): Refresh hop le -> rotate token + tra access/refresh moi.
- UTCID07 (A): Exception he thong -> Failure generic refresh error.

### F008 - AuthService.LogoutAllAsync (2 case)
- UTCID01 (N): Revoke all token thanh cong -> Success.
- UTCID02 (A): Revoke all token throw -> Failure.

### F009 - AuthService.ConfirmEmailAsync (6 case)
- UTCID01 (A): `userId` khong parse duoc Guid -> Failure invalid id.
- UTCID02 (A): User khong ton tai -> NotFound.
- UTCID03 (A): Confirm email fail (token sai/het han) -> Failure errors.
- UTCID04 (N): User role ClinicStaff -> gui notification den tat ca admin.
- UTCID05 (N): Confirm email thanh cong -> Success.
- UTCID06 (A): Exception runtime -> Failure generic.

### F010 - AuthService.ForgotPasswordAsync (3 case)
- UTCID01 (N): Email ton tai -> tao token + gui reset email + Success.
- UTCID02 (N): Email khong ton tai -> khong gui, van Success (anti enumeration).
- UTCID03 (A): Exception send mail/token -> Failure.

### F011 - AuthService.ResetPasswordAsync (5 case)
- UTCID01 (A): UserId sai format -> Failure invalid id.
- UTCID02 (A): User khong ton tai -> NotFound.
- UTCID03 (A): Reset fail do token/password policy -> Failure errors.
- UTCID04 (N): Reset thanh cong -> revoke all refresh token + Success.
- UTCID05 (A): Exception runtime -> Failure.

### F012 - AuthService.GetCurrentUserAsync (5 case)
- UTCID01 (A): User khong ton tai -> Unauthorized.
- UTCID02 (N): User patient -> map `RoleId` bang patient id.
- UTCID03 (N): User ClinicStaff -> map `RoleId`, `StaffSubRole`, `Permissions`.
- UTCID04 (N): Tra ve `UserInfoResponse` day du profile + roles + 2FA flag.
- UTCID05 (A): Exception khi query details -> Failure.

### F013 - AuthService.ResendConfirmationAsync (4 case)
- UTCID01 (N): User ton tai va chua confirm -> tao token + gui email.
- UTCID02 (N): User da confirm -> bo qua gui email, van Success.
- UTCID03 (N): User khong ton tai -> van Success.
- UTCID04 (A): Exception gui email -> Failure.

### F014 - IdentityService.CheckPasswordAsync (3 case)
- UTCID01 (A): UserId khong ton tai -> false.
- UTCID02 (A): Password sai -> false.
- UTCID03 (N): Password dung -> true.

### F015 - IdentityService.GetUserByEmailAsync (3 case)
- UTCID01 (N): User ton tai va !IsDeleted -> tra `UserDto`.
- UTCID02 (A): User bi IsDeleted -> null.
- UTCID03 (A): Khong tim thay email -> null.

### F016 - IdentityService.GetUserByIdAsync (3 case)
- UTCID01 (N): User ton tai va !IsDeleted -> dto day du.
- UTCID02 (A): User deleted -> null.
- UTCID03 (A): UserId khong ton tai -> null.

### F017 - IdentityService.IsEmailConfirmedAsync (3 case)
- UTCID01 (A): User khong ton tai -> false.
- UTCID02 (N): User ton tai, EmailConfirmed=false -> false.
- UTCID03 (N): User ton tai, EmailConfirmed=true -> true.

### F018 - IdentityService.IsUserActiveAsync (3 case)
- UTCID01 (A): User khong ton tai -> false.
- UTCID02 (A): User inactive hoac deleted -> false.
- UTCID03 (N): User active va !deleted -> true.

### F019 - IdentityService.GenerateEmailConfirmationTokenAsync (2 case)
- UTCID01 (A): User khong ton tai -> throw `InvalidOperationException`.
- UTCID02 (N): User ton tai -> token khong rong.

### F020 - IdentityService.GeneratePasswordResetTokenAsync (2 case)
- UTCID01 (A): User khong ton tai -> throw.
- UTCID02 (N): User ton tai -> token khong rong.

### F021 - IdentityService.GetUserRolesAsync (3 case)
- UTCID01 (A): User khong ton tai -> empty list.
- UTCID02 (N): User 1 role -> dung role do.
- UTCID03 (N): User nhieu role -> tra du role.

### F022 - IdentityService.IsInRoleAsync (3 case)
- UTCID01 (A): User khong ton tai -> false.
- UTCID02 (N): User co role -> true.
- UTCID03 (N): User khong co role -> false.

### F023 - IdentityService.UpdateLastLoginAsync (2 case)
- UTCID01 (A): User khong ton tai -> no-op.
- UTCID02 (N): User ton tai -> LastLoginAt cap nhat + `UpdateAsync` duoc goi.

### F024 - IdentityService.IsTwoFactorEnabledAsync (3 case)
- UTCID01 (A): User khong ton tai -> false.
- UTCID02 (N): User co 2FA=false -> false.
- UTCID03 (N): User co 2FA=true -> true.

### F025 - IdentityService.GetAuthenticatorKeyAsync (2 case)
- UTCID01 (A): User khong ton tai -> null.
- UTCID02 (N): User ton tai -> tra key.

### F026 - IdentityService.GetOrCreateAuthenticatorKeyAsync (2 case)
- UTCID01 (A): User khong ton tai -> throw.
- UTCID02 (N): User ton tai -> reset key va lay key moi thanh cong.

### F027 - IdentityService.VerifyTwoFactorCodeAsync (3 case)
- UTCID01 (A): User khong ton tai -> false.
- UTCID02 (A): Code sai -> false.
- UTCID03 (N): Code dung -> true.

### F028 - IdentityService.GenerateNewRecoveryCodesAsync (2 case)
- UTCID01 (A): User khong ton tai -> throw.
- UTCID02 (N): User ton tai -> tra mang recovery code theo count.

### F029 - IdentityService.GetRecoveryCodesCountAsync (2 case)
- UTCID01 (A): User khong ton tai -> 0.
- UTCID02 (N): User ton tai -> so luong dung.

### F030 - IdentityService.GenerateAuthenticatorUri (2 case)
- UTCID01 (N): Email + key hop le -> URI dung schema `otpauth://totp`.
- UTCID02 (B): Email co ky tu dac biet -> duoc UrlEncode dung.

### F031 - IdentityService.FormatAuthenticatorKey (2 case)
- UTCID01 (N): Key dai -> chia block 4 ky tu + uppercase.
- UTCID02 (B): Key ngan hon 4 -> van uppercase, khong loi.

### F032 - IdentityService.GetUserMetricsAsync (3 case)
- UTCID01 (N): Du lieu co lich su thang truoc -> tinh % change dung.
- UTCID02 (B): Thang truoc =0 nhung co user moi -> change =100.
- UTCID03 (N): Pending approvals dem dung theo dieu kien.

### F033 - IdentityService.GetUsersInRoleCountAsync (3 case)
- UTCID01 (N): `activeOnly=true` -> chi dem active + !deleted.
- UTCID02 (N): `activeOnly=false` -> dem toan bo !deleted.
- UTCID03 (B): Role khong co user -> 0.

### F034 - IdentityService.GetPendingApprovalsCountAsync (2 case)
- UTCID01 (N): Dem dung user `!EmailConfirmed || !IsActive`.
- UTCID02 (B): Khong co pending -> 0.

### F035 - IdentityService.GetUserDetailsAsync (3 case)
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

### F036 - RefreshTokenService.CreateRefreshTokenAsync (3 case)
- UTCID01 (N): Tao refresh token va save DB thanh cong -> return Id.
- UTCID02 (N): Du lieu luu dung `UserId`, `JwtId`, `TokenHash`.
- UTCID03 (N): `IsActive=true` khi moi tao va chua used/revoked.

### F037 - RefreshTokenService.GetByTokenHashAsync (3 case)
- UTCID01 (A): Hash khong ton tai -> null.
- UTCID02 (N): Hash ton tai -> map dto dung fields.
- UTCID03 (N): Token revoked/used -> `IsActive=false` trong dto.

### F038 - RefreshTokenService.RotateRefreshTokenAsync (3 case)
- UTCID01 (A): Old token id khong ton tai -> throw.
- UTCID02 (N): Old token ton tai -> tao new token thanh cong.
- UTCID03 (N): Old token duoc `MarkAsUsed` va lien ket token moi.

### F039 - RefreshTokenService.RevokeTokenAsync (2 case)
- UTCID01 (N): Token ton tai -> revoke va save.
- UTCID02 (N): Token khong ton tai -> no-op, khong throw.

### F040 - RefreshTokenService.RevokeAllUserTokensAsync (2 case)
- UTCID01 (N): Co nhieu token chua revoke -> tat ca bi revoke.
- UTCID02 (B): Khong co token hop le -> save van an toan.

### F041 - RefreshTokenService.RevokeTokenFamilyAsync (2 case)
- UTCID01 (A): Token goc khong ton tai -> return, khong throw.
- UTCID02 (N): Token ton tai -> revoke toan bo token cua user.

### F042 - RefreshTokenService.CleanupExpiredTokensAsync (3 case)
- UTCID01 (N): Token het han qua cutoff -> bi xoa.
- UTCID02 (N): Token revoked qua cutoff -> bi xoa.
- UTCID03 (B): Khong co token can xoa -> return 0.

### F043 - TokenService.GenerateAccessTokenAsync (4 case)
- UTCID01 (N): Tao token co claims bat buoc (`sub`,`uid`,`email`,`jti`,`iat`).
- UTCID02 (N): Roles duoc dua vao `ClaimTypes.Role` va `role`.
- UTCID03 (N): Additional claims duoc append dung.
- UTCID04 (B): ExpiresAt dung theo config `AccessTokenExpiryMinutes`.

### F044 - TokenService.GenerateRefreshToken (3 case)
- UTCID01 (N): Token base64 hop le, khong rong.
- UTCID02 (N): 2 lan sinh lien tiep khac nhau.
- UTCID03 (B): Do dai token du lon (>=64 byte random truoc encode).

### F045 - TokenService.ValidateToken (3 case)
- UTCID01 (N): Token hop le -> tra ClaimsPrincipal.
- UTCID02 (A): Token sai signature -> null.
- UTCID03 (A): Token alg khong phai HmacSha256 -> null.

### F046 - TokenService.GetUserIdFromToken (3 case)
- UTCID01 (N): Token hop le co `sub` -> parse Guid thanh cong.
- UTCID02 (N): Khong co `sub` nhung co `uid`/`nameidentifier` -> van lay duoc.
- UTCID03 (A): Claim user id khong parse duoc -> null.

### F047 - TokenService.GetJtiFromToken (2 case)
- UTCID01 (N): Token hop le -> lay `jti`.
- UTCID02 (A): Token invalid -> null.

### F048 - TokenService.HashToken (3 case)
- UTCID01 (N): Cung input -> hash giong nhau.
- UTCID02 (N): Khac input -> hash khac nhau.
- UTCID03 (B): Input rong -> van hash duoc (khong null).

### F049 - AdminQueryService.GetOphthalmologistsAsync (5 case)
- UTCID01 (N): Khong filter -> paging + sort CreatedAt desc dung.
- UTCID02 (N): SearchTerm tim theo fullName/email/phone ILike.
- UTCID03 (N): VerificationStatus nhieu gia tri phan cach dau phay -> loc dung.
- UTCID04 (N): Map licenses/degrees tu Certificates dung theo Type.
- UTCID05 (B): VerificationStatus khong parse duoc -> bo qua filter status.

### F050 - AdminQueryService.GetPatientsAsync (4 case)
- UTCID01 (N): SearchTerm hoat dong theo fullName/email.
- UTCID02 (N): Filter status `active|pending|suspended` dung.
- UTCID03 (N): Paging dung totalCount/items.
- UTCID04 (A): User deleted khong duoc hien thi.

### F051 - AdminQueryService.GetAuditLogsAsync (5 case)
- UTCID01 (N): Loc theo search term action/entity/email dung.
- UTCID02 (N): Loc theo action/entityName/userId dung.
- UTCID03 (N): Loc theo fromDate/toDate dung.
- UTCID04 (N): Paging + sort `CreatedAt desc` dung.
- UTCID05 (B): Log khong co user join -> `UserName=null` van map duoc.

### F052 - AiQuotaService.GetQuotaAsync (5 case)
- UTCID01 (N): Role Patient co profile -> tinh total/used/remaining/free-purchased source dung.
- UTCID02 (A): Role Patient khong co profile -> fallback free quota.
- UTCID03 (N): Role ClinicStaff co clinic -> tinh quota theo clinic.
- UTCID04 (A): Role clinic nhung user khong co clinic -> None quota.
- UTCID05 (A): Role la -> None quota.

### F053 - AiQuotaService.HasAvailableQuotaAsync (2 case)
- UTCID01 (N): Remaining > 0 -> true.
- UTCID02 (B): Remaining = 0 -> false.

### F054 - AiQuotaService.DeductQuotaAsync (4 case)
- UTCID01 (N): Patient ton tai -> consume quota + save.
- UTCID02 (N): Clinic role va clinic ton tai -> consume quota clinic + save.
- UTCID03 (A): Patient khong ton tai -> throw `InvalidOperationException`.
- UTCID04 (A): User clinic role khong co clinic -> throw.

### F055 - AiQuotaService.AddPurchasedQuotaAsync (4 case)
- UTCID01 (N): Patient ton tai -> tang purchased quota.
- UTCID02 (N): Clinic role co clinic -> tang purchased quota clinic.
- UTCID03 (A): Patient khong ton tai -> throw.
- UTCID04 (A): Clinic khong ton tai -> throw.

### F056 - BetterStackHeartbeatService.GetEmbedUrl (2 case)
- UTCID01 (B): EmbedUrl null/white-space -> null.
- UTCID02 (N): EmbedUrl hop le -> tra lai dung gia tri.

### F057 - BetterStackHeartbeatService.GetMonitorDescriptors (2 case)
- UTCID01 (N): Tra du descriptor cho tat ca enum monitor.
- UTCID02 (N): Truong `Configured` dung theo endpoint setting.

### F058 - BetterStackHeartbeatService.NotifyStartedAsync (3 case)
- UTCID01 (B): Endpoint khong config -> skip, khong throw.
- UTCID02 (N): Endpoint config -> POST thanh cong.
- UTCID03 (A): HTTP fail/exception -> warning log, khong throw.

### F059 - BetterStackHeartbeatService.NotifySucceededAsync (2 case)
- UTCID01 (N): Ping URL hop le -> goi POST.
- UTCID02 (A): Non-success status -> warning log.

### F060 - BetterStackHeartbeatService.NotifyFailedAsync (2 case)
- UTCID01 (N): Co FailUrl -> POST den fail url.
- UTCID02 (A): Khong co FailUrl -> fallback ping url, loi cung khong throw.

- UTCID01 (N): Co du lieu UsedAiQuota>0 -> reset thanh cong va notify started/succeeded.
- UTCID02 (B): Khong co dong can reset -> count=0 van thanh cong.
- UTCID03 (A): Exception DB -> notify failed va rethrow.

### F061 - DashboardMetricsService.GetSystemAdminMetricsAsync (6 case)
- UTCID01 (N): Tinh tong doctor/clinic/patient va growth phan tram dung.
- UTCID02 (N): Revenue by payment method + percentage dung.
- UTCID03 (N): Monthly/Daily revenue map day du theo period.
- UTCID04 (N): Pending actions (verification/onboarding) dung.
- UTCID05 (N): Top doctor by consultation revenue + top clinic by rating dung.
- UTCID06 (N): BetterStack monitor map vao response dung.

### F062 - DashboardMetricsService.GetRecentScreeningsAsync (4 case)
- UTCID01 (N): Paging total/items dung.
- UTCID02 (N): RiskLevel va `IsCritical` map dung.
- UTCID03 (N): Status `Completed/Analyzing` map theo `ProcessedAt`.
- UTCID04 (B): Screening khong co risk result -> risk null van map duoc.

### F063 - DashboardMetricsService.GetScreeningVolumeTrendsAsync (3 case)
- UTCID01 (N): `weekly` -> group theo dau tuan, label dd MMM.
- UTCID02 (N): `monthly` -> group theo thang, label MMM yyyy.
- UTCID03 (B): `timeRange` la -> fallback monthly.

### F064 - DashboardMetricsService.GetPopulationRiskAnalysisAsync (3 case)
- UTCID01 (N): Tong `TotalPatients` dung.
- UTCID02 (N): Bo qua RiskLevel.None trong danh sach output.
- UTCID03 (B): Khong co result -> percentage=0.

### F065 - DashboardMetricsService.GetSystemHealthAsync (2 case)
- UTCID01 (N): Tra payload co 3 component Database/AI/Notifications.
- UTCID02 (N): `AllSystemsOperational=true` va fields health day du.

### F066 - DashboardMetricsService.GetOphthalmologistMetricsAsync (4 case)
- UTCID01 (A): UserId khong map duoc doctor -> dto rong.
- UTCID02 (N): Dem pending reviews va urgent cases dung.
- UTCID03 (N): Dem completed today dung.
- UTCID04 (N): Tinh open slots today dung theo template/clinic.

### F067 - DashboardMetricsService.GetPatientMetricsAsync (3 case)
- UTCID01 (A): Khong tim thay patient profile -> dto rong.
- UTCID02 (N): Dem completed screenings / total reports dung.
- UTCID03 (N): Dem upcoming appointments + remaining quota dung.

### F068 - DateTimeService.Now (2 case)
- UTCID01 (N): Gia tri `Now` gan voi `DateTime.Now`.
- UTCID02 (B): Moi lan goi co the thay doi theo thoi gian thuc.

### F069 - DateTimeService.UtcNow (2 case)
- UTCID01 (N): Gia tri `UtcNow` gan voi `DateTime.UtcNow`.
- UTCID02 (B): `Kind` cua gia tri la UTC.

### F070 - EmailService.SendEmailConfirmationAsync (2 case)
- UTCID01 (N): Dung subject/template confirm email.
- UTCID02 (N): Goi `SendAsync` voi `isHtml=true`.

### F071 - EmailService.SendPasswordResetAsync (2 case)
- UTCID01 (N): Dung subject/template reset password.
- UTCID02 (N): Goi `SendAsync` thanh cong.

### F072 - EmailService.SendWelcomeEmailAsync (2 case)
- UTCID01 (N): Dung subject/template welcome.
- UTCID02 (N): Goi `SendAsync` va ghi log info.

### F073 - EmailService.SendAsync (4 case)
- UTCID01 (A): `to` rong -> `ArgumentException`.
- UTCID02 (A): `subject` hoac `body` rong -> `ArgumentException`.
- UTCID03 (N): SMTP connect/auth/send/disconnect thanh cong -> debug log.
- UTCID04 (A): SMTP fail -> log error va rethrow.

### F074 - GoogleMeetService.CreateMeetingAsync (5 case)
- UTCID01 (N): Tao event co meet link ngay -> return `MeetingInfo`.
- UTCID02 (N): Khong co link ngay lan dau -> retry va lay duoc link.
- UTCID03 (A): Retry het van khong co link -> cleanup orphan event + throw.
- UTCID04 (N): Co attendeeEmails -> map attendees vao event.
- UTCID05 (B): `durationMinutes` null -> dung default duration setting.

### F075 - GoogleMeetService.DeleteMeetingAsync (2 case)
- UTCID01 (N): Xoa event thanh cong.
- UTCID02 (A): API tra 404 not found -> warning, khong throw.

### F076 - GoogleMeetService.Dispose (1 case)
- UTCID01 (N): Goi dispose -> calendar service duoc dispose an toan.

### F077 - NotificationService.SendAsync (typed) (4 case)
- UTCID01 (N): Payload object -> serialize camelCase JSON.
- UTCID02 (N): Persist notification + SaveChanges + broadcast notification DTO.
- UTCID03 (N): Tinh unread count va broadcast unread count dung.
- UTCID04 (A): Loi repository/hub -> log error va rethrow.

### F078 - NotificationService.SendAsync (legacy) (1 case)
- UTCID01 (N): Overload legacy chuyen dung tham so sang overload typed.



### F079 - PatientRoadmapGenerationService.GenerateFromDiagnosisAsync (6 case)
- UTCID01 (A): `PatientId` rong -> Failure validation.
- UTCID02 (A): `ScreeningId` rong hoac `AiScreeningRawJson` rong -> Failure validation.
- UTCID03 (A): Chua config ApiKey -> Failure.
- UTCID04 (N): AI tra JSON hop le -> parse + normalize + Success.
- UTCID05 (A): AI tra JSON sai format lien tiep -> retry het va Failure.
- UTCID06 (A): HTTP timeout/network error qua max retry -> Failure unavailable.

### F080 - PayOSService.CreatePaymentLinkAsync (5 case)
- UTCID01 (N): Tao payment link thanh cong, return `checkoutUrl` + `orderCode`.
- UTCID02 (B): `returnUrl/cancelUrl` rong -> dung default URL tu settings.
- UTCID03 (N): Description >25 ky tu -> bi truncate dung gioi han.
- UTCID04 (N): URL output co append query param `orderCode`.
- UTCID05 (A): SDK tra null hoac empty url -> throw exception.

### F081 - PayOSService.GetPaymentStatusAsync (3 case)
- UTCID01 (N): Query thanh cong -> map `Status/Amount/TxnRef`.
- UTCID02 (A): PaymentInfo null -> throw "Payment not found".
- UTCID03 (A): SDK throw -> wrap va throw exception service.

### F082 - PayOSService.VerifyWebhookSignatureAsync (2 case)
- UTCID01 (N): Flow hien tai -> return true.
- UTCID02 (A): Exception branch -> return false + log error.

### F083 - PayOSService.CancelPaymentAsync (3 case)
- UTCID01 (N): SDK cancel tra object -> true.
- UTCID02 (B): SDK tra null -> false.
- UTCID03 (A): Exception -> false va log error.

### F084 - SupabaseStorageService.SaveFileAsync (5 case)
- UTCID01 (N): Filename duoc sanitize + path tao dung theo subfolder.
- UTCID02 (A): Stream rong -> throw `InvalidOperationException`.
- UTCID03 (N): Upload thanh cong -> tra public URL.
- UTCID04 (B): SDK tra URL tuong doi -> fallback thanh absolute URL.
- UTCID05 (N): ContentType map dung theo extension file.

### F085 - SupabaseStorageService.DeleteFile (3 case)
- UTCID01 (N): Input full URL -> extract path va remove thanh cong -> true.
- UTCID02 (N): Input relative path hop le -> remove thanh cong -> true.
- UTCID03 (A): Remove throw exception/path invalid -> false.

### F086 - SupabaseStorageService.FileExists (3 case)
- UTCID01 (N): HEAD tra success -> true.
- UTCID02 (A): HEAD tra non-success -> false.
- UTCID03 (A): Input rong/exception request -> false.

### F087 - SystemSettingService.GetSettingAsync (2 case)
- UTCID01 (N): Key ton tai -> tra value dung.
- UTCID02 (A): Key khong ton tai -> null.

### F088 - SystemSettingService.GetAllSettingsAsync (2 case)
- UTCID01 (N): Co du lieu -> dictionary day du key/value.
- UTCID02 (B): Khong co du lieu -> dictionary rong.

### F089 - SystemSettingService.UpdateSettingsAsync (4 case)
- UTCID01 (B): Input null/rong -> no-op.
- UTCID02 (N): Key da ton tai -> goi `UpdateValue`.
- UTCID03 (N): Key moi -> add `SystemSetting` moi.
- UTCID04 (N): Sau update -> `SaveChangesAsync` duoc goi + log info keys.

### F090 - ClinicVisitService.ProcessPaymentCompletionAsync (4 case)
- UTCID01 (N): Order status Paid -> appointment status Completed.
- UTCID02 (N): Generate result record thanh cong.
- UTCID03 (N): Broadcast notification den Clinic Staff.
- UTCID04 (A): Order null hoac Not Paid -> Failure.

### F091 - PayOSPayoutService.CreatePayoutAsync (3 case)
- UTCID01 (N): Balance du -> tao disbursement success.
- UTCID02 (A): Balance khong du -> Failure insufficient balance.
- UTCID03 (A): SDK throw error -> Failure generic.

### F092 - PayOSPayoutService.GetPayoutAsync (3 case)
- UTCID01 (N): PayoutId hop le -> tra trang thai disbursement.
- UTCID02 (A): PayoutId khong ton tai -> NotFound.
- UTCID03 (A): Exception API -> Failure.

### F093 - PayOSPayoutService.GetPayoutsAsync (3 case)
- UTCID01 (N): Fetch list payouts thanh cong.
- UTCID02 (N): Search/Filter theo status hoat dong.
- UTCID03 (B): Khong co du lieu -> empty list.

### F094 - PayOSPayoutService.EstimateCreditAsync (3 case)
- UTCID01 (N): Amount hop le -> tinh phi va credit dung.
- UTCID02 (A): Amount <= 0 -> Failure.
- UTCID03 (B): Amount qua lon -> Failure limit.

### F095 - PayOSPayoutService.GetPayoutAccountBalanceAsync (3 case)
- UTCID01 (N): API tra ve so du hop le.
- UTCID02 (A): API auth fail -> throw Unauthorized.
- UTCID03 (A): Exception mang -> throw.

### F096 - PatientScreeningPdfService.GenerateScreeningReportPdf (3 case)
- UTCID01 (N): Data hop le -> tra ve byte[] PDF (QuestPDF).
- UTCID02 (A): Screening data thieu -> Failure/Exception.
- UTCID03 (A): Render engine error -> Failure.
