using ChudoObuv.Core;

namespace ChudoObuv.WinForms;

internal sealed class OrderEditor : Form
{
    private readonly Store store;
    private readonly User user;
    private readonly Order? order;
    private readonly bool readOnly;
    private Catalog catalog = new([], []);
    private readonly List<OrderLine> lines = [];
    private readonly TextBox last = new() { Width = 180, MaxLength = 100 }, first = new() { Width = 170, MaxLength = 100 }, patronymic = new() { Width = 180, MaxLength = 100 };
    private readonly DateTimePicker date = new() { Format = DateTimePickerFormat.Short, Width = 125 };
    private readonly ComboBox product = Ui.Combo(480), size = Ui.Combo(170);
    private readonly NumericUpDown quantity = new() { Minimum = 1, Maximum = 100000, Width = 85, Margin = new(4, 8, 4, 4) };
    private readonly DataGridView grid = Ui.Grid();
    private readonly Label total = Ui.Label("");
    private readonly IReadOnlyList<RequestedLine> initial;

    public OrderEditor(Store store, User user, Order? order = null, bool readOnly = false, IReadOnlyList<RequestedLine>? initial = null)
    {
        this.store = store; this.user = user; this.order = order; this.readOnly = readOnly;
        this.initial = initial ?? [];
        Ui.Setup(this, order is null ? "Оформление заказа" : $"Заказ №{order.Id}", 1120, 730);
        var layout = Ui.Layout(SizeType.AutoSize, SizeType.AutoSize, SizeType.AutoSize, SizeType.Percent, SizeType.AutoSize);
        layout.Controls.Add(new Label { Text = order is null ? "Новый заказ" : $"Заказ №{order.Id}", Font = new(Font.FontFamily, 22, FontStyle.Bold), AutoSize = true }, 0, 0);
        last.Text = order?.LastName ?? user.LastName; first.Text = order?.FirstName ?? user.FirstName; patronymic.Text = order?.Patronymic ?? user.Patronymic;
        date.Value = order?.Date ?? DateTime.Now;
        last.ReadOnly = first.ReadOnly = patronymic.ReadOnly = readOnly || !user.CanManageOrders;
        date.Enabled = !readOnly && user.CanManageOrders;
        layout.Controls.Add(Ui.Bar(Ui.Label("Фамилия"), last, Ui.Label("Имя"), first, Ui.Label("Отчество"), patronymic, Ui.Label("Дата"), date), 0, 1);
        var picker = Ui.Bar(product, size, Ui.Label("Пар"), quantity, Ui.Button("Добавить", (_, _) => Add(), true)); picker.Visible = !readOnly;
        layout.Controls.Add(picker, 0, 2);
        Ui.Column(grid, "name", "Модель", 220); Ui.Column(grid, "size", "Размер", 45); Ui.Column(grid, "quantity", "Количество", 65);
        Ui.Column(grid, "price", "Цена за пару, ₽", 80, "N2"); Ui.Column(grid, "total", "Сумма, ₽", 80, "N2");
        grid.ReadOnly = readOnly;
        foreach (DataGridViewColumn c in grid.Columns) c.ReadOnly = c.Name != "quantity" || readOnly;
        grid.EditMode = DataGridViewEditMode.EditOnEnter;
        grid.CellValidating += (_, e) =>
        {
            if (readOnly || e.ColumnIndex != 2) return;
            if (!int.TryParse(e.FormattedValue?.ToString(), out int qty) || qty < 1 || qty > 100000)
            { e.Cancel = true; grid.Rows[e.RowIndex].ErrorText = "Введите целое количество от 1 до 100000."; }
            else grid.Rows[e.RowIndex].ErrorText = "";
        };
        grid.CellEndEdit += (_, e) =>
        {
            if (e.RowIndex < 0 || e.RowIndex >= lines.Count || e.ColumnIndex != 2) return;
            if (int.TryParse(grid.Rows[e.RowIndex].Cells[2].Value?.ToString(), out int qty) && qty > 0)
            {
                lines[e.RowIndex] = lines[e.RowIndex] with { Quantity = qty };
                grid.Rows[e.RowIndex].Cells[4].Value = lines[e.RowIndex].Total; UpdateTotal();
            }
        };
        grid.DataError += (_, e) => { e.ThrowException = false; };
        layout.Controls.Add(grid, 0, 3);
        var remove = Ui.Button("Удалить позицию", (_, _) =>
        {
            if (grid.CurrentRow is null) return;
            grid.CancelEdit(); lines.RemoveAt(grid.CurrentRow.Index); DrawLines();
        }); remove.Visible = !readOnly; remove.CausesValidation = false;
        var save = Ui.Button(order is null ? "Оформить заказ" : "Сохранить изменения", async (_, _) => await Save(), true); save.Visible = !readOnly;
        var close = Ui.Button(readOnly ? "Закрыть" : "Отмена", (_, _) => { DialogResult = DialogResult.Cancel; Close(); }); close.CausesValidation = false;
        layout.Controls.Add(Ui.Bar(total, remove, save, close), 0, 4);
        Controls.Add(layout);
        product.SelectedIndexChanged += (_, _) => FillSizes();
        Shown += async (_, _) => await Ui.Run(this, async () =>
        {
            catalog = await Task.Run(store.Catalog);
            if (order is not null) lines.AddRange(order.Lines);
            else foreach (var line in this.initial)
            {
                var stock = catalog.Stock.Single(s => s.Id == line.StockItemId);
                var p = catalog.Products.Single(p => p.Id == stock.ProductId);
                lines.Add(new(stock.Id, p.Name, stock.Size, line.Quantity, p.Price));
            }
            foreach (var p in catalog.Products) product.Items.Add(new ProductChoice(p));
            if (product.Items.Count > 0) product.SelectedIndex = 0;
            DrawLines();
        });
    }
    private void FillSizes()
    {
        size.Items.Clear();
        if (product.SelectedItem is not ProductChoice selected) return;
        foreach (var s in catalog.Stock.Where(s => s.ProductId == selected.Product.Id))
        {
            int previous = order?.Lines.FirstOrDefault(l => l.StockItemId == s.Id)?.Quantity ?? 0;
            if (s.Available + previous > 0) size.Items.Add(new StockChoice(s, s.Available + previous));
        }
        if (size.Items.Count > 0) size.SelectedIndex = 0;
    }
    private void Add()
    {
        if (!grid.EndEdit()) return;
        if (product.SelectedItem is not ProductChoice p || size.SelectedItem is not StockChoice s)
        { Ui.Error(new InvalidOperationException("Выберите модель и доступный размер.")); return; }
        var existing = lines.FirstOrDefault(l => l.StockItemId == s.Stock.Id);
        int qty = (existing?.Quantity ?? 0) + (int)quantity.Value;
        if (qty > s.Available) { Ui.Error(new InvalidOperationException($"Доступно для этого заказа: {s.Available} пар.")); return; }
        if (existing is null) lines.Add(new(s.Stock.Id, p.Product.Name, s.Stock.Size, qty, p.Product.Price));
        else lines[lines.IndexOf(existing)] = existing with { Quantity = qty };
        DrawLines();
    }
    private void DrawLines()
    {
        grid.Rows.Clear();
        foreach (var line in lines) grid.Rows.Add(line.ProductName, line.Size, line.Quantity, line.UnitPrice, line.Total);
        UpdateTotal();
    }
    private void UpdateTotal() => total.Text = $"Итого: {lines.Sum(l => l.Total):N2} ₽   •   Пар: {lines.Sum(l => l.Quantity)}";
    private async Task Save()
    {
        if (!ValidateChildren() || !grid.EndEdit()) return;
        await Ui.Run(this, async () =>
        {
            var draft = new OrderDraft(date.Value, last.Text, first.Text, patronymic.Text,
                lines.Select(l => new RequestedLine(l.StockItemId, l.Quantity)).ToList(), order?.Id, order?.Version ?? 0);
            int id = await Task.Run(() => store.SaveOrder(user, draft));
            MessageBox.Show($"Заказ №{id} сохранён. Сумма: {lines.Sum(l => l.Total):N2} ₽.", "Заказ оформлен", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
        });
    }
    private sealed record ProductChoice(Product Product) { public override string ToString() => $"{Product.Name} / {Product.Manufacturer}"; }
    private sealed record StockChoice(StockItem Stock, int Available) { public override string ToString() => $"{Stock.Size} — доступно {Available}"; }
}
