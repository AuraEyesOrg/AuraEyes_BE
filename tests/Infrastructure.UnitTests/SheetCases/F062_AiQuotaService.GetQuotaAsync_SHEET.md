# F062 - AiQuotaService.GetQuotaAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F062 |
| Function Name | GetQuotaAsync |
| Class Name | AiQuotaService |
| Method | GetQuotaAsync |
| Requirement | Get AI quota |
| Description | Validate 'Get AI quota' in AiQuotaService.GetQuotaAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext and setting dependency are seeded with quota data.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC062-01 |
| Method Should Exist | UTC062-02 |
| Source Should Contain Method Declaration | UTC062-03 |
| Return Case Set Should Be Valid | UTC062-04 |
| Log Message Case Set Should Be Valid | UTC062-05 |
| When Logger Used Should Follow Log Message Convention | UTC062-06 |
| When Result Response Used Should Follow Response Convention | UTC062-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- await GetPatientQuotaAsync(userId, cancellationToken)

#### Exception

- No dedicated exception case asserted.

#### Log message

- [AiQuotaService] GetQuotaAsync called â€” UserId: {UserId}, Role: {Role}

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC062-01 | N | Type Should Exist |
| UTC062-02 | N | Method Should Exist |
| UTC062-03 | N | Source Should Contain Method Declaration |
| UTC062-04 | N | Return Case Set Should Be Valid |
| UTC062-05 | N | Log Message Case Set Should Be Valid |
| UTC062-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC062-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F062_AiQuotaService_GetQuotaAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



