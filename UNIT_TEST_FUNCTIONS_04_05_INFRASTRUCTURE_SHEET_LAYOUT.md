# Infrastructure Unit Test - Sheet Layout Ready

Last updated: 27/04/2026
Source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

## Audit summary

- Total function blocks: **87** (`F001`-`F087`, continuous).
- Duplicate function codes: **0**.
- Missing function codes: **0**.
- Layout regenerated from checklist Function Catalog + test-case statistics to remove stale/duplicated blocks.

---

## F001 - AuthService.RegisterPatientAsync

| Header           | Value                                                                                                                                            |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F001                                                                                                                                             |
| Function Name    | AuthService.RegisterPatientAsync                                                                                                                 |
| Total Test Cases | 10                                                                                                                                               |
| Created By       |                                                                                                                                                  |
| Executed By      |                                                                                                                                                  |
| Lines of Code    | 1456 |
| Passed           | 10                                                                                                                                               |
| Failed           | 0                                                                                                                                                |
| Untested         | 0                                                                                                                                                |
| Count type N     | 1                                                                                                                                                |
| Count type A     | 9                                                                                                                                                |
| Count type B     | 0                                                                                                                                                |
| Test Requirement | Validate 'Register patient account' in AuthService.RegisterPatientAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                             | Precondition                                                                                                 | UTCIDs  |
| ------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------ | ------- |
| Email hợp lệ, user chưa tồn tại, `CreateAsync` thành công → tạo user + Patient profile, commit, gửi email confirm → trả về `Success`. | UserManager.FindByEmailAsync trả về null; CreateAsync thành công; EmailService mock; UnitOfWork mock commit. | UTCID01 |
| Email đã tồn tại trong DB → trả về `Failure("Email already in use")`.                                                                 | UserManager.FindByEmailAsync trả về user hợp lệ; các mock khác không cần setup.                              | UTCID02 |
| Email hợp lệ nhưng `CreateAsync` thất bại (Identity errors) → trả về `Failure` chứa danh sách lỗi.                                    | FindByEmailAsync trả về null; CreateAsync trả về IdentityResult.Failed(...); rollback mock.                  | UTCID03 |
| Password không đáp ứng policy (quá ngắn, thiếu ký tự đặc biệt…) → `CreateAsync` trả về lỗi → `Failure`.                               | FindByEmailAsync trả về null; CreateAsync trả về IdentityResult.Failed với password error.                   | UTCID04 |
| FullName rỗng hoặc null → validation guard trả về `Failure("Full name is required")`.                                                 | Request DTO có FullName = null/""; không cần mock UserManager.                                               | UTCID05 |
| Email rỗng hoặc null → validation guard trả về `Failure("Email is required")`.                                                        | Request DTO có Email = null/""; không cần mock UserManager.                                                  | UTCID06 |
| `AddToRoleAsync` thất bại sau khi tạo user thành công → rollback → trả về `Failure`.                                                  | CreateAsync thành công; AddToRoleAsync trả về IdentityResult.Failed; RollbackTransactionAsync được gọi.      | UTCID07 |
| `UnitOfWork.CommitTransactionAsync` ném exception → rollback → trả về `Failure("An error occurred during registration")`.             | CreateAsync, AddToRoleAsync thành công; CommitTransactionAsync ném exception; RollbackTransactionAsync mock. | UTCID08 |
| Email hợp lệ, user chưa tồn tại, luồng thành công → `SendEmailConfirmationAsync` được gọi đúng 1 lần với đúng email và token.         | Toàn bộ luồng mock thành công; verify EmailService.SendEmailConfirmationAsync được invoked 1 lần.            | UTCID09 |
| Đăng ký thành công → response chứa `UserId` là Guid không rỗng và `Email` khớp input.                                                 | Toàn bộ luồng mock thành công; kiểm tra giá trị trả về UserId != Guid.Empty và Email == request.Email.       | UTCID10 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                           | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `Result.Success` với UserId != Guid.Empty                 | –                  | –                    | P             |               |           |
| UTCID02 | A            | `Result.Failure("Email already in use")`                  | –                  | –                    | P             |               |           |
| UTCID03 | A            | `Result.Failure` chứa Identity errors                     | –                  | –                    | P             |               |           |
| UTCID04 | A            | `Result.Failure` chứa password policy error               | –                  | –                    | P             |               |           |
| UTCID05 | A            | `Result.Failure("Full name is required")`                 | –                  | –                    | P             |               |           |
| UTCID06 | A            | `Result.Failure("Email is required")`                     | –                  | –                    | P             |               |           |
| UTCID07 | A            | `Result.Failure` – rollback do AddToRole thất bại         | –                  | –                    | P             |               |           |
| UTCID08 | A            | `Result.Failure("An error occurred during registration")` | –                  | –                    | P             |               |           |
| UTCID09 | A            | `Result.Success`; EmailService gọi đúng 1 lần             | –                  | –                    | P             |               |           |
| UTCID10 | A            | `Result.Success`; UserId != Guid.Empty; Email == input    | –                  | –                    | P             |               |           |

---

## F002 - AuthService.LookupAccountByCitizenIdAsync

| Header           | Value                                                                                                                                    |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F002                                                                                                                                     |
| Function Name    | AuthService.LookupAccountByCitizenIdAsync                                                                                                |
| Total Test Cases | 3                                                                                                                                        |
| Created By       |                                                                                                                                          |
| Executed By      |                                                                                                                                          |
| Lines of Code    | 1456 |
| Passed           | 3                                                                                                                                        |
| Failed           | 0                                                                                                                                        |
| Untested         | 0                                                                                                                                        |
| Count type N     | 1                                                                                                                                        |
| Count type A     | 2                                                                                                                                        |
| Count type B     | 0                                                                                                                                        |
| Test Requirement | Validate 'Lookup account by citizen id' in AuthService.LookupAccountByCitizenIdAsync, covering exists/not exists and masked email logic. |

### Condition Matrix

| Condition                                                                                                                | Precondition                                                                                             | UTCIDs  |
| ------------------------------------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------------------------- | ------- |
| citizenId hợp lệ, user tồn tại (IsDeleted=false) → trả về Success + Exists=true + MaskedEmail (vd. `ab***@gmail.com`).   | `_userManager.Users` mock/seed có user khớp CitizenId và IsDeleted=false; citizenId là chuỗi không rỗng. | UTCID01 |
| citizenId hợp lệ, không tìm thấy user trong DB → trả về Success + Exists=false + MaskedEmail=null.                       | `_userManager.Users` mock/seed không có user khớp CitizenId; citizenId là chuỗi không rỗng.              | UTCID02 |
| citizenId là null, rỗng hoặc chỉ khoảng trắng → trả về Failure với message "Citizen ID is required." (validation guard). | citizenId truyền vào là `null`, `""` hoặc `"   "`; không cần mock UserManager.                           | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                       | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ----------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `Result.Success`; Exists=true; MaskedEmail chứa `***` | –                  | –                    | P             |               |           |
| UTCID02 | A            | `Result.Success`; Exists=false; MaskedEmail=null      | –                  | –                    | P             |               |           |
| UTCID03 | A            | `Result.Failure("Citizen ID is required.")`           | –                  | –                    | P             |               |           |

---

## F003 - AuthService.GoogleLoginAsync

| Header           | Value                                                                                                                            |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F003                                                                                                                             |
| Function Name    | AuthService.GoogleLoginAsync                                                                                                     |
| Total Test Cases | 10                                                                                                                               |
| Created By       |                                                                                                                                  |
| Executed By      |                                                                                                                                  |
| Lines of Code    | 1456 |
| Passed           | 10                                                                                                                               |
| Failed           | 0                                                                                                                                |
| Untested         | 0                                                                                                                                |
| Count type N     | 1                                                                                                                                |
| Count type A     | 9                                                                                                                                |
| Count type B     | 0                                                                                                                                |
| Test Requirement | Validate 'Google login' in AuthService.GoogleLoginAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                                       | Precondition                                                                                                           | UTCIDs  |
| ----------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------- | ------- |
| Google ID token không hợp lệ → `InvalidJwtException` bị bắt → trả về `Unauthorized("Invalid Google token")`.                                    | `GoogleJsonWebSignature.ValidateAsync` được mock/stub ném `InvalidJwtException`; tất cả dependency khác được mock sẵn. | UTCID01 |
| Google token hợp lệ nhưng payload không có email (Email rỗng/null) → trả về `Failure("Google account does not have an email address")`.         | `ValidateAsync` trả về payload với `Email = null` hoặc `""`; UserManager không cần setup.                              | UTCID02 |
| User tồn tại, `IsDeleted = true` → trả về `Unauthorized("Account has been deleted")`.                                                           | `UserManager.FindByEmailAsync` trả về user có `IsDeleted = true`; payload email hợp lệ.                                | UTCID03 |
| User tồn tại, `IsActive = false` → trả về `Unauthorized("Account is deactivated. Please contact support.")`.                                    | `UserManager.FindByEmailAsync` trả về user có `IsActive = false`, `IsDeleted = false`.                                 | UTCID04 |
| User tồn tại, `EmailConfirmed = false` → tự động confirm email, gọi `UpdateAsync`, tiếp tục luồng login.                                        | UserManager trả về user có `EmailConfirmed = false`; `UpdateAsync` được mock thành công.                               | UTCID05 |
| User tồn tại, 2FA bật (`GetTwoFactorEnabledAsync = true`) → trả về `Success(LoginResponse.TwoFactorRequired(user.Id))`.                         | `UserManager.GetTwoFactorEnabledAsync` mock trả về `true`; user active, email confirmed.                               | UTCID06 |
| User tồn tại, hợp lệ, 2FA tắt → login thành công, trả về `Success(LoginResponse.Success(authResponse))` với access/refresh token đầy đủ.        | User active, email confirmed, 2FA disabled; TokenService và RefreshTokenService mock thành công.                       | UTCID07 |
| User mới (không tìm thấy qua email/provider) → tạo user mới, add role Patient, tạo Patient profile → commit → login thành công.                 | `UserManager.FindByEmailAsync` và `FindByLoginAsync` trả về `null`; `CreateAsync` thành công.                          | UTCID08 |
| User mới, `CreateAsync` thất bại (Identity errors) → rollback transaction → trả về `Failure` chứa danh sách lỗi.                                | `UserManager.CreateAsync` trả về `IdentityResult.Failed(...)` với errors; `BeginTransactionAsync` mock sẵn.            | UTCID09 |
| Xảy ra exception bất kỳ trong quá trình xử lý → rollback transaction (best-effort) → trả về `Failure("An error occurred during Google login")`. | Một dependency nào đó (vd. `UnitOfWork.CommitTransactionAsync`) ném exception; transaction đã bắt đầu.                 | UTCID10 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                                                    | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ---------------------------------------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | `Result.Unauthorized("Invalid Google token")`                                      | –                  | –                    | P             |               |           |
| UTCID02 | A            | `Result.Failure("Google account does not have an email address")`                  | –                  | –                    | P             |               |           |
| UTCID03 | A            | `Result.Unauthorized("Account has been deleted")`                                  | –                  | –                    | P             |               |           |
| UTCID04 | A            | `Result.Unauthorized("Account is deactivated. Please contact support.")`           | –                  | –                    | P             |               |           |
| UTCID05 | A            | `Result.Success`; EmailConfirmed được set true; UpdateAsync được gọi               | –                  | –                    | P             |               |           |
| UTCID06 | A            | `Result.Success(LoginResponse.TwoFactorRequired(userId))`                          | –                  | –                    | P             |               |           |
| UTCID07 | N            | `Result.Success(LoginResponse.Success(authResponse))` với access/refresh token     | –                  | –                    | P             |               |           |
| UTCID08 | A            | `Result.Success` – user mới được tạo, role Patient added, Patient profile created  | –                  | –                    | P             |               |           |
| UTCID09 | A            | `Result.Failure` chứa Identity errors; transaction rolled back                     | –                  | –                    | P             |               |           |
| UTCID10 | A            | `Result.Failure("An error occurred during Google login")`; transaction rolled back | –                  | –                    | P             |               |           |

---

## F004 - AuthService.LoginAsync

| Header           | Value                                                                                                                             |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F004                                                                                                                              |
| Function Name    | AuthService.LoginAsync                                                                                                            |
| Total Test Cases | 12                                                                                                                                |
| Created By       |                                                                                                                                   |
| Executed By      |                                                                                                                                   |
| Lines of Code    | 1456 |
| Passed           | 12                                                                                                                                |
| Failed           | 0                                                                                                                                 |
| Untested         | 0                                                                                                                                 |
| Count type N     | 1                                                                                                                                 |
| Count type A     | 11                                                                                                                                |
| Count type B     | 0                                                                                                                                 |
| Test Requirement | Validate 'Login with password' in AuthService.LoginAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                   | Precondition                                                                                               | UTCIDs  |
| --------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------- | ------- |
| Email rỗng/null → `Failure("Email is required")`.                                                                           | Request DTO có Email = null/"".                                                                            | UTCID01 |
| Password rỗng/null → `Failure("Password is required")`.                                                                     | Request DTO có Password = null/"".                                                                         | UTCID02 |
| Email không tìm thấy user → `Unauthorized("Invalid credentials")`.                                                          | FindByEmailAsync trả về null.                                                                              | UTCID03 |
| User bị xoá mềm (IsDeleted=true) → `Unauthorized("Account has been deleted")`.                                              | FindByEmailAsync trả về user có IsDeleted=true.                                                            | UTCID04 |
| User không active (IsActive=false) → `Unauthorized("Account is deactivated")`.                                              | FindByEmailAsync trả về user có IsActive=false.                                                            | UTCID05 |
| Email chưa xác nhận → `Unauthorized("Email not confirmed")`.                                                                | FindByEmailAsync trả về user có EmailConfirmed=false.                                                      | UTCID06 |
| Sai mật khẩu (CheckPasswordSignInAsync thất bại) → `Unauthorized("Invalid credentials")`.                                   | CheckPasswordSignInAsync trả về Failed.                                                                    | UTCID07 |
| Tài khoản bị khóa (LockoutEnabled) → `Unauthorized("Account is locked out")`.                                               | CheckPasswordSignInAsync trả về Lockout.                                                                   | UTCID08 |
| 2FA bật → `Success(LoginResponse.TwoFactorRequired(userId))`.                                                               | GetTwoFactorEnabledAsync trả về true; password đúng.                                                       | UTCID09 |
| Login thành công, 2FA tắt → `Success(LoginResponse.Success(authResponse))` với token đầy đủ; UpdateLastLoginAsync được gọi. | Password đúng, user active, email confirmed, 2FA off; TokenService và RefreshTokenService mock thành công. | UTCID10 |
| UnitOfWork.CommitAsync ném exception → `Failure("An error occurred during login")`.                                         | Password đúng nhưng CommitAsync ném exception.                                                             | UTCID11 |
| MustUpdateProfile=true → `Success(LoginResponse.MustUpdateProfile(userId))`.                                                | User có MustUpdateProfile=true; password đúng; 2FA off.                                                    | UTCID12 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                                              | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ---------------------------------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | `Result.Failure("Email is required")`                                        | –                  | –                    | P             |               |           |
| UTCID02 | A            | `Result.Failure("Password is required")`                                     | –                  | –                    | P             |               |           |
| UTCID03 | A            | `Result.Unauthorized("Invalid credentials")`                                 | –                  | –                    | P             |               |           |
| UTCID04 | A            | `Result.Unauthorized("Account has been deleted")`                            | –                  | –                    | P             |               |           |
| UTCID05 | A            | `Result.Unauthorized("Account is deactivated")`                              | –                  | –                    | P             |               |           |
| UTCID06 | A            | `Result.Unauthorized("Email not confirmed")`                                 | –                  | –                    | P             |               |           |
| UTCID07 | A            | `Result.Unauthorized("Invalid credentials")`                                 | –                  | –                    | P             |               |           |
| UTCID08 | A            | `Result.Unauthorized("Account is locked out")`                               | –                  | –                    | P             |               |           |
| UTCID09 | A            | `Result.Success(LoginResponse.TwoFactorRequired(userId))`                    | –                  | –                    | P             |               |           |
| UTCID10 | N            | `Result.Success(LoginResponse.Success(authResponse))` với access/refresh JWT | –                  | –                    | P             |               |           |
| UTCID11 | A            | `Result.Failure("An error occurred during login")`                           | –                  | –                    | P             |               |           |
| UTCID12 | A            | `Result.Success(LoginResponse.MustUpdateProfile(userId))`                    | –                  | –                    | P             |               |           |

---

## F005 - AuthService.VerifyTwoFactorLoginAsync

| Header           | Value                                                                                                                                         |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F005                                                                                                                                          |
| Function Name    | AuthService.VerifyTwoFactorLoginAsync                                                                                                         |
| Total Test Cases | 10                                                                                                                                            |
| Created By       |                                                                                                                                               |
| Executed By      |                                                                                                                                               |
| Lines of Code    | 1456 |
| Passed           | 10                                                                                                                                            |
| Failed           | 0                                                                                                                                             |
| Untested         | 0                                                                                                                                             |
| Count type N     | 1                                                                                                                                             |
| Count type A     | 9                                                                                                                                             |
| Count type B     | 0                                                                                                                                             |
| Test Requirement | Validate 'Verify 2FA login' in AuthService.VerifyTwoFactorLoginAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                     | Precondition                                                                                    | UTCIDs  |
| ----------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------- | ------- |
| UserId rỗng/null → `Failure("User ID is required")`.                                                                          | Request DTO có UserId = null/"".                                                                | UTCID01 |
| Code rỗng/null → `Failure("Verification code is required")`.                                                                  | Request DTO có Code = null/"".                                                                  | UTCID02 |
| Không tìm thấy user theo UserId → `Unauthorized("User not found")`.                                                           | FindByIdAsync trả về null.                                                                      | UTCID03 |
| User bị xoá mềm → `Unauthorized("Account has been deleted")`.                                                                 | FindByIdAsync trả về user có IsDeleted=true.                                                    | UTCID04 |
| User không active → `Unauthorized("Account is deactivated")`.                                                                 | FindByIdAsync trả về user có IsActive=false.                                                    | UTCID05 |
| Mã 2FA không hợp lệ (VerifyTwoFactorTokenAsync = false) → `Unauthorized("Invalid verification code")`.                        | VerifyTwoFactorTokenAsync trả về false.                                                         | UTCID06 |
| Mã 2FA hợp lệ, dùng TOTP provider → login thành công, trả về `Success(LoginResponse.Success(authResponse))` với token đầy đủ. | VerifyTwoFactorTokenAsync trả về true; TokenService và RefreshTokenService mock thành công.     | UTCID07 |
| Mã 2FA hợp lệ, dùng recovery code (RedeemTwoFactorRecoveryCodeAsync = true) → login thành công; mã recovery bị vô hiệu hoá.   | RedeemTwoFactorRecoveryCodeAsync trả về IdentityResult.Success; toàn bộ luồng token thành công. | UTCID08 |
| Recovery code không hợp lệ (RedeemTwoFactorRecoveryCodeAsync thất bại) → `Unauthorized("Invalid recovery code")`.             | RedeemTwoFactorRecoveryCodeAsync trả về IdentityResult.Failed.                                  | UTCID09 |
| UnitOfWork.CommitAsync ném exception trong lúc lưu refresh token → `Failure("An error occurred during 2FA verification")`.    | VerifyTwoFactorTokenAsync thành công nhưng CommitAsync ném exception.                           | UTCID10 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                                              | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ---------------------------------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | `Result.Failure("User ID is required")`                                      | –                  | –                    | P             |               |           |
| UTCID02 | A            | `Result.Failure("Verification code is required")`                            | –                  | –                    | P             |               |           |
| UTCID03 | A            | `Result.Unauthorized("User not found")`                                      | –                  | –                    | P             |               |           |
| UTCID04 | A            | `Result.Unauthorized("Account has been deleted")`                            | –                  | –                    | P             |               |           |
| UTCID05 | A            | `Result.Unauthorized("Account is deactivated")`                              | –                  | –                    | P             |               |           |
| UTCID06 | A            | `Result.Unauthorized("Invalid verification code")`                           | –                  | –                    | P             |               |           |
| UTCID07 | N            | `Result.Success(LoginResponse.Success(authResponse))` với access/refresh JWT | –                  | –                    | P             |               |           |
| UTCID08 | A            | `Result.Success(LoginResponse.Success(authResponse))`; recovery code revoked | –                  | –                    | P             |               |           |
| UTCID09 | A            | `Result.Unauthorized("Invalid recovery code")`                               | –                  | –                    | P             |               |           |
| UTCID10 | A            | `Result.Failure("An error occurred during 2FA verification")`                | –                  | –                    | P             |               |           |

---

## F006 - AuthService.RefreshTokenAsync

