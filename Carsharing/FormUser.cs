using CarsharingApp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CarsharingApp
{
    public class Car
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public int PricePerMinute { get; set; }
        public int MinAge { get; set; }
        public int MinExperience { get; set; }
        public int Deposit { get; set; }
    }

    public class Ride
    {
        public DateTime Date { get; set; }
        public string CarName { get; set; }
        public double Hours { get; set; }
        public int Cost { get; set; }
        public string Status { get; set; }
        public int DistanceKm { get; set; }
    }

    public class FormUser : Form
    {
        private ComboBox cmbCars;
        private DateTimePicker dtpDate, dtpTime;
        private NumericUpDown nudDuration;
        private ComboBox cmbDurationUnit;
        private Label lblRequirements, lblTotal;
        private Button btnCalculate, btnBook, btnHistory;
        private List<Ride> history = new List<Ride>();
        private string username;
        private List<Car> cars;

        public FormUser(string login)
        {
            username = login;
            this.Text = $"Carcharing — Пользователь: {login} [Выйти] [✖]";
            this.Size = new Size(700, 550);
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeCars();
            LoadDemoHistory();

            Label lblAvailable = new Label() { Text = "ДОСТУПНЫЕ АВТОМОБИЛИ", Font = new Font("Arial", 12, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };

            cmbCars = new ComboBox() { Location = new Point(20, 50), Size = new Size(300, 30), DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var car in cars)
                cmbCars.Items.Add($"{car.Name} | {car.Class} | {car.PricePerMinute} ₽/мин");
            cmbCars.SelectedIndex = 0;
            cmbCars.SelectedIndexChanged += (s, e) => UpdateRequirements();

            // Параметры аренды
            Label lblRentParams = new Label() { Text = "ПАРАМЕТРЫ АРЕНДЫ", Font = new Font("Arial", 12, FontStyle.Bold), Location = new Point(20, 100), AutoSize = true };

            dtpDate = new DateTimePicker() { Location = new Point(20, 130), Size = new Size(150, 25), Format = DateTimePickerFormat.Short };
            dtpTime = new DateTimePicker() { Location = new Point(180, 130), Size = new Size(100, 25), Format = DateTimePickerFormat.Time, ShowUpDown = true };

            Label lblDuration = new Label() { Text = "Длительность:", Location = new Point(20, 165), AutoSize = true };
            nudDuration = new NumericUpDown() { Location = new Point(120, 163), Size = new Size(60, 25), Minimum = 0.5M, Maximum = 48, DecimalPlaces = 1, Increment = 0.5M, Value = 2 };
            cmbDurationUnit = new ComboBox() { Location = new Point(190, 163), Size = new Size(80, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbDurationUnit.Items.AddRange(new[] { "часа", "минуты" });
            cmbDurationUnit.SelectedIndex = 0;

            lblRequirements = new Label() { Location = new Point(20, 210), Size = new Size(400, 100), Font = new Font("Arial", 9, FontStyle.Regular) };

            lblTotal = new Label() { Location = new Point(20, 320), Size = new Size(400, 30), Font = new Font("Arial", 10, FontStyle.Bold) };

            btnCalculate = new Button() { Text = "Рассчитать стоимость", Location = new Point(20, 360), Size = new Size(180, 35), BackColor = Color.LightBlue };
            btnCalculate.Click += BtnCalculate_Click;

            btnBook = new Button() { Text = "Забронировать", Location = new Point(220, 360), Size = new Size(180, 35), BackColor = Color.LightGreen };
            btnBook.Click += BtnBook_Click;

            btnHistory = new Button() { Text = "История поездок", Location = new Point(420, 360), Size = new Size(150, 35) };
            btnHistory.Click += BtnHistory_Click;

            Button btnSaveHistory = new Button() { Text = "Сохранить историю в файл", Location = new Point(420, 400), Size = new Size(150, 35) };
            btnSaveHistory.Click += BtnSaveHistory_Click;

            this.Controls.Add(lblAvailable);
            this.Controls.Add(cmbCars);
            this.Controls.Add(lblRentParams);
            this.Controls.Add(dtpDate);
            this.Controls.Add(dtpTime);
            this.Controls.Add(lblDuration);
            this.Controls.Add(nudDuration);
            this.Controls.Add(cmbDurationUnit);
            this.Controls.Add(lblRequirements);
            this.Controls.Add(lblTotal);
            this.Controls.Add(btnCalculate);
            this.Controls.Add(btnBook);
            this.Controls.Add(btnHistory);
            this.Controls.Add(btnSaveHistory);

            UpdateRequirements();
        }

        private void InitializeCars()
        {
            cars = new List<Car>
            {
                new Car { Name = "Hyundai Solaris", Class = "Эконом", PricePerMinute = 5, MinAge = 19, MinExperience = 0, Deposit = 1000 },
                new Car { Name = "Kia Rio", Class = "Эконом", PricePerMinute = 6, MinAge = 19, MinExperience = 0, Deposit = 1000 },
                new Car { Name = "Volkswagen Polo", Class = "Комфорт", PricePerMinute = 8, MinAge = 21, MinExperience = 1, Deposit = 2000 },
                new Car { Name = "BMW 3 Series", Class = "Бизнес", PricePerMinute = 15, MinAge = 21, MinExperience = 2, Deposit = 3000 }
            };
        }

        private void LoadDemoHistory()
        {
            history = new List<Ride>
            {
                new Ride { Date = new DateTime(2026, 5, 20), CarName = "Kia Rio", Hours = 1.5, Cost = 540, Status = "Завершена", DistanceKm = 45 },
                new Ride { Date = new DateTime(2026, 5, 18), CarName = "Hyundai Solaris", Hours = 0.75, Cost = 225, Status = "Завершена", DistanceKm = 22 },
                new Ride { Date = new DateTime(2026, 5, 15), CarName = "BMW 3 Series", Hours = 2, Cost = 1800, Status = "Завершена", DistanceKm = 68 },
                new Ride { Date = new DateTime(2026, 5, 10), CarName = "Volkswagen Polo", Hours = 3, Cost = 1440, Status = "Отменена", DistanceKm = 0 }
            };
        }

        private void UpdateRequirements()
        {
            Car selected = cars[cmbCars.SelectedIndex];
            lblRequirements.Text = $"Требования к аренде:\n" +
                                   $"  • Минимальный возраст: {selected.MinAge} год\n" +
                                   $"  • Стаж вождения: от {selected.MinExperience} лет\n" +
                                   $"  • Депозит: {selected.Deposit} ₽\n" +
                                   $"  • Необходимо: паспорт, права";
        }

        private int CalculateTotal()
        {
            Car selected = cars[cmbCars.SelectedIndex];
            double duration = (double)nudDuration.Value;
            if (cmbDurationUnit.SelectedIndex == 1) // минуты
                return (int)(duration * selected.PricePerMinute);
            else // часы
                return (int)(duration * 60 * selected.PricePerMinute);
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            int total = CalculateTotal();
            lblTotal.Text = $"Итого к оплате: {total} ₽ (включая страховку)";
        }

        private void BtnBook_Click(object sender, EventArgs e)
        {
            int total = CalculateTotal();
            Car selected = cars[cmbCars.SelectedIndex];

            DialogResult result = MessageBox.Show($"Подтвердите бронирование:\n" +
                $"Автомобиль: {selected.Name}\n" +
                $"Дата: {dtpDate.Value.ToShortDateString()} {dtpTime.Value.ToShortTimeString()}\n" +
                $"Сумма: {total} ₽\n" +
                $"Депозит: {selected.Deposit} ₽\n\n" +
                $"Забронировать?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                double durationHours = cmbDurationUnit.SelectedIndex == 0 ? (double)nudDuration.Value : (double)nudDuration.Value / 60;
                history.Insert(0, new Ride
                {
                    Date = dtpDate.Value,
                    CarName = selected.Name,
                    Hours = durationHours,
                    Cost = total,
                    Status = "Забронирована",
                    DistanceKm = 0
                });
                MessageBox.Show("Бронирование успешно создано!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnHistory_Click(object sender, EventArgs e)
        {
            FormHistory historyForm = new FormHistory(history);
            historyForm.ShowDialog();
        }

        private void BtnSaveHistory_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV файл|*.csv|Текстовый файл|*.txt";
            sfd.FileName = $"history_{username}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    sw.WriteLine("Дата;Автомобиль;Длительность(ч);Стоимость;Статус");
                    foreach (var ride in history)
                        sw.WriteLine($"{ride.Date:dd.MM.yyyy};{ride.CarName};{ride.Hours:F1};{ride.Cost} ₽;{ride.Status}");
                }
                MessageBox.Show("История сохранена!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}