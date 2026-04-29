# AuraEyes Backend ERD (All-in-One)

This document provides a single, unified Mermaid ER diagram that combines all modules from the backend model.

Sources:
- `src/Infrastructure/Persistence/ApplicationDbContext.cs`
- `src/Infrastructure/Persistence/Configurations/**`
- `src/Infrastructure/Persistence/Migrations/ApplicationDbContextModelSnapshot.cs`

```mermaid
erDiagram
    %% =========================
    %% Clinical Core
    %% =========================
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

    %% =========================
    %% Identity, Authorization, Financial, Contracts, Platform
    %% =========================
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

    %% =========================
    %% Professional Network
    %% =========================
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

    %% =========================
    %% Relationships - Clinical Core
    %% =========================
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

    %% =========================
    %% Relationships - Identity, Authorization, Financial, Contracts
    %% =========================
    AspNetUsers ||--o{ RefreshToken : owns

    AspNetRoles ||--o{ RolePermission : grants
    Permission ||--o{ RolePermission : mapped_to
    Permission ||--o{ UserPermission : overridden_by

    Wallet ||--o{ WalletTransaction : records
    Wallet ||--o{ DepositRequest : topup_requests
    Wallet ||--o{ WithdrawalRequest : payout_requests

    Order ||--o{ Payment : has

    ContractTemplate ||--o{ Contract : instantiates

    %% =========================
    %% Relationships - Professional Network
    %% =========================
    ProfessionalPost o|--o{ ProfessionalPost : repost_of
    ProfessionalPost ||--o{ PostReaction : has
    ProfessionalPost ||--o{ PostComment : has
    PostComment o|--o{ PostComment : parent_of
    ProfessionalPost ||--o{ PostAttachment : has
    ProfessionalPost ||--o{ SavedPost : bookmarked_as
```

## Notes

This all-in-one diagram keeps the same relationship coverage as `docs/ERD.md`, but merged into one Mermaid `erDiagram` block for a single-system view.
