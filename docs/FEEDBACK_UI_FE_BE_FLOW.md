# Feedback UI/UX + FE-BE Flow (Patient)

Version: 1.0  
Date: 2026-03-13

## 1. Goal

Thiết kế trải nghiệm feedback cho 3 ngữ cảnh tùy chọn:

1. Website feedback (sau khi patient đã dùng AI analytic >= 1 lần)
2. Organisation feedback (sau khi clinic appointment COMPLETED)
3. Ophthalmologist feedback (sau khi consultation session COMPLETED)

Mục tiêu UX:

- Khuyến khích feedback nhưng không gây phiền.
- Gửi nhanh trong 15-30 giây.
- Rõ trạng thái đã gửi/chưa gửi, không submit trùng.

## 2. Information Architecture

### 2.1 Entry Points

- Organisation feedback:
  - Trang Past Appointments trong dashboard patient.
  - Hiển thị CTA khi appointment.status = COMPLETED và chưa feedback.
- Ophthalmologist feedback:
  - Post-session modal ngay sau khi kết thúc consultation.
  - Fallback ở Consultation History item (nếu user đóng modal).
- Website feedback:
  - Footer link hoặc mục Help & Feedback.
  - Chỉ hiển thị khi user có ít nhất 1 AI analytic processed result.

### 2.2 Main User Journey

1. User thấy prompt đúng ngữ cảnh (completed event).
2. Click CTA Leave Feedback.
3. Chọn sao (bắt buộc), nhập comment (optional), chọn category (website).
4. Submit.
5. Nhận success state + UI item chuyển thành Feedback Submitted.

## 3. UI Flow by Type

## 3.1 Organisation Feedback Flow

### Page Structure

- Page: Past Appointments.
- Card mỗi appointment gồm:
  - Basic info (clinic, date/time, doctor nếu có, status)
  - Action zone:
    - `Leave feedback` khi eligible
    - `Feedback submitted` badge khi đã gửi

### Components

- `AppointmentFeedbackTrigger` (button/badge)
- `FeedbackBottomSheet` (mobile) / `FeedbackModal` (desktop)
- `StarRatingInput` (1-5)
- `OptionalCommentTextarea` (0-2000)
- `SubmitButton` + `SkipButton`
- `InlineSuccessAlert`

### Interaction States

- Initial: button enabled.
- Hover/focus: highlight + a11y focus ring.
- Submitting: button loading + form disabled.
- Submitted: close modal, update card badge `Feedback submitted`.
- Already submitted: button hidden/disabled + icon check.

### Empty States

- Không có completed appointments:
  - Message: `No completed appointments yet.`
  - CTA: `Book a clinic visit`.

### Success State

- Toast: `Thanks for your feedback.`
- Card state đổi ngay (optimistic hoặc sau refetch).

### Validation Errors

- Rating missing: `Please select a rating from 1 to 5.`
- Comment > 2000: `Comment is too long.`
- Duplicate (409): `Feedback already exists for this appointment.`
- Appointment not completed: `Feedback can only be submitted after completion.`

## 3.2 Ophthalmologist Feedback Flow

### Page Structure

- Trigger A (recommended): post-session modal sau khi session chuyển COMPLETED.
- Trigger B: Consultation History list item với CTA `Rate consultation`.

### Components

- `PostSessionFeedbackModal`
- `ConsultationFeedbackTrigger`
- `StarRatingInput`
- `OptionalCommentTextarea`
- `SubmitButton`
- `NotNowLink`

### Interaction States

- Modal opened once per completed session (không ép submit).
- User chọn `Not now` -> đặt reminder nhẹ trên history item.
- User submit thành công -> modal đóng, card hiển thị submitted badge.

### Empty States

- Không có completed session: empty panel + CTA `Book online consultation`.

### Success State

- Message: `Your review helps improve consultation quality.`

### Validation Errors

- Rating required.
- Session mismatch/not completed (400).
- Duplicate by consultationSessionId (409).

## 3.3 Website Feedback Flow

### Page Structure

