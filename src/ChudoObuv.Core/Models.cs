using System.Text.Json;
using MySqlConnector;

namespace ChudoObuv.Core;

public enum Role { Customer, Manager, Admin }

public sealed record User(int Id, string Login, string LastName, string FirstName, string Patronymic, Role Role)
{
    public string FullName => $"{LastName} {FirstName} {Patronymic}";
    public bool CanManageOrders => Role is Role.Manager or Role.Admin;
    public bool CanEditOrders => Role == Role.Admin;
    public string RoleName => Role switch { Role.Admin => "Администратор", Role.Manager => "Менеджер", _ => "Покупатель" };
}

public sealed record Product(int Id, string Category, string Subcategory, string Image, string Name,
    string Manufacturer, string Description, string Composition, decimal Price);
public sealed record StockItem(int Id, int ProductId, string Size, int Available);
public sealed record Catalog(List<Product> Products, List<StockItem> Stock);
public sealed record OrderLine(int StockItemId, string ProductName, string Size, int Quantity, decimal UnitPrice)
{
    public decimal Total => Quantity * UnitPrice;
}
public sealed record Order(int Id, DateTime Date, string LastName, string FirstName, string Patronymic,
    int Version, List<OrderLine> Lines)
{
    public string Client => $"{LastName} {FirstName} {Patronymic}";
    public decimal Total => Lines.Sum(l => l.Total);
}
public sealed record RequestedLine(int StockItemId, int Quantity);
public sealed record OrderDraft(DateTime Date, string LastName, string FirstName, string Patronymic,
    IReadOnlyList<RequestedLine> Lines, int? Id = null, int Version = 0);

public sealed class DatabaseSettings
{
    public string Server { get; set; } = "177.1.181.207";
    public uint Port { get; set; } = 3306;
    public string Database { get; set; } = "isp-44_ilchenko_dm_chudo_obuv";
    public string Username { get; set; } = "isp-44_ilchenko_dm";
    public string Password { get; set; } = "";
    public string ConnectionString(bool withDatabase = true) => new MySqlConnectionStringBuilder
    {
        Server = Server, Port = Port, Database = withDatabase ? Database : "",
        UserID = Username, Password = Password, ConnectionTimeout = 5, DefaultCommandTimeout = 30,
        CharacterSet = "utf8mb4"
    }.ConnectionString;
    public static DatabaseSettings Load(string path) => File.Exists(path)
        ? JsonSerializer.Deserialize<DatabaseSettings>(File.ReadAllText(path)) ?? new() : new();
    public void Save(string path) => File.WriteAllText(path, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
}
