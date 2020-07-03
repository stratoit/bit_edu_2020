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
using System.IO;
using MBStore_MVP.Model;
using MBStore_MVP.Presenter;

namespace MBStore_MVP.View
{
    /// <summary>
    /// Login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Login : Window, ILogin_view
    {
        string path = @"autoloing.txt";
        ILogin presenter;

        public Login()
        {
            InitializeComponent();
            presenter = new Pre_Login(this);
            if (File.Exists(path))
            {
                string[] value = File.ReadAllText(path).Split('#');
                if (value.Length == 2) 
                    func_login(value[0], value[1]);
            }

        }
        private void Path_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("프로그램을 종료하시겠습니까?", "프로그램 종료", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                this.Close();
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("계정 생성문의나 비밀번호를 잊어버리신 경우 지원팀에 문의하십시오.", "Help");
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btn_Login_Click(sender, e);
        }
        #region 로그인 함수
        public void func_login(string id, string pw)
        {
            Employee emp = new Employee();

            emp = presenter.SelectEmpId(id);

            try
            {
                //id와 pw가 일치하면
                if (id == emp.Login_id && pw == emp.Login_pw)
                {
                    //자동로그인 체크 검사
                    if (cb_auto.IsChecked == true)
                    {
                        string value;
                        value = tb_id.Text + "#" + presenter.ComputeSha256Hash(tb_id.Text + tb_pw.Password);
                        File.WriteAllText(path, value, Encoding.Default);
                    }
                    MainWindow main = new MainWindow(emp);
                    this.Close();
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
        #endregion
        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            func_login(tb_id.Text, presenter.ComputeSha256Hash(tb_id.Text + tb_pw.Password));
        }

        private void btn_signup_Click(object sender, RoutedEventArgs e)
        {
            SignUp su = new SignUp();
            su.Show(this);
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
