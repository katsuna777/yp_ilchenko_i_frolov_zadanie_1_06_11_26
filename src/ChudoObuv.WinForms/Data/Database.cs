using MySqlConnector;

namespace ChudoObuv.WinForms.Data;

public class Database
{
    private readonly string connectionString;

    public Database(string connectionString)
    {
        this.connectionString = connectionString;
    }
    public MySqlConnection Open()
    {
        var connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();
            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    public void Check()
    {
        using var c = Open();
        using var cmd = new MySqlCommand("SELECT COUNT(*) FROM users", c);
        cmd.ExecuteScalar();
    }

}
