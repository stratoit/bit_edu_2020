using MaterialDesignThemes.Wpf;
using MBStore_MVC.Model;
using System;
using System.Collections.Generic;
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
    /// UI_Notice.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class UI_Notice : UserControl
    {
        public List<Notices> noticeList = new List<Notices>();
        mbDB db = new mbDB();

        public void loaded_notice()
        {
            tb_emp_title.Text = noticeList[1].Title;
            tb_emp_Text.Text = noticeList[1].Text;
            img_emp_image.Source = new BitmapImage(new Uri(noticeList[1].Url, UriKind.Absolute));

            tb_notice_title.Text = noticeList[0].Title;
            tb_notice_text.Text = noticeList[0].Text;
            img_notice_image.Source = new BitmapImage(new Uri(noticeList[0].Url, UriKind.Absolute));

        }
  
        public UI_Notice()
        {
            InitializeComponent();
            noticeList = db.SelectRecentNotice();
            #region 공지사항 불러오기
            loaded_notice();
            #endregion
        }

        private void btn_Prev_Click(object sender, RoutedEventArgs e)
        {
            Notices notices = db.SelectPreNotice(noticeList[0].Notice_id);
            
            if(notices.Name != null)
            {
                noticeList[0] = notices;
                loaded_notice();
            }
            else
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "가장 최신 게시물 입니다." }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            Notices notices = db.SelectNextNotice(noticeList[0].Notice_id);
            if (notices.Name != null)
            {
                noticeList[0] = notices;
                loaded_notice();
            }
            else
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "마지막 게시물 입니다." }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }


        private void btn_notice_write_Click(object sender, RoutedEventArgs e)
        {
            Notice noti = new Notice(this);
            noti.Show();
        }

        private void btn_notice_all_Click(object sender, RoutedEventArgs e)
        {
            UI_FullText2.ui_fulltext2.ui_noti = this;
            UI_FullText2.ui_fulltext2.print();
        }

        private void btn_notice_all_Click2(object sender, RoutedEventArgs e)
        {
            UI_FullText.ui_fulltext.ui_noti = this;
            UI_FullText.ui_fulltext.print();
        }
    }
}
