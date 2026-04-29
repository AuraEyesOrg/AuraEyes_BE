# Organisation Clinic Booking Flow

## Tổng Quan

Luồng này cho phép bệnh nhân đặt lịch khám tại phòng khám/bệnh viện (Organisation). Khác với luồng consultation online (gắn với bác sĩ cụ thể), lịch khám tại clinic thuộc về Organisation và không gắn bác sĩ cụ thể ở cấp Appointment.

### Đặc Điểm Chính

| Feature | Online Consultation | Clinic Booking |
|---------|---------------------|----------------|
| Owner | Ophthalmologist | Organisation |
| Capacity | 1 patient/slot | Multiple patients/slot |
| Doctor Assignment | At booking time | Not assigned on clinic appointment |
| Entity | Appointment + ConsultationSession | Appointment (Type = ClinicVisit) |

---

## 1. Database Schema

### 1.1 AppointmentStatus Enum (Mới)

```csharp
public enum AppointmentStatus
{
    Pending = 1,      // Chờ xác nhận
    Confirmed = 2,    // Đã xác nhận
    CheckedIn = 3,    // Đã check-in tại clinic
    InProgress = 4,   // Đang khám
    Completed = 5,    // Hoàn thành
    Cancelled = 6,    // Đã hủy
    NoShow = 7        // Không đến
}
```

### 1.2 Unified Appointment Entity (Clinic Visit)

```csharp
public class Appointment : BaseEntity, IAggregateRoot
{
    public AppointmentType Type { get; private set; } // ClinicVisit
    public Guid PatientId { get; private set; }
    public Guid OrganisationId { get; private set; }
    public Guid AppointmentSlotId { get; private set; }
    public Guid? DoctorId { get; private set; }  // Always NULL for ClinicVisit
    public AppointmentStatus Status { get; private set; }
    public string? VisitReason { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? CheckedInAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    
    // Navigation properties
    public Patient? Patient { get; private set; }
    public Organisation? Organisation { get; private set; }
    public AppointmentSlot? AppointmentSlot { get; private set; }
}
```

### 1.3 AppointmentSlot Modifications

Thêm `MaxCapacity` property để copy từ ScheduleTemplate:

```csharp
// Existing fields...
public int BookedCount { get; private set; }

// New field - copied from ScheduleTemplate during generation
public int MaxCapacity { get; private set; }

// Navigation
private readonly List<Appointment> _appointments = new();
public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();
```

---

## 2. API Endpoints

### 2.1 Organisation Schedule Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/organisations/{orgId}/schedule-templates` | Tạo schedule template |
| GET | `/api/organisations/{orgId}/schedule-templates` | Lấy danh sách templates |
| PUT | `/api/organisations/{orgId}/schedule-templates/{id}` | Cập nhật template |
| DELETE | `/api/organisations/{orgId}/schedule-templates/{id}` | Xóa template |

### 2.2 Organisation Slot Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/organisations/{orgId}/slots/generate` | Generate slots từ template |
| GET | `/api/organisations/{orgId}/slots` | Lấy danh sách slots |
| PUT | `/api/organisations/{orgId}/slots/{slotId}/block` | Block slot |
| PUT | `/api/organisations/{orgId}/slots/{slotId}/unblock` | Unblock slot |
| PUT | `/api/organisations/{orgId}/slots/{slotId}/capacity` | Cập nhật capacity |
| POST | `/api/organisations/{orgId}/slots/create` | Tạo thêm slot ngoài template |

### 2.3 Patient Booking

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/organisations` | Danh sách organisations (public) |
| GET | `/api/organisations/{orgId}/available-slots` | Xem slots còn trống |
| POST | `/api/clinic-appointments` | Đặt lịch khám |
| DELETE | `/api/clinic-appointments/{id}` | Hủy lịch khám |
| GET | `/api/patients/{patientId}/clinic-appointments` | Lịch khám của patient |

### 2.4 Organisation Staff Operations

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/organisations/{orgId}/appointments` | Xem appointments theo ngày |
| PUT | `/api/clinic-appointments/{id}/check-in` | Check-in bệnh nhân |
| PUT | `/api/clinic-appointments/{id}/start` | Bắt đầu khám |
| PUT | `/api/clinic-appointments/{id}/complete` | Hoàn thành khám |
| PUT | `/api/clinic-appointments/{id}/no-show` | Đánh dấu không đến |

