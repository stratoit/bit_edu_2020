using MaterialDesignThemes.Wpf;
using MBStore_MVC.Model;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace MBStore_MVC
{
    /// <summary>
    /// SignUp.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SignUp : Window
    {
        mbDB db = new mbDB();
        string checked_id = "";
        public SignUp()
        {
            InitializeComponent();
        }


        #region 숫자 여부 확인
        public bool IsNumeric(string source)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(source);
        }
        #endregion


        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void pb_inputPW_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pb_inputPW.Password.Length >= 4)
                text_pw.Foreground = Brushes.Green;
            else
                text_pw.Foreground = Brushes.Red;
        }

        private void pb_checkPW_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pb_inputPW.Password == pb_checkPW.Password)
            {
                text_checkpw.Text = "V";
                text_checkpw.Foreground = Brushes.Green;
            }
            else
            {
                text_checkpw.Text = "X";
                text_checkpw.Foreground = Brushes.Red;
            }

        }

        private void btn_checkID_Click(object sender, RoutedEventArgs e)
        {
            if (tb_inputID.Text == "")
            {
                MessageBox.Show("아이디를 입력해주세요");

             
                tb_inputID.Focus();
            }
            else if (tb_inputID.Text.Length < 4)
            {
                MessageBox.Show("아이디는 4자 이상 16자 이하로 입력해주세요");

                tb_inputID.Focus();
            }
            else
            {
                if (db.Check_empID(tb_inputID.Text))
                {
                    MessageBox.Show("사용 가능한 ID 입니다");

                    checked_id = tb_inputID.Text;
                    btn_checkID.BorderBrush = Brushes.Black;
                    btn_checkID.BorderThickness = new Thickness(1);
                    pb_inputPW.Focus();
                }
                else
                {
                    MessageBox.Show("이미 존재하는 ID 입니다.");
                    tb_inputID.Focus();
                }
            }

        }

        private void btn_signup_Click(object sender, RoutedEventArgs e)
        {
            if (checked_id == "")
            {
                MessageBox.Show("아이디 중복확인이 필요합니다");
                btn_checkID.BorderBrush = Brushes.Red;
                btn_checkID.BorderThickness = new Thickness(2);
            }
            else if (pb_inputPW.Password == "")
            {
                MessageBox.Show("비밀번호를 입력해주세요");
                pb_inputPW.BorderBrush = Brushes.Red;
                pb_inputPW.BorderThickness = new Thickness(2);
            }
            else if (text_checkpw.Text == "" || text_checkpw.Text == "X")
            {
                MessageBox.Show("비밀번호가 일치하지 않습니다");

                pb_checkPW.BorderBrush = Brushes.Red;
                pb_checkPW.BorderThickness = new Thickness(2);
            }
            else if (tb_inputName.Text == "")
            {
                MessageBox.Show("이름을 정확하게 입력해주세요");

            }
            else if (tb_inputSocial_1.Text == "" || tb_inputSocial_2.Password == "")
            {
                MessageBox.Show("주민등록번호를 정확하게 입력해주세요");

            }
            else if (tb_inputAddress_1.Text == "" || tb_inputAddress_2.Text == "")
            {
                MessageBox.Show("주소를 정확하게 입력해주세요");
            }
            else if (tb_inputPhone_1.Text == "" || tb_inputPhone_2.Text == "")
            {
                MessageBox.Show("휴대전화번호를 정확하게 입력해주세요");
            }
            else if (tb_inputEmail_1.Text == "" || tb_inputEmail_2.Text == "")
            {
                MessageBox.Show("이메일을 정확하게 입력해주세요");
            }
            else
            {
                string gender;
                if (tb_inputSocial_2.Password.Substring(0, 1) == "1"
                    || tb_inputSocial_2.Password.Substring(0, 1) == "3")
                {
                    gender = "남성";
                }
                else
                {
                    gender = "여성";
                }

                Sha256 sha256 = new Sha256();
                string pw = sha256.ComputeSha256Hash(checked_id + pb_checkPW.Password);
                string social = tb_inputSocial_1.Text + "-" + tb_inputSocial_2.Password;
                string phone = cb_phone.Text + tb_inputPhone_1.Text + tb_inputPhone_2.Text;
                string address = tb_inputAddress_2.Text;
                string post_num = tb_inputAddress_1.Text;
                string email = tb_inputEmail_1.Text + "@" + tb_inputEmail_2.Text;
                string name = tb_inputName.Text;
                if (db.Insert_SignUp(name, checked_id, pw, post_num, gender, social, phone, address, email, DateTime.Now))
                {
                    MessageBox.Show("성공적으로 가입신청 되었습니다");

                    checked_id = "";
                    tb_inputID.Text = "";
                    pb_inputPW.Password = "";
                    pb_checkPW.Password = "";
                    tb_inputSocial_1.Text = "";
                    tb_inputSocial_2.Password = "";
                    tb_inputAddress_1.Text = "";
                    tb_inputAddress_2.Text = "";
                    tb_inputPhone_1.Text = "";
                    tb_inputPhone_2.Text = "";
                    tb_inputEmail_1.Text = "";
                    tb_inputEmail_2.Text = "";
                    tb_inputName.Text = "";
                    text_checkpw.Text = "";

                }
                else
                {
                    MessageBox.Show("성공적으로 가입신청 되었습니다");
                }
            }
        }

        private void tb_inputSocial_1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }

        private void tb_inputSocial_2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);

        }

        private void tb_inputSocial_2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (tb_inputSocial_2.Password.Length > 1)
            {
                if (tb_inputSocial_2.Password[0] < '1'
                    || tb_inputSocial_2.Password[0] > '4')
                {
                    tb_inputSocial_2.Password = "";

                }
                if (tb_inputSocial_2.Password.Length >= 7)
                {
                    btn_find.Focus();
                }
            }
        }

        private void tb_inputAddress_1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }

        private void tb_inputPhone_1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }

        private void tb_inputPhone_2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }
        public void Show(Window owner)
        {
            this.Owner = owner;
            this.Show();
        }

        private void btn_find_Click(object sender, RoutedEventArgs e)
        {
            FindAddress find = new FindAddress();
            find.ShowDialog();

            if (find.Tag == null) { return; }
            DataRow dr = (DataRow)find.Tag;

            tb_inputAddress_1.Text = dr["zonecode"].ToString();
            tb_inputAddress_2.Text = dr["ADDR1"].ToString();

            tb_inputAddress_2.Focus();
        }

        private void tb_inputSocial_1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb_inputSocial_1.Text.Length >= 6)
            {
                tb_inputSocial_2.Focus();
            }
        }

        private void tb_inputPhone_1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb_inputPhone_1.Text.Length >= 4)
            {
                tb_inputPhone_2.Focus();
            }
        }

        private void tb_inputPhone_2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb_inputPhone_2.Text.Length >= 4)
            {
                tb_inputEmail_1.Focus();
            }
        }
    }
}
