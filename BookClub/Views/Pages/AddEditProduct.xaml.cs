using BookClub.Database;
using BookClub.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
        private int editID;
        private string editImageName;
        private List<TextBox> inputs= new List<TextBox>();

        public AddEditProduct()
        {
            InitializeComponent();

            if (Storage.GetInstance().Data.ContainsKey("EditProduct"))
            {
                LoadProductInfo();
            }

            cbCategories.ItemsSource = DatabaseContext.GetInstance().Categories.ToList();
            cbCategories.SelectedIndex = 0;

            inputs.Add(tbName);
            inputs.Add(tbDescription);
            inputs.Add(tbDiscount);
            inputs.Add(tbPrice);
            inputs.Add(tbQuantity);
        }

        private void LoadProductInfo()
        {
            Products product = Storage.GetInstance().Data["EditProduct"] as Products;

            editID= product.ID;
            tbName.Text = product.Name;
            tbDescription.Text = product.Description;
            tbDiscount.Text = product.Discount.ToString();
            tbPrice.Text = product.Price.ToString();
            tbQuantity.Text = product.Quantity.ToString();
            cbCategories.SelectedItem = product.Categories;
            editImageName = product.Image;
        }

        private void cbCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            foreach (TextBox textBox in inputs)
            {
                if (string.IsNullOrEmpty(textBox.Text.Trim()))
                {
                    MessageBox.Show("Заполните все поля");

                    return;
                }
            }

            decimal price = 0;
            if (!decimal.TryParse(tbPrice.Text, out price))
            {
                MessageBox.Show("Цена должна представлять собой десятичное число");
                return;
            }

            decimal discount = 0;
            if (!decimal.TryParse(tbDiscount.Text, out discount))
            {
                MessageBox.Show("Скидка должна представлять собой десятичное число");
                return;
            }

            if (discount > 1 || discount < 0)
            {
                MessageBox.Show("Скидка должна находиться в диапазоне от 0 до 1");
                return;
            }

            int quantity = 0;
            if (!int.TryParse(tbQuantity.Text, out quantity))
            {
                MessageBox.Show("Количество должно быть целым числом");
                return;
            }

            if (quantity <= 0)
            {
                MessageBox.Show("Минимальное значение количества товара 1");
                return;
            }

            var product = new Products()
            {
                Name = tbName.Text.Trim(),
                Price = price,
                Description = tbDescription.Text.Trim(),
                CategoryID = (cbCategories.SelectedItem as Categories).ID,
                Discount = discount,
                Image = fileName,
                Quantity= quantity,
            };


            if (fileName != null)
            {
                string projectPath = Directory.GetCurrentDirectory().Replace("\\bin\\Debug", "").Replace("\\bin\\Release", "");
                string toPath = $"{projectPath}\\Src\\Images\\{DateTime.Now.ToFileTime()}{fileName}";

                try
                {
                    File.Copy(fromPath, toPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            if (!Storage.GetInstance().Data.ContainsKey("EditProduct"))
            {          
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
                product.ID = editID;
                if(fileName == null)
                {
                    product.Image = editImageName;
                }

                try
                {
                    DatabaseContext.GetInstance().Products.AddOrUpdate(product);
                    DatabaseContext.GetInstance().SaveChanges();

                    MessageBox.Show("Запись успешно изменена");
                    Storage.GetInstance().Data.Remove("EditProduct");

                    Manager.MainFrame.Navigate(new ItemsPage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }              
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.MainFrame.CanGoBack)
            {
                Storage.GetInstance().Data.Remove("EditProduct");

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
