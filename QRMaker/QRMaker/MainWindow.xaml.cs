using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
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
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace QRMaker
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
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
        private void btn_print_Click(object sender, RoutedEventArgs e)
        {
            if (tb_name.Text == "")
            {
                MessageBox.Show("제품명을 제대로 입력해주세요.");
                return;
            }
            else if (dp_manufacture.Text == "")
            {
                MessageBox.Show("출시일을 제대로 입력해주세요.");
                return;
            }
            else if (tb_cpu.Text == "")
            {
                MessageBox.Show("CPU를 제대로 입력해주세요.");
                return;
            }
            else if (tb_inch.Text == "")
            {
                MessageBox.Show("크기를 제대로 입력해주세요.");
                return;
            }
            else if (tb_mah.Text == "")
            {
                MessageBox.Show("배터리를 제대로 입력해주세요.");
                return;
            }
            else if (tb_ram.Text == "")
            {
                MessageBox.Show("RAM을 제대로 입력해주세요.");
                return;
            }
            else if (tb_brand.Text == "")
            {
                MessageBox.Show("제조사를 제대로 입력해주세요.");
                return;
            }
            else if (tb_camera.Text == "")
            {
                MessageBox.Show("카메라를 제대로 입력해주세요.");
                return;
            }
            else if (tb_weight.Text == "")
            {
                MessageBox.Show("무게를 제대로 입력해주세요.");
                return;
            }
            else if (tb_price.Text == "")
            {
                MessageBox.Show("가격을 제대로 입력해주세요.");
                return;
            }
            else if (tb_display.Text == "")
            {
                MessageBox.Show("Display를 제대로 입력해주세요.");
                return;
            }
            else if (tb_memory.Text == "")
            {
                MessageBox.Show("Display를 제대로 입력해주세요.");
                return;
            }
            else
            {
                ZXing.BarcodeWriter barcodeWriter = new ZXing.BarcodeWriter();
                QrCodeEncodingOptions qr = new QrCodeEncodingOptions()
                {
                    CharacterSet = "UTF-8",
                    ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H,
                    Height = 400,
                    Width = 400
                };
                barcodeWriter.Options = qr;
                barcodeWriter.Format = ZXing.BarcodeFormat.QR_CODE;

                string strQRCode = tb_name.Text + "#" + dp_manufacture.Text + "#" + tb_cpu.Text + "#" +
                    tb_inch.Text + "#" + tb_mah.Text + "#" + tb_ram.Text + "#" + tb_brand.Text + "#" +
                    tb_camera.Text + "#" + tb_weight.Text + "#" + tb_price.Text + "#" + tb_display.Text
                    + "#" + tb_memory.Text;
                string deskPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Image files (*.jpeg)|*.jpeg";
                saveFileDialog.FileName = tb_name.Text + ".jpeg";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (saveFileDialog.ShowDialog() == true)
                    barcodeWriter.Write(strQRCode).Save(saveFileDialog.FileName, ImageFormat.Jpeg);

                tb_name.Text = "";
                dp_manufacture.Text = "";
                tb_cpu.Text = "";
                tb_inch.Text = "";
                tb_mah.Text = "";
                tb_ram.Text = "";
                tb_brand.Text = "";
                tb_camera.Text = "";
                tb_weight.Text = "";
                tb_price.Text = "";
                tb_display.Text = "";
                tb_memory.Text = "";
                MessageBox.Show("QR코드 생성완료");
            }

        }

        private void tb_mah_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }

        private void tb_inch_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }

        private void tb_ram_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }

        private void tb_camera_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }

        private void tb_weight_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }

        private void tb_price_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }

        private void tb_memory_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumeric(e.Text);
        }

        private void btn_read_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ZXing.BarcodeReader barcodeReader = new ZXing.BarcodeReader();

                //string deskPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.DefaultExt = ".jpeg";
                dlg.Filter = "Image files (*.jpeg)|*.jpeg";

                if (dlg.ShowDialog() == true)
                {
                    var barcodeBitmap = (Bitmap)System.Drawing.Image.FromFile(dlg.FileName);
                    string[] result = barcodeReader.Decode(barcodeBitmap).Text.Split('#');
                    tb_name.Text = result[0];
                    dp_manufacture.Text = result[1];
                    tb_cpu.Text = result[2];
                    tb_inch.Text = result[3];
                    tb_mah.Text = result[4];
                    tb_ram.Text = result[5];
                    tb_brand.Text = result[6];
                    tb_camera.Text = result[7];
                    tb_weight.Text = result[8];
                    tb_price.Text = result[9];
                    tb_display.Text = result[10];
                    tb_memory.Text = result[11];
                }
            }
            catch
            {
                MessageBox.Show("형식이 일치하지 않은 QR코드 입니다.");
            }
        }

    }
}
