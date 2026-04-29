# Patient Appointment Booking Flow - Implementation Documentation

## Overview

This document describes the implementation of the patient appointment booking flow with ophthalmologists in the Aura Eyes system.

## Table of Contents

1. [Database Changes](#database-changes)
2. [Domain Layer Changes](#domain-layer-changes)
3. [Application Layer Changes](#application-layer-changes)
4. [Infrastructure Layer Changes](#infrastructure-layer-changes)
5. [API Endpoints](#api-endpoints)
6. [Data Transfer Objects (DTOs)](#data-transfer-objects-dtos)
7. [Manual Testing Guide](#manual-testing-guide)
8. [Flow Diagrams](#flow-diagrams)

---

## Database Changes

### New Columns in `AppointmentSlots` Table

| Column | Type | Description |
|--------|------|-------------|
| `ReservedBy` | `Guid?` | Patient ID who has reserved the slot |
| `ReservationExpireAt` | `DateTime?` | When the reservation expires |

### New Column in `ConsultationSessions` Table

| Column | Type | Description |
|--------|------|-------------|
| `AppointmentSlotId` | `Guid?` | FK to the booked appointment slot |

### Updated `ScheduleStatus` Enum

```csharp
public enum ScheduleStatus
{
    Available = 1,    // Slot is available for booking
    Booked = 2,       // Slot is booked by a patient
    Cancelled = 3,    // Slot has been cancelled
    Completed = 4,    // Consultation has been completed
    NoShow = 5,       // Patient did not show up
    Reserved = 6,     // NEW: Slot is temporarily reserved (pending payment)
    Blocked = 7       // NEW: Slot is blocked by doctor (not available)
}
```

### Migration Command

```bash
cd d:\CShaft\ASP.NET\AuraEyes_BE
dotnet ef migrations add AddSlotReservationAndSessionSlotLink --project src/Infrastructure --startup-project src/API
dotnet ef database update --project src/Infrastructure --startup-project src/API
```

---

## Domain Layer Changes

### Files Modified

1. **`ScheduleStatus.cs`** - Added `Reserved` and `Blocked` statuses
2. **`AppointmentSlot.cs`** - Added reservation fields and methods:
   - `ReservedBy`, `ReservationExpireAt` properties
   - `Reserve()`, `ConfirmReservation()`, `ReleaseReservation()`, `IsReservationExpired()`
   - `Block()`, `Unblock()`
3. **`ConsultationSession.cs`** - Added `AppointmentSlotId` and navigation property
4. **`IAppointmentSlotRepository.cs`** - Added:
   - `GetExpiredReservationsAsync()`
   - `GetByIdWithLockAsync()`

---

## Application Layer Changes

### New Commands Created

| Command | Description | Handler |
|---------|-------------|---------|
| `GenerateSlotsCommand` | Generate slots from template for date range | `GenerateSlotsCommandHandler` |
| `ReserveSlotCommand` | Reserve a slot for a patient (with timeout) | `ReserveSlotCommandHandler` |
| `ConfirmReservationCommand` | Confirm reservation after payment | `ConfirmReservationCommandHandler` |
| `ReleaseReservationCommand` | Release a reservation | `ReleaseReservationCommandHandler` |
| `BlockSlotCommand` | Doctor blocks a slot | `BlockSlotCommandHandler` |
| `UnblockSlotCommand` | Doctor unblocks a slot | `UnblockSlotCommandHandler` |

### Files Created

```
src/Application/Scheduling/AppointmentSlots/Commands/
├── GenerateSlots/
│   ├── GenerateSlotsCommand.cs
│   ├── GenerateSlotsCommandHandler.cs
│   └── GenerateSlotsCommandValidator.cs
├── ReserveSlot/
│   ├── ReserveSlotCommand.cs
│   ├── ReserveSlotCommandHandler.cs
│   └── ReserveSlotCommandValidator.cs
├── ConfirmReservation/
│   ├── ConfirmReservationCommand.cs
│   ├── ConfirmReservationCommandHandler.cs
│   └── ConfirmReservationCommandValidator.cs
├── ReleaseReservation/
│   ├── ReleaseReservationCommand.cs
│   └── ReleaseReservationCommandHandler.cs
├── BlockSlot/
│   ├── BlockSlotCommand.cs
│   └── BlockSlotCommandHandler.cs
└── UnblockSlot/
    ├── UnblockSlotCommand.cs
    └── UnblockSlotCommandHandler.cs
```

---

## Infrastructure Layer Changes

### New Background Service

**`ReservationExpirationWorker.cs`** - Runs every 1 minute to:
- Find all slots with `Status = Reserved` and `ReservationExpireAt < now`
- Release expired reservations (set status back to `Available`)

### Repository Changes

**`AppointmentSlotRepository.cs`** - Added:
- `GetExpiredReservationsAsync()` - Find expired reservations
- `GetByIdWithLockAsync()` - Get slot with PostgreSQL row-level lock (`FOR UPDATE`)

### Dependency Injection

The `ReservationExpirationWorker` is registered as a hosted service in `DependencyInjection.cs`.

---

## API Endpoints

### New Endpoints in `AppointmentSlotsController`

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| POST | `/api/appointment-slots/generate` | Generate slots from template | Ophthalmologist/OrgAdmin |
| POST | `/api/appointment-slots/{slotId}/reserve` | Reserve a slot | Patient |
| POST | `/api/appointment-slots/{slotId}/confirm` | Confirm reservation | Patient |
| POST | `/api/appointment-slots/{slotId}/release` | Release reservation | Patient |
| POST | `/api/appointment-slots/{slotId}/block` | Block a slot | Ophthalmologist/OrgAdmin |
| POST | `/api/appointment-slots/{slotId}/unblock` | Unblock a slot | Ophthalmologist/OrgAdmin |

### Request/Response Examples

#### Generate Slots

```bash
POST /api/appointment-slots/generate
Authorization: Bearer {doctor_token}
Content-Type: application/json

{
    "scheduleTemplateId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fromDate": "2026-03-15",
    "toDate": "2026-04-15",
    "skipExistingDates": true
}
```

Response:
```json
{
    "success": true,
    "data": 24,  // Number of slots created
    "message": "Success"
}
```

#### Reserve Slot

```bash
POST /api/appointment-slots/{slotId}/reserve
Authorization: Bearer {patient_token}
Content-Type: application/json

{
    "patientId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "reservationMinutes": 5
}
```

Response:
```json
{
    "success": true,
    "data": {
        "slotId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "expiresAt": "2026-03-12T14:05:00Z",
        "remainingSeconds": 300
    }
}
```

#### Confirm Reservation

```bash
POST /api/appointment-slots/{slotId}/confirm
Authorization: Bearer {patient_token}
Content-Type: application/json

{
    "patientId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "aiScreeningId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "shareRetinalImages": true,
    "shareAiResults": true
}
```

Response:
```json
{
    "success": true,
    "data": {
        "consultationSessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "appointmentSlotId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "appointmentTime": "2026-03-15T18:00:00Z"
    }
}
```

#### Block Slot

```bash
POST /api/appointment-slots/{slotId}/block
Authorization: Bearer {doctor_token}
Content-Type: application/json

{
    "ophthalmologistId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "reason": "Personal appointment"
}
```

---

## Data Transfer Objects (DTOs)

### AppointmentSlotListDto

Returned from `GET /api/appointment-slots` (list view):

```json
{
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "scheduleTemplateId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "ophthalId": "a2f30076-6cb8-432a-b920-687c90dd0af0",
    "orgId": null,
    "date": "2026-03-15",
    "startTime": "18:00:00",
    "endTime": "18:30:00",
    "status": "Available",
    "cost": 200000,
    "maxCapacity": 1,
    "bookedCount": 0,
    "availableCapacity": 1,
    "createdAt": "2026-03-12T10:00:00Z"
}
```

### AppointmentSlotDto

Returned from `GET /api/appointment-slots/{slotId}` (detail view):

```json
{
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "scheduleTemplateId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "ophthalId": "a2f30076-6cb8-432a-b920-687c90dd0af0",
    "orgId": null,
    "date": "2026-03-15",
    "startTime": "18:00:00",
    "endTime": "18:30:00",
    "status": "Reserved",
    "cost": 200000,
    "maxCapacity": 1,
    "bookedCount": 0,
    "availableCapacity": 1,
    "reservedBy": "b3c41087-7dc9-543b-c031-798d01ee1bf1",
    "reservationExpireAt": "2026-03-12T14:05:00Z",
    "createdAt": "2026-03-12T10:00:00Z",
    "updatedAt": "2026-03-12T14:00:00Z"
}
```

### Field Descriptions

| Field | Type | Description |
|-------|------|-------------|
| `id` | UUID | Unique slot identifier |
| `scheduleTemplateId` | UUID | Reference to parent template |
| `ophthalId` | UUID? | Ophthalmologist ID (from template) |
| `orgId` | UUID? | Organisation ID (from template) |
| `date` | string | Slot date (YYYY-MM-DD) |
| `startTime` | string | Start time (HH:mm:ss) |
| `endTime` | string | End time (HH:mm:ss) |
| `status` | string | "Available", "Reserved", "Booked", "Blocked", "Cancelled", "Completed", "NoShow" |
| `cost` | decimal? | Consultation fee in VND |
| `maxCapacity` | int | Maximum concurrent bookings allowed |
| `bookedCount` | int | Current number of confirmed bookings |
| `availableCapacity` | int | Remaining capacity (maxCapacity - bookedCount) |
| `reservedBy` | UUID? | Patient ID who reserved this slot |
| `reservationExpireAt` | datetime? | When the reservation expires |
| `createdAt` | datetime | Record creation timestamp |
| `updatedAt` | datetime? | Last update timestamp |

---

## Manual Testing Guide

### Prerequisites

1. Database is running and migrated
2. API server is running
3. Test accounts exist (see `TEST_ACCOUNTS.md`)

### Step 1: Doctor Creates Schedule Template

```bash
# Login as doctor
POST /api/auth/login
{
    "email": "doctor@test.com",
    "password": "TestPassword123!"
}

# Create schedule template
POST /api/schedule-templates
Authorization: Bearer {doctor_token}
{
    "ophthalId": "{doctor_ophthalmologist_id}",
    "dayOfWeek": 1,  // Monday
    "startTime": "18:00:00",
    "endTime": "21:00:00",
    "slotDuration": 30,
    "maxCapacity": 1
}
```

### Step 2: Generate Appointment Slots

```bash
POST /api/appointment-slots/generate
Authorization: Bearer {doctor_token}
{
    "scheduleTemplateId": "{template_id}",
    "fromDate": "2026-03-15",
    "toDate": "2026-03-31"
}
```

### Step 3: Doctor Blocks a Slot (Optional)

```bash
# Get available slots
GET /api/appointment-slots?ophthalId={doctor_id}&status=Available

# Block one slot
POST /api/appointment-slots/{slot_id}/block
Authorization: Bearer {doctor_token}
{
    "ophthalmologistId": "{doctor_ophthalmologist_id}",
    "reason": "Busy"
}
```

### Step 4: Patient Views Available Slots

```bash
# Login as patient
POST /api/auth/login
{
    "email": "patient@test.com",
    "password": "TestPassword123!"
}

# Get available slots for doctor
GET /api/appointment-slots?ophthalId={doctor_id}&status=Available&fromDate=2026-03-15
Authorization: Bearer {patient_token}
```

### Step 5: Patient Reserves a Slot

```bash
POST /api/appointment-slots/{slot_id}/reserve
Authorization: Bearer {patient_token}
{
    "patientId": "{patient_id}",
    "reservationMinutes": 5
}
```

**Expected Result:**
- Slot status changes to `Reserved`
- Response includes expiration time (5 minutes from now)
- Other patients cannot reserve this slot

### Step 6: Patient Confirms Booking (within 5 minutes)

```bash
POST /api/appointment-slots/{slot_id}/confirm
Authorization: Bearer {patient_token}
{
    "patientId": "{patient_id}",
    "aiScreeningId": "{screening_id}",  // Optional
    "shareRetinalImages": true,
    "shareAiResults": true
}
```

**Expected Result:**
- Slot status changes to `Booked`
- New `ConsultationSession` is created with `AppointmentSlotId`
- Response includes consultation session ID

### Step 7: Test Reservation Timeout

```bash
# Reserve a slot but do NOT confirm
POST /api/appointment-slots/{slot_id}/reserve
{
    "patientId": "{patient_id}",
    "reservationMinutes": 1  # 1 minute for faster testing
}

# Wait 2 minutes...

# Try to reserve again - should work because it expired
POST /api/appointment-slots/{slot_id}/reserve
{
    "patientId": "{another_patient_id}",
    "reservationMinutes": 5
}
```

### Step 8: Test Race Condition

Open two terminal windows and run simultaneously:

**Terminal 1:**
```bash
curl -X POST "http://localhost:5000/api/appointment-slots/{slot_id}/reserve" \
  -H "Authorization: Bearer {patient_a_token}" \
  -H "Content-Type: application/json" \
  -d '{"patientId": "{patient_a_id}", "reservationMinutes": 5}'
```

**Terminal 2:**
```bash
curl -X POST "http://localhost:5000/api/appointment-slots/{slot_id}/reserve" \
  -H "Authorization: Bearer {patient_b_token}" \
  -H "Content-Type: application/json" \
  -d '{"patientId": "{patient_b_id}", "reservationMinutes": 5}'
```

**Expected Result:**
- One request succeeds with `200 OK`
- The other fails with `409 Conflict` ("This slot is already reserved...")

---

## Flow Diagrams

### Complete Booking Flow

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         DOCTOR SETUP PHASE                                   │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   Doctor                    System                        Database          │
│     │                         │                              │              │
│     │──Create Template────────▶│                              │              │
│     │                         │──Save ScheduleTemplate───────▶│              │
│     │                         │                              │              │
│     │──Generate Slots─────────▶│                              │              │
│     │                         │──Create AppointmentSlots─────▶│              │
│     │                         │  (Status: Available)         │              │
│     │                         │                              │              │
│     │──Block Slot (opt)───────▶│                              │              │
│     │                         │──Update Status: Blocked──────▶│              │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                         PATIENT BOOKING PHASE                               │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   Patient                   System                        Database          │
│     │                         │                              │              │
│     │──View Available Slots───▶│                              │              │
│     │                         │──Query (Status=Available)────▶│              │
│     │◀──Return Slots──────────│                              │              │
│     │                         │                              │              │
│     │──Reserve Slot───────────▶│                              │              │
│     │                         │──BEGIN TRANSACTION           │              │
│     │                         │──SELECT FOR UPDATE───────────▶│              │
│     │                         │──Check Status=Available      │              │
│     │                         │──Update Status=Reserved──────▶│              │
│     │                         │──Set ReservedBy, ExpireAt    │              │
│     │                         │──COMMIT                      │              │
│     │◀──Reservation Info──────│  (ExpiresAt, SlotId)        │              │
│     │                         │                              │              │
│  ┌──│── 5 MIN TIMER ──────────│──────────────────────────────│──┐           │
│  │  │                         │                              │  │           │
│  │  │──Confirm Booking────────▶│                              │  │           │
│  │  │  (after payment)        │──Check Reservation Valid     │  │           │
│  │  │                         │──Update Status=Booked────────▶│  │           │
│  │  │                         │──Create ConsultationSession──▶│  │           │
│  │  │◀──Session Created───────│                              │  │           │
│  └──│─────────────────────────│──────────────────────────────│──┘           │
│                                                                             │
│  [IF TIMEOUT - Background Worker releases reservation]                      │
│                                                                             │
│     Worker                    │                              │              │
│       │──Check Expired────────▶│                              │              │
│       │                       │──Find (Status=Reserved       │              │
│       │                       │   AND ExpireAt < now)        │              │
│       │                       │──Update Status=Available─────▶│              │
│       │                       │──Clear ReservedBy, ExpireAt  │              │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Race Condition Handling

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                     CONCURRENT BOOKING SCENARIO                              │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   Patient A                 System                      Patient B           │
│     │                         │                            │                │
│     │──Reserve Slot───────────▶│◀───Reserve Same Slot──────│                │
│     │                         │                            │                │
│     │                    [POSTGRES ROW LOCK]               │                │
│     │                         │                            │                │
│     │                    Patient A gets lock first         │                │
│     │                    Patient B waits...                │                │
│     │                         │                            │                │
│     │◀──SUCCESS (Reserved)────│                            │                │
│     │                         │                            │                │
│     │                    Lock released                     │                │
│     │                    Patient B gets lock               │                │
│     │                         │                            │                │
│     │                    Status = Reserved (by A)          │                │
│     │                         │                            │                │
│     │                         │──CONFLICT (409)────────────▶│                │
│     │                         │  "Slot already reserved"   │                │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## Slot Status State Machine

```
                                ┌──────────────┐
                                │   AVAILABLE  │
                                └──────┬───────┘
                                       │
              ┌────────────────────────┼────────────────────────┐
              │                        │                        │
              ▼                        ▼                        ▼
       ┌──────────────┐        ┌──────────────┐        ┌──────────────┐
       │    BLOCKED   │        │   RESERVED   │        │    BOOKED    │
       │  (by doctor) │        │ (5min timer) │        │  (confirmed) │
       └──────┬───────┘        └──────┬───────┘        └──────┬───────┘
              │                       │                        │
              │                       │                        ├──────────────┐
              │ Unblock               │ Timeout/               │              │
              │                       │ Release                ▼              ▼
              │                       │                 ┌──────────────┐┌──────────────┐
              │                       │                 │  COMPLETED   ││   NO_SHOW    │
              │                       │                 └──────────────┘└──────────────┘
              │                       │
              │                       │ Confirm
              │                       │ (after payment)
              │                       ▼
              │               ┌──────────────┐
              │               │    BOOKED    │
              │               └──────────────┘
              │
              ▼
       ┌──────────────┐
       │   AVAILABLE  │
       └──────────────┘
```

---

## Error Handling

| Error Code | Scenario | Message |
|------------|----------|---------|
| 404 | Slot not found | "Appointment slot '{id}' not found." |
| 409 | Slot already reserved | "This slot is already reserved by another patient." |
| 409 | Slot already booked | "This slot is already booked." |
| 409 | Slot blocked | "This slot is not available." |
| 403 | Wrong patient | "This slot was reserved by a different patient." |
| 400 | Reservation expired | "Your reservation has expired. Please try booking again." |

---

## Configuration

### Reservation Timeout

Default: **5 minutes** (configurable via API request, max 15 minutes)

### Background Worker Interval

Default: **1 minute** (checks for expired reservations)

### Maximum Slot Generation Range

Default: **90 days** (to prevent excessive slot creation)

---

## Files Changed Summary

### Domain Layer
- `src/Domain/Enums/ScheduleStatus.cs`
- `src/Domain/Entities/Scheduling/AppointmentSlot.cs`
- `src/Domain/Entities/Consultation/ConsultationSession.cs`
- `src/Domain/Repositories/IAppointmentSlotRepository.cs`

### Application Layer
- `src/Application/Scheduling/AppointmentSlots/Commands/GenerateSlots/*`
- `src/Application/Scheduling/AppointmentSlots/Commands/ReserveSlot/*`
- `src/Application/Scheduling/AppointmentSlots/Commands/ConfirmReservation/*`
- `src/Application/Scheduling/AppointmentSlots/Commands/ReleaseReservation/*`
- `src/Application/Scheduling/AppointmentSlots/Commands/BlockSlot/*`
- `src/Application/Scheduling/AppointmentSlots/Commands/UnblockSlot/*`
- `src/Application/ConsultationSessions/Commands/CreateVideoCallSession/CreateVideoCallSessionCommandHandler.cs`

### Infrastructure Layer
- `src/Infrastructure/DependencyInjection.cs`
- `src/Infrastructure/Persistence/Repositories/AppointmentSlotRepository.cs`
- `src/Infrastructure/Persistence/Configurations/Scheduling/AppointmentSlotConfiguration.cs`
- `src/Infrastructure/Persistence/Configurations/Consultation/ConsultationSessionConfiguration.cs`
- `src/Infrastructure/Services/ReservationExpirationWorker.cs`

### API Layer
- `src/API/Controllers/AppointmentSlotsController.cs`

---

## Frontend Implementation

### New Pages Created

| Page | Path | Description |
|------|------|-------------|
| Book Appointment | `/patient/book` | Calendar view to browse doctors and select available slots |
| Booking Confirmation | `/patient/book/confirm?slotId={id}` | Confirmation page with countdown timer |
| Slot Management | `/ophthalmologist/slot-management` | Doctor's page to manage templates and slots |

### Frontend Files Created

```
src/features/patient/
├── api/
│   └── booking.api.ts          # API functions for slot operations
├── hooks/
│   └── use-booking.ts          # React Query hooks for booking flow
├── pages/
│   ├── book-appointment.tsx    # Main booking calendar page
│   └── booking-confirmation.tsx # Confirmation with countdown

src/features/ophthalmologist/
└── pages/
    └── slot-management.tsx     # Template & slot management for doctors

src/types/
└── schedule.ts                 # Updated with new types & statuses

src/lib/
└── endpoints.ts                # Added APPOINTMENT_SLOTS & SCHEDULE_TEMPLATES
```

### Frontend Routes Added

```typescript
// Patient routes
<Route path="/patient/book" element={<BookAppointmentPage />} />
<Route path="/patient/book/confirm" element={<BookingConfirmationPage />} />

// Ophthalmologist routes
<Route path="/ophthalmologist/slot-management" element={<OphthalmologistSlotManagementPage />} />
```

### React Query Hooks

| Hook | Purpose |
|------|---------|
| `useAppointmentSlots(params)` | Fetch available slots with filters |
| `useAppointmentSlot(slotId)` | Fetch single slot details |
| `useReserveSlot()` | Reserve a slot (mutation) |
| `useConfirmReservation()` | Confirm booking after payment |
| `useReleaseReservation()` | Cancel reservation |
| `useGenerateSlots()` | Generate slots from template |
| `useBlockSlot()` | Block a slot (doctor) |
| `useUnblockSlot()` | Unblock a slot (doctor) |
| `useScheduleTemplates(ophthalId)` | Fetch doctor's templates |
| `useCreateScheduleTemplate()` | Create new template |
| `useDeleteScheduleTemplate()` | Delete template |

### Frontend Types (src/types/schedule.ts)

Key TypeScript interfaces for the booking flow:

```typescript
// List view DTO
interface AppointmentSlotListDto {
  id: string;
  scheduleTemplateId: string;
  ophthalId: string | null;     // From linked ScheduleTemplate
  orgId: string | null;         // From linked ScheduleTemplate
  date: string;                 // "YYYY-MM-DD"
  startTime: string;            // "HH:mm:ss"
  endTime: string;              // "HH:mm:ss"
  status: string;               // "Available", "Reserved", "Booked", etc.
  cost: number | null;
  maxCapacity: number;
  bookedCount: number;
  availableCapacity: number;
  createdAt: string;
}

// Detail view DTO (includes reservation info)
interface AppointmentSlotDto extends AppointmentSlotListDto {
  reservedBy: string | null;
  reservationExpireAt: string | null;
  updatedAt: string | null;
}

// Reserve slot response
interface SlotReservationResult {
  slotId: string;
  expiresAt: string;
  remainingSeconds: number;
}

// Confirm booking response
interface BookingConfirmationResult {
  consultationSessionId: string;
  appointmentSlotId: string;
  appointmentTime: string;
}
```

**Note:** The slot DTOs use **string-based status** (e.g., "Available") rather than numeric enums to match the backend response format.

### UI Components Features

#### Patient Booking Page (`/patient/book`)
- Weekly calendar view with navigation
- Available slots shown in green, clickable
- Reserved/Booked/Blocked slots shown in different colors
- Click slot → Reserve → Modal with countdown timer
- "Confirm Booking" navigates to confirmation page

#### Booking Confirmation Page (`/patient/book/confirm`)
- Displays reservation details with countdown timer
- Options to share retinal images and AI results
- Cancel/Confirm buttons
- Success state shows appointment confirmation
- Auto-redirect if reservation expires

#### Doctor Slot Management Page (`/ophthalmologist/slot-management`)
- Schedule template management (create/delete)
- Generate slots from templates
- Weekly calendar view of all slots
- Block/Unblock individual slots with hover buttons
- Statistics: Total, Available, Booked, Blocked

---

## Frontend Manual Testing Guide

### Test Patient Booking Flow

1. **Navigate to booking page**
   - Go to `/patient/book`
   - Enter a doctor ID (ophthalmologistId)

2. **View available slots**
   - Weekly calendar should show available slots in green
   - Navigate between weeks using arrows

3. **Reserve a slot**
   - Click on an available slot
   - Modal appears with countdown timer (5:00)
   - Slot details shown (doctor, date, time, price)

4. **Confirm booking**
   - Click "Confirm Booking" in modal
   - Redirected to `/patient/book/confirm?slotId={id}`
   - Countdown continues
   - Choose data sharing options
   - Click "Confirm & Pay"

5. **Verify success**
   - Success message with session ID
   - Navigate to "My Appointments" to see booking

### Test Reservation Timeout (Frontend)

1. Reserve a slot
2. Wait on modal without confirming
3. Watch countdown timer
4. At 0:00, modal should close and redirect back

### Test Doctor Slot Management

1. **Navigate to slot management**
   - Go to `/ophthalmologist/slot-management`

2. **Create schedule template**
   - Click "New Template"
   - Select day of week, time range, slot duration
   - Set price and slot type
   - Click "Create"

3. **Generate slots**
   - Click "Generate Slots" on a template
   - Select date range
   - Click "Generate"
   - Refresh to see new slots in calendar

4. **Block/Unblock slots**
   - Hover over an available slot
   - Click power-off icon to block
   - Slot turns gray
   - Hover and click power icon to unblock
