# Infrastructure Unit Test - Sheet Layout Ready

Ngay cap nhat: 07/04/2026
Nguon: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

## F005 - AuthService.RegisterPatientAsync

| Header | Value |
|---|---|
| Function Code | F005 |
| Function Name | AuthService.RegisterPatientAsync |
| Total Test Cases | 10 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Email da ton tai -> Failure conflict message. | Valid dependencies and data setup for this scenario | UTCID01-UTCID02 |
| `_userManager.CreateAsync` fail -> rollback transaction + tra identity errors. | Valid dependencies and data setup for this scenario | UTCID03-UTCID04 |
| Tao user + add role + tao patient + commit -> Success co UserId/Email. | Valid dependencies and data setup for this scenario | UTCID05-UTCID06 |
| Gui email confirm fail -> van Success, co warning log. | Valid dependencies and data setup for this scenario | UTCID07-UTCID08 |
| Exception trong transaction -> rollback + Failure generic. | Valid dependencies and data setup for this scenario | UTCID09-UTCID10 |

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

## F006 - AuthService.RegisterOphthalmologistAsync

| Header | Value |
|---|---|
| Function Code | F006 |
| Function Name | AuthService.RegisterOphthalmologistAsync |
| Total Test Cases | 60 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Khong co credential nao -> Failure. | Valid dependencies and data setup for this scenario | UTCID01-UTCID08 |
| Co credentials nhung khong co Degree -> Failure. | Valid dependencies and data setup for this scenario | UTCID09-UTCID16 |
| Co Degree nhung khong co License -> Failure. | Valid dependencies and data setup for this scenario | UTCID17-UTCID24 |
| Degree level/file/expiry validation fail -> Failure validation. | Valid dependencies and data setup for this scenario | UTCID25-UTCID40 |
| License expiry validation fail (`<= issued`) -> Failure validation. | Valid dependencies and data setup for this scenario | UTCID41-UTCID50 |
| Upload credentials + tao profile + commit -> Success. | Valid dependencies and data setup for this scenario | UTCID51-UTCID54 |
| Loi sau khi upload file -> rollback va cleanup file da upload. | Valid dependencies and data setup for this scenario | UTCID55-UTCID58 |
| Loi gui email confirm/admin -> van Success, ghi warning. | Valid dependencies and data setup for this scenario | UTCID59-UTCID60 |

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

## F007 - AuthService.RegisterOrganisationAsync

| Header | Value |
|---|---|
| Function Code | F007 |
| Function Name | AuthService.RegisterOrganisationAsync |
| Total Test Cases | 20 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Onboarding service tra Success -> AuthService tra Success payload dung. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |
| Onboarding service tra Conflict -> AuthService giu nguyen conflict. | Valid dependencies and data setup for this scenario | UTCID11-UTCID20 |

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

## F008 - AuthService.GoogleLoginAsync

| Header | Value |
|---|---|
| Function Code | F008 |
| Function Name | AuthService.GoogleLoginAsync |
| Total Test Cases | 10 |
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

## F009 - AuthService.LoginAsync

| Header | Value |
|---|---|
| Function Code | F009 |
| Function Name | AuthService.LoginAsync |
| Total Test Cases | 10 |
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

## F010 - AuthService.VerifyTwoFactorLoginAsync

| Header | Value |
|---|---|
| Function Code | F010 |
| Function Name | AuthService.VerifyTwoFactorLoginAsync |
| Total Test Cases | 10 |
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

## F011 - AuthService.RefreshTokenAsync

| Header | Value |
|---|---|
| Function Code | F011 |
| Function Name | AuthService.RefreshTokenAsync |
| Total Test Cases | 10 |
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

## F012 - AuthService.LogoutAsync

| Header | Value |
|---|---|
| Function Code | F012 |
| Function Name | AuthService.LogoutAsync |
| Total Test Cases | 20 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Token khong ton tai -> van Success (idempotent). | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |
| Exception khi revoke -> Failure. | Valid dependencies and data setup for this scenario | UTCID11-UTCID20 |

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

## F013 - AuthService.LogoutAllAsync

| Header | Value |
|---|---|
| Function Code | F013 |
| Function Name | AuthService.LogoutAllAsync |
| Total Test Cases | 20 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Revoke all token thanh cong -> Success. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |
| Revoke all token throw -> Failure. | Valid dependencies and data setup for this scenario | UTCID11-UTCID20 |

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

## F014 - AuthService.ConfirmEmailAsync

| Header | Value |
|---|---|
| Function Code | F014 |
| Function Name | AuthService.ConfirmEmailAsync |
| Total Test Cases | 10 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| `userId` khong parse duoc Guid -> Failure invalid id. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |

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

## F015 - AuthService.ForgotPasswordAsync

| Header | Value |
|---|---|
| Function Code | F015 |
| Function Name | AuthService.ForgotPasswordAsync |
| Total Test Cases | 20 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Email khong ton tai -> khong gui, van Success (anti enumeration). | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |
| Exception send mail/token -> Failure. | Valid dependencies and data setup for this scenario | UTCID11-UTCID20 |

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

## F016 - AuthService.ResetPasswordAsync

| Header | Value |
|---|---|
| Function Code | F016 |
| Function Name | AuthService.ResetPasswordAsync |
| Total Test Cases | 10 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| UserId sai format -> Failure invalid id. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |

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

## F017 - AuthService.GetCurrentUserAsync

| Header | Value |
|---|---|
| Function Code | F017 |
| Function Name | AuthService.GetCurrentUserAsync |
| Total Test Cases | 10 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai -> Unauthorized. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |

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

## F018 - AuthService.ResendConfirmationAsync

| Header | Value |
|---|---|
| Function Code | F018 |
| Function Name | AuthService.ResendConfirmationAsync |
| Total Test Cases | 20 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai/da confirm -> van Success. | Valid dependencies and data setup for this scenario | UTCID01-UTCID10 |
| Exception gui email -> Failure. | Valid dependencies and data setup for this scenario | UTCID11-UTCID20 |

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

