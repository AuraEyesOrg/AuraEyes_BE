-- Demo seed data for clinic workflows (ASCII only)
-- Target schema: docs/database_script.sql

BEGIN;

-- Ophthalmologists (3)
INSERT INTO public.Ophthalmologists (
  Id, UserId, Bio, Phone, EmploymentType, YearsOfExperience,
  LicenseUrl, DegreeUrl, RatingAverage, RatingCount, ConsultationFee,
  CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted
) VALUES
  ('11111111-1111-1111-1111-111111111111', 'aaaa1111-0000-0000-0000-000000000001', 'Senior retina specialist', '+84900000001', 'FullTime', 12, NULL, NULL, 4.8, 120, 450000, NOW() - interval '30 days', NOW() - interval '1 days', 'seed', NULL, false),
  ('22222222-2222-2222-2222-222222222222', 'aaaa2222-0000-0000-0000-000000000002', 'General ophthalmologist', '+84900000002', 'PartTime', 6, NULL, NULL, 4.5, 70, 350000, NOW() - interval '20 days', NOW() - interval '1 days', 'seed', NULL, false),
  ('33333333-3333-3333-3333-333333333333', 'aaaa3333-0000-0000-0000-000000000003', 'New doctor (idle today)', '+84900000003', 'PartTime', 2, NULL, NULL, 4.2, 12, 250000, NOW() - interval '10 days', NOW() - interval '1 days', 'seed', NULL, false);

-- Patients (12)
INSERT INTO public.Patients (
  Id, UserId, FullName, PhoneNumber, CitizenId, DateOfBirth,
  GenderId, Address, MedicalRecordNumber, BMI, DiseaseHistory,
  CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted
) VALUES
  ('aaaaaaaa-0000-0000-0000-000000000001', NULL, 'Nguyen Van A', '0901000001', '012345000001', NOW() - interval '30 years', 1, 'Hanoi', 'MRN-0001', 22.1, 'None', NOW() - interval '90 days', NOW() - interval '1 days', 'seed', NULL, false),
  ('aaaaaaaa-0000-0000-0000-000000000002', NULL, 'Tran Thi B', '0901000002', '012345000002', NOW() - interval '28 years', 2, 'Hanoi', 'MRN-0002', 20.4, 'Myopia', NOW() - interval '80 days', NOW() - interval '1 days', 'seed', NULL, false),
  ('aaaaaaaa-0000-0000-0000-000000000003', NULL, 'Le Minh C', '0901000003', '012345000003', NOW() - interval '45 years', 1, 'Da Nang', 'MRN-0003', 24.9, 'Hypertension', NOW() - interval '70 days', NOW() - interval '1 days', 'seed', NULL, false),
  ('aaaaaaaa-0000-0000-0000-000000000004', NULL, 'Pham Thi D', '0901000004', '012345000004', NOW() - interval '36 years', 2, 'Hanoi', 'MRN-0004', 21.3, 'None', NOW() - interval '60 days', NOW() - interval '1 days', 'seed', NULL, false),
  ('aaaaaaaa-0000-0000-0000-000000000005', NULL, 'Vu Quang E', '0901000005', '012345000005', NOW() - interval '52 years', 1, 'Hai Phong', 'MRN-0005', 26.5, 'Diabetes', NOW() - interval '50 days', NOW() - interval '1 days', 'seed', NULL, false),
  ('aaaaaaaa-0000-0000-0000-000000000006', NULL, 'Hoang Thi F', '0901000006', '012345000006', NOW() - interval '41 years', 2, 'Hanoi', 'MRN-0006', 23.8, 'Dry eye', NOW() - interval '40 days', NOW() - interval '1 days', 'seed', NULL, false),
  ('aaaaaaaa-0000-0000-0000-000000000007', NULL, 'Nguyen Van G', '0901000007', '012345000007', NOW() - interval '33 years', 1, 'HCMC', 'MRN-0007', 25.2, 'None', NOW() - interval '35 days', NOW() - interval '1 days', 'seed', NULL, false),
  ('aaaaaaaa-0000-0000-0000-000000000008', NULL, 'Tran Thi H', '0901000008', '012345000008', NOW() - interval '29 years', 2, 'HCMC', 'MRN-0008', 19.9, 'Astigmatism', NOW() - interval '30 days', NOW() - interval '1 days', 'seed', NULL, false),
  ('aaaaaaaa-0000-0000-0000-000000000009', NULL, 'Le Minh I', '0901000009', '012345000009', NOW() - interval '60 years', 1, 'Hanoi', 'MRN-0009', 27.8, 'Diabetes', NOW() - interval '25 days', NOW() - interval '1 days', 'seed', NULL, false),
  ('aaaaaaaa-0000-0000-0000-00000000000a', NULL, 'Pham Thi K', '0901000010', '012345000010', NOW() - interval '39 years', 2, 'Da Nang', 'MRN-0010', 22.7, 'None', NOW() - interval '20 days', NOW() - interval '1 days', 'seed', NULL, false),
  ('aaaaaaaa-0000-0000-0000-00000000000b', NULL, 'Vu Quang L', '0901000011', '012345000011', NOW() - interval '47 years', 1, 'Hanoi', 'MRN-0011', 28.0, 'Hypertension', NOW() - interval '18 days', NOW() - interval '1 days', 'seed', NULL, false),
  ('aaaaaaaa-0000-0000-0000-00000000000c', NULL, 'Hoang Thi M', '0901000012', '012345000012', NOW() - interval '55 years', 2, 'Hai Phong', 'MRN-0012', 26.1, 'Cataract', NOW() - interval '15 days', NOW() - interval '1 days', 'seed', NULL, false);

