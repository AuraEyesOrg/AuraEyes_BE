# Infrastructure Unit Test - Sheet Layout Ready

Last updated: 27/04/2026
Source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

## Audit summary

- Total function blocks: **96** (`F001`-`F096`, continuous).
- Duplicate function codes: **0**.
- Missing function codes: **0**.
- Layout regenerated from checklist Function Catalog + test-case statistics to remove stale/duplicated blocks.

---

## F001 - AuthService.RegisterPatientAsync

| Header | Value |
|---|---|
| Function Code | F001 |
| Function Name | AuthService.RegisterPatientAsync |
| Total Test Cases | 10 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Register patient account' in AuthService.RegisterPatientAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Register patient account scenarios for `RegisterPatientAsync`. | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. | UTCID01-UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |
| UTCID06 |  |  |  |  |  |  |  |
| UTCID07 |  |  |  |  |  |  |  |
| UTCID08 |  |  |  |  |  |  |  |
| UTCID09 |  |  |  |  |  |  |  |
| UTCID10 |  |  |  |  |  |  |  |

---

## F002 - AuthService.LookupAccountByCitizenIdAsync

| Header | Value |
|---|---|
| Function Code | F002 |
| Function Name | AuthService.LookupAccountByCitizenIdAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Lookup account by citizen id' in AuthService.LookupAccountByCitizenIdAsync, covering exists/not exists and masked email logic. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Lookup account by citizen id scenarios for `LookupAccountByCitizenIdAsync`. | UserManager and repositories are mocked; citizenId input is valid or invalid. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F003 - AuthService.GoogleLoginAsync

| Header | Value |
|---|---|
| Function Code | F003 |
| Function Name | AuthService.GoogleLoginAsync |
| Total Test Cases | 10 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Google login' in AuthService.GoogleLoginAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Google login scenarios for `GoogleLoginAsync`. | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. | UTCID01-UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |
| UTCID06 |  |  |  |  |  |  |  |
| UTCID07 |  |  |  |  |  |  |  |
| UTCID08 |  |  |  |  |  |  |  |
| UTCID09 |  |  |  |  |  |  |  |
| UTCID10 |  |  |  |  |  |  |  |

---

## F004 - AuthService.LoginAsync

| Header | Value |
|---|---|
| Function Code | F004 |
| Function Name | AuthService.LoginAsync |
| Total Test Cases | 12 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Login with password' in AuthService.LoginAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Login with password scenarios for `LoginAsync`. | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. | UTCID01-UTCID12 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |
| UTCID06 |  |  |  |  |  |  |  |
| UTCID07 |  |  |  |  |  |  |  |
| UTCID08 |  |  |  |  |  |  |  |
| UTCID09 |  |  |  |  |  |  |  |
| UTCID10 |  |  |  |  |  |  |  |
| UTCID11 |  |  |  |  |  |  |  |
| UTCID12 |  |  |  |  |  |  |  |

---

## F005 - AuthService.VerifyTwoFactorLoginAsync

| Header | Value |
|---|---|
| Function Code | F005 |
| Function Name | AuthService.VerifyTwoFactorLoginAsync |
| Total Test Cases | 10 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Verify 2FA login' in AuthService.VerifyTwoFactorLoginAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Verify 2FA login scenarios for `VerifyTwoFactorLoginAsync`. | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. | UTCID01-UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |
| UTCID06 |  |  |  |  |  |  |  |
| UTCID07 |  |  |  |  |  |  |  |
| UTCID08 |  |  |  |  |  |  |  |
| UTCID09 |  |  |  |  |  |  |  |
| UTCID10 |  |  |  |  |  |  |  |

---

## F006 - AuthService.RefreshTokenAsync

| Header | Value |
|---|---|
| Function Code | F006 |
| Function Name | AuthService.RefreshTokenAsync |
| Total Test Cases | 10 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Refresh token flow' in AuthService.RefreshTokenAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Refresh token flow scenarios for `RefreshTokenAsync`. | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. | UTCID01-UTCID10 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |
| UTCID06 |  |  |  |  |  |  |  |
| UTCID07 |  |  |  |  |  |  |  |
| UTCID08 |  |  |  |  |  |  |  |
| UTCID09 |  |  |  |  |  |  |  |
| UTCID10 |  |  |  |  |  |  |  |

---

## F007 - AuthService.LogoutAsync

| Header | Value |
|---|---|
| Function Code | F007 |
| Function Name | AuthService.LogoutAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Logout single device' in AuthService.LogoutAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Logout single device scenarios for `LogoutAsync`. | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F008 - AuthService.LogoutAllAsync

| Header | Value |
|---|---|
| Function Code | F008 |
| Function Name | AuthService.LogoutAllAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Logout all devices' in AuthService.LogoutAllAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Logout all devices scenarios for `LogoutAllAsync`. | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F009 - AuthService.ConfirmEmailAsync

