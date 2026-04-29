# Seeded Accounts Reference

> Auto-generated from `DatabaseSeeder` + `DigitalClinicDemoSeeder`.
> All passwords are demo values. Reset in production.

---

## System Admin

| Email | Password | Role | Full Name |
|-------|----------|------|-----------|
| `systemadmin@auraeyes.vn` | `Admin@123$` | SystemAdmin | Clinic Owner (System Admin) |

---

## Ophthalmologists (Doctors)

| Email | Password | Role | Full Name | Employment Type | Workload |
|-------|----------|------|-----------|-----------------|----------|
| `doctor@auraeyes.vn` | `Doctor@123$` | Ophthalmologist | BS. Nguyen Van An | FullTime | **Overloaded** — 5 assigned cases |
| `demo-doctor-hoa@auraeyes.vn` | `Doctor@123$` | Ophthalmologist | BS. Tran Thi Hoa | FullTime | Moderate — 4 assigned cases |
| `demo-doctor-chien@auraeyes.vn` | `Doctor@123$` | Ophthalmologist | BS. Le Minh Chien | PartTime | **Idle** — 1 assigned case |

---

## Clinic Staff

| Email | Password | Role | Full Name | Sub-role |
|-------|----------|------|-----------|----------|
| `receptionist@auraeyes.vn` | `Staff@123$` | ClinicStaff | Tran Thi Binh - Receptionist | Receptionist |
| `coordinator@auraeyes.vn` | `Staff@123$` | ClinicStaff | Le Van Ca - Coordinator | Coordinator |
| `cashier@auraeyes.vn` | `Staff@123$` | ClinicStaff | Pham Thi Dung - Cashier | Cashier |

---

## Patients (Base Account)

| Email | Password | Role | Full Name |
|-------|----------|------|-----------|
| `patient@auraeyes.vn` | `Patient@123$` | Patient | Nguyen Van Em |

---

## Demo Patients (Digital Clinic Flow)

| Email | Password | Role | Full Name | AI Risk | Doctor Assigned | Scenario |
|-------|----------|------|-----------|---------|---------------|----------|
| `demo-patient-a@auraeyes.vn` | `Patient@123$` | Patient | Nguyen Van A | Low | Dr. An | Normal — checked in 5m ago |
| `demo-patient-b@auraeyes.vn` | `Patient@123$` | Patient | Tran Thi B | **High** | Dr. An | Urgent — waiting 25m |
| `demo-patient-c@auraeyes.vn` | `Patient@123$` | Patient | Le Minh C | **Critical** | Dr. Hoa | Urgent — waiting 35m |
| `demo-patient-d@auraeyes.vn` | `Patient@123$` | Patient | Pham Thi D | Moderate | Dr. An | **In Progress** — reviewing |
| `demo-patient-e@auraeyes.vn` | `Patient@123$` | Patient | Hoang Van E | Low | Dr. An | Completed visit |
| `demo-patient-f@auraeyes.vn` | `Patient@123$` | Patient | Vo Thi F | — | Dr. Hoa | **No Show** |
| `demo-patient-g@auraeyes.vn` | `Patient@123$` | Patient | Dang Van G | Moderate | Dr. Hoa | **Walk-in** (no appointment) |
| `demo-patient-h@auraeyes.vn` | `Patient@123$` | Patient | Bui Thi H | — | Dr. Hoa | Pending AI screening |
| `demo-patient-i@auraeyes.vn` | `Patient@123$` | Patient | Ngo Van I | Low | Dr. An | Checked in 20m ago |
| `demo-patient-k@auraeyes.vn` | `Patient@123$` | Patient | Ly Thi K | **High** | Dr. Hoa | Checked in 5m ago |
| `demo-patient-l@auraeyes.vn` | `Patient@123$` | Patient | Do Van L | Low | Dr. Hoa | Checked in 5m ago |
| `demo-patient-m@auraeyes.vn` | `Patient@123$` | Patient | Truong Thi M | Low | Dr. Chien | Checked in 8m ago |

---

## Quick Login Cheatsheet

```bash
# Doctor (overloaded)
curl -X POST https://api.auraeyes.vn/api/auth/login \
  -d '{"email":"doctor@auraeyes.vn","password":"Doctor@123$"}'

# Doctor (moderate)
curl -X POST https://api.auraeyes.vn/api/auth/login \
  -d '{"email":"demo-doctor-hoa@auraeyes.vn","password":"Doctor@123$"}'

# System Admin
curl -X POST https://api.auraeyes.vn/api/auth/login \
  -d '{"email":"systemadmin@auraeyes.vn","password":"Admin@123$"}'

# Patient
curl -X POST https://api.auraeyes.vn/api/auth/login \
  -d '{"email":"demo-patient-c@auraeyes.vn","password":"Patient@123$"}'
```

---

## Demo Scenarios Summary

| # | Scenario | Patient | Status | Risk |
|---|----------|---------|--------|------|
| 1 | Normal flow | Nguyen Van A | CheckedIn, Pending | Low |
| 2 | Urgent case (High) | Tran Thi B | CheckedIn, Pending | **High** |
| 3 | Urgent case (Critical) | Le Minh C | CheckedIn, Confirmed | **Critical** |
| 4 | In Progress | Pham Thi D | InProgress | Moderate |
| 5 | Completed | Hoang Van E | Completed | Low |
| 6 | No Show | Vo Thi F | NoShow | — |
| 7 | Walk-in | Dang Van G | CheckedIn, Pending | Moderate |
| 8 | Pending AI | Bui Thi H | CheckedIn, Pending | — |
| 9 | Urgent case (High) | Ly Thi K | CheckedIn, Pending | **High** |
