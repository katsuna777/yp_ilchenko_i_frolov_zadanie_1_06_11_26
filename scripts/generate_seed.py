"""Read original XLSX as ZIP/XML with Python's standard library. No macros executed."""
import datetime
import pathlib
import xml.etree.ElementTree as ET
import zipfile

ROOT = pathlib.Path(__file__).resolve().parents[1]
NS = {'s': 'http://schemas.openxmlformats.org/spreadsheetml/2006/main'}


def read(name):
    with zipfile.ZipFile(ROOT / 'resources' / f'{name}_import.xlsx') as z:
        strings = [''.join(e.itertext()) for e in ET.fromstring(z.read('xl/sharedStrings.xml')).findall('s:si', NS)] if 'xl/sharedStrings.xml' in z.namelist() else []
        rows = []
        for row in ET.fromstring(z.read('xl/worksheets/sheet1.xml')).findall('.//s:row', NS):
            values = {}
            for cell in row.findall('s:c', NS):
                col = ''.join(c for c in cell.attrib['r'] if c.isalpha())
                v = cell.find('s:v', NS)
                value = v.text if v is not None else ''.join(cell.itertext())
                values[col] = strings[int(value)] if cell.get('t') == 's' else value
            rows.append(values)
        return rows[1:]


def q(value):
    # Hex UTF-8 literals avoid dependence on NO_BACKSLASH_ESCAPES/sql_mode.
    return "CONVERT(X'" + str(value).encode('utf-8').hex() + "' USING utf8mb4)"


def model(name, manufacturer):
    name = ' '.join(name.split())
    # Explicit source discrepancy: shortened name in stock/orders (see docs/import.md).
    if name == 'Черные туфли в классическом стиле':
        name = 'Черные туфли в классическом стиле — база для деловых образов'
    return name, manufacturer.strip()


def generate():
    sql = ['-- Generated from the supplied XLSX; stocks are already AVAILABLE quantities.', 'START TRANSACTION;']
    roles = {'Администратор': 'Admin', 'Менеджер': 'Manager', 'Авторизованный пользователь': 'Customer'}
    for i, r in enumerate(read('Users'), 1):
        sql.append(f"INSERT INTO users VALUES ({i},{','.join(q(r[k]) for k in ['D','A','B','C'])},{q(roles[r['E']])});")
    sizes = {}
    for i, r in enumerate(read('Sizes'), 1):
        sizes[r['A']] = i
        sql.append(f"INSERT INTO sizes VALUES ({i},{q(r['A'])});")
    products = {}
    for i, r in enumerate(read('Products'), 1):
        products[model(r['D'], r['E'])] = i
        assert (ROOT / 'resources' / 'Images' / r['C']).is_file(), r['C']
        sql.append(f"INSERT INTO products VALUES ({i},{','.join(q(r[k]) for k in ['A','B','C','D','E','F','G'])},{float(r['H']):.2f});")
    stock = {}
    for i, r in enumerate(read('Stock_Items'), 1):
        pid = products[model(r['A'], r['B'])]
        stock[(pid, r['C'])] = i
        sql.append(f"INSERT INTO stock_items VALUES ({i},{pid},{sizes[r['C']]},{int(r['D'])});")
    orders = set()
    for r in read('Orders'):
        oid = int(r['A'])
        if oid not in orders:
            last, first, patronymic = r['C'].split()
            date = (datetime.datetime(1899, 12, 30) + datetime.timedelta(days=float(r['B']))).strftime('%Y-%m-%d %H:%M:%S')
            sql.append(f"INSERT INTO orders VALUES ({oid},{q(date)},{q(last)},{q(first)},{q(patronymic)},NULL,1);")
            orders.add(oid)
        sid = stock[(products[model(r['E'], r['F'])], r['G'])]
        sql.append(f"INSERT INTO order_items VALUES ({oid},{sid},{int(r['H'])},{float(r['I']):.2f});")
    sql.append('COMMIT;')
    (ROOT / 'database' / 'seed.sql').write_text('\n'.join(sql) + '\n', encoding='utf-8')
    print(f'Imported: {len(products)} models, {len(stock)} stock items, {len(orders)} orders')


if __name__ == '__main__':
    generate()
