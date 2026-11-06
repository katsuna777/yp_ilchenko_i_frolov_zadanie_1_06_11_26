#nullable enable

namespace ChudoObuv.WinForms;

partial class LoginForm
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        title = new Label();
        hint = new Label();
        login = new TextBox();
        loginButton = new Button();
        SuspendLayout();
        // title
        title.Name = "title";
        title.Location = new Point(25, 22);
        title.Size = new Size(390, 35);
        title.Text = "Вход в систему";
        title.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        // hint
        hint.Name = "hint";
        hint.Location = new Point(25, 65);
        hint.Size = new Size(390, 25);
        hint.Text = "Введите логин. Пароль не требуется.";
        // login
        login.Name = "login";
        login.Location = new Point(25, 102);
        login.Size = new Size(390, 28);
        login.MaxLength = 100;
        // loginButton
        loginButton.Name = "loginButton";
        loginButton.Location = new Point(25, 155);
        loginButton.Size = new Size(130, 34);
        loginButton.Text = "Войти";
        loginButton.UseVisualStyleBackColor = true;
        loginButton.Click += LoginButton_Click;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(450, 225);
        MinimumSize = new Size(450, 225);
        Font = new Font("Segoe UI", 9F);
        Text = "Вход | Чудо Обувь";
        StartPosition = FormStartPosition.CenterScreen;
        Controls.Add(title);
        Controls.Add(hint);
        Controls.Add(login);
        Controls.Add(loginButton);
        ResumeLayout(false);
        PerformLayout();
    }

    private Label title = null!;
    private Label hint = null!;
    private TextBox login = null!;
    private Button loginButton = null!;
}
