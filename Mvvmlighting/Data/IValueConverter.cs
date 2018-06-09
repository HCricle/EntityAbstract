using System;
using System.Collections.Generic;
using System.Text;

namespace Mvvmlighting.Data
{
    public interface IValueConverter
    {
        
        object Convert(object value, Type targetType, object parameter);
        
        object ConvertBack(object value, Type targetType, object parameter);

    }
}