## F019 - IdentityService.CheckPasswordAsync

| Header | Value |
|---|---|
| Function Code | F019 |
| Function Name | IdentityService.CheckPasswordAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| UserId khong ton tai -> false. | Valid dependencies and data setup for this scenario | UTCID01 |
| Password sai -> false. | Valid dependencies and data setup for this scenario | UTCID02 |
| Password dung -> true. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | N |  |  |  |

---

## F020 - IdentityService.GetUserByEmailAsync

| Header | Value |
|---|---|
| Function Code | F020 |
| Function Name | IdentityService.GetUserByEmailAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User ton tai va !IsDeleted -> tra `UserDto`. | Valid dependencies and data setup for this scenario | UTCID01 |
| User bi IsDeleted -> null. | Valid dependencies and data setup for this scenario | UTCID02 |
| Khong tim thay email -> null. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |

---

## F021 - IdentityService.GetUserByIdAsync

| Header | Value |
|---|---|
| Function Code | F021 |
| Function Name | IdentityService.GetUserByIdAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User ton tai va !IsDeleted -> dto day du. | Valid dependencies and data setup for this scenario | UTCID01 |
| User deleted -> null. | Valid dependencies and data setup for this scenario | UTCID02 |
| UserId khong ton tai -> null. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |

---

## F022 - IdentityService.IsPhoneNumberInUseByOrganizationAsync

| Header | Value |
|---|---|
| Function Code | F022 |
| Function Name | IdentityService.IsPhoneNumberInUseByOrganizationAsync |
| Total Test Cases | 4 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Phone input rong/chi ky tu trang -> false. | Valid dependencies and data setup for this scenario | UTCID01 |
| Cung so nhung khac format (`+84`, khoang trang, dau gach) -> true. | Valid dependencies and data setup for this scenario | UTCID02 |
| So ton tai nhung khac organisation -> false. | Valid dependencies and data setup for this scenario | UTCID03 |
| Cung org nhung user deleted -> false. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | B |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |

---

## F023 - IdentityService.IsEmailConfirmedAsync

| Header | Value |
|---|---|
| Function Code | F023 |
| Function Name | IdentityService.IsEmailConfirmedAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai -> false. | Valid dependencies and data setup for this scenario | UTCID01 |
| User ton tai, EmailConfirmed=false -> false. | Valid dependencies and data setup for this scenario | UTCID02 |
| User ton tai, EmailConfirmed=true -> true. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

---

## F024 - IdentityService.IsUserActiveAsync

| Header | Value |
|---|---|
| Function Code | F024 |
| Function Name | IdentityService.IsUserActiveAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai -> false. | Valid dependencies and data setup for this scenario | UTCID01 |
| User inactive hoac deleted -> false. | Valid dependencies and data setup for this scenario | UTCID02 |
| User active va !deleted -> true. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | N |  |  |  |

---

## F025 - IdentityService.GenerateEmailConfirmationTokenAsync

| Header | Value |
|---|---|
| Function Code | F025 |
| Function Name | IdentityService.GenerateEmailConfirmationTokenAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai -> throw `InvalidOperationException`. | Valid dependencies and data setup for this scenario | UTCID01 |
| User ton tai -> token khong rong. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

---

## F026 - IdentityService.GeneratePasswordResetTokenAsync

| Header | Value |
|---|---|
| Function Code | F026 |
| Function Name | IdentityService.GeneratePasswordResetTokenAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai -> throw. | Valid dependencies and data setup for this scenario | UTCID01 |
| User ton tai -> token khong rong. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

---

## F027 - IdentityService.GetUserRolesAsync

| Header | Value |
|---|---|
| Function Code | F027 |
| Function Name | IdentityService.GetUserRolesAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai -> empty list. | Valid dependencies and data setup for this scenario | UTCID01 |
| User 1 role -> dung role do. | Valid dependencies and data setup for this scenario | UTCID02 |
| User nhieu role -> tra du role. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

---

## F028 - IdentityService.IsInRoleAsync

| Header | Value |
|---|---|
| Function Code | F028 |
| Function Name | IdentityService.IsInRoleAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai -> false. | Valid dependencies and data setup for this scenario | UTCID01 |
| User co role -> true. | Valid dependencies and data setup for this scenario | UTCID02 |
| User khong co role -> false. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

---

## F029 - IdentityService.GetUserIdsByRoleAndOrganizationAsync

| Header | Value |
|---|---|
| Function Code | F029 |
| Function Name | IdentityService.GetUserIdsByRoleAndOrganizationAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Loc dung user theo role + org. | Valid dependencies and data setup for this scenario | UTCID01 |
| User deleted/inactive du cung role -> bi loai. | Valid dependencies and data setup for this scenario | UTCID02 |
| Khong co user hop le -> danh sach rong. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | B |  |  |  |

---

## F030 - IdentityService.UpdateLastLoginAsync

| Header | Value |
|---|---|
| Function Code | F030 |
| Function Name | IdentityService.UpdateLastLoginAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai -> no-op. | Valid dependencies and data setup for this scenario | UTCID01 |
| User ton tai -> LastLoginAt cap nhat + `UpdateAsync` duoc goi. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

---

## F031 - IdentityService.IsTwoFactorEnabledAsync

| Header | Value |
|---|---|
| Function Code | F031 |
| Function Name | IdentityService.IsTwoFactorEnabledAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai -> false. | Valid dependencies and data setup for this scenario | UTCID01 |
| User co 2FA=false -> false. | Valid dependencies and data setup for this scenario | UTCID02 |
| User co 2FA=true -> true. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

---

## F032 - IdentityService.GetAuthenticatorKeyAsync

