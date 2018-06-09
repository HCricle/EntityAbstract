using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Core.Interactive.Models
{
    /// <summary>
    /// 视图项目
    /// </summary>
    public class ViewItem
    {
        public ViewItem(string title, char icon, Type navigateType=null)
        {
            Title = title;
            Icon = icon;
            NavigateType = navigateType;
        }

        public string Title { get; }
        public char Icon { get;  }
        public Type NavigateType { get;  }
        public string Descript { get; set; }
    }
}
