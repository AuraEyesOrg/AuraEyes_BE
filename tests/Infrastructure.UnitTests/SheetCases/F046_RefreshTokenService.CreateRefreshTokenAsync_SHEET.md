# F046 - RefreshTokenService.CreateRefreshTokenAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F046 |
| Function Name | CreateRefreshTokenAsync |
| Class Name | RefreshTokenService |
| Method | CreateRefreshTokenAsync |
| Requirement | Create refresh token record |
| Description | Validate 'Create refresh token record' in RefreshTokenService.CreateRefreshTokenAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: Refresh-token repository or DbContext is seeded; token hash, jwtId, and userId are valid.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC046-01 |
| Method Should Exist | UTC046-02 |
| Source Should Contain Method Declaration | UTC046-03 |
| Return Case Set Should Be Valid | UTC046-04 |
| Log Message Case Set Should Be Valid | UTC046-05 |
| When Logger Used Should Follow Log Message Convention | UTC046-06 |
| When Result Response Used Should Follow Response Convention | UTC046-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- refreshToken.Id

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC046-01 | N | Type Should Exist |
| UTC046-02 | N | Method Should Exist |
| UTC046-03 | N | Source Should Contain Method Declaration |
| UTC046-04 | N | Return Case Set Should Be Valid |
| UTC046-05 | N | Log Message Case Set Should Be Valid |
| UTC046-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC046-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F046_RefreshTokenService_CreateRefreshTokenAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



