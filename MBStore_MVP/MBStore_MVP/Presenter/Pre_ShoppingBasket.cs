using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MBStore_MVP.Model;

namespace MBStore_MVP.Presenter
{
    class Pre_ShoppingBasket : IShoppingBasket
    {
        #region Fileds
        private readonly IShoppingBasket_view view;
        mbDB mbdb = new mbDB();
        #endregion

        #region Constructor
        public Pre_ShoppingBasket(IShoppingBasket_view view)
        {
            this.view = view;
        }

        #endregion

        #region Create IPresenter method
        public BitmapImage GetPhoneImage(string uri, string name, string color, string option, UriKind urikind)
        {
            return new BitmapImage(new Uri(@uri + name + "_" + color + option, urikind));
        }
        public BitmapImage GetBrandImage(string uri, string brand, string option, UriKind urikind)
        {
            return new BitmapImage(new Uri(@uri + brand + option, urikind));
        }
        #endregion
    }
}