| Header           | Value                                                                                                                                   |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F006                                                                                                                                    |
| Function Name    | AuthService.RefreshTokenAsync                                                                                                           |
| Total Test Cases | 10                                                                                                                                      |
| Created By       |                                                                                                                                         |
| Executed By      |                                                                                                                                         |
| Lines of Code    | 1456 |
| Passed           | 10                                                                                                                                      |
| Failed           | 0                                                                                                                                       |
| Untested         | 0                                                                                                                                       |
| Count type N     | 1                                                                                                                                       |
| Count type A     | 9                                                                                                                                       |
| Count type B     | 0                                                                                                                                       |
| Test Requirement | Validate 'Refresh token flow' in AuthService.RefreshTokenAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                              | Precondition                                                                                            | UTCIDs  |
| -------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------- | ------- |
| AccessToken rỗng/null → `Failure("Access token is required")`.                                                                         | Request DTO có AccessToken = null/"".                                                                   | UTCID01 |
| RefreshToken rỗng/null → `Failure("Refresh token is required")`.                                                                       | Request DTO có RefreshToken = null/"".                                                                  | UTCID02 |
| AccessToken không thể validate (hết hạn hoặc sai chữ ký) → `Unauthorized("Invalid access token")`.                                     | TokenService.ValidateToken trả về null/exception.                                                       | UTCID03 |
| RefreshToken không tìm thấy trong DB → `Unauthorized("Refresh token not found")`.                                                      | RefreshTokenService.GetByTokenHashAsync trả về null.                                                    | UTCID04 |
| RefreshToken đã bị thu hồi (IsRevoked=true) → phát hiện token reuse, thu hồi toàn bộ family → `Unauthorized("Refresh token revoked")`. | DB trả về token có IsRevoked=true; RevokeTokenFamilyAsync được gọi.                                     | UTCID05 |
| RefreshToken đã hết hạn (ExpiresAt < now) → `Unauthorized("Refresh token has expired")`.                                               | DB trả về token có ExpiresAt trong quá khứ.                                                             | UTCID06 |
| JTI trên AccessToken không khớp JTI lưu trong DB → `Unauthorized("Token mismatch")`.                                                   | JTI từ ValidateToken ≠ JTI lưu trên record DB.                                                          | UTCID07 |
| User không tìm thấy theo UserId trong refresh token → `Unauthorized("User not found")`.                                                | FindByIdAsync trả về null.                                                                              | UTCID08 |
| Tất cả hợp lệ → rotate refresh token, cấp access token mới → `Success(authResponse)`.                                                  | Token hợp lệ, user active; TokenService và RefreshTokenService mock thành công; CommitAsync thành công. | UTCID09 |
| CommitAsync ném exception khi rotate → `Failure("An error occurred during token refresh")`.                                            | Toàn bộ validate hợp lệ; CommitAsync ném exception.                                                     | UTCID10 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                                      | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | -------------------------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | `Result.Failure("Access token is required")`                         | –                  | –                    | P             |               |           |
| UTCID02 | A            | `Result.Failure("Refresh token is required")`                        | –                  | –                    | P             |               |           |
| UTCID03 | A            | `Result.Unauthorized("Invalid access token")`                        | –                  | –                    | P             |               |           |
| UTCID04 | A            | `Result.Unauthorized("Refresh token not found")`                     | –                  | –                    | P             |               |           |
| UTCID05 | A            | `Result.Unauthorized("Refresh token revoked")`; token family revoked | –                  | –                    | P             |               |           |
| UTCID06 | A            | `Result.Unauthorized("Refresh token has expired")`                   | –                  | –                    | P             |               |           |
| UTCID07 | A            | `Result.Unauthorized("Token mismatch")`                              | –                  | –                    | P             |               |           |
| UTCID08 | A            | `Result.Unauthorized("User not found")`                              | –                  | –                    | P             |               |           |
| UTCID09 | N            | `Result.Success(authResponse)` với access/refresh token mới          | –                  | –                    | P             |               |           |
| UTCID10 | A            | `Result.Failure("An error occurred during token refresh")`           | –                  | –                    | P             |               |           |

---

## F007 - AuthService.LogoutAsync

| Header           | Value                                                                                                                               |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F007                                                                                                                                |
| Function Name    | AuthService.LogoutAsync                                                                                                             |
| Total Test Cases | 3                                                                                                                                   |
| Created By       |                                                                                                                                     |
| Executed By      |                                                                                                                                     |
| Lines of Code    | 1456 |
| Passed           | 3                                                                                                                                   |
| Failed           | 0                                                                                                                                   |
| Untested         | 0                                                                                                                                   |
| Count type N     | 1                                                                                                                                   |
| Count type A     | 2                                                                                                                                   |
| Count type B     | 0                                                                                                                                   |
| Test Requirement | Validate 'Logout single device' in AuthService.LogoutAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                | Precondition                                                                         | UTCIDs  |
| ---------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | ------- |
| RefreshToken hợp lệ, tìm thấy trong DB → thu hồi token, commit → trả về `Success`.       | GetByTokenHashAsync trả về token hợp lệ; RevokeTokenAsync và CommitAsync thành công. | UTCID01 |
| RefreshToken không tìm thấy trong DB → vẫn trả về `Success` (idempotent logout).         | GetByTokenHashAsync trả về null; không cần commit.                                   | UTCID02 |
| CommitAsync ném exception sau khi revoke → `Failure("An error occurred during logout")`. | RevokeTokenAsync thành công; CommitAsync ném exception.                              | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                     | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `Result.Success`; token revoked                     | –                  | –                    | P             |               |           |
| UTCID02 | A            | `Result.Success` (idempotent)                       | –                  | –                    | P             |               |           |
| UTCID03 | A            | `Result.Failure("An error occurred during logout")` | –                  | –                    | P             |               |           |

---

## F008 - AuthService.LogoutAllAsync

| Header           | Value                                                                                                                                |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F008                                                                                                                                 |
| Function Name    | AuthService.LogoutAllAsync                                                                                                           |
| Total Test Cases | 2                                                                                                                                    |
| Created By       |                                                                                                                                      |
| Executed By      |                                                                                                                                      |
| Lines of Code    | 1456 |
| Passed           | 2                                                                                                                                    |
| Failed           | 0                                                                                                                                    |
| Untested         | 0                                                                                                                                    |
| Count type N     | 1                                                                                                                                    |
| Count type A     | 1                                                                                                                                    |
| Count type B     | 0                                                                                                                                    |
| Test Requirement | Validate 'Logout all devices' in AuthService.LogoutAllAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                     | Precondition                                                      | UTCIDs  |
| ----------------------------------------------------------------------------- | ----------------------------------------------------------------- | ------- |
| UserId hợp lệ → thu hồi tất cả refresh token của user → commit → `Success`.   | RevokeAllUserTokensAsync mock thành công; CommitAsync thành công. | UTCID01 |
| CommitAsync ném exception → `Failure("An error occurred during logout all")`. | RevokeAllUserTokensAsync thành công; CommitAsync ném exception.   | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                         | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `Result.Success`; all tokens revoked                    | –                  | –                    | P             |               |           |
| UTCID02 | A            | `Result.Failure("An error occurred during logout all")` | –                  | –                    | P             |               |           |

---

## F009 - AuthService.ConfirmEmailAsync

| Header           | Value                                                                                                                              |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F009                                                                                                                               |
| Function Name    | AuthService.ConfirmEmailAsync                                                                                                      |
| Total Test Cases | 6                                                                                                                                  |
| Created By       |                                                                                                                                    |
| Executed By      |                                                                                                                                    |
| Lines of Code    | 1456 |
| Passed           | 6                                                                                                                                  |
| Failed           | 0                                                                                                                                  |
| Untested         | 0                                                                                                                                  |
| Count type N     | 1                                                                                                                                  |
| Count type A     | 5                                                                                                                                  |
| Count type B     | 0                                                                                                                                  |
| Test Requirement | Validate 'Confirm email' in AuthService.ConfirmEmailAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                        | Precondition                                     | UTCIDs  |
| -------------------------------------------------------------------------------- | ------------------------------------------------ | ------- |
| UserId rỗng/null → `Failure("User ID is required")`.                             | Request DTO có UserId = null/"".                 | UTCID01 |
| Token rỗng/null → `Failure("Token is required")`.                                | Request DTO có Token = null/"".                  | UTCID02 |
| Không tìm thấy user → `Failure("User not found")`.                               | FindByIdAsync trả về null.                       | UTCID03 |
| Email đã được xác nhận trước đó → `Failure("Email already confirmed")`.          | User có EmailConfirmed=true.                     | UTCID04 |
| Token confirm hợp lệ → ConfirmEmailAsync thành công → `Success`.                 | ConfirmEmailAsync trả về IdentityResult.Success. | UTCID05 |
| ConfirmEmailAsync thất bại (token sai/hết hạn) → `Failure` chứa Identity errors. | ConfirmEmailAsync trả về IdentityResult.Failed.  | UTCID06 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                             | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | `Result.Failure("User ID is required")`     | –                  | –                    | P             |               |           |
| UTCID02 | A            | `Result.Failure("Token is required")`       | –                  | –                    | P             |               |           |
| UTCID03 | A            | `Result.Failure("User not found")`          | –                  | –                    | P             |               |           |
| UTCID04 | A            | `Result.Failure("Email already confirmed")` | –                  | –                    | P             |               |           |
| UTCID05 | N            | `Result.Success`                            | –                  | –                    | P             |               |           |
| UTCID06 | A            | `Result.Failure` chứa Identity errors       | –                  | –                    | P             |               |           |

---

## F010 - AuthService.ForgotPasswordAsync

| Header           | Value                                                                                                                                  |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F010                                                                                                                                   |
| Function Name    | AuthService.ForgotPasswordAsync                                                                                                        |
| Total Test Cases | 3                                                                                                                                      |
| Created By       |                                                                                                                                        |
| Executed By      |                                                                                                                                        |
| Lines of Code    | 1456 |
| Passed           | 3                                                                                                                                      |
| Failed           | 0                                                                                                                                      |
| Untested         | 0                                                                                                                                      |
| Count type N     | 1                                                                                                                                      |
| Count type A     | 2                                                                                                                                      |
| Count type B     | 0                                                                                                                                      |
| Test Requirement | Validate 'Forgot password' in AuthService.ForgotPasswordAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                    | Precondition                                                                                          | UTCIDs  |
| -------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------- | ------- |
| Email rỗng/null → `Failure("Email is required")`.                                            | Request DTO có Email = null/"".                                                                       | UTCID01 |
| Email không tìm thấy user → trả về `Success` (không tiết lộ thông tin – security by design). | FindByEmailAsync trả về null; EmailService không được gọi.                                            | UTCID02 |
| Email tìm thấy, user hợp lệ → tạo token, gửi email reset → `Success`.                        | FindByEmailAsync trả về user hợp lệ; GeneratePasswordResetTokenAsync và EmailService mock thành công. | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                    | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | -------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | `Result.Failure("Email is required")`              | –                  | –                    | P             |               |           |
| UTCID02 | A            | `Result.Success` (email không tồn tại, idempotent) | –                  | –                    | P             |               |           |
| UTCID03 | N            | `Result.Success`; email reset được gửi             | –                  | –                    | P             |               |           |

---

## F011 - AuthService.ResetPasswordAsync

| Header           | Value                                                                                                                                |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F011                                                                                                                                 |
| Function Name    | AuthService.ResetPasswordAsync                                                                                                       |
| Total Test Cases | 5                                                                                                                                    |
| Created By       |                                                                                                                                      |
| Executed By      |                                                                                                                                      |
| Lines of Code    | 1456 |
| Passed           | 5                                                                                                                                    |
| Failed           | 0                                                                                                                                    |
| Untested         | 0                                                                                                                                    |
| Count type N     | 1                                                                                                                                    |
| Count type A     | 4                                                                                                                                    |
| Count type B     | 0                                                                                                                                    |
| Test Requirement | Validate 'Reset password' in AuthService.ResetPasswordAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                                    | Precondition                                                                   | UTCIDs  |
| -------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------ | ------- |
| UserId rỗng/null → `Failure("User ID is required")`.                                                                                         | Request DTO có UserId = null/"".                                               | UTCID01 |
| Token rỗng/null → `Failure("Token is required")`.                                                                                            | Request DTO có Token = null/"".                                                | UTCID02 |
| NewPassword rỗng/null → `Failure("New password is required")`.                                                                               | Request DTO có NewPassword = null/"".                                          | UTCID03 |
| Không tìm thấy user → `Failure("User not found")`.                                                                                           | FindByIdAsync trả về null.                                                     | UTCID04 |
| Token hợp lệ, user tồn tại → ResetPasswordAsync thành công → `Success`; tất cả refresh token của user bị thu hồi (RevokeAllUserTokensAsync). | ResetPasswordAsync trả về IdentityResult.Success; RevokeAllUserTokensAsync OK. | UTCID05 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                              | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | -------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | `Result.Failure("User ID is required")`      | –                  | –                    | P             |               |           |
| UTCID02 | A            | `Result.Failure("Token is required")`        | –                  | –                    | P             |               |           |
| UTCID03 | A            | `Result.Failure("New password is required")` | –                  | –                    | P             |               |           |
| UTCID04 | A            | `Result.Failure("User not found")`           | –                  | –                    | P             |               |           |
| UTCID05 | N            | `Result.Success`; all tokens revoked         | –                  | –                    | P             |               |           |

---

## F012 - AuthService.GetCurrentUserAsync

| Header           | Value                                                                                                                           |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F012                                                                                                                            |
| Function Name    | AuthService.GetCurrentUserAsync                                                                                                 |
| Total Test Cases | 5                                                                                                                               |
| Created By       |                                                                                                                                 |
| Executed By      |                                                                                                                                 |
| Lines of Code    | 1456 |
| Passed           | 5                                                                                                                               |
| Failed           | 0                                                                                                                               |
| Untested         | 0                                                                                                                               |
| Count type N     | 1                                                                                                                               |
| Count type A     | 4                                                                                                                               |
| Count type B     | 0                                                                                                                               |
| Test Requirement | Validate 'Get current user' in AuthService.GetCurrentUserAsync, covering success/failure flow and ClinicStaff sub-role mapping. |

### Condition Matrix

| Condition                                                                                                                                | Precondition                                                                                     | UTCIDs  |
| ---------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------ | ------- |
| UserId null/empty → `Failure("User ID is required")`.                                                                                    | UserId claim trong HttpContext = null/"".                                                        | UTCID01 |
| Không tìm thấy user → `Failure("User not found")`.                                                                                       | FindByIdAsync trả về null.                                                                       | UTCID02 |
| User tồn tại, role không phải ClinicStaff → trả về `Success` với UserInfo đầy đủ, SubRole = null.                                        | GetRolesAsync trả về ["Patient"]; không truy vấn ClinicStaff profile.                            | UTCID03 |
| User tồn tại, role là ClinicStaff → trả về `Success` với UserInfo đầy đủ, SubRole = Receptionist/Doctor tương ứng từ ClinicStaffProfile. | GetRolesAsync trả về ["ClinicStaff"]; ClinicStaffRepository trả về profile với SubRole xác định. | UTCID04 |
| User tồn tại, role là ClinicStaff nhưng không có profile → trả về `Success` với SubRole = null.                                          | GetRolesAsync trả về ["ClinicStaff"]; ClinicStaffRepository trả về null.                         | UTCID05 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                           | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | `Result.Failure("User ID is required")`                   | –                  | –                    | P             |               |           |
| UTCID02 | A            | `Result.Failure("User not found")`                        | –                  | –                    | P             |               |           |
| UTCID03 | N            | `Result.Success`; UserInfo đầy đủ; SubRole = null         | –                  | –                    | P             |               |           |
| UTCID04 | A            | `Result.Success`; SubRole = từ ClinicStaffProfile         | –                  | –                    | P             |               |           |
| UTCID05 | A            | `Result.Success`; SubRole = null (no ClinicStaff profile) | –                  | –                    | P             |               |           |

---

## F013 - AuthService.ResendConfirmationAsync

| Header           | Value                                                                                                                                                |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F013                                                                                                                                                 |
| Function Name    | AuthService.ResendConfirmationAsync                                                                                                                  |
| Total Test Cases | 4                                                                                                                                                    |
| Created By       |                                                                                                                                                      |
| Executed By      |                                                                                                                                                      |
| Lines of Code    | 1456 |
| Passed           | 4                                                                                                                                                    |
| Failed           | 0                                                                                                                                                    |
| Untested         | 0                                                                                                                                                    |
| Count type N     | 1                                                                                                                                                    |
| Count type A     | 3                                                                                                                                                    |
| Count type B     | 0                                                                                                                                                    |
| Test Requirement | Validate 'Resend confirmation email' in AuthService.ResendConfirmationAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                      | Precondition                                                                                                   | UTCIDs  |
| -------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------- | ------- |
| Email rỗng/null → `Failure("Email is required")`.              | Request DTO có Email = null/"".                                                                                | UTCID01 |
| Email không tìm thấy user → `Failure("User not found")`.       | FindByEmailAsync trả về null.                                                                                  | UTCID02 |
| Email đã được xác nhận → `Failure("Email already confirmed")`. | FindByEmailAsync trả về user có EmailConfirmed=true.                                                           | UTCID03 |
| Email chưa xác nhận → tạo token mới, gửi email → `Success`.    | FindByEmailAsync trả về user có EmailConfirmed=false; GenerateEmailConfirmationTokenAsync OK; EmailService OK. | UTCID04 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                             | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | `Result.Failure("Email is required")`       | –                  | –                    | P             |               |           |
| UTCID02 | A            | `Result.Failure("User not found")`          | –                  | –                    | P             |               |           |
| UTCID03 | A            | `Result.Failure("Email already confirmed")` | –                  | –                    | P             |               |           |
| UTCID04 | N            | `Result.Success`; email confirmation resent | –                  | –                    | P             |               |           |

---

## F014 - IdentityService.CheckPasswordAsync

| Header           | Value                                                                                                                                    |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F014                                                                                                                                     |
| Function Name    | IdentityService.CheckPasswordAsync                                                                                                       |
| Total Test Cases | 3                                                                                                                                        |
| Created By       |                                                                                                                                          |
| Executed By      |                                                                                                                                          |
| Lines of Code    | 1054 |
| Passed           | 3                                                                                                                                        |
| Failed           | 0                                                                                                                                        |
| Untested         | 0                                                                                                                                        |
| Count type N     | 1                                                                                                                                        |
| Count type A     | 2                                                                                                                                        |
| Count type B     | 0                                                                                                                                        |
| Test Requirement | Validate 'Check password' in IdentityService.CheckPasswordAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                        | Precondition                                                   | UTCIDs  |
| ---------------------------------------------------------------- | -------------------------------------------------------------- | ------- |
| User null → ném `ArgumentNullException` hoặc trả về `false`.     | Truyền user = null vào method.                                 | UTCID01 |
| Mật khẩu đúng (CheckPasswordAsync trả về true) → trả về `true`.  | UserManager.CheckPasswordAsync mock trả về true; user hợp lệ.  | UTCID02 |
| Mật khẩu sai (CheckPasswordAsync trả về false) → trả về `false`. | UserManager.CheckPasswordAsync mock trả về false; user hợp lệ. | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return    | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------ | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | `false` hoặc throw | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | `true`             | –                       | –                    | P             |               |           |
| UTCID03 | A            | `false`            | –                       | –                    | P             |               |           |

---

## F015 - IdentityService.GetUserByEmailAsync

| Header           | Value                                                                                                                                        |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F015                                                                                                                                         |
| Function Name    | IdentityService.GetUserByEmailAsync                                                                                                          |
| Total Test Cases | 3                                                                                                                                            |
| Created By       |                                                                                                                                              |
| Executed By      |                                                                                                                                              |
| Lines of Code    | 1054 |
| Passed           | 3                                                                                                                                            |
| Failed           | 0                                                                                                                                            |
| Untested         | 0                                                                                                                                            |
| Count type N     | 1                                                                                                                                            |
| Count type A     | 2                                                                                                                                            |
| Count type B     | 0                                                                                                                                            |
| Test Requirement | Validate 'Get user by email' in IdentityService.GetUserByEmailAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                         | Precondition                                       | UTCIDs  |
| ----------------------------------------------------------------- | -------------------------------------------------- | ------- |
| Email rỗng/null → ném `ArgumentException` hoặc trả về null.       | email tham số = null/"".                           | UTCID01 |
| Email tìm thấy user hợp lệ → trả về `ApplicationUser` khớp email. | FindByEmailAsync trả về user với Email khớp input. | UTCID02 |
| Email không tìm thấy → trả về `null`.                             | FindByEmailAsync trả về null.                      | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return          | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------ | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | `null` hoặc throw        | `ArgumentException` | –                    | P             |               |           |
| UTCID02 | N            | `ApplicationUser` object | –                   | –                    | P             |               |           |
| UTCID03 | A            | `null`                   | –                   | –                    | P             |               |           |

---

## F016 - IdentityService.GetUserByIdAsync

