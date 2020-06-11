using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// <summary>
    /// InputCustomer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InputCustomer : Window
    {
        MainWindow mainWindow;
        int userid;
        mbDB db;
        public InputCustomer()
        {
            InitializeComponent();
            db = new mbDB();
        }

        public void SetProduct(MainWindow mainWindow, int userid)
        {
            this.mainWindow = mainWindow;
            this.userid = userid;


            string str_total_price = mainWindow.la_se_sell_total_price.Content.ToString();
            str_total_price = str_total_price.Substring(0, str_total_price.Length - 2);
            long total_price = long.Parse(str_total_price, NumberStyles.AllowThousands);
            long saving = total_price / 100;
            tb_se_inputcus_saving.Text = string.Format("{0:#,##0}", saving);
        }
        private void Btn_se_basket_search(object sender, RoutedEventArgs e)
        {
            string phone_num = "";
            List<Customer> cusList = new List<Customer>();

            try
            {
                if (tb_se_basket_phone.Text != "" && tb_se_basket_phone.Text.Length == 4)
                    phone_num = tb_se_basket_phone.Text;
                else
                    throw new Exception();

                try
                {

                    cusList = db.SelectCustomer(phone_num);
                    if (cusList.Count != 0)
                        lv_se_basket_expect_cutom_info.ItemsSource = cusList;
                    else
                        MessageBox.Show("찾으시는 데이터가 없습니다");
                }
                catch
                {
                    MessageBox.Show("DB오류");
                }
            }
            catch
            {
                MessageBox.Show("잘못 입력하셨습니다");
            }
        }

        private void Btn_se_sell_enroll(object sender, RoutedEventArgs e)
        {
            Customer customer_info;
            try
            {
                customer_info = (Customer)lv_se_basket_expect_cutom_info.SelectedItems[0];

                if (MessageBox.Show("판매 하시겠습니까?", "알림창", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    List<Sell_Info> sell_list = new List<Sell_Info>();

                    try
                    {
                        for (int i = 0; i < mainWindow.lv_se_expect_sell.Items.Count; i++)
                        {
                            Sell_Info item = (Sell_Info)mainWindow.lv_se_expect_sell.Items[i];

                            sell_list.Add(item);
                            int stock = db.SelectStockProductStock(item.Stock_product);
                            if (item.Quantity > stock)
                            {
                                throw new Exception("재고가 부족합니다");
                            }
                        }

                        string str_saving_price = tb_se_inputcus_saving.Text;
                        long saving = long.Parse(str_saving_price, NumberStyles.AllowThousands);

                        db.sell_transaction(sell_list, customer_info.Id, userid, DateTime.Now, saving);

                        MessageBox.Show("판매가 완료 되었습니다", "알림창");
                        mainWindow.lv_se_expect_sell.ItemsSource = null;
                        mainWindow.la_se_sell_total_price.Content = "0 원";
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "재고")
                            MessageBox.Show("재고부족");
                        else
                            MessageBox.Show("오류");
                        mainWindow.lv_se_expect_sell.Items.Clear();
                    }
                    this.Close();
                }
            }
            catch
            {
                MessageBox.Show("고객을 선택 하세요");
            }
        
       }

        private void KeyDown_se_phoneNumber(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Btn_se_basket_search(sender, e);
        }
    }
}