-- Schedule templates (3)
INSERT INTO public.ScheduleTemplates (
  Id, DayOfWeek, StartTime, EndTime, SlotDuration, MaxCapacity, Cost,
  IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted
) VALUES
  ('t1111111-1111-1111-1111-111111111111', 'Monday', '08:00', '12:00', 30, 1, 300000, true, NOW() - interval '60 days', NOW() - interval '1 days', 'seed', NULL, false),
  ('t2222222-2222-2222-2222-222222222222', 'Monday', '13:30', '17:00', 30, 3, 300000, true, NOW() - interval '60 days', NOW() - interval '1 days', 'seed', NULL, false),
  ('t3333333-3333-3333-3333-333333333333', 'Monday', '08:00', '12:00', 30, 5, 350000, true, NOW() - interval '60 days', NOW() - interval '1 days', 'seed', NULL, false);

-- Appointment slots (12) for today
INSERT INTO public.AppointmentSlots (
  Id, ScheduleTemplateId, Date, StartTime, EndTime, Status, OphthalId,
  Cost, MaxCapacity, BookedCount, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted
) VALUES
  ('s1111111-0000-0000-0000-000000000001', 't1111111-1111-1111-1111-111111111111', CURRENT_DATE, '08:00', '08:30', 'Available', '11111111-1111-1111-1111-111111111111', 300000, 1, 1, NOW() - interval '2 hours', NOW() - interval '30 minutes', 'seed', NULL, false),
  ('s1111111-0000-0000-0000-000000000002', 't3333333-3333-3333-3333-333333333333', CURRENT_DATE, '08:30', '09:00', 'Available', '11111111-1111-1111-1111-111111111111', 350000, 3, 3, NOW() - interval '2 hours', NOW() - interval '30 minutes', 'seed', NULL, false),
  ('s1111111-0000-0000-0000-000000000003', 't3333333-3333-3333-3333-333333333333', CURRENT_DATE, '09:00', '09:30', 'Available', '11111111-1111-1111-1111-111111111111', 350000, 3, 2, NOW() - interval '2 hours', NOW() - interval '30 minutes', 'seed', NULL, false),
  ('s1111111-0000-0000-0000-000000000004', 't1111111-1111-1111-1111-111111111111', CURRENT_DATE, '09:30', '10:00', 'Available', '11111111-1111-1111-1111-111111111111', 300000, 1, 0, NOW() - interval '2 hours', NOW() - interval '30 minutes', 'seed', NULL, false),
  ('s1111111-0000-0000-0000-000000000005', 't3333333-3333-3333-3333-333333333333', CURRENT_DATE, '10:00', '10:30', 'Available', '11111111-1111-1111-1111-111111111111', 350000, 5, 0, NOW() - interval '2 hours', NOW() - interval '30 minutes', 'seed', NULL, false),
  ('s1111111-0000-0000-0000-000000000006', 't1111111-1111-1111-1111-111111111111', CURRENT_DATE, '10:30', '11:00', 'Available', '11111111-1111-1111-1111-111111111111', 300000, 1, 1, NOW() - interval '2 hours', NOW() - interval '30 minutes', 'seed', NULL, false),
  ('s2222222-0000-0000-0000-000000000007', 't2222222-2222-2222-2222-222222222222', CURRENT_DATE, '13:30', '14:00', 'Available', '22222222-2222-2222-2222-222222222222', 300000, 3, 1, NOW() - interval '3 hours', NOW() - interval '20 minutes', 'seed', NULL, false),
  ('s2222222-0000-0000-0000-000000000008', 't3333333-3333-3333-3333-333333333333', CURRENT_DATE, '14:00', '14:30', 'Available', '22222222-2222-2222-2222-222222222222', 350000, 5, 3, NOW() - interval '3 hours', NOW() - interval '20 minutes', 'seed', NULL, false),
  ('s2222222-0000-0000-0000-000000000009', 't2222222-2222-2222-2222-222222222222', CURRENT_DATE, '14:30', '15:00', 'Available', '22222222-2222-2222-2222-222222222222', 300000, 5, 1, NOW() - interval '3 hours', NOW() - interval '20 minutes', 'seed', NULL, false),
  ('s2222222-0000-0000-0000-00000000000a', 't2222222-2222-2222-2222-222222222222', CURRENT_DATE, '15:00', '15:30', 'Available', '22222222-2222-2222-2222-222222222222', 300000, 1, 0, NOW() - interval '3 hours', NOW() - interval '20 minutes', 'seed', NULL, false),
  ('s2222222-0000-0000-0000-00000000000b', 't2222222-2222-2222-2222-222222222222', CURRENT_DATE, '15:30', '16:00', 'Available', '22222222-2222-2222-2222-222222222222', 300000, 3, 0, NOW() - interval '3 hours', NOW() - interval '20 minutes', 'seed', NULL, false),
  ('s2222222-0000-0000-0000-00000000000c', 't2222222-2222-2222-2222-222222222222', CURRENT_DATE, '16:00', '16:30', 'Available', '22222222-2222-2222-2222-222222222222', 300000, 1, 0, NOW() - interval '3 hours', NOW() - interval '20 minutes', 'seed', NULL, false);

