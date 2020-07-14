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
using System.Windows.Shapes;
using MBStore_MVC.Model;
using System.Threading;

namespace MBStore_MVC
{
    /// <summary>
    /// recommend.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Recommend : Window
    {
        MainWindow mainWindow;
        public Recommend()
        {
            InitializeComponent();
        }

        public void SetRecommend(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        private void Btn_Ok(object sender, RoutedEventArgs e)
        {
            if (tb_age.Text == "")
                MessageBox.Show("나이를 입력해주세요");
            else
            {
                using (StreamWriter outputFile = new StreamWriter(@"C:/Users/bit/Desktop/bit_project/python/recommend_input.txt"))
                {
                    outputFile.WriteLine(tb_age.Text);
                    outputFile.WriteLine(cb_gender.SelectedIndex);
                }
                string result_path = "C:/Users/bit/Desktop/bit_project/python/recommend_result.txt";
                string result_value;
                FileInfo fileInfo;
                List<Product> proList = new List<Product>();
                mbDB db = new mbDB();
                while (true)
                {
                    fileInfo = new FileInfo(result_path);
                    if (fileInfo.Exists)
                    {
                        Thread.Sleep(100);
                        string write_path = result_path;
                        result_value = File.ReadAllText(write_path);
                        fileInfo.Delete();
                        break;
                    }
                }
                proList = db.SelectProduct(int.Parse(result_value), 0, 0, 0, "", "", new DateTime(), "AND p.product_id = @Product_id");
                if (proList.Count != 0)
                {
                    mainWindow.lv_se_product_info.ItemsSource = proList;
                }
                this.Close();
            }
        }

        private void Btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
