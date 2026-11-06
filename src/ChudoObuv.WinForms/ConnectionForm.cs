using ChudoObuv.Core;

namespace ChudoObuv.WinForms;

internal sealed class ConnectionForm : Form
{
    private readonly TextBox server = new(), database = new(), username = new(), password = new() { UseSystemPasswordChar = true };
    private readonly NumericUpDown port = new() { Minimum = 1, Maximum = 65535 };
    public DatabaseSettings Settings { get; private set; }

    public ConnectionForm(DatabaseSettings settings)
    {
        Settings = settings;
        Ui.Setup(this, "Подключение к MySQL", 650, 520); MinimumSize = new(650, 520);
        var layout = Ui.Layout(SizeType.AutoSize, SizeType.AutoSize, SizeType.Percent, SizeType.AutoSize);
        layout.Controls.Add(new Label { Text = "Локальная база магазина", Font = new(Font, FontStyle.Bold), AutoSize = true }, 0, 0);
        layout.Controls.Add(new Label { Text = "Запустите MySQL. При первом запуске создайте базу из ресурсов.", AutoSize = true, MaximumSize = new(580, 0), Margin = new(3, 16, 3, 16) }, 0, 1);
        var fields = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
        fields.ColumnStyles.Add(new(SizeType.Absolute, 180)); fields.ColumnStyles.Add(new(SizeType.Percent, 100));
        var labels = new[] { "Сервер", "Порт", "База данных", "Пользователь MySQL", "Пароль MySQL" };
        Control[] inputs = [server, port, database, username, password];
        for (int i = 0; i < inputs.Length; i++) { inputs[i].Dock = DockStyle.Fill; inputs[i].Margin = new(5, 8, 5, 8); fields.Controls.Add(Ui.Label(labels[i]), 0, i); fields.Controls.Add(inputs[i], 1, i); }
        server.Text = settings.Server; port.Value = settings.Port; database.Text = settings.Database;
        username.Text = settings.Username; password.Text = settings.Password;
        layout.Controls.Add(fields, 0, 2);
        layout.Controls.Add(Ui.Bar(Ui.Button("Создать и заполнить базу", async (_, _) => await Connect(true)),
            Ui.Button("Подключиться", async (_, _) => await Connect(false), true)), 0, 3);
        Controls.Add(layout);
    }
    private async Task Connect(bool initialize) => await Ui.Run(this, async () =>
    {
        Settings = new() { Server = server.Text.Trim(), Port = (uint)port.Value, Database = database.Text.Trim(), Username = username.Text.Trim(), Password = password.Text };
        await Task.Run(() => { if (initialize) Database.Initialize(Settings); new Database(Settings.ConnectionString()).Check(); });
        Settings.Save(Program.SettingsPath); DialogResult = DialogResult.OK;
    });
}
