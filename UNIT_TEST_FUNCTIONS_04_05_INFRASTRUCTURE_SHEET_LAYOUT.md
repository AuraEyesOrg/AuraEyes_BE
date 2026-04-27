# Infrastructure Unit Test - Sheet Layout Ready

Last updated: 27/04/2026
Source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

## Hướng dẫn nhanh (tiếng Việt — để copy sang Excel)

**Lưu ý quan trọng:** File này **chỉ copy format markdown** từ bản V1 cũ. Danh sách function/case phải theo checklist Infrastructure hiện tại: **F001-F096, tổng 327 UTCID**. Không lấy lại function/case thừa của V1 cũ như `RegisterOphthalmologistAsync`, `RegisterOrganisationAsync`, `IsPhoneNumberInUseByOrganizationAsync`, ... nếu không còn nằm trong checklist.

**Hai bảng trong mỗi khối `## Fxxx` khác nhau thế nào?**

| Bảng                 | Mục đích                                 | Một dòng =                                               |
| -------------------- | ---------------------------------------- | -------------------------------------------------------- |
| **Condition Matrix** | Tóm tắt kịch bản theo checklist          | Có thể **nhiều UTCID** (cột `UTCIDs`: `UTCID01-UTCID03`) |
| **Result Matrix**    | Chi tiết **từng** case để trace / assert | **Đúng 1 UTCID** mỗi hàng                                |

**Quy tắc bắt buộc — không bịa**

- **Expected return / Expected exception / Expected log message** chỉ ghi khi **có chứng cứ**: assert hoặc `Verify` trong `tests/Infrastructure.UnitTests`, hoặc bạn đã **xác minh có chủ đích** (ghi nguồn: tên test / dòng assert).
- **Không** điền theo cảm tưởng hay “hợp lý theo code” nếu test không kiểm tra — để **trống** hoặc ghi rõ `n/a (chưa assert trong test)`.
- Cột **log**: chỉ ghi khi test thật sự assert log; không có thì **trống**.

**Làm theo 3 bước**

1. Mở đúng function: tìm `## F001` … `## F096` (cùng mã **F** với bảng catalog trong checklist).
2. Đọc **Condition Matrix** để nắm ý chung. Nội dung được sinh từ checklist hiện tại theo số lượng UTCID chuẩn.
3. Điền **Result Matrix**: mỗi hàng `UTCID01`, `UTCID02`, … — Expected để trống làm placeholder nếu chưa đối chiếu assert/test.

**Sau khi chạy test** mới điền: **Passed/Failed**, **Executed Date**, **Defect ID**.

**Excel của bạn (Confirm: 3 hàng Return / Exception / Log × nhiều cột UTCID)**  
Trong file markdown là **3 cột** trên **một hàng** (một UTCID). Nội dung giống nhau, chỉ khác xoay bảng: copy từ markdown sang Excel rồi **Transpose** (dán chuyển vị), hoặc điền tay theo cùng một ý.

**Copy markdown → bảng Excel:** dùng công cụ chuyển markdown table sang grid (ví dụ tableconvert) rồi dán vào sheet, hoặc copy trực tiếp từng bảng.

---

## How to copy into Sheet / Excel (English)

- This file keeps the **V1 markdown format only**. Function list and UTCID counts come from the current Infrastructure checklist: **F001-F096, 327 total UTCIDs**.
- **Do not invent.** Put something in **Expected return / exception / log** only when **evidence exists**: an assert or `Verify` in `tests/Infrastructure.UnitTests`, or you explicitly verified and cite the test name. Otherwise leave **empty** or write `n/a (not asserted in test)`. **Log** only if the test asserts logging.
- Each block `## F001` … `## F096` is **one function**; copy the whole block into one sheet or one table range.
- In the Header table **Value** column, fill in **Created By**, **Executed By**, **Lines of Code**, **Passed / Failed / Untested**, **Count type N / A / B** after you run tests.
- **Condition Matrix** = high-level summary; one row may list **several** UTCIDs in the third column.
- **Result Matrix** = **one row per UTCID**; Expected columns are placeholders until verified from tests/asserts.
- **Passed/Failed**, **Executed Date**, **Defect ID** — after execution only.
- Excel **Confirm** uses **rows** (Return / Exception / Log) × UTCID **columns**; markdown uses **columns** on one row per UTCID — same content, transpose when pasting.

---

## F001 - AuthService.RegisterPatientAsync

| Header           | Value                                                                                                                    |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F001                                                                                                                     |
| Function Name    | AuthService.RegisterPatientAsync                                                                                         |
| Total Test Cases | 10                                                                                                                       |
| Created By       |                                                                                                                          |
| Executed By      |                                                                                                                          |
| Lines of Code    |                                                                                                                          |
| Passed           |                                                                                                                          |
| Failed           |                                                                                                                          |
| Untested         |                                                                                                                          |
| Count type N     |                                                                                                                          |
| Count type A     |                                                                                                                          |
| Count type B     |                                                                                                                          |
| Test Requirement | Validate 'Register patient account' in AuthService.RegisterPatientAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                      | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AuthService.RegisterPatientAsync; split into 10 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID10 |

### Result Matrix

| UTCID | N | A | B | Expected return | Expected exception | Expected log message | Passed/Failed |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |
| UTCID06 | A |  |  |  |  |  |  |
| UTCID07 | A |  |  |  |  |  |  |
| UTCID08 | A |  |  |  |  |  |  |
| UTCID09 | A |  |  |  |  |  |  |
| UTCID10 | A |  |  |  |  |  |  |

---

## F002 - AuthService.LookupAccountByCitizenIdAsync

| Header           | Value                                                                                                                                 |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F002                                                                                                                                  |
| Function Name    | AuthService.LookupAccountByCitizenIdAsync                                                                                             |
| Total Test Cases | 3                                                                                                                                     |
| Created By       |                                                                                                                                       |
| Executed By      |                                                                                                                                       |
| Lines of Code    |                                                                                                                                       |
| Passed           |                                                                                                                                       |
| Failed           |                                                                                                                                       |
| Untested         |                                                                                                                                       |
| Count type N     |                                                                                                                                       |
| Count type A     |                                                                                                                                       |
| Count type B     |                                                                                                                                       |
| Test Requirement | Validate 'Lookup account by citizen id' in AuthService.LookupAccountByCitizenIdAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                              | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AuthService.LookupAccountByCitizenIdAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F003 - AuthService.GoogleLoginAsync

| Header           | Value                                                                                                    |
| ---------------- | -------------------------------------------------------------------------------------------------------- |
| Function Code    | F003                                                                                                     |
| Function Name    | AuthService.GoogleLoginAsync                                                                             |
| Total Test Cases | 10                                                                                                       |
| Created By       |                                                                                                          |
| Executed By      |                                                                                                          |
| Lines of Code    |                                                                                                          |
| Passed           |                                                                                                          |
| Failed           |                                                                                                          |
| Untested         |                                                                                                          |
| Count type N     |                                                                                                          |
| Count type A     |                                                                                                          |
| Count type B     |                                                                                                          |
| Test Requirement | Validate 'Google login' in AuthService.GoogleLoginAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                  | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AuthService.GoogleLoginAsync; split into 10 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |
| UTCID06 | A |  |  |  |  |  |  |
| UTCID07 | A |  |  |  |  |  |  |
| UTCID08 | A |  |  |  |  |  |  |
| UTCID09 | A |  |  |  |  |  |  |
| UTCID10 | A |  |  |  |  |  |  |

---

## F004 - AuthService.LoginAsync

| Header           | Value                                                                                                     |
| ---------------- | --------------------------------------------------------------------------------------------------------- |
| Function Code    | F004                                                                                                      |
| Function Name    | AuthService.LoginAsync                                                                                    |
| Total Test Cases | 12                                                                                                        |
| Created By       |                                                                                                           |
| Executed By      |                                                                                                           |
| Lines of Code    |                                                                                                           |
| Passed           |                                                                                                           |
| Failed           |                                                                                                           |
| Untested         |                                                                                                           |
| Count type N     |                                                                                                           |
| Count type A     |                                                                                                           |
| Count type B     |                                                                                                           |
| Test Requirement | Validate 'Login with password' in AuthService.LoginAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                            | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AuthService.LoginAsync; split into 12 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID12 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |
| UTCID06 | A |  |  |  |  |  |  |
| UTCID07 | A |  |  |  |  |  |  |
| UTCID08 | A |  |  |  |  |  |  |
| UTCID09 | A |  |  |  |  |  |  |
| UTCID10 | A |  |  |  |  |  |  |
| UTCID11 | A |  |  |  |  |  |  |
| UTCID12 | A |  |  |  |  |  |  |

---

## F005 - AuthService.VerifyTwoFactorLoginAsync

| Header           | Value                                                                                                                 |
| ---------------- | --------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F005                                                                                                                  |
| Function Name    | AuthService.VerifyTwoFactorLoginAsync                                                                                 |
| Total Test Cases | 10                                                                                                                    |
| Created By       |                                                                                                                       |
| Executed By      |                                                                                                                       |
| Lines of Code    |                                                                                                                       |
| Passed           |                                                                                                                       |
| Failed           |                                                                                                                       |
| Untested         |                                                                                                                       |
| Count type N     |                                                                                                                       |
| Count type A     |                                                                                                                       |
| Count type B     |                                                                                                                       |
| Test Requirement | Validate 'Verify 2FA login' in AuthService.VerifyTwoFactorLoginAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                           | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AuthService.VerifyTwoFactorLoginAsync; split into 10 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |
| UTCID06 | A |  |  |  |  |  |  |
| UTCID07 | A |  |  |  |  |  |  |
| UTCID08 | A |  |  |  |  |  |  |
| UTCID09 | A |  |  |  |  |  |  |
| UTCID10 | A |  |  |  |  |  |  |

---

## F006 - AuthService.RefreshTokenAsync

| Header           | Value                                                                                                           |
| ---------------- | --------------------------------------------------------------------------------------------------------------- |
| Function Code    | F006                                                                                                            |
| Function Name    | AuthService.RefreshTokenAsync                                                                                   |
| Total Test Cases | 10                                                                                                              |
| Created By       |                                                                                                                 |
| Executed By      |                                                                                                                 |
| Lines of Code    |                                                                                                                 |
| Passed           |                                                                                                                 |
| Failed           |                                                                                                                 |
| Untested         |                                                                                                                 |
| Count type N     |                                                                                                                 |
| Count type A     |                                                                                                                 |
| Count type B     |                                                                                                                 |
| Test Requirement | Validate 'Refresh token flow' in AuthService.RefreshTokenAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                   | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AuthService.RefreshTokenAsync; split into 10 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |
| UTCID06 | A |  |  |  |  |  |  |
| UTCID07 | A |  |  |  |  |  |  |
| UTCID08 | A |  |  |  |  |  |  |
| UTCID09 | A |  |  |  |  |  |  |
| UTCID10 | A |  |  |  |  |  |  |

