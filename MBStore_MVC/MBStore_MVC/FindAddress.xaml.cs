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
//Web
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Data;

namespace MBStore_MVC
{
    /// <summary>
    /// FindAddress.xaml에 대한 상호 작용 논리
    /// </summary>
    
    //Web    
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class FindAddress : Window
    {
      
        public FindAddress()
        {
            InitializeComponent();
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {               
                wb_find.Navigate("file:///C:/Users/cs950/Downloads/index.html");
                wb_find.ObjectForScripting = this;

                this.Tag = null;
            }
            catch (Exception ex)
            {
                Base_Message(ex);
            }
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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

        #region RUN
        //WEB -> C#
        public void lfn_WEB_Cshap_Script2(
                                                         object obj01, object obj02, object obj03, object obj04, object obj05, object obj06, object obj07, object obj08, object obj09
                                         , object obj10, object obj11, object obj12, object obj13, object obj14, object obj15, object obj16, object obj17, object obj18, object obj19
                                         , object obj20, object obj21, object obj22, object obj23, object obj24, object obj25, object obj26, object obj27, object obj28, object obj29
                                         , object obj30, object obj31, object obj32, object obj33
                                         )
        {
            try
            {
                #region "변수선언"
                string val01 = (string)obj01;
                string val02 = (string)obj02;
                string val03 = (string)obj03;
                string val04 = (string)obj04;
                string val05 = (string)obj05;
                string val06 = (string)obj06;
                string val07 = (string)obj07;
                string val08 = (string)obj08;
                string val09 = (string)obj09;

                string val10 = (string)obj10;
                string val11 = (string)obj11;
                string val12 = (string)obj12;
                string val13 = (string)obj13;
                string val14 = (string)obj14;
                string val15 = (string)obj15;
                string val16 = (string)obj16;
                string val17 = (string)obj17;
                string val18 = (string)obj18;
                string val19 = (string)obj19;

                string val20 = (string)obj20;
                string val21 = (string)obj21;
                string val22 = (string)obj22;
                string val23 = (string)obj23;
                string val24 = (string)obj24;
                string val25 = (string)obj25;
                string val26 = (string)obj26;
                string val27 = (string)obj27;
                string val28 = (string)obj28;
                string val29 = (string)obj29;

                string val30 = (string)obj30;
                string val31 = (string)obj31;
                string val32 = (string)obj32;
                string val33 = (string)obj33;

                string strADDR1 = "";
                string strADDR2 = "";
                string strEX = "";
                #endregion
                string strTMP1 = "";


                DataRow dr = null;
                lfn_dr(ref dr);

                #region "dr<-변수기입1"
                dr["zonecode"] = val01;
                dr["address"] = val02;
                dr["addressEnglish"] = val03;
                dr["addressType"] = val04;
                dr["userSelectedType"] = val05;

                dr["noSelected"] = val06;
                dr["userLanguageType"] = val07;
                dr["roadAddress"] = val08;
                dr["roadAddressEnglish"] = val09;
                dr["jibunAddress"] = val10;

                dr["jibunAddressEnglish"] = val11;
                dr["autoRoadAddress"] = val12;
                dr["autoRoadAddressEnglish"] = val13;
                dr["autoJbunAddress"] = val14;
                dr["autoJibunAddressEnglish"] = val15;

                dr["buildngCode"] = val16;
                dr["buildingName"] = val17;
                dr["apartment"] = val18;
                dr["sido"] = val19;
                dr["sigungu"] = val20;

                dr["sigunguCode"] = val21;
                dr["roadnameCode"] = val22;
                dr["bcode"] = val23;
                dr["roadname"] = val24;
                dr["bname"] = val25;

                dr["bname1"] = val26;
                dr["bname2"] = val27;
                dr["hname"] = val28;
                dr["query"] = val29;
                dr["postcode"] = val30;

                dr["postcode1"] = val31;
                dr["postcode2"] = val32;
                dr["postcodeSeq"] = val33;
                #endregion





                #region "ADDR1, ADDR2 X, EX 산출"
                try
                {
                    // 검색결과 항목을 클릭했을때 실행할 코드를 작성하는 부분.
                    // 각 주소의 노출 규칙에 따라 주소를 조합한다.
                    // 내려오는 변수가 값이 없는 경우엔 공백('')값을 가지므로, 이를 참고하여 분기 한다.
                    strADDR1 = ""; // 주소 변수
                    strEX = ""; // 참고항목 변수

                    //주소변수1
                    if (dr["userSelectedType"].Equals("R"))
                    {
                        strADDR1 = dr["roadAddress"].ToString();   // 사용자가 도로명 주소를 선택했을 경우
                    }
                    else
                    {
                        strADDR1 = dr["jibunAddress"].ToString();  // 사용자가 지번 주소를 선택했을 경우(J)
                    }


                    // 참고항목변수
                    // 사용자가 선택한 주소가 도로명 타입일때 참고항목을 조합한다.
                    if (dr["userSelectedType"].Equals("R"))
                    {
                        strTMP1 = dr["bname"].ToString();
                        if (!strTMP1.Equals("")) { strTMP1 = strTMP1.Substring(strTMP1.Length - 1, 1).ToString(); }
                        if (!dr["bname"].Equals("") && (strTMP1.Equals("동") || strTMP1.Equals("로") || strTMP1.Equals("가")))
                        {
                            strEX += dr["bname"].ToString();
                        }

                        if (!dr["buildingName"].Equals("") && dr["apartment"].Equals("Y"))
                        { // 건물명이 있고, 공동주택일 경우 추가한다.
                            if (!strEX.Equals("")) { strEX += ", "; }
                            strEX += dr["buildingName"].ToString();
                        }

                        if (!strEX.Equals("")) { strEX = "(" + strEX + ")"; }

                    }
                    else
                    {
                        strEX = "";
                    }


                }
                catch { }
                #endregion



                #region "dr<=정제된데이터 (ADDR1, ADDR2, EX)"
                dr["ADDR1"] = strADDR1;
                dr["EX"] = strEX;
                #endregion
                this.Tag = dr;
                this.Close();
            }
            catch (Exception ex)
            {
                Base_Message(ex);
            }
        }



        private void lfn_dr(ref DataRow dr)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("zonecode");
                dt.Columns.Add("address");
                dt.Columns.Add("addressEnglish");
                dt.Columns.Add("addressType");
                dt.Columns.Add("userSelectedType");

                dt.Columns.Add("noSelected");
                dt.Columns.Add("userLanguageType");
                dt.Columns.Add("roadAddress");
                dt.Columns.Add("roadAddressEnglish");
                dt.Columns.Add("jibunAddress");

                dt.Columns.Add("jibunAddressEnglish");
                dt.Columns.Add("autoRoadAddress");
                dt.Columns.Add("autoRoadAddressEnglish");
                dt.Columns.Add("autoJbunAddress");
                dt.Columns.Add("autoJibunAddressEnglish");

                dt.Columns.Add("buildngCode");
                dt.Columns.Add("buildingName");
                dt.Columns.Add("apartment");
                dt.Columns.Add("sido");
                dt.Columns.Add("sigungu");

                dt.Columns.Add("sigunguCode");
                dt.Columns.Add("roadnameCode");
                dt.Columns.Add("bcode");
                dt.Columns.Add("roadname");
                dt.Columns.Add("bname");

                dt.Columns.Add("bname1");
                dt.Columns.Add("bname2");
                dt.Columns.Add("hname");
                dt.Columns.Add("query");
                dt.Columns.Add("postcode");

                dt.Columns.Add("postcode1");
                dt.Columns.Add("postcode2");
                dt.Columns.Add("postcodeSeq");

                dt.Columns.Add("ADDR1");
                dt.Columns.Add("EX");

                dr = dt.NewRow();
            }
            catch (Exception ex)
            {
                Base_Message(ex);
            }
        }



        //C# -> WEB
        private void lfn_Cshap_WEB_initLayerPosition(int w, int h)
        {
            wb_find.InvokeScript("initLayerPosition", new object[] { w, h }); //웹 자바스크립트에전달
        }



        private void lfn_Cshap_WEB_Script2()
        {
            string str1 = "A";
            string str2 = "B";

            wb_find.InvokeScript("Cshap_WEB_Script2", new object[] { str1, str2 }); //웹 자바스크립트에전달
            //webBrowser1.Document.InvokeScript("CallScript", new object[] { svalue1, svalue2 });
        }

        #endregion //FUN

    }
}
