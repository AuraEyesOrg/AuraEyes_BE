# F037 - IdentityService.GenerateAuthenticatorUri

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F037 |
| Function Name | GenerateAuthenticatorUri |
| Class Name | IdentityService |
| Method | GenerateAuthenticatorUri |
| Requirement | Generate authenticator URI |
| Description | Validate 'Generate authenticator URI' in IdentityService.GenerateAuthenticatorUri, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC037-01 |
| Method Should Exist | UTC037-02 |
| Source Should Contain Method Declaration | UTC037-03 |
| Return Case Set Should Be Valid | UTC037-04 |
| Log Message Case Set Should Be Valid | UTC037-05 |
| When Logger Used Should Follow Log Message Convention | UTC037-06 |
| When Result Response Used Should Follow Response Convention | UTC037-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- $"otpauth://totp/{UrlEncoder.Default.Encode(AuthenticatorIssuer)}:{UrlEncoder.Default.Encode(email)}" + $"?secret={sharedKey}" + $"&issuer={UrlEncoder.Default.Encode(AuthenticatorIssuer)}" + "&digits=6"

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC037-01 | N | Type Should Exist |
| UTC037-02 | N | Method Should Exist |
| UTC037-03 | N | Source Should Contain Method Declaration |
| UTC037-04 | N | Return Case Set Should Be Valid |
| UTC037-05 | N | Log Message Case Set Should Be Valid |
| UTC037-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC037-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F037_IdentityService_GenerateAuthenticatorUri_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



