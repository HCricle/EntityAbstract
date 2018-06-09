using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityAbstract.Core.Helpers
{
    public abstract class ContentNavHelperBase
    {
        public static readonly int ContentNavHelperKey = 0x10202;
        public abstract void SetContent(object value);
    }
}
