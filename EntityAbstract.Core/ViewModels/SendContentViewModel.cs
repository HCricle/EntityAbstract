using EntityAbstract.Core.Helpers;
using EntityAbstract.Core.Models;
using Mvvmlighting.Ioc;
using Mvvmlighting.Mvvm;
using Shared.Core.Api.WebApi;
using SharedEA.Core.WebApi.JWT;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EntityAbstract.Core.ViewModels
{
    //发送内容模型
    public class SendContentViewModel : MsgDpViewModelBase
    {
        public static readonly int SendContentViewModelKey = 0x12329;
        private static readonly uint DefaultGroup = 1;
        private WebApiManager apiManager;
        private IStringResource stringResource;
        public SendContentViewModel()
        {
            apiManager = SimpleIoc.Inst.GetInstance<WebApiManager>(ViewModelLocatorBase.ApiManagerKey);
            stringResource = ViewModelLocatorBase.StringResources;

            SendFiles = new ObservableCollection<ApiFileModel>();
            SendCommand = new RelayCommand(Send);
            CleanFilesCommand = new RelayCommand(CleanFiles);

            Lable = string.Empty;
        }
        private string sendContent;
        private string title;
        private string label;

        /// <summary>
        /// 标签
        /// </summary>
        public string Lable
        {
            get => label;
            set => RaisePropertyChanged(ref label, value);
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get => title;
            set => RaisePropertyChanged(ref title, value);
        }

        /// <summary>
        /// 欲发送的文本内容
        /// </summary>
        public string SendContent
        {
            get => sendContent;
            set => RaisePropertyChanged(ref sendContent, value);
        }
        public ObservableCollection<ApiFileModel> SendFiles { get; }

        public RelayCommand SendCommand { get; }
        public RelayCommand CleanFilesCommand { get; }
        public void AddFiles(params ApiFileModel[] files)
        {
            foreach (var item in files)
            {
                SendFiles.Add(item);
            }
        }
        public void RemoveFiles(params ApiFileModel[] files)
        {
            foreach (var item in files)
            {
                SendFiles.Remove(item);
            }
        }
        public void CleanFiles()
        {
            SendFiles.Clear();
        }
        public async void Send()
        {
            try
            {
                var model = new SendContentModel(DefaultGroup, Title, SendContent, Lable)
                {
                    FileDatas = SendFiles
                };
                var fpm = await apiManager.SendContentAsync(model);
                if (!fpm.Reponse.IsSucceed||!fpm.Data)
                {
                    ShowMsg("SendContentErr");
                    return;
                }
                ShowMsg("SendContentOk");
                SendContent =Title= string.Empty;
                SendFiles.Clear();
            }
            catch (Exception)
            {
                ShowMsg("SendException");
            }
        }
        private void ShowMsg(string key)
        {
            Message.Send<MainViewModel>(MainViewModel.ShowMsgKey, new MsgShowArgs(1000, stringResource.GetString(key)));
        }
    }
}
