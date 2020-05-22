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
using MBStore_MVC.Model;

namespace MBStore_MVC
{
    public partial class ShoppingBasket : Window
    {
        Product product;
        MainWindow mainWindow;
        public ShoppingBasket()
        {
            InitializeComponent();
        }
        public void SetProduct(Product product, MainWindow mainWindow)
        {
            this.product = product;
            this.mainWindow = mainWindow;
            string productinfo;
            productinfo = product.Product_id + " ";
            productinfo += product.Name + " ";
            productinfo += product.Color + " ";
            la_se_basket_product_info.Text = productinfo;
        }

        private void Btn_se_basket_enroll(object sender, RoutedEventArgs e)
        {
            int quantity;

            try
            {
                if (tb_se_basket_quantity.Text == "")
                    throw new Exception();

                quantity = int.Parse(tb_se_basket_quantity.Text);

                if (quantity > product.Stock)
                    throw new Exception("재고부족");

                mainWindow.Set_Sell_listview(product, quantity);
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
    }
}
