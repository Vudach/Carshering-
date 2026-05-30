using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CarsharingApp
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public int Rides { get; set; }
        public int Distance { get; set; }
        public int Fines { get; set; }
        public bool IsActive { get; set; }
    }

    public class FormAdmin : Form
    {
        private DataGridView dgvUsers;
        private TextBox txtSearch;
        private NumericUpDown nudMinAge, nudMinExp, nudMaxHours, nudDeposit;
        private NumericUpDown nudEco, nudComfort, nudBusiness;
        private NumericUpDown nudDiscount3h, nudDiscount6h;
        private List<UserInfo> users = new List<UserInfo>();

        public FormAdmin()
        {
            this.Text = "Carcharing — Администратор: Admin [Выйти] [✗]";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            LoadDemoUsers();

            // Пользователи
            Label lblUsers = new Label() { Text = "ПОЛЬЗОВАТЕЛИ", Font = new Font("Arial", 12, FontStyle.Bold), Location = new Point(10, 10), AutoSize = true };
            Label lblSearch = new Label() { Text = "Поиск:", Location = new Point(10, 40), AutoSize = true };
            txtSearch = new TextBox() { Location = new Point(60, 37), Size = new Size(200, 25) };
            txtSearch.TextChanged += (s, e) => FilterUsers();

            dgvUsers = new DataGridView()
            {
                Location = new Point(10, 70),
                Size = new Size(800, 250),
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvUsers.Columns.Add("Id", "ID");
            dgvUsers.Columns.Add("Login", "Логин");
            dgvUsers.Columns.Add("Rides", "Поездок");
            dgvUsers.Columns.Add("Distance", "Пробег(км)");
            dgvUsers.Columns.Add("Fines", "Штрафы");
            dgvUsers.Columns.Add("Status", "Статус");
            dgvUsers.Columns.Add("Actions", "Действия");

            RefreshUsersGrid();

            Button btnBlock = new Button() { Text = "Блокировать", Location = new Point(10, 330), Size = new Size(120, 35), BackColor = Color.Orange };
            btnBlock.Click += (s, e) => { if (dgvUsers.SelectedRows.Count > 0) BlockUser(); };

            Button btnUnblock = new Button() { Text = "Разблокировать", Location = new Point(140, 330), Size = new Size(120, 35), BackColor = Color.LightGreen };
            btnUnblock.Click += (s, e) => { if (dgvUsers.SelectedRows.Count > 0) UnblockUser(); };

            Button btnDelete = new Button() { Text = "Удалить", Location = new Point(270, 330), Size = new Size(120, 35), BackColor = Color.LightCoral };
            btnDelete.Click += (s, e) => { if (dgvUsers.SelectedRows.Count > 0) DeleteUser(); };

            // Настройки системы
            Label lblSettings = new Label() { Text = "НАСТРОЙКИ СИСТЕМЫ", Font = new Font("Arial", 12, FontStyle.Bold), Location = new Point(10, 380), AutoSize = true };

            // Требования
            Label lblRequirements = new Label() { Text = "Требования к аренде:", Font = new Font("Arial", 10, FontStyle.Bold), Location = new Point(10, 410), AutoSize = true };
            nudMinAge = CreateNUD(10, 440, 60, 18, 70, 21);
            nudMinExp = CreateNUD(180, 440, 60, 0, 30, 2);
            nudDeposit = CreateNUD(350, 440, 80, 0, 10000, 3000);
            nudMaxHours = CreateNUD(540, 440, 60, 1, 72, 24);

            AddLabeledControl("Мин. возраст:", 10, 440, nudMinAge);
            AddLabeledControl("Мин. стаж:", 170, 440, nudMinExp);
            AddLabeledControl("Депозит:", 300, 440, nudDeposit);
            AddLabeledControl("Макс. время:", 500, 440, nudMaxHours);

            // Тарифы
            Label lblTariffs = new Label() { Text = "Тарифы:", Font = new Font("Arial", 10, FontStyle.Bold), Location = new Point(10, 480), AutoSize = true };
            nudEco = CreateNUD(100, 500, 60, 1, 50, 5);
            nudComfort = CreateNUD(250, 500, 60, 1, 50, 8);
            nudBusiness = CreateNUD(400, 500, 60, 1, 50, 15);

            AddLabeledControl("Эконом:", 10, 500, nudEco);
            AddLabeledControl("Комфорт:", 160, 500, nudComfort);
            AddLabeledControl("Бизнес:", 310, 500, nudBusiness);

            // Скидки
            Label lblDiscounts = new Label() { Text = "Спецусловия (скидка):", Font = new Font("Arial", 10, FontStyle.Bold), Location = new Point(10, 540), AutoSize = true };
            nudDiscount3h = CreateNUD(200, 560, 50, 0, 50, 5);
            nudDiscount6h = CreateNUD(350, 560, 50, 0, 50, 10);

            AddLabeledControl("От 3 часов:", 10, 560, nudDiscount3h);
            AddLabeledControl("От 6 часов:", 260, 560, nudDiscount6h);

            Button btnSaveSettings = new Button() { Text = "Сохранить настройки", Location = new Point(10, 600), Size = new Size(200, 40), BackColor = Color.LightBlue };
            btnSaveSettings.Click += BtnSaveSettings_Click;

            this.Controls.Add(lblUsers);
            this.Controls.Add(lblSearch);
            this.Controls.Add(txtSearch);
            this.Controls.Add(dgvUsers);
            this.Controls.Add(btnBlock);
            this.Controls.Add(btnUnblock);
            this.Controls.Add(btnDelete);
            this.Controls.Add(lblSettings);
            this.Controls.Add(lblRequirements);
            this.Controls.Add(lblTariffs);
            this.Controls.Add(lblDiscounts);
            this.Controls.Add(btnSaveSettings);
        }

        private NumericUpDown CreateNUD(int x, int y, int width, int min, int max, int value)
        {
            var nud = new NumericUpDown() { Location = new Point(x, y), Size = new Size(width, 25), Minimum = min, Maximum = max, Value = value };
            this.Controls.Add(nud);
            return nud;
        }

        private void AddLabeledControl(string text, int x, int y, NumericUpDown nud)
        {
            Label lbl = new Label() { Text = text, Location = new Point(x, y - 20), AutoSize = true };
            this.Controls.Add(lbl);
        }

        private void LoadDemoUsers()
        {
            users = new List<UserInfo>
            {
                new UserInfo { Id = 1, Login = "Ivanov", Rides = 12, Distance = 347, Fines = 500, IsActive = true },
                new UserInfo { Id = 2, Login = "Petrov", Rides = 8, Distance = 211, Fines = 0, IsActive = true },
                new UserInfo { Id = 3, Login = "Sidorov", Rides = 23, Distance = 882, Fines = 1200, IsActive = false },
                new UserInfo { Id = 4, Login = "Kuznetsov", Rides = 3, Distance = 67, Fines = 0, IsActive = true }
            };
        }

        private void RefreshUsersGrid()
        {
            dgvUsers.Rows.Clear();
            var filtered = users.Where(u => u.Login.ToLower().Contains(txtSearch.Text.ToLower())).ToList();
            foreach (var u in filtered)
            {
                dgvUsers.Rows.Add(u.Id, u.Login, u.Rides, u.Distance, $"{u.Fines} ₽", u.IsActive ? "Активен" : "Заблок.", "Стат");
            }
        }

        private void FilterUsers() => RefreshUsersGrid();

        private void BlockUser()
        {
            int id = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["Id"].Value);
            var user = users.First(u => u.Id == id);
            user.IsActive = false;
            RefreshUsersGrid();
            MessageBox.Show($"Пользователь {user.Login} заблокирован.");
        }

        private void UnblockUser()
        {
            int id = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["Id"].Value);
            var user = users.First(u => u.Id == id);
            user.IsActive = true;
            RefreshUsersGrid();
            MessageBox.Show($"Пользователь {user.Login} разблокирован.");
        }

        private void DeleteUser()
        {
            int id = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["Id"].Value);
            var user = users.First(u => u.Id == id);
            if (MessageBox.Show($"Удалить {user.Login}?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                users.Remove(user);
                RefreshUsersGrid();
            }
        }

        private void BtnSaveSettings_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Настройки сохранены:\n" +
                $"Возраст: {nudMinAge.Value} лет, Стаж: {nudMinExp.Value} лет\n" +
                $"Депозит: {nudDeposit.Value} ₽\n" +
                $"Тарифы: Эконом {nudEco.Value}₽/мин, Комфорт {nudComfort.Value}₽/мин, Бизнес {nudBusiness.Value}₽/мин\n" +
                $"Скидки: от 3ч {nudDiscount3h.Value}%, от 6ч {nudDiscount6h.Value}%",
                "Сохранено", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}