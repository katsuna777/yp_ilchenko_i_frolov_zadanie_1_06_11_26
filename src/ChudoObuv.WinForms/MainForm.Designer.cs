#nullable enable

namespace ChudoObuv.WinForms;

partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        logo = new PictureBox();
        title = new Label();
        identity = new Label();
        loginButton = new Button();
        basketButton = new Button();
        ordersButton = new Button();
        refreshButton = new Button();
        filters = new Panel();
        count = new Label();
        grid = new DataGridView();
        detail = new TextBox();
        orderSize = new ComboBox();
        quantity = new NumericUpDown();
        add = new Button();
        search = new TextBox();
        category = new ComboBox();
        subcategory = new ComboBox();
        manufacturer = new ComboBox();
        size = new ComboBox();
        available = new CheckBox();
        sort = new ComboBox();
        nameColumn = new DataGridViewTextBoxColumn();
        categoryColumn = new DataGridViewTextBoxColumn();
        manufacturerColumn = new DataGridViewTextBoxColumn();
        priceColumn = new DataGridViewTextBoxColumn();
        sizesColumn = new DataGridViewTextBoxColumn();
        stockColumn = new DataGridViewTextBoxColumn();
        ((System.ComponentModel.ISupportInitialize)logo).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)quantity).BeginInit();
        SuspendLayout();
        // logo
        logo.Name = "logo";
        logo.Location = new Point(12, 10);
        logo.Size = new Size(50, 50);
        logo.SizeMode = PictureBoxSizeMode.Zoom;
        // title
        title.Name = "title";
        title.Location = new Point(75, 14);
        title.Size = new Size(220, 38);
        title.Text = "Чудо Обувь";
        title.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
        // identity
        identity.Name = "identity";
        identity.Location = new Point(310, 12);
        identity.Size = new Size(330, 45);
        identity.Text = "Гость";
        // loginButton
        loginButton.Name = "loginButton";
        loginButton.Location = new Point(660, 16);
        loginButton.Size = new Size(90, 34);
        loginButton.Text = "Войти";
        loginButton.UseVisualStyleBackColor = true;
        // basketButton
        basketButton.Name = "basketButton";
        basketButton.Location = new Point(760, 16);
        basketButton.Size = new Size(130, 34);
        basketButton.Text = "Корзина";
        basketButton.UseVisualStyleBackColor = true;
        // ordersButton
        ordersButton.Name = "ordersButton";
        ordersButton.Location = new Point(900, 16);
        ordersButton.Size = new Size(95, 34);
        ordersButton.Text = "Заказы";
        ordersButton.UseVisualStyleBackColor = true;
        // refreshButton
        refreshButton.Name = "refreshButton";
        refreshButton.Location = new Point(1005, 16);
        refreshButton.Size = new Size(105, 34);
        refreshButton.Text = "Обновить";
        refreshButton.UseVisualStyleBackColor = true;
        // filters
        filters.Name = "filters";
        filters.Location = new Point(12, 72);
        filters.Size = new Size(1098, 92);
        filters.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        // count
        count.Name = "count";
        count.Location = new Point(12, 172);
        count.Size = new Size(1098, 25);
        // grid
        grid.Name = "grid";
        grid.Location = new Point(12, 205);
        grid.Size = new Size(1098, 350);
        grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AutoGenerateColumns = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.ReadOnly = true;
        grid.RowHeadersVisible = false;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.BackgroundColor = Color.White;
        // detail
        detail.Name = "detail";
        detail.Location = new Point(12, 565);
        detail.Size = new Size(1098, 88);
        detail.Multiline = true;
        detail.ReadOnly = true;
        detail.ScrollBars = ScrollBars.Vertical;
        detail.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        // orderSize
        orderSize.Name = "orderSize";
        orderSize.Location = new Point(12, 670);
        orderSize.Size = new Size(220, 28);
        orderSize.DropDownStyle = ComboBoxStyle.DropDownList;
        orderSize.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        // quantity
        quantity.Name = "quantity";
        quantity.Location = new Point(245, 670);
        quantity.Size = new Size(80, 28);
        quantity.Minimum = 1;
        quantity.Maximum = 100000;
        quantity.Value = 1;
        quantity.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        // add
        add.Name = "add";
        add.Location = new Point(345, 665);
        add.Size = new Size(190, 34);
        add.Text = "Добавить в корзину";
        add.UseVisualStyleBackColor = true;
        add.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        // search
        search.Name = "search";
        search.Location = new Point(0, 5);
        search.Size = new Size(300, 28);
        search.PlaceholderText = "Название, описание, производитель";
        // category
        category.Name = "category";
        category.Location = new Point(310, 5);
        category.Size = new Size(200, 28);
        category.DropDownStyle = ComboBoxStyle.DropDownList;
        // subcategory
        subcategory.Name = "subcategory";
        subcategory.Location = new Point(520, 5);
        subcategory.Size = new Size(180, 28);
        subcategory.DropDownStyle = ComboBoxStyle.DropDownList;
        // manufacturer
        manufacturer.Name = "manufacturer";
        manufacturer.Location = new Point(710, 5);
        manufacturer.Size = new Size(200, 28);
        manufacturer.DropDownStyle = ComboBoxStyle.DropDownList;
        // size
        size.Name = "size";
        size.Location = new Point(920, 5);
        size.Size = new Size(160, 28);
        size.DropDownStyle = ComboBoxStyle.DropDownList;
        // available
        available.Name = "available";
        available.Location = new Point(0, 48);
        available.Size = new Size(150, 28);
        available.Text = "Только в наличии";
        // sort
        sort.Name = "sort";
        sort.Location = new Point(170, 48);
        sort.Size = new Size(230, 28);
        sort.DropDownStyle = ComboBoxStyle.DropDownList;
        nameColumn.Name = "name";
        nameColumn.HeaderText = "Модель";
        nameColumn.FillWeight = 180;
        nameColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        categoryColumn.Name = "category";
        categoryColumn.HeaderText = "Категория";
        categoryColumn.FillWeight = 85;
        categoryColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        manufacturerColumn.Name = "manufacturer";
        manufacturerColumn.HeaderText = "Производство";
        manufacturerColumn.FillWeight = 80;
        manufacturerColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        priceColumn.Name = "price";
        priceColumn.HeaderText = "Цена, ₽";
        priceColumn.FillWeight = 60;
        priceColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        priceColumn.DefaultCellStyle.Format = "N2";
        sizesColumn.Name = "sizes";
        sizesColumn.HeaderText = "Размеры";
        sizesColumn.FillWeight = 80;
        sizesColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        stockColumn.Name = "stock";
        stockColumn.HeaderText = "Пар";
        stockColumn.FillWeight = 35;
        stockColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        grid.Columns.Add(new DataGridViewImageColumn { Name = "image", HeaderText = "Фото", ImageLayout = DataGridViewImageCellLayout.Zoom, FillWeight = 60, DefaultCellStyle = new DataGridViewCellStyle { NullValue = null } });
        grid.Columns.AddRange(new DataGridViewColumn[] { nameColumn, categoryColumn, manufacturerColumn, priceColumn, sizesColumn, stockColumn });
        Shown += MainForm_Shown;
        FormClosed += MainForm_FormClosed;
        loginButton.Click += LoginButton_Click;
        basketButton.Click += BasketButton_Click;
        ordersButton.Click += OrdersButton_Click;
        refreshButton.Click += RefreshButton_Click;
        add.Click += AddButton_Click;
        grid.SelectionChanged += Grid_SelectionChanged;
        search.TextChanged += Filters_Changed;
        available.CheckedChanged += Filters_Changed;
        orderSize.SelectedIndexChanged += OrderSize_Changed;
        category.SelectedIndexChanged += Filters_Changed;
        subcategory.SelectedIndexChanged += Filters_Changed;
        manufacturer.SelectedIndexChanged += Filters_Changed;
        size.SelectedIndexChanged += Filters_Changed;
        sort.SelectedIndexChanged += Filters_Changed;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1125, 720);
        MinimumSize = new Size(1125, 720);
        Font = new Font("Segoe UI", 9F);
        Text = "Каталог | Чудо Обувь";
        StartPosition = FormStartPosition.CenterScreen;
        Controls.Add(logo);
        Controls.Add(title);
        Controls.Add(identity);
        Controls.Add(loginButton);
        Controls.Add(basketButton);
        Controls.Add(ordersButton);
        Controls.Add(refreshButton);
        Controls.Add(filters);
        Controls.Add(count);
        Controls.Add(grid);
        Controls.Add(detail);
        Controls.Add(orderSize);
        Controls.Add(quantity);
        Controls.Add(add);
        filters.Controls.Add(search);
        filters.Controls.Add(category);
        filters.Controls.Add(subcategory);
        filters.Controls.Add(manufacturer);
        filters.Controls.Add(size);
        filters.Controls.Add(available);
        filters.Controls.Add(sort);
        ((System.ComponentModel.ISupportInitialize)logo).EndInit();
        ((System.ComponentModel.ISupportInitialize)grid).EndInit();
        ((System.ComponentModel.ISupportInitialize)quantity).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private PictureBox logo = null!;
    private Label title = null!;
    private Label identity = null!;
    private Button loginButton = null!;
    private Button basketButton = null!;
    private Button ordersButton = null!;
    private Button refreshButton = null!;
    private Panel filters = null!;
    private Label count = null!;
    private DataGridView grid = null!;
    private TextBox detail = null!;
    private ComboBox orderSize = null!;
    private NumericUpDown quantity = null!;
    private Button add = null!;
    private TextBox search = null!;
    private ComboBox category = null!;
    private ComboBox subcategory = null!;
    private ComboBox manufacturer = null!;
    private ComboBox size = null!;
    private CheckBox available = null!;
    private ComboBox sort = null!;
    private DataGridViewTextBoxColumn nameColumn = null!;
    private DataGridViewTextBoxColumn categoryColumn = null!;
    private DataGridViewTextBoxColumn manufacturerColumn = null!;
    private DataGridViewTextBoxColumn priceColumn = null!;
    private DataGridViewTextBoxColumn sizesColumn = null!;
    private DataGridViewTextBoxColumn stockColumn = null!;
}
