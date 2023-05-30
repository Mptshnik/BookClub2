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
using System.Windows.Shapes;

namespace BookClub.Views.Windows
{
    public partial class OrderWindow : Window
    {
        private Orders currentOrder;

        public OrderWindow()
        {
            InitializeComponent();

            cbPickupPoint.ItemsSource = DatabaseContext.GetInstance().PickupPoints.ToList();
            cbPickupPoint.SelectedIndex = 0;

            if (Storage.GetInstance().Data.TryGetValue("CurrentOrder", out dynamic obj))
            {
                currentOrder = obj as Orders;

                tbOrderCode.Text = currentOrder.Code.ToString();
                tbOrderDate.Text = currentOrder.OrderDate.ToShortDateString();
                tbOrderID.Text = currentOrder.ID.ToString();

                CalculateSum();

                lvProducts.ItemsSource = currentOrder.OrdersProducts.Select(x => x.Products).ToList();
            }
        }

        private void CalculateSum()
        {
            decimal totalPrice = 0;
            decimal totalPriceDiscount = 0;
            currentOrder.OrdersProducts.Select(x => x.Products).ToList().ForEach(x =>
            {
                totalPrice += x.PriceForQuantuty;
                totalPriceDiscount += x.PriceForQuantutyWithDiscount;

            });

            decimal orderDiscount = totalPrice - totalPriceDiscount;

            tbDiscount.Text = orderDiscount.ToString() + "Руб.";
            tbTotalSum.Text = totalPriceDiscount.ToString() + "Руб.";
        }

        private void btnConfirmOrder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Products product = lvProducts.SelectedItem as Products;

            if (product == null)
            {
                MessageBox.Show("Выберите товар");

                return;
            }

            OrdersProducts ordersProducts = currentOrder.OrdersProducts.FirstOrDefault(x => x.ProductID == product.ID);

            if (ordersProducts.Quantity > 1)
            {
                ordersProducts.Quantity--;

                //currentOrder.OrdersProducts.ToList().Where(x => x.ID == ordersProducts.ID).FirstOrDefault().Quantity;

                DatabaseContext.GetInstance().OrdersProducts.AddOrUpdate(ordersProducts);
                DatabaseContext.GetInstance().SaveChanges();
            }
            else if (ordersProducts.Quantity == 1)
            {
                DatabaseContext.GetInstance().OrdersProducts.Remove(ordersProducts);
                DatabaseContext.GetInstance().SaveChanges();
            }

            CalculateSum();

            lvProducts.ItemsSource = DatabaseContext.GetInstance().Orders.Where(x=>x.ID == currentOrder.ID).FirstOrDefault()
                .OrdersProducts.Select(x => x.Products).ToList();
        }
    }
}
