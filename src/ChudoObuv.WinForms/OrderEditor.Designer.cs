#nullable enable

namespace ChudoObuv.WinForms;

partial class OrderEditor
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
        lastLabel = new Label();
        last = new TextBox();
        firstLabel = new Label();
        first = new TextBox();
        patronymicLabel = new Label();
        patronymic = new TextBox();
        dateLabel = new Label();
        date = new DateTimePicker();
        picker = new Panel();
        product = new ComboBox();
        size = new ComboBox();
        quantity = new NumericUpDown();
        addButton = new Button();
        grid = new DataGridView();
        total = new Label();
        remove = new Button();
        save = new Button();
        closeButton = new Button();
        nameColumn = new DataGridViewTextBoxColumn();
        sizeColumn = new DataGridViewTextBoxColumn();
        quantityColumn = new DataGridViewTextBoxColumn();
        priceColumn = new DataGridViewTextBoxColumn();
        totalColumn = new DataGridViewTextBoxColumn();
        ((System.ComponentModel.ISupportInitialize)quantity).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
        SuspendLayout();
        // title
        title.Name = "title";
        title.Location = new Point(12, 10);
        title.Size = new Size(650, 40);
        title.Text = "Новый заказ";
        title.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        // lastLabel
        lastLabel.Name = "lastLabel";
        lastLabel.Location = new Point(12, 58);
        lastLabel.Size = new Size(230, 22);
        lastLabel.Text = "Фамилия";
        // last
        last.Name = "last";
        last.Location = new Point(12, 82);
        last.Size = new Size(230, 28);
        last.MaxLength = 100;
        // firstLabel
        firstLabel.Name = "firstLabel";
        firstLabel.Location = new Point(260, 58);
        firstLabel.Size = new Size(230, 22);
        firstLabel.Text = "Имя";
        // first
        first.Name = "first";
        first.Location = new Point(260, 82);
        first.Size = new Size(230, 28);
        first.MaxLength = 100;
        // patronymicLabel
        patronymicLabel.Name = "patronymicLabel";
        patronymicLabel.Location = new Point(510, 58);
        patronymicLabel.Size = new Size(230, 22);
        patronymicLabel.Text = "Отчество";
        // patronymic
        patronymic.Name = "patronymic";
        patronymic.Location = new Point(510, 82);
        patronymic.Size = new Size(230, 28);
        patronymic.MaxLength = 100;
        // dateLabel
        dateLabel.Name = "dateLabel";
        dateLabel.Location = new Point(760, 58);
        dateLabel.Size = new Size(200, 22);
        dateLabel.Text = "Дата заказа";
        // date
        date.Name = "date";
        date.Location = new Point(760, 82);
        date.Size = new Size(200, 28);
        date.Format = DateTimePickerFormat.Short;
        // picker
        picker.Name = "picker";
        picker.Location = new Point(12, 125);
        picker.Size = new Size(1048, 50);
        // product
        product.Name = "product";
        product.Location = new Point(0, 8);
        product.Size = new Size(540, 28);
        product.DropDownStyle = ComboBoxStyle.DropDownList;
        // size
        size.Name = "size";
        size.Location = new Point(550, 8);
        size.Size = new Size(190, 28);
        size.DropDownStyle = ComboBoxStyle.DropDownList;
        // quantity
        quantity.Name = "quantity";
        quantity.Location = new Point(750, 8);
        quantity.Size = new Size(80, 28);
        quantity.Minimum = 1;
        quantity.Maximum = 100000;
        quantity.Value = 1;
        // addButton
        addButton.Name = "addButton";
        addButton.Location = new Point(845, 3);
        addButton.Size = new Size(180, 34);
        addButton.Text = "Добавить";
        addButton.UseVisualStyleBackColor = true;
        // grid
        grid.Name = "grid";
        grid.Location = new Point(12, 185);
        grid.Size = new Size(1048, 350);
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
        // total
        total.Name = "total";
        total.Location = new Point(12, 548);
        total.Size = new Size(1000, 25);
        total.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        // remove
        remove.Name = "remove";
        remove.Location = new Point(12, 590);
        remove.Size = new Size(160, 34);
        remove.Text = "Удалить позицию";
        remove.UseVisualStyleBackColor = true;
        remove.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        remove.CausesValidation = false;
        // save
        save.Name = "save";
        save.Location = new Point(190, 590);
        save.Size = new Size(200, 34);
        save.Text = "Оформить заказ";
        save.UseVisualStyleBackColor = true;
        save.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        // closeButton
        closeButton.Name = "closeButton";
        closeButton.Location = new Point(410, 590);
        closeButton.Size = new Size(130, 34);
        closeButton.Text = "Отмена";
        closeButton.UseVisualStyleBackColor = true;
        closeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        closeButton.CausesValidation = false;
        nameColumn.Name = "name";
        nameColumn.HeaderText = "Модель";
        nameColumn.FillWeight = 220;
        nameColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        sizeColumn.Name = "size";
        sizeColumn.HeaderText = "Размер";
        sizeColumn.FillWeight = 45;
        sizeColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        quantityColumn.Name = "quantity";
        quantityColumn.HeaderText = "Количество";
        quantityColumn.FillWeight = 65;
        quantityColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        priceColumn.Name = "price";
        priceColumn.HeaderText = "Цена за пару, ₽";
        priceColumn.FillWeight = 80;
        priceColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        priceColumn.DefaultCellStyle.Format = "N2";
        totalColumn.Name = "total";
        totalColumn.HeaderText = "Сумма, ₽";
        totalColumn.FillWeight = 80;
        totalColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        totalColumn.DefaultCellStyle.Format = "N2";
        grid.Columns.AddRange(new DataGridViewColumn[] { nameColumn, sizeColumn, quantityColumn, priceColumn, totalColumn });
        Shown += OrderEditor_Shown;
        product.SelectedIndexChanged += Product_SelectedIndexChanged;
        grid.CellValidating += Grid_CellValidating;
        grid.CellEndEdit += Grid_CellEndEdit;
        grid.DataError += Grid_DataError;
        addButton.Click += AddButton_Click;
        remove.Click += RemoveButton_Click;
        save.Click += SaveButton_Click;
        closeButton.Click += CloseButton_Click;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1080, 645);
        MinimumSize = new Size(1080, 645);
        Font = new Font("Segoe UI", 9F);
        Text = "Сумма, ₽ | Чудо Обувь";
        StartPosition = FormStartPosition.CenterScreen;
        Controls.Add(title);
        Controls.Add(lastLabel);
        Controls.Add(last);
        Controls.Add(firstLabel);
        Controls.Add(first);
        Controls.Add(patronymicLabel);
        Controls.Add(patronymic);
        Controls.Add(dateLabel);
        Controls.Add(date);
        Controls.Add(picker);
        picker.Controls.Add(product);
        picker.Controls.Add(size);
        picker.Controls.Add(quantity);
        picker.Controls.Add(addButton);
        Controls.Add(grid);
        Controls.Add(total);
        Controls.Add(remove);
        Controls.Add(save);
        Controls.Add(closeButton);
        ((System.ComponentModel.ISupportInitialize)quantity).EndInit();
        ((System.ComponentModel.ISupportInitialize)grid).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label title = null!;
    private Label lastLabel = null!;
    private TextBox last = null!;
    private Label firstLabel = null!;
    private TextBox first = null!;
    private Label patronymicLabel = null!;
    private TextBox patronymic = null!;
    private Label dateLabel = null!;
    private DateTimePicker date = null!;
    private Panel picker = null!;
    private ComboBox product = null!;
    private ComboBox size = null!;
    private NumericUpDown quantity = null!;
    private Button addButton = null!;
    private DataGridView grid = null!;
    private Label total = null!;
    private Button remove = null!;
    private Button save = null!;
    private Button closeButton = null!;
    private DataGridViewTextBoxColumn nameColumn = null!;
    private DataGridViewTextBoxColumn sizeColumn = null!;
    private DataGridViewTextBoxColumn quantityColumn = null!;
    private DataGridViewTextBoxColumn priceColumn = null!;
    private DataGridViewTextBoxColumn totalColumn = null!;
}
