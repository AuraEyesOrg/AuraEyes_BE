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
## F002 - AuthService.RegisterOphthalmologistAsync

| Header | Value |
|---|---|
| Function Code | F002 |
| Function Name | AuthService.RegisterOphthalmologistAsync |
| Total Test Cases | 60 |
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
| No credentials (empty Certificates/Degrees) -> Failure. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |
| License only, no Degree -> Failure. | Valid dependencies and data setup for this scenario | UTCID11-UTCID20 |
| Degree only, no License -> Failure. | Valid dependencies and data setup for this scenario | UTCID21-UTCID30 |
| Degree with null DegreeLevel -> Failure. | Valid dependencies and data setup for this scenario | UTCID31-UTCID40 |
| Degree credential with null/empty File -> Failure. | Valid dependencies and data setup for this scenario | UTCID41-UTCID50 |
| License ExpiryDate not greater than IssuedDate -> Failure. | Valid dependencies and data setup for this scenario | UTCID51-UTCID60 |

*UTCID ranges match `AuthServiceTests` methods: `RegisterOphthalmologistAsync_WhenNoCredentials` (01-10), `WhenNoDegree` (11-20), `WhenNoLicense` (21-30), `WhenDegreeLevelMissing` (31-40), `WhenCredentialFileMissing` (41-50), `WhenLicenseExpiryNotGreaterThanIssued` (51-60). Tests use `CreateServiceForValidationOnly` — no logger assertions. Success path, S3 rollback after upload, and confirmation-email warning paths are implemented in `AuthService` but not covered in this test class.*

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.Failure; Errors contains "At least one credential is required" | none |  |  |  |  |
| UTCID02 | A | Result.Failure; Errors contains "At least one credential is required" | none |  |  |  |  |
| UTCID03 | A | Result.Failure; Errors contains "At least one credential is required" | none |  |  |  |  |
| UTCID04 | A | Result.Failure; Errors contains "At least one credential is required" | none |  |  |  |  |
| UTCID05 | A | Result.Failure; Errors contains "At least one credential is required" | none |  |  |  |  |
| UTCID06 | A | Result.Failure; Errors contains "At least one credential is required" | none |  |  |  |  |
| UTCID07 | A | Result.Failure; Errors contains "At least one credential is required" | none |  |  |  |  |
| UTCID08 | A | Result.Failure; Errors contains "At least one credential is required" | none |  |  |  |  |
| UTCID09 | A | Result.Failure; Errors contains "At least one credential is required" | none |  |  |  |  |
| UTCID10 | A | Result.Failure; Errors contains "At least one credential is required" | none |  |  |  |  |
| UTCID11 | A | Result.Failure; Errors contains "At least one degree is required" | none |  |  |  |  |
| UTCID12 | A | Result.Failure; Errors contains "At least one degree is required" | none |  |  |  |  |
| UTCID13 | A | Result.Failure; Errors contains "At least one degree is required" | none |  |  |  |  |
| UTCID14 | A | Result.Failure; Errors contains "At least one degree is required" | none |  |  |  |  |
| UTCID15 | A | Result.Failure; Errors contains "At least one degree is required" | none |  |  |  |  |
| UTCID16 | A | Result.Failure; Errors contains "At least one degree is required" | none |  |  |  |  |
| UTCID17 | A | Result.Failure; Errors contains "At least one degree is required" | none |  |  |  |  |
| UTCID18 | A | Result.Failure; Errors contains "At least one degree is required" | none |  |  |  |  |
| UTCID19 | A | Result.Failure; Errors contains "At least one degree is required" | none |  |  |  |  |
| UTCID20 | A | Result.Failure; Errors contains "At least one degree is required" | none |  |  |  |  |
| UTCID21 | A | Result.Failure; Errors contains "At least one license/certificate is required" | none |  |  |  |  |
| UTCID22 | A | Result.Failure; Errors contains "At least one license/certificate is required" | none |  |  |  |  |
| UTCID23 | A | Result.Failure; Errors contains "At least one license/certificate is required" | none |  |  |  |  |
| UTCID24 | A | Result.Failure; Errors contains "At least one license/certificate is required" | none |  |  |  |  |
| UTCID25 | A | Result.Failure; Errors contains "At least one license/certificate is required" | none |  |  |  |  |
| UTCID26 | A | Result.Failure; Errors contains "At least one license/certificate is required" | none |  |  |  |  |
| UTCID27 | A | Result.Failure; Errors contains "At least one license/certificate is required" | none |  |  |  |  |
| UTCID28 | A | Result.Failure; Errors contains "At least one license/certificate is required" | none |  |  |  |  |
| UTCID29 | A | Result.Failure; Errors contains "At least one license/certificate is required" | none |  |  |  |  |
| UTCID30 | A | Result.Failure; Errors contains "At least one license/certificate is required" | none |  |  |  |  |
| UTCID31 | A | Result.Failure; Errors contains "Degree level is required for degree credentials" | none |  |  |  |  |
| UTCID32 | A | Result.Failure; Errors contains "Degree level is required for degree credentials" | none |  |  |  |  |
| UTCID33 | A | Result.Failure; Errors contains "Degree level is required for degree credentials" | none |  |  |  |  |
| UTCID34 | A | Result.Failure; Errors contains "Degree level is required for degree credentials" | none |  |  |  |  |
| UTCID35 | A | Result.Failure; Errors contains "Degree level is required for degree credentials" | none |  |  |  |  |
| UTCID36 | A | Result.Failure; Errors contains "Degree level is required for degree credentials" | none |  |  |  |  |
| UTCID37 | A | Result.Failure; Errors contains "Degree level is required for degree credentials" | none |  |  |  |  |
| UTCID38 | A | Result.Failure; Errors contains "Degree level is required for degree credentials" | none |  |  |  |  |
| UTCID39 | A | Result.Failure; Errors contains "Degree level is required for degree credentials" | none |  |  |  |  |
| UTCID40 | A | Result.Failure; Errors contains "Degree level is required for degree credentials" | none |  |  |  |  |
| UTCID41 | A | Result.Failure; Errors contains "Credential file is required" | none |  |  |  |  |
| UTCID42 | A | Result.Failure; Errors contains "Credential file is required" | none |  |  |  |  |
| UTCID43 | A | Result.Failure; Errors contains "Credential file is required" | none |  |  |  |  |
| UTCID44 | A | Result.Failure; Errors contains "Credential file is required" | none |  |  |  |  |
| UTCID45 | A | Result.Failure; Errors contains "Credential file is required" | none |  |  |  |  |
| UTCID46 | A | Result.Failure; Errors contains "Credential file is required" | none |  |  |  |  |
| UTCID47 | A | Result.Failure; Errors contains "Credential file is required" | none |  |  |  |  |
| UTCID48 | A | Result.Failure; Errors contains "Credential file is required" | none |  |  |  |  |
| UTCID49 | A | Result.Failure; Errors contains "Credential file is required" | none |  |  |  |  |
| UTCID50 | A | Result.Failure; Errors contains "Credential file is required" | none |  |  |  |  |
| UTCID51 | A | Result.Failure; Errors contains "Certificate expiry date must be later than issued date" | none |  |  |  |  |
| UTCID52 | A | Result.Failure; Errors contains "Certificate expiry date must be later than issued date" | none |  |  |  |  |
| UTCID53 | A | Result.Failure; Errors contains "Certificate expiry date must be later than issued date" | none |  |  |  |  |
| UTCID54 | A | Result.Failure; Errors contains "Certificate expiry date must be later than issued date" | none |  |  |  |  |
| UTCID55 | A | Result.Failure; Errors contains "Certificate expiry date must be later than issued date" | none |  |  |  |  |
| UTCID56 | A | Result.Failure; Errors contains "Certificate expiry date must be later than issued date" | none |  |  |  |  |
| UTCID57 | A | Result.Failure; Errors contains "Certificate expiry date must be later than issued date" | none |  |  |  |  |
| UTCID58 | A | Result.Failure; Errors contains "Certificate expiry date must be later than issued date" | none |  |  |  |  |
| UTCID59 | A | Result.Failure; Errors contains "Certificate expiry date must be later than issued date" | none |  |  |  |  |
| UTCID60 | A | Result.Failure; Errors contains "Certificate expiry date must be later than issued date" | none |  |  |  |  |

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
| UTCID02 | A | Result.IsUnauthorized true; Errors contains "Invalid Google token" | none |  |  |  |  |
| UTCID03 | A | Result.IsUnauthorized true; Errors contains "Invalid Google token" | none |  |  |  |  |
| UTCID04 | A | Result.IsUnauthorized true; Errors contains "Invalid Google token" | none |  |  |  |  |
| UTCID05 | A | Result.IsUnauthorized true; Errors contains "Invalid Google token" | none |  |  |  |  |
| UTCID06 | A | Result.IsUnauthorized true; Errors contains "Invalid Google token" | none |  |  |  |  |
| UTCID07 | A | Result.IsUnauthorized true; Errors contains "Invalid Google token" | none |  |  |  |  |
| UTCID08 | A | Result.IsUnauthorized true; Errors contains "Invalid Google token" | none |  |  |  |  |
| UTCID09 | A | Result.IsUnauthorized true; Errors contains "Invalid Google token" | none |  |  |  |  |
| UTCID10 | A | Result.IsUnauthorized true; Errors contains "Invalid Google token" | none |  |  |  |  |

