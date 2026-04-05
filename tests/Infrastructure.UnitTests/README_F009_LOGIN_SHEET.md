# F009 - AuthService.LoginAsync

### 1) Function Header

| Field            | Value                                                                                                                                 |
| ---------------- | ------------------------------------------------------------------------------------------------------------------------------------- |
| Function Code    | F009                                                                                                                                  |
| Function Name    | LoginAsync                                                                                                                            |
| Class Name       | AuthService                                                                                                                           |
| Method           | LoginAsync                                                                                                                            |
| Requirement      | Login with password                                                                                                                   |
| Description      | Validate 'Login with password' in AuthService.LoginAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 9                                                                                                                                     |
| Passed           | 9                                                                                                                                     |
| Failed           | 0                                                                                                                                     |
| Untested         | 0                                                                                                                                     |
| N/A/B            | N=2, A=7, B=0                                                                                                                         |

### 2) Condition + Precondition

- Precondition: UserManager, SignInManager, token services, repositories, and UnitOfWork are mocked; request and cancellation token are valid.

| Condition Item                                                                        | UTC Ref   |
| ------------------------------------------------------------------------------------- | --------- |
| MethodSignature Should Exist                                                          | UTC009-01 |
| Condition UserNotFoundOrDeleted Should ReturnUnauthorized InvalidEmailOrPassword      | UTC009-02 |
| Condition UserInactive Should ReturnUnauthorized DeactivatedMessage                   | UTC009-03 |
| Condition UserLockedOutOrSignInLockedOut Should ReturnUnauthorized LockMessage        | UTC009-04 |
| Condition EmailNotConfirmed Should ReturnUnauthorized NotAllowedMessage               | UTC009-05 |
| Condition OphthalmologistRejected Should ReturnUnauthorized CredentialRejectedMessage | UTC009-06 |
| Condition TwoFactorRequired Should LogInformation And ReturnTwoFactorRequired         | UTC009-07 |
| Condition SuccessfulPasswordLogin Should ReturnLoginResponseSuccess                   | UTC009-08 |
| Exception Should LogError And ReturnFailureMessage                                    | UTC009-09 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result<LoginResponse>.Unauthorized("Invalid email or password")
- Result<LoginResponse>.Unauthorized("Account is deactivated. Please contact support.")
- Result<LoginResponse>.Unauthorized("Account is temporarily locked due to multiple failed attempts. Please try again later.")
- Result<LoginResponse>.Unauthorized("Please confirm your email before logging in.")
- Result<LoginResponse>.Unauthorized("Your credential verification has been rejected. Please contact support for more information.")
- Result<LoginResponse>.Success(LoginResponse.TwoFactorRequired(user.Id))
- Result<LoginResponse>.Success(LoginResponse.Success(authResponse))
- Result<LoginResponse>.Failure("An error occurred during login")

#### Exception

- Exception Should LogError And ReturnFailureMessage

#### Log message

- \_logger.LogInformation("2FA required for user: {Email}", request.Email);
- \_logger.LogError(ex, "Error during login: {Email}", request.Email);

### 4) UTC Test Case Matrix

| UTC ID    | Test Type (N/A/B) | Test Method                                                                           |
| --------- | ----------------- | ------------------------------------------------------------------------------------- |
| UTC009-01 | N                 | MethodSignature Should Exist                                                          |
| UTC009-02 | A                 | Condition UserNotFoundOrDeleted Should ReturnUnauthorized InvalidEmailOrPassword      |
| UTC009-03 | A                 | Condition UserInactive Should ReturnUnauthorized DeactivatedMessage                   |
| UTC009-04 | A                 | Condition UserLockedOutOrSignInLockedOut Should ReturnUnauthorized LockMessage        |
| UTC009-05 | A                 | Condition EmailNotConfirmed Should ReturnUnauthorized NotAllowedMessage               |
| UTC009-06 | A                 | Condition OphthalmologistRejected Should ReturnUnauthorized CredentialRejectedMessage |
| UTC009-07 | A                 | Condition TwoFactorRequired Should LogInformation And ReturnTwoFactorRequired         |
| UTC009-08 | N                 | Condition SuccessfulPasswordLogin Should ReturnLoginResponseSuccess                   |
| UTC009-09 | A                 | Exception Should LogError And ReturnFailureMessage                                    |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, A, A, A, A, A, A, N, A
- Passed/Failed: P, P, P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F009_AuthService_LoginAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---
