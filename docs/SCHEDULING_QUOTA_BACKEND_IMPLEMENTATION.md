# Scheduling Quota Backend Implementation

## Scope
This document summarizes the backend implementation for:
- Full-time slot generation by system (no manual slot creation/generation).
- Global part-time daily quota enforced at slot creation time.
- Concurrency-safe quota reservation under concurrent requests.
- Rolling-window auto generation for full-time templates.
- Admin observability for daily part-time quota usage.

## Architecture Decisions

### 1. Atomic counter model (DailySlotQuota)
Instead of calculating daily usage with COUNT(...) under race conditions, the system uses a dedicated aggregate table:
- Entity: DailySlotQuota
- Key: Date
- Fields:
  - PartTimeSlotCount (used)
  - QuotaSnapshot (effective quota for that date)

Reservation is done transactionally with row lock on that date row.

### 2. Reserve-before-insert for bulk generation
For bulk slot generation, the handler reserves quota per day first, then inserts slots.
This prevents over-allocation when many requests compete at the same time.

### 3. Source tracking
Both template and slot have source metadata:
- ScheduleTemplate.Source: Doctor or SystemGenerated
- AppointmentSlot.Source: Doctor or System

This supports operational transparency and backfill compatibility.

## Implemented Components

### Domain
- src/Domain/Entities/Scheduling/DailySlotQuota.cs
- src/Domain/Repositories/IDailySlotQuotaRepository.cs
- src/Domain/Enums/ScheduleTemplateSource.cs
- src/Domain/Enums/SlotSource.cs
- Updated:
  - src/Domain/Entities/Scheduling/ScheduleTemplate.cs
  - src/Domain/Entities/Scheduling/AppointmentSlot.cs

### Infrastructure
- src/Infrastructure/Persistence/Configurations/Scheduling/DailySlotQuotaConfiguration.cs
- src/Infrastructure/Persistence/Repositories/DailySlotQuotaRepository.cs
- Updated:
  - src/Infrastructure/Persistence/ApplicationDbContext.cs
  - src/Infrastructure/Persistence/Configurations/Scheduling/ScheduleTemplateConfiguration.cs
  - src/Infrastructure/Persistence/Configurations/Scheduling/AppointmentSlotConfiguration.cs
  - src/Infrastructure/Persistence/Configurations/Platform/SystemSettingConfiguration.cs
  - src/Infrastructure/Persistence/Repositories/AppointmentSlotRepository.cs

### Application
- src/Application/Common/Constants/SystemSettingKeys.cs
- Updated scheduling handlers:
  - src/Application/Scheduling/AppointmentSlots/Commands/CreateAppointmentSlot/CreateAppointmentSlotCommandHandler.cs
  - src/Application/Scheduling/AppointmentSlots/Commands/GenerateSlots/GenerateSlotsCommandHandler.cs
  - src/Application/Scheduling/ScheduleTemplates/Commands/CreateScheduleTemplate/CreateScheduleTemplateCommandHandler.cs
- Admin dashboard usage query:
  - src/Application/SystemAdmin/Dashboard/Queries/GetPartTimeSlotQuotaUsage/GetPartTimeSlotQuotaUsageQuery.cs
  - src/Application/SystemAdmin/Dashboard/Queries/GetPartTimeSlotQuotaUsage/GetPartTimeSlotQuotaUsageQueryHandler.cs
  - src/Application/SystemAdmin/Dashboard/Queries/GetPartTimeSlotQuotaUsage/PartTimeSlotQuotaUsageDto.cs

### API / Jobs
- src/API/Controllers/SystemAdmin/DashboardController.cs
  - GET api/system-admin/dashboard/part-time-slot-usage
- src/Infrastructure/Services/FullTimeSlotGenerationJob.cs
- Updated startup wiring:
  - src/Infrastructure/DependencyInjection.cs
  - src/API/Program.cs

### Persistence migration
- src/Infrastructure/Persistence/Migrations/20260407180715_AddDailySlotQuotaAndSlotSources.cs
- Includes:
  - DailySlotQuotas table
  - Source columns on templates and slots
  - New settings seed values:
    - FULLTIME_SLOT_WINDOW_DAYS = 30
    - PART_TIME_MAX_SLOTS_PER_DAY = 100
  - Backfill SQL for existing full-time template/slot source values
  - Unique idempotency index for generated slots

## Runtime Behavior

### Part-time slot create / generate
- Quota is reserved in transaction before slot insert (bulk reserve per day for generate).
- If quota is exhausted, request fails with business error.
- Bounded retry handles transient concurrency conflicts.

### Full-time slot rules
- Full-time ophthalmologists cannot manually create or generate slots.
- System job continuously maintains future slots in rolling window.

### Monitoring and logs
- Structured logs for reserve success/failure and near-limit conditions.
- Admin endpoint provides per-day used/quota/remaining values.

## Configuration keys
- PART_TIME_MAX_SLOTS_PER_DAY
- FULLTIME_SLOT_WINDOW_DAYS

Both can be changed at runtime through system settings.

## Validation status
- Solution build passed.
- Application/API/Infrastructure unit test suites relevant to the scope passed during implementation session.
