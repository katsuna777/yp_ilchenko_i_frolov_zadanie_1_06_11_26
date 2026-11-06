using ChudoObuv.Core;
using MySqlConnector;

// This executable intentionally creates only a NEW isolated test database.
var settings = new DatabaseSettings
{
    Server = Environment.GetEnvironmentVariable("CHUDO_TEST_HOST") ?? "127.0.0.1",
    Port = uint.Parse(Environment.GetEnvironmentVariable("CHUDO_TEST_PORT") ?? "3306"),
    Username = Environment.GetEnvironmentVariable("CHUDO_TEST_USER") ?? "root",
    Password = Environment.GetEnvironmentVariable("CHUDO_TEST_PASSWORD") ?? "",
    Database = "chudo_test_" + Guid.NewGuid().ToString("N")
};
int checks = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new Exception("FAIL: " + name);
    checks++; Console.WriteLine("PASS: " + name);
}
void Reject(Action action, string name)
{
    try { action(); }
    catch (InvalidOperationException) { Check(true, name); return; }
    throw new Exception("FAIL: expected rejection: " + name);
}
try
{
    Database.Initialize(settings);
    var db = new Database(settings.ConnectionString());
    var store = new Store(db);
    var admin = store.Login("isivanov")!;
    var manager = store.Login("papetrov")!;
    var customer = store.Login("asidorova")!;
    Check(admin.CanEditOrders && manager.CanManageOrders && !manager.CanEditOrders, "resource roles");
    Check(store.Login("unknown") is null && store.Login("ISIVANOV") is null && store.Login("' OR 1=1 --") is null, "login lookup is exact and parameterized");
    var catalog = store.Catalog();
    Check(catalog.Products.Count == 31 && catalog.Stock.Count == 92, "all models and stock imported");
    Check(store.Orders(admin).Count == 10 && store.Orders(admin).Sum(o => o.Lines.Count) == 30, "historical orders and all lines imported");
    Reject(() => store.Orders(customer), "customer cannot view all orders");
    Reject(() => store.Orders(null), "guest cannot view orders");
    var stock = catalog.Stock.First(s => s.Available >= 5);
    var second = catalog.Stock.First(s => s.Id != stock.Id && s.Available > 0);
    OrderDraft Draft(User u, params RequestedLine[] lines) => new(DateTime.Now, u.LastName, u.FirstName, u.Patronymic, lines);
    Reject(() => store.SaveOrder(null, Draft(customer, new RequestedLine(stock.Id, 1))), "guest cannot order");
    Reject(() => store.SaveOrder(customer, Draft(admin, new RequestedLine(stock.Id, 1))), "customer cannot impersonate another client");
    Reject(() => store.SaveOrder(customer, Draft(customer)), "empty orders rejected");
    Reject(() => store.SaveOrder(customer, Draft(customer, new RequestedLine(stock.Id, 0))), "zero quantity rejected");
    Reject(() => store.SaveOrder(customer, Draft(customer, new RequestedLine(stock.Id, 1), new RequestedLine(stock.Id, 2))), "duplicate stock lines rejected");
    int orderCount = store.Orders(admin).Count;
    Reject(() => store.SaveOrder(customer, Draft(customer, new RequestedLine(stock.Id, 1), new RequestedLine(second.Id, second.Available + 1))), "insufficient stock rejected");
    Check(store.Catalog().Stock.Single(s => s.Id == stock.Id).Available == stock.Available && store.Orders(admin).Count == orderCount, "failed multi-line order rolls back all stock and header");
    int id = store.SaveOrder(customer, Draft(customer, new RequestedLine(stock.Id, 2)));
    var saved = store.Orders(admin).Single(o => o.Id == id);
    Check(store.Catalog().Stock.Single(s => s.Id == stock.Id).Available == stock.Available - 2, "create reserves exact quantity");
    Check(saved.Total == catalog.Products.Single(p => p.Id == stock.ProductId).Price * 2, "order total uses database price");
    var edit = new OrderDraft(saved.Date, saved.LastName, saved.FirstName, saved.Patronymic, [new RequestedLine(stock.Id, 3)], id, saved.Version);
    Reject(() => store.SaveOrder(manager, edit), "manager cannot edit existing order");
    Reject(() => store.SaveOrder(customer, edit), "customer cannot edit existing order");
    Reject(() => store.DeleteOrder(customer, id, saved.Version), "customer cannot delete orders");
    store.SaveOrder(admin, edit);
    var edited = store.Orders(admin).Single(o => o.Id == id);
    Check(edited.Version == 2 && store.Catalog().Stock.Single(s => s.Id == stock.Id).Available == stock.Available - 3, "admin edit reserves only quantity difference");
    Reject(() => store.SaveOrder(admin, edit), "stale edit rejected");
    Reject(() => store.DeleteOrder(manager, id, saved.Version), "stale delete rejected");
    store.SaveOrder(admin, edit with { Lines = [new RequestedLine(second.Id, 1)], Version = edited.Version });
    var replaced = store.Orders(admin).Single(o => o.Id == id);
    Check(store.Catalog().Stock.Single(s => s.Id == stock.Id).Available == stock.Available, "removing line restores its stock");
    store.DeleteOrder(manager, id, replaced.Version);
    Check(store.Catalog().Stock.Single(s => s.Id == second.Id).Available == second.Available, "manager deletion restores remaining lines");
    Reject(() => store.DeleteOrder(admin, id, replaced.Version), "repeated deletion cannot restore stock twice");
    // Imported prices remain fixed when current catalog price changes.
    var historical = store.Orders(admin).First();
    var historicalLine = historical.Lines.First();
    using (var c = db.Open())
    using (var cmd = new MySqlCommand("UPDATE products p JOIN stock_items s ON s.product_id=p.id SET p.price=p.price+100 WHERE s.id=@id", c))
    { cmd.Parameters.AddWithValue("@id", historicalLine.StockItemId); cmd.ExecuteNonQuery(); }
    store.SaveOrder(admin, new(historical.Date,historical.LastName,historical.FirstName,historical.Patronymic,
        historical.Lines.Select(l => new RequestedLine(l.StockItemId,l.Quantity)).ToList(),historical.Id,historical.Version));
    Check(store.Orders(admin).Single(o => o.Id == historical.Id).Lines.First(l => l.StockItemId == historicalLine.StockItemId).UnitPrice == historicalLine.UnitPrice, "editing preserves historical unit prices");
    // Two separate connections compete for one remaining pair.
    using (var c = db.Open())
    using (var cmd = new MySqlCommand("UPDATE stock_items SET quantity=1 WHERE id=@id", c))
    { cmd.Parameters.AddWithValue("@id", stock.Id); cmd.ExecuteNonQuery(); }
    using var start = new ManualResetEventSlim(false);
    async Task<bool> Compete() => await Task.Run(() =>
    {
        start.Wait();
        try { store.SaveOrder(customer, Draft(customer, new RequestedLine(stock.Id, 1))); return true; }
        catch (InvalidOperationException) { return false; }
    });
    var a = Compete(); var b = Compete(); start.Set();
    var results = await Task.WhenAll(a,b);
    Check(results.Count(x => x) == 1 && store.Catalog().Stock.Single(s => s.Id == stock.Id).Available == 0, "concurrent checkout cannot oversell last pair");
    Console.WriteLine($"{checks} checks passed.");
}
finally
{
    MySqlConnection.ClearAllPools();
    using var c = new MySqlConnection(settings.ConnectionString(false)); c.Open();
    using var cmd = new MySqlCommand($"DROP DATABASE IF EXISTS `{settings.Database}`", c); cmd.ExecuteNonQuery();
}