---

## F007 - AuthService.LogoutAsync

| Header           | Value                                                                                                       |
| ---------------- | ----------------------------------------------------------------------------------------------------------- |
| Function Code    | F007                                                                                                        |
| Function Name    | AuthService.LogoutAsync                                                                                     |
| Total Test Cases | 3                                                                                                           |
| Created By       |                                                                                                             |
| Executed By      |                                                                                                             |
| Lines of Code    |                                                                                                             |
| Passed           |                                                                                                             |
| Failed           |                                                                                                             |
| Untested         |                                                                                                             |
| Count type N     |                                                                                                             |
| Count type A     |                                                                                                             |
| Count type B     |                                                                                                             |
| Test Requirement | Validate 'Logout single device' in AuthService.LogoutAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                            | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AuthService.LogoutAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F008 - AuthService.LogoutAllAsync

| Header           | Value                                                                                                        |
| ---------------- | ------------------------------------------------------------------------------------------------------------ |
| Function Code    | F008                                                                                                         |
| Function Name    | AuthService.LogoutAllAsync                                                                                   |
| Total Test Cases | 2                                                                                                            |
| Created By       |                                                                                                              |
| Executed By      |                                                                                                              |
| Lines of Code    |                                                                                                              |
| Passed           |                                                                                                              |
| Failed           |                                                                                                              |
| Untested         |                                                                                                              |
| Count type N     |                                                                                                              |
| Count type A     |                                                                                                              |
| Count type B     |                                                                                                              |
| Test Requirement | Validate 'Logout all devices' in AuthService.LogoutAllAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                               | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AuthService.LogoutAllAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F009 - AuthService.ConfirmEmailAsync

| Header           | Value                                                                                                      |
| ---------------- | ---------------------------------------------------------------------------------------------------------- |
| Function Code    | F009                                                                                                       |
| Function Name    | AuthService.ConfirmEmailAsync                                                                              |
| Total Test Cases | 6                                                                                                          |
| Created By       |                                                                                                            |
| Executed By      |                                                                                                            |
| Lines of Code    |                                                                                                            |
| Passed           |                                                                                                            |
| Failed           |                                                                                                            |
| Untested         |                                                                                                            |
| Count type N     |                                                                                                            |
| Count type A     |                                                                                                            |
| Count type B     |                                                                                                            |
| Test Requirement | Validate 'Confirm email' in AuthService.ConfirmEmailAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                  | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AuthService.ConfirmEmailAsync; split into 6 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID06 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |
| UTCID06 | A |  |  |  |  |  |  |

---

## F010 - AuthService.ForgotPasswordAsync

| Header           | Value                                                                                                          |
| ---------------- | -------------------------------------------------------------------------------------------------------------- |
| Function Code    | F010                                                                                                           |
| Function Name    | AuthService.ForgotPasswordAsync                                                                                |
| Total Test Cases | 3                                                                                                              |
| Created By       |                                                                                                                |
| Executed By      |                                                                                                                |
| Lines of Code    |                                                                                                                |
| Passed           |                                                                                                                |
| Failed           |                                                                                                                |
| Untested         |                                                                                                                |
| Count type N     |                                                                                                                |
| Count type A     |                                                                                                                |
| Count type B     |                                                                                                                |
| Test Requirement | Validate 'Forgot password' in AuthService.ForgotPasswordAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                    | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AuthService.ForgotPasswordAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F011 - AuthService.ResetPasswordAsync

| Header           | Value                                                                                                        |
| ---------------- | ------------------------------------------------------------------------------------------------------------ |
| Function Code    | F011                                                                                                         |
| Function Name    | AuthService.ResetPasswordAsync                                                                               |
| Total Test Cases | 5                                                                                                            |
| Created By       |                                                                                                              |
| Executed By      |                                                                                                              |
| Lines of Code    |                                                                                                              |
| Passed           |                                                                                                              |
| Failed           |                                                                                                              |
| Untested         |                                                                                                              |
| Count type N     |                                                                                                              |
| Count type A     |                                                                                                              |
| Count type B     |                                                                                                              |
| Test Requirement | Validate 'Reset password' in AuthService.ResetPasswordAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                   | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AuthService.ResetPasswordAsync; split into 5 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |

---

## F012 - AuthService.GetCurrentUserAsync

| Header           | Value                                                                                                           |
| ---------------- | --------------------------------------------------------------------------------------------------------------- |
| Function Code    | F012                                                                                                            |
| Function Name    | AuthService.GetCurrentUserAsync                                                                                 |
| Total Test Cases | 5                                                                                                               |
| Created By       |                                                                                                                 |
| Executed By      |                                                                                                                 |
| Lines of Code    |                                                                                                                 |
| Passed           |                                                                                                                 |
| Failed           |                                                                                                                 |
| Untested         |                                                                                                                 |
| Count type N     |                                                                                                                 |
| Count type A     |                                                                                                                 |
| Count type B     |                                                                                                                 |
| Test Requirement | Validate 'Get current user' in AuthService.GetCurrentUserAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                    | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AuthService.GetCurrentUserAsync; split into 5 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |

---

## F013 - AuthService.ResendConfirmationAsync

| Header           | Value                                                                                                                        |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F013                                                                                                                         |
| Function Name    | AuthService.ResendConfirmationAsync                                                                                          |
| Total Test Cases | 4                                                                                                                            |
| Created By       |                                                                                                                              |
| Executed By      |                                                                                                                              |
| Lines of Code    |                                                                                                                              |
| Passed           |                                                                                                                              |
| Failed           |                                                                                                                              |
| Untested         |                                                                                                                              |
| Count type N     |                                                                                                                              |
| Count type A     |                                                                                                                              |
| Count type B     |                                                                                                                              |
| Test Requirement | Validate 'Resend confirmation email' in AuthService.ResendConfirmationAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                        | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AuthService.ResendConfirmationAsync; split into 4 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |

---

## F014 - IdentityService.CheckPasswordAsync

| Header           | Value                                                                                                            |
| ---------------- | ---------------------------------------------------------------------------------------------------------------- |
| Function Code    | F014                                                                                                             |
| Function Name    | IdentityService.CheckPasswordAsync                                                                               |
| Total Test Cases | 3                                                                                                                |
| Created By       |                                                                                                                  |
| Executed By      |                                                                                                                  |
| Lines of Code    |                                                                                                                  |
| Passed           |                                                                                                                  |
| Failed           |                                                                                                                  |
| Untested         |                                                                                                                  |
| Count type N     |                                                                                                                  |
| Count type A     |                                                                                                                  |
| Count type B     |                                                                                                                  |
| Test Requirement | Validate 'Check password' in IdentityService.CheckPasswordAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                       | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.CheckPasswordAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F015 - IdentityService.GetUserByEmailAsync

| Header           | Value                                                                                                                |
| ---------------- | -------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F015                                                                                                                 |
| Function Name    | IdentityService.GetUserByEmailAsync                                                                                  |
| Total Test Cases | 3                                                                                                                    |
| Created By       |                                                                                                                      |
| Executed By      |                                                                                                                      |
| Lines of Code    |                                                                                                                      |
| Passed           |                                                                                                                      |
| Failed           |                                                                                                                      |
| Untested         |                                                                                                                      |
| Count type N     |                                                                                                                      |
| Count type A     |                                                                                                                      |
| Count type B     |                                                                                                                      |
| Test Requirement | Validate 'Get user by email' in IdentityService.GetUserByEmailAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                        | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.GetUserByEmailAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F016 - IdentityService.GetUserByIdAsync

| Header           | Value                                                                                                          |
| ---------------- | -------------------------------------------------------------------------------------------------------------- |
| Function Code    | F016                                                                                                           |
| Function Name    | IdentityService.GetUserByIdAsync                                                                               |
| Total Test Cases | 3                                                                                                              |
| Created By       |                                                                                                                |
| Executed By      |                                                                                                                |
| Lines of Code    |                                                                                                                |
| Passed           |                                                                                                                |
| Failed           |                                                                                                                |
| Untested         |                                                                                                                |
| Count type N     |                                                                                                                |
| Count type A     |                                                                                                                |
| Count type B     |                                                                                                                |
| Test Requirement | Validate 'Get user by id' in IdentityService.GetUserByIdAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                     | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.GetUserByIdAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F017 - IdentityService.IsEmailConfirmedAsync

| Header           | Value                                                                                                                      |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F017                                                                                                                       |
| Function Name    | IdentityService.IsEmailConfirmedAsync                                                                                      |
| Total Test Cases | 3                                                                                                                          |
| Created By       |                                                                                                                            |
| Executed By      |                                                                                                                            |
| Lines of Code    |                                                                                                                            |
| Passed           |                                                                                                                            |
| Failed           |                                                                                                                            |
| Untested         |                                                                                                                            |
| Count type N     |                                                                                                                            |
| Count type A     |                                                                                                                            |
| Count type B     |                                                                                                                            |
| Test Requirement | Validate 'Check email confirmed' in IdentityService.IsEmailConfirmedAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                          | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.IsEmailConfirmedAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F018 - IdentityService.IsUserActiveAsync

| Header           | Value                                                                                                              |
| ---------------- | ------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F018                                                                                                               |
| Function Name    | IdentityService.IsUserActiveAsync                                                                                  |
| Total Test Cases | 3                                                                                                                  |
| Created By       |                                                                                                                    |
| Executed By      |                                                                                                                    |
| Lines of Code    |                                                                                                                    |
| Passed           |                                                                                                                    |
| Failed           |                                                                                                                    |
| Untested         |                                                                                                                    |
| Count type N     |                                                                                                                    |
| Count type A     |                                                                                                                    |
| Count type B     |                                                                                                                    |
| Test Requirement | Validate 'Check user active' in IdentityService.IsUserActiveAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                      | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.IsUserActiveAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F019 - IdentityService.GenerateEmailConfirmationTokenAsync

| Header           | Value                                                                                                                                                |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F019                                                                                                                                                 |
| Function Name    | IdentityService.GenerateEmailConfirmationTokenAsync                                                                                                  |
| Total Test Cases | 2                                                                                                                                                    |
| Created By       |                                                                                                                                                      |
| Executed By      |                                                                                                                                                      |
| Lines of Code    |                                                                                                                                                      |
| Passed           |                                                                                                                                                      |
| Failed           |                                                                                                                                                      |
| Untested         |                                                                                                                                                      |
| Count type N     |                                                                                                                                                      |
| Count type A     |                                                                                                                                                      |
| Count type B     |                                                                                                                                                      |
| Test Requirement | Validate 'Generate email confirmation token' in IdentityService.GenerateEmailConfirmationTokenAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                        | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.GenerateEmailConfirmationTokenAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F020 - IdentityService.GeneratePasswordResetTokenAsync

