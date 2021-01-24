using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;





// Namespace
namespace Lockscreen_Swap.Pages
{





    // 10.0 Screen Changer Werbung
    public partial class _10_0_Screen_Changer : PhoneApplicationPage
    {





        // Klasse erzeugen
        //---------------------------------------------------------------------------------------------------------
        public _10_0_Screen_Changer()
        {
            InitializeComponent();
        }
        //---------------------------------------------------------------------------------------------------------





        //Button Store
        //---------------------------------------------------------------------------------------------------------
        private void BtnStore(object sender, RoutedEventArgs e)
        {
            var wb = new WebBrowserTask();
            wb.URL = "http://windowsphone.com/s?appid=8d7478b1-8bde-429d-b55c-d03a21029b51";
            wb.Show();
        }
        //---------------------------------------------------------------------------------------------------------





    }
}