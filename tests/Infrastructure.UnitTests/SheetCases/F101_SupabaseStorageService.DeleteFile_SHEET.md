# F101 - SupabaseStorageService.DeleteFile

### 1) Function Header

| Field | Value |
|---|---|
| Function Code | F101 |
| Function Name | DeleteFile |
| Class Name | SupabaseStorageService |
| Method | DeleteFile |
| Requirement | Delete file from Supabase |
| Description | Validate 'Delete file from Supabase' in SupabaseStorageService.DeleteFile, covering success and failure flow, response contract, and logging behavior. |
| Total Test Cases | 7 |
| Passed | 7 |
| Failed | 0 |
| Untested | 0 |
| N/A/B | N=7, A=0, B=0 |

### 2) Condition + Precondition

- Precondition: Supabase settings are valid; stream and path inputs are valid.

| Condition Item | UTC Ref |
|---|---|
| Type Should Exist | UTC101-01 |
| Method Should Exist | UTC101-02 |
| Source Should Contain Method Declaration | UTC101-03 |
| Return Case Set Should Be Valid | UTC101-04 |
| Log Message Case Set Should Be Valid | UTC101-05 |
| When Logger Used Should Follow Log Message Convention | UTC101-06 |
| When Result Response Used Should Follow Response Convention | UTC101-07 |

### 3) Confirm - Return / Exception / Log message

#### Return

- false

#### Exception

- No dedicated exception case asserted.

#### Log message

- Deleted file from Supabase Storage: {Path}

### 4) UTC Test Case Matrix

| UTC ID | Test Type (N/A/B) | Test Method |
|---|---|---|
| UTC101-01 | N | Type Should Exist |
| UTC101-02 | N | Method Should Exist |
| UTC101-03 | N | Source Should Contain Method Declaration |
| UTC101-04 | N | Return Case Set Should Be Valid |
| UTC101-05 | N | Log Message Case Set Should Be Valid |
| UTC101-06 | N | When Logger Used Should Follow Log Message Convention |
| UTC101-07 | N | When Result Response Used Should Follow Response Convention |

### 5) Result row (copy nhanh)

- Type(N/A/B): N, N, N, N, N, N, N
- Passed/Failed: P, P, P, P, P, P, P
- Defect ID: leave blank if no bug.

### 6) Source of truth

- Test file: tests/Infrastructure.UnitTests/Generated/Functions/F101_SupabaseStorageService_DeleteFile_Tests.cs
- Checklist source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

---



