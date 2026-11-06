using System.Data;
using MySqlConnector;

namespace ChudoObuv.Core;

/// <summary>All writes enforce roles and stock rules here, independently of the forms.</summary>
public sealed class Store(Database db)
{
    public User? Login(string login)
    {
        using var c = db.Open();
        using var cmd = new MySqlCommand("SELECT id,login,last_name,first_name,patronymic,role FROM users WHERE login=@login", c);
        cmd.Parameters.AddWithValue("@login", login.Trim());
        using var r = cmd.ExecuteReader();
        return r.Read() ? ReadUser(r) : null;
    }

    private static User ReadUser(MySqlDataReader r) => new(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3), r.GetString(4), Enum.Parse<Role>(r.GetString(5)));

    private static User Authorize(MySqlConnection c, MySqlTransaction? tx, User? user, bool manage = false, bool edit = false)
    {
        if (user is null) throw new InvalidOperationException("Для оформления заказа войдите по логину.");
        using var cmd = new MySqlCommand("SELECT id,login,last_name,first_name,patronymic,role FROM users WHERE id=@id", c, tx);
        cmd.Parameters.AddWithValue("@id", user.Id);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) throw new InvalidOperationException("Пользователь не найден.");
        var actual = ReadUser(r);
        if (manage && !actual.CanManageOrders || edit && !actual.CanEditOrders)
            throw new InvalidOperationException("Недостаточно прав для работы с заказами.");
        return actual;
    }

    public Catalog Catalog()
    {
        using var c = db.Open();
        var products = new List<Product>();
        using (var cmd = new MySqlCommand("SELECT id,category,subcategory,image,name,manufacturer,description,composition,price FROM products ORDER BY name", c))
        using (var r = cmd.ExecuteReader())
            while (r.Read()) products.Add(new(r.GetInt32(0),r.GetString(1),r.GetString(2),r.GetString(3),r.GetString(4),r.GetString(5),r.GetString(6),r.GetString(7),r.GetDecimal(8)));
        var stock = new List<StockItem>();
        using (var cmd = new MySqlCommand("SELECT si.id,si.product_id,s.label,si.quantity FROM stock_items si JOIN sizes s ON s.id=si.size_id ORDER BY CAST(s.label AS DECIMAL(8,2)),s.label", c))
        using (var r = cmd.ExecuteReader())
            while (r.Read()) stock.Add(new(r.GetInt32(0),r.GetInt32(1),r.GetString(2),r.GetInt32(3)));
        return new(products, stock);
    }

    public List<Order> Orders(User? user)
    {
        using var c = db.Open();
        Authorize(c, null, user, manage: true);
        var orders = new Dictionary<int, Order>();
        using var cmd = new MySqlCommand("""
            SELECT o.id,o.ordered_at,o.client_last_name,o.client_first_name,o.client_patronymic,o.version,
                   i.stock_item_id,p.name,s.label,i.quantity,i.unit_price
            FROM orders o JOIN order_items i ON i.order_id=o.id
            JOIN stock_items si ON si.id=i.stock_item_id JOIN products p ON p.id=si.product_id
            JOIN sizes s ON s.id=si.size_id ORDER BY o.ordered_at DESC,o.id DESC,i.stock_item_id
            """, c);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            int id = r.GetInt32(0);
            if (!orders.TryGetValue(id, out var order))
                orders[id] = order = new(id,r.GetDateTime(1),r.GetString(2),r.GetString(3),r.GetString(4),r.GetInt32(5),[]);
            order.Lines.Add(new(r.GetInt32(6),r.GetString(7),r.GetString(8),r.GetInt32(9),r.GetDecimal(10)));
        }
        return orders.Values.ToList();
    }

    public int SaveOrder(User? user, OrderDraft draft)
    {
        Validate(draft);
        using var c = db.Open();
        using var tx = c.BeginTransaction(IsolationLevel.ReadCommitted);
        var actual = Authorize(c, tx, user, edit: draft.Id.HasValue);
        // Customers can place only their own order. Employees can specify another client.
        if (!actual.CanManageOrders && (draft.LastName.Trim() != actual.LastName || draft.FirstName.Trim() != actual.FirstName || draft.Patronymic.Trim() != actual.Patronymic))
            throw new InvalidOperationException("Покупатель может оформить заказ только на своё имя.");
        var old = new Dictionary<int, (int Quantity, decimal Price)>();
        if (draft.Id.HasValue)
        {
            LockOrder(c, tx, draft.Id.Value, draft.Version);
            old = ReadLines(c, tx, draft.Id.Value);
        }
        var requested = draft.Lines.ToDictionary(l => l.StockItemId, l => l.Quantity);
        var prices = AdjustStock(c, tx, old, requested);
        using var cmd = new MySqlCommand(draft.Id.HasValue
            ? "UPDATE orders SET ordered_at=@date,client_last_name=@last,client_first_name=@first,client_patronymic=@patronymic,version=version+1 WHERE id=@id"
            : "INSERT INTO orders(ordered_at,client_last_name,client_first_name,client_patronymic,created_by) VALUES(@date,@last,@first,@patronymic,@user)", c, tx);
        cmd.Parameters.AddWithValue("@date", draft.Date);
        cmd.Parameters.AddWithValue("@last", draft.LastName.Trim());
        cmd.Parameters.AddWithValue("@first", draft.FirstName.Trim());
        cmd.Parameters.AddWithValue("@patronymic", draft.Patronymic.Trim());
        cmd.Parameters.AddWithValue("@user", actual.Id);
        cmd.Parameters.AddWithValue("@id", draft.Id);
        cmd.ExecuteNonQuery();
        int id = draft.Id ?? checked((int)cmd.LastInsertedId);
        using (var clear = new MySqlCommand("DELETE FROM order_items WHERE order_id=@id", c, tx))
        { clear.Parameters.AddWithValue("@id", id); clear.ExecuteNonQuery(); }
        foreach (var line in draft.Lines)
        {
            using var insert = new MySqlCommand("INSERT INTO order_items VALUES(@order,@stock,@qty,@price)", c, tx);
            insert.Parameters.AddWithValue("@order", id);
            insert.Parameters.AddWithValue("@stock", line.StockItemId);
            insert.Parameters.AddWithValue("@qty", line.Quantity);
            insert.Parameters.AddWithValue("@price", old.TryGetValue(line.StockItemId, out var previous) ? previous.Price : prices[line.StockItemId]);
            insert.ExecuteNonQuery();
        }
        tx.Commit();
        return id;
    }

    public void DeleteOrder(User? user, int id, int version)
    {
        using var c = db.Open();
        using var tx = c.BeginTransaction(IsolationLevel.ReadCommitted);
        Authorize(c, tx, user, manage: true);
        LockOrder(c, tx, id, version);
        AdjustStock(c, tx, ReadLines(c, tx, id), new());
        using var cmd = new MySqlCommand("DELETE FROM orders WHERE id=@id", c, tx);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
        tx.Commit();
    }

    private static void Validate(OrderDraft d)
    {
        if (string.IsNullOrWhiteSpace(d.LastName) || string.IsNullOrWhiteSpace(d.FirstName))
            throw new InvalidOperationException("Заполните фамилию и имя клиента.");
        if (new[] { d.LastName, d.FirstName, d.Patronymic }.Any(s => s.Length > 100))
            throw new InvalidOperationException("Каждое поле ФИО должно быть не длиннее 100 символов.");
        if (d.Lines.Count == 0) throw new InvalidOperationException("Добавьте хотя бы одну товарную позицию.");
        if (d.Lines.Any(l => l.Quantity < 1 || l.Quantity > 100000)) throw new InvalidOperationException("Количество должно быть от 1 до 100000.");
        if (d.Lines.Select(l => l.StockItemId).Distinct().Count() != d.Lines.Count)
            throw new InvalidOperationException("Одинаковые модели и размеры должны быть в одной строке.");
    }

    private static void LockOrder(MySqlConnection c, MySqlTransaction tx, int id, int version)
    {
        using var cmd = new MySqlCommand("SELECT version FROM orders WHERE id=@id FOR UPDATE", c, tx);
        cmd.Parameters.AddWithValue("@id", id);
        var current = cmd.ExecuteScalar();
        if (current is null) throw new InvalidOperationException("Заказ уже удалён. Обновите список.");
        if (Convert.ToInt32(current) != version) throw new InvalidOperationException("Заказ изменён другим пользователем. Обновите список и откройте заказ заново.");
    }

    private static Dictionary<int, (int Quantity, decimal Price)> ReadLines(MySqlConnection c, MySqlTransaction tx, int id)
    {
        using var cmd = new MySqlCommand("SELECT stock_item_id,quantity,unit_price FROM order_items WHERE order_id=@id", c, tx);
        cmd.Parameters.AddWithValue("@id", id);
        using var r = cmd.ExecuteReader();
        var lines = new Dictionary<int, (int, decimal)>();
        while (r.Read()) lines.Add(r.GetInt32(0), (r.GetInt32(1), r.GetDecimal(2)));
        return lines;
    }

    private static Dictionary<int, decimal> AdjustStock(MySqlConnection c, MySqlTransaction tx,
        Dictionary<int, (int Quantity, decimal Price)> old, Dictionary<int, int> requested)
    {
        var prices = new Dictionary<int, decimal>();
        // Same lock order in every transaction prevents interleaved baskets deadlocking.
        foreach (int sid in old.Keys.Union(requested.Keys).Order())
        {
            using var select = new MySqlCommand("SELECT si.quantity,p.price,p.name,s.label FROM stock_items si JOIN products p ON p.id=si.product_id JOIN sizes s ON s.id=si.size_id WHERE si.id=@id FOR UPDATE", c, tx);
            select.Parameters.AddWithValue("@id", sid);
            int available; string name; string size;
            using (var r = select.ExecuteReader())
            {
                if (!r.Read()) throw new InvalidOperationException("Товарная позиция не найдена.");
                available = r.GetInt32(0); prices[sid] = r.GetDecimal(1); name = r.GetString(2); size = r.GetString(3);
            }
            int before = old.TryGetValue(sid, out var previous) ? previous.Quantity : 0;
            int after = requested.GetValueOrDefault(sid);
            long remaining = (long)available + before - after;
            if (remaining < 0) throw new InvalidOperationException($"Недостаточно товара «{name}», размер {size}. Доступно для этого заказа: {available + before}.");
            if (remaining > int.MaxValue) throw new InvalidOperationException("Превышен допустимый остаток товара.");
            using var update = new MySqlCommand("UPDATE stock_items SET quantity=@qty WHERE id=@id", c, tx);
            update.Parameters.AddWithValue("@qty", remaining); update.Parameters.AddWithValue("@id", sid);
            update.ExecuteNonQuery();
        }
        return prices;
    }
}
