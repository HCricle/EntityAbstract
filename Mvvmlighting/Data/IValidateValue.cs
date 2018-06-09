using System;
using System.Collections.Generic;
using System.Text;

namespace Mvvmlighting.Data
{
    /// <summary>
    /// 更新时，验证属性传入的属性是否合法
    /// </summary>
    public interface IValidateValue
    {
        bool IsNewValueVali(object value);
    }
}
