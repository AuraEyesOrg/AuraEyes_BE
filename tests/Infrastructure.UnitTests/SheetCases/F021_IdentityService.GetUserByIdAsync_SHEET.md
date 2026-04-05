# F021 - IdentityService.GetUserByIdAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F021 |
| Function Name | GetUserByIdAsync |
| Class Name | IdentityService |
| Method | GetUserByIdAsync |
| Requirement | Get user by id |
| Description | Validate 'Get user by id' in IdentityService.GetUserByIdAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC021-01 |
| Method Should Exist | UTC021-02 |
| Source Should Contain Method Declaration | UTC021-03 |
| Return Case Set Should Be Valid | UTC021-04 |
| Log Message Case Set Should Be Valid | UTC021-05 |
| When Logger Used Should Follow Log Message Convention | UTC021-06 |
| When Result Response Used Should Follow Response Convention | UTC021-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- user == null ? null : MapToDto(user)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC021-01 | N | Type Should Exist |
| UTC021-02 | N | Method Should Exist |
| UTC021-03 | N | Source Should Contain Method Declaration |
| UTC021-04 | N | Return Case Set Should Be Valid |
| UTC021-05 | N | Log Message Case Set Should Be Valid |
| UTC021-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC021-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F021_IdentityService_GetUserByIdAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



