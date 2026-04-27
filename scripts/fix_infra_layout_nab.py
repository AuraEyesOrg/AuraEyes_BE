from pathlib import Path

ROOT = Path('d:/sep/AuraEyes_BE')
FILES = [
    ROOT / 'UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_SHEET_LAYOUT.md',
    ROOT / 'UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_SHEET_LAYOUT_V1.md',
]

for path in FILES:
    out = []
    in_result = False
    for line in path.read_text(encoding='utf-8').splitlines():
        if line.strip() == '### Result Matrix':
            in_result = True
            out.append(line)
            continue
        if in_result and line.startswith('---'):
            in_result = False
            out.append(line)
            continue
        if in_result and 'UTCID' in line and 'Type (N/A/B)' in line:
            out.append('| UTCID | N | A | B | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |')
            continue
        if in_result and line.strip().startswith('|') and set(line.replace('|', '').strip()) <= set('-: '):
            out.append('|---|---|---|---|---|---|---|---|---|---|')
            continue
        if in_result and line.lstrip().startswith('| UTCID'):
            cells = [c.strip() for c in line.strip().strip('|').split('|')]
            if len(cells) >= 8:
                utc, typ, ret, exc, log, pf, date, defect = (cells + [''] * 8)[:8]
                n = 'x' if typ == 'N' else ''
                a = 'x' if typ in ('', 'A') else ''
                b = 'x' if typ == 'B' else ''
                out.append(f'| {utc} | {n} | {a} | {b} | {ret} | {exc} | {log} | {pf} | {date} | {defect} |')
                continue
        out.append(line)
    path.write_text('\n'.join(out) + '\n', encoding='utf-8')
    print(f'fixed {path.name}')
