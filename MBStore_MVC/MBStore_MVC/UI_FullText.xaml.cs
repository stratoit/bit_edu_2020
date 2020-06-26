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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MBStore_MVC
{
    /// <summary>
    /// UI_FullText.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UI_FullText : UserControl
    {
        public UI_FullText()
        {
            InitializeComponent();

            tb_text.Text = "나 『김민철』이 우수사원으로 선정된 것은 너무나도 당연하고 아무런 감흥조차 느껴지지 않는다. 아아.. 이것이 『공허』인가. 큭큭.. \r\n\r\n『본인』이 우수사원이 될 수 있었던 것은 단지 많이 팔았기 때문이다. \r\n\r\n이거이거, 범인(凡人)들의 가슴이 웅장해지겠는걸? 비소로 『나』는 여기서도 세계관 최강자가 되어버렸군.. 시시해서... 죽고싶어졌다.\r\n\r\n 내 『왕좌』를 탐하는 이들이여, 더욱더 발버둥쳐서 『나, 킹민철』의 먹잇감이 되어라.";
        }
    }
}