| Header | Value |
|---|---|
| Function Code | F009 |
| Function Name | AuthService.ConfirmEmailAsync |
| Total Test Cases | 6 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Confirm email' in AuthService.ConfirmEmailAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Confirm email scenarios for `ConfirmEmailAsync`. | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. | UTCID01-UTCID06 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |
| UTCID06 |  |  |  |  |  |  |  |

---

## F010 - AuthService.ForgotPasswordAsync

| Header | Value |
|---|---|
| Function Code | F010 |
| Function Name | AuthService.ForgotPasswordAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Forgot password' in AuthService.ForgotPasswordAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Forgot password scenarios for `ForgotPasswordAsync`. | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F011 - AuthService.ResetPasswordAsync

| Header | Value |
|---|---|
| Function Code | F011 |
| Function Name | AuthService.ResetPasswordAsync |
| Total Test Cases | 5 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Reset password' in AuthService.ResetPasswordAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Reset password scenarios for `ResetPasswordAsync`. | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |

---

## F012 - AuthService.GetCurrentUserAsync

| Header | Value |
|---|---|
| Function Code | F012 |
| Function Name | AuthService.GetCurrentUserAsync |
| Total Test Cases | 5 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get current user' in AuthService.GetCurrentUserAsync, covering success/failure flow and ClinicStaff sub-role mapping. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get current user scenarios for `GetCurrentUserAsync`. | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |

---

## F013 - AuthService.ResendConfirmationAsync

| Header | Value |
|---|---|
| Function Code | F013 |
| Function Name | AuthService.ResendConfirmationAsync |
| Total Test Cases | 4 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Resend confirmation email' in AuthService.ResendConfirmationAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Resend confirmation email scenarios for `ResendConfirmationAsync`. | UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request DTO and cancellation token are valid. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |

---

## F014 - IdentityService.CheckPasswordAsync

| Header | Value |
|---|---|
| Function Code | F014 |
| Function Name | IdentityService.CheckPasswordAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Check password' in IdentityService.CheckPasswordAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Check password scenarios for `CheckPasswordAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F015 - IdentityService.GetUserByEmailAsync

| Header | Value |
|---|---|
| Function Code | F015 |
| Function Name | IdentityService.GetUserByEmailAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get user by email' in IdentityService.GetUserByEmailAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get user by email scenarios for `GetUserByEmailAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F016 - IdentityService.GetUserByIdAsync

| Header | Value |
|---|---|
| Function Code | F016 |
| Function Name | IdentityService.GetUserByIdAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get user by id' in IdentityService.GetUserByIdAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get user by id scenarios for `GetUserByIdAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F017 - IdentityService.IsEmailConfirmedAsync

| Header | Value |
|---|---|
| Function Code | F017 |
| Function Name | IdentityService.IsEmailConfirmedAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Check email confirmed' in IdentityService.IsEmailConfirmedAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Check email confirmed scenarios for `IsEmailConfirmedAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F018 - IdentityService.IsUserActiveAsync

| Header | Value |
|---|---|
| Function Code | F018 |
| Function Name | IdentityService.IsUserActiveAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Check user active' in IdentityService.IsUserActiveAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Check user active scenarios for `IsUserActiveAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F019 - IdentityService.GenerateEmailConfirmationTokenAsync

| Header | Value |
|---|---|
| Function Code | F019 |
| Function Name | IdentityService.GenerateEmailConfirmationTokenAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Generate email confirmation token' in IdentityService.GenerateEmailConfirmationTokenAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Generate email confirmation token scenarios for `GenerateEmailConfirmationTokenAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F020 - IdentityService.GeneratePasswordResetTokenAsync

| Header | Value |
|---|---|
| Function Code | F020 |
| Function Name | IdentityService.GeneratePasswordResetTokenAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Generate password reset token' in IdentityService.GeneratePasswordResetTokenAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Generate password reset token scenarios for `GeneratePasswordResetTokenAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F021 - IdentityService.GetUserRolesAsync

| Header | Value |
|---|---|
| Function Code | F021 |
| Function Name | IdentityService.GetUserRolesAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get user roles' in IdentityService.GetUserRolesAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get user roles scenarios for `GetUserRolesAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F022 - IdentityService.IsInRoleAsync

| Header | Value |
|---|---|
| Function Code | F022 |
| Function Name | IdentityService.IsInRoleAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Check user in role' in IdentityService.IsInRoleAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Check user in role scenarios for `IsInRoleAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F023 - IdentityService.UpdateLastLoginAsync

| Header | Value |
|---|---|
| Function Code | F023 |
| Function Name | IdentityService.UpdateLastLoginAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Update last login async' in IdentityService.UpdateLastLoginAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Update last login async scenarios for `UpdateLastLoginAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F024 - IdentityService.IsTwoFactorEnabledAsync

| Header | Value |
|---|---|
| Function Code | F024 |
| Function Name | IdentityService.IsTwoFactorEnabledAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Check 2FA enabled' in IdentityService.IsTwoFactorEnabledAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Check 2FA enabled scenarios for `IsTwoFactorEnabledAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F025 - IdentityService.GetAuthenticatorKeyAsync