| Header           | Value                                                                                                                                  |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F016                                                                                                                                   |
| Function Name    | IdentityService.GetUserByIdAsync                                                                                                       |
| Total Test Cases | 3                                                                                                                                      |
| Created By       |                                                                                                                                        |
| Executed By      |                                                                                                                                        |
| Lines of Code    | 1054 |
| Passed           | 3                                                                                                                                      |
| Failed           | 0                                                                                                                                      |
| Untested         | 0                                                                                                                                      |
| Count type N     | 1                                                                                                                                      |
| Count type A     | 2                                                                                                                                      |
| Count type B     | 0                                                                                                                                      |
| Test Requirement | Validate 'Get user by id' in IdentityService.GetUserByIdAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                        | Precondition                                 | UTCIDs  |
| ---------------------------------------------------------------- | -------------------------------------------- | ------- |
| UserId rỗng/null → ném `ArgumentException` hoặc trả về null.     | userId tham số = null/"".                    | UTCID01 |
| UserId hợp lệ, tìm thấy user → trả về `ApplicationUser` khớp Id. | FindByIdAsync trả về user với Id khớp input. | UTCID02 |
| UserId hợp lệ, không tìm thấy user → trả về `null`.              | FindByIdAsync trả về null.                   | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return          | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------ | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | `null` hoặc throw        | `ArgumentException` | –                    | P             |               |           |
| UTCID02 | N            | `ApplicationUser` object | –                   | –                    | P             |               |           |
| UTCID03 | A            | `null`                   | –                   | –                    | P             |               |           |

---

## F017 - IdentityService.IsEmailConfirmedAsync

| Header           | Value                                                                                                                                              |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F017                                                                                                                                               |
| Function Name    | IdentityService.IsEmailConfirmedAsync                                                                                                              |
| Total Test Cases | 3                                                                                                                                                  |
| Created By       |                                                                                                                                                    |
| Executed By      |                                                                                                                                                    |
| Lines of Code    | 1054 |
| Passed           | 3                                                                                                                                                  |
| Failed           | 0                                                                                                                                                  |
| Untested         | 0                                                                                                                                                  |
| Count type N     | 1                                                                                                                                                  |
| Count type A     | 2                                                                                                                                                  |
| Count type B     | 0                                                                                                                                                  |
| Test Requirement | Validate 'Check email confirmed' in IdentityService.IsEmailConfirmedAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                        | Precondition                             | UTCIDs  |
| ------------------------------------------------ | ---------------------------------------- | ------- |
| User null → ném `ArgumentNullException`.         | Truyền user = null.                      | UTCID01 |
| User có EmailConfirmed = true → trả về `true`.   | IsEmailConfirmedAsync mock trả về true.  | UTCID02 |
| User có EmailConfirmed = false → trả về `false`. | IsEmailConfirmedAsync mock trả về false. | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------- | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw           | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | `true`          | –                       | –                    | P             |               |           |
| UTCID03 | A            | `false`         | –                       | –                    | P             |               |           |

---

## F018 - IdentityService.IsUserActiveAsync

| Header           | Value                                                                                                                                      |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F018                                                                                                                                       |
| Function Name    | IdentityService.IsUserActiveAsync                                                                                                          |
| Total Test Cases | 3                                                                                                                                          |
| Created By       |                                                                                                                                            |
| Executed By      |                                                                                                                                            |
| Lines of Code    | 1054 |
| Passed           | 3                                                                                                                                          |
| Failed           | 0                                                                                                                                          |
| Untested         | 0                                                                                                                                          |
| Count type N     | 1                                                                                                                                          |
| Count type A     | 2                                                                                                                                          |
| Count type B     | 0                                                                                                                                          |
| Test Requirement | Validate 'Check user active' in IdentityService.IsUserActiveAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                        | Precondition                                      | UTCIDs  |
| ---------------------------------------------------------------- | ------------------------------------------------- | ------- |
| User null → ném `ArgumentNullException`.                         | Truyền user = null.                               | UTCID01 |
| User có IsActive = true và IsDeleted = false → trả về `true`.    | User seed với IsActive=true, IsDeleted=false.     | UTCID02 |
| User có IsActive = false hoặc IsDeleted = true → trả về `false`. | User seed với IsActive=false hoặc IsDeleted=true. | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------- | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw           | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | `true`          | –                       | –                    | P             |               |           |
| UTCID03 | A            | `false`         | –                       | –                    | P             |               |           |

---

## F019 - IdentityService.GenerateEmailConfirmationTokenAsync

| Header           | Value                                                                                                                                                                        |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F019                                                                                                                                                                         |
| Function Name    | IdentityService.GenerateEmailConfirmationTokenAsync                                                                                                                          |
| Total Test Cases | 2                                                                                                                                                                            |
| Created By       |                                                                                                                                                                              |
| Executed By      |                                                                                                                                                                              |
| Lines of Code    | 1054 |
| Passed           | 2                                                                                                                                                                            |
| Failed           | 0                                                                                                                                                                            |
| Untested         | 0                                                                                                                                                                            |
| Count type N     | 1                                                                                                                                                                            |
| Count type A     | 1                                                                                                                                                                            |
| Count type B     | 0                                                                                                                                                                            |
| Test Requirement | Validate 'Generate email confirmation token' in IdentityService.GenerateEmailConfirmationTokenAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                | Precondition                                                        | UTCIDs  |
| ------------------------------------------------------------------------ | ------------------------------------------------------------------- | ------- |
| User null → ném `ArgumentNullException`.                                 | Truyền user = null.                                                 | UTCID01 |
| User hợp lệ → trả về token string không rỗng (generated by UserManager). | GenerateEmailConfirmationTokenAsync mock trả về chuỗi token hợp lệ. | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return               | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ----------------------------- | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw                         | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | token string không rỗng (≠"") | –                       | –                    | P             |               |           |

---

## F020 - IdentityService.GeneratePasswordResetTokenAsync

| Header           | Value                                                                                                                                                                |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F020                                                                                                                                                                 |
| Function Name    | IdentityService.GeneratePasswordResetTokenAsync                                                                                                                      |
| Total Test Cases | 2                                                                                                                                                                    |
| Created By       |                                                                                                                                                                      |
| Executed By      |                                                                                                                                                                      |
| Lines of Code    | 1054 |
| Passed           | 2                                                                                                                                                                    |
| Failed           | 0                                                                                                                                                                    |
| Untested         | 0                                                                                                                                                                    |
| Count type N     | 1                                                                                                                                                                    |
| Count type A     | 1                                                                                                                                                                    |
| Count type B     | 0                                                                                                                                                                    |
| Test Requirement | Validate 'Generate password reset token' in IdentityService.GeneratePasswordResetTokenAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                | Precondition                                                    | UTCIDs  |
| ------------------------------------------------------------------------ | --------------------------------------------------------------- | ------- |
| User null → ném `ArgumentNullException`.                                 | Truyền user = null.                                             | UTCID01 |
| User hợp lệ → trả về token string không rỗng (generated by UserManager). | GeneratePasswordResetTokenAsync mock trả về chuỗi token hợp lệ. | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return               | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ----------------------------- | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw                         | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | token string không rỗng (≠"") | –                       | –                    | P             |               |           |

---

## F021 - IdentityService.GetUserRolesAsync

| Header           | Value                                                                                                                                   |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F021                                                                                                                                    |
| Function Name    | IdentityService.GetUserRolesAsync                                                                                                       |
| Total Test Cases | 3                                                                                                                                       |
| Created By       |                                                                                                                                         |
| Executed By      |                                                                                                                                         |
| Lines of Code    | 1054 |
| Passed           | 3                                                                                                                                       |
| Failed           | 0                                                                                                                                       |
| Untested         | 0                                                                                                                                       |
| Count type N     | 1                                                                                                                                       |
| Count type A     | 2                                                                                                                                       |
| Count type B     | 0                                                                                                                                       |
| Test Requirement | Validate 'Get user roles' in IdentityService.GetUserRolesAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                               | Precondition                                          | UTCIDs  |
| ----------------------------------------------------------------------- | ----------------------------------------------------- | ------- |
| User null → ném `ArgumentNullException`.                                | Truyền user = null.                                   | UTCID01 |
| User có roles (vd. ["Patient", "Admin"]) → trả về danh sách roles đúng. | GetRolesAsync mock trả về IList {"Patient", "Admin"}. | UTCID02 |
| User không có role nào → trả về danh sách rỗng.                         | GetRolesAsync mock trả về IList rỗng.                 | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                      | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------ | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw                                | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | `IList<string>` chứa roles chính xác | –                       | –                    | P             |               |           |
| UTCID03 | A            | `IList<string>` rỗng                 | –                       | –                    | P             |               |           |

---

## F022 - IdentityService.IsInRoleAsync

| Header           | Value                                                                                                                                   |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F022                                                                                                                                    |
| Function Name    | IdentityService.IsInRoleAsync                                                                                                           |
| Total Test Cases | 3                                                                                                                                       |
| Created By       |                                                                                                                                         |
| Executed By      |                                                                                                                                         |
| Lines of Code    | 1054 |
| Passed           | 3                                                                                                                                       |
| Failed           | 0                                                                                                                                       |
| Untested         | 0                                                                                                                                       |
| Count type N     | 1                                                                                                                                       |
| Count type A     | 2                                                                                                                                       |
| Count type B     | 0                                                                                                                                       |
| Test Requirement | Validate 'Check user in role' in IdentityService.IsInRoleAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                             | Precondition                                              | UTCIDs  |
| ----------------------------------------------------- | --------------------------------------------------------- | ------- |
| User null → ném `ArgumentNullException`.              | Truyền user = null.                                       | UTCID01 |
| User thuộc role được kiểm tra → trả về `true`.        | IsInRoleAsync mock trả về true với role name khớp.        | UTCID02 |
| User không thuộc role được kiểm tra → trả về `false`. | IsInRoleAsync mock trả về false với role name không khớp. | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------- | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw           | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | `true`          | –                       | –                    | P             |               |           |
| UTCID03 | A            | `false`         | –                       | –                    | P             |               |           |

---

## F023 - IdentityService.UpdateLastLoginAsync

| Header           | Value                                                                                                                                               |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F023                                                                                                                                                |
| Function Name    | IdentityService.UpdateLastLoginAsync                                                                                                                |
| Total Test Cases | 2                                                                                                                                                   |
| Created By       |                                                                                                                                                     |
| Executed By      |                                                                                                                                                     |
| Lines of Code    | 1054 |
| Passed           | 2                                                                                                                                                   |
| Failed           | 0                                                                                                                                                   |
| Untested         | 0                                                                                                                                                   |
| Count type N     | 1                                                                                                                                                   |
| Count type A     | 1                                                                                                                                                   |
| Count type B     | 0                                                                                                                                                   |
| Test Requirement | Validate 'Update last login async' in IdentityService.UpdateLastLoginAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                     | Precondition                                                                | UTCIDs  |
| --------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------- | ------- |
| User null → ném `ArgumentNullException`.                                                      | Truyền user = null.                                                         | UTCID01 |
| User hợp lệ → cập nhật LastLoginAt = UtcNow, gọi UpdateAsync, trả về (không có return value). | UpdateAsync mock thành công; user seed hợp lệ; DateTimeService mock UtcNow. | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                         | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------------- | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw                                                   | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | void (UpdateAsync gọi đúng 1 lần; LastLoginAt được set) | –                       | –                    | P             |               |           |

---

## F024 - IdentityService.IsTwoFactorEnabledAsync

| Header           | Value                                                                                                                                            |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F024                                                                                                                                             |
| Function Name    | IdentityService.IsTwoFactorEnabledAsync                                                                                                          |
| Total Test Cases | 3                                                                                                                                                |
| Created By       |                                                                                                                                                  |
| Executed By      |                                                                                                                                                  |
| Lines of Code    | 1054 |
| Passed           | 3                                                                                                                                                |
| Failed           | 0                                                                                                                                                |
| Untested         | 0                                                                                                                                                |
| Count type N     | 1                                                                                                                                                |
| Count type A     | 2                                                                                                                                                |
| Count type B     | 0                                                                                                                                                |
| Test Requirement | Validate 'Check 2FA enabled' in IdentityService.IsTwoFactorEnabledAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                         | Precondition                                             | UTCIDs  |
| ----------------------------------------------------------------- | -------------------------------------------------------- | ------- |
| User null → ném `ArgumentNullException`.                          | Truyền user = null.                                      | UTCID01 |
| 2FA đang bật (GetTwoFactorEnabledAsync = true) → trả về `true`.   | GetTwoFactorEnabledAsync mock trả về true; user hợp lệ.  | UTCID02 |
| 2FA đang tắt (GetTwoFactorEnabledAsync = false) → trả về `false`. | GetTwoFactorEnabledAsync mock trả về false; user hợp lệ. | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------- | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw           | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | `true`          | –                       | –                    | P             |               |           |
| UTCID03 | A            | `false`         | –                       | –                    | P             |               |           |

---

## F025 - IdentityService.GetAuthenticatorKeyAsync

| Header           | Value                                                                                                                                                 |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F025                                                                                                                                                  |
| Function Name    | IdentityService.GetAuthenticatorKeyAsync                                                                                                              |
| Total Test Cases | 2                                                                                                                                                     |
| Created By       |                                                                                                                                                       |
| Executed By      |                                                                                                                                                       |
| Lines of Code    | 1054 |
| Passed           | 2                                                                                                                                                     |
| Failed           | 0                                                                                                                                                     |
| Untested         | 0                                                                                                                                                     |
| Count type N     | 1                                                                                                                                                     |
| Count type A     | 1                                                                                                                                                     |
| Count type B     | 0                                                                                                                                                     |
| Test Requirement | Validate 'Get authenticator key' in IdentityService.GetAuthenticatorKeyAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                              | Precondition                                                           | UTCIDs  |
| -------------------------------------------------------------------------------------- | ---------------------------------------------------------------------- | ------- |
| User null → ném `ArgumentNullException`.                                               | Truyền user = null.                                                    | UTCID01 |
| User hợp lệ, đã có authenticator key → trả về key string (có thể null nếu chưa setup). | GetAuthenticatorKeyAsync mock trả về chuỗi key hoặc null; user hợp lệ. | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                             | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------- | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw                                       | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | authenticator key string (or null if unset) | –                       | –                    | P             |               |           |

---

## F026 - IdentityService.GetOrCreateAuthenticatorKeyAsync

| Header           | Value                                                                                                                                                                   |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F026                                                                                                                                                                    |
| Function Name    | IdentityService.GetOrCreateAuthenticatorKeyAsync                                                                                                                        |
| Total Test Cases | 2                                                                                                                                                                       |
| Created By       |                                                                                                                                                                         |
| Executed By      |                                                                                                                                                                         |
| Lines of Code    | 1054 |
| Passed           | 2                                                                                                                                                                       |
| Failed           | 0                                                                                                                                                                       |
| Untested         | 0                                                                                                                                                                       |
| Count type N     | 1                                                                                                                                                                       |
| Count type A     | 1                                                                                                                                                                       |
| Count type B     | 0                                                                                                                                                                       |
| Test Requirement | Validate 'Get or create authenticator key' in IdentityService.GetOrCreateAuthenticatorKeyAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                          | Precondition                                                                                 | UTCIDs  |
| ------------------------------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------------- | ------- |
| User null → ném `ArgumentNullException`.                                                                           | Truyền user = null.                                                                          | UTCID01 |
| User hợp lệ, chưa có key → ResetAuthenticatorKeyAsync được gọi, sau đó trả về key mới từ GetAuthenticatorKeyAsync. | GetAuthenticatorKeyAsync trả về null lần đầu; sau ResetAuthenticatorKeyAsync trả về key mới. | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                             | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------- | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw                                       | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | key string không rỗng (created or existing) | –                       | –                    | P             |               |           |

---

## F027 - IdentityService.VerifyTwoFactorCodeAsync

| Header           | Value                                                                                                                                           |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F027                                                                                                                                            |
| Function Name    | IdentityService.VerifyTwoFactorCodeAsync                                                                                                        |
| Total Test Cases | 3                                                                                                                                               |
| Created By       |                                                                                                                                                 |
| Executed By      |                                                                                                                                                 |
| Lines of Code    | 1054 |
| Passed           | 3                                                                                                                                               |
| Failed           | 0                                                                                                                                               |
| Untested         | 0                                                                                                                                               |
| Count type N     | 1                                                                                                                                               |
| Count type A     | 2                                                                                                                                               |
| Count type B     | 0                                                                                                                                               |
| Test Requirement | Validate 'Verify 2FA code' in IdentityService.VerifyTwoFactorCodeAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                              | Precondition                                                                   | UTCIDs  |
| ---------------------------------------------------------------------- | ------------------------------------------------------------------------------ | ------- |
| User null → ném `ArgumentNullException`.                               | Truyền user = null.                                                            | UTCID01 |
| Mã TOTP đúng → VerifyTwoFactorTokenAsync trả về true → trả về `true`.  | VerifyTwoFactorTokenAsync mock trả về true với token provider "Authenticator". | UTCID02 |
| Mã TOTP sai → VerifyTwoFactorTokenAsync trả về false → trả về `false`. | VerifyTwoFactorTokenAsync mock trả về false.                                   | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------- | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw           | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | `true`          | –                       | –                    | P             |               |           |
| UTCID03 | A            | `false`         | –                       | –                    | P             |               |           |

---

## F028 - IdentityService.GenerateNewRecoveryCodesAsync

| Header           | Value                                                                                                                                                        |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F028                                                                                                                                                         |
| Function Name    | IdentityService.GenerateNewRecoveryCodesAsync                                                                                                                |
| Total Test Cases | 2                                                                                                                                                            |
| Created By       |                                                                                                                                                              |
| Executed By      |                                                                                                                                                              |
| Lines of Code    | 1054 |
| Passed           | 2                                                                                                                                                            |
| Failed           | 0                                                                                                                                                            |
| Untested         | 0                                                                                                                                                            |
| Count type N     | 1                                                                                                                                                            |
| Count type A     | 1                                                                                                                                                            |
| Count type B     | 0                                                                                                                                                            |
| Test Requirement | Validate 'Generate recovery codes' in IdentityService.GenerateNewRecoveryCodesAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                        | Precondition                                                                 | UTCIDs  |
| ---------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------- | ------- |
| User null → ném `ArgumentNullException`.                                                                         | Truyền user = null.                                                          | UTCID01 |
| User hợp lệ → GenerateNewTwoFactorRecoveryCodesAsync trả về danh sách codes, hàm wrapper trả về chính danh sách. | GenerateNewTwoFactorRecoveryCodesAsync mock trả về IEnumerable với 10 codes. | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                             | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------- | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw                                       | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | `IEnumerable<string>` với 10 recovery codes | –                       | –                    | P             |               |           |

---

## F029 - IdentityService.GetRecoveryCodesCountAsync

| Header           | Value                                                                                                                                                     |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F029                                                                                                                                                      |
| Function Name    | IdentityService.GetRecoveryCodesCountAsync                                                                                                                |
| Total Test Cases | 2                                                                                                                                                         |
| Created By       |                                                                                                                                                           |
| Executed By      |                                                                                                                                                           |
| Lines of Code    | 1054 |
| Passed           | 2                                                                                                                                                         |
| Failed           | 0                                                                                                                                                         |
| Untested         | 0                                                                                                                                                         |
| Count type N     | 1                                                                                                                                                         |
| Count type A     | 1                                                                                                                                                         |
| Count type B     | 0                                                                                                                                                         |
| Test Requirement | Validate 'Get recovery code count' in IdentityService.GetRecoveryCodesCountAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                           | Precondition                                                   | UTCIDs  |
| ----------------------------------------------------------------------------------- | -------------------------------------------------------------- | ------- |
| User null → ném `ArgumentNullException`.                                            | Truyền user = null.                                            | UTCID01 |
| User hợp lệ → CountRecoveryCodesAsync trả về số codes còn lại → trả về `int` count. | CountRecoveryCodesAsync mock trả về 5 (còn 5 codes chưa dùng). | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return               | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ----------------------------- | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw                         | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | `int` = số codes còn lại (≥0) | –                       | –                    | P             |               |           |

---

## F030 - IdentityService.GenerateAuthenticatorUri

| Header           | Value                                                                                                                                                      |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F030                                                                                                                                                       |
| Function Name    | IdentityService.GenerateAuthenticatorUri                                                                                                                   |
| Total Test Cases | 2                                                                                                                                                          |
| Created By       |                                                                                                                                                            |
| Executed By      |                                                                                                                                                            |
| Lines of Code    | 1054 |
| Passed           | 2                                                                                                                                                          |
| Failed           | 0                                                                                                                                                          |
| Untested         | 0                                                                                                                                                          |
| Count type N     | 1                                                                                                                                                          |
| Count type A     | 1                                                                                                                                                          |
| Count type B     | 0                                                                                                                                                          |
| Test Requirement | Validate 'Generate authenticator URI' in IdentityService.GenerateAuthenticatorUri, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                     | Precondition                                                          | UTCIDs  |
| ----------------------------------------------------------------------------- | --------------------------------------------------------------------- | ------- |
| Email hoặc unformattedKey null/rỗng → ném `ArgumentException`.                | Truyền email = null hoặc unformattedKey = "".                         | UTCID01 |
| Email và key hợp lệ → trả về URI dạng `otpauth://totp/...` chứa email và key. | email = "user@test.com"; unformattedKey = "ABCD1234"; không cần mock. | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                          | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | -------------------------------------------------------- | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw                                                    | `ArgumentException` | –                    | P             |               |           |
| UTCID02 | N            | URI string bắt đầu bằng `otpauth://totp/` chứa email/key | –                   | –                    | P             |               |           |

