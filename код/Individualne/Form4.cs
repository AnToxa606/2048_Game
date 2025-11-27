using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.IO;

namespace Individualne
{
    public partial class GameForm: Form
    {
        private Tile[,] tiles = new Tile[4, 4];
        private int[,] grid = new int[4, 4];
        private Random rnd = new Random();
        private Panel pnlBoard;
        private int score = 0;
        private string currentUser;
        private int bestScore;
        private Label lblScore;
        private Label lblBest;
        private const string ScoreFile = "scores.txt";
        private bool isMoving = false;
        private Button btnMenu;
        private Button btnTheme;

        public GameForm(string username)
        {
            currentUser = username;
            bestScore = LoadBestScore(currentUser);

            InitUI();
            InitGame();
            this.KeyPreview = true;
            this.KeyDown += GameForm_KeyDown;
        }

        private void InitUI()
        {
            this.Text = "2048 Анімована";
            this.ClientSize = new Size(380, 540);
            this.BackColor = Color.FromArgb(40, 40, 40);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            pnlBoard = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(340, 340),
                BackColor = Color.FromArgb(60, 60, 60),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(pnlBoard);

            lblScore = new Label
            {
                Location = new Point(20, 370),
                Size = new Size(180, 40),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Рахунок: 0"
            };
            this.Controls.Add(lblScore);

            lblBest = new Label
            {
                Location = new Point(180, 370),
                Size = new Size(180, 40),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleRight,
                Text = $"Найкращий: {bestScore}"
            };
            this.Controls.Add(lblBest);

            btnMenu = new Button
            {
                Location = new Point(50, 420),
                Size = new Size(120, 40),
                Text = "Меню",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnMenu.FlatAppearance.BorderSize = 0;
            btnMenu.Click += BtnMenu_Click;
            this.Controls.Add(btnMenu);

            btnTheme = new Button
            {
                Location = new Point(200, 420),
                Size = new Size(120, 40),
                Text = "Тема",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnTheme.FlatAppearance.BorderSize = 0;
            btnTheme.Click += BtnTheme_Click;
            this.Controls.Add(btnTheme);

            ThemeManager.ApplyTheme(this);
        }

        private void BtnTheme_Click(object sender, EventArgs e)
        {
            ThemeManager.ToggleTheme();
            ThemeManager.ApplyTheme(this);
            for(int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (tiles[i, j] != null)
                    {
                        if (tiles[i,j] != null)
                        {
                            tiles[i,j].UpdateValue(grid[i, j]);
                        }
                    }
                }
            }

        }

        private void BtnMenu_Click(object sender, EventArgs e)
        {
            using (var pauseMenu = new PauseMenuForm())
            {
                var result = pauseMenu.ShowDialog();
                if (result == DialogResult.OK)
                {
                    switch (pauseMenu.SelectedAction)
                    {
                        case PauseMenuForm.PauseAction.Restart:
                            InitGame();
                            break;
                        case PauseMenuForm.PauseAction.ExitToMenu:
                            this.Close();

                            break;
                    }
                }
            }
        }

        private void InitGame()
        {
            grid = new int[4, 4];
            tiles = new Tile[4, 4];
            pnlBoard.Controls.Clear();
            score = 0;
            AddRandomTile();
            AddRandomTile();
            UpdateUI();
        }

        private void UpdateUI()
        {
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    if (grid[r, c] != 0)
                    {
                        if (tiles[r, c] == null)
                        {
                            tiles[r, c] = new Tile(r, c, grid[r, c], pnlBoard);
                        }
                        else
                        {
                            tiles[r, c].UpdateValue(grid[r, c]);
                            tiles[r, c].Label.Visible = true; // 🛠 гарантуємо, що вона видима
                        }
                    }
                    else if (tiles[r, c] != null)
                    {
                        pnlBoard.Controls.Remove(tiles[r, c].Label);
                        tiles[r, c] = null;
                    }
                }
            }

            lblScore.Text = $"Рахунок: {score}";
            if (score > bestScore)
            {
                bestScore = score;
                lblBest.Text = $"Найкращий: {bestScore}";
                SaveBestScore(currentUser, bestScore);
            }

            if (IsGameOver())
            {
                MessageBox.Show($"Гру завершено!\nВаш рахунок: {score}", "Кінець гри", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BtnMenu_Click(null, null);
            }
        }

        private bool IsGameOver()
        {
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    if (grid[r, c] == 0)
                        return false;

            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                {
                    if (r < 3 && grid[r, c] == grid[r + 1, c]) return false;
                    if (c < 3 && grid[r, c] == grid[r, c + 1]) return false;
                }

            return true;
        }
        private async void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (isMoving) return;

