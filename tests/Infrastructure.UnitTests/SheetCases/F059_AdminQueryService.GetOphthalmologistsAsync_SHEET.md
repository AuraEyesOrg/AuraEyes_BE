# F059 - AdminQueryService.GetOphthalmologistsAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F059 |
| Function Name | GetOphthalmologistsAsync |
| Class Name | AdminQueryService |
| Method | GetOphthalmologistsAsync |
| Requirement | Get ophthalmologists query |
| Description | Validate 'Get ophthalmologists query' in AdminQueryService.GetOphthalmologistsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext is seeded; filter, sort, and paging inputs are valid.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC059-01 |
| Method Should Exist | UTC059-02 |
| Source Should Contain Method Declaration | UTC059-03 |
| Return Case Set Should Be Valid | UTC059-04 |
| Log Message Case Set Should Be Valid | UTC059-05 |
| When Logger Used Should Follow Log Message Convention | UTC059-06 |
| When Result Response Used Should Follow Response Convention | UTC059-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new OphthalmologistListDto { Id = row.OphthalmologistId, UserId = row.UserId, FullName = row.FullName, Email = row.Email, Phone = row.Phone, Bio = row.Bio, YearsOfExperience = row.YearsOfExperience, EmploymentType = row.EmploymentType, WorkingHoursPerWeek = row.WorkingHoursPerWeek, ExpectedMonthlySalary = row.ExpectedMonthlySalary, CommissionRate = row.CommissionRate, ActualMonthlySalary = row.ActualMonthlySalary, VerificationStatus = row.VerificationStatus, IsVerified = row.IsVerified, LicenseUrl = row.LicenseUrl, DegreeUrl = row.DegreeUrl, Licenses = licenses, Degrees = degrees, RejectionReason = row.RejectionReason, OrganisationName = row.OrganisationName, IsActive = row.IsActive, CreatedAt = row.CreatedAt }

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC059-01 | N | Type Should Exist |
| UTC059-02 | N | Method Should Exist |
| UTC059-03 | N | Source Should Contain Method Declaration |
| UTC059-04 | N | Return Case Set Should Be Valid |
| UTC059-05 | N | Log Message Case Set Should Be Valid |
| UTC059-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC059-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F059_AdminQueryService_GetOphthalmologistsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



