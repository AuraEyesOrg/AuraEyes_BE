# F054 - TokenService.GenerateRefreshToken

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F054 |
| Function Name | GenerateRefreshToken |
| Class Name | TokenService |
| Method | GenerateRefreshToken |
| Requirement | Generate refresh token string |
| Description | Validate 'Generate refresh token string' in TokenService.GenerateRefreshToken, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: JWT settings and token inputs are valid for both success and failure paths.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC054-01 |
| Method Should Exist | UTC054-02 |
| Source Should Contain Method Declaration | UTC054-03 |
| Return Case Set Should Be Valid | UTC054-04 |
| Log Message Case Set Should Be Valid | UTC054-05 |
| When Logger Used Should Follow Log Message Convention | UTC054-06 |
| When Result Response Used Should Follow Response Convention | UTC054-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Convert.ToBase64String(randomBytes)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC054-01 | N | Type Should Exist |
| UTC054-02 | N | Method Should Exist |
| UTC054-03 | N | Source Should Contain Method Declaration |
| UTC054-04 | N | Return Case Set Should Be Valid |
| UTC054-05 | N | Log Message Case Set Should Be Valid |
| UTC054-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC054-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F054_TokenService_GenerateRefreshToken_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



