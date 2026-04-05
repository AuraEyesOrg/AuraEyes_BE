# F005 - AuthService.RegisterPatientAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F005 |
| Function Name | RegisterPatientAsync |
| Class Name | AuthService |
| Method | RegisterPatientAsync |
| Requirement | Register patient account |
| Description | Validate 'Register patient account' in AuthService.RegisterPatientAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC005-01 |
| Method Should Exist | UTC005-02 |
| Source Should Contain Method Declaration | UTC005-03 |
| Return Case Set Should Be Valid | UTC005-04 |
| Log Message Case Set Should Be Valid | UTC005-05 |
| When Logger Used Should Follow Log Message Convention | UTC005-06 |
| When Result Response Used Should Follow Response Convention | UTC005-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result<RegisterResponse>.Failure("A user with this email already exists")

#### Exception

- No dedicated exception case asserted.

#### Log message

- Patient registered: {Email}

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC005-01 | N | Type Should Exist |
| UTC005-02 | N | Method Should Exist |
| UTC005-03 | N | Source Should Contain Method Declaration |
| UTC005-04 | N | Return Case Set Should Be Valid |
| UTC005-05 | N | Log Message Case Set Should Be Valid |
| UTC005-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC005-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F005_AuthService_RegisterPatientAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



