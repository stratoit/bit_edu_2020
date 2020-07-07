using System;
using System.Windows.Input;
//Web
using System.Security.Permissions;
using System.Runtime.InteropServices;

//using System.Windows.Forms;
using System.Windows;
using MBStore_MVC.Model;

namespace MBStore_MVC
{
    /// <summary>
    /// FindAddress.xaml에 대한 상호 작용 논리
    /// </summary>

    //Web    
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]
    public partial class FindAddress : Window
    {
        public FindAddress()
        {
            InitializeComponent();
        }

   

        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // wb.ScriptErrorsSuppressed = true;
                wb.ObjectForScripting = new Browser(this);

               this.wb.Navigate(new Uri("http://20.41.81.89/adress/index.html"));

                this.Tag = null;
            }
            catch (Exception ex)
            {
                Base_Message(ex);
            }
        }

        #region Message
        public void Base_Message(Exception ex)
        {
            MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
        }

        public void Base_Message(string messgage)
        {
            MessageBox.Show(messgage, "Information", MessageBoxButton.OK,
                        MessageBoxImage.Information);
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


    }
}
