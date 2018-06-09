using EntityAbstract.Core.Helpers;
using Mvvmlighting.Attributes;
using Mvvmlighting.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace EntityAbstract.Uwp.Helpers
{
    public class NavigationHelper: NavigationHelperBase
    {
        public Frame Frame { get; set; }

        public override bool CanGoBack => Frame.CanGoBack;

        public override bool CanGoForward => Frame.CanGoForward;

        public NavigationHelper()
        {
            IocHelper.EnsureIocKey(this);
        }

        public override void Navigate(Type navType, object param = null)
        {
            Frame.Navigate(navType, param);
        }

        public override void GoBack()
        {
            Frame.GoBack();
        }

        public override void GoForward()
        {
            Frame.GoForward();
        }
    }
}