- Entry từ footer: `Help & Feedback`.
- Page layout:
  - Left: Why your feedback matters.
  - Right: Form card.

### Components

- `WebsiteFeedbackEntry` (footer/help menu)
- `FeedbackEligibilityGuard` (ẩn entry nếu chưa đủ điều kiện)
- `CategorySegmentedControl` (BUG | UX | SUGGESTION | OTHER)
- `StarRatingInput`
- `OptionalMessageTextarea`
- `SubmitButton`

### Interaction States

- Nếu chưa đủ điều kiện AI usage:
  - Ẩn entry hoặc show disabled + tooltip giải thích.
- Đủ điều kiện:
  - Form active.
- Submit thành công:
  - Success panel + quick links quay lại dashboard.

### Empty States

- Chưa có AI usage:
  - Message: `Use AI analysis at least once to unlock website feedback.`
  - CTA: `Go to AI Screening`.

### Success State

- Thank-you panel + option `Send another later`.

### Validation Errors

- Rating required (1-5)
- Category required
- Comment > 2000
- Eligibility fail: `Feedback is available after your first AI analysis.`

## 4. FE-BE Contract Mapping

Base API: `/api/feedback`

### 4.1 Write APIs

1. Website feedback
- POST `/website`
- Request:
```json
{
  "rating": 5,
  "category": "UX",
  "comment": "The flow is smooth"
}
```

2. Organisation feedback
- POST `/organisations/{organisationId}`
- Request:
```json
{
  "appointmentId": "uuid",
  "rating": 4,
  "comment": "Clinic staff was helpful"
}
```

3. Ophthalmologist feedback
- POST `/ophthalmologists/{ophthalmologistId}`
- Request:
```json
{
  "consultationSessionId": "uuid",
  "rating": 5,
  "comment": "Clear explanation"
}
```

### 4.2 Read APIs

- GET `/website/{feedbackId}`
- GET `/organisations/{organisationId}/items/{feedbackId}`
- GET `/ophthalmologists/{ophthalmologistId}/items/{feedbackId}`
- GET `/organisations/{organisationId}/items?pageNumber=1&pageSize=10`
- GET `/ophthalmologists/{ophthalmologistId}/items?pageNumber=1&pageSize=10`
- GET `/organisations/{organisationId}/rating`
- GET `/ophthalmologists/{ophthalmologistId}/rating`

### 4.3 FE Data Layer Proposal (React Query)

- Query keys:
  - `feedbackKeys.organisationItems(organisationId, page, size)`
  - `feedbackKeys.ophthalmologistItems(ophthalmologistId, page, size)`
  - `feedbackKeys.organisationRating(organisationId)`
  - `feedbackKeys.ophthalmologistRating(ophthalmologistId)`
- Mutations:
  - `useCreateWebsiteFeedback`
  - `useCreateOrganisationFeedback`
  - `useCreateOphthalmologistFeedback`
- Invalidations after success:
  - list + rating summary + appointment/session list item.

### 4.4 Error Mapping (BE -> FE UX)

- 400: validation/business rule -> inline form error or alert banner.
- 401: redirect login + preserve return URL.
- 403: show role-specific restriction message.
- 404: show not found state for detail page.
- 409: show duplicate submission message and lock form.

## 5. Page Layout Blueprint

### 5.1 Past Appointments (Organisation)

- Top filter tabs: All | Completed | Cancelled.
- Appointment card:
  - Left: summary info.
  - Right: action area (`Leave feedback` or submitted badge).
- Feedback modal:
  - Header context (clinic name/date)
  - 5-star control
  - Optional comment
  - Actions row

### 5.2 Consultation History (Ophthalmologist)

- Session timeline list.
- Completed session row:
  - `Rate consultation` CTA.
- Post-session modal shared component with different copy.

### 5.3 Help & Feedback (Website)

- Hero copy + trust note.
- Form card with category chips + stars + message.
- After success, show confirmation card instead of form.

## 6. Component Structure (Suggested)