---

## 3. CQRS Commands

### 3.1 Organisation Slot Management Commands

```
Application/
└── Scheduling/
    └── OrganisationSlots/
        └── Commands/
            ├── BlockOrganisationSlot/
            │   ├── BlockOrganisationSlotCommand.cs
            │   └── BlockOrganisationSlotCommandHandler.cs
            ├── UnblockOrganisationSlot/
            ├── UpdateSlotCapacity/
            └── CreateAdditionalSlot/
```

### 3.2 Clinic Appointment Commands

```
Application/
└── ClinicAppointments/
    └── Commands/
        ├── CreateClinicAppointment/
        │   ├── CreateClinicAppointmentCommand.cs
        │   └── CreateClinicAppointmentCommandHandler.cs
        ├── CancelClinicAppointment/
        ├── CheckInAppointment/
        ├── StartAppointment/
        └── CompleteAppointment/
```

---

## 4. Race Condition Handling

### 4.1 Vấn Đề

Khi capacity = 5 và booked = 4, nếu 2 patients đặt cùng lúc có thể dẫn đến 6 bookings.

### 4.2 Giải Pháp: Atomic Update với Optimistic Concurrency

```csharp
// Command Handler
public async Task<Result<Guid>> Handle(CreateClinicAppointmentCommand request, CancellationToken cancellationToken)
{
    await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    
    try
    {
        // 1. Load slot với lock
        var slot = await _slotRepository.GetByIdWithLockAsync(request.SlotId, cancellationToken);
        
        // 2. Check capacity
        if (slot.BookedCount >= slot.MaxCapacity)
            return Result<Guid>.Failure("Slot is full");
        
        // 3. Atomic increment
        slot.IncrementBookedCount();
        
        // 4. Create appointment
        var appointment = Appointment.CreateClinicVisit(
            request.PatientId,
            request.SlotId,
            request.OrganisationId,
            request.VisitReason);
        
        await _appointmentRepository.AddAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        
        return Result<Guid>.Success(appointment.Id);
    }
    catch (DbUpdateConcurrencyException)
    {
        await transaction.RollbackAsync(cancellationToken);
        return Result<Guid>.Failure("Slot was updated by another user. Please retry.");
    }
}
```

### 4.3 Repository Method với Row Lock

```csharp
public async Task<AppointmentSlot?> GetByIdWithLockAsync(Guid id, CancellationToken cancellationToken)
{
    return await _context.AppointmentSlots
        .FromSqlRaw("SELECT * FROM \"AppointmentSlots\" WHERE \"Id\" = {0} FOR UPDATE", id)
        .FirstOrDefaultAsync(cancellationToken);
}
```

---

## 5. Flow Diagrams

