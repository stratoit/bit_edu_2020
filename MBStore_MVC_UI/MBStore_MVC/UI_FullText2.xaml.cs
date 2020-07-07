using MaterialDesignThemes.Wpf;
using MBStore_MVC.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MBStore_MVC
{
    /// <summary>
    /// UI_FullText2.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UI_FullText2 : UserControl
    {
        public UI_Notice ui_noti = new UI_Notice();
        public static UI_FullText2 ui_fulltext2;

        mbDB db = new mbDB();
        bool edit = false;
        public void print()
        {
            tb_text.Text = ui_noti.noticeList[0].Text;
            tb_title.Text = ui_noti.noticeList[0].Title;
            tb_date.Text = "작성일 : " + ui_noti.noticeList[0].Date;
            tb_name.Text = "작성자 : " + ui_noti.noticeList[0].Name;
            img_emp_image.Source = new BitmapImage(new Uri(ui_noti.noticeList[0].Url, UriKind.Absolute));
        }
        public UI_FullText2()
        {
            InitializeComponent();
            ui_fulltext2 = this;
            print();
        }


        private void btn_editMode_Click(object sender, RoutedEventArgs e)
        {
            if (!edit)
            {
                btn_editMode.Content = "편집 취소";
                tb_title.IsReadOnly = false;
                tb_text.IsReadOnly = false;
                edit = true;
            }
            else
            {
                btn_editMode.Content = "편집 모드";
                tb_text.Text = ui_noti.noticeList[0].Text;
                tb_title.Text = ui_noti.noticeList[0].Title;
                tb_title.IsReadOnly = true;
                tb_text.IsReadOnly = true;
                edit = false;
            }
        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            if (edit)
            {
                if (db.Update_Notice(MainWindow.emp.Employee_id, ui_noti.noticeList[0].Notice_id, tb_title.Text, tb_text.Text, ui_noti.noticeList[0].Url))
                {
                    ui_noti.noticeList[0] = db.SelectNotice_noticeid(ui_noti.noticeList[0].Notice_id);
                    ui_noti.loaded_notice();

                    var MessageDialog = new MessageDialog
                    {
                        Message = { Text = "변경한 내용이 적용됐습니다." }
                    };
                    DialogHost.Show(MessageDialog, "RootDialog");
                    btn_editMode_Click(sender, e);
                }


                else
                {
                    var MessageDialog = new MessageDialog
                    {
                        Message = { Text = "자신이 작성한 글만 수정이 가능합니다." }
                    };
                    DialogHost.Show(MessageDialog, "RootDialog");
                }
            }
            else
            {
                var MessageDialog = new MessageDialog
                {
                    Message = { Text = "편집 모드를 실행 중일 때 가능합니다." }
                };
                DialogHost.Show(MessageDialog, "RootDialog");
            }
        }


    }
}

