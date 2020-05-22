using MBStore_MVC.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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
        Employee emp;

        Timer _timer = null;
        string a;

        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(Employee employee)
        {
            this.Show();
            InitializeComponent();
            emp = employee;
            tb_main.Text = "환영합니다 " + emp.Name + "["+emp.Rank+"] 님.";
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
                if (tb_se_proudct_id.Text != "")
                {
                    product_id = int.Parse(tb_se_proudct_id.Text);
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

                if (tb_se_ram.Text != "")
                {
                    ram = int.Parse(tb_se_ram.Text);
                    query += " AND ram = @Ram";
                }

                if (tb_se_proudct_name.Text != "")
                {
                    product_name = tb_se_proudct_name.Text;
                    query += " AND name Like @Name";
                }

                if (tb_se_brand.Text != "")
                {
                    brand = tb_se_brand.Text;
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
                MessageBox.Show("잘못 입력하셨습니다");
            }

            Btn_se_Reset(sender, e);

            try
            {
                mbDB db = new mbDB();

                proList = db.SelectProduct(product_id, memory, price, ram, product_name, brand, manufacture, query);
                if (proList.Count != 0)
                {
                    //proList.Sort((x, y) => String.Compare(x.Color, y.Color)); //정렬
                    lv_se_product_info.ItemsSource = proList;
                }
                else
                    MessageBox.Show("찾으시는 데이터가 없습니다");
            }
            catch
            {
                MessageBox.Show("DB 오류");
            }
        }



        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        private void gv_se_head_click(object sender, RoutedEventArgs e)
        {
            ListView listview;
            DependencyObject present = VisualTreeHelper.GetParent((DependencyObject)sender);
            try
            {
                while(true)
                {
                    if (present.GetType().Name == "ListView") 
                        break;
                    present = VisualTreeHelper.GetParent((DependencyObject)present);
                }
            }
            catch
            {
                MessageBox.Show("오류");
            }
            //MessageBox.Show(p[i+1].GetValue(Control.NameProperty) as string);
            listview = (ListView)present; 
            if(listview.Items.Count!=0)
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
                        var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

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



        private void Btn_se_Reset(object sender, RoutedEventArgs e)
        {
            tb_se_proudct_id.Text = "";
            tb_se_proudct_name.Text = "";
            tb_se_brand.Text = "";
            cb_se_memory.Text = "memory";
            tb_se_price.Text = "";
            dtp_se_manufac.SelectedDate = null;
            tb_se_ram.Text = "";

            lv_se_product_info.ItemsSource = null;
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

        private void Lv_se_item_product_info_doubleClick(object sender, MouseButtonEventArgs e)
        {
            Product product = new Product();
            product = (Product)lv_se_product_info.SelectedItems[0];

            ShoppingBasket shoppingBasket_w = new ShoppingBasket();
            shoppingBasket_w.SetProduct(product, this);
            shoppingBasket_w.ShowDialog();
        }
        #endregion


        #region 상품판매
        public void Set_Sell_listview(Product product, int quantity)
        {
            lv_se_expect_sell.Items.Add(new Sell_Info()
            {
                Product_id = product.Product_id,
                Product_name = product.Name,
                Color = product.Color,
                ColorValue = product.ColorValue,
                Quantity = quantity,
                Total_price = product.Price * quantity,
            });
            lv_se_expect_sell.Items.Refresh();

            string str_total_price = la_se_sell_total_price.Content.ToString();
            str_total_price = str_total_price.Substring(0, str_total_price.Length - 2);
            long total_price = long.Parse(str_total_price, NumberStyles.AllowThousands);
            total_price += product.Price * quantity;
            la_se_sell_total_price.Content = string.Format("{0:#,##0}", total_price) + " 원";
        }

        private void Btn_se_sell_list_remove(object sender, RoutedEventArgs e)
        {
            lv_se_expect_sell.Items.Clear();
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

        private void Btn_se_Salse_history_Search(object sender, RoutedEventArgs e)
        {
            string customer_name = "", employee_name = "", sales_type = "";
            DateTime sales_date = new DateTime();
            string query = "";
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

                if (dtp_se_sold_date.SelectedDate.HasValue)
                {
                    sales_date = dtp_se_sold_date.SelectedDate.Value;

                    query += " AND sh.sales_date = @Sales_date";
                }
                else
                    sales_date = Convert.ToDateTime("0001/01/01");
            }
            catch
            {
                MessageBox.Show("잘못 입력하셨습니다");
            }

            Btn_se_Salse_history_Reset(sender, e);

            try
            {
                mbDB db = new mbDB();

                if (query != "")    //안 비어있을때
                {
                    query = query.Substring(5);
                    query = "WHERE " + query;
                }
                sellinfoList = db.SelectSalesHistory(customer_name, employee_name, sales_type, sales_date, -1, query);
                if (sellinfoList.Count != 0)
                {
                    //proList.Sort((x, y) => String.Compare(x.Color, y.Color)); //정렬
                    lv_se_sales_history.ItemsSource = sellinfoList;
                }
                else
                    MessageBox.Show("찾으시는 데이터가 없습니다");
            }
            catch
            {
                MessageBox.Show("DB 오류");
            }
        }

        private void Btn_se_Salse_history_Reset(object sender, RoutedEventArgs e)
        {
            tb_se_customer_name.Text = "";
            tb_se_employee_name.Text = "";
            cb_se_type.Text = "모두";
            dtp_se_sold_date.SelectedDate = null;

            lv_se_sales_history.ItemsSource = null;
        }

        private void Lv_se_sales_history_doubleClick(object sender, MouseButtonEventArgs e)
        {
            Sell_Info sell_info = new Sell_Info();
            sell_info = (Sell_Info)lv_se_sales_history.SelectedItems[0];
            bool refunded = false;

            try
            {
                mbDB db = new mbDB();
                refunded = db.SelectSalesHistoryRefunded(sell_info.Sales_history_id);
            }
            catch
            {
                MessageBox.Show("오류");
            }

            if (sell_info.Sales_date.AddDays(7) >= DateTime.Now && sell_info.Sales_type == "판매" && !refunded)
            {
                if (MessageBox.Show("환불 목록에 추가하시겠습니까?", "알림창", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    List<Sell_Info> refundList = new List<Sell_Info>();
                    string query = "WHERE sh.sales_history_id = @Sales_hisory_id";
                    try
                    {
                        mbDB db = new mbDB();
                        long total_price = 0;

                        refundList = db.SelectSalesHistory("", "", "", DateTime.Now, sell_info.Sales_history_id, query);
                        lv_se_expect_refund.ItemsSource = refundList;

                        for (int i = 0; i < refundList.Count; i++)
                            total_price += refundList[i].Total_price;

                        la_se_refund_total_price.Content = string.Format("{0:#,##0}", total_price) + " 원";
                    }
                    catch
                    {
                        MessageBox.Show("오류");
                    }
                }
            }
            else
            {
                MessageBox.Show("환불이 불가능한 제품입니다");
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
                mbDB db = new mbDB();
                List<Sell_Info> refund_list = new List<Sell_Info>();

                for (int i = 0; i < lv_se_expect_refund.Items.Count; i++)
                {
                    refund_list.Add((Sell_Info)lv_se_expect_refund.Items[i]);
                }

                try
                {
                    db.UpdateSalesHistory(refund_list[0].Sales_history_id);

                    int new_history_id;
                    db.InsertSalesHistroy(refund_list[0].Customer_id, emp.Employee_id, DateTime.Now);
                    new_history_id = db.SelectMaxHistoryId();
                    for (int i = 0; i < refund_list.Count; i++)
                    {
                        Sell_Info item = refund_list[i];
                        db.InsertSalesProduct(new_history_id, item.Product_id, item.Quantity, item.Color, item.ColorValue, "환불");
                        db.UpdateStockProduct(item.Product_id, item.Color, item.Quantity);
                    }

                    Sell_Info sell_Info = db.SelectHistoryTotalPrice(refund_list[0].Sales_history_id);
                    long savings = sell_Info.Total_price / 100;
                    db.UpdateCustomerSavings(sell_Info.Customer_id, -savings);

                    MessageBox.Show("환불이 완료 되었습니다", "알림창");
                    Btn_se_refund_list_remove(sender, e);
                }
                catch
                {
                    MessageBox.Show("오류");
                    lv_se_expect_refund.ItemsSource = null;
                }
            }

        }
        #endregion

        #endregion

        #region 물류팀
        //함수 이름예시
        //_Lo_, _lo_ : 물류팀 (Logistics)
        //_Reg_, _reg_ : 제품등록 영역
        //_Io_, _io_ : 입고/반품 영역(Input/Output)
        //_Pse_, _pse_ : 제품 조회 영역(Product Search)
        //_Rse_, _rse_ : 내역 조회 영역(Receipt Search) ------- 미구현

        //여기부터 시작
        int plusStock; //클래스 변수
                       //제품 조회 영역 - 조회버튼 눌렀을 때
        #region 물품 조회
        private void btn_lo_pse_search_Click(object sender, RoutedEventArgs e)
        {
            int product_id = 0, memory = 0, price = 0, ram = 0;
            string product_name = "", brand = "";
            DateTime manufacture = new DateTime();
            string query = "";
            List<Product> proList = new List<Product>();

            try
            {
                if (tb_lo_pse_productID.Text != "")
                {
                    product_id = int.Parse(tb_lo_pse_productID.Text);
                    query += " AND p.product_id = @Product_id";
                }

                if (cb_lo_pse_memory.SelectedIndex.ToString() != "-1")
                {
                    memory = int.Parse(cb_lo_pse_memory.Text);
                    query += " AND memory = @Memory";
                }

                if (tb_lo_pse_price.Text != "")
                {
                    price = int.Parse(tb_lo_pse_price.Text);
                    if (btn_lo_pse_price.Content.Equals("▲"))
                        query += " AND price >= @Price";
                    else
                        query += " AND price <= @Price";
                }

                if (tb_lo_pse_ram.Text != "")
                {
                    ram = int.Parse(tb_lo_pse_ram.Text);
                    query += " AND ram = @Ram";
                }

                if (tb_lo_pse_name.Text != "")
                {
                    product_name = tb_lo_pse_name.Text;
                    query += " AND name Like @Name";
                }

                if (tb_lo_pse_brand.Text != "")
                {
                    brand = tb_lo_pse_brand.Text;
                    query += " AND brand Like @Brand";
                }

                if (dtp_lo_pse_manufacture.SelectedDate.HasValue)
                {
                    manufacture = dtp_lo_pse_manufacture.SelectedDate.Value;
                    if (btn_lo_pse_manufacture.Content.Equals("▲"))
                        query += " AND manufacture >= @Manufacture";
                    else
                        query += " AND manufacture <= @Manufacture";
                }
                else
                    manufacture = Convert.ToDateTime("0001/01/01");
            }
            catch
            {
                MessageBox.Show("잘못 입력하셨습니다");
            }

            Btn_se_Reset(sender, e);

            try
            {
                mbDB db = new mbDB();

                proList = db.SelectProduct(product_id, memory, price, ram, product_name, brand, manufacture, query);
                if (proList.Count != 0)
                {
                    //proList.Sort((x, y) => String.Compare(x.Color, y.Color)); //정렬
                    lv_lo_pse_productList.ItemsSource = proList;
                }
                else
                    MessageBox.Show("찾으시는 데이터가 없습니다");
            }
            catch
            {
                MessageBox.Show("DB 오류");
            }
        }
        #endregion

        //제품 조회 영역 : 리셋버튼 클릭
        private void btn_lo_pse_reset_Click(object sender, RoutedEventArgs e)
        {
            tb_lo_pse_productID.Text = "";
            tb_lo_pse_name.Text = "";
            tb_lo_pse_brand.Text = "";
            cb_lo_pse_memory.Text = "memory";
            tb_lo_pse_price.Text = "";
            dtp_lo_pse_manufacture.SelectedDate = null;
            tb_lo_pse_ram.Text = "";
        }
        //제품조회영역 : 화살표 버튼 - 미구현
        private void btn_lo_pse_price_Click(object sender, RoutedEventArgs e)
        {
            if (btn_lo_pse_price.Content.Equals("▲"))
                btn_lo_pse_price.Content = "▼";
            else
                btn_lo_pse_price.Content = "▲";
        }
        //제품조회영역 : 화살표 버튼 - 미구현
        private void btn_lo_pse_manufacture_Click(object sender, RoutedEventArgs e)
        {
            if (btn_lo_pse_manufacture.Content.Equals("▲"))
                btn_lo_pse_manufacture.Content = "▼";
            else
                btn_lo_pse_manufacture.Content = "▲";
        }
        //입출고 영역 : DB에 등록된 제품번호를 해당 콤보박스에 리스트로 가져옴
        private void cb_lo_io_productNumber_DropDownOpened(object sender, EventArgs e)
        {
            List<Int32> productdata;
            mbDB mb = new mbDB();

            productdata = mb.Get_Lo_Io_ProductNumList();
            cb_lo_io_productNumber.ItemsSource = productdata;
        }
        //입출고 영역 : 조회버튼
        private void btn_lo_io_search_Click(object sender, RoutedEventArgs e)
        {
            List<Product> productdata;
            mbDB mb = new mbDB();

            productdata = mb.Get_Lo_Io_ProductList();
            lv_lo_io_objectList.ItemsSource = productdata;
            if (lv_lo_io_objectList.Items.Count == 0)
                MessageBox.Show("데이터가 존재하지 않습니다");
        }
        //입출고 영역 : 등록버튼
        private void btn_lo_io_register_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("조회를 먼저해야 최신정보를 받아올 수 있습니다.\r등록 직전에 조회를 하는 것을 추천합니다.\r등록하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (lv_lo_io_addList.Items.Count != 0)	//추가할 제품의 목록 리스트뷰 목록이 존재할 때
                    {
                        List<Product> dbdata = new List<Product>();
                        List<Product> inputdata = new List<Product>();
                        bool[] check = new bool[lv_lo_io_addList.Items.Count];
                        int[] position = new int[lv_lo_io_addList.Items.Count];

                        for (int i = 0; i < lv_lo_io_objectList.Items.Count; i++)
                            dbdata.Add((Product)lv_lo_io_objectList.Items[i]);
                        for (int i = 0; i < lv_lo_io_addList.Items.Count; i++)
                            inputdata.Add((Product)lv_lo_io_addList.Items[i]);

                        for (int i = 0; i < lv_lo_io_addList.Items.Count; i++)  //추가하려는 항목과 기존에 존재하는 항목간에 중복값 검사
                        {
                            if (lv_lo_io_objectList.Items.Count != 0)
                            {
                                for (int j = 0; j < lv_lo_io_objectList.Items.Count; j++)
                                {
                                    //추가하려는 항목 중에 동일한 항목이 이미 DB에 존재하는 경우(수량 제외)
                                    if (inputdata[i].Product_id == dbdata[j].Product_id && inputdata[i].Color == dbdata[j].Color)
                                    {
                                        check[i] = true;
                                        position[i] = j;
                                        break;
                                    }
                                    else check[i] = false;
                                }
                            }
                            else check[i] = false;
                        }

                        for (int i = 0; i < lv_lo_io_addList.Items.Count; i++)	//추가할 리스트뷰에 존재하는 항목 수 만큼
                        {
                            if (check[i] == false && inputdata[i].Trade_type == "입고") //DB에 없는 새로운 항목
                            {
                                try
                                {
                                    string sql = "insert into stock_product(product_id, color, stock, color_value) values("
                                        + inputdata[i].Product_id.ToString() + ",'" + inputdata[i].Color.ToString() + "',"
                                        + plusStock.ToString() + ",'" + inputdata[i].ColorValue.ToString() + "')";
                                    string sql2 = "insert into trade_history values (" + inputdata[i].Employee_id.ToString()
                                        + ",'" + inputdata[i].Trade_date.ToShortDateString() + "')";
                                    string sql3 = "insert into trade_product(product_id, trade_history_id, color, quantity, trade_type, color_value) values ("
                                        + inputdata[i].Product_id.ToString() + ",(SELECT MAX(trade_history_id) FROM trade_history),'" + inputdata[i].Color.ToString() + "'," + plusStock.ToString() + ",'" + inputdata[i].Trade_type.ToString()
                                        + "','" + inputdata[i].ColorValue.ToString() + "')";

                                    Product productdata;
                                    mbDB mb = new mbDB();

                                    productdata = mb.Set_Lo_Product(sql);
                                    if (i == 0) productdata = mb.Set_Lo_History(sql2);
                                    productdata = mb.Set_Lo_History(sql3);

                                    //cb_lo_productNumber.SelectedIndex = -1;	//빈칸 지우기
                                    //tb_lo_numberOf.Clear();
                                    //tb_lo_Color.Clear();
                                    //tb_lo_rgb.Clear();
                                    //cb_lo_employeenum.SelectedIndex = -1;
                                    //lv_lo_addList.Items.Clear();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                            else if (check[i] == true && inputdata[i].Trade_type == "입고") //기존 항목
                            {
                                try
                                {
                                    //고쳐야할 방향 : 단순히 이름과 색깔로 겹치는걸 찾는 게 아니라 자동으로 부여되는 stock_product를 찾아서 바꿔야한다
                                    string sql = "update stock_product set stock = stock + " + plusStock + " where stock_product = (select stock_product from stock_product where product_id = " + inputdata[i].Product_id + " and color = '" + inputdata[i].Color + "')";
                                    string sql2 = "update trade_product set quantity = quantity + " + plusStock + " where trade_id = (select trade_id from trade_product where product_id = " + inputdata[i].Product_id + " and color = '" + inputdata[i].Color + "' and trade_type = '입고')";
                                    Product productdata;
                                    mbDB mb = new mbDB();

                                    productdata = mb.Set_Lo_Product(sql);
                                    productdata = mb.Set_Lo_History(sql2);

                                    //cb_lo_productNumber.SelectedIndex = -1;
                                    //tb_lo_numberOf.Clear();
                                    //tb_lo_Color.Clear();
                                    //tb_lo_rgb.Clear();
                                    //cb_lo_employeenum.SelectedIndex = -1;
                                    //lv_lo_addList.Items.Clear();

                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                            else if (check[i] == false && inputdata[i].Trade_type == "반품")	//DB에 없는데 반품인 경우
                            {
                                MessageBox.Show("재고가 없거나 부족합니다");
                            }
                            else if (check[i] == true && inputdata[i].Trade_type == "반품")	//DB에 있고 반품인 경우
                            {   //DB에서 해당하는 물품의 재고를 불러오고 입력한 데이터 inputdata와 비교하여 재고가 더 적으면 메시지 출력
                                Product productdata;
                                mbDB mb = new mbDB();

                                for (int j = 0; j < lv_lo_io_objectList.Items.Count; j++)
                                {
                                    if (inputdata[j].Stock <= dbdata[position[j]].Stock)
                                    {
                                        //고쳐야할 방향 : 단순히 이름과 색깔로 겹치는걸 찾는 게 아니라 자동으로 부여되는 stock_product를 찾아서 바꿔야한다
                                        string sql = "update stock_product set stock = stock - " + plusStock + " where stock_product = (select stock_product from stock_product where product_id = " + inputdata[j].Product_id + " and color = '" + inputdata[j].Color + "')";
                                        string sql2 = "insert into trade_history values (" + inputdata[j].Employee_id.ToString()
                                            + ",'" + inputdata[j].Trade_date.ToShortDateString() + "')";
                                        string sql3 = "insert into trade_product(product_id, trade_history_id, color, quantity, trade_type, color_value) values ("
                                            + inputdata[j].Product_id.ToString() + ",(SELECT MAX(trade_history_id) FROM trade_history),'" + inputdata[j].Color.ToString() + "'," + plusStock.ToString() + ",'" + inputdata[j].Trade_type.ToString()
                                            + "','" + inputdata[j].ColorValue.ToString() + "')";

                                        productdata = mb.Set_Lo_Product(sql);
                                        if (j == 0) productdata = mb.Set_Lo_History(sql2);
                                        productdata = mb.Set_Lo_History(sql3);

                                        //cb_lo_productNumber.SelectedIndex = -1;
                                        //tb_lo_numberOf.Clear();
                                        //tb_lo_Color.Clear();
                                        //tb_lo_rgb.Clear();
                                        //cb_lo_employeenum.SelectedIndex = -1;
                                        //lv_lo_addList.Items.Clear();
                                        break;
                                    }
                                    else if (inputdata[j].Stock > dbdata[position[j]].Stock)
                                    {
                                        MessageBox.Show("반품수량이 재고를 초과합니다");
                                        break;
                                    }
                                }
                            }
                        }
                        MessageBox.Show("등록완료");
                    }
                    else
                    {
                        MessageBox.Show("등록할 항목이 없습니다");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //입출고 영역 : 추가 버튼 클릭
        private void btn_lo_io_listAdd_Click(object sender, RoutedEventArgs e)
        {
            List<Product> cus = new List<Product>();
            bool check = true;
            if (cb_lo_io_productNumber.SelectedIndex == -1 || tb_lo_io_color.Text == "" || tb_lo_io_numberOf.Text == "" || tb_lo_io_rgb.Text == "" || cb_lo_io_employeeNum.SelectedIndex == -1 || dp_lo_io_inputDate.SelectedDate.ToString() == "" || cb_lo_io_inOutput.SelectedIndex == -1)
            {
                MessageBox.Show("입력 오류");
            }
            else
            {
                if (tb_lo_io_rgb.Text.Length == 6)	//RGB값 6자리 검사
                {
                    for (int i = 0; i < lv_lo_io_addList.Items.Count; i++)
                    {
                        try
                        {   //기존에 있는 항목인 경우. DB가 아닌 로컬로 추가하므로 여기에서 Add
                            cus.Add((Product)lv_lo_io_addList.Items[i]);
                            if (cb_lo_io_productNumber.SelectedItem.ToString() == cus[i].Product_id.ToString() && tb_lo_io_color.Text == cus[i].Color && dp_lo_io_inputDate.SelectedDate.Value == cus[i].Trade_date)
                            {
                                plusStock = Convert.ToInt32(Convert.ToInt32(tb_lo_io_numberOf.Text) + cus[i].Stock);
                                lv_lo_io_addList.Items.Add(new Product()
                                {
                                    Product_id = Convert.ToInt32(cb_lo_io_productNumber.Text),
                                    Color = tb_lo_io_color.Text,
                                    Stock = plusStock,
                                    ColorValue = "#" + tb_lo_io_rgb.Text,
                                    Employee_id = Convert.ToInt32(cb_lo_io_employeeNum.Text),
                                    Trade_date = Convert.ToDateTime(dp_lo_io_inputDate.SelectedDate),
                                    Trade_type = Convert.ToString(cb_lo_io_inOutput.Text)
                                });
                                lv_lo_io_addList.Items.RemoveAt(i);
                                lv_lo_io_addList.Items.Refresh();
                                check = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    if (check == true)  //새로운 항목인 경우
                    {
                        plusStock = Convert.ToInt32(tb_lo_io_numberOf.Text);
                        lv_lo_io_addList.Items.Add(new Product()
                        {
                            Product_id = Convert.ToInt32(cb_lo_io_productNumber.Text),
                            Color = tb_lo_io_color.Text,
                            Stock = plusStock,
                            ColorValue = "#" + tb_lo_io_rgb.Text,
                            Employee_id = Convert.ToInt32(cb_lo_io_employeeNum.Text),
                            Trade_date = Convert.ToDateTime(dp_lo_io_inputDate.SelectedDate),
                            Trade_type = Convert.ToString(cb_lo_io_inOutput.Text)
                        });
                        lv_lo_io_addList.Items.Refresh();
                    }
                }
                else
                    MessageBox.Show("RGB값 입력 오류");
            }
        }
        //입출고 영역 : 삭제 버튼
        private void btn_lo_io_remove_Click(object sender, RoutedEventArgs e)
        {
            if (lv_lo_io_addList.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("정말 삭제하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int index = lv_lo_io_addList.SelectedIndex;
                    lv_lo_io_addList.Items.RemoveAt(index);
                }
            }
            else
            {
                MessageBox.Show("선택된 항목이 없습니다.");
            }
        }
        //로그인한 사용자의 사번을 불러와서 사번 콤보박스에 넣는다
        private void cb_lo_io_employeeNum_DropDownOpened(object sender, EventArgs e)
        {
            List<Int32> productdata;
            mbDB mb = new mbDB();

            productdata = mb.Get_Lo_Io_EmployeeID(emp.Name);
            cb_lo_io_employeeNum.ItemsSource = productdata;
            cb_lo_io_employeeNum.SelectedIndex = 0;
        }
        //제품등록 영역 : 조회버튼 클릭
        private void btn_lo_reg_search_Click(object sender, RoutedEventArgs e)
        {
            List<Product> product;
            mbDB mb = new mbDB();

            try
            {
                product = mb.Get_Lo_Reg_RegistProductList();
                lv_lo_reg_productList.ItemsSource = product;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (lv_lo_reg_productList.Items.Count == 0)
                MessageBox.Show("데이터가 존재하지 않습니다");
        }
        //제품등록 영역 : 등록 버튼
        private void btn_lo_reg_regist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mbDB db = new mbDB();
                //비어있는 항목이 없는지 검사
                if (tb_lo_reg_objectName.Text != "" && dtp_lo_reg_objectManufacture.Text != "" && tb_lo_reg_objectCPU.Text != "" && tb_lo_reg_objectInch.Text != "" && tb_lo_reg_objectmAh.Text != "" && tb_lo_reg_objectRAM.Text != "" && tb_lo_reg_objectBrand.Text != "" && tb_lo_reg_objectCamera.Text != "" && tb_lo_reg_objectWeight.Text != "" && tb_lo_reg_objectPrice.Text != "" && tb_lo_reg_objectDisplay.Text != "" && tb_lo_reg_objectMemory.Text != "")
                {
                    if (MessageBox.Show("등록하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            db.Add_Lo_Reg_Product(tb_lo_reg_objectName.Text, DateTime.Parse(dtp_lo_reg_objectManufacture.Text), tb_lo_reg_objectCPU.Text, tb_lo_reg_objectInch.Text, Int16.Parse(tb_lo_reg_objectmAh.Text), Int16.Parse(tb_lo_reg_objectRAM.Text), tb_lo_reg_objectBrand.Text, Int16.Parse(tb_lo_reg_objectCamera.Text), Int16.Parse(tb_lo_reg_objectWeight.Text), Int64.Parse(tb_lo_reg_objectPrice.Text), tb_lo_reg_objectDisplay.Text, Int16.Parse(tb_lo_reg_objectMemory.Text));
                            tb_lo_reg_objectName.Text = string.Empty;
                            dtp_lo_reg_objectManufacture.Text = string.Empty;
                            tb_lo_reg_objectCPU.Text = string.Empty;
                            tb_lo_reg_objectInch.Text = string.Empty;
                            tb_lo_reg_objectmAh.Text = string.Empty;
                            tb_lo_reg_objectRAM.Text = string.Empty;
                            tb_lo_reg_objectBrand.Text = string.Empty;
                            tb_lo_reg_objectCamera.Text = string.Empty;
                            tb_lo_reg_objectWeight.Text = string.Empty;
                            tb_lo_reg_objectPrice.Text = string.Empty;
                            tb_lo_reg_objectDisplay.Text = string.Empty;
                            tb_lo_reg_objectMemory.Text = string.Empty;
                        }
                        catch
                        {
                            MessageBox.Show("형식 오류");
                        }
                    }
                }
                else
                    MessageBox.Show("입력을 완료하세요");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }

        private void UpdateColumnsWidth(ListView listView)
        {
            int autoFillColumnIndex = (listView.View as GridView).Columns.Count - 3;
            if (listView.ActualWidth == Double.NaN)
                listView.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            double remainingSpace = listView.ActualWidth - 27;
            for (int i = 0; i < (listView.View as GridView).Columns.Count; i++)
                if (i != autoFillColumnIndex)
                    remainingSpace -= (listView.View as GridView).Columns[i].ActualWidth;
            (listView.View as GridView).Columns[autoFillColumnIndex].Width = remainingSpace >= 0 ? remainingSpace : 0;
        }

        #endregion


    }
}
