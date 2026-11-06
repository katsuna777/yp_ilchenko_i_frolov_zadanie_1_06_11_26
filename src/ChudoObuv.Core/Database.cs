using System.Text.RegularExpressions;
using MySqlConnector;

namespace ChudoObuv.Core;

public sealed class Database(string connectionString)
{
    public MySqlConnection Open()
    {
        var connection = new MySqlConnection(connectionString);
        try { connection.Open(); return connection; }
        catch { connection.Dispose(); throw; }
    }

    public void Check()
    {
        using var c = Open();
        using var cmd = new MySqlCommand("SELECT COUNT(*) FROM users", c);
        cmd.ExecuteScalar();
    }

    public static void Initialize(DatabaseSettings settings)
    {
        if (!Regex.IsMatch(settings.Database, @"\A[a-zA-Z][a-zA-Z0-9_-]{0,63}\z"))
            throw new InvalidOperationException("Название базы: латинские буквы, цифры знак подчёркивания и дефис, до 64 символов.");
        using var c = new MySqlConnection(settings.ConnectionString(false));
        c.Open();
        using (var check = new MySqlCommand("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema=@db", c))
        {
            check.Parameters.AddWithValue("@db", settings.Database);
            if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                throw new InvalidOperationException("База уже содержит таблицы. Подключитесь к ней или укажите новое имя базы.");
        }
        using (var create = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS `{settings.Database}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci", c))
            create.ExecuteNonQuery();
        c.ChangeDatabase(settings.Database);
        foreach (var file in new[] { "schema.sql", "seed.sql" })
        {
            var assembly = typeof(Database).Assembly;
            using var stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().Single(n => n.EndsWith("." + file)))!;
            using var reader = new StreamReader(stream);
            using var command = new MySqlCommand(reader.ReadToEnd(), c);
            command.ExecuteNonQuery();
        }
    }
}