| Header | Value |
|---|---|
| Function Code | F032 |
| Function Name | IdentityService.GetAuthenticatorKeyAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai -> null. | Valid dependencies and data setup for this scenario | UTCID01 |
| User ton tai -> tra key. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

---

## F033 - IdentityService.GetOrCreateAuthenticatorKeyAsync

| Header | Value |
|---|---|
| Function Code | F033 |
| Function Name | IdentityService.GetOrCreateAuthenticatorKeyAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai -> throw. | Valid dependencies and data setup for this scenario | UTCID01 |
| User ton tai -> reset key va lay key moi thanh cong. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

---

## F034 - IdentityService.VerifyTwoFactorCodeAsync

| Header | Value |
|---|---|
| Function Code | F034 |
| Function Name | IdentityService.VerifyTwoFactorCodeAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai -> false. | Valid dependencies and data setup for this scenario | UTCID01 |
| Code sai -> false. | Valid dependencies and data setup for this scenario | UTCID02 |
| Code dung -> true. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | N |  |  |  |

---

## F035 - IdentityService.GenerateNewRecoveryCodesAsync

| Header | Value |
|---|---|
| Function Code | F035 |
| Function Name | IdentityService.GenerateNewRecoveryCodesAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai -> throw. | Valid dependencies and data setup for this scenario | UTCID01 |
| User ton tai -> tra mang recovery code theo count. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

---

## F036 - IdentityService.GetRecoveryCodesCountAsync

| Header | Value |
|---|---|
| Function Code | F036 |
| Function Name | IdentityService.GetRecoveryCodesCountAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai -> 0. | Valid dependencies and data setup for this scenario | UTCID01 |
| User ton tai -> so luong dung. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

---

## F037 - IdentityService.GenerateAuthenticatorUri

| Header | Value |
|---|---|
| Function Code | F037 |
| Function Name | IdentityService.GenerateAuthenticatorUri |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Email + key hop le -> URI dung schema `otpauth://totp`. | Valid dependencies and data setup for this scenario | UTCID01 |
| Email co ky tu dac biet -> duoc UrlEncode dung. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |

---

## F038 - IdentityService.FormatAuthenticatorKey

| Header | Value |
|---|---|
| Function Code | F038 |
| Function Name | IdentityService.FormatAuthenticatorKey |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Key dai -> chia block 4 ky tu + uppercase. | Valid dependencies and data setup for this scenario | UTCID01 |
| Key ngan hon 4 -> van uppercase, khong loi. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |

---

## F039 - IdentityService.GetUserMetricsAsync

| Header | Value |
|---|---|
| Function Code | F039 |
| Function Name | IdentityService.GetUserMetricsAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Du lieu co lich su thang truoc -> tinh % change dung. | Valid dependencies and data setup for this scenario | UTCID01 |
| Thang truoc =0 nhung co user moi -> change =100. | Valid dependencies and data setup for this scenario | UTCID02 |
| Pending approvals dem dung theo dieu kien. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |
| UTCID03 | N |  |  |  |

---

## F040 - IdentityService.GetUsersInRoleCountAsync

| Header | Value |
|---|---|
| Function Code | F040 |
| Function Name | IdentityService.GetUsersInRoleCountAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| `activeOnly=true` -> chi dem active + !deleted. | Valid dependencies and data setup for this scenario | UTCID01 |
| `activeOnly=false` -> dem toan bo !deleted. | Valid dependencies and data setup for this scenario | UTCID02 |
| Role khong co user -> 0. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | B |  |  |  |

---

## F041 - IdentityService.GetPendingApprovalsCountAsync

| Header | Value |
|---|---|
| Function Code | F041 |
| Function Name | IdentityService.GetPendingApprovalsCountAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Dem dung user `!EmailConfirmed // !IsActive`. | Valid dependencies and data setup for this scenario | UTCID01 |
| Khong co pending -> 0. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |

---

## F042 - IdentityService.GetUserDetailsAsync

| Header | Value |
|---|---|
| Function Code | F042 |
| Function Name | IdentityService.GetUserDetailsAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong ton tai -> null. | Valid dependencies and data setup for this scenario | UTCID01 |
| User deleted -> null. | Valid dependencies and data setup for this scenario | UTCID02 |
| User hop le -> dto day du thong tin profile. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | N |  |  |  |

---

## F046 - RefreshTokenService.CreateRefreshTokenAsync

| Header | Value |
|---|---|
| Function Code | F046 |
| Function Name | RefreshTokenService.CreateRefreshTokenAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Tao refresh token va save DB thanh cong -> return Id. | Valid dependencies and data setup for this scenario | UTCID01 |
| Du lieu luu dung `UserId`, `JwtId`, `TokenHash`. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

---

## F047 - RefreshTokenService.GetByTokenHashAsync

| Header | Value |
|---|---|
| Function Code | F047 |
| Function Name | RefreshTokenService.GetByTokenHashAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Hash khong ton tai -> null. | Valid dependencies and data setup for this scenario | UTCID01 |
| Hash ton tai -> map dto dung fields. | Valid dependencies and data setup for this scenario | UTCID02 |
| Token revoked/used -> `IsActive=false` trong dto. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

---

## F048 - RefreshTokenService.RotateRefreshTokenAsync

| Header | Value |
|---|---|
| Function Code | F048 |
| Function Name | RefreshTokenService.RotateRefreshTokenAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Old token id khong ton tai -> throw. | Valid dependencies and data setup for this scenario | UTCID01 |
| Old token ton tai -> tao new token thanh cong. | Valid dependencies and data setup for this scenario | UTCID02 |
| Old token duoc `MarkAsUsed` va lien ket token moi. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

---

## F049 - RefreshTokenService.RevokeTokenAsync

| Header | Value |
|---|---|
| Function Code | F049 |
| Function Name | RefreshTokenService.RevokeTokenAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Token ton tai -> revoke va save. | Valid dependencies and data setup for this scenario | UTCID01 |
| Token khong ton tai -> no-op, khong throw. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