| Header | Value |
|---|---|
| Function Code | F025 |
| Function Name | IdentityService.GetAuthenticatorKeyAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get authenticator key' in IdentityService.GetAuthenticatorKeyAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get authenticator key scenarios for `GetAuthenticatorKeyAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F026 - IdentityService.GetOrCreateAuthenticatorKeyAsync

| Header | Value |
|---|---|
| Function Code | F026 |
| Function Name | IdentityService.GetOrCreateAuthenticatorKeyAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get or create authenticator key' in IdentityService.GetOrCreateAuthenticatorKeyAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get or create authenticator key scenarios for `GetOrCreateAuthenticatorKeyAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F027 - IdentityService.VerifyTwoFactorCodeAsync

| Header | Value |
|---|---|
| Function Code | F027 |
| Function Name | IdentityService.VerifyTwoFactorCodeAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Verify 2FA code' in IdentityService.VerifyTwoFactorCodeAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Verify 2FA code scenarios for `VerifyTwoFactorCodeAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F028 - IdentityService.GenerateNewRecoveryCodesAsync

| Header | Value |
|---|---|
| Function Code | F028 |
| Function Name | IdentityService.GenerateNewRecoveryCodesAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Generate recovery codes' in IdentityService.GenerateNewRecoveryCodesAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Generate recovery codes scenarios for `GenerateNewRecoveryCodesAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F029 - IdentityService.GetRecoveryCodesCountAsync

| Header | Value |
|---|---|
| Function Code | F029 |
| Function Name | IdentityService.GetRecoveryCodesCountAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get recovery code count' in IdentityService.GetRecoveryCodesCountAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get recovery code count scenarios for `GetRecoveryCodesCountAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F030 - IdentityService.GenerateAuthenticatorUri

| Header | Value |
|---|---|
| Function Code | F030 |
| Function Name | IdentityService.GenerateAuthenticatorUri |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Generate authenticator URI' in IdentityService.GenerateAuthenticatorUri, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Generate authenticator URI scenarios for `GenerateAuthenticatorUri`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F031 - IdentityService.FormatAuthenticatorKey

| Header | Value |
|---|---|
| Function Code | F031 |
| Function Name | IdentityService.FormatAuthenticatorKey |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Format authenticator key' in IdentityService.FormatAuthenticatorKey, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Format authenticator key scenarios for `FormatAuthenticatorKey`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F032 - IdentityService.GetUserMetricsAsync

| Header | Value |
|---|---|
| Function Code | F032 |
| Function Name | IdentityService.GetUserMetricsAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get user metrics' in IdentityService.GetUserMetricsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get user metrics scenarios for `GetUserMetricsAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F033 - IdentityService.GetUsersInRoleCountAsync

| Header | Value |
|---|---|
| Function Code | F033 |
| Function Name | IdentityService.GetUsersInRoleCountAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get users in role count' in IdentityService.GetUsersInRoleCountAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get users in role count scenarios for `GetUsersInRoleCountAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F034 - IdentityService.GetPendingApprovalsCountAsync

| Header | Value |
|---|---|
| Function Code | F034 |
| Function Name | IdentityService.GetPendingApprovalsCountAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get pending approvals count' in IdentityService.GetPendingApprovalsCountAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get pending approvals count scenarios for `GetPendingApprovalsCountAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F035 - IdentityService.GetUserDetailsAsync

| Header | Value |
|---|---|
| Function Code | F035 |
| Function Name | IdentityService.GetUserDetailsAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get user details' in IdentityService.GetUserDetailsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get user details scenarios for `GetUserDetailsAsync`. | UserManager and RoleManager are mocked with user/role seed data matching each scenario. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F036 - RefreshTokenService.CreateRefreshTokenAsync

| Header | Value |
|---|---|
| Function Code | F036 |
| Function Name | RefreshTokenService.CreateRefreshTokenAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Create refresh token record' in RefreshTokenService.CreateRefreshTokenAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Create refresh token record scenarios for `CreateRefreshTokenAsync`. | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F037 - RefreshTokenService.GetByTokenHashAsync

| Header | Value |
|---|---|
| Function Code | F037 |
| Function Name | RefreshTokenService.GetByTokenHashAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get refresh token by hash' in RefreshTokenService.GetByTokenHashAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get refresh token by hash scenarios for `GetByTokenHashAsync`. | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F038 - RefreshTokenService.RotateRefreshTokenAsync

| Header | Value |
|---|---|
| Function Code | F038 |
| Function Name | RefreshTokenService.RotateRefreshTokenAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Rotate refresh token' in RefreshTokenService.RotateRefreshTokenAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Rotate refresh token scenarios for `RotateRefreshTokenAsync`. | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F039 - RefreshTokenService.RevokeTokenAsync

