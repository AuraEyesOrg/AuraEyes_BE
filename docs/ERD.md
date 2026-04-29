# AuraEyes Backend ERD

This document summarizes the database model from:
- `src/Infrastructure/Persistence/ApplicationDbContext.cs`
- EF Core configurations in `src/Infrastructure/Persistence/Configurations/**`
- EF model snapshot `src/Infrastructure/Persistence/Migrations/ApplicationDbContextModelSnapshot.cs`

Scope and conventions:
- Diagram relations represent explicit EF Core foreign-key mappings.
- Some columns are ID references by design without DB-level FK constraints (listed in Notes).
- Most tables inherit audit + soft-delete fields from `BaseEntity` (`Id`, `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`, `IsDeleted`).

## 1) Clinical Core (Users, Screening, Consultation, Scheduling)

```mermaid
erDiagram
    Patient {
      uuid Id PK
      uuid UserId UK_nullable
    }

    Organisation {
      uuid Id PK
      uuid OwnerId
    }

    Ophthalmologist {
      uuid Id PK
      uuid UserId UK
    }

    OrganisationPatientLink {
      uuid Id PK
      uuid OrganisationId FK
      uuid PatientId FK
    }

    Certificate {
      uuid Id PK
      uuid OphthalmologistId FK
    }

    Consent {
      uuid Id PK
      uuid AiScreeningId FK
      uuid PatientId FK
    }

    AiScreening {
      uuid Id PK
      uuid PatientId FK
      uuid OrganisationId FK_nullable
    }

    RetinalImage {
      uuid Id PK
      uuid PatientId FK
      uuid AiScreeningId FK_nullable
    }

    ScreeningResult {
      uuid Id PK
      uuid AiScreeningId FK
    }

    MedicalDiagnosis {
      uuid Id PK
      uuid AiScreeningId FK
      uuid ConsultationSessionId FK
      uuid DoctorId
    }

    PatientRoadmap {
      uuid Id PK
      uuid PatientId FK
      uuid MedicalDiagnosisId FK_Unique
    }

    ConsultationSession {
      uuid Id PK
      uuid PatientId FK
      uuid OphthalmologistId FK_nullable
      uuid OrganisationId FK_nullable
      uuid AiScreeningId FK_nullable
      uuid AppointmentSlotId FK_nullable
    }

    Conversation {
      uuid Id PK
      uuid OphthalmologistId FK
      uuid ConsultationSessionId FK
    }

    ChatMessage {
      uuid Id PK
      uuid ConversationId FK
      uuid SenderUserId
    }

    ScheduleTemplate {
      uuid Id PK
      uuid OrgId_ref_nullable
      uuid OphthalId_ref_nullable
    }

    AppointmentSlot {
      uuid Id PK
      uuid ScheduleTemplateId FK
    }

    Appointment {
      uuid Id PK
      uuid PatientId FK
      uuid AppointmentSlotId FK
      uuid OrganisationId FK_nullable
      uuid DoctorId FK_nullable
      uuid ConsultationSessionId FK_nullable
    }

    WebsiteFeedback {
      uuid Id PK
      uuid PatientId FK
    }

    OrganisationFeedback {
      uuid Id PK
      uuid PatientId FK
      uuid OrganisationId FK
      uuid AppointmentId FK
    }

    OphthalmologistFeedback {
      uuid Id PK
      uuid PatientId FK
      uuid OphthalmologistId FK
      uuid ConsultationSessionId FK
    }

    OphthalmologistLeaveRequest {
      uuid Id PK
      uuid OphthalmologistId FK
    }

    OphthalmologistEmploymentTypeChangeRequest {
      uuid Id PK
      uuid OphthalmologistId FK
    }

    Organisation ||--o{ OrganisationPatientLink : links
    Patient ||--o{ OrganisationPatientLink : links

    Ophthalmologist ||--o{ Certificate : owns

    Patient ||--o{ AiScreening : owns
    Organisation o|--o{ AiScreening : scoped_to

    Patient ||--o{ RetinalImage : captures
    AiScreening o|--o{ RetinalImage : groups
    AiScreening ||--o{ ScreeningResult : produces

    AiScreening ||--o| Consent : has
    Patient ||--o{ Consent : signs

    Patient ||--o{ ConsultationSession : starts
    Ophthalmologist o|--o{ ConsultationSession : assigned
    Organisation o|--o{ ConsultationSession : via_org
    AiScreening o|--o{ ConsultationSession : source
    AppointmentSlot o|--o{ ConsultationSession : scheduled_in

    ConsultationSession ||--o{ Conversation : has
    Ophthalmologist ||--o{ Conversation : chats_in
    Conversation ||--o{ ChatMessage : contains

    ConsultationSession ||--o{ MedicalDiagnosis : results_in
    AiScreening ||--o{ MedicalDiagnosis : based_on
    MedicalDiagnosis o|--|| PatientRoadmap : roadmap
    Patient ||--o{ PatientRoadmap : receives

    ScheduleTemplate ||--o{ AppointmentSlot : generates
    AppointmentSlot ||--o{ Appointment : booked_as

    Patient ||--o{ Appointment : books
    Organisation o|--o{ Appointment : clinic_context
    Ophthalmologist o|--o{ Appointment : doctor
    ConsultationSession o|--o{ Appointment : linked_session

    Patient ||--o{ WebsiteFeedback : writes

    Patient ||--o{ OrganisationFeedback : writes
    Organisation ||--o{ OrganisationFeedback : receives
    Appointment ||--o{ OrganisationFeedback : about

    Patient ||--o{ OphthalmologistFeedback : writes
    Ophthalmologist ||--o{ OphthalmologistFeedback : receives
    ConsultationSession ||--o{ OphthalmologistFeedback : about

    Ophthalmologist ||--o{ OphthalmologistLeaveRequest : creates
    Ophthalmologist ||--o{ OphthalmologistEmploymentTypeChangeRequest : creates
```