---

## F050 - RefreshTokenService.RevokeAllUserTokensAsync

| Header | Value |
|---|---|
| Function Code | F050 |
| Function Name | RefreshTokenService.RevokeAllUserTokensAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Co nhieu token chua revoke -> tat ca bi revoke. | Valid dependencies and data setup for this scenario | UTCID01 |
| Khong co token hop le -> save van an toan. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |

---

## F051 - RefreshTokenService.RevokeTokenFamilyAsync

| Header | Value |
|---|---|
| Function Code | F051 |
| Function Name | RefreshTokenService.RevokeTokenFamilyAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Token goc khong ton tai -> return, khong throw. | Valid dependencies and data setup for this scenario | UTCID01 |
| Token ton tai -> revoke toan bo token cua user. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |

---

## F052 - RefreshTokenService.CleanupExpiredTokensAsync

| Header | Value |
|---|---|
| Function Code | F052 |
| Function Name | RefreshTokenService.CleanupExpiredTokensAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Token het han qua cutoff -> bi xoa. | Valid dependencies and data setup for this scenario | UTCID01 |
| Token revoked qua cutoff -> bi xoa. | Valid dependencies and data setup for this scenario | UTCID02 |
| Khong co token can xoa -> return 0. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | B |  |  |  |

---

## F053 - TokenService.GenerateAccessTokenAsync

| Header | Value |
|---|---|
| Function Code | F053 |
| Function Name | TokenService.GenerateAccessTokenAsync |
| Total Test Cases | 4 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Tao token co claims bat buoc (`sub`,`uid`,`email`,`jti`,`iat`). | Valid dependencies and data setup for this scenario | UTCID01 |
| Roles duoc dua vao `ClaimTypes.Role` va `role`. | Valid dependencies and data setup for this scenario | UTCID02 |
| Additional claims duoc append dung. | Valid dependencies and data setup for this scenario | UTCID03 |
| ExpiresAt dung theo config `AccessTokenExpiryMinutes`. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | B |  |  |  |

---

## F054 - TokenService.GenerateRefreshToken

| Header | Value |
|---|---|
| Function Code | F054 |
| Function Name | TokenService.GenerateRefreshToken |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Token base64 hop le, khong rong. | Valid dependencies and data setup for this scenario | UTCID01 |
| 2 lan sinh lien tiep khac nhau. | Valid dependencies and data setup for this scenario | UTCID02 |
| Do dai token du lon (>=64 byte random truoc encode). | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | B |  |  |  |

---

## F055 - TokenService.ValidateToken

| Header | Value |
|---|---|
| Function Code | F055 |
| Function Name | TokenService.ValidateToken |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Token hop le -> tra ClaimsPrincipal. | Valid dependencies and data setup for this scenario | UTCID01 |
| Token sai signature -> null. | Valid dependencies and data setup for this scenario | UTCID02 |
| Token alg khong phai HmacSha256 -> null. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |

---

## F056 - TokenService.GetUserIdFromToken

| Header | Value |
|---|---|
| Function Code | F056 |
| Function Name | TokenService.GetUserIdFromToken |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Token hop le co `sub` -> parse Guid thanh cong. | Valid dependencies and data setup for this scenario | UTCID01 |
| Khong co `sub` nhung co `uid`/`nameidentifier` -> van lay duoc. | Valid dependencies and data setup for this scenario | UTCID02 |
| Claim user id khong parse duoc -> null. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | A |  |  |  |

---

## F057 - TokenService.GetJtiFromToken

| Header | Value |
|---|---|
| Function Code | F057 |
| Function Name | TokenService.GetJtiFromToken |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Token hop le -> lay `jti`. | Valid dependencies and data setup for this scenario | UTCID01 |
| Token invalid -> null. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |

---

## F058 - TokenService.HashToken

| Header | Value |
|---|---|
| Function Code | F058 |
| Function Name | TokenService.HashToken |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Cung input -> hash giong nhau. | Valid dependencies and data setup for this scenario | UTCID01 |
| Khac input -> hash khac nhau. | Valid dependencies and data setup for this scenario | UTCID02 |
| Input rong -> van hash duoc (khong null). | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | B |  |  |  |

---

## F059 - AdminQueryService.GetOphthalmologistsAsync

| Header | Value |
|---|---|
| Function Code | F059 |
| Function Name | AdminQueryService.GetOphthalmologistsAsync |
| Total Test Cases | 5 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Khong filter -> paging + sort CreatedAt desc dung. | Valid dependencies and data setup for this scenario | UTCID01 |
| SearchTerm tim theo fullName/email/phone ILike. | Valid dependencies and data setup for this scenario | UTCID02 |
| VerificationStatus nhieu gia tri phan cach dau phay -> loc dung. | Valid dependencies and data setup for this scenario | UTCID03 |
| Map licenses/degrees tu Certificates dung theo Type. | Valid dependencies and data setup for this scenario | UTCID04 |
| VerificationStatus khong parse duoc -> bo qua filter status. | Valid dependencies and data setup for this scenario | UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | N |  |  |  |
| UTCID05 | B |  |  |  |

---

## F060 - AdminQueryService.GetPatientsAsync

| Header | Value |
|---|---|
| Function Code | F060 |
| Function Name | AdminQueryService.GetPatientsAsync |
| Total Test Cases | 4 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| SearchTerm hoat dong theo fullName/email. | Valid dependencies and data setup for this scenario | UTCID01 |
| Filter status `active/pending/suspended` dung. | Valid dependencies and data setup for this scenario | UTCID02 |
| Paging dung totalCount/items. | Valid dependencies and data setup for this scenario | UTCID03 |
| User deleted khong duoc hien thi. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | A |  |  |  |

---

## F061 - AdminQueryService.GetAuditLogsAsync

