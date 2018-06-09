using System;
using System.Collections.Generic;
using System.Text;

namespace Mvvmlighting.Args
{
    public class MessageAcceptArgs
    {
        public MessageAcceptArgs(object key, object param)
        {
            Key = key;
            Param = param;
        }

        public object Key { get; }
        public object Param { get;  }
    }
}
