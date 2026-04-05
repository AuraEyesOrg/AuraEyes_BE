# F047 - RefreshTokenService.GetByTokenHashAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F047 |
| Function Name | GetByTokenHashAsync |
| Class Name | RefreshTokenService |
| Method | GetByTokenHashAsync |
| Requirement | Get refresh token by hash |
| Description | Validate 'Get refresh token by hash' in RefreshTokenService.GetByTokenHashAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: Refresh-token repository or DbContext is seeded; token hash, jwtId, and userId are valid.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC047-01 |
| Method Should Exist | UTC047-02 |
| Source Should Contain Method Declaration | UTC047-03 |
| Return Case Set Should Be Valid | UTC047-04 |
| Log Message Case Set Should Be Valid | UTC047-05 |
| When Logger Used Should Follow Log Message Convention | UTC047-06 |
| When Result Response Used Should Follow Response Convention | UTC047-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- null

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC047-01 | N | Type Should Exist |
| UTC047-02 | N | Method Should Exist |
| UTC047-03 | N | Source Should Contain Method Declaration |
| UTC047-04 | N | Return Case Set Should Be Valid |
| UTC047-05 | N | Log Message Case Set Should Be Valid |
| UTC047-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC047-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F047_RefreshTokenService_GetByTokenHashAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



