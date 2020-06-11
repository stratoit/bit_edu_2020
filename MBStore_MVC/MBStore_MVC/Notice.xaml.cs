using MBStore_MVC.Model;
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

namespace MBStore_MVC
{
    /// <summary>
    /// Notice.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class Notice : Window
    {
        Employee emp;
        mbDB db = new mbDB();
        public Notice()
        {
            InitializeComponent();
            cb_part.Items.Add("전체공지");
            cb_part.Items.Add("물류팀");
            cb_part.Items.Add("판매팀");
            cb_part.Items.Add("지원팀");
        }
        public void Show(Employee employee)
        {
            emp = employee;
            if(emp.Rank == "물류팀장")
                cb_part.SelectedIndex = 1;
            else if (emp.Rank == "판매팀장")
                cb_part.SelectedIndex = 2;
            else if (emp.Rank == "지원팀장")
                cb_part.SelectedIndex = 3;
            else
            {
                cb_part.SelectedIndex = 0;
                cb_part.IsEnabled = true;
            }

            this.Show();
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btn_post_Click(object sender, RoutedEventArgs e)
        {
            if (tb_title.Text == "")
                MessageBox.Show("제목을 입력해주세요.");
            else if (tb_text.Text == "")
                MessageBox.Show("본문 내용을 입력해주세요.");
            else
            {
                if (db.Insert_Notice(emp.Employee_id, DateTime.Now, tb_title.Text, tb_text.Text, cb_part.SelectedItem.ToString()))
                {
                    MessageBox.Show("공지 등록 완료.");
                    Close();
                }
                else
                {
                    MessageBox.Show("공지 등록 실패!");
                }
            }
        }
    }
}
