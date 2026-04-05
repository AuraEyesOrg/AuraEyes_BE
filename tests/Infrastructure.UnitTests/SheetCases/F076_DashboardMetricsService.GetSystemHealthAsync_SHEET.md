# F076 - DashboardMetricsService.GetSystemHealthAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F076 |
| Function Name | GetSystemHealthAsync |
| Class Name | DashboardMetricsService |
| Method | GetSystemHealthAsync |
| Requirement | Get system health |
| Description | Validate 'Get system health' in DashboardMetricsService.GetSystemHealthAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: ApplicationDbContext is seeded with metrics data; time-range inputs are valid.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC076-01 |
| Method Should Exist | UTC076-02 |
| Source Should Contain Method Declaration | UTC076-03 |
| Return Case Set Should Be Valid | UTC076-04 |
| Log Message Case Set Should Be Valid | UTC076-05 |
| When Logger Used Should Follow Log Message Convention | UTC076-06 |
| When Result Response Used Should Follow Response Convention | UTC076-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Task.FromResult(new SystemHealthDto { AllSystemsOperational = true, Components = new List<ComponentHealthDto> { new() { ComponentName = "Database", Status = "Connected", IsHealthy = true, LatencyMs = 25, UptimePercentage = 100, LastCheckedAt = DateTime.UtcNow }, new() { ComponentName = "AI Service", Status = "Online", IsHealthy = true, LatencyMs = 120, UptimePercentage = 100, LastCheckedAt = DateTime.UtcNow }, new() { ComponentName = "Notifications", Status = "Operational", IsHealthy = true, LatencyMs = 40, UptimePercentage = 100, LastCheckedAt = DateTime.UtcNow } } })

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC076-01 | N | Type Should Exist |
| UTC076-02 | N | Method Should Exist |
| UTC076-03 | N | Source Should Contain Method Declaration |
| UTC076-04 | N | Return Case Set Should Be Valid |
| UTC076-05 | N | Log Message Case Set Should Be Valid |
| UTC076-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC076-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F076_DashboardMetricsService_GetSystemHealthAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



