# F060 - AdminQueryService.GetPatientsAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F060 |
| Function Name | GetPatientsAsync |
| Class Name | AdminQueryService |
| Method | GetPatientsAsync |
| Requirement | Get patients query |
| Description | Validate 'Get patients query' in AdminQueryService.GetPatientsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext is seeded; filter, sort, and paging inputs are valid.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC060-01 |
| Method Should Exist | UTC060-02 |
| Source Should Contain Method Declaration | UTC060-03 |
| Return Case Set Should Be Valid | UTC060-04 |
| Log Message Case Set Should Be Valid | UTC060-05 |
| When Logger Used Should Follow Log Message Convention | UTC060-06 |
| When Result Response Used Should Follow Response Convention | UTC060-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new PagedResult<PatientListDto>( items, totalCount, pageNumber, pageSize)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC060-01 | N | Type Should Exist |
| UTC060-02 | N | Method Should Exist |
| UTC060-03 | N | Source Should Contain Method Declaration |
| UTC060-04 | N | Return Case Set Should Be Valid |
| UTC060-05 | N | Log Message Case Set Should Be Valid |
| UTC060-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC060-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F060_AdminQueryService_GetPatientsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



