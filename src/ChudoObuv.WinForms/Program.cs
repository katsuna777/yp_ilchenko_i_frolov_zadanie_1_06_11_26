using System.Globalization;
using ChudoObuv.WinForms.Data;

namespace ChudoObuv.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        ApplicationConfiguration.Initialize();
        Application.ThreadException += (_, e) => Ui.Error(e.Exception);
        var database = new Database(DatabaseConfig.ConnectionString);
        var store = new Store(database);
        Application.Run(new LoginForm(store));
    }
}
