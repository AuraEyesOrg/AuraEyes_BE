# F013 - AuthService.LogoutAllAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F013 |
| Function Name | LogoutAllAsync |
| Class Name | AuthService |
| Method | LogoutAllAsync |
| Requirement | Logout all devices |
| Description | Validate 'Logout all devices' in AuthService.LogoutAllAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC013-01 |
| Method Should Exist | UTC013-02 |
| Source Should Contain Method Declaration | UTC013-03 |
| Return Case Set Should Be Valid | UTC013-04 |
| Log Message Case Set Should Be Valid | UTC013-05 |
| When Logger Used Should Follow Log Message Convention | UTC013-06 |
| When Result Response Used Should Follow Response Convention | UTC013-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result.Success()

#### Exception

- No dedicated exception case asserted.

#### Log message

- All tokens revoked for user: {UserId}

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC013-01 | N | Type Should Exist |
| UTC013-02 | N | Method Should Exist |
| UTC013-03 | N | Source Should Contain Method Declaration |
| UTC013-04 | N | Return Case Set Should Be Valid |
| UTC013-05 | N | Log Message Case Set Should Be Valid |
| UTC013-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC013-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F013_AuthService_LogoutAllAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



