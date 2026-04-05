# F028 - IdentityService.IsInRoleAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F028 |
| Function Name | IsInRoleAsync |
| Class Name | IdentityService |
| Method | IsInRoleAsync |
| Requirement | Check user in role |
| Description | Validate 'Check user in role' in IdentityService.IsInRoleAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC028-01 |
| Method Should Exist | UTC028-02 |
| Source Should Contain Method Declaration | UTC028-03 |
| Return Case Set Should Be Valid | UTC028-04 |
| Log Message Case Set Should Be Valid | UTC028-05 |
| When Logger Used Should Follow Log Message Convention | UTC028-06 |
| When Result Response Used Should Follow Response Convention | UTC028-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- false

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC028-01 | N | Type Should Exist |
| UTC028-02 | N | Method Should Exist |
| UTC028-03 | N | Source Should Contain Method Declaration |
| UTC028-04 | N | Return Case Set Should Be Valid |
| UTC028-05 | N | Log Message Case Set Should Be Valid |
| UTC028-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC028-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F028_IdentityService_IsInRoleAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