---

## F031 - IdentityService.FormatAuthenticatorKey

| Header           | Value                                                                                                                                                  |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F031                                                                                                                                                   |
| Function Name    | IdentityService.FormatAuthenticatorKey                                                                                                                 |
| Total Test Cases | 2                                                                                                                                                      |
| Created By       |                                                                                                                                                        |
| Executed By      |                                                                                                                                                        |
| Lines of Code    | 1054 |
| Passed           | 2                                                                                                                                                      |
| Failed           | 0                                                                                                                                                      |
| Untested         | 0                                                                                                                                                      |
| Count type N     | 1                                                                                                                                                      |
| Count type A     | 1                                                                                                                                                      |
| Count type B     | 0                                                                                                                                                      |
| Test Requirement | Validate 'Format authenticator key' in IdentityService.FormatAuthenticatorKey, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                       | Precondition                                               | UTCIDs  |
| --------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- | ------- |
| unformattedKey null/rỗng → ném `ArgumentException` hoặc trả về empty string.                                    | unformattedKey = null/"".                                  | UTCID01 |
| unformattedKey hợp lệ (vd. "abcde12345fghij") → trả về key được format thành nhóm 4 ký tự cách nhau bằng space. | unformattedKey = "ABCDE12345FGHIJ" (uppercase, no spaces). | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                     | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------------------------------------------- | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw hoặc empty string                             | `ArgumentException` | –                    | P             |               |           |
| UTCID02 | N            | formatted string dạng "ABCD E123 45FG HIJ" (nhóm 4) | –                   | –                    | P             |               |           |

---

## F032 - IdentityService.GetUserMetricsAsync

| Header           | Value                                                                                                                                       |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F032                                                                                                                                        |
| Function Name    | IdentityService.GetUserMetricsAsync                                                                                                         |
| Total Test Cases | 3                                                                                                                                           |
| Created By       |                                                                                                                                             |
| Executed By      |                                                                                                                                             |
| Lines of Code    | 1054 |
| Passed           | 3                                                                                                                                           |
| Failed           | 0                                                                                                                                           |
| Untested         | 0                                                                                                                                           |
| Count type N     | 1                                                                                                                                           |
| Count type A     | 2                                                                                                                                           |
| Count type B     | 0                                                                                                                                           |
| Test Requirement | Validate 'Get user metrics' in IdentityService.GetUserMetricsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                              | Precondition                                                                                  | UTCIDs  |
| ---------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------- | ------- |
| DB có dữ liệu users đầy đủ → trả về UserMetrics với TotalUsers, ActiveUsers, NewUsersThisMonth, PendingApprovals đúng. | UserManager.Users mock/seed có 10 users; 7 active; 3 created trong tháng; 2 pending approval. | UTCID01 |
| DB rỗng (không có user) → trả về UserMetrics với tất cả count = 0.                                                     | UserManager.Users mock trả về danh sách rỗng.                                                 | UTCID02 |
| GetUsersInRoleAsync ném exception → exception được propagate hoặc trả về partial metrics.                              | GetUsersInRoleAsync mock ném InvalidOperationException.                                       | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                                | Expected exception          | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | -------------------------------------------------------------- | --------------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `UserMetrics` với TotalUsers=10, ActiveUsers=7, NewThisMonth=3 | –                           | –                    | P             |               |           |
| UTCID02 | A            | `UserMetrics` với tất cả count = 0                             | –                           | –                    | P             |               |           |
| UTCID03 | A            | throw hoặc partial metrics                                     | `InvalidOperationException` | –                    | P             |               |           |

---

## F033 - IdentityService.GetUsersInRoleCountAsync

| Header           | Value                                                                                                                                                   |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F033                                                                                                                                                    |
| Function Name    | IdentityService.GetUsersInRoleCountAsync                                                                                                                |
| Total Test Cases | 3                                                                                                                                                       |
| Created By       |                                                                                                                                                         |
| Executed By      |                                                                                                                                                         |
| Lines of Code    | 1054 |
| Passed           | 3                                                                                                                                                       |
| Failed           | 0                                                                                                                                                       |
| Untested         | 0                                                                                                                                                       |
| Count type N     | 1                                                                                                                                                       |
| Count type A     | 2                                                                                                                                                       |
| Count type B     | 0                                                                                                                                                       |
| Test Requirement | Validate 'Get users in role count' in IdentityService.GetUsersInRoleCountAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                               | Precondition                                                          | UTCIDs  |
| --------------------------------------------------------------------------------------- | --------------------------------------------------------------------- | ------- |
| Role hợp lệ, có users → GetUsersInRoleAsync trả về danh sách users → trả về count đúng. | GetUsersInRoleAsync mock trả về IList với 3 users cho role "Patient". | UTCID01 |
| Role hợp lệ, không có users → GetUsersInRoleAsync trả về danh sách rỗng → trả về 0.     | GetUsersInRoleAsync mock trả về IList rỗng.                           | UTCID02 |
| roleName null/rỗng → ném `ArgumentException` hoặc trả về 0.                             | roleName = null/"".                                                   | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------- | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `int` = 3       | –                   | –                    | P             |               |           |
| UTCID02 | A            | `int` = 0       | –                   | –                    | P             |               |           |
| UTCID03 | A            | throw hoặc 0    | `ArgumentException` | –                    | P             |               |           |

---

## F034 - IdentityService.GetPendingApprovalsCountAsync

| Header           | Value                                                                                                                                                            |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F034                                                                                                                                                             |
| Function Name    | IdentityService.GetPendingApprovalsCountAsync                                                                                                                    |
| Total Test Cases | 2                                                                                                                                                                |
| Created By       |                                                                                                                                                                  |
| Executed By      |                                                                                                                                                                  |
| Lines of Code    | 1054 |
| Passed           | 2                                                                                                                                                                |
| Failed           | 0                                                                                                                                                                |
| Untested         | 0                                                                                                                                                                |
| Count type N     | 1                                                                                                                                                                |
| Count type A     | 1                                                                                                                                                                |
| Count type B     | 0                                                                                                                                                                |
| Test Requirement | Validate 'Get pending approvals count' in IdentityService.GetPendingApprovalsCountAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                  | Precondition                                                               | UTCIDs  |
| ------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------- | ------- |
| Có users với IsActive=false và EmailConfirmed=true (pending approval) → trả về count đúng. | UserManager.Users seed với 2 users có IsActive=false, EmailConfirmed=true. | UTCID01 |
| Không có pending approvals → trả về 0.                                                     | UserManager.Users seed với tất cả users active hoặc email không confirmed. | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return     | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `int` = 2 (pending) | –                  | –                    | P             |               |           |
| UTCID02 | A            | `int` = 0           | –                  | –                    | P             |               |           |

---

## F035 - IdentityService.GetUserDetailsAsync

| Header           | Value                                                                                                                                       |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F035                                                                                                                                        |
| Function Name    | IdentityService.GetUserDetailsAsync                                                                                                         |
| Total Test Cases | 3                                                                                                                                           |
| Created By       |                                                                                                                                             |
| Executed By      |                                                                                                                                             |
| Lines of Code    | 1054 |
| Passed           | 3                                                                                                                                           |
| Failed           | 0                                                                                                                                           |
| Untested         | 0                                                                                                                                           |
| Count type N     | 1                                                                                                                                           |
| Count type A     | 2                                                                                                                                           |
| Count type B     | 0                                                                                                                                           |
| Test Requirement | Validate 'Get user details' in IdentityService.GetUserDetailsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                      | Precondition                                                                              | UTCIDs  |
| -------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------- | ------- |
| UserId null/rỗng → ném `ArgumentException`.                                                                    | userId = null/"".                                                                         | UTCID01 |
| UserId hợp lệ, không tìm thấy user → trả về null.                                                              | FindByIdAsync trả về null.                                                                | UTCID02 |
| UserId hợp lệ, tìm thấy user → trả về UserDetailsDto đầy đủ (Id, Email, FullName, roles, LastLogin, IsActive). | FindByIdAsync trả về user hợp lệ; GetRolesAsync trả về ["Patient"]; DateTimeService mock. | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                        | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------------ | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw                                                  | `ArgumentException` | –                    | P             |               |           |
| UTCID02 | A            | `null`                                                 | –                   | –                    | P             |               |           |
| UTCID03 | N            | `UserDetailsDto` với Id, Email, FullName, Roles đầy đủ | –                   | –                    | P             |               |           |

---

## F036 - RefreshTokenService.CreateRefreshTokenAsync

| Header           | Value                                                                                                                                                          |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F036                                                                                                                                                           |
| Function Name    | RefreshTokenService.CreateRefreshTokenAsync                                                                                                                    |
| Total Test Cases | 3                                                                                                                                                              |
| Created By       |                                                                                                                                                                |
| Executed By      |                                                                                                                                                                |
| Lines of Code    | 167 |
| Passed           | 3                                                                                                                                                              |
| Failed           | 0                                                                                                                                                              |
| Untested         | 0                                                                                                                                                              |
| Count type N     | 1                                                                                                                                                              |
| Count type A     | 2                                                                                                                                                              |
| Count type B     | 0                                                                                                                                                              |
| Test Requirement | Validate 'Create refresh token record' in RefreshTokenService.CreateRefreshTokenAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                        | Precondition                                                                        | UTCIDs  |
| ------------------------------------------------------------------------------------------------ | ----------------------------------------------------------------------------------- | ------- |
| Token hash, jwtId, userId hợp lệ → tạo record RefreshToken, gọi AddAsync → trả về entity đã tạo. | Repository.AddAsync mock thành công; DateTimeService mock UtcNow; expiresAt hợp lệ. | UTCID01 |
| Token hash null/rỗng → ném `ArgumentException`.                                                  | tokenHash = null/"".                                                                | UTCID02 |
| Repository.AddAsync ném exception → exception được propagate.                                    | AddAsync mock ném DbUpdateException.                                                | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                         | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------------- | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `RefreshToken` entity với TokenHash, JwtId, UserId đúng | –                   | –                    | P             |               |           |
| UTCID02 | A            | throw                                                   | `ArgumentException` | –                    | P             |               |           |
| UTCID03 | A            | throw                                                   | `DbUpdateException` | –                    | P             |               |           |

---

## F037 - RefreshTokenService.GetByTokenHashAsync

| Header           | Value                                                                                                                                                    |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F037                                                                                                                                                     |
| Function Name    | RefreshTokenService.GetByTokenHashAsync                                                                                                                  |
| Total Test Cases | 3                                                                                                                                                        |
| Created By       |                                                                                                                                                          |
| Executed By      |                                                                                                                                                          |
| Lines of Code    | 167 |
| Passed           | 3                                                                                                                                                        |
| Failed           | 0                                                                                                                                                        |
| Untested         | 0                                                                                                                                                        |
| Count type N     | 1                                                                                                                                                        |
| Count type A     | 2                                                                                                                                                        |
| Count type B     | 0                                                                                                                                                        |
| Test Requirement | Validate 'Get refresh token by hash' in RefreshTokenService.GetByTokenHashAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                     | Precondition                                                      | UTCIDs  |
| ----------------------------------------------------------------------------- | ----------------------------------------------------------------- | ------- |
| tokenHash null/rỗng → ném `ArgumentException`.                                | tokenHash = null/"".                                              | UTCID01 |
| tokenHash hợp lệ, tìm thấy trong DB → trả về `RefreshToken` entity khớp hash. | Repository.FindAsync/FirstOrDefaultAsync trả về entity khớp hash. | UTCID02 |
| tokenHash hợp lệ, không tìm thấy → trả về `null`.                             | Repository.FindAsync/FirstOrDefaultAsync trả về null.             | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return       | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------------- | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw                 | `ArgumentException` | –                    | P             |               |           |
| UTCID02 | N            | `RefreshToken` entity | –                   | –                    | P             |               |           |
| UTCID03 | A            | `null`                | –                   | –                    | P             |               |           |

---

## F038 - RefreshTokenService.RotateRefreshTokenAsync

| Header           | Value                                                                                                                                                   |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F038                                                                                                                                                    |
| Function Name    | RefreshTokenService.RotateRefreshTokenAsync                                                                                                             |
| Total Test Cases | 3                                                                                                                                                       |
| Created By       |                                                                                                                                                         |
| Executed By      |                                                                                                                                                         |
| Lines of Code    | 167 |
| Passed           | 3                                                                                                                                                       |
| Failed           | 0                                                                                                                                                       |
| Untested         | 0                                                                                                                                                       |
| Count type N     | 1                                                                                                                                                       |
| Count type A     | 2                                                                                                                                                       |
| Count type B     | 0                                                                                                                                                       |
| Test Requirement | Validate 'Rotate refresh token' in RefreshTokenService.RotateRefreshTokenAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                                | Precondition                                                                 | UTCIDs  |
| ---------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------- | ------- |
| oldToken null → ném `ArgumentNullException`.                                                                                             | oldToken = null.                                                             | UTCID01 |
| oldToken hợp lệ → revoke old token (IsRevoked=true, RevokedAt=now), tạo new token với newHash/newJwtId → trả về new RefreshToken entity. | oldToken seed hợp lệ; Repository.AddAsync mock; DateTimeService mock UtcNow. | UTCID02 |
| Repository.AddAsync ném exception → exception propagate.                                                                                 | AddAsync mock ném DbUpdateException.                                         | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                      | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ---------------------------------------------------- | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw                                                | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | new `RefreshToken` entity; oldToken.IsRevoked = true | –                       | –                    | P             |               |           |
| UTCID03 | A            | throw                                                | `DbUpdateException`     | –                    | P             |               |           |

---

## F039 - RefreshTokenService.RevokeTokenAsync

| Header           | Value                                                                                                                                            |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F039                                                                                                                                             |
| Function Name    | RefreshTokenService.RevokeTokenAsync                                                                                                             |
| Total Test Cases | 2                                                                                                                                                |
| Created By       |                                                                                                                                                  |
| Executed By      |                                                                                                                                                  |
| Lines of Code    | 167 |
| Passed           | 2                                                                                                                                                |
| Failed           | 0                                                                                                                                                |
| Untested         | 0                                                                                                                                                |
| Count type N     | 1                                                                                                                                                |
| Count type A     | 1                                                                                                                                                |
| Count type B     | 0                                                                                                                                                |
| Test Requirement | Validate 'Revoke token by hash' in RefreshTokenService.RevokeTokenAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                             | Precondition                                                                     | UTCIDs  |
| ------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------- | ------- |
| token null → ném `ArgumentNullException`.                                             | Truyền token = null.                                                             | UTCID01 |
| token hợp lệ → set IsRevoked=true, RevokedAt=UtcNow; UpdateAsync được gọi đúng 1 lần. | token seed hợp lệ (IsRevoked=false); Repository.UpdateAsync mock; DateTime mock. | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                        | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------------ | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw                                                  | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | N            | void; token.IsRevoked=true; UpdateAsync gọi đúng 1 lần | –                       | –                    | P             |               |           |

---

## F040 - RefreshTokenService.RevokeAllUserTokensAsync

| Header           | Value                                                                                                                                                      |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F040                                                                                                                                                       |
| Function Name    | RefreshTokenService.RevokeAllUserTokensAsync                                                                                                               |
| Total Test Cases | 2                                                                                                                                                          |
| Created By       |                                                                                                                                                            |
| Executed By      |                                                                                                                                                            |
| Lines of Code    | 167 |
| Passed           | 2                                                                                                                                                          |
| Failed           | 0                                                                                                                                                          |
| Untested         | 0                                                                                                                                                          |
| Count type N     | 1                                                                                                                                                          |
| Count type A     | 1                                                                                                                                                          |
| Count type B     | 0                                                                                                                                                          |
| Test Requirement | Validate 'Revoke all user tokens' in RefreshTokenService.RevokeAllUserTokensAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                              | Precondition                                                                                  | UTCIDs  |
| ---------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------- | ------- |
| userId hợp lệ, có tokens active → tất cả tokens của user bị set IsRevoked=true, UpdateRangeAsync/bulk update được gọi. | Repository.GetAllByUserIdAsync trả về 3 active tokens; UpdateRangeAsync/bulk mock thành công. | UTCID01 |
| userId không có token nào active → không có update nào được thực hiện, hàm kết thúc bình thường.                       | Repository.GetAllByUserIdAsync trả về danh sách rỗng.                                         | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                            | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ---------------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; tất cả 3 tokens.IsRevoked=true; bulk update được gọi | –                  | –                    | P             |               |           |
| UTCID02 | A            | void; không có update nào                                  | –                  | –                    | P             |               |           |

---

## F041 - RefreshTokenService.RevokeTokenFamilyAsync

| Header           | Value                                                                                                                                                 |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F041                                                                                                                                                  |
| Function Name    | RefreshTokenService.RevokeTokenFamilyAsync                                                                                                            |
| Total Test Cases | 2                                                                                                                                                     |
| Created By       |                                                                                                                                                       |
| Executed By      |                                                                                                                                                       |
| Lines of Code    | 167 |
| Passed           | 2                                                                                                                                                     |
| Failed           | 0                                                                                                                                                     |
| Untested         | 0                                                                                                                                                     |
| Count type N     | 1                                                                                                                                                     |
| Count type A     | 1                                                                                                                                                     |
| Count type B     | 0                                                                                                                                                     |
| Test Requirement | Validate 'Revoke token family' in RefreshTokenService.RevokeTokenFamilyAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                     | Precondition                                                                            | UTCIDs  |
| ----------------------------------------------------------------------------- | --------------------------------------------------------------------------------------- | ------- |
| familyId hợp lệ, có tokens trong family → tất cả tokens của family bị revoke. | Repository.GetByFamilyIdAsync trả về 2 tokens cùng family; bulk revoke mock thành công. | UTCID01 |
| familyId không có token nào → không có update, kết thúc bình thường.          | Repository.GetByFamilyIdAsync trả về danh sách rỗng.                                    | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                 | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ----------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; tất cả tokens trong family.IsRevoked=true | –                  | –                    | P             |               |           |
| UTCID02 | A            | void; không có update nào                       | –                  | –                    | P             |               |           |

---

## F042 - RefreshTokenService.CleanupExpiredTokensAsync

| Header           | Value                                                                                                                                                       |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F042                                                                                                                                                        |
| Function Name    | RefreshTokenService.CleanupExpiredTokensAsync                                                                                                               |
| Total Test Cases | 3                                                                                                                                                           |
| Created By       |                                                                                                                                                             |
| Executed By      |                                                                                                                                                             |
| Lines of Code    | 167 |
| Passed           | 3                                                                                                                                                           |
| Failed           | 0                                                                                                                                                           |
| Untested         | 0                                                                                                                                                           |
| Count type N     | 1                                                                                                                                                           |
| Count type A     | 2                                                                                                                                                           |
| Count type B     | 0                                                                                                                                                           |
| Test Requirement | Validate 'Cleanup expired tokens' in RefreshTokenService.CleanupExpiredTokensAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                             | Precondition                                                                                       | UTCIDs  |
| --------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------- | ------- |
| Có tokens hết hạn → xóa chúng khỏi DB, trả về số lượng tokens đã xóa. | Repository seed với 3 tokens hết hạn (ExpiresAt < UtcNow); RemoveRangeAsync/DeleteRangeAsync mock. | UTCID01 |
| Không có token hết hạn → trả về 0 tokens xóa.                         | Repository seed với 0 tokens hết hạn.                                                              | UTCID02 |
| Repository.RemoveRangeAsync ném exception → exception được propagate. | RemoveRangeAsync mock ném DbUpdateException.                                                       | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return              | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ---------------------------- | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `int` = 3 (số tokens đã xóa) | –                   | –                    | P             |               |           |
| UTCID02 | A            | `int` = 0                    | –                   | –                    | P             |               |           |
| UTCID03 | A            | throw                        | `DbUpdateException` | –                    | P             |               |           |

---

## F043 - TokenService.GenerateAccessTokenAsync

