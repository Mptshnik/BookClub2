using BookClub.Database;
using BookClub.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace BookClub.Views.Pages
{
    public partial class AddEditProduct : Page
    {
        private string fileName = null;
        private string fromPath;

        public AddEditProduct()
        {
            InitializeComponent();

            cbCategories.ItemsSource = DatabaseContext.GetInstance().Categories.ToList();
        }

        private void cbCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!Storage.GetInstance().Data.TryGetValue("EditProduct", out dynamic value))
            {
                var product = new Products()
                {
                    Name = tbName.Text.Trim(),
                    Price = Convert.ToDecimal(tbPrice.Text.Trim()),
                    Description = tbDescription.Text.Trim(),
                    CategoryID = (cbCategories.SelectedItem as Categories).ID,
                    Discount = Convert.ToDecimal(tbDiscount.Text.Trim()),
                    Image = fileName,
                    //Тут еще должен быть Quantity, но его нет
                };

                if (fileName != null)
                {
                    string projectPath = Directory.GetCurrentDirectory().Replace("\\bin\\Debug", "").Replace("\\bin\\Release", "");
                    string toPath = $"{projectPath}\\Src\\Images\\{DateTime.Now.ToFileTime()}{fileName}";
                    
                    try 
                    { 
                        File.Copy(fromPath, toPath); 
                    } 
                    catch(Exception ex) 
                    { 
                        MessageBox.Show(ex.Message); 
                    }
                }

                try
                {
                    DatabaseContext.GetInstance().Products.Add(product);
                    DatabaseContext.GetInstance().SaveChanges();

                    MessageBox.Show("Запись успешно добавлена");

                    Manager.MainFrame.Navigate(new ItemsPage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.MainFrame.CanGoBack)
            {
                Manager.MainFrame.GoBack();
            }
        }

        private void btmAddImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog() { Multiselect = false };
                if (fileDialog.ShowDialog() == true)
                {
                    fileName = fileDialog.SafeFileName;
                    fromPath = fileDialog.FileName;

                    MessageBox.Show("Изображение успешно добавлено");
                }
            }
            catch 
            {
                MessageBox.Show("Не удалось добавить изображение");
            }
            
        }
    }
}
