using MBStore_MVC.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace MBStore_MVC
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void TempDelegate();
        public TempDelegate tempDelegate;
        string[] user;

        Timer _timer = null;
        string a;

        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(string str)
        {
            this.Show();
            user = str.Split('#');
            InitializeComponent();
            tb_main.Text = "환영합니다 " + user[0] + "["+user[1]+"] 님.";
            InitTimer();
        }
        private void InitTimer()
        {
            if (_timer != null)
                return;
            TimerCallback tcb = new TimerCallback(ThreadFunc);
            _timer = new Timer(tcb, null, 0, 1000);
        }

        public void ThreadFunc(Object stateInfo)
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                tempDelegate += new TempDelegate(SetTextBox);
                Dispatcher.Invoke(DispatcherPriority.Normal, tempDelegate);
            }
        }

        private void SetTextBox()
        {
            text.Text = DateTime.Now.ToString();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            this.Close();
        }

        private void btn_RR_Close(object sender, RoutedEventArgs e)
        {
            List<Customer> cus = new List<Customer>();
            for (int i = 0; i < lv_product_list.Items.Count; i++)
            {
                cus.Add((Customer)lv_product_list.Items[i]);
                MessageBox.Show(cus[i].Name + cus[i].Phone + cus[i].Gender);
            }            
        }

        private void btn_se_search(object sender, RoutedEventArgs e)
        {
            mbDB db = new mbDB();

            List<string> customer = db.SelectCustomer(tb_se_cusName.Text,cb_su_sex.Text,tb_se_phone.Text);
            List<Customer> items = new List<Customer>();
          

            foreach (string info in customer)
            {
                string[] temp = info.Split('#');

                items.Add(new Customer()
                {
                    Name = temp[0],
                    Gender = temp[1],
                    Date = temp[2],
                    Phone = temp[3],
                    Saving = temp[4]
                });                 
            }
            lv_product_list.ItemsSource = items;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = @"autoloing.txt";
            File.Delete(path);
            Login log = new Login();
            this.Hide();
            log.Show();          
        }

        private void btn_notice_Click(object sender, RoutedEventArgs e)
        {
            Notice noti = new Notice();
            noti.Show();
        }
    }
}
