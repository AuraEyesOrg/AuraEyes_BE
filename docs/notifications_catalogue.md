# Danh Mục Thông Báo Hệ Thống AURA (AURA Notification Catalogue)

Tài liệu này tổng hợp toàn bộ các thông báo được gửi từ hệ thống AURA Digital Clinic, bao gồm điều kiện kích hoạt, đối tượng nhận, nội dung chi tiết và đường dẫn điều hướng (routing).

---

## 1. Hồ Sơ Bệnh Án & Quy Trình Khám (Medical Records & Clinical Flow)

### Kết quả Sàng lọc AI đã sẵn sàng (AI Screening Completed)
- **Khi nào gửi**: Hệ thống AI hoàn tất phân tích ảnh chụp đáy mắt.
- **Gửi cho ai**: Bệnh nhân (và Nhân viên phòng khám để theo dõi).
- **Nội dung**:
    - **Tiêu đề**: `Kết quả sàng lọc AI đã sẵn sàng`
    - **Thông điệp**: `Kết quả sàng lọc đáy mắt AI của bạn đã có. Vui lòng xem chi tiết trong ứng dụng.`
- **Điều hướng (Link)**:
    - **Bệnh nhân**: `/vi/patient/screening?screeningId={ScreeningId}`
    - **Nhân viên**: `/vi/clinic-staff/management/screening/result?id={ScreeningId}`

### Giao ca khám mới (New Case Assigned)
- **Khi nào gửi**: Nhân viên Coordinator chỉ định bệnh nhân cho bác sĩ sau khi sàng lọc xong.
- **Gửi cho ai**: Bác sĩ (Ophthalmologist).
- **Nội dung**:
    - **Tiêu đề**: `Ca khám mới được chỉ định`
    - **Thông điệp**: `Bạn vừa được giao một ca khám mới cho bệnh nhân {FullName}.`
- **Điều hướng (Link)**: `/vi/ophthalmologist/screenings/{ScreeningId}/review`

### Hoàn thành Bệnh án (Medical Record Finalized)
- **Khi nào gửi**: Bác sĩ nhấn "Finalize" để khóa hồ sơ bệnh án.
- **Gửi cho ai**: Bệnh nhân & Nhân viên thu ngân.
- **Nội dung (Bệnh nhân)**: `Hồ sơ {MedicalRecordNumber} đã sẵn sàng. Bạn có thể xem trên ứng dụng hoặc email.`
- **Nội dung (Nhân viên)**: `Bệnh nhân {FullName} ({MedicalRecordNumber}) đã khám xong. Vui lòng kiểm tra thanh toán.`
- **Điều hướng (Link)**:
    - **Bệnh nhân**: `/vi/medical-records/patient/{RecordId}`
    - **Nhân viên (Cashier)**: `/vi/clinic-staff/cashier?visitId={VisitId}`

---

## 2. Hội Chẩn & Cộng Tác (Consilium & Collaboration)

### Yêu cầu Hội chẩn Khẩn cấp (Urgent Consilium Request)
- **Khi nào gửi**: Một bác sĩ mời đồng nghiệp tham gia hội chẩn một ca bệnh khó thông qua Network.
- **Gửi cho ai**: Bác sĩ được mời (Invited Doctor).
- **Nội dung**:
    - **Tiêu đề**: `Mời hội chẩn lâm sàng khẩn cấp`
    - **Thông điệp**: `Bác sĩ {InviterName} mời bạn hội chẩn ca bệnh: {GroupName}.`
- **Điều hướng (Link)**: `/vi/network/collaborations` (Tham gia nhóm chat chuyên môn).

---

## 3. Tài Chính & Thanh Toán (Financial & Payments)

### Nạp tiền Cọc Thành công (Deposit Successful / Wallet Top-up)
- **Khi nào gửi**: Bệnh nhân nạp tiền vào ví hoặc đặt cọc lịch hẹn thành công qua PayOS.
- **Gửi cho ai**: Bệnh nhân.
- **Nội dung**:
    - **Tiêu đề**: `Giao dịch thành công`
    - **Thông điệp**: `Bạn đã nạp thành công {Amount} VND vào tài khoản AURA.`
- **Điều hướng (Link)**: `/vi/patient/wallet`

### Thanh toán Viện phí Thành công (Payment Completed)
- **Khi nào gửi**: Bệnh nhân thanh toán đủ viện phí tại quầy hoặc qua ứng dụng.
- **Gửi cho ai**: Bệnh nhân.
- **Nội dung**:
    - **Tiêu đề**: `Thanh toán thành công`
    - **Thông điệp**: `Đơn hàng {OrderCode} trị giá {Amount} VND đã được thanh toán thành công.`
- **Điều hướng (Link)**: `/vi/patient/wallet` (Lịch sử giao dịch).

---

## 4. Lịch Hẹn (Scheduling)

### Đặt lịch khám thành công (Appointment Booked)
- **Khi nào gửi**: Khi một lịch hẹn mới được tạo (online hoặc walk-in).
- **Gửi cho ai**: Bệnh nhân.
- **Nội dung**: `Bạn đã đặt lịch thành công vào lúc {Time}, ngày {Date}.`
- **Điều hướng (Link)**: `/vi/patient/appointments`

---

## Nguyên Tắc Bảo Mật & UX (Hardening Rules)
1. **KHÔNG HIỂN THỊ GUID**: Tuyệt đối không đưa chuỗi ID thô (ví dụ: `550e8400...`) vào nội dung thông báo. Phải sử dụng FullName, MedicalRecordNumber hoặc OrderCode.
2. **NGÔN NGỮ ĐỒNG NHẤT**: Tất cả thông báo hướng tới người dùng phải bằng tiếng Việt (hoặc theo locale đã chọn).
3. **ĐIỀU HƯỚNG CHUẨN**: Mọi thông báo phải đi kèm với link điều hướng chính xác tới trang xử lý nghiệp vụ, không chỉ gửi text suông.
