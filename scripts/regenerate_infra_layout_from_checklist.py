import re
from pathlib import Path

root = Path('d:/sep/AuraEyes_BE')
checklist_path = root / 'UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md'
out_paths = [
    root / 'UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_SHEET_LAYOUT_V1.md',
    root / 'UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_SHEET_LAYOUT.md',
]
text = checklist_path.read_text(encoding='utf-8')

catalog = []
for line in text.splitlines():
    m = re.match(r'\|\s*(\d+)\s*\|\s*([^|]+?)\s*\|\s*([^|]+?)\s*\|\s*([^|]+?)\s*\|\s*(F\d{3})\s*\|', line)
    if m:
        no, req, cls, fn, code = m.groups()
        catalog.append({
            'code': code,
            'class': cls.strip(),
            'function': fn.strip(),
            'requirement': req.strip(),
        })

counts = {}
for line in text.splitlines():
    m = re.match(r'\|\s*(F\d{3})\s*\|\s*([^|]+?)\s*\|\s*([^|]+?)\s*\|\s*(\d+)\s*\|', line)
    if m:
        counts[m.group(1)] = int(m.group(4))

if len(catalog) != 96 or len(counts) != 96:
    raise SystemExit(f'Expected 96 catalog/count rows, got catalog={len(catalog)}, counts={len(counts)}')

header = """# Infrastructure Unit Test - Sheet Layout Ready

Last updated: 27/04/2026
Source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md

## Hướng dẫn nhanh (tiếng Việt — để copy sang Excel)

**Lưu ý quan trọng:** File này **chỉ copy format markdown** từ bản V1 cũ. Danh sách function/case phải theo checklist Infrastructure hiện tại: **F001-F096, tổng 327 UTCID**. Không lấy lại function/case thừa của V1 cũ như `RegisterOphthalmologistAsync`, `RegisterOrganisationAsync`, `IsPhoneNumberInUseByOrganizationAsync`, ... nếu không còn nằm trong checklist.

**Hai bảng trong mỗi khối `## Fxxx` khác nhau thế nào?**

| Bảng | Mục đích | Một dòng = |
|------|----------|------------|
| **Condition Matrix** | Tóm tắt kịch bản theo checklist | Có thể **nhiều UTCID** (cột `UTCIDs`: `UTCID01-UTCID03`) |
| **Result Matrix** | Chi tiết **từng** case để trace / assert | **Đúng 1 UTCID** mỗi hàng |

**Quy tắc bắt buộc — không bịa**

- **Expected return / Expected exception / Expected log message** chỉ ghi khi **có chứng cứ**: assert hoặc `Verify` trong `tests/Infrastructure.UnitTests`, hoặc bạn đã **xác minh có chủ đích** (ghi nguồn: tên test / dòng assert).
- **Không** điền theo cảm tưởng hay “hợp lý theo code” nếu test không kiểm tra — để **trống** hoặc ghi rõ `n/a (chưa assert trong test)`.
- Cột **log**: chỉ ghi khi test thật sự assert log; không có thì **trống**.

**Làm theo 3 bước**

1. Mở đúng function: tìm `## F001` … `## F096` (cùng mã **F** với bảng catalog trong checklist).
2. Đọc **Condition Matrix** để nắm ý chung. Nội dung được sinh từ checklist hiện tại theo số lượng UTCID chuẩn.
3. Điền **Result Matrix**: mỗi hàng `UTCID01`, `UTCID02`, … — Expected để trống làm placeholder nếu chưa đối chiếu assert/test.

**Sau khi chạy test** mới điền: **Passed/Failed**, **Executed Date**, **Defect ID**.

**Excel của bạn (Confirm: 3 hàng Return / Exception / Log × nhiều cột UTCID)**  
Trong file markdown là **3 cột** trên **một hàng** (một UTCID). Nội dung giống nhau, chỉ khác xoay bảng: copy từ markdown sang Excel rồi **Transpose** (dán chuyển vị), hoặc điền tay theo cùng một ý.

**Copy markdown → bảng Excel:** dùng công cụ chuyển markdown table sang grid (ví dụ tableconvert) rồi dán vào sheet, hoặc copy trực tiếp từng bảng.

---

## How to copy into Sheet / Excel (English)

- This file keeps the **V1 markdown format only**. Function list and UTCID counts come from the current Infrastructure checklist: **F001-F096, 327 total UTCIDs**.
- **Do not invent.** Put something in **Expected return / exception / log** only when **evidence exists**: an assert or `Verify` in `tests/Infrastructure.UnitTests`, or you explicitly verified and cite the test name. Otherwise leave **empty** or write `n/a (not asserted in test)`. **Log** only if the test asserts logging.
- Each block `## F001` … `## F096` is **one function**; copy the whole block into one sheet or one table range.
- In the Header table **Value** column, fill in **Created By**, **Executed By**, **Lines of Code**, **Passed / Failed / Untested**, **Count type N / A / B** after you run tests.
- **Condition Matrix** = high-level summary; one row may list **several** UTCIDs in the third column.
- **Result Matrix** = **one row per UTCID**; Expected columns are placeholders until verified from tests/asserts.
- **Passed/Failed**, **Executed Date**, **Defect ID** — after execution only.
- Excel **Confirm** uses **rows** (Return / Exception / Log) × UTCID **columns**; markdown uses **columns** on one row per UTCID — same content, transpose when pasting.

---
"""

def utc_range(n: int) -> str:
    return 'UTCID01' if n == 1 else f'UTCID01-UTCID{n:02d}'

parts = [header]
for item in catalog:
    code = item['code']
    total = counts[code]
    full_name = f"{item['class']}.{item['function']}"
    parts.append(f"\n## {code} - {full_name}\n\n")
    parts.append("| Header | Value |\n|---|---|\n")
    rows = [
        ('Function Code', code),
        ('Function Name', full_name),
        ('Total Test Cases', str(total)),
        ('Created By', ''),
        ('Executed By', ''),
        ('Lines of Code', ''),
        ('Passed', ''),
        ('Failed', ''),
        ('Untested', ''),
        ('Count type N', ''),
        ('Count type A', ''),
        ('Count type B', ''),
        ('Test Requirement', f"Validate '{item['requirement']}' in {full_name}, following the current Infrastructure checklist."),
    ]
    for k, v in rows:
        parts.append(f"| {k} | {v} |\n")
    parts.append("\n### Condition Matrix\n\n")
    parts.append("| Condition | Precondition | UTCIDs |\n|---|---|---|\n")
    parts.append(f"| Checklist-defined scenarios for {full_name}; split into {total} UTCID row(s) in Result Matrix. | Use the pre-condition from Function Catalog and scenario setup from checklist/tests. | {utc_range(total)} |\n")
    parts.append("\n### Result Matrix\n\n")
    parts.append("| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |\n")
    parts.append("|---|---|---|---|---|---|---|---|\n")
    for i in range(1, total + 1):
        parts.append(f"| UTCID{i:02d} |  |  |  |  |  |  |  |\n")
    parts.append("\n---\n")

output = ''.join(parts).rstrip() + '\n'
for path in out_paths:
    path.write_text(output, encoding='utf-8')
print(f'Wrote {len(catalog)} function blocks, {sum(counts.values())} UTCID rows to {len(out_paths)} files')
