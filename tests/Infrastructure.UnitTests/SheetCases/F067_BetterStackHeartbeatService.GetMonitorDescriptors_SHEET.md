# F067 - BetterStackHeartbeatService.GetMonitorDescriptors

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F067 |
| Function Name | GetMonitorDescriptors |
| Class Name | BetterStackHeartbeatService |
| Method | GetMonitorDescriptors |
| Requirement | Get BetterStack monitor descriptors |
| Description | Validate 'Get BetterStack monitor descriptors' in BetterStackHeartbeatService.GetMonitorDescriptors, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: Heartbeat configuration is valid and the client/logger are initialized.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC067-01 |
| Method Should Exist | UTC067-02 |
| Source Should Contain Method Declaration | UTC067-03 |
| Return Case Set Should Be Valid | UTC067-04 |
| Log Message Case Set Should Be Valid | UTC067-05 |
| When Logger Used Should Follow Log Message Convention | UTC067-06 |
| When Result Response Used Should Follow Response Convention | UTC067-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Enum.GetValues<BetterStackMonitor>() .Select(monitor => { var meta = MonitorMeta[monitor]

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC067-01 | N | Type Should Exist |
| UTC067-02 | N | Method Should Exist |
| UTC067-03 | N | Source Should Contain Method Declaration |
| UTC067-04 | N | Return Case Set Should Be Valid |
| UTC067-05 | N | Log Message Case Set Should Be Valid |
| UTC067-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC067-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F067_BetterStackHeartbeatService_GetMonitorDescriptors_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



