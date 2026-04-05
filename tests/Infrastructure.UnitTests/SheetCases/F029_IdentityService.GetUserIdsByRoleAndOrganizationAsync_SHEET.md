# F029 - IdentityService.GetUserIdsByRoleAndOrganizationAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F029 |
| Function Name | GetUserIdsByRoleAndOrganizationAsync |
| Class Name | IdentityService |
| Method | GetUserIdsByRoleAndOrganizationAsync |
| Requirement | Get user ids by role and org |
| Description | Validate 'Get user ids by role and org' in IdentityService.GetUserIdsByRoleAndOrganizationAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC029-01 |
| Method Should Exist | UTC029-02 |
| Source Should Contain Method Declaration | UTC029-03 |
| Return Case Set Should Be Valid | UTC029-04 |
| Log Message Case Set Should Be Valid | UTC029-05 |
| When Logger Used Should Follow Log Message Convention | UTC029-06 |
| When Result Response Used Should Follow Response Convention | UTC029-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- usersInRole .Where(u => u.OrganizationId == organizationId && u.IsActive && !u.IsDeleted) .Select(u => u.Id) .ToList()

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC029-01 | N | Type Should Exist |
| UTC029-02 | N | Method Should Exist |
| UTC029-03 | N | Source Should Contain Method Declaration |
| UTC029-04 | N | Return Case Set Should Be Valid |
| UTC029-05 | N | Log Message Case Set Should Be Valid |
| UTC029-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC029-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F029_IdentityService_GetUserIdsByRoleAndOrganizationAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