| Header           | Value                                                                                                                                              |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F043                                                                                                                                               |
| Function Name    | TokenService.GenerateAccessTokenAsync                                                                                                              |
| Total Test Cases | 4                                                                                                                                                  |
| Created By       |                                                                                                                                                    |
| Executed By      |                                                                                                                                                    |
| Lines of Code    | 157 |
| Passed           | 4                                                                                                                                                  |
| Failed           | 0                                                                                                                                                  |
| Untested         | 0                                                                                                                                                  |
| Count type N     | 1                                                                                                                                                  |
| Count type A     | 3                                                                                                                                                  |
| Count type B     | 0                                                                                                                                                  |
| Test Requirement | Validate 'Generate access token' in TokenService.GenerateAccessTokenAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                  | Precondition                                                                   | UTCIDs  |
| -------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------ | ------- |
| user null → ném `ArgumentNullException`.                                                                                   | Truyền user = null.                                                            | UTCID01 |
| roles null/rỗng → ném `ArgumentException` hoặc tạo token không có role claims.                                             | Truyền roles = null hoặc empty list.                                           | UTCID02 |
| user và roles hợp lệ → tạo JWT token với đầy đủ claims (sub, email, roles, jti, exp) → trả về token string không rỗng.     | user seed với Id, Email hợp lệ; roles = ["Patient"]; JWT settings mock hợp lệ. | UTCID03 |
| Token được tạo → parse và verify claims: sub = userId, email claim khớp, role claim chứa tất cả roles, jti là Guid hợp lệ. | Toàn bộ flow thành công; validate token bằng JwtSecurityTokenHandler.          | UTCID04 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                   | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------- | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw                                             | `ArgumentNullException` | –                    | P             |               |           |
| UTCID02 | A            | throw hoặc token không có role claims             | `ArgumentException`     | –                    | P             |               |           |
| UTCID03 | N            | JWT string không rỗng, valid JWT format           | –                       | –                    | P             |               |           |
| UTCID04 | A            | JWT có claims: sub=userId, email, roles, jti=Guid | –                       | –                    | P             |               |           |

---

## F044 - TokenService.GenerateRefreshToken

| Header           | Value                                                                                                                                                  |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F044                                                                                                                                                   |
| Function Name    | TokenService.GenerateRefreshToken                                                                                                                      |
| Total Test Cases | 3                                                                                                                                                      |
| Created By       |                                                                                                                                                        |
| Executed By      |                                                                                                                                                        |
| Lines of Code    | 157 |
| Passed           | 3                                                                                                                                                      |
| Failed           | 0                                                                                                                                                      |
| Untested         | 0                                                                                                                                                      |
| Count type N     | 1                                                                                                                                                      |
| Count type A     | 2                                                                                                                                                      |
| Count type B     | 0                                                                                                                                                      |
| Test Requirement | Validate 'Generate refresh token string' in TokenService.GenerateRefreshToken, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                         | Precondition                                       | UTCIDs  |
| ------------------------------------------------------------------------------------------------- | -------------------------------------------------- | ------- |
| Gọi GenerateRefreshToken() → trả về chuỗi Base64Url-encoded ngẫu nhiên không rỗng.                | Không có input; sử dụng RandomNumberGenerator.     | UTCID01 |
| Gọi 2 lần liên tiếp → hai token khác nhau (random, không trùng lặp).                              | Gọi 2 lần trong cùng test; so sánh kết quả ≠.      | UTCID02 |
| Token trả về có độ dài cố định (≥ 32 chars sau base64 encoding) và không chứa ký tự không hợp lệ. | Verify format: length và charset của chuỗi trả về. | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                          | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | -------------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | string ngẫu nhiên không rỗng, độ dài ≥ 32                | –                  | –                    | P             |               |           |
| UTCID02 | A            | 2 lần gọi trả về 2 giá trị khác nhau (≠)                 | –                  | –                    | P             |               |           |
| UTCID03 | A            | string chỉ chứa base64url chars (A-Z, a-z, 0-9, +, /, =) | –                  | –                    | P             |               |           |

---

## F045 - TokenService.ValidateToken

| Header           | Value                                                                                                                            |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F045                                                                                                                             |
| Function Name    | TokenService.ValidateToken                                                                                                       |
| Total Test Cases | 3                                                                                                                                |
| Created By       |                                                                                                                                  |
| Executed By      |                                                                                                                                  |
| Lines of Code    | 157 |
| Passed           | 3                                                                                                                                |
| Failed           | 0                                                                                                                                |
| Untested         | 0                                                                                                                                |
| Count type N     | 1                                                                                                                                |
| Count type A     | 2                                                                                                                                |
| Count type B     | 0                                                                                                                                |
| Test Requirement | Validate 'Validate token' in TokenService.ValidateToken, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                              | Precondition                                                          | UTCIDs  |
| -------------------------------------------------------------------------------------- | --------------------------------------------------------------------- | ------- |
| token null/rỗng → trả về null hoặc ném `ArgumentException`.                            | token = null/"".                                                      | UTCID01 |
| token hợp lệ (chữ ký đúng, chưa hết hạn) → trả về `ClaimsPrincipal` với claims đầy đủ. | token được tạo bằng GenerateAccessTokenAsync với JWT settings hợp lệ. | UTCID02 |
| token sai chữ ký hoặc hết hạn → trả về null (không throw, bắt exception).              | token bị tamper hoặc có exp đã qua.                                   | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return          | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------ | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | `null` hoặc throw        | `ArgumentException` | –                    | P             |               |           |
| UTCID02 | N            | `ClaimsPrincipal` hợp lệ | –                   | –                    | P             |               |           |
| UTCID03 | A            | `null`                   | –                   | –                    | P             |               |           |

---

## F046 - TokenService.GetUserIdFromToken

| Header           | Value                                                                                                                                         |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F046                                                                                                                                          |
| Function Name    | TokenService.GetUserIdFromToken                                                                                                               |
| Total Test Cases | 3                                                                                                                                             |
| Created By       |                                                                                                                                               |
| Executed By      |                                                                                                                                               |
| Lines of Code    | 157 |
| Passed           | 3                                                                                                                                             |
| Failed           | 0                                                                                                                                             |
| Untested         | 0                                                                                                                                             |
| Count type N     | 1                                                                                                                                             |
| Count type A     | 2                                                                                                                                             |
| Count type B     | 0                                                                                                                                             |
| Test Requirement | Validate 'Get user id from token' in TokenService.GetUserIdFromToken, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                  | Precondition                                                                     | UTCIDs  |
| ---------------------------------------------------------- | -------------------------------------------------------------------------------- | ------- |
| token null/rỗng → trả về null.                             | token = null/"".                                                                 | UTCID01 |
| token hợp lệ có claim sub = userId → trả về userId string. | token được tạo với sub = valid Guid string; ValidateToken mock trả về principal. | UTCID02 |
| token hợp lệ nhưng không có claim sub → trả về null.       | token được tạo không có NameIdentifier claim.                                    | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return             | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | `null`                      | –                  | –                    | P             |               |           |
| UTCID02 | N            | userId string (Guid format) | –                  | –                    | P             |               |           |
| UTCID03 | A            | `null`                      | –                  | –                    | P             |               |           |

---

## F047 - TokenService.GetJtiFromToken

| Header           | Value                                                                                                                                  |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F047                                                                                                                                   |
| Function Name    | TokenService.GetJtiFromToken                                                                                                           |
| Total Test Cases | 2                                                                                                                                      |
| Created By       |                                                                                                                                        |
| Executed By      |                                                                                                                                        |
| Lines of Code    | 157 |
| Passed           | 2                                                                                                                                      |
| Failed           | 0                                                                                                                                      |
| Untested         | 0                                                                                                                                      |
| Count type N     | 1                                                                                                                                      |
| Count type A     | 1                                                                                                                                      |
| Count type B     | 0                                                                                                                                      |
| Test Requirement | Validate 'Get jti from token' in TokenService.GetJtiFromToken, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                    | Precondition                                                                       | UTCIDs  |
| ------------------------------------------------------------ | ---------------------------------------------------------------------------------- | ------- |
| token null/rỗng → trả về null.                               | token = null/"".                                                                   | UTCID01 |
| token hợp lệ có claim jti → trả về jti string (Guid format). | token được tạo với jti = Guid.NewGuid().ToString(); ValidateToken mock thành công. | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return          | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------ | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | `null`                   | –                  | –                    | P             |               |           |
| UTCID02 | N            | jti string (Guid format) | –                  | –                    | P             |               |           |

---

## F048 - TokenService.HashToken

| Header           | Value                                                                                                                    |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F048                                                                                                                     |
| Function Name    | TokenService.HashToken                                                                                                   |
| Total Test Cases | 3                                                                                                                        |
| Created By       |                                                                                                                          |
| Executed By      |                                                                                                                          |
| Lines of Code    | 157 |
| Passed           | 3                                                                                                                        |
| Failed           | 0                                                                                                                        |
| Untested         | 0                                                                                                                        |
| Count type N     | 1                                                                                                                        |
| Count type A     | 2                                                                                                                        |
| Count type B     | 0                                                                                                                        |
| Test Requirement | Validate 'Hash token' in TokenService.HashToken, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                              | Precondition                             | UTCIDs  |
| ------------------------------------------------------------------------------------------------------ | ---------------------------------------- | ------- |
| token null/rỗng → ném `ArgumentException`.                                                             | token = null/"".                         | UTCID01 |
| token hợp lệ → trả về SHA256 hash string (64 hex chars hoặc base64 tùy implementation); deterministic. | token = "validToken123"; không cần mock. | UTCID02 |
| Cùng input → cùng output (deterministic hashing); khác input → khác output.                            | Gọi 2 lần với cùng input và khác input.  | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                   | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------- | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | A            | throw                                             | `ArgumentException` | –                    | P             |               |           |
| UTCID02 | N            | hash string không rỗng (64 hex chars hoặc base64) | –                   | –                    | P             |               |           |
| UTCID03 | A            | cùng input → cùng hash; khác input → khác hash    | –                   | –                    | P             |               |           |

---

## F049 - AdminQueryService.GetOphthalmologistsAsync

| Header           | Value                                                                                                                                                        |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F049                                                                                                                                                         |
| Function Name    | AdminQueryService.GetOphthalmologistsAsync                                                                                                                   |
| Total Test Cases | 5                                                                                                                                                            |
| Created By       |                                                                                                                                                              |
| Executed By      |                                                                                                                                                              |
| Lines of Code    | 363 |
| Passed           | 5                                                                                                                                                            |
| Failed           | 0                                                                                                                                                            |
| Untested         | 0                                                                                                                                                            |
| Count type N     | 1                                                                                                                                                            |
| Count type A     | 4                                                                                                                                                            |
| Count type B     | 0                                                                                                                                                            |
| Test Requirement | Validate 'Get ophthalmologists query' in AdminQueryService.GetOphthalmologistsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                         | Precondition                                                                                | UTCIDs  |
| --------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------- | ------- |
| Không có filter → trả về tất cả bác sĩ nhãn khoa (paginated) với đúng TotalCount. | DbContext seed với 5 bác sĩ nhãn khoa; pageIndex=1, pageSize=10; không có filter.           | UTCID01 |
| Filter theo tên (name search) → chỉ trả về bác sĩ có tên khớp (case-insensitive). | DbContext seed với 5 bác sĩ; filter.Name = "Nguyen"; 2 trong số đó có tên chứa "Nguyen".    | UTCID02 |
| Filter theo IsActive=true → chỉ trả về bác sĩ đang hoạt động.                     | DbContext seed với 3 active + 2 inactive bác sĩ; filter.IsActive = true.                    | UTCID03 |
| Không có bác sĩ nào → trả về danh sách rỗng với TotalCount=0.                     | DbContext seed không có bác sĩ nhãn khoa; pageIndex=1, pageSize=10.                         | UTCID04 |
| pageSize=2, pageIndex=2 → trả về đúng page thứ 2 (2 items), TotalCount=5.         | DbContext seed với 5 bác sĩ; pageIndex=2, pageSize=2; verify Items.Count=2 và TotalCount=5. | UTCID05 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                             | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ----------------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `PagedResult` với Items.Count=5, TotalCount=5               | –                  | –                    | P             |               |           |
| UTCID02 | A            | `PagedResult` với Items chỉ chứa bác sĩ tên "Nguyen"        | –                  | –                    | P             |               |           |
| UTCID03 | A            | `PagedResult` với Items.Count=3 (active only)               | –                  | –                    | P             |               |           |
| UTCID04 | A            | `PagedResult` với Items.Count=0, TotalCount=0               | –                  | –                    | P             |               |           |
| UTCID05 | A            | `PagedResult` với Items.Count=2, TotalCount=5 (page 2 of 3) | –                  | –                    | P             |               |           |

---

## F050 - AdminQueryService.GetPatientsAsync

| Header           | Value                                                                                                                                        |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F050                                                                                                                                         |
| Function Name    | AdminQueryService.GetPatientsAsync                                                                                                           |
| Total Test Cases | 4                                                                                                                                            |
| Created By       |                                                                                                                                              |
| Executed By      |                                                                                                                                              |
| Lines of Code    | 363 |
| Passed           | 4                                                                                                                                            |
| Failed           | 0                                                                                                                                            |
| Untested         | 0                                                                                                                                            |
| Count type N     | 1                                                                                                                                            |
| Count type A     | 3                                                                                                                                            |
| Count type B     | 0                                                                                                                                            |
| Test Requirement | Validate 'Get patients query' in AdminQueryService.GetPatientsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                  | Precondition                                                                               | UTCIDs  |
| -------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------ | ------- |
| Không có filter → trả về tất cả bệnh nhân (paginated) với đúng TotalCount. | DbContext seed với 8 bệnh nhân; pageIndex=1, pageSize=10; không có filter.                 | UTCID01 |
| Filter theo email → chỉ trả về bệnh nhân có email khớp.                    | DbContext seed với 8 bệnh nhân; filter.Email = "test@"; 2 bệnh nhân có email chứa "test@". | UTCID02 |
| Filter theo IsActive=false → chỉ trả về bệnh nhân bị vô hiệu hóa.          | DbContext seed với 5 active + 3 inactive; filter.IsActive = false.                         | UTCID03 |
| Không có bệnh nhân → trả về danh sách rỗng với TotalCount=0.               | DbContext seed không có bệnh nhân; pageIndex=1, pageSize=10.                               | UTCID04 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                 | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ----------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `PagedResult` với Items.Count=8, TotalCount=8   | –                  | –                    | P             |               |           |
| UTCID02 | A            | `PagedResult` với Items chỉ chứa email "test@"  | –                  | –                    | P             |               |           |
| UTCID03 | A            | `PagedResult` với Items.Count=3 (inactive only) | –                  | –                    | P             |               |           |
| UTCID04 | A            | `PagedResult` với Items.Count=0, TotalCount=0   | –                  | –                    | P             |               |           |

---

## F051 - AdminQueryService.GetAuditLogsAsync

| Header           | Value                                                                                                                                           |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F051                                                                                                                                            |
| Function Name    | AdminQueryService.GetAuditLogsAsync                                                                                                             |
| Total Test Cases | 5                                                                                                                                               |
| Created By       |                                                                                                                                                 |
| Executed By      |                                                                                                                                                 |
| Lines of Code    | 363 |
| Passed           | 5                                                                                                                                               |
| Failed           | 0                                                                                                                                               |
| Untested         | 0                                                                                                                                               |
| Count type N     | 1                                                                                                                                               |
| Count type A     | 4                                                                                                                                               |
| Count type B     | 0                                                                                                                                               |
| Test Requirement | Validate 'Get audit logs query' in AdminQueryService.GetAuditLogsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                         | Precondition                                                                          | UTCIDs  |
| --------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------- | ------- |
| Không có filter → trả về tất cả audit logs (paginated) với TotalCount đúng.       | DbContext seed với 10 audit logs; pageIndex=1, pageSize=10.                           | UTCID01 |
| Filter theo UserId → chỉ trả về logs của user đó.                                 | DbContext seed với logs của 3 users; filter.UserId = "userId-A"; 4 logs của userId-A. | UTCID02 |
| Filter theo Action (vd. "Login") → chỉ trả về logs có Action = "Login".           | DbContext seed với logs nhiều action; filter.Action = "Login"; 3 logs loại "Login".   | UTCID03 |
| Filter theo DateRange (FromDate-ToDate) → chỉ trả về logs trong khoảng thời gian. | DbContext seed với logs nhiều ngày; filter với FromDate=hôm nay-7 và ToDate=hôm nay.  | UTCID04 |
| Không có audit log → trả về danh sách rỗng với TotalCount=0.                      | DbContext seed không có audit log.                                                    | UTCID05 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                     | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `PagedResult` với Items.Count=10, TotalCount=10     | –                  | –                    | P             |               |           |
| UTCID02 | A            | `PagedResult` với Items chỉ chứa logs của userId-A  | –                  | –                    | P             |               |           |
| UTCID03 | A            | `PagedResult` với Items chỉ chứa Action="Login"     | –                  | –                    | P             |               |           |
| UTCID04 | A            | `PagedResult` với Items trong DateRange đã chỉ định | –                  | –                    | P             |               |           |
| UTCID05 | A            | `PagedResult` với Items.Count=0, TotalCount=0       | –                  | –                    | P             |               |           |

---

## F052 - BetterStackHeartbeatService.GetEmbedUrl

| Header           | Value                                                                                                                                                    |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F052                                                                                                                                                     |
| Function Name    | BetterStackHeartbeatService.GetEmbedUrl                                                                                                                  |
| Total Test Cases | 2                                                                                                                                                        |
| Created By       |                                                                                                                                                          |
| Executed By      |                                                                                                                                                          |
| Lines of Code    | 156 |
| Passed           | 2                                                                                                                                                        |
| Failed           | 0                                                                                                                                                        |
| Untested         | 0                                                                                                                                                        |
| Count type N     | 1                                                                                                                                                        |
| Count type A     | 1                                                                                                                                                        |
| Count type B     | 0                                                                                                                                                        |
| Test Requirement | Validate 'Get BetterStack embed URL' in BetterStackHeartbeatService.GetEmbedUrl, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                     | Precondition                                                                         | UTCIDs  |
| ----------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | ------- |
| EmbedUrl được cấu hình trong settings → trả về chuỗi URL hợp lệ (không rỗng). | BetterStackOptions.EmbedUrl = "https://betterstack.com/embed/xxx"; service mock sẵn. | UTCID01 |
| EmbedUrl không được cấu hình (null/rỗng) → trả về null hoặc empty string.     | BetterStackOptions.EmbedUrl = null/""; service được khởi tạo với config thiếu.       | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------ | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | URL string không rỗng (hợp lệ) | –                  | –                    | P             |               |           |
| UTCID02 | A            | `null` hoặc empty string       | –                  | –                    | P             |               |           |

---

## F053 - BetterStackHeartbeatService.GetMonitorDescriptors

| Header           | Value                                                                                                                                                                        |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F053                                                                                                                                                                         |
| Function Name    | BetterStackHeartbeatService.GetMonitorDescriptors                                                                                                                            |
| Total Test Cases | 2                                                                                                                                                                            |
| Created By       |                                                                                                                                                                              |
| Executed By      |                                                                                                                                                                              |
| Lines of Code    | 156 |
| Passed           | 2                                                                                                                                                                            |
| Failed           | 0                                                                                                                                                                            |
| Untested         | 0                                                                                                                                                                            |
| Count type N     | 1                                                                                                                                                                            |
| Count type A     | 1                                                                                                                                                                            |
| Count type B     | 0                                                                                                                                                                            |
| Test Requirement | Validate 'Get BetterStack monitor descriptors' in BetterStackHeartbeatService.GetMonitorDescriptors, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                        | Precondition                                                                     | UTCIDs  |
| ------------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------------- | ------- |
| Service được cấu hình với N monitors → trả về IEnumerable với N MonitorDescriptor đúng name/url. | BetterStackOptions.Monitors seed với 3 monitors có name và heartbeat URL hợp lệ. | UTCID01 |
| Service được cấu hình không có monitor nào → trả về danh sách rỗng.                              | BetterStackOptions.Monitors = null hoặc danh sách rỗng.                          | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                    | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | -------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `IEnumerable<MonitorDescriptor>` với 3 descriptors | –                  | –                    | P             |               |           |
| UTCID02 | A            | `IEnumerable<MonitorDescriptor>` rỗng              | –                  | –                    | P             |               |           |

---

## F054 - BetterStackHeartbeatService.NotifyStartedAsync

| Header           | Value                                                                                                                                                        |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F054                                                                                                                                                         |
| Function Name    | BetterStackHeartbeatService.NotifyStartedAsync                                                                                                               |
| Total Test Cases | 3                                                                                                                                                            |
| Created By       |                                                                                                                                                              |
| Executed By      |                                                                                                                                                              |
| Lines of Code    | 156 |
| Passed           | 3                                                                                                                                                            |
| Failed           | 0                                                                                                                                                            |
| Untested         | 0                                                                                                                                                            |
| Count type N     | 1                                                                                                                                                            |
| Count type A     | 2                                                                                                                                                            |
| Count type B     | 0                                                                                                                                                            |
| Test Requirement | Validate 'Notify monitor started' in BetterStackHeartbeatService.NotifyStartedAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                      | Precondition                                                                   | UTCIDs  |
| -------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------ | ------- |
| monitorName hợp lệ, heartbeat URL hợp lệ → HTTP GET đến heartbeat URL được thực hiện thành công → không throw. | HttpClient mock trả về 200 OK; monitorName khớp 1 entry trong Monitors config. | UTCID01 |
| monitorName không tìm thấy trong config → bỏ qua (no-op) hoặc log warning; không throw.                        | Monitors config không có entry khớp monitorName; HttpClient không được gọi.    | UTCID02 |
| HttpClient ném exception (network error) → exception được bắt, log error; không propagate (best-effort).       | HttpClient mock ném HttpRequestException; monitorName hợp lệ.                  | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                        | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | -------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; HTTP GET được gọi đúng 1 lần     | –                  | –                    | P             |               |           |
| UTCID02 | A            | void; HTTP GET không được gọi          | –                  | –                    | P             |               |           |
| UTCID03 | A            | void (exception bị bắt, không rethrow) | –                  | log error            | P             |               |           |

