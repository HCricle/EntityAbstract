using Mvvmlighting.Attributes;
using Mvvmlighting.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityAbstract.Core.Helpers
{
    public abstract class NavigationHelperBase
    {
        [NeedKeyInIoc]
        public static readonly int NavigationHelperKey = 0x10001;
        public abstract void Navigate(Type navType, object param = null);
        public abstract void GoBack();
        public abstract void GoForward();
        public abstract bool CanGoBack { get; }
        public abstract bool CanGoForward { get; }

    }
}
