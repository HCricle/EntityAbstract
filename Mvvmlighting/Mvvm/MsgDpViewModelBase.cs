using Mvvmlighting.Args;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mvvmlighting.Mvvm
{
    /// <summary>
    /// 消息依赖视图模型基类
    /// </summary>
    public abstract class MsgDpViewModelBase : DependencyViewModelBase
    {
        /// <summary>
        /// 消息处理
        /// </summary>
        protected Dictionary<object, AcceptedMessageHandle> MsgHandle { get; } = new Dictionary<object, AcceptedMessageHandle>();
        /// <summary>
        /// 异步消息处理
        /// </summary>
        protected Dictionary<object, AcceptedMessageAsyncHandle> AsyncMsgHandle { get; } = new Dictionary<object, AcceptedMessageAsyncHandle>();
        public MsgDpViewModelBase()
        {
            Init();
        }

        public MsgDpViewModelBase(Message message) : base(message)
        {
            Init();
        }
        private void Init()
        {
            Message.AcceptedMessage += Message_AcceptedMessage;
            Message.AcceptedMessageAsync += Message_AcceptedMessageAsync;
        }
        private object Message_AcceptedMessage(IMessageUsable sender, MessageAcceptArgs args)
        {
            if (!MsgHandle.TryGetValue(args.Key, out var handle))
            {
                throw new ArgumentException($"不存在key={args.Key} 的处理方法");
            }
            return handle.Invoke(sender, args);
        }
        private async Task<object> Message_AcceptedMessageAsync(IMessageUsable sender, MessageAcceptArgs args)
        {
            if (!AsyncMsgHandle.TryGetValue(args.Key, out var handle))
            {
                throw new ArgumentException($"不存在key={args.Key} 的异步处理方法");
            }
            return await handle.Invoke(sender, args);
        }
    }
}
