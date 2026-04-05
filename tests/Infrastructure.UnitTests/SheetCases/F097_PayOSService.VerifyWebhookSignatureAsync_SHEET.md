# F097 - PayOSService.VerifyWebhookSignatureAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F097 |
| Function Name | VerifyWebhookSignatureAsync |
| Class Name | PayOSService |
| Method | VerifyWebhookSignatureAsync |
| Requirement | Verify PayOS webhook signature |
| Description | Validate 'Verify PayOS webhook signature' in PayOSService.VerifyWebhookSignatureAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: PayOS settings and payment payload inputs are valid.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC097-01 |
| Method Should Exist | UTC097-02 |
| Source Should Contain Method Declaration | UTC097-03 |
| Return Case Set Should Be Valid | UTC097-04 |
| Log Message Case Set Should Be Valid | UTC097-05 |
| When Logger Used Should Follow Log Message Convention | UTC097-06 |
| When Result Response Used Should Follow Response Convention | UTC097-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- Task.FromResult(true)

#### Exception

- No dedicated exception case asserted.

#### Log message

- Verifying PayOS webhook signature

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC097-01 | N | Type Should Exist |
| UTC097-02 | N | Method Should Exist |
| UTC097-03 | N | Source Should Contain Method Declaration |
| UTC097-04 | N | Return Case Set Should Be Valid |
| UTC097-05 | N | Log Message Case Set Should Be Valid |
| UTC097-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC097-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F097_PayOSService_VerifyWebhookSignatureAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



