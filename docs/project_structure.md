# AURA Digital Clinic - Project Structure Map

This document provides a comprehensive overview of the AURA project's file hierarchy for both the Backend and Frontend.

---

## 🏗️ Backend: AuraEyes_BE (Clean Architecture)

Location: `d:\ki9-lastdancefpt\Aura\BE\AuraEyes_BE\src`

### 1. Domain Layer (`Domain`)
*The core of the system, contains business entities and rules. No dependencies on other layers.*

- **Entities**:
  - `Authorization/`: Role, Permission, UserRole, RolePermission.
  - `Consultation/`: ConsultationSession, SessionFeedback, ChatMessage.
  - `Contracts/`: Contract, ContractTemplate, Agreement.
  - `Financial/`: Wallet, Transaction, PaymentLink.
  - `MedicalRecords/`: **MedicalRecord**, MedicalRecordStatus.
  - `Network/`: ProfessionalPost, NetworkMember, CollaborationGroup.
  - `Platform/`: Organisation, Clinic, SystemSettings.
  - `Scheduling/`: Appointment, AppointmentSlot, ScheduleTemplate, **PatientVisit**.
  - `Screening/`: AiScreening, ScreeningResult, RetinalImage.
  - `Users/`: Patient, Ophthalmologist, ClinicStaff, ApplicationUser.
- **Enums**: `PatientVisitStatus`, `MedicalRecordStatus`, `SessionStatus`, etc.
- **Repositories**: Interfaces like `IMedicalRecordRepository`, `IPatientVisitRepository`.
- **Common**: `BaseEntity`, `IAggregateRoot`.

### 2. Application Layer (`Application`)
*Contains business logic via CQRS (MediatR). Depends only on Domain.*

- **CQRS (Feature-based)**:
  - `Auth/`: Commands (Login, Register), Queries.
  - `MedicalRecords/`:
    - `Commands/`: `CreateMedicalRecord`, `UpdateMedicalRecordClinical`, `FinalizeMedicalRecord`.
    - `Queries/`: `GetMedicalRecordById`, `GetPatientMedicalRecords`.
  - `Scheduling/`: `CheckInClinicAppointment`, `ScheduleAppointment`.
  - `ClinicQueue/`: `GetClinicQueue`.
- **Common**:
  - `Interfaces/`: `IApplicationDbContext`, `IEmailService`, `IFileStorageService`, `IMedicalRecordPdfService`.
  - `Models/`: `Result`, `PagedList`.
  - `Mappings/`: AutoMapper profiles (e.g., `MedicalRecordProfile`).
  - `Validators/`: FluentValidation rules.
- **DTOs**: `MedicalRecordDto`, `ClinicQueueItemDto`, `PatientVisitDto`.

### 3. Infrastructure Layer (`Infrastructure`)
*Implementation of interfaces defined in Application/Domain. Depends on Application.*

- **Persistence**:
  - `ApplicationDbContext`: EF Core context.
  - `Configurations/`:
    - `MedicalRecordConfiguration.cs`, `ApplicationUserConfiguration.cs`, `InternalGroupChatConfiguration.cs`.
    - Subfolders for: `Authorization/`, `Consultation/`, `Contracts/`, `Financial/`, `Network/`, `Platform/`, `Scheduling/`, `Screening/`, `Users/`.
  - `Repositories/`: `MedicalRecordRepository`, `PatientVisitRepository`, `AppointmentRepository`.
  - `Migrations/`: Database migration history.
- **Services**:
  - `FileStorage/`: `CloudinaryFileStorageService.cs`.
  - `Email/`: `EmailService.cs`.
  - `Pdf/`: **MedicalRecordPdfService.cs** (QuestPDF).
  - `Notification/`: `NotificationService.cs`, `SignalRHubs/`.
- **Identity**: `IdentityService.cs`, `CurrentUserService.cs`.

### 4. API Layer (`API`)
*Entry point of the system. Depends on Application and Infrastructure.*

- **Controllers**:
  - `MedicalRecordsController.cs`: Endpoints for EMR management.
  - `ClinicQueueController.cs`: Queue management for staff.
  - `PatientsController.cs`: Patient-facing endpoints.
  - `ConsultationSessionsController.cs`: Chat and consultation logic.
- **Middleware**: Error handling, Authentication, Logging.

---

## 🎨 Frontend: AURA-FE (Feature-based React)

Location: `d:\ki9-lastdancefpt\Aura\FE\AURA-FE\src`

### 1. Features (`features/`)
*Modular organization by business domain. Each feature has its own API, components, and state.*

- **auth**: Login/Register logic, JWT handling.
- **medical-records**:
  - `api/`: `medical-record.api.ts`.
  - `pages/`: `ErmForm.tsx`, `ErmFormPatient.tsx`.
- **patient**:
  - `pages/`: `dashboard.tsx`, `appointments.tsx`, `medical-history.tsx` [NEW].
  - `components/`: `PatientLayout.tsx`, `PatientSidebar.tsx`.
- **clinic-staff**:
  - `pages/`: `queue.tsx`, `dashboard.tsx`.
- **ophthalmologist**:
  - `pages/`: `ConsultationsChatView.tsx`, `screening-review.tsx`.
- **professional-network**: Social feed and collab tools.
- **system-admin**: Management dashboards.

### 2. Core Modules
- **components/ui**: Shared atomic components (Button, Input, Modal, Spinner).
- **lib**: Utility libraries.
- **routes**: `index.tsx` (Main router configuration).
- **store**: `auth-store.ts`, `app-store.ts` (Zustand state).
- **i18n**: Multi-language support (EN/VI).
- **types**: TypeScript interfaces and types.
- **styles**: Tailwind CSS and global themes.

---

## 🧬 Data Flow (Relay Integration)

1. **Receptionist** (`clinic-staff/queue.tsx`) ➡️ `CheckInClinicAppointmentCommand` (BE) ➡️ Creates `MedicalRecord`.
2. **Doctor** (`medical-records/ErmForm.tsx`) ➡️ `UpdateMedicalRecordClinicalCommand` (BE) ➡️ Sets Visit to `WaitingForPayment`.
3. **Staff/Doctor** ➡️ `FinalizeMedicalRecordCommand` (BE) ➡️ Generates PDF ➡️ Sends Email ➡️ Notifies Patient.
4. **Patient** (`patient/medical-history.tsx`) ➡️ `GetPatientMedicalRecordsQuery` (BE) ➡️ Views history/Downloads PDF.
