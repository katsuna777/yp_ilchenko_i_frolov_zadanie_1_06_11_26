using ChudoObuv.Core;

namespace ChudoObuv.WinForms;

internal sealed class MainForm : Form
{
    private readonly Store store;
    private User? user;
    private Catalog catalog = new([], []);
    private readonly List<RequestedLine> basket = [];
    private readonly DataGridView grid = Ui.Grid();
    private readonly TextBox search = new() { Width = 250, PlaceholderText = "Название, описание, производитель" };
    private readonly ComboBox category = Ui.Combo(), subcategory = Ui.Combo(), manufacturer = Ui.Combo(), size = Ui.Combo(100), sort = Ui.Combo(210);
    private readonly CheckBox available = new() { Text = "В наличии", AutoSize = true, Margin = new(6, 10, 6, 4) };
    private readonly Label identity = Ui.Label("Гость"), count = Ui.Label("");
    private readonly TextBox detail = new() { Dock = DockStyle.Fill, ReadOnly = true, Multiline = true, ScrollBars = ScrollBars.Vertical, BorderStyle = BorderStyle.None, BackColor = Ui.Paper };
    private readonly FlowLayoutPanel filters;
    private readonly Button loginButton, basketButton, ordersButton, add;
    private readonly ComboBox orderSize = Ui.Combo(190);
    private readonly NumericUpDown quantity = new() { Minimum = 1, Maximum = 100000, Width = 75, Margin = new(5, 8, 5, 5) };
    private readonly Dictionary<int, Image?> images = [];
    private bool loading;
    private Product? Selected => grid.CurrentRow?.Tag as Product;

    public MainForm(Store store)
    {
        this.store = store; Ui.Setup(this, "Каталог");
        var layout = Ui.Layout(SizeType.AutoSize, SizeType.AutoSize, SizeType.AutoSize, SizeType.Percent, SizeType.AutoSize);
        var logo = new PictureBox { Size = new(56, 56), SizeMode = PictureBoxSizeMode.Zoom, Image = Ui.LoadImage("logo.png") };
        loginButton = Ui.Button("Войти", (_, _) => ChangeUser());
        basketButton = Ui.Button("Корзина (0)", async (_, _) => await OpenBasket(), true);
        ordersButton = Ui.Button("Заказы", async (_, _) =>
        {
            if (user is null) return;
            using var form = new OrdersForm(store, user); form.ShowDialog(this); await Reload();
        });
        layout.Controls.Add(Ui.Bar(logo, new Label { Text = "Чудо Обувь", AutoSize = true, Font = new(Font.FontFamily, 25, FontStyle.Bold), Margin = new(12, 7, 25, 4) }, identity, loginButton, basketButton, ordersButton,
            Ui.Button("Обновить", async (_, _) => await Reload())), 0, 0);
        sort.Items.AddRange(["По названию", "Цена по возрастанию", "Цена по убыванию", "По остатку"]); sort.SelectedIndex = 0;
        filters = Ui.Bar(Ui.Label("Поиск"), search, Ui.Label("Категория"), category, Ui.Label("Подкатегория"), subcategory,
            Ui.Label("Производитель"), manufacturer, Ui.Label("Размер"), size, available, sort);
        layout.Controls.Add(filters, 0, 1); layout.Controls.Add(count, 0, 2);
        grid.Columns.Add(new DataGridViewImageColumn { Name = "image", HeaderText = "Фото", ImageLayout = DataGridViewImageCellLayout.Zoom, FillWeight = 45, DefaultCellStyle = new() { NullValue = null } });
        Ui.Column(grid, "name", "Модель", 160); Ui.Column(grid, "category", "Категория / вид", 80);
        Ui.Column(grid, "manufacturer", "Производство", 75); Ui.Column(grid, "price", "Цена, ₽", 48, "N2");
        Ui.Column(grid, "sizes", "Доступные размеры", 100); Ui.Column(grid, "stock", "Пар", 30);
        grid.RowTemplate.Height = 100; grid.SelectionChanged += (_, _) => ShowDetail();
        layout.Controls.Add(grid, 0, 3);
        var bottom = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, Height = 170, AutoSize = false };
        bottom.RowStyles.Add(new(SizeType.Percent, 100)); bottom.RowStyles.Add(new(SizeType.AutoSize));
        bottom.Controls.Add(detail, 0, 0);
        add = Ui.Button("Добавить в корзину", (_, _) => AddToBasket(), true);
        bottom.Controls.Add(Ui.Bar(Ui.Label("Размер"), orderSize, Ui.Label("Количество"), quantity, add), 0, 1);
        layout.Controls.Add(bottom, 0, 4); Controls.Add(layout);
        search.TextChanged += (_, _) => Filter();
        foreach (var combo in new[] { category, subcategory, manufacturer, size, sort }) combo.SelectedIndexChanged += (_, _) => Filter();
        available.CheckedChanged += (_, _) => Filter();
        orderSize.SelectedIndexChanged += (_, _) => { if (orderSize.SelectedItem is SizeChoice s) quantity.Maximum = Math.Max(1, s.Available); };
        UpdateUser(); Shown += async (_, _) => await Reload();
        FormClosed += (_, _) => { foreach (var image in images.Values) image?.Dispose(); logo.Image?.Dispose(); };
    }

    private async Task Reload() => await Ui.Run(this, async () =>
    {
        catalog = await Task.Run(store.Catalog); loading = true;
        Fill(category, "Все категории", catalog.Products.Select(p => p.Category));
        Fill(subcategory, "Все виды", catalog.Products.Select(p => p.Subcategory));
        Fill(manufacturer, "Все производители", catalog.Products.Select(p => p.Manufacturer));
        Fill(size, "Любой", catalog.Stock.Select(s => s.Size));
        loading = false; Filter();
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
        if (user is not null)
        {
            if (basket.Count > 0 && MessageBox.Show("При выходе корзина будет очищена. Выйти?", "Выход", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            user = null; basket.Clear();
        }
        else { using var form = new LoginForm(store); if (form.ShowDialog(this) == DialogResult.OK) user = form.User; }
        UpdateUser(); Filter();
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
    private sealed record SizeChoice(int Id, string Size, int Available)
    { public override string ToString() => $"{Size} — {Available} пар"; }
}
