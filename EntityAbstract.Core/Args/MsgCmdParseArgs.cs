using EntityAbstract.Core.Helpers;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityAbstract.Core.Args
{
    public class MsgCmdParseArgs<TWithUi>
        where TWithUi:class
    {
        public MsgCmdParseArgs(MsgCmdHelperBase<TWithUi> sender, MsgCmd parseObject, TWithUi uiElement)
        {
            Sender = sender;
            ParseObject = parseObject;
            UiElement = uiElement;
        }

        public MsgCmdHelperBase<TWithUi> Sender { get; }
        public MsgCmd ParseObject { get;  }
        public TWithUi UiElement { get;  }
    }
}
