using EntityAbstract.Core.Helpers;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityAbstract.Core.Args
{
    /// <summary>
    /// 处理解析异常的参数
    /// </summary>
    public class HandleParseErrArgs<TWithUi>
        where TWithUi:class
    {
        public HandleParseErrArgs(MsgCmdHelperBase<TWithUi> sender, MsgDetail msgDetail, MsgCmd msgCmd, Exception exception=null)
        {
            Sender = sender;
            MsgDetail = msgDetail;
            MsgCmd = msgCmd;
            Exception = exception;
        }

        /// <summary>
        /// 发送者
        /// </summary>
        public MsgCmdHelperBase<TWithUi> Sender { get;  }
        /// <summary>
        /// 当前解析的消息
        /// </summary>
        public MsgDetail MsgDetail { get;  }
        /// <summary>
        /// 当前处理的命令
        /// </summary>
        public MsgCmd MsgCmd { get;  }
        /// <summary>
        /// 异常信息，如果有
        /// </summary>
        public Exception Exception { get;  }
        public bool HasException => Exception != null;
    }
}