| Header           | Value                                                                                                                                        |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F020                                                                                                                                         |
| Function Name    | IdentityService.GeneratePasswordResetTokenAsync                                                                                              |
| Total Test Cases | 2                                                                                                                                            |
| Created By       |                                                                                                                                              |
| Executed By      |                                                                                                                                              |
| Lines of Code    |                                                                                                                                              |
| Passed           |                                                                                                                                              |
| Failed           |                                                                                                                                              |
| Untested         |                                                                                                                                              |
| Count type N     |                                                                                                                                              |
| Count type A     |                                                                                                                                              |
| Count type B     |                                                                                                                                              |
| Test Requirement | Validate 'Generate password reset token' in IdentityService.GeneratePasswordResetTokenAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                    | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.GeneratePasswordResetTokenAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F021 - IdentityService.GetUserRolesAsync

| Header           | Value                                                                                                           |
| ---------------- | --------------------------------------------------------------------------------------------------------------- |
| Function Code    | F021                                                                                                            |
| Function Name    | IdentityService.GetUserRolesAsync                                                                               |
| Total Test Cases | 3                                                                                                               |
| Created By       |                                                                                                                 |
| Executed By      |                                                                                                                 |
| Lines of Code    |                                                                                                                 |
| Passed           |                                                                                                                 |
| Failed           |                                                                                                                 |
| Untested         |                                                                                                                 |
| Count type N     |                                                                                                                 |
| Count type A     |                                                                                                                 |
| Count type B     |                                                                                                                 |
| Test Requirement | Validate 'Get user roles' in IdentityService.GetUserRolesAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                      | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.GetUserRolesAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F022 - IdentityService.IsInRoleAsync

| Header           | Value                                                                                                           |
| ---------------- | --------------------------------------------------------------------------------------------------------------- |
| Function Code    | F022                                                                                                            |
| Function Name    | IdentityService.IsInRoleAsync                                                                                   |
| Total Test Cases | 3                                                                                                               |
| Created By       |                                                                                                                 |
| Executed By      |                                                                                                                 |
| Lines of Code    |                                                                                                                 |
| Passed           |                                                                                                                 |
| Failed           |                                                                                                                 |
| Untested         |                                                                                                                 |
| Count type N     |                                                                                                                 |
| Count type A     |                                                                                                                 |
| Count type B     |                                                                                                                 |
| Test Requirement | Validate 'Check user in role' in IdentityService.IsInRoleAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                  | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.IsInRoleAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F023 - IdentityService.UpdateLastLoginAsync

| Header           | Value                                                                                                                       |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F023                                                                                                                        |
| Function Name    | IdentityService.UpdateLastLoginAsync                                                                                        |
| Total Test Cases | 2                                                                                                                           |
| Created By       |                                                                                                                             |
| Executed By      |                                                                                                                             |
| Lines of Code    |                                                                                                                             |
| Passed           |                                                                                                                             |
| Failed           |                                                                                                                             |
| Untested         |                                                                                                                             |
| Count type N     |                                                                                                                             |
| Count type A     |                                                                                                                             |
| Count type B     |                                                                                                                             |
| Test Requirement | Validate 'Update last login async' in IdentityService.UpdateLastLoginAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                         | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.UpdateLastLoginAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F024 - IdentityService.IsTwoFactorEnabledAsync

| Header           | Value                                                                                                                    |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F024                                                                                                                     |
| Function Name    | IdentityService.IsTwoFactorEnabledAsync                                                                                  |
| Total Test Cases | 3                                                                                                                        |
| Created By       |                                                                                                                          |
| Executed By      |                                                                                                                          |
| Lines of Code    |                                                                                                                          |
| Passed           |                                                                                                                          |
| Failed           |                                                                                                                          |
| Untested         |                                                                                                                          |
| Count type N     |                                                                                                                          |
| Count type A     |                                                                                                                          |
| Count type B     |                                                                                                                          |
| Test Requirement | Validate 'Check 2FA enabled' in IdentityService.IsTwoFactorEnabledAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                            | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.IsTwoFactorEnabledAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F025 - IdentityService.GetAuthenticatorKeyAsync

| Header           | Value                                                                                                                         |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F025                                                                                                                          |
| Function Name    | IdentityService.GetAuthenticatorKeyAsync                                                                                      |
| Total Test Cases | 2                                                                                                                             |
| Created By       |                                                                                                                               |
| Executed By      |                                                                                                                               |
| Lines of Code    |                                                                                                                               |
| Passed           |                                                                                                                               |
| Failed           |                                                                                                                               |
| Untested         |                                                                                                                               |
| Count type N     |                                                                                                                               |
| Count type A     |                                                                                                                               |
| Count type B     |                                                                                                                               |
| Test Requirement | Validate 'Get authenticator key' in IdentityService.GetAuthenticatorKeyAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                             | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.GetAuthenticatorKeyAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F026 - IdentityService.GetOrCreateAuthenticatorKeyAsync

| Header           | Value                                                                                                                                           |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F026                                                                                                                                            |
| Function Name    | IdentityService.GetOrCreateAuthenticatorKeyAsync                                                                                                |
| Total Test Cases | 2                                                                                                                                               |
| Created By       |                                                                                                                                                 |
| Executed By      |                                                                                                                                                 |
| Lines of Code    |                                                                                                                                                 |
| Passed           |                                                                                                                                                 |
| Failed           |                                                                                                                                                 |
| Untested         |                                                                                                                                                 |
| Count type N     |                                                                                                                                                 |
| Count type A     |                                                                                                                                                 |
| Count type B     |                                                                                                                                                 |
| Test Requirement | Validate 'Get or create authenticator key' in IdentityService.GetOrCreateAuthenticatorKeyAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                     | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.GetOrCreateAuthenticatorKeyAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F027 - IdentityService.VerifyTwoFactorCodeAsync

| Header           | Value                                                                                                                   |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F027                                                                                                                    |
| Function Name    | IdentityService.VerifyTwoFactorCodeAsync                                                                                |
| Total Test Cases | 3                                                                                                                       |
| Created By       |                                                                                                                         |
| Executed By      |                                                                                                                         |
| Lines of Code    |                                                                                                                         |
| Passed           |                                                                                                                         |
| Failed           |                                                                                                                         |
| Untested         |                                                                                                                         |
| Count type N     |                                                                                                                         |
| Count type A     |                                                                                                                         |
| Count type B     |                                                                                                                         |
| Test Requirement | Validate 'Verify 2FA code' in IdentityService.VerifyTwoFactorCodeAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                             | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.VerifyTwoFactorCodeAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F028 - IdentityService.GenerateNewRecoveryCodesAsync

| Header           | Value                                                                                                                                |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F028                                                                                                                                 |
| Function Name    | IdentityService.GenerateNewRecoveryCodesAsync                                                                                        |
| Total Test Cases | 2                                                                                                                                    |
| Created By       |                                                                                                                                      |
| Executed By      |                                                                                                                                      |
| Lines of Code    |                                                                                                                                      |
| Passed           |                                                                                                                                      |
| Failed           |                                                                                                                                      |
| Untested         |                                                                                                                                      |
| Count type N     |                                                                                                                                      |
| Count type A     |                                                                                                                                      |
| Count type B     |                                                                                                                                      |
| Test Requirement | Validate 'Generate recovery codes' in IdentityService.GenerateNewRecoveryCodesAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                  | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.GenerateNewRecoveryCodesAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F029 - IdentityService.GetRecoveryCodesCountAsync

| Header           | Value                                                                                                                             |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F029                                                                                                                              |
| Function Name    | IdentityService.GetRecoveryCodesCountAsync                                                                                        |
| Total Test Cases | 2                                                                                                                                 |
| Created By       |                                                                                                                                   |
| Executed By      |                                                                                                                                   |
| Lines of Code    |                                                                                                                                   |
| Passed           |                                                                                                                                   |
| Failed           |                                                                                                                                   |
| Untested         |                                                                                                                                   |
| Count type N     |                                                                                                                                   |
| Count type A     |                                                                                                                                   |
| Count type B     |                                                                                                                                   |
| Test Requirement | Validate 'Get recovery code count' in IdentityService.GetRecoveryCodesCountAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                               | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.GetRecoveryCodesCountAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F030 - IdentityService.GenerateAuthenticatorUri

| Header           | Value                                                                                                                              |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F030                                                                                                                               |
| Function Name    | IdentityService.GenerateAuthenticatorUri                                                                                           |
| Total Test Cases | 2                                                                                                                                  |
| Created By       |                                                                                                                                    |
| Executed By      |                                                                                                                                    |
| Lines of Code    |                                                                                                                                    |
| Passed           |                                                                                                                                    |
| Failed           |                                                                                                                                    |
| Untested         |                                                                                                                                    |
| Count type N     |                                                                                                                                    |
| Count type A     |                                                                                                                                    |
| Count type B     |                                                                                                                                    |
| Test Requirement | Validate 'Generate authenticator URI' in IdentityService.GenerateAuthenticatorUri, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                             | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.GenerateAuthenticatorUri; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F031 - IdentityService.FormatAuthenticatorKey

| Header           | Value                                                                                                                          |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F031                                                                                                                           |
| Function Name    | IdentityService.FormatAuthenticatorKey                                                                                         |
| Total Test Cases | 2                                                                                                                              |
| Created By       |                                                                                                                                |
| Executed By      |                                                                                                                                |
| Lines of Code    |                                                                                                                                |
| Passed           |                                                                                                                                |
| Failed           |                                                                                                                                |
| Untested         |                                                                                                                                |
| Count type N     |                                                                                                                                |
| Count type A     |                                                                                                                                |
| Count type B     |                                                                                                                                |
| Test Requirement | Validate 'Format authenticator key' in IdentityService.FormatAuthenticatorKey, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                           | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.FormatAuthenticatorKey; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F032 - IdentityService.GetUserMetricsAsync

| Header           | Value                                                                                                               |
| ---------------- | ------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F032                                                                                                                |
| Function Name    | IdentityService.GetUserMetricsAsync                                                                                 |
| Total Test Cases | 3                                                                                                                   |
| Created By       |                                                                                                                     |
| Executed By      |                                                                                                                     |
| Lines of Code    |                                                                                                                     |
| Passed           |                                                                                                                     |
| Failed           |                                                                                                                     |
| Untested         |                                                                                                                     |
| Count type N     |                                                                                                                     |
| Count type A     |                                                                                                                     |
| Count type B     |                                                                                                                     |
| Test Requirement | Validate 'Get user metrics' in IdentityService.GetUserMetricsAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                        | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.GetUserMetricsAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F033 - IdentityService.GetUsersInRoleCountAsync

