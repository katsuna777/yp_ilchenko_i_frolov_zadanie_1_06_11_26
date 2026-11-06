using ChudoObuv.WinForms.Data;
using MySqlConnector;

namespace ChudoObuv.WinForms;

internal static class Ui
{
    public static async Task Run(Control owner, Func<Task> action)
    {
        owner.Enabled = false; owner.Cursor = Cursors.WaitCursor;
        try { await action(); }
        catch (Exception ex) { Error(ex); }
        finally { if (!owner.IsDisposed) { owner.Enabled = true; owner.Cursor = Cursors.Default; } }
    }
    public static void Error(Exception ex)
    {
        if (ex is MySqlException && ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(
                "Не удалось подключиться к MySQL за 30 секунд.\n" +
                "Сервер: 177.1.181.207, порт: 3306.\n" +
                "Проверьте доступ к серверу с этого компьютера командой PowerShell:\n" +
                "Test-NetConnection 177.1.181.207 -Port 3306",
                "Сервер не ответил", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        string message = ex is MySqlException m
            ? $"Ошибка MySQL ({m.Number}). Проверьте доступность сервера базы данных."
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
