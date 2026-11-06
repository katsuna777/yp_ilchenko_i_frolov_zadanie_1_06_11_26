using ChudoObuv.Core;

namespace ChudoObuv.WinForms;

internal sealed class LoginForm : Form
{
    public User? User { get; private set; }
    public LoginForm(Store store)
    {
        Ui.Setup(this, "Вход", 480, 300); MinimumSize = new(480, 300); MaximizeBox = false;
        var login = new TextBox { Width = 370, PlaceholderText = "Логин из списка пользователей", MaxLength = 100, Margin = new(6, 12, 6, 12) };
        var button = Ui.Button("Войти", async (_, _) => await Ui.Run(this, async () =>
        {
            User = await Task.Run(() => store.Login(login.Text));
            if (User is null) throw new InvalidOperationException("Логин не найден. Проверьте ввод или продолжите как гость.");
            DialogResult = DialogResult.OK;
        }), true);
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new(24), FlowDirection = FlowDirection.TopDown };
        panel.Controls.Add(new Label { Text = "Войдите по логину", AutoSize = true, Font = new(Font.FontFamily, 20, FontStyle.Bold) });
        panel.Controls.Add(Ui.Label("Пароль для входа в приложение не требуется.")); panel.Controls.Add(login); panel.Controls.Add(button);
        Controls.Add(panel); AcceptButton = button;
    }
}
