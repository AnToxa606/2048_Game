using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace Individualne
{
    //
    public static class UserManager
    {
        private static string filePath = "users.txt";


        public static bool Register(string username, string password)
        {
            if (Login(username, password)) return false;
            if (!File.Exists(filePath))
            {
                string hashed = Hash(password);
                File.AppendAllText(filePath, $"{username}:{hashed}{Environment.NewLine}");
                return true;
            }
            else
            {
                if (File.ReadAllLines(filePath).Any(line => line.StartsWith(username + ":"))) return false;
                string hashed = Hash(password);
                File.AppendAllText(filePath, $"{username}:{hashed}{Environment.NewLine}");
                return true;
            }
        }

        public static bool Login(string username, string password)
        {
            if (!File.Exists(filePath)) return false;

            string hashed = Hash(password);
            return File.ReadAllLines(filePath).Any(line => line == $"{username}:{hashed}");
        }

        private static string Hash(string password)
        {
            // Простий хеш (для прикладу, бажано використовувати SHA256 у реальних програмах)
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }
    }
    public class GameManager
    {
        public int[,] Grid { get; private set; }
        public int Score { get; private set; }

        private Random rnd = new Random();

        public GameManager()
        {
            Grid = new int[4, 4];
            AddRandomTile();
            AddRandomTile();
        }

        public void Restart()
        {
            Grid = new int[4, 4];
            Score = 0;
            AddRandomTile();
            AddRandomTile();
        }

        private void AddRandomTile()
        {
            List<(int, int)> empty = new List<(int, int)>();
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    if (Grid[r, c] == 0)
                        empty.Add((r, c));

            if (empty.Count > 0)
            {
                var (r, c) = empty[rnd.Next(empty.Count)];
                Grid[r, c] = rnd.Next(10) < 9 ? 2 : 4;
            }
        }

        public bool MoveLeft()
        {
            bool moved = false;

            for (int row = 0; row < 4; row++)
            {
                int[] line = new int[4];
                int index = 0;

                // зсув вліво
                for (int col = 0; col < 4; col++)
                {
                    if (Grid[row, col] != 0)
                        line[index++] = Grid[row, col];
                }

                // об'єднання
                for (int i = 0; i < 3; i++)
                {
                    if (line[i] != 0 && line[i] == line[i + 1])
                    {
                        line[i] *= 2;
                        Score += line[i];
                        line[i + 1] = 0;
                        moved = true;
                    }
                }

                // повторний зсув
                int[] newLine = new int[4];
                index = 0;
                for (int i = 0; i < 4; i++)
                    if (line[i] != 0)
                        newLine[index++] = line[i];

                for (int col = 0; col < 4; col++)
                {
                    if (Grid[row, col] != newLine[col])
                        moved = true;
                    Grid[row, col] = newLine[col];
                }
            }

            if (moved) AddRandomTile();
            return moved;
        }

        public bool MoveRight()
        {
            Rotate(180);
            bool moved = MoveLeft();
            Rotate(180);
            return moved;
        }

        public bool MoveUp()
        {
            Rotate(270);
            bool moved = MoveLeft();
            Rotate(90);
            return moved;
        }

        public bool MoveDown()
        {
            Rotate(90);
            bool moved = MoveLeft();
            Rotate(270);
            return moved;
        }
        private void Rotate(int angle)
        {
            for (int times = 0; times < angle / 90; times++)
            {
                int[,] rotated = new int[4, 4];
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                        rotated[j, 3 - i] = Grid[i, j];
                Grid = rotated;
            }
        }
        public bool IsGameOver()
        {
            // Є хоч одна порожня клітинка — гра триває
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    if (Grid[r, c] == 0)
                        return false;

            // Перевіряємо чи можна щось об'єднати
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    int val = Grid[r, c];

                    if (r < 3 && Grid[r + 1, c] == val) return false;
                    if (c < 3 && Grid[r, c + 1] == val) return false;
                }
            }

            return true; // ніде не можна зрушити
        }
    }
    public static class ScoreManager
    {
        private static string filePath = "scores.txt";

        public static int GetBestScore(string username)
        {
            if (!File.Exists(filePath)) return 0;

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(':');
                if (parts[0] == username && int.TryParse(parts[1], out int score))
                    return score;
            }

            return 0;
        }

        public static void SaveBestScore(string username, int score)
        {
            Dictionary<string, int> scores = new Dictionary<string, int>();

            if (File.Exists(filePath))
            {
                foreach (var line in File.ReadAllLines(filePath))
                {
                    var parts = line.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int s))
                    {
                        scores[parts[0]] = s;
                    }
                }
            }

            if (!scores.ContainsKey(username) || score > scores[username])
            {
                scores[username] = score;
                File.WriteAllLines(filePath, scores.Select(kvp => $"{kvp.Key}:{kvp.Value}"));
            }
        }
    }
    public class Tile
    {
        public Label Label { get; set; }
        public int Value { get; private set; }

        public Tile(int row, int col, int value, Panel container)
        {
            Value = value;
            Label = new Label
            {
                Size = new Size(80, 80),
                Location = new Point(col * 85, row * 85),
                BackColor = GetColor(value),
                Font = new Font("Arial", 20, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = value.ToString()
            };
            container.Controls.Add(Label);
        }

        public async Task MoveTo(int targetRow, int targetCol)
        {
            int targetX = targetCol * 85;
            int targetY = targetRow * 85;

            while (Math.Abs(Label.Location.X - targetX) > 0 || Math.Abs(Label.Location.Y - targetY) > 0)
            {
                int dx = targetX - Label.Location.X;
                int dy = targetY - Label.Location.Y;

                int stepX = Math.Abs(dx) <= 10 ? dx : Math.Sign(dx) * 10;
                int stepY = Math.Abs(dy) <= 10 ? dy : Math.Sign(dy) * 10;

                Label.Location = new Point(
                    Label.Location.X + stepX,
                    Label.Location.Y + stepY
                );

                await Task.Delay(3); // швидше
            }
        }

        public async Task AnimateMerge()
        {
            for (int i = 0; i < 2; i++)
            {
                Label.Font = new Font(Label.Font.FontFamily, 26, FontStyle.Bold);
                await Task.Delay(30);
                Label.Font = new Font(Label.Font.FontFamily, 20, FontStyle.Bold);
                await Task.Delay(30);
            }
        }

        public void UpdateValue(int newValue)
        {
            Value = newValue;
            Label.Text = newValue.ToString();
            Label.BackColor = GetColor(newValue);
        }

        private Color GetColor(int value)
        {
            switch (value)
            {
                case 2: return ThemeManager.CurrentTheme == ThemeMode.Dark ? Color.LightGray : Color.LightYellow;
                case 4:  return Color.Beige;
                case 8: return Color.Orange;
                case 16: return Color.DarkOrange;
                case 32: return Color.Tomato;
                case 64: return Color.Red;
                case 128: return Color.YellowGreen;
                case 256: return Color.LimeGreen;
                case 512: return Color.MediumSeaGreen;
                case 1024: return Color.DodgerBlue;
                case 2048: return Color.DarkViolet;
                case 4096: return Color.Black;
                    default: return Color.Gray;
            }
            ;
        }
    }

    public struct Vector2
    {
        public int dx, dy;
        public Vector2(int dx, int dy) { this.dx = dx; this.dy = dy; }

        public static Vector2 Left => new Vector2(-1, 0);
        public static Vector2 Right => new Vector2(1, 0);
        public static Vector2 Up => new Vector2(0, -1);
        public static Vector2 Down => new Vector2(0, 1);
    }
    public enum ThemeMode
    {
        Light,
        Dark
    }

    public static class ThemeManager
    {
        public static ThemeMode CurrentTheme { get; private set; } = ThemeMode.Dark;

        public static void ToggleTheme()
        {
            CurrentTheme = CurrentTheme == ThemeMode.Dark ? ThemeMode.Light : ThemeMode.Dark;
        }

        public static void ApplyTheme(Form form)
        {
            Color backColor, foreColor, panelColor, buttonBack, buttonFore;

            if (CurrentTheme == ThemeMode.Dark)
            {
                backColor = Color.FromArgb(40, 40, 40);
                foreColor = Color.White;
                panelColor = Color.FromArgb(60, 60, 60);
                buttonBack = Color.FromArgb(100, 100, 100);
                buttonFore = Color.White;
            }
            else
            {
                backColor = Color.White;
                foreColor = Color.Black;
                panelColor = Color.LightGray;
                buttonBack = Color.WhiteSmoke;
                buttonFore = Color.Black;
            }

            form.BackColor = backColor;
            foreach (Control ctrl in form.Controls)
            {
                if (ctrl is Label lbl)
                {
                    lbl.ForeColor = foreColor;
                }
                else if (ctrl is Button btn)
                {
                    btn.BackColor = buttonBack;
                    btn.ForeColor = buttonFore;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                }
                else if (ctrl is Panel pnl)
                {
                    pnl.BackColor = panelColor;
                }
                else if (ctrl is CheckBox chk)
                {
                    chk.ForeColor = foreColor;
                }
            }
        }
    }
}