| Header | Value |
|---|---|
| Function Code | F039 |
| Function Name | RefreshTokenService.RevokeTokenAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Revoke token by hash' in RefreshTokenService.RevokeTokenAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Revoke token by hash scenarios for `RevokeTokenAsync`. | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F040 - RefreshTokenService.RevokeAllUserTokensAsync

| Header | Value |
|---|---|
| Function Code | F040 |
| Function Name | RefreshTokenService.RevokeAllUserTokensAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Revoke all user tokens' in RefreshTokenService.RevokeAllUserTokensAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Revoke all user tokens scenarios for `RevokeAllUserTokensAsync`. | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F041 - RefreshTokenService.RevokeTokenFamilyAsync

| Header | Value |
|---|---|
| Function Code | F041 |
| Function Name | RefreshTokenService.RevokeTokenFamilyAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Revoke token family' in RefreshTokenService.RevokeTokenFamilyAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Revoke token family scenarios for `RevokeTokenFamilyAsync`. | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F042 - RefreshTokenService.CleanupExpiredTokensAsync

| Header | Value |
|---|---|
| Function Code | F042 |
| Function Name | RefreshTokenService.CleanupExpiredTokensAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Cleanup expired tokens' in RefreshTokenService.CleanupExpiredTokensAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Cleanup expired tokens scenarios for `CleanupExpiredTokensAsync`. | Refresh-token repository/DbContext is seeded; token hash, jwtId, and userId are valid; persistence context is ready. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F043 - TokenService.GenerateAccessTokenAsync

| Header | Value |
|---|---|
| Function Code | F043 |
| Function Name | TokenService.GenerateAccessTokenAsync |
| Total Test Cases | 4 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Generate access token' in TokenService.GenerateAccessTokenAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Generate access token scenarios for `GenerateAccessTokenAsync`. | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |

---

## F044 - TokenService.GenerateRefreshToken

| Header | Value |
|---|---|
| Function Code | F044 |
| Function Name | TokenService.GenerateRefreshToken |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Generate refresh token string' in TokenService.GenerateRefreshToken, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Generate refresh token string scenarios for `GenerateRefreshToken`. | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F045 - TokenService.ValidateToken

| Header | Value |
|---|---|
| Function Code | F045 |
| Function Name | TokenService.ValidateToken |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Validate token' in TokenService.ValidateToken, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Validate token scenarios for `ValidateToken`. | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F046 - TokenService.GetUserIdFromToken

| Header | Value |
|---|---|
| Function Code | F046 |
| Function Name | TokenService.GetUserIdFromToken |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get user id from token' in TokenService.GetUserIdFromToken, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get user id from token scenarios for `GetUserIdFromToken`. | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F047 - TokenService.GetJtiFromToken

| Header | Value |
|---|---|
| Function Code | F047 |
| Function Name | TokenService.GetJtiFromToken |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get jti from token' in TokenService.GetJtiFromToken, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get jti from token scenarios for `GetJtiFromToken`. | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F048 - TokenService.HashToken

| Header | Value |
|---|---|
| Function Code | F048 |
| Function Name | TokenService.HashToken |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Hash token' in TokenService.HashToken, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Hash token scenarios for `HashToken`. | JWT settings (secret, issuer, audience) are valid; token/claims inputs are prepared for valid and invalid paths. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F049 - AdminQueryService.GetOphthalmologistsAsync

| Header | Value |
|---|---|
| Function Code | F049 |
| Function Name | AdminQueryService.GetOphthalmologistsAsync |
| Total Test Cases | 5 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get ophthalmologists query' in AdminQueryService.GetOphthalmologistsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get ophthalmologists query scenarios for `GetOphthalmologistsAsync`. | ApplicationDbContext is seeded; filter, sort, and paging inputs are valid. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |

---

## F050 - AdminQueryService.GetPatientsAsync

| Header | Value |
|---|---|
| Function Code | F050 |
| Function Name | AdminQueryService.GetPatientsAsync |
| Total Test Cases | 4 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get patients query' in AdminQueryService.GetPatientsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get patients query scenarios for `GetPatientsAsync`. | ApplicationDbContext is seeded; filter, sort, and paging inputs are valid. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |

---

## F051 - AdminQueryService.GetAuditLogsAsync

| Header | Value |
|---|---|
| Function Code | F051 |
| Function Name | AdminQueryService.GetAuditLogsAsync |
| Total Test Cases | 5 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get audit logs query' in AdminQueryService.GetAuditLogsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get audit logs query scenarios for `GetAuditLogsAsync`. | ApplicationDbContext is seeded; filter, sort, and paging inputs are valid. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |

---

## F052 - AiQuotaService.GetQuotaAsync

| Header | Value |
|---|---|
| Function Code | F052 |
| Function Name | AiQuotaService.GetQuotaAsync |
| Total Test Cases | 5 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get AI quota' in AiQuotaService.GetQuotaAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get AI quota scenarios for `GetQuotaAsync`. | ApplicationDbContext and system-setting dependency are seeded with quota data; user context is valid. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |

