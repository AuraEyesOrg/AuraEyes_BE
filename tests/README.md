# AuraEyes Backend - Unit Test Documentation

## Overview

This document catalogs all unit tests for the **AuraEyes Backend** application. Tests follow the **Arrange-Act-Assert** pattern using:

- **xUnit** — Test framework
- **FluentAssertions** — Readable assertion syntax
- **NSubstitute** — Mocking framework
- **FluentValidation.TestHelper** — Validator testing

## Test Summary

| Project                  | Tests   | Status             |
| ------------------------ | ------- | ------------------ |
| Domain.UnitTests         | 108     | ✅ All Passing     |
| Application.UnitTests    | 41      | ✅ All Passing     |
| Infrastructure.UnitTests | 42      | ✅ All Passing     |
| API.UnitTests            | 39      | ✅ All Passing     |
| **Total**                | **230** | ✅ **All Passing** |

## Running Tests

```bash
# Run all tests
dotnet test

# Run specific project
dotnet test tests/Domain.UnitTests
dotnet test tests/Application.UnitTests
dotnet test tests/Infrastructure.UnitTests
dotnet test tests/API.UnitTests

# Run with verbose output
dotnet test --verbosity normal

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## 1. Domain.UnitTests (108 tests)

Tests for domain entities, value objects, and common base classes.

### 1.1 Value Objects

#### `MoneyTests` — [ValueObjects/MoneyTests.cs](tests/Domain.UnitTests/ValueObjects/MoneyTests.cs)

| #   | Test                                        | Description                                  |
| --- | ------------------------------------------- | -------------------------------------------- |
| 1   | `Constructor_ValidInput_ShouldCreateMoney`  | Creates Money with valid amount and currency |
| 2   | `Constructor_NegativeAmount_ShouldThrow`    | Rejects negative money amounts               |
| 3   | `Constructor_ZeroAmount_ShouldSucceed`      | Allows zero-value Money                      |
| 4   | `Currency_ShouldBeUpperCase`                | Currency code normalized to uppercase        |
| 5   | `Addition_SameCurrency_ShouldAdd`           | Adds two Money with same currency            |
| 6   | `Addition_DifferentCurrency_ShouldThrow`    | Cannot add different currencies              |
| 7   | `Subtraction_SameCurrency_ShouldSubtract`   | Subtracts Money with same currency           |
| 8   | `Subtraction_DifferentCurrency_ShouldThrow` | Cannot subtract different currencies         |
| 9   | `Equals_SameValues_ShouldBeEqual`           | Value equality for same amount+currency      |
| 10  | `Equals_DifferentAmount_ShouldNotBeEqual`   | Different amounts are not equal              |
| 11  | `Equals_DifferentCurrency_ShouldNotBeEqual` | Different currencies are not equal           |
| 12  | `ToString_ShouldFormatCorrectly`            | String representation is correct             |

#### `AddressTests` — [ValueObjects/AddressTests.cs](tests/Domain.UnitTests/ValueObjects/AddressTests.cs)

| #   | Test                                         | Description                            |
| --- | -------------------------------------------- | -------------------------------------- |
| 1   | `Constructor_ValidInput_ShouldCreateAddress` | Creates Address with all fields        |
| 2   | `Equals_SameValues_ShouldBeEqual`            | Value equality for identical addresses |
| 3   | `Equals_DifferentValues_ShouldNotBeEqual`    | Different addresses are not equal      |
| 4   | `ToString_ShouldFormatCorrectly`             | Formatted address string               |
| 5   | `GetHashCode_SameValues_ShouldBeSame`        | Consistent hash for equal values       |

### 1.2 Common

#### `BaseEntityTests` — [Common/BaseEntityTests.cs](tests/Domain.UnitTests/Common/BaseEntityTests.cs)

| #   | Test                                           | Description                      |
| --- | ---------------------------------------------- | -------------------------------- |
| 1   | `Constructor_Default_ShouldGenerateId`         | Auto-generates unique GUID ID    |
| 2   | `Constructor_WithId_ShouldUseProvidedId`       | Uses explicitly provided ID      |
| 3   | `AddDomainEvent_ShouldAddToCollection`         | Domain events can be added       |
| 4   | `RemoveDomainEvent_ShouldRemoveFromCollection` | Domain events can be removed     |
| 5   | `ClearDomainEvents_ShouldRemoveAllEvents`      | All domain events can be cleared |

#### `DomainEventTests` — [Common/DomainEventTests.cs](tests/Domain.UnitTests/Common/DomainEventTests.cs)

| #   | Test                                      | Description                |
| --- | ----------------------------------------- | -------------------------- |
| 1   | `DomainEvent_ShouldSetOccurredOnToUtcNow` | Event timestamp set to UTC |
| 2   | `DomainEvent_ShouldGenerateUniqueEventId` | Events get unique IDs      |

#### `ConcurrencyExceptionTests` — [Common/ConcurrencyExceptionTests.cs](tests/Domain.UnitTests/Common/ConcurrencyExceptionTests.cs)

| #   | Test                                                     | Description              |
| --- | -------------------------------------------------------- | ------------------------ |
| 1   | `DefaultConstructor_ShouldHaveDefaultMessage`            | Default error message    |
| 2   | `Constructor_WithMessage_ShouldSetMessage`               | Custom message support   |
| 3   | `Constructor_WithMessageAndInnerException_ShouldSetBoth` | Inner exception chaining |

### 1.3 Entities

#### `ConsultationSessionTests` — [Entities/ConsultationSessionTests.cs](tests/Domain.UnitTests/Entities/ConsultationSessionTests.cs)

| #   | Test                                                 | Description                      |
| --- | ---------------------------------------------------- | -------------------------------- |
| 1   | `CreateVerification_ShouldCreateWithCorrectDefaults` | Verification session defaults    |
| 2   | `CreateVerification_WithDoctor_ShouldAssignDoctor`   | Doctor assignment on creation    |
| 3   | `CreateVideoCall_ShouldCreateWithMemoOnlyChat`       | Video call has memo-only chat    |
| 4   | `CreateVideoCall_PastAppointment_ShouldThrow`        | Rejects past appointment times   |
| 5   | `CreateClinicBooking_ShouldCreateWithLockedChat`     | Clinic booking has locked chat   |
| 6   | `CreateClinicBooking_PastAppointment_ShouldThrow`    | Rejects past clinic bookings     |
| 7   | `Confirm_PendingSession_ShouldConfirm`               | Pending → Confirmed transition   |
| 8   | `Confirm_NonPendingSession_ShouldThrow`              | Invalid state transition blocked |
| 9   | `EndSession_AssignedDoctor_ShouldComplete`           | Assigned doctor can end session  |
| 10  | `EndSession_WrongDoctor_ShouldThrow`                 | Wrong doctor cannot end session  |
| 11  | `Cancel_PendingSession_ShouldCancel`                 | Pending can be cancelled         |
| 12  | `Cancel_CompletedSession_ShouldThrow`                | Completed cannot be cancelled    |
| 13  | `OpenChat_LockedSession_ShouldOpen`                  | Chat can be unlocked             |
| 14  | `OpenChat_ArchivedSession_ShouldThrow`               | Archived sessions stay locked    |
| 15  | `AssignDoctor_ShouldSetOphthalmologistId`            | Doctor can be assigned           |
| 16  | `SetMeetingInfo_ValidLink_ShouldSet`                 | Meeting URL can be set           |
| 17  | `SetMeetingInfo_EmptyLink_ShouldThrow`               | Empty meeting URL rejected       |
| 18  | `ClearMeetingInfo_ShouldNullify`                     | Meeting info can be cleared      |
| 19  | `RecordActivity_ShouldUpdateLastActivityAt`          | Activity tracking                |
| 20  | `RecordReminderSent_ShouldSetLastReminderSentAt`     | Reminder tracking                |

#### `WalletTests` — [Entities/WalletTests.cs](tests/Domain.UnitTests/Entities/WalletTests.cs)

| #   | Test                                               | Description                      |
| --- | -------------------------------------------------- | -------------------------------- |
| 1   | `Constructor_ValidInput_ShouldCreateWallet`        | Creates wallet with user ID      |
| 2   | `Constructor_DefaultBalance_ShouldBeZero`          | Default balance is zero          |
| 3   | `Constructor_NegativeBalance_ShouldThrow`          | Rejects negative initial balance |
| 4   | `Deposit_PositiveAmount_ShouldIncreaseBalance`     | Deposits increase balance        |
| 5   | `Withdraw_SufficientBalance_ShouldDecreaseBalance` | Withdrawals decrease balance     |
| 6   | `Withdraw_InsufficientBalance_ShouldThrow`         | Overdraft protection             |
| 7   | `Withdraw_ExactBalance_ShouldResultInZero`         | Exact withdrawal leaves zero     |

#### `OrderTests` — [Entities/OrderTests.cs](tests/Domain.UnitTests/Entities/OrderTests.cs)

| #   | Test                                                   | Description                      |
| --- | ------------------------------------------------------ | -------------------------------- |
| 1   | `Constructor_ShouldCreatePendingOrder`                 | New orders start as Pending      |
| 2   | `FullLifecycle_Pending_Confirmed_Processing_Completed` | Happy path lifecycle             |
| 3   | `Refund_CompletedOrder_ShouldRefund`                   | Completed orders can be refunded |
| 4   | `Confirm_NonPendingOrder_ShouldThrow`                  | Invalid confirm transition       |
| 5   | `StartProcessing_NonConfirmedOrder_ShouldThrow`        | Invalid processing transition    |
| 6   | `Complete_NonProcessingOrder_ShouldThrow`              | Invalid complete transition      |
| 7   | `Cancel_CompletedOrder_ShouldThrow`                    | Completed cannot be cancelled    |
| 8   | `Cancel_RefundedOrder_ShouldThrow`                     | Refunded cannot be cancelled     |
| 9   | `Refund_NonCompletedOrder_ShouldThrow`                 | Only completed can be refunded   |
| 10  | `Cancel_PendingOrder_ShouldSucceed`                    | Pending can be cancelled         |
| 11  | `Cancel_ConfirmedOrder_ShouldSucceed`                  | Confirmed can be cancelled       |
| 12  | `Cancel_ProcessingOrder_ShouldSucceed`                 | Processing can be cancelled      |

#### `ScheduleTests` — [Entities/ScheduleTests.cs](tests/Domain.UnitTests/Entities/ScheduleTests.cs)

| #   | Test                                                   | Description                    |
| --- | ------------------------------------------------------ | ------------------------------ |
| 1   | `Constructor_ValidInput_ShouldCreateAvailableSchedule` | Creates available schedule     |
| 2   | `Constructor_EndTimeBeforeStartTime_ShouldThrow`       | Rejects invalid time range     |
| 3   | `Constructor_EqualStartAndEndTime_ShouldThrow`         | Rejects zero-duration schedule |
| 4   | `Book_AvailableSchedule_ShouldBook`                    | Available → Booked transition  |
| 5   | `Book_AlreadyBookedSchedule_ShouldThrow`               | Cannot double-book             |
| 6   | `Cancel_ShouldSetCancelledStatus`                      | Schedule can be cancelled      |
| 7   | `Complete_BookedSchedule_ShouldComplete`               | Booked → Completed transition  |
| 8   | `Complete_AvailableSchedule_ShouldThrow`               | Unbooked cannot complete       |
| 9   | `MarkNoShow_BookedSchedule_ShouldMarkNoShow`           | No-show tracking               |
| 10  | `MarkNoShow_AvailableSchedule_ShouldThrow`             | Unbooked cannot be no-show     |
| 11  | `UpdateCost_ShouldSetNewCost`                          | Schedule cost can be updated   |
| 12  | `UpdateCost_Null_ShouldClearCost`                      | Cost can be cleared            |

#### `OphthalmologistTests` — [Entities/OphthalmologistTests.cs](tests/Domain.UnitTests/Entities/OphthalmologistTests.cs)

| #   | Test                                                | Description                   |
| --- | --------------------------------------------------- | ----------------------------- |
| 1   | `Constructor_ShouldCreateWithPendingVerification`   | New doctors start pending     |
| 2   | `Constructor_DefaultValues_ShouldUseDefaults`       | Default field values          |
| 3   | `UpdateProfile_ValidInput_ShouldUpdate`             | Profile fields can be updated |
| 4   | `UpdateProfile_NegativeYears_ShouldThrow`           | Rejects negative experience   |
| 5   | `Verify_ShouldSetApproved`                          | Pending → Approved transition |
| 6   | `Reject_ShouldSetRejected`                          | Pending → Rejected transition |
| 7   | `Unverify_ShouldResetToPending`                     | Approved → Pending rollback   |
| 8   | `VerificationLifecycle_Verify_Unverify_Reject`      | Full lifecycle test           |
| 9   | `UpdateCredentialFiles_ShouldUpdateNonNullValues`   | Selective file update         |
| 10  | `UpdateCredentialFiles_BothValues_ShouldUpdateBoth` | Bulk file update              |

#### `PatientTests` — [Entities/PatientTests.cs](tests/Domain.UnitTests/Entities/PatientTests.cs)

| #   | Test                                         | Description                     |
| --- | -------------------------------------------- | ------------------------------- |
| 1   | `Constructor_ShouldCreatePatient`            | Creates patient with user ID    |
| 2   | `Constructor_DefaultValues_ShouldBeNull`     | Optional fields default to null |
| 3   | `UpdateProfile_ShouldUpdateFields`           | Profile update works            |
| 4   | `UpdateProfile_NullValues_ShouldClearFields` | Fields can be cleared           |

---

## 2. Application.UnitTests (41 tests)

Tests for CQRS handlers, validators, behaviors, and models.

### 2.1 Validators

#### `CreateVerificationSessionCommandValidatorTests` — [Validators/CreateVerificationSessionCommandValidatorTests.cs](tests/Application.UnitTests/Validators/CreateVerificationSessionCommandValidatorTests.cs)

| #   | Test                                            | Description                     |
| --- | ----------------------------------------------- | ------------------------------- |
| 1   | `Validate_ValidCommand_ShouldPass`              | Valid command passes validation |
| 2   | `Validate_EmptyPatientId_ShouldFail`            | Patient ID required             |
| 3   | `Validate_EmptyAiScreeningId_ShouldFail`        | AI Screening ID required        |
| 4   | `Validate_NegativePrice_ShouldFail`             | Price cannot be negative        |
| 5   | `Validate_ZeroPrice_ShouldPass`                 | Zero price is allowed           |
| 6   | `Validate_OptionalOphthalmologistId_ShouldPass` | Doctor ID is optional           |

#### `UpdatePatientProfileCommandValidatorTests` — [Validators/UpdatePatientProfileCommandValidatorTests.cs](tests/Application.UnitTests/Validators/UpdatePatientProfileCommandValidatorTests.cs)

| #   | Test                                     | Description                   |
| --- | ---------------------------------------- | ----------------------------- |
| 1   | `Validate_ValidCommand_ShouldPass`       | Valid update passes           |
| 2   | `Validate_EmptyUserId_ShouldFail`        | User ID required              |
| 3   | `Validate_EmptyFullName_ShouldFail`      | Full name required            |
| 4   | `Validate_FullNameTooLong_ShouldFail`    | Name length limit enforced    |
| 5   | `Validate_PhoneTooLong_ShouldFail`       | Phone length limit enforced   |
| 6   | `Validate_InvalidDateOfBirth_ShouldFail` | Birth date validation         |
| 7   | `Validate_InvalidGender_ShouldFail`      | Gender enum validation        |
| 8   | `Validate_AddressTooLong_ShouldFail`     | Address length limit enforced |
| 9   | `Validate_NullOptionalFields_ShouldPass` | Optional fields can be null   |

### 2.2 Handlers

#### `CreateVerificationSessionCommandHandlerTests` — [Handlers/CreateVerificationSessionCommandHandlerTests.cs](tests/Application.UnitTests/Handlers/CreateVerificationSessionCommandHandlerTests.cs)

| #   | Test                                                     | Description                     |
| --- | -------------------------------------------------------- | ------------------------------- |
| 1   | `Handle_ValidCommand_ShouldCreateSessionAndReturnId`     | Creates session and returns ID  |
| 2   | `Handle_WithoutDoctor_ShouldCreateSessionWithNullDoctor` | Session without assigned doctor |

### 2.3 Behaviors (MediatR Pipeline)

#### `ValidationBehaviorTests` — [Behaviors/ValidationBehaviorTests.cs](tests/Application.UnitTests/Behaviors/ValidationBehaviorTests.cs)

| #   | Test                                                   | Description                       |
| --- | ------------------------------------------------------ | --------------------------------- |
| 1   | `Handle_NoValidators_ShouldCallNext`                   | Passes through without validators |
| 2   | `Handle_ValidRequest_ShouldCallNext`                   | Valid requests proceed            |
| 3   | `Handle_InvalidRequest_ShouldThrowValidationException` | Invalid requests throw            |

#### `PerformanceBehaviorTests` — [Behaviors/PerformanceBehaviorTests.cs](tests/Application.UnitTests/Behaviors/PerformanceBehaviorTests.cs)

| #   | Test                                     | Description                         |
| --- | ---------------------------------------- | ----------------------------------- |
| 1   | `Handle_FastRequest_ShouldNotLogWarning` | Fast requests don't trigger warning |
| 2   | `Handle_SlowRequest_ShouldLogWarning`    | Slow requests trigger warning       |

#### `LoggingBehaviorTests` — [Behaviors/LoggingBehaviorTests.cs](tests/Application.UnitTests/Behaviors/LoggingBehaviorTests.cs)

| #   | Test                                      | Description                 |
| --- | ----------------------------------------- | --------------------------- |
| 1   | `Handle_ShouldLogBeforeAndAfterExecution` | Logs pre/post execution     |
| 2   | `Handle_ShouldReturnResponseFromNext`     | Preserves pipeline response |

### 2.4 Models

#### `ResultTests` — [Models/ResultTests.cs](tests/Application.UnitTests/Models/ResultTests.cs)

| #   | Test                                            | Description                  |
| --- | ----------------------------------------------- | ---------------------------- |
| 1   | `Success_ShouldCreateSuccessResult`             | Generic success with data    |
| 2   | `Failure_WithMessage_ShouldCreateFailureResult` | Generic failure with message |
| 3   | `Failure_WithMultipleErrors_ShouldJoinErrors`   | Multiple error aggregation   |
| 4   | `Unauthorized_ShouldSetFlag`                    | Unauthorized flag set        |
| 5   | `Forbidden_ShouldSetFlag`                       | Forbidden flag set           |
| 6   | `NotFound_ShouldSetFlag`                        | NotFound flag set            |
| 7   | `Conflict_ShouldSetFlag`                        | Conflict flag set            |
| 8   | `NonGeneric_Success_ShouldBeSuccessful`         | Non-generic success          |
| 9   | `NonGeneric_Failure_ShouldNotBeSuccessful`      | Non-generic failure          |
| 10  | `NonGeneric_Unauthorized_ShouldSetFlag`         | Non-generic unauthorized     |
| 11  | `NonGeneric_Forbidden_ShouldSetFlag`            | Non-generic forbidden        |
| 12  | `NonGeneric_NotFound_ShouldSetFlag`             | Non-generic not found        |
| 13  | `NonGeneric_Conflict_ShouldSetFlag`             | Non-generic conflict         |

---

## 3. Infrastructure.UnitTests (42 tests)

Tests for identity services, token handling, and configuration settings.

### 3.1 Identity

#### `TokenServiceTests` — [Identity/TokenServiceTests.cs](tests/Infrastructure.UnitTests/Identity/TokenServiceTests.cs)

| #   | Test                                                               | Description                         |
| --- | ------------------------------------------------------------------ | ----------------------------------- |
| 1   | `GenerateAccessTokenAsync_ShouldReturnValidToken`                  | Generates non-empty JWT             |
| 2   | `GenerateAccessTokenAsync_ShouldContainCorrectClaims`              | JWT contains sub, email, name, role |
| 3   | `GenerateAccessTokenAsync_WithMultipleRoles_ShouldContainAllRoles` | Multi-role support                  |
| 4   | `GenerateAccessTokenAsync_WithAdditionalClaims_ShouldIncludeThem`  | Custom claims included              |
| 5   | `GenerateAccessTokenAsync_ExpiresAt_ShouldMatchSettings`           | Expiry matches JwtSettings          |
| 6   | `GenerateAccessTokenAsync_Jti_ShouldBeUniquePerCall`               | Unique JTI per token                |
| 7   | `GenerateAccessTokenAsync_TokenFormat_ShouldBeValidJwt`            | JWT has 3 dot-separated parts       |
| 8   | `GenerateRefreshToken_ShouldReturnNonEmptyString`                  | Non-empty refresh token             |
| 9   | `GenerateRefreshToken_ShouldReturnBase64String`                    | Base64-encoded output               |
| 10  | `GenerateRefreshToken_ShouldReturn64BytesWhenDecoded`              | 64-byte cryptographic random        |
| 11  | `GenerateRefreshToken_ShouldGenerateUniqueTokens`                  | Cryptographic uniqueness            |
| 12  | `ValidateToken_WithValidToken_ShouldReturnPrincipal`               | Valid JWT returns principal         |
| 13  | `ValidateToken_WithInvalidToken_ShouldReturnNull`                  | Invalid JWT returns null            |
| 14  | `ValidateToken_WithEmptyString_ShouldReturnNull`                   | Empty string returns null           |
| 15  | `ValidateToken_WithTamperedToken_ShouldReturnNull`                 | Tampered JWT detected               |
| 16  | `GetUserIdFromToken_WithValidToken_ShouldReturnUserId`             | Extracts user ID from JWT           |
| 17  | `GetUserIdFromToken_WithInvalidToken_ShouldReturnNull`             | Invalid token returns null          |
| 18  | `GetJtiFromToken_WithValidToken_ShouldReturnJti`                   | Extracts JTI from JWT               |
| 19  | `GetJtiFromToken_WithInvalidToken_ShouldReturnNull`                | Invalid token returns null          |
| 20  | `HashToken_ShouldReturnNonEmptyString`                             | SHA256 hash is non-empty            |
| 21  | `HashToken_SameInput_ShouldReturnSameHash`                         | Deterministic hashing               |
| 22  | `HashToken_DifferentInputs_ShouldReturnDifferentHashes`            | Different inputs → different hashes |
| 23  | `HashToken_ShouldReturnBase64String`                               | Result is Base64 encoded            |
| 24  | `HashToken_ShouldReturn32BytesWhenDecoded`                         | SHA256 produces 32 bytes            |

#### `JwtSettingsTests` — [Identity/JwtSettingsTests.cs](tests/Infrastructure.UnitTests/Identity/JwtSettingsTests.cs)

| #   | Test                              | Description                    |
| --- | --------------------------------- | ------------------------------ |
| 1   | `SectionName_ShouldBeJwtSettings` | Config section name is correct |
| 2   | `DefaultValues_ShouldBeCorrect`   | Default values match expected  |
| 3   | `Properties_ShouldBeSettable`     | All properties can be set      |

### 3.2 Settings

#### `SmtpSettingsTests` — [Settings/SettingsTests.cs](tests/Infrastructure.UnitTests/Settings/SettingsTests.cs)

| #   | Test                            | Description                      |
| --- | ------------------------------- | -------------------------------- |
| 1   | `SectionName_ShouldBeSmtp`      | Config section name              |
| 2   | `DefaultValues_ShouldBeCorrect` | Port 587, StartTLS enabled, etc. |
| 3   | `Properties_ShouldBeSettable`   | All SMTP properties settable     |

#### `PayOSSettingsTests` — [Settings/SettingsTests.cs](tests/Infrastructure.UnitTests/Settings/SettingsTests.cs)

| #   | Test                          | Description                   |
| --- | ----------------------------- | ----------------------------- |
| 1   | `SectionName_ShouldBePayOS`   | Config section name           |
| 2   | `DefaultValues_ShouldBeEmpty` | All fields default to empty   |
| 3   | `Properties_ShouldBeSettable` | All PayOS properties settable |

#### `SupabaseStorageSettingsTests` — [Settings/SettingsTests.cs](tests/Infrastructure.UnitTests/Settings/SettingsTests.cs)

| #   | Test                                  | Description                      |
| --- | ------------------------------------- | -------------------------------- |
| 1   | `SectionName_ShouldBeSupabaseStorage` | Config section name              |
| 2   | `DefaultValues_ShouldBeEmpty`         | All fields default to empty      |
| 3   | `Properties_ShouldBeSettable`         | All Supabase properties settable |

### 3.3 Services

#### `DateTimeServiceTests` — [Services/DateTimeServiceTests.cs](tests/Infrastructure.UnitTests/Services/DateTimeServiceTests.cs)

| #   | Test                                            | Description                    |
| --- | ----------------------------------------------- | ------------------------------ |
| 1   | `Now_ShouldReturnCurrentLocalTime`              | Returns current local DateTime |
| 2   | `UtcNow_ShouldReturnCurrentUtcTime`             | Returns current UTC DateTime   |
| 3   | `Now_ShouldReturnLocalKind`                     | DateTimeKind is Local          |
| 4   | `UtcNow_ShouldReturnUtcKind`                    | DateTimeKind is Utc            |
| 5   | `Now_ConsecutiveCalls_ShouldBeNonDecreasing`    | Monotonically non-decreasing   |
| 6   | `UtcNow_ConsecutiveCalls_ShouldBeNonDecreasing` | Monotonically non-decreasing   |

---

## 4. API.UnitTests (39 tests)

Tests for API controllers and HTTP response handling.

### 4.1 Controllers

#### `BaseApiControllerTests` — [Controllers/BaseApiControllerTests.cs](tests/API.UnitTests/Controllers/BaseApiControllerTests.cs)

| #   | Test                                                             | Description                      |
| --- | ---------------------------------------------------------------- | -------------------------------- |
| 1   | `HandleResult_Generic_Success_ShouldReturnOk200`                 | Success → 200 OK                 |
| 2   | `HandleResult_Generic_Success_ShouldContainData`                 | Response contains data + message |
| 3   | `HandleResult_Generic_Unauthorized_ShouldReturn401`              | Unauthorized → 401               |
| 4   | `HandleResult_Generic_Forbidden_ShouldReturn403`                 | Forbidden → 403                  |
| 5   | `HandleResult_Generic_NotFound_ShouldReturn404`                  | NotFound → 404                   |
| 6   | `HandleResult_Generic_Conflict_ShouldReturn409`                  | Conflict → 409                   |
| 7   | `HandleResult_Generic_Failure_ShouldReturn400`                   | Failure → 400 Bad Request        |
| 8   | `HandleResult_Generic_FailureWithMultipleErrors_ShouldReturn400` | Multiple errors → 400            |
| 9   | `HandleResult_NonGeneric_Success_ShouldReturnOk200`              | Non-generic success → 200        |
| 10  | `HandleResult_NonGeneric_Unauthorized_ShouldReturn401`           | Non-generic → 401                |
| 11  | `HandleResult_NonGeneric_Forbidden_ShouldReturn403`              | Non-generic → 403                |
| 12  | `HandleResult_NonGeneric_NotFound_ShouldReturn404`               | Non-generic → 404                |
| 13  | `HandleResult_NonGeneric_Conflict_ShouldReturn409`               | Non-generic → 409                |
| 14  | `HandleResult_NonGeneric_Failure_ShouldReturn400`                | Non-generic → 400                |
| 15  | `OkResponse_WithData_ShouldReturnOk200`                          | OkResponse with data             |
| 16  | `OkResponse_WithoutData_ShouldReturnOk200`                       | OkResponse message only          |
| 17  | `ErrorResponse_Default_ShouldReturn400`                          | Default error → 400              |
| 18  | `ErrorResponse_CustomStatusCode_ShouldReturnSpecifiedCode`       | Custom status code               |
| 19  | `InternalError_ShouldReturn500`                                  | Internal error → 500             |
| 20  | `InternalError_CustomMessage_ShouldReturn500WithMessage`         | Custom 500 message               |

#### `WalletsControllerTests` — [Controllers/WalletsControllerTests.cs](tests/API.UnitTests/Controllers/WalletsControllerTests.cs)

| #   | Test                                                              | Description                      |
| --- | ----------------------------------------------------------------- | -------------------------------- |
| 1   | `GetWallet_WhenUserNotAuthenticated_ShouldReturn401`              | Unauthenticated → 401            |
| 2   | `GetWallet_WhenAuthenticated_ShouldSendQuery`                     | Sends GetWalletQuery via MediatR |
| 3   | `GetWallet_WhenNotFound_ShouldReturn404`                          | Missing wallet → 404             |
| 4   | `GetTransactions_WhenUserNotAuthenticated_ShouldReturn401`        | Unauthenticated → 401            |
| 5   | `GetTransactions_WhenAuthenticated_ShouldSendQueryWithPagination` | Pagination parameters forwarded  |
| 6   | `GetDepositHistory_WhenUserNotAuthenticated_ShouldReturn401`      | Unauthenticated → 401            |
| 7   | `GetDepositRequest_WhenUserNotAuthenticated_ShouldReturn401`      | Unauthenticated → 401            |
| 8   | `CreateDeposit_WhenUserNotAuthenticated_ShouldReturn401`          | Unauthenticated → 401            |
| 9   | `VerifyPayment_WhenUserNotAuthenticated_ShouldReturn401`          | Unauthenticated → 401            |
| 10  | `PayOSWebhook_WhenNotSuccessful_ShouldReturnOk`                   | Failed webhook → 200 (ack)       |
| 11  | `PayOSWebhook_WhenDataIsNull_ShouldReturnOk`                      | Null data webhook → 200 (ack)    |

#### `PatientProfileControllerTests` — [Controllers/PatientProfileControllerTests.cs](tests/API.UnitTests/Controllers/PatientProfileControllerTests.cs)

| #   | Test                                                     | Description                  |
| --- | -------------------------------------------------------- | ---------------------------- |
| 1   | `GetProfile_WhenUserNotAuthenticated_ShouldReturn401`    | Unauthenticated → 401        |
| 2   | `GetProfile_WhenAuthenticated_ShouldSendQuery`           | Sends GetPatientProfileQuery |
| 3   | `GetProfile_WhenNotFound_ShouldReturn404`                | Missing profile → 404        |
| 4   | `UpdateProfile_WhenUserNotAuthenticated_ShouldReturn401` | Unauthenticated → 401        |
| 5   | `UploadAvatar_WhenUserNotAuthenticated_ShouldReturn401`  | Unauthenticated → 401        |
| 6   | `UploadAvatar_WhenEmptyFile_ShouldReturn400`             | Empty file → 400             |
| 7   | `UploadAvatar_WhenInvalidFileType_ShouldReturn400`       | Invalid MIME type → 400      |
| 8   | `UploadAvatar_WhenFileTooLarge_ShouldReturn400`          | File > 5MB → 400             |

---

## Architecture Coverage

```
┌─────────────────────────────────────────────────────────────┐
│                        API Layer (39)                        │
│  Controllers: BaseApiController, Wallets, PatientProfile     │
├─────────────────────────────────────────────────────────────┤
│                   Application Layer (41)                     │
│  Validators, Handlers, Behaviors, Result Model               │
├─────────────────────────────────────────────────────────────┤
│                  Infrastructure Layer (42)                    │
│  TokenService, DateTimeService, JwtSettings, Settings        │
├─────────────────────────────────────────────────────────────┤
│                      Domain Layer (108)                      │
│  Entities, Value Objects, Base Classes, Exceptions           │
└─────────────────────────────────────────────────────────────┘
```
