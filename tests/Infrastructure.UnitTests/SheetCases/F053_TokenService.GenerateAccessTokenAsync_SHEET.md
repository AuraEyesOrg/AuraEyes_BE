# F053 - TokenService.GenerateAccessTokenAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F053 |
| Function Name | GenerateAccessTokenAsync |
| Class Name | TokenService |
| Method | GenerateAccessTokenAsync |
| Requirement | Generate access token |
| Description | Validate 'Generate access token' in TokenService.GenerateAccessTokenAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: JWT settings and token inputs are valid for both success and failure paths.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC053-01 |
| Method Should Exist | UTC053-02 |
| Source Should Contain Method Declaration | UTC053-03 |
| Return Case Set Should Be Valid | UTC053-04 |
| Log Message Case Set Should Be Valid | UTC053-05 |
| When Logger Used Should Follow Log Message Convention | UTC053-06 |
| When Result Response Used Should Follow Response Convention | UTC053-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Task.FromResult(new TokenResult(accessToken, jti, expiresAt))

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC053-01 | N | Type Should Exist |
| UTC053-02 | N | Method Should Exist |
| UTC053-03 | N | Source Should Contain Method Declaration |
| UTC053-04 | N | Return Case Set Should Be Valid |
| UTC053-05 | N | Log Message Case Set Should Be Valid |
| UTC053-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC053-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F053_TokenService_GenerateAccessTokenAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



