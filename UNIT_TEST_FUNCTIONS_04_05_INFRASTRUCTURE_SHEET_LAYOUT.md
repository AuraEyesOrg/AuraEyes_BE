# Infrastructure Unit Test - Sheet Layout Ready

Last updated: 13/04/2026
Source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

## How to copy into Sheet / Excel

- Each block `## F001` … `## F096` is **one function**; copy the whole block into one sheet or one table range.
- In the Header table **Value** column, fill in **Created By**, **Executed By**, **Lines of Code**, **Passed / Failed / Untested**, **Count type N / A / B** after you run tests.
- **Result Matrix**: enter **P**/**F** (or Passed/Failed), **Executed Date**, **Defect ID** per UTCID.
- (Optional) Before **Result Matrix**, you may insert a **Confirm** table: Expected Return / Exception / Log message per UTCID (add columns yourself if needed).
- Empty cells are **placeholders**; to convert markdown tables to Sheet, paste into a table converter (e.g. tableconvert) if needed.

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
| Email already exists -> Failure conflict message. | Valid dependencies and data setup for this scenario | UTCID01-UTCID02 |
| `_userManager.CreateAsync` fails -> rollback transaction + return identity errors. | Valid dependencies and data setup for this scenario | UTCID03-UTCID04 |
| Create user + add role + create patient + commit -> Success with UserId/Email. | Valid dependencies and data setup for this scenario | UTCID05-UTCID06 |
| Confirmation email send fails -> still Success, warning log. | Valid dependencies and data setup for this scenario | UTCID07-UTCID08 |
| Exception during transaction -> rollback + generic Failure. | Valid dependencies and data setup for this scenario | UTCID09-UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |
| UTCID06 | A |  |  |  |
| UTCID07 | A |  |  |  |
| UTCID08 | A |  |  |  |
| UTCID09 | A |  |  |  |
| UTCID10 | A |  |  |  |

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
| No credentials -> Failure. | Valid dependencies and data setup for this scenario | UTCID01-UTCID08 |
| Has credentials but no Degree -> Failure. | Valid dependencies and data setup for this scenario | UTCID09-UTCID16 |
| Has Degree but no License -> Failure. | Valid dependencies and data setup for this scenario | UTCID17-UTCID24 |
| Degree level/file/expiry validation fails -> Failure validation. | Valid dependencies and data setup for this scenario | UTCID25-UTCID40 |
| License expiry validation fails (`<= issued`) -> Failure validation. | Valid dependencies and data setup for this scenario | UTCID41-UTCID50 |
| Upload credentials + create profile + commit -> Success. | Valid dependencies and data setup for this scenario | UTCID51-UTCID54 |
| Error after file upload -> rollback and cleanup uploaded files. | Valid dependencies and data setup for this scenario | UTCID55-UTCID58 |
| Confirm/admin email error -> still Success, log warning. | Valid dependencies and data setup for this scenario | UTCID59-UTCID60 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |
| UTCID06 | A |  |  |  |
| UTCID07 | A |  |  |  |
| UTCID08 | A |  |  |  |
| UTCID09 | A |  |  |  |
| UTCID10 | A |  |  |  |
| UTCID11 | A |  |  |  |
| UTCID12 | A |  |  |  |
| UTCID13 | A |  |  |  |
| UTCID14 | A |  |  |  |
| UTCID15 | A |  |  |  |
| UTCID16 | A |  |  |  |
| UTCID17 | A |  |  |  |
| UTCID18 | A |  |  |  |
| UTCID19 | A |  |  |  |
| UTCID20 | A |  |  |  |
| UTCID21 | A |  |  |  |
| UTCID22 | A |  |  |  |
| UTCID23 | A |  |  |  |
| UTCID24 | A |  |  |  |
| UTCID25 | A |  |  |  |
| UTCID26 | A |  |  |  |
| UTCID27 | A |  |  |  |
| UTCID28 | A |  |  |  |
| UTCID29 | A |  |  |  |
| UTCID30 | A |  |  |  |
| UTCID31 | A |  |  |  |
| UTCID32 | A |  |  |  |
| UTCID33 | A |  |  |  |
| UTCID34 | A |  |  |  |
| UTCID35 | A |  |  |  |
| UTCID36 | A |  |  |  |
| UTCID37 | A |  |  |  |
| UTCID38 | A |  |  |  |
| UTCID39 | A |  |  |  |
| UTCID40 | A |  |  |  |
| UTCID41 | A |  |  |  |
| UTCID42 | A |  |  |  |
| UTCID43 | A |  |  |  |
| UTCID44 | A |  |  |  |
| UTCID45 | A |  |  |  |
| UTCID46 | A |  |  |  |
| UTCID47 | A |  |  |  |
| UTCID48 | A |  |  |  |
| UTCID49 | A |  |  |  |
| UTCID50 | A |  |  |  |
| UTCID51 | A |  |  |  |
| UTCID52 | A |  |  |  |
| UTCID53 | A |  |  |  |
| UTCID54 | A |  |  |  |
| UTCID55 | A |  |  |  |
| UTCID56 | A |  |  |  |
| UTCID57 | A |  |  |  |
| UTCID58 | A |  |  |  |
| UTCID59 | A |  |  |  |
| UTCID60 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |
| UTCID06 | A |  |  |  |
| UTCID07 | A |  |  |  |
| UTCID08 | A |  |  |  |
| UTCID09 | A |  |  |  |
| UTCID10 | A |  |  |  |
| UTCID11 | A |  |  |  |
| UTCID12 | A |  |  |  |
| UTCID13 | A |  |  |  |
| UTCID14 | A |  |  |  |
| UTCID15 | A |  |  |  |
| UTCID16 | A |  |  |  |
| UTCID17 | A |  |  |  |
| UTCID18 | A |  |  |  |
| UTCID19 | A |  |  |  |
| UTCID20 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |
| UTCID06 | A |  |  |  |
| UTCID07 | A |  |  |  |
| UTCID08 | A |  |  |  |
| UTCID09 | A |  |  |  |
| UTCID10 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |
| UTCID06 | A |  |  |  |
| UTCID07 | A |  |  |  |
| UTCID08 | A |  |  |  |
| UTCID09 | A |  |  |  |
| UTCID10 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |
| UTCID06 | A |  |  |  |
| UTCID07 | A |  |  |  |
| UTCID08 | A |  |  |  |
| UTCID09 | A |  |  |  |
| UTCID10 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |
| UTCID06 | A |  |  |  |
| UTCID07 | A |  |  |  |
| UTCID08 | A |  |  |  |
| UTCID09 | A |  |  |  |
| UTCID10 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |
| UTCID06 | A |  |  |  |
| UTCID07 | A |  |  |  |
| UTCID08 | A |  |  |  |
| UTCID09 | A |  |  |  |
| UTCID10 | A |  |  |  |
| UTCID11 | A |  |  |  |
| UTCID12 | A |  |  |  |
| UTCID13 | A |  |  |  |
| UTCID14 | A |  |  |  |
| UTCID15 | A |  |  |  |
| UTCID16 | A |  |  |  |
| UTCID17 | A |  |  |  |
| UTCID18 | A |  |  |  |
| UTCID19 | A |  |  |  |
| UTCID20 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |
| UTCID06 | A |  |  |  |
| UTCID07 | A |  |  |  |
| UTCID08 | A |  |  |  |
| UTCID09 | A |  |  |  |
| UTCID10 | A |  |  |  |
| UTCID11 | A |  |  |  |
| UTCID12 | A |  |  |  |
| UTCID13 | A |  |  |  |
| UTCID14 | A |  |  |  |
| UTCID15 | A |  |  |  |
| UTCID16 | A |  |  |  |
| UTCID17 | A |  |  |  |
| UTCID18 | A |  |  |  |
| UTCID19 | A |  |  |  |
| UTCID20 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |
| UTCID06 | A |  |  |  |
| UTCID07 | A |  |  |  |
| UTCID08 | A |  |  |  |
| UTCID09 | A |  |  |  |
| UTCID10 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |
| UTCID06 | A |  |  |  |
| UTCID07 | A |  |  |  |
| UTCID08 | A |  |  |  |
| UTCID09 | A |  |  |  |
| UTCID10 | A |  |  |  |
| UTCID11 | A |  |  |  |
| UTCID12 | A |  |  |  |
| UTCID13 | A |  |  |  |
| UTCID14 | A |  |  |  |
| UTCID15 | A |  |  |  |
| UTCID16 | A |  |  |  |
| UTCID17 | A |  |  |  |
| UTCID18 | A |  |  |  |
| UTCID19 | A |  |  |  |
| UTCID20 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |
| UTCID06 | A |  |  |  |
| UTCID07 | A |  |  |  |
| UTCID08 | A |  |  |  |
| UTCID09 | A |  |  |  |
| UTCID10 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |
| UTCID06 | A |  |  |  |
| UTCID07 | A |  |  |  |
| UTCID08 | A |  |  |  |
| UTCID09 | A |  |  |  |
| UTCID10 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |
| UTCID06 | A |  |  |  |
| UTCID07 | A |  |  |  |
| UTCID08 | A |  |  |  |
| UTCID09 | A |  |  |  |
| UTCID10 | A |  |  |  |
| UTCID11 | A |  |  |  |
| UTCID12 | A |  |  |  |
| UTCID13 | A |  |  |  |
| UTCID14 | A |  |  |  |
| UTCID15 | A |  |  |  |
| UTCID16 | A |  |  |  |
| UTCID17 | A |  |  |  |
| UTCID18 | A |  |  |  |
| UTCID19 | A |  |  |  |
| UTCID20 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | B |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |
| UTCID03 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | N |  |  |  |
| UTCID05 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | N |  |  |  |
| UTCID05 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | B |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | B |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | N |  |  |  |
| UTCID05 | N |  |  |  |
| UTCID06 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | N |  |  |  |
| UTCID05 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | N |  |  |  |
| UTCID05 | N |  |  |  |
| UTCID06 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | N |  |  |  |
| UTCID05 | A |  |  |  |
| UTCID06 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | N |  |  |  |
| UTCID05 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |
| UTCID03 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | B |  |  |  |
| UTCID05 | N |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |

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

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | B |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | N |  |  |  |

---


