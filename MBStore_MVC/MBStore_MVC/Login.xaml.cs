using MBStore_MVC.Model;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace MBStore_MVC
{
    /// <summary>
    /// Login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Login : Window
    {
        string path = @"autoloing.txt";
       
        public Login()
        {
            InitializeComponent();
            if (File.Exists(path))
            {
                string[] value = File.ReadAllText(path).Split('#');
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

        private void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_close.Fill = Brushes.Gray;
        }

        private void btn_close_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_close.Fill = Brushes.White;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("계정 생성문의나 비밀번호를 잊어버리신 경우 지원팀에 문의하십시오.", "Help");
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btn_sign_Click(sender, e);
        }
        #region 로그인 함수
        private void func_login(string id, string pw)
        {
            mbDB db = new mbDB();

            string[] str = db.SelectEmpId(id).Split('#');
            string auth;

            try
            {
                //id와 pw가 일치하면
                if (id == str[0] && pw == str[1])
                {
                    auth = str[3] + "#" + str[2];
                    //자동로그인 체크 검사
                    if (cb_auto.IsChecked == true)
                    {
                        string value;
                        value = tb_id.Text + "#" + tb_pw.Password;
                        File.WriteAllText(path, value, Encoding.Default);
                    }
                    MainWindow main = new MainWindow(auth);
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
        private void btn_sign_Click(object sender, RoutedEventArgs e)
        {
            func_login(tb_id.Text, tb_pw.Password);
        }

    }
}
