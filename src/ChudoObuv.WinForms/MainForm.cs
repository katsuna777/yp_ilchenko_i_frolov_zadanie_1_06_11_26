using ChudoObuv.WinForms.Data;

namespace ChudoObuv.WinForms;

public partial class MainForm : Form
{
    private readonly Store store;
    private User? user;
    private Catalog catalog = new Catalog(new List<Product>(), new List<StockItem>());
    private readonly List<RequestedLine> basket = new List<RequestedLine>();
    private readonly Dictionary<int, Image?> images = new Dictionary<int, Image?>();
    private bool loading;
    private Product? Selected => grid.CurrentRow?.Tag as Product;

    public MainForm() : this(new Store(new Database(DatabaseConfig.ConnectionString))) { }

    public MainForm(Store store, User? user = null)
    {
        this.store = store;
        this.user = user;
        InitializeComponent();
        logo.Image = Ui.LoadImage("logo.png");
        grid.RowTemplate.Height = 90;
        sort.Items.AddRange(new object[] { "По названию", "Цена по возрастанию", "Цена по убыванию", "По остатку" });
        sort.SelectedIndex = 0;
        UpdateUser();
    }

    private async void MainForm_Shown(object? sender, EventArgs e) { await Reload(); }
    private void LoginButton_Click(object? sender, EventArgs e) { ChangeUser(); }
    private async void BasketButton_Click(object? sender, EventArgs e) { await OpenBasket(); }
    private async void RefreshButton_Click(object? sender, EventArgs e) { await Reload(); }
    private void AddButton_Click(object? sender, EventArgs e) { AddToBasket(); }
    private void Grid_SelectionChanged(object? sender, EventArgs e) { ShowDetail(); }
    private void Filters_Changed(object? sender, EventArgs e) { Filter(); }
    private void OrderSize_Changed(object? sender, EventArgs e)
    {
        if (orderSize.SelectedItem is SizeChoice choice)
            quantity.Maximum = Math.Max(1, choice.Available);
    }
    private async void OrdersButton_Click(object? sender, EventArgs e)
    {
        if (user == null) return;
        using (var form = new OrdersForm(store, user)) form.ShowDialog(this);
        await Reload();
    }
    private void MainForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        foreach (var image in images.Values) image?.Dispose();
        logo.Image?.Dispose();
    }

    private async Task Reload() => await Ui.Run(this, async () =>
    {
        catalog = await Task.Run(store.Catalog);
        loading = true;
        try
        {
            Fill(category, "Все категории", catalog.Products.Select(p => p.Category));
            Fill(subcategory, "Все виды", catalog.Products.Select(p => p.Subcategory));
            Fill(manufacturer, "Все производители", catalog.Products.Select(p => p.Manufacturer));
            Fill(size, "Любой", catalog.Stock.Select(s => s.Size));
        }
        finally { loading = false; }
        Filter();
    });
    private static void Fill(ComboBox combo, string all, IEnumerable<string> values)
    {
        var selected = combo.SelectedItem?.ToString(); combo.Items.Clear(); combo.Items.Add(all);
        combo.Items.AddRange(values.Distinct().Order().Cast<object>().ToArray());
        combo.SelectedIndex = selected is not null && combo.Items.Contains(selected) ? combo.Items.IndexOf(selected) : 0;
    }
    private void Filter()
    {
        if (loading) return;
        var products = catalog.Products.AsEnumerable();
        if (user is not null)
        {
            var term = search.Text.Trim();
            products = products.Where(p => string.IsNullOrEmpty(term) || $"{p.Name} {p.Manufacturer} {p.Description} {p.Composition}".Contains(term, StringComparison.CurrentCultureIgnoreCase));
            if (category.SelectedIndex > 0) products = products.Where(p => p.Category == category.Text);
            if (subcategory.SelectedIndex > 0) products = products.Where(p => p.Subcategory == subcategory.Text);
            if (manufacturer.SelectedIndex > 0) products = products.Where(p => p.Manufacturer == manufacturer.Text);
            if (size.SelectedIndex > 0) products = products.Where(p => catalog.Stock.Any(s => s.ProductId == p.Id && s.Size == size.Text && s.Available > 0));
            if (available.Checked) products = products.Where(p => catalog.Stock.Any(s => s.ProductId == p.Id && s.Available > 0));
            products = sort.SelectedIndex switch { 1 => products.OrderBy(p => p.Price), 2 => products.OrderByDescending(p => p.Price),
                3 => products.OrderByDescending(p => catalog.Stock.Where(s => s.ProductId == p.Id).Sum(s => s.Available)), _ => products.OrderBy(p => p.Name) };
        }
        var selected = Selected?.Id;
        grid.Rows.Clear();
        foreach (var p in products)
        {
            if (!images.ContainsKey(p.Id)) images[p.Id] = Ui.LoadImage(Path.Combine("Images", Path.GetFileName(p.Image)));
            var stock = catalog.Stock.Where(s => s.ProductId == p.Id).ToList();
            int row = grid.Rows.Add(images[p.Id], p.Name, $"{p.Category}\n{p.Subcategory}", p.Manufacturer, p.Price,
                string.Join(", ", stock.Where(s => s.Available > 0).Select(s => s.Size)), stock.Sum(s => s.Available));
            grid.Rows[row].Tag = p;
        }
        grid.Columns["name"]!.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        grid.Columns["sizes"]!.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        if (grid.Rows.Count > 0)
        {
            var row = grid.Rows.Cast<DataGridViewRow>().FirstOrDefault(r => (r.Tag as Product)?.Id == selected) ?? grid.Rows[0];
            grid.CurrentCell = row.Cells[1];
        }
        count.Text = $"Моделей: {grid.Rows.Count} из {catalog.Products.Count}" + (user is null ? "   •   Войдите, чтобы искать товары и оформлять заказы" : "");
        ShowDetail();
    }
    private void ShowDetail()
    {
        orderSize.Items.Clear();
        var p = Selected;
        if (p is null) { detail.Text = "Товары не найдены. Измените фильтры."; add.Enabled = false; return; }
        var stock = catalog.Stock.Where(s => s.ProductId == p.Id).ToList();
        detail.Text = $"{p.Name}\n{p.Description}\nСостав: {p.Composition}\nРазмерный ряд: {string.Join(", ", stock.Select(s => s.Size))}";
        foreach (var s in stock.Where(s => s.Available > 0)) orderSize.Items.Add(new SizeChoice(s.Id, s.Size, s.Available));
        if (orderSize.Items.Count > 0) orderSize.SelectedIndex = 0;
        add.Enabled = user is not null && orderSize.Items.Count > 0;
    }
    private void AddToBasket()
    {
        if (user is null || orderSize.SelectedItem is not SizeChoice choice) return;
        int qty = (int)quantity.Value;
        var line = basket.FirstOrDefault(l => l.StockItemId == choice.Id);
        if ((line?.Quantity ?? 0) + qty > choice.Available) { Ui.Error(new InvalidOperationException("В корзине уже есть этот размер. Суммарное количество превышает остаток.")); return; }
        if (line is not null) basket.Remove(line);
        basket.Add(new(choice.Id, (line?.Quantity ?? 0) + qty)); UpdateUser();
    }
    private void ChangeUser()
    {
        if (basket.Count > 0 && MessageBox.Show("При выходе корзина будет очищена. Выйти?", "Выход", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        DialogResult = DialogResult.Retry;
        Close();
    }
    private void UpdateUser()
    {
        identity.Text = user is null ? "Гость" : $"{user.FullName}\n{user.RoleName}";
        loginButton.Text = user is null ? "Войти" : "Выйти";
        filters.Visible = user is not null; ordersButton.Visible = user?.CanManageOrders == true;
        basketButton.Visible = user is not null; basketButton.Text = $"Корзина ({basket.Sum(l => l.Quantity)})";
    }
    private async Task OpenBasket()
    {
        if (user is null) return;
        using var form = new OrderEditor(store, user, initial: basket);
        if (form.ShowDialog(this) == DialogResult.OK) basket.Clear();
        UpdateUser(); await Reload();
    }
    private class SizeChoice
    {
        public int Id { get; }
        public string Size { get; }
        public int Available { get; }
        public SizeChoice(int id, string size, int available)
        {
            Id = id;
            Size = size;
            Available = available;
        }
        public override string ToString() => Size + " — " + Available + " пар";
    }
}
