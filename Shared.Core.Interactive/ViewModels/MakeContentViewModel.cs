using MvvmLighting.Ioc;
using MvvmLighting.Mvvm;
using Shared.Core.Api.Models;
using Shared.Core.Api.WebApi;
using SharedEA.Core.WebApi.JWT;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Shared.Core.Interactive.ViewModels
{
    /// <summary>
    /// 发送内容的vm
    /// </summary>
    public class MakeContentViewModel : ViewModelBase
    {
        private static readonly uint DefaultGroupId=1;
        private WebApiManager webApiManager;
        private uint groupId;
        private string title;
        private string content;
        private string lable;
        private string errorMsg;
        public MakeContentViewModel()
        {
            webApiManager = SimpleIoc.Inst.GetInstance<WebApiManager>(ViewModelLocator.WebApiManagerKey);
            Files = new ObservableCollection<ApiFileModel>();
            SendCommand = new RelayCommand(Send);
            RemoveFileCommand = new RelayCommand<ApiFileModel>(RemoveFile);
            GroupId = DefaultGroupId;
        }

        #region Events
        public event Action<ApiReponse> SendedContent;
        public event Action<string> ErrorMsgChanged;
        #endregion
        #region Can_Bind_Properties
        /// <summary>
        /// 欲发送的组id
        /// </summary>
        public uint GroupId
        {
            get => groupId;
            set => RaisePropertyChanged(ref groupId, value);
        }
        /// <summary>
        /// 内容标题
        /// </summary>
        public string Title
        {
            get => title;
            set => RaisePropertyChanged(ref title, value);
        }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get => content;
            set => RaisePropertyChanged(ref content, value);
        }
        /// <summary>
        /// 内容标签
        /// </summary>
        public string Lable
        {
            get => lable;
            set => RaisePropertyChanged(ref lable, value);
        }
        /// <summary>
        /// 错误的信息
        /// </summary>
        public string ErrorMsg
        {
            get => errorMsg;
            set => RaisePropertyChanged(ref errorMsg, value);
        }
        /// <summary>
        /// 发送的文件集合
        /// </summary>
        public ObservableCollection<ApiFileModel> Files { get;  }
        #endregion
        #region Commands
        public RelayCommand SendCommand { get;  }
        public RelayCommand<ApiFileModel> RemoveFileCommand { get;  }
        #endregion
        public async void Send()
        {
            ErrorMsg = string.Empty;
            if (!webApiManager.HasAccountToken)
            {
                ErrorMsg = "未登陆";
                SendedContent?.Invoke(null);
                return;
            }
            if (string.IsNullOrEmpty(Title))
            {
                ErrorMsg = "标题为空";
            }
            Lable = Lable ?? string.Empty;
            Content = Content ?? string.Empty;
            var model = new SendContentModel(GroupId, Title, Content, Lable)
            {
                FileDatas = Files
            };
            var res=await webApiManager.SendContentAsync(model);
            if (!res.IsSucceed)
            {
                ErrorMsg = await res.GetContentStreamAsync();
                ErrorMsgChanged?.Invoke(ErrorMsg);
            }
            SendedContent?.Invoke(res);
        }
        public void RemoveFile(ApiFileModel model)
        {
            if (model != null) 
            {
                if (Files.Contains(model))
                {
                    Files.Remove(model);
                }
            }
        }
    }
}