### 5.1 Organisation Setup Flow

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        Organisation Setup Flow                               │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   Organisation Admin                    System                              │
│         │                                  │                                │
│         │──Create ScheduleTemplate────────▶│                                │
│         │  (Mon 8:00-12:00, 30min, cap=5)  │                                │
│         │                                  │                                │
│         │◀─────────Template Created────────│                                │
│         │                                  │                                │
│         │──Generate Slots for Date Range──▶│                                │
│         │  (12 Mar - 18 Mar)               │                                │
│         │                                  │                                │
│         │                                  │──Create AppointmentSlots──┐    │
│         │                                  │  08:00-08:30 (cap=5)      │    │
│         │                                  │  08:30-09:00 (cap=5)      │    │
│         │                                  │  09:00-09:30 (cap=5)      │    │
│         │                                  │  ...                      │    │
│         │                                  │◀─────────────────────────┘    │
│         │                                  │                                │
│         │◀─────────Slots Generated─────────│                                │
│         │                                  │                                │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 5.2 Patient Booking Flow

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          Patient Booking Flow                               │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   Patient                 API                    Database                   │
│      │                     │                        │                       │
│      │──Browse Orgs───────▶│                        │                       │
│      │                     │───Query Organisations─▶│                       │
│      │                     │◀──Organisation List────│                       │
│      │◀─List of Clinics────│                        │                       │
│      │                     │                        │                       │
│      │──Select Org & Date─▶│                        │                       │
│      │                     │───Get Available Slots─▶│                       │
│      │                     │   WHERE booked < max   │                       │
│      │                     │◀──Slots with Remaining─│                       │
│      │◀─Available Slots────│                        │                       │
│      │                     │                        │                       │
│      │──Book Slot─────────▶│                        │                       │
│      │  (slotId, reason)   │───BEGIN TRANSACTION───▶│                       │
│      │                     │───SELECT FOR UPDATE───▶│                       │
│      │                     │◀──Slot (with lock)─────│                       │
│      │                     │                        │                       │
│      │                     │   Check: booked < max  │                       │
│      │                     │   ✓ Has capacity       │                       │
│      │                     │                        │                       │
│      │                     │───UPDATE booked_count─▶│                       │
│      │                     │───INSERT Appointment──▶│                       │
│      │                     │───COMMIT──────────────▶│                       │
│      │                     │◀──Success──────────────│                       │
│      │◀─Booking Confirmed──│                        │                       │
│      │                     │                        │                       │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 5.3 Day of Visit Flow

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          Day of Visit Flow                                  │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   Patient        Staff             System                                   │
│      │             │                  │                                     │
│      │──Arrives───▶│                  │                                     │
│      │             │                  │                                     │
│      │             │──Check In───────▶│                                     │
│      │             │   (appointmentId)│                                     │
│      │             │                  │──Update Status: CHECKED_IN────────▶│
│      │             │◀─Confirmed───────│                                     │
│      │             │                  │                                     │
│      │             │──Start──────────▶│                                     │
│      │             │                  │──Update Status: IN_PROGRESS───────▶│
│      │◀──────────────────────Consultation──────────────────────────────────▶│
│      │             │                  │                                     │
│      │             │──Complete───────▶│                                     │
│      │             │                  │──Update Status: COMPLETED─────────▶│
│      │             │◀─Completed───────│                                     │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 6. Implementation Checklist

### 6.1 Domain Layer

- [ ] `AppointmentStatus` enum
- [ ] `ClinicAppointment` entity
- [ ] Update `AppointmentSlot` (add MaxCapacity, navigation to appointments)
- [ ] `IClinicAppointmentRepository` interface

### 6.2 Application Layer

**Organisation Slot Management:**
- [ ] `BlockOrganisationSlotCommand/Handler`
- [ ] `UnblockOrganisationSlotCommand/Handler`
- [ ] `UpdateSlotCapacityCommand/Handler`
- [ ] `CreateAdditionalSlotCommand/Handler`

**Clinic Appointments:**
- [ ] `CreateClinicAppointmentCommand/Handler`
- [ ] `CancelClinicAppointmentCommand/Handler`
- [ ] `CheckInAppointmentCommand/Handler`
- [ ] `StartAppointmentCommand/Handler`
- [ ] `CompleteAppointmentCommand/Handler`
- [ ] `MarkNoShowCommand/Handler`

**Queries:**
- [ ] `GetOrganisationAvailableSlotsQuery/Handler`
- [ ] `GetOrganisationAppointmentsQuery/Handler`
- [ ] `GetPatientClinicAppointmentsQuery/Handler`
- [ ] `GetAppointmentDetailQuery/Handler`

### 6.3 Infrastructure Layer

- [ ] `ClinicAppointmentRepository`
- [ ] `ClinicAppointmentConfiguration` (EF Core)
- [ ] Update `AppointmentSlotConfiguration`
- [ ] Update `AppointmentSlotRepository` (GetByIdWithLockAsync)
- [ ] Database Migration

