# Danh Mục Thông Báo Hệ Thống AURA

Tài liệu này tổng hợp toàn bộ các thông báo được gửi từ hệ thống, bao gồm điều kiện kích hoạt, đối tượng nhận, nội dung chi tiết và đường dẫn liên kết tương ứng.

---

## 1. Hồ Sơ Bệnh Án & Khám Bệnh

### Hoàn thành Bệnh án (Medical Record Finalized)
- **Khi nào gửi**: Bác sĩ nhấn nút "Hoàn thành" (Finalize) để khóa hồ sơ bệnh án sau khi khám xong.
- **Gửi cho ai**: 
    - **Bệnh nhân**: Nhận thông báo trong app và Email.
    - **Nhân viên (Coordinator)**: Nhận thông báo hệ thống để làm thủ tục thanh toán.
- **Nội dung (Bệnh nhân)**:
    - **Tiêu đề**: `Hồ sơ bệnh án đã hoàn thành`
    - **Thông điệp**: `Hồ sơ {record.MedicalRecordNumber} đã sẵn sàng. Bạn có thể xem chi tiết tại đây.`
    - **Link**: `/vi/medical-records/patient/{Id}`
- **Nội dung (Nhân viên)**:
    - **Tiêu đề**: `Bệnh án đã được khóa`
    - **Thông điệp**: `Bệnh nhân {FullName} ({record.MedicalRecordNumber}) đã khám xong. Vui lòng kiểm tra thanh toán.`
    - **Link**: `/vi/cashier/checkout/{VisitId}`

### Giao ca khám mới (New Case Assigned)
- **Khi nào gửi**: Nhân viên điều phối chỉ định một bệnh nhân vào phòng khám của một Bác sĩ cụ thể.
- **Gửi cho ai**: **Bác sĩ (Ophthalmologist)**.
- **Nội dung**:
    - **Tiêu đề**: `Ca khám mới được chỉ định`
    - **Thông điệp**: `Bạn vừa được giao một ca khám mới cho bệnh nhân {FullName}.`
    - **Link**: `/vi/doctor/cases/{ConsultationSessionId}`

---

## 2. Lịch Hẹn & Tiếp Nhận

### Đặt lịch hẹn thành công (Appointment Created)
- **Khi nào gửi**: Bệnh nhân hoặc nhân viên tạo mới một lịch hẹn trên hệ thống.
- **Gửi cho ai**: **Bệnh nhân**.
- **Nội dung**:
    - **Tiêu đề**: `Xác nhận lịch hẹn thành công`
    - **Thông điệp**: `Lịch hẹn của bạn tại Aura Digital Clinic vào lúc {DateTime} đã được xác nhận.`
    - **Link**: `/vi/appointments/{AppointmentId}`

### Bệnh nhân đã đến (Appointment Check-In)
- **Khi nào gửi**: Bệnh nhân đến phòng khám và nhân viên nhấn nút "Check-in" trên danh sách chờ.
- **Gửi cho ai**: **Nhân viên phòng khám (Điều phối/Sàng lọc)**.
- **Nội dung**:
    - **Tiêu đề**: `Bệnh nhân đã có mặt`
    - **Thông điệp**: `Bệnh nhân {FullName} đã làm thủ tục và đang đợi tại phòng sàng lọc.`
    - **Link**: `/vi/reception/queue`

---

## 3. Sàng lọc & Kết quả AI

### Kết quả AI đã sẵn sàng (AI Screening Completed)
- **Khi nào gửi**: Hệ thống AI hoàn tất việc phân tích ảnh chụp đáy mắt của bệnh nhân.
- **Gửi cho ai**: **Bệnh nhân**.
- **Nội dung**:
    - **Tiêu đề**: `Kết quả sàng lọc AI đã sẵn sàng`
    - **Thông điệp**: `Kết quả phân tích ảnh mắt ngày {Date} của bạn đã có. Nhấn để xem chi tiết.`
    - **Link**: `/vi/screening/results/{ScreeningId}`

---

## 4. Quản trị & Nhân sự

### Trạng thái đơn nghỉ phép (Leave Request Status)
- **Khi nào gửi**: Quản trị viên (Admin) phê duyệt hoặc từ chối đơn xin nghỉ của bác sĩ.
- **Gửi cho ai**: **Bác sĩ (Ophthalmologist)**.
- **Nội dung**:
    - **Tiêu đề**: `Trạng thái đơn nghỉ phép: [Duyệt/Từ chối]`
    - **Thông điệp**: `Đơn nghỉ ngày {Date} của bạn đã được [Phê duyệt/Từ chối]. [Lý do nếu có].`
    - **Link**: `/vi/profile/leave-requests`

### Xác minh tài khoản chuyên môn (Account Verification)
- **Khi nào gửi**: Admin xác thực danh tính và chứng chỉ hành nghề của bác sĩ trên hệ thống.
- **Gửi cho ai**: **Bác sĩ (Ophthalmologist)**.
- **Nội dung**:
    - **Tiêu đề**: `Tài khoản đã được xác minh`
    - **Thông điệp**: `Chứng chỉ hành nghề của bạn đã được xác thực. Bạn có thể bắt đầu tiếp nhận ca khám.`
    - **Link**: `/vi/doctor/dashboard`

---
*Ghi chú: Các biến trong ngoặc nhọn `{}` sẽ được hệ thống thay thế bằng dữ liệu thực tế tại thời điểm gửi.*