## 2) Identity, Authorization, Financial, Contracts, Platform

```mermaid
erDiagram
    AspNetUsers {
      uuid Id PK
    }

    AspNetRoles {
      uuid Id PK
    }

    RefreshToken {
      uuid Id PK
      uuid UserId FK
    }

    Permission {
      uuid Id PK
      string Name UK
    }

    RolePermission {
      uuid Id PK
      uuid RoleId FK
      uuid PermissionId FK
    }

    UserPermission {
      uuid Id PK
      uuid UserId_ref
      uuid PermissionId FK
    }

    Wallet {
      uuid Id PK
      uuid UserId UK_ref
    }

    WalletTransaction {
      uuid Id PK
      uuid WalletId FK
    }

    DepositRequest {
      uuid Id PK
      uuid WalletId FK
      uuid UserId_ref
    }

    WithdrawalRequest {
      uuid Id PK
      uuid WalletId FK
      uuid UserId_ref
    }

    Order {
      uuid Id PK
      uuid UserId_ref
    }

    Payment {
      uuid Id PK
      uuid OrderId FK
    }

    ContractTemplate {
      uuid Id PK
    }

    Contract {
      uuid Id PK
      uuid TemplateId FK
      uuid UserId_ref
    }

    Notification {
      uuid Id PK
      uuid UserId_ref
    }

    AuditLog {
      uuid Id PK
      uuid UserId_ref_nullable
    }

    SystemSetting {
      string Key PK
    }

    AspNetUsers ||--o{ RefreshToken : owns

    AspNetRoles ||--o{ RolePermission : grants
    Permission ||--o{ RolePermission : mapped_to
    Permission ||--o{ UserPermission : overridden_by

    Wallet ||--o{ WalletTransaction : records
    Wallet ||--o{ DepositRequest : topup_requests
    Wallet ||--o{ WithdrawalRequest : payout_requests

    Order ||--o{ Payment : has

    ContractTemplate ||--o{ Contract : instantiates
```

## 3) Professional Network

```mermaid
erDiagram
    ProfessionalPost {
      uuid Id PK
      uuid AuthorId_ref
      uuid OrganisationId_ref_nullable
      uuid OriginalPostId FK_nullable
      uuid ConsultationSessionId_ref_nullable
      uuid AiScreeningId_ref_nullable
    }

    PostReaction {
      uuid Id PK
      uuid PostId FK
      uuid UserId_ref
    }

    PostComment {
      uuid Id PK
      uuid PostId FK
      uuid AuthorId_ref
      uuid ParentCommentId FK_nullable
    }

    PostAttachment {
      uuid Id PK
      uuid PostId FK
    }

    SavedPost {
      uuid Id PK
      uuid PostId FK
      uuid UserId_ref
    }

    ProfessionalPost o|--o{ ProfessionalPost : repost_of
    ProfessionalPost ||--o{ PostReaction : has
    ProfessionalPost ||--o{ PostComment : has
    PostComment o|--o{ PostComment : parent_of
    ProfessionalPost ||--o{ PostAttachment : has
    ProfessionalPost ||--o{ SavedPost : bookmarked_as
```

## Notes: ID References Without Enforced FK

The following columns are used as references in business logic but are not enforced as DB-level FK constraints in current EF mapping:
- `Patient.UserId`
- `Ophthalmologist.UserId`
- `Organisation.OwnerId`
- `ScheduleTemplate.OrgId`, `ScheduleTemplate.OphthalId`
- `Wallet.UserId`
- `DepositRequest.UserId`, `WithdrawalRequest.UserId`
- `Order.UserId`
- `Contract.UserId`
- `UserPermission.UserId`
- `Notification.UserId`, `AuditLog.UserId`
- `ProfessionalPost.AuthorId`, `ProfessionalPost.OrganisationId`, `ProfessionalPost.ConsultationSessionId`, `ProfessionalPost.AiScreeningId`
- `PostReaction.UserId`, `PostComment.AuthorId`, `SavedPost.UserId`

## Important Constraints Worth Knowing

- Soft delete global filter: all `BaseEntity` tables default to `IsDeleted = false` in queries.
- `Consent` is one-to-one with `AiScreening` (`Consent.AiScreeningId` FK).
- `PatientRoadmap.MedicalDiagnosisId` has a unique index (at most one roadmap per diagnosis).
- `OrganisationPatientLink` is unique by (`OrganisationId`, `PatientId`) with filter `IsDeleted = false`.
- `Appointment` has a filtered unique index on (`PatientId`, `AppointmentSlotId`) for active statuses.
- `AppointmentSlot` has unique index on (`ScheduleTemplateId`, `Date`, `StartTime`, `EndTime`) with filter `IsDeleted = false`.
- `RolePermission` unique by (`RoleId`, `PermissionId`), `UserPermission` unique by (`UserId`, `PermissionId`).
- `PostReaction` unique by (`PostId`, `UserId`), `SavedPost` unique by (`UserId`, `PostId`).
- `OrganisationFeedback` unique by (`PatientId`, `AppointmentId`).
- `OphthalmologistFeedback` unique by (`PatientId`, `ConsultationSessionId`).

## Standalone Tables (No FK in Current Mapping)

These tables are independent in the current schema mapping:
- `OrganisationOnboardingRequests`
- `ExperiencePricingRules`
- `SystemSettings`