---

## F053 - AiQuotaService.HasAvailableQuotaAsync

| Header | Value |
|---|---|
| Function Code | F053 |
| Function Name | AiQuotaService.HasAvailableQuotaAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Check AI quota available' in AiQuotaService.HasAvailableQuotaAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Check AI quota available scenarios for `HasAvailableQuotaAsync`. | ApplicationDbContext and system-setting dependency are seeded with quota data; user context is valid. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F054 - AiQuotaService.DeductQuotaAsync

| Header | Value |
|---|---|
| Function Code | F054 |
| Function Name | AiQuotaService.DeductQuotaAsync |
| Total Test Cases | 4 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Deduct AI quota' in AiQuotaService.DeductQuotaAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Deduct AI quota scenarios for `DeductQuotaAsync`. | ApplicationDbContext and system-setting dependency are seeded with quota data; user context is valid. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |

---

## F055 - AiQuotaService.AddPurchasedQuotaAsync

| Header | Value |
|---|---|
| Function Code | F055 |
| Function Name | AiQuotaService.AddPurchasedQuotaAsync |
| Total Test Cases | 4 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Add purchased AI quota' in AiQuotaService.AddPurchasedQuotaAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Add purchased AI quota scenarios for `AddPurchasedQuotaAsync`. | ApplicationDbContext and system-setting dependency are seeded with quota data; user context is valid. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |

---

## F056 - BetterStackHeartbeatService.GetEmbedUrl

| Header | Value |
|---|---|
| Function Code | F056 |
| Function Name | BetterStackHeartbeatService.GetEmbedUrl |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get BetterStack embed URL' in BetterStackHeartbeatService.GetEmbedUrl, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get BetterStack embed URL scenarios for `GetEmbedUrl`. | Heartbeat/monitor configuration is valid; client and logger are initialized. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F057 - BetterStackHeartbeatService.GetMonitorDescriptors

| Header | Value |
|---|---|
| Function Code | F057 |
| Function Name | BetterStackHeartbeatService.GetMonitorDescriptors |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get BetterStack monitor descriptors' in BetterStackHeartbeatService.GetMonitorDescriptors, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get BetterStack monitor descriptors scenarios for `GetMonitorDescriptors`. | Heartbeat/monitor configuration is valid; client and logger are initialized. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F058 - BetterStackHeartbeatService.NotifyStartedAsync

| Header | Value |
|---|---|
| Function Code | F058 |
| Function Name | BetterStackHeartbeatService.NotifyStartedAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Notify monitor started' in BetterStackHeartbeatService.NotifyStartedAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Notify monitor started scenarios for `NotifyStartedAsync`. | Heartbeat/monitor configuration is valid; client and logger are initialized. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F059 - BetterStackHeartbeatService.NotifySucceededAsync

| Header | Value |
|---|---|
| Function Code | F059 |
| Function Name | BetterStackHeartbeatService.NotifySucceededAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Notify monitor succeeded' in BetterStackHeartbeatService.NotifySucceededAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Notify monitor succeeded scenarios for `NotifySucceededAsync`. | Heartbeat/monitor configuration is valid; client and logger are initialized. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F060 - BetterStackHeartbeatService.NotifyFailedAsync

| Header | Value |
|---|---|
| Function Code | F060 |
| Function Name | BetterStackHeartbeatService.NotifyFailedAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Notify monitor failed' in BetterStackHeartbeatService.NotifyFailedAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Notify monitor failed scenarios for `NotifyFailedAsync`. | Heartbeat/monitor configuration is valid; client and logger are initialized. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F061 - DashboardMetricsService.GetSystemAdminMetricsAsync

| Header | Value |
|---|---|
| Function Code | F061 |
| Function Name | DashboardMetricsService.GetSystemAdminMetricsAsync |
| Total Test Cases | 6 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get system admin metrics' in DashboardMetricsService.GetSystemAdminMetricsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get system admin metrics scenarios for `GetSystemAdminMetricsAsync`. | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid. | UTCID01-UTCID06 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |
| UTCID06 |  |  |  |  |  |  |  |

---

## F062 - DashboardMetricsService.GetRecentScreeningsAsync

| Header | Value |
|---|---|
| Function Code | F062 |
| Function Name | DashboardMetricsService.GetRecentScreeningsAsync |
| Total Test Cases | 4 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get recent screenings' in DashboardMetricsService.GetRecentScreeningsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get recent screenings scenarios for `GetRecentScreeningsAsync`. | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |

---

## F063 - DashboardMetricsService.GetScreeningVolumeTrendsAsync

| Header | Value |
|---|---|
| Function Code | F063 |
| Function Name | DashboardMetricsService.GetScreeningVolumeTrendsAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get screening volume trends' in DashboardMetricsService.GetScreeningVolumeTrendsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get screening volume trends scenarios for `GetScreeningVolumeTrendsAsync`. | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F064 - DashboardMetricsService.GetPopulationRiskAnalysisAsync