-- Appointments (12)
INSERT INTO public.Appointments (
  Id, PatientId, AppointmentSlotId, Status, VisitReason, CancelledBy,
  CancellationReason, RequestedDoctorId, PricingType, Price,
  CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted
) VALUES
  ('a0000000-0000-0000-0000-000000000001', 'aaaaaaaa-0000-0000-0000-000000000001', 's1111111-0000-0000-0000-000000000001', 'CheckedIn', 'Routine check', NULL, NULL, '11111111-1111-1111-1111-111111111111', 'DoctorSelected', 450000, NOW() - interval '40 minutes', NOW() - interval '30 minutes', 'seed', NULL, false),
  ('a0000000-0000-0000-0000-000000000002', 'aaaaaaaa-0000-0000-0000-000000000002', 's1111111-0000-0000-0000-000000000002', 'Confirmed', 'Blurred vision', NULL, NULL, '11111111-1111-1111-1111-111111111111', 'DoctorSelected', 450000, NOW() - interval '2 hours', NOW() - interval '1 hours', 'seed', NULL, false),
  ('a0000000-0000-0000-0000-000000000003', 'aaaaaaaa-0000-0000-0000-000000000003', 's1111111-0000-0000-0000-000000000002', 'Confirmed', 'Dry eye', NULL, NULL, NULL, 'AutoAssign', 300000, NOW() - interval '2 hours', NOW() - interval '1 hours', 'seed', NULL, false),
  ('a0000000-0000-0000-0000-000000000004', 'aaaaaaaa-0000-0000-0000-000000000004', 's1111111-0000-0000-0000-000000000002', 'Confirmed', 'Annual check', NULL, NULL, NULL, 'AutoAssign', 300000, NOW() - interval '2 hours', NOW() - interval '1 hours', 'seed', NULL, false),
  ('a0000000-0000-0000-0000-000000000005', 'aaaaaaaa-0000-0000-0000-000000000005', 's1111111-0000-0000-0000-000000000003', 'CheckedIn', 'Diabetes screening', NULL, NULL, '11111111-1111-1111-1111-111111111111', 'DoctorSelected', 450000, NOW() - interval '50 minutes', NOW() - interval '35 minutes', 'seed', NULL, false),
  ('a0000000-0000-0000-0000-000000000006', 'aaaaaaaa-0000-0000-0000-000000000006', 's1111111-0000-0000-0000-000000000003', 'Confirmed', 'Eye strain', NULL, NULL, NULL, 'AutoAssign', 300000, NOW() - interval '1 hours', NOW() - interval '50 minutes', 'seed', NULL, false),
  ('a0000000-0000-0000-0000-000000000007', 'aaaaaaaa-0000-0000-0000-000000000007', 's1111111-0000-0000-0000-000000000006', 'Completed', 'Cataract follow-up', NULL, NULL, '11111111-1111-1111-1111-111111111111', 'DoctorSelected', 450000, NOW() - interval '3 hours', NOW() - interval '2 hours', 'seed', NULL, false),
  ('a0000000-0000-0000-0000-000000000008', 'aaaaaaaa-0000-0000-0000-000000000008', 's2222222-0000-0000-0000-000000000007', 'Pending', 'Headache', NULL, NULL, '22222222-2222-2222-2222-222222222222', 'DoctorSelected', 350000, NOW() - interval '1 hours', NOW() - interval '55 minutes', 'seed', NULL, false),
  ('a0000000-0000-0000-0000-000000000009', 'aaaaaaaa-0000-0000-0000-000000000009', 's2222222-0000-0000-0000-000000000008', 'InProgress', 'Sudden vision loss', NULL, NULL, '11111111-1111-1111-1111-111111111111', 'DoctorSelected', 450000, NOW() - interval '35 minutes', NOW() - interval '10 minutes', 'seed', NULL, false),
  ('a0000000-0000-0000-0000-00000000000a', 'aaaaaaaa-0000-0000-0000-00000000000a', 's2222222-0000-0000-0000-000000000008', 'CheckedIn', 'Floaters', NULL, NULL, '11111111-1111-1111-1111-111111111111', 'DoctorSelected', 450000, NOW() - interval '30 minutes', NOW() - interval '25 minutes', 'seed', NULL, false),
  ('a0000000-0000-0000-0000-00000000000b', 'aaaaaaaa-0000-0000-0000-00000000000b', 's2222222-0000-0000-0000-000000000008', 'NoShow', 'Appointment only', NULL, NULL, NULL, 'AutoAssign', 300000, NOW() - interval '2 hours', NOW() - interval '1 hours', 'seed', NULL, false),
  ('a0000000-0000-0000-0000-00000000000c', 'aaaaaaaa-0000-0000-0000-00000000000c', 's2222222-0000-0000-0000-000000000009', 'Confirmed', 'Routine check', NULL, NULL, NULL, 'AutoAssign', 300000, NOW() - interval '1 hours', NOW() - interval '45 minutes', 'seed', NULL, false);

