using System.Globalization;
using ChudoObuv.Core;

namespace ChudoObuv.WinForms;

internal static class Program
{
    public static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ChudoObuv", "177.1.181.207", "appsettings.local.json");

    [STAThread]
    private static void Main()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");
        ApplicationConfiguration.Initialize();
        Application.ThreadException += (_, e) => Ui.Error(e.Exception);
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        DatabaseSettings settings;
        try { settings = DatabaseSettings.Load(File.Exists(SettingsPath) ? SettingsPath : Path.Combine(AppContext.BaseDirectory, "appsettings.local.json")); }
        catch { settings = new(); }
        using var connection = new ConnectionForm(settings);
        if (connection.ShowDialog() != DialogResult.OK) return;
        Application.Run(new MainForm(new Store(new Database(connection.Settings.ConnectionString()))));
    }
}