---
## F005 - AuthService.LoginAsync

| Header | Value |
|---|---|
| Function Code | F005 |
| Function Name | AuthService.LoginAsync |
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
| Login flow exception path -> Failure generic login error. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.Failure; Errors contains "An error occurred during login" | none |  |  |  |  |
| UTCID02 | A | Result.Failure; Errors contains "An error occurred during login" | none |  |  |  |  |
| UTCID03 | A | Result.Failure; Errors contains "An error occurred during login" | none |  |  |  |  |
| UTCID04 | A | Result.Failure; Errors contains "An error occurred during login" | none |  |  |  |  |
| UTCID05 | A | Result.Failure; Errors contains "An error occurred during login" | none |  |  |  |  |
| UTCID06 | A | Result.Failure; Errors contains "An error occurred during login" | none |  |  |  |  |
| UTCID07 | A | Result.Failure; Errors contains "An error occurred during login" | none |  |  |  |  |
| UTCID08 | A | Result.Failure; Errors contains "An error occurred during login" | none |  |  |  |  |
| UTCID09 | A | Result.Failure; Errors contains "An error occurred during login" | none |  |  |  |  |
| UTCID10 | A | Result.Failure; Errors contains "An error occurred during login" | none |  |  |  |  |

---
## F006 - AuthService.VerifyTwoFactorLoginAsync

| Header | Value |
|---|---|
| Function Code | F006 |
| Function Name | AuthService.VerifyTwoFactorLoginAsync |
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
| Verify 2FA flow exception/invalid path -> Failure generic or unauthorized. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.Failure; Errors contains "An error occurred during verification" | none |  |  |  |  |
| UTCID02 | A | Result.Failure; Errors contains "An error occurred during verification" | none |  |  |  |  |
| UTCID03 | A | Result.Failure; Errors contains "An error occurred during verification" | none |  |  |  |  |
| UTCID04 | A | Result.Failure; Errors contains "An error occurred during verification" | none |  |  |  |  |
| UTCID05 | A | Result.Failure; Errors contains "An error occurred during verification" | none |  |  |  |  |
| UTCID06 | A | Result.Failure; Errors contains "An error occurred during verification" | none |  |  |  |  |
| UTCID07 | A | Result.Failure; Errors contains "An error occurred during verification" | none |  |  |  |  |
| UTCID08 | A | Result.Failure; Errors contains "An error occurred during verification" | none |  |  |  |  |
| UTCID09 | A | Result.Failure; Errors contains "An error occurred during verification" | none |  |  |  |  |
| UTCID10 | A | Result.Failure; Errors contains "An error occurred during verification" | none |  |  |  |  |

---
## F007 - AuthService.RefreshTokenAsync

| Header | Value |
|---|---|
| Function Code | F007 |
| Function Name | AuthService.RefreshTokenAsync |
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
| Access token invalid path -> Unauthorized invalid access token. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.IsUnauthorized true; Errors contains "Invalid access token" | none |  |  |  |  |
| UTCID02 | A | Result.IsUnauthorized true; Errors contains "Invalid access token" | none |  |  |  |  |
| UTCID03 | A | Result.IsUnauthorized true; Errors contains "Invalid access token" | none |  |  |  |  |
| UTCID04 | A | Result.IsUnauthorized true; Errors contains "Invalid access token" | none |  |  |  |  |
| UTCID05 | A | Result.IsUnauthorized true; Errors contains "Invalid access token" | none |  |  |  |  |
| UTCID06 | A | Result.IsUnauthorized true; Errors contains "Invalid access token" | none |  |  |  |  |
| UTCID07 | A | Result.IsUnauthorized true; Errors contains "Invalid access token" | none |  |  |  |  |
| UTCID08 | A | Result.IsUnauthorized true; Errors contains "Invalid access token" | none |  |  |  |  |
| UTCID09 | A | Result.IsUnauthorized true; Errors contains "Invalid access token" | none |  |  |  |  |
| UTCID10 | A | Result.IsUnauthorized true; Errors contains "Invalid access token" | none |  |  |  |  |

---
## F008 - AuthService.LogoutAsync

| Header | Value |
|---|---|
| Function Code | F008 |
| Function Name | AuthService.LogoutAsync |
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
| Token not found -> still Success (idempotent). | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |
| Exception when revoking -> Failure. | Valid dependencies and data setup for this scenario | UTCID11-UTCID20 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.Success (idempotent revoke when token missing) | none |  |  |  |  |
| UTCID02 | A | Result.Success (idempotent revoke when token missing) | none |  |  |  |  |
| UTCID03 | A | Result.Success (idempotent revoke when token missing) | none |  |  |  |  |
| UTCID04 | A | Result.Success (idempotent revoke when token missing) | none |  |  |  |  |
| UTCID05 | A | Result.Success (idempotent revoke when token missing) | none |  |  |  |  |
| UTCID06 | A | Result.Success (idempotent revoke when token missing) | none |  |  |  |  |
| UTCID07 | A | Result.Success (idempotent revoke when token missing) | none |  |  |  |  |
| UTCID08 | A | Result.Success (idempotent revoke when token missing) | none |  |  |  |  |
| UTCID09 | A | Result.Success (idempotent revoke when token missing) | none |  |  |  |  |
| UTCID10 | A | Result.Success (idempotent revoke when token missing) | none |  |  |  |  |
| UTCID11 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID12 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID13 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID14 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID15 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID16 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID17 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID18 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID19 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID20 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |

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
| UTCID12 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID13 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID14 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID15 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID16 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID17 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID18 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID19 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |
| UTCID20 | A | Result.Failure; Errors contains "An error occurred during logout" | none |  |  |  |  |

---
## F010 - AuthService.ConfirmEmailAsync

| Header | Value |
|---|---|
| Function Code | F010 |
| Function Name | AuthService.ConfirmEmailAsync |
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
| `userId` cannot parse Guid -> Failure invalid id. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID02 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID03 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID04 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID05 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID06 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID07 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID08 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID09 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID10 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |

---
## F011 - AuthService.ForgotPasswordAsync

| Header | Value |
|---|---|
| Function Code | F011 |
| Function Name | AuthService.ForgotPasswordAsync |
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
| Email does not exist -> no email sent, still Success (anti-enumeration). | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |
| Exception sending mail/token -> Failure. | Valid dependencies and data setup for this scenario | UTCID11-UTCID20 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.Success (user missing still succeeds — anti-enumeration) | none |  |  |  |  |
| UTCID02 | A | Result.Success (user missing still succeeds — anti-enumeration) | none |  |  |  |  |
| UTCID03 | A | Result.Success (user missing still succeeds — anti-enumeration) | none |  |  |  |  |
| UTCID04 | A | Result.Success (user missing still succeeds — anti-enumeration) | none |  |  |  |  |
| UTCID05 | A | Result.Success (user missing still succeeds — anti-enumeration) | none |  |  |  |  |
| UTCID06 | A | Result.Success (user missing still succeeds — anti-enumeration) | none |  |  |  |  |
| UTCID07 | A | Result.Success (user missing still succeeds — anti-enumeration) | none |  |  |  |  |
| UTCID08 | A | Result.Success (user missing still succeeds — anti-enumeration) | none |  |  |  |  |
| UTCID09 | A | Result.Success (user missing still succeeds — anti-enumeration) | none |  |  |  |  |
| UTCID10 | A | Result.Success (user missing still succeeds — anti-enumeration) | none |  |  |  |  |
| UTCID11 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID12 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID13 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID14 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID15 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID16 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID17 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID18 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID19 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID20 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |

---
## F012 - AuthService.ResetPasswordAsync

| Header | Value |
|---|---|
| Function Code | F012 |
| Function Name | AuthService.ResetPasswordAsync |
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
| UserId invalid format -> Failure invalid id. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID02 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID03 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID04 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID05 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID06 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID07 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID08 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID09 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |
| UTCID10 | A | Result.Failure; Errors contains "Invalid user ID" | none |  |  |  |  |

---
## F013 - AuthService.GetCurrentUserAsync

| Header | Value |
|---|---|
| Function Code | F013 |
| Function Name | AuthService.GetCurrentUserAsync |
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
| User not found -> Unauthorized. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.IsUnauthorized true; Errors contains "User not found" | none |  |  |  |  |
| UTCID02 | A | Result.IsUnauthorized true; Errors contains "User not found" | none |  |  |  |  |
| UTCID03 | A | Result.IsUnauthorized true; Errors contains "User not found" | none |  |  |  |  |
| UTCID04 | A | Result.IsUnauthorized true; Errors contains "User not found" | none |  |  |  |  |
| UTCID05 | A | Result.IsUnauthorized true; Errors contains "User not found" | none |  |  |  |  |
| UTCID06 | A | Result.IsUnauthorized true; Errors contains "User not found" | none |  |  |  |  |
| UTCID07 | A | Result.IsUnauthorized true; Errors contains "User not found" | none |  |  |  |  |
| UTCID08 | A | Result.IsUnauthorized true; Errors contains "User not found" | none |  |  |  |  |
| UTCID09 | A | Result.IsUnauthorized true; Errors contains "User not found" | none |  |  |  |  |
| UTCID10 | A | Result.IsUnauthorized true; Errors contains "User not found" | none |  |  |  |  |