-- Patient visits (5 with appointments + 1 walk-in)
INSERT INTO public.PatientVisits (
  Id, AppointmentId, PatientId, AssignedDoctorId, CheckedInAt, StartedAt,
  CompletedAt, Status, Notes, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted
) VALUES
  -- Normal flow: checked-in, AI done, waiting for doctor
  ('v0000000-0000-0000-0000-000000000001', 'a0000000-0000-0000-0000-000000000001', 'aaaaaaaa-0000-0000-0000-000000000001', '11111111-1111-1111-1111-111111111111', NOW() - interval '5 minutes', NULL, NULL, 'CheckedIn', 'Waiting for doctor', NOW() - interval '5 minutes', NOW() - interval '2 minutes', 'seed', NULL, false),

  -- Urgent case: waiting > 15 minutes
  ('v0000000-0000-0000-0000-000000000002', 'a0000000-0000-0000-0000-000000000005', 'aaaaaaaa-0000-0000-0000-000000000005', '11111111-1111-1111-1111-111111111111', NOW() - interval '25 minutes', NULL, NULL, 'CheckedIn', 'High risk waiting', NOW() - interval '25 minutes', NOW() - interval '20 minutes', 'seed', NULL, false),

  -- In progress
  ('v0000000-0000-0000-0000-000000000003', 'a0000000-0000-0000-0000-000000000009', 'aaaaaaaa-0000-0000-0000-000000000009', '11111111-1111-1111-1111-111111111111', NOW() - interval '30 minutes', NOW() - interval '10 minutes', NULL, 'InProgress', 'Doctor reviewing', NOW() - interval '30 minutes', NOW() - interval '5 minutes', 'seed', NULL, false),

  -- Completed visit
  ('v0000000-0000-0000-0000-000000000004', 'a0000000-0000-0000-0000-000000000007', 'aaaaaaaa-0000-0000-0000-000000000007', '11111111-1111-1111-1111-111111111111', NOW() - interval '3 hours', NOW() - interval '2 hours', NOW() - interval '90 minutes', 'Completed', 'Visit completed', NOW() - interval '3 hours', NOW() - interval '80 minutes', 'seed', NULL, false),

  -- Waiting for payment
  ('v0000000-0000-0000-0000-000000000005', 'a0000000-0000-0000-0000-00000000000a', 'aaaaaaaa-0000-0000-0000-00000000000a', '11111111-1111-1111-1111-111111111111', NOW() - interval '40 minutes', NOW() - interval '25 minutes', NOW() - interval '5 minutes', 'WaitingForPayment', 'Awaiting cashier', NOW() - interval '40 minutes', NOW() - interval '2 minutes', 'seed', NULL, false),

  -- Walk-in patient (no appointment)
  ('v0000000-0000-0000-0000-000000000006', NULL, 'aaaaaaaa-0000-0000-0000-000000000008', '11111111-1111-1111-1111-111111111111', NOW() - interval '12 minutes', NULL, NULL, 'CheckedIn', 'Walk-in patient', NOW() - interval '12 minutes', NOW() - interval '1 minutes', 'seed', NULL, false);

