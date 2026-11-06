#nullable enable

namespace ChudoObuv.WinForms;

partial class OrdersForm
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
        title = new Label();
        search = new TextBox();
        refreshButton = new Button();
        grid = new DataGridView();
        viewButton = new Button();
        newButton = new Button();
        editButton = new Button();
        deleteButton = new Button();
        idColumn = new DataGridViewTextBoxColumn();
        dateColumn = new DataGridViewTextBoxColumn();
        clientColumn = new DataGridViewTextBoxColumn();
        qtyColumn = new DataGridViewTextBoxColumn();
        totalColumn = new DataGridViewTextBoxColumn();
        ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
        SuspendLayout();
        // title
        title.Name = "title";
        title.Location = new Point(12, 12);
        title.Size = new Size(250, 40);
        title.Text = "Заказы";
        title.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
        // search
        search.Name = "search";
        search.Location = new Point(12, 65);
        search.Size = new Size(400, 28);
        search.PlaceholderText = "ФИО или номер заказа";
        // refreshButton
        refreshButton.Name = "refreshButton";
        refreshButton.Location = new Point(430, 60);
        refreshButton.Size = new Size(130, 34);
        refreshButton.Text = "Обновить";
        refreshButton.UseVisualStyleBackColor = true;
        // grid
        grid.Name = "grid";
        grid.Location = new Point(12, 110);
        grid.Size = new Size(1050, 460);
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
        // viewButton
        viewButton.Name = "viewButton";
        viewButton.Location = new Point(12, 585);
        viewButton.Size = new Size(130, 34);
        viewButton.Text = "Состав заказа";
        viewButton.UseVisualStyleBackColor = true;
        viewButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        // newButton
        newButton.Name = "newButton";
        newButton.Location = new Point(155, 585);
        newButton.Size = new Size(130, 34);
        newButton.Text = "Новый заказ";
        newButton.UseVisualStyleBackColor = true;
        newButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        // editButton
        editButton.Name = "editButton";
        editButton.Location = new Point(300, 585);
        editButton.Size = new Size(130, 34);
        editButton.Text = "Редактировать";
        editButton.UseVisualStyleBackColor = true;
        editButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        // deleteButton
        deleteButton.Name = "deleteButton";
        deleteButton.Location = new Point(445, 585);
        deleteButton.Size = new Size(130, 34);
        deleteButton.Text = "Удалить заказ";
        deleteButton.UseVisualStyleBackColor = true;
        deleteButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        idColumn.Name = "id";
        idColumn.HeaderText = "№";
        idColumn.FillWeight = 30;
        idColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        dateColumn.Name = "date";
        dateColumn.HeaderText = "Дата заказа";
        dateColumn.FillWeight = 85;
        dateColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        dateColumn.DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
        clientColumn.Name = "client";
        clientColumn.HeaderText = "ФИО клиента";
        clientColumn.FillWeight = 180;
        clientColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        qtyColumn.Name = "qty";
        qtyColumn.HeaderText = "Пар";
        qtyColumn.FillWeight = 40;
        qtyColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        totalColumn.Name = "total";
        totalColumn.HeaderText = "Итого, ₽";
        totalColumn.FillWeight = 75;
        totalColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        totalColumn.DefaultCellStyle.Format = "N2";
        grid.Columns.AddRange(new DataGridViewColumn[] { idColumn, dateColumn, clientColumn, qtyColumn, totalColumn });
        Shown += OrdersForm_Shown;
        search.TextChanged += Search_TextChanged;
        grid.CellDoubleClick += Grid_CellDoubleClick;
        refreshButton.Click += RefreshButton_Click;
        viewButton.Click += ViewButton_Click;
        newButton.Click += NewButton_Click;
        editButton.Click += EditButton_Click;
        deleteButton.Click += DeleteButton_Click;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1080, 640);
        MinimumSize = new Size(1080, 640);
        Font = new Font("Segoe UI", 9F);
        Text = "Итого, ₽ | Чудо Обувь";
        StartPosition = FormStartPosition.CenterScreen;
        Controls.Add(title);
        Controls.Add(search);
        Controls.Add(refreshButton);
        Controls.Add(grid);
        Controls.Add(viewButton);
        Controls.Add(newButton);
        Controls.Add(editButton);
        Controls.Add(deleteButton);
        ((System.ComponentModel.ISupportInitialize)grid).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label title = null!;
    private TextBox search = null!;
    private Button refreshButton = null!;
    private DataGridView grid = null!;
    private Button viewButton = null!;
    private Button newButton = null!;
    private Button editButton = null!;
    private Button deleteButton = null!;
    private DataGridViewTextBoxColumn idColumn = null!;
    private DataGridViewTextBoxColumn dateColumn = null!;
    private DataGridViewTextBoxColumn clientColumn = null!;
    private DataGridViewTextBoxColumn qtyColumn = null!;
    private DataGridViewTextBoxColumn totalColumn = null!;
}