| Header           | Value                                                                                                                           |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F033                                                                                                                            |
| Function Name    | IdentityService.GetUsersInRoleCountAsync                                                                                        |
| Total Test Cases | 3                                                                                                                               |
| Created By       |                                                                                                                                 |
| Executed By      |                                                                                                                                 |
| Lines of Code    |                                                                                                                                 |
| Passed           |                                                                                                                                 |
| Failed           |                                                                                                                                 |
| Untested         |                                                                                                                                 |
| Count type N     |                                                                                                                                 |
| Count type A     |                                                                                                                                 |
| Count type B     |                                                                                                                                 |
| Test Requirement | Validate 'Get users in role count' in IdentityService.GetUsersInRoleCountAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                             | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.GetUsersInRoleCountAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F034 - IdentityService.GetPendingApprovalsCountAsync

| Header           | Value                                                                                                                                    |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F034                                                                                                                                     |
| Function Name    | IdentityService.GetPendingApprovalsCountAsync                                                                                            |
| Total Test Cases | 2                                                                                                                                        |
| Created By       |                                                                                                                                          |
| Executed By      |                                                                                                                                          |
| Lines of Code    |                                                                                                                                          |
| Passed           |                                                                                                                                          |
| Failed           |                                                                                                                                          |
| Untested         |                                                                                                                                          |
| Count type N     |                                                                                                                                          |
| Count type A     |                                                                                                                                          |
| Count type B     |                                                                                                                                          |
| Test Requirement | Validate 'Get pending approvals count' in IdentityService.GetPendingApprovalsCountAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                  | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.GetPendingApprovalsCountAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F035 - IdentityService.GetUserDetailsAsync

| Header           | Value                                                                                                               |
| ---------------- | ------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F035                                                                                                                |
| Function Name    | IdentityService.GetUserDetailsAsync                                                                                 |
| Total Test Cases | 3                                                                                                                   |
| Created By       |                                                                                                                     |
| Executed By      |                                                                                                                     |
| Lines of Code    |                                                                                                                     |
| Passed           |                                                                                                                     |
| Failed           |                                                                                                                     |
| Untested         |                                                                                                                     |
| Count type N     |                                                                                                                     |
| Count type A     |                                                                                                                     |
| Count type B     |                                                                                                                     |
| Test Requirement | Validate 'Get user details' in IdentityService.GetUserDetailsAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                        | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for IdentityService.GetUserDetailsAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F036 - RefreshTokenService.CreateRefreshTokenAsync

| Header           | Value                                                                                                                                  |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F036                                                                                                                                   |
| Function Name    | RefreshTokenService.CreateRefreshTokenAsync                                                                                            |
| Total Test Cases | 3                                                                                                                                      |
| Created By       |                                                                                                                                        |
| Executed By      |                                                                                                                                        |
| Lines of Code    |                                                                                                                                        |
| Passed           |                                                                                                                                        |
| Failed           |                                                                                                                                        |
| Untested         |                                                                                                                                        |
| Count type N     |                                                                                                                                        |
| Count type A     |                                                                                                                                        |
| Count type B     |                                                                                                                                        |
| Test Requirement | Validate 'Create refresh token record' in RefreshTokenService.CreateRefreshTokenAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for RefreshTokenService.CreateRefreshTokenAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F037 - RefreshTokenService.GetByTokenHashAsync

| Header           | Value                                                                                                                            |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F037                                                                                                                             |
| Function Name    | RefreshTokenService.GetByTokenHashAsync                                                                                          |
| Total Test Cases | 3                                                                                                                                |
| Created By       |                                                                                                                                  |
| Executed By      |                                                                                                                                  |
| Lines of Code    |                                                                                                                                  |
| Passed           |                                                                                                                                  |
| Failed           |                                                                                                                                  |
| Untested         |                                                                                                                                  |
| Count type N     |                                                                                                                                  |
| Count type A     |                                                                                                                                  |
| Count type B     |                                                                                                                                  |
| Test Requirement | Validate 'Get refresh token by hash' in RefreshTokenService.GetByTokenHashAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                            | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for RefreshTokenService.GetByTokenHashAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F038 - RefreshTokenService.RotateRefreshTokenAsync

| Header           | Value                                                                                                                           |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F038                                                                                                                            |
| Function Name    | RefreshTokenService.RotateRefreshTokenAsync                                                                                     |
| Total Test Cases | 3                                                                                                                               |
| Created By       |                                                                                                                                 |
| Executed By      |                                                                                                                                 |
| Lines of Code    |                                                                                                                                 |
| Passed           |                                                                                                                                 |
| Failed           |                                                                                                                                 |
| Untested         |                                                                                                                                 |
| Count type N     |                                                                                                                                 |
| Count type A     |                                                                                                                                 |
| Count type B     |                                                                                                                                 |
| Test Requirement | Validate 'Rotate refresh token' in RefreshTokenService.RotateRefreshTokenAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for RefreshTokenService.RotateRefreshTokenAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F039 - RefreshTokenService.RevokeTokenAsync

| Header           | Value                                                                                                                    |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F039                                                                                                                     |
| Function Name    | RefreshTokenService.RevokeTokenAsync                                                                                     |
| Total Test Cases | 2                                                                                                                        |
| Created By       |                                                                                                                          |
| Executed By      |                                                                                                                          |
| Lines of Code    |                                                                                                                          |
| Passed           |                                                                                                                          |
| Failed           |                                                                                                                          |
| Untested         |                                                                                                                          |
| Count type N     |                                                                                                                          |
| Count type A     |                                                                                                                          |
| Count type B     |                                                                                                                          |
| Test Requirement | Validate 'Revoke token by hash' in RefreshTokenService.RevokeTokenAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                         | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for RefreshTokenService.RevokeTokenAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F040 - RefreshTokenService.RevokeAllUserTokensAsync

| Header           | Value                                                                                                                              |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F040                                                                                                                               |
| Function Name    | RefreshTokenService.RevokeAllUserTokensAsync                                                                                       |
| Total Test Cases | 2                                                                                                                                  |
| Created By       |                                                                                                                                    |
| Executed By      |                                                                                                                                    |
| Lines of Code    |                                                                                                                                    |
| Passed           |                                                                                                                                    |
| Failed           |                                                                                                                                    |
| Untested         |                                                                                                                                    |
| Count type N     |                                                                                                                                    |
| Count type A     |                                                                                                                                    |
| Count type B     |                                                                                                                                    |
| Test Requirement | Validate 'Revoke all user tokens' in RefreshTokenService.RevokeAllUserTokensAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                 | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for RefreshTokenService.RevokeAllUserTokensAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F041 - RefreshTokenService.RevokeTokenFamilyAsync

| Header           | Value                                                                                                                         |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F041                                                                                                                          |
| Function Name    | RefreshTokenService.RevokeTokenFamilyAsync                                                                                    |
| Total Test Cases | 2                                                                                                                             |
| Created By       |                                                                                                                               |
| Executed By      |                                                                                                                               |
| Lines of Code    |                                                                                                                               |
| Passed           |                                                                                                                               |
| Failed           |                                                                                                                               |
| Untested         |                                                                                                                               |
| Count type N     |                                                                                                                               |
| Count type A     |                                                                                                                               |
| Count type B     |                                                                                                                               |
| Test Requirement | Validate 'Revoke token family' in RefreshTokenService.RevokeTokenFamilyAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                               | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for RefreshTokenService.RevokeTokenFamilyAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F042 - RefreshTokenService.CleanupExpiredTokensAsync

| Header           | Value                                                                                                                               |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F042                                                                                                                                |
| Function Name    | RefreshTokenService.CleanupExpiredTokensAsync                                                                                       |
| Total Test Cases | 3                                                                                                                                   |
| Created By       |                                                                                                                                     |
| Executed By      |                                                                                                                                     |
| Lines of Code    |                                                                                                                                     |
| Passed           |                                                                                                                                     |
| Failed           |                                                                                                                                     |
| Untested         |                                                                                                                                     |
| Count type N     |                                                                                                                                     |
| Count type A     |                                                                                                                                     |
| Count type B     |                                                                                                                                     |
| Test Requirement | Validate 'Cleanup expired tokens' in RefreshTokenService.CleanupExpiredTokensAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                  | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for RefreshTokenService.CleanupExpiredTokensAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F043 - TokenService.GenerateAccessTokenAsync

| Header           | Value                                                                                                                      |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F043                                                                                                                       |
| Function Name    | TokenService.GenerateAccessTokenAsync                                                                                      |
| Total Test Cases | 4                                                                                                                          |
| Created By       |                                                                                                                            |
| Executed By      |                                                                                                                            |
| Lines of Code    |                                                                                                                            |
| Passed           |                                                                                                                            |
| Failed           |                                                                                                                            |
| Untested         |                                                                                                                            |
| Count type N     |                                                                                                                            |
| Count type A     |                                                                                                                            |
| Count type B     |                                                                                                                            |
| Test Requirement | Validate 'Generate access token' in TokenService.GenerateAccessTokenAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                          | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for TokenService.GenerateAccessTokenAsync; split into 4 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |

---

## F044 - TokenService.GenerateRefreshToken

| Header           | Value                                                                                                                          |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F044                                                                                                                           |
| Function Name    | TokenService.GenerateRefreshToken                                                                                              |
| Total Test Cases | 3                                                                                                                              |
| Created By       |                                                                                                                                |
| Executed By      |                                                                                                                                |
| Lines of Code    |                                                                                                                                |
| Passed           |                                                                                                                                |
| Failed           |                                                                                                                                |
| Untested         |                                                                                                                                |
| Count type N     |                                                                                                                                |
| Count type A     |                                                                                                                                |
| Count type B     |                                                                                                                                |
| Test Requirement | Validate 'Generate refresh token string' in TokenService.GenerateRefreshToken, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                      | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for TokenService.GenerateRefreshToken; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F045 - TokenService.ValidateToken

| Header           | Value                                                                                                    |
| ---------------- | -------------------------------------------------------------------------------------------------------- |
| Function Code    | F045                                                                                                     |
| Function Name    | TokenService.ValidateToken                                                                               |
| Total Test Cases | 3                                                                                                        |
| Created By       |                                                                                                          |
| Executed By      |                                                                                                          |
| Lines of Code    |                                                                                                          |
| Passed           |                                                                                                          |
| Failed           |                                                                                                          |
| Untested         |                                                                                                          |
| Count type N     |                                                                                                          |
| Count type A     |                                                                                                          |
| Count type B     |                                                                                                          |
| Test Requirement | Validate 'Validate token' in TokenService.ValidateToken, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                               | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for TokenService.ValidateToken; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F046 - TokenService.GetUserIdFromToken

| Header           | Value                                                                                                                 |
| ---------------- | --------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F046                                                                                                                  |
| Function Name    | TokenService.GetUserIdFromToken                                                                                       |
| Total Test Cases | 3                                                                                                                     |
| Created By       |                                                                                                                       |
| Executed By      |                                                                                                                       |
| Lines of Code    |                                                                                                                       |
| Passed           |                                                                                                                       |
| Failed           |                                                                                                                       |
| Untested         |                                                                                                                       |
| Count type N     |                                                                                                                       |
| Count type A     |                                                                                                                       |
| Count type B     |                                                                                                                       |
| Test Requirement | Validate 'Get user id from token' in TokenService.GetUserIdFromToken, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                    | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for TokenService.GetUserIdFromToken; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F047 - TokenService.GetJtiFromToken