---

## F055 - BetterStackHeartbeatService.NotifySucceededAsync

| Header           | Value                                                                                                                                                            |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F055                                                                                                                                                             |
| Function Name    | BetterStackHeartbeatService.NotifySucceededAsync                                                                                                                 |
| Total Test Cases | 2                                                                                                                                                                |
| Created By       |                                                                                                                                                                  |
| Executed By      |                                                                                                                                                                  |
| Lines of Code    | 156 |
| Passed           | 2                                                                                                                                                                |
| Failed           | 0                                                                                                                                                                |
| Untested         | 0                                                                                                                                                                |
| Count type N     | 1                                                                                                                                                                |
| Count type A     | 1                                                                                                                                                                |
| Count type B     | 0                                                                                                                                                                |
| Test Requirement | Validate 'Notify monitor succeeded' in BetterStackHeartbeatService.NotifySucceededAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                 | Precondition                                                    | UTCIDs  |
| ------------------------------------------------------------------------- | --------------------------------------------------------------- | ------- |
| monitorName hợp lệ → HTTP GET heartbeat URL thành công → void (no error). | HttpClient mock trả về 200 OK; monitorName hợp lệ trong config. | UTCID01 |
| HttpClient ném exception → exception bị bắt, log; không propagate.        | HttpClient mock ném HttpRequestException; monitorName hợp lệ.   | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                        | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | -------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; HTTP GET được gọi đúng 1 lần     | –                  | –                    | P             |               |           |
| UTCID02 | A            | void (exception bị bắt, không rethrow) | –                  | log error            | P             |               |           |

---

## F056 - BetterStackHeartbeatService.NotifyFailedAsync

| Header           | Value                                                                                                                                                      |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F056                                                                                                                                                       |
| Function Name    | BetterStackHeartbeatService.NotifyFailedAsync                                                                                                              |
| Total Test Cases | 2                                                                                                                                                          |
| Created By       |                                                                                                                                                            |
| Executed By      |                                                                                                                                                            |
| Lines of Code    | 156 |
| Passed           | 2                                                                                                                                                          |
| Failed           | 0                                                                                                                                                          |
| Untested         | 0                                                                                                                                                          |
| Count type N     | 1                                                                                                                                                          |
| Count type A     | 1                                                                                                                                                          |
| Count type B     | 0                                                                                                                                                          |
| Test Requirement | Validate 'Notify monitor failed' in BetterStackHeartbeatService.NotifyFailedAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                 | Precondition                                                    | UTCIDs  |
| ----------------------------------------------------------------------------------------- | --------------------------------------------------------------- | ------- |
| monitorName hợp lệ, có error message → HTTP POST/GET đến fail endpoint thành công → void. | HttpClient mock trả về 200 OK; monitorName hợp lệ trong config. | UTCID01 |
| HttpClient ném exception khi notify fail → exception bị bắt, log; không propagate.        | HttpClient mock ném HttpRequestException; monitorName hợp lệ.   | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                        | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | -------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; fail notification được gửi       | –                  | –                    | P             |               |           |
| UTCID02 | A            | void (exception bị bắt, không rethrow) | –                  | log error            | P             |               |           |

---

## F057 - DashboardMetricsService.GetSystemAdminMetricsAsync

| Header           | Value                                                                                                                                                              |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F057                                                                                                                                                               |
| Function Name    | DashboardMetricsService.GetSystemAdminMetricsAsync                                                                                                                 |
| Total Test Cases | 6                                                                                                                                                                  |
| Created By       |                                                                                                                                                                    |
| Executed By      |                                                                                                                                                                    |
| Lines of Code    | 941 |
| Passed           | 6                                                                                                                                                                  |
| Failed           | 0                                                                                                                                                                  |
| Untested         | 0                                                                                                                                                                  |
| Count type N     | 1                                                                                                                                                                  |
| Count type A     | 5                                                                                                                                                                  |
| Count type B     | 0                                                                                                                                                                  |
| Test Requirement | Validate 'Get system admin metrics' in DashboardMetricsService.GetSystemAdminMetricsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                                 | Precondition                                                                                             | UTCIDs  |
| ----------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------- | ------- |
| DB có dữ liệu đầy đủ → trả về SystemAdminMetrics với TotalScreenings, TotalPatients, TotalDoctors, AverageSeverity, ScreeningsTrend đúng. | DbContext seed với 20 screenings, 15 patients, 5 doctors; DateTimeService mock.                          | UTCID01 |
| DB rỗng (không có dữ liệu) → trả về metrics với tất cả count = 0, averages = 0.                                                           | DbContext seed rỗng; không có screenings/patients/doctors.                                               | UTCID02 |
| Chỉ có screenings trong tháng hiện tại → ScreeningsTrend tính đúng số lượng screenings trong 30 ngày qua.                                 | DbContext seed với 5 screenings trong tháng + 10 screenings cũ hơn; DateTimeService.UtcNow mock = today. | UTCID03 |
| TotalDoctors chỉ tính bác sĩ active (IsActive=true) → không tính bác sĩ inactive.                                                         | DbContext seed với 3 active doctors + 2 inactive doctors.                                                | UTCID04 |
| Có screenings với severity khác nhau → AverageSeverity = trung bình đúng.                                                                 | DbContext seed với screenings có severity [1, 2, 3, 4, 5]; expected avg = 3.0.                           | UTCID05 |
| DbContext.Database.ExecuteSqlRaw hoặc LINQ query ném exception → exception propagate.                                                     | Một query trong method ném InvalidOperationException.                                                    | UTCID06 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                                     | Expected exception          | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------------------------- | --------------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `SystemAdminMetrics` với TotalScreenings=20, TotalPatients=15, etc. | –                           | –                    | P             |               |           |
| UTCID02 | A            | `SystemAdminMetrics` với tất cả count = 0                           | –                           | –                    | P             |               |           |
| UTCID03 | A            | `SystemAdminMetrics` với ScreeningsTrend = 5                        | –                           | –                    | P             |               |           |
| UTCID04 | A            | `SystemAdminMetrics` với TotalDoctors = 3 (active only)             | –                           | –                    | P             |               |           |
| UTCID05 | A            | `SystemAdminMetrics` với AverageSeverity = 3.0                      | –                           | –                    | P             |               |           |
| UTCID06 | A            | throw                                                               | `InvalidOperationException` | –                    | P             |               |           |

---

## F058 - DashboardMetricsService.GetRecentScreeningsAsync

| Header           | Value                                                                                                                                                         |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F058                                                                                                                                                          |
| Function Name    | DashboardMetricsService.GetRecentScreeningsAsync                                                                                                              |
| Total Test Cases | 4                                                                                                                                                             |
| Created By       |                                                                                                                                                               |
| Executed By      |                                                                                                                                                               |
| Lines of Code    | 941 |
| Passed           | 4                                                                                                                                                             |
| Failed           | 0                                                                                                                                                             |
| Untested         | 0                                                                                                                                                             |
| Count type N     | 1                                                                                                                                                             |
| Count type A     | 3                                                                                                                                                             |
| Count type B     | 0                                                                                                                                                             |
| Test Requirement | Validate 'Get recent screenings' in DashboardMetricsService.GetRecentScreeningsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                     | Precondition                                                                       | UTCIDs  |
| --------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------- | ------- |
| limit=5, có 10 screenings → trả về 5 screenings gần nhất (sắp xếp desc theo CreatedAt).       | DbContext seed với 10 screenings, CreatedAt khác nhau; limit=5.                    | UTCID01 |
| limit=5, không có screening nào → trả về danh sách rỗng.                                      | DbContext seed không có screenings.                                                | UTCID02 |
| limit > số lượng screenings thực tế (limit=10, có 3 screenings) → trả về tất cả 3 screenings. | DbContext seed với 3 screenings; limit=10.                                         | UTCID03 |
| Mỗi item trong kết quả chứa đủ thông tin: PatientName, ScreeningDate, Severity, Status.       | DbContext seed với 5 screenings đầy đủ thông tin; verify các field trong response. | UTCID04 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                                   | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ----------------------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `IList<RecentScreeningDto>` với 5 items, sorted desc by CreatedAt | –                  | –                    | P             |               |           |
| UTCID02 | A            | `IList<RecentScreeningDto>` rỗng                                  | –                  | –                    | P             |               |           |
| UTCID03 | A            | `IList<RecentScreeningDto>` với 3 items (all)                     | –                  | –                    | P             |               |           |
| UTCID04 | A            | Mỗi item có PatientName, ScreeningDate, Severity, Status đầy đủ   | –                  | –                    | P             |               |           |

---

## F059 - DashboardMetricsService.GetScreeningVolumeTrendsAsync

| Header           | Value                                                                                                                                                                    |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F059                                                                                                                                                                     |
| Function Name    | DashboardMetricsService.GetScreeningVolumeTrendsAsync                                                                                                                    |
| Total Test Cases | 3                                                                                                                                                                        |
| Created By       |                                                                                                                                                                          |
| Executed By      |                                                                                                                                                                          |
| Lines of Code    | 941 |
| Passed           | 3                                                                                                                                                                        |
| Failed           | 0                                                                                                                                                                        |
| Untested         | 0                                                                                                                                                                        |
| Count type N     | 1                                                                                                                                                                        |
| Count type A     | 2                                                                                                                                                                        |
| Count type B     | 0                                                                                                                                                                        |
| Test Requirement | Validate 'Get screening volume trends' in DashboardMetricsService.GetScreeningVolumeTrendsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                  | Precondition                                                                        | UTCIDs  |
| ------------------------------------------------------------------------------------------ | ----------------------------------------------------------------------------------- | ------- |
| Có screenings trong 12 tháng → trả về danh sách trend theo tháng, mỗi tháng có count đúng. | DbContext seed với screenings trải qua 12 tháng khác nhau; DateTimeService mock.    | UTCID01 |
| Không có screening nào → trả về danh sách rỗng hoặc tất cả tháng có count = 0.             | DbContext seed không có screenings.                                                 | UTCID02 |
| Screenings trong cùng 1 tháng → count đúng, group by month chính xác.                      | DbContext seed với 5 screenings cùng tháng 3/2026; verify entry tháng 3 có Count=5. | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                                     | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `IList<VolumeTrendDto>` với 12 entries, mỗi entry có Month và Count | –                  | –                    | P             |               |           |
| UTCID02 | A            | `IList<VolumeTrendDto>` rỗng hoặc tất cả Count = 0                  | –                  | –                    | P             |               |           |
| UTCID03 | A            | Entry tháng 3 có Count = 5                                          | –                  | –                    | P             |               |           |

---

## F060 - DashboardMetricsService.GetPopulationRiskAnalysisAsync

| Header           | Value                                                                                                                                                                      |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F060                                                                                                                                                                       |
| Function Name    | DashboardMetricsService.GetPopulationRiskAnalysisAsync                                                                                                                     |
| Total Test Cases | 3                                                                                                                                                                          |
| Created By       |                                                                                                                                                                            |
| Executed By      |                                                                                                                                                                            |
| Lines of Code    | 941 |
| Passed           | 3                                                                                                                                                                          |
| Failed           | 0                                                                                                                                                                          |
| Untested         | 0                                                                                                                                                                          |
| Count type N     | 1                                                                                                                                                                          |
| Count type A     | 2                                                                                                                                                                          |
| Count type B     | 0                                                                                                                                                                          |
| Test Requirement | Validate 'Get population risk analysis' in DashboardMetricsService.GetPopulationRiskAnalysisAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                           | Precondition                                                                         | UTCIDs  |
| ------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | ------- |
| Có screenings với severity khác nhau → trả về phân bố risk (Low/Medium/High/Critical) với count và percentage đúng. | DbContext seed với 10 screenings: 3 Low(1), 3 Medium(2-3), 2 High(4), 2 Critical(5). | UTCID01 |
| Không có screening → trả về danh sách rỗng hoặc tất cả risk level có count = 0.                                     | DbContext seed không có screenings.                                                  | UTCID02 |
| Tất cả screenings cùng severity → chỉ 1 risk level có count > 0, percentage = 100%.                                 | DbContext seed với 5 screenings tất cả severity = 5 (Critical).                      | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                                                | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------------------------------------ | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `IList<RiskDistributionDto>` với Low=3(30%), Medium=3(30%), High=2, Critical=2 | –                  | –                    | P             |               |           |
| UTCID02 | A            | `IList<RiskDistributionDto>` rỗng hoặc tất cả count = 0                        | –                  | –                    | P             |               |           |
| UTCID03 | A            | Chỉ Critical có Count=5, Percentage=100%; các level khác = 0                   | –                  | –                    | P             |               |           |

---

## F061 - DashboardMetricsService.GetSystemHealthAsync

| Header           | Value                                                                                                                                                 |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F061                                                                                                                                                  |
| Function Name    | DashboardMetricsService.GetSystemHealthAsync                                                                                                          |
| Total Test Cases | 2                                                                                                                                                     |
| Created By       |                                                                                                                                                       |
| Executed By      |                                                                                                                                                       |
| Lines of Code    | 941 |
| Passed           | 2                                                                                                                                                     |
| Failed           | 0                                                                                                                                                     |
| Untested         | 0                                                                                                                                                     |
| Count type N     | 1                                                                                                                                                     |
| Count type A     | 1                                                                                                                                                     |
| Count type B     | 0                                                                                                                                                     |
| Test Requirement | Validate 'Get system health' in DashboardMetricsService.GetSystemHealthAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                          | Precondition                                                                | UTCIDs  |
| -------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------- | ------- |
| DB reachable, có data → trả về SystemHealth với Status="Healthy", DB connection time, uptime đúng. | DbContext mock connection thành công; DateTimeService mock; server UtcNow.  | UTCID01 |
| DB không reachable → trả về SystemHealth với Status="Unhealthy" và error message.                  | DbContext mock ném exception khi ping; service bắt exception và trả về DTO. | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                                   | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ----------------------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `SystemHealthDto` với Status="Healthy", DbConnectionMs > 0        | –                  | –                    | P             |               |           |
| UTCID02 | A            | `SystemHealthDto` với Status="Unhealthy", ErrorMessage không rỗng | –                  | –                    | P             |               |           |

---

## F062 - DashboardMetricsService.GetOphthalmologistMetricsAsync

| Header           | Value                                                                                                                                                                     |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F062                                                                                                                                                                      |
| Function Name    | DashboardMetricsService.GetOphthalmologistMetricsAsync                                                                                                                    |
| Total Test Cases | 4                                                                                                                                                                         |
| Created By       |                                                                                                                                                                           |
| Executed By      |                                                                                                                                                                           |
| Lines of Code    | 941 |
| Passed           | 4                                                                                                                                                                         |
| Failed           | 0                                                                                                                                                                         |
| Untested         | 0                                                                                                                                                                         |
| Count type N     | 1                                                                                                                                                                         |
| Count type A     | 3                                                                                                                                                                         |
| Count type B     | 0                                                                                                                                                                         |
| Test Requirement | Validate 'Get ophthalmologist metrics' in DashboardMetricsService.GetOphthalmologistMetricsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                            | Precondition                                                                       | UTCIDs  |
| ------------------------------------------------------------------------------------------------------------------------------------ | ---------------------------------------------------------------------------------- | ------- |
| doctorId hợp lệ, có screenings → trả về OphthalmologistMetrics với TotalCases, PendingReviews, CompletedCases, AverageSeverity đúng. | DbContext seed với 10 cases cho doctorId; 3 pending, 7 completed.                  | UTCID01 |
| doctorId hợp lệ, không có case nào → trả về metrics với tất cả = 0.                                                                  | DbContext seed không có cases cho doctorId.                                        | UTCID02 |
| doctorId null/rỗng → ném `ArgumentException`.                                                                                        | doctorId = null/"".                                                                | UTCID03 |
| Metrics trong tháng hiện tại (NewCasesThisMonth) được tính đúng.                                                                     | DbContext seed với 4 cases tháng này và 6 cases tháng trước; DateTimeService mock. | UTCID04 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                              | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------------------ | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `OphthalmologistMetrics` với TotalCases=10, PendingReviews=3 | –                   | –                    | P             |               |           |
| UTCID02 | A            | `OphthalmologistMetrics` với tất cả count = 0                | –                   | –                    | P             |               |           |
| UTCID03 | A            | throw                                                        | `ArgumentException` | –                    | P             |               |           |
| UTCID04 | A            | `OphthalmologistMetrics` với NewCasesThisMonth = 4           | –                   | –                    | P             |               |           |

---

## F063 - DashboardMetricsService.GetPatientMetricsAsync

| Header           | Value                                                                                                                                                     |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F063                                                                                                                                                      |
| Function Name    | DashboardMetricsService.GetPatientMetricsAsync                                                                                                            |
| Total Test Cases | 3                                                                                                                                                         |
| Created By       |                                                                                                                                                           |
| Executed By      |                                                                                                                                                           |
| Lines of Code    | 941 |
| Passed           | 3                                                                                                                                                         |
| Failed           | 0                                                                                                                                                         |
| Untested         | 0                                                                                                                                                         |
| Count type N     | 1                                                                                                                                                         |
| Count type A     | 2                                                                                                                                                         |
| Count type B     | 0                                                                                                                                                         |
| Test Requirement | Validate 'Get patient metrics' in DashboardMetricsService.GetPatientMetricsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                                   | Precondition                                                                 | UTCIDs  |
| ------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------- | ------- |
| patientId hợp lệ, có screenings → trả về PatientMetrics với TotalScreenings, LastScreeningDate, HighestSeverity, CurrentRoadmapStatus đúng. | DbContext seed với 5 screenings cho patientId; LastScreening = tháng 3/2026. | UTCID01 |
| patientId hợp lệ, không có screening → trả về metrics với count=0, dates=null.                                                              | DbContext seed không có screenings cho patientId.                            | UTCID02 |
| patientId null/rỗng → ném `ArgumentException`.                                                                                              | patientId = null/"".                                                         | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                                | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | -------------------------------------------------------------- | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `PatientMetrics` với TotalScreenings=5, LastScreeningDate đúng | –                   | –                    | P             |               |           |
| UTCID02 | A            | `PatientMetrics` với TotalScreenings=0, LastScreeningDate=null | –                   | –                    | P             |               |           |
| UTCID03 | A            | throw                                                          | `ArgumentException` | –                    | P             |               |           |

---

## F064 - DateTimeService.Now (property)

| Header           | Value                                                                                                                               |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F064                                                                                                                                |
| Function Name    | DateTimeService.Now (property)                                                                                                      |
| Total Test Cases | 2                                                                                                                                   |
| Created By       |                                                                                                                                     |
| Executed By      |                                                                                                                                     |
| Lines of Code    | 9 |
| Passed           | 2                                                                                                                                   |
| Failed           | 0                                                                                                                                   |
| Untested         | 0                                                                                                                                   |
| Count type N     | 1                                                                                                                                   |
| Count type A     | 1                                                                                                                                   |
| Count type B     | 0                                                                                                                                   |
| Test Requirement | Validate 'Get local now' in DateTimeService.Now (property), covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                           | Precondition                                | UTCIDs  |
| --------------------------------------------------------------------------------------------------- | ------------------------------------------- | ------- |
| Gọi Now → trả về `DateTime` local (Kind = Local), giá trị gần với `DateTime.Now` (within 1 second). | Service khởi tạo trực tiếp; không cần mock. | UTCID01 |
| Gọi Now 2 lần liên tiếp → giá trị thứ 2 ≥ giá trị thứ 1 (thời gian tăng dần).                       | Gọi 2 lần; so sánh result2 >= result1.      | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return            | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | -------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- | --- | --- |
| UTCID01 | N            | `DateTime` với Kind=Local; | Now - DateTime.Now | < 1 giây             | –             | –             | P         |     |     |
| UTCID02 | A            | result2 >= result1         | –                  | –                    | P             |               |           |

---

## F065 - DateTimeService.UtcNow (property)