---
## F014 - AuthService.ResendConfirmationAsync

| Header | Value |
|---|---|
| Function Code | F014 |
| Function Name | AuthService.ResendConfirmationAsync |
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
| User missing / already confirmed -> still Success. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |
| Exception sending email -> Failure. | Valid dependencies and data setup for this scenario | UTCID11-UTCID20 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.Success (`ResendConfirmationAsync_WhenUserMissing`) | none |  |  |  |  |
| UTCID02 | A | Result.Success (`ResendConfirmationAsync_WhenUserMissing`) | none |  |  |  |  |
| UTCID03 | A | Result.Success (`ResendConfirmationAsync_WhenUserMissing`) | none |  |  |  |  |
| UTCID04 | A | Result.Success (`ResendConfirmationAsync_WhenUserMissing`) | none |  |  |  |  |
| UTCID05 | A | Result.Success (`ResendConfirmationAsync_WhenUserMissing`) | none |  |  |  |  |
| UTCID06 | A | Result.Success (`ResendConfirmationAsync_WhenUserMissing`) | none |  |  |  |  |
| UTCID07 | A | Result.Success (`ResendConfirmationAsync_WhenUserMissing`) | none |  |  |  |  |
| UTCID08 | A | Result.Success (`ResendConfirmationAsync_WhenUserMissing`) | none |  |  |  |  |
| UTCID09 | A | Result.Success (`ResendConfirmationAsync_WhenUserMissing`) | none |  |  |  |  |
| UTCID10 | A | Result.Success (`ResendConfirmationAsync_WhenUserMissing`) | none |  |  |  |  |
| UTCID11 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID12 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID13 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID14 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID15 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID16 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID17 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID18 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID19 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |
| UTCID20 | A | Result.Failure; Errors contains "An error occurred while processing your request" | none |  |  |  |  |

---
## F015 - IdentityService.CheckPasswordAsync

| Header | Value |
|---|---|
| Function Code | F015 |
| Function Name | IdentityService.CheckPasswordAsync |
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
| UserId not found -> false. | Valid dependencies and data setup for this scenario | UTCID01 |
| Wrong password -> false. | Valid dependencies and data setup for this scenario | UTCID02 |
| Correct password -> true. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | `false` when user id not found (`CheckPasswordAsync` returns false) | none |  |  |  |  |
| UTCID02 | A | `false` when password mismatch (Identity `CheckPasswordAsync`) | none |  |  |  |  |
| UTCID03 | N | `true` when password correct (not covered by Infrastructure.UnitTests) | none |  |  |  |  |

---
## F016 - IdentityService.GetUserByEmailAsync

| Header | Value |
|---|---|
| Function Code | F016 |
| Function Name | IdentityService.GetUserByEmailAsync |
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
| User exists and !IsDeleted -> returns `UserDto`. | Valid dependencies and data setup for this scenario | UTCID01 |
| User is IsDeleted -> null. | Valid dependencies and data setup for this scenario | UTCID02 |
| Email not found -> null. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | `UserDto` mapped when user exists and !IsDeleted (contract; add targeted test) | none |  |  |  |  |
| UTCID02 | A | `null` when user row is soft-deleted (contract) | none |  |  |  |  |
| UTCID03 | A | `null` when no match (`GetUserByEmailAsync_WhenMissing_ShouldReturnNull`) | none |  |  |  |  |

---
## F017 - IdentityService.GetUserByIdAsync

| Header | Value |
|---|---|
| Function Code | F017 |
| Function Name | IdentityService.GetUserByIdAsync |
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
| User exists and !IsDeleted -> full DTO. | Valid dependencies and data setup for this scenario | UTCID01 |
| User deleted -> null. | Valid dependencies and data setup for this scenario | UTCID02 |
| UserId not found -> null. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | `UserDto` when exists !IsDeleted (contract) | none |  |  |  |  |
| UTCID02 | A | `null` when deleted (contract) | none |  |  |  |  |
| UTCID03 | A | `null` when missing (`GetUserByIdAsync_WhenMissing_ShouldReturnNull`) | none |  |  |  |  |

---
## F018 - IdentityService.IsPhoneNumberInUseByOrganizationAsync

| Header | Value |
|---|---|
| Function Code | F018 |
| Function Name | IdentityService.IsPhoneNumberInUseByOrganizationAsync |
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
| Empty phone / whitespace-only -> false. | Valid dependencies and data setup for this scenario | UTCID01 |
| Same number, different format (`+84`, spaces, dashes) -> true. | Valid dependencies and data setup for this scenario | UTCID02 |
| Number exists but different organisation -> false. | Valid dependencies and data setup for this scenario | UTCID03 |
| Same org but user deleted -> false. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | B | `false` when normalized phone empty/whitespace (`IsPhoneNumberInUseByOrganizationAsync`) | none |  |  |  |  |
| UTCID02 | N | `true` when same org + suffix match after normalize (`WithNormalizedPhone_ShouldMatch`) | none |  |  |  |  |
| UTCID03 | A | `false` when number used in another organisation (contract) | none |  |  |  |  |
| UTCID04 | A | `false` when user deleted (contract) | none |  |  |  |  |

---
## F019 - IdentityService.IsEmailConfirmedAsync

| Header | Value |
|---|---|
| Function Code | F019 |
| Function Name | IdentityService.IsEmailConfirmedAsync |
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
| User not found -> false. | Valid dependencies and data setup for this scenario | UTCID01 |
| User exists, EmailConfirmed=false -> false. | Valid dependencies and data setup for this scenario | UTCID02 |
| User exists, EmailConfirmed=true -> true. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | `false` when user missing (`IsEmailConfirmedAsync_WhenMissing`) | none |  |  |  |  |
| UTCID02 | N | `false` when user exists but not confirmed (contract) | none |  |  |  |  |
| UTCID03 | N | `true` when EmailConfirmed (contract) | none |  |  |  |  |

---
## F020 - IdentityService.IsUserActiveAsync

| Header | Value |
|---|---|
| Function Code | F020 |
| Function Name | IdentityService.IsUserActiveAsync |
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
| User not found -> false. | Valid dependencies and data setup for this scenario | UTCID01 |
| User inactive or deleted -> false. | Valid dependencies and data setup for this scenario | UTCID02 |
| User active and !deleted -> true. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | `false` when missing (`IsUserActiveAsync_WhenMissing`) | none |  |  |  |  |
| UTCID02 | A | `false` when inactive or deleted (contract) | none |  |  |  |  |
| UTCID03 | N | `true` when active and !deleted (contract) | none |  |  |  |  |

---
## F021 - IdentityService.GenerateEmailConfirmationTokenAsync

| Header | Value |
|---|---|
| Function Code | F021 |
| Function Name | IdentityService.GenerateEmailConfirmationTokenAsync |
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
| User not found -> throw `InvalidOperationException`. | Valid dependencies and data setup for this scenario | UTCID01 |
| User exists -> token non-empty. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | n/a | InvalidOperationException; message contains User not found |  |  |  |  |
| UTCID02 | N | non-empty token string when user exists (contract; only missing-user test in suite) | none |  |  |  |  |

---
## F022 - IdentityService.GeneratePasswordResetTokenAsync

| Header | Value |
|---|---|
| Function Code | F022 |
| Function Name | IdentityService.GeneratePasswordResetTokenAsync |
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
| User not found -> throw. | Valid dependencies and data setup for this scenario | UTCID01 |
| User exists -> token non-empty. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | n/a | InvalidOperationException; message contains User not found |  |  |  |  |
| UTCID02 | N | non-empty reset token when user exists (contract) | none |  |  |  |  |

---
## F023 - IdentityService.GetUserRolesAsync

| Header | Value |
|---|---|
| Function Code | F023 |
| Function Name | IdentityService.GetUserRolesAsync |
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
| User not found -> empty list. | Valid dependencies and data setup for this scenario | UTCID01 |
| User has one role -> that role returned. | Valid dependencies and data setup for this scenario | UTCID02 |
| User has multiple roles -> all roles returned. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | empty list when user missing (`GetUserRolesAsync_WhenMissing`) | none |  |  |  |  |
| UTCID02 | N | roles list includes assigned role (via `CreateUserWithRoleAsync` path) | none |  |  |  |  |
| UTCID03 | N | multiple roles returned (contract) | none |  |  |  |  |

---
## F024 - IdentityService.IsInRoleAsync

| Header | Value |
|---|---|
| Function Code | F024 |
| Function Name | IdentityService.IsInRoleAsync |
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
| User not found -> false. | Valid dependencies and data setup for this scenario | UTCID01 |
| User has role -> true. | Valid dependencies and data setup for this scenario | UTCID02 |
| User has no role -> false. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
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
| Function Code | F026 |
| Function Name | IdentityService.UpdateLastLoginAsync |
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
| User not found -> no-op. | Valid dependencies and data setup for this scenario | UTCID01 |
| User exists -> LastLoginAt updated + `UpdateAsync` called. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | no-op completes (`UpdateLastLoginAsync_WhenMissingUser_ShouldNotThrow`) | none |  |  |  |  |
| UTCID02 | N | LastLogin updated and persisted when user exists (contract) | none |  |  |  |  |

