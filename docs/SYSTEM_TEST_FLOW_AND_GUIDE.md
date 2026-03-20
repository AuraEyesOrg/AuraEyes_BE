# AURA System Test Flow and Guide

## 1. Muc tieu
Tai lieu nay gom nhat thiet ke Kien truc E2E Test (Playwright) va Test Plan cho 5 luong cot loi cua AURA, dong thoi mo ta phan hien thuc backend de chay test theo triet ly:

- Moi truong test doc lap (ASPNETCORE_ENVIRONMENT=Test)
- Mock toan bo external services (Email, Payment, AI model)
- Locator resilient (user-facing locators, khong dung CSS class)
- Hybrid AAA (Arrange/Assert qua Backdoor API, Act qua UI)

## 2. Phan da hien thuc trong code

### 2.1 Test Environment
Da them profile Test trong launch settings:
- File: src/API/Properties/launchSettings.json
- Profile: Test
- Port: https://localhost:5101;http://localhost:5100
- ASPNETCORE_ENVIRONMENT=Test

Da them appsettings cho Test:
- File: src/API/appsettings.Test.json
- Connection string DB test rieng
- Tat Hangfire worker trong test
- Them key bao ve backdoor: Testing:BackdoorKey

### 2.2 Mock External Services
Da them test doubles:
- FakeEmailService (khong goi SMTP that)
- FakePayOSService (khong goi gateway that, quan ly trang thai payment trong memory)

Da wire vao Program chi khi environment la Test:
- IEmailService -> FakeEmailService
- IPayOSService -> FakePayOSService

### 2.3 TestBackdoorController
Da them controller:
- Route base: /api/test-backdoor
- Chi cho phep khi ENV=Test
- Bat buoc header: X-Test-Key = Testing:BackdoorKey

Endpoints da co:
- POST /api/test-backdoor/reset-and-seed
- GET /api/test-backdoor/email/verify-token?email=
- POST /api/test-backdoor/email/confirm
- POST /api/test-backdoor/payments/mark-success
- POST /api/test-backdoor/payments/verify
- POST /api/test-backdoor/screenings/complete-mock-ai

## 3. Cach chay nhanh

### 3.1 Run backend o mode Test
- Chon launch profile Test trong src/API/Properties/launchSettings.json
- Dam bao DB test ton tai va truy cap duoc

### 3.2 Header bat buoc cho backdoor
Tat ca request den /api/test-backdoor phai co:
- Header: X-Test-Key: <gia tri Testing:BackdoorKey trong appsettings.Test.json>

### 3.3 Luong setup chuan cho moi E2E suite
1. Goi reset-and-seed
2. Goi email/confirm hoac email/verify-token tuy test case
3. Setup payment state qua payments/mark-success (neu can)
4. Setup AI result qua screenings/complete-mock-ai (neu can)

## 4. Locator Policy cho Playwright

Bat buoc su dung:
- getByRole
- getByLabel
- getByPlaceholder
- getByText
- data-testid (neu can bo sung)

Khong duoc dung:
- CSS class selectors nhu .btn, .input-group, .xyz

## 5. Hybrid AAA Pattern

### Arrange (Backdoor API)
- reset data
- seed du lieu role/user/slot/contract
- bypass email
- mock payment state
- mock AI completion

### Act (UI only)
- thao tac trang web dung locators user-facing

### Assert (Backdoor/API + UI)
- assert business state qua endpoint/domain data
- assert UI state (toast, badge, status)
- assert notification realtime

## 6. Test Plan theo 5 luong cot loi

## 6.1 Doctor Onboarding
Flow:
- Register doctor -> Upload credentials -> Email confirm bypass -> Admin verify credentials -> Doctor upload signed contract -> Admin approve contract -> Doctor login

Steps:
1. Arrange
- reset-and-seed
- seed admin account + contract template
2. Act UI
- Doctor dang ky va upload license/degree
3. Arrange
- email/confirm cho doctor
4. Act UI
- Admin verify doctor
- Doctor upload signed contract
- Admin approve/sign contract
- Doctor login
5. Assert
- VerificationStatus=Approved
- Contract status active/pending-signature dung voi business rule
- Doctor vao duoc dashboard

## 6.2 AI Quota and Screening
Flow:
- Upload retinal image -> Mock AI response -> Quota deduct -> Notification

Steps:
1. Arrange
- reset-and-seed
- seed patient co quota
2. Act UI
- Upload screening
3. Arrange
- screenings/complete-mock-ai
4. Act UI
- mo notifications/reports
5. Assert
- Screening processed
- Quota giam
- Notification AiScreeningCompleted xuat hien

## 6.3 Appointment Booking
Flow:
- Patient chon slot -> Dat lich -> DB status doi -> SignalR notification

Steps:
1. Arrange
- reset-and-seed
- tao slot available
2. Act UI
- chon slot va confirm booking
3. Assert
- Slot status doi dung
- Appointment record duoc tao
- Notification NewAppointmentBooked xuat hien

## 6.4 Top-up Quota
Flow:
- Top-up wallet -> Mock payment success -> Verify payment -> Buy quota -> Quota tang

Steps:
1. Arrange
- reset-and-seed
- patient wallet balance ban dau
2. Act UI
- tao deposit tu wallet page
3. Arrange
- payments/mark-success
- payments/verify
4. Act UI
- mua quota bundles
5. Assert
- wallet tang roi giam dung theo business
- quota tang
- notification wallet payment xuat hien

## 6.5 Consultation Result
Flow:
- Doctor submit consultation result -> Patient thay report -> Download PDF

Steps:
1. Arrange
- reset-and-seed
- seed consultation session + ai screening linked
2. Act UI
- doctor submit report
3. Assert
- medical diagnosis duoc tao
- patient nhan notification ConsultationResultProvided
- patient thay report va tai duoc PDF

## 7. Goi y skeleton Playwright suite

- test.beforeEach:
  - call reset-and-seed
  - login state theo role
- helper api client:
  - inject X-Test-Key
- page object:
  - chi user-facing locators
- assert:
  - db/business state qua backdoor
  - ui state qua expect

## 8. Luu y van hanh

- Khong su dung external service that trong test run
- Moi test case can du lieu doc lap, deterministic
- Neu test realtime flaky, uu tien assert event + poll co timeout ngan
- Khuyen nghi pipeline CI chay profile Test + DB test rieng
