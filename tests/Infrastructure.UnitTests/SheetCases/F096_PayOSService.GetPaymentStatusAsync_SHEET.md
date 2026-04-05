# F096 - PayOSService.GetPaymentStatusAsync

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F096 |
| Function Name | GetPaymentStatusAsync |
| Class Name | PayOSService |
| Method | GetPaymentStatusAsync |
| Requirement | Get PayOS payment status |
| Description | Validate 'Get PayOS payment status' in PayOSService.GetPaymentStatusAsync, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: PayOS settings and payment payload inputs are valid.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC096-01 |
| Method Should Exist | UTC096-02 |
| Source Should Contain Method Declaration | UTC096-03 |
| Return Case Set Should Be Valid | UTC096-04 |
| Log Message Case Set Should Be Valid | UTC096-05 |
| When Logger Used Should Follow Log Message Convention | UTC096-06 |
| When Result Response Used Should Follow Response Convention | UTC096-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- (status, amount, txnRef)

#### Exception

- No dedicated exception case asserted.

#### Log message

- Querying PayOS payment status for OrderCode={OrderCode}

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC096-01 | N | Type Should Exist |
| UTC096-02 | N | Method Should Exist |
| UTC096-03 | N | Source Should Contain Method Declaration |
| UTC096-04 | N | Return Case Set Should Be Valid |
| UTC096-05 | N | Log Message Case Set Should Be Valid |
| UTC096-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC096-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F096_PayOSService_GetPaymentStatusAsync_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