| Header | Value |
|---|---|
| Function Code | F064 |
| Function Name | DashboardMetricsService.GetPopulationRiskAnalysisAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get population risk analysis' in DashboardMetricsService.GetPopulationRiskAnalysisAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get population risk analysis scenarios for `GetPopulationRiskAnalysisAsync`. | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F065 - DashboardMetricsService.GetSystemHealthAsync

| Header | Value |
|---|---|
| Function Code | F065 |
| Function Name | DashboardMetricsService.GetSystemHealthAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get system health' in DashboardMetricsService.GetSystemHealthAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get system health scenarios for `GetSystemHealthAsync`. | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F066 - DashboardMetricsService.GetOphthalmologistMetricsAsync

| Header | Value |
|---|---|
| Function Code | F066 |
| Function Name | DashboardMetricsService.GetOphthalmologistMetricsAsync |
| Total Test Cases | 4 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get ophthalmologist metrics' in DashboardMetricsService.GetOphthalmologistMetricsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get ophthalmologist metrics scenarios for `GetOphthalmologistMetricsAsync`. | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |

---

## F067 - DashboardMetricsService.GetPatientMetricsAsync

| Header | Value |
|---|---|
| Function Code | F067 |
| Function Name | DashboardMetricsService.GetPatientMetricsAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get patient metrics' in DashboardMetricsService.GetPatientMetricsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get patient metrics scenarios for `GetPatientMetricsAsync`. | ApplicationDbContext is seeded with metrics data; time-range and paging inputs are valid. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F068 - DateTimeService.Now (property)

| Header | Value |
|---|---|
| Function Code | F068 |
| Function Name | DateTimeService.Now (property) |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get local now' in DateTimeService.Now (property), covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get local now scenarios for `Now (property)`. | Service is instantiated directly; no external dependency is required. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F069 - DateTimeService.UtcNow (property)

| Header | Value |
|---|---|
| Function Code | F069 |
| Function Name | DateTimeService.UtcNow (property) |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get UTC now' in DateTimeService.UtcNow (property), covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get UTC now scenarios for `UtcNow (property)`. | Service is instantiated directly; no external dependency is required. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F070 - EmailService.SendEmailConfirmationAsync

| Header | Value |
|---|---|
| Function Code | F070 |
| Function Name | EmailService.SendEmailConfirmationAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Send email confirmation' in EmailService.SendEmailConfirmationAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Send email confirmation scenarios for `SendEmailConfirmationAsync`. | SMTP settings are valid; recipient and template/content inputs are valid; transport can be mocked. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F071 - EmailService.SendPasswordResetAsync

| Header | Value |
|---|---|
| Function Code | F071 |
| Function Name | EmailService.SendPasswordResetAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Send password reset email' in EmailService.SendPasswordResetAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Send password reset email scenarios for `SendPasswordResetAsync`. | SMTP settings are valid; recipient and template/content inputs are valid; transport can be mocked. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F072 - EmailService.SendWelcomeEmailAsync

| Header | Value |
|---|---|
| Function Code | F072 |
| Function Name | EmailService.SendWelcomeEmailAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Send welcome email' in EmailService.SendWelcomeEmailAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Send welcome email scenarios for `SendWelcomeEmailAsync`. | SMTP settings are valid; recipient and template/content inputs are valid; transport can be mocked. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F073 - EmailService.SendAsync

| Header | Value |
|---|---|
| Function Code | F073 |
| Function Name | EmailService.SendAsync |
| Total Test Cases | 4 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Send generic email' in EmailService.SendAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Send generic email scenarios for `SendAsync`. | SMTP settings are valid; recipient and template/content inputs are valid; transport can be mocked. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |

---

## F074 - GoogleMeetService.CreateMeetingAsync

| Header | Value |
|---|---|
| Function Code | F074 |
| Function Name | GoogleMeetService.CreateMeetingAsync |
| Total Test Cases | 5 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Create Google Meet meeting' in GoogleMeetService.CreateMeetingAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Create Google Meet meeting scenarios for `CreateMeetingAsync`. | Google credentials and service configuration are valid; meeting request inputs are valid. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |

---

## F075 - GoogleMeetService.DeleteMeetingAsync

| Header | Value |
|---|---|
| Function Code | F075 |
| Function Name | GoogleMeetService.DeleteMeetingAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Delete Google Meet meeting' in GoogleMeetService.DeleteMeetingAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Delete Google Meet meeting scenarios for `DeleteMeetingAsync`. | Google credentials and service configuration are valid; meeting request inputs are valid. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F076 - GoogleMeetService.Dispose

| Header | Value |
|---|---|
| Function Code | F076 |
| Function Name | GoogleMeetService.Dispose |
| Total Test Cases | 1 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Dispose Google Meet service' in GoogleMeetService.Dispose, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Dispose Google Meet service scenarios for `Dispose`. | Google credentials and service configuration are valid; meeting request inputs are valid. | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |

---

## F077 - NotificationService.SendAsync (typed)

| Header | Value |
|---|---|
| Function Code | F077 |
| Function Name | NotificationService.SendAsync (typed) |
| Total Test Cases | 4 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Send typed notification' in NotificationService.SendAsync (typed), covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Send typed notification scenarios for `SendAsync (typed)`. | Notification repository, UnitOfWork, and hub service are mocked; user/message/type payload is prepared. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |

---

## F078 - NotificationService.SendAsync (legacy)

| Header | Value |
|---|---|
| Function Code | F078 |
| Function Name | NotificationService.SendAsync (legacy) |
| Total Test Cases | 1 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Send legacy notification' in NotificationService.SendAsync (legacy), covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Send legacy notification scenarios for `SendAsync (legacy)`. | Notification repository, UnitOfWork, and hub service are mocked; user/message/type payload is prepared. | UTCID01 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |

---

## F079 - PatientRoadmapGenerationService.GenerateFromDiagnosisAsync

| Header | Value |
|---|---|
| Function Code | F079 |
| Function Name | PatientRoadmapGenerationService.GenerateFromDiagnosisAsync |
| Total Test Cases | 6 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Generate roadmap from diagnosis' in PatientRoadmapGenerationService.GenerateFromDiagnosisAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Generate roadmap from diagnosis scenarios for `GenerateFromDiagnosisAsync`. | Diagnosis input and roadmap rules/mapping are prepared. | UTCID01-UTCID06 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |
| UTCID06 |  |  |  |  |  |  |  |

---

## F080 - PayOSService.CreatePaymentLinkAsync

| Header | Value |
|---|---|
| Function Code | F080 |
| Function Name | PayOSService.CreatePaymentLinkAsync |
| Total Test Cases | 5 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Create PayOS payment link' in PayOSService.CreatePaymentLinkAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Create PayOS payment link scenarios for `CreatePaymentLinkAsync`. | PayOS settings are valid (clientId, apiKey, checksumKey); payment payload inputs are valid. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |

---

## F081 - PayOSService.GetPaymentStatusAsync

| Header | Value |
|---|---|
| Function Code | F081 |
| Function Name | PayOSService.GetPaymentStatusAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get PayOS payment status' in PayOSService.GetPaymentStatusAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get PayOS payment status scenarios for `GetPaymentStatusAsync`. | PayOS settings are valid (clientId, apiKey, checksumKey); payment payload inputs are valid. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F082 - PayOSService.VerifyWebhookSignatureAsync

| Header | Value |
|---|---|
| Function Code | F082 |
| Function Name | PayOSService.VerifyWebhookSignatureAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Verify PayOS webhook signature' in PayOSService.VerifyWebhookSignatureAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Verify PayOS webhook signature scenarios for `VerifyWebhookSignatureAsync`. | PayOS settings are valid (clientId, apiKey, checksumKey); payment payload inputs are valid. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F083 - PayOSService.CancelPaymentAsync

| Header | Value |
|---|---|
| Function Code | F083 |
| Function Name | PayOSService.CancelPaymentAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Cancel PayOS payment' in PayOSService.CancelPaymentAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Cancel PayOS payment scenarios for `CancelPaymentAsync`. | PayOS settings are valid (clientId, apiKey, checksumKey); payment payload inputs are valid. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F084 - SupabaseStorageService.SaveFileAsync

| Header | Value |
|---|---|
| Function Code | F084 |
| Function Name | SupabaseStorageService.SaveFileAsync |
| Total Test Cases | 5 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Save file to Supabase' in SupabaseStorageService.SaveFileAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Save file to Supabase scenarios for `SaveFileAsync`. | Supabase storage settings are valid; stream/path inputs are valid; network calls can be mocked. | UTCID01-UTCID05 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |
| UTCID05 |  |  |  |  |  |  |  |

---

## F085 - SupabaseStorageService.DeleteFile

| Header | Value |
|---|---|
| Function Code | F085 |
| Function Name | SupabaseStorageService.DeleteFile |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Delete file from Supabase' in SupabaseStorageService.DeleteFile, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Delete file from Supabase scenarios for `DeleteFile`. | Supabase storage settings are valid; stream/path inputs are valid; network calls can be mocked. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F086 - SupabaseStorageService.FileExists

| Header | Value |
|---|---|
| Function Code | F086 |
| Function Name | SupabaseStorageService.FileExists |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Check Supabase file exists' in SupabaseStorageService.FileExists, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Check Supabase file exists scenarios for `FileExists`. | Supabase storage settings are valid; stream/path inputs are valid; network calls can be mocked. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F087 - SystemSettingService.GetSettingAsync

| Header | Value |
|---|---|
| Function Code | F087 |
| Function Name | SystemSettingService.GetSettingAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get setting by key' in SystemSettingService.GetSettingAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get setting by key scenarios for `GetSettingAsync`. | System-setting repository and UnitOfWork are seeded with valid key/value data. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F088 - SystemSettingService.GetAllSettingsAsync

