﻿#pragma checksum "..\..\..\View\StockReturn.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "57773EE73CBE589D0BA34EFAA18693880A3C52761C03191523F315E2D333A246"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

using MBStore_MVP.View;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MBStore_MVP.View {
    
    
    /// <summary>
    /// StockReturn
    /// </summary>
    public partial class StockReturn : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 23 "..\..\..\View\StockReturn.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox lo_pse_basket_product_info;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\View\StockReturn.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox lo_pse_basket_quantity;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\View\StockReturn.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lo_pse_basket_stock;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\View\StockReturn.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_return;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MBStore_MVP;component/view/stockreturn.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\View\StockReturn.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.lo_pse_basket_product_info = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.lo_pse_basket_quantity = ((System.Windows.Controls.TextBox)(target));
            
            #line 25 "..\..\..\View\StockReturn.xaml"
            this.lo_pse_basket_quantity.KeyDown += new System.Windows.Input.KeyEventHandler(this.KeyDown_lo_quantity);
            
            #line default
            #line hidden
            return;
            case 3:
            this.lo_pse_basket_stock = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.btn_return = ((System.Windows.Controls.Button)(target));
            
            #line 28 "..\..\..\View\StockReturn.xaml"
            this.btn_return.Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

