# F081 - DateTimeService.UtcNow (property)

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F081 |
| Function Name | UtcNow (property) |
| Class Name | DateTimeService |
| Method | UtcNow (property) |
| Requirement | Get UTC now |
| Description | Validate 'Get UTC now' in DateTimeService.UtcNow (property), covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 4 |
| Passed | 4 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=4, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: Service is instantiated directly with no external dependency.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC081-01 |
| Property Should Exist | UTC081-02 |
| Source Should Contain Property Declaration | UTC081-03 |
| Property Should Be Readable | UTC081-04 |

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
| UTC081-01 | N | Type Should Exist |
| UTC081-02 | N | Property Should Exist |
| UTC081-03 | N | Source Should Contain Property Declaration |
| UTC081-04 | N | Property Should Be Readable |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N
- Passed/Failed: P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F081_DateTimeService_UtcNow_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



