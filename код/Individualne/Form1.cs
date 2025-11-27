using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Individualne
{
    public partial class MainMenuForm : Form
    {
        private Button btnPlay;
        private Button btnHighScore;
        private Button btnExit;

        public MainMenuForm()
        {
            InitializeComponent();
            this.Text = "2048 - Головне меню";
            this.ClientSize = new Size(300, 300);
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;

            btnPlay = new Button
            {
                Text = "Грати",
                Size = new Size(200, 40),
                Location = new Point(50, 50),
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnPlay.Click += BtnPlay_Click;

            btnHighScore = new Button
            {
                Text = "Макс. рахунок",
                Size = new Size(200, 40),
                Location = new Point(50, 110),
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnHighScore.Click += BtnHighScore_Click;

            btnExit = new Button
            {
                Text = "Вийти",
                Size = new Size(200, 40),
                Location = new Point(50, 170),
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnExit.Click += (s, e) => Application.Exit();

            this.Controls.Add(btnPlay);
            this.Controls.Add(btnHighScore);
            this.Controls.Add(btnExit);

            ThemeManager.ApplyTheme(this);
            axWindowsMediaPlayer1.uiMode = "none"; // Без кнопок управління
            axWindowsMediaPlayer1.stretchToFit = true; // Розтягнути відео на весь екран
            axWindowsMediaPlayer1.URL = ThemeManager.CurrentTheme == ThemeMode.Light ? "vidio w.mp4" : "vidio.mp4";  // шлях до відео
            axWindowsMediaPlayer1.settings.setMode("loop", true); // Зациклення
            axWindowsMediaPlayer1.SendToBack(); // Відправити за всі елементи

        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            using (Login login = new Login())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    var game = new GameForm(login.LoggedInUser);
                    game.ShowDialog();
                    ThemeManager.ApplyTheme(this);
                    axWindowsMediaPlayer1.uiMode = "none"; // Без кнопок управління
                    axWindowsMediaPlayer1.stretchToFit = true; // Розтягнути відео на весь екран
                    axWindowsMediaPlayer1.URL = ThemeManager.CurrentTheme == ThemeMode.Light ? "vidio w.mp4" : "vidio.mp4";  // шлях до відео
                    axWindowsMediaPlayer1.settings.setMode("loop", true); // Зациклення
                    axWindowsMediaPlayer1.SendToBack();
                }
            }
        }

        private void BtnHighScore_Click(object sender, EventArgs e)
        {
            string highest = LoadGlobalHighScore();
            MessageBox.Show($"Найкращий рахунок: {highest}", "Рекорд", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string LoadGlobalHighScore()
        {
            string file = "scores.txt";
            if (!File.Exists(file)) return "0";

            var lines = File.ReadAllLines(file);
            int max = 0;
            foreach (var line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out int val))
                {
                    if (val > max) max = val;
                }
            }
            return max.ToString();
        }

        private void MainMenuForm_Load(object sender, EventArgs e)
        {

        }
    }
}

