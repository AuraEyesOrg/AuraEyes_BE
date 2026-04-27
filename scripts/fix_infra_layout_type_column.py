from pathlib import Path

path = Path('d:/sep/AuraEyes_BE/UNIT_TEST_FUNCTIONS_04_05_INFRASTRUCTURE_SHEET_LAYOUT.md')
out = []
in_result = False
changed = 0

for line in path.read_text(encoding='utf-8').splitlines():
    stripped = line.strip()
    if stripped == '### Result Matrix':
        in_result = True
        out.append(line)
        continue
    if in_result and line.startswith('---'):
        in_result = False
        out.append(line)
        continue

    if in_result and stripped.startswith('| UTCID | N | A | B |'):
        out.append('| UTCID | Type (N/A/B) | Expected return | Expected exception | Expected log message | Passed/Failed | Executed Date | Defect ID |')
        changed += 1
        continue

    if in_result and stripped == '|---|---|---|---|---|---|---|---|---|---|':
        out.append('|---|---|---|---|---|---|---|---|')
        changed += 1
        continue

    if in_result and stripped.startswith('| UTCID'):
        cells = [c.strip() for c in stripped.strip('|').split('|')]
        if len(cells) >= 10 and cells[0].startswith('UTCID'):
            utc, n, a, b, ret, exc, log, pf, date, defect = (cells + [''] * 10)[:10]
            typ = 'N' if n else ('B' if b else 'A')
            out.append(f'| {utc} | {typ} | {ret} | {exc} | {log} | {pf} | {date} | {defect} |')
            changed += 1
            continue

    out.append(line)

path.write_text('\n'.join(out) + '\n', encoding='utf-8')
print(f'changed={changed}')
