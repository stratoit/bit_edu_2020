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
using MBStore_MVP.Model;
using MBStore_MVP.Presenter;

namespace MBStore_MVP.View
{
    /// <summary>
    /// Emp.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Emp_Sign_up : Window, IEmp_Sign_up_view
    {
        IEmp_Sing_up presenter;
        public Emp_Sign_up()
        {
            InitializeComponent();
            presenter = new Pre_Emp_Sign_up(this);
        }

        public void Show(Window owner)
        {
            this.Owner = owner;
            this.Show();
        }

        // 지원 -> 직원관리 -> 회원등록 조회버튼
        private List<Sign_up> emp_signup;
        public void Update_List()
        {
            mainwindow1.lv_employee_sign_list.ItemsSource = null;
            emp_signup = presenter.GetList_Sign_Up_Emp();
            mainwindow1.lv_employee_sign_list.ItemsSource = emp_signup;
        }

        // 부모값 가져오기
        Sign_up sign_up_data = new Sign_up();
        MainWindow mainwindow1;
        public void Set_Update_Cost(Sign_up sign, MainWindow mainwindow)
        {
            sign_up_data = sign;
            mainwindow1 = mainwindow;
        }

        //버튼 ok 누를 시 data update
        private Nullable<DateTime> dt = null;
        private void Btn_Ok(object sender, RoutedEventArgs e)
        {
            Sign_up real_Sign_up = new Sign_up();

            try
            {
                //로그인 아이디로 sign_up에서 가져오고
                real_Sign_up = presenter.Sign_Up_Data_Catch(sign_up_data);
                //cb, tb에서 값 가져와서 다 넣어주고 업데이트
                presenter.Upload_Emp_Sign_up(int.Parse(tb_emp_id.Text), cb_select_team.Text, DateTime.Now, dt, real_Sign_up);
                presenter.Delete_Sign_up(sign_up_data.Login_id);
                MessageBox.Show("등록되었습니다");
                //sign_up에 delete
                //select item delete
                Update_List();
                this.Close();
                mainwindow1.WindowState = WindowState.Normal;
                mainwindow1.Activate();
                foreach (ListViewItem eachItem in mainwindow1.lv_employee_sign_list.SelectedItems)
                {
                    mainwindow1.lv_employee_sign_list.Items.Remove(eachItem);
                }
            }
            catch
            {
                MessageBox.Show("직무를 지정해주세요");
            }

        }

        //close하면 자식폼 끄기
        private void Btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        //combobox가 떨어지면 값 넣어주기
        string team_reader = string.Empty;
        private void cb_select_team_DropDownClosed(object sender, EventArgs e)
        {
            team_reader = "";
            Employee employee = new Employee();

            if (cb_select_team.Text == "지원팀") team_reader = "지원팀장";
            else if (cb_select_team.Text == "판매팀") team_reader = "판매팀장";
            else if (cb_select_team.Text == "물류팀") team_reader = "물류팀장";
            else if (cb_select_team.Text == "지원팀장") team_reader = "지원팀";
            else if (cb_select_team.Text == "판매팀장") team_reader = "판매팀";
            else if (cb_select_team.Text == "물류팀장") team_reader = "물류팀";

            employee = presenter.locate_Emp_Id(cb_select_team.Text, team_reader);
            tb_emp_id.Text = (employee.Employee_id + 1).ToString();
        }


    }
}
