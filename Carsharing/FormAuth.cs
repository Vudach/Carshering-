using CarsharingApp;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CarsharingApp
{
    public class FormAuth : Form
    {
        private TextBox txtLogin, txtPassword;
        private Button btnLogin;
        private RadioButton rbUser, rbAdmin;

        public FormAuth()
        {
            this.Text = "Carcharing — Авторизация";
            this.Size = new Size(350, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblTitle = new Label() { Text = "Carcharing — Авторизация", Font = new Font("Arial", 14, FontStyle.Bold), Location = new Point(20, 10), Size = new Size(300, 30) };

            Label lblLogin = new Label() { Text = "Логин:", Location = new Point(30, 60), Size = new Size(80, 25) };
            txtLogin = new TextBox() { Location = new Point(120, 60), Size = new Size(180, 25) };

            Label lblPassword = new Label() { Text = "Пароль:", Location = new Point(30, 95), Size = new Size(80, 25) };
            txtPassword = new TextBox() { Location = new Point(120, 95), Size = new Size(180, 25), PasswordChar = '*' };

            rbUser = new RadioButton() { Text = "Пользователь", Location = new Point(120, 130), Checked = true };
            rbAdmin = new RadioButton() { Text = "Администратор", Location = new Point(230, 130) };

            btnLogin = new Button() { Text = "Войти", Location = new Point(120, 165), Size = new Size(100, 30), BackColor = Color.LightGreen };
            btnLogin.Click += BtnLogin_Click;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblLogin);
            this.Controls.Add(txtLogin);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(rbUser);
            this.Controls.Add(rbAdmin);
            this.Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;

            // Упрощённая авторизация для демо
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (rbUser.Checked)
            {
                // Передаём логин пользователя
                FormUser userForm = new FormUser(login);
                userForm.Show();
                this.Hide();
            }
            else
            {
                FormAdmin adminForm = new FormAdmin();
                adminForm.Show();
                this.Hide();
            }
        }
    }
}