using MBStore_MVC.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        mbDB db = new mbDB();
        UI_Notice ui_notice;
        string uri;
        string http_uri;
        string ftp_uri;

        public Notice(UI_Notice ui_notice)
        {
            InitializeComponent();

            this.ui_notice = ui_notice;
            cb_part.Items.Add("공지사항");
            cb_part.Items.Add("우수사원");

            uri = "20.41.81.89";
            http_uri = "http://" + uri;
            ftp_uri = "ftp://" + uri + ":21";

        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

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

        private void btn_su_emp_img(object sender, RoutedEventArgs e)
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
                tb_img_source.Text = filename;
                img_su_emp.ImageSource = new BitmapImage(new Uri(@dlg.FileName, UriKind.Absolute));
            }
        }


        private void btn_post_Click(object sender, RoutedEventArgs e)
        {
            if (tb_title.Text == "")
                MessageBox.Show("제목을 입력해주세요.");
            else if (tb_text.Text == "")
                MessageBox.Show("본문 내용을 입력해주세요.");
            else
            {
                int notice_id = db.Select_LastNoticeid() + 1;
                string input_uri = "";
                if (tb_img_source.Text != "")
                {
                    input_uri = "/notice/" + cb_part.SelectedItem.ToString() + "_" + notice_id + "_" + tb_title.Text + ".JPG";
                }
                if (db.Insert_Notice(MainWindow.emp.Employee_id, tb_title.Text, tb_text.Text, cb_part.SelectedItem.ToString(), http_uri + input_uri))
                {
                    MessageBox.Show("공지 등록 완료.");
                    if (tb_img_source.Text != "")
                    {
                        FtpUploadFile(tb_img_source.Text, ftp_uri + input_uri);
                    }
                    ui_notice.noticeList = db.SelectRecentNotice();
                    ui_notice.loaded_notice();
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