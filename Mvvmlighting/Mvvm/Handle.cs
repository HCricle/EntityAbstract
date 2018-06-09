using Mvvmlighting.Args;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mvvmlighting.Mvvm
{
    public delegate object AcceptedMessageHandle(IMessageUsable sender,MessageAcceptArgs args);
    public delegate Task<object> AcceptedMessageAsyncHandle(IMessageUsable sender, MessageAcceptArgs args);
}
