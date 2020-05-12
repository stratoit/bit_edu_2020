﻿using System;
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
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void TempDelegate();
        public TempDelegate tempDelegate;
        string[] user;

        Timer _timer = null;
        string a;

        public MainWindow()
        {
            Login log = new Login();
            log.Show();
            this.Hide();
        }
        public MainWindow(string str)
        {
            this.Show();
            user = str.Split('#');
            InitializeComponent();
            tb_main.Text = "환영합니다 " + user[0] + "님. [부서 : " + user[1] + "]";
            InitTimer();
        }
        private void InitTimer()
        {
            if (_timer != null)
                return;
            TimerCallback tcb = new TimerCallback(ThreadFunc);
            _timer = new Timer(tcb, null, 0, 1000);
        }

        public void ThreadFunc(Object stateInfo)
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                tempDelegate += new TempDelegate(SetTextBox);
                Dispatcher.Invoke(DispatcherPriority.Normal, tempDelegate);
            }
        }

        private void SetTextBox()
        {
            text.Text = DateTime.Now.ToString();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            this.Close();
        }
    }
}