            Vector2? dir = null;
            if (e.KeyCode == Keys.A) dir = Vector2.Left;
            else if (e.KeyCode == Keys.D) dir = Vector2.Right;
            else if (e.KeyCode == Keys.W) dir = Vector2.Up;
            else if (e.KeyCode == Keys.S) dir = Vector2.Down;

            if (dir != null)
            {
                isMoving = true;
                bool moved = await Move(dir.Value);
                if (moved)
                {
                    AddRandomTile();
                    UpdateUI();
                }
                isMoving = false;
            }
        }
        private void AddRandomTile()
        {
            var empty = new List<(int r, int c)>();
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    if (grid[r, c] == 0)
                        empty.Add((r, c));

            if (empty.Count > 0)
            {
                var (r, c) = empty[rnd.Next(empty.Count)];
                grid[r, c] = rnd.Next(10) < 9 ? 2 : 4;
            }
        }
        private async Task<bool> Move(Vector2 dir)
        {
            bool moved = false;
            bool[,] merged = new bool[4, 4];
            List<Task> tasks = new List<Task>();
            Tile[,] oldTiles = (Tile[,])tiles.Clone();
            tiles = new Tile[4, 4];
            List<(Tile fromTile, Tile toTile, int r, int c)> toMerge = new List<(Tile fromTile, Tile toTile, int r, int c)>();

            int[] range = { 0, 1, 2, 3 };
            if (dir.dx == 1 || dir.dy == 1)
                range = range.Reverse().ToArray();

            foreach (int r in range)
            {
                foreach (int c in range)
                {
                    int x = r, y = c;
                    if (grid[x, y] == 0) continue;

                    int nx = x, ny = y;
                    while (true)
                    {
                        int tx = nx + dir.dy;
                        int ty = ny + dir.dx;
                        if (tx < 0 || tx >= 4 || ty < 0 || ty >= 4) break;
                        if (grid[tx, ty] == 0)
                        {
                            nx = tx; ny = ty;
                        }
                        else if (grid[tx, ty] == grid[x, y] && !merged[tx, ty])
                        {
                            nx = tx; ny = ty;
                            merged[nx, ny] = true;
                            break;
                        }
                        else break;
                    }

                    if (nx != x || ny != y)
                    {
                        moved = true;
                        var tile = oldTiles[x, y];
                        var targetTile = oldTiles[nx, ny];
                        grid[x, y] = 0;

                        if (grid[nx, ny] == 0)
                        {
                            grid[nx, ny] = tile.Value;
                            tiles[nx, ny] = tile;
                            tasks.Add(tile.MoveTo(nx, ny));
                        }
                        else if (grid[nx, ny] == tile.Value)
                        {
                            grid[nx, ny] *= 2;
                            score += grid[nx, ny];
                            toMerge.Add((tile, targetTile, nx, ny));
                            tasks.Add(tile.MoveTo(nx, ny));
                        }
                    }
                    else
                    {
                        tiles[nx, ny] = oldTiles[x, y];
                    }
                }
            }

            await Task.WhenAll(tasks);

            foreach (var (a, b, r, c) in toMerge)
            {
                if (a?.Label != null && pnlBoard.Controls.Contains(a.Label))
                    pnlBoard.Controls.Remove(a.Label);
                if (b?.Label != null && pnlBoard.Controls.Contains(b.Label))
                    pnlBoard.Controls.Remove(b.Label);

                tiles[r, c] = null;

                var existing = pnlBoard.Controls.OfType<Label>()
                    .Where(lbl => lbl.Location == new Point(c * 85, r * 85))
                    .ToList();
                foreach (var label in existing)
                    pnlBoard.Controls.Remove(label);

                var newTile = new Tile(r, c, grid[r, c], pnlBoard);
                tiles[r, c] = newTile;
                _ = newTile.AnimateMerge(); // запускаємо не чекаючи (швидше)
            }

            return moved;
        }

        private int LoadBestScore(string user)
        {
            if (!File.Exists(ScoreFile)) return 0;
            foreach (var line in File.ReadAllLines(ScoreFile))
            {
                var parts = line.Split(':');
                if (parts.Length == 2 && parts[0] == user && int.TryParse(parts[1], out int val))
                    return val;
            }
            return 0;
        }

        private void SaveBestScore(string user, int score)
        {
            var dict = new Dictionary<string, int>();
            if (File.Exists(ScoreFile))
            {
                foreach (var line in File.ReadAllLines(ScoreFile))
                {
                    var parts = line.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int val))
                        dict[parts[0]] = val;
                }
            }

            dict[user] = score;
            File.WriteAllLines(ScoreFile, dict.Select(kvp => $"{kvp.Key}:{kvp.Value}"));
        }
        private void GameForm_Load(object sender, EventArgs e)
        {

        }
    }
}


