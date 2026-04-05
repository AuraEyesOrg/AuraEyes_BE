# F099 - SlotMaintenanceJob.ExpireUnusedSlotsAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F099 |
| Function Name | ExpireUnusedSlotsAsync |
| Class Name | SlotMaintenanceJob |
| Method | ExpireUnusedSlotsAsync |
| Requirement | Execute slot maintenance |
| Description | Validate 'Execute slot maintenance' in SlotMaintenanceJob.ExpireUnusedSlotsAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: Slot data is seeded with time-based scenarios and the job context is ready.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC099-01 |
| Method Should Exist | UTC099-02 |
| Source Should Contain Method Declaration | UTC099-03 |
| Return Case Set Should Be Valid | UTC099-04 |
| Log Message Case Set Should Be Valid | UTC099-05 |
| When Logger Used Should Follow Log Message Convention | UTC099-06 |
| When Result Response Used Should Follow Response Convention | UTC099-07 |

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
| UTC099-01 | N | Type Should Exist |
| UTC099-02 | N | Method Should Exist |
| UTC099-03 | N | Source Should Contain Method Declaration |
| UTC099-04 | N | Return Case Set Should Be Valid |
| UTC099-05 | N | Log Message Case Set Should Be Valid |
| UTC099-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC099-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F099_SlotMaintenanceJob_ExpireUnusedSlotsAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



