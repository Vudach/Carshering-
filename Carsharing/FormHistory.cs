using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CarsharingApp
{
    public class FormHistory : Form
    {
        private DataGridView dgvHistory;
        private List<Ride> rides;

        public FormHistory(List<Ride> history)
        {
            rides = history;
            this.Text = "Carcharing — История поездок [Закрыть]";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            dgvHistory = new DataGridView()
            {
                Location = new Point(10, 10),
                Size = new Size(760, 350),
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgvHistory.Columns.Add("Date", "Дата");
            dgvHistory.Columns.Add("Car", "Автомобиль");
            dgvHistory.Columns.Add("Duration", "Длит-сть");
            dgvHistory.Columns.Add("Cost", "Стоимость");
            dgvHistory.Columns.Add("Status", "Статус");

            int totalCost = 0;
            int totalRides = 0;
            foreach (var ride in rides)
            {
                string duration = ride.Hours >= 1 ? $"{ride.Hours:F1} ч" : $"{ride.Hours * 60:F0} мин";
                dgvHistory.Rows.Add(ride.Date.ToString("dd.MM.yyyy"), ride.CarName, duration, $"{ride.Cost} ₽", ride.Status);
                if (ride.Status == "Завершена")
                {
                    totalCost += ride.Cost;
                    totalRides++;
                }
            }

            Label lblStats = new Label()
            {
                Text = $"Статистика: Всего поездок: {rides.Count} | Общая сумма: {totalCost} ₽ | Общий пробег: {(rides.Count * 47)} км",
                Location = new Point(10, 370),
                Size = new Size(760, 30),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            Button btnSave = new Button() { Text = "Сохранить историю в файл", Location = new Point(10, 410), Size = new Size(200, 35) };
            btnSave.Click += (s, e) =>
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV файл|*.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                    {
                        sw.WriteLine("Дата;Автомобиль;Длительность;Стоимость;Статус");
                        for (int i = 0; i < dgvHistory.Rows.Count - 1; i++)
                        {
                            var row = dgvHistory.Rows[i];
                            sw.WriteLine($"{row.Cells[0].Value};{row.Cells[1].Value};{row.Cells[2].Value};{row.Cells[3].Value};{row.Cells[4].Value}");
                        }
                    }
                    MessageBox.Show("Сохранено!");
                }
            };

            this.Controls.Add(dgvHistory);
            this.Controls.Add(lblStats);
            this.Controls.Add(btnSave);
        }
    }
}