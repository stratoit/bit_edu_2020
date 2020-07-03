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
            tb_se_basket_quantity.Focus();
        }
        public void SetProduct(Product product, MainWindow mainWindow, string uri)
        {
            this.product = product;
            this.mainWindow = mainWindow;

            img_sb_phone_f.Source = new BitmapImage(new Uri(uri + "/phone/" + product.Name.Replace("+", "plus") + "_" + product.Color + "_F.JPG", UriKind.Absolute));
            img_sb_phone_b.Source = new BitmapImage(new Uri(uri + "/phone/" + product.Name.Replace("+", "plus") + "_" + product.Color + "_B.JPG", UriKind.Absolute));
            lb_sb_name.Content = product.Name;
            lb_sb_color.Content = "(" + product.Color + ")";
            img_sb_brand.Source = new BitmapImage(new Uri(@uri+"/brand/" + product.Brand + ".png", UriKind.Absolute));
            lb_sb_manufacture.Content = ((DateTime)product.Manufacture).ToString("yyyy. MM");
            lb_sb_inch.Content = product.Inch + " inch";
            lb_sb_ram.Content = product.Ram + " GB";
            lb_sb_cpu.Content = product.Cpu;
            lb_sb_display.Content = product.Display;
            lb_sb_memory.Content = product.Memory + " GB";
            lb_sb_camera.Content = product.Camera + " MP";
            lb_sb_weight.Content = product.Weight + " g";
            lb_sb_mah.Content = product.MAh + " mAh";
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

        private void KeyDown_se_basket_enroll(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Btn_se_basket_enroll(sender, e);
        }
    }
}
