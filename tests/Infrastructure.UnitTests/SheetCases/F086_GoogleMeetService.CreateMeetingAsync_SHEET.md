# F086 - GoogleMeetService.CreateMeetingAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F086 |
| Function Name | CreateMeetingAsync |
| Class Name | GoogleMeetService |
| Method | CreateMeetingAsync |
| Requirement | Create Google Meet meeting |
| Description | Validate 'Create Google Meet meeting' in GoogleMeetService.CreateMeetingAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: Google credentials and service configuration are valid.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC086-01 |
| Method Should Exist | UTC086-02 |
| Source Should Contain Method Declaration | UTC086-03 |
| Return Case Set Should Be Valid | UTC086-04 |
| Log Message Case Set Should Be Valid | UTC086-05 |
| When Logger Used Should Follow Log Message Convention | UTC086-06 |
| When Result Response Used Should Follow Response Convention | UTC086-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- new MeetingInfo(meetLink, createdEvent.Id)

#### Exception

- No dedicated exception case asserted.

#### Log message

- No explicit log snippet asserted in this test file.

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC086-01 | N | Type Should Exist |
| UTC086-02 | N | Method Should Exist |
| UTC086-03 | N | Source Should Contain Method Declaration |
| UTC086-04 | N | Return Case Set Should Be Valid |
| UTC086-05 | N | Log Message Case Set Should Be Valid |
| UTC086-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC086-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F086_GoogleMeetService_CreateMeetingAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



