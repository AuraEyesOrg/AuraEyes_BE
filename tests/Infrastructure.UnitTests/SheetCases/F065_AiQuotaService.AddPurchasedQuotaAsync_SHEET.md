# F065 - AiQuotaService.AddPurchasedQuotaAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F065 |
| Function Name | AddPurchasedQuotaAsync |
| Class Name | AiQuotaService |
| Method | AddPurchasedQuotaAsync |
| Requirement | Add purchased AI quota |
| Description | Validate 'Add purchased AI quota' in AiQuotaService.AddPurchasedQuotaAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext and setting dependency are seeded with quota data.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC065-01 |
| Method Should Exist | UTC065-02 |
| Source Should Contain Method Declaration | UTC065-03 |
| Return Case Set Should Be Valid | UTC065-04 |
| Log Message Case Set Should Be Valid | UTC065-05 |
| When Logger Used Should Follow Log Message Convention | UTC065-06 |
| When Result Response Used Should Follow Response Convention | UTC065-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC065-01 | N | Type Should Exist |
| UTC065-02 | N | Method Should Exist |
| UTC065-03 | N | Source Should Contain Method Declaration |
| UTC065-04 | N | Return Case Set Should Be Valid |
| UTC065-05 | N | Log Message Case Set Should Be Valid |
| UTC065-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC065-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F065_AiQuotaService_AddPurchasedQuotaAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