- `src/features/patient/components/feedback/StarRatingInput.tsx`
- `src/features/patient/components/feedback/FeedbackModal.tsx`
- `src/features/patient/components/feedback/FeedbackSuccessState.tsx`
- `src/features/patient/components/feedback/FeedbackSubmittedBadge.tsx`
- `src/features/patient/components/feedback/WebsiteFeedbackForm.tsx`
- `src/features/patient/hooks/use-feedback.ts`
- `src/features/patient/api/feedback.api.ts`
- `src/features/patient/types/feedback.types.ts`

## 7. UX Patterns to Encourage Feedback (Non-intrusive)

- Gentle prompt window:
  - Show after completion event, tối đa 1 lần/phiên.
- Snooze option:
  - `Remind me later` thay vì ép đóng.
- Progressive disclosure:
  - Chỉ bắt buộc rating; comment optional.
- Trust cue:
  - Copy rõ `Takes less than 30 seconds`.
- Frequency cap:
  - Không show modal liên tiếp nhiều lần trong cùng phiên user.

## 8. Optional Product Enhancements

- Verified patient badge cạnh review item.
- Hiển thị average rating + total reviews ở doctor/organisation profile.
- Nút filter review theo số sao.
- Doctor profile có snippet `Most helpful reviews`.

## 9. Manual Test Document

## 9.1 Functional Scenarios

1. Organisation feedback happy path
- Preconditions: appointment COMPLETED, chưa feedback.
- Steps:
  - Mở Past Appointments.
  - Click Leave feedback.
  - Chọn 4 sao, nhập comment, submit.
- Expected:
  - API 201.
  - Toast success.
  - Card đổi thành Feedback submitted.

2. Organisation duplicate feedback
- Preconditions: đã feedback appointment đó.
- Steps: submit lại.
- Expected:
  - API 409.
  - UI hiển thị duplicate message.
  - Không tạo record mới.

3. Ophthalmologist feedback via post-session modal
- Preconditions: session vừa COMPLETED.
- Steps:
  - Modal hiện.
  - Chọn 5 sao, submit.
- Expected:
  - API 201.
  - Modal đóng.
  - History row hiển thị submitted badge.

4. Ophthalmologist feedback via history fallback
- Preconditions: modal đã dismiss, session completed.
- Steps:
  - Vào history.
  - Click Rate consultation.
  - Submit.
- Expected: giống scenario 3.

5. Website feedback eligibility ON
- Preconditions: user có ít nhất 1 AI analytic processed.
- Steps:
  - Vào Help & Feedback.
  - Chọn category UX + 5 sao + message.
  - Submit.
- Expected:
  - API 201.
  - Confirmation panel hiển thị.

6. Website feedback eligibility OFF
- Preconditions: user chưa từng AI analytic processed.
- Steps: mở Help & Feedback.
- Expected:
  - Không hiển thị form submit hoặc disabled + hướng dẫn đi screening.

## 9.2 Validation & Error Cases

1. Submit không chọn sao -> chặn client-side.
2. Comment > 2000 -> chặn client-side.
3. Token hết hạn -> API 401, điều hướng login.
4. Role không phải patient -> API 403.
5. Session/appointment không completed -> API 400 + message đúng.

## 9.3 UI/UX Checks

1. Keyboard accessibility cho star input (arrow/enter).
2. Focus trap trong modal hoạt động.
3. Mobile bottom sheet hiển thị đầy đủ, không che nút submit.
4. Loading state không cho double-click submit.
5. Success toast không chặn thao tác kế tiếp.

## 9.4 Regression Checks

1. Past Appointments list vẫn filter/sort đúng.
2. Consultation history không ảnh hưởng chức năng cancel/view detail.
3. Footer/help menu không lỗi với tài khoản chưa eligible.
4. Query invalidation cập nhật rating summary đúng sau submit.

## 10. Release Checklist

- FE env trỏ đúng `VITE_API_END_POINT`.
- API auth header hoạt động cho toàn bộ feedback endpoints.
- Theo dõi metric:
  - feedback conversion rate
  - modal dismiss rate
  - error rate theo endpoint/status code
