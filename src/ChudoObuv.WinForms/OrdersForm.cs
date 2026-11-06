using ChudoObuv.WinForms.Data;

namespace ChudoObuv.WinForms;

public partial class OrdersForm : Form
{
    private readonly Store store;
    private readonly User user;
    private List<Order> orders = new List<Order>();
    private Order? Selected => grid.CurrentRow?.Tag as Order;

    public OrdersForm() : this(new Store(new Database(DatabaseConfig.ConnectionString)), new User(0, "", "", "", "", Role.Customer)) { }
    public OrdersForm(Store store, User user)
    {
        this.store = store;
        this.user = user;
        InitializeComponent();
        editButton.Visible = user.CanEditOrders;
    }

    private async void OrdersForm_Shown(object? sender, EventArgs e) { await Reload(); }
    private void Search_TextChanged(object? sender, EventArgs e) { Draw(); }
    private async void RefreshButton_Click(object? sender, EventArgs e) { await Reload(); }
    private async void ViewButton_Click(object? sender, EventArgs e) { await Open(true); }
    private async void EditButton_Click(object? sender, EventArgs e) { await Open(false); }
    private async void DeleteButton_Click(object? sender, EventArgs e) { await Delete(); }
    private async void Grid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) await Open(true);
    }
    private async void NewButton_Click(object? sender, EventArgs e)
    {
        using (var form = new OrderEditor(store, user)) form.ShowDialog(this);
        await Reload();
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
