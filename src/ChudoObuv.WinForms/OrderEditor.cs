using ChudoObuv.WinForms.Data;

namespace ChudoObuv.WinForms;

public partial class OrderEditor : Form
{
    private readonly Store store;
    private readonly User user;
    private readonly Order? order;
    private readonly bool readOnly;
    private Catalog catalog = new Catalog(new List<Product>(), new List<StockItem>());
    private readonly List<OrderLine> lines = new List<OrderLine>();
    private readonly IReadOnlyList<RequestedLine> initial;

    public OrderEditor() : this(new Store(new Database(DatabaseConfig.ConnectionString)), new User(0, "", "", "", "", Role.Customer)) { }
    public OrderEditor(Store store, User user, Order? order = null, bool readOnly = false, IReadOnlyList<RequestedLine>? initial = null)
    {
        this.store = store;
        this.user = user;
        this.order = order;
        this.readOnly = readOnly;
        this.initial = initial ?? new List<RequestedLine>();
        InitializeComponent();
        title.Text = order == null ? "Новый заказ" : "Заказ №" + order.Id;
        last.Text = order?.LastName ?? user.LastName;
        first.Text = order?.FirstName ?? user.FirstName;
        patronymic.Text = order?.Patronymic ?? user.Patronymic;
        date.Value = order?.Date ?? DateTime.Now;
        last.ReadOnly = first.ReadOnly = patronymic.ReadOnly = readOnly || !user.CanManageOrders;
        date.Enabled = !readOnly && user.CanManageOrders;
        picker.Visible = remove.Visible = save.Visible = !readOnly;
        save.Text = order == null ? "Оформить заказ" : "Сохранить изменения";
        grid.ReadOnly = readOnly;
        foreach (DataGridViewColumn column in grid.Columns)
            column.ReadOnly = column.Name != "quantity" || readOnly;
        grid.EditMode = DataGridViewEditMode.EditOnEnter;
    }

    private async void OrderEditor_Shown(object? sender, EventArgs e)
    {
        await Ui.Run(this, async () =>
        {
            catalog = await Task.Run(store.Catalog);
            if (order != null) lines.AddRange(order.Lines);
            else foreach (var line in initial)
            {
                var stock = catalog.Stock.Single(s => s.Id == line.StockItemId);
                var model = catalog.Products.Single(p => p.Id == stock.ProductId);
                lines.Add(new OrderLine(stock.Id, model.Name, stock.Size, line.Quantity, model.Price));
            }
            foreach (var model in catalog.Products) product.Items.Add(new ProductChoice(model));
            if (product.Items.Count > 0) product.SelectedIndex = 0;
            DrawLines();
        });
    }
    private void Product_SelectedIndexChanged(object? sender, EventArgs e) { FillSizes(); }
    private void AddButton_Click(object? sender, EventArgs e) { Add(); }
    private async void SaveButton_Click(object? sender, EventArgs e) { await Save(); }
    private void CloseButton_Click(object? sender, EventArgs e) { DialogResult = DialogResult.Cancel; Close(); }
    private void RemoveButton_Click(object? sender, EventArgs e)
    {
        if (grid.CurrentRow == null) return;
        grid.CancelEdit();
        lines.RemoveAt(grid.CurrentRow.Index);
        DrawLines();
    }
    private void Grid_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
    {
        if (readOnly || e.ColumnIndex != 2) return;
        int qty;
        if (!int.TryParse(e.FormattedValue?.ToString(), out qty) || qty < 1 || qty > 100000)
        {
            e.Cancel = true;
            grid.Rows[e.RowIndex].ErrorText = "Введите целое количество от 1 до 100000.";
        }
        else grid.Rows[e.RowIndex].ErrorText = "";
    }
    private void Grid_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= lines.Count || e.ColumnIndex != 2) return;
        int qty;
        if (int.TryParse(grid.Rows[e.RowIndex].Cells[2].Value?.ToString(), out qty) && qty > 0)
        {
            lines[e.RowIndex].Quantity = qty;
            grid.Rows[e.RowIndex].Cells[4].Value = lines[e.RowIndex].Total;
            UpdateTotal();
        }
    }
    private void Grid_DataError(object? sender, DataGridViewDataErrorEventArgs e) { e.ThrowException = false; }

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
        else existing.Quantity = qty;
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
    private class ProductChoice
    {
        public Product Product { get; }
        public ProductChoice(Product product) { Product = product; }
        public override string ToString() => Product.Name + " / " + Product.Manufacturer;
    }
    private class StockChoice
    {
        public StockItem Stock { get; }
        public int Available { get; }
        public StockChoice(StockItem stock, int available) { Stock = stock; Available = available; }
        public override string ToString() => Stock.Size + " — доступно " + Available;
    }
}