-- AI screenings (6 total, 2 pending with no ScreeningResults)
INSERT INTO public.AiScreenings (
  Id, PatientId, ModelVersion, ProcessedAt, RawJsonOutput,
  IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted
) VALUES
  ('ai000000-0000-0000-0000-000000000001', 'aaaaaaaa-0000-0000-0000-000000000001', 'AuraEyes-AI-v1.2', NOW() - interval '3 minutes', '{"risk":"Low"}', true, NOW() - interval '6 minutes', NOW() - interval '3 minutes', 'seed', NULL, false),
  ('ai000000-0000-0000-0000-000000000002', 'aaaaaaaa-0000-0000-0000-000000000005', 'AuraEyes-AI-v1.2', NOW() - interval '20 minutes', '{"risk":"High"}', true, NOW() - interval '25 minutes', NOW() - interval '20 minutes', 'seed', NULL, false),
  ('ai000000-0000-0000-0000-000000000003', 'aaaaaaaa-0000-0000-0000-000000000009', 'AuraEyes-AI-v1.2', NOW() - interval '8 minutes', '{"risk":"High"}', true, NOW() - interval '15 minutes', NOW() - interval '8 minutes', 'seed', NULL, false),
  ('ai000000-0000-0000-0000-000000000004', 'aaaaaaaa-0000-0000-0000-000000000007', 'AuraEyes-AI-v1.2', NOW() - interval '2 hours', '{"risk":"Medium"}', true, NOW() - interval '3 hours', NOW() - interval '2 hours', 'seed', NULL, false),
  ('ai000000-0000-0000-0000-000000000005', 'aaaaaaaa-0000-0000-0000-00000000000a', 'AuraEyes-AI-v1.2', NULL, NULL, true, NOW() - interval '10 minutes', NOW() - interval '10 minutes', 'seed', NULL, false),
  ('ai000000-0000-0000-0000-000000000006', 'aaaaaaaa-0000-0000-0000-000000000008', 'AuraEyes-AI-v1.2', NULL, NULL, true, NOW() - interval '12 minutes', NOW() - interval '12 minutes', 'seed', NULL, false);

-- AI screening results (4 total)
INSERT INTO public.ScreeningResults (
  Id, AiScreeningId, RiskLevel, Summary, ConfidenceScore, Findings,
  CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted
) VALUES
  ('sr000000-0000-0000-0000-000000000001', 'ai000000-0000-0000-0000-000000000001', 'Low', 'No significant risk detected', 0.93, 'Normal retinal scan', NOW() - interval '3 minutes', NOW() - interval '3 minutes', 'seed', NULL, false),
  ('sr000000-0000-0000-0000-000000000002', 'ai000000-0000-0000-0000-000000000002', 'High', 'Possible diabetic retinopathy', 0.91, 'Multiple microaneurysms', NOW() - interval '20 minutes', NOW() - interval '20 minutes', 'seed', NULL, false),
  ('sr000000-0000-0000-0000-000000000003', 'ai000000-0000-0000-0000-000000000003', 'High', 'Acute vision risk', 0.88, 'Vessel leakage suspected', NOW() - interval '8 minutes', NOW() - interval '8 minutes', 'seed', NULL, false),
  ('sr000000-0000-0000-0000-000000000004', 'ai000000-0000-0000-0000-000000000004', 'Medium', 'Early AMD signs', 0.79, 'Drusen present', NOW() - interval '2 hours', NOW() - interval '2 hours', 'seed', NULL, false);