### 6.4 API Layer

- [ ] `ClinicAppointmentsController`
- [ ] `OrganisationSlotsController` (or extend existing)
- [ ] DTOs for requests/responses

---

## 7. Frontend UI Implementation

### 7.1 Patient UI (AuraEyes_FE)

- Page: `/patient/clinics`
- Chức năng đã implement:
    - Tải danh sách organisations từ `GET /api/organisations`
    - Search organisation theo tên/địa chỉ/thành phố
    - Chọn ngày khám và tải available slots từ `GET /api/organisations/{orgId}/available-slots?date=...`
    - Đặt lịch trực tiếp từ slot card bằng `POST /api/clinic-appointments`
    - Hiển thị danh sách "My Clinic Appointments"
    - Hủy lịch pending bằng `DELETE /api/clinic-appointments/{id}`

### 7.2 Organisation UI (AuraEyes_FE)

- Page: `/organisation/calendar`
- Chức năng đã implement:
    - Lọc theo ngày và tải lịch từ `GET /api/organisations/{orgId}/appointments?date=...`
    - Check-in bệnh nhân: `PUT /api/clinic-appointments/{id}/check-in`
    - Bắt đầu khám: `PUT /api/clinic-appointments/{id}/start`
    - Hoàn thành khám: `PUT /api/clinic-appointments/{id}/complete`
    - Đánh dấu no-show: `PUT /api/clinic-appointments/{id}/no-show`

### 7.3 Frontend Files Added/Updated

- `AuraEyes_FE/src/lib/endpoints.ts` (thêm CLINIC_BOOKING + CLINIC_APPOINTMENTS endpoints)
- `AuraEyes_FE/src/features/patient/types/clinic-booking.types.ts`
- `AuraEyes_FE/src/features/patient/api/clinic-booking.api.ts`
- `AuraEyes_FE/src/features/patient/hooks/use-clinic-booking.ts`
- `AuraEyes_FE/src/features/patient/pages/clinics.tsx` (rewrite dùng API thật)
- `AuraEyes_FE/src/features/organisation/pages/calendar.tsx` (rewrite cho staff operations)

---

## 8. Manual Testing Guide

### 8.1 Chuẩn Bị Test Data

**Tạo Organisation:**
```http
POST /api/system-admin/organisations
Authorization: Bearer {admin_token}
Content-Type: application/json

{
    "name": "Test Eye Clinic",
    "address": "123 Medical Street",
    "orgType": 1,
    "licenseNumber": "MED-12345"
}
```

**Tạo Schedule Template:**
```http
POST /api/organisations/{orgId}/schedule-templates
Authorization: Bearer {org_admin_token}
Content-Type: application/json

{
    "dayOfWeek": 1,
    "startTime": "08:00",
    "endTime": "12:00",
    "slotDuration": 30,
    "maxCapacity": 5
}
```

### 8.2 Test Case 1: Generate Slots

**Request:**
```http
POST /api/organisations/{orgId}/slots/generate
Authorization: Bearer {org_admin_token}
Content-Type: application/json

{
    "templateId": "{template_id}",
    "fromDate": "2026-03-16",
    "toDate": "2026-03-22"
}
```

**Expected:** Tạo 8 slots cho ngày thứ Hai (16 Mar) từ 8:00-12:00.

**Verify:**
```http
GET /api/organisations/{orgId}/slots?date=2026-03-16
```

### 8.3 Test Case 2: Patient Books Slot

**Step 1 - View available slots:**
```http
GET /api/organisations/{orgId}/available-slots?date=2026-03-16
Authorization: Bearer {patient_token}
```

**Expected Result:**
```json
[
    {
        "slotId": "...",
        "date": "2026-03-16",
        "startTime": "08:00",
        "endTime": "08:30",
        "maxCapacity": 5,
        "bookedCount": 0,
        "remaining": 5
    }
]
```

