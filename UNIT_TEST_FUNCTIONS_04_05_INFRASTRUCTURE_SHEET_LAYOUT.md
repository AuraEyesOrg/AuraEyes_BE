# Infrastructure Unit Test - Sheet Layout Ready

Last updated: 13/04/2026
Source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

## Hướng dẫn nhanh (tiếng Việt — để copy sang Excel)

**Hai bảng trong mỗi khối `## Fxxx` khác nhau thế nào?**

| Bảng | Mục đích | Một dòng = |
|------|----------|------------|
| **Condition Matrix** | Tóm tắt kịch bản, gom nhóm UTCID | Có thể **nhiều UTCID** (cột `UTCIDs`: `UTCID01-UTCID02`) |
| **Result Matrix** | Chi tiết **từng** case để trace / assert | **Đúng 1 UTCID** mỗi hàng |

**Quy tắc bắt buộc — không bịa**

- **Expected return / Expected exception / Expected log message** chỉ ghi khi **có chứng cứ**: assert hoặc `Verify` trong `tests/Infrastructure.UnitTests`, hoặc bạn đã **xác minh có chủ đích** (ghi nguồn: tên test / dòng assert).
- **Không** điền theo cảm tưởng hay “hợp lý theo code” nếu test không kiểm tra — để **trống** hoặc ghi rõ `n/a (chưa assert trong test)`.
- Cột **log**: chỉ ghi khi test thật sự assert log; không có thì **trống**.

**Làm theo 3 bước**

1. Mở đúng function: tìm `## F001` … `## F096` (cùng mã **F** với bảng catalog trong checklist).
2. Đọc **Condition Matrix** để nắm ý chung (phần sau `->` là kết quả *tóm tắt*).
3. Điền **Result Matrix**: mỗi hàng `UTCID01`, `UTCID02`, … — **ưu tiên** trích từ test; checklist chỉ là gợi ý khi đã khớp với assert. Phần sau `->` trong checklist có thể **tách** vào 3 cột nếu **đã đối chiếu** với test:
   - **Expected return** — service trả gì (Success/Failure, message, field như `UserId`, …).
   - **Expected exception** — có ném exception không, loại gì; không thì để trống hoặc ghi `none`.
   - **Expected log message** — có assert log không (level + đoạn text); không thì để trống.

**Sau khi chạy test** mới điền: **Passed/Failed**, **Executed Date**, **Defect ID**.

**Excel của bạn (Confirm: 3 hàng Return / Exception / Log × nhiều cột UTCID)**  
Trong file markdown là **3 cột** trên **một hàng** (một UTCID). Nội dung giống nhau, chỉ khác xoay bảng: copy từ markdown sang Excel rồi **Transpose** (dán chuyển vị), hoặc điền tay theo cùng một ý.

**Ví dụ cụ thể — F001 / `RegisterPatientAsync`**

- **Sheet (F001) đã điền theo test thực tế:** cả 10 UTCID map `AuthServiceTests.RegisterPatientAsync_WhenDependenciesMissing_ShouldReturnFailure` — *Expected return* = Failure + `"An error occurred during registration"`. Phần bullet tóm tắt khác trong CHECKLIST (email trùng, rollback, …) là kịch bản service đầy đủ, chưa có test tương ứng trong class đó.

**Copy markdown → bảng Excel:** dùng công cụ chuyển markdown table sang grid (ví dụ tableconvert) rồi dán vào sheet, hoặc copy trực tiếp từng bảng.

---

## How to copy into Sheet / Excel (English)

- **Do not invent.** Put something in **Expected return / exception / log** only when **evidence exists**: an assert or `Verify` in `tests/Infrastructure.UnitTests`, or you explicitly verified and cite the test name. Otherwise leave **empty** or write `n/a (not asserted in test)`. **Log** only if the test asserts logging.
- Each block `## F001` … `## F096` is **one function**; copy the whole block into one sheet or one table range.
- In the Header table **Value** column, fill in **Created By**, **Executed By**, **Lines of Code**, **Passed / Failed / Untested**, **Count type N / A / B** after you run tests.
- **Condition Matrix** = high-level summary; one row may list **several** UTCIDs in the third column.
- **Result Matrix** = **one row per UTCID**; align **Expected** columns with real tests first; checklist bullets are hints only after you match them to asserts.
- **Passed/Failed**, **Executed Date**, **Defect ID** — after execution only.
- Excel **Confirm** uses **rows** (Return / Exception / Log) × UTCID **columns**; markdown uses **columns** on one row per UTCID — same content, transpose when pasting.
- Empty cells are **placeholders** (unknown / not asserted).

---

## F001 - AuthService.RegisterPatientAsync

| Header | Value |
|---|---|
| Function Code | F001 |
| Function Name | AuthService.RegisterPatientAsync |
| Total Test Cases | 10 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| `CreateServiceForValidationOnly` (null infrastructure) -> `RegisterPatientAsync` fails immediately with generic registration error. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |

*All UTCIDs: `AuthServiceTests.RegisterPatientAsync_WhenDependenciesMissing_ShouldReturnFailure` (10× `InlineData`) — same assertion.*

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.Failure; Errors contains "An error occurred during registration" | none |  |  |  |  |
| UTCID02 | A | Result.Failure; Errors contains "An error occurred during registration" | none |  |  |  |  |
| UTCID03 | A | Result.Failure; Errors contains "An error occurred during registration" | none |  |  |  |  |
| UTCID04 | A | Result.Failure; Errors contains "An error occurred during registration" | none |  |  |  |  |
| UTCID05 | A | Result.Failure; Errors contains "An error occurred during registration" | none |  |  |  |  |
| UTCID06 | A | Result.Failure; Errors contains "An error occurred during registration" | none |  |  |  |  |
| UTCID07 | A | Result.Failure; Errors contains "An error occurred during registration" | none |  |  |  |  |
| UTCID08 | A | Result.Failure; Errors contains "An error occurred during registration" | none |  |  |  |  |
| UTCID09 | A | Result.Failure; Errors contains "An error occurred during registration" | none |  |  |  |  |
| UTCID10 | A | Result.Failure; Errors contains "An error occurred during registration" | none |  |  |  |  |

---
## F002 - AuthService.LookupAccountByCitizenIdAsync

| Header | Value |
|---|---|
| Function Code | F002 |
| Function Name | AuthService.LookupAccountByCitizenIdAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate citizen ID lookup logic for account discovery. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Citizen ID found -> Return masked email. | Account exists in DB | UTCID01 |
| Citizen ID not found -> Return failure. | No account matches | UTCID02 |
| Citizen ID invalid format -> Return validation error. | Input fails regex/length | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success; Masked email (e.g., r***@gmail.com) | none | | | | |
| UTCID02 | A | Failure; "Account not found" | none | | | | |
| UTCID03 | A | Failure; Validation errors | none | | | | |

---
## F003 - AuthService.GoogleLoginAsync

| Header | Value |
|---|---|
| Function Code | F003 |
| Function Name | AuthService.GoogleLoginAsync |
| Total Test Cases | 10 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 3 |
| Count type A | 6 |
| Count type B | 1 |
| Test Requirement | Validate Google OAuth login flow and user mapping. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Invalid JWT -> Unauthorized. | External provider returns invalid token | UTCID01 |
| Missing email in payload -> Failure. | Payload filtered by provider | UTCID02 |
| User deleted/inactive -> Unauthorized. | Account state in DB | UTCID03-UTCID04 |
| Existing user, unconfirmed -> Confirm and login. | Account matches email | UTCID05 |
| Existing user, 2FA enabled -> 2FA Required. | Account matches email | UTCID06 |
| New user -> Create patient and login. | No account matches email | UTCID07 |
| DB/Identity error -> Failure. | System failure during commit | UTCID08 |
| Token expired -> Unauthorized. | JWT exp claim passed | UTCID09 |
| Linking policy violation -> Failure. | Security constraint | UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Unauthorized | none | | | | |
| UTCID02 | A | Failure | none | | | | |
| UTCID03 | A | Unauthorized | none | | | | |
| UTCID04 | A | Unauthorized | none | | | | |
| UTCID05 | N | Success; AuthResponse | none | | | | |
| UTCID06 | N | Success; TwoFactorRequired | none | | | | |
| UTCID07 | N | Success; AuthResponse | none | | | | |
| UTCID08 | A | Failure | none | | | | |
| UTCID09 | B | Unauthorized | none | | | | |
| UTCID10 | A | Failure | none | | | | |

