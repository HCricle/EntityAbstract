using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EntityAbstract.Uwp.Selectors
{
    public class MsgDetailSelector : DataTemplateSelector
    {
        /// <summary>
        /// 网络图片
        /// </summary>
        public DataTemplate Img { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is MsgCmd mc)
            {
                if (mc.Id == 0) 
                {
                    return Img;
                }
            }
            return base.SelectTemplateCore(item);
        }
    }
}