**Step 2 - Book slot:**
```http
POST /api/clinic-appointments
Authorization: Bearer {patient_token}
Content-Type: application/json

{
    "organisationId": "{org_id}",
    "slotId": "{slot_id}",
    "visitReason": "Annual eye checkup"
}
```

**Expected Result:**
```json
{
    "success": true,
    "data": {
        "appointmentId": "...",
        "status": "Pending",
        "slotInfo": {
            "date": "2026-03-16",
            "time": "08:00-08:30"
        }
    }
}
```

### 8.4 Test Case 3: Race Condition Test

**Setup:** Có slot với capacity=5, booked=4

**Execute:** Gửi 2 requests đồng thời từ 2 patients khác nhau.

```bash
# Terminal 1
curl -X POST /api/clinic-appointments -H "Authorization: Bearer {patient1_token}" -d '{"slotId": "...", "visitReason": "Test 1"}'

# Terminal 2 (run at same time)
curl -X POST /api/clinic-appointments -H "Authorization: Bearer {patient2_token}" -d '{"slotId": "...", "visitReason": "Test 2"}'
```

**Expected:** Chỉ 1 request thành công, request còn lại nhận error "Slot is full".

### 8.5 Test Case 4: Organisation Block Slot

```http
PUT /api/organisations/{orgId}/slots/{slotId}/block
Authorization: Bearer {org_admin_token}
Content-Type: application/json

{
    "reason": "Clinic closed for maintenance"
}
```

**Expected:** Slot status = BLOCKED, không hiển thị trong available-slots.

### 8.6 Test Case 5: Check-in và Start Appointment

**Check-in:**
```http
PUT /api/clinic-appointments/{appointmentId}/check-in
Authorization: Bearer {org_staff_token}
```

**Start Appointment:**
```http
PUT /api/clinic-appointments/{appointmentId}/start
Authorization: Bearer {org_staff_token}
```

### 8.7 Test Case 6: Patient Cancel Appointment

```http
DELETE /api/clinic-appointments/{appointmentId}
Authorization: Bearer {patient_token}
```

**Expected:**
- Appointment status = CANCELLED
- Slot booked_count giảm 1
- Slot lại available cho patients khác

---

### 8.8 Frontend Manual Test (UI)

#### Patient luồng đặt lịch clinic

1. Login bằng patient account.
2. Vào `/patient/clinics`.
3. Tìm và chọn organisation.
4. Chọn ngày khám, nhập `Visit Reason` (optional).
5. Bấm `Book Clinic Visit` tại slot còn chỗ.
6. Kiểm tra mục `My Clinic Appointments` xuất hiện lịch mới status `Pending`.
7. Bấm `Cancel` để verify API hủy lịch và refresh danh sách.

#### Organisation luồng vận hành trong ngày

1. Login bằng organisation account có `organizationId` trong user profile.
2. Vào `/organisation/calendar`.
3. Chọn ngày có lịch.
4. Chạy lần lượt hành động theo flow:
    - `Check-in`
    - `Start`
    - `Complete`
5. Verify badge trạng thái cập nhật đúng theo action.

---

## 9. Error Codes

| Code | Message | Description |
|------|---------|-------------|
| SLOT_FULL | "Slot has reached maximum capacity" | BookedCount >= MaxCapacity |
| SLOT_BLOCKED | "Slot is blocked" | Status = Blocked |
| ALREADY_BOOKED | "Patient already has appointment at this time" | Duplicate booking |
| INVALID_SLOT | "Slot not found or not available" | Slot doesn't exist |
| APPOINTMENT_NOT_FOUND | "Appointment not found" | ID không tồn tại |
| CANNOT_CANCEL | "Cannot cancel checked-in appointment" | Status đã là CheckedIn |
| CONCURRENT_UPDATE | "Slot was modified. Please retry" | Optimistic concurrency conflict |

---

## 10. Next Steps

1. Implement Domain entities
2. Implement Repository interfaces
3. Implement CQRS Commands/Handlers
4. Create API Controllers
5. Create Database Migration
6. Frontend UI Implementation
