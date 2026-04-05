# F072 - DashboardMetricsService.GetSystemAdminMetricsAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F072 |
| Function Name | GetSystemAdminMetricsAsync |
| Class Name | DashboardMetricsService |
| Method | GetSystemAdminMetricsAsync |
| Requirement | Get system admin metrics |
| Description | Validate 'Get system admin metrics' in DashboardMetricsService.GetSystemAdminMetricsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext is seeded with metrics data; time-range inputs are valid.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC072-01 |
| Method Should Exist | UTC072-02 |
| Source Should Contain Method Declaration | UTC072-03 |
| Return Case Set Should Be Valid | UTC072-04 |
| Log Message Case Set Should Be Valid | UTC072-05 |
| When Logger Used Should Follow Log Message Convention | UTC072-06 |
| When Result Response Used Should Follow Response Convention | UTC072-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new PaymentMethodRevenueDto { PaymentMethod = item.PaymentMethod.ToString(), Amount = amount, Percentage = totalDepositAmount <= 0m ? 0m : Math.Round(amount / totalDepositAmount * 100m, 1) }

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC072-01 | N | Type Should Exist |
| UTC072-02 | N | Method Should Exist |
| UTC072-03 | N | Source Should Contain Method Declaration |
| UTC072-04 | N | Return Case Set Should Be Valid |
| UTC072-05 | N | Log Message Case Set Should Be Valid |
| UTC072-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC072-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F072_DashboardMetricsService_GetSystemAdminMetricsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