| Header | Value |
|---|---|
| Function Code | F061 |
| Function Name | AdminQueryService.GetAuditLogsAsync |
| Total Test Cases | 5 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Loc theo search term action/entity/email dung. | Valid dependencies and data setup for this scenario | UTCID01 |
| Loc theo action/entityName/userId dung. | Valid dependencies and data setup for this scenario | UTCID02 |
| Loc theo fromDate/toDate dung. | Valid dependencies and data setup for this scenario | UTCID03 |
| Paging + sort `CreatedAt desc` dung. | Valid dependencies and data setup for this scenario | UTCID04 |
| Log khong co user join -> `UserName=null` van map duoc. | Valid dependencies and data setup for this scenario | UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | N |  |  |  |
| UTCID05 | B |  |  |  |

---

## F062 - AiQuotaService.GetQuotaAsync

| Header | Value |
|---|---|
| Function Code | F062 |
| Function Name | AiQuotaService.GetQuotaAsync |
| Total Test Cases | 5 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Role Patient co profile -> tinh total/used/remaining/free-purchased source dung. | Valid dependencies and data setup for this scenario | UTCID01 |
| Role Patient khong co profile -> fallback free quota. | Valid dependencies and data setup for this scenario | UTCID02 |
| Role OrgAdmin/Ophthalmologist co org -> tinh quota theo organisation. | Valid dependencies and data setup for this scenario | UTCID03 |
| Role organisation nhung user khong co org -> None quota. | Valid dependencies and data setup for this scenario | UTCID04 |
| Role la -> None quota. | Valid dependencies and data setup for this scenario | UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | A |  |  |  |
| UTCID05 | A |  |  |  |

---

## F063 - AiQuotaService.HasAvailableQuotaAsync

| Header | Value |
|---|---|
| Function Code | F063 |
| Function Name | AiQuotaService.HasAvailableQuotaAsync |
| Total Test Cases | 2 |
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

## F064 - AiQuotaService.DeductQuotaAsync

| Header | Value |
|---|---|
| Function Code | F064 |
| Function Name | AiQuotaService.DeductQuotaAsync |
| Total Test Cases | 4 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Patient ton tai -> consume quota + save. | Valid dependencies and data setup for this scenario | UTCID01 |
| Org role va org ton tai -> consume quota organisation + save. | Valid dependencies and data setup for this scenario | UTCID02 |
| Patient khong ton tai -> throw `InvalidOperationException`. | Valid dependencies and data setup for this scenario | UTCID03 |
| User org role khong co org -> throw. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |

---

## F065 - AiQuotaService.AddPurchasedQuotaAsync

| Header | Value |
|---|---|
| Function Code | F065 |
| Function Name | AiQuotaService.AddPurchasedQuotaAsync |
| Total Test Cases | 4 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Patient ton tai -> tang purchased quota. | Valid dependencies and data setup for this scenario | UTCID01 |
| Org role co org -> tang purchased quota org. | Valid dependencies and data setup for this scenario | UTCID02 |
| Patient khong ton tai -> throw. | Valid dependencies and data setup for this scenario | UTCID03 |
| Org khong ton tai -> throw. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | A |  |  |  |

---

## F066 - BetterStackHeartbeatService.GetEmbedUrl

| Header | Value |
|---|---|
| Function Code | F066 |
| Function Name | BetterStackHeartbeatService.GetEmbedUrl |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| EmbedUrl null/white-space -> null. | Valid dependencies and data setup for this scenario | UTCID01 |
| EmbedUrl hop le -> tra lai dung gia tri. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | B |  |  |  |
| UTCID02 | N |  |  |  |

---

## F067 - BetterStackHeartbeatService.GetMonitorDescriptors

| Header | Value |
|---|---|
| Function Code | F067 |
| Function Name | BetterStackHeartbeatService.GetMonitorDescriptors |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Tra du descriptor cho tat ca enum monitor. | Valid dependencies and data setup for this scenario | UTCID01 |
| Truong `Configured` dung theo endpoint setting. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

---

## F068 - BetterStackHeartbeatService.NotifyStartedAsync

| Header | Value |
|---|---|
| Function Code | F068 |
| Function Name | BetterStackHeartbeatService.NotifyStartedAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Endpoint khong config -> skip, khong throw. | Valid dependencies and data setup for this scenario | UTCID01 |
| Endpoint config -> POST thanh cong. | Valid dependencies and data setup for this scenario | UTCID02 |
| HTTP fail/exception -> warning log, khong throw. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | B |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | A |  |  |  |

---

## F069 - BetterStackHeartbeatService.NotifySucceededAsync

| Header | Value |
|---|---|
| Function Code | F069 |
| Function Name | BetterStackHeartbeatService.NotifySucceededAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Ping URL hop le -> goi POST. | Valid dependencies and data setup for this scenario | UTCID01 |
| Non-success status -> warning log. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |

---

## F070 - BetterStackHeartbeatService.NotifyFailedAsync

| Header | Value |
|---|---|
| Function Code | F070 |
| Function Name | BetterStackHeartbeatService.NotifyFailedAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Co FailUrl -> POST den fail url. | Valid dependencies and data setup for this scenario | UTCID01 |
| Khong co FailUrl -> fallback ping url, loi cung khong throw. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |

---

## F072 - DashboardMetricsService.GetSystemAdminMetricsAsync

| Header | Value |
|---|---|
| Function Code | F072 |
| Function Name | DashboardMetricsService.GetSystemAdminMetricsAsync |
| Total Test Cases | 6 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Tinh tong doctor/org/patient va growth phan tram dung. | Valid dependencies and data setup for this scenario | UTCID01 |
| Revenue by payment method + percentage dung. | Valid dependencies and data setup for this scenario | UTCID02 |
| Monthly/Daily revenue map day du theo period. | Valid dependencies and data setup for this scenario | UTCID03 |
| Pending actions (verification/withdraw/onboarding) dung. | Valid dependencies and data setup for this scenario | UTCID04 |
| Top doctor by consultation revenue + top org by rating dung. | Valid dependencies and data setup for this scenario | UTCID05 |
| BetterStack monitor map vao response dung. | Valid dependencies and data setup for this scenario | UTCID06 |

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