---
## F027 - IdentityService.IsTwoFactorEnabledAsync

| Header | Value |
|---|---|
| Function Code | F027 |
| Function Name | IdentityService.IsTwoFactorEnabledAsync |
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
| User not found -> false. | Valid dependencies and data setup for this scenario | UTCID01 |
| User 2FA=false -> false. | Valid dependencies and data setup for this scenario | UTCID02 |
| User 2FA=true -> true. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | `false` when user missing (contract) | none |  |  |  |  |
| UTCID02 | N | `false` when 2FA disabled (contract) | none |  |  |  |  |
| UTCID03 | N | `true` when 2FA enabled (contract) | none |  |  |  |  |

---
## F028 - IdentityService.GetAuthenticatorKeyAsync

| Header | Value |
|---|---|
| Function Code | F028 |
| Function Name | IdentityService.GetAuthenticatorKeyAsync |
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
| User not found -> null. | Valid dependencies and data setup for this scenario | UTCID01 |
| User exists -> returns key. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | `null` when user missing (contract) | none |  |  |  |  |
| UTCID02 | N | shared key string when present (contract) | none |  |  |  |  |

---
## F029 - IdentityService.GetOrCreateAuthenticatorKeyAsync

| Header | Value |
|---|---|
| Function Code | F029 |
| Function Name | IdentityService.GetOrCreateAuthenticatorKeyAsync |
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
| User not found -> throw. | Valid dependencies and data setup for this scenario | UTCID01 |
| User exists -> reset key and obtain new key successfully. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | n/a | InvalidOperationException User not found |  |  |  |  |
| UTCID02 | N | key string after reset/create path (contract) | none |  |  |  |  |

---
## F030 - IdentityService.VerifyTwoFactorCodeAsync

| Header | Value |
|---|---|
| Function Code | F030 |
| Function Name | IdentityService.VerifyTwoFactorCodeAsync |
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
| User not found -> false. | Valid dependencies and data setup for this scenario | UTCID01 |
| Wrong code -> false. | Valid dependencies and data setup for this scenario | UTCID02 |
| Correct code -> true. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | `false` missing user (`VerifyTwoFactorCodeAsync_WhenMissing`) | none |  |  |  |  |
| UTCID02 | A | `false` wrong code (contract) | none |  |  |  |  |
| UTCID03 | N | `true` valid code (contract) | none |  |  |  |  |

---
## F031 - IdentityService.GenerateNewRecoveryCodesAsync

| Header | Value |
|---|---|
| Function Code | F031 |
| Function Name | IdentityService.GenerateNewRecoveryCodesAsync |
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
| User not found -> throw. | Valid dependencies and data setup for this scenario | UTCID01 |
| User exists -> returns recovery codes per count. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | n/a | InvalidOperationException User not found |  |  |  |  |
| UTCID02 | N | string[] recovery codes (contract) | none |  |  |  |  |

---
## F032 - IdentityService.GetRecoveryCodesCountAsync

| Header | Value |
|---|---|
| Function Code | F032 |
| Function Name | IdentityService.GetRecoveryCodesCountAsync |
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
| User not found -> 0. | Valid dependencies and data setup for this scenario | UTCID01 |
| User exists -> correct count. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | `0` when missing (`GetRecoveryCodesCountAsync_WhenMissing`) | none |  |  |  |  |
| UTCID02 | N | count from store when user exists (contract) | none |  |  |  |  |

---
## F033 - IdentityService.GenerateAuthenticatorUri

| Header | Value |
|---|---|
| Function Code | F033 |
| Function Name | IdentityService.GenerateAuthenticatorUri |
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
| Valid email + key -> URI uses `otpauth://totp` schema. | Valid dependencies and data setup for this scenario | UTCID01 |
| Email with special characters -> UrlEncoded correctly. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | URI starts with otpauth://totp/; contains secret= and issuer= (`GenerateAuthenticatorUri_ShouldContainExpectedPayload`) | none |  |  |  |  |
| UTCID02 | B | URI valid for special emails; key embedded (`GenerateAuthenticatorUri` theory) | none |  |  |  |  |

---
## F034 - IdentityService.FormatAuthenticatorKey

| Header | Value |
|---|---|
| Function Code | F034 |
| Function Name | IdentityService.FormatAuthenticatorKey |
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
| Long key -> 4-char blocks + uppercase. | Valid dependencies and data setup for this scenario | UTCID01 |
| Key shorter than 4 -> still uppercase, no error. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | grouped uppercase string equals expected (`FormatAuthenticatorKey_ShouldGroupAndUppercase`) | none |  |  |  |  |
| UTCID02 | B | shorter keys still grouped/uppercased per algorithm (theory) | none |  |  |  |  |

---
## F035 - IdentityService.GetUserMetricsAsync

| Header | Value |
|---|---|
| Function Code | F035 |
| Function Name | IdentityService.GetUserMetricsAsync |
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
| Data with prior month history -> percent change calculated correctly. | Valid dependencies and data setup for this scenario | UTCID01 |
| Prior month =0 but new users -> change =100. | Valid dependencies and data setup for this scenario | UTCID02 |
| Pending approvals counted correctly per rules. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | UserMetricsDto with computed counts (contract / add Dashboard-level test) | none |  |  |  |  |
| UTCID02 | B | percent change edge cases (contract) | none |  |  |  |  |
| UTCID03 | N | pending approvals embedded in metrics (contract) | none |  |  |  |  |

---
## F036 - IdentityService.GetUsersInRoleCountAsync

| Header | Value |
|---|---|
| Function Code | F036 |
| Function Name | IdentityService.GetUsersInRoleCountAsync |
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
| `activeOnly=true` -> count only active + !deleted. | Valid dependencies and data setup for this scenario | UTCID01 |
| `activeOnly=false` -> count all !deleted. | Valid dependencies and data setup for this scenario | UTCID02 |
| Role has no users -> 0. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | count active-only=true (contract) | none |  |  |  |  |
| UTCID02 | N | count active-only=false (contract) | none |  |  |  |  |
| UTCID03 | B | `0` when role empty (contract) | none |  |  |  |  |

---
## F037 - IdentityService.GetPendingApprovalsCountAsync

| Header | Value |
|---|---|
| Function Code | F037 |
| Function Name | IdentityService.GetPendingApprovalsCountAsync |
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
| Count users matching `!EmailConfirmed || !IsActive` correctly. | Valid dependencies and data setup for this scenario | UTCID01 |
| No pending -> 0. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | pending count per query (`GetPendingApprovalsCountAsync_WithNoUsers_ShouldBeZero` for empty db) | none |  |  |  |  |
| UTCID02 | B | `0` when no pending (contract) | none |  |  |  |  |

---
## F038 - IdentityService.GetUserDetailsAsync

| Header | Value |
|---|---|
| Function Code | F038 |
| Function Name | IdentityService.GetUserDetailsAsync |
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
| User not found -> null. | Valid dependencies and data setup for this scenario | UTCID01 |
| User deleted -> null. | Valid dependencies and data setup for this scenario | UTCID02 |
| Valid user -> DTO with full profile. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | `null` missing (`GetUserDetailsAsync_WhenMissingUser`) | none |  |  |  |  |
| UTCID02 | A | `null` when deleted (contract) | none |  |  |  |  |
| UTCID03 | N | `UserDetailsDto` when valid user (contract) | none |  |  |  |  |

---
## F039 - RefreshTokenService.CreateRefreshTokenAsync

| Header | Value |
|---|---|
| Function Code | F039 |
| Function Name | RefreshTokenService.CreateRefreshTokenAsync |
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
| Refresh token created and saved -> return Id. | Valid dependencies and data setup for this scenario | UTCID01 |
| Persisted data correct for `UserId`, `JwtId`, `TokenHash`. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | non-empty Guid; row persisted with UserId/TokenHash/JwtId (`CreateRefreshTokenAsync_ShouldPersistAndReturnId`) | none |  |  |  |  |
| UTCID02 | N | ExpiresAt in future for custom expiry days (`CreateRefreshTokenAsync_WithCustomExpiry` theory) | none |  |  |  |  |

---
## F040 - RefreshTokenService.GetByTokenHashAsync

| Header | Value |
|---|---|
| Function Code | F040 |
| Function Name | RefreshTokenService.GetByTokenHashAsync |
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
| Hash not found -> null. | Valid dependencies and data setup for this scenario | UTCID01 |
| Hash exists -> DTO mapped with correct fields. | Valid dependencies and data setup for this scenario | UTCID02 |
| Token revoked/used -> `IsActive=false` in DTO. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | `null` when hash missing (`GetByTokenHashAsync_WhenNotFound`) | none |  |  |  |  |
| UTCID02 | N | RefreshTokenDto with Id/UserId/IsActive true when found | none |  |  |  |  |
| UTCID03 | N | DTO reflects revoked/used state when token inactive (contract) | none |  |  |  |  |

---
## F041 - RefreshTokenService.RotateRefreshTokenAsync

