# F102 - SupabaseStorageService.FileExists

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F102 |
| Function Name | FileExists |
| Class Name | SupabaseStorageService |
| Method | FileExists |
| Requirement | Check Supabase file exists |
| Description | Validate 'Check Supabase file exists' in SupabaseStorageService.FileExists, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: Supabase settings are valid; stream and path inputs are valid.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC102-01 |
| Method Should Exist | UTC102-02 |
| Source Should Contain Method Declaration | UTC102-03 |
| Return Case Set Should Be Valid | UTC102-04 |
| Log Message Case Set Should Be Valid | UTC102-05 |
| When Logger Used Should Follow Log Message Convention | UTC102-06 |
| When Result Response Used Should Follow Response Convention | UTC102-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- false

#### Exception

- No dedicated exception case asserted.

#### Log message

- Error checking file existence: {Path}

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC102-01 | N | Type Should Exist |
| UTC102-02 | N | Method Should Exist |
| UTC102-03 | N | Source Should Contain Method Declaration |
| UTC102-04 | N | Return Case Set Should Be Valid |
| UTC102-05 | N | Log Message Case Set Should Be Valid |
| UTC102-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC102-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F102_SupabaseStorageService_FileExists_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