| Header | Value |
|---|---|
| Function Code | F088 |
| Function Name | SystemSettingService.GetAllSettingsAsync |
| Total Test Cases | 2 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get all settings' in SystemSettingService.GetAllSettingsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get all settings scenarios for `GetAllSettingsAsync`. | System-setting repository and UnitOfWork are seeded with valid key/value data. | UTCID01-UTCID02 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |

---

## F089 - SystemSettingService.UpdateSettingsAsync

| Header | Value |
|---|---|
| Function Code | F089 |
| Function Name | SystemSettingService.UpdateSettingsAsync |
| Total Test Cases | 4 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Update settings' in SystemSettingService.UpdateSettingsAsync, covering success/failure flow, response contract, and logging behavior. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Update settings scenarios for `UpdateSettingsAsync`. | System-setting repository and UnitOfWork are seeded with valid key/value data. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |

---

## F090 - ClinicVisitService.ProcessPaymentCompletionAsync

| Header | Value |
|---|---|
| Function Code | F090 |
| Function Name | ClinicVisitService.ProcessPaymentCompletionAsync |
| Total Test Cases | 4 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Process payment completion' in ClinicVisitService.ProcessPaymentCompletionAsync, covering appointment completion and session creation. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Process payment completion scenarios for `ProcessPaymentCompletionAsync`. | Repositories and UnitOfWork are mocked; order and payment data are valid. | UTCID01-UTCID04 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |
| UTCID04 |  |  |  |  |  |  |  |

---

## F091 - PayOSPayoutService.CreatePayoutAsync

| Header | Value |
|---|---|
| Function Code | F091 |
| Function Name | PayOSPayoutService.CreatePayoutAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Create PayOS payout' in PayOSPayoutService.CreatePayoutAsync, covering request validation, signature generation, and API call. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Create PayOS payout scenarios for `CreatePayoutAsync`. | PayOS settings and HTTP client are prepared; request payload is valid. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F092 - PayOSPayoutService.GetPayoutAsync

| Header | Value |
|---|---|
| Function Code | F092 |
| Function Name | PayOSPayoutService.GetPayoutAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get PayOS payout details' in PayOSPayoutService.GetPayoutAsync, covering response mapping and error handling. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get PayOS payout details scenarios for `GetPayoutAsync`. | PayOS settings and HTTP client are prepared; payout ID is valid. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F093 - PayOSPayoutService.GetPayoutsAsync

| Header | Value |
|---|---|
| Function Code | F093 |
| Function Name | PayOSPayoutService.GetPayoutsAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get all PayOS payouts' in PayOSPayoutService.GetPayoutsAsync, covering pagination and list mapping. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get all PayOS payouts scenarios for `GetPayoutsAsync`. | PayOS settings and HTTP client are prepared. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F094 - PayOSPayoutService.EstimateCreditAsync

| Header | Value |
|---|---|
| Function Code | F094 |
| Function Name | PayOSPayoutService.EstimateCreditAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Estimate payout credit' in PayOSPayoutService.EstimateCreditAsync, covering calculation logic. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Estimate payout credit scenarios for `EstimateCreditAsync`. | PayOS settings and HTTP client are prepared; amount is valid. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F095 - PayOSPayoutService.GetPayoutAccountBalanceAsync

| Header | Value |
|---|---|
| Function Code | F095 |
| Function Name | PayOSPayoutService.GetPayoutAccountBalanceAsync |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Get payout account balance' in PayOSPayoutService.GetPayoutAccountBalanceAsync, covering response mapping. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Get payout account balance scenarios for `GetPayoutAccountBalanceAsync`. | PayOS settings and HTTP client are prepared. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

## F096 - PatientScreeningPdfService.GenerateScreeningReportPdf

| Header | Value |
|---|---|
| Function Code | F096 |
| Function Name | PatientScreeningPdfService.GenerateScreeningReportPdf |
| Total Test Cases | 3 |
| Created By |  |
| Executed By |  |
| Lines of Code |  |
| Passed |  |
| Failed |  |
| Untested |  |
| Count type N |  |
| Count type A |  |
| Count type B |  |
| Test Requirement | Validate 'Generate patient screening PDF' in PatientScreeningPdfService.GenerateScreeningReportPdf, covering PDF layout and data population. |

### Condition Matrix

| Condition | Precondition | UTCIDs |
|---|---|---|
| Validate Generate patient screening PDF scenarios for `GenerateScreeningReportPdf`. | QuestPDF library is initialized; screening data is valid. | UTCID01-UTCID03 |

### Result Matrix

| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |
|---|---|---|---|---|---|---|---|
| UTCID01 |  |  |  |  |  |  |  |
| UTCID02 |  |  |  |  |  |  |  |
| UTCID03 |  |  |  |  |  |  |  |

---