| Header | Value |
|---|---|
| Function Code | F041 |
| Function Name | RefreshTokenService.RotateRefreshTokenAsync |
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
| Old token id not found -> throw. | Valid dependencies and data setup for this scenario | UTCID01 |
| Old token exists -> new token created successfully. | Valid dependencies and data setup for this scenario | UTCID02 |
| Old token `MarkAsUsed` and linked to new token. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | n/a | InvalidOperationException "Original token not found" |  |  |  |  |
| UTCID02 | N | new token id; old marked used (`RotateRefreshTokenAsync_ShouldCreateNewAndMarkOldUsed`) | none |  |  |  |  |
| UTCID03 | N | metadata+expiry on new token (`RotateRefreshTokenAsync_WithMetadataAndExpiry` theory) | none |  |  |  |  |

---
## F042 - RefreshTokenService.RevokeTokenAsync

| Header | Value |
|---|---|
| Function Code | F042 |
| Function Name | RefreshTokenService.RevokeTokenAsync |
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
| Token exists -> revoke and save. | Valid dependencies and data setup for this scenario | UTCID01 |
| Token not found -> no-op, does not throw. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | RevokedAt/Reason set (`RevokeTokenAsync_WhenTokenExists`) | none |  |  |  |  |
| UTCID02 | N | no throw when token id unknown (`RevokeTokenAsync_WhenTokenMissing`) | none |  |  |  |  |

---
## F043 - RefreshTokenService.RevokeAllUserTokensAsync

| Header | Value |
|---|---|
| Function Code | F043 |
| Function Name | RefreshTokenService.RevokeAllUserTokensAsync |
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
| Multiple unre revoked tokens -> all revoked. | Valid dependencies and data setup for this scenario | UTCID01 |
| No valid tokens -> save still safe. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | all user tokens revoked (`RevokeAllUserTokensAsync_ShouldRevokeAllMatchingTokens`) | none |  |  |  |  |
| UTCID02 | B | safe when no rows / other users unaffected (theory + `ShouldNotAffectOtherUsersTokens`) | none |  |  |  |  |

---
## F044 - RefreshTokenService.RevokeTokenFamilyAsync

| Header | Value |
|---|---|
| Function Code | F044 |
| Function Name | RefreshTokenService.RevokeTokenFamilyAsync |
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
| Root token missing -> return, does not throw. | Valid dependencies and data setup for this scenario | UTCID01 |
| Token exists -> revoke all user tokens. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | completes without throw when token missing (`RevokeTokenFamilyAsync_WhenTokenMissing`) | none |  |  |  |  |
| UTCID02 | N | all tokens for user revoked with reason (`RevokeTokenFamilyAsync_ShouldRevokeAllUserTokensWhenTokenFound`) | none |  |  |  |  |

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
## F046 - TokenService.GenerateAccessTokenAsync

| Header | Value |
|---|---|
| Function Code | F046 |
| Function Name | TokenService.GenerateAccessTokenAsync |
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
| Token has required claims (`sub`,`uid`,`email`,`jti`,`iat`). | Valid dependencies and data setup for this scenario | UTCID01 |
| Roles added to `ClaimTypes.Role` and `role`. | Valid dependencies and data setup for this scenario | UTCID02 |
| Additional claims appended correctly. | Valid dependencies and data setup for this scenario | UTCID03 |
| ExpiresAt matches `AccessTokenExpiryMinutes` config. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | AccessToken + Jti non-empty; claims sub/email/jti (`GenerateAccessTokenAsync_ShouldReturnJwt_AndClaims`) | none |  |  |  |  |
| UTCID02 | N | Role claims on ClaimTypes.Role and role (`ShouldIncludeAllRoleClaimVariants` + single-role theory) | none |  |  |  |  |
| UTCID03 | N | Additional custom claims present (`GenerateAccessTokenAsync_ShouldIncludeAdditionalClaimPair`) | none |  |  |  |  |
| UTCID04 | B | exp ~ AccessTokenExpiryMinutes from JwtSettings (contract) | none |  |  |  |  |

---
## F047 - TokenService.GenerateRefreshToken

| Header | Value |
|---|---|
| Function Code | F047 |
| Function Name | TokenService.GenerateRefreshToken |
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
| Valid non-empty base64 token. | Valid dependencies and data setup for this scenario | UTCID01 |
| Two consecutive generations differ. | Valid dependencies and data setup for this scenario | UTCID02 |
| Token length large enough (>=64 random bytes before encode). | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | non-empty base64; two calls differ (`GenerateRefreshToken_ShouldCreateNonEmptyUniqueValues`) | none |  |  |  |  |
| UTCID02 | N | same as UTCID01 (duplicate condition slot) | none |  |  |  |  |
| UTCID03 | B | Base64 decodes to 64 bytes (`GenerateRefreshToken_ShouldProduceBase64StringWithExpectedEntropy`) | none |  |  |  |  |

---
## F048 - TokenService.ValidateToken

| Header | Value |
|---|---|
| Function Code | F048 |
| Function Name | TokenService.ValidateToken |
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
| Valid token -> returns ClaimsPrincipal. | Valid dependencies and data setup for this scenario | UTCID01 |
| Invalid token signature -> null. | Valid dependencies and data setup for this scenario | UTCID02 |
| Token alg not HmacSha256 -> null. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | ClaimsPrincipal non-null for valid JWT (`ValidateToken_WithValidToken`) | none |  |  |  |  |
| UTCID02 | A | `null` for bad signature/string (`ValidateToken_WithInvalidToken` + invalid inputs theory) | none |  |  |  |  |
| UTCID03 | A | `null` when algorithm/validation fails (contract) | none |  |  |  |  |

---
## F049 - TokenService.GetUserIdFromToken

| Header | Value |
|---|---|
| Function Code | F049 |
| Function Name | TokenService.GetUserIdFromToken |
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
| Valid token with `sub` -> Guid parses successfully. | Valid dependencies and data setup for this scenario | UTCID01 |
| No `sub` but has `uid`/`nameidentifier` -> still resolved. | Valid dependencies and data setup for this scenario | UTCID02 |
| User id claim cannot be parsed -> null. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Guid equals user sub (`GetUserIdFromToken_AndGetJtiFromToken_ShouldExtractValues`) | none |  |  |  |  |
| UTCID02 | N | Guid from uid fallback (`GetUserIdFromToken_WhenSubMissing_ShouldFallbackToUid`) | none |  |  |  |  |
| UTCID03 | A | `null` bad guid / corrupt / invalid format (theories + `WhenTokenCorrupted`) | none |  |  |  |  |

---
## F050 - TokenService.GetJtiFromToken

| Header | Value |
|---|---|
| Function Code | F050 |
| Function Name | TokenService.GetJtiFromToken |
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
| Valid token -> read `jti`. | Valid dependencies and data setup for this scenario | UTCID01 |
| Invalid token -> null. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | jti matches token Jti (`GetJtiFromToken_WithValidToken_ShouldReturnTokenJti`) | none |  |  |  |  |
| UTCID02 | A | `null` invalid token (`GetJtiFromToken_WithInvalidInputs_ShouldReturnNull`) | none |  |  |  |  |

---
## F051 - TokenService.HashToken

| Header | Value |
|---|---|
| Function Code | F051 |
| Function Name | TokenService.HashToken |
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
| Same input -> same hash. | Valid dependencies and data setup for this scenario | UTCID01 |
| Different input -> different hash. | Valid dependencies and data setup for this scenario | UTCID02 |
| Empty input -> still hashable (non-null). | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | deterministic same input (`HashToken_ShouldBeDeterministic_AndDifferentForDifferentInput`) | none |  |  |  |  |
| UTCID02 | N | different hash different input | none |  |  |  |  |
| UTCID03 | B | non-empty for many inputs (theories) | none |  |  |  |  |

---
## F052 - AdminQueryService.GetOphthalmologistsAsync

| Header | Value |
|---|---|
| Function Code | F052 |
| Function Name | AdminQueryService.GetOphthalmologistsAsync |
| Total Test Cases | 5 |
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
| No filter -> paging + CreatedAt desc sort correct. | Valid dependencies and data setup for this scenario | UTCID01 |
| SearchTerm matches fullName/email/phone with ILike. | Valid dependencies and data setup for this scenario | UTCID02 |
| VerificationStatus comma-separated values -> filter correct. | Valid dependencies and data setup for this scenario | UTCID03 |
| Map licenses/degrees from Certificates by Type correctly. | Valid dependencies and data setup for this scenario | UTCID04 |
| VerificationStatus unparsable -> skip status filter. | Valid dependencies and data setup for this scenario | UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Paged empty when no data (`GetOphthalmologistsAsync_WhenNoData`) | none |  |  |  |  |
| UTCID02 | N | SearchTerm filters name/email/phone (contract; tests use verification + paging) | none |  |  |  |  |
| UTCID03 | N | VerificationStatus filter union (`WithMultipleVerificationStatuses_ShouldReturnUnion`) | none |  |  |  |  |
| UTCID04 | N | Certificates mapped to licenses/degrees (contract) | none |  |  |  |  |
| UTCID05 | B | Invalid verification tokens skipped / handled (`VerificationFilter_WithInvalidOrMixedValues`) | none |  |  |  |  |

---
## F053 - AdminQueryService.GetPatientsAsync

