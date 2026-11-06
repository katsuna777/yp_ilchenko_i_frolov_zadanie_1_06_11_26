using ChudoObuv.Core;
using MySqlConnector;

namespace ChudoObuv.WinForms;

internal static class Ui
{
    public static readonly Color Ink = Color.FromArgb(30, 58, 53);
    public static readonly Color Mint = Color.FromArgb(83, 236, 199);
    public static readonly Color Paper = Color.FromArgb(245, 249, 248);
    public static void Setup(Form form, string title, int width = 1180, int height = 780)
    {
        form.Text = title + " | Чудо Обувь";
        form.Size = new(width, height); form.MinimumSize = new(Math.Min(width, 1000), Math.Min(height, 700));
        form.StartPosition = FormStartPosition.CenterScreen;
        form.Font = new("Segoe UI", 10); form.BackColor = Paper; form.ForeColor = Ink;
        form.AutoScaleMode = AutoScaleMode.Dpi;
        var icon = Path.Combine(AppContext.BaseDirectory, "app.ico");
        if (File.Exists(icon)) form.Icon = new Icon(icon);
    }
    public static Button Button(string text, EventHandler handler, bool primary = false)
    {
        var b = new Button { Text = text, AutoSize = true, MinimumSize = new(105, 36), Padding = new(9, 3, 9, 3),
            BackColor = primary ? Mint : Color.White, ForeColor = Ink, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        b.FlatAppearance.BorderColor = Color.FromArgb(195, 216, 210);
        b.Click += handler; return b;
    }
    public static FlowLayoutPanel Bar(params Control[] controls)
    {
        var bar = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new(6), WrapContents = true };
        bar.Controls.AddRange(controls); return bar;
    }
    public static Label Label(string text) => new() { Text = text, AutoSize = true, Margin = new(6, 10, 6, 5) };
    public static ComboBox Combo(int width = 170) => new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = width, Margin = new(4, 7, 4, 4) };
    public static DataGridView Grid() => new()
    {
        Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
        AutoGenerateColumns = false, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
        ReadOnly = true, RowHeadersVisible = false, MultiSelect = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowTemplate = { Height = 38 },
        ColumnHeadersHeight = 42, EnableHeadersVisualStyles = false,
        ColumnHeadersDefaultCellStyle = new() { BackColor = Ink, ForeColor = Color.White, Font = new("Segoe UI", 10, FontStyle.Bold) },
        DefaultCellStyle = new() { SelectionBackColor = Color.FromArgb(211, 248, 237), SelectionForeColor = Ink, Padding = new(5) },
        AlternatingRowsDefaultCellStyle = new() { BackColor = Paper }
    };
    public static void Column(DataGridView grid, string name, string title, float weight = 100, string? format = null)
    {
        var c = new DataGridViewTextBoxColumn { Name = name, HeaderText = title, FillWeight = weight, SortMode = DataGridViewColumnSortMode.NotSortable };
        if (format is not null) c.DefaultCellStyle.Format = format;
        grid.Columns.Add(c);
    }
    public static TableLayoutPanel Layout(params SizeType[] rows)
    {
        var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = rows.Length, Padding = new(12) };
        foreach (var type in rows) table.RowStyles.Add(new(type, type == SizeType.Percent ? 100 : 0));
        table.ColumnStyles.Add(new(SizeType.Percent, 100)); return table;
    }
    public static async Task Run(Control owner, Func<Task> action)
    {
        owner.Enabled = false; owner.Cursor = Cursors.WaitCursor;
        try { await action(); }
        catch (Exception ex) { Error(ex); }
        finally { if (!owner.IsDisposed) { owner.Enabled = true; owner.Cursor = Cursors.Default; } }
    }
    public static void Error(Exception ex)
    {
        string message = ex is MySqlException m
            ? $"Ошибка MySQL ({m.Number}). Проверьте, что сервер запущен, параметры подключения верны и база создана."
            : ex is InvalidOperationException or ArgumentException ? ex.Message : "Не удалось выполнить действие. Повторите попытку и проверьте подключение к базе.";
        MessageBox.Show(message, "Не удалось выполнить действие", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
    public static Image? LoadImage(string name)
    {
        var path = Path.Combine(AppContext.BaseDirectory, name);
        if (!File.Exists(path)) return null;
        try { using var image = Image.FromFile(path); return new Bitmap(image); }
        catch (ArgumentException) { return null; }
    }
}
