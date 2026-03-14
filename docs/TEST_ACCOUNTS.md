# Tài Khoản Test - Aura Eyes System

> **Lưu ý quan trọng**: Các tài khoản này chỉ dùng cho mục đích phát triển và test. **KHÔNG SỬ DỤNG** trên môi trường production.

## 📋 Danh Sách Tài Khoản

### 1. System Administrator (Quản trị viên hệ thống)

| Thông tin      | Giá trị                 |
| -------------- | ----------------------- |
| **Email**      | `systemadmin@gmail.com` |
| **Mật khẩu**   | `SystemAdmin@123$`      |
| **Vai trò**    | `SystemAdmin`           |
| **Trạng thái** | Email đã xác nhận ✅    |

**Quyền hạn:**

- Toàn quyền truy cập hệ thống
- Quản lý tất cả người dùng và tổ chức
- Cấu hình hệ thống
- Xem tất cả báo cáo và thống kê

---

### 2. Organization Administrator (Quản trị viên tổ chức)

| Thông tin      | Giá trị              |
| -------------- | -------------------- |
| **Email**      | `orgadmin@gmail.com` |
| **Mật khẩu**   | `OrgAdmin@123$`      |
| **Vai trò**    | `OrgAdmin`           |
| **Trạng thái** | Email đã xác nhận ✅ |

**Quyền hạn:**

- Quản lý người dùng trong tổ chức
- Quản lý cài đặt tổ chức
- Xem báo cáo của tổ chức
- Quản lý bác sĩ nhãn khoa trong tổ chức

---

### 3. Ophthalmologist (Bác sĩ nhãn khoa)

| Thông tin      | Giá trị                     |
| -------------- | --------------------------- |
| **Email**      | `ophthalmologist@gmail.com` |
| **Mật khẩu**   | `Ophthalmologist@123$`      |
| **Vai trò**    | `Ophthalmologist`           |
| **Trạng thái** | Email đã xác nhận ✅        |

**Quyền hạn:**

- Xem lịch khám và danh sách bệnh nhân
- Đánh giá kết quả sàng lọc võng mạc
- Đưa ra chẩn đoán và khuyến nghị
- Quản lý hồ sơ y tế bệnh nhân

---

### 4. Patient (Bệnh nhân)

| Thông tin      | Giá trị              |
| -------------- | -------------------- |
| **Email**      | `patient@gmail.com`  |
| **Mật khẩu**   | `Patient@123$`       |
| **Vai trò**    | `Patient`            |
| **Trạng thái** | Email đã xác nhận ✅ |

**Quyền hạn:**

- Đặt lịch khám sàng lọc võng mạc
- Xem kết quả khám của bản thân
- Quản lý thông tin cá nhân
- Nhận thông báo và nhắc nhở

---

## 🔐 Cách Đăng Nhập

### Sử dụng Swagger UI

1. Chạy ứng dụng: `dotnet run --project src/API`
2. Truy cập Swagger: `https://localhost:5001/swagger`
3. Gọi endpoint `POST /api/auth/login` với body:

```json
{
  "email": "systemadmin@gmail.com",
  "password": "SystemAdmin@123$"
}
```

4. Copy `accessToken` từ response
5. Click nút **Authorize** ở góc trên bên phải
6. Nhập: `Bearer <accessToken>`
7. Click **Authorize** để xác thực

### Sử dụng Postman / HTTP Client

```http
POST /api/auth/login HTTP/1.1
Host: localhost:5001
Content-Type: application/json

{
  "email": "systemadmin@gmail.com",
  "password": "SystemAdmin@123$"
}
```

---

## ⚠️ Lưu Ý Bảo Mật

1. **Đây là tài khoản test** - Chỉ sử dụng trong môi trường development
2. **Không commit credentials thật** - Sử dụng biến môi trường cho production
3. **Thay đổi mật khẩu** - Khi deploy lên staging/production
4. **Xóa tài khoản test** - Trước khi go-live

---

## 📝 Seed Logic

Các tài khoản này được tạo tự động khi ứng dụng khởi động thông qua `DatabaseSeeder`.

**File:** `src/Infrastructure/Services/DatabaseSeeder.cs`

**Logic:**

- Kiểm tra role đã tồn tại chưa → Tạo nếu chưa có
- Kiểm tra user đã tồn tại chưa → Tạo nếu chưa có
- Gán role cho user sau khi tạo
- Idempotent: Chạy nhiều lần không tạo duplicate

---

_Cập nhật lần cuối: Tháng 1, 2026_
