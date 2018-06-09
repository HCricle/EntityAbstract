using System;
using System.Collections.Generic;
using System.Text;

namespace Mvvmlighting.Objects
{
    /// <summary>
    /// 验证
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate bool ValidateValueCallback(object value);
    /// <summary>
    /// 有效更改回调
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    public delegate void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e);
    /// <summary>
    /// 转换数据
    /// </summary>
    /// <param name="d"></param>
    /// <param name="baseValue"></param>
    /// <returns></returns>
    public delegate object CoerceValueCallback(DependencyObject d, object baseValue);


}
