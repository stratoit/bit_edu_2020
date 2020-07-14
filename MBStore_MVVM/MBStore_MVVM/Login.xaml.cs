using MBStore_MVVM.Model;
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

namespace MBStore_MVVM
{
    /// <summary>
    /// Login.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            DataContext = this;
            this.DataContext = new ViewModel.MainViewModel(this);
        }
    }
}
