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

        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditProduct());
        }
    }
}
