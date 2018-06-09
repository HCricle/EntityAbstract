using MvvmLighting.Ioc;
using MvvmLighting.Mvvm;
using Shared.Core.Api.WebApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Core.Interactive.ViewModels
{
    /// <summary>
    /// 发送评论
    /// </summary>
    public class MakeCommentViewModel : ViewModelBase
    {
        private WebApiManager webApiManager;
        private string content;
        private uint contentId;
        private string contentTitle;
        private string errorMsg;
        public MakeCommentViewModel()
        {
            webApiManager = SimpleIoc.Inst.GetInstance<WebApiManager>(ViewModelLocator.WebApiManagerKey);
            SendCommand = new RelayCommand(Send);
        }
        #region Events
        public event Action<string,bool> Sended;
        #endregion
        #region Can_Bind_Properties
        /// <summary>
        /// 内容id
        /// </summary>
        public uint ContentId
        {
            get => contentId;
            set => RaisePropertyChanged(ref contentId, value);
        }
        /// <summary>
        /// 发送的内容
        /// </summary>
        public string Content
        {
            get => content;
            set => RaisePropertyChanged(ref content, value);
        }
        /// <summary>
        /// 内容标题
        /// </summary>
        public string ContentTitle
        {
            get => contentTitle;
            set => RaisePropertyChanged(ref contentTitle, value);
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg
        {
            get => errorMsg;
            set => RaisePropertyChanged(ref errorMsg, value);
        }

        #endregion
        #region Commands
        public RelayCommand SendCommand { get; }
        #endregion
        public async void Send()
        {
            if (!webApiManager.HasAccountToken)
            {
                ErrorMsg = "未登陆";
                Sended?.Invoke(ErrorMsg, false);
                return;
            }
            var res = await webApiManager.SendCommentAsync(ContentId, Content);
            if (res!=null)
            {
                var rstr = await res.GetContentStreamAsync();
                ErrorMsg = rstr;
            }
            else
            {
                ErrorMsg = "发送失败";
            }
            Sended?.Invoke(ErrorMsg, res.IsSucceed);
        }
    }
}
