# Database Table Relationships

Source: `src/Infrastructure/Persistence/Migrations/ApplicationDbContextModelSnapshot.cs`

## Summary

- Total tables: 52
- Tables participating in FK relationships: 46
- Total FK relationships: 58
- Generated at (UTC): 2026-04-20 18:38:20

## Foreign Key Matrix

| # | Dependent table | FK column(s) | Principal table | Relationship | On delete | Required |
|---|---|---|---|---|---|---|
| 1 | AiScreenings | OrganisationId | Organisations | Many-to-one (inverse one-to-many) | Restrict | Optional/Implicit |
| 2 | AiScreenings | PatientId | Patients | Many-to-one (inverse one-to-many) | Restrict | Required |
| 3 | Appointments | AppointmentSlotId | AppointmentSlots | Many-to-one (inverse one-to-many) | Restrict | Required |
| 4 | Appointments | ConsultationSessionId | ConsultationSessions | Many-to-one (inverse one-to-many) | SetNull | Optional/Implicit |
| 5 | Appointments | DoctorId | Ophthalmologists | Many-to-one (inverse one-to-many) | SetNull | Optional/Implicit |
| 6 | Appointments | OrganisationId | Organisations | Many-to-one (inverse one-to-many) | Restrict | Optional/Implicit |
| 7 | Appointments | PatientId | Patients | Many-to-one (inverse one-to-many) | Restrict | Required |
| 8 | AppointmentSlots | ScheduleTemplateId | ScheduleTemplates | Many-to-one (inverse one-to-many) | Restrict | Required |
| 9 | AspNetRoleClaims | RoleId | AspNetRoles | Many-to-one (inverse one-to-many) | Cascade | Required |
| 10 | AspNetUserClaims | UserId | AspNetUsers | Many-to-one (inverse one-to-many) | Cascade | Required |
| 11 | AspNetUserLogins | UserId | AspNetUsers | Many-to-one (inverse one-to-many) | Cascade | Required |
| 12 | AspNetUserRoles | RoleId | AspNetRoles | Many-to-one (inverse one-to-many) | Cascade | Required |
| 13 | AspNetUserRoles | UserId | AspNetUsers | Many-to-one (inverse one-to-many) | Cascade | Required |
| 14 | AspNetUserTokens | UserId | AspNetUsers | Many-to-one (inverse one-to-many) | Cascade | Required |
| 15 | Certificates | OphthalmologistId | Ophthalmologists | Many-to-one (inverse one-to-many) | Cascade | Required |
| 16 | ChatMessages | ConversationId | Conversations | Many-to-one (inverse one-to-many) | Cascade | Required |
| 17 | Consents | AiScreeningId | AiScreenings | One-to-one | Restrict | Required |
| 18 | Consents | PatientId | Patients | Many-to-one (inverse one-to-many) | Restrict | Required |
| 19 | ConsultationSessions | AiScreeningId | AiScreenings | Many-to-one (inverse one-to-many) | Restrict | Optional/Implicit |
| 20 | ConsultationSessions | AppointmentSlotId | AppointmentSlots | Many-to-one (inverse one-to-many) | Restrict | Optional/Implicit |
| 21 | ConsultationSessions | OphthalmologistId | Ophthalmologists | Many-to-one (inverse one-to-many) | Restrict | Optional/Implicit |
| 22 | ConsultationSessions | OrganisationId | Organisations | Many-to-one (inverse one-to-many) | Restrict | Optional/Implicit |
| 23 | ConsultationSessions | PatientId | Patients | Many-to-one (inverse one-to-many) | Restrict | Required |
| 24 | Contracts | TemplateId | ContractTemplates | Many-to-one (inverse one-to-many) | Restrict | Required |
| 25 | Conversations | ConsultationSessionId | ConsultationSessions | Many-to-one (inverse one-to-many) | Restrict | Required |
| 26 | Conversations | OphthalmologistId | Ophthalmologists | Many-to-one (inverse one-to-many) | Restrict | Required |
| 27 | DepositRequests | WalletId | Wallets | Many-to-one (inverse one-to-many) | Restrict | Required |
| 28 | MedicalDiagnoses | AiScreeningId | AiScreenings | Many-to-one (inverse one-to-many) | Restrict | Required |
| 29 | MedicalDiagnoses | ConsultationSessionId | ConsultationSessions | Many-to-one (inverse one-to-many) | Restrict | Required |
| 30 | OphthalmologistEmploymentTypeChangeRequests | OphthalmologistId | Ophthalmologists | Many-to-one (inverse one-to-many) | Restrict | Required |
| 31 | OphthalmologistFeedback | ConsultationSessionId | ConsultationSessions | Many-to-one (inverse one-to-many) | Restrict | Required |
| 32 | OphthalmologistFeedback | OphthalmologistId | Ophthalmologists | Many-to-one (inverse one-to-many) | Restrict | Required |
| 33 | OphthalmologistFeedback | PatientId | Patients | Many-to-one (inverse one-to-many) | Restrict | Required |
| 34 | OphthalmologistLeaveRequests | OphthalmologistId | Ophthalmologists | Many-to-one (inverse one-to-many) | Restrict | Required |
| 35 | OrganisationFeedback | AppointmentId | Appointments | Many-to-one (inverse one-to-many) | Restrict | Required |
| 36 | OrganisationFeedback | OrganisationId | Organisations | Many-to-one (inverse one-to-many) | Restrict | Required |
| 37 | OrganisationFeedback | PatientId | Patients | Many-to-one (inverse one-to-many) | Restrict | Required |
| 38 | OrganisationPatientLinks | OrganisationId | Organisations | Many-to-one (inverse one-to-many) | Restrict | Required |
| 39 | OrganisationPatientLinks | PatientId | Patients | Many-to-one (inverse one-to-many) | Restrict | Required |
| 40 | PatientRoadmaps | MedicalDiagnosisId | MedicalDiagnoses | Many-to-one (inverse one-to-many) | Restrict | Required |
| 41 | PatientRoadmaps | PatientId | Patients | Many-to-one (inverse one-to-many) | Restrict | Required |
| 42 | Payments | OrderId | Orders | Many-to-one (inverse one-to-many) | Restrict | Required |
| 43 | PostAttachments | PostId | ProfessionalPosts | Many-to-one (inverse one-to-many) | Cascade | Required |
| 44 | PostComments | ParentCommentId | PostComments | Many-to-one (inverse one-to-many) | Cascade | Optional/Implicit |
| 45 | PostComments | PostId | ProfessionalPosts | Many-to-one (inverse one-to-many) | Cascade | Required |
| 46 | PostReactions | PostId | ProfessionalPosts | Many-to-one (inverse one-to-many) | Cascade | Required |
| 47 | ProfessionalPosts | OriginalPostId | ProfessionalPosts | Many-to-one (inverse one-to-many) | SetNull | Optional/Implicit |
| 48 | RefreshTokens | UserId | AspNetUsers | Many-to-one (inverse one-to-many) | Cascade | Required |
| 49 | RetinalImages | AiScreeningId | AiScreenings | Many-to-one (inverse one-to-many) | Restrict | Optional/Implicit |
| 50 | RetinalImages | PatientId | Patients | Many-to-one (inverse one-to-many) | Restrict | Required |
| 51 | RolePermissions | RoleId | AspNetRoles | Many-to-one (inverse one-to-many) | Cascade | Required |
| 52 | RolePermissions | PermissionId | Permissions | Many-to-one (inverse one-to-many) | Restrict | Required |
| 53 | SavedPosts | PostId | ProfessionalPosts | Many-to-one (inverse one-to-many) | Cascade | Required |
| 54 | ScreeningResults | AiScreeningId | AiScreenings | Many-to-one (inverse one-to-many) | Restrict | Required |
| 55 | UserPermissions | PermissionId | Permissions | Many-to-one (inverse one-to-many) | Restrict | Required |
| 56 | WalletTransactions | WalletId | Wallets | Many-to-one (inverse one-to-many) | Restrict | Required |
| 57 | WebsiteFeedback | PatientId | Patients | Many-to-one (inverse one-to-many) | Restrict | Required |
| 58 | WithdrawalRequests | WalletId | Wallets | Many-to-one (inverse one-to-many) | Restrict | Required |

## Tables Without Explicit Foreign Keys

- AuditLogs
- ExperiencePricingRules
- Notifications
- OrganisationOnboardingRequests
- SystemSettings
- WorkloadRequirements

## Notes

- This report reflects explicit EF Core relationships defined in the current model snapshot.
- Tables can still be related logically in application code without an explicit database FK constraint.