## F073 - DashboardMetricsService.GetRecentScreeningsAsync

| Header | Value |
|---|---|
| Function Code | F073 |
| Function Name | DashboardMetricsService.GetRecentScreeningsAsync |
| Total Test Cases | 4 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Paging total/items dung. | Valid dependencies and data setup for this scenario | UTCID01 |
| RiskLevel va `IsCritical` map dung. | Valid dependencies and data setup for this scenario | UTCID02 |
| Status `Completed/Analyzing` map theo `ProcessedAt`. | Valid dependencies and data setup for this scenario | UTCID03 |
| Screening khong co risk result -> risk null van map duoc. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | B |  |  |  |

---

## F074 - DashboardMetricsService.GetScreeningVolumeTrendsAsync

| Header | Value |
|---|---|
| Function Code | F074 |
| Function Name | DashboardMetricsService.GetScreeningVolumeTrendsAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| `weekly` -> group theo dau tuan, label dd MMM. | Valid dependencies and data setup for this scenario | UTCID01 |
| `monthly` -> group theo thang, label MMM yyyy. | Valid dependencies and data setup for this scenario | UTCID02 |
| `timeRange` la -> fallback monthly. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | B |  |  |  |

---

## F075 - DashboardMetricsService.GetPopulationRiskAnalysisAsync

| Header | Value |
|---|---|
| Function Code | F075 |
| Function Name | DashboardMetricsService.GetPopulationRiskAnalysisAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Tong `TotalPatients` dung. | Valid dependencies and data setup for this scenario | UTCID01 |
| Bo qua RiskLevel.None trong danh sach output. | Valid dependencies and data setup for this scenario | UTCID02 |
| Khong co result -> percentage=0. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | B |  |  |  |

---

## F076 - DashboardMetricsService.GetSystemHealthAsync

| Header | Value |
|---|---|
| Function Code | F076 |
| Function Name | DashboardMetricsService.GetSystemHealthAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Tra payload co 3 component Database/AI/Notifications. | Valid dependencies and data setup for this scenario | UTCID01 |
| `AllSystemsOperational=true` va fields health day du. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

---

## F077 - DashboardMetricsService.GetOphthalmologistMetricsAsync

| Header | Value |
|---|---|
| Function Code | F077 |
| Function Name | DashboardMetricsService.GetOphthalmologistMetricsAsync |
| Total Test Cases | 4 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| UserId khong map duoc doctor -> dto rong. | Valid dependencies and data setup for this scenario | UTCID01 |
| Dem pending reviews va urgent cases dung. | Valid dependencies and data setup for this scenario | UTCID02 |
| Dem completed today dung. | Valid dependencies and data setup for this scenario | UTCID03 |
| Tinh open slots today dung theo template/org. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | N |  |  |  |

---

## F078 - DashboardMetricsService.GetOrganisationMetricsAsync

| Header | Value |
|---|---|
| Function Code | F078 |
| Function Name | DashboardMetricsService.GetOrganisationMetricsAsync |
| Total Test Cases | 4 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| User khong co organisation -> dto rong. | Valid dependencies and data setup for this scenario | UTCID01 |
| Dem total appointment + breakdown status dung. | Valid dependencies and data setup for this scenario | UTCID02 |
| Utilization rate theo booked/capacity dung. | Valid dependencies and data setup for this scenario | UTCID03 |
| Remaining AI quota lay tu `AiQuotaService` dung. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | N |  |  |  |

---

## F079 - DashboardMetricsService.GetPatientMetricsAsync

| Header | Value |
|---|---|
| Function Code | F079 |
| Function Name | DashboardMetricsService.GetPatientMetricsAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Khong tim thay patient profile -> dto rong. | Valid dependencies and data setup for this scenario | UTCID01 |
| Dem completed screenings / total reports dung. | Valid dependencies and data setup for this scenario | UTCID02 |
| Dem upcoming appointments + remaining quota dung. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

---

## F080 - DateTimeService.Now

| Header | Value |
|---|---|
| Function Code | F080 |
| Function Name | DateTimeService.Now |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Gia tri `Now` gan voi `DateTime.Now`. | Valid dependencies and data setup for this scenario | UTCID01 |
| Moi lan goi co the thay doi theo thoi gian thuc. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |

---

## F081 - DateTimeService.UtcNow

| Header | Value |
|---|---|
| Function Code | F081 |
| Function Name | DateTimeService.UtcNow |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Gia tri `UtcNow` gan voi `DateTime.UtcNow`. | Valid dependencies and data setup for this scenario | UTCID01 |
| `Kind` cua gia tri la UTC. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |

---

## F082 - EmailService.SendEmailConfirmationAsync

| Header | Value |
|---|---|
| Function Code | F082 |
| Function Name | EmailService.SendEmailConfirmationAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Dung subject/template confirm email. | Valid dependencies and data setup for this scenario | UTCID01 |
| Goi `SendAsync` voi `isHtml=true`. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

---

## F083 - EmailService.SendPasswordResetAsync

| Header | Value |
|---|---|
| Function Code | F083 |
| Function Name | EmailService.SendPasswordResetAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Dung subject/template reset password. | Valid dependencies and data setup for this scenario | UTCID01 |
| Goi `SendAsync` thanh cong. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

---

## F084 - EmailService.SendWelcomeEmailAsync

| Header | Value |
|---|---|
| Function Code | F084 |
| Function Name | EmailService.SendWelcomeEmailAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Dung subject/template welcome. | Valid dependencies and data setup for this scenario | UTCID01 |
| Goi `SendAsync` va ghi log info. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

---

## F085 - EmailService.SendAsync