| Header           | Value                                                                                                                                |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F065                                                                                                                                 |
| Function Name    | DateTimeService.UtcNow (property)                                                                                                    |
| Total Test Cases | 2                                                                                                                                    |
| Created By       |                                                                                                                                      |
| Executed By      |                                                                                                                                      |
| Lines of Code    | 9 |
| Passed           | 2                                                                                                                                    |
| Failed           | 0                                                                                                                                    |
| Untested         | 0                                                                                                                                    |
| Count type N     | 1                                                                                                                                    |
| Count type A     | 1                                                                                                                                    |
| Count type B     | 0                                                                                                                                    |
| Test Requirement | Validate 'Get UTC now' in DateTimeService.UtcNow (property), covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                             | Precondition                                | UTCIDs  |
| ----------------------------------------------------------------------------------------------------- | ------------------------------------------- | ------- |
| Gọi UtcNow → trả về `DateTime` UTC (Kind = Utc), giá trị gần với `DateTime.UtcNow` (within 1 second). | Service khởi tạo trực tiếp; không cần mock. | UTCID01 |
| UtcNow luôn nhỏ hơn hoặc bằng Now trong cùng timezone (UTC <= Local nếu offset > 0).                  | Gọi cả Now và UtcNow; so sánh offset.       | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                | Expected exception       | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------ | ------------------------ | -------------------- | ------------- | ------------- | --------- | --- | --- |
| UTCID01 | N            | `DateTime` với Kind=Utc;       | UtcNow - DateTime.UtcNow | < 1 giây             | –             | –             | P         |     |     |
| UTCID02 | A            | UtcNow.Kind = DateTimeKind.Utc | –                        | –                    | P             |               |           |

---

## F066 - EmailService.SendEmailConfirmationAsync

| Header           | Value                                                                                                                                                  |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F066                                                                                                                                                   |
| Function Name    | EmailService.SendEmailConfirmationAsync                                                                                                                |
| Total Test Cases | 2                                                                                                                                                      |
| Created By       |                                                                                                                                                        |
| Executed By      |                                                                                                                                                        |
| Lines of Code    | 418 |
| Passed           | 2                                                                                                                                                      |
| Failed           | 0                                                                                                                                                      |
| Untested         | 0                                                                                                                                                      |
| Count type N     | 1                                                                                                                                                      |
| Count type A     | 1                                                                                                                                                      |
| Count type B     | 0                                                                                                                                                      |
| Test Requirement | Validate 'Send email confirmation' in EmailService.SendEmailConfirmationAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                                                 | Precondition                                                                                   | UTCIDs  |
| --------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------- | ------- |
| Email và confirmationLink hợp lệ → SendAsync được gọi với đúng recipient, subject chứa "Confirm", body chứa confirmationLink; trả về void (no exception). | FluentEmail/MailKit mock; email = "user@test.com"; confirmationLink = "https://...?token=xxx". | UTCID01 |
| SMTP transport ném exception → exception propagate (hoặc được wrap tùy implementation).                                                                   | FluentEmail mock ném SmtpCommandException; verify exception không bị nuốt.                     | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                        | Expected exception     | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------------ | ---------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; SendAsync gọi đúng 1 lần với đúng recipient/body | –                      | –                    | P             |               |           |
| UTCID02 | A            | throw                                                  | `SmtpCommandException` | –                    | P             |               |           |

---

## F067 - EmailService.SendPasswordResetAsync

| Header           | Value                                                                                                                                                |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F067                                                                                                                                                 |
| Function Name    | EmailService.SendPasswordResetAsync                                                                                                                  |
| Total Test Cases | 2                                                                                                                                                    |
| Created By       |                                                                                                                                                      |
| Executed By      |                                                                                                                                                      |
| Lines of Code    | 418 |
| Passed           | 2                                                                                                                                                    |
| Failed           | 0                                                                                                                                                    |
| Untested         | 0                                                                                                                                                    |
| Count type N     | 1                                                                                                                                                    |
| Count type A     | 1                                                                                                                                                    |
| Count type B     | 0                                                                                                                                                    |
| Test Requirement | Validate 'Send password reset email' in EmailService.SendPasswordResetAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                  | Precondition                                                                    | UTCIDs  |
| -------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------- | ------- |
| Email và resetLink hợp lệ → SendAsync được gọi với đúng recipient, subject chứa "Reset", body chứa resetLink; trả về void. | FluentEmail mock; email = "user@test.com"; resetLink = "https://...?token=yyy". | UTCID01 |
| SMTP transport ném exception → exception propagate.                                                                        | FluentEmail mock ném SmtpCommandException.                                      | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                        | Expected exception     | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------------ | ---------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; SendAsync gọi đúng 1 lần với đúng recipient/body | –                      | –                    | P             |               |           |
| UTCID02 | A            | throw                                                  | `SmtpCommandException` | –                    | P             |               |           |

---

## F068 - EmailService.SendWelcomeEmailAsync

| Header           | Value                                                                                                                                        |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F068                                                                                                                                         |
| Function Name    | EmailService.SendWelcomeEmailAsync                                                                                                           |
| Total Test Cases | 2                                                                                                                                            |
| Created By       |                                                                                                                                              |
| Executed By      |                                                                                                                                              |
| Lines of Code    | 418 |
| Passed           | 2                                                                                                                                            |
| Failed           | 0                                                                                                                                            |
| Untested         | 0                                                                                                                                            |
| Count type N     | 1                                                                                                                                            |
| Count type A     | 1                                                                                                                                            |
| Count type B     | 0                                                                                                                                            |
| Test Requirement | Validate 'Send welcome email' in EmailService.SendWelcomeEmailAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                             | Precondition                                                          | UTCIDs  |
| ----------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------- | ------- |
| Email và fullName hợp lệ → SendAsync gọi với subject chứa "Welcome", body chứa fullName; trả về void. | FluentEmail mock; email = "user@test.com"; fullName = "Nguyen Van A". | UTCID01 |
| SMTP ném exception → exception propagate.                                                             | FluentEmail mock ném SmtpCommandException.                            | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                  | Expected exception     | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------ | ---------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; body chứa fullName; subject chứa "Welcome" | –                      | –                    | P             |               |           |
| UTCID02 | A            | throw                                            | `SmtpCommandException` | –                    | P             |               |           |

---

## F069 - EmailService.SendAsync

| Header           | Value                                                                                                                            |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F069                                                                                                                             |
| Function Name    | EmailService.SendAsync                                                                                                           |
| Total Test Cases | 4                                                                                                                                |
| Created By       |                                                                                                                                  |
| Executed By      |                                                                                                                                  |
| Lines of Code    | 418 |
| Passed           | 4                                                                                                                                |
| Failed           | 0                                                                                                                                |
| Untested         | 0                                                                                                                                |
| Count type N     | 1                                                                                                                                |
| Count type A     | 3                                                                                                                                |
| Count type B     | 0                                                                                                                                |
| Test Requirement | Validate 'Send generic email' in EmailService.SendAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                            | Precondition                                                              | UTCIDs  |
| ---------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------- | ------- |
| to, subject, body hợp lệ → FluentEmail.To().Subject().Body().SendAsync() được gọi đúng; trả về void. | FluentEmail mock; to = "user@test.com"; subject = "Test"; body = "Hello". | UTCID01 |
| to null/rỗng → ném `ArgumentException`.                                                              | to = null/"".                                                             | UTCID02 |
| subject null/rỗng → ném `ArgumentException`.                                                         | subject = null/"".                                                        | UTCID03 |
| FluentEmail SMTP transport ném exception → exception propagate.                                      | FluentEmail mock ném SmtpCommandException; to, subject, body hợp lệ.      | UTCID04 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                            | Expected exception     | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------ | ---------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; FluentEmail.SendAsync gọi đúng 1 lần | –                      | –                    | P             |               |           |
| UTCID02 | A            | throw                                      | `ArgumentException`    | –                    | P             |               |           |
| UTCID03 | A            | throw                                      | `ArgumentException`    | –                    | P             |               |           |
| UTCID04 | A            | throw                                      | `SmtpCommandException` | –                    | P             |               |           |

---

## F070 - GoogleMeetService.CreateMeetingAsync

| Header           | Value                                                                                                                                                  |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F070                                                                                                                                                   |
| Function Name    | GoogleMeetService.CreateMeetingAsync                                                                                                                   |
| Total Test Cases | 5                                                                                                                                                      |
| Created By       |                                                                                                                                                        |
| Executed By      |                                                                                                                                                        |
| Lines of Code    | 211 |
| Passed           | 5                                                                                                                                                      |
| Failed           | 0                                                                                                                                                      |
| Untested         | 0                                                                                                                                                      |
| Count type N     | 1                                                                                                                                                      |
| Count type A     | 4                                                                                                                                                      |
| Count type B     | 0                                                                                                                                                      |
| Test Requirement | Validate 'Create Google Meet meeting' in GoogleMeetService.CreateMeetingAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                            | Precondition                                                                                           | UTCIDs  |
| ------------------------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------ | ------- |
| Title và StartTime, EndTime hợp lệ → Google Calendar API tạo event thành công → trả về MeetingResponse với MeetLink không rỗng.      | CalendarService mock trả về Event với HangoutLink = "https://meet.google.com/xxx"; credentials hợp lệ. | UTCID01 |
| Title null/rỗng → ném `ArgumentException`.                                                                                           | title = null/"".                                                                                       | UTCID02 |
| StartTime >= EndTime → ném `ArgumentException("End time must be after start time")`.                                                 | startTime = 10:00, endTime = 09:00.                                                                    | UTCID03 |
| Google Calendar API trả về event không có HangoutLink → trả về MeetingResponse với MeetLink = null hoặc throw.                       | CalendarService mock trả về Event với HangoutLink = null; HangoutLink property absent.                 | UTCID04 |
| Google API ném `GoogleApiException` (quota exceeded hoặc auth error) → exception được propagate hoặc wrapped thành MeetingException. | CalendarService mock ném `GoogleApiException` với HttpStatusCode=429 (TooManyRequests).                | UTCID05 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                  | Expected exception   | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------ | -------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `MeetingResponse` với MeetLink != null           | –                    | –                    | P             |               |           |
| UTCID02 | A            | throw                                            | `ArgumentException`  | –                    | P             |               |           |
| UTCID03 | A            | throw                                            | `ArgumentException`  | –                    | P             |               |           |
| UTCID04 | A            | `MeetingResponse` với MeetLink = null hoặc throw | –                    | –                    | P             |               |           |
| UTCID05 | A            | throw                                            | `GoogleApiException` | –                    | P             |               |           |

---

## F071 - GoogleMeetService.DeleteMeetingAsync

| Header           | Value                                                                                                                                                  |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F071                                                                                                                                                   |
| Function Name    | GoogleMeetService.DeleteMeetingAsync                                                                                                                   |
| Total Test Cases | 2                                                                                                                                                      |
| Created By       |                                                                                                                                                        |
| Executed By      |                                                                                                                                                        |
| Lines of Code    | 211 |
| Passed           | 2                                                                                                                                                      |
| Failed           | 0                                                                                                                                                      |
| Untested         | 0                                                                                                                                                      |
| Count type N     | 1                                                                                                                                                      |
| Count type A     | 1                                                                                                                                                      |
| Count type B     | 0                                                                                                                                                      |
| Test Requirement | Validate 'Delete Google Meet meeting' in GoogleMeetService.DeleteMeetingAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                        | Precondition                                                               | UTCIDs  |
| ------------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------- | ------- |
| eventId hợp lệ → Google Calendar API xóa event thành công → trả về void (không throw).           | CalendarService.Events.Delete mock thành công; eventId = "valid-event-id". | UTCID01 |
| Google API ném `GoogleApiException` (event không tồn tại hoặc auth error) → exception propagate. | CalendarService mock ném `GoogleApiException` với HttpStatusCode=404.      | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                 | Expected exception   | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------- | -------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; Delete API gọi đúng 1 lần | –                    | –                    | P             |               |           |
| UTCID02 | A            | throw                           | `GoogleApiException` | –                    | P             |               |           |

---

## F072 - GoogleMeetService.Dispose

| Header           | Value                                                                                                                                        |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F072                                                                                                                                         |
| Function Name    | GoogleMeetService.Dispose                                                                                                                    |
| Total Test Cases | 1                                                                                                                                            |
| Created By       |                                                                                                                                              |
| Executed By      |                                                                                                                                              |
| Lines of Code    | 211 |
| Passed           | 1                                                                                                                                            |
| Failed           | 0                                                                                                                                            |
| Untested         | 0                                                                                                                                            |
| Count type N     | 1                                                                                                                                            |
| Count type A     | 0                                                                                                                                            |
| Count type B     | 0                                                                                                                                            |
| Test Requirement | Validate 'Dispose Google Meet service' in GoogleMeetService.Dispose, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                             | Precondition                                                          | UTCIDs  |
| ------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------- | ------- |
| Gọi Dispose() → service không throw; tài nguyên Google credentials/service được giải phóng; gọi lại Dispose không throw (idempotent). | Service đã khởi tạo với credentials hợp lệ; service chưa bị disposed. | UTCID01 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                     | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ----------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; không throw; tài nguyên freed | –                  | –                    | P             |               |           |

---

## F073 - NotificationService.SendAsync (typed)

| Header           | Value                                                                                                                                                |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F073                                                                                                                                                 |
| Function Name    | NotificationService.SendAsync (typed)                                                                                                                |
| Total Test Cases | 4                                                                                                                                                    |
| Created By       |                                                                                                                                                      |
| Executed By      |                                                                                                                                                      |
| Lines of Code    | 273 |
| Passed           | 4                                                                                                                                                    |
| Failed           | 0                                                                                                                                                    |
| Untested         | 0                                                                                                                                                    |
| Count type N     | 1                                                                                                                                                    |
| Count type A     | 3                                                                                                                                                    |
| Count type B     | 0                                                                                                                                                    |
| Test Requirement | Validate 'Send typed notification' in NotificationService.SendAsync (typed), covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                           | Precondition                                                                                               | UTCIDs  |
| ----------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------- | ------- |
| userId, message, type hợp lệ → Notification entity được lưu vào DB (AddAsync), commit, push realtime qua SignalR hub → trả về void. | NotificationRepository.AddAsync mock; UnitOfWork.CommitAsync mock; INotificationHubContext.SendAsync mock. | UTCID01 |
| userId null/rỗng → ném `ArgumentException`.                                                                                         | userId = null/"".                                                                                          | UTCID02 |
| message null/rỗng → ném `ArgumentException`.                                                                                        | message = null/"".                                                                                         | UTCID03 |
| CommitAsync ném exception → exception propagate; notification không được gửi qua hub.                                               | AddAsync thành công; CommitAsync ném DbUpdateException; verify hub.SendAsync không được gọi.               | UTCID04 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                                          | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------------------------------------ | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; AddAsync gọi 1 lần; CommitAsync gọi 1 lần; hub.SendAsync gọi 1 lần | –                   | –                    | P             |               |           |
| UTCID02 | A            | throw                                                                    | `ArgumentException` | –                    | P             |               |           |
| UTCID03 | A            | throw                                                                    | `ArgumentException` | –                    | P             |               |           |
| UTCID04 | A            | throw; hub.SendAsync không được gọi                                      | `DbUpdateException` | –                    | P             |               |           |

---

## F074 - NotificationService.SendAsync (legacy)

| Header           | Value                                                                                                                                                  |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F074                                                                                                                                                   |
| Function Name    | NotificationService.SendAsync (legacy)                                                                                                                 |
| Total Test Cases | 1                                                                                                                                                      |
| Created By       |                                                                                                                                                        |
| Executed By      |                                                                                                                                                        |
| Lines of Code    | 273 |
| Passed           | 1                                                                                                                                                      |
| Failed           | 0                                                                                                                                                      |
| Untested         | 0                                                                                                                                                      |
| Count type N     | 1                                                                                                                                                      |
| Count type A     | 0                                                                                                                                                      |
| Count type B     | 0                                                                                                                                                      |
| Test Requirement | Validate 'Send legacy notification' in NotificationService.SendAsync (legacy), covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                     | Precondition                                                                                          | UTCIDs  |
| ----------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------- | ------- |
| Legacy overload (không có type parameter) → delegate sang typed overload với type = General; behavior giống UTCID01 của F073. | NotificationRepository mock; UnitOfWork mock; hub mock; verify typed overload được gọi với đúng args. | UTCID01 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                             | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ----------------------------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; typed SendAsync được gọi với NotificationType.General | –                  | –                    | P             |               |           |

---

## F075 - PatientRoadmapGenerationService.GenerateFromDiagnosisAsync

| Header           | Value                                                                                                                                                                             |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F075                                                                                                                                                                              |
| Function Name    | PatientRoadmapGenerationService.GenerateFromDiagnosisAsync                                                                                                                        |
| Total Test Cases | 6                                                                                                                                                                                 |
| Created By       |                                                                                                                                                                                   |
| Executed By      |                                                                                                                                                                                   |
| Lines of Code    | 0 |
| Passed           | 6                                                                                                                                                                                 |
| Failed           | 0                                                                                                                                                                                 |
| Untested         | 0                                                                                                                                                                                 |
| Count type N     | 1                                                                                                                                                                                 |
| Count type A     | 5                                                                                                                                                                                 |
| Count type B     | 0                                                                                                                                                                                 |
| Test Requirement | Validate 'Generate roadmap from diagnosis' in PatientRoadmapGenerationService.GenerateFromDiagnosisAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                         | Precondition                                                                                                          | UTCIDs  |
| --------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------- | ------- |
| diagnosisId hợp lệ, diagnosis tồn tại → tạo Roadmap với các Steps theo mapping rules → commit → trả về RoadmapDto đầy đủ.         | DiagnosisRepository trả về diagnosis hợp lệ; RoadmapRepository.AddAsync mock; UnitOfWork.CommitAsync mock thành công. | UTCID01 |
| diagnosisId null/rỗng → ném `ArgumentException`.                                                                                  | diagnosisId = null/"".                                                                                                | UTCID02 |
| Không tìm thấy diagnosis → ném `NotFoundException("Diagnosis not found")` hoặc trả về null.                                       | DiagnosisRepository.GetByIdAsync trả về null.                                                                         | UTCID03 |
| Diagnosis đã có roadmap → không tạo mới, trả về roadmap hiện có hoặc throw `InvalidOperationException("Roadmap already exists")`. | DiagnosisRepository trả về diagnosis với Roadmap != null.                                                             | UTCID04 |
| Diagnosis severity = 1 (Low) → roadmap chứa steps theo template Low risk (vd. 1 tháng theo dõi).                                  | DiagnosisRepository trả về diagnosis với Severity = 1; verify Steps.Count và StepType đúng với Low risk template.     | UTCID05 |
| CommitAsync ném exception → exception propagate; roadmap entity không được lưu.                                                   | AddAsync thành công; CommitAsync ném DbUpdateException.                                                               | UTCID06 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                            | Expected exception          | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ---------------------------------------------------------- | --------------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `RoadmapDto` với Steps đúng mapping; CommitAsync gọi 1 lần | –                           | –                    | P             |               |           |
| UTCID02 | A            | throw                                                      | `ArgumentException`         | –                    | P             |               |           |
| UTCID03 | A            | throw hoặc null                                            | `NotFoundException`         | –                    | P             |               |           |
| UTCID04 | A            | existing RoadmapDto hoặc throw                             | `InvalidOperationException` | –                    | P             |               |           |
| UTCID05 | A            | `RoadmapDto` với Steps theo Low risk template              | –                           | –                    | P             |               |           |
| UTCID06 | A            | throw                                                      | `DbUpdateException`         | –                    | P             |               |           |

---

## F076 - PayOSService.CreatePaymentLinkAsync

| Header           | Value                                                                                                                                                |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F076                                                                                                                                                 |
| Function Name    | PayOSService.CreatePaymentLinkAsync                                                                                                                  |
| Total Test Cases | 5                                                                                                                                                    |
| Created By       |                                                                                                                                                      |
| Executed By      |                                                                                                                                                      |
| Lines of Code    | 318 |
| Passed           | 5                                                                                                                                                    |
| Failed           | 0                                                                                                                                                    |
| Untested         | 0                                                                                                                                                    |
| Count type N     | 1                                                                                                                                                    |
| Count type A     | 4                                                                                                                                                    |
| Count type B     | 0                                                                                                                                                    |
| Test Requirement | Validate 'Create PayOS payment link' in PayOSService.CreatePaymentLinkAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                    | Precondition                                                                                | UTCIDs  |
| ---------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------- | ------- |
| amount > 0, orderCode, items hợp lệ → PayOS SDK tạo link thành công → trả về PaymentLinkResponse với checkoutUrl không rỗng. | PayOS SDK mock trả về CreatePaymentResult với Code="00" và CheckoutUrl; credentials hợp lệ. | UTCID01 |
| amount = 0 hoặc âm → ném `ArgumentException("Amount must be positive")`.                                                     | amount = 0 hoặc -100.                                                                       | UTCID02 |
| orderCode trùng lặp → PayOS trả về lỗi (Code != "00") → throw hoặc trả về Failure.                                           | PayOS SDK mock trả về Code="01" (duplicate); ErrorMessage = "Order code already exists".    | UTCID03 |
| PayOS SDK ném exception (network/auth) → exception propagate.                                                                | PayOS SDK mock ném `PayOSException` hoặc `HttpRequestException`.                            | UTCID04 |
| Signature verification (checksum) fail khi nhận response → throw `InvalidOperationException("Invalid PayOS checksum")`.      | PayOS SDK trả về response với checksum không khớp; verify checksum logic.                   | UTCID05 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                               | Expected exception          | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------------------------------------- | --------------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `PaymentLinkResponse` với CheckoutUrl != null | –                           | –                    | P             |               |           |
| UTCID02 | A            | throw                                         | `ArgumentException`         | –                    | P             |               |           |
| UTCID03 | A            | throw hoặc Failure response                   | –                           | –                    | P             |               |           |
| UTCID04 | A            | throw                                         | `HttpRequestException`      | –                    | P             |               |           |
| UTCID05 | A            | throw                                         | `InvalidOperationException` | –                    | P             |               |           |

