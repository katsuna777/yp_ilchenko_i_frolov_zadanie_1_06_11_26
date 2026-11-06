using ChudoObuv.WinForms.Data;

namespace ChudoObuv.WinForms;

public partial class LoginForm : Form
{
    private readonly Store store;
    public User? User { get; private set; }

    public LoginForm() : this(new Store(new Database(DatabaseConfig.ConnectionString))) { }
    public LoginForm(Store store)
    {
        this.store = store;
        InitializeComponent();
        AcceptButton = loginButton;
        Shown += (_, _) => login.Focus();
    }

    private async void LoginButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(login.Text))
        {
            MessageBox.Show("Введите логин.", "Вход");
            login.Focus();
            return;
        }
        User = null;
        await Ui.Run(this, async () =>
        {
            User = await Task.Run(() => store.Login(login.Text));
            if (User == null)
            {
                MessageBox.Show("Логин не найден. Проверьте ввод.", "Вход");
                return;
            }
        });
        if (User != null) OpenMainForm();
    }

    private void GuestButton_Click(object? sender, EventArgs e)
    {
        User = null;
        OpenMainForm();
    }

    private void OpenMainForm()
    {
        Hide();
        DialogResult result;
        using (var main = new MainForm(store, User))
        {
            result = main.ShowDialog();
        }
        if (result == DialogResult.Retry)
        {
            User = null;
            login.Clear();
            Show();
            login.Focus();
        }
        else Close();
    }
}
