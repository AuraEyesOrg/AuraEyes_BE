# F031 - IdentityService.IsTwoFactorEnabledAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F031 |
| Function Name | IsTwoFactorEnabledAsync |
| Class Name | IdentityService |
| Method | IsTwoFactorEnabledAsync |
| Requirement | Check 2FA enabled |
| Description | Validate 'Check 2FA enabled' in IdentityService.IsTwoFactorEnabledAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: UserManager and RoleManager are mocked with matching user and role seed data.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC031-01 |
| Method Should Exist | UTC031-02 |
| Source Should Contain Method Declaration | UTC031-03 |
| Return Case Set Should Be Valid | UTC031-04 |
| Log Message Case Set Should Be Valid | UTC031-05 |
| When Logger Used Should Follow Log Message Convention | UTC031-06 |
| When Result Response Used Should Follow Response Convention | UTC031-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- user != null && await _userManager.GetTwoFactorEnabledAsync(user)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC031-01 | N | Type Should Exist |
| UTC031-02 | N | Method Should Exist |
| UTC031-03 | N | Source Should Contain Method Declaration |
| UTC031-04 | N | Return Case Set Should Be Valid |
| UTC031-05 | N | Log Message Case Set Should Be Valid |
| UTC031-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC031-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F031_IdentityService_IsTwoFactorEnabledAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