---

## F077 - PayOSService.GetPaymentStatusAsync

| Header           | Value                                                                                                                                              |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F077                                                                                                                                               |
| Function Name    | PayOSService.GetPaymentStatusAsync                                                                                                                 |
| Total Test Cases | 3                                                                                                                                                  |
| Created By       |                                                                                                                                                    |
| Executed By      |                                                                                                                                                    |
| Lines of Code    | 318 |
| Passed           | 3                                                                                                                                                  |
| Failed           | 0                                                                                                                                                  |
| Untested         | 0                                                                                                                                                  |
| Count type N     | 1                                                                                                                                                  |
| Count type A     | 2                                                                                                                                                  |
| Count type B     | 0                                                                                                                                                  |
| Test Requirement | Validate 'Get PayOS payment status' in PayOSService.GetPaymentStatusAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                  | Precondition                                                             | UTCIDs  |
| ---------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------ | ------- |
| orderCode hợp lệ, tìm thấy → PayOS SDK trả về PaymentInfo với Status = "PAID" → trả về PaymentStatus.Paid. | PayOS SDK mock trả về GetPaymentInfoResult với Status="PAID"; Code="00". | UTCID01 |
| orderCode hợp lệ, không tìm thấy (Code != "00") → throw hoặc trả về null.                                  | PayOS SDK mock trả về Code="01" (not found).                             | UTCID02 |
| PayOS SDK ném exception → exception propagate.                                                             | PayOS SDK mock ném `HttpRequestException`.                               | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return      | Expected exception     | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | -------------------- | ---------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `PaymentStatus.Paid` | –                      | –                    | P             |               |           |
| UTCID02 | A            | null hoặc throw      | –                      | –                    | P             |               |           |
| UTCID03 | A            | throw                | `HttpRequestException` | –                    | P             |               |           |

---

## F078 - PayOSService.VerifyWebhookSignatureAsync

| Header           | Value                                                                                                                                                          |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F078                                                                                                                                                           |
| Function Name    | PayOSService.VerifyWebhookSignatureAsync                                                                                                                       |
| Total Test Cases | 2                                                                                                                                                              |
| Created By       |                                                                                                                                                                |
| Executed By      |                                                                                                                                                                |
| Lines of Code    | 318 |
| Passed           | 2                                                                                                                                                              |
| Failed           | 0                                                                                                                                                              |
| Untested         | 0                                                                                                                                                              |
| Count type N     | 1                                                                                                                                                              |
| Count type A     | 1                                                                                                                                                              |
| Count type B     | 0                                                                                                                                                              |
| Test Requirement | Validate 'Verify PayOS webhook signature' in PayOSService.VerifyWebhookSignatureAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                             | Precondition                                                                                   | UTCIDs  |
| ------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------- | ------- |
| Webhook payload với signature hợp lệ (HMAC-SHA256 khớp checksumKey) → trả về `true`.  | Payload được tạo đúng với checksumKey từ settings; signature = HMAC-SHA256(data, checksumKey). | UTCID01 |
| Webhook payload với signature không hợp lệ (bị tamper hoặc sai key) → trả về `false`. | Payload có signature không khớp; verify trả về false (không throw).                            | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `true`          | –                  | –                    | P             |               |           |
| UTCID02 | A            | `false`         | –                  | –                    | P             |               |           |

---

## F079 - PayOSService.CancelPaymentAsync

| Header           | Value                                                                                                                                       |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F079                                                                                                                                        |
| Function Name    | PayOSService.CancelPaymentAsync                                                                                                             |
| Total Test Cases | 3                                                                                                                                           |
| Created By       |                                                                                                                                             |
| Executed By      |                                                                                                                                             |
| Lines of Code    | 318 |
| Passed           | 3                                                                                                                                           |
| Failed           | 0                                                                                                                                           |
| Untested         | 0                                                                                                                                           |
| Count type N     | 1                                                                                                                                           |
| Count type A     | 2                                                                                                                                           |
| Count type B     | 0                                                                                                                                           |
| Test Requirement | Validate 'Cancel PayOS payment' in PayOSService.CancelPaymentAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                                     | Precondition                                                           | UTCIDs  |
| ----------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------- | ------- |
| orderCode hợp lệ, payment chưa hoàn thành → PayOS SDK hủy thành công → trả về CancelPaymentResponse với Status = "CANCELLED". | PayOS SDK mock trả về Code="00", Status="CANCELLED"; orderCode hợp lệ. | UTCID01 |
| orderCode không tìm thấy → PayOS SDK trả về lỗi → throw hoặc Failure response.                                                | PayOS SDK mock trả về Code="01"; ErrorMessage = "Order not found".     | UTCID02 |
| PayOS SDK ném exception → exception propagate.                                                                                | PayOS SDK mock ném `HttpRequestException`.                             | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                | Expected exception     | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ---------------------------------------------- | ---------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `CancelPaymentResponse` với Status="CANCELLED" | –                      | –                    | P             |               |           |
| UTCID02 | A            | throw hoặc Failure response                    | –                      | –                    | P             |               |           |
| UTCID03 | A            | throw                                          | `HttpRequestException` | –                    | P             |               |           |

---

## F080 - SupabaseStorageService.SaveFileAsync

| Header           | Value                                                                                                                                             |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F080                                                                                                                                              |
| Function Name    | SupabaseStorageService.SaveFileAsync                                                                                                              |
| Total Test Cases | 5                                                                                                                                                 |
| Created By       |                                                                                                                                                   |
| Executed By      |                                                                                                                                                   |
| Lines of Code    | 204 |
| Passed           | 5                                                                                                                                                 |
| Failed           | 0                                                                                                                                                 |
| Untested         | 0                                                                                                                                                 |
| Count type N     | 1                                                                                                                                                 |
| Count type A     | 4                                                                                                                                                 |
| Count type B     | 0                                                                                                                                                 |
| Test Requirement | Validate 'Save file to Supabase' in SupabaseStorageService.SaveFileAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                  | Precondition                                                                                        | UTCIDs  |
| ------------------------------------------------------------------------------------------ | --------------------------------------------------------------------------------------------------- | ------- |
| stream và path hợp lệ → Supabase Storage upload thành công → trả về public URL không rỗng. | Supabase client mock trả về UploadResponse với Key = "path/to/file"; GetPublicUrl trả về valid URL. | UTCID01 |
| stream null → ném `ArgumentNullException`.                                                 | stream = null.                                                                                      | UTCID02 |
| path null/rỗng → ném `ArgumentException`.                                                  | path = null/"".                                                                                     | UTCID03 |
| Supabase client ném exception (auth error, network) → exception propagate.                 | Supabase client mock ném `StorageException` hoặc `HttpRequestException`.                            | UTCID04 |
| Upload thành công nhưng GetPublicUrl trả về null → trả về null URL hoặc throw.             | UploadResponse.Key hợp lệ; GetPublicUrl mock trả về null.                                           | UTCID05 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------ | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | public URL string (không rỗng) | –                       | –                    | P             |               |           |
| UTCID02 | A            | throw                          | `ArgumentNullException` | –                    | P             |               |           |
| UTCID03 | A            | throw                          | `ArgumentException`     | –                    | P             |               |           |
| UTCID04 | A            | throw                          | `StorageException`      | –                    | P             |               |           |
| UTCID05 | A            | null URL hoặc throw            | –                       | –                    | P             |               |           |

---

## F081 - SupabaseStorageService.DeleteFile

| Header           | Value                                                                                                                                              |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F081                                                                                                                                               |
| Function Name    | SupabaseStorageService.DeleteFile                                                                                                                  |
| Total Test Cases | 3                                                                                                                                                  |
| Created By       |                                                                                                                                                    |
| Executed By      |                                                                                                                                                    |
| Lines of Code    | 204 |
| Passed           | 3                                                                                                                                                  |
| Failed           | 0                                                                                                                                                  |
| Untested         | 0                                                                                                                                                  |
| Count type N     | 1                                                                                                                                                  |
| Count type A     | 2                                                                                                                                                  |
| Count type B     | 0                                                                                                                                                  |
| Test Requirement | Validate 'Delete file from Supabase' in SupabaseStorageService.DeleteFile, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                 | Precondition                                                                                 | UTCIDs  |
| ----------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------- | ------- |
| path hợp lệ, file tồn tại → Supabase Storage xóa thành công → trả về void (no exception). | Supabase client.Storage.From(bucket).Remove(paths) mock thành công; path = "valid/path.png". | UTCID01 |
| path null/rỗng → ném `ArgumentException`.                                                 | path = null/"".                                                                              | UTCID02 |
| Supabase client ném exception (file không tồn tại hoặc auth error) → exception propagate. | Supabase client mock ném `StorageException` khi Remove.                                      | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return             | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------------------- | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; Remove gọi đúng 1 lần | –                   | –                    | P             |               |           |
| UTCID02 | A            | throw                       | `ArgumentException` | –                    | P             |               |           |
| UTCID03 | A            | throw                       | `StorageException`  | –                    | P             |               |           |

---

## F082 - SupabaseStorageService.FileExists

| Header           | Value                                                                                                                                               |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F082                                                                                                                                                |
| Function Name    | SupabaseStorageService.FileExists                                                                                                                   |
| Total Test Cases | 3                                                                                                                                                   |
| Created By       |                                                                                                                                                     |
| Executed By      |                                                                                                                                                     |
| Lines of Code    | 204 |
| Passed           | 3                                                                                                                                                   |
| Failed           | 0                                                                                                                                                   |
| Untested         | 0                                                                                                                                                   |
| Count type N     | 1                                                                                                                                                   |
| Count type A     | 2                                                                                                                                                   |
| Count type B     | 0                                                                                                                                                   |
| Test Requirement | Validate 'Check Supabase file exists' in SupabaseStorageService.FileExists, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                          | Precondition                                                                   | UTCIDs  |
| -------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------ | ------- |
| path hợp lệ, file tồn tại trong bucket → Supabase List trả về ≥1 object khớp path → trả về `true`. | Supabase client.Storage.List mock trả về danh sách với 1 FileObject khớp path. | UTCID01 |
| path hợp lệ, file không tồn tại → Supabase List trả về danh sách rỗng → trả về `false`.            | Supabase client.Storage.List mock trả về danh sách rỗng.                       | UTCID02 |
| path null/rỗng → ném `ArgumentException`.                                                          | path = null/"".                                                                | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------- | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `true`          | –                   | –                    | P             |               |           |
| UTCID02 | A            | `false`         | –                   | –                    | P             |               |           |
| UTCID03 | A            | throw           | `ArgumentException` | –                    | P             |               |           |

---

## F083 - SystemSettingService.GetSettingAsync

| Header           | Value                                                                                                                                          |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F083                                                                                                                                           |
| Function Name    | SystemSettingService.GetSettingAsync                                                                                                           |
| Total Test Cases | 2                                                                                                                                              |
| Created By       |                                                                                                                                                |
| Executed By      |                                                                                                                                                |
| Lines of Code    | 72 |
| Passed           | 2                                                                                                                                              |
| Failed           | 0                                                                                                                                              |
| Untested         | 0                                                                                                                                              |
| Count type N     | 1                                                                                                                                              |
| Count type A     | 1                                                                                                                                              |
| Count type B     | 0                                                                                                                                              |
| Test Requirement | Validate 'Get setting by key' in SystemSettingService.GetSettingAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                          | Precondition                                                                                  | UTCIDs  |
| ---------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------- | ------- |
| key hợp lệ, setting tồn tại → trả về `SystemSetting` entity với Key và Value đúng. | SystemSettingRepository.GetByKeyAsync mock trả về entity với Key="AppName", Value="AuraEyes". | UTCID01 |
| key hợp lệ, setting không tồn tại → trả về `null`.                                 | SystemSettingRepository.GetByKeyAsync mock trả về null.                                       | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                        | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | -------------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `SystemSetting` với Key="AppName" đúng | –                  | –                    | P             |               |           |
| UTCID02 | A            | `null`                                 | –                  | –                    | P             |               |           |

---

## F084 - SystemSettingService.GetAllSettingsAsync

| Header           | Value                                                                                                                                            |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F084                                                                                                                                             |
| Function Name    | SystemSettingService.GetAllSettingsAsync                                                                                                         |
| Total Test Cases | 2                                                                                                                                                |
| Created By       |                                                                                                                                                  |
| Executed By      |                                                                                                                                                  |
| Lines of Code    | 72 |
| Passed           | 2                                                                                                                                                |
| Failed           | 0                                                                                                                                                |
| Untested         | 0                                                                                                                                                |
| Count type N     | 1                                                                                                                                                |
| Count type A     | 1                                                                                                                                                |
| Count type B     | 0                                                                                                                                                |
| Test Requirement | Validate 'Get all settings' in SystemSettingService.GetAllSettingsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                      | Precondition                                                          | UTCIDs  |
| ------------------------------------------------------------------------------ | --------------------------------------------------------------------- | ------- |
| Có settings trong DB → trả về tất cả `IList<SystemSetting>` với đúng số lượng. | SystemSettingRepository.GetAllAsync mock trả về IList với 5 settings. | UTCID01 |
| DB không có setting nào → trả về danh sách rỗng.                               | SystemSettingRepository.GetAllAsync mock trả về IList rỗng.           | UTCID02 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                    | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ---------------------------------- | ------------------ | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `IList<SystemSetting>` với 5 items | –                  | –                    | P             |               |           |
| UTCID02 | A            | `IList<SystemSetting>` rỗng        | –                  | –                    | P             |               |           |

---

## F085 - SystemSettingService.UpdateSettingsAsync

| Header           | Value                                                                                                                                           |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F085                                                                                                                                            |
| Function Name    | SystemSettingService.UpdateSettingsAsync                                                                                                        |
| Total Test Cases | 4                                                                                                                                               |
| Created By       |                                                                                                                                                 |
| Executed By      |                                                                                                                                                 |
| Lines of Code    | 72 |
| Passed           | 4                                                                                                                                               |
| Failed           | 0                                                                                                                                               |
| Untested         | 0                                                                                                                                               |
| Count type N     | 1                                                                                                                                               |
| Count type A     | 3                                                                                                                                               |
| Count type B     | 0                                                                                                                                               |
| Test Requirement | Validate 'Update settings' in SystemSettingService.UpdateSettingsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition                                                                                                | Precondition                                                                                   | UTCIDs  |
| -------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------- | ------- |
| Dictionary<string,string> với N keys hợp lệ → cập nhật từng key trong DB (upsert), commit → trả về void. | SystemSettingRepository mock hỗ trợ GetByKeyAsync và UpdateAsync; CommitAsync mock thành công. | UTCID01 |
| Dictionary rỗng → không có update nào → commit không cần thiết hoặc commit 0 changes → trả về void.      | settings = new Dictionary<string,string>() rỗng; verify UpdateAsync không được gọi.            | UTCID02 |
| Một key trong dictionary không tồn tại trong DB → tạo mới setting (upsert behavior) → commit thành công. | GetByKeyAsync trả về null cho 1 key; AddAsync mock thành công.                                 | UTCID03 |
| CommitAsync ném exception → exception propagate; partial updates có thể xảy ra.                          | UpdateAsync thành công; CommitAsync ném DbUpdateException.                                     | UTCID04 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                             | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ------------------------------------------- | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; UpdateAsync gọi N lần; CommitAsync 1x | –                   | –                    | P             |               |           |
| UTCID02 | A            | void; UpdateAsync không được gọi            | –                   | –                    | P             |               |           |
| UTCID03 | A            | void; AddAsync được gọi cho key mới         | –                   | –                    | P             |               |           |
| UTCID04 | A            | throw                                       | `DbUpdateException` | –                    | P             |               |           |

---

## F086 - ClinicVisitService.ProcessPaymentCompletionAsync

| Header           | Value                                                                                                                                            |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F086                                                                                                                                             |
| Function Name    | ClinicVisitService.ProcessPaymentCompletionAsync                                                                                                 |
| Total Test Cases | 4                                                                                                                                                |
| Created By       |                                                                                                                                                  |
| Executed By      |                                                                                                                                                  |
| Lines of Code    | 163 |
| Passed           | 4                                                                                                                                                |
| Failed           | 0                                                                                                                                                |
| Untested         | 0                                                                                                                                                |
| Count type N     | 1                                                                                                                                                |
| Count type A     | 3                                                                                                                                                |
| Count type B     | 0                                                                                                                                                |
| Test Requirement | Validate 'Process payment completion' in ClinicVisitService.ProcessPaymentCompletionAsync, covering appointment completion and session creation. |

### Condition Matrix

| Condition                                                                                                                           | Precondition                                                                                                   | UTCIDs  |
| ----------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------- | ------- |
| orderCode hợp lệ, ClinicVisitOrder tồn tại, chưa thanh toán → cập nhật Status = Paid, tạo ClinicVisitSession, commit → trả về void. | OrderRepository trả về order với Status=Pending; SessionRepository.AddAsync mock; CommitAsync mock thành công. | UTCID01 |
| orderCode không tìm thấy → throw `NotFoundException("Order not found")` hoặc trả về Failure.                                        | OrderRepository.GetByOrderCodeAsync trả về null.                                                               | UTCID02 |
| Order đã ở trạng thái Paid (idempotent) → bỏ qua (no-op) hoặc trả về void mà không tạo thêm session.                                | OrderRepository trả về order với Status=Paid; verify SessionRepository.AddAsync không được gọi thêm.           | UTCID03 |
| CommitAsync ném exception → exception propagate; order status và session không được persist.                                        | Order hợp lệ; SessionRepository.AddAsync thành công; CommitAsync ném DbUpdateException.                        | UTCID04 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                                                  | Expected exception  | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | ---------------------------------------------------------------- | ------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | void; Order.Status=Paid; Session được tạo; CommitAsync gọi 1 lần | –                   | –                    | P             |               |           |
| UTCID02 | A            | throw hoặc Failure                                               | `NotFoundException` | –                    | P             |               |           |
| UTCID03 | A            | void; không tạo thêm session; idempotent                         | –                   | –                    | P             |               |           |
| UTCID04 | A            | throw                                                            | `DbUpdateException` | –                    | P             |               |           |

---

## F087 - PatientScreeningPdfService.GenerateScreeningReportPdf

| Header           | Value                                                                                                                                        |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F087                                                                                                                                         |
| Function Name    | PatientScreeningPdfService.GenerateScreeningReportPdf                                                                                        |
| Total Test Cases | 3                                                                                                                                            |
| Created By       |                                                                                                                                              |
| Executed By      |                                                                                                                                              |
| Lines of Code    | 617 |
| Passed           | 3                                                                                                                                            |
| Failed           | 0                                                                                                                                            |
| Untested         | 0                                                                                                                                            |
| Count type N     | 1                                                                                                                                            |
| Count type A     | 2                                                                                                                                            |
| Count type B     | 0                                                                                                                                            |
| Test Requirement | Validate 'Generate patient screening PDF' in PatientScreeningPdfService.GenerateScreeningReportPdf, covering PDF layout and data population. |

### Condition Matrix

| Condition                                                                                                                                       | Precondition                                                                                                               | UTCIDs  |
| ----------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------- | ------- |
| screeningData hợp lệ (PatientName, ScreeningDate, Severity, DoctorName đầy đủ) → GeneratePdf thành công → trả về byte[] không rỗng (valid PDF). | QuestPDF Document.GeneratePdf mock/real; screeningData seed với đầy đủ fields.                                             | UTCID01 |
| screeningData null → ném `ArgumentNullException`.                                                                                               | screeningData = null.                                                                                                      | UTCID02 |
| PDF byte[] trả về bắt đầu bằng PDF header `%PDF-` (kiểm tra format).                                                                            | screeningData hợp lệ; sử dụng QuestPDF thật (không mock); verify response.StartsWith(new byte[]{"25","50","44","46","-"}). | UTCID03 |

### Result Matrix

| UTCID   | Type (N/A/B) | Expected return                               | Expected exception      | Expected log message | Passed/Failed | Executed Date | Defect ID |
| ------- | ------------ | --------------------------------------------- | ----------------------- | -------------------- | ------------- | ------------- | --------- |
| UTCID01 | N            | `byte[]` không rỗng, Length > 0               | –                       | –                    | P             |               |           |
| UTCID02 | A            | throw                                         | `ArgumentNullException` | –                    | P             |               |           |
| UTCID03 | A            | `byte[]` bắt đầu bằng PDF magic bytes `%PDF-` | –                       | –                    | P             |               |           |

---
