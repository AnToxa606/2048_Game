//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace Individualne
//{
//    public partial class PauseMenuForm: Form
//    {
//        public enum PauseAction { None, Restart, ExitToMenu }
//        public PauseAction SelectedAction { get; private set; } = PauseAction.None;

//        private Button btnContinue;
//        private Button btnRestart;
//        private Button btnExitToMenu;

//        public PauseMenuForm()
//        {
//            this.Text = "Меню паузи";
//            this.ClientSize = new Size(300, 250);
//            this.FormBorderStyle = FormBorderStyle.FixedDialog;
//            this.MaximizeBox = false;
//            this.StartPosition = FormStartPosition.CenterParent;

//            btnContinue = new Button
//            {
//                Text = "Продовжити",
//                Size = new Size(200, 40),
//                Location = new Point(50, 30),
//                Font = new Font("Segoe UI", 12, FontStyle.Bold)
//            };
//            btnContinue.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

//            btnRestart = new Button
//            {
//                Text = "Почати знову",

//                Size = new Size(200, 40),
//                Location = new Point(50, 90),
//                Font = new Font("Segoe UI", 12, FontStyle.Bold)
//            };
//            btnRestart.Click += (s, e) => { SelectedAction = PauseAction.Restart; this.DialogResult = DialogResult.OK; this.Close(); };



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Individualne
{
    public partial class PauseMenuForm : Form
    {
        public enum PauseAction { None, Restart, ExitToMenu }
        public PauseAction SelectedAction { get; private set; } = PauseAction.None;

        private Button btnContinue;
        private Button btnRestart;
        private Button btnExitToMenu;

        public PauseMenuForm()
        {
            this.Text = "Меню паузи";
            this.ClientSize = new Size(300, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            btnContinue = new Button
            {
                Text = "Продовжити",
                Size = new Size(200, 40),
                Location = new Point(50, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnContinue.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            btnRestart = new Button
            {
                Text = "Почати знову",

                Size = new Size(200, 40),
                Location = new Point(50, 90),
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnRestart.Click += (s, e) => { SelectedAction = PauseAction.Restart; this.DialogResult = DialogResult.OK; this.Close(); };

            btnExitToMenu = new Button
            {
                Text = "Вийти в меню",
                Size = new Size(200, 40),
                Location = new Point(50, 150),
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnExitToMenu.Click += (s, e) => { SelectedAction = PauseAction.ExitToMenu; this.DialogResult = DialogResult.OK; this.Close(); };

            this.Controls.Add(btnContinue);
            this.Controls.Add(btnRestart);
            this.Controls.Add(btnExitToMenu);

            ThemeManager.ApplyTheme(this);
        }
    }
}

//            btnExitToMenu = new Button
//            {
//                Text = "Вийти в меню",
//                Size = new Size(200, 40),
//                Location = new Point(50, 150),
//                Font = new Font("Segoe UI", 12, FontStyle.Bold)
//            };
//            btnExitToMenu.Click += (s, e) => { SelectedAction = PauseAction.ExitToMenu; this.DialogResult = DialogResult.OK; this.Close(); };

//            this.Controls.Add(btnContinue);
//            this.Controls.Add(btnRestart);
//            this.Controls.Add(btnExitToMenu);

//            ThemeManager.ApplyTheme(this);
//        }
//    }
//}
