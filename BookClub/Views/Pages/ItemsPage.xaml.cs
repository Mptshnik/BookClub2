using BookClub.Database;
using BookClub.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
    public partial class ItemsPage : Page
    {
        private List<Products> products;
        private List<Products> rawProducts;
 
        public ItemsPage()
        {
            InitializeComponent();

            rawProducts = DatabaseContext.GetInstance().Products.ToList();
            products = rawProducts;

            lvProducts.ItemsSource = products;

            cbSortByPrice.SelectedIndex = 0;

            List<Categories> categories = DatabaseContext.GetInstance().Categories.ToList();

            categories.Add(new Categories() { Name = "Все", ID = -1 });

            cbCategories.ItemsSource = categories;
            cbCategories.SelectedIndex = categories.Count - 1;
        }

        private void miAddToOrder_Click(object sender, RoutedEventArgs e)
        {
            Products product = lvProducts.SelectedItem as Products;

            if (product == null)
            {
                MessageBox.Show("Выберите товар");

                return;
            }

            Orders currentOrder = new Orders();
            if(Storage.GetInstance().Data.TryGetValue("CurrentOrder", out dynamic value))
            {
                currentOrder = value as Orders;

                Products productExistsInOrder = currentOrder.OrdersProducts.Select(x => x.Products)
                    .Where(x => x.ID == product.ID).FirstOrDefault();


                OrdersProducts ordersProducts = new OrdersProducts();
                if (productExistsInOrder == null)
                {
                    ordersProducts.Products = product;
                    ordersProducts.Orders = currentOrder;
                    ordersProducts.Quantity = 1;

                    DatabaseContext.GetInstance().OrdersProducts.Add(ordersProducts);
                }
                else
                {
                    ordersProducts = currentOrder.OrdersProducts.ToList().Where(x => x.ProductID == product.ID)
                        .FirstOrDefault();

                    ordersProducts.Quantity++;

                    DatabaseContext.GetInstance().OrdersProducts.AddOrUpdate(ordersProducts);
                }
                         
                DatabaseContext.GetInstance().SaveChanges();
            }
            else
            {
                Manager.OrderButton.Visibility = Visibility.Visible;

                currentOrder.OrderDate = DateTime.Now;
                currentOrder.Code = new Random().Next(100, 999);
                currentOrder.PickupPoints = DatabaseContext.GetInstance().PickupPoints.ToList().FirstOrDefault();

                OrdersProducts ordersProducts = new OrdersProducts();
                ordersProducts.Products = product;
                ordersProducts.Orders = currentOrder;
                ordersProducts.Quantity = 1;

                currentOrder.OrdersProducts.Add(ordersProducts);

                DatabaseContext.GetInstance().Orders.Add(currentOrder);
                DatabaseContext.GetInstance().SaveChanges();


                Storage.GetInstance().Data.Add("CurrentOrder", currentOrder);
            }

        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            lvProducts.ItemsSource = Filter();
        }

        private void cbSortByPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lvProducts.ItemsSource = Filter();
        }

        private void cbCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lvProducts.ItemsSource = Filter();

        }

        private List<Products> Filter()
        {
            if (cbCategories.SelectedItem == null || cbSortByPrice.SelectedItem == null)
            {
                return products;
            }

            string search = tbSearch.Text;
            int index = cbSortByPrice.SelectedIndex;
            Categories category = cbCategories.SelectedItem as Categories;

            if(string.IsNullOrEmpty(search))
            {
                products = rawProducts;
            }
            else
            {
                products = rawProducts.Where(x=>x.Name.Contains(search) || x.Description.Contains(search)).ToList();
            }

            if (category.ID != -1)
            {
                products = products.Where(x => x.CategoryID == category.ID).ToList();
            }

            if (index == 1)
            {
                products = products.OrderBy(x => x.PriceWithDiscount).ToList();
            }
            else if(index == 2) 
            {
                products = products.OrderByDescending(x => x.PriceWithDiscount).ToList();
            }

            return products;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Products product = lvProducts.SelectedItem as Products;

            if (product == null)
            {
                MessageBox.Show("Выберите товар");

                return;
            }

            Storage.GetInstance().Data.Add("EditProduct", product);

            Manager.MainFrame.Navigate(new AddEditProduct());
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditProduct());
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Products product = lvProducts.SelectedItem as Products;

            if (product == null)
            {
                MessageBox.Show("Выберите товар");

                return;
            }

            try
            {
                if (MessageBox.Show("Удалить запись?", "Удаление записи", 
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DatabaseContext.GetInstance().Products.Remove(product);
                    DatabaseContext.GetInstance().SaveChanges();

                    MessageBox.Show("Запись успешно удалена");

                    lvProducts.ItemsSource = DatabaseContext.GetInstance().Products.ToList();
                }                     
            }
            catch
            (Exception ex)
            { 
                MessageBox.Show(ex.Message);
            }        
        }
    }
}