---
## F003 - AuthService.RegisterOrganisationAsync

| Header | Value |
|---|---|
| Function Code | F003 |
| Function Name | AuthService.RegisterOrganisationAsync |
| Total Test Cases | 20 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Onboarding service returns Success -> AuthService returns correct Success payload. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |
| Onboarding service returns Conflict -> AuthService preserves conflict. | Valid dependencies and data setup for this scenario | UTCID11-UTCID20 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.Success; Data.Email matches request; Data.Message from onboarding stub (`RegisterOrganisationAsync_ShouldReturnServiceResult`) | none |  |  |  |  |
| UTCID02 | A | Result.Success; Data.Email matches request; Data.Message from onboarding stub (`RegisterOrganisationAsync_ShouldReturnServiceResult`) | none |  |  |  |  |
| UTCID03 | A | Result.Success; Data.Email matches request; Data.Message from onboarding stub (`RegisterOrganisationAsync_ShouldReturnServiceResult`) | none |  |  |  |  |
| UTCID04 | A | Result.Success; Data.Email matches request; Data.Message from onboarding stub (`RegisterOrganisationAsync_ShouldReturnServiceResult`) | none |  |  |  |  |
| UTCID05 | A | Result.Success; Data.Email matches request; Data.Message from onboarding stub (`RegisterOrganisationAsync_ShouldReturnServiceResult`) | none |  |  |  |  |
| UTCID06 | A | Result.Success; Data.Email matches request; Data.Message from onboarding stub (`RegisterOrganisationAsync_ShouldReturnServiceResult`) | none |  |  |  |  |
| UTCID07 | A | Result.Success; Data.Email matches request; Data.Message from onboarding stub (`RegisterOrganisationAsync_ShouldReturnServiceResult`) | none |  |  |  |  |
| UTCID08 | A | Result.Success; Data.Email matches request; Data.Message from onboarding stub (`RegisterOrganisationAsync_ShouldReturnServiceResult`) | none |  |  |  |  |
| UTCID09 | A | Result.Success; Data.Email matches request; Data.Message from onboarding stub (`RegisterOrganisationAsync_ShouldReturnServiceResult`) | none |  |  |  |  |
| UTCID10 | A | Result.Success; Data.Email matches request; Data.Message from onboarding stub (`RegisterOrganisationAsync_ShouldReturnServiceResult`) | none |  |  |  |  |
| UTCID11 | A | Result.IsConflict true; Errors contains "duplicate" (`WhenOnboardingReturnsConflict`) | none |  |  |  |  |
| UTCID12 | A | Result.IsConflict true; Errors contains "duplicate" (`WhenOnboardingReturnsConflict`) | none |  |  |  |  |
| UTCID13 | A | Result.IsConflict true; Errors contains "duplicate" (`WhenOnboardingReturnsConflict`) | none |  |  |  |  |
| UTCID14 | A | Result.IsConflict true; Errors contains "duplicate" (`WhenOnboardingReturnsConflict`) | none |  |  |  |  |
| UTCID15 | A | Result.IsConflict true; Errors contains "duplicate" (`WhenOnboardingReturnsConflict`) | none |  |  |  |  |
| UTCID16 | A | Result.IsConflict true; Errors contains "duplicate" (`WhenOnboardingReturnsConflict`) | none |  |  |  |  |
| UTCID17 | A | Result.IsConflict true; Errors contains "duplicate" (`WhenOnboardingReturnsConflict`) | none |  |  |  |  |
| UTCID18 | A | Result.IsConflict true; Errors contains "duplicate" (`WhenOnboardingReturnsConflict`) | none |  |  |  |  |
| UTCID19 | A | Result.IsConflict true; Errors contains "duplicate" (`WhenOnboardingReturnsConflict`) | none |  |  |  |  |
| UTCID20 | A | Result.IsConflict true; Errors contains "duplicate" (`WhenOnboardingReturnsConflict`) | none |  |  |  |  |

---
## F004 - AuthService.GoogleLoginAsync

| Header | Value |
|---|---|
| Function Code | F004 |
| Function Name | AuthService.GoogleLoginAsync |
| Total Test Cases | 10 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Google credential invalid jwt -> Unauthorized. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.IsUnauthorized true; Errors contains "Invalid Google token" | none |  |  |  |  |
| Function Name | AuthService.LoginAsync |
| Total Test Cases | 12 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 3 |
| Count type A | 9 |
| Count type B | 0 |
| Test Requirement | Validate credentials and account status during login. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Invalid email/password -> Unauthorized. | Wrong credentials | UTCID01, UTCID07 |
| Deleted/Inactive user -> Unauthorized. | Account state | UTCID02-UTCID03 |
| Lockout state -> Unauthorized. | Too many failed attempts | UTCID04-UTCID05 |
| Unconfirmed email -> Unauthorized. | Account matches but unconfirmed | UTCID06 |
| Rejected ClinicStaff -> Unauthorized. | VerificationStatus=Rejected | UTCID08 |
| 2FA required -> TwoFactorRequired. | Account has 2FA enabled | UTCID09-UTCID10 |
| Success -> AuthResponse. | Valid credentials and active state | UTCID11 |
| System error -> Failure. | Internal exception | UTCID12 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Unauthorized | none | | | | |
| UTCID02 | A | Unauthorized | none | | | | |
| UTCID03 | A | Unauthorized | none | | | | |
| UTCID04 | A | Unauthorized | none | | | | |
| UTCID05 | A | Unauthorized | none | | | | |
| UTCID06 | A | Unauthorized | none | | | | |
| UTCID07 | A | Unauthorized | none | | | | |
| UTCID08 | A | Unauthorized | none | | | | |
| UTCID09 | N | Success; TwoFactorRequired | none | | | | |
| UTCID10 | N | Success; TwoFactorRequired | none | | | | |
| UTCID11 | N | Success; AuthResponse | none | | | | |
| UTCID12 | A | Failure | none | | | | |

---
## F005 - AuthService.VerifyTwoFactorLoginAsync

| Header | Value |
|---|---|
| Function Code | F005 |
| Function Name | AuthService.VerifyTwoFactorLoginAsync |
| Total Test Cases | 10 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 6 |
| Count type B | 2 |
| Test Requirement | Validate 2FA codes and recovery tokens. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User invalid/deleted/inactive -> Unauthorized. | Account state | UTCID01 |
| 2FA not enabled -> Failure. | Security mismatch | UTCID02 |
| Invalid authenticator/recovery code -> Unauthorized. | Wrong code | UTCID03-UTCID04 |
| Valid recovery code -> Success. | Token match | UTCID05 |
| Valid authenticator code -> Success. | TOTP match | UTCID06, UTCID10 |
| System error -> Failure. | Internal exception | UTCID07 |
| Expired code -> Unauthorized. | Time window passed | UTCID08 |
| Empty code -> Failure. | Validation | UTCID09 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Unauthorized | none | | | | |
| UTCID02 | A | Failure | none | | | | |
| UTCID03 | A | Unauthorized | none | | | | |
| UTCID04 | A | Unauthorized | none | | | | |
| UTCID05 | N | Success; AuthResponse | none | | | | |
| UTCID06 | N | Success; AuthResponse | none | | | | |
| UTCID07 | A | Failure | none | | | | |
| UTCID08 | B | Unauthorized | none | | | | |
| UTCID09 | B | Failure | none | | | | |
| UTCID10 | N | Success; AuthResponse | none | | | | |

---
## F006 - AuthService.RefreshTokenAsync

| Header | Value |
|---|---|
| Function Code | F006 |
| Function Name | AuthService.RefreshTokenAsync |
| Total Test Cases | 10 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 7 |
| Count type B | 1 |
| Test Requirement | Validate token rotation and security. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Invalid access token (jti/uid missing) -> Unauthorized. | Malformed token | UTCID01 |
| Refresh token not found -> Unauthorized. | Invalid hash | UTCID02 |
| Token inactive/revoked -> Revoke family + Unauthorized. | Reuse or expired | UTCID03, UTCID08 |
| Jti/UserId mismatch -> Unauthorized. | Token mismatch | UTCID04 |
| User inactive/deleted -> Unauthorized. | Account state | UTCID05 |
| Valid refresh -> Rotate tokens. | Happy path | UTCID06 |
| System error -> Failure. | Internal exception | UTCID07 |
| Token expired -> Unauthorized. | Expiry passed | UTCID09 |
| Family revocation -> Security success. | Multi-device revocation | UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Unauthorized | none | | | | |
| UTCID02 | A | Unauthorized | none | | | | |
| UTCID03 | A | Unauthorized | none | | | | |
| UTCID04 | A | Unauthorized | none | | | | |
| UTCID05 | A | Unauthorized | none | | | | |
| UTCID06 | N | Success; TokenResponse | none | | | | |
| UTCID07 | A | Failure | none | | | | |
| UTCID08 | A | Unauthorized | none | | | | |
| UTCID09 | B | Unauthorized | none | | | | |
| UTCID10 | N | Unauthorized | none | | | | |

