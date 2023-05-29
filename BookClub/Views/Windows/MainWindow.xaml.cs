using BookClub.Database;
using BookClub.Utils;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookClub.Views.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Manager.MainFrame = FrameContent;

            Users user = Storage.GetInstance().Data["CurrentUser"] as Users;
            tbUsername.Text = $"{user.LastName} {user.FirstName} {user.MiddleName}";
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Storage.GetInstance().Clear();

            AuthorizationWindow authorizationWindow = new AuthorizationWindow();

            authorizationWindow.Show();

            Close();
        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
