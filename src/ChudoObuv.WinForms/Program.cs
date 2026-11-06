using System.Globalization;

namespace ChudoObuv.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        ApplicationConfiguration.Initialize();
        Application.ThreadException += (_, e) => Ui.Error(e.Exception);
        Application.Run(new MainForm());
    }
}
