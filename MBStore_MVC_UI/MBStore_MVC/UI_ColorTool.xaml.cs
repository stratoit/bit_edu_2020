using MaterialDesignThemes.Wpf;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MBStore_MVC
{
    /// <summary>
    /// UI_ColorTool.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UI_ColorTool : UserControl
    {
        public UI_ColorTool()
        {
            this.DataContext = new Model.ColorTool();
            InitializeComponent();
        }

        mbDB db = new mbDB();

        private async void btn_SaveColor_Click(object sender, RoutedEventArgs e)
        {
            //string path = @"theme.txt";
            string value;
            value = tb_PrimaryColor.Text + "/" + tb_PrimaryColor_Font.Text + "/" + tb_SecondaryColor.Text + "/" + tb_SecondaryColor_Font.Text;
            db.Update_Theme(value, MainWindow.emp.Employee_id);
            //File.WriteAllText(path, value, Encoding.Default);

            var MessageDialog = new MessageDialog
            {
                Message = { Text = "테마 색상이 저장되었습니다." }
            };
            await DialogHost.Show(MessageDialog, "RootDialog");
        }
    }
}
