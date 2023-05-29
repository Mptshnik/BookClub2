using BookClub.Database;
using BookClub.Utils;
using BookClub.Views.Pages;
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

namespace BookClub.Views.Windows
{
    public partial class AuthorizationWindow : Window
    {
        public AuthorizationWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = tbLogin.Text.Trim();
            string password = tbPassword.Password.Trim();

            Users user = DatabaseContext.GetInstance().Users.Where(u => u.Login == login && u.Password == password).FirstOrDefault();
            if (user == null)
            {
                MessageBox.Show("Не правильно введен логин или пароль");

                return;
            }

            Storage.GetInstance().Data.Add("CurrentUser", user);

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();

            Manager.MainFrame.Navigate(new ItemsPage());
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
