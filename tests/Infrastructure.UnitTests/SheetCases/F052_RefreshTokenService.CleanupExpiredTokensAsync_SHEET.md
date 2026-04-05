# F052 - RefreshTokenService.CleanupExpiredTokensAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F052 |
| Function Name | CleanupExpiredTokensAsync |
| Class Name | RefreshTokenService |
| Method | CleanupExpiredTokensAsync |
| Requirement | Cleanup expired tokens |
| Description | Validate 'Cleanup expired tokens' in RefreshTokenService.CleanupExpiredTokensAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: Refresh-token repository or DbContext is seeded; token hash, jwtId, and userId are valid.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC052-01 |
| Method Should Exist | UTC052-02 |
| Source Should Contain Method Declaration | UTC052-03 |
| Return Case Set Should Be Valid | UTC052-04 |
| Log Message Case Set Should Be Valid | UTC052-05 |
| When Logger Used Should Follow Log Message Convention | UTC052-06 |
| When Result Response Used Should Follow Response Convention | UTC052-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- tokensToDelete.Count

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC052-01 | N | Type Should Exist |
| UTC052-02 | N | Method Should Exist |
| UTC052-03 | N | Source Should Contain Method Declaration |
| UTC052-04 | N | Return Case Set Should Be Valid |
| UTC052-05 | N | Log Message Case Set Should Be Valid |
| UTC052-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC052-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F052_RefreshTokenService_CleanupExpiredTokensAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