| Header           | Value                                                                                                          |
| ---------------- | -------------------------------------------------------------------------------------------------------------- |
| Function Code    | F047                                                                                                           |
| Function Name    | TokenService.GetJtiFromToken                                                                                   |
| Total Test Cases | 2                                                                                                              |
| Created By       |                                                                                                                |
| Executed By      |                                                                                                                |
| Lines of Code    |                                                                                                                |
| Passed           |                                                                                                                |
| Failed           |                                                                                                                |
| Untested         |                                                                                                                |
| Count type N     |                                                                                                                |
| Count type A     |                                                                                                                |
| Count type B     |                                                                                                                |
| Test Requirement | Validate 'Get jti from token' in TokenService.GetJtiFromToken, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                 | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for TokenService.GetJtiFromToken; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F048 - TokenService.HashToken

| Header           | Value                                                                                            |
| ---------------- | ------------------------------------------------------------------------------------------------ |
| Function Code    | F048                                                                                             |
| Function Name    | TokenService.HashToken                                                                           |
| Total Test Cases | 3                                                                                                |
| Created By       |                                                                                                  |
| Executed By      |                                                                                                  |
| Lines of Code    |                                                                                                  |
| Passed           |                                                                                                  |
| Failed           |                                                                                                  |
| Untested         |                                                                                                  |
| Count type N     |                                                                                                  |
| Count type A     |                                                                                                  |
| Count type B     |                                                                                                  |
| Test Requirement | Validate 'Hash token' in TokenService.HashToken, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                           | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for TokenService.HashToken; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F049 - AdminQueryService.GetOphthalmologistsAsync

| Header           | Value                                                                                                                                |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F049                                                                                                                                 |
| Function Name    | AdminQueryService.GetOphthalmologistsAsync                                                                                           |
| Total Test Cases | 5                                                                                                                                    |
| Created By       |                                                                                                                                      |
| Executed By      |                                                                                                                                      |
| Lines of Code    |                                                                                                                                      |
| Passed           |                                                                                                                                      |
| Failed           |                                                                                                                                      |
| Untested         |                                                                                                                                      |
| Count type N     |                                                                                                                                      |
| Count type A     |                                                                                                                                      |
| Count type B     |                                                                                                                                      |
| Test Requirement | Validate 'Get ophthalmologists query' in AdminQueryService.GetOphthalmologistsAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                               | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AdminQueryService.GetOphthalmologistsAsync; split into 5 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |

---

## F050 - AdminQueryService.GetPatientsAsync

| Header           | Value                                                                                                                |
| ---------------- | -------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F050                                                                                                                 |
| Function Name    | AdminQueryService.GetPatientsAsync                                                                                   |
| Total Test Cases | 4                                                                                                                    |
| Created By       |                                                                                                                      |
| Executed By      |                                                                                                                      |
| Lines of Code    |                                                                                                                      |
| Passed           |                                                                                                                      |
| Failed           |                                                                                                                      |
| Untested         |                                                                                                                      |
| Count type N     |                                                                                                                      |
| Count type A     |                                                                                                                      |
| Count type B     |                                                                                                                      |
| Test Requirement | Validate 'Get patients query' in AdminQueryService.GetPatientsAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                       | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AdminQueryService.GetPatientsAsync; split into 4 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |

---

## F051 - AdminQueryService.GetAuditLogsAsync

| Header           | Value                                                                                                                   |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F051                                                                                                                    |
| Function Name    | AdminQueryService.GetAuditLogsAsync                                                                                     |
| Total Test Cases | 5                                                                                                                       |
| Created By       |                                                                                                                         |
| Executed By      |                                                                                                                         |
| Lines of Code    |                                                                                                                         |
| Passed           |                                                                                                                         |
| Failed           |                                                                                                                         |
| Untested         |                                                                                                                         |
| Count type N     |                                                                                                                         |
| Count type A     |                                                                                                                         |
| Count type B     |                                                                                                                         |
| Test Requirement | Validate 'Get audit logs query' in AdminQueryService.GetAuditLogsAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                        | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AdminQueryService.GetAuditLogsAsync; split into 5 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |

---

## F052 - AiQuotaService.GetQuotaAsync

| Header           | Value                                                                                                    |
| ---------------- | -------------------------------------------------------------------------------------------------------- |
| Function Code    | F052                                                                                                     |
| Function Name    | AiQuotaService.GetQuotaAsync                                                                             |
| Total Test Cases | 5                                                                                                        |
| Created By       |                                                                                                          |
| Executed By      |                                                                                                          |
| Lines of Code    |                                                                                                          |
| Passed           |                                                                                                          |
| Failed           |                                                                                                          |
| Untested         |                                                                                                          |
| Count type N     |                                                                                                          |
| Count type A     |                                                                                                          |
| Count type B     |                                                                                                          |
| Test Requirement | Validate 'Get AI quota' in AiQuotaService.GetQuotaAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                 | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AiQuotaService.GetQuotaAsync; split into 5 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |

---

## F053 - AiQuotaService.HasAvailableQuotaAsync

| Header           | Value                                                                                                                         |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F053                                                                                                                          |
| Function Name    | AiQuotaService.HasAvailableQuotaAsync                                                                                         |
| Total Test Cases | 2                                                                                                                             |
| Created By       |                                                                                                                               |
| Executed By      |                                                                                                                               |
| Lines of Code    |                                                                                                                               |
| Passed           |                                                                                                                               |
| Failed           |                                                                                                                               |
| Untested         |                                                                                                                               |
| Count type N     |                                                                                                                               |
| Count type A     |                                                                                                                               |
| Count type B     |                                                                                                                               |
| Test Requirement | Validate 'Check AI quota available' in AiQuotaService.HasAvailableQuotaAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                          | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AiQuotaService.HasAvailableQuotaAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F054 - AiQuotaService.DeductQuotaAsync

| Header           | Value                                                                                                          |
| ---------------- | -------------------------------------------------------------------------------------------------------------- |
| Function Code    | F054                                                                                                           |
| Function Name    | AiQuotaService.DeductQuotaAsync                                                                                |
| Total Test Cases | 4                                                                                                              |
| Created By       |                                                                                                                |
| Executed By      |                                                                                                                |
| Lines of Code    |                                                                                                                |
| Passed           |                                                                                                                |
| Failed           |                                                                                                                |
| Untested         |                                                                                                                |
| Count type N     |                                                                                                                |
| Count type A     |                                                                                                                |
| Count type B     |                                                                                                                |
| Test Requirement | Validate 'Deduct AI quota' in AiQuotaService.DeductQuotaAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                    | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AiQuotaService.DeductQuotaAsync; split into 4 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |

---

## F055 - AiQuotaService.AddPurchasedQuotaAsync

| Header           | Value                                                                                                                       |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F055                                                                                                                        |
| Function Name    | AiQuotaService.AddPurchasedQuotaAsync                                                                                       |
| Total Test Cases | 4                                                                                                                           |
| Created By       |                                                                                                                             |
| Executed By      |                                                                                                                             |
| Lines of Code    |                                                                                                                             |
| Passed           |                                                                                                                             |
| Failed           |                                                                                                                             |
| Untested         |                                                                                                                             |
| Count type N     |                                                                                                                             |
| Count type A     |                                                                                                                             |
| Count type B     |                                                                                                                             |
| Test Requirement | Validate 'Add purchased AI quota' in AiQuotaService.AddPurchasedQuotaAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                          | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for AiQuotaService.AddPurchasedQuotaAsync; split into 4 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |

---

## F056 - BetterStackHeartbeatService.GetEmbedUrl

| Header           | Value                                                                                                                            |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F056                                                                                                                             |
| Function Name    | BetterStackHeartbeatService.GetEmbedUrl                                                                                          |
| Total Test Cases | 2                                                                                                                                |
| Created By       |                                                                                                                                  |
| Executed By      |                                                                                                                                  |
| Lines of Code    |                                                                                                                                  |
| Passed           |                                                                                                                                  |
| Failed           |                                                                                                                                  |
| Untested         |                                                                                                                                  |
| Count type N     |                                                                                                                                  |
| Count type A     |                                                                                                                                  |
| Count type B     |                                                                                                                                  |
| Test Requirement | Validate 'Get BetterStack embed URL' in BetterStackHeartbeatService.GetEmbedUrl, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                            | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for BetterStackHeartbeatService.GetEmbedUrl; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F057 - BetterStackHeartbeatService.GetMonitorDescriptors

| Header           | Value                                                                                                                                                |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F057                                                                                                                                                 |
| Function Name    | BetterStackHeartbeatService.GetMonitorDescriptors                                                                                                    |
| Total Test Cases | 2                                                                                                                                                    |
| Created By       |                                                                                                                                                      |
| Executed By      |                                                                                                                                                      |
| Lines of Code    |                                                                                                                                                      |
| Passed           |                                                                                                                                                      |
| Failed           |                                                                                                                                                      |
| Untested         |                                                                                                                                                      |
| Count type N     |                                                                                                                                                      |
| Count type A     |                                                                                                                                                      |
| Count type B     |                                                                                                                                                      |
| Test Requirement | Validate 'Get BetterStack monitor descriptors' in BetterStackHeartbeatService.GetMonitorDescriptors, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                      | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for BetterStackHeartbeatService.GetMonitorDescriptors; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F058 - BetterStackHeartbeatService.NotifyStartedAsync

| Header           | Value                                                                                                                                |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F058                                                                                                                                 |
| Function Name    | BetterStackHeartbeatService.NotifyStartedAsync                                                                                       |
| Total Test Cases | 3                                                                                                                                    |
| Created By       |                                                                                                                                      |
| Executed By      |                                                                                                                                      |
| Lines of Code    |                                                                                                                                      |
| Passed           |                                                                                                                                      |
| Failed           |                                                                                                                                      |
| Untested         |                                                                                                                                      |
| Count type N     |                                                                                                                                      |
| Count type A     |                                                                                                                                      |
| Count type B     |                                                                                                                                      |
| Test Requirement | Validate 'Notify monitor started' in BetterStackHeartbeatService.NotifyStartedAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                   | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for BetterStackHeartbeatService.NotifyStartedAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F059 - BetterStackHeartbeatService.NotifySucceededAsync

