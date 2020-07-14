using MBStore_MVC.Model;
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

namespace MBStore_MVC
{
    /// <summary>
    /// StockReturn.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StockReturn : Window
    {
        Product product;
        MainWindow mainWindow;
        public StockReturn()
        {
            InitializeComponent();
        }

        public void SetProduct(Product product, MainWindow mainWindow)
        {
            this.product = product;
            this.mainWindow = mainWindow;
            string productinfo;
            productinfo = product.Name + " ";
            productinfo += product.Color + " ";
            lo_pse_basket_product_info.Text = productinfo;
            lo_pse_basket_stock.Content = "/" + product.Stock;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int quantity;

            try
            {
                if (lo_pse_basket_product_info.Text == "")
                    throw new Exception();

                quantity = int.Parse(lo_pse_basket_quantity.Text);
                //DB에 등록된 재고보다 적어낸 수량이 더 많으면
                if (quantity > product.Stock)
                    throw new Exception("재고부족");

                mainWindow.Set_Lo_Return_listview(product, quantity);
                this.Close();
            }
            catch (Exception ex)
            {
                if (ex.Message == "재고부족")
                    MessageBox.Show("재고가 부족합니다");
                else
                    MessageBox.Show("수량을 입력 하세요");
            }
        }
        private void KeyDown_lo_quantity(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Button_Click(sender, e);
        }
    }
}
