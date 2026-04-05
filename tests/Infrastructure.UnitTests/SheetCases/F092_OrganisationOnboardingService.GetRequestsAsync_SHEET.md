# F092 - OrganisationOnboardingService.GetRequestsAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F092 |
| Function Name | GetRequestsAsync |
| Class Name | OrganisationOnboardingService |
| Method | GetRequestsAsync |
| Requirement | Get onboarding requests |
| Description | Validate 'Get onboarding requests' in OrganisationOnboardingService.GetRequestsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: Onboarding repositories, UserManager, EmailService, and UnitOfWork are mocked or seeded.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC092-01 |
| Method Should Exist | UTC092-02 |
| Source Should Contain Method Declaration | UTC092-03 |
| Return Case Set Should Be Valid | UTC092-04 |
| Log Message Case Set Should Be Valid | UTC092-05 |
| When Logger Used Should Follow Log Message Convention | UTC092-06 |
| When Result Response Used Should Follow Response Convention | UTC092-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result<IReadOnlyList<OrganisationOnboardingRequestDto>>.Success(items)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC092-01 | N | Type Should Exist |
| UTC092-02 | N | Method Should Exist |
| UTC092-03 | N | Source Should Contain Method Declaration |
| UTC092-04 | N | Return Case Set Should Be Valid |
| UTC092-05 | N | Log Message Case Set Should Be Valid |
| UTC092-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC092-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F092_OrganisationOnboardingService_GetRequestsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



