using Mvvmlighting.Args;
using Mvvmlighting.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace Mvvmlighting.Mvvm
{
    /// <summary>
    /// 允许通过此类向其它IMessageUsable发送消息
    /// </summary>
    public class Message
    {
        /// <summary>
        /// 所有注册message的都会加入此对象
        /// </summary>
        internal static List<IMessageUsable> MessageViewModels = new List<IMessageUsable>();
        
        public Message(IMessageUsable viewModel)
        {
            depencyViewModel = viewModel;
            MessageViewModels.Add(viewModel);
        }
        private IMessageUsable depencyViewModel;
        /// <summary>
        /// 接收到消息,发送者，消息参数
        /// </summary>
        public event AcceptedMessageHandle AcceptedMessage;
        /// <summary>
        /// 异步接到信息
        /// </summary>
        public event AcceptedMessageAsyncHandle AcceptedMessageAsync;
        /// <summary>
        /// 收到信息
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">参数</param>
        internal object AcceptMsg(IMessageUsable sender, MessageAcceptArgs args)
        {
            return AcceptedMessage?.Invoke(sender, args);
        }
        /// <summary>
        /// 异步接收信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal async Task<object> AcceptMsgAsync(IMessageUsable sender,MessageAcceptArgs args)
        {
            if (AcceptedMessageAsync!=null)
            {
                return await AcceptedMessageAsync.Invoke(sender, args);
            }
            return null;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <returns>是否发送成功，如果viewmodel不存在message就会发送失败</returns>
        public object Send<TTarget>(object key,object param)
            where TTarget: IMessageUsable
        {
            return Send<TTarget>(new MessageAcceptArgs(key, param));
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public object Send<TTarget>(MessageAcceptArgs args)
            where TTarget:IMessageUsable
        {
            var vm = MessageViewModels.SingleOrDefault(v => v is TTarget);
            return SendMsg(vm, args);
        }
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <typeparam name="TTarget">目标</typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<object> SendAsync<TTarget>(MessageAcceptArgs args)
            where TTarget : IMessageUsable
        {
            var vm = MessageViewModels.SingleOrDefault(v => v is TTarget);
            return await SendMsgAsync(vm, args);
        }
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="key"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<object> SendAsync<TTarget>(object key,object param)
            where TTarget : IMessageUsable
        {
            var vm = MessageViewModels.SingleOrDefault(v => v is TTarget);
            return await SendMsgAsync(vm, new MessageAcceptArgs(key, param));
        }
        private async Task<object> SendMsgAsync(IMessageUsable vm, MessageAcceptArgs args)
        {
            if (vm != null && vm.Message != null)
            {
                return await vm.Message.AcceptedMessageAsync(depencyViewModel, args);
            }
            return null;
        }
        private object SendMsg(IMessageUsable vm, MessageAcceptArgs args)
        {
            if (vm != null && vm.Message != null)
            {
                return vm.Message.AcceptMsg(depencyViewModel, args);
            }
            return null;
        }
    }
}