| Header           | Value                                                                                                                                    |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F059                                                                                                                                     |
| Function Name    | BetterStackHeartbeatService.NotifySucceededAsync                                                                                         |
| Total Test Cases | 2                                                                                                                                        |
| Created By       |                                                                                                                                          |
| Executed By      |                                                                                                                                          |
| Lines of Code    |                                                                                                                                          |
| Passed           |                                                                                                                                          |
| Failed           |                                                                                                                                          |
| Untested         |                                                                                                                                          |
| Count type N     |                                                                                                                                          |
| Count type A     |                                                                                                                                          |
| Count type B     |                                                                                                                                          |
| Test Requirement | Validate 'Notify monitor succeeded' in BetterStackHeartbeatService.NotifySucceededAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                     | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for BetterStackHeartbeatService.NotifySucceededAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F060 - BetterStackHeartbeatService.NotifyFailedAsync

| Header           | Value                                                                                                                              |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F060                                                                                                                               |
| Function Name    | BetterStackHeartbeatService.NotifyFailedAsync                                                                                      |
| Total Test Cases | 2                                                                                                                                  |
| Created By       |                                                                                                                                    |
| Executed By      |                                                                                                                                    |
| Lines of Code    |                                                                                                                                    |
| Passed           |                                                                                                                                    |
| Failed           |                                                                                                                                    |
| Untested         |                                                                                                                                    |
| Count type N     |                                                                                                                                    |
| Count type A     |                                                                                                                                    |
| Count type B     |                                                                                                                                    |
| Test Requirement | Validate 'Notify monitor failed' in BetterStackHeartbeatService.NotifyFailedAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                  | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for BetterStackHeartbeatService.NotifyFailedAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F061 - DashboardMetricsService.GetSystemAdminMetricsAsync

| Header           | Value                                                                                                                                      |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F061                                                                                                                                       |
| Function Name    | DashboardMetricsService.GetSystemAdminMetricsAsync                                                                                         |
| Total Test Cases | 6                                                                                                                                          |
| Created By       |                                                                                                                                            |
| Executed By      |                                                                                                                                            |
| Lines of Code    |                                                                                                                                            |
| Passed           |                                                                                                                                            |
| Failed           |                                                                                                                                            |
| Untested         |                                                                                                                                            |
| Count type N     |                                                                                                                                            |
| Count type A     |                                                                                                                                            |
| Count type B     |                                                                                                                                            |
| Test Requirement | Validate 'Get system admin metrics' in DashboardMetricsService.GetSystemAdminMetricsAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                       | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for DashboardMetricsService.GetSystemAdminMetricsAsync; split into 6 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID06 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |
| UTCID06 | A |  |  |  |  |  |  |

---

## F062 - DashboardMetricsService.GetRecentScreeningsAsync

| Header           | Value                                                                                                                                 |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F062                                                                                                                                  |
| Function Name    | DashboardMetricsService.GetRecentScreeningsAsync                                                                                      |
| Total Test Cases | 4                                                                                                                                     |
| Created By       |                                                                                                                                       |
| Executed By      |                                                                                                                                       |
| Lines of Code    |                                                                                                                                       |
| Passed           |                                                                                                                                       |
| Failed           |                                                                                                                                       |
| Untested         |                                                                                                                                       |
| Count type N     |                                                                                                                                       |
| Count type A     |                                                                                                                                       |
| Count type B     |                                                                                                                                       |
| Test Requirement | Validate 'Get recent screenings' in DashboardMetricsService.GetRecentScreeningsAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                     | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for DashboardMetricsService.GetRecentScreeningsAsync; split into 4 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |

---

## F063 - DashboardMetricsService.GetScreeningVolumeTrendsAsync

| Header           | Value                                                                                                                                            |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F063                                                                                                                                             |
| Function Name    | DashboardMetricsService.GetScreeningVolumeTrendsAsync                                                                                            |
| Total Test Cases | 3                                                                                                                                                |
| Created By       |                                                                                                                                                  |
| Executed By      |                                                                                                                                                  |
| Lines of Code    |                                                                                                                                                  |
| Passed           |                                                                                                                                                  |
| Failed           |                                                                                                                                                  |
| Untested         |                                                                                                                                                  |
| Count type N     |                                                                                                                                                  |
| Count type A     |                                                                                                                                                  |
| Count type B     |                                                                                                                                                  |
| Test Requirement | Validate 'Get screening volume trends' in DashboardMetricsService.GetScreeningVolumeTrendsAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                          | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for DashboardMetricsService.GetScreeningVolumeTrendsAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F064 - DashboardMetricsService.GetPopulationRiskAnalysisAsync

| Header           | Value                                                                                                                                              |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F064                                                                                                                                               |
| Function Name    | DashboardMetricsService.GetPopulationRiskAnalysisAsync                                                                                             |
| Total Test Cases | 3                                                                                                                                                  |
| Created By       |                                                                                                                                                    |
| Executed By      |                                                                                                                                                    |
| Lines of Code    |                                                                                                                                                    |
| Passed           |                                                                                                                                                    |
| Failed           |                                                                                                                                                    |
| Untested         |                                                                                                                                                    |
| Count type N     |                                                                                                                                                    |
| Count type A     |                                                                                                                                                    |
| Count type B     |                                                                                                                                                    |
| Test Requirement | Validate 'Get population risk analysis' in DashboardMetricsService.GetPopulationRiskAnalysisAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                           | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for DashboardMetricsService.GetPopulationRiskAnalysisAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F065 - DashboardMetricsService.GetSystemHealthAsync

| Header           | Value                                                                                                                         |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F065                                                                                                                          |
| Function Name    | DashboardMetricsService.GetSystemHealthAsync                                                                                  |
| Total Test Cases | 2                                                                                                                             |
| Created By       |                                                                                                                               |
| Executed By      |                                                                                                                               |
| Lines of Code    |                                                                                                                               |
| Passed           |                                                                                                                               |
| Failed           |                                                                                                                               |
| Untested         |                                                                                                                               |
| Count type N     |                                                                                                                               |
| Count type A     |                                                                                                                               |
| Count type B     |                                                                                                                               |
| Test Requirement | Validate 'Get system health' in DashboardMetricsService.GetSystemHealthAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                 | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for DashboardMetricsService.GetSystemHealthAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F066 - DashboardMetricsService.GetOphthalmologistMetricsAsync

| Header           | Value                                                                                                                                             |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F066                                                                                                                                              |
| Function Name    | DashboardMetricsService.GetOphthalmologistMetricsAsync                                                                                            |
| Total Test Cases | 4                                                                                                                                                 |
| Created By       |                                                                                                                                                   |
| Executed By      |                                                                                                                                                   |
| Lines of Code    |                                                                                                                                                   |
| Passed           |                                                                                                                                                   |
| Failed           |                                                                                                                                                   |
| Untested         |                                                                                                                                                   |
| Count type N     |                                                                                                                                                   |
| Count type A     |                                                                                                                                                   |
| Count type B     |                                                                                                                                                   |
| Test Requirement | Validate 'Get ophthalmologist metrics' in DashboardMetricsService.GetOphthalmologistMetricsAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                           | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for DashboardMetricsService.GetOphthalmologistMetricsAsync; split into 4 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |

---

## F067 - DashboardMetricsService.GetPatientMetricsAsync

| Header           | Value                                                                                                                             |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F067                                                                                                                              |
| Function Name    | DashboardMetricsService.GetPatientMetricsAsync                                                                                    |
| Total Test Cases | 3                                                                                                                                 |
| Created By       |                                                                                                                                   |
| Executed By      |                                                                                                                                   |
| Lines of Code    |                                                                                                                                   |
| Passed           |                                                                                                                                   |
| Failed           |                                                                                                                                   |
| Untested         |                                                                                                                                   |
| Count type N     |                                                                                                                                   |
| Count type A     |                                                                                                                                   |
| Count type B     |                                                                                                                                   |
| Test Requirement | Validate 'Get patient metrics' in DashboardMetricsService.GetPatientMetricsAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                   | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for DashboardMetricsService.GetPatientMetricsAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F068 - DateTimeService.Now (property)

| Header           | Value                                                                                                       |
| ---------------- | ----------------------------------------------------------------------------------------------------------- |
| Function Code    | F068                                                                                                        |
| Function Name    | DateTimeService.Now (property)                                                                              |
| Total Test Cases | 2                                                                                                           |
| Created By       |                                                                                                             |
| Executed By      |                                                                                                             |
| Lines of Code    |                                                                                                             |
| Passed           |                                                                                                             |
| Failed           |                                                                                                             |
| Untested         |                                                                                                             |
| Count type N     |                                                                                                             |
| Count type A     |                                                                                                             |
| Count type B     |                                                                                                             |
| Test Requirement | Validate 'Get local now' in DateTimeService.Now (property), following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                   | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for DateTimeService.Now (property); split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F069 - DateTimeService.UtcNow (property)

| Header           | Value                                                                                                        |
| ---------------- | ------------------------------------------------------------------------------------------------------------ |
| Function Code    | F069                                                                                                         |
| Function Name    | DateTimeService.UtcNow (property)                                                                            |
| Total Test Cases | 2                                                                                                            |
| Created By       |                                                                                                              |
| Executed By      |                                                                                                              |
| Lines of Code    |                                                                                                              |
| Passed           |                                                                                                              |
| Failed           |                                                                                                              |
| Untested         |                                                                                                              |
| Count type N     |                                                                                                              |
| Count type A     |                                                                                                              |
| Count type B     |                                                                                                              |
| Test Requirement | Validate 'Get UTC now' in DateTimeService.UtcNow (property), following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                      | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for DateTimeService.UtcNow (property); split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F070 - EmailService.SendEmailConfirmationAsync

| Header           | Value                                                                                                                          |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F070                                                                                                                           |
| Function Name    | EmailService.SendEmailConfirmationAsync                                                                                        |
| Total Test Cases | 2                                                                                                                              |
| Created By       |                                                                                                                                |
| Executed By      |                                                                                                                                |
| Lines of Code    |                                                                                                                                |
| Passed           |                                                                                                                                |
| Failed           |                                                                                                                                |
| Untested         |                                                                                                                                |
| Count type N     |                                                                                                                                |
| Count type A     |                                                                                                                                |
| Count type B     |                                                                                                                                |
| Test Requirement | Validate 'Send email confirmation' in EmailService.SendEmailConfirmationAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                            | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for EmailService.SendEmailConfirmationAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F071 - EmailService.SendPasswordResetAsync

| Header           | Value                                                                                                                        |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F071                                                                                                                         |
| Function Name    | EmailService.SendPasswordResetAsync                                                                                          |
| Total Test Cases | 2                                                                                                                            |
| Created By       |                                                                                                                              |
| Executed By      |                                                                                                                              |
| Lines of Code    |                                                                                                                              |
| Passed           |                                                                                                                              |
| Failed           |                                                                                                                              |
| Untested         |                                                                                                                              |
| Count type N     |                                                                                                                              |
| Count type A     |                                                                                                                              |
| Count type B     |                                                                                                                              |
| Test Requirement | Validate 'Send password reset email' in EmailService.SendPasswordResetAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                        | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for EmailService.SendPasswordResetAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F072 - EmailService.SendWelcomeEmailAsync