-- Consultation sessions (doctor review workload)
INSERT INTO public.ConsultationSessions (
  Id, PatientId, OphthalmologistId, AiScreeningId, Type, Status, ChatStatus,
  Price, AppointmentTime, StartTime, EndTime, LastActivityAt,
  CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted
) VALUES
  -- Waiting for doctor (normal flow)
  ('cs000000-0000-0000-0000-000000000001', 'aaaaaaaa-0000-0000-0000-000000000001', '11111111-1111-1111-1111-111111111111', 'ai000000-0000-0000-0000-000000000001', 'Verification', 'Pending', 'Open', 450000, NOW() + interval '30 minutes', NULL, NULL, NOW() - interval '2 minutes', NOW() - interval '6 minutes', NOW() - interval '2 minutes', 'seed', NULL, false),

  -- Urgent case waiting
  ('cs000000-0000-0000-0000-000000000002', 'aaaaaaaa-0000-0000-0000-000000000005', '11111111-1111-1111-1111-111111111111', 'ai000000-0000-0000-0000-000000000002', 'Verification', 'Pending', 'Open', 450000, NOW() + interval '15 minutes', NULL, NULL, NOW() - interval '20 minutes', NOW() - interval '25 minutes', NOW() - interval '20 minutes', 'seed', NULL, false),

  -- In progress review
  ('cs000000-0000-0000-0000-000000000003', 'aaaaaaaa-0000-0000-0000-000000000009', '11111111-1111-1111-1111-111111111111', 'ai000000-0000-0000-0000-000000000003', 'Verification', 'Confirmed', 'Open', 450000, NOW(), NOW() - interval '10 minutes', NULL, NOW() - interval '2 minutes', NOW() - interval '15 minutes', NOW() - interval '2 minutes', 'seed', NULL, false),

  -- Completed review
  ('cs000000-0000-0000-0000-000000000004', 'aaaaaaaa-0000-0000-0000-000000000007', '11111111-1111-1111-1111-111111111111', 'ai000000-0000-0000-0000-000000000004', 'Verification', 'Completed', 'Archived', 450000, NOW() - interval '2 hours', NOW() - interval '2 hours', NOW() - interval '90 minutes', NOW() - interval '80 minutes', NOW() - interval '3 hours', NOW() - interval '80 minutes', 'seed', NULL, false);

-- Medical diagnoses (completed cases)
INSERT INTO public.MedicalDiagnoses (
  Id, AiScreeningId, DoctorId, ConsultationSessionId, DiagnosisCode, CodingSystem,
  ClinicalFindings, SeverityLevel, ConfidenceLevel, TreatmentPlan, Recommendations,
  IsUrgent, Status, FinalizedAt, LifestyleAdvice, IsReferralNeeded, FollowUpDate,
  ConfirmedAt, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted
) VALUES
  ('md000000-0000-0000-0000-000000000001', 'ai000000-0000-0000-0000-000000000004', '11111111-1111-1111-1111-111111111111', 'cs000000-0000-0000-0000-000000000004', 'H35.30', 'ICD-10', 'Early AMD signs', 'Moderate', 0.82, 'Monitor and follow-up', 'Use UV protection', false, 'Finalized', NOW() - interval '80 minutes', 'Reduce screen time', false, NOW() + interval '60 days', NOW() - interval '80 minutes', NOW() - interval '3 hours', NOW() - interval '80 minutes', 'seed', NULL, false),
  ('md000000-0000-0000-0000-000000000002', 'ai000000-0000-0000-0000-000000000003', '11111111-1111-1111-1111-111111111111', 'cs000000-0000-0000-0000-000000000003', 'H34.9', 'ICD-10', 'Suspected retinal issue', 'High', 0.88, 'Immediate monitoring', 'Urgent follow-up', true, 'InReview', NULL, 'Avoid driving', true, NOW() + interval '7 days', NOW() - interval '5 minutes', NOW() - interval '15 minutes', NOW() - interval '5 minutes', 'seed', NULL, false);

COMMIT;
