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
    public partial class Login : Form
    {
        public string LoggedInUser { get; private set; }

        public Login()
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            paswtextBox.PasswordChar = '*';
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = logtextBox.Text.Trim();
            string password = paswtextBox.Text.Trim();

            if (UserManager.Login(username, password)&&(LoggedInUser == null || paswtextBox == null))
            {
                LoggedInUser = username;
                MessageBox.Show("Вхід успішний!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Невірний логін або пароль.");
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string username = logtextBox.Text.Trim();
            string password = paswtextBox.Text.Trim();

            if (UserManager.Register(username, password))
            {
                MessageBox.Show("Реєстрація успішна!");
            }
            else
            {
                MessageBox.Show("Користувач вже існує.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                paswtextBox.UseSystemPasswordChar=false;
            }
            else
            {
                paswtextBox.UseSystemPasswordChar = true;
                paswtextBox.PasswordChar = '*';
            }
        }
    }
}