| Header | Value |
|---|---|
| Function Code | F053 |
| Function Name | AdminQueryService.GetPatientsAsync |
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
| SearchTerm works on fullName/email. | Valid dependencies and data setup for this scenario | UTCID01 |
| Filter status `active/pending/suspended` correct. | Valid dependencies and data setup for this scenario | UTCID02 |
| Paging correct totalCount/items. | Valid dependencies and data setup for this scenario | UTCID03 |
| Deleted user not shown. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | SearchTerm on name/email (contract; tests cover status filters) | none |  |  |  |  |
| UTCID02 | N | active/pending/suspended filters (`GetPatientsAsync_With*Status`) | none |  |  |  |  |
| UTCID03 | N | Paging totals (`GetPatientsAsync_WithPaging` / related) | none |  |  |  |  |
| UTCID04 | A | Deleted users excluded (`GetPatientsAsync_ShouldExcludeDeletedUsers`) | none |  |  |  |  |

---
## F054 - AdminQueryService.GetAuditLogsAsync

| Header | Value |
|---|---|
| Function Code | F054 |
| Function Name | AdminQueryService.GetAuditLogsAsync |
| Total Test Cases | 5 |
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
| Filter by search term action/entity/email correct. | Valid dependencies and data setup for this scenario | UTCID01 |
| Filter by action/entityName/userId correct. | Valid dependencies and data setup for this scenario | UTCID02 |
| Filter by fromDate/toDate correct. | Valid dependencies and data setup for this scenario | UTCID03 |
| Paging + `CreatedAt desc` sort correct. | Valid dependencies and data setup for this scenario | UTCID04 |
| Log without user join -> `UserName=null` still maps. | Valid dependencies and data setup for this scenario | UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Action filter (`GetAuditLogsAsync_WithActionFilter`) | none |  |  |  |  |
| UTCID02 | N | Date range (`WithDateRange`) | none |  |  |  |  |
| UTCID03 | N | Paging (`WithPaging`) | none |  |  |  |  |
| UTCID04 | N | User filter (`WithUserFilter`) | none |  |  |  |  |
| UTCID05 | B | Entity filter + unknown user name null (`WithEntityFilter` / `WithUnknownUser`) | none |  |  |  |  |

---
## F055 - AiQuotaService.GetQuotaAsync

| Header | Value |
|---|---|
| Function Code | F055 |
| Function Name | AiQuotaService.GetQuotaAsync |
| Total Test Cases | 5 |
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
| Patient with profile -> total/used/remaining/free+purchased correct. | Valid dependencies and data setup for this scenario | UTCID01 |
| Patient without profile -> fallback free quota. | Valid dependencies and data setup for this scenario | UTCID02 |
| OrgAdmin/Ophthalmologist with org -> quota by organisation. | Valid dependencies and data setup for this scenario | UTCID03 |
| Organisation role but user has no org -> None quota. | Valid dependencies and data setup for this scenario | UTCID04 |
| Unknown role -> None quota. | Valid dependencies and data setup for this scenario | UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | AiQuotaDto for patient with profile (`GetQuotaAsync_ForPatient`) | none |  |  |  |  |
| UTCID02 | A | Free fallback when patient missing profile (`GetQuotaAsync_WhenPatientMissing_ShouldReturnFreeFallbackQuota`) | none |  |  |  |  |
| UTCID03 | N | Organisation quota for org-linked roles (`ForOrgRole` / ophthalmologist) | none |  |  |  |  |
| UTCID04 | A | None when org role but no org (`ForOrgRole_WhenUserMissing_ShouldReturnNone`) | none |  |  |  |  |
| UTCID05 | A | None / unsupported role (`ForUnknownRole` / `WithUnsupportedRole`) | none |  |  |  |  |

---
## F056 - AiQuotaService.HasAvailableQuotaAsync

| Header | Value |
|---|---|
| Function Code | F056 |
| Function Name | AiQuotaService.HasAvailableQuotaAsync |
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
| Remaining > 0 -> true. | Valid dependencies and data setup for this scenario | UTCID01 |
| Remaining = 0 -> false. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | `true` when remaining > 0 (`HasAvailableQuotaAsync_ShouldReflectRemainingQuota`) | none |  |  |  |  |
| UTCID02 | B | `false` when exhausted (contract; mirrored in theory rows) | none |  |  |  |  |

---
## F057 - AiQuotaService.DeductQuotaAsync

| Header | Value |
|---|---|
| Function Code | F057 |
| Function Name | AiQuotaService.DeductQuotaAsync |
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
| Patient exists -> consume quota + save. | Valid dependencies and data setup for this scenario | UTCID01 |
| Org role and org exist -> consume organisation quota + save. | Valid dependencies and data setup for this scenario | UTCID02 |
| Patient not found -> throw `InvalidOperationException`. | Valid dependencies and data setup for this scenario | UTCID03 |
| Org role user without org -> throw. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Patient used quota increments (`DeductQuotaAsync_ForPatient`) | none |  |  |  |  |
| UTCID02 | N | Org used quota increments (`DeductQuotaAsync_ForOrgAdmin`) | none |  |  |  |  |
| UTCID03 | A | n/a | InvalidOperationException when patient missing |  |  |  |  |
| UTCID04 | A | n/a | InvalidOperationException org role without org |  |  |  |  |

---
## F058 - AiQuotaService.AddPurchasedQuotaAsync

| Header | Value |
|---|---|
| Function Code | F058 |
| Function Name | AiQuotaService.AddPurchasedQuotaAsync |
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
| Patient exists -> increase purchased quota. | Valid dependencies and data setup for this scenario | UTCID01 |
| Org role with org -> increase org purchased quota. | Valid dependencies and data setup for this scenario | UTCID02 |
| Patient not found -> throw. | Valid dependencies and data setup for this scenario | UTCID03 |
| Organisation not found -> throw. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | Patient purchased increases (`AddPurchasedQuotaAsync_ForPatient`) | none |  |  |  |  |
| UTCID02 | N | Org purchased increases (`AddPurchasedQuotaAsync_ForOrgAdmin`) | none |  |  |  |  |
| UTCID03 | A | n/a | throws when patient missing (add-purchase path; mirror deduct tests) |  |  |  |  |
| UTCID04 | A | n/a | InvalidOperationException when org role without org (`AddPurchasedQuotaAsync_WhenOrgRoleWithoutOrg`) |  |  |  |  |

---
## F059 - BetterStackHeartbeatService.GetEmbedUrl

| Header | Value |
|---|---|
| Function Code | F059 |
| Function Name | BetterStackHeartbeatService.GetEmbedUrl |
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
| EmbedUrl null/whitespace -> null. | Valid dependencies and data setup for this scenario | UTCID01 |
| Valid EmbedUrl -> returns correct value. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | B | `null` when embed blank/whitespace (`GetEmbedUrl_WhenBlank` / whitespace theory) | none |  |  |  |  |
| UTCID02 | N | returns configured string (`GetEmbedUrl_WhenProvided`) | none |  |  |  |  |

---
## F060 - BetterStackHeartbeatService.GetMonitorDescriptors

| Header | Value |
|---|---|
| Function Code | F060 |
| Function Name | BetterStackHeartbeatService.GetMonitorDescriptors |
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
| Return descriptors for all monitor enums. | Valid dependencies and data setup for this scenario | UTCID01 |
| `Configured` field matches endpoint setting. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | descriptors for all monitors (`GetMonitorDescriptors_ShouldReturnKnownMonitors` / include all enum) | none |  |  |  |  |
| UTCID02 | N | Configured flag matches options (`ShouldMapMonitorToExpectedKey` theory) | none |  |  |  |  |

---
## F061 - BetterStackHeartbeatService.NotifyStartedAsync

| Header | Value |
|---|---|
| Function Code | F061 |
| Function Name | BetterStackHeartbeatService.NotifyStartedAsync |
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
| Endpoint not configured -> skip, does not throw. | Valid dependencies and data setup for this scenario | UTCID01 |
| Endpoint configured -> POST succeeds. | Valid dependencies and data setup for this scenario | UTCID02 |
| HTTP failure/exception -> warning log, does not throw. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | B | HTTP skipped when no URL (`NotifyStartedAsync_WhenNoUrlsConfigured_ShouldSkipRequest`) | none |  |  |  |  |
| UTCID02 | N | POST when start/ping configured (multiple tests) | none |  |  |  |  |
| UTCID03 | A | failure logged warning; no throw (contract covered by resilient HTTP tests) | none |  |  |  |  |

---
## F062 - BetterStackHeartbeatService.NotifySucceededAsync

| Header | Value |
|---|---|
| Function Code | F062 |
| Function Name | BetterStackHeartbeatService.NotifySucceededAsync |
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
| Valid ping URL -> POST called. | Valid dependencies and data setup for this scenario | UTCID01 |
| Non-success status -> warning log. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | POST ping when configured (`NotifySucceededAsync_WhenPingConfigured`) | none |  |  |  |  |
| UTCID02 | A | non-success -> warning log (contract) | none |  |  |  |  |

---
## F063 - BetterStackHeartbeatService.NotifyFailedAsync

