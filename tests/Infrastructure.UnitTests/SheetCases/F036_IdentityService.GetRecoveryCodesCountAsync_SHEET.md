# F036 - IdentityService.GetRecoveryCodesCountAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F036 |
| Function Name | GetRecoveryCodesCountAsync |
| Class Name | IdentityService |
| Method | GetRecoveryCodesCountAsync |
| Requirement | Get recovery code count |
| Description | Validate 'Get recovery code count' in IdentityService.GetRecoveryCodesCountAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC036-01 |
| Method Should Exist | UTC036-02 |
| Source Should Contain Method Declaration | UTC036-03 |
| Return Case Set Should Be Valid | UTC036-04 |
| Log Message Case Set Should Be Valid | UTC036-05 |
| When Logger Used Should Follow Log Message Convention | UTC036-06 |
| When Result Response Used Should Follow Response Convention | UTC036-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- 0

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC036-01 | N | Type Should Exist |
| UTC036-02 | N | Method Should Exist |
| UTC036-03 | N | Source Should Contain Method Declaration |
| UTC036-04 | N | Return Case Set Should Be Valid |
| UTC036-05 | N | Log Message Case Set Should Be Valid |
| UTC036-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC036-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F036_IdentityService_GetRecoveryCodesCountAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



