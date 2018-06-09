using EntityAbstract.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EntityAbstract.Uwp.Helpers
{
    public class ContentNavHelper:ContentNavHelperBase
    {
        public ContentNavHelper()
        {
        }

        public Border Border { get; set; }

        public override void SetContent(object value)
        {
            if (value is FrameworkElement fe)
            {
                Border.Child = fe;
            }
        }
    }
}