---
## F007 - AuthService.LogoutAsync

| Header | Value |
|---|---|
| Function Code | F007 |
| Function Name | AuthService.LogoutAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate token revocation on logout. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Token exists -> Revoke success. | Active token | UTCID01 |
| Token doesn't exist -> Idempotent success. | No token | UTCID02 |
| Revoke failure -> Failure. | DB error | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | N | Success | none | | | | |
| UTCID03 | A | Failure | none | | | | |

---
## F008 - AuthService.LogoutAllAsync

| Header | Value |
|---|---|
| Function Code | F008 |
| Function Name | AuthService.LogoutAllAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate mass token revocation. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Success -> All tokens revoked. | User has tokens | UTCID01 |
| DB error -> Failure. | System failure | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | A | Failure | none | | | | |

---
## F009 - AuthService.ConfirmEmailAsync

| Header | Value |
|---|---|
| Function Code | F009 |
| Function Name | AuthService.ConfirmEmailAsync |
| Total Test Cases | 6 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 4 |
| Count type B | 0 |
| Test Requirement | Validate email confirmation flow and notifications. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Invalid Guid -> Failure. | Malformed ID | UTCID01 |
| User not found -> NotFound. | Missing ID | UTCID02 |
| Token invalid/expired -> Failure. | Identity error | UTCID03 |
| ClinicStaff confirmation -> Notify admins. | Role=ClinicStaff | UTCID04 |
| Success -> Confirmed. | Valid token | UTCID05 |
| System error -> Failure. | Internal exception | UTCID06 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Failure | none | | | | |
| UTCID02 | A | NotFound | none | | | | |
| UTCID03 | A | Failure | none | | | | |
| UTCID04 | N | Success | none | | | | |
| UTCID05 | N | Success | none | | | | |
| UTCID06 | A | Failure | none | | | | |

---
## F009 - AuthService.LogoutAllAsync

| Header | Value |
|---|---|
| Function Code | F009 |
| Function Name | AuthService.LogoutAllAsync |
| Total Test Cases | 20 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Revoke all tokens succeeds -> Success. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |
| Revoke all tokens throws -> Failure. | Valid dependencies and data setup for this scenario | UTCID11-UTCID20 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.Success (`RevokeAllUserTokensAsync`) | none |  |  |  |  |
| UTCID02 | A | Result.Success (`RevokeAllUserTokensAsync`) | none |  |  |  |  |
| UTCID03 | A | Result.Success (`RevokeAllUserTokensAsync`) | none |  |  |  |  |
| UTCID04 | A | Result.Success (`RevokeAllUserTokensAsync`) | none |  |  |  |  |
| UTCID05 | A | Result.Success (`RevokeAllUserTokensAsync`) | none |  |  |  |  |
| UTCID06 | A | Result.Success (`RevokeAllUserTokensAsync`) | none |  |  |  |  |
| UTCID07 | A | Result.Success (`RevokeAllUserTokensAsync`) | none |  |  |  |  |
| UTCID08 | A | Result.Success (`RevokeAllUserTokensAsync`) | none |  |  |  |  |
| UTCID09 | A | Result.Success (`RevokeAllUserTokensAsync`) | none |  |  |  |  |
| UTCID10 | A | Result.Success (`RevokeAllUserTokensAsync`) | none |  |  |  |  |
| UTCID11 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
## F010 - AuthService.ForgotPasswordAsync

| Header | Value |
|---|---|
| Function Code | F010 |
| Function Name | AuthService.ForgotPasswordAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate password reset request and email trigger. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Email exists -> Send token. | Active account | UTCID01 |
| Email doesn't exist -> Idempotent success. | No account | UTCID02 |
| System error -> Failure. | Mail/Token error | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | N | Success | none | | | | |
| UTCID03 | A | Failure | none | | | | |

---
## F011 - AuthService.ResetPasswordAsync

| Header | Value |
|---|---|
| Function Code | F011 |
| Function Name | AuthService.ResetPasswordAsync |
| Total Test Cases | 5 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 4 |
| Count type B | 0 |
| Test Requirement | Validate password reset with token. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Invalid ID -> Failure. | Malformed ID | UTCID01 |
| User not found -> NotFound. | Missing ID | UTCID02 |
| Identity errors (token/policy) -> Failure. | Token mismatch | UTCID03 |
| Success -> Revoke all tokens. | Valid token | UTCID04 |
| System error -> Failure. | Internal exception | UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Failure | none | | | | |
| UTCID02 | A | NotFound | none | | | | |
| UTCID03 | A | Failure | none | | | | |
| UTCID04 | N | Success | none | | | | |
| UTCID05 | A | Failure | none | | | | |

---
## F012 - AuthService.GetCurrentUserAsync

| Header | Value |
|---|---|
| Function Code | F012 |
| Function Name | AuthService.GetCurrentUserAsync |
---
## F013 - AuthService.ResendConfirmationAsync

| Header | Value |
|---|---|
| Function Code | F013 |
| Function Name | AuthService.ResendConfirmationAsync |
| Total Test Cases | 4 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 3 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate confirmation email resending. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Unconfirmed user -> Send email. | Active unconfirmed | UTCID01 |
| Already confirmed -> Idempotent success. | Active confirmed | UTCID02 |
| User not found -> Idempotent success. | No account | UTCID03 |
| Email error -> Failure. | Mail server fail | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | N | Success | none | | | | |
| UTCID03 | N | Success | none | | | | |
| UTCID04 | A | Failure | none | | | | |

---
## F014 - IdentityService.CheckPasswordAsync

| Header | Value |
|---|---|
| Function Code | F014 |
| Function Name | IdentityService.CheckPasswordAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate user password verification. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User not found -> false. | Invalid ID | UTCID01 |
| Wrong password -> false. | Valid ID | UTCID02 |
| Correct password -> true. | Valid ID | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | false | none | | | | |
| UTCID02 | A | false | none | | | | |
| UTCID03 | N | true | none | | | | |

---
---
## F015 - IdentityService.GetUserByEmailAsync

| Header | Value |
|---|---|
| Function Code | F015 |
| Function Name | IdentityService.GetUserByEmailAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate user retrieval by email. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User exists and !IsDeleted -> full DTO. | Active account | UTCID01 |
| User deleted -> null. | Soft-deleted account | UTCID02 |
| Email not found -> null. | No account matches | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | UserDto | none | | | | |
| UTCID02 | A | null | none | | | | |
| UTCID03 | A | null | none | | | | |

---
## F016 - IdentityService.GetUserByIdAsync

| Header | Value |
|---|---|
| Function Code | F016 |
| Function Name | IdentityService.GetUserByIdAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate user retrieval by ID. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User exists and !IsDeleted -> full DTO. | Active account | UTCID01 |
| User deleted -> null. | Soft-deleted account | UTCID02 |
| UserId not found -> null. | No account matches | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | UserDto | none | | | | |
| UTCID02 | A | null | none | | | | |
| UTCID03 | A | null | none | | | | |

---
## F017 - IdentityService.IsEmailConfirmedAsync

| Header | Value |
|---|---|
| Function Code | F017 |
| Function Name | IdentityService.IsEmailConfirmedAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate email confirmation status check. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User not found -> false. | Invalid ID | UTCID01 |
| User exists, EmailConfirmed=false -> false. | Active unconfirmed | UTCID02 |
| User exists, EmailConfirmed=true -> true. | Active confirmed | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | false | none | | | | |
| UTCID02 | N | false | none | | | | |
| UTCID03 | N | true | none | | | | |

---
## F018 - IdentityService.IsUserActiveAsync

| Header | Value |
|---|---|
| Function Code | F018 |
| Function Name | IdentityService.IsUserActiveAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate user activity status check. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User not found -> false. | Invalid ID | UTCID01 |
| User inactive or deleted -> false. | Account state | UTCID02 |
| User active and !deleted -> true. | Account state | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | false | none | | | | |
| UTCID02 | A | false | none | | | | |
| UTCID03 | N | true | none | | | | |

