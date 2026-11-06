using ChudoObuv.Core;

namespace ChudoObuv.WinForms;

internal sealed class OrdersForm : Form
{
    private readonly Store store;
    private readonly User user;
    private readonly DataGridView grid = Ui.Grid();
    private readonly TextBox search = new() { Width = 300, PlaceholderText = "ФИО или номер заказа" };
    private List<Order> orders = [];
    private Order? Selected => grid.CurrentRow?.Tag as Order;
    public OrdersForm(Store store, User user)
    {
        this.store = store; this.user = user; Ui.Setup(this, "Заказы", 1100, 720);
        var layout = Ui.Layout(SizeType.AutoSize, SizeType.AutoSize, SizeType.Percent, SizeType.AutoSize);
        layout.Controls.Add(new Label { Text = "Заказы", Font = new(Font.FontFamily, 24, FontStyle.Bold), AutoSize = true }, 0, 0);
        layout.Controls.Add(Ui.Bar(search, Ui.Button("Обновить", async (_, _) => await Reload())), 0, 1);
        Ui.Column(grid, "id", "№", 30); Ui.Column(grid, "date", "Дата заказа", 80, "dd.MM.yyyy HH:mm");
        Ui.Column(grid, "client", "ФИО клиента", 180); Ui.Column(grid, "qty", "Пар", 40); Ui.Column(grid, "total", "Итого, ₽", 80, "N2");
        layout.Controls.Add(grid, 0, 2);
        var edit = Ui.Button("Редактировать", async (_, _) => await Open(false)); edit.Visible = user.CanEditOrders;
        layout.Controls.Add(Ui.Bar(Ui.Button("Состав заказа", async (_, _) => await Open(true)),
            Ui.Button("Новый заказ", async (_, _) => { using var form = new OrderEditor(store, user); form.ShowDialog(this); await Reload(); }, true),
            edit, Ui.Button("Удалить заказ", async (_, _) => await Delete())), 0, 3);
        Controls.Add(layout); search.TextChanged += (_, _) => Draw();
        grid.CellDoubleClick += async (_, e) => { if (e.RowIndex >= 0) await Open(true); };
        Shown += async (_, _) => await Reload();
    }
    private async Task Reload() => await Ui.Run(this, async () => { orders = await Task.Run(() => store.Orders(user)); Draw(); });
    private void Draw()
    {
        grid.Rows.Clear();
        foreach (var o in orders.Where(o => $"{o.Id} {o.Client}".Contains(search.Text.Trim(), StringComparison.CurrentCultureIgnoreCase)))
            grid.Rows[grid.Rows.Add(o.Id, o.Date, o.Client, o.Lines.Sum(l => l.Quantity), o.Total)].Tag = o;
    }
    private async Task Open(bool readOnly)
    {
        if (Selected is not { } order) return;
        using var form = new OrderEditor(store, user, order, readOnly); form.ShowDialog(this); await Reload();
    }
    private async Task Delete()
    {
        if (Selected is not { } order) return;
        if (MessageBox.Show($"Удалить заказ №{order.Id} клиента {order.Client}?\nТовары вернутся в доступный остаток.", "Удаление заказа", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        await Ui.Run(this, async () => { await Task.Run(() => store.DeleteOrder(user, order.Id, order.Version)); orders = await Task.Run(() => store.Orders(user)); Draw(); });
    }
}
