using MySqlConnector;

namespace ChudoObuv.WinForms;

internal static class Ui
{
    public static async Task Run(Control owner, Func<Task> action)
    {
        // Не закрываем форму, пока выполняется запрос и обновляются её элементы.
        Form? form = owner as Form;
        FormClosingEventHandler preventClose = (_, e) =>
        {
            if (e.CloseReason == CloseReason.UserClosing) e.Cancel = true;
        };
        if (form != null) form.FormClosing += preventClose;
        owner.Enabled = false;
        owner.Cursor = Cursors.WaitCursor;
        try { await action(); }
        catch (Exception ex) { Error(ex); }
        finally
        {
            if (form != null) form.FormClosing -= preventClose;
            if (!owner.IsDisposed)
            {
                owner.Enabled = true;
                owner.Cursor = Cursors.Default;
            }
        }
    }
    public static void Error(Exception ex)
    {
        if (ex is MySqlException &&
            (ex.Message.Contains("Connect Timeout", StringComparison.OrdinalIgnoreCase) ||
             ex.Message.Contains("Connection Timeout", StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show(
                "Не удалось подключиться к MySQL за 30 секунд.\n" +
                "Сервер: 177.1.181.207, порт: 3306.\n" +
                "Проверьте доступ к серверу с этого компьютера командой PowerShell:\n" +
                "Test-NetConnection 177.1.181.207 -Port 3306",
                "Сервер не ответил", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        string message;
        if (ex is MySqlException mysql)
        {
            string reason = mysql.Number switch
            {
                1045 => "MySQL отклонил логин или пароль пользователя базы данных.",
                1044 => "У пользователя MySQL нет доступа к базе данных.",
                1049 => "Указанная база данных не найдена на сервере.",
                1146 => "В базе отсутствует необходимая таблица. Проверьте импорт SQL-файла.",
                _ => mysql.Message
            };
            message = $"Ошибка MySQL ({mysql.Number}).\n{reason}";
        }
        else if (ex is InvalidOperationException || ex is ArgumentException)
            message = ex.Message;
        else message = "Не удалось выполнить действие: " + ex.Message;
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