---
## F019 - IdentityService.GenerateEmailConfirmationTokenAsync

| Header | Value |
|---|---|
| Function Code | F019 |
| Function Name | IdentityService.GenerateEmailConfirmationTokenAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate confirmation token generation. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User not found -> throw InvalidOperationException. | Invalid ID | UTCID01 |
| User exists -> token non-empty. | Valid ID | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | n/a | InvalidOperationException | | | | |
| UTCID02 | N | string | none | | | | |

---
## F020 - IdentityService.GeneratePasswordResetTokenAsync

| Header | Value |
|---|---|
| Function Code | F020 |
| Function Name | IdentityService.GeneratePasswordResetTokenAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate password reset token generation. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User not found -> throw. | Invalid ID | UTCID01 |
| User exists -> token non-empty. | Valid ID | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | n/a | InvalidOperationException | | | | |
| UTCID02 | N | string | none | | | | |

---
## F021 - IdentityService.GetUserRolesAsync

| Header | Value |
|---|---|
| Function Code | F021 |
| Function Name | IdentityService.GetUserRolesAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate user role retrieval. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User not found -> empty list. | Invalid ID | UTCID01 |
| User has one role -> that role returned. | Role assigned | UTCID02 |
| User has multiple roles -> all roles returned. | Roles assigned | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | empty list | none | | | | |
| UTCID02 | N | list containing role | none | | | | |
| UTCID03 | N | list containing roles | none | | | | |

---
## F022 - IdentityService.IsInRoleAsync

| Header | Value |
|---|---|
| Function Code | F022 |
| Function Name | IdentityService.IsInRoleAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| UTCID01 | A | `false` when missing (`IsInRoleAsync_WhenMissing`) | none |  |  |  |  |
| UTCID02 | N | `true` when user in role (contract) | none |  |  |  |  |
| UTCID03 | N | `false` when not in role (contract) | none |  |  |  |  |

---
## F025 - IdentityService.GetUserIdsByRoleAndOrganizationAsync

| Header | Value |
|---|---|
| Function Code | F025 |
| Function Name | IdentityService.GetUserIdsByRoleAndOrganizationAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Filter users correctly by role + org. | Valid dependencies and data setup for this scenario | UTCID01 |
| Deleted/inactive despite same role -> excluded. | Valid dependencies and data setup for this scenario | UTCID02 |
| No valid users -> empty list. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Guids for active users in role+org (contract) | none |  |  |  |  |
| UTCID02 | A | excludes inactive/deleted (contract) | none |  |  |  |  |
| UTCID03 | B | empty list when none (contract) | none |  |  |  |  |

---
## F026 - IdentityService.UpdateLastLoginAsync

| Header | Value |
|---|---|
## F023 - IdentityService.UpdateLastLoginAsync

| Header | Value |
|---|---|
| Function Code | F023 |
| Function Name | IdentityService.UpdateLastLoginAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate last login timestamp update. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User not found -> no-op. | Invalid ID | UTCID01 |
| User exists -> update success. | Valid ID | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | no-op | none | | | | |
| UTCID02 | N | Success | none | | | | |

---
## F024 - IdentityService.IsTwoFactorEnabledAsync

| Header | Value |
|---|---|
| Function Code | F024 |
| Function Name | IdentityService.IsTwoFactorEnabledAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate 2FA status check. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User not found -> false. | Invalid ID | UTCID01 |
| User 2FA=false -> false. | 2FA disabled | UTCID02 |
| User 2FA=true -> true. | 2FA enabled | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | false | none | | | | |
| UTCID02 | N | false | none | | | | |
| UTCID03 | N | true | none | | | | |

---
## F025 - IdentityService.GetAuthenticatorKeyAsync

| Header | Value |
|---|---|
| Function Code | F025 |
| Function Name | IdentityService.GetAuthenticatorKeyAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate authenticator key retrieval. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User not found -> null. | Invalid ID | UTCID01 |
| User exists -> return key. | Key present | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | null | none | | | | |
| UTCID02 | N | string | none | | | | |

---
## F026 - IdentityService.GetOrCreateAuthenticatorKeyAsync

| Header | Value |
|---|---|
| Function Code | F026 |
| Function Name | IdentityService.GetOrCreateAuthenticatorKeyAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate lazy authenticator key creation. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User not found -> throw. | Invalid ID | UTCID01 |
| User exists -> key returned. | Valid ID | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | n/a | InvalidOperationException | | | | |
| UTCID02 | N | string | none | | | | |

---
## F027 - IdentityService.VerifyTwoFactorCodeAsync

| Header | Value |
|---|---|
| Function Code | F027 |
| Function Name | IdentityService.VerifyTwoFactorCodeAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate 2FA TOTP verification. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User not found -> false. | Invalid ID | UTCID01 |
| Code sai -> false. | Invalid code | UTCID02 |
| Code dung -> true. | Valid code | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | false | none | | | | |
| UTCID02 | A | false | none | | | | |
| UTCID03 | N | true | none | | | | |

---
## F028 - IdentityService.GenerateNewRecoveryCodesAsync

| Header | Value |
|---|---|
| Function Code | F028 |
| Function Name | IdentityService.GenerateNewRecoveryCodesAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate recovery code regeneration. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User not found -> throw. | Invalid ID | UTCID01 |
| User exists -> codes returned. | Valid ID | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | n/a | InvalidOperationException | | | | |
| UTCID02 | N | string[] | none | | | | |

---
## F029 - IdentityService.GetRecoveryCodesCountAsync

| Header | Value |
|---|---|
| Function Code | F029 |
| Function Name | IdentityService.GetRecoveryCodesCountAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate recovery codes counting. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User not found -> 0. | Invalid ID | UTCID01 |
| User exists -> count returned. | Valid ID | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | 0 | none | | | | |
| UTCID02 | N | int | none | | | | |

---
## F030 - IdentityService.GenerateAuthenticatorUri

