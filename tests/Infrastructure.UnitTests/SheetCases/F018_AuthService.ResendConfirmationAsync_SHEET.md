# F018 - AuthService.ResendConfirmationAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F018 |
| Function Name | ResendConfirmationAsync |
| Class Name | AuthService |
| Method | ResendConfirmationAsync |
| Requirement | Resend confirmation email |
| Description | Validate 'Resend confirmation email' in AuthService.ResendConfirmationAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC018-01 |
| Method Should Exist | UTC018-02 |
| Source Should Contain Method Declaration | UTC018-03 |
| Return Case Set Should Be Valid | UTC018-04 |
| Log Message Case Set Should Be Valid | UTC018-05 |
| When Logger Used Should Follow Log Message Convention | UTC018-06 |
| When Result Response Used Should Follow Response Convention | UTC018-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result.Success()

#### Exception

- No dedicated exception case asserted.

#### Log message

- Confirmation email resent to: {Email}

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC018-01 | N | Type Should Exist |
| UTC018-02 | N | Method Should Exist |
| UTC018-03 | N | Source Should Contain Method Declaration |
| UTC018-04 | N | Return Case Set Should Be Valid |
| UTC018-05 | N | Log Message Case Set Should Be Valid |
| UTC018-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC018-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F018_AuthService_ResendConfirmationAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



