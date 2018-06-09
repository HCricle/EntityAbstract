using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace EntityAbstract.Uwp.Converters
{
    public class StirngUriConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string v)
            {
                Uri.TryCreate(v, UriKind.Absolute, out var uri);
                return uri;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
    public class ListStringUriConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MsgCmd md)
            {
                if (md.Params.Count>0)
                {

                    Uri.TryCreate(md.Params.ElementAt(0), UriKind.Absolute, out var uri);
                    return uri;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
