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
    }

    private async void LoginButton_Click(object? sender, EventArgs e)
    {
        await Ui.Run(this, async () =>
        {
            User = await Task.Run(() => store.Login(login.Text));
            if (User == null)
            {
                MessageBox.Show("Логин не найден. Проверьте ввод.", "Вход");
                return;
            }
            DialogResult = DialogResult.OK;
        });
    }
}