| Header | Value |
|---|---|
| Function Code | F063 |
| Function Name | BetterStackHeartbeatService.NotifyFailedAsync |
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
| Has FailUrl -> POST to fail URL. | Valid dependencies and data setup for this scenario | UTCID01 |
| No FailUrl -> fallback ping URL, errors still do not throw. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | uses FailUrl when set (`NotifyFailedAsync_WhenFailConfigured`) | none |  |  |  |  |
| UTCID02 | A | fallback ping / skip when missing (`WhenFailUrlMissing_ShouldFallbackToPingUrl` / only ping) | none |  |  |  |  |

---
## F064 - DashboardMetricsService.GetSystemAdminMetricsAsync

| Header | Value |
|---|---|
| Function Code | F064 |
| Function Name | DashboardMetricsService.GetSystemAdminMetricsAsync |
| Total Test Cases | 6 |
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
## F076 - EmailService.SendWelcomeEmailAsync

| Header | Value |
|---|---|
| Function Code | F076 |
| Function Name | EmailService.SendWelcomeEmailAsync |
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
| Correct subject/template for welcome email. | Valid dependencies and data setup for this scenario | UTCID01 |
| Calls `SendAsync` and logs info. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | welcome template + SendAsync (contract) | none |  |  |  |  |
| UTCID02 | N | info log on send (contract) | none |  |  |  |  |

---
## F077 - EmailService.SendAsync

| Header | Value |
|---|---|
| Function Code | F077 |
| Function Name | EmailService.SendAsync |
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
| `to` empty -> `ArgumentException`. | Valid dependencies and data setup for this scenario | UTCID01 |
| `subject` or `body` empty -> `ArgumentException`. | Valid dependencies and data setup for this scenario | UTCID02 |
| SMTP connect/auth/send/disconnect success -> debug log. | Valid dependencies and data setup for this scenario | UTCID03 |
| SMTP failure -> log error and rethrow. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | n/a | ArgumentException when to empty (`SendAsync_WithInvalidArguments` first rows) |  |  |  |  |
| UTCID02 | A | n/a | ArgumentException subject/body empty |  |  |  |  |
| UTCID03 | N | SMTP success -> completes (not asserted in unit suite) | none |  |  |  |  |
| UTCID04 | A | SMTP failure -> error log + throw (contract) | none |  |  |  |  |

---
## F078 - GoogleMeetService.CreateMeetingAsync

| Header | Value |
|---|---|
| Function Code | F078 |
| Function Name | GoogleMeetService.CreateMeetingAsync |
| Total Test Cases | 5 |
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
| Event created with meet link immediately -> return `MeetingInfo`. | Valid dependencies and data setup for this scenario | UTCID01 |
| No link on first try -> retry and obtain link. | Valid dependencies and data setup for this scenario | UTCID02 |
| Retries exhausted without link -> cleanup orphan event + throw. | Valid dependencies and data setup for this scenario | UTCID03 |
| Has attendeeEmails -> map attendees into event. | Valid dependencies and data setup for this scenario | UTCID04 |
| `durationMinutes` null -> use default duration setting. | Valid dependencies and data setup for this scenario | UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | MeetingInfo when Meet link available (contract; no integration test in suite) | none |  |  |  |  |
| UTCID02 | N | retry until link (contract) | none |  |  |  |  |
| UTCID03 | A | cleanup + throw when no link (contract) | none |  |  |  |  |
| UTCID04 | N | attendees mapped (contract) | none |  |  |  |  |
| UTCID05 | B | default duration when null (contract) | none |  |  |  |  |

---
## F079 - GoogleMeetService.DeleteMeetingAsync

| Header | Value |
|---|---|
| Function Code | F079 |
| Function Name | GoogleMeetService.DeleteMeetingAsync |
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
| Delete event succeeds. | Valid dependencies and data setup for this scenario | UTCID01 |
| API returns 404 not found -> warning, does not throw. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | delete succeeds (contract) | none |  |  |  |  |
| UTCID02 | A | 404 warning no throw (contract) | none |  |  |  |  |

---
## F080 - GoogleMeetService.Dispose

| Header | Value |
|---|---|
| Function Code | F080 |
| Function Name | GoogleMeetService.Dispose |
| Total Test Cases | 1 |
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
| Dispose -> calendar service disposed safely. | Valid dependencies and data setup for this scenario | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | no throw on multiple dispose (`Dispose_CalledMultipleTimes_ShouldNotThrow`) | none |  |  |  |  |

---
## F081 - NotificationService.SendAsync (typed)

| Header | Value |
|---|---|
| Function Code | F081 |
| Function Name | NotificationService.SendAsync (typed) |
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
| Payload object -> serialize camelCase JSON. | Valid dependencies and data setup for this scenario | UTCID01 |
| Persist notification + SaveChanges + broadcast notification DTO. | Valid dependencies and data setup for this scenario | UTCID02 |
| Unread count and broadcast correct. | Valid dependencies and data setup for this scenario | UTCID03 |
| Repository/hub error -> log error and rethrow. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | payload JSON camelCase (`SendAsync_Typed_ShouldPersistAndBroadcast`) | none |  |  |  |  |
| UTCID02 | N | notification persisted + saved | none |  |  |  |  |
| UTCID03 | N | unread count broadcast (`ShouldBroadcastUnreadCountIncludingExisting`) | none |  |  |  |  |
| UTCID04 | A | hub throws -> exception after persist (`WhenHubBroadcastFails`) | none |  |  |  |  |

---
## F082 - NotificationService.SendAsync (legacy)

| Header | Value |
|---|---|
| Function Code | F082 |
| Function Name | NotificationService.SendAsync (legacy) |
| Total Test Cases | 1 |
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
| Legacy overload forwards parameters to typed overload. | Valid dependencies and data setup for this scenario | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | forwards to typed overload with defaults (`SendAsync_Legacy`) | none |  |  |  |  |

---
## F083 - OrganisationOnboardingService.SubmitRequestAsync

| Header | Value |
|---|---|
| Function Code | F083 |
| Function Name | OrganisationOnboardingService.SubmitRequestAsync |
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
| Pending request with same email already exists -> Conflict. | Valid dependencies and data setup for this scenario | UTCID01 |
| New request created + save succeeds. | Valid dependencies and data setup for this scenario | UTCID02 |
| Notify admins after successful save. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.Conflict when pending exists (`SubmitRequestAsync_WhenPendingAlreadyExists`) | none |  |  |  |  |
| UTCID02 | N | Success persisted (`SubmitRequestAsync_ShouldPersistPendingRequest`) | none |  |  |  |  |
| UTCID03 | N | admin notification attempted (`...NotifyConfiguredAdminEmail` / distinct admins) | none |  |  |  |  |

---
## F084 - OrganisationOnboardingService.GetRequestsAsync

| Header | Value |
|---|---|
| Function Code | F084 |
| Function Name | OrganisationOnboardingService.GetRequestsAsync |
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
| Return list sorted `CreatedAt desc`. | Valid dependencies and data setup for this scenario | UTCID01 |
| Full DTO map (`OrgType`, `Status`, `ApprovedAt`...). | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | all requests returned (`GetRequestsAsync_ShouldReturnAllRequests`) | none |  |  |  |  |
| UTCID02 | N | DTO fields mapped (contract) | none |  |  |  |  |

---
## F085 - OrganisationOnboardingService.ApproveRequestAsync

| Header | Value |
|---|---|
| Function Code | F085 |
| Function Name | OrganisationOnboardingService.ApproveRequestAsync |
| Total Test Cases | 6 |
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
| RequestId not found -> NotFound. | Valid dependencies and data setup for this scenario | UTCID01 |
| Request no longer Pending -> Failure already processed. | Valid dependencies and data setup for this scenario | UTCID02 |
| Contact email already used by a user -> Conflict. | Valid dependencies and data setup for this scenario | UTCID03 |
| Create org admin + add role + create organisation + approve request -> Success. | Valid dependencies and data setup for this scenario | UTCID04 |
| Active contract template -> create contract and send for signature. | Valid dependencies and data setup for this scenario | UTCID05 |
| Any exception -> rollback transaction + Failure. | Valid dependencies and data setup for this scenario | UTCID06 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | NotFound when missing (`ApproveRequestAsync_WhenRequestNotFound`) | none |  |  |  |  |
| UTCID02 | A | Failure when not pending (`WhenRequestAlreadyProcessed`) | none |  |  |  |  |
| UTCID03 | A | Conflict when email exists (`WhenUserWithContactEmailExists`) | none |  |  |  |  |
| UTCID04 | N | Success creates org+user (`ApproveRequestAsync_ShouldCreateOrganisationAdmin`) | none |  |  |  |  |
| UTCID05 | N | contract optional (`WhenNoActiveTemplate`) | none |  |  |  |  |
| UTCID06 | A | Failure when email send fails (`WhenEmailSendingFails`) | none |  |  |  |  |

---
## F086 - PatientRoadmapGenerationService.GenerateFromDiagnosisAsync

