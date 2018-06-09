using EntityAbstract.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EntityAbstract.Uwp.Selectors
{
    public class ViewItemSelector:DataTemplateSelector
    {
        public DataTemplate HeadTemple { get; set; }
        public DataTemplate ItemTemple { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            return GetTemplate(item);   
        }
        private DataTemplate GetTemplate(object item)
        {
            var temp = ItemTemple ;
            if (item is ViewItem vi)
            {
                if (vi.Type == ViewItemTypes.Head)
                {
                    temp =  HeadTemple ;
                }
            }
            return temp;
        }
    }
}