| Header | Value |
|---|---|
| Function Code | F085 |
| Function Name | EmailService.SendAsync |
| Total Test Cases | 4 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| `to` rong -> `ArgumentException`. | Valid dependencies and data setup for this scenario | UTCID01 |
| `subject` hoac `body` rong -> `ArgumentException`. | Valid dependencies and data setup for this scenario | UTCID02 |
| SMTP connect/auth/send/disconnect thanh cong -> debug log. | Valid dependencies and data setup for this scenario | UTCID03 |
| SMTP fail -> log error va rethrow. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | A |  |  |  |

---

## F086 - GoogleMeetService.CreateMeetingAsync

| Header | Value |
|---|---|
| Function Code | F086 |
| Function Name | GoogleMeetService.CreateMeetingAsync |
| Total Test Cases | 5 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Tao event co meet link ngay -> return `MeetingInfo`. | Valid dependencies and data setup for this scenario | UTCID01 |
| Khong co link ngay lan dau -> retry va lay duoc link. | Valid dependencies and data setup for this scenario | UTCID02 |
| Retry het van khong co link -> cleanup orphan event + throw. | Valid dependencies and data setup for this scenario | UTCID03 |
| Co attendeeEmails -> map attendees vao event. | Valid dependencies and data setup for this scenario | UTCID04 |
| `durationMinutes` null -> dung default duration setting. | Valid dependencies and data setup for this scenario | UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | A |  |  |  |
| UTCID04 | N |  |  |  |
| UTCID05 | B |  |  |  |

---

## F087 - GoogleMeetService.DeleteMeetingAsync

| Header | Value |
|---|---|
| Function Code | F087 |
| Function Name | GoogleMeetService.DeleteMeetingAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Xoa event thanh cong. | Valid dependencies and data setup for this scenario | UTCID01 |
| API tra 404 not found -> warning, khong throw. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |

---

## F088 - GoogleMeetService.Dispose

| Header | Value |
|---|---|
| Function Code | F088 |
| Function Name | GoogleMeetService.Dispose |
| Total Test Cases | 1 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Goi dispose -> calendar service duoc dispose an toan. | Valid dependencies and data setup for this scenario | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |

---

## F089 - NotificationService.SendAsync (typed)

| Header | Value |
|---|---|
| Function Code | F089 |
| Function Name | NotificationService.SendAsync (typed) |
| Total Test Cases | 4 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Payload object -> serialize camelCase JSON. | Valid dependencies and data setup for this scenario | UTCID01 |
| Persist notification + SaveChanges + broadcast notification DTO. | Valid dependencies and data setup for this scenario | UTCID02 |
| Tinh unread count va broadcast unread count dung. | Valid dependencies and data setup for this scenario | UTCID03 |
| Loi repository/hub -> log error va rethrow. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | A |  |  |  |

---

## F090 - NotificationService.SendAsync (legacy)

| Header | Value |
|---|---|
| Function Code | F090 |
| Function Name | NotificationService.SendAsync (legacy) |
| Total Test Cases | 1 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Overload legacy chuyen dung tham so sang overload typed. | Valid dependencies and data setup for this scenario | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |

---

## F091 - OrganisationOnboardingService.SubmitRequestAsync

| Header | Value |
|---|---|
| Function Code | F091 |
| Function Name | OrganisationOnboardingService.SubmitRequestAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Da ton tai pending request cung email -> Conflict. | Valid dependencies and data setup for this scenario | UTCID01 |
| Tao request moi + save thanh cong. | Valid dependencies and data setup for this scenario | UTCID02 |
| Gui notify admin sau khi save thanh cong. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | A |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |

---

## F092 - OrganisationOnboardingService.GetRequestsAsync

| Header | Value |
|---|---|
| Function Code | F092 |
| Function Name | OrganisationOnboardingService.GetRequestsAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Tra danh sach sort `CreatedAt desc`. | Valid dependencies and data setup for this scenario | UTCID01 |
| Map dto day du (`OrgType`, `Status`, `ApprovedAt`...). | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |

---

## F093 - OrganisationOnboardingService.ApproveRequestAsync

| Header | Value |
|---|---|
| Function Code | F093 |
| Function Name | OrganisationOnboardingService.ApproveRequestAsync |
| Total Test Cases | 6 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| RequestId khong ton tai -> NotFound. | Valid dependencies and data setup for this scenario | UTCID01 |
| Request khong con Pending -> Failure da xu ly. | Valid dependencies and data setup for this scenario | UTCID02 |
| Contact email da ton tai user -> Conflict. | Valid dependencies and data setup for this scenario | UTCID03 |
| Tao org admin + add role + tao organisation + approve request -> Success. | Valid dependencies and data setup for this scenario | UTCID04 |
| Co contract template active -> tao contract va send for signature. | Valid dependencies and data setup for this scenario | UTCID05 |
| Exception bat ky -> rollback transaction + Failure. | Valid dependencies and data setup for this scenario | UTCID06 |

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

## F094 - PatientRoadmapGenerationService.GenerateFromDiagnosisAsync

| Header | Value |
|---|---|
| Function Code | F094 |
| Function Name | PatientRoadmapGenerationService.GenerateFromDiagnosisAsync |
| Total Test Cases | 6 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| `PatientId` rong -> Failure validation. | Valid dependencies and data setup for this scenario | UTCID01 |
| `ScreeningId` rong hoac `AiScreeningRawJson` rong -> Failure validation. | Valid dependencies and data setup for this scenario | UTCID02 |
| Chua config ApiKey -> Failure. | Valid dependencies and data setup for this scenario | UTCID03 |
| AI tra JSON hop le -> parse + normalize + Success. | Valid dependencies and data setup for this scenario | UTCID04 |
| AI tra JSON sai format lien tiep -> retry het va Failure. | Valid dependencies and data setup for this scenario | UTCID05 |
| HTTP timeout/network error qua max retry -> Failure unavailable. | Valid dependencies and data setup for this scenario | UTCID06 |

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