| Header | Value |
|---|---|
| Function Code | F086 |
| Function Name | PatientRoadmapGenerationService.GenerateFromDiagnosisAsync |
| Total Test Cases | 6 |
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
| `PatientId` empty -> Failure validation. | Valid dependencies and data setup for this scenario | UTCID01 |
| `ScreeningId` empty or `AiScreeningRawJson` empty -> Failure validation. | Valid dependencies and data setup for this scenario | UTCID02 |
| ApiKey not configured -> Failure. | Valid dependencies and data setup for this scenario | UTCID03 |
| AI returns valid JSON -> parse + normalize + Success. | Valid dependencies and data setup for this scenario | UTCID04 |
| AI returns invalid JSON repeatedly -> exhaust retries and Failure. | Valid dependencies and data setup for this scenario | UTCID05 |
| HTTP timeout/network past max retries -> Failure unavailable. | Valid dependencies and data setup for this scenario | UTCID06 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A | Result.Failure; Errors contains "Patient ID is required for roadmap generation." | none |  |  |  |  |
| UTCID02 | A | Result.Failure; Errors contains "Screening ID is required..." OR "AI screening result is required..." | none |  |  |  |  |
| UTCID03 | A | Result.Failure; Errors contains "Google AI Studio API key is not configured." | none |  |  |  |  |
| UTCID04 | N | Result.Success; RiskLevel upper; deduped steps (`WithValidRiskLevels`) | none |  |  |  |  |
| UTCID05 | A | Result.Failure; Errors contains "AI returned an invalid roadmap format after retries." | none |  |  |  |  |
| UTCID06 | A | Result.Failure; Errors contains "Unable to generate patient roadmap from AI at this time." | none |  |  |  |  |

---
## F087 - PayOSService.CreatePaymentLinkAsync

| Header | Value |
|---|---|
| Function Code | F087 |
| Function Name | PayOSService.CreatePaymentLinkAsync |
| Total Test Cases | 5 |
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
| Payment link created, return `checkoutUrl` + `orderCode`. | Valid dependencies and data setup for this scenario | UTCID01 |
| `returnUrl/cancelUrl` empty -> default URLs from settings. | Valid dependencies and data setup for this scenario | UTCID02 |
| Description >25 chars -> truncated to limit correctly. | Valid dependencies and data setup for this scenario | UTCID03 |
| Output URL appends `orderCode` query param. | Valid dependencies and data setup for this scenario | UTCID04 |
| SDK returns null or empty URL -> throw. | Valid dependencies and data setup for this scenario | UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | checkoutUrl + orderCode (contract; no unit test) | none |  |  |  |  |
| UTCID02 | B | default return/cancel URLs (contract) | none |  |  |  |  |
| UTCID03 | N | description truncated (contract) | none |  |  |  |  |
| UTCID04 | N | orderCode query param appended (contract) | none |  |  |  |  |
| UTCID05 | A | n/a | throws when SDK returns empty URL (contract) |  |  |  |  |

---
## F088 - PayOSService.GetPaymentStatusAsync

| Header | Value |
|---|---|
| Function Code | F088 |
| Function Name | PayOSService.GetPaymentStatusAsync |
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
| Query succeeds -> map `Status/Amount/TxnRef`. | Valid dependencies and data setup for this scenario | UTCID01 |
| PaymentInfo null -> throw "Payment not found". | Valid dependencies and data setup for this scenario | UTCID02 |
| SDK throws -> wrap and throw service exception. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | status dto mapped (contract) | none |  |  |  |  |
| UTCID02 | A | n/a | throws "Payment not found" when null (contract) |  |  |  |  |
| UTCID03 | A | n/a | wrapped exception from SDK (contract) |  |  |  |  |

---
## F089 - PayOSService.VerifyWebhookSignatureAsync

| Header | Value |
|---|---|
| Function Code | F089 |
| Function Name | PayOSService.VerifyWebhookSignatureAsync |
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
| Current flow -> return true. | Valid dependencies and data setup for this scenario | UTCID01 |
| Exception branch -> return false + log error. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | `true` (`VerifyWebhookSignatureAsync_ShouldReturnTrueForCurrentImplementation`) | none |  |  |  |  |
| UTCID02 | A | `false` + error log on exception inside verifier (contract; not separately tested) | none |  |  |  |  |

---
## F090 - PayOSService.CancelPaymentAsync

| Header | Value |
|---|---|
| Function Code | F090 |
| Function Name | PayOSService.CancelPaymentAsync |
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
| SDK cancel returns object -> true. | Valid dependencies and data setup for this scenario | UTCID01 |
| SDK returns null -> false. | Valid dependencies and data setup for this scenario | UTCID02 |
| Exception -> false and log error. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | `true` when SDK returns object (contract) | none |  |  |  |  |
| UTCID02 | B | `false` when SDK returns null (contract) | none |  |  |  |  |
| UTCID03 | A | `false` on exception (contract) | none |  |  |  |  |

---
## F091 - SupabaseStorageService.SaveFileAsync

| Header | Value |
|---|---|
| Function Code | F091 |
| Function Name | SupabaseStorageService.SaveFileAsync |
| Total Test Cases | 5 |
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
| Filename sanitized + path built correctly per subfolder. | Valid dependencies and data setup for this scenario | UTCID01 |
| Empty stream -> throw `InvalidOperationException`. | Valid dependencies and data setup for this scenario | UTCID02 |
| Upload succeeds -> returns public URL. | Valid dependencies and data setup for this scenario | UTCID03 |
| SDK returns relative URL -> fallback to absolute URL. | Valid dependencies and data setup for this scenario | UTCID04 |
| ContentType mapped correctly by file extension. | Valid dependencies and data setup for this scenario | UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | sanitized path + upload (contract) | none |  |  |  |  |
| UTCID02 | A | n/a | InvalidOperationException empty stream (`SaveFileAsync_WithEmptyStream`) |  |  |  |  |
| UTCID03 | N | public URL string (contract) | none |  |  |  |  |
| UTCID04 | B | absolute URL fallback (contract) | none |  |  |  |  |
| UTCID05 | N | Content-Type by extension (contract) | none |  |  |  |  |

---
## F092 - SupabaseStorageService.DeleteFile

| Header | Value |
|---|---|
| Function Code | F092 |
| Function Name | SupabaseStorageService.DeleteFile |
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
| Full URL input -> extract path and remove succeeds -> true. | Valid dependencies and data setup for this scenario | UTCID01 |
| Valid relative path -> remove succeeds -> true. | Valid dependencies and data setup for this scenario | UTCID02 |
| Remove throws or invalid path -> false. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | true when remove succeeds (contract) | none |  |  |  |  |
| UTCID02 | N | true relative path (contract) | none |  |  |  |  |
| UTCID03 | A | false when throws / invalid (`DeleteFile_WithBlankPath_ShouldReturnFalse`) | none |  |  |  |  |

---
## F093 - SupabaseStorageService.FileExists

| Header | Value |
|---|---|
| Function Code | F093 |
| Function Name | SupabaseStorageService.FileExists |
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
| HEAD success -> true. | Valid dependencies and data setup for this scenario | UTCID01 |
| HEAD non-success -> false. | Valid dependencies and data setup for this scenario | UTCID02 |
| Empty input/request exception -> false. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | true HEAD success (contract) | none |  |  |  |  |
| UTCID02 | A | false non-success (contract) | none |  |  |  |  |
| UTCID03 | A | false blank path (`FileExists_WithBlankPath_ShouldReturnFalse`) | none |  |  |  |  |

---
## F094 - SystemSettingService.GetSettingAsync

| Header | Value |
|---|---|
| Function Code | F094 |
| Function Name | SystemSettingService.GetSettingAsync |
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
| Key exists -> returns correct value. | Valid dependencies and data setup for this scenario | UTCID01 |
| Key not found -> null. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | value when key exists (`GetSettingAsync_WhenKeyExists`) | none |  |  |  |  |
| UTCID02 | A | `null` when missing (`WhenKeyNotExists`) | none |  |  |  |  |

---
## F095 - SystemSettingService.GetAllSettingsAsync

| Header | Value |
|---|---|
| Function Code | F095 |
| Function Name | SystemSettingService.GetAllSettingsAsync |
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
| Has data -> dictionary with full key/value. | Valid dependencies and data setup for this scenario | UTCID01 |
| No data -> empty dictionary. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | N | dictionary all rows (`GetAllSettingsAsync_ShouldReturnDictionaryWithAllValues`) | none |  |  |  |  |
| UTCID02 | B | empty dict when no data (`WhenNoData_ShouldReturnEmptyDictionary`) | none |  |  |  |  |

---
## F096 - SystemSettingService.UpdateSettingsAsync

| Header | Value |
|---|---|
| Function Code | F096 |
| Function Name | SystemSettingService.UpdateSettingsAsync |
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
| Input null/empty -> no-op. | Valid dependencies and data setup for this scenario | UTCID01 |
| Key exists -> call `UpdateValue`. | Valid dependencies and data setup for this scenario | UTCID02 |
| New key -> add new `SystemSetting`. | Valid dependencies and data setup for this scenario | UTCID03 |
| After update -> `SaveChangesAsync` called + log info keys. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | B | no SaveChanges when null/empty (`WhenInputEmpty` / null) | none |  |  |  |  |
| UTCID02 | N | UpdateValue existing (`ShouldOverwriteExistingValue`) | none |  |  |  |  |
| UTCID03 | N | insert new key (`WithSingleNewKey`) | none |  |  |  |  |
| UTCID04 | N | SaveChanges + info log listing keys (`ShouldUpdateExistingAndInsertNew_AndWriteInfoLog`) | none |  |  |  |  |

---

