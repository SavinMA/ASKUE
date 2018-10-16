using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Npgsql;
using System.IO;

namespace askue
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            loginTextBox.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            login();
        }

        private void login()
        {
            if (loginTextBox.Text.Length > 0 && passBox.Password.Length > 0)
            {
                NpgsqlConnection conn = new NpgsqlConnection();
                conn.ConnectionString = File.ReadAllText("connection.ini");
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(String.Format("select count(login) from userpass where login='{0}' AND pass='{1}'", loginTextBox.Text.Trim(), passBox.Password.Trim()), conn);
                conn.Open();
                if (1 != Convert.ToInt32(da.SelectCommand.ExecuteScalar())) //this error rewrite
                {
                    MainWindow mw = new MainWindow();
                    this.Close();
                    mw.ShowDialog();
                }
                else
                    MessageBox.Show("Логин/Пароль не верный.\nПопробуйте ещё раз.");
                conn.Close();
            }
            else
                MessageBox.Show("Заполните все поля");
        }

        private void passBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                login();
        }

        private void loginTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                login();
        }
    }
}
