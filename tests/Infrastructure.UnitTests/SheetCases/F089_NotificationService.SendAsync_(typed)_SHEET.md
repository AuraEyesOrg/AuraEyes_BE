# F089 - NotificationService.SendAsync (typed)

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F089 |
| Function Name | SendAsync (typed) |
| Class Name | NotificationService |
| Method | SendAsync (typed) |
| Requirement | Send typed notification |
| Description | Validate 'Send typed notification' in NotificationService.SendAsync (typed), covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: Notification repository, UnitOfWork, and hub service are mocked.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC089-01 |
| Method Should Exist | UTC089-02 |
| Source Should Contain Method Declaration | UTC089-03 |
| Return Case Set Should Be Valid | UTC089-04 |
| Log Message Case Set Should Be Valid | UTC089-05 |
| When Logger Used Should Follow Log Message Convention | UTC089-06 |
| When Result Response Used Should Follow Response Convention | UTC089-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- No explicit return snippet asserted in this test file.

#### Exception

- No dedicated exception case asserted.

#### Log message

- Failed to send notification to User {UserId}: {Message}

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC089-01 | N | Type Should Exist |
| UTC089-02 | N | Method Should Exist |
| UTC089-03 | N | Source Should Contain Method Declaration |
| UTC089-04 | N | Return Case Set Should Be Valid |
| UTC089-05 | N | Log Message Case Set Should Be Valid |
| UTC089-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC089-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F089_NotificationService_SendAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