## F095 - PayOSService.CreatePaymentLinkAsync

| Header | Value |
|---|---|
| Function Code | F095 |
| Function Name | PayOSService.CreatePaymentLinkAsync |
| Total Test Cases | 5 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Tao payment link thanh cong, return `checkoutUrl` + `orderCode`. | Valid dependencies and data setup for this scenario | UTCID01 |
| `returnUrl/cancelUrl` rong -> dung default URL tu settings. | Valid dependencies and data setup for this scenario | UTCID02 |
| Description >25 ky tu -> bi truncate dung gioi han. | Valid dependencies and data setup for this scenario | UTCID03 |
| URL output co append query param `orderCode`. | Valid dependencies and data setup for this scenario | UTCID04 |
| SDK tra null hoac empty url -> throw exception. | Valid dependencies and data setup for this scenario | UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | N |  |  |  |
| UTCID05 | A |  |  |  |

---

## F096 - PayOSService.GetPaymentStatusAsync

| Header | Value |
|---|---|
| Function Code | F096 |
| Function Name | PayOSService.GetPaymentStatusAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Query thanh cong -> map `Status/Amount/TxnRef`. | Valid dependencies and data setup for this scenario | UTCID01 |
| PaymentInfo null -> throw "Payment not found". | Valid dependencies and data setup for this scenario | UTCID02 |
| SDK throw -> wrap va throw exception service. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |

---

## F097 - PayOSService.VerifyWebhookSignatureAsync

| Header | Value |
|---|---|
| Function Code | F097 |
| Function Name | PayOSService.VerifyWebhookSignatureAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Flow hien tai -> return true. | Valid dependencies and data setup for this scenario | UTCID01 |
| Exception branch -> return false + log error. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |

---

## F098 - PayOSService.CancelPaymentAsync

| Header | Value |
|---|---|
| Function Code | F098 |
| Function Name | PayOSService.CancelPaymentAsync |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| SDK cancel tra object -> true. | Valid dependencies and data setup for this scenario | UTCID01 |
| SDK tra null -> false. | Valid dependencies and data setup for this scenario | UTCID02 |
| Exception -> false va log error. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |
| UTCID03 | A |  |  |  |

---

## F100 - SupabaseStorageService.SaveFileAsync

| Header | Value |
|---|---|
| Function Code | F100 |
| Function Name | SupabaseStorageService.SaveFileAsync |
| Total Test Cases | 5 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Filename duoc sanitize + path tao dung theo subfolder. | Valid dependencies and data setup for this scenario | UTCID01 |
| Stream rong -> throw `InvalidOperationException`. | Valid dependencies and data setup for this scenario | UTCID02 |
| Upload thanh cong -> tra public URL. | Valid dependencies and data setup for this scenario | UTCID03 |
| SDK tra URL tuong doi -> fallback thanh absolute URL. | Valid dependencies and data setup for this scenario | UTCID04 |
| ContentType map dung theo extension file. | Valid dependencies and data setup for this scenario | UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | B |  |  |  |
| UTCID05 | N |  |  |  |

---

## F101 - SupabaseStorageService.DeleteFile

| Header | Value |
|---|---|
| Function Code | F101 |
| Function Name | SupabaseStorageService.DeleteFile |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Input full URL -> extract path va remove thanh cong -> true. | Valid dependencies and data setup for this scenario | UTCID01 |
| Input relative path hop le -> remove thanh cong -> true. | Valid dependencies and data setup for this scenario | UTCID02 |
| Remove throw exception/path invalid -> false. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | A |  |  |  |

---

## F102 - SupabaseStorageService.FileExists

| Header | Value |
|---|---|
| Function Code | F102 |
| Function Name | SupabaseStorageService.FileExists |
| Total Test Cases | 3 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| HEAD tra success -> true. | Valid dependencies and data setup for this scenario | UTCID01 |
| HEAD tra non-success -> false. | Valid dependencies and data setup for this scenario | UTCID02 |
| Input rong/exception request -> false. | Valid dependencies and data setup for this scenario | UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |
| UTCID03 | A |  |  |  |

---

## F103 - SystemSettingService.GetSettingAsync

| Header | Value |
|---|---|
| Function Code | F103 |
| Function Name | SystemSettingService.GetSettingAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Key ton tai -> tra value dung. | Valid dependencies and data setup for this scenario | UTCID01 |
| Key khong ton tai -> null. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | A |  |  |  |

---

## F104 - SystemSettingService.GetAllSettingsAsync

| Header | Value |
|---|---|
| Function Code | F104 |
| Function Name | SystemSettingService.GetAllSettingsAsync |
| Total Test Cases | 2 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Co du lieu -> dictionary day du key/value. | Valid dependencies and data setup for this scenario | UTCID01 |
| Khong co du lieu -> dictionary rong. | Valid dependencies and data setup for this scenario | UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | N |  |  |  |
| UTCID02 | B |  |  |  |

---

## F105 - SystemSettingService.UpdateSettingsAsync

| Header | Value |
|---|---|
| Function Code | F105 |
| Function Name | SystemSettingService.UpdateSettingsAsync |
| Total Test Cases | 4 |
| Test Requirement | Validate service/function behavior with realistic success, failure, and boundary conditions. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Input null/rong -> no-op. | Valid dependencies and data setup for this scenario | UTCID01 |
| Key da ton tai -> goi `UpdateValue`. | Valid dependencies and data setup for this scenario | UTCID02 |
| Key moi -> add `SystemSetting` moi. | Valid dependencies and data setup for this scenario | UTCID03 |
| Sau update -> `SaveChangesAsync` duoc goi + log info keys. | Valid dependencies and data setup for this scenario | UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|
| UTCID01 | B |  |  |  |
| UTCID02 | N |  |  |  |
| UTCID03 | N |  |  |  |
| UTCID04 | N |  |  |  |

---


