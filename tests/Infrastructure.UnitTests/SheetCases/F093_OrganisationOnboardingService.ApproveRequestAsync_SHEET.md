# F093 - OrganisationOnboardingService.ApproveRequestAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F093 |
| Function Name | ApproveRequestAsync |
| Class Name | OrganisationOnboardingService |
| Method | ApproveRequestAsync |
| Requirement | Approve onboarding request |
| Description | Validate 'Approve onboarding request' in OrganisationOnboardingService.ApproveRequestAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: Onboarding repositories, UserManager, EmailService, and UnitOfWork are mocked or seeded.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC093-01 |
| Method Should Exist | UTC093-02 |
| Source Should Contain Method Declaration | UTC093-03 |
| Return Case Set Should Be Valid | UTC093-04 |
| Log Message Case Set Should Be Valid | UTC093-05 |
| When Logger Used Should Follow Log Message Convention | UTC093-06 |
| When Result Response Used Should Follow Response Convention | UTC093-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Result<ApproveOrganisationOnboardingResult>.NotFound( "Organisation onboarding request not found.")

#### Exception

- No dedicated exception case asserted.

#### Log message

- Failed to approve organisation onboarding request {RequestId}

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC093-01 | N | Type Should Exist |
| UTC093-02 | N | Method Should Exist |
| UTC093-03 | N | Source Should Contain Method Declaration |
| UTC093-04 | N | Return Case Set Should Be Valid |
| UTC093-05 | N | Log Message Case Set Should Be Valid |
| UTC093-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC093-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F093_OrganisationOnboardingService_ApproveRequestAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



