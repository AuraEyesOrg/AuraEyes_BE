from pathlib import Path
import re

base = Path(r"d:\sep\AuraEyes_BE")
chk = base / "UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md"
sheet = base / "UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_SHEET_LAYOUT.md"
s = chk.read_text(encoding="utf-8")

cat = re.findall(
    r"^\|\s*(\d+)\s*\|\s*([^|]+?)\s*\|\s*([^|]+?)\s*\|\s*([^|]+?)\s*\|\s*(F\d{3})\s*\|\s*([^|]+?)\s*\|\s*([^|]+?)\s*\|\s*([^|]+?)\s*\|",
    s,
    re.M,
)
stats = {
    m[0]: (m[1].strip(), m[2].strip(), int(m[3]))
    for m in re.findall(r"^\|\s*(F\d{3})\s*\|\s*([^|]+?)\s*\|\s*([^|]+?)\s*\|\s*(\d+)\s*\|", s, re.M)
}
rows = []
for no, req, cls, fn, code, sheetname, desc, pre in cat:
    tc = stats.get(code, (cls.strip(), fn.strip(), 0))[2]
    rows.append((int(no), code, req.strip(), cls.strip(), fn.strip(), desc.strip(), pre.strip(), tc))

expected_codes = [f"F{i:03d}" for i in range(1, 97)]
assert len(rows) == 96, len(rows)
assert [r[1] for r in rows] == expected_codes

out = []
out.append("# Infrastructure Unit Test - Sheet Layout Ready\n\n")
out.append("Last updated: 27/04/2026\n")
out.append("Source: UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_CHECKLIST.md\n\n")
out.append("## Audit summary\n\n")
out.append("- Total function blocks: **96** (`F001`-`F096`, continuous).\n")
out.append("- Duplicate function codes: **0**.\n")
out.append("- Missing function codes: **0**.\n")
out.append("- Layout regenerated from checklist Function Catalog + test-case statistics to remove stale/duplicated blocks.\n\n")
out.append("---\n\n")

for no, code, req, cls, fn, desc, pre, tc in rows:
    out.append(f"## {code} - {cls}.{fn}\n\n")
    out.append("| Header | Value |\n|---|---|\n")
    values = [
        ("Function Code", code),
        ("Function Name", f"{cls}.{fn}"),
        ("Total Test Cases", str(tc)),
        ("Created By", ""),
        ("Executed By", ""),
        ("Lines of Code", ""),
        ("Passed", ""),
        ("Failed", ""),
        ("Untested", ""),
        ("Count type N", ""),
        ("Count type A", ""),
        ("Count type B", ""),
        ("Test Requirement", desc),
    ]
    for k, v in values:
        out.append(f"| {k} | {v} |\n")
    out.append("\n### Condition Matrix\n\n")
    out.append("| Condition | Precondition | UTCIDs |\n|---|---|---|\n")
    rng = f"UTCID01-UTCID{tc:02d}" if tc > 1 else "UTCID01"
    out.append(f"| Validate {req} scenarios for `{fn}`. | {pre} | {rng} |\n")
    out.append("\n### Result Matrix\n\n")
    out.append("| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |\n")
    out.append("|---|---|---|---|---|---|---|---|\n")
    for i in range(1, tc + 1):
        out.append(f"| UTCID{i:02d} |  |  |  |  |  |  |  |\n")
    out.append("\n---\n\n")

sheet.write_text("".join(out), encoding="utf-8")

marker = "## Sheet-ready UTCID chi tiet tung function"
idx = s.find(marker)
if idx != -1:
    clean = """## Sheet-ready UTCID chi tiet tung function

Phan nay da duoc reset ngay 27/04/2026 de tranh trung/lac ma. Chi tiet Result Matrix chuan nam trong `UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_SHEET_LAYOUT.md`.

### Audit summary

- Function Catalog: **96** function, `F001`-`F096` lien tuc.
- Bang thong ke test case: **96** function, khop catalog.
- Sheet layout: da regenerate sach, **96** block, khong trung code, khong thieu code.
"""
    chk.write_text(s[:idx] + clean, encoding="utf-8")

print("OK regenerated sheet layout and cleaned checklist tail")