| Header           | Value                                                                                                                |
| ---------------- | -------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F072                                                                                                                 |
| Function Name    | EmailService.SendWelcomeEmailAsync                                                                                   |
| Total Test Cases | 2                                                                                                                    |
| Created By       |                                                                                                                      |
| Executed By      |                                                                                                                      |
| Lines of Code    |                                                                                                                      |
| Passed           |                                                                                                                      |
| Failed           |                                                                                                                      |
| Untested         |                                                                                                                      |
| Count type N     |                                                                                                                      |
| Count type A     |                                                                                                                      |
| Count type B     |                                                                                                                      |
| Test Requirement | Validate 'Send welcome email' in EmailService.SendWelcomeEmailAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                       | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for EmailService.SendWelcomeEmailAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F073 - EmailService.SendAsync

| Header           | Value                                                                                                    |
| ---------------- | -------------------------------------------------------------------------------------------------------- |
| Function Code    | F073                                                                                                     |
| Function Name    | EmailService.SendAsync                                                                                   |
| Total Test Cases | 4                                                                                                        |
| Created By       |                                                                                                          |
| Executed By      |                                                                                                          |
| Lines of Code    |                                                                                                          |
| Passed           |                                                                                                          |
| Failed           |                                                                                                          |
| Untested         |                                                                                                          |
| Count type N     |                                                                                                          |
| Count type A     |                                                                                                          |
| Count type B     |                                                                                                          |
| Test Requirement | Validate 'Send generic email' in EmailService.SendAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                           | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for EmailService.SendAsync; split into 4 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |

---

## F074 - GoogleMeetService.CreateMeetingAsync

| Header           | Value                                                                                                                          |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F074                                                                                                                           |
| Function Name    | GoogleMeetService.CreateMeetingAsync                                                                                           |
| Total Test Cases | 5                                                                                                                              |
| Created By       |                                                                                                                                |
| Executed By      |                                                                                                                                |
| Lines of Code    |                                                                                                                                |
| Passed           |                                                                                                                                |
| Failed           |                                                                                                                                |
| Untested         |                                                                                                                                |
| Count type N     |                                                                                                                                |
| Count type A     |                                                                                                                                |
| Count type B     |                                                                                                                                |
| Test Requirement | Validate 'Create Google Meet meeting' in GoogleMeetService.CreateMeetingAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                         | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for GoogleMeetService.CreateMeetingAsync; split into 5 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |

---

## F075 - GoogleMeetService.DeleteMeetingAsync

| Header           | Value                                                                                                                          |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F075                                                                                                                           |
| Function Name    | GoogleMeetService.DeleteMeetingAsync                                                                                           |
| Total Test Cases | 2                                                                                                                              |
| Created By       |                                                                                                                                |
| Executed By      |                                                                                                                                |
| Lines of Code    |                                                                                                                                |
| Passed           |                                                                                                                                |
| Failed           |                                                                                                                                |
| Untested         |                                                                                                                                |
| Count type N     |                                                                                                                                |
| Count type A     |                                                                                                                                |
| Count type B     |                                                                                                                                |
| Test Requirement | Validate 'Delete Google Meet meeting' in GoogleMeetService.DeleteMeetingAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                         | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for GoogleMeetService.DeleteMeetingAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F076 - GoogleMeetService.Dispose

| Header           | Value                                                                                                                |
| ---------------- | -------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F076                                                                                                                 |
| Function Name    | GoogleMeetService.Dispose                                                                                            |
| Total Test Cases | 1                                                                                                                    |
| Created By       |                                                                                                                      |
| Executed By      |                                                                                                                      |
| Lines of Code    |                                                                                                                      |
| Passed           |                                                                                                                      |
| Failed           |                                                                                                                      |
| Untested         |                                                                                                                      |
| Count type N     |                                                                                                                      |
| Count type A     |                                                                                                                      |
| Count type B     |                                                                                                                      |
| Test Requirement | Validate 'Dispose Google Meet service' in GoogleMeetService.Dispose, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                              | Precondition                                                                         | UTCIDs  |
| ------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------ | ------- |
| Checklist-defined scenarios for GoogleMeetService.Dispose; split into 1 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |

---

## F077 - NotificationService.SendAsync (typed)

| Header           | Value                                                                                                                        |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F077                                                                                                                         |
| Function Name    | NotificationService.SendAsync (typed)                                                                                        |
| Total Test Cases | 4                                                                                                                            |
| Created By       |                                                                                                                              |
| Executed By      |                                                                                                                              |
| Lines of Code    |                                                                                                                              |
| Passed           |                                                                                                                              |
| Failed           |                                                                                                                              |
| Untested         |                                                                                                                              |
| Count type N     |                                                                                                                              |
| Count type A     |                                                                                                                              |
| Count type B     |                                                                                                                              |
| Test Requirement | Validate 'Send typed notification' in NotificationService.SendAsync (typed), following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                          | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for NotificationService.SendAsync (typed); split into 4 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |

---

## F078 - NotificationService.SendAsync (legacy)

| Header           | Value                                                                                                                          |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F078                                                                                                                           |
| Function Name    | NotificationService.SendAsync (legacy)                                                                                         |
| Total Test Cases | 1                                                                                                                              |
| Created By       |                                                                                                                                |
| Executed By      |                                                                                                                                |
| Lines of Code    |                                                                                                                                |
| Passed           |                                                                                                                                |
| Failed           |                                                                                                                                |
| Untested         |                                                                                                                                |
| Count type N     |                                                                                                                                |
| Count type A     |                                                                                                                                |
| Count type B     |                                                                                                                                |
| Test Requirement | Validate 'Send legacy notification' in NotificationService.SendAsync (legacy), following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                           | Precondition                                                                         | UTCIDs  |
| ------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | ------- |
| Checklist-defined scenarios for NotificationService.SendAsync (legacy); split into 1 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |

---

## F079 - PatientRoadmapGenerationService.GenerateFromDiagnosisAsync

| Header           | Value                                                                                                                                                     |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F079                                                                                                                                                      |
| Function Name    | PatientRoadmapGenerationService.GenerateFromDiagnosisAsync                                                                                                |
| Total Test Cases | 6                                                                                                                                                         |
| Created By       |                                                                                                                                                           |
| Executed By      |                                                                                                                                                           |
| Lines of Code    |                                                                                                                                                           |
| Passed           |                                                                                                                                                           |
| Failed           |                                                                                                                                                           |
| Untested         |                                                                                                                                                           |
| Count type N     |                                                                                                                                                           |
| Count type A     |                                                                                                                                                           |
| Count type B     |                                                                                                                                                           |
| Test Requirement | Validate 'Generate roadmap from diagnosis' in PatientRoadmapGenerationService.GenerateFromDiagnosisAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                               | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for PatientRoadmapGenerationService.GenerateFromDiagnosisAsync; split into 6 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID06 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |
| UTCID06 | A |  |  |  |  |  |  |

---

## F080 - PayOSService.CreatePaymentLinkAsync

| Header           | Value                                                                                                                        |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F080                                                                                                                         |
| Function Name    | PayOSService.CreatePaymentLinkAsync                                                                                          |
| Total Test Cases | 5                                                                                                                            |
| Created By       |                                                                                                                              |
| Executed By      |                                                                                                                              |
| Lines of Code    |                                                                                                                              |
| Passed           |                                                                                                                              |
| Failed           |                                                                                                                              |
| Untested         |                                                                                                                              |
| Count type N     |                                                                                                                              |
| Count type A     |                                                                                                                              |
| Count type B     |                                                                                                                              |
| Test Requirement | Validate 'Create PayOS payment link' in PayOSService.CreatePaymentLinkAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                        | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for PayOSService.CreatePaymentLinkAsync; split into 5 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |

---

## F081 - PayOSService.GetPaymentStatusAsync

| Header           | Value                                                                                                                      |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F081                                                                                                                       |
| Function Name    | PayOSService.GetPaymentStatusAsync                                                                                         |
| Total Test Cases | 3                                                                                                                          |
| Created By       |                                                                                                                            |
| Executed By      |                                                                                                                            |
| Lines of Code    |                                                                                                                            |
| Passed           |                                                                                                                            |
| Failed           |                                                                                                                            |
| Untested         |                                                                                                                            |
| Count type N     |                                                                                                                            |
| Count type A     |                                                                                                                            |
| Count type B     |                                                                                                                            |
| Test Requirement | Validate 'Get PayOS payment status' in PayOSService.GetPaymentStatusAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                       | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for PayOSService.GetPaymentStatusAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F082 - PayOSService.VerifyWebhookSignatureAsync

| Header           | Value                                                                                                                                  |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F082                                                                                                                                   |
| Function Name    | PayOSService.VerifyWebhookSignatureAsync                                                                                               |
| Total Test Cases | 2                                                                                                                                      |
| Created By       |                                                                                                                                        |
| Executed By      |                                                                                                                                        |
| Lines of Code    |                                                                                                                                        |
| Passed           |                                                                                                                                        |
| Failed           |                                                                                                                                        |
| Untested         |                                                                                                                                        |
| Count type N     |                                                                                                                                        |
| Count type A     |                                                                                                                                        |
| Count type B     |                                                                                                                                        |
| Test Requirement | Validate 'Verify PayOS webhook signature' in PayOSService.VerifyWebhookSignatureAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                             | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for PayOSService.VerifyWebhookSignatureAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F083 - PayOSService.CancelPaymentAsync

| Header           | Value                                                                                                               |
| ---------------- | ------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F083                                                                                                                |
| Function Name    | PayOSService.CancelPaymentAsync                                                                                     |
| Total Test Cases | 3                                                                                                                   |
| Created By       |                                                                                                                     |
| Executed By      |                                                                                                                     |
| Lines of Code    |                                                                                                                     |
| Passed           |                                                                                                                     |
| Failed           |                                                                                                                     |
| Untested         |                                                                                                                     |
| Count type N     |                                                                                                                     |
| Count type A     |                                                                                                                     |
| Count type B     |                                                                                                                     |
| Test Requirement | Validate 'Cancel PayOS payment' in PayOSService.CancelPaymentAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                    | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for PayOSService.CancelPaymentAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F084 - SupabaseStorageService.SaveFileAsync

| Header           | Value                                                                                                                     |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F084                                                                                                                      |
| Function Name    | SupabaseStorageService.SaveFileAsync                                                                                      |
| Total Test Cases | 5                                                                                                                         |
| Created By       |                                                                                                                           |
| Executed By      |                                                                                                                           |
| Lines of Code    |                                                                                                                           |
| Passed           |                                                                                                                           |
| Failed           |                                                                                                                           |
| Untested         |                                                                                                                           |
| Count type N     |                                                                                                                           |
| Count type A     |                                                                                                                           |
| Count type B     |                                                                                                                           |
| Test Requirement | Validate 'Save file to Supabase' in SupabaseStorageService.SaveFileAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                         | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for SupabaseStorageService.SaveFileAsync; split into 5 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |
| UTCID05 | A |  |  |  |  |  |  |

