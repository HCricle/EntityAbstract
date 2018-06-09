using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityAbstract.Core.Models
{
    public class ViewItem
    {
        public ViewItem()
        {
        }

        public ViewItem(string title, char icon, string descript,int id, ViewItemTypes type = ViewItemTypes.Item)
        {
            Title = title;
            Icon = icon;
            Descript = descript;
            Id = id;
            Type = type;
        }

        public ViewItem(string title, char icon, string descript,Type pageType, ViewItemTypes type= ViewItemTypes.Item)
        {
            Title = title;
            Icon = icon;
            Descript = descript;
            PageType = pageType;
            Type = type;
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public char Icon { get; set; }
        public string Descript { get; set; }
        public Type PageType { get; set; }
        public Func<object> ParamGetter { get; set; }
        public ViewItemTypes Type { get; set; }
    }
    public enum ViewItemTypes
    {
        Head,
        Item
    }
}