| Header | Value |
|---|---|
| Function Code | F030 |
| Function Name | IdentityService.GenerateAuthenticatorUri |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 0 |
| Count type B | 1 |
| Test Requirement | Validate TOTP URI formatting. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Valid email + key -> URI schema ok. | Inputs present | UTCID01 |
| Special characters -> encoded. | Edge case email | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | string (otpauth://) | none | | | | |
| UTCID02 | B | string (encoded) | none | | | | |

---
## F031 - IdentityService.FormatAuthenticatorKey

| Header | Value |
|---|---|
| Function Code | F031 |
| Function Name | IdentityService.FormatAuthenticatorKey |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 0 |
| Count type B | 1 |
| Test Requirement | Validate key block formatting. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Long key -> 4-char blocks. | Input length > 4 | UTCID01 |
| Short key -> no blocks. | Input length <= 4 | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | string (formatted) | none | | | | |
| UTCID02 | B | string (uppercase) | none | | | | |

---
## F033 - IdentityService.GetUsersInRoleCountAsync

| Header | Value |
|---|---|
| Function Code | F033 |
| Function Name | IdentityService.GetUsersInRoleCountAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 0 |
| Count type B | 1 |
| Test Requirement | Validate role-based user counting. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| activeOnly=true -> filtered. | Data mixed | UTCID01 |
| activeOnly=false -> total. | Data mixed | UTCID02 |
| Role has no users -> 0. | Role empty | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | int | none | | | | |
| UTCID02 | N | int | none | | | | |
| UTCID03 | B | 0 | none | | | | |

---
## F034 - IdentityService.GetPendingApprovalsCountAsync

| Header | Value |
|---|---|
| Function Code | F034 |
| Function Name | IdentityService.GetPendingApprovalsCountAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 0 |
| Count type B | 1 |
| Test Requirement | Validate pending user counting. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Pending exists -> count ok. | Filter active | UTCID01 |
| No pending -> 0. | Filter empty | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | int | none | | | | |
| UTCID02 | B | 0 | none | | | | |

---
## F035 - IdentityService.GetUserDetailsAsync

| Header | Value |
|---|---|
| Function Code | F035 |
| Function Name | IdentityService.GetUserDetailsAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate detailed profile retrieval. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User not found -> null. | Invalid ID | UTCID01 |
| User deleted -> null. | Soft-deleted | UTCID02 |
| User valid -> DTO ok. | Active account | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | null | none | | | | |
| UTCID02 | A | null | none | | | | |
| UTCID03 | N | UserDetailsDto | none | | | | |

---
## F036 - RefreshTokenService.CreateRefreshTokenAsync

| Header | Value |
|---|---|
| Function Code | F036 |
| Function Name | RefreshTokenService.CreateRefreshTokenAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 3 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate refresh token creation. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Valid data -> token created. | Success DB | UTCID01 |
| Data fields correct. | Success DB | UTCID02 |
| Initial state active. | New token | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Guid | none | | | | |
| UTCID02 | N | Success | none | | | | |
| UTCID03 | N | Success | none | | | | |

---
## F037 - RefreshTokenService.GetByTokenHashAsync

| Header | Value |
|---|---|
| Function Code | F037 |
| Function Name | RefreshTokenService.GetByTokenHashAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate token retrieval by hash. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Hash not found -> null. | Missing hash | UTCID01 |
| Hash exists -> DTO ok. | Valid hash | UTCID02 |
| Inactive token -> DTO ok. | Revoked token | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | null | none | | | | |
| UTCID02 | N | RefreshTokenDto | none | | | | |
| UTCID03 | N | RefreshTokenDto | none | | | | |

---
## F038 - RefreshTokenService.RotateRefreshTokenAsync

| Header | Value |
|---|---|
| Function Code | F038 |
| Function Name | RefreshTokenService.RotateRefreshTokenAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate refresh token rotation. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Old token missing -> throw. | Invalid ID | UTCID01 |
| Old token valid -> new created. | Valid ID | UTCID02 |
| Old marked as used. | Rotation link | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | n/a | InvalidOperationException | | | | |
| UTCID02 | N | Guid | none | | | | |
| UTCID03 | N | Success | none | | | | |

---
## F039 - RefreshTokenService.RevokeTokenAsync

| Header | Value |
|---|---|
| Function Code | F039 |
| Function Name | RefreshTokenService.RevokeTokenAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate single token revocation. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Token exists -> revoked. | Valid ID | UTCID01 |
| Token not found -> no-op. | Invalid ID | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | N | Success | none | | | | |

---
## F040 - RefreshTokenService.RevokeAllUserTokensAsync

| Header | Value |
|---|---|
| Function Code | F040 |
| Function Name | RefreshTokenService.RevokeAllUserTokensAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 0 |
| Count type B | 1 |
| Test Requirement | Validate mass token revocation. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Active tokens -> all revoked. | User has tokens | UTCID01 |
| No active tokens -> safe. | Empty list | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | B | Success | none | | | | |

---
## F041 - RefreshTokenService.RevokeTokenFamilyAsync

| Header | Value |
|---|---|
| Function Code | F041 |
| Function Name | RefreshTokenService.RevokeTokenFamilyAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |

---
## F045 - RefreshTokenService.CleanupExpiredTokensAsync

| Header | Value |
|---|---|
| Function Code | F045 |
| Function Name | RefreshTokenService.CleanupExpiredTokensAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Token expired past cutoff -> deleted. | Valid dependencies and data setup for this scenario | UTCID01 |
| Token revoked past cutoff -> deleted. | Valid dependencies and data setup for this scenario | UTCID02 |
| No tokens to delete -> return 0. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | deleted count matches expired+old revoked (`CleanupExpiredTokensAsync_ShouldDeleteExpiredAndOldRevoked`) | none |  |  |  |  |
| UTCID02 | N | 0 deleted when only fresh tokens (`CleanupExpiredTokensAsync_WithDifferentRetentionDays`) | none |  |  |  |  |
| UTCID03 | B | boundary cases per keepDays (`CleanupExpiredTokensAsync_ShouldDeleteByCutoffBoundary` theory) | none |  |  |  |  |

---
## F043 - TokenService.GenerateAccessTokenAsync

| Header | Value |
|---|---|
| Function Code | F043 |
| Function Name | TokenService.GenerateAccessTokenAsync |
| Total Test Cases | 4 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 3 |
| Count type A | 0 |
| Count type B | 1 |
| Test Requirement | Validate access token generation and claims. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Required claims present. | Inputs valid | UTCID01 |
| Role claims mapped. | Roles present | UTCID02 |
| Custom claims added. | Extra data | UTCID03 |
| Expiry matches config. | Settings ok | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | string | none | | | | |
| UTCID02 | N | string | none | | | | |
| UTCID03 | N | string | none | | | | |
| UTCID04 | B | string | none | | | | |

---
## F044 - TokenService.GenerateRefreshToken

| Header | Value |
|---|---|
| Function Code | F044 |
| Function Name | TokenService.GenerateRefreshToken |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 0 |
| Count type B | 1 |
| Test Requirement | Validate refresh token entropy. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Non-empty base64 string. | Random gen | UTCID01 |
| Unique across calls. | Random gen | UTCID02 |
| Decodes to 64 bytes. | Length check | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | string | none | | | | |
| UTCID02 | N | string | none | | | | |
| UTCID03 | B | string | none | | | | |

---
## F045 - TokenService.ValidateToken

| Header | Value |
|---|---|
| Function Code | F045 |
| Function Name | TokenService.ValidateToken |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate token signature and algo. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Valid token -> Principal ok. | Signature match | UTCID01 |
| Invalid signature -> null. | Mismatch | UTCID02 |
| Wrong algorithm -> null. | Alg mismatch | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | ClaimsPrincipal | none | | | | |
| UTCID02 | A | null | none | | | | |
| UTCID03 | A | null | none | | | | |

---
## F046 - TokenService.GetUserIdFromToken

| Header | Value |
|---|---|
| Function Code | F046 |
| Function Name | TokenService.GetUserIdFromToken |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate UID extraction from JWT. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| sub claim present -> Guid. | Valid JWT | UTCID01 |
| uid fallback -> Guid. | sub missing | UTCID02 |
| Parse failure -> null. | Bad Guid | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Guid | none | | | | |
| UTCID02 | N | Guid | none | | | | |
| UTCID03 | A | null | none | | | | |

---
## F047 - TokenService.GetJtiFromToken

| Header | Value |
|---|---|
| Function Code | F047 |
| Function Name | TokenService.GetJtiFromToken |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate JTI extraction from JWT. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Valid token -> JTI string. | JTI present | UTCID01 |
| Invalid token -> null. | Claim missing | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | string | none | | | | |
| UTCID02 | A | null | none | | | | |

---
## F048 - TokenService.HashToken

| Header | Value |
|---|---|
| Function Code | F048 |
| Function Name | TokenService.HashToken |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 0 |
| Count type B | 1 |
| Test Requirement | Validate deterministic token hashing. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Same input -> same hash. | Determ. check | UTCID01 |
| Different input -> diff hash. | Collision check | UTCID02 |
| Empty input -> non-null hash. | Boundary check | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | string | none | | | | |
| UTCID02 | N | string | none | | | | |
| UTCID03 | B | string | none | | | | |

---
## F049 - LocalFileStorageService.SaveFileAsync

| Header | Value |
|---|---|
| Function Code | F049 |
| Function Name | LocalFileStorageService.SaveFileAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate local file saving. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Null stream -> throw. | Invalid input | UTCID01 |
| Success -> return path. | Valid stream | UTCID02 |
| IO error -> throw. | Disk failure | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | n/a | ArgumentNullException | | | | |
| UTCID02 | N | string (path) | none | | | | |
| UTCID03 | A | n/a | IOException | | | | |

---
## F050 - LocalFileStorageService.DeleteFileAsync

| Header | Value |
|---|---|
| Function Code | F050 |
| Function Name | LocalFileStorageService.DeleteFileAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate local file deletion. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| File exists -> deleted. | File present | UTCID01 |
| File missing -> no-op. | File absent | UTCID02 |
| Path invalid -> no-op. | Bad path string | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | N | Success | none | | | | |
| UTCID03 | A | Success | none | | | | |

---
## F051 - AdminQueryService.GetOphthalmologistsAsync

| Header | Value |
|---|---|
| Function Code | F051 |
| Function Name | AdminQueryService.GetOphthalmologistsAsync |
| Total Test Cases | 5 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 4 |
| Count type A | 0 |
| Count type B | 1 |
| Test Requirement | Validate admin list query for doctors. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| No filter -> CreatedAt desc. | Data present | UTCID01 |
| SearchTerm matches ILike. | Query string | UTCID02 |
| Status filter union. | Comma values | UTCID03 |
| Certificates mapped correctly. | Certs present | UTCID04 |
| Invalid status -> skipped. | Bad token | UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | PagedList | none | | | | |
| UTCID02 | N | PagedList | none | | | | |
| UTCID03 | N | PagedList | none | | | | |
| UTCID04 | N | PagedList | none | | | | |
| UTCID05 | B | PagedList | none | | | | |

---
## F052 - AdminQueryService.GetPatientsAsync

| Header | Value |
|---|---|
| Function Code | F052 |
| Function Name | AdminQueryService.GetPatientsAsync |
| Total Test Cases | 4 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | SearchTerm on name/email (contract; tests cover status filters) | none |  |  |  |  |
| UTCID02 | N | active/pending/suspended filters (`GetPatientsAsync_With*Status`) | none |  |  |  |  |
| UTCID03 | N | Paging totals (`GetPatientsAsync_WithPaging` / related) | none |  |  |  |  |
| UTCID04 | A | Deleted users excluded (`GetPatientsAsync_ShouldExcludeDeletedUsers`) | none |  |  |  |  |

---
## F053 - AdminQueryService.GetClinicStaffAsync

| Header | Value |
|---|---|
| Function Code | F053 |
| Function Name | AdminQueryService.GetClinicStaffAsync |
| Total Test Cases | 4 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 3 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate admin staff list query. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| SearchTerm fullName/phone ok. | Query string | UTCID01 |
| SubRole filtering ok. | Enum match | UTCID02 |
| Paging metadata correct. | Multi-page | UTCID03 |
| Non-staff users hidden. | Filter check | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | PagedList | none | | | | |
| UTCID02 | N | PagedList | none | | | | |
| UTCID03 | N | PagedList | none | | | | |
| UTCID04 | A | PagedList | none | | | | |

---
## F054 - AiQuotaService.CheckQuotaAsync

| Header | Value |
|---|---|
| Function Code | F054 |
| Function Name | AiQuotaService.CheckQuotaAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 0 |
| Count type B | 1 |
| Test Requirement | Validate quota availability check. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Remaining > 0 -> true. | Success | UTCID01 |
| Remaining = 0 -> false. | Exhausted | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | true | none | | | | |
| UTCID02 | B | false | none | | | | |

---
## F055 - AiQuotaService.GetQuotaStatusAsync

| Header | Value |
|---|---|
| Function Code | F055 |
| Function Name | AiQuotaService.GetQuotaStatusAsync |
| Total Test Cases | 5 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 3 |
| Count type B | 0 |
| Test Requirement | Validate quota status summary. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Patient with profile -> ok. | Valid user | UTCID01 |
| Patient without profile -> free fallback. | Missing data | UTCID02 |
| Clinic staff -> clinic quota. | Staff role | UTCID03 |
| Role but no link -> None. | Mismatch | UTCID04 |
| Unknown role -> None. | Invalid role | UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | AiQuotaDto | none | | | | |
| UTCID02 | A | AiQuotaDto | none | | | | |
| UTCID03 | N | AiQuotaDto | none | | | | |
| UTCID04 | A | AiQuotaDto | none | | | | |
| UTCID05 | A | AiQuotaDto | none | | | | |

---
## F056 - DashboardService.GetAdminDashboardDataAsync

| Header | Value |
|---|---|
| Function Code | F056 |
| Function Name | DashboardService.GetAdminDashboardDataAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate admin dashboard metrics. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Success -> return summary data. | Data ok | UTCID01 |
| Empty DB -> return zeroed data. | No data | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | AdminDashboardDto | none | | | | |
| UTCID02 | N | AdminDashboardDto | none | | | | |

---
## F057 - DashboardService.GetOphthalmologistDashboardDataAsync

| Header | Value |
|---|---|
| Function Code | F057 |
| Function Name | DashboardService.GetOphthalmologistDashboardDataAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate doctor dashboard metrics. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Success -> return relevant counts. | Data ok | UTCID01 |
| No patients -> zeroed data. | No data | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | DoctorDashboardDto | none | | | | |
| UTCID02 | N | DoctorDashboardDto | none | | | | |

---
## F058 - DashboardService.GetPatientDashboardDataAsync

| Header | Value |
|---|---|
| Function Code | F058 |
| Function Name | DashboardService.GetPatientDashboardDataAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate patient dashboard metrics. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Success -> return history/quota. | Data ok | UTCID01 |
| New user -> zeroed data. | No data | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | PatientDashboardDto | none | | | | |
| UTCID02 | N | PatientDashboardDto | none | | | | |

---
## F059 - DashboardService.GetStaffDashboardDataAsync

| Header | Value |
|---|---|
| Function Code | F059 |
| Function Name | DashboardService.GetStaffDashboardDataAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate staff dashboard metrics. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Success -> return pending visits. | Data ok | UTCID01 |
| No visits -> zeroed data. | No data | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | StaffDashboardDto | none | | | | |
| UTCID02 | N | StaffDashboardDto | none | | | | |

---
## F060 - DateTimeService.Now

| Header | Value |
|---|---|
| Function Code | F060 |
| Function Name | DateTimeService.Now |
| Total Test Cases | 1 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate system clock wrapper. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Success -> return current time. | Always | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | DateTime | none | | | | |

---
## F061 - EmailService.SendEmailAsync

| Header | Value |
|---|---|
| Function Code | F061 |
| Function Name | EmailService.SendEmailAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate generic email sending. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Valid input -> SMTP success. | Config ok | UTCID01 |
| Invalid recipient -> throw. | Bad email | UTCID02 |
| SMTP server down -> throw. | Network fail | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | A | n/a | ArgumentException | | | | |
| UTCID03 | A | n/a | SmtpException | | | | |

---
## F062 - EmailService.SendTemplateEmailAsync

| Header | Value |
|---|---|
| Function Code | F062 |
| Function Name | EmailService.SendTemplateEmailAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate template-based email sending. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Template valid -> merge success. | Template exists | UTCID01 |
| Template missing -> throw. | Invalid template | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | A | n/a | FileNotFoundException | | | | |

---
## F063 - GoogleMeetService.CreateMeetingAsync

| Header | Value |
|---|---|
| Function Code | F063 |
| Function Name | GoogleMeetService.CreateMeetingAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate Google Calendar API integration. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Valid event -> returns link. | API Auth ok | UTCID01 |
| Auth expired -> throw. | Token invalid | UTCID02 |
| API quota exceeded -> throw. | Rate limited | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | string (url) | none | | | | |
| UTCID02 | A | n/a | UnauthorizedAccessException | | | | |
| UTCID03 | A | n/a | HttpRequestException | | | | |

---
## F064 - NotificationService.SendNotificationAsync

| Header | Value |
|---|---|
| Function Code | F064 |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Totals doctor/org/patient and growth % correct. | Valid dependencies and data setup for this scenario | UTCID01 |
| Revenue by payment method + percentage correct. | Valid dependencies and data setup for this scenario | UTCID02 |
| Monthly/Daily revenue mapped fully for period. | Valid dependencies and data setup for this scenario | UTCID03 |
| Pending actions (verification/withdraw/onboarding) correct. | Valid dependencies and data setup for this scenario | UTCID04 |
| Top doctor by consultation revenue + top org by rating correct. | Valid dependencies and data setup for this scenario | UTCID05 |
| BetterStack monitors mapped into response correctly. | Valid dependencies and data setup for this scenario | UTCID06 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | SystemAdmin metrics shape; zeros when empty DB (`GetSystemAdminMetricsAsync_WhenNoData`) | none |  |  |  |  |
| UTCID02 | N | SystemAdmin metrics shape; zeros when empty DB (`GetSystemAdminMetricsAsync_WhenNoData`) | none |  |  |  |  |
| UTCID03 | N | SystemAdmin metrics shape; zeros when empty DB (`GetSystemAdminMetricsAsync_WhenNoData`) | none |  |  |  |  |
| UTCID04 | N | SystemAdmin metrics shape; zeros when empty DB (`GetSystemAdminMetricsAsync_WhenNoData`) | none |  |  |  |  |
| UTCID05 | N | SystemAdmin metrics shape; zeros when empty DB (`GetSystemAdminMetricsAsync_WhenNoData`) | none |  |  |  |  |
| UTCID06 | N | SystemAdmin metrics shape; zeros when empty DB (`GetSystemAdminMetricsAsync_WhenNoData`) | none |  |  |  |  |

---
## F065 - DashboardMetricsService.GetRecentScreeningsAsync

| Header | Value |
|---|---|
| Function Code | F065 |
| Function Name | DashboardMetricsService.GetRecentScreeningsAsync |
| Total Test Cases | 4 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Paging total/items correct. | Valid dependencies and data setup for this scenario | UTCID01 |
| RiskLevel and `IsCritical` mapped correctly. | Valid dependencies and data setup for this scenario | UTCID02 |
| Status `Completed/Analyzing` mapped from `ProcessedAt`. | Valid dependencies and data setup for this scenario | UTCID03 |
| Screening without risk result -> risk null still maps. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | paging (`GetRecentScreeningsAsync_WithPaging`) | none |  |  |  |  |
| UTCID02 | N | risk + critical flag (`ShouldMapStatusAndCriticalFlag` / without result theory) | none |  |  |  |  |
| UTCID03 | N | status from ProcessedAt | none |  |  |  |  |
| UTCID04 | B | null risk still maps | none |  |  |  |  |

---
## F066 - DashboardMetricsService.GetScreeningVolumeTrendsAsync

| Header | Value |
|---|---|
| Function Code | F066 |
| Function Name | DashboardMetricsService.GetScreeningVolumeTrendsAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| `weekly` -> group by week start, label dd MMM. | Valid dependencies and data setup for this scenario | UTCID01 |
| `monthly` -> group by month, label MMM yyyy. | Valid dependencies and data setup for this scenario | UTCID02 |
| `timeRange` empty -> fallback monthly. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | weekly grouping (`GetScreeningVolumeTrendsAsync_Weekly`) | none |  |  |  |  |
| UTCID02 | N | monthly grouping | none |  |  |  |  |
| UTCID03 | B | unknown range -> monthly fallback | none |  |  |  |  |

---
## F067 - DashboardMetricsService.GetPopulationRiskAnalysisAsync

| Header | Value |
|---|---|
| Function Code | F067 |
| Function Name | DashboardMetricsService.GetPopulationRiskAnalysisAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Total `TotalPatients` correct. | Valid dependencies and data setup for this scenario | UTCID01 |
| Skip RiskLevel.None in output list. | Valid dependencies and data setup for this scenario | UTCID02 |
| No results -> percentage=0. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | totals correct | none |  |  |  |  |
| UTCID02 | N | excludes RiskLevel.None (`GetPopulationRiskAnalysisAsync`) | none |  |  |  |  |
| UTCID03 | B | 0% when empty | none |  |  |  |  |

---
## F068 - DashboardMetricsService.GetSystemHealthAsync

| Header | Value |
|---|---|
| Function Code | F068 |
| Function Name | DashboardMetricsService.GetSystemHealthAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Payload has Database/AI/Notifications components. | Valid dependencies and data setup for this scenario | UTCID01 |
| `AllSystemsOperational=true` and health fields complete. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | components populated (`GetSystemHealthAsync_ShouldReturnOperationalPayload`) | none |  |  |  |  |
| UTCID02 | N | AllSystemsOperational true in happy path | none |  |  |  |  |

---
## F069 - DashboardMetricsService.GetOphthalmologistMetricsAsync

| Header | Value |
|---|---|
| Function Code | F069 |
| Function Name | DashboardMetricsService.GetOphthalmologistMetricsAsync |
| Total Test Cases | 4 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| UserId maps to no doctor -> empty DTO. | Valid dependencies and data setup for this scenario | UTCID01 |
| Pending reviews and urgent cases counted correctly. | Valid dependencies and data setup for this scenario | UTCID02 |
| Completed today counted correctly. | Valid dependencies and data setup for this scenario | UTCID03 |
| Open slots today calculated correctly per template/org. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | default/empty dto when no doctor (`GetOphthalmologistMetricsAsync_WhenNoDoctor`) | none |  |  |  |  |
| UTCID02 | N | pending/urgent counts (`WithPendingUrgentData`) | none |  |  |  |  |
| UTCID03 | N | completed today | none |  |  |  |  |
| UTCID04 | N | open slots | none |  |  |  |  |

---
## F070 - DashboardMetricsService.GetOrganisationMetricsAsync

| Header | Value |
|---|---|
| Function Code | F070 |
| Function Name | DashboardMetricsService.GetOrganisationMetricsAsync |
| Total Test Cases | 4 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User has no organisation -> empty DTO. | Valid dependencies and data setup for this scenario | UTCID01 |
| Total appointments + status breakdown counted correctly. | Valid dependencies and data setup for this scenario | UTCID02 |
| Utilization rate by booked/capacity correct. | Valid dependencies and data setup for this scenario | UTCID03 |
| Remaining AI quota from `AiQuotaService` correct. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | default when no org (`GetOrganisationMetricsAsync_WhenNoOrganisation`) | none |  |  |  |  |
| UTCID02 | N | appointments breakdown (`WithOrganisation`) | none |  |  |  |  |
| UTCID03 | N | utilization (`ShouldComputeUtilizationRateFromSlots`) | none |  |  |  |  |
| UTCID04 | N | remaining quota from AiQuota mock | none |  |  |  |  |

---
## F071 - DashboardMetricsService.GetPatientMetricsAsync

| Header | Value |
|---|---|
| Function Code | F071 |
| Function Name | DashboardMetricsService.GetPatientMetricsAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Patient profile not found -> empty DTO. | Valid dependencies and data setup for this scenario | UTCID01 |
| Completed screenings / total reports counted correctly. | Valid dependencies and data setup for this scenario | UTCID02 |
| Upcoming appointments + remaining quota counted correctly. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | default dto when patient missing (`GetPatientMetricsAsync_WhenPatientMissing`) | none |  |  |  |  |
| UTCID02 | N | completed reports (`ShouldComputeCompletedReportsAndUpcomingAppointments`) | none |  |  |  |  |
| UTCID03 | N | upcoming + remaining quota (`ShouldMapRemainingQuotaFromService`) | none |  |  |  |  |

---
## F072 - DateTimeService.Now

| Header | Value |
|---|---|
| Function Code | F072 |
| Function Name | DateTimeService.Now |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| `Now` value close to `DateTime.Now`. | Valid dependencies and data setup for this scenario | UTCID01 |
| Each call may differ with real time. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | DateTime close to DateTime.Now; Kind local/unspecified (`Now_ShouldReturnLocalTime_CloseToSystemNow`) | none |  |  |  |  |
| UTCID02 | B | monotonic non-decreasing across reads (theories) | none |  |  |  |  |

---
## F073 - DateTimeService.UtcNow

| Header | Value |
|---|---|
| Function Code | F073 |
| Function Name | DateTimeService.UtcNow |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| `UtcNow` value close to `DateTime.UtcNow`. | Valid dependencies and data setup for this scenario | UTCID01 |
| `Kind` of value is UTC. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Utc close to DateTime.UtcNow (`UtcNow_ShouldReturnUtcTime_CloseToSystemUtcNow`) | none |  |  |  |  |
| UTCID02 | B | Kind=Utc (`UtcNow_ShouldHaveUtcKind`) | none |  |  |  |  |

---
## F074 - EmailService.SendEmailConfirmationAsync

| Header | Value |
|---|---|
| Function Code | F074 |
| Function Name | EmailService.SendEmailConfirmationAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Correct subject/template for confirmation email. | Valid dependencies and data setup for this scenario | UTCID01 |
| Calls `SendAsync` with `isHtml=true`. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | n/a | Exception for invalid email (`SendEmailConfirmationAsync_WithInvalidRecipient_ShouldThrow`) |  |  |  |  |
| UTCID02 | N | valid path builds message and calls SendAsync (contract; no happy-path unit test) | none |  |  |  |  |

---
## F075 - EmailService.SendPasswordResetAsync

| Header | Value |
|---|---|
| Function Code | F075 |
| Function Name | EmailService.SendPasswordResetAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | |
| Count type A | |
| Count type B | |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Correct subject/template for password reset. | Valid dependencies and data setup for this scenario | UTCID01 |
| `SendAsync` succeeds. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | reset email template + SendAsync (contract; no dedicated test) | none |  |  |  |  |
| UTCID02 | N | SendAsync success path (contract) | none |  |  |  |  |

---
## F076 - SystemSettingsService.UpdateSettingsAsync

| Header | Value |
|---|---|
| Function Code | F076 |
| Function Name | SystemSettingsService.UpdateSettingsAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate mass settings update. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Valid DTO -> DB updated. | Auth Admin | UTCID01 |
| Null DTO -> throw. | Invalid input | UTCID02 |
| Save fail -> throw. | DB error | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | A | n/a | ArgumentNullException | | | | |
| UTCID03 | A | n/a | DbUpdateException | | | | |

---
## F077 - SystemSettingsService.GetMaintenanceModeAsync

| Header | Value |
|---|---|
| Function Code | F077 |
| Function Name | SystemSettingsService.GetMaintenanceModeAsync |
| Total Test Cases | 1 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate maintenance mode flag retrieval. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Success -> return bool. | Always | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | bool | none | | | | |

---
## F078 - SystemSettingsService.SetMaintenanceModeAsync

| Header | Value |
|---|---|
| Function Code | F078 |
| Function Name | SystemSettingsService.SetMaintenanceModeAsync |
| Total Test Cases | 1 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate maintenance mode flag update. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Success -> persisted. | Auth Admin | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |

---
## F079 - SystemSettingsService.GetAiSettingsAsync

| Header | Value |
|---|---|
| Function Code | F079 |
| Function Name | SystemSettingsService.GetAiSettingsAsync |
| Total Test Cases | 1 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate AI config retrieval. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Success -> return DTO. | Always | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | AiSettingsDto | none | | | | |

---
## F080 - SystemSettingsService.UpdateAiSettingsAsync

| Header | Value |
|---|---|
| Function Code | F080 |
| Function Name | SystemSettingsService.UpdateAiSettingsAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate AI config update. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Valid DTO -> Success. | Auth Admin | UTCID01 |
| Invalid DTO -> throw. | Validation fail | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | A | n/a | ValidationException | | | | |

---
## F081 - SystemSettingsService.GetPaymentSettingsAsync

| Header | Value |
|---|---|
| Function Code | F081 |
| Function Name | SystemSettingsService.GetPaymentSettingsAsync |
| Total Test Cases | 1 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate payment config retrieval. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Success -> return DTO. | Always | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | PaymentSettingsDto | none | | | | |

---
## F082 - SystemSettingsService.UpdatePaymentSettingsAsync

| Header | Value |
|---|---|
| Function Code | F082 |
| Function Name | SystemSettingsService.UpdatePaymentSettingsAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate payment config update. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Valid DTO -> Success. | Auth Admin | UTCID01 |
| Missing fields -> throw. | Validation fail | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | A | n/a | ValidationException | | | | |

---
## F083 - SystemSettingsService.GetEmailSettingsAsync

| Header | Value |
|---|---|
| Function Code | F083 |
| Function Name | SystemSettingsService.GetEmailSettingsAsync |
| Total Test Cases | 1 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate email config retrieval. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Success -> return DTO. | Always | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | EmailSettingsDto | none | | | | |

---
## F084 - SystemSettingsService.UpdateEmailSettingsAsync

| Header | Value |
|---|---|
| Function Code | F084 |
| Function Name | SystemSettingsService.UpdateEmailSettingsAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate email config update. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Valid DTO -> Success. | Auth Admin | UTCID01 |
| Invalid DTO -> throw. | Validation fail | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | A | n/a | ValidationException | | | | |

---
## F085 - SystemSettingsService.GetGeneralSettingsAsync

| Header | Value |
|---|---|
| Function Code | F085 |
| Function Name | SystemSettingsService.GetGeneralSettingsAsync |
| Total Test Cases | 1 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate general config retrieval. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Success -> return DTO. | Always | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | GeneralSettingsDto | none | | | | |

---
## F086 - SystemSettingsService.UpdateGeneralSettingsAsync

| Header | Value |
|---|---|
| Function Code | F086 |
| Function Name | SystemSettingsService.UpdateGeneralSettingsAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate general config update. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Valid DTO -> Success. | Auth Admin | UTCID01 |
| Invalid DTO -> throw. | Validation fail | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | A | n/a | ValidationException | | | | |

---
## F087 - SystemSettingsService.GetSecuritySettingsAsync

| Header | Value |
|---|---|
| Function Code | F087 |
| Function Name | SystemSettingsService.GetSecuritySettingsAsync |
| Total Test Cases | 1 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate security config retrieval. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Success -> return DTO. | Always | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | SecuritySettingsDto | none | | | | |

---
## F088 - SystemSettingsService.UpdateSecuritySettingsAsync

| Header | Value |
|---|---|
| Function Code | F088 |
| UTCID01 | N | status dto mapped (contract) | none |  |  |  |  |
| UTCID02 | A | n/a | throws "Payment not found" when null (contract) |  |  |  |  |
| UTCID03 | A | n/a | wrapped exception from SDK (contract) |  |  |  |  |

---
---
## F089 - AppointmentService.GetAvailableSlotsAsync

| Header | Value |
|---|---|
| Function Code | F089 |
| Function Name | AppointmentService.GetAvailableSlotsAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate appointment slot availability logic. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Slots free -> returns list. | Template exists | UTCID01 |
| Slots booked -> filtered out. | Bookings exist | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | List<SlotDto> | none | | | | |
| UTCID02 | N | List<SlotDto> | none | | | | |

---
## F090 - ClinicVisitService.ProcessPaymentAsync

| Header | Value |
|---|---|
| Function Code | F090 |
| Function Name | ClinicVisitService.ProcessPaymentAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate visit payment processing. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Valid Visit -> Success. | Visit exists | UTCID01 |
| Visit missing -> throw. | Invalid ID | UTCID02 |
| Already paid -> throw. | Status=PAID | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | A | n/a | NotFoundException | | | | |
| UTCID03 | A | n/a | InvalidOperationException | | | | |

---
## F091 - PayOSPayoutService.DisburseToClinicAsync

| Header | Value |
|---|---|
| Function Code | F091 |
| Function Name | PayOSPayoutService.DisburseToClinicAsync |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate PayOS disbursement. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Valid request -> Success. | API Auth ok | UTCID01 |
| Insufficient balance -> throw. | Low funds | UTCID02 |
| Invalid bank account -> throw. | Bad account | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | A | n/a | PayOSException | | | | |
| UTCID03 | A | n/a | PayOSException | | | | |

---
## F092 - PayOSPayoutService.GetPayoutHistoryAsync

| Header | Value |
|---|---|
| Function Code | F092 |
| Function Name | PayOSPayoutService.GetPayoutHistoryAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 2 |
| Count type A | 0 |
| Count type B | 0 |
| Test Requirement | Validate disbursement history retrieval. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Has history -> returns list. | Data ok | UTCID01 |
| No history -> empty list. | No data | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | List<PayoutDto> | none | | | | |
| UTCID02 | N | List<PayoutDto> | none | | | | |

---
## F093 - PayOSPayoutService.GetPayoutStatusAsync

| Header | Value |
|---|---|
| Function Code | F093 |
| Function Name | PayOSPayoutService.GetPayoutStatusAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate disbursement status check. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Payout found -> returns status. | Valid ID | UTCID01 |
| Payout missing -> throw. | Invalid ID | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | PayoutStatusDto | none | | | | |
| UTCID02 | A | n/a | PayOSException | | | | |

---
## F094 - PayOSPayoutService.CancelPayoutAsync

| Header | Value |
|---|---|
| Function Code | F094 |
| Function Name | PayOSPayoutService.CancelPayoutAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate disbursement cancellation. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Pending -> Cancelled. | Status=PENDING | UTCID01 |
| Processed -> throw. | Status=COMPLETED | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | A | n/a | PayOSException | | | | |

---
## F095 - PayOSPayoutService.ConfirmPayoutWebhookAsync

| Header | Value |
|---|---|
| Function Code | F095 |
| Function Name | PayOSPayoutService.ConfirmPayoutWebhookAsync |
| Total Test Cases | 2 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 1 |
| Count type B | 0 |
| Test Requirement | Validate disbursement webhook confirmation. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Valid sig -> confirmed. | Success | UTCID01 |
| Invalid sig -> throw. | Tampered | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Success | none | | | | |
| UTCID02 | A | n/a | PayOSException | | | | |

---
## F096 - PatientScreeningPdfService.GenerateScreeningReportPdf

| Header | Value |
|---|---|
| Function Code | F096 |
| Function Name | PatientScreeningPdfService.GenerateScreeningReportPdf |
| Total Test Cases | 3 |
| Created By | |
| Executed By | |
| Lines of Code | |
| Passed | |
| Failed | |
| Untested | |
| Count type N | 1 |
| Count type A | 2 |
| Count type B | 0 |
| Test Requirement | Validate PDF report generation. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Valid data -> returns byte[]. | QuestPDF ok | UTCID01 |
| Missing results -> throw. | Data incomplete | UTCID02 |
| Layout error -> throw. | PDF Engine fail | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | byte[] | none | | | | |
| UTCID02 | A | n/a | InvalidOperationException | | | | |
| UTCID03 | A | n/a | PdfGenerationException | | | | |
---

