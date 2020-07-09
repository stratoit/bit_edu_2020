using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Wpf;
using MBStore_MVVM.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MBStore_MVVM.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private mbDB db;
        private Sha256 sha256 = new Sha256();

        private string uri;
        private string ftp_uri;
        private string http_uri;
        string path = @"autoloing.txt";
        Employee emp;
        int plusStock;

        private MainWindow mainWindow_w;
        Login login_w;
        SignUp signUp_w;
        ShoppingBasket shoppingBasket_w;
        InputCustomer inputCustomer_w;
        StockReturn stock;
        Emp_Sign_up emp_sign_up_Window;


        #region 통계 변수선언
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

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel(Login login_w)
        {
            db = new mbDB();
            this.login_w = login_w;

            uri = "//20.41.81.89";
            http_uri = "http:" + uri;
            ftp_uri = "ftp:" + uri + ":21";
            Label_First = "신규등록";

            #region 통계자료 초기화
            PointLabel = chartPoint =>
                   string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            PointLabel1 = chartPoint1 =>
                    string.Format("{0} ({1:P})", chartPoint1.Y, chartPoint1.Participation);

            PointLabel2 = chartPoint2 =>
                    string.Format("{0} ({1:P})", chartPoint2.Y, chartPoint2.Participation);

            Series = new SeriesCollection { };
            Setting_Chart();

            Labels = new[] { "Jan.", "Feb.", "Mar.", "Apr.", "May", "Jun.", "Jul.", "Aug.", "Sep.", "Oct.", "Nov.", "Dec." };
            #endregion


            List<string> list = new List<string>();
            List<string> list1 = new List<string>();

            list.Add("월별");
            list.Add("분기별");
            list.Add("년별");

            cb_PieChart1 = list;
            cb_PieChart2 = list;
            cb_PieChart3 = list;
            Index_P1 = 0;
            Index_P2 = 0;
            Index_P3 = 0;

            list1.Add("관리자");
            list1.Add("지원팀장");
            list1.Add("판매팀장");
            list1.Add("물류팀장");
            list1.Add("지원팀");
            list1.Add("판매팀");
            list1.Add("물류팀");
            select_Team_ItemSource = list1;
            RankItems = list1;

            cus_id_text_visible = Visibility.Hidden;
            Btn_search_res_Content = "등록";
            su_Emp_Name = "";

            #region 물류

            #region 물류 - 제품등록
            tb_lo_reg_objectnameContent = null;
            dp_lo_reg_inputdateContent = null;
            tb_lo_reg_objectcpuContent = null;
            tb_lo_reg_objectinchContent = null;
            tb_lo_reg_objectmAhContent = null;
            tb_lo_reg_objectramContent = null;
            tb_lo_reg_objectbrandContent = null;
            tb_lo_reg_objectcameraContent = null;
            tb_lo_reg_objectweightContent = null;
            tb_lo_reg_objectpriceContent = null;
            tb_lo_reg_objectdisplayContent = null;
            tb_lo_reg_objectmemoryContent = null;
            #endregion

            #region 물류 - 입고
            cb_lo_input_productnumberList = new List<int>();
            cb_lo_input_productnumberContent = -1;
            tb_lo_input_colorContent = null;
            tb_lo_input_numberOfContent = null;
            tb_lo_input_rgbContent = null;
            tb_lo_input_employeenumContent = null;
            cb_lo_input_inoutputContent = 0;
            tb_lo_input_numberOfContent = null;
            dp_lo_input_inputdateContent = null;
            cb_lo_input_inoutputContent = 0;
            Cb_Lo_Input_InOutputSelected = "입고";
            Tb_Lo_Input_RGB_ReadOnly = false;
            #endregion

            #region 물류 - 거래내역
            tb_lo_rse_productidContent = null;
            cb_lo_rse_inoutputContent = 0;
            tb_lo_rse_tradehistoryidContent = null;
            tb_lo_rse_colorContent = null;
            tb_lo_rse_productnameContent = null;
            dp_lo_rse_startdateContent = null;
            dp_lo_rse_enddateContent = null;
            #endregion

            #region 물류 - 제품조회
            tb_lo_pse_stockidContent = null;
            tb_lo_pse_productidContent = null;
            tb_lo_pse_productnameContent = null;
            tb_lo_pse_colorContent = null;
            #endregion

            #endregion

            #region 판매
            Se_search_ItemSourceProductID = new List<int>();
            Se_search_ItemSourceMemory = new List<int>();
            Se_search_ItemSourceProductBrand = new List<string>();
            Se_search_ItemSourceProductName = new List<string>();
            Se_search_ItemSourceRam = new List<int>();
            Se_history_ItemSourceProductName = new List<string>();
            this.Lv_se_product_infoItemSource = new List<Product>();
            this.lv_se_expect_sellItemSource = new List<Sell_Info>();
            this.Lv_se_sales_historyItemSource = new List<Sell_Info>();
            this.Lv_refundItemSource = new List<Sell_Info>();
            this.Lv_se_cus_searchItemSource = new List<Customer>();
            Lv_Emp_Search_ItemsSource = new List<Employee>();
            Lv_Emp_Sign_ItemsSource = new List<Sign_up>();
            Lv_Cus_Search_ItemsSource = new List<Customer>();
            cus_id_label_visible2 = Visibility.Hidden;
            cus_id_text_visible2 = Visibility.Hidden;
            #endregion


            if (File.Exists(path))
            {
                string[] value = File.ReadAllText(path).Split('#');
                Tb_login_idContent = value[0];
                emp = db.SelectEmpId(value[0]);

                if (value[0] == emp.Login_id && value[1] == emp.Login_pw)
                {
                    mainWindow_w = new MainWindow();
                    mainWindow_w.DataContext = this;
                    login_w.Close();
                    mainWindow_w.ShowDialog();
                }
            }
        }


        #region 로그인

        private ICommand login_MouseLeftDownCommand;
        public ICommand Login_MouseLeftDownCommand
        {
            get { return (this.login_MouseLeftDownCommand) ?? (this.login_MouseLeftDownCommand = new DelegateCommand(LoginWindow_move)); }
        }
        private void LoginWindow_move()
        {
            //login_w.DragMove();
        }

        private ICommand btn_closeCommand;
        public ICommand Btn_closeCommand
        {
            get { return (this.btn_closeCommand) ?? (this.btn_closeCommand = new DelegateCommand(ExitWindow)); }
        }
        private void ExitWindow()
        {
            if (MessageBox.Show("프로그램을 종료하시겠습니까?", "프로그램 종료", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                //this.Close();
            }
        }

        private string tb_login_idContent;
        public string Tb_login_idContent
        {
            get { return tb_login_idContent; }
            set
            {
                tb_login_idContent = Convert.ToString(value);
                OnPropertyChanged("Tb_login_idContent");
            }
        }

        private string login_password = "";
        private string tb_login_passwordContent;
        public string Tb_login_passwordContent
        {
            get { return tb_login_passwordContent; }
            set
            {
                if (value.Length != 0 && value[value.Length - 1] != ' ')
                {
                    tb_login_passwordContent = Convert.ToString(value);
                    OnPropertyChanged("Tb_login_passwordContent");

                    string star = "";
                    int ui_len = tb_login_passwordContent.Length;
                    int bg_len = login_password.Length;
                    if (ui_len > bg_len)
                    {
                        login_password += tb_login_passwordContent[ui_len - 1];
                    }
                    else
                    {
                        login_password = login_password.Substring(0, ui_len);
                    }
                    for (int i = 0; i < ui_len; i++)
                    {
                        star += '*';
                    }
                    tb_login_passwordContent = star;
                }
                else if (value.Length == 0)
                {
                    tb_login_passwordContent = "";
                    login_password = "";
                }
            }
        }

        private bool cb_autoContent = false;
        public bool Cb_autoContent
        {
            get
            {
                return cb_autoContent;
            }
            set
            {
                cb_autoContent = value;
                OnPropertyChanged("CheckedVar");
            }
        }

        private ICommand btn_loginCommand;
        public ICommand Btn_loginCommand
        {
            get { return (this.btn_loginCommand) ?? (this.btn_loginCommand = new DelegateCommand(ClickLogin)); }
        }
        private void ClickLogin()
        {
            if (Tb_login_idContent != null)
            {
                emp = db.SelectEmpId(Tb_login_idContent);
                try
                {
                    //id와 pw가 일치하면
                    if (Tb_login_idContent == emp.Login_id && sha256.ComputeSha256Hash(Tb_login_idContent + login_password) == emp.Login_pw)
                    {
                        //자동로그인 체크 검사
                        if (Cb_autoContent == true)
                        {
                            string value;
                            value = Tb_login_idContent + "#" + sha256.ComputeSha256Hash(Tb_login_idContent + login_password);
                            File.WriteAllText(path, value, Encoding.Default);
                        }
                        mainWindow_w = new MainWindow();
                        mainWindow_w.DataContext = this;
                        login_w.Close();
                        mainWindow_w.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("아이디나 비밀번호가 일치하지 않습니다", "로그인 에러");
                    }
                }
                catch
                {
                    MessageBox.Show("아이디와 비밀번호를 정확하게 입력해주세요.");
                }
            }

        }

        private ICommand btn_signupCommand;
        public ICommand Btn_signupCommand
        {
            get { return (this.btn_signupCommand) ?? (this.btn_signupCommand = new DelegateCommand(ClickSignup)); }
        }
        private void ClickSignup()
        {
            signUp_w = new SignUp();
            signUp_w.DataContext = this;
            signUp_w.ShowDialog();
        }

        private ICommand btn_findidCommand;
        public ICommand Btn_findidCommand
        {
            get { return (this.btn_findidCommand) ?? (this.btn_findidCommand = new DelegateCommand(FindIdInfo)); }
        }
        private void FindIdInfo()
        {
            MessageBox.Show("계정 생성문의나 비밀번호를 잊어버리신 경우 지원팀에 문의하십시오.", "Help");
        }

        #endregion


        #region 회원가입

        string checked_id = "";

        private ICommand signup_MouseLeftDownCommand;
        public ICommand Signup_MouseLeftDownCommand
        {
            get { return (this.signup_MouseLeftDownCommand) ?? (this.signup_MouseLeftDownCommand = new DelegateCommand(SignupWindow_move)); }
        }
        private void SignupWindow_move()
        {
            //signUp_w.DragMove();
        }


        private ICommand btn_signup_closeCommand;
        public ICommand Btn_signup_closeCommand
        {
            get { return (this.btn_signup_closeCommand) ?? (this.btn_signup_closeCommand = new DelegateCommand(ExitSignup)); }
        }
        private void ExitSignup()
        {
            signUp_w.Close();
            Tb_Signup_idContent = "";
            Tb_Signup_nameContent = "";
            Tb_Signup_social1Content = "";
            Tb_Signup_address1Content = "";
            Tb_Signup_address2Content = "";
            Tb_Signup_pw_check2Content = "";
            Cb_Signup_phone1Content = "010";
            Tb_Signup_phone2Content = "";
            Tb_Signup_phone3Content = "";
            Tb_Signup_email1Content = "";
            Tb_Signup_email2Content = "";
            Tb_Signup_social2Content = "";
            Tb_signup_password1Content = "";
            Tb_signup_password2Content = "";
            Color_Signup_pw_check1 = Brushes.Red;
            Color_Signup_pw_check2 = Brushes.Red;
            signup_password1 = "";
            signup_password2 = "";
            signup_social2 = "";
        }

        private string tb_Signup_idContent = "";
        public string Tb_Signup_idContent
        {
            get { return tb_Signup_idContent; }
            set
            {
                tb_Signup_idContent = value;
                OnPropertyChanged("Tb_Signup_idContent");
            }
        }

        private bool duplicateId = false;
        private ICommand btn_Signup_id_dupleCommand;
        public ICommand Btn_Signup_id_dupleCommand
        {
            get { return (this.btn_Signup_id_dupleCommand) ?? (this.btn_Signup_id_dupleCommand = new DelegateCommand(Check_duplicateId)); }
        }
        private void Check_duplicateId()
        {
            if (Tb_Signup_idContent == "")
            {
                duplicateId = false;
                MessageBox.Show("아이디를 입력해주세요");
            }
            else if (Tb_Signup_idContent.Length < 4)
            {
                duplicateId = false;
                MessageBox.Show("아이디는 4자 이상 16자 이하로 입력해주세요.");
            }
            else
            {
                if (db.Check_empID(Tb_Signup_idContent))
                {
                    duplicateId = true;
                    MessageBox.Show("사용 가능한 ID 입니다.");
                    checked_id = Tb_Signup_idContent;
                }
                else
                {
                    duplicateId = false;
                    MessageBox.Show("이미 존재하는 아이디 입니다.");
                }
            }
        }

        private string signup_password1 = "";
        private string tb_signup_password1Content;
        public string Tb_signup_password1Content
        {
            get { return tb_signup_password1Content; }
            set
            {
                if (value.Length != 0 && value[value.Length - 1] != ' ')
                {
                    tb_signup_password1Content = Convert.ToString(value);
                    OnPropertyChanged("Tb_signup_password1Content");

                    string star = "";
                    int ui_len = tb_signup_password1Content.Length;
                    int bg_len = signup_password1.Length;
                    if (ui_len > bg_len)
                    {
                        signup_password1 += tb_signup_password1Content[ui_len - 1];
                    }
                    else
                    {
                        signup_password1 = signup_password1.Substring(0, ui_len);
                    }
                    for (int i = 0; i < ui_len; i++)
                    {
                        star += '*';
                    }
                    tb_signup_password1Content = star;
                    if (ui_len >= 4 && ui_len <= 16)
                    {
                        Color_Signup_pw_check1 = Brushes.Green;

                        if (signup_password2 != "" && signup_password1 == signup_password2)
                        {
                            Tb_Signup_pw_check2Content = "V";
                            Color_Signup_pw_check2 = Brushes.Green;
                        }
                        else if (signup_password2 != "" && signup_password1 != signup_password2)
                        {
                            Tb_Signup_pw_check2Content = "V";
                            Color_Signup_pw_check2 = Brushes.Red;
                        }
                    }
                    else
                        Color_Signup_pw_check1 = Brushes.Red;

                }
                else if (value.Length == 0)
                {
                    tb_signup_password1Content = "";
                    signup_password1 = "";
                    Color_Signup_pw_check2 = Brushes.Red;
                }
            }
        }

        private string signup_password2 = "";
        private string tb_signup_password2Content;
        public string Tb_signup_password2Content
        {
            get { return tb_signup_password2Content; }
            set
            {
                if (value.Length != 0 && value[value.Length - 1] != ' ')
                {
                    tb_signup_password2Content = Convert.ToString(value);
                    OnPropertyChanged("Tb_signup_password2Content");

                    string star = "";
                    int ui_len = tb_signup_password2Content.Length;
                    int bg_len = signup_password2.Length;
                    if (ui_len > bg_len)
                    {
                        signup_password2 += tb_signup_password2Content[ui_len - 1];
                    }
                    else
                    {
                        signup_password2 = signup_password2.Substring(0, ui_len);
                    }
                    for (int i = 0; i < ui_len; i++)
                    {
                        star += '*';
                    }
                    tb_signup_password2Content = star;

                    if (color_Signup_pw_check1 == Brushes.Green && signup_password1 == signup_password2)
                    {
                        Tb_Signup_pw_check2Content = "V";
                        Color_Signup_pw_check2 = Brushes.Green;
                    }
                    else
                    {
                        Tb_Signup_pw_check2Content = "V";
                        Color_Signup_pw_check2 = Brushes.Red;
                    }
                }
                else if (value.Length == 0)
                {
                    tb_signup_password2Content = "";
                    signup_password2 = "";

                    Tb_Signup_pw_check2Content = "";
                    Color_Signup_pw_check2 = Brushes.Red;
                }
            }
        }

        private System.Windows.Media.Brush color_Signup_pw_check1 = Brushes.Red;
        public System.Windows.Media.Brush Color_Signup_pw_check1
        {
            get { return color_Signup_pw_check1; }
            set
            {
                color_Signup_pw_check1 = value;
                OnPropertyChanged("Color_Signup_pw_check1");
            }
        }

        private string tb_Signup_pw_check2Content = "";
        public string Tb_Signup_pw_check2Content
        {
            get { return tb_Signup_pw_check2Content; }
            set
            {
                tb_Signup_pw_check2Content = value;
                OnPropertyChanged("Tb_Signup_pw_check2Content");
            }
        }

        private System.Windows.Media.Brush color_Signup_pw_check2 = Brushes.Red;
        public System.Windows.Media.Brush Color_Signup_pw_check2
        {
            get { return color_Signup_pw_check2; }
            set
            {
                color_Signup_pw_check2 = value;
                OnPropertyChanged("Color_Signup_pw_check2");
            }
        }

        private string tb_Signup_nameContent = "";
        public string Tb_Signup_nameContent
        {
            get { return tb_Signup_nameContent; }
            set
            {
                tb_Signup_nameContent = value;
                OnPropertyChanged("Tb_Signup_nameContent");
            }
        }

        private string tb_Signup_social1Content = "";
        public string Tb_Signup_social1Content
        {
            get { return tb_Signup_social1Content; }
            set
            {
                tb_Signup_social1Content = value;
                OnPropertyChanged("Tb_Signup_social1Content");
            }
        }

        private string signup_social2 = "";
        private string tb_Signup_social2Content;
        public string Tb_Signup_social2Content
        {
            get { return tb_Signup_social2Content; }
            set
            {
                if (value.Length != 0 && value[value.Length - 1] != ' ')
                {
                    tb_Signup_social2Content = Convert.ToString(value);
                    OnPropertyChanged("Tb_Signup_social2Content");

                    string star = "";
                    int ui_len = tb_Signup_social2Content.Length;
                    int bg_len = signup_social2.Length;
                    if (ui_len > bg_len)
                    {
                        signup_social2 += tb_Signup_social2Content[ui_len - 1];
                    }
                    else
                    {
                        signup_social2 = signup_social2.Substring(0, ui_len);
                    }
                    for (int i = 0; i < ui_len; i++)
                    {
                        star += '*';
                    }
                    tb_Signup_social2Content = star;
                }
                else if (value.Length == 0)
                {
                    tb_Signup_social2Content = "";
                    signup_social2 = "";
                }
            }
        }

        private string tb_Signup_address1Content = "";
        public string Tb_Signup_address1Content
        {
            get { return tb_Signup_address1Content; }
            set
            {
                tb_Signup_address1Content = value;
                OnPropertyChanged("Tb_Signup_address1Content");
            }
        }

        private string tb_Signup_address2Content = "";
        public string Tb_Signup_address2Content
        {
            get { return tb_Signup_address2Content; }
            set
            {
                tb_Signup_address2Content = value;
                OnPropertyChanged("Tb_Signup_address2Content");
            }
        }

        private string cb_Signup_phone1Content = "010";
        public string Cb_Signup_phone1Content
        {
            get { return cb_Signup_phone1Content; }
            set
            {
                cb_Signup_phone1Content = value;
                OnPropertyChanged("Cb_Signup_phone1Content");
            }
        }

        private string tb_Signup_phone2Content = "";
        public string Tb_Signup_phone2Content
        {
            get { return tb_Signup_phone2Content; }
            set
            {
                tb_Signup_phone2Content = value;
                OnPropertyChanged("Tb_Signup_phone2Content");
            }
        }

        private string tb_Signup_phone3Content = "";
        public string Tb_Signup_phone3Content
        {
            get { return tb_Signup_phone3Content; }
            set
            {
                tb_Signup_phone3Content = value;
                OnPropertyChanged("Tb_Signup_phone3Content");
            }
        }

        private string tb_Signup_email1Content = "";
        public string Tb_Signup_email1Content
        {
            get { return tb_Signup_email1Content; }
            set
            {
                tb_Signup_email1Content = value;
                OnPropertyChanged("Tb_Signup_email1Content");
            }
        }

        private string tb_Signup_email2Content = "";
        public string Tb_Signup_email2Content
        {
            get { return tb_Signup_email2Content; }
            set
            {
                tb_Signup_email2Content = value;
                OnPropertyChanged("Tb_Signup_email2Content");
            }
        }

        private ICommand btn_Signup_submitCommand;
        public ICommand Btn_Signup_submitCommand
        {
            get { return (this.btn_Signup_submitCommand) ?? (this.btn_Signup_submitCommand = new DelegateCommand(Submit_signup)); }
        }
        private void Submit_signup()
        {
            if (!duplicateId)
            {
                MessageBox.Show("아이디 중복확인이 필요합니다.");
            }
            else if (Color_Signup_pw_check2 == Brushes.Red)
            {
                MessageBox.Show("비밀번호를 확인해주세요.");
            }
            else if (Tb_Signup_nameContent == "")
            {
                MessageBox.Show("이름을 정확하게 입력해주세요.");
            }
            else if (Tb_Signup_social1Content.Length != 6 || Tb_Signup_social2Content.Length != 7)
            {
                MessageBox.Show("주민등록번호를 정확하게 입력해주세요.");
            }
            else if (tb_Signup_address1Content == "" || tb_Signup_address2Content == "")
            {
                MessageBox.Show("주소를 정확하게 입력해주세요.");
            }
            else if (Cb_Signup_phone1Content == "" || Tb_Signup_phone2Content == "" || Tb_Signup_phone3Content == "")
            {
                MessageBox.Show("휴대전화번호를 정확하게 입력해주세요.");
            }
            else if (Tb_Signup_email1Content == "" || Tb_Signup_email2Content == "")
            {
                MessageBox.Show("이메일을 정확하게 입력해주세요.");
            }
            else
            {
                string gender;
                if (signup_social2[0] == '1' || signup_social2[0] == '3')
                {
                    gender = "남성";
                }
                else
                {
                    gender = "여성";
                }

                string pw = sha256.ComputeSha256Hash(checked_id + signup_password1);
                string social = Tb_Signup_social1Content + "-" + signup_social2;
                string phone = Cb_Signup_phone1Content + Tb_Signup_phone2Content + Tb_Signup_phone3Content;
                string post = Tb_Signup_address1Content;
                string address = Tb_Signup_address2Content;
                string email = Tb_Signup_email1Content + "@" + Tb_Signup_email2Content;
                string name = Tb_Signup_nameContent;
                try
                {
                    if (db.Insert_SignUp(name, checked_id, pw, gender, social, phone, post, address, email, DateTime.Now))
                    {
                        MessageBox.Show("성공적으로 가입신청 되었습니다.");
                        ExitSignup();
                    }
                }
                catch
                {
                    MessageBox.Show("가입신청에 실패했습니다!");
                }
            }
        }

        #endregion


        #region 판매

        #region 판매-제품조회

        private string Se_search_productIDContent;
        public string Se_search_ProductIDContent
        {
            get { return Se_search_productIDContent; }
            set
            {
                Se_search_productIDContent = Convert.ToString(value);
                OnPropertyChanged("Se_search_ProductIDContent");
            }
        }

        private string Se_search_productNameContent;
        public string Se_search_ProductNameContent
        {
            get { return Se_search_productNameContent; }
            set
            {
                Se_search_productNameContent = Convert.ToString(value);
                OnPropertyChanged("Se_search_ProductNameContent");
            }
        }

        private string Se_search_brandContent;
        public string Se_search_BrandContent
        {
            get { return Se_search_brandContent; }
            set
            {
                Se_search_brandContent = Convert.ToString(value);
                OnPropertyChanged("Se_search_BrandContent");
            }
        }

        private string Se_search_memoryContent;
        public string Se_search_MemoryContent
        {
            get { return Se_search_memoryContent; }
            set
            {
                Se_search_memoryContent = Convert.ToString(value);
                OnPropertyChanged("Se_search_MemoryContent");
            }
        }

        private string Se_search_priceContent;
        public string Se_search_PriceContent
        {
            get { return Se_search_priceContent; }
            set
            {
                Se_search_priceContent = Convert.ToString(value);
                OnPropertyChanged("Se_search_PriceContent");
            }
        }

        private string Se_search_manufactureContent;
        public string Se_search_ManufactureContent
        {
            get { return Se_search_manufactureContent; }
            set
            {
                Se_search_manufactureContent = Convert.ToString(value);
                OnPropertyChanged("Se_search_ManufactureContent");
            }
        }

        private string Se_search_ramContent;
        public string Se_search_RamContent
        {
            get { return Se_search_ramContent; }
            set
            {
                Se_search_ramContent = Convert.ToString(value);
                OnPropertyChanged("Se_search_RamContent");
            }
        }


        #region 콤보박스 ItemSource
        private List<int> Se_search_itemSourceProductID;
        public List<int> Se_search_ItemSourceProductID
        {
            get { return Se_search_itemSourceProductID; }
            set
            {
                Se_search_itemSourceProductID = db.SelectStockProductId();
            }
        }

        private List<string> Se_search_itemSourceProductName;
        public List<string> Se_search_ItemSourceProductName
        {
            get { return Se_search_itemSourceProductName; }
            set
            {
                Se_search_itemSourceProductName = db.SelectStockName();
            }
        }

        private List<string> Se_search_itemSourceProductBrand;
        public List<string> Se_search_ItemSourceProductBrand
        {
            get { return Se_search_itemSourceProductBrand; }
            set
            {
                Se_search_itemSourceProductBrand = db.SelectStockBrand();
            }
        }

        private List<int> Se_search_itemSourceMemory;
        public List<int> Se_search_ItemSourceMemory
        {
            get { return Se_search_itemSourceMemory; }
            set
            {
                Se_search_itemSourceMemory = db.SelectStockMemory();
            }
        }

        private List<int> Se_search_itemSourceRam;
        public List<int> Se_search_ItemSourceRam
        {
            get { return Se_search_itemSourceRam; }
            set
            {
                Se_search_itemSourceRam = db.SelectStockRam();
            }
        }
        #endregion


        public bool CheckNullorEmpty(ref string s)
        {
            if (s == null || s == "")
            {
                s = null;
                return true;
            }
            else
                return false;
        }

        private ICommand btn_se_SearchCommand;
        public ICommand Btn_se_SearchCommand
        {
            get { return (this.btn_se_SearchCommand) ?? (this.btn_se_SearchCommand = new DelegateCommand(PrintProduct)); }
        }

        private void PrintProduct()
        {
            List<Product> proList = new List<Product>();
            string query = "";
            try
            {
                if (!CheckNullorEmpty(ref Se_search_productIDContent))
                {
                    query += " AND p.product_id = @Product_id";
                }

                if (!CheckNullorEmpty(ref Se_search_productNameContent))
                {
                    query += " AND name Like @Name";
                }

                if (!CheckNullorEmpty(ref Se_search_brandContent))
                {
                    query += " AND brand Like @Brand";
                }

                if (!CheckNullorEmpty(ref Se_search_memoryContent))
                {
                    query += " AND memory = @Memory";
                }

                if (!CheckNullorEmpty(ref Se_search_priceContent))
                {
                    if (Btn_Se_search_priceContent == "▲")
                        query += " AND price >= @Price";
                    else
                        query += " AND price <= @Price";
                }

                if (!CheckNullorEmpty(ref Se_search_manufactureContent))
                {
                    if (Btn_Se_search_manufactureContent == "▲")
                        query += " AND manufacture >= @Manufacture";
                    else
                        query += " AND manufacture <= @Manufacture";
                }

                if (!CheckNullorEmpty(ref Se_search_ramContent))
                {
                    query += " AND ram = @Ram";
                }

                proList = db.SelectProduct(Convert.ToInt32(Se_search_productIDContent), Convert.ToInt32(Se_search_memoryContent), Convert.ToInt32(Se_search_priceContent), Convert.ToInt32(Se_search_ramContent), Se_search_productNameContent, Se_search_brandContent, Convert.ToDateTime(Se_search_manufactureContent), query);

                Lv_se_product_infoItemSource = proList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand btn_Se_search_PriceBtnCommand;
        public ICommand Btn_Se_search_PriceBtnCommand
        {
            get { return (this.btn_Se_search_PriceBtnCommand) ?? (this.btn_Se_search_PriceBtnCommand = new DelegateCommand(SetPriceBtn)); }
        }
        private void SetPriceBtn()
        {
            if (Btn_Se_search_priceContent == "▲")
                Btn_Se_search_priceContent = "▼";
            else
                Btn_Se_search_priceContent = "▲";
        }

        private string btn_Se_search_priceContent = "▲";
        public string Btn_Se_search_priceContent
        {
            get
            {
                return btn_Se_search_priceContent;
            }
            set
            {
                btn_Se_search_priceContent = Convert.ToString(value);
                OnPropertyChanged("Btn_Se_search_priceContent");
            }
        }


        private ICommand btn_Se_search_ManufactureBtnCommand;
        public ICommand Btn_Se_search_ManufactureBtnCommand
        {
            get { return (this.btn_Se_search_ManufactureBtnCommand) ?? (this.btn_Se_search_ManufactureBtnCommand = new DelegateCommand(SetManufactureBtn)); }
        }
        private void SetManufactureBtn()
        {
            if (Btn_Se_search_manufactureContent == "▲")
                Btn_Se_search_manufactureContent = "▼";
            else
                Btn_Se_search_manufactureContent = "▲";
        }

        private string btn_Se_search_manufactureContent = "▲";
        public string Btn_Se_search_manufactureContent
        {
            get
            {
                return btn_Se_search_manufactureContent;
            }
            set
            {
                btn_Se_search_manufactureContent = Convert.ToString(value);
                OnPropertyChanged("Btn_Se_search_manufactureContent");
            }
        }


        private ICommand btn_se_ResetCommand;
        public ICommand Btn_se_ResetCommand
        {
            get { return (this.btn_se_ResetCommand) ?? (this.btn_se_ResetCommand = new DelegateCommand(ResetSearchCondition)); }
        }
        private void ResetSearchCondition()
        {
            Se_search_ProductIDContent = null;
            Se_search_ProductNameContent = null;
            Se_search_BrandContent = null;
            Se_search_MemoryContent = null;
            Se_search_PriceContent = null;
            Se_search_ManufactureContent = null;
            Se_search_RamContent = null;
            Btn_Se_search_manufactureContent = "▲";
            Btn_Se_search_priceContent = "▲";
            Lv_se_product_infoItemSource = null;
        }

        private List<Product> lv_se_product_infoItemSource;
        public List<Product> Lv_se_product_infoItemSource
        {
            get
            {
                return lv_se_product_infoItemSource;
            }
            set
            {
                lv_se_product_infoItemSource = value;
                OnPropertyChanged("Lv_se_product_infoItemSource");
            }
        }


        private ICommand doubleClick_product_info_Command;
        public ICommand DoubleClick_product_info_Command
        {
            get { return (this.doubleClick_product_info_Command) ?? (this.doubleClick_product_info_Command = new DelegateCommand(Product_info_DoubleClick)); }
        }
        private void Product_info_DoubleClick()
        {
            if (Lv_se_product_info_SelectedItem != null)
            {
                Product product = Lv_se_product_info_SelectedItem;
                shoppingBasket_w = new ShoppingBasket();
                shoppingBasket_w.DataContext = this;
                shoppingBasket_w.ShowDialog();
            }
        }

        private Product lv_se_product_info_SelectedItem;
        public Product Lv_se_product_info_SelectedItem
        {
            get
            {
                return lv_se_product_info_SelectedItem;
            }
            set
            {
                lv_se_product_info_SelectedItem = value;
            }

        }



        #endregion


        #region 판매-장바구니

        public BitmapImage Select_productImg_fSource
        {
            get
            {
                try
                {
                    return new BitmapImage(new Uri(http_uri + "/phone/" + Lv_se_product_info_SelectedItem.Name.Replace("+", "plus") + "_" + Lv_se_product_info_SelectedItem.Color + "_F.JPG", UriKind.Absolute));
                }
                catch
                {
                    return null;
                }
            }
        }

        public BitmapImage Select_productImg_bSource
        {
            get
            {
                try
                {
                    return new BitmapImage(new Uri(@http_uri + "/phone/" + Lv_se_product_info_SelectedItem.Name.Replace("+", "plus") + "_" + Lv_se_product_info_SelectedItem.Color + "_B.JPG", UriKind.Absolute));
                }
                catch
                {
                    return null;
                }
            }
        }

        public string Select_productColorContent
        {
            get
            {
                if (Lv_se_product_info_SelectedItem != null)
                    return "(" + Lv_se_product_info_SelectedItem.Color + ")";
                else
                    return "";
            }
        }

        public BitmapImage Select_productBrandSource
        {
            get
            {
                try
                {
                    return new BitmapImage(new Uri(http_uri + " /brand/" + Lv_se_product_info_SelectedItem.Brand + ".png", UriKind.Absolute));
                }
                catch
                {
                    return null;
                }
            }
        }

        public string Select_productManufactureContent
        {
            get
            {
                if (Lv_se_product_info_SelectedItem != null)
                    return ((DateTime)Lv_se_product_info_SelectedItem.Manufacture).ToString("yyyy. MM");
                else
                    return "";
            }
        }

        public string Select_productInchContent
        {
            get
            {
                if (Lv_se_product_info_SelectedItem != null)
                    return Lv_se_product_info_SelectedItem.Inch + " inch";
                else
                    return "";
            }
        }

        public string Select_productRamContent
        {
            get
            {
                if (Lv_se_product_info_SelectedItem != null)
                    return Lv_se_product_info_SelectedItem.Ram + " GB";
                else
                    return "";
            }
        }

        public string Select_productCpuContent
        {
            get
            {
                if (Lv_se_product_info_SelectedItem != null)
                    return Lv_se_product_info_SelectedItem.Cpu;
                else
                    return "";
            }
        }

        public string Select_productDisplayContent
        {
            get
            {
                if (Lv_se_product_info_SelectedItem != null)
                    return Lv_se_product_info_SelectedItem.Display;
                else
                    return "";
            }
        }

        public string Select_productMemoryContent
        {
            get
            {
                if (Lv_se_product_info_SelectedItem != null)
                    return Lv_se_product_info_SelectedItem.Memory + " GB";
                else
                    return "";
            }
        }

        public string Select_productCameraContent
        {
            get
            {
                if (Lv_se_product_info_SelectedItem != null)
                    return Lv_se_product_info_SelectedItem.Camera + " MP";
                else
                    return "";
            }
        }

        public string Select_productWeightContent
        {
            get
            {
                if (Lv_se_product_info_SelectedItem != null)
                    return Lv_se_product_info_SelectedItem.Weight + " g";
                else
                    return "";
            }
        }

        public string Select_productMahContent
        {
            get
            {
                if (Lv_se_product_info_SelectedItem != null)
                    return Lv_se_product_info_SelectedItem.MAh + " mAh";
                else
                    return "";
            }
        }

        private int basket_quantityContent = 1;
        public int Basket_quantityContent
        {
            get { return basket_quantityContent; }
            set
            {
                basket_quantityContent = Convert.ToInt32(value);
                OnPropertyChanged("Basket_quantityContent");
            }
        }

        private ICommand btn_basket_enrollCommand;
        public ICommand Btn_basket_enrollCommand
        {
            get { return (this.btn_basket_enrollCommand) ?? (this.btn_basket_enrollCommand = new DelegateCommand(Set_sellList)); }
        }
        private void Set_sellList()
        {
            if (Basket_quantityContent > Lv_se_product_info_SelectedItem.Stock)
                MessageBox.Show("재고부족");
            else
            {
                MessageBox.Show("판매목록에 추가 되었습니다");

                List<Sell_Info> sell_Info_list = new List<Sell_Info>();
                bool duple = false;
                for (int i = 0; i < Lv_se_expect_sellItemSource.Count; i++)
                {
                    Sell_Info sell_Info = (Sell_Info)Lv_se_expect_sellItemSource[i];
                    sell_Info_list.Add(sell_Info);
                }
                for (int i = 0; i < sell_Info_list.Count; i++)
                {
                    if (sell_Info_list[i].Stock_product == Lv_se_product_info_SelectedItem.Stock_product)
                    {
                        sell_Info_list[i].Quantity += Basket_quantityContent;
                        sell_Info_list[i].Total_price = sell_Info_list[i].Quantity * Lv_se_product_info_SelectedItem.Price;
                        duple = true;
                        break;
                    }
                }

                if (!duple)
                {
                    sell_Info_list.Add(new Sell_Info()
                    {
                        Stock_product = Lv_se_product_info_SelectedItem.Stock_product,
                        Product_id = Lv_se_product_info_SelectedItem.Product_id,
                        Product_name = Lv_se_product_info_SelectedItem.Name,
                        Color = Lv_se_product_info_SelectedItem.Color,
                        ColorValue = Lv_se_product_info_SelectedItem.ColorValue,
                        Quantity = Basket_quantityContent,
                        Total_price = Lv_se_product_info_SelectedItem.Price * Basket_quantityContent
                    });
                }
                Lv_se_expect_sellItemSource = sell_Info_list;

                string str_total_price = Sell_total_priceContent;
                str_total_price = str_total_price.Substring(0, str_total_price.Length - 2);
                long total_price = long.Parse(str_total_price, NumberStyles.AllowThousands);
                total_price += Lv_se_product_info_SelectedItem.Price * Basket_quantityContent;
                Sell_total_priceContent = string.Format("{0:#,##0}", total_price) + " 원";

                Basket_quantityContent = 1;
                shoppingBasket_w.Close();
            }
        }
        #endregion


        #region 판매-제품판매

        private List<Sell_Info> lv_se_expect_sellItemSource;
        public List<Sell_Info> Lv_se_expect_sellItemSource
        {
            get
            {
                return lv_se_expect_sellItemSource;
            }
            set
            {
                lv_se_expect_sellItemSource = value;
                OnPropertyChanged("Lv_se_expect_sellItemSource");
            }
        }

        private string sell_total_priceContent = "0 원";
        public string Sell_total_priceContent
        {
            get { return sell_total_priceContent; }
            set
            {
                sell_total_priceContent = Convert.ToString(value);
                OnPropertyChanged("Sell_total_priceContent");
            }
        }

        private ICommand btn_expect_sell_resetCommand;
        public ICommand Btn_expect_sell_resetCommand
        {
            get { return (this.btn_expect_sell_resetCommand) ?? (this.btn_expect_sell_resetCommand = new DelegateCommand(Reset_expect_sell)); }
        }
        private void Reset_expect_sell()
        {
            Lv_se_expect_sellItemSource = new List<Sell_Info>();
            Sell_total_priceContent = "0 원";
        }

        private ICommand btn_expect_sell_enroll_Command;
        public ICommand Btn_expect_sell_enroll_Command
        {
            get { return (this.btn_expect_sell_enroll_Command) ?? (this.btn_expect_sell_enroll_Command = new DelegateCommand(Enroll_expect_sell)); }
        }
        private void Enroll_expect_sell()
        {
            if (Lv_se_expect_sellItemSource.Count != 0)
            {
                inputCustomer_w = new InputCustomer();
                inputCustomer_w.DataContext = this;
                Se_Customer_infoContent = null;
                Customer_phoneContent = "";
                inputCustomer_w.ShowDialog();
            }
        }
        #endregion


        #region 판매-고객마일리지

        private string customer_phoneContent;
        public string Customer_phoneContent
        {
            get { return customer_phoneContent; }
            set
            {
                customer_phoneContent = Convert.ToString(value);
                OnPropertyChanged("Customer_phoneContent");
            }
        }

        private ICommand btn_customer_phone_SearchCommand;
        public ICommand Btn_customer_phone_SearchCommand
        {
            get { return (this.btn_customer_phone_SearchCommand) ?? (this.btn_customer_phone_SearchCommand = new DelegateCommand(Customer_phone_Search)); }
        }
        private void Customer_phone_Search()
        {
            List<Customer> cusList = new List<Customer>();

            cusList = db.SelectCustomer(Customer_phoneContent);
            if (cusList.Count != 0)
                Se_Customer_infoContent = cusList;
            else
                MessageBox.Show("찾으시는 데이터가 없습니다");
        }

        private List<Customer> se_Customer_infoContent;
        public List<Customer> Se_Customer_infoContent
        {
            get
            {
                return se_Customer_infoContent;
            }
            set
            {
                se_Customer_infoContent = value;
                OnPropertyChanged("Se_Customer_infoContent");
            }
        }

        public Customer lv_se_Customer_info_SelectedItem;
        public Customer Lv_se_Customer_info_SelectedItem
        {
            get { return lv_se_Customer_info_SelectedItem; }
            set { lv_se_Customer_info_SelectedItem = value; }
        }

        private string add_savingContent;
        public string Add_savingContent
        {
            get
            {
                string str_total_price = Sell_total_priceContent;
                str_total_price = str_total_price.Substring(0, str_total_price.Length - 2);
                long total_price = long.Parse(str_total_price, NumberStyles.AllowThousands);
                long saving = total_price / 100;
                return string.Format("{0:#,##0}", saving);
            }
            set {; }
        }

        private ICommand btn_final_sell_enrollCommand;
        public ICommand Btn_final_sell_enrollCommand
        {
            get { return (this.btn_final_sell_enrollCommand) ?? (this.btn_final_sell_enrollCommand = new DelegateCommand(Enroll_final_sell)); }
        }
        private void Enroll_final_sell()
        {
            Customer customer_info = Lv_se_Customer_info_SelectedItem;
            if (Lv_se_Customer_info_SelectedItem != null)
            {
                if (MessageBox.Show("판매 하시겠습니까?", "알림창", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    List<Sell_Info> sell_list = new List<Sell_Info>();

                    try
                    {
                        for (int i = 0; i < Lv_se_expect_sellItemSource.Count; i++)
                        {
                            Sell_Info item = Lv_se_expect_sellItemSource[i];

                            sell_list.Add(item);
                            int stock = db.SelectStockProductStock(item.Stock_product);
                            if (item.Quantity > stock)
                            {
                                throw new Exception("재고가 부족합니다");
                            }
                        }

                        string str_saving_price = Add_savingContent;
                        long saving = long.Parse(str_saving_price, NumberStyles.AllowThousands);

                        db.sell_transaction(sell_list, customer_info.Id, emp.Employee_id, DateTime.Now, saving);

                        MessageBox.Show("판매가 완료 되었습니다", "알림창");

                    }
                    catch (Exception ex)
                    {
                        if (ex.Message == "재고")
                            MessageBox.Show("재고부족");
                        else
                            MessageBox.Show("오류");
                    }
                    Reset_expect_sell();
                    inputCustomer_w.Close();
                }
            }
            else
            {
                MessageBox.Show("고객을 선택 하세요");
            }
        }
        #endregion


        #region 판매-내역조회

        private string se_history_CustomerNameContent;
        public string Se_history_CustomerNameContent
        {
            get { return se_history_CustomerNameContent; }
            set
            {
                se_history_CustomerNameContent = Convert.ToString(value);
                OnPropertyChanged("Se_history_CustomerNameContent");
            }
        }

        private string se_history_EmployeeNameContent;
        public string Se_history_EmployeeNameContent
        {
            get { return se_history_EmployeeNameContent; }
            set
            {
                se_history_EmployeeNameContent = Convert.ToString(value);
                OnPropertyChanged("Se_history_EmployeeNameContent");
            }
        }

        private string se_history_Sold_s_dateContent;
        public string Se_history_Sold_s_dateContent
        {
            get { return se_history_Sold_s_dateContent; }
            set
            {
                se_history_Sold_s_dateContent = Convert.ToString(value);
                OnPropertyChanged("Se_history_Sold_s_dateContent");
            }
        }

        private string se_history_Sold_e_dateContent;
        public string Se_history_Sold_e_dateContent
        {
            get { return se_history_Sold_e_dateContent; }
            set
            {
                se_history_Sold_e_dateContent = Convert.ToString(value);
                OnPropertyChanged("Se_history_Sold_e_dateContent");
            }
        }

        private string se_history_typeContent = "모두";
        public string Se_history_typeContent
        {
            get { return se_history_typeContent; }
            set
            {
                se_history_typeContent = Convert.ToString(value);
                OnPropertyChanged("Se_history_typeContent");
            }
        }

        private string se_history_ProductNameContent;
        public string Se_history_ProductNameContent
        {
            get { return se_history_ProductNameContent; }
            set
            {
                se_history_ProductNameContent = Convert.ToString(value);
                OnPropertyChanged("Se_history_ProductNameContent");
            }
        }

        #region 콤보박스 ItemSource

        private List<string> Se_history_itemSourceProductName;
        public List<string> Se_history_ItemSourceProductName
        {
            get { return Se_history_itemSourceProductName; }
            set
            {
                Se_history_itemSourceProductName = db.SelectSalesname();
            }
        }

        #endregion

        private ICommand btn_se_Salse_Search_historyCommand;
        public ICommand Btn_se_Salse_Search_historyCommand
        {
            get { return (this.btn_se_Salse_Search_historyCommand) ?? (this.btn_se_Salse_Search_historyCommand = new DelegateCommand(PrintSalesHistory)); }
        }
        private void PrintSalesHistory()
        {
            string query = "";
            List<Sell_Info> sellinfoList = new List<Sell_Info>();
            try
            {
                if (!CheckNullorEmpty(ref se_history_CustomerNameContent))
                {
                    query += " AND c.name = @Customer_name";
                }
                else
                    Se_history_CustomerNameContent = "";

                if (!CheckNullorEmpty(ref se_history_EmployeeNameContent))
                {
                    query += " AND e.name = @Employee_name";
                }
                else
                    Se_history_EmployeeNameContent = "";

                if (se_history_typeContent != "모두")
                {
                    query += " AND sp.type = @Type";
                }

                if (!CheckNullorEmpty(ref se_history_Sold_s_dateContent))
                {
                    query += " AND sh.sales_date >= @Sales_s_date";
                }

                if (!CheckNullorEmpty(ref se_history_Sold_e_dateContent))
                {
                    query += " AND sh.sales_date <= @Sales_e_date";
                }

                if (!CheckNullorEmpty(ref se_history_ProductNameContent))
                {
                    query += " AND p.name = @Product_name";
                }
                else
                    Se_history_ProductNameContent = "";

                if (query != "")    //안 비어있을때
                {
                    query = query.Substring(5);
                    query = "WHERE " + query;
                }

                sellinfoList = db.SelectSalesHistory(se_history_CustomerNameContent, se_history_EmployeeNameContent, se_history_typeContent, Convert.ToDateTime(se_history_Sold_s_dateContent), Convert.ToDateTime(se_history_Sold_e_dateContent), -1, se_history_ProductNameContent, query);
                if (sellinfoList.Count != 0)
                {
                    Lv_se_sales_historyItemSource = sellinfoList;
                }
                else
                    MessageBox.Show("찾으시는 데이터가 없습니다");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private ICommand btn_se_Salse_Reset_historyCommand;
        public ICommand Btn_se_Salse_Reset_historyCommand
        {
            get { return (this.btn_se_Salse_Reset_historyCommand) ?? (this.btn_se_Salse_Reset_historyCommand = new DelegateCommand(ResetSalesHistory)); }
        }
        private void ResetSalesHistory()
        {
            Se_history_CustomerNameContent = null;
            Se_history_EmployeeNameContent = null;
            Se_history_Sold_s_dateContent = null;
            Se_history_Sold_e_dateContent = null;
            Se_history_typeContent = "모두";
            Se_history_ProductNameContent = null;
            Lv_se_sales_historyItemSource = null;
        }

        private List<Sell_Info> lv_se_sales_historyItemSource;
        public List<Sell_Info> Lv_se_sales_historyItemSource
        {
            get { return lv_se_sales_historyItemSource; }
            set
            {
                lv_se_sales_historyItemSource = value;
                OnPropertyChanged("Lv_se_sales_historyItemSource");
            }
        }

        private Sell_Info lv_se_slaes_history_SelectedItem;
        public Sell_Info Lv_se_slaes_history_SelectedItem
        {
            get { return lv_se_slaes_history_SelectedItem; }
            set { lv_se_slaes_history_SelectedItem = value; }
        }


        private ICommand doubleClick_sales_history_Command;
        public ICommand DoubleClick_sales_history_Command
        {
            get { return (this.doubleClick_sales_history_Command) ?? (this.doubleClick_sales_history_Command = new DelegateCommand(Sales_history_DoubleClick)); }
        }
        private void Sales_history_DoubleClick()
        {
            if (Lv_se_slaes_history_SelectedItem != null)
            {
                if (Lv_se_slaes_history_SelectedItem.Sales_date.AddDays(7) >= DateTime.Now && Lv_se_slaes_history_SelectedItem.Sales_type == "판매" && !Lv_se_slaes_history_SelectedItem.Refunded)
                {
                    Sell_Info sell_Info = Lv_se_slaes_history_SelectedItem;
                    if (MessageBox.Show("환불 목록에 추가하시겠습니까?", "알림창", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        List<Sell_Info> refundList = new List<Sell_Info>();
                        string query = "WHERE sh.sales_history_id = @Sales_hisory_id";
                        try
                        {
                            long total_price = 0;

                            refundList = db.SelectSalesHistory("", "", "", DateTime.Now, DateTime.Now, Lv_se_slaes_history_SelectedItem.Sales_history_id, "", query);
                            Lv_refundItemSource = refundList;

                            for (int i = 0; i < refundList.Count; i++)
                                total_price += refundList[i].Total_price;

                            Se_refund_total_priceContent = string.Format("{0:#,##0}", total_price) + " 원";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                }
                else
                {
                    MessageBox.Show("환불이 불가능한 제품입니다");
                }
            }
        }
        #endregion


        #region 판매 - 환불

        private List<Sell_Info> lv_refundItemSource;
        public List<Sell_Info> Lv_refundItemSource
        {
            get { return lv_refundItemSource; }
            set
            {
                lv_refundItemSource = value;
                OnPropertyChanged("Lv_refundItemSource");
            }
        }


        private string se_refund_total_priceContent = "0 원";
        public string Se_refund_total_priceContent
        {
            get { return se_refund_total_priceContent; }
            set
            {
                se_refund_total_priceContent = value;
                OnPropertyChanged("Se_refund_total_priceContent");
            }
        }

        private ICommand btn_se_refund_list_resetCommand;
        public ICommand Btn_se_refund_list_resetCommand
        {
            get { return (this.btn_se_refund_list_resetCommand) ?? (this.btn_se_refund_list_resetCommand = new DelegateCommand(Reset_refund_list)); }
        }
        private void Reset_refund_list()
        {
            Lv_refundItemSource = null;
            Se_refund_total_priceContent = "0 원";
        }

        private ICommand btn_se_refundCommand;
        public ICommand Btn_se_refundCommand
        {
            get { return (this.btn_se_refundCommand) ?? (this.btn_se_refundCommand = new DelegateCommand(Enroll_refund_list)); }
        }
        private void Enroll_refund_list()
        {
            if (MessageBox.Show("환불 하시겠습니까?", "알림창", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    long savings = 0;
                    for (int i = 0; i < Lv_refundItemSource.Count; i++)
                    {
                        savings += Lv_refundItemSource[i].Total_price;
                    }
                    savings /= 100;
                    db.transaction_refund(Lv_refundItemSource[0].Sales_history_id, Lv_refundItemSource[0].Customer_id, emp.Employee_id, DateTime.Now, Lv_refundItemSource, Lv_refundItemSource[0].Customer_id, savings);

                    MessageBox.Show("환불이 완료 되었습니다", "알림창");
                }
                catch
                {
                    MessageBox.Show("오류");
                }
                Reset_refund_list();
            }
        }


        #endregion


        #region 판매 - 고객관리

        private void Check_and2(int cnt)
        {
            cnt++;
            and_ck2 = cnt;
            if (and_ck2 > 1) qr2 += " and";
        }

        string qr2 = "";
        int and_ck2 = 0;

        private string _tb_Cus_Id2;
        public string tb_Cus_Id2
        {
            get { return _tb_Cus_Id2; }
            set
            {
                _tb_Cus_Id2 = value;
                OnPropertyChanged("tb_Cus_Id2");
            }
        }

        private string _tb_Cus_Name2;
        public string tb_Cus_Name2
        {
            get { return _tb_Cus_Name2; }
            set
            {
                _tb_Cus_Name2 = value;
                OnPropertyChanged("tb_Cus_Name2");
            }
        }

        private bool _Gender_Check_M2;
        public bool Gender_Check_M2
        {
            get { return _Gender_Check_M2; }
            set
            {
                _Gender_Check_M2 = value;
                OnPropertyChanged("Gender_Check_M2");
            }
        }

        private bool _Gender_Check_F2;
        public bool Gender_Check_F2
        {
            get { return _Gender_Check_F2; }
            set
            {
                _Gender_Check_F2 = value;
                OnPropertyChanged("Gender_Check_F2");
            }
        }

        private string _tb_Cus_Phone2;
        public string tb_Cus_Phone2
        {
            get { return _tb_Cus_Phone2; }
            set
            {
                _tb_Cus_Phone2 = value;
                OnPropertyChanged("tb_Cus_Phone2");
            }
        }

        private DateTime? _Cus_Birth_Content2;
        public DateTime? Cus_Birth_Content2
        {
            get { return _Cus_Birth_Content2; }
            set
            {
                _Cus_Birth_Content2 = value;
                OnPropertyChanged("Cus_Birth_Content2");
            }
        }

        private string _tb_Cus_Savings2;
        public string tb_Cus_Savings2
        {
            get { return _tb_Cus_Savings2; }
            set
            {
                _tb_Cus_Savings2 = value;
                OnPropertyChanged("tb_Cus_Savings2");
            }
        }

        private string cus_name_search2;
        public string Cus_Name_Search2
        {
            get { return cus_name_search2; }
            set
            {
                cus_name_search2 = value;
                OnPropertyChanged("Cus_Name_Search2");
            }
        }

        private string cus_gender_search2;
        public string Cus_Gender_Search2
        {
            get { return cus_gender_search2; }
            set
            {
                cus_gender_search2 = value;
                OnPropertyChanged("Cus_Gender_Search2");
            }
        }

        private string cus_phone_search2;
        public string Cus_Phone_Search2
        {
            get { return cus_phone_search2; }
            set
            {
                cus_phone_search2 = value;
                OnPropertyChanged("Cus_Phone_Search2");
            }
        }

        private string _Label_First2 = "신규등록";
        public string Label_First2
        {
            get { return _Label_First2; }
            set
            {
                _Label_First2 = value;
                OnPropertyChanged("Label_First2");
            }
        }

        private string btn_search_res_content2 = "등록";
        public string Btn_search_res_Content2
        {
            get { return btn_search_res_content2; }
            set
            {
                btn_search_res_content2 = value;
                OnPropertyChanged("Btn_search_res_Content2");
            }
        }

        private Customer lv_selectitem2;
        public Customer Lv_SelectItem2
        {
            get { return lv_selectitem2; }
            set
            {
                lv_selectitem2 = value;
                OnPropertyChanged("lv_selectitem2");
            }
        }

        #region 고객관리 - 등록/변경 버튼
        private ICommand _Btn_su_cus_register2;
        public ICommand Btn_su_cus_register2
        {
            get { return (this._Btn_su_cus_register2) ?? (this._Btn_su_cus_register2 = new DelegateCommand(Cus_Register_Change2)); }
        }

        private int res_change_check2 = 0;
        void Cus_Register_Change2()
        {
            string gen = string.Empty;

            if (res_change_check2 == 0)//신규등록
            {
                try
                {
                    if (Gender_Check_M2 == false && Gender_Check_F2 == true)
                    { gen = "여성"; }
                    else if (Gender_Check_M2 == true && Gender_Check_F2 == false)
                    { gen = "남성"; }

                    if (tb_Cus_Savings2 == null)
                    {
                        tb_Cus_Savings2 = "0";
                    }

                    db.Insert_Cus_Info(tb_Cus_Name2, gen, Cus_Birth_Content2
                        , tb_Cus_Phone2, long.Parse(tb_Cus_Savings2));
                    Cus_All_Reset2();
                    MessageBox.Show("완료");
                }
                catch
                {
                    MessageBox.Show("입력되지 않은 값이 있습니다");
                }
            }
            else
            {
                try
                {
                    if (Gender_Check_M2 == false && Gender_Check_F2 == true)
                    { gen = "여성"; }
                    else if (Gender_Check_M2 == true && Gender_Check_F2 == false)
                    { gen = "남성"; }

                    db.Update_Cus_Info(int.Parse(tb_Cus_Id2), tb_Cus_Name2, gen, Cus_Birth_Content2
                        , tb_Cus_Phone2, long.Parse(tb_Cus_Savings2));
                    Cus_All_Reset2();
                    MessageBox.Show("완료");
                }
                catch
                {
                    MessageBox.Show("입력되지 않은 값이 있습니다");
                }
            }
        }
        #endregion

        #region 고객관리 - 초기화 버튼
        private ICommand btn_su_cus_reset2;
        public ICommand Btn_Su_Cus_Reset2
        {
            get { return (this.btn_su_cus_reset2) ?? (this.btn_su_cus_reset2 = new DelegateCommand(Cus_All_Reset2)); }
        }

        void Cus_All_Reset2()
        {
            Label_First2 = "신규등록";
            Btn_search_res_Content2 = "등록";
            res_change_check2 = 0;
            tb_Cus_Id2 = null;
            tb_Cus_Name2 = null;
            Gender_Check_F2 = false;
            Gender_Check_M2 = false;
            tb_Cus_Phone2 = null;
            Cus_Birth_Content2 = DateTime.Now;
            Cus_Birth_Content2 = null;
            tb_Cus_Savings2 = null;
            cus_id_label_visible2 = Visibility.Hidden;
            cus_id_text_visible2 = Visibility.Hidden;
        }

        #endregion

        #region 고객관리 - 고객조회 버튼
        private ICommand btn_su_search2;
        public ICommand Btn_Su_Search2
        {
            get { return (this.btn_su_search2) ?? (this.btn_su_search2 = new DelegateCommand(Searching_Cus2)); }
        }

        private void Searching_Cus2()//지원 -> 고객조회 -> 조회버튼
        {
            List<Customer> customers;
            string ap = string.Empty;
            if (Cus_Name_Search2 != null)
            {
                Check_and2(and_ck2);
                qr2 += " name like @name";
            }
            else
            {
                Cus_Name_Search2 = "";
            }
            if (Cus_Gender_Search2 != null)
            {
                Check_and2(and_ck2);
                qr2 += " gender=@gender";
            }
            else
            {
                Cus_Gender_Search2 = "";
            }
            if (Cus_Phone_Search2 != null)
            {
                Check_and2(and_ck2);
                qr2 += " phone like @phone";
            }
            else
            {
                Cus_Phone_Search2 = "";
            }

            if (qr2 != "") ap = " where" + qr2;

            try
            {
                customers = db.GetList_Customer_Search(Cus_Name_Search2, Cus_Gender_Search2, Cus_Phone_Search2, ap); //데이터 바인딩 & 전체출력
                Lv_se_cus_searchItemSource = customers;
            }
            catch
            {
                MessageBox.Show("실패");
            }
            Cus_Name_Search2 = null;
            Cus_Gender_Search2 = null;
            Cus_Phone_Search2 = null;
            qr2 = "";
            and_ck2 = 0;
        }

        private List<Customer> lv_se_cus_searchItemSource;
        public List<Customer> Lv_se_cus_searchItemSource
        {
            get
            {
                return lv_se_cus_searchItemSource;
            }
            set
            {
                lv_se_cus_searchItemSource = value;
                OnPropertyChanged("Lv_se_cus_searchItemSource");
            }
        }

        #endregion//끝

        #region 고객관리 - 고객리스트 더블클릭
        private ICommand lv_cus_doublieclickcommand2;
        public ICommand Lv_Cus_DoubleClickCommand2
        {
            get { return (this.lv_cus_doublieclickcommand2) ?? (this.lv_cus_doublieclickcommand2 = new DelegateCommand(Lo_Input_AddDoubleClick2)); }
        }
        private void Lo_Input_AddDoubleClick2()
        {
            if (Lv_SelectItem2 != null)
            {
                Customer customer = new Customer();
                try
                {
                    Label_First2 = "정보변경";
                    Btn_search_res_Content2 = "변경";
                    customer = Lv_SelectItem2;
                    tb_Cus_Id2 = customer.Id.ToString();
                    tb_Cus_Name2 = customer.Name;
                    tb_Cus_Phone2 = customer.Phone;
                    Cus_Birth_Content2 = customer.Date;
                    tb_Cus_Savings2 = customer.Savings.ToString();

                    if (customer.Gender == "남성")
                    {
                        Gender_Check_M2 = true;
                        Gender_Check_F2 = false;
                    }
                    else
                    {
                        Gender_Check_M2 = false;
                        Gender_Check_F2 = true;
                    }
                    res_change_check2 = 1;
                    cus_id_label_visible2 = Visibility.Visible;
                    cus_id_text_visible2 = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }


        private Visibility _cus_id_label_visible2;
        public Visibility cus_id_label_visible2
        {
            get
            {
                return _cus_id_label_visible2;
            }
            set
            {
                _cus_id_label_visible2 = value;

                OnPropertyChanged("cus_id_label_visible2");
            }
        }

        private Visibility _cus_id_text_visible2;
        public Visibility cus_id_text_visible2
        {
            get
            {
                return _cus_id_text_visible2;
            }
            set
            {
                _cus_id_text_visible2 = value;

                OnPropertyChanged("cus_id_text_visible2");
            }
        }



        #endregion

        #endregion

        #endregion


        #region 물류

        #region 물류 - 제품등록 - 리셋버튼
        private ICommand btn_lo_reg_resetCommand;
        public ICommand Btn_Lo_Reg_ResetCommand
        {
            get { return (this.btn_lo_reg_resetCommand) ?? (this.btn_lo_reg_resetCommand = new DelegateCommand(Lo_Reg_Reset)); }
        }
        private void Lo_Reg_Reset()
        {
            Tb_Lo_Reg_ObjectNameContent = "";
            Dp_Lo_Reg_InputDateText = DateTime.Now.ToString();
            Dp_Lo_Reg_InputDateText = "";
            Tb_Lo_Reg_ObjectCPUContent = "";
            Tb_Lo_Reg_ObjectInchContent = "";
            Tb_Lo_Reg_ObjectmAhContent = "";
            Tb_Lo_Reg_ObjectRAMContent = "";
            Tb_Lo_Reg_ObjectBrandContent = "";
            Tb_Lo_Reg_ObjectCameraContent = "";
            Tb_Lo_Reg_ObjectWeightContent = "";
            Tb_Lo_Reg_ObjectPriceContent = "";
            Tb_Lo_Reg_ObjectDisplayContent = "";
            Tb_Lo_Reg_ObjectMemoryContent = "";
        }
        #endregion

        #region 물류 - 제품등록 - 등록버튼
        private ICommand btn_lo_reg_registCommand;
        public ICommand Btn_Lo_Reg_RegistCommand
        {
            get { return (this.btn_lo_reg_registCommand) ?? (this.btn_lo_reg_registCommand = new DelegateCommand(Lo_Reg_Regist)); }
        }
        private void Lo_Reg_Regist()
        {
            List<Product> list = new List<Product>();
            List<string> namelist = new List<string>();

            if (Tb_Lo_Reg_ObjectNameContent == "")
                Tb_Lo_Reg_ObjectNameContent = null;
            if (Dp_Lo_Reg_InputDateContent == "")
                Dp_Lo_Reg_InputDateContent = null;
            if (Tb_Lo_Reg_ObjectCPUContent == "")
                Tb_Lo_Reg_ObjectCPUContent = null;
            if (Tb_Lo_Reg_ObjectInchContent == "")
                Tb_Lo_Reg_ObjectInchContent = null;
            if (Tb_Lo_Reg_ObjectmAhContent == "")
                Tb_Lo_Reg_ObjectmAhContent = null;
            if (Tb_Lo_Reg_ObjectRAMContent == "")
                Tb_Lo_Reg_ObjectRAMContent = null;
            if (Tb_Lo_Reg_ObjectBrandContent == "")
                Tb_Lo_Reg_ObjectBrandContent = null;
            if (Tb_Lo_Reg_ObjectCameraContent == "")
                Tb_Lo_Reg_ObjectCameraContent = null;
            if (Tb_Lo_Reg_ObjectWeightContent == "")
                Tb_Lo_Reg_ObjectWeightContent = null;
            if (Tb_Lo_Reg_ObjectPriceContent == "")
                Tb_Lo_Reg_ObjectPriceContent = null;
            if (Tb_Lo_Reg_ObjectDisplayContent == "")
                Tb_Lo_Reg_ObjectDisplayContent = null;
            if (Tb_Lo_Reg_ObjectMemoryContent == "")
                Tb_Lo_Reg_ObjectMemoryContent = null;


            if (Tb_Lo_Reg_ObjectNameContent != null && Dp_Lo_Reg_InputDateContent != null && Tb_Lo_Reg_ObjectCPUContent != null && Tb_Lo_Reg_ObjectInchContent != null && Tb_Lo_Reg_ObjectmAhContent != null && Tb_Lo_Reg_ObjectRAMContent != null && Tb_Lo_Reg_ObjectBrandContent != null && Tb_Lo_Reg_ObjectCameraContent != null && Tb_Lo_Reg_ObjectWeightContent != null && Tb_Lo_Reg_ObjectPriceContent != null && Tb_Lo_Reg_ObjectDisplayContent != null && Tb_Lo_Reg_ObjectMemoryContent != null)
            {
                try
                {
                    namelist = db.Check_Lo_Reg_Overlap();
                    bool check = false;

                    for (int i = 0; i < namelist.Count; i++)
                    {
                        if (namelist[i] == Tb_Lo_Reg_ObjectNameContent)
                            check = true;
                    }

                    if (!check)
                    {
                        if (MessageBox.Show("등록하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            db.Add_Lo_Reg_Product(Tb_Lo_Reg_ObjectNameContent, Convert.ToDateTime(Dp_Lo_Reg_InputDateContent), Tb_Lo_Reg_ObjectCPUContent, Tb_Lo_Reg_ObjectInchContent, Convert.ToInt32(Tb_Lo_Reg_ObjectmAhContent), Convert.ToInt32(Tb_Lo_Reg_ObjectRAMContent), Tb_Lo_Reg_ObjectBrandContent, Convert.ToInt32(Tb_Lo_Reg_ObjectCameraContent), Convert.ToInt32(Tb_Lo_Reg_ObjectWeightContent), Convert.ToInt64(Tb_Lo_Reg_ObjectPriceContent), Tb_Lo_Reg_ObjectDisplayContent, Convert.ToInt32(Tb_Lo_Reg_ObjectMemoryContent));
                            Dp_Lo_Reg_InputDateText = DateTime.Now.ToString();
                            Dp_Lo_Reg_InputDateText = null;
                            MessageBox.Show("등록 완료");
                        }
                    }
                    else if (check)
                        MessageBox.Show("이미 존재하는 제품입니다");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("입력을 완료하세요");
            }
        }
        #endregion

        #region 물류 - 제품등록 - 조회버튼
        private ICommand btn_lo_reg_searchCommand;
        public ICommand Btn_Lo_Reg_SearchCommand
        {
            get { return (this.btn_lo_reg_searchCommand) ?? (this.btn_lo_reg_searchCommand = new DelegateCommand(Lo_Reg_PrintAll)); }
        }
        private void Lo_Reg_PrintAll()
        {
            ObservableCollection<Product> list = new ObservableCollection<Product>();

            try
            {
                list = db.Get_Lo_Reg_RegistProductList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Lv_Lo_Reg_ProductListItemsSource = list;
        }
        #endregion

        #region 물류 - 제품등록 - 헤더클릭
        #endregion

        #region 물류 - 제품등록 - 프로퍼티
        private ObservableCollection<Product> lv_lo_reg_productlistItemsSource;
        public ObservableCollection<Product> Lv_Lo_Reg_ProductListItemsSource
        {
            get { return lv_lo_reg_productlistItemsSource; }
            set
            {
                lv_lo_reg_productlistItemsSource = value;
                OnPropertyChanged("Lv_Lo_Reg_ProductListItemsSource");
            }
        }

        private string tb_lo_reg_objectnameContent;
        public string Tb_Lo_Reg_ObjectNameContent
        {
            get { return tb_lo_reg_objectnameContent; }
            set
            {
                tb_lo_reg_objectnameContent = value;
                OnPropertyChanged("Tb_Lo_Reg_ObjectNameContent");
            }
        }
        private string dp_lo_reg_inputdateContent;
        public string Dp_Lo_Reg_InputDateContent
        {
            get { return dp_lo_reg_inputdateContent; }
            set
            {
                dp_lo_reg_inputdateContent = value;
                OnPropertyChanged("Dp_Lo_Reg_InputDateContent");
            }
        }
        private string dp_lo_reg_inputDateText;
        public string Dp_Lo_Reg_InputDateText
        {
            get { return dp_lo_reg_inputDateText; }
            set
            {
                dp_lo_reg_inputDateText = value;
                OnPropertyChanged("Dp_Lo_Reg_InputDateText");
            }
        }
        private string tb_lo_reg_objectcpuContent;
        public string Tb_Lo_Reg_ObjectCPUContent
        {
            get { return tb_lo_reg_objectcpuContent; }
            set
            {
                tb_lo_reg_objectcpuContent = value;
                OnPropertyChanged("Tb_Lo_Reg_ObjectCPUContent");
            }
        }
        private string tb_lo_reg_objectinchContent;
        public string Tb_Lo_Reg_ObjectInchContent
        {
            get { return tb_lo_reg_objectinchContent; }
            set
            {
                tb_lo_reg_objectinchContent = value;
                OnPropertyChanged("Tb_Lo_Reg_ObjectInchContent");
            }
        }
        private string tb_lo_reg_objectmAhContent;
        public string Tb_Lo_Reg_ObjectmAhContent
        {
            get { return tb_lo_reg_objectmAhContent; }
            set
            {
                tb_lo_reg_objectmAhContent = value;
                OnPropertyChanged("Tb_Lo_Reg_ObjectmAhContent");
            }
        }
        private string tb_lo_reg_objectramContent;
        public string Tb_Lo_Reg_ObjectRAMContent
        {
            get { return tb_lo_reg_objectramContent; }
            set
            {
                tb_lo_reg_objectramContent = value;
                OnPropertyChanged("Tb_Lo_Reg_ObjectRAMContent");
            }
        }
        private string tb_lo_reg_objectbrandContent;
        public string Tb_Lo_Reg_ObjectBrandContent
        {
            get { return tb_lo_reg_objectbrandContent; }
            set
            {
                tb_lo_reg_objectbrandContent = value;
                OnPropertyChanged("Tb_Lo_Reg_ObjectBrandContent");
            }
        }
        private string tb_lo_reg_objectcameraContent;
        public string Tb_Lo_Reg_ObjectCameraContent
        {
            get { return tb_lo_reg_objectcameraContent; }
            set
            {
                tb_lo_reg_objectcameraContent = value;
                OnPropertyChanged("Tb_Lo_Reg_ObjectCameraContent");
            }
        }
        private string tb_lo_reg_objectweightContent;
        public string Tb_Lo_Reg_ObjectWeightContent
        {
            get { return tb_lo_reg_objectweightContent; }
            set
            {
                tb_lo_reg_objectweightContent = value;
                OnPropertyChanged("Tb_Lo_Reg_ObjectWeightContent");
            }
        }
        private string tb_lo_reg_objectpriceContent;
        public string Tb_Lo_Reg_ObjectPriceContent
        {
            get { return tb_lo_reg_objectpriceContent; }
            set
            {
                tb_lo_reg_objectpriceContent = value;
                OnPropertyChanged("Tb_Lo_Reg_ObjectPriceContent");
            }
        }
        private string tb_lo_reg_objectdisplayContent;
        public string Tb_Lo_Reg_ObjectDisplayContent
        {
            get { return tb_lo_reg_objectdisplayContent; }
            set
            {
                tb_lo_reg_objectdisplayContent = value;
                OnPropertyChanged("Tb_Lo_Reg_ObjectDisplayContent");
            }
        }
        private string tb_lo_reg_objectmemoryContent;
        public string Tb_Lo_Reg_ObjectMemoryContent
        {
            get { return tb_lo_reg_objectmemoryContent; }
            set
            {
                tb_lo_reg_objectmemoryContent = value;
                OnPropertyChanged("Tb_Lo_Reg_ObjectMemoryContent");
            }
        }
        #endregion

        #region 물류 - 입고 - 물품번호 목록 - 드롭다운오픈
        private ICommand lv_lo_input_dropdownopenedCommand;
        public ICommand Lv_Lo_Input_DropDownOpenedCommand
        {
            get { return (this.lv_lo_input_dropdownopenedCommand) ?? (this.lv_lo_input_dropdownopenedCommand = new DelegateCommand(Lo_Input_Drop)); }
        }
        private void Lo_Input_Drop()
        {
            try
            {
                Cb_Lo_Input_ProductNumberList = db.Get_Lo_Input_ProductNumList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 물류 - 입고 - 색상 목록 - 드롭다운오픈
        private ICommand lv_lo_input_colordropdownopenedCommand;
        public ICommand Lv_Lo_Input_ColorDropDownOpenedCommand
        {
            get { return (this.lv_lo_input_colordropdownopenedCommand) ?? (this.lv_lo_input_colordropdownopenedCommand = new DelegateCommand(Lo_Input_ColorDrop)); }
        }
        private void Lo_Input_ColorDrop()
        {
            try
            {
                string query = "";

                if (Cb_Lo_Input_ProductNumberContent == -1)
                    MessageBox.Show("제품번호를 먼저 선택하세요");
                else if (Cb_Lo_Input_ProductNumberContent > -1)
                {
                    Cb_Lo_Input_ColorEdit = true;
                    query += Cb_Lo_Input_ProductNumberItem;
                    Cb_Lo_Input_ColorSource = db.Get_Lo_Input_ColorList(query);
                }

                if (Cb_Lo_Input_ColorSource.Count == 0)
                {
                    MessageBox.Show("등록된 색상이 없습니다");
                    Tb_Lo_Input_RGB_ReadOnly = false;
                    Cb_Lo_Input_ColorEdit = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 물류 - 입고 - 색상 선택
        private ICommand cb_lo_input_colorSelChanged;
        public ICommand Cb_Lo_Input_ColorSelChanged
        {
            get { return (this.cb_lo_input_colorSelChanged) ?? (this.cb_lo_input_colorSelChanged = new DelegateCommand(Lo_Input_ColorSelChanged)); }
        }
        private void Lo_Input_ColorSelChanged()
        {
            try
            {
                Cb_Lo_Input_ColorEdit = false;
                Tb_Lo_Input_RGB_ReadOnly = true;
                Tb_Lo_Input_RGBContent = "";
                Tb_Lo_Input_ImgContent = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 물류 - 입고 - 색상 직접입력
        private ICommand cb_lo_input_colorTextChanged;
        public ICommand Cb_Lo_Input_ColorTextChanged
        {
            get { return (this.cb_lo_input_colorTextChanged) ?? (this.cb_lo_input_colorTextChanged = new DelegateCommand(Lo_Input_ColorTextChanged)); }
        }
        private void Lo_Input_ColorTextChanged()
        {
            try
            {
                MessageBox.Show("클릭");
                Tb_Lo_Input_RGB_ReadOnly = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 물류 - 입고 - 이미지경로 삭제
        private ICommand btn_lo_input_imgresetCommand;
        public ICommand Btn_Lo_Input_ImgResetCommand
        {
            get { return (this.btn_lo_input_imgresetCommand) ?? (this.btn_lo_input_imgresetCommand = new DelegateCommand(Lo_Input_ImgReset)); }
        }
        private void Lo_Input_ImgReset()
        {
            try
            {
                Tb_Lo_Input_ImgContent = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 물류 - 입고 - 등록물품 더블클릭
        private ICommand lv_lo_input_productdoubleclickCommand;
        public ICommand Lv_Lo_Input_ProductDoubleClickCommand
        {
            get { return (this.lv_lo_input_productdoubleclickCommand) ?? (this.lv_lo_input_productdoubleclickCommand = new DelegateCommand(Lo_Input_DoubleClick)); }
        }
        private void Lo_Input_DoubleClick()
        {
            List<Product> list = new List<Product>();

            try
            {
                if (Lv_Lo_Input_ObjectSelected != null)
                {
                    Product product = Lv_Lo_Input_ObjectSelected;
                    Cb_Lo_Input_ProductNumberItem = product.Product_id;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 물류 - 입고 - 조회버튼
        private ICommand btn_lo_input_searchCommand;
        public ICommand Btn_Lo_Input_SearchCommand
        {
            get { return (this.btn_lo_input_searchCommand) ?? (this.btn_lo_input_searchCommand = new DelegateCommand(Lo_Input_PrintAll)); }
        }
        private void Lo_Input_PrintAll()
        {
            ObservableCollection<Product> list = new ObservableCollection<Product>();

            try
            {
                list = db.Get_Lo_Reg_RegistProductList();
                Tb_Lo_Input_EmployeeNumContent = Convert.ToString(emp.Employee_id);
                Cb_Lo_Input_ProductNumberList = db.Get_Lo_Input_ProductNumList();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Lv_Lo_Input_ObjectListSource = list;
        }
        #endregion

        #region 물류 - 입고 - 추가물품 더블클릭
        private ICommand lv_lo_input_addproductdoubleclickCommand;
        public ICommand Lv_Lo_Input_AddProductDoubleClickCommand
        {
            get { return (this.lv_lo_input_addproductdoubleclickCommand) ?? (this.lv_lo_input_addproductdoubleclickCommand = new DelegateCommand(Lo_Input_AddDoubleClick)); }
        }
        private void Lo_Input_AddDoubleClick()
        {
            ObservableCollection<Product> list = new ObservableCollection<Product>();

            try
            {
                if (Lv_Lo_Input_AddSelected != null)
                {
                    if (MessageBox.Show("정말 삭제하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        int index = Lv_Lo_Input_AddSelectedIndex;

                        Lv_Lo_Input_AddListSource.RemoveAt(index);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 물류 - 입고 - 추가버튼
        private ICommand btn_lo_input_listaddCommand;
        public ICommand Btn_Lo_Input_ListAddCommand
        {
            get { return (this.btn_lo_input_listaddCommand) ?? (this.btn_lo_input_listaddCommand = new DelegateCommand(Lo_Input_ListAdd)); }
        }
        private void Lo_Input_ListAdd()
        {
            try
            {
                ObservableCollection<Product> cus = new ObservableCollection<Product>();
                ObservableCollection<Product> list = new ObservableCollection<Product>();
                Product item = new Product();
                bool check = true;
                bool duple;

                int addlistcount;
                if (Lv_Lo_Input_AddListSource == null)
                    addlistcount = 0;
                else addlistcount = Lv_Lo_Input_AddListSource.Count;

                if (Tb_Lo_Input_NumberOfContent == "")
                    Tb_Lo_Input_NumberOfContent = null;
                if (Dp_Lo_Input_InputDateContent == "")
                    Dp_Lo_Input_InputDateContent = null;
                if (Tb_Lo_Input_ImgContent == "")
                    Tb_Lo_Input_ImgContent = null;

                if (Cb_Lo_Input_ProductNumberContent == -1 || Tb_Lo_Input_ColorContent == null)
                {
                    MessageBox.Show("입력 오류");
                }
                //기존에 입고된 상품일경우
                else if (duple = db.DupleCheck_stock_product(Cb_Lo_Input_ProductNumberItem, Tb_Lo_Input_ColorContent) && Tb_Lo_Input_NumberOfContent != null && Dp_Lo_Input_InputDateContent != null && Cb_Lo_Input_InOutputContent != -1)
                {
                    if (Lv_Lo_Input_AddListSource != null)
                    {
                        for (int i = 0; i < addlistcount; i++)
                        {
                            //기존에 있는 항목인 경우. DB가 아닌 로컬로 추가하므로 여기에서 Add
                            cus.Add(Lv_Lo_Input_AddListSource[i]);
                            if (Cb_Lo_Input_ProductNumberItem == cus[i].Product_id && Tb_Lo_Input_ColorContent == cus[i].Color && Convert.ToDateTime(Dp_Lo_Input_InputDateContent) == cus[i].Trade_date)
                            {
                                plusStock = Convert.ToInt32(Convert.ToInt32(Tb_Lo_Input_NumberOfContent) + cus[i].Stock);
                                item.Product_id = Convert.ToInt32(Cb_Lo_Input_ProductNumberItem);
                                item.Color = Tb_Lo_Input_ColorContent;
                                item.Stock = plusStock;
                                item.Employee_id = Convert.ToInt32(Tb_Lo_Input_EmployeeNumContent);
                                item.Trade_date = Convert.ToDateTime(Dp_Lo_Input_InputDateContent);
                                item.Trade_type = Cb_Lo_Input_InOutputSelected;
                                item.Image_dir = Tb_Lo_Input_ImgContent;

                                Lv_Lo_Input_AddListSource.Add(item);
                                Lv_Lo_Input_AddListSource.RemoveAt(i);
                                check = false;
                                break;
                            }
                        }
                        if (check == true)  //DB 검사 후 새로운 항목인 경우
                        {
                            plusStock = Convert.ToInt32(Tb_Lo_Input_NumberOfContent);
                            item.Product_id = Convert.ToInt32(Cb_Lo_Input_ProductNumberItem);
                            item.Color = Tb_Lo_Input_ColorContent;
                            item.Stock = plusStock;
                            item.Employee_id = Convert.ToInt32(Tb_Lo_Input_EmployeeNumContent);
                            item.Trade_date = Convert.ToDateTime(Dp_Lo_Input_InputDateContent);
                            item.Trade_type = Cb_Lo_Input_InOutputSelected;
                            item.Image_dir = Tb_Lo_Input_ImgContent;

                            Lv_Lo_Input_AddListSource.Add(item);
                        }
                    }
                    else
                    {
                        plusStock = Convert.ToInt32(Tb_Lo_Input_NumberOfContent);
                        list.Add(new Product()
                        {
                            Product_id = Convert.ToInt32(Cb_Lo_Input_ProductNumberItem),
                            Color = Tb_Lo_Input_ColorContent,
                            Stock = plusStock,
                            Employee_id = Convert.ToInt32(Tb_Lo_Input_EmployeeNumContent),
                            Trade_date = Convert.ToDateTime(Dp_Lo_Input_InputDateContent),
                            Trade_type = Cb_Lo_Input_InOutputSelected,
                            Image_dir = Tb_Lo_Input_ImgContent
                        });
                        Lv_Lo_Input_AddListSource = list;
                    }
                }

                //처음 입고된 상품일경우
                else if (!duple && Tb_Lo_Input_NumberOfContent != null && Dp_Lo_Input_InputDateContent != null && Cb_Lo_Input_InOutputContent != -1 && Tb_Lo_Input_RGBContent != null && Tb_Lo_Input_ImgContent != null)
                {
                    if (Tb_Lo_Input_RGBContent.Length == 6)   //RGB값 6자리 검사
                    {              
                        if (Lv_Lo_Input_AddListSource != null)
                        {
                            for (int i = 0; i < addlistcount; i++)
                            {
                                //기존에 있는 항목인 경우. DB가 아닌 로컬로 추가하므로 여기에서 Add
                                cus.Add(Lv_Lo_Input_AddListSource[i]);
                                if (Cb_Lo_Input_ProductNumberItem == cus[i].Product_id && Tb_Lo_Input_ColorContent == cus[i].Color && Convert.ToDateTime(Dp_Lo_Input_InputDateContent) == cus[i].Trade_date)
                                {
                                    plusStock = Convert.ToInt32(Convert.ToInt32(Tb_Lo_Input_NumberOfContent) + cus[i].Stock);
                                    item.Product_id = Convert.ToInt32(Cb_Lo_Input_ProductNumberItem);
                                    item.Color = Tb_Lo_Input_ColorContent;
                                    item.ColorValue = "#" + Tb_Lo_Input_RGBContent;
                                    item.Stock = plusStock;
                                    item.Employee_id = Convert.ToInt32(Tb_Lo_Input_EmployeeNumContent);
                                    item.Trade_date = Convert.ToDateTime(Dp_Lo_Input_InputDateContent);
                                    item.Trade_type = Cb_Lo_Input_InOutputSelected;
                                    item.Image_dir = Tb_Lo_Input_ImgContent;

                                    Lv_Lo_Input_AddListSource.Add(item);
                                    Lv_Lo_Input_AddListSource.RemoveAt(i);
                                    check = false;
                                    break;
                                }
                            }
                            if (check == true)  //DB 검사 후 새로운 항목인 경우
                            {
                                plusStock = Convert.ToInt32(Tb_Lo_Input_NumberOfContent);
                                item.Product_id = Convert.ToInt32(Cb_Lo_Input_ProductNumberItem);
                                item.Color = Tb_Lo_Input_ColorContent;
                                item.ColorValue = "#" + Tb_Lo_Input_RGBContent;
                                item.Stock = plusStock;
                                item.Employee_id = Convert.ToInt32(Tb_Lo_Input_EmployeeNumContent);
                                item.Trade_date = Convert.ToDateTime(Dp_Lo_Input_InputDateContent);
                                item.Trade_type = Cb_Lo_Input_InOutputSelected;
                                item.Image_dir = Tb_Lo_Input_ImgContent;

                                Lv_Lo_Input_AddListSource.Add(item);
                            }
                        }
                        else
                        {
                            plusStock = Convert.ToInt32(Tb_Lo_Input_NumberOfContent);
                            list.Add(new Product()
                            {
                                Product_id = Convert.ToInt32(Cb_Lo_Input_ProductNumberItem),
                                Color = Tb_Lo_Input_ColorContent,
                                Stock = plusStock,
                                ColorValue = "#" + Tb_Lo_Input_RGBContent,
                                Employee_id = Convert.ToInt32(Tb_Lo_Input_EmployeeNumContent),
                                Trade_date = Convert.ToDateTime(Dp_Lo_Input_InputDateContent),
                                Trade_type = Cb_Lo_Input_InOutputSelected,
                                Image_dir = Tb_Lo_Input_ImgContent
                            });
                           
                            Lv_Lo_Input_AddListSource = list;
                        }
                    }
                    else
                        MessageBox.Show("RGB값 입력 오류");
                }
                else
                {
                    MessageBox.Show("입력 오류");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 물류 - 입고 - 등록버튼
        private ICommand btn_lo_input_registCommand;
        public ICommand Btn_Lo_Input_RegistCommand
        {
            get { return (this.btn_lo_input_registCommand) ?? (this.btn_lo_input_registCommand = new DelegateCommand(Lo_Input_RegistList)); }
        }
        private void Lo_Input_RegistList()
        {
            try
            {
                if (Lv_Lo_Input_AddListSource == null)
                    MessageBox.Show("등록할 물품이 없습니다.");
                else
                {
                    if (MessageBox.Show("등록하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ObservableCollection<Product> dbdata = new ObservableCollection<Product>();
                        ObservableCollection<Product> inputdata = new ObservableCollection<Product>();
                        string query = "";

                        bool[] check = new bool[Lv_Lo_Input_AddListSource.Count];
                        int[] position = new int[Lv_Lo_Input_AddListSource.Count];
                        int[] newstock = new int[Lv_Lo_Input_AddListSource.Count];
                        int addlistcount;

                        if (Lv_Lo_Input_AddListSource == null)
                            addlistcount = 0;
                        else addlistcount = Lv_Lo_Input_AddListSource.Count;

                        for (int i = 0; i < addlistcount; i++)
                        {
                            if (i == 0)
                            {
                                query += "WHERE ";
                            }
                            Product product = (Product)Lv_Lo_Input_AddListSource[i];
                            query += "product_id = " + product.Product_id;
                            if (i + 1 != addlistcount)
                            {
                                query += " OR ";
                            }
                        }

                        dbdata = db.Select_Lo_Pse_stockProduct(query);
                        for (int i = 0; i < addlistcount; i++)
                            inputdata.Add((Product)Lv_Lo_Input_AddListSource[i]);

                        for (int i = 0; i < addlistcount; i++)  //추가하려는 항목과 기존에 존재하는 항목간에 중복값 검사
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

                        for (int i = 0; i < inputdata.Count; i++)
                        {
                            if (check[i] == false)
                            {
                                string product_name = db.Select_productname_id(inputdata[i].Product_id);
                                FtpUploadFile(inputdata[i].Image_dir, ftp_uri + "/phone/" + product_name + "_" + inputdata[i].Color + "_F.JPG");
                            }
                        }
                        db.input_transaction(inputdata, check, newstock);
                        MessageBox.Show("등록완료");

                        Cb_Lo_Input_ProductNumberContent = -1;
                        Tb_Lo_Input_ColorContent = null;
                        Tb_Lo_Input_NumberOfContent = null;
                        Tb_Lo_Input_RGBContent = null;
                        Dp_Lo_Input_InputDateContent = null;
                        Tb_Lo_Input_ImgContent = null;
                        Lv_Lo_Input_AddListSource.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FtpUploadFile(string filename, string to_uri)
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request =
                (FtpWebRequest)WebRequest.Create(to_uri.Replace("+", "plus"));
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

        #region 물류 - 입고 - 지움버튼
        private ICommand btn_lo_input_removeCommand;
        public ICommand Btn_Lo_Input_RemoveCommand
        {
            get { return (this.btn_lo_input_removeCommand) ?? (this.btn_lo_input_removeCommand = new DelegateCommand(Lo_Input_Remove)); }
        }
        private void Lo_Input_Remove()
        {
            try
            {
                Cb_Lo_Input_ProductNumberContent = -1;
                Tb_Lo_Input_ColorContent = null;
                Tb_Lo_Input_NumberOfContent = null;
                Tb_Lo_Input_RGBContent = null;
                Dp_Lo_Input_InputDateText = DateTime.Now.ToString();
                Dp_Lo_Input_InputDateText = null;
                Cb_Lo_Input_InOutputContent = 0;
                Tb_Lo_Input_ImgContent = null;
                Tb_Lo_Input_RGB_ReadOnly = false;
                Cb_Lo_Input_ColorEdit = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 물류 - 입고 - 이미지 추가 버튼
        private ICommand btn_lo_input_uploadimgCommand;
        public ICommand Btn_Lo_Input_UploadImgCommand
        {
            get { return (this.btn_lo_input_uploadimgCommand) ?? (this.btn_lo_input_uploadimgCommand = new DelegateCommand(Lo_Input_UploadImg)); }
        }
        private void Lo_Input_UploadImg()
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".png";
                dlg.Filter = "JPG Files (*.jpg)|*.jpg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif|JPEG Files (*.jpeg)|*.jpeg";

                // Display OpenFileDialog by calling ShowDialog method 
                Nullable<bool> result = dlg.ShowDialog();


                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    // Open document 
                    string filename = dlg.FileName;
                    Tb_Lo_Input_ImgContent = filename;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 물류 - 입고 - 프로퍼티
        private int cb_lo_input_productnumberitem;
        public int Cb_Lo_Input_ProductNumberItem
        {
            get { return cb_lo_input_productnumberitem; }
            set
            {
                cb_lo_input_productnumberitem = value;
                OnPropertyChanged("Cb_Lo_Input_ProductNumberItem");
            }
        }

        private List<int> cb_lo_input_productnumberList;
        public List<int> Cb_Lo_Input_ProductNumberList
        {
            get { return cb_lo_input_productnumberList; }
            set
            {
                cb_lo_input_productnumberList = value;
                OnPropertyChanged("Cb_Lo_Input_ProductNumberList");
            }
        }

        private List<string> cb_lo_input_colorSource;
        public List<string> Cb_Lo_Input_ColorSource
        {
            get { return cb_lo_input_colorSource; }
            set
            {
                cb_lo_input_colorSource = value;
                OnPropertyChanged("Cb_Lo_Input_ColorSource");
            }
        }

        private bool cb_lo_input_colorEdit;
        public bool Cb_Lo_Input_ColorEdit
        {
            get { return cb_lo_input_colorEdit; }
            set
            {
                cb_lo_input_colorEdit = value;
                OnPropertyChanged("Cb_Lo_Input_ColorEdit");
            }
        }

        private ObservableCollection<Product> lv_lo_input_addlistSource;
        public ObservableCollection<Product> Lv_Lo_Input_AddListSource
        {
            get { return lv_lo_input_addlistSource; }
            set
            {
                lv_lo_input_addlistSource = value;
                OnPropertyChanged("Lv_Lo_Input_AddListSource");
            }
        }

        private int lv_lo_input_addselectedindex;
        public int Lv_Lo_Input_AddSelectedIndex
        {
            get { return lv_lo_input_addselectedindex; }
            set
            {
                lv_lo_input_addselectedindex = value;
                OnPropertyChanged("Lv_Lo_Input_AddSelectedIndex");
            }
        }

        private Product lv_lo_input_addselected;
        public Product Lv_Lo_Input_AddSelected
        {
            get { return lv_lo_input_addselected; }
            set
            {
                lv_lo_input_addselected = value;
                OnPropertyChanged("Lv_Lo_Input_AddSelected");
            }
        }

        private ObservableCollection<Product> lv_lo_input_objectlistSource;
        public ObservableCollection<Product> Lv_Lo_Input_ObjectListSource
        {
            get { return lv_lo_input_objectlistSource; }
            set
            {
                lv_lo_input_objectlistSource = value;
                OnPropertyChanged("Lv_Lo_Input_ObjectListSource");
            }
        }

        private Product lv_lo_input_objectselected;
        public Product Lv_Lo_Input_ObjectSelected
        {
            get { return lv_lo_input_objectselected; }
            set
            {
                lv_lo_input_objectselected = value;
                OnPropertyChanged("Lv_Lo_Input_ObjectSelected");
            }
        }

        private int cb_lo_input_productnumberContent;
        public int Cb_Lo_Input_ProductNumberContent
        {
            get { return cb_lo_input_productnumberContent; }
            set
            {
                cb_lo_input_productnumberContent = value;
                OnPropertyChanged("Cb_Lo_Input_ProductNumberContent");
            }
        }

        private string tb_lo_input_colorContent;
        public string Tb_Lo_Input_ColorContent
        {
            get { return tb_lo_input_colorContent; }
            set
            {
                tb_lo_input_colorContent = value;
                OnPropertyChanged("Tb_Lo_Input_ColorContent");
            }
        }

        private string tb_lo_input_numberOfContent;
        public string Tb_Lo_Input_NumberOfContent
        {
            get { return tb_lo_input_numberOfContent; }
            set
            {
                tb_lo_input_numberOfContent = value;
                OnPropertyChanged("Tb_Lo_Input_NumberOfContent");
            }
        }

        private string tb_lo_input_rgbContent;
        public string Tb_Lo_Input_RGBContent
        {
            get { return tb_lo_input_rgbContent; }
            set
            {
                tb_lo_input_rgbContent = value;
                OnPropertyChanged("Tb_Lo_Input_RGBContent");
            }
        }

        private bool btn_lo_input_rgb_ReadOnly;
        public bool Tb_Lo_Input_RGB_ReadOnly
        {
            get { return btn_lo_input_rgb_ReadOnly; }
            set
            {
                btn_lo_input_rgb_ReadOnly = value;
                OnPropertyChanged("Tb_Lo_Input_RGB_ReadOnly");
            }
        }

        private string tb_lo_input_employeenumContent;
        public string Tb_Lo_Input_EmployeeNumContent
        {
            get { return tb_lo_input_employeenumContent; }
            set
            {
                tb_lo_input_employeenumContent = value;
                OnPropertyChanged("Tb_Lo_Input_EmployeeNumContent");
            }
        }

        private string dp_lo_input_inputdateContent;
        public string Dp_Lo_Input_InputDateContent
        {
            get { return dp_lo_input_inputdateContent; }
            set
            {
                dp_lo_input_inputdateContent = value;
                OnPropertyChanged("Dp_Lo_Input_InputDateContent");
            }
        }

        private string dp_lo_input_inputDateText;
        public string Dp_Lo_Input_InputDateText
        {
            get { return dp_lo_input_inputDateText; }
            set
            {
                dp_lo_input_inputDateText = value;
                OnPropertyChanged("Dp_Lo_Input_InputDateText");
            }
        }

        private int cb_lo_input_inoutputContent;
        public int Cb_Lo_Input_InOutputContent
        {
            get { return cb_lo_input_inoutputContent; }
            set
            {
                cb_lo_input_inoutputContent = value;
                OnPropertyChanged("Cb_Lo_Input_InOutputContent");
            }
        }

        private string cb_lo_input_inoutputSelected;
        public string Cb_Lo_Input_InOutputSelected
        {
            get { return cb_lo_input_inoutputSelected; }
            set
            {
                cb_lo_input_inoutputSelected = value;
                OnPropertyChanged("Cb_Lo_Input_InOutputSelected");
            }
        }

        private string tb_lo_input_imgContent;
        public string Tb_Lo_Input_ImgContent
        {
            get { return tb_lo_input_imgContent; }
            set
            {
                tb_lo_input_imgContent = value;
                OnPropertyChanged("Tb_Lo_Input_ImgContent");
            }
        }

        private List<string> cb_lo_input_inoutputSource;
        public List<string> Cb_Lo_Input_InOutputSource
        {
            get { return cb_lo_input_inoutputSource; }
            set
            {
                cb_lo_input_inoutputSource = value;
                OnPropertyChanged("Cb_Lo_Input_InOutputSource");
            }
        }
        #endregion

        #region 물류 - 내역조회 - 조회버튼
        private ICommand btn_lo_rse_searchcommand;
        public ICommand Btn_Lo_Rse_SearchCommand
        {
            get { return (this.btn_lo_rse_searchcommand) ?? (this.btn_lo_rse_searchcommand = new DelegateCommand(Lo_Rse_PrintAll)); }
        }
        private void Lo_Rse_PrintAll()
        {
            string query = "";

            List<Product> list = new List<Product>();

            if (Tb_Lo_Rse_ProductIDContent == "")
                Tb_Lo_Rse_ProductIDContent = null;
            if (Tb_Lo_Rse_TradeHistoryIDContent == "")
                Tb_Lo_Rse_TradeHistoryIDContent = null;
            if (Tb_Lo_Rse_ColorContent == "")
                Tb_Lo_Rse_ColorContent = null;
            if (Tb_Lo_Rse_ProductNameContent == "")
                Tb_Lo_Rse_ProductNameContent = null;


            if (Tb_Lo_Rse_ProductIDContent != null || Cb_Lo_Rse_InOutputContent >= 0 || Tb_Lo_Rse_TradeHistoryIDContent != null || Tb_Lo_Rse_ColorContent != null || Tb_Lo_Rse_ProductNameContent != null || Dp_Lo_Rse_StartDateContent != null || Dp_Lo_Rse_EndDateContent != null)
            {
                if (Tb_Lo_Rse_ProductIDContent != null)
                {
                    query += "and p.product_id = " + Tb_Lo_Rse_ProductIDContent + " ";
                }
                if (Cb_Lo_Rse_InOutputContent == 1)
                {
                    query += "and tp.trade_type = '입고' ";
                }
                else if (Cb_Lo_Rse_InOutputContent == 2)
                {
                    query += "and tp.trade_type = '반품' ";
                }
                if (Tb_Lo_Rse_TradeHistoryIDContent != null)
                {
                    query += "and tp.trade_history_id = " + Tb_Lo_Rse_TradeHistoryIDContent + " ";
                }
                if (Tb_Lo_Rse_ColorContent != null)
                {
                    query += "and tp.color like '%" + Tb_Lo_Rse_ColorContent + "%' ";
                }
                if (Tb_Lo_Rse_ProductNameContent != null)
                {
                    query += "and p.name like '%" + Tb_Lo_Rse_ProductNameContent + "%' ";
                }
                if (Dp_Lo_Rse_StartDateContent != null)
                {
                    query += "and th.trade_date >= @Start_date ";
                }
                if (Dp_Lo_Rse_EndDateContent != null)
                {
                    query += "and th.trade_date <= @End_date ";
                }

                try
                {
                    list = db.Get_Lo_Rse_ProductList(Convert.ToInt32(Tb_Lo_Rse_ProductIDContent), Convert.ToInt32(Tb_Lo_Rse_TradeHistoryIDContent), Tb_Lo_Rse_ProductNameContent, Tb_Lo_Rse_ColorContent, Convert.ToDateTime(Dp_Lo_Rse_StartDateContent), Convert.ToDateTime(Dp_Lo_Rse_EndDateContent), query);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                Lv_Lo_Rse_ProductListSource = list;
            }
            //else
            //{
            //    try
            //    {
            //        list = db.Get_Lo_Rse_ProductList(Convert.ToInt32(Tb_Lo_Rse_ProductIDContent), Convert.ToInt32(Tb_Lo_Rse_TradeHistoryIDContent), Tb_Lo_Rse_ProductNameContent, Tb_Lo_Rse_ColorContent, Convert.ToDateTime(Dp_Lo_Rse_StartDateContent), Convert.ToDateTime(Dp_Lo_Rse_EndDateContent), query);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }

            //    Lv_Lo_Rse_ProductListSource = list;
            //}
        }
        #endregion

        #region 물류 - 내역조회 - 초기화버튼
        private ICommand btn_lo_rse_resetcommand;
        public ICommand Btn_Lo_Rse_ResetCommand
        {
            get { return (this.btn_lo_rse_resetcommand) ?? (this.btn_lo_rse_resetcommand = new DelegateCommand(Lo_Rse_ResetAll)); }
        }

        private void Lo_Rse_ResetAll()
        {
            Tb_Lo_Rse_ProductIDContent = null;
            Cb_Lo_Rse_InOutputContent = 0;
            Tb_Lo_Rse_TradeHistoryIDContent = null;
            Tb_Lo_Rse_ColorContent = null;
            Tb_Lo_Rse_ProductNameContent = null;
            Dp_Lo_Rse_StartDateContent = null;
            Dp_Lo_Rse_StartDateText = DateTime.Now.ToString();
            Dp_Lo_Rse_StartDateText = null;
            Dp_Lo_Rse_EndDateContent = null;
            Dp_Lo_Rse_EndDateText = DateTime.Now.ToString();
            Dp_Lo_Rse_EndDateText = null;
        }
        #endregion

        #region 물류 - 내역조회 - 프로퍼티
        private List<Product> lv_lo_rse_productlistSource;
        public List<Product> Lv_Lo_Rse_ProductListSource
        {
            get { return lv_lo_rse_productlistSource; }
            set
            {
                lv_lo_rse_productlistSource = value;
                OnPropertyChanged("Lv_Lo_Rse_ProductListSource");
            }
        }

        private string tb_lo_rse_productidContent;
        public string Tb_Lo_Rse_ProductIDContent
        {
            get { return tb_lo_rse_productidContent; }
            set
            {
                tb_lo_rse_productidContent = value;
                OnPropertyChanged("Tb_Lo_Rse_ProductIDContent");
            }
        }

        private int cb_lo_rse_inoutputContent;
        public int Cb_Lo_Rse_InOutputContent
        {
            get { return Convert.ToInt32(cb_lo_rse_inoutputContent); }
            set
            {
                cb_lo_rse_inoutputContent = value;
                OnPropertyChanged("Cb_Lo_Rse_InOutputContent");
            }
        }

        private string tb_lo_rse_tradehistoryidContent;
        public string Tb_Lo_Rse_TradeHistoryIDContent
        {
            get { return tb_lo_rse_tradehistoryidContent; }
            set
            {
                tb_lo_rse_tradehistoryidContent = value;
                OnPropertyChanged("Tb_Lo_Rse_TradeHistoryIDContent");
            }
        }

        private string tb_lo_rse_colorContent;
        public string Tb_Lo_Rse_ColorContent
        {
            get { return tb_lo_rse_colorContent; }
            set
            {
                tb_lo_rse_colorContent = value;
                OnPropertyChanged("Tb_Lo_Rse_ColorContent");
            }
        }

        private string tb_lo_rse_productnameContent;
        public string Tb_Lo_Rse_ProductNameContent
        {
            get { return tb_lo_rse_productnameContent; }
            set
            {
                tb_lo_rse_productnameContent = value;
                OnPropertyChanged("Tb_Lo_Rse_ProductNameContent");
            }
        }

        private string dp_lo_rse_startdateContent;
        public string Dp_Lo_Rse_StartDateContent
        {
            get { return dp_lo_rse_startdateContent; }
            set
            {
                dp_lo_rse_startdateContent = value;
                OnPropertyChanged("Dp_Lo_Rse_StartDateContent");
            }
        }

        private string dp_lo_rse_startDateText;
        public string Dp_Lo_Rse_StartDateText
        {
            get { return dp_lo_rse_startDateText; }
            set
            {
                dp_lo_rse_startDateText = value;
                OnPropertyChanged("Dp_Lo_Rse_StartDateText");
            }
        }

        private string dp_lo_rse_enddateContent;
        public string Dp_Lo_Rse_EndDateContent
        {
            get { return dp_lo_rse_enddateContent; }
            set
            {
                dp_lo_rse_enddateContent = value;
                OnPropertyChanged("Dp_Lo_Rse_EndDateContent");
            }
        }

        private string dp_lo_rse_endDateText;
        public string Dp_Lo_Rse_EndDateText
        {
            get { return dp_lo_rse_endDateText; }
            set
            {
                dp_lo_rse_endDateText = value;
                OnPropertyChanged("Dp_Lo_Rse_EndDateText");
            }
        }
        #endregion

        #region 물류 - 재고조회 - 물품 더블클릭
        private ICommand lv_lo_pse_productDoubleClickCommand;
        public ICommand Lv_Lo_Pse_ProductDoubleClickCommand
        {
            get { return (this.lv_lo_pse_productDoubleClickCommand) ?? (this.lv_lo_pse_productDoubleClickCommand = new DelegateCommand(Lo_Pse_ProductDoubleClick)); }
        }
        private void Lo_Pse_ProductDoubleClick()
        {
            try
            {
                if (Lv_Lo_Pse_ProductSelectedItem != null)
                {
                    Product product = Lv_Lo_Pse_ProductSelectedItem;
                    stock = new StockReturn();

                    Tb_Lo_Refund_ProductInfoContent = product.Name + " " + product.Color;
                    Tb_Lo_Refund_QuantityContent = "1";
                    Tb_Lo_Refund_ProductQuantityContent = "/ " + product.Stock;
                    stock.DataContext = this;
                    stock.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 물류 - 재고조회 - 조회버튼
        private ICommand btn_lo_pse_searchcommand;
        public ICommand Btn_Lo_Pse_SearchCommand
        {
            get { return (this.btn_lo_pse_searchcommand) ?? (this.btn_lo_pse_searchcommand = new DelegateCommand(Lo_Pse_PrintAll)); }
        }

        private void Lo_Pse_PrintAll()
        {
            string query = "";

            List<Product> list = new List<Product>();

            if (Tb_Lo_Pse_StockIDContent == "")
                Tb_Lo_Pse_StockIDContent = null;
            if (Tb_Lo_Pse_ProductIDContent == "")
                Tb_Lo_Pse_ProductIDContent = null;

            if (Tb_Lo_Pse_StockIDContent != null || Tb_Lo_Pse_ProductIDContent != null || Tb_Lo_Pse_ProductNameContent != null || Tb_Lo_Pse_ColorContent != null)
            {
                if (Tb_Lo_Pse_StockIDContent != null)
                {
                    query += "and sp.stock_product = @StockID ";
                }
                if (Tb_Lo_Pse_ProductIDContent != null)
                {
                    query += "and sp.product_id = @ProductID ";
                }
                if (Tb_Lo_Pse_ProductNameContent != null)
                {
                    query += "and p.name like @ProductName ";
                }
                if (Tb_Lo_Pse_ColorContent != null)
                {
                    query += "and sp.color like @Color ";
                }

                try
                {
                    list = db.Select_Lo_Pse_Product(Convert.ToInt32(Tb_Lo_Pse_StockIDContent), Convert.ToInt32(Tb_Lo_Pse_ProductIDContent), Tb_Lo_Pse_ProductNameContent, Tb_Lo_Pse_ColorContent, query);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                Lv_Lo_Pse_ProductListSource = list;
            }
            else
            {
                try
                {
                    list = db.Select_Lo_Pse_Product(Convert.ToInt32(Tb_Lo_Pse_StockIDContent), Convert.ToInt32(Tb_Lo_Pse_ProductIDContent), Tb_Lo_Pse_ProductNameContent, Tb_Lo_Pse_ColorContent, query);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                Lv_Lo_Pse_ProductListSource = list;
            }
        }
        #endregion

        #region 물류 - 재고조회 - 초기화버튼
        private ICommand btn_lo_pse_resetcommand;
        public ICommand Btn_Lo_Pse_ResetCommand
        {
            get { return (this.btn_lo_pse_resetcommand) ?? (this.btn_lo_pse_resetcommand = new DelegateCommand(Lo_Pse_ResetAll)); }
        }

        private void Lo_Pse_ResetAll()
        {
            Tb_Lo_Pse_StockIDContent = null;
            Tb_Lo_Pse_ProductIDContent = null;
            Tb_Lo_Pse_ProductNameContent = null;
            Tb_Lo_Pse_ColorContent = null;
        }
        #endregion

        #region 물류 - 재고조회 - 프로퍼티
        private Product lv_lo_pse_productSelectedItem;
        public Product Lv_Lo_Pse_ProductSelectedItem
        {
            get { return lv_lo_pse_productSelectedItem; }
            set
            {
                lv_lo_pse_productSelectedItem = value;
                OnPropertyChanged("Lv_Lo_Pse_ProductSelectedItem");
            }
        }

        private List<Product> lv_lo_pse_productlistSource;
        public List<Product> Lv_Lo_Pse_ProductListSource
        {
            get { return lv_lo_pse_productlistSource; }
            set
            {
                lv_lo_pse_productlistSource = value;
                OnPropertyChanged("Lv_Lo_Pse_ProductListSource");
            }
        }

        private string tb_lo_pse_stockidContent;
        public string Tb_Lo_Pse_StockIDContent
        {
            get { return tb_lo_pse_stockidContent; }
            set
            {
                tb_lo_pse_stockidContent = value;
                OnPropertyChanged("Tb_Lo_Pse_StockIDContent");
            }
        }

        private string tb_lo_pse_productidContent;
        public string Tb_Lo_Pse_ProductIDContent
        {
            get { return tb_lo_pse_productidContent; }
            set
            {
                tb_lo_pse_productidContent = value;
                OnPropertyChanged("Tb_Lo_Pse_ProductIDContent");
            }
        }

        private string tb_lo_pse_productnameContent;
        public string Tb_Lo_Pse_ProductNameContent
        {
            get { return tb_lo_pse_productnameContent; }
            set
            {
                tb_lo_pse_productnameContent = value;
                OnPropertyChanged("Tb_Lo_Pse_ProductNameContent");
            }
        }

        private string tb_lo_pse_colorContent;
        public string Tb_Lo_Pse_ColorContent
        {
            get { return tb_lo_pse_colorContent; }
            set
            {
                tb_lo_pse_colorContent = value;
                OnPropertyChanged("Tb_Lo_Pse_ColorContent");
            }
        }
        #endregion

        #region 물류 - 반품창 - 반품등록 버튼
        private ICommand Btn_returnregistCommand;
        public ICommand Btn_ReturnRegistCommand
        {
            get { return (this.Btn_returnregistCommand) ?? (this.Btn_returnregistCommand = new DelegateCommand(Lo_RefundRegist)); }
        }

        private void Lo_RefundRegist()
        {
            try
            {
                if (Tb_Lo_Refund_QuantityContent != "0" && Tb_Lo_Refund_QuantityContent != "")
                {
                    if (Convert.ToInt32(Tb_Lo_Refund_QuantityContent) > Convert.ToInt32(Tb_Lo_Refund_ProductQuantityContent.Substring(2)))
                        MessageBox.Show("재고 수량 초과");
                    else
                    {
                        ObservableCollection<Return_Info> list = new ObservableCollection<Return_Info>();
                        if (Lv_Lo_Refund_ObjectList == null)    //맨 처음 반품목록에 올라갈 때
                        {
                            Product product = Lv_Lo_Pse_ProductSelectedItem;

                            list.Add(new Return_Info()
                            {
                                Product_id = product.Product_id,
                                Name = product.Name,
                                Color = product.Color,
                                ColorValue = product.ColorValue,
                                Quantity = Convert.ToInt32(Tb_Lo_Refund_QuantityContent)
                            });

                            Lv_Lo_Refund_ObjectList = list;
                            stock.Close();
                        }
                        else    //이미 반품목록 리스트가 존재할 때
                        {
                            bool check = false;
                            Product product = Lv_Lo_Pse_ProductSelectedItem;
                            Return_Info item = new Return_Info();

                            for (int i = 0; i < Lv_Lo_Refund_ObjectList.Count; i++) //중복 리스트 체크 - 중복되는 경우 수량만 더한다
                            {
                                if (product.Product_id == Lv_Lo_Refund_ObjectList[i].Product_id && product.Color == Lv_Lo_Refund_ObjectList[i].Color)
                                {
                                    item.Product_id = product.Product_id;
                                    item.Name = product.Name;
                                    item.Color = product.Color;
                                    item.ColorValue = product.ColorValue;
                                    item.Quantity = Lv_Lo_Refund_ObjectList[i].Quantity + Convert.ToInt32(Tb_Lo_Refund_QuantityContent);

                                    Lv_Lo_Refund_ObjectList.Add(item);
                                    Lv_Lo_Refund_ObjectList.RemoveAt(i);
                                    check = true;
                                    break;
                                }
                            }

                            if (!check)
                            {
                                item.Product_id = product.Product_id;
                                item.Name = product.Name;
                                item.Color = product.Color;
                                item.ColorValue = product.ColorValue;
                                item.Quantity = Convert.ToInt32(Tb_Lo_Refund_QuantityContent);

                                Lv_Lo_Refund_ObjectList.Add(item);
                            }
                            stock.Close();
                        }
                    }
                }
                else MessageBox.Show("수량을 입력하세요");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 물류 - 반품 - 반품버튼
        private ICommand btn_lo_refund_returnCommand;
        public ICommand Btn_Lo_Refund_ReturnCommand
        {
            get { return (this.btn_lo_refund_returnCommand) ?? (this.btn_lo_refund_returnCommand = new DelegateCommand(Lo_Pse_Return)); }
        }

        private void Lo_Pse_Return()
        {
            if (Lv_Lo_Refund_ObjectList != null)
            {
                if (MessageBox.Show("반품하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ObservableCollection<Return_Info> returnInfo = new ObservableCollection<Return_Info>();

                    for (int i = 0; i < Lv_Lo_Refund_ObjectList.Count; i++)
                        returnInfo.Add(Lv_Lo_Refund_ObjectList[i]);

                    for (int i = 0; i < Lv_Lo_Refund_ObjectList.Count; i++)
                    {
                        if (returnInfo[i].Quantity > db.Lo_Check_Stock(returnInfo[i].Product_id, returnInfo[i].Color))
                        {
                            MessageBox.Show("반품 목록의 수량이 재고와 맞지않습니다.");
                            return;
                        }
                    }

                    for (int i = 0; i < Lv_Lo_Refund_ObjectList.Count; i++)
                        db.Set_Lo_Return_Stock(returnInfo[i], emp.Employee_id);

                    MessageBox.Show("반품이 완료되었습니다");
                    Lv_Lo_Refund_ObjectList.Clear();
                }
            }
            else MessageBox.Show("반품 목록이 존재하지 않습니다");
        }
        #endregion

        #region 물류 - 반품 - 초기화버튼
        private ICommand btn_lo_refund_resetCommand;
        public ICommand Btn_Lo_Refund_ResetCommand
        {
            get { return (this.btn_lo_refund_resetCommand) ?? (this.btn_lo_refund_resetCommand = new DelegateCommand(Lo_Refund_Reset)); }
        }
        private void Lo_Refund_Reset()
        {
            ObservableCollection<Product> list = new ObservableCollection<Product>();

            try
            {
                if (Lv_Lo_Refund_ObjectList != null)
                    Lv_Lo_Refund_ObjectList.Clear();
                //else MessageBox.Show("목록이 존재하지 않습니다");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 물류 - 반품 - 프로퍼티
        private ObservableCollection<Return_Info> lv_lo_refund_objectList;
        public ObservableCollection<Return_Info> Lv_Lo_Refund_ObjectList
        {
            get { return lv_lo_refund_objectList; }
            set
            {
                lv_lo_refund_objectList = value;
                OnPropertyChanged("Lv_Lo_Refund_ObjectList");
            }
        }

        private string tb_lo_refund_quantityContent;
        public string Tb_Lo_Refund_QuantityContent
        {
            get { return tb_lo_refund_quantityContent; }
            set
            {
                tb_lo_refund_quantityContent = value;
                OnPropertyChanged("Tb_Lo_Refund_QuantityContent");
            }
        }

        private string tb_lo_refund_productinfoContent;
        public string Tb_Lo_Refund_ProductInfoContent
        {
            get { return tb_lo_refund_productinfoContent; }
            set
            {
                tb_lo_refund_productinfoContent = value;
                OnPropertyChanged("Tb_Lo_Refund_ProductInfoContent");
            }
        }

        private string tb_lo_refund_productquantityContent;
        public string Tb_Lo_Refund_ProductQuantityContent
        {
            get { return tb_lo_refund_productquantityContent; }
            set
            {
                tb_lo_refund_productquantityContent = value;
                OnPropertyChanged("Tb_Lo_Refund_ProductQuantityContent");
            }
        }
        #endregion

        #endregion


        #region 지원팀
        //끝
        #region 직원조회

        #region 지원-직원조회- 조회버튼
        private ICommand btn_su_pre_searchcommand;
        public ICommand Btn_Su_Pre_SearchCommand
        {
            get { return (this.btn_su_pre_searchcommand) ?? (this.btn_su_pre_searchcommand = new DelegateCommand(Emp_Print_All)); }
        }

        string qr = string.Empty;
        int and_ck = 0;
        private void Check_and(int cnt)
        {
            cnt++;
            and_ck = cnt;
            if (and_ck > 1) qr += " and";
        }

        private void Emp_Clear_All()
        {
            EmpNameContent = null;
            EmpIDContent = null;
            EmpGenderContent = null;
            EmpPhoneContent = null;
            EmpStartDateContent = null;
            EmpEndDateContent = null;
        }

        private void Emp_Print_All()
        {
            try
            {
                List<Employee> emp_info;

                string ap = string.Empty;

                Lv_Emp_Search_ItemsSource = null;
                if (EmpNameContent != null)
                {
                    Check_and(and_ck);
                    qr += " name like @name";
                }
                if (EmpIDContent != null)
                {
                    Check_and(and_ck);
                    qr += " login_id like @login_id";
                }
                if (EmpGenderContent != null && EmpGenderContent != "모두")
                {
                    Check_and(and_ck);
                    qr += " gender like @gender";
                }
                if (EmpPhoneContent != null)
                {
                    Check_and(and_ck);
                    qr += " phone like @phone";
                }
                if (EmpStartDateContent != null)
                {
                    Check_and(and_ck);
                    qr += " start_date=@start_date";
                }
                if (EmpEndDateContent != null)
                {
                    Check_and(and_ck);
                    qr += " end_date=@end_date";
                }

                if (qr != "")
                {
                    ap = " where" + qr;
                }

                emp_info = db.GetList_Emp_info(ap, EmpIDContent, EmpNameContent, EmpPhoneContent, EmpGenderContent, EmpStartDateContent, EmpEndDateContent);
                Lv_Emp_Search_ItemsSource = emp_info;
                qr = "";
                and_ck = 0;
                Emp_Clear_All();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion


        #endregion


        #region 직원관리

        #region 지원 - 직원관리 - 조회버튼

        private ICommand btn_su_employee_search;
        public ICommand Btn_su_employee_search
        {
            get { return (this.btn_su_employee_search) ?? (this.btn_su_employee_search = new DelegateCommand(Seek_Emp_id)); }
        }

        public void Seek_Emp_id()
        {
            int flag = 0;
            string social_n = string.Empty, social_n1 = string.Empty, social_n2 = string.Empty, str = string.Empty;

            Employee employee = new Employee();
            if (SuEmpId == null)
            {
                MessageBox.Show("ID를 입력해주세요");
            }
            else
            {
                try
                {
                    str = "";
                    employee = db.Get_Emloyee_info(int.Parse(SuEmpId));

                    All_Reset_Text();
                    su_Emp_Login_Id = employee.Login_id;
                    su_Emp_Name = employee.Name;
                    if (employee.Gender == "남성")
                    {
                        rb_Gender1 = true;
                        rb_Gender2 = false;
                    }
                    else
                    {
                        rb_Gender1 = false;
                        rb_Gender2 = true;
                    }
                    social_n = employee.Social_number;
                    social_n1 = "";
                    social_n2 = "";
                    flag = 0;
                    for (int i = 0; i < social_n.Length; i++)
                    {
                        if (social_n[i] == '-')
                        {
                            flag = 1;
                            continue;
                        }
                        if (flag == 0) social_n1 += social_n[i];
                        else social_n2 += social_n[i];
                    }
                    su_Emp_Social_n1 = social_n1;
                    su_Emp_Social_n2 = social_n2;


                    str = employee.Phone;
                    if (str.Length - 3 == 7)
                    {
                        su_Emp_Phone1 = str.Substring(0, 3);
                        su_Emp_Phone2 = str.Substring(0, 3);
                        su_Emp_Phone3 = str.Substring(0, 4);
                    }
                    else
                    {
                        su_Emp_Phone1 = str.Substring(0, 3);
                        su_Emp_Phone2 = str.Substring(3, 4);
                        su_Emp_Phone3 = str.Substring(7, 4);
                    }

                    su_Emp_Start_Date = employee.Start_date.ToString();
                    su_Emp_End_Date = employee.End_date.ToString();
                    su_Emp_Adress = employee.Address;
                    su_Emp_Email = employee.Email;
                    su_Emp_Rank = employee.Rank;

                    X_or_V();
                    Login_id_Enable = false;
                    Start_Date_Enable = false;
                    ImgSource = new BitmapImage(new Uri(http_uri + "/employee/" + employee.Login_id + "_" + employee.Rank + "_" + employee.Name + ".JPG", UriKind.Absolute));
                }
                catch
                {
                    All_Reset_Text();
                    MessageBox.Show("등록되지 않은 직원입니다\n지원팀에 문의해주세요");
                }
            }
        }

        #region 유효성 체크
        #region 유효성 - 이름
        private ICommand _NameTextChangedCommand;
        public ICommand NameTextChangedCommand
        {
            get { return (this._NameTextChangedCommand) ?? (this._NameTextChangedCommand = new DelegateCommand(su_em_valid_name_check)); }
        }

        private void su_em_valid_name_check()// 이름 올바른지 확인
        {
            if (su_Emp_Name != "" && su_Emp_Name != null)
            {
                Name_Check = "V";
                Name_for= Brushes.Green;
            }
            else
            {
                Name_Check = "X";
                Name_for = Brushes.Red;
            }
        }
        #endregion
        #region 유효성 - id
        private ICommand _LoginIdTextChangedCommand;
        public ICommand LoginIdTextChangedCommand
        {
            get { return (this._LoginIdTextChangedCommand) ?? (this._LoginIdTextChangedCommand = new DelegateCommand(su_em_valid_id_check)); }
        }

        private void su_em_valid_id_check()
        {
            if (su_Emp_Login_Id != "" && su_Emp_Login_Id != null)
            {
                Login_Id_Check = "V";
                Login_id_for = Brushes.Green;
            }
            else
            {
                Login_Id_Check = "X";
                Login_id_for = Brushes.Red;
            }
        }
        #endregion
        #region 유효성 - Rank
        private ICommand _RankTextChangedCommand;
        public ICommand RankTextChangedCommand
        {
            get { return (this._RankTextChangedCommand) ?? (this._RankTextChangedCommand = new DelegateCommand(su_em_valid_rank_check)); }
        }

        private void su_em_valid_rank_check()
        {
            if (su_Emp_Rank != "" && su_Emp_Rank != null)
            {
                Rank_Check = "V";
                Rank_for = Brushes.Green;
            }
            else
            {
                Rank_Check = "X";
                Rank_for = Brushes.Red;
            }
        }
        #endregion
        #region 유효성 - Email
        private ICommand _EmailTextChangedCommand;
        public ICommand EmailTextChangedCommand
        {
            get { return (this._EmailTextChangedCommand) ?? (this._EmailTextChangedCommand = new DelegateCommand(IsValidEmail)); }
        }

        public void IsValidEmail()//이메일이 유효한 이메일인지 확인
        {
            string em;
            if (su_Emp_Email == null || su_Emp_Email == "")
            {
                em = "";
            }
            else
            {
                em = su_Emp_Email;
            }
            bool? valid = Regex.IsMatch(em, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
            if (valid == true)
            {
                Email_Check = "V";
                Email_for = Brushes.Green;
            }
            else
            {
                Email_Check = "X";
                Email_for = Brushes.Red;
            }
        }
        #endregion
        #region 유효성 - Address
        private ICommand _AddressTextChangedCommand;
        public ICommand AddressTextChangedCommand
        {
            get { return (this._AddressTextChangedCommand) ?? (this._AddressTextChangedCommand = new DelegateCommand(su_em_valid_address_check)); }
        }

        void su_em_valid_address_check() // 주소가 유효한지 확인
        {
            if (su_Emp_Adress != "" && su_Emp_Adress != null)
            {
                Adress_Check = "V";
                Adress_for = Brushes.Green;
            }
            else
            {
                Adress_Check = "X";
                Adress_for = Brushes.Red;
            }
        }
        #endregion
        #region 유효성 - 성별
        private ICommand _GenderChangedCommand;
        public ICommand GenderChangedCommand
        {
            get { return (this._GenderChangedCommand) ?? (this._GenderChangedCommand = new DelegateCommand(su_em_valid_gender_check)); }
        }

        void su_em_valid_gender_check()
        {
            if (rb_Gender1 == true || rb_Gender2 == true)
            {
                Gender_Check = "V";
                Gender_for = Brushes.Green;
            }
            else
            {
                Gender_Check = "X";
                Gender_for = Brushes.Red;
            }
        }

        #endregion
        #region 유효성 - 주민
        private ICommand _SocialChangedCommand;
        public ICommand SocialChangedCommand
        {
            get { return (this._SocialChangedCommand) ?? (this._SocialChangedCommand = new DelegateCommand(su_em_valid_social_check)); }
        }

        public bool IsNumeric(string source)
        {

            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(source);

        }
        void su_em_valid_social_check()
        {
            if (su_Emp_Social_n1 != null && su_Emp_Social_n1.Length == 6 && su_Emp_Social_n2 != null && su_Emp_Social_n2.Length == 7 && IsNumeric(su_Emp_Social_n1 + su_Emp_Social_n2) == true)
            {
                Social_Check = "V";
                Social_for = Brushes.Green;
            }
            else
            {
                Social_Check = "X";
                Social_for = Brushes.Red;
            }
        }
        #endregion
        #region 유효성 - phone
        private ICommand _PhoneChangedCommand;
        public ICommand PhoneChangedCommand
        {
            get { return (this._PhoneChangedCommand) ?? (this._PhoneChangedCommand = new DelegateCommand(su_em_valid_phone_check)); }
        }

        void su_em_valid_phone_check()
        {
            if (su_Emp_Phone1 != null && su_Emp_Phone1.Length == 3 && su_Emp_Phone3 != null && su_Emp_Phone3.Length == 4 && su_Emp_Phone2 != null && (su_Emp_Phone2.Length == 3 || su_Emp_Phone2.Length == 4)
                && IsNumeric(su_Emp_Phone1 + su_Emp_Phone2 + su_Emp_Phone3) == true)
            {
                Phone_Check = "V";
                Phone_for = Brushes.Green;
            }
            else
            {
                Phone_Check = "X";
                Phone_for = Brushes.Red;
            }
        }
        #endregion
        public bool V_All_Check()
        {
            if (Rank_Check == "V" && Login_Id_Check == "V" && Gender_Check == "V" && Name_Check == "V" &&
                Social_Check == "V" && Phone_Check == "V" && Email_Check == "V" && Adress_Check == "V")
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
            su_em_valid_name_check();
            su_em_valid_id_check();
            su_em_valid_rank_check();
            IsValidEmail();
            su_em_valid_address_check();
            su_em_valid_gender_check();
            su_em_valid_social_check();
            su_em_valid_phone_check();
        }

        public IEnumerable GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #endregion

        //끝
        #region 지원 - 직원관리 - 비밀번호 초기화 버튼
        private ICommand btn_su_em_change_pw;
        public ICommand Btn_Su_Em_Change_Pw
        {
            get { return (this.btn_su_em_change_pw) ?? (this.btn_su_em_change_pw = new DelegateCommand(Emp_Change_Pw)); }
        }

        public void Emp_Change_Pw()
        {
            Employee employee = new Employee();

            if (SuEmpId != null)
            {
                if (MessageBox.Show("비밀번호를 초기화 하시겠습니까?", "알림창", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        employee = db.Get_Emloyee_info(int.Parse(SuEmpId));
                        db.Reset_PW_EMP(employee.Login_id);
                        MessageBox.Show("비밀번호 변경완료");
                    }
                    catch
                    {
                        MessageBox.Show("비밀번호 변경불가");
                    }
                }
            }
            else
            {
                MessageBox.Show("ID가 입력되지 않았습니다");
            }
        }
        #endregion //완료

        #region 지원 - 직원관리 - 변경버튼
        private ICommand _Btn_Change_Emp;
        public ICommand Btn_Change_Emp
        {
            get { return (this._Btn_Change_Emp) ?? (this._Btn_Change_Emp = new DelegateCommand(Change_Info_Emp)); }
        }

        void Change_Info_Emp()
        {
            string rb_gender_check = string.Empty;
            string social_1and2 = string.Empty;
            string phone = string.Empty;
            DateTime? end_d = null;

            if (rb_Gender1 == true) rb_gender_check = "남성";
            else if (rb_Gender2 == true) rb_gender_check = "여성";
            social_1and2 = su_Emp_Social_n1 + "-" + su_Emp_Social_n2;
            phone = su_Emp_Phone1 + su_Emp_Phone2 + su_Emp_Phone3;

            if (su_Emp_End_Date == "해당사항없음") end_d = null;
            try
            {
                if (V_All_Check() == true)
                {
                    db.Update_Emp_Info(su_Emp_Login_Id, su_Emp_Rank, su_Emp_Name,
                    rb_gender_check, social_1and2, phone, su_Emp_Email, su_Emp_Adress, end_d);
                    if (ImgText != "" && ImgText != null)
                        FtpUploadFile(ImgText, ftp_uri + "/employee/" + su_Emp_Login_Id + "_" + su_Emp_Rank + "_" + su_Emp_Name + ".JPG");
                    MessageBox.Show("완료");
                }
                else
                {
                    MessageBox.Show("값이 채워지지 않음");
                }
            }
            catch
            {
                MessageBox.Show("비어있는 값이 있습니다");
            }

        }
        #endregion

        #region 지원 - 직원관리 - 초기화버튼
        private ICommand btn_su_text_reset;
        public ICommand Btn_Su_Text_Reset
        {
            get { return (this.btn_su_text_reset) ?? (this.btn_su_text_reset = new DelegateCommand(All_Reset_Text)); }
        }
        public void All_Reset_Text()
        {
            //SuEmpId = null;
            su_Emp_Login_Id = null;
            Login_Id_Check = null;
            su_Emp_Rank = null;
            su_Emp_Name = null;
            rb_Gender1 = false;
            rb_Gender2 = false;
            su_Emp_Social_n1 = null;
            su_Emp_Social_n2 = null;
            su_Emp_Phone1 = null;
            su_Emp_Phone2 = null;
            su_Emp_Phone3 = null;
            su_Emp_Email = null;
            su_Emp_Adress = null;
            su_Emp_Start_Date = null;
            su_Emp_End_Date = null;
            ImgSource = null;
            X_or_V();
        }

        #endregion

        #region 지원 - 직원관리 - 사진 변경 버튼
        private ICommand _Btn_Img_Input;
        public ICommand Btn_Img_Input
        {
            get { return (this._Btn_Img_Input) ?? (this._Btn_Img_Input = new DelegateCommand(Input_Image)); }
        }

        void Input_Image()
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
                ImgText = filename;

                ImgSource = new BitmapImage(new Uri(@dlg.FileName, UriKind.Absolute));
            }
        }
        #endregion

        #region 지원 -직원관리 - 신규 가입 조회
        private ICommand _Sign_Up_Command;
        public ICommand Sign_Up_Command
        {
            get { return (this._Sign_Up_Command) ?? (this._Sign_Up_Command = new DelegateCommand(Sign_Up_Print_All)); }
        }

        void Sign_Up_Print_All()
        {
            try
            {
                List<Sign_up> sign_up_list = new List<Sign_up>();
                sign_up_list = db.GetList_Sign_Up_Emp();
                Lv_Emp_Sign_ItemsSource = sign_up_list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + " ");
            }
        }
        #endregion

        #region 지원 - 직원관리 - 리스트 더블클릭
        private ICommand _Lv_Emp_DoubleClickCommand;
        public ICommand Lv_Emp_DoubleClickCommand
        {
            get { return (this._Lv_Emp_DoubleClickCommand) ?? (this._Lv_Emp_DoubleClickCommand = new DelegateCommand(Update_DB_New_Emp)); }
        }

        void Update_DB_New_Emp()
        {
            try
            {
                emp_sign_up_Window = new Emp_Sign_up();
                emp_sign_up_Window.DataContext = this;
                emp_sign_up_Window.Show();
            }
            catch (Exception ex)
            {

            }
        }

        #region 직원관리 sign_up 콤보박스
        private ICommand _DropdownCommand_Select_Team;
        public ICommand DropdownCommand_Select_Team
        {
            get { return (this._DropdownCommand_Select_Team) ?? (this._DropdownCommand_Select_Team = new DelegateCommand(cb_select_team_DropDownClosed)); }
        }

        private void cb_select_team_DropDownClosed()
        {
            string team_reader = string.Empty;
            mbDB mbdb = new mbDB();
            Employee employee = new Employee();

            if (Combo_Select_Team_Text == "지원팀") team_reader = "지원팀장";
            else if (Combo_Select_Team_Text == "판매팀") team_reader = "판매팀장";
            else if (Combo_Select_Team_Text == "물류팀") team_reader = "물류팀장";
            else if (Combo_Select_Team_Text == "지원팀장") team_reader = "지원팀";
            else if (Combo_Select_Team_Text == "판매팀장") team_reader = "판매팀";
            else if (Combo_Select_Team_Text == "물류팀장") team_reader = "물류팀";

            employee = mbdb.locate_Emp_Id(Combo_Select_Team_Text, team_reader);
            Job_Number = (employee.Employee_id + 1).ToString();
        }
        #endregion

        #region 직원관리 sign_up Ok버튼
        private ICommand _Signup_Ok;
        public ICommand Signup_Ok
        {
            get { return (this._Signup_Ok) ?? (this._Signup_Ok = new DelegateCommand(Update_Sign_up)); }
        }

        private Nullable<DateTime> dt = null;
        void Update_Sign_up()
        {
            Sign_up real_Sign_up = new Sign_up();

            try
            {
                //로그인 아이디로 sign_up에서 가져오고
                real_Sign_up = db.Sign_Up_Data_Catch(Sign_up_SelectItem);
                //cb, tb에서 값 가져와서 다 넣어주고 업데이트
                db.Upload_Emp_Sign_up(int.Parse(Job_Number), Combo_Select_Team_Text, DateTime.Now, dt, real_Sign_up);
                db.Delete_Sign_up(Sign_up_SelectItem.Login_id);
                MessageBox.Show("등록되었습니다");
                //sign_up에 delete
                //select item delete
                Sign_Up_Print_All();
                Sign_up_Close();
                //mainwindow1.WindowState = WindowState.Normal;
                //mainwindow1.Activate();
            }
            catch
            {
                MessageBox.Show("직무를 지정해주세요");
            }

        }
        #endregion

        #region 직원관리 sign_up Close 버튼
        private ICommand _Signup_Close;
        public ICommand Signup_Close
        {
            get { return (this._Signup_Close) ?? (this._Signup_Close = new DelegateCommand(Sign_up_Close)); }
        }

        void Sign_up_Close()
        {
            Job_Number = null;
            emp_sign_up_Window.Close();
        }

        #endregion

        #endregion
        #endregion
        //끝

        #region 고객관리

        #region 고객관리 - 등록/변경 버튼
        private ICommand _Btn_su_cus_register;
        public ICommand Btn_su_cus_register
        {
            get { return (this._Btn_su_cus_register) ?? (this._Btn_su_cus_register = new DelegateCommand(Cus_Register_Change)); }
        }


        private int res_change_check = 0;
        void Cus_Register_Change()
        {
            string gen = string.Empty;

            if (res_change_check == 0)//신규등록
            {
                try
                {
                    if (Gender_Check_M == false && Gender_Check_F == true)
                    { gen = "여성"; }
                    else if (Gender_Check_M == true && Gender_Check_F == false)
                    { gen = "남성"; }

                    if (tb_Cus_Savings == null)
                    {
                        tb_Cus_Savings = "0";
                    }

                    db.Insert_Cus_Info(tb_Cus_Name, gen, Cus_Birth_Content
                        , tb_Cus_Phone, long.Parse(tb_Cus_Savings));
                    Cus_All_Reset();
                    MessageBox.Show("완료");
                }
                catch
                {
                    MessageBox.Show("입력되지 않은 값이 있습니다");
                }
            }
            else
            {
                try
                {
                    if (Gender_Check_M == false && Gender_Check_F == true)
                    { gen = "여성"; }
                    else if (Gender_Check_M == true && Gender_Check_F == false)
                    { gen = "남성"; }

                    db.Update_Cus_Info(int.Parse(tb_Cus_Id), tb_Cus_Name, gen, Cus_Birth_Content
                        , tb_Cus_Phone, long.Parse(tb_Cus_Savings));
                    Cus_All_Reset();
                    MessageBox.Show("완료");
                }
                catch
                {
                    MessageBox.Show("입력되지 않은 값이 있습니다");
                }
            }
        }
        #endregion

        #region 고객관리 - 초기화 버튼
        private ICommand btn_su_cus_reset;
        public ICommand Btn_Su_Cus_Reset
        {
            get { return (this.btn_su_cus_reset) ?? (this.btn_su_cus_reset = new DelegateCommand(Cus_All_Reset)); }
        }

        void Cus_All_Reset()
        {
            Label_First = "신규등록";
            Btn_search_res_Content = "등록";
            res_change_check = 0;
            tb_Cus_Id = null;
            tb_Cus_Name = null;
            Gender_Check_F = false;
            Gender_Check_M = false;
            tb_Cus_Phone = null;
            Cus_Birth_Content = DateTime.Now;
            Cus_Birth_Content = null;
            tb_Cus_Savings = null;
            cus_id_label_visible = Visibility.Hidden;
            cus_id_text_visible = Visibility.Hidden;
        }

        #endregion

        #region 고객관리 - 고객조회 버튼
        private ICommand btn_su_search;
        public ICommand Btn_Su_Search
        {
            get { return (this.btn_su_search) ?? (this.btn_su_search = new DelegateCommand(Searching_Cus)); }
        }

        private void Searching_Cus()//지원 -> 고객조회 -> 조회버튼
        {
            List<Customer> customers;
            string ap = string.Empty;
            Lv_Cus_Search_ItemsSource = null;
            if (Cus_Name_Search != null)
            {
                Check_and(and_ck);
                qr += " name like @name";
            }
            else
            {
                Cus_Name_Search = "";
            }
            if (Cus_Gender_Search != null)
            {
                Check_and(and_ck);
                qr += " gender=@gender";
            }
            else
            {
                Cus_Gender_Search = "";
            }
            if (Cus_Phone_Search != null)
            {
                Check_and(and_ck);
                qr += " phone like @phone";
            }
            else
            {
                Cus_Phone_Search = "";
            }

            if (qr != "") ap = " where" + qr;

            try
            {
                customers = db.GetList_Customer_Search(Cus_Name_Search, Cus_Gender_Search, Cus_Phone_Search, ap); //데이터 바인딩 & 전체출력
                Lv_Cus_Search_ItemsSource = customers;
            }
            catch
            {
                MessageBox.Show("실패");
            }
            Cus_Name_Search = null;
            Cus_Gender_Search = null;
            Cus_Phone_Search = null;
            qr = "";
            and_ck = 0;
        }

        #endregion//끝

        #region 고객관리 - 고객리스트 더블클릭
        private ICommand lv_cus_doublieclickcommand;
        public ICommand Lv_Cus_DoubleClickCommand
        {
            get { return (this.lv_cus_doublieclickcommand) ?? (this.lv_cus_doublieclickcommand = new DelegateCommand(Cus_Info_Change)); }
        }

        private Visibility _cus_id_label_visible;
        public Visibility cus_id_label_visible
        {
            get
            {
                return _cus_id_label_visible;
            }
            set
            {
                _cus_id_label_visible = value;

                OnPropertyChanged("cus_id_label_visible");
            }
        }

        private Visibility _cus_id_text_visible;
        public Visibility cus_id_text_visible
        {
            get
            {
                return _cus_id_text_visible;
            }
            set
            {
                _cus_id_text_visible = value;

                OnPropertyChanged("cus_id_text_visible");
            }
        }

        private void Cus_Info_Change()
        {
            Customer customer = new Customer();
            try
            {
                Label_First = "정보변경";
                Btn_search_res_Content = "변경";
                customer = Lv_SelectItem;
                tb_Cus_Id = customer.Id.ToString();
                tb_Cus_Name = customer.Name;
                tb_Cus_Phone = customer.Phone;
                Cus_Birth_Content = customer.Date;
                tb_Cus_Savings = customer.Savings.ToString();

                if (customer.Gender == "남성")
                {
                    Gender_Check_M = true;
                    Gender_Check_F = false;
                }
                else
                {
                    Gender_Check_M = false;
                    Gender_Check_F = true;
                }
                res_change_check = 1;
                cus_id_label_visible = Visibility.Visible;
                cus_id_text_visible = Visibility.Visible;
            }
            catch (Exception ex)
            {
            }
        }


        #endregion

        //piechart만 움직이는거만하면됌
        #region 통계자료

        private ICommand dropdowncommand_p1;
        public ICommand DropDownCommand_p1
        {
            get { return (this.dropdowncommand_p1) ?? (this.dropdowncommand_p1 = new DelegateCommand(DropDown_PieChart1)); }
        }
        private void DropDown_PieChart1()
        {
            try
            {
                List<Chart_Brsell> chart_bList = new List<Chart_Brsell>();
                string Query = string.Empty;
                piechartData_1.Clear();
                try
                {
                    if (PieChart1_Content == "월별")
                    {
                        Query = "AND DATEPART(MM,sh.sales_date) = MONTH(GETDATE())";
                        chart_bList = db.Get_Sell_Unit(Query);
                        for (int i = 0; i < chart_bList.Count; i++)
                        {
                            Input_Pie_Chart_1(chart_bList[i].Brand, chart_bList[i].Count);
                        }
                    }
                    else if (PieChart1_Content == "분기별")
                    {
                        Query = "AND DATEPART(QUARTER, sh.sales_date) = DATEPART(QUARTER, GETDATE())";
                        chart_bList = db.Get_Sell_Unit(Query);
                        for (int i = 0; i < chart_bList.Count; i++)
                        {
                            Input_Pie_Chart_1(chart_bList[i].Brand, chart_bList[i].Count);
                        }
                    }
                    else if (PieChart1_Content == "년별")
                    {
                        Query = "";
                        chart_bList = db.Get_Sell_Unit(Query);
                        for (int i = 0; i < chart_bList.Count; i++)
                        {
                            Input_Pie_Chart_1(chart_bList[i].Brand, chart_bList[i].Count);
                        }
                    }

                    // Define the collection of Values to display in the Pie Chart
                    P1_Series = piechartData_1;
                    // Set the legend location to appear in the Right side of the chart
                    //pieChart1.LegendLocation = LegendLocation.Right;
                }
                catch
                {
                    MessageBox.Show("다시 선택");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private ICommand dropdowncommand_p2;
        public ICommand DropDownCommand_p2
        {
            get { return (this.dropdowncommand_p2) ?? (this.dropdowncommand_p2 = new DelegateCommand(DropDown_PieChart2)); }
        }
        private void DropDown_PieChart2()
        {
            List<Chart_SellKing> chart_sList = new List<Chart_SellKing>();
            string Query = string.Empty;
            piechartData_2.Clear();

            try
            {
                if (PieChart2_Content == "월별")
                {
                    Query = "AND DATEPART(MM,sh.sales_date) = MONTH(GETDATE()))";
                    chart_sList = db.PieChart_Sell_King(Query);
                    for (int i = 0; i < chart_sList.Count; i++)
                    {
                        Input_Pie_Chart_2(chart_sList[i].Sell_Price, chart_sList[i].Name);
                    }
                }
                else if (PieChart2_Content == "분기별")
                {
                    Query = "AND  DATEPART(QUARTER,sh.sales_date) = DATEPART(QUARTER,GETDATE()))";
                    chart_sList = db.PieChart_Sell_King(Query);
                    for (int i = 0; i < chart_sList.Count; i++)
                    {
                        Input_Pie_Chart_2(chart_sList[i].Sell_Price, chart_sList[i].Name);
                    }
                }
                else if (PieChart2_Content == "년별")
                {
                    Query = ")";
                    chart_sList = db.PieChart_Sell_King(Query);
                    for (int i = 0; i < chart_sList.Count; i++)
                    {
                        Input_Pie_Chart_2(chart_sList[i].Sell_Price, chart_sList[i].Name);
                    }
                }

                // Define the collection of Values to display in the Pie Chart
                P2_Series = piechartData_2;
                // Set the legend location to appear in the Right side of the chart
                //pieChart2.LegendLocation = LegendLocation.Right;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand dropdowncommand_p3;
        public ICommand DropDownCommand_p3
        {
            get { return (this.dropdowncommand_p3) ?? (this.dropdowncommand_p3 = new DelegateCommand(DropDown_PieChart3)); }
        }
        private void DropDown_PieChart3()
        {
            try
            {
                List<Chart_Sellproduct> chart_spList = new List<Chart_Sellproduct>();
                string Query = string.Empty;
                piechartData_3.Clear();


                if (PieChart3_Content == "월별")
                {
                    Query = "AND DATEPART(MM,sh.sales_date) = MONTH(GETDATE()))";
                    chart_spList = db.PieChart_Sell_Product(Query);
                    for (int i = 0; i < chart_spList.Count; i++)
                    {
                        Input_Pie_Chart_3(chart_spList[i].Product_Name, chart_spList[i].Prouct_Cnt);
                    }
                }
                else if (PieChart3_Content == "분기별")
                {
                    Query = "AND  DATEPART(QUARTER,sh.sales_date) = DATEPART(QUARTER,GETDATE()))";
                    chart_spList = db.PieChart_Sell_Product(Query);
                    for (int i = 0; i < chart_spList.Count; i++)
                    {
                        Input_Pie_Chart_3(chart_spList[i].Product_Name, chart_spList[i].Prouct_Cnt);
                    }
                }
                else if (PieChart3_Content == "년별")
                {
                    Query = ")";
                    chart_spList = db.PieChart_Sell_Product(Query);
                    for (int i = 0; i < chart_spList.Count; i++)
                    {
                        Input_Pie_Chart_3(chart_spList[i].Product_Name, chart_spList[i].Prouct_Cnt);
                    }
                }

                // Define the collection of Values to display in the Pie Chart
                P3_Series = piechartData_3;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand pie1clickcommand;
        public ICommand Pie1ClickCommand
        {
            get { return (this.pie1clickcommand) ?? (this.pie1clickcommand = new RelayCommand<ChartPoint>(OnDrillDownCommand)); }
        }

        private void OnDrillDownCommand(ChartPoint chartPoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartPoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartPoint.SeriesView;
            selectedSeries.PushOut = 8;
            // access the chartPoint.Instance (Cast it to YourObjectHere and access its properties)
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
                MessageBox.Show(e + " ");
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
                MessageBox.Show(e + " ");
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
                MessageBox.Show(e + " ");
            }
        }

        private ICommand basic_chart_reset;
        public ICommand Basic_Chart_Reset
        {
            get { return (this.basic_chart_reset) ?? (this.basic_chart_reset = new DelegateCommand(Btn_BChart_Reset)); }
        }

        void Btn_BChart_Reset()
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


                while (Series.Count > 0)
                {
                    Series.RemoveAt(Series.Count - 1);
                }


                Series.Add(new ColumnSeries
                {
                    Title = last_basic_list[0].Year_Check.ToString(),
                    Values = new ChartValues<long>(price_list_last)
                });

                Series.Add(new ColumnSeries
                {
                    Title = this_basic_list[0].Year_Check.ToString(),
                    Values = new ChartValues<long>(price_list_this)
                });

            }
            catch
            {
                MessageBox.Show("다시 선택");
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


            while (Series.Count > 0)
            {
                Series.RemoveAt(Series.Count - 1);
            }


            Series.Add(new ColumnSeries
            {
                Title = last_basic_list[0].Year_Check.ToString(),
                Values = new ChartValues<long>(price_list_last)
            });

            Series.Add(new ColumnSeries
            {
                Title = this_basic_list[0].Year_Check.ToString(),
                Values = new ChartValues<long>(price_list_this)
            });

            P1_Series = piechartData_1;
            P2_Series = piechartData_2;
            P3_Series = piechartData_3;
        }

        #endregion
        #endregion

        #endregion


        #region properties

        private List<Customer> _Lv_Cus_Search_ItemsSource;
        public List<Customer> Lv_Cus_Search_ItemsSource
        {
            get { return _Lv_Cus_Search_ItemsSource; }
            set
            {
                _Lv_Cus_Search_ItemsSource = value;
                OnPropertyChanged("Lv_Cus_Search_ItemsSource");
            }
        }

        private List<Sign_up> _Lv_Emp_Sign_ItemsSource;
        public List<Sign_up> Lv_Emp_Sign_ItemsSource
        {
            get { return _Lv_Emp_Sign_ItemsSource; }
            set
            {
                _Lv_Emp_Sign_ItemsSource = value;
                OnPropertyChanged("Lv_Emp_Sign_ItemsSource");
            }
        }

        private List<Employee> _Lv_Emp_Search_ItemsSource;
        public List<Employee> Lv_Emp_Search_ItemsSource
        {
            get { return _Lv_Emp_Search_ItemsSource; }
            set
            {
                _Lv_Emp_Search_ItemsSource = value;
                OnPropertyChanged("Lv_Emp_Search_ItemsSource");
            }
        }

        private Boolean _Login_id_Enable;
        public Boolean  Login_id_Enable
        {
            get { return _Login_id_Enable; }
            set
            {
                _Login_id_Enable = value;
                OnPropertyChanged("Login_id_Enable");
            }
        }

        private Boolean _Start_Date_Enable;
        public Boolean Start_Date_Enable
        {
            get { return _Start_Date_Enable; }
            set
            {
                _Start_Date_Enable = value;
                OnPropertyChanged("Start_Date_Enable");
            }
        }

        private System.Windows.Media.Brush _Login_id_for;
        public System.Windows.Media.Brush Login_id_for
        {
            get { return _Login_id_for; }
            set
            {
                _Login_id_for = value;
                OnPropertyChanged("Login_id_for");
            }
        }

        private System.Windows.Media.Brush _End_Date_for;
        public System.Windows.Media.Brush End_Date_for
        {
            get { return _End_Date_for; }
            set
            {
                _End_Date_for = value;
                OnPropertyChanged("End_Date_for");
            }
        }

        private System.Windows.Media.Brush _Start_Date_for;
        public System.Windows.Media.Brush Start_Date_for
        {
            get { return _Start_Date_for; }
            set
            {
                _Start_Date_for = value;
                OnPropertyChanged("Start_Date_for");
            }
        }

        private System.Windows.Media.Brush _Adress_for;
        public System.Windows.Media.Brush Adress_for
        {
            get { return _Adress_for; }
            set
            {
                _Adress_for = value;
                OnPropertyChanged("Adress_for");
            }
        }

        private System.Windows.Media.Brush _Email_for;
        public System.Windows.Media.Brush Email_for
        {
            get { return _Email_for; }
            set
            {
                _Email_for = value;
                OnPropertyChanged("Email_for");
            }
        }

        private System.Windows.Media.Brush _Phone_for;
        public System.Windows.Media.Brush Phone_for
        {
            get { return _Phone_for; }
            set
            {
                _Phone_for = value;
                OnPropertyChanged("Phone_for");
            }
        }

        private System.Windows.Media.Brush _Social_for;
        public System.Windows.Media.Brush Social_for
        {
            get { return _Social_for; }
            set
            {
                _Social_for = value;
                OnPropertyChanged("Social_for");
            }
        }

        private System.Windows.Media.Brush _Gender_for;
        public System.Windows.Media.Brush Gender_for
        {
            get { return _Gender_for; }
            set
            {
                _Gender_for = value;
                OnPropertyChanged("Gender_for");
            }
        }

        private System.Windows.Media.Brush _Name_for;
        public System.Windows.Media.Brush Name_for
        {
            get { return _Name_for; }
            set
            {
                _Name_for = value;
                OnPropertyChanged("Name_for");
            }
        }

        private System.Windows.Media.Brush _Rank_for;
        public System.Windows.Media.Brush Rank_for
        {
            get { return _Rank_for; }
            set
            {
                _Rank_for = value;
                OnPropertyChanged("Rank_for");
            }
        }

        private Sign_up _Sign_up_SelectItem;
        public Sign_up Sign_up_SelectItem
        {
            get { return _Sign_up_SelectItem; }
            set
            {
                _Sign_up_SelectItem = value;
                OnPropertyChanged("Sign_up_SelectItem");
            }
        }

        private string _ImgText;
        public string ImgText
        {
            get { return _ImgText; }
            set
            {
                _ImgText = value;
                OnPropertyChanged("ImgText");
            }
        }

        private string _Job_Number;
        public string Job_Number
        {
            get { return _Job_Number; }
            set
            {
                _Job_Number = value;
                OnPropertyChanged("Job_Number");
            }
        }

        private string _Combo_Select_Team_Text;
        public string Combo_Select_Team_Text
        {
            get { return _Combo_Select_Team_Text; }
            set
            {
                _Combo_Select_Team_Text = value;
                OnPropertyChanged("Combo_Select_Team_Text");
            }
        }

        private List<string> _select_Team_ItemSource;
        public List<string> select_Team_ItemSource
        {
            get { return _select_Team_ItemSource; }
            set
            {
                _select_Team_ItemSource = value;
                OnPropertyChanged("select_Team_ItemSource");
            }
        }

        private Sign_up lv_signup_selectitem;
        public Sign_up Lv_Signup_Selectitem
        {
            get { return lv_signup_selectitem; }
            set
            {
                lv_signup_selectitem = value;
                OnPropertyChanged("Lv_Signup_Selectitem");
            }
        }

        private Customer lv_selectitem;
        public Customer Lv_SelectItem
        {
            get { return lv_selectitem; }
            set
            {
                lv_selectitem = value;
                OnPropertyChanged("lv_selectitem");
            }
        }

        private string btn_search_res_content;
        public string Btn_search_res_Content
        {
            get { return btn_search_res_content; }
            set
            {
                btn_search_res_content = value;
                OnPropertyChanged("Btn_search_res_Content");
            }
        }


        private string _Label_First;
        public string Label_First
        {
            get { return _Label_First; }
            set
            {
                _Label_First = value;
                OnPropertyChanged("Label_First");
            }
        }

        private string _tb_Cus_Id;
        public string tb_Cus_Id
        {
            get { return _tb_Cus_Id; }
            set
            {
                _tb_Cus_Id = value;
                OnPropertyChanged("tb_Cus_Id");
            }
        }

        private string _tb_Cus_Name;
        public string tb_Cus_Name
        {
            get { return _tb_Cus_Name; }
            set
            {
                _tb_Cus_Name = value;
                OnPropertyChanged("tb_Cus_Name");
            }
        }

        private bool _Gender_Check_M;
        public bool Gender_Check_M
        {
            get { return _Gender_Check_M; }
            set
            {
                _Gender_Check_M = value;
                OnPropertyChanged("Gender_Check_M");
            }
        }

        private bool _Gender_Check_F;
        public bool Gender_Check_F
        {
            get { return _Gender_Check_F; }
            set
            {
                _Gender_Check_F = value;
                OnPropertyChanged("Gender_Check_F");
            }
        }

        private string _tb_Cus_Phone;
        public string tb_Cus_Phone
        {
            get { return _tb_Cus_Phone; }
            set
            {
                _tb_Cus_Phone = value;
                OnPropertyChanged("tb_Cus_Phone");
            }
        }

        private DateTime? _DisplayDate_Birth;
        private DateTime? DisplayDate_Birth
        {
            get { return _DisplayDate_Birth; }
            set
            {
                _DisplayDate_Birth = value;
                OnPropertyChanged("DisplayDate_Birth");
            }
        }


        private DateTime? _Cus_Birth_Content;
        public DateTime? Cus_Birth_Content
        {
            get { return _Cus_Birth_Content; }
            set
            {
                _Cus_Birth_Content = value;
                OnPropertyChanged("Cus_Birth_Content");
            }
        }

        private string _tb_Cus_Savings;
        public string tb_Cus_Savings
        {
            get { return _tb_Cus_Savings; }
            set
            {
                _tb_Cus_Savings = value;
                OnPropertyChanged("tb_Cus_Savings");
            }
        }

        private string cus_name_search;
        public string Cus_Name_Search
        {
            get { return cus_name_search; }
            set
            {
                cus_name_search = value;
                OnPropertyChanged("Cus_Name_Search");
            }
        }

        private string cus_gender_search;
        public string Cus_Gender_Search
        {
            get { return cus_gender_search; }
            set
            {
                cus_gender_search = value;
                OnPropertyChanged("Cus_Gender_Search");
            }
        }

        private string cus_phone_search;
        public string Cus_Phone_Search
        {
            get { return cus_phone_search; }
            set
            {
                cus_phone_search = value;
                OnPropertyChanged("Cus_Phone_Search");
            }
        }

        private int index_p1;
        public int Index_P1
        {
            get { return index_p1; }
            set
            {
                index_p1 = value;
                OnPropertyChanged("Index_P1");
            }
        }

        private int index_p2;
        public int Index_P2
        {
            get { return index_p2; }
            set
            {
                index_p2 = value;
                OnPropertyChanged("Index_P2");
            }
        }

        private int index_p3;
        public int Index_P3
        {
            get { return index_p3; }
            set
            {
                index_p3 = value;
                OnPropertyChanged("Index_P3");
            }
        }

        private SeriesCollection p1_series;
        public SeriesCollection P1_Series
        {
            get { return p1_series; }
            set
            {
                p1_series = value;
                OnPropertyChanged("P1_Series");
            }
        }

        private SeriesCollection p2_series;
        public SeriesCollection P2_Series
        {
            get { return p2_series; }
            set
            {
                p2_series = value;
                OnPropertyChanged("P2_Series");
            }
        }

        private SeriesCollection p3_series;
        public SeriesCollection P3_Series
        {
            get { return p3_series; }
            set
            {
                p3_series = value;
                OnPropertyChanged("P3_Series");
            }
        }

        private string piechart1_content;
        public string PieChart1_Content
        {
            get { return piechart1_content; }
            set
            {
                piechart1_content = value;
                OnPropertyChanged("PieChart1_Content");
            }
        }

        private string piechart2_content;
        public string PieChart2_Content
        {
            get { return piechart2_content; }
            set
            {
                piechart2_content = value;
                OnPropertyChanged("PieChart2_Content");
            }
        }

        private string piechart3_content;
        public string PieChart3_Content
        {
            get { return piechart3_content; }
            set
            {
                piechart3_content = value;
                OnPropertyChanged("PieChart3_Content");
            }
        }

        private List<string> _RankItems;
        public List<string> RankItems
        {
            get { return _RankItems; }
            set
            {
                _RankItems = value;
                OnPropertyChanged("RankItems");
            }
        }

        private List<string> cb_piechart1;
        public List<string> cb_PieChart1
        {
            get { return cb_piechart1; }
            set
            {
                cb_piechart1 = value;
                OnPropertyChanged("cb_PieChart1");
            }
        }

        private List<string> cb_piechart2;
        public List<string> cb_PieChart2
        {
            get { return cb_piechart2; }
            set
            {
                cb_piechart2 = value;
                OnPropertyChanged("cb_PieChart2");
            }
        }

        private List<string> cb_piechart3;
        public List<string> cb_PieChart3
        {
            get { return cb_piechart3; }
            set
            {
                cb_piechart3 = value;
                OnPropertyChanged("cb_PieChart3");
            }
        }

        private string nameContent;
        public string EmpNameContent
        {
            get { return nameContent; }
            set
            {
                nameContent = value;
                OnPropertyChanged("EmpNameContent");
            }
        }

        private string genderContent;
        public string EmpGenderContent
        {
            get { return genderContent; }
            set
            {
                genderContent = value;
                OnPropertyChanged("EmpGenderContent");
            }
        }

        private string phoneContent;
        public string EmpPhoneContent
        {
            get { return phoneContent; }
            set
            {
                phoneContent = value;
                OnPropertyChanged("EmpPhoneContent");
            }
        }

        private string idContent;
        public string EmpIDContent
        {
            get { return idContent; }
            set
            {
                idContent = value;
                OnPropertyChanged("EmpIDContent");
            }
        }

        private DateTime? startdateContent;
        public DateTime? EmpStartDateContent
        {
            get { return startdateContent; }
            set
            {
                startdateContent = value;
                OnPropertyChanged("EmpStartDateContent");
            }
        }

        private DateTime? enddateContent;
        public DateTime? EmpEndDateContent
        {
            get { return enddateContent; }
            set
            {
                enddateContent = value;
                OnPropertyChanged("EmpEndDateContent");
            }
        }

        private string suempid;
        public string SuEmpId
        {
            get { return suempid; }
            set
            {
                suempid = value;
                OnPropertyChanged("SuEmpId");
            }
        }

        private string su_emp_login_id;
        public string su_Emp_Login_Id
        {
            get { return su_emp_login_id; }
            set
            {
                su_emp_login_id = value;
                OnPropertyChanged("su_Emp_Login_Id");
            }
        }

        private string su_emp_name;
        public string su_Emp_Name
        {
            get { return su_emp_name; }
            set
            {
                su_emp_name = value;
                OnPropertyChanged("su_Emp_Name");
            }
        }

        private string su_emp_social_n1;
        public string su_Emp_Social_n1
        {
            get { return su_emp_social_n1; }
            set
            {
                su_emp_social_n1 = value;
                OnPropertyChanged("su_Emp_Social_n1");
            }
        }

        private string su_emp_social_n2;
        public string su_Emp_Social_n2
        {
            get { return su_emp_social_n2; }
            set
            {
                su_emp_social_n2 = value;
                OnPropertyChanged("su_Emp_Social_n2");
            }
        }

        private string su_emp_phone1;
        public string su_Emp_Phone1
        {
            get { return su_emp_phone1; }
            set
            {
                su_emp_phone1 = value;
                OnPropertyChanged("su_Emp_Phone1");
            }
        }

        private string su_emp_phone2;
        public string su_Emp_Phone2
        {
            get { return su_emp_phone2; }
            set
            {
                su_emp_phone2 = value;
                OnPropertyChanged("su_Emp_Phone2");
            }
        }

        private string su_emp_phone3;
        public string su_Emp_Phone3
        {
            get { return su_emp_phone3; }
            set
            {
                su_emp_phone3 = value;
                OnPropertyChanged("su_Emp_Phone3");
            }
        }

        private string su_emp_email;
        public string su_Emp_Email
        {
            get { return su_emp_email; }
            set
            {
                su_emp_email = value;
                OnPropertyChanged("su_Emp_Email");
            }
        }

        private string su_emp_adress;
        public string su_Emp_Adress
        {
            get { return su_emp_adress; }
            set
            {
                su_emp_adress = value;
                OnPropertyChanged("su_Emp_Adress");
            }
        }

        private string su_emp_start_date;
        public string su_Emp_Start_Date
        {
            get { return su_emp_start_date; }
            set
            {
                su_emp_start_date = value;
                OnPropertyChanged("su_Emp_Start_Date");
            }
        }

        private string su_emp_end_date;
        public string su_Emp_End_Date
        {
            get { return su_emp_end_date; }
            set
            {
                su_emp_end_date = value;
                OnPropertyChanged("su_Emp_End_Date");
            }
        }

        private string su_emp_rank;
        public string su_Emp_Rank
        {
            get { return su_emp_rank; }
            set
            {
                su_emp_rank = value;
                OnPropertyChanged("su_Emp_Rank");
            }
        }

        private bool rb_gender1;
        public bool rb_Gender1
        {
            get { return rb_gender1; }
            set
            {
                rb_gender1 = value;
                OnPropertyChanged("rb_Gender1");
            }
        }

        private bool rb_gender2;
        public bool rb_Gender2
        {
            get { return rb_gender2; }
            set
            {
                rb_gender2 = value;
                OnPropertyChanged("rb_Gender2");
            }
        }

        private string login_id_check;
        public string Login_Id_Check
        {
            get { return login_id_check; }
            set
            {
                login_id_check = value;
                OnPropertyChanged("Login_Id_Check");
            }
        }

        private string rank_check;
        public string Rank_Check
        {
            get { return rank_check; }
            set
            {
                rank_check = value;
                OnPropertyChanged("Rank_Check");
            }
        }

        private string name_check;
        public string Name_Check
        {
            get { return name_check; }
            set
            {
                name_check = value;
                OnPropertyChanged("Name_Check");
            }
        }

        private string gender_check;
        public string Gender_Check
        {
            get { return gender_check; }
            set
            {
                gender_check = value;
                OnPropertyChanged("Gender_Check");
            }
        }

        private string social_check;
        public string Social_Check
        {
            get { return social_check; }
            set
            {
                social_check = value;
                OnPropertyChanged("Social_Check");
            }
        }

        private string phone_check;
        public string Phone_Check
        {
            get { return phone_check; }
            set
            {
                phone_check = value;
                OnPropertyChanged("Phone_Check");
            }
        }

        private string email_check;
        public string Email_Check
        {
            get { return email_check; }
            set
            {
                email_check = value;
                OnPropertyChanged("Email_Check");
            }
        }

        private string adress_check;
        public string Adress_Check
        {
            get { return adress_check; }
            set
            {
                adress_check = value;
                OnPropertyChanged("Adress_Check");
            }
        }

        private string start_date_check;
        public string Start_Date_Check
        {
            get { return start_date_check; }
            set
            {
                start_date_check = value;
                OnPropertyChanged("Start_Date_Check");
            }
        }

        private string end_date_check;
        public string End_Date_Check
        {
            get { return end_date_check; }
            set
            {
                end_date_check = value;
                OnPropertyChanged("End_Date_Check");
            }
        }

        private BitmapImage imgsource;
        public BitmapImage ImgSource
        {
            get { return imgsource; }
            set
            {
                imgsource = value;
                OnPropertyChanged("ImgSource");
            }
        }

        #endregion

        #region DelegateCommand Class
        public class DelegateCommand : ICommand
        {
            private readonly Func<bool> canExecute;
            private readonly Action execute;

            /// <summary>
            /// Initializes a new instance of the DelegateCommand class.
            /// </summary>
            /// <param name="execute">indicate an execute function</param>
            public DelegateCommand(Action execute) : this(execute, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the DelegateCommand class.
            /// </summary>
            /// <param name="execute">execute function </param>
            /// <param name="canExecute">can execute function</param>
            public DelegateCommand(Action execute, Func<bool> canExecute)
            {
                this.execute = execute;
                this.canExecute = canExecute;
            }
            /// <summary>
            /// can executes event handler
            /// </summary>
            public event EventHandler CanExecuteChanged;

            /// <summary>
            /// implement of icommand can execute method
            /// </summary>
            /// <param name="o">parameter by default of icomand interface</param>
            /// <returns>can execute or not</returns>
            public bool CanExecute(object o)
            {
                if (this.canExecute == null)
                {
                    return true;
                }
                return this.canExecute();
            }

            /// <summary>
            /// implement of icommand interface execute method
            /// </summary>
            /// <param name="o">parameter by default of icomand interface</param>
            public void Execute(object o)
            {
                this.execute();
            }

            /// <summary>
            /// raise ca excute changed when property changed
            /// </summary>
            public void RaiseCanExecuteChanged()
            {
                if (this.CanExecuteChanged != null)
                {
                    this.CanExecuteChanged(this, EventArgs.Empty);
                }
            }
        }
        #endregion

    }
}