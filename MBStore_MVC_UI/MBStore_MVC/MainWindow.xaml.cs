using LiveCharts;
using LiveCharts.Wpf;
using MaterialDesignThemes.Wpf;
using MBStore_MVC.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using MaterialDesignColors;
using Microsoft.Win32;
using System.Drawing;
using Brushes = System.Windows.Media.Brushes;

namespace MBStore_MVC
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 차트
        SeriesCollection piechartData_1 = new SeriesCollection();
        SeriesCollection piechartData_2 = new SeriesCollection();
        SeriesCollection piechartData_3 = new SeriesCollection();
        public Func<ChartPoint, string> PointLabel { get; set; }
        public Func<ChartPoint, string> PointLabel1 { get; set; }
        public Func<ChartPoint, string> PointLabel2 { get; set; }

        public SeriesCollection Series { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        #endregion

        //타이머
        private delegate void TempDelegate();
        private TempDelegate tempDelegate;
        private Timer _timer = null;
        private string uri;
        private string http_uri;
        private string ftp_uri;

        public static Employee emp;
        public static Snackbar Snackbar;
        mbDB db = new mbDB();

        private int res_change_check = 0;
        private int res_change_check2 = 0;


        public MainWindow()
        {

        }
        public MainWindow(Employee employee) : base()
        {
            this.Show();
            InitializeComponent();
            InitTimer();
            uri = "20.41.81.89";
            http_uri = "http://" + uri;
            ftp_uri = "ftp://" + uri + ":21";
            emp = employee;
            img_main_emp.ImageSource = new BitmapImage(new Uri(@http_uri + "/employee/" + employee.Login_id + "_" + employee.Rank + "_" + employee.Name + ".JPG", UriKind.Absolute));
            text_myinfo_name.Text = emp.Name + " [" + emp.Rank + "]";
            lb_user.Content = "[" + emp.Rank + "]" + " " + emp.Name;
            #region 통계자료 초기화
            PointLabel = chartPoint =>
                   string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            PointLabel1 = chartPoint1 =>
                    string.Format("{0} ({1:P})", chartPoint1.Y, chartPoint1.Participation);

            PointLabel2 = chartPoint2 =>
                    string.Format("{0} ({1:P})", chartPoint2.Y, chartPoint2.Participation);

            pieChart1.Foreground = Brushes.Black;
            pieChart2.Foreground = Brushes.Black;
            pieChart3.Foreground = Brushes.Black;
            barChart.Foreground = Brushes.Black;

            barChart.Series = new SeriesCollection { };
            Setting_Chart();

            Labels = new[] { "Jan.", "Feb.", "Mar.", "Apr.", "May", "Jun.", "Jul.", "Aug.", "Sep.", "Oct.", "Nov.", "Dec." };
            DataContext = this;
            #endregion

            #region 스낵메세지

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
            }).ContinueWith(t =>
            {
                sb_main.MessageQueue.Enqueue("환영합니다 " + emp.Name + " [" + emp.Rank + "] 님.");
            }, TaskScheduler.FromCurrentSynchronizationContext());
            sb_main.DataContext = sb_main.MessageQueue;

            Snackbar = this.sb_main;

            #endregion

            #region UI_ColorTool + 테마 불러오기

            UI_ColorTool color = new UI_ColorTool();
            ti_setting.Content = color;





            #endregion

            #region MyInfo 불러오기

            tb_myinfo_address.Text = emp.Post_number + "/" + emp.Address;
            tb_myinfo_email.Text = emp.Email;
            tb_myinfo_phone.Text = emp.Phone;

            #endregion
        }

        #region 타이머
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
            text.Text = DateTime.Now.ToString("yyyy/MM/dd");
        }
        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            this.Close();
        }


        //로그아웃 버튼
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = @"autoloing.txt";
            File.Delete(path);
            Login log = new Login();
            this.Visibility = Visibility.Collapsed;
            log.ShowDialog();

        }

        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }

        #region 파일 업로드
        private void FtpUploadFile(string filename, string to_uri)
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request =
                (FtpWebRequest)WebRequest.Create(to_uri);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UsePassive = false;
            // Get network credentials.
            request.Credentials =
                new NetworkCredential("ftp_bit", "dlqtkgkwk12#$");

            // Read the file's contents into a byte array.
            byte[] bytes = System.IO.File.ReadAllBytes(filename);

            // Write the bytes into the request stream.
            request.ContentLength = bytes.Length;
            using (Stream request_stream = request.GetRequestStream())
            {
                request_stream.Write(bytes, 0, bytes.Length);
            }
        }
        #endregion
        #region ListView 헤더클릭 정렬
        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        private void gv_se_head_click(object sender, RoutedEventArgs e)
        {
            ListView listview;
            DependencyObject present = VisualTreeHelper.GetParent((DependencyObject)sender);
            try
            {
                while (true)
                {
                    if (present.GetType().Name == "ListView")
                        break;
                    present = VisualTreeHelper.GetParent((DependencyObject)present);
                }
            }
            catch (Exception ex)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "오류 : " + ex.Message }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }

            listview = (ListView)present;
            if (listview.Items.Count != 0)
            {
                var headerClicked = e.OriginalSource as GridViewColumnHeader;
                ListSortDirection direction;

                if (headerClicked != null)
                {
                    if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                    {
                        if (headerClicked != _lastHeaderClicked)
                        {
                            direction = ListSortDirection.Ascending;
                        }
                        else
                        {
                            if (_lastDirection == ListSortDirection.Ascending)
                            {
                                direction = ListSortDirection.Descending;
                            }
                            else
                            {
                                direction = ListSortDirection.Ascending;
                            }
                        }

                        var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                        var sortBy = columnBinding?.Path.Path as string;
                        if (sortBy == null)
                        {
                            if (headerClicked.Content.ToString() == "색상")
                                sortBy = "Color";
                        }
                        Sort(sortBy, direction, listview);

                        if (direction == ListSortDirection.Ascending)
                        {
                            headerClicked.Column.HeaderTemplate =
                              Resources["HeaderTemplateArrowUp"] as DataTemplate;
                        }
                        else
                        {
                            headerClicked.Column.HeaderTemplate =
                              Resources["HeaderTemplateArrowDown"] as DataTemplate;
                        }

                        // Remove arrow from previously sorted header
                        if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                        {
                            _lastHeaderClicked.Column.HeaderTemplate = null;
                        }

                        _lastHeaderClicked = headerClicked;
                        _lastDirection = direction;
                    }
                }
            }
        }
        private void Sort(string sortBy, ListSortDirection direction, ListView listview)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(listview.ItemsSource);

            dataView.SortDescriptions.Clear();

            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
        #endregion

        #region 판매팀
        #region 물품조회
        private void Btn_se_Search(object sender, RoutedEventArgs e)
        {
            int product_id = 0, memory = 0, price = 0, ram = 0;
            string product_name = "", brand = "";
            DateTime manufacture = new DateTime();
            string query = "";
            List<Product> proList = new List<Product>();

            try
            {
                if (cb_se_product_id.Text != "")
                {
                    product_id = int.Parse(cb_se_product_id.Text);
                    query += " AND p.product_id = @Product_id";
                }

                if (cb_se_memory.SelectedIndex.ToString() != "-1")
                {
                    memory = int.Parse(cb_se_memory.Text);
                    query += " AND memory = @Memory";
                }

                if (tb_se_price.Text != "")
                {
                    price = int.Parse(tb_se_price.Text);
                    if (btn_se_price.Content.Equals("▲"))
                        query += " AND price >= @Price";
                    else
                        query += " AND price <= @Price";
                }

                if (cb_se_ram.Text != "")
                {
                    ram = int.Parse(cb_se_ram.Text);
                    query += " AND ram = @Ram";
                }

                if (cb_se_proudct_name.Text != "")
                {
                    product_name = cb_se_proudct_name.Text;
                    query += " AND name Like @Name";
                }

                if (cb_se_brand.Text != "")
                {
                    brand = cb_se_brand.Text;
                    query += " AND brand Like @Brand";
                }
                if (dtp_se_manufac.SelectedDate.HasValue)
                {
                    manufacture = dtp_se_manufac.SelectedDate.Value;
                    if (btn_se_manufacture.Content.Equals("▲"))
                        query += " AND manufacture >= @Manufacture";
                    else
                        query += " AND manufacture <= @Manufacture";
                }
                else
                    manufacture = Convert.ToDateTime("0001/01/01");
            }
            catch
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "입력이 잘못됐습니다." }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }

            Btn_se_Reset(sender, e);

            try
            {
                proList = db.SelectProduct(product_id, memory, price, ram, product_name, brand, manufacture, query);
                if (proList.Count != 0)
                {
                    //proList.Sort((x, y) => String.Compare(x.Color, y.Color)); //정렬
                    lv_se_product_info.ItemsSource = proList;
                }
                else
                {
                    var MessageDialog = new MessageDialog
                    {
                        Message = { Text = "찾으시는 데이터가 없습니다." }
                    };
                    DialogHost.Show(MessageDialog, "RootDialog");
                }
            }
            catch
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "DB 에러!!" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }

        #region ComboBox DropDownOpened
        private void cb_se_brand_DropDownOpened(object sender, EventArgs e)
        {
            List<string> brandList;

            brandList = db.SelectStockBrand();
            cb_se_brand.ItemsSource = brandList;
        }

        private void cb_se_proudct_name_DropDownOpened(object sender, EventArgs e)
        {
            List<string> productList;

            productList = db.SelectStockName();
            cb_se_proudct_name.ItemsSource = productList;
        }

        private void cb_se_ram_DropDownOpened(object sender, EventArgs e)
        {
            List<int> ramList;
            ramList = db.SelectStockRam();
            cb_se_ram.ItemsSource = ramList;
        }

        private void cb_se_memory_DropDownOpened(object sender, EventArgs e)
        {
            List<int> memoryList;
            memoryList = db.SelectStockMemory();
            cb_se_memory.ItemsSource = memoryList;
        }

        private void cb_se_product_id_DropDownOpened(object sender, EventArgs e)
        {
            List<int> IdList;
            IdList = db.SelectStockProductId();
            cb_se_product_id.ItemsSource = IdList;
        }
        private void cb_se_history_product_name_DropDownOpened(object sender, EventArgs e)
        {
            List<string> NameList;
            NameList = db.SelectSalesname();
            cb_se_histroy_name.ItemsSource = NameList;
        }
        #endregion

        private void Btn_se_Reset(object sender, RoutedEventArgs e)
        {
            cb_se_product_id.Text = "";
            cb_se_proudct_name.Text = "";
            cb_se_brand.Text = "";
            cb_se_memory.Text = "";
            tb_se_price.Text = "";
            dtp_se_manufac.SelectedDate = DateTime.Today;
            dtp_se_manufac.Text = "";
            cb_se_ram.Text = "";
            btn_se_manufacture.Content = "▲";
            btn_se_price.Content = "▲";

            lv_se_product_info.ItemsSource = null;
        }

        private void Btn_se_recommend(object sender, RoutedEventArgs e)
        {
            Recommend recommend = new Recommend();
            recommend.SetRecommend(this);
            recommend.ShowDialog();
        }

        private void Btn_se_price(object sender, RoutedEventArgs e)
        {
            if (btn_se_price.Content.Equals("▲"))
                btn_se_price.Content = "▼";
            else
                btn_se_price.Content = "▲";
        }

        private void Btn_se_manufacture(object sender, RoutedEventArgs e)
        {
            if (btn_se_manufacture.Content.Equals("▲"))
                btn_se_manufacture.Content = "▼";
            else
                btn_se_manufacture.Content = "▲";
        }

        private void KeyDown_se_search_product(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Btn_se_Search(sender, e);
        }

        private void Lv_se_item_product_info_doubleClick(object sender, MouseButtonEventArgs e)
        {
            Product product = new Product();
            product = (Product)lv_se_product_info.SelectedItems[0];

            ShoppingBasket shoppingBasket_w = new ShoppingBasket();
            shoppingBasket_w.SetProduct(product, this, http_uri);
            shoppingBasket_w.ShowDialog();
        }
        #endregion


        #region 상품판매
        public void Set_Sell_listview(Product product, int quantity)
        {
            List<Sell_Info> sell_Info_list = new List<Sell_Info>();
            bool duple = false;
            for (int i = 0; i < lv_se_expect_sell.Items.Count; i++)
            {
                Sell_Info sell_Info = (Sell_Info)lv_se_expect_sell.Items[i];
                sell_Info_list.Add(sell_Info);
            }
            for (int i = 0; i < sell_Info_list.Count; i++)
            {
                if (sell_Info_list[i].Stock_product == product.Stock_product)
                {
                    sell_Info_list[i].Quantity += quantity;
                    sell_Info_list[i].Total_price = sell_Info_list[i].Quantity * product.Price;
                    duple = true;
                    break;
                }
            }

            if (!duple)
            {
                sell_Info_list.Add(new Sell_Info()
                {
                    Stock_product = product.Stock_product,
                    Product_id = product.Product_id,
                    Product_name = product.Name,
                    Color = product.Color,
                    ColorValue = product.ColorValue,
                    Quantity = quantity,
                    Total_price = product.Price * quantity
                });
            }
            lv_se_expect_sell.ItemsSource = sell_Info_list;

            string str_total_price = la_se_sell_total_price.Content.ToString();
            str_total_price = str_total_price.Substring(0, str_total_price.Length - 2);
            long total_price = long.Parse(str_total_price, NumberStyles.AllowThousands);
            total_price += product.Price * quantity;
            la_se_sell_total_price.Content = string.Format("{0:#,##0}", total_price) + " 원";
        }

        private void Btn_se_sell_list_remove(object sender, RoutedEventArgs e)
        {
            lv_se_expect_sell.ItemsSource = null;
            la_se_sell_total_price.Content = "0 원";
        }

        private void Btn_se_sell(object sender, RoutedEventArgs e)
        {
            if (lv_se_expect_sell.Items.Count != 0)
            {
                InputCustomer inputCustomer = new InputCustomer();
                inputCustomer.SetProduct(this, emp.Employee_id);
                inputCustomer.ShowDialog();
            }
        }

        #endregion


        #region 판매내역조회

        private void Btn_se_Salse_Search_history(object sender, RoutedEventArgs e)
        {
            string customer_name = "", employee_name = "", sales_type = "";
            DateTime sales_s_date = new DateTime();
            DateTime sales_e_date = new DateTime();
            string query = "";
            string product_name = "";
            List<Sell_Info> sellinfoList = new List<Sell_Info>();

            try
            {
                if (tb_se_customer_name.Text != "")
                {
                    customer_name = tb_se_customer_name.Text;
                    query += " AND c.name = @Customer_name";
                }

                if (tb_se_employee_name.Text != "")
                {
                    employee_name = tb_se_employee_name.Text;
                    query += " AND e.name = @Employee_name";
                }

                if (cb_se_type.SelectedIndex.ToString() != "-1" && cb_se_type.SelectedIndex.ToString() != "0")
                {
                    sales_type = cb_se_type.Text;
                    query += " AND sp.type = @Type";
                }

                if (dtp_se_sold_s_date.SelectedDate.HasValue)
                {
                    sales_s_date = dtp_se_sold_s_date.SelectedDate.Value;

                    query += " AND sh.sales_date >= @Sales_s_date";
                }
                else
                    sales_s_date = Convert.ToDateTime("0001/01/01");

                if (dtp_se_sold_e_date.SelectedDate.HasValue)
                {
                    sales_e_date = dtp_se_sold_e_date.SelectedDate.Value;

                    query += " AND sh.sales_date <= @Sales_e_date";
                }
                else
                    sales_e_date = Convert.ToDateTime("0001/01/01");
                if (cb_se_histroy_name.Text != "")
                {
                    product_name = cb_se_histroy_name.Text;
                    query += " AND p.name = @Product_name";
                }

            }
            catch
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "잘못 입력했습니다" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }

            Btn_se_Salse_Reset_history(sender, e);

            try
            {
                if (query != "")    //안 비어있을때
                {
                    query = query.Substring(5);
                    query = "WHERE " + query;
                }
                sellinfoList = db.SelectSalesHistory(customer_name, employee_name, sales_type, sales_s_date, sales_e_date, -1, product_name, query);
                if (sellinfoList.Count != 0)
                {
                    //proList.Sort((x, y) => String.Compare(x.Color, y.Color)); //정렬
                    lv_se_sales_history.ItemsSource = sellinfoList;
                }
                else
                {
                    var MessageDialog = new MessageDialog
                    {
                        Message = { Text = "찾으시는 데이터가 없습니다" }
                    };
                    DialogHost.Show(MessageDialog, "RootDialog");
                }
            }
            catch
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "DB 에러!!" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }

        private void Btn_se_Salse_Reset_history(object sender, RoutedEventArgs e)
        {
            tb_se_customer_name.Text = "";
            tb_se_employee_name.Text = "";
            cb_se_type.Text = "모두";
            dtp_se_sold_s_date.SelectedDate = DateTime.Today;
            dtp_se_sold_e_date.SelectedDate = DateTime.Today;
            dtp_se_sold_e_date.Text = "";
            dtp_se_sold_s_date.Text = "";
            cb_se_histroy_name.Text = "";

            lv_se_sales_history.ItemsSource = null;
        }

        private void KeyDown_se_search_history(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Btn_se_Salse_Search_history(sender, e);
        }

        private void Lv_se_sales_history_doubleClick(object sender, MouseButtonEventArgs e)
        {
            Sell_Info sell_info = new Sell_Info();
            sell_info = (Sell_Info)lv_se_sales_history.SelectedItems[0];
            bool refunded = false;

            try
            {
                refunded = db.SelectSalesHistoryRefunded(sell_info.Sales_history_id);
            }
            catch
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "DB 에러!!" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }

            if (sell_info.Sales_date.AddDays(7) >= DateTime.Now && sell_info.Sales_type == "판매" && !refunded)
            {
                if (MessageBox.Show("환불 목록에 추가하시겠습니까?", "알림창", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    List<Sell_Info> refundList = new List<Sell_Info>();
                    string query = "WHERE sh.sales_history_id = @Sales_hisory_id";
                    try
                    {
                        long total_price = 0;

                        refundList = db.SelectSalesHistory("", "", "", DateTime.Now, DateTime.Now, sell_info.Sales_history_id, "", query);
                        lv_se_expect_refund.ItemsSource = refundList;

                        for (int i = 0; i < refundList.Count; i++)
                            total_price += refundList[i].Total_price;

                        la_se_refund_total_price.Content = string.Format("{0:#,##0}", total_price) + " 원";
                    }
                    catch
                    {
                        var MessageDialog = new MessageDialog
                        {
                            Message = { Text = "DB 에러!!" }
                        };
                        DialogHost.Show(MessageDialog, "RootDialog");
                    }
                }
            }
            else
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "환불이 불가능한 제품입니다" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }

        private void Btn_se_refund_list_remove(object sender, RoutedEventArgs e)
        {
            lv_se_expect_refund.ItemsSource = null;
            la_se_refund_total_price.Content = "0 원";
        }

        private void Btn_se_refund(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("환불 하시겠습니까?", "알림창", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                List<Sell_Info> refund_list = new List<Sell_Info>();

                for (int i = 0; i < lv_se_expect_refund.Items.Count; i++)
                {
                    refund_list.Add((Sell_Info)lv_se_expect_refund.Items[i]);
                }

                try
                {
                    List<Sell_Info> list_sell_info = db.SelectHistoryTotalPrice(refund_list[0].Sales_history_id);
                    long savings = 0;
                    for (int i = 0; i < list_sell_info.Count; i++)
                    {
                        savings += list_sell_info[i].Total_price;
                    }
                    savings /= 100;
                    db.transaction_refund(refund_list[0].Sales_history_id, refund_list[0].Customer_id, emp.Employee_id, DateTime.Now, refund_list, list_sell_info[0].Customer_id, savings);

                    var MessageDialog = new MessageDialog
                    {
                        Message = { Text = "환불이 완료 되었습니다" }
                    };
                    DialogHost.Show(MessageDialog, "RootDialog");
                    Btn_se_refund_list_remove(sender, e);
                }
                catch
                {
                    var MessageDialog = new MessageDialog
                    {
                        Message = { Text = "DB 에러!!" }
                    };
                    DialogHost.Show(MessageDialog, "RootDialog");
                    lv_se_expect_refund.ItemsSource = null;
                }
            }

        }
        #endregion


        #region 고객관리

        string qr2 = string.Empty;
        int and_ck2 = 0;

        private void Check_and2(int cnt)
        {
            cnt++;
            and_ck2 = cnt;
            if (and_ck2 > 1) qr2 += " and";
        }

        private void Btn_se_cus_register(object sender, RoutedEventArgs e)
        {
            string gen = string.Empty;
            if (res_change_check2 == 0 && tb_se_cus_search_name.Text != "" && (rb_su_em_gender32.IsChecked != false || rb_su_em_gender42.IsChecked != false) && tb_su_cus_search_phone2.Text != "")//신규등록
            {
                try
                {
                    if (rb_su_em_gender32.IsChecked == false && rb_su_em_gender42.IsChecked == true)
                    { gen = "여성"; }
                    else if (rb_su_em_gender32.IsChecked == true && rb_su_em_gender42.IsChecked == false)
                    { gen = "남성"; }

                    if (tb_su_cus_search_saving2.Text == "")
                    {
                        tb_su_cus_search_saving2.Text = "0";
                    }

                    db.Insert_Cus_Info(tb_se_cus_search_name.Text, gen, dtp_su_cus_search_birth2.SelectedDate
                        , tb_su_cus_search_phone2.Text, long.Parse(tb_su_cus_search_saving2.Text));
                    su_cus_All_Clear2();
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(500);
                    }).ContinueWith(t =>
                    {
                        sb_main.MessageQueue.Enqueue("신규고객 등록이 완료됐습니다.");
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    sb_main.DataContext = sb_main.MessageQueue;

                    Snackbar = this.sb_main;
                }
                catch
                {
                    var MessageDialog = new MessageDialog
                    {
                        Message = { Text = "입력되지 않은 값이 있습니다" }
                    };
                    DialogHost.Show(MessageDialog, "RootDialog");
                }
            }
            else if (res_change_check2 == 1 && tb_se_cus_search_name.Text != "" && (rb_su_em_gender32.IsChecked != false || rb_su_em_gender42.IsChecked != false) && tb_su_cus_search_phone2.Text != "")
            {
                try
                {
                    if (rb_su_em_gender32.IsChecked == false && rb_su_em_gender42.IsChecked == true)
                    { gen = "여성"; }
                    else if (rb_su_em_gender32.IsChecked == true && rb_su_em_gender42.IsChecked == false)
                    { gen = "남성"; }

                    db.Update_Cus_Info(int.Parse(tb_se_cus_search_cus_id.Text), tb_se_cus_search_name.Text, gen, dtp_su_cus_search_birth2.SelectedDate
                        , tb_su_cus_search_phone2.Text, long.Parse(tb_su_cus_search_saving2.Text));
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(500);
                    }).ContinueWith(t =>
                    {
                        sb_main.MessageQueue.Enqueue("신규고객 등록이 완료됐습니다.");
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    sb_main.DataContext = sb_main.MessageQueue;

                    Snackbar = this.sb_main;
                    su_cus_All_Clear2();
                }
                catch
                {
                    var MessageDialog = new MessageDialog
                    {
                        Message = { Text = "입력되지 않은 값이 있습니다" }
                    };
                    DialogHost.Show(MessageDialog, "RootDialog");
                }
            }
            else
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "입력되지 않은 값이 있습니다" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }


        private void Btn_se_cus_reset(object sender, RoutedEventArgs e)
        {
            {
                Customer customer = new Customer();
                lable_first2.Content = "신규등록";
                btn_se_cus_search_res.Content = "등록";
                dtp_su_cus_search_birth2.SelectedDate = DateTime.Today;
                su_cus_All_Clear2();
                res_change_check2 = 0;
                customer = db.Get_Cus_Id();
                tb_se_cus_search_cus_id.Text = (customer.Id + 1).ToString();
                label_cus_id2.Visibility = Visibility.Hidden;
                tb_se_cus_search_cus_id.Visibility = Visibility.Hidden;
            }
        }

        private void btn_se_cu_search(object sender, RoutedEventArgs e)
        {

            List<Customer> customers;
            string ap = string.Empty;

            if (tb_se_cusName2.Text != "")
            {
                Check_and2(and_ck2);
                qr2 += " name like @name";
            }
            if (cb_su_sex2.Text != "모두")
            {
                Check_and2(and_ck2);
                qr2 += " gender=@gender";
            }
            if (tb_se_phone2.Text != "")
            {
                Check_and2(and_ck2);
                qr2 += " phone like @phone";
            }

            if (qr2 != "")
                ap = " where" + qr2;

            try
            {
                customers = db.GetList_Customer_Search(tb_se_cusName2.Text, cb_su_sex2.Text, tb_se_phone2.Text, ap); //데이터 바인딩 & 전체출력
                lv_cus_search2.ItemsSource = customers;
                tb_se_cusName2.Clear();
                cb_su_sex2.SelectedIndex = 0;
                tb_se_phone2.Clear();
            }
            catch
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "DB 에러!!" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
            qr2 = "";
            and_ck2 = 0;
        }

        void su_cus_All_Clear2()
        {
            tb_se_cus_search_cus_id.Text = "";
            tb_se_cus_search_name.Text = "";
            tb_su_cus_search_phone2.Text = "";
            tb_su_cus_search_saving2.Text = "";
            rb_su_em_gender32.IsChecked = false;
            rb_su_em_gender42.IsChecked = false;
            dtp_su_cus_search_birth2.Text = "";
        }

        private void Lv_se_item_cus_info_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Customer customer = new Customer();
            lable_first2.Content = "정보변경";
            btn_se_cus_search_res.Content = "변경";
            customer = ((Customer)lv_cus_search2.SelectedItems[0]);
            tb_se_cus_search_cus_id.Text = customer.Id.ToString();
            tb_se_cus_search_name.Text = customer.Name;

            if (customer.Gender == "남성")
            {
                rb_su_em_gender32.IsChecked = true;
                rb_su_em_gender42.IsChecked = false;
            }
            else
            {
                rb_su_em_gender32.IsChecked = false;
                rb_su_em_gender42.IsChecked = true;
            }
            tb_su_cus_search_phone2.Text = customer.Phone;
            dtp_su_cus_search_birth2.Text = customer.Date.ToString();
            tb_su_cus_search_saving2.Text = customer.Savings.ToString();
            res_change_check2 = 1;
            label_cus_id2.Visibility = Visibility.Visible;
            tb_se_cus_search_cus_id.Visibility = Visibility.Visible;
        }
        #endregion

        #endregion

        #region 물류팀
        //함수 이름예시
        //_Lo_, _lo_ : 물류팀 (Logistics)
        //_Reg_, _reg_ : 제품등록 영역
        //_Input_, _input_ : 입고 영역(Input)
        //_Pse_, _pse_ : 제품 조회 영역(Product Search)
        //_Rse_, _rse_ : 내역 조회 영역(Receipt Search)
        //_refund_ : 반품 영역

        //여기부터 시작
        int plusStock; //클래스 변수


        #region 제품등록
        //제품등록 : 조회버튼
        private void btn_lo_reg_search_Click(object sender, RoutedEventArgs e)
        {
            List<Product> product;
            try
            {
                product = db.Get_Lo_Reg_RegistProductList();
                lv_lo_reg_productList.ItemsSource = product;
            }
            catch (Exception ex)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = ex.Message }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
            if (lv_lo_reg_productList.Items.Count == 0)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "데이터가 존재하지 않습니다" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }
        //제품등록 : 등록버튼
        private void btn_lo_reg_regist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //비어있는 항목이 없는지 검사
                if (tb_lo_reg_objectName.Text != "" && dp_lo_reg_inputDate.SelectedDate.Value.ToString() != "" && tb_lo_reg_objectCPU.Text != "" && tb_lo_reg_objectInch.Text != "" && tb_lo_reg_objectmAh.Text != "" && tb_lo_reg_objectRAM.Text != "" && tb_lo_reg_objectBrand.Text != "" && tb_lo_reg_objectCamera.Text != "" && tb_lo_reg_objectWeight.Text != "" && tb_lo_reg_objectPrice.Text != "" && tb_lo_reg_objectDisplay.Text != "" && tb_lo_reg_objectMemory.Text != "")
                {
                    if (Convert.ToInt32(tb_lo_reg_objectmAh.Text) > 0 && Convert.ToInt32(tb_lo_reg_objectRAM.Text) > 0 && Convert.ToInt32(tb_lo_reg_objectCamera.Text) > 0 && Convert.ToInt32(tb_lo_reg_objectWeight.Text) > 0 && Convert.ToInt32(tb_lo_reg_objectPrice.Text) > 0 && Convert.ToInt32(tb_lo_reg_objectMemory.Text) > 0)
                    {
                        List<string> checkOverlap = db.Check_Lo_Reg_Overlap();
                        bool check = false;

                        for (int i = 0; i < checkOverlap.Count; i++)
                        {
                            if (checkOverlap[i] == tb_lo_reg_objectName.Text)
                                check = true;
                        }

                        if (!check)
                        {
                            if (MessageBox.Show("등록하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                db.Add_Lo_Reg_Product(tb_lo_reg_objectName.Text, DateTime.Parse(dp_lo_reg_inputDate.SelectedDate.Value.ToString()), tb_lo_reg_objectCPU.Text, tb_lo_reg_objectInch.Text, Int16.Parse(tb_lo_reg_objectmAh.Text), Int16.Parse(tb_lo_reg_objectRAM.Text), tb_lo_reg_objectBrand.Text, Int16.Parse(tb_lo_reg_objectCamera.Text), Int16.Parse(tb_lo_reg_objectWeight.Text), Int64.Parse(tb_lo_reg_objectPrice.Text), tb_lo_reg_objectDisplay.Text, Int16.Parse(tb_lo_reg_objectMemory.Text));

                                tb_lo_reg_objectName.Text = "";
                                dp_lo_reg_inputDate.SelectedDate = DateTime.Now;
                                dp_lo_reg_inputDate.SelectedDate = null;
                                tb_lo_reg_objectCPU.Text = "";
                                tb_lo_reg_objectInch.Text = "";
                                tb_lo_reg_objectmAh.Text = "";
                                tb_lo_reg_objectRAM.Text = "";
                                tb_lo_reg_objectBrand.Text = "";
                                tb_lo_reg_objectCamera.Text = "";
                                tb_lo_reg_objectWeight.Text = "";
                                tb_lo_reg_objectPrice.Text = "";
                                tb_lo_reg_objectDisplay.Text = "";
                                tb_lo_reg_objectMemory.Text = "";
                            }
                        }
                        else if (check)
                            MessageBox.Show("이미 존재하는 제품입니다");
                    }
                }
                else
                {
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(1000);
                    }).ContinueWith(t =>
                    {
                        sb_main.MessageQueue.Enqueue("입력이 완료되지 않았습니다.");
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    sb_main.DataContext = sb_main.MessageQueue;

                    Snackbar = this.sb_main;
                }
            }
            catch (Exception ex)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = ex.Message }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }

        private void btn_lo_upload_img_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                tb_lo_reg_img.Text = filename;
            }
        }

        #endregion

        #region 제품조회
        //제품조회 : 검색버튼
        private void btn_lo_pse_search_Click(object sender, RoutedEventArgs e)
        {
            int stock_product = 0, product_id = 0;
            string product_name = "", color = "", query = "";
            List<Product> proList = new List<Product>();

            try
            {
                if (tb_lo_pse_tradeID.Text != "")
                {
                    stock_product = int.Parse(tb_lo_pse_tradeID.Text);
                    query += " AND sp.stock_product = @Stock_product";
                }

                if (tb_lo_pse_productID.Text != "")
                {
                    product_id = int.Parse(tb_lo_pse_productID.Text);
                    query += " AND sp.product_id = @Product_id";
                }

                if (tb_lo_pse_productName.Text != "")
                {
                    product_name = tb_lo_pse_productName.Text;
                    query += " AND name LIKE @Name";
                }

                if (tb_lo_pse_color.Text != "")
                {
                    color = tb_lo_pse_color.Text;
                    query += " AND color LIKE @Color";
                }
            }
            catch
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "입력이 잘못됐습니다" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }

            try
            {
                if (query != "")
                    query = " where stock > 0 " + query;
                else if (query == "")
                    query = " where stock > 0";

                proList = db.Select_Lo_Pse_Product(stock_product, product_id, product_name, color, query);
                if (proList.Count != 0)
                {
                    lv_lo_pse_productList.ItemsSource = proList;
                }
                else
                {
                    var MessageDialog = new MessageDialog
                    {
                        Message = { Text = "찾으시는 데이터가 없습니다" }
                    };
                    DialogHost.Show(MessageDialog, "RootDialog");
                }
            }
            catch (Exception ex)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = ex.Message }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }
        //제품조회 : 리셋버튼
        private void btn_lo_pse_reset_Click(object sender, RoutedEventArgs e)
        {
            tb_lo_pse_tradeID.Text = "";
            tb_lo_pse_productName.Text = "";
            tb_lo_pse_color.Text = "";
        }

        #endregion

        #region 내역조회
        //내역조회 : 조회버튼
        private void btn_lo_rse_search_Click(object sender, RoutedEventArgs e)
        {
            List<Product> product;
            int productid = 0, tradehistoryID = 0;
            string query = "", tradetype = "", color = "", name = "";
            DateTime startDay = new DateTime();
            DateTime endDay = new DateTime();

            try
            {
                if (tb_lo_rse_productID.Text != "")
                {
                    productid = int.Parse(tb_lo_rse_productID.Text);
                    query += " and tp.product_id=@Product_id";
                }

                if (cb_lo_rse_inOutput.SelectedIndex.ToString() == "1")
                {
                    tradetype = "입고";
                    query += " and trade_type=@Trade_type";
                }
                else if (cb_lo_rse_inOutput.SelectedIndex.ToString() == "2")
                {
                    tradetype = "반품";
                    query += " and trade_type=@Trade_type";
                }

                if (tb_lo_rse_tradeHistoryID.Text != "")
                {
                    tradehistoryID = int.Parse(tb_lo_rse_tradeHistoryID.Text);
                    query += " and tp.trade_history_id=@Trade_history_id";
                }

                if (tb_lo_rse_color.Text != "")
                {
                    color = tb_lo_rse_color.Text;
                    query += " and tp.color like @Color";
                }

                if (tb_lo_rse_productName.Text != "")
                {
                    name = tb_lo_rse_productName.Text;
                    query += " and p.name like @Name";
                }

                if (dp_lo_rse_startDate.SelectedDate.ToString() != "" && dp_lo_rse_endDate.SelectedDate.ToString() == "")
                {
                    startDay = dp_lo_rse_startDate.SelectedDate.Value;
                    query += " and th.trade_date >= @Start_date";
                }
                else if (dp_lo_rse_startDate.SelectedDate.ToString() == "" && dp_lo_rse_endDate.SelectedDate.ToString() != "")
                {
                    endDay = dp_lo_rse_endDate.SelectedDate.Value;
                    query += " and th.trade_date <= @End_date";
                }
                else if (dp_lo_rse_startDate.SelectedDate.ToString() != "" && dp_lo_rse_endDate.SelectedDate.ToString() != "")
                {
                    startDay = dp_lo_rse_startDate.SelectedDate.Value;
                    endDay = dp_lo_rse_endDate.SelectedDate.Value;
                    query += " and th.trade_date >= @Start_date and th.trade_date <= @End_date";
                }

                product = db.Get_Lo_Rse_ProductList(productid, tradetype, tradehistoryID, color, name, startDay, endDay, query);
                lv_lo_rse_productList.ItemsSource = product;
                cb_lo_rse_inOutput.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = ex.Message }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
            if (lv_lo_rse_productList.Items.Count == 0)
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                }).ContinueWith(t =>
                {
                    sb_main.MessageQueue.Enqueue("데이터가 존재하지 않습니다");
                }, TaskScheduler.FromCurrentSynchronizationContext());
                sb_main.DataContext = sb_main.MessageQueue;

                Snackbar = this.sb_main;
            }
        }
        //내역조회 : 초기화버튼
        private void btn_lo_rse_delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tb_lo_rse_color.Text = "";
                tb_lo_rse_productID.Text = "";
                tb_lo_rse_productName.Text = "";
                tb_lo_rse_tradeHistoryID.Text = "";
                cb_lo_rse_inOutput.SelectedIndex = 0;
                dp_lo_rse_startDate.SelectedDate = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-01");

                dp_lo_rse_endDate.SelectedDate = DateTime.Today;
            }
            catch (Exception ex)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = ex.Message }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }
        //내역조회 : 시작날짜 선택 시 범위제한
        private void dp_lo_rse_startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dp_lo_rse_endDate.DisplayDateStart = dp_lo_rse_startDate.SelectedDate.Value;
        }
        //내역조회 : 마지막날짜 선택 시 범위제한
        private void dp_lo_rse_endDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            dp_lo_rse_startDate.DisplayDateEnd = dp_lo_rse_endDate.SelectedDate.Value;
        }
        #endregion

        #region 입고
        //입고 : 조회버튼
        private void btn_lo_input_search_Click(object sender, RoutedEventArgs e)
        {
            List<Product> product;
            try
            {
                product = db.Get_Lo_Reg_RegistProductList();
                lv_lo_input_objectList.ItemsSource = product;
                tb_lo_input_employeeID.Text = emp.Employee_id.ToString();
                cb_lo_input_inOutput.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = ex.Message }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
            if (lv_lo_input_objectList.Items.Count == 0)
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                }).ContinueWith(t =>
                {
                    sb_main.MessageQueue.Enqueue("데이터가 존재하지 않습니다");
                }, TaskScheduler.FromCurrentSynchronizationContext());
                sb_main.DataContext = sb_main.MessageQueue;

                Snackbar = this.sb_main;
            }
        }
        //입고 : DropDownOpend, SelectionChanged
        //물품번호
        private void cb_lo_input_productNumber_DropDownOpened(object sender, EventArgs e)
        {
            List<Int32> productdata;

            productdata = db.Get_Lo_Input_ProductNumList();
            cb_lo_input_productNumber.ItemsSource = productdata;
        }
        private void cb_lo_productIdChanged(object sender, EventArgs e)
        {
            cb_lo_input_color.IsEditable = true;
            cb_lo_input_color.Text = "";
            tb_lo_input_rgb.Text = "";

            ComboBox combo = sender as ComboBox;
            if (combo.SelectedValue != null)
            {
                string selected_id = combo.SelectedValue.ToString();

                List<Product> productdata;
                productdata = db.Get_Lo_Input_ProductColor(int.Parse(selected_id));

                List<string> ColorList = new List<string>();
                for (int i = 0; i < productdata.Count; i++)
                {
                    ColorList.Add(productdata[i].Color);
                }

                cb_lo_input_color.ItemsSource = ColorList;

                if (productdata.Count == 0)
                {
                    tb_lo_input_rgb.IsReadOnly = false;
                }
                else
                {
                    tb_lo_input_rgb.IsReadOnly = true;
                }
            }
        }

        //물품색상
        private void cb_lo_productColorChanged(object sender, EventArgs e)
        {
            tb_lo_input_rgb.IsReadOnly = false;

            for (int i = 0; i < cb_lo_input_color.Items.Count; i++)
            {
                if (cb_lo_input_color.Items[i].ToString() == cb_lo_input_color.Text)
                {
                    tb_lo_input_rgb.IsReadOnly = true;
                    tb_lo_input_rgb.Text = "";
                    break;
                }
            }
        }

        //입고 : 추가버튼
        private void btn_lo_input_listAdd_Click(object sender, RoutedEventArgs e)
        {
            List<Product> cus = new List<Product>();
            bool check = true;
            bool duple;

            if (cb_lo_input_productNumber.SelectedIndex == -1 || cb_lo_input_color.Text == "")
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "입력이 잘못됐습니다" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
            //기존에 입고된 상품일경우
            else if (duple = db.DupleCheck_stock_product(int.Parse(cb_lo_input_productNumber.Text), cb_lo_input_color.Text) && tb_lo_input_numberOf.Text != "" && dp_lo_input_inputDate.SelectedDate.ToString() != "" && cb_lo_input_inOutput.SelectedIndex != -1)
            {
                for (int i = 0; i < lv_lo_input_addList.Items.Count; i++)
                {
                    try
                    {   //기존에 있는 항목인 경우. DB가 아닌 로컬로 추가하므로 여기에서 Add
                        cus.Add((Product)lv_lo_input_addList.Items[i]);
                        if (cb_lo_input_productNumber.SelectedItem.ToString() == cus[i].Product_id.ToString() && cb_lo_input_color.Text == cus[i].Color && dp_lo_input_inputDate.SelectedDate.Value == cus[i].Trade_date)
                        {
                            plusStock = Convert.ToInt32(Convert.ToInt32(tb_lo_input_numberOf.Text) + cus[i].Stock);
                            lv_lo_input_addList.Items.Add(new Product()
                            {
                                Product_id = Convert.ToInt32(cb_lo_input_productNumber.Text),
                                Color = cb_lo_input_color.Text,
                                Stock = plusStock,
                                //ColorValue = "#" + tb_lo_input_rgb.Text,
                                Employee_id = Convert.ToInt32(tb_lo_input_employeeID.Text),
                                Trade_date = Convert.ToDateTime(dp_lo_input_inputDate.SelectedDate),
                                Trade_type = Convert.ToString(cb_lo_input_inOutput.Text)
                            });
                            lv_lo_input_addList.Items.RemoveAt(i);
                            lv_lo_input_addList.Items.Refresh();
                            check = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        var MessageDialog = new MessageDialog
                        {
                            Message = { Text = ex.Message }
                        };
                        DialogHost.Show(MessageDialog, "RootDialog");
                    }
                }
                if (check == true)  //새로운 항목인 경우
                {
                    plusStock = Convert.ToInt32(tb_lo_input_numberOf.Text);
                    lv_lo_input_addList.Items.Add(new Product()
                    {
                        Product_id = Convert.ToInt32(cb_lo_input_productNumber.Text),
                        Color = cb_lo_input_color.Text,
                        Stock = plusStock,
                        Employee_id = Convert.ToInt32(tb_lo_input_employeeID.Text),
                        Trade_date = Convert.ToDateTime(dp_lo_input_inputDate.SelectedDate),
                        Trade_type = Convert.ToString(cb_lo_input_inOutput.Text)
                    });
                    lv_lo_input_addList.Items.Refresh();
                }
            }

            //처음 입고된 상품일경우
            else if (!duple && tb_lo_input_numberOf.Text != "" && dp_lo_input_inputDate.SelectedDate.ToString() != "" && cb_lo_input_inOutput.SelectedIndex != -1 && tb_lo_input_rgb.Text != "" && tb_lo_reg_img.Text != "")
            {
                if (tb_lo_input_rgb.Text.Length == 6)	//RGB값 6자리 검사
                {
                    for (int i = 0; i < lv_lo_input_addList.Items.Count; i++)
                    {
                        try
                        {   //기존에 있는 항목인 경우. DB가 아닌 로컬로 추가하므로 여기에서 Add
                            cus.Add((Product)lv_lo_input_addList.Items[i]);
                            if (cb_lo_input_productNumber.SelectedItem.ToString() == cus[i].Product_id.ToString() && cb_lo_input_color.Text == cus[i].Color && dp_lo_input_inputDate.SelectedDate.Value == cus[i].Trade_date)
                            {
                                plusStock = Convert.ToInt32(Convert.ToInt32(tb_lo_input_numberOf.Text) + cus[i].Stock);
                                lv_lo_input_addList.Items.Add(new Product()
                                {
                                    Product_id = Convert.ToInt32(cb_lo_input_productNumber.Text),
                                    Color = cb_lo_input_color.Text,
                                    Stock = plusStock,
                                    ColorValue = "#" + tb_lo_input_rgb.Text,
                                    Employee_id = Convert.ToInt32(tb_lo_input_employeeID.Text),
                                    Trade_date = Convert.ToDateTime(dp_lo_input_inputDate.SelectedDate),
                                    Trade_type = Convert.ToString(cb_lo_input_inOutput.Text),
                                    Image_dir = tb_lo_reg_img.Text
                                });
                                lv_lo_input_addList.Items.RemoveAt(i);
                                lv_lo_input_addList.Items.Refresh();
                                check = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            var MessageDialog = new MessageDialog
                            {
                                Message = { Text = ex.Message }
                            };
                            DialogHost.Show(MessageDialog, "RootDialog");
                        }
                    }
                    if (check == true)  //새로운 항목인 경우
                    {
                        plusStock = Convert.ToInt32(tb_lo_input_numberOf.Text);
                        lv_lo_input_addList.Items.Add(new Product()
                        {
                            Product_id = Convert.ToInt32(cb_lo_input_productNumber.Text),
                            Color = cb_lo_input_color.Text,
                            Stock = plusStock,
                            ColorValue = "#" + tb_lo_input_rgb.Text,
                            Employee_id = Convert.ToInt32(tb_lo_input_employeeID.Text),
                            Trade_date = Convert.ToDateTime(dp_lo_input_inputDate.SelectedDate),
                            Trade_type = Convert.ToString(cb_lo_input_inOutput.Text),
                            Image_dir = tb_lo_reg_img.Text
                        });
                        lv_lo_input_addList.Items.Refresh();
                    }
                }
                else
                {
                    var MessageDialog = new MessageDialog
                    {
                        Message = { Text = "RGB값 입력이 잘못됐습니다" }
                    };
                    DialogHost.Show(MessageDialog, "RootDialog");
                }
            }
            else
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "입력이 잘못됐습니다" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }

        //입고 : 삭제버튼
        private void btn_lo_input_remove_Click(object sender, RoutedEventArgs e)
        {
            cb_lo_input_color.Text = "";
            tb_lo_input_numberOf.Text = "";
            tb_lo_input_rgb.Text = "";
            dp_lo_input_inputDate.SelectedDate = DateTime.Today;
            dp_lo_input_inputDate.Text = "";
            tb_lo_reg_img.Text = "";
            cb_lo_input_productNumber.SelectedIndex = -1;
        }
        //입고 : 등록버튼
        private void btn_lo_input_register_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("등록하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    btn_lo_pse_search_Click(sender, e);
                    if (lv_lo_input_addList.Items.Count != 0)	//추가할 제품이 리스트뷰에 이미 존재할 때
                    {
                        List<Product> dbdata = new List<Product>();
                        List<Product> inputdata = new List<Product>();
                        string query = "";
                        bool[] check = new bool[lv_lo_input_addList.Items.Count];
                        int[] position = new int[lv_lo_input_addList.Items.Count];
                        int[] newstock = new int[lv_lo_input_addList.Items.Count];

                        for (int i = 0; i < lv_lo_input_addList.Items.Count; i++)
                        {
                            if (i == 0)
                            {
                                query += "WHERE ";
                            }
                            Product product = (Product)lv_lo_input_addList.Items[i];
                            query += "product_id = " + product.Product_id;
                            if (i + 1 != lv_lo_input_addList.Items.Count)
                            {
                                query += " OR ";
                            }
                        }

                        dbdata = db.Select_Lo_Pse_stockProduct(query);
                        for (int i = 0; i < lv_lo_input_addList.Items.Count; i++)
                            inputdata.Add((Product)lv_lo_input_addList.Items[i]);

                        for (int i = 0; i < lv_lo_input_addList.Items.Count; i++)  //추가하려는 항목과 기존에 존재하는 항목간에 중복값 검사
                        {
                            newstock[i] = inputdata[i].Stock;
                            if (dbdata.Count != 0)
                            {
                                for (int j = 0; j < dbdata.Count; j++)
                                {
                                    //추가하려는 항목 중에 동일한 항목이 이미 DB에 존재하는 경우(수량 제외)
                                    if (inputdata[i].Product_id == dbdata[j].Product_id && inputdata[i].Color == dbdata[j].Color)
                                    {
                                        check[i] = true;
                                        position[i] = j;
                                        inputdata[i].ColorValue = dbdata[j].ColorValue;
                                        break;
                                    }
                                    else check[i] = false;
                                }
                            }
                            else check[i] = false;
                        }
                        db.input_transaction(inputdata, check, newstock);
                        for (int i = 0; i < inputdata.Count; i++)
                        {
                            if (check[i] == false)
                            {
                                string product_name = db.Select_productname_id(inputdata[i].Product_id);
                                FtpUploadFile(inputdata[i].Image_dir, ftp_uri + "/phone/" + product_name + "_" + inputdata[i].Color + "_F.JPG");
                            }
                        }
                        Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(1000);
                        }).ContinueWith(t =>
                        {
                            sb_main.MessageQueue.Enqueue("등록이 완료됐습니다");
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                        sb_main.DataContext = sb_main.MessageQueue;

                        Snackbar = this.sb_main;

                        cb_lo_input_productNumber.SelectedIndex = -1;
                        cb_lo_input_color.Text = "";
                        tb_lo_input_numberOf.Text = "";
                        tb_lo_input_rgb.Text = "";
                        dp_lo_input_inputDate.Text = "";
                        tb_lo_reg_img.Text = "";
                        lv_lo_input_addList.Items.Clear();
                    }
                    else
                    {
                        var MessageDialog = new MessageDialog
                        {
                            Message = { Text = "등록할 항목이 없습니다" }
                        };
                        DialogHost.Show(MessageDialog, "RootDialog");
                    }
                }
            }
            catch (Exception ex)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = ex.Message }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }
        //입고 : 추가할 리스트뷰 항목 더블클릭
        private void Lv_lo_input_item_addproduct_info_doubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("정말 삭제하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int index = lv_lo_input_addList.SelectedIndex;
                lv_lo_input_addList.Items.RemoveAt(index);
            }
        }
        //입고 : 등록물품 리스트뷰 항목 더블클릭
        private void Lv_lo_input_item_product_info_doubleClick(object sender, MouseButtonEventArgs e)
        {
            Product product = new Product();
            product = (Product)lv_lo_input_objectList.SelectedItem;

            cb_lo_input_productNumber.SelectedItem = product.Product_id;
            cb_lo_input_productNumber_DropDownOpened(sender, e);
        }

        #endregion

        #region 반품
        //반품 : 반품버튼
        private void btn_lo_refund_return_Click(object sender, RoutedEventArgs e)
        {
            List<Return_Info> return_Infos = new List<Return_Info>();

            if (lv_lo_refund_objectList.Items.Count < 1)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "선택된 물품이 없습니다." }
                };
                DialogHost.Show(MessageDialog, "RootDialog");

            }
            else
            {
                for (int i = 0; i < lv_lo_refund_objectList.Items.Count; i++)
                    return_Infos.Add((Return_Info)lv_lo_refund_objectList.Items[i]);
                for (int k = 0; k < lv_lo_refund_objectList.Items.Count - 1; k++)
                {
                    for (int j = k + 1; j < lv_lo_refund_objectList.Items.Count; j++)
                    {
                        if (return_Infos[k].Name == return_Infos[j].Name && return_Infos[k].Color == return_Infos[j].Color)
                        {
                            return_Infos[k].Quantity += return_Infos[j].Quantity;
                            return_Infos[j].Quantity = 0;
                        }
                        if (return_Infos[k].Quantity > db.Lo_Check_Stock(return_Infos[k].Product_id, return_Infos[k].Color))
                        {
                            var MessageDialog = new MessageDialog
                            {
                                Message = { Text = "반품 목록의 수량이 재고와 맞지않습니다." }
                            };
                            DialogHost.Show(MessageDialog, "RootDialog");

                            return;
                        }
                    }
                }

                try
                {
                    for (int l = 0; l < lv_lo_refund_objectList.Items.Count; l++)
                        db.return_transacion(return_Infos[l], emp.Employee_id);
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(1000);
                    }).ContinueWith(t =>
                    {
                        sb_main.MessageQueue.Enqueue("반품이 완료됐습니다");
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    sb_main.DataContext = sb_main.MessageQueue;

                    Snackbar = this.sb_main;
                    btn_lo_refund_reset_Click(sender, e);
                }
                catch (Exception ex)
                {
                    var MessageDialog = new MessageDialog
                    {
                        Message = { Text = ex.Message }
                    };
                    DialogHost.Show(MessageDialog, "RootDialog");
                }
            }

        }
        //반품 : 초기화버튼
        private void btn_lo_refund_reset_Click(object sender, RoutedEventArgs e)
        {
            lv_lo_refund_objectList.ItemsSource = null;
        }
        //반품 : 제품조회에서 반품할 항목을 더블클릭
        private void lv_lo_pse_productList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Product product = new Product();
                product = (Product)lv_lo_pse_productList.SelectedItems[0];
                StockReturn stock = new StockReturn();
                stock.SetProduct(product, this);
                stock.ShowDialog();
            }
            catch (Exception ex)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = ex.Message }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }
        //반품 : 제품조회에서 더블클릭하면 나오는 '수량 설정 창'에서 쓰임
        public void Set_Lo_Return_listview(Product product, int quantity)
        {
            List<Return_Info> return_Info_list = new List<Return_Info>();
            for (int i = 0; i < lv_lo_refund_objectList.Items.Count; i++)
            {
                Return_Info return_Info = (Return_Info)lv_lo_refund_objectList.Items[i];
                return_Info_list.Add(return_Info);
            }
            return_Info_list.Add(new Return_Info()
            {
                Product_id = product.Product_id,
                Name = product.Name,
                Color = product.Color,
                ColorValue = product.ColorValue,
                Quantity = quantity
            });
            lv_lo_refund_objectList.ItemsSource = return_Info_list;
        }

        #endregion

        #endregion

        #region 지원팀

        #region 직원조회
        string qr = string.Empty;
        int and_ck = 0;
        private void btn_su_search(object sender, RoutedEventArgs e)//지원 -> 고객조회 -> 조회버튼
        {

            List<Customer> customers;
            string ap = string.Empty;

            if (tb_se_cusName.Text != "")
            {
                Check_and(and_ck);
                qr += " name like @name";
            }
            if (cb_su_sex.Text != "모두")
            {
                Check_and(and_ck);
                qr += " gender=@gender";
            }
            if (tb_se_phone.Text != "")
            {
                Check_and(and_ck);
                qr += " phone like @phone";
            }

            if (qr != "") ap = " where" + qr;

            try
            {
                customers = db.GetList_Customer_Search(tb_se_cusName.Text, cb_su_sex.Text, tb_se_phone.Text, ap); //데이터 바인딩 & 전체출력
                lv_cus_search.ItemsSource = customers;
                tb_se_cusName.Clear();
                cb_su_sex.SelectedIndex = 0;
                tb_se_phone.Clear();
            }
            catch (Exception ex)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = ex.Message }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
            qr = "";
            and_ck = 0;
        }
        private void Check_and(int cnt)
        {
            cnt++;
            and_ck = cnt;
            if (and_ck > 1) qr += " and";
        }

        private void Btn_su_employee_lv_search(object sender, RoutedEventArgs e)// 지원 -> 직원조회 -> 조회버튼
        {
            List<Employee> emp_info;
            string ap = string.Empty;
            lv_emp_search.ItemsSource = null;

            if (tb_su_emp_search_name.Text != "")
            {
                Check_and(and_ck);
                qr += " name like @name";
            }
            if (tb_su_emp_search_login_id.Text != "")
            {
                Check_and(and_ck);
                qr += " login_id like @login_id";
            }
            if (cb_su_emp_search_gender.Text != "모두")
            {
                Check_and(and_ck);
                qr += " gender like @gender";
            }
            if (tb_su_emp_search_phone.Text != "")
            {
                Check_and(and_ck);
                qr += " phone like @phone";
            }
            if (dtp_su_emp_search_start_date.SelectedDate != null)
            {
                Check_and(and_ck);
                qr += " start_date=@start_date";
            }
            if (dtp_su_emp_search_end_date.SelectedDate != null)
            {
                Check_and(and_ck);
                qr += " end_date=@end_date";
            }

            if (qr != "")
            {
                ap = " where" + qr;
            }
            lv_emp_search.ItemsSource = null;

            emp_info = db.GetList_Emp_info(ap, tb_su_emp_search_login_id.Text, tb_su_emp_search_name.Text, tb_su_emp_search_phone.Text,
                cb_su_emp_search_gender.Text, dtp_su_emp_search_start_date.SelectedDate, dtp_su_emp_search_end_date.SelectedDate);
            lv_emp_search.ItemsSource = emp_info;

            qr = "";
            and_ck = 0;
        }

        #endregion

        #region 직원관리
        public void su_em_Reset_text()//지원 -> 직원관리 -> 리셋버튼
        {

            tb_su_em_login_id.Clear();
            tb_su_em_name.Clear();
            rb_su_em_gender1.IsChecked = false;
            rb_su_em_gender2.IsChecked = false;
            tb_su_em_social_n1.Clear();
            tb_su_em_phone1.Clear();
            tb_su_em_phone2.Clear();
            tb_su_em_phone3.Clear();
            tb_su_em_email.Clear();
            tb_su_em_adress.Clear();
            tb_su_em_start.Clear();
            tb_su_em_end.Clear();
            img_su_emp.ImageSource = null;
            tb_su_emp_img.Clear();
            cb_su_em_rank.Text = null;
            X_or_V();
        }


        public bool IsValidEmail(string email)//이메일이 유효한 이메일인지 확인
        {
            bool valid = Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
            return valid;
        }


        private void btn_RR_Close(object sender, RoutedEventArgs e)
        {
            tb_se_cusName.Text = "";
        }


        public bool V_check()
        {
            if (tb_su_em_rank_check.Text == "V" && tb_su_em_login_id_check.Text == "V" && tb_su_gender_check.Text == "V" && tb_su_name_check.Text == "V" &&
                tb_su_em_social_check.Text == "V" && tb_su_phone_check.Text == "V" && tb_su_email_check.Text == "V" && tb_su_adress_check.Text == "V")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void X_or_V()
        {
            if (tb_su_em_login_id.Text != "")
            {
                tb_su_em_login_id_check.Text = "V";
                tb_su_em_login_id_check.Foreground = Brushes.Green;
            }
            else
            {
                tb_su_em_login_id_check.Text = "X";
                tb_su_em_login_id_check.Foreground = Brushes.Red;
            }

            if (cb_su_em_rank.SelectedIndex.ToString() != "-1")
            {
                tb_su_em_rank_check.Text = "V";
                tb_su_em_rank_check.Foreground = Brushes.Green;
            }
            else
            {
                tb_su_em_rank_check.Text = "X";
                tb_su_em_rank_check.Foreground = Brushes.Red;
            }

            if (tb_su_em_name.Text != "")
            {
                tb_su_name_check.Text = "V";
                tb_su_name_check.Foreground = Brushes.Green;
            }
            else
            {
                tb_su_name_check.Text = "X";
                tb_su_name_check.Foreground = Brushes.Red;
            }

            if (rb_su_em_gender1.IsChecked != false || rb_su_em_gender2.IsChecked != false)
            {
                tb_su_gender_check.Text = "V";
                tb_su_gender_check.Foreground = Brushes.Green;
            }
            else
            {
                tb_su_gender_check.Text = "X";
                tb_su_gender_check.Foreground = Brushes.Red;
            }

            if (tb_su_em_social_n1.Text != "")
            {
                tb_su_em_social_check.Text = "V";
                tb_su_em_social_check.Foreground = Brushes.Green;
            }
            else
            {
                tb_su_em_social_check.Text = "X";
                tb_su_em_social_check.Foreground = Brushes.Red;
            }

            if ((tb_su_em_phone1.Text).Length == 3 && tb_su_em_phone3.Text.Length == 4 && tb_su_em_phone2.Text.Length >= 3 && tb_su_em_phone2.Text.Length <= 4)
            {
                tb_su_phone_check.Text = "V";
                tb_su_phone_check.Foreground = Brushes.Green;
            }
            else
            {
                tb_su_phone_check.Text = "X";
                tb_su_phone_check.Foreground = Brushes.Red;
            }

            if (tb_su_em_email.Text != "")
            {
                tb_su_email_check.Text = "V";
                tb_su_email_check.Foreground = Brushes.Green;
            }
            else
            {
                tb_su_email_check.Text = "X";
                tb_su_email_check.Foreground = Brushes.Red;
            }

            if (tb_su_em_adress.Text != "")
            {
                tb_su_adress_check.Text = "V";
                tb_su_adress_check.Foreground = Brushes.Green;
            }
            else
            {
                tb_su_adress_check.Text = "X";
                tb_su_adress_check.Foreground = Brushes.Red;
            }
        }

        private void TabControl_Selectionchanged(object sender, SelectionChangedEventArgs e)
        {
            if (menuitem2 != null & menuitem2.IsSelected) X_or_V();
        }//메뉴탭 클릭시 X_OR_V 를 실행

        private void su_em_valid_name_check(object sender, TextChangedEventArgs e)// 이름 올바른지 확인
        {
            if (tb_su_em_name.Text != "")
            {
                tb_su_name_check.Text = "V";
                tb_su_name_check.Foreground = Brushes.Green;
            }
            else
            {
                tb_su_name_check.Text = "X";
                tb_su_name_check.Foreground = Brushes.Red;
            }
        }

        private void su_em_valid_email_check(object sender, TextChangedEventArgs e)//주소 올바른지 확인
        {
            string valid_Email = "";
            valid_Email = tb_su_em_email.Text;
            if (IsValidEmail(valid_Email) == true)
            {
                tb_su_email_check.Text = "V";
                tb_su_email_check.Foreground = Brushes.Green;
            }
            else
            {
                tb_su_email_check.Text = "X";
                tb_su_email_check.Foreground = Brushes.Red;
            }
        }

        private void su_em_valid_address_check(object sender, TextChangedEventArgs e) // 주소가 유효한지 확인
        {
            if (tb_su_em_adress.Text != "")
            {
                tb_su_adress_check.Text = "V";
                tb_su_adress_check.Foreground = Brushes.Green;
            }
            else
            {
                tb_su_adress_check.Text = "X";
                tb_su_adress_check.Foreground = Brushes.Red;
            }
        }

        private void su_em_social1_textChange_event(object sender, TextChangedEventArgs e)//주민등록번호가 올바른지 확인
        {
            string social1_str = "";
            social1_str = tb_su_em_social_n1.Text;
            if (social1_str.Length == 6)
            {
                tb_su_em_social_check.Text = "V";
                tb_su_em_social_check.Foreground = Brushes.Green;
            }
            else
            {
                tb_su_em_social_check.Text = "X";
                tb_su_em_social_check.Foreground = Brushes.Red;
            }
        }

        private void su_em_valid_phone_check(object sender, TextChangedEventArgs e)// 휴대폰번호가 올바른지 확인
        {
            if ((tb_su_em_phone1.Text).Length == 3 && tb_su_em_phone3.Text.Length == 4 && tb_su_em_phone2.Text.Length >= 3 && tb_su_em_phone2.Text.Length <= 4)
            {
                tb_su_phone_check.Text = "V";
                tb_su_phone_check.Foreground = Brushes.Green;
            }
            else
            {
                tb_su_phone_check.Text = "X";
                tb_su_phone_check.Foreground = Brushes.Red;
            }
        }

        private void Btn_su_employee_search(object sender, RoutedEventArgs e) // 지원-> 직원관리 -> 직원id조회버튼
        {
            int flag = 0;
            string social_n = string.Empty, social_n1 = string.Empty, str = string.Empty;

            Employee employee = new Employee();
            if (tb_su_em_id.Text == "")
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "ID를 입력해 주세요" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
                tb_su_em_id.Clear();
            }
            else
            {
                try
                {
                    str = "";
                    employee = db.Get_Employee_info(int.Parse(tb_su_em_id.Text));

                    su_em_Reset_text();
                    tb_su_em_login_id.Text = employee.Login_id;
                    tb_su_em_name.Text = employee.Name;
                    if (employee.Gender == "남성")
                    {
                        rb_su_em_gender1.IsChecked = true;
                        rb_su_em_gender2.IsChecked = false;
                    }
                    else
                    {
                        rb_su_em_gender1.IsChecked = false;
                        rb_su_em_gender2.IsChecked = true;
                    }
                    social_n = (employee.Social_number).Substring(0, 6);
                    flag = 0;

                    tb_su_em_social_n1.Text = social_n;


                    str = employee.Phone;
                    if (str.Length - 3 == 7)
                    {
                        tb_su_em_phone1.Text = str.Substring(0, 3);
                        tb_su_em_phone2.Text = str.Substring(0, 3);
                        tb_su_em_phone3.Text = str.Substring(0, 4);
                    }
                    else
                    {
                        tb_su_em_phone1.Text = str.Substring(0, 3);
                        tb_su_em_phone2.Text = str.Substring(3, 4);
                        tb_su_em_phone3.Text = str.Substring(7, 4);
                    }

                    tb_su_em_start.Text = employee.Start_date.ToString();
                    tb_su_em_end.Text = employee.End_date.ToString();
                    tb_su_em_adress.Text = employee.Address;
                    tb_su_em_email.Text = employee.Email;
                    cb_su_em_rank.Text = employee.Rank;
                    X_or_V();
                    tb_su_em_login_id.IsReadOnly = true;
                    tb_su_em_start.IsReadOnly = true;
                    rb_su_em_gender1.IsEnabled = false;
                    rb_su_em_gender2.IsEnabled = false;

                    tb_su_em_social_n1.IsReadOnly = true;

                    tb_su_em_phone1.IsReadOnly = false;
                    tb_su_em_phone2.IsReadOnly = false;
                    tb_su_em_phone3.IsReadOnly = false;
                    tb_su_em_end.IsReadOnly = false;
                    tb_su_em_adress.IsReadOnly = false;
                    tb_su_em_email.IsReadOnly = false;
                    tb_su_em_name.IsReadOnly = false;
                    cb_su_em_rank.IsEnabled = true;
                    btn_su_emp_img.IsEnabled = true;


                    img_su_emp.ImageSource = new BitmapImage(new Uri(@http_uri + "/employee/" + employee.Login_id + "_" + employee.Rank + "_" + employee.Name + ".JPG", UriKind.Absolute));
                }
                catch
                {
                    var MessageDialog = new MessageDialog
                    {
                        Message = { Text = "등록되지 않은 직원입니다\n지원팀에 문의해주세요" }
                    };
                    DialogHost.Show(MessageDialog, "RootDialog");
                }
            }
        }

        private void Btn_su_text_reset(object sender, RoutedEventArgs e)// 지원 -> 직원관리 -> 리셋버튼
        {
            su_All_Reset();
        }

        void su_All_Reset()
        {
            su_em_Reset_text();
            img_su_emp.ImageSource = null;
            tb_su_emp_img.Text = null;
            tb_su_em_phone1.IsReadOnly = true;
            tb_su_em_phone2.IsReadOnly = true;
            tb_su_em_phone3.IsReadOnly = true;
            tb_su_em_end.IsReadOnly = true;
            tb_su_em_adress.IsReadOnly = true;
            tb_su_em_email.IsReadOnly = true;
            tb_su_em_name.IsReadOnly = true;
            cb_su_em_rank.IsEnabled = false;
            btn_su_emp_img.IsEnabled = false;
            tb_su_em_id.Clear();
        }


        private void Btn_su_employee_change(object sender, RoutedEventArgs e) // 직원 -> 직원관리 -> 조회버튼
        {
            string rb_gender_check = string.Empty;
            string phone = string.Empty;
            DateTime? end_d = null;
            if (rb_su_em_gender1.IsChecked == true) rb_gender_check = "남성";
            else if (rb_su_em_gender2.IsChecked == true) rb_gender_check = "여성";
            phone = tb_su_em_phone1.Text + tb_su_em_phone2.Text + tb_su_em_phone3.Text;

            if (tb_su_em_end.Text == "해당사항없음") end_d = null;
            else end_d = DateTime.Parse(tb_su_em_end.Text);
            try
            {
                if (V_check() == true)
                {
                    db.Update_Emp_Info(tb_su_em_login_id.Text, cb_su_em_rank.Text, tb_su_em_name.Text,
                    rb_gender_check, phone, tb_su_em_email.Text, tb_su_em_adress.Text, end_d);
                    if (tb_su_emp_img.Text != "")
                        FtpUploadFile(tb_su_emp_img.Text, ftp_uri + "/employee/" + tb_su_em_login_id.Text + "_" + cb_su_em_rank.Text + "_" + tb_su_em_name.Text + ".JPG");

                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(500);
                    }).ContinueWith(t =>
                    {
                        sb_main.MessageQueue.Enqueue("직원정보 변경 완료.");
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    sb_main.DataContext = sb_main.MessageQueue;

                    Snackbar = this.sb_main;
                    su_All_Reset();
                }
                else
                {
                    var MessageDialog = new MessageDialog
                    {
                        Message = { Text = "입력되지 않은 값이 있습니다" }
                    };
                    DialogHost.Show(MessageDialog, "RootDialog");
                }
            }
            catch
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "입력되지 않은 값이 있습니다" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }

        private void btn_su_em_change_pw(object sender, RoutedEventArgs e) // 지원 -> 직원관리 -> 비밀번호 변경버튼
        {
            Employee employee = new Employee();

            if (tb_su_em_login_id.Text != "")
            {
                if (MessageBox.Show("비밀번호를 초기화 하시겠습니까?", "알림창", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        employee = db.Get_Employee_info(int.Parse(tb_su_em_id.Text));
                        db.Reset_PW_EMP(employee.Login_id);
                        Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(500);
                        }).ContinueWith(t =>
                        {
                            sb_main.MessageQueue.Enqueue("비밀번호 변경 완료.");
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                        sb_main.DataContext = sb_main.MessageQueue;

                        Snackbar = this.sb_main;

                    }
                    catch
                    {
                        var MessageDialog = new MessageDialog
                        {
                            Message = { Text = "비밀번호 변경이 불가능합니다." }
                        };
                        DialogHost.Show(MessageDialog, "RootDialog");
                    }
                }
            }
        }

        private void Btn_su_emp_signup(object sender, RoutedEventArgs e) // 지원 -> 직원관리 -> 회원등록 조회버튼
        {
            lv_employee_sign_list.ItemsSource = null;
            lv_employee_sign_list.ItemsSource = db.GetList_Sign_Up_Emp();
        }

        public void Lv_su_item_emp_signup_DoubleClick(object sender, MouseButtonEventArgs e)//지원-> 직원관리-> 리스트 더블클릭 시 폼이동
        {
            Sign_up sign = new Sign_up();

            sign = ((Sign_up)lv_employee_sign_list.SelectedItems[0]);

            Emp_Sign_up emp_sign_up_Window = new Emp_Sign_up();
            emp_sign_up_Window.Set_Update_Cost(sign, this);
            emp_sign_up_Window.Show(this);
        }

        private void btn_su_upload_img_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                tb_su_emp_img.Text = filename;

                img_su_emp.ImageSource = new BitmapImage(new Uri(@dlg.FileName, UriKind.Absolute));
            }
        }

        #endregion 직원관리

        #region 고객관리
        private void Btn_su_cus_register(object sender, RoutedEventArgs e)
        {
            string gen = string.Empty;

            if (res_change_check == 0 && tb_su_cus_search_name.Text != "" && (rb_su_em_gender3.IsChecked != false || rb_su_em_gender4.IsChecked != false) && tb_su_cus_search_phone.Text != "")//신규등록
            {
                try
                {
                    if (rb_su_em_gender3.IsChecked == false && rb_su_em_gender4.IsChecked == true)
                    { gen = "여성"; }
                    else if (rb_su_em_gender3.IsChecked == true && rb_su_em_gender4.IsChecked == false)
                    { gen = "남성"; }

                    if (tb_su_cus_search_saving.Text == "")
                    {
                        tb_su_cus_search_saving.Text = "0";
                    }

                    db.Insert_Cus_Info(tb_su_cus_search_name.Text, gen, dtp_su_cus_search_birth.SelectedDate
                        , tb_su_cus_search_phone.Text, long.Parse(tb_su_cus_search_saving.Text));
                    su_cus_All_Clear();
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(500);
                    }).ContinueWith(t =>
                    {
                        sb_main.MessageQueue.Enqueue("고객 등록이 완료됐습니다.");
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    sb_main.DataContext = sb_main.MessageQueue;

                    Snackbar = this.sb_main;
                }
                catch
                {
                    var MessageDialog = new MessageDialog
                    {
                        Message = { Text = "입력되지 않은 값이 있습니다" }
                    };
                    DialogHost.Show(MessageDialog, "RootDialog");
                }
            }
            else if (res_change_check == 1 && tb_su_cus_search_name.Text != "" && (rb_su_em_gender3.IsChecked != false || rb_su_em_gender4.IsChecked != false) && tb_su_cus_search_phone.Text != "")
            {
                try
                {
                    if (rb_su_em_gender3.IsChecked == false && rb_su_em_gender4.IsChecked == true)
                    { gen = "여성"; }
                    else if (rb_su_em_gender3.IsChecked == true && rb_su_em_gender4.IsChecked == false)
                    { gen = "남성"; }

                    db.Update_Cus_Info(int.Parse(tb_su_cus_search_cus_id.Text), tb_su_cus_search_name.Text, gen, dtp_su_cus_search_birth.SelectedDate
                        , tb_su_cus_search_phone.Text, long.Parse(tb_su_cus_search_saving.Text));
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(500);
                    }).ContinueWith(t =>
                    {
                        sb_main.MessageQueue.Enqueue("고객 정보가 변경되었습니다.");
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                    sb_main.DataContext = sb_main.MessageQueue;

                    Snackbar = this.sb_main;
                    su_cus_All_Clear();
                }
                catch
                {
                    var MessageDialog = new MessageDialog
                    {
                        Message = { Text = "입력되지 않은 값이 있습니다" }
                    };
                    DialogHost.Show(MessageDialog, "RootDialog");
                }
            }
            else
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "입력되지 않은 값이 있습니다" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }

        private void Lv_su_item_cus_info_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Customer customer = new Customer();
            lable_first.Content = "정보변경";
            btn_su_cus_search_res.Content = "변경";
            customer = ((Customer)lv_cus_search.SelectedItems[0]);
            tb_su_cus_search_cus_id.Text = customer.Id.ToString();
            tb_su_cus_search_name.Text = customer.Name;

            if (customer.Gender == "남성")
            {
                rb_su_em_gender3.IsChecked = true;
                rb_su_em_gender4.IsChecked = false;
            }
            else
            {
                rb_su_em_gender3.IsChecked = false;
                rb_su_em_gender4.IsChecked = true;
            }
            tb_su_cus_search_phone.Text = customer.Phone;
            dtp_su_cus_search_birth.Text = customer.Date.ToString();
            tb_su_cus_search_saving.Text = customer.Savings.ToString();
            res_change_check = 1;
            label_cus_id.Visibility = Visibility.Visible;
            tb_su_cus_search_cus_id.Visibility = Visibility.Visible;
        }

        void su_cus_All_Clear()
        {
            tb_su_cus_search_cus_id.Text = "";
            tb_su_cus_search_name.Text = "";
            tb_su_cus_search_phone.Text = "";
            tb_su_cus_search_saving.Text = "";
            rb_su_em_gender3.IsChecked = false;
            rb_su_em_gender4.IsChecked = false;
            dtp_su_cus_search_birth.Text = "";
        }

        private void Btn_su_cus_reset(object sender, RoutedEventArgs e)
        {
            Customer customer = new Customer();
            lable_first.Content = "신규등록";
            btn_su_cus_search_res.Content = "등록";
            dtp_su_cus_search_birth.SelectedDate = DateTime.Today;
            su_cus_All_Clear();
            res_change_check = 0;
            customer = db.Get_Cus_Id();
            tb_su_cus_search_cus_id.Text = (customer.Id + 1).ToString();
            label_cus_id.Visibility = Visibility.Hidden;
            tb_su_cus_search_cus_id.Visibility = Visibility.Hidden;
        }

        #endregion

        #region 통계관리
        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }


        private void Chart_OnDataClick_sell_king(object sender, ChartPoint chartpoint1)
        {
            var chart1 = (LiveCharts.Wpf.PieChart)chartpoint1.ChartView;
            foreach (PieSeries series in chart1.Series)
                series.PushOut = 0;
            var selectedSeries = (PieSeries)chartpoint1.SeriesView;
            selectedSeries.PushOut = 8;
        }

        private void Chart_OnDataClick_sell_product(object sender, ChartPoint chartpoint2)
        {
            var chart2 = (LiveCharts.Wpf.PieChart)chartpoint2.ChartView;
            foreach (PieSeries series in chart2.Series)
                series.PushOut = 0;
            var selectedSeries = (PieSeries)chartpoint2.SeriesView;
            selectedSeries.PushOut = 8;
        }

        private void Input_Pie_Chart_1(string a, int b)
        {
            try
            {
                piechartData_1.Add(new PieSeries
                {
                    Title = a,
                    Values = new ChartValues<double> { b },
                    DataLabels = true,
                    LabelPoint = PointLabel,
                    //Fill = System.Windows.Media.Brushes.Gray
                });
            }
            catch (Exception e)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = e.Message }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }

        private void Input_Pie_Chart_2(long a, string b)
        {
            try
            {
                piechartData_2.Add(new PieSeries
                {
                    Title = b,
                    Values = new ChartValues<double> { a },
                    DataLabels = true,
                    LabelPoint = PointLabel1,
                    //Fill = System.Windows.Media.Brushes.Gray
                });
            }
            catch (Exception e)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = e.Message }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }

        private void Input_Pie_Chart_3(string a, int b)
        {
            try
            {
                piechartData_3.Add(new PieSeries
                {
                    Title = a,
                    Values = new ChartValues<double> { b },
                    DataLabels = true,
                    LabelPoint = PointLabel2,
                });
            }
            catch (Exception e)
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = e.Message }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }

        private void combo_dropdown(object sender, EventArgs e)
        {
            List<Chart_Brsell> chart_bList = new List<Chart_Brsell>();
            string Query = string.Empty;
            piechartData_1.Clear();
            try
            {
                if (cb_chart_select_sell.Text == "월별")
                {
                    Query = "AND DATEPART(MM,sh.sales_date) = MONTH(GETDATE())";
                    chart_bList = db.Get_Sell_Unit(Query);
                    for (int i = 0; i < chart_bList.Count; i++)
                    {
                        Input_Pie_Chart_1(chart_bList[i].Brand, chart_bList[i].Count);
                    }
                }
                else if (cb_chart_select_sell.Text == "분기별")
                {
                    Query = "AND DATEPART(QUARTER, sh.sales_date) = DATEPART(QUARTER, GETDATE())";
                    chart_bList = db.Get_Sell_Unit(Query);
                    for (int i = 0; i < chart_bList.Count; i++)
                    {
                        Input_Pie_Chart_1(chart_bList[i].Brand, chart_bList[i].Count);
                    }
                }
                else if (cb_chart_select_sell.Text == "년별")
                {
                    Query = "";
                    chart_bList = db.Get_Sell_Unit(Query);
                    for (int i = 0; i < chart_bList.Count; i++)
                    {
                        Input_Pie_Chart_1(chart_bList[i].Brand, chart_bList[i].Count);
                    }
                }

                // Define the collection of Values to display in the Pie Chart
                pieChart1.Series = piechartData_1;
                // Set the legend location to appear in the Right side of the chart
                pieChart1.LegendLocation = LegendLocation.Right;
            }
            catch
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "다시 선택해 주세요" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }

        private void combo_dropdown1(object sender, EventArgs e)
        {
            List<Chart_SellKing> chart_sList = new List<Chart_SellKing>();
            string Query = string.Empty;
            piechartData_2.Clear();

            try
            {
                if (cb_chart_select_sell_king.Text == "월별")
                {
                    Query = "AND DATEPART(MM,sh.sales_date) = MONTH(GETDATE()))";
                    chart_sList = db.PieChart_Sell_King(Query);
                    for (int i = 0; i < chart_sList.Count; i++)
                    {
                        Input_Pie_Chart_2(chart_sList[i].Sell_Price, chart_sList[i].Name);
                    }
                }
                else if (cb_chart_select_sell_king.Text == "분기별")
                {
                    Query = "AND  DATEPART(QUARTER,sh.sales_date) = DATEPART(QUARTER,GETDATE()))";
                    chart_sList = db.PieChart_Sell_King(Query);
                    for (int i = 0; i < chart_sList.Count; i++)
                    {
                        Input_Pie_Chart_2(chart_sList[i].Sell_Price, chart_sList[i].Name);
                    }
                }
                else if (cb_chart_select_sell_king.Text == "년별")
                {
                    Query = ")";
                    chart_sList = db.PieChart_Sell_King(Query);
                    for (int i = 0; i < chart_sList.Count; i++)
                    {
                        Input_Pie_Chart_2(chart_sList[i].Sell_Price, chart_sList[i].Name);
                    }
                }

                // Define the collection of Values to display in the Pie Chart
                pieChart2.Series = piechartData_2;
                // Set the legend location to appear in the Right side of the chart
                pieChart2.LegendLocation = LegendLocation.Right;
            }
            catch
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "다시 선택해 주세요" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }

        private void combo_dropdown2(object sender, EventArgs e)
        {
            List<Chart_Sellproduct> chart_spList = new List<Chart_Sellproduct>();
            string Query = string.Empty;
            piechartData_3.Clear();

            try
            {
                if (cb_chart_select_sell_product.Text == "월별")
                {
                    Query = "AND DATEPART(MM,sh.sales_date) = MONTH(GETDATE()))";
                    chart_spList = db.PieChart_Sell_Product(Query);
                    for (int i = 0; i < chart_spList.Count; i++)
                    {
                        Input_Pie_Chart_3(chart_spList[i].Product_Name, chart_spList[i].Prouct_Cnt);
                    }
                }
                else if (cb_chart_select_sell_product.Text == "분기별")
                {
                    Query = "AND  DATEPART(QUARTER,sh.sales_date) = DATEPART(QUARTER,GETDATE()))";
                    chart_spList = db.PieChart_Sell_Product(Query);
                    for (int i = 0; i < chart_spList.Count; i++)
                    {
                        Input_Pie_Chart_3(chart_spList[i].Product_Name, chart_spList[i].Prouct_Cnt);
                    }
                }
                else if (cb_chart_select_sell_product.Text == "년별")
                {
                    Query = ")";
                    chart_spList = db.PieChart_Sell_Product(Query);
                    for (int i = 0; i < chart_spList.Count; i++)
                    {
                        Input_Pie_Chart_3(chart_spList[i].Product_Name, chart_spList[i].Prouct_Cnt);
                    }
                }

                // Define the collection of Values to display in the Pie Chart
                pieChart3.Series = piechartData_3;
                // Set the legend location to appear in the Right side of the chart
                pieChart3.LegendLocation = LegendLocation.Right;
            }
            catch
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "다시 선택해 주세요" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }


        private void Basic_Chart_AddItem(List<long> p, string title)// 값 넣어주기
        {
            Series.Add(new ColumnSeries
            {
                Title = title,
                Values = new ChartValues<long>(p)
            });

        }

        private void Chart_btn_Reset(object sender, EventArgs e)
        {
            List<long> price_list_last = new List<long>();
            List<long> price_list_this = new List<long>();
            List<Basic_Chart_Sales> bchart_salesList = new List<Basic_Chart_Sales>();
            List<Basic_Chart_Sales> this_basic_list = new List<Basic_Chart_Sales>();
            List<Basic_Chart_Sales> last_basic_list = new List<Basic_Chart_Sales>();

            try
            {
                for (int i = 1; i <= 12; i++)
                {
                    Basic_Chart_Sales basic = new Basic_Chart_Sales();
                    basic.Year_Check = DateTime.Now.Year;
                    basic.Month_Check = i;
                    basic.Sell_Price = 0;
                    this_basic_list.Add(basic);
                }
                for (int i = 1; i <= 12; i++)
                {
                    Basic_Chart_Sales basic = new Basic_Chart_Sales();
                    basic.Year_Check = DateTime.Now.Year - 1;
                    basic.Month_Check = i;
                    basic.Sell_Price = 0;
                    last_basic_list.Add(basic);
                }

                bchart_salesList = db.BChart_Sales_Product();

                for (int i = 0; i < bchart_salesList.Count; i++)
                {
                    Basic_Chart_Sales basic = bchart_salesList[i];
                    if (basic.Year_Check == DateTime.Now.Year)
                    {
                        this_basic_list[basic.Month_Check - 1].Sell_Price = basic.Sell_Price;
                    }
                    else
                    {
                        last_basic_list[basic.Month_Check - 1].Sell_Price = basic.Sell_Price;
                    }
                }

                for (int i = 0; i < this_basic_list.Count; i++)
                {
                    price_list_this.Add(this_basic_list[i].Sell_Price);
                    price_list_last.Add(last_basic_list[i].Sell_Price);
                }


                while (barChart.Series.Count > 0)
                {
                    barChart.Series.RemoveAt(barChart.Series.Count - 1);
                }


                barChart.Series.Add(new ColumnSeries
                {
                    Title = last_basic_list[0].Year_Check.ToString(),
                    Values = new ChartValues<long>(price_list_last)
                });

                barChart.Series.Add(new ColumnSeries
                {
                    Title = this_basic_list[0].Year_Check.ToString(),
                    Values = new ChartValues<long>(price_list_this)
                });
            }
            catch
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "다시 선택해 주세요" }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }


        private void Setting_Chart()
        {
            List<Chart_Brsell> chart_bList = new List<Chart_Brsell>();
            List<Chart_SellKing> chart_sList = new List<Chart_SellKing>();
            List<Chart_Sellproduct> chart_spList = new List<Chart_Sellproduct>();
            List<long> price_list_last = new List<long>();
            List<long> price_list_this = new List<long>();
            List<Basic_Chart_Sales> bchart_salesList = new List<Basic_Chart_Sales>();
            List<Basic_Chart_Sales> this_basic_list = new List<Basic_Chart_Sales>();
            List<Basic_Chart_Sales> last_basic_list = new List<Basic_Chart_Sales>();
            string Query = string.Empty;

            piechartData_1.Clear();
            piechartData_2.Clear();
            piechartData_3.Clear();

            Query = "AND DATEPART(MM,sh.sales_date) = MONTH(GETDATE())";
            chart_bList = db.Get_Sell_Unit(Query);
            for (int i = 0; i < chart_bList.Count; i++)
            {
                Input_Pie_Chart_1(chart_bList[i].Brand, chart_bList[i].Count);
            }

            Query = "AND DATEPART(MM,sh.sales_date) = MONTH(GETDATE()))";
            chart_sList = db.PieChart_Sell_King(Query);
            for (int i = 0; i < chart_sList.Count; i++)
            {
                Input_Pie_Chart_2(chart_sList[i].Sell_Price, chart_sList[i].Name);
            }


            Query = "AND DATEPART(MM,sh.sales_date) = MONTH(GETDATE()))";
            chart_spList = db.PieChart_Sell_Product(Query);
            for (int i = 0; i < chart_spList.Count; i++)
            {
                Input_Pie_Chart_3(chart_spList[i].Product_Name, chart_spList[i].Prouct_Cnt);
            }

            for (int i = 1; i <= 12; i++)
            {
                Basic_Chart_Sales basic = new Basic_Chart_Sales();
                basic.Year_Check = DateTime.Now.Year;
                basic.Month_Check = i;
                basic.Sell_Price = 0;
                this_basic_list.Add(basic);
            }
            for (int i = 1; i <= 12; i++)
            {
                Basic_Chart_Sales basic = new Basic_Chart_Sales();
                basic.Year_Check = DateTime.Now.Year - 1;
                basic.Month_Check = i;
                basic.Sell_Price = 0;
                last_basic_list.Add(basic);
            }

            bchart_salesList = db.BChart_Sales_Product();

            for (int i = 0; i < bchart_salesList.Count; i++)
            {
                Basic_Chart_Sales basic = bchart_salesList[i];
                if (basic.Year_Check == DateTime.Now.Year)
                {
                    this_basic_list[basic.Month_Check - 1].Sell_Price = basic.Sell_Price;
                }
                else
                {
                    last_basic_list[basic.Month_Check - 1].Sell_Price = basic.Sell_Price;
                }
            }

            for (int i = 0; i < this_basic_list.Count; i++)
            {
                price_list_this.Add(this_basic_list[i].Sell_Price);
                price_list_last.Add(last_basic_list[i].Sell_Price);
            }


            while (barChart.Series.Count > 0)
            {
                barChart.Series.RemoveAt(barChart.Series.Count - 1);
            }


            barChart.Series.Add(new ColumnSeries
            {
                Title = last_basic_list[0].Year_Check.ToString(),
                Values = new ChartValues<long>(price_list_last)
            });

            barChart.Series.Add(new ColumnSeries
            {
                Title = this_basic_list[0].Year_Check.ToString(),
                Values = new ChartValues<long>(price_list_this)
            });

            pieChart1.Series = piechartData_1;
            pieChart1.LegendLocation = LegendLocation.Right;
            pieChart2.Series = piechartData_2;
            pieChart2.LegendLocation = LegendLocation.Right;
            pieChart3.Series = piechartData_3;
            pieChart3.LegendLocation = LegendLocation.Right;
        }

        #endregion

        #endregion


        private void btn_myinfo_update_Click(object sender, RoutedEventArgs e)
        {
            Sha256 sha256 = new Sha256();
            string strPw = sha256.ComputeSha256Hash(emp.Login_id + pb_myinfo_usePw.Password);

            if (pb_myinfo_usePw.Password == "")
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "현재 비밀번호를 입력해주세요." }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
            else if (strPw != emp.Login_pw)
            {


                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "기존 비밀번호가 일치하지 않습니다." }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
            else
            {
                if (tb_myinfo_phone.Text != emp.Phone)
                    emp.Phone = tb_myinfo_phone.Text;

                if (tb_myinfo_email.Text != emp.Email)
                    emp.Email = tb_myinfo_email.Text;

                if (tb_myinfo_address.Text != emp.Post_number + "/" + emp.Address)
                {
                    string[] strTemp = tb_myinfo_address.Text.Split('/');
                    emp.Post_number = strTemp[0];
                    emp.Address = strTemp[1];
                }

                if (pb_myinfo_newPw.Password != "")
                {
                    if (pb_myinfo_newPw.Password == pb_myinfo_NewPw_Check.Password)
                        strPw = pb_myinfo_newPw.Password;
                    else
                    {
                        var MessageDialog_check = new MessageDialog
                        {
                            Message = { Text = "변경할 비밀번호가 일치하지 않습니다." }
                        };
                        DialogHost.Show(MessageDialog_check, "RootDialog");
                        return;
                    }
                }

                db.Update_MyInfo(emp.Employee_id, emp.Phone, emp.Email, emp.Post_number, emp.Address, sha256.ComputeSha256Hash(emp.Login_id + strPw));
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "개인정보 변경에 성공했습니다." }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
                pb_myinfo_usePw.Password = "";
                pb_myinfo_newPw.Password = "";
                pb_myinfo_NewPw_Check.Password = "";

            }
        }

        private void Schedule_DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {

            if (!Equals(eventArgs.Parameter, true)) return;

            if (!string.IsNullOrWhiteSpace(ScheduleTextBox.Text))
                ScheduleListBox.Items.Add(ScheduleTextBox.Text.Trim());
        }

        private void btn_read_QR_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ZXing.BarcodeReader barcodeReader = new ZXing.BarcodeReader();

                //string deskPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.DefaultExt = ".jpeg";
                dlg.Filter = "Image files (*.jpeg)|*.jpeg";

                if (dlg.ShowDialog() == true)
                {
                    var barcodeBitmap = (Bitmap)System.Drawing.Image.FromFile(dlg.FileName);
                    string[] result = barcodeReader.Decode(barcodeBitmap).Text.Split('#');
                    tb_lo_reg_objectName.Text = result[0];
                    dp_lo_reg_inputDate.Text = result[1];
                    tb_lo_reg_objectCPU.Text = result[2];
                    tb_lo_reg_objectInch.Text = result[3];
                    tb_lo_reg_objectmAh.Text = result[4];
                    tb_lo_reg_objectRAM.Text = result[5];
                    tb_lo_reg_objectBrand.Text = result[6];
                    tb_lo_reg_objectCamera.Text = result[7];
                    tb_lo_reg_objectWeight.Text = result[8];
                    tb_lo_reg_objectPrice.Text = result[9];
                    tb_lo_reg_objectDisplay.Text = result[10];
                    tb_lo_reg_objectMemory.Text = result[11];
                }
            }
            catch
            {
                MessageBox.Show("형식이 일치하지 않은 QR코드 입니다.");
            }
        }
    }
}