---

## F085 - SupabaseStorageService.DeleteFile

| Header           | Value                                                                                                                      |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F085                                                                                                                       |
| Function Name    | SupabaseStorageService.DeleteFile                                                                                          |
| Total Test Cases | 3                                                                                                                          |
| Created By       |                                                                                                                            |
| Executed By      |                                                                                                                            |
| Lines of Code    |                                                                                                                            |
| Passed           |                                                                                                                            |
| Failed           |                                                                                                                            |
| Untested         |                                                                                                                            |
| Count type N     |                                                                                                                            |
| Count type A     |                                                                                                                            |
| Count type B     |                                                                                                                            |
| Test Requirement | Validate 'Delete file from Supabase' in SupabaseStorageService.DeleteFile, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                      | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for SupabaseStorageService.DeleteFile; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F086 - SupabaseStorageService.FileExists

| Header           | Value                                                                                                                       |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F086                                                                                                                        |
| Function Name    | SupabaseStorageService.FileExists                                                                                           |
| Total Test Cases | 3                                                                                                                           |
| Created By       |                                                                                                                             |
| Executed By      |                                                                                                                             |
| Lines of Code    |                                                                                                                             |
| Passed           |                                                                                                                             |
| Failed           |                                                                                                                             |
| Untested         |                                                                                                                             |
| Count type N     |                                                                                                                             |
| Count type A     |                                                                                                                             |
| Count type B     |                                                                                                                             |
| Test Requirement | Validate 'Check Supabase file exists' in SupabaseStorageService.FileExists, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                      | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for SupabaseStorageService.FileExists; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F087 - SystemSettingService.GetSettingAsync

| Header           | Value                                                                                                                  |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F087                                                                                                                   |
| Function Name    | SystemSettingService.GetSettingAsync                                                                                   |
| Total Test Cases | 2                                                                                                                      |
| Created By       |                                                                                                                        |
| Executed By      |                                                                                                                        |
| Lines of Code    |                                                                                                                        |
| Passed           |                                                                                                                        |
| Failed           |                                                                                                                        |
| Untested         |                                                                                                                        |
| Count type N     |                                                                                                                        |
| Count type A     |                                                                                                                        |
| Count type B     |                                                                                                                        |
| Test Requirement | Validate 'Get setting by key' in SystemSettingService.GetSettingAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                         | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for SystemSettingService.GetSettingAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F088 - SystemSettingService.GetAllSettingsAsync

| Header           | Value                                                                                                                    |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F088                                                                                                                     |
| Function Name    | SystemSettingService.GetAllSettingsAsync                                                                                 |
| Total Test Cases | 2                                                                                                                        |
| Created By       |                                                                                                                          |
| Executed By      |                                                                                                                          |
| Lines of Code    |                                                                                                                          |
| Passed           |                                                                                                                          |
| Failed           |                                                                                                                          |
| Untested         |                                                                                                                          |
| Count type N     |                                                                                                                          |
| Count type A     |                                                                                                                          |
| Count type B     |                                                                                                                          |
| Test Requirement | Validate 'Get all settings' in SystemSettingService.GetAllSettingsAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                             | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for SystemSettingService.GetAllSettingsAsync; split into 2 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |

---

## F089 - SystemSettingService.UpdateSettingsAsync

| Header           | Value                                                                                                                   |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F089                                                                                                                    |
| Function Name    | SystemSettingService.UpdateSettingsAsync                                                                                |
| Total Test Cases | 4                                                                                                                       |
| Created By       |                                                                                                                         |
| Executed By      |                                                                                                                         |
| Lines of Code    |                                                                                                                         |
| Passed           |                                                                                                                         |
| Failed           |                                                                                                                         |
| Untested         |                                                                                                                         |
| Count type N     |                                                                                                                         |
| Count type A     |                                                                                                                         |
| Count type B     |                                                                                                                         |
| Test Requirement | Validate 'Update settings' in SystemSettingService.UpdateSettingsAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                             | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for SystemSettingService.UpdateSettingsAsync; split into 4 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |

---

## F090 - ClinicVisitService.ProcessPaymentCompletionAsync

| Header           | Value                                                                                                                                      |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| Function Code    | F090                                                                                                                                       |
| Function Name    | ClinicVisitService.ProcessPaymentCompletionAsync                                                                                           |
| Total Test Cases | 4                                                                                                                                          |
| Created By       |                                                                                                                                            |
| Executed By      |                                                                                                                                            |
| Lines of Code    |                                                                                                                                            |
| Passed           |                                                                                                                                            |
| Failed           |                                                                                                                                            |
| Untested         |                                                                                                                                            |
| Count type N     |                                                                                                                                            |
| Count type A     |                                                                                                                                            |
| Count type B     |                                                                                                                                            |
| Test Requirement | Validate 'Process payment completion' in ClinicVisitService.ProcessPaymentCompletionAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                     | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for ClinicVisitService.ProcessPaymentCompletionAsync; split into 4 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |
| UTCID04 | A |  |  |  |  |  |  |

---

## F091 - PayOSPayoutService.CreatePayoutAsync

| Header           | Value                                                                                                                   |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F091                                                                                                                    |
| Function Name    | PayOSPayoutService.CreatePayoutAsync                                                                                    |
| Total Test Cases | 3                                                                                                                       |
| Created By       |                                                                                                                         |
| Executed By      |                                                                                                                         |
| Lines of Code    |                                                                                                                         |
| Passed           |                                                                                                                         |
| Failed           |                                                                                                                         |
| Untested         |                                                                                                                         |
| Count type N     |                                                                                                                         |
| Count type A     |                                                                                                                         |
| Count type B     |                                                                                                                         |
| Test Requirement | Validate 'Create PayOS payout' in PayOSPayoutService.CreatePayoutAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                         | Precondition                                                                         | UTCIDs          |
| ----------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for PayOSPayoutService.CreatePayoutAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F092 - PayOSPayoutService.GetPayoutAsync

| Header           | Value                                                                                                                     |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F092                                                                                                                      |
| Function Name    | PayOSPayoutService.GetPayoutAsync                                                                                         |
| Total Test Cases | 3                                                                                                                         |
| Created By       |                                                                                                                           |
| Executed By      |                                                                                                                           |
| Lines of Code    |                                                                                                                           |
| Passed           |                                                                                                                           |
| Failed           |                                                                                                                           |
| Untested         |                                                                                                                           |
| Count type N     |                                                                                                                           |
| Count type A     |                                                                                                                           |
| Count type B     |                                                                                                                           |
| Test Requirement | Validate 'Get PayOS payout details' in PayOSPayoutService.GetPayoutAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                      | Precondition                                                                         | UTCIDs          |
| -------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for PayOSPayoutService.GetPayoutAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F093 - PayOSPayoutService.GetPayoutsAsync

| Header           | Value                                                                                                                   |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F093                                                                                                                    |
| Function Name    | PayOSPayoutService.GetPayoutsAsync                                                                                      |
| Total Test Cases | 3                                                                                                                       |
| Created By       |                                                                                                                         |
| Executed By      |                                                                                                                         |
| Lines of Code    |                                                                                                                         |
| Passed           |                                                                                                                         |
| Failed           |                                                                                                                         |
| Untested         |                                                                                                                         |
| Count type N     |                                                                                                                         |
| Count type A     |                                                                                                                         |
| Count type B     |                                                                                                                         |
| Test Requirement | Validate 'Get all PayOS payouts' in PayOSPayoutService.GetPayoutsAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                       | Precondition                                                                         | UTCIDs          |
| --------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for PayOSPayoutService.GetPayoutsAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F094 - PayOSPayoutService.EstimateCreditAsync

| Header           | Value                                                                                                                        |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F094                                                                                                                         |
| Function Name    | PayOSPayoutService.EstimateCreditAsync                                                                                       |
| Total Test Cases | 3                                                                                                                            |
| Created By       |                                                                                                                              |
| Executed By      |                                                                                                                              |
| Lines of Code    |                                                                                                                              |
| Passed           |                                                                                                                              |
| Failed           |                                                                                                                              |
| Untested         |                                                                                                                              |
| Count type N     |                                                                                                                              |
| Count type A     |                                                                                                                              |
| Count type B     |                                                                                                                              |
| Test Requirement | Validate 'Estimate payout credit' in PayOSPayoutService.EstimateCreditAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                           | Precondition                                                                         | UTCIDs          |
| ------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for PayOSPayoutService.EstimateCreditAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F095 - PayOSPayoutService.GetPayoutAccountBalanceAsync

| Header           | Value                                                                                                                                     |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F095                                                                                                                                      |
| Function Name    | PayOSPayoutService.GetPayoutAccountBalanceAsync                                                                                           |
| Total Test Cases | 3                                                                                                                                         |
| Created By       |                                                                                                                                           |
| Executed By      |                                                                                                                                           |
| Lines of Code    |                                                                                                                                           |
| Passed           |                                                                                                                                           |
| Failed           |                                                                                                                                           |
| Untested         |                                                                                                                                           |
| Count type N     |                                                                                                                                           |
| Count type A     |                                                                                                                                           |
| Count type B     |                                                                                                                                           |
| Test Requirement | Validate 'Get payout account balance' in PayOSPayoutService.GetPayoutAccountBalanceAsync, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                    | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for PayOSPayoutService.GetPayoutAccountBalanceAsync; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---

## F096 - PatientScreeningPdfService.GenerateScreeningReportPdf

| Header           | Value                                                                                                                                               |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F096                                                                                                                                                |
| Function Name    | PatientScreeningPdfService.GenerateScreeningReportPdf                                                                                               |
| Total Test Cases | 3                                                                                                                                                   |
| Created By       |                                                                                                                                                     |
| Executed By      |                                                                                                                                                     |
| Lines of Code    |                                                                                                                                                     |
| Passed           |                                                                                                                                                     |
| Failed           |                                                                                                                                                     |
| Untested         |                                                                                                                                                     |
| Count type N     |                                                                                                                                                     |
| Count type A     |                                                                                                                                                     |
| Count type B     |                                                                                                                                                     |
| Test Requirement | Validate 'Generate patient screening PDF' in PatientScreeningPdfService.GenerateScreeningReportPdf, following the current Infrastructure checklist. |

### Condition Matrix

| Condition                                                                                                                          | Precondition                                                                         | UTCIDs          |
| ---------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ | --------------- |
| Checklist-defined scenarios for PatientScreeningPdfService.GenerateScreeningReportPdf; split into 3 UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 | A |  |  |  |  |  |  |
| UTCID02 | A |  |  |  |  |  |  |
| UTCID03 | A |  |  |  |  |  |  |

---
