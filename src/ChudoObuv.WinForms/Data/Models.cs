namespace ChudoObuv.WinForms.Data;

public enum Role { Customer, Manager, Admin }

public class User
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Patronymic { get; set; }
    public Role Role { get; set; }

    public User(int Id, string Login, string LastName, string FirstName, string Patronymic, Role Role)
    {
        this.Id = Id;
        this.Login = Login;
        this.LastName = LastName;
        this.FirstName = FirstName;
        this.Patronymic = Patronymic;
        this.Role = Role;
    }
    public string FullName => LastName + " " + FirstName + " " + Patronymic;
    public bool CanManageOrders => Role == Role.Manager || Role == Role.Admin;
    public bool CanEditOrders => Role == Role.Admin;
    public string RoleName => Role == Role.Admin ? "Администратор" : Role == Role.Manager ? "Менеджер" : "Покупатель";
}

public class Product
{
    public int Id { get; set; }
    public string Category { get; set; }
    public string Subcategory { get; set; }
    public string Image { get; set; }
    public string Name { get; set; }
    public string Manufacturer { get; set; }
    public string Description { get; set; }
    public string Composition { get; set; }
    public decimal Price { get; set; }

    public Product(int Id, string Category, string Subcategory, string Image, string Name, string Manufacturer, string Description, string Composition, decimal Price)
    {
        this.Id = Id;
        this.Category = Category;
        this.Subcategory = Subcategory;
        this.Image = Image;
        this.Name = Name;
        this.Manufacturer = Manufacturer;
        this.Description = Description;
        this.Composition = Composition;
        this.Price = Price;
    }
}

public class StockItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Size { get; set; }
    public int Available { get; set; }

    public StockItem(int Id, int ProductId, string Size, int Available)
    {
        this.Id = Id;
        this.ProductId = ProductId;
        this.Size = Size;
        this.Available = Available;
    }
}

public class Catalog
{
    public List<Product> Products { get; set; }
    public List<StockItem> Stock { get; set; }

    public Catalog(List<Product> Products, List<StockItem> Stock)
    {
        this.Products = Products;
        this.Stock = Stock;
    }
}

public class OrderLine
{
    public int StockItemId { get; set; }
    public string ProductName { get; set; }
    public string Size { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public OrderLine(int StockItemId, string ProductName, string Size, int Quantity, decimal UnitPrice)
    {
        this.StockItemId = StockItemId;
        this.ProductName = ProductName;
        this.Size = Size;
        this.Quantity = Quantity;
        this.UnitPrice = UnitPrice;
    }
    public decimal Total => Quantity * UnitPrice;
}

public class Order
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Patronymic { get; set; }
    public int Version { get; set; }
    public List<OrderLine> Lines { get; set; }

    public Order(int Id, DateTime Date, string LastName, string FirstName, string Patronymic, int Version, List<OrderLine> Lines)
    {
        this.Id = Id;
        this.Date = Date;
        this.LastName = LastName;
        this.FirstName = FirstName;
        this.Patronymic = Patronymic;
        this.Version = Version;
        this.Lines = Lines;
    }
    public string Client => LastName + " " + FirstName + " " + Patronymic;
    public decimal Total => Lines.Sum(line => line.Total);
}

public class RequestedLine
{
    public int StockItemId { get; set; }
    public int Quantity { get; set; }

    public RequestedLine(int StockItemId, int Quantity)
    {
        this.StockItemId = StockItemId;
        this.Quantity = Quantity;
    }
}

public class OrderDraft
{
    public DateTime Date { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Patronymic { get; set; }
    public IReadOnlyList<RequestedLine> Lines { get; set; }
    public int? Id { get; set; }
    public int Version { get; set; }

    public OrderDraft(DateTime Date, string LastName, string FirstName, string Patronymic, IReadOnlyList<RequestedLine> Lines, int? Id = null, int Version = 0)
    {
        this.Date = Date;
        this.LastName = LastName;
        this.FirstName = FirstName;
        this.Patronymic = Patronymic;
        this.Lines = Lines;
        this.Id = Id;
        this.Version = Version;
    }
}
