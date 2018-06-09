using EntityAbstract.Core.Helpers;
using EntityAbstract.Core.Models;
using Mvvmlighting.Args;
using Mvvmlighting.Attributes;
using Mvvmlighting.Ioc;
using Mvvmlighting.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityAbstract.Core.ViewModels
{
    public class MainViewModel : MsgDpViewModelBase
    {
        [NeedKeyInIoc]
        public static readonly int MainViewModelKey = 0x00234;
        [NeedKeyInIoc]
        public static readonly int ShowMsgKey = 0x10234;
        /// <summary>
        /// ViewItem Content Id
        /// </summary>
        public static readonly int ContentId = 0x1232;
        /// <summary>
        /// 主页id
        /// </summary>
        public static readonly int HomePageId = 0x2333;
        /// <summary>
        /// 内容页id
        /// </summary>
        public static readonly int ContentPageId = 0x2344;
        /// <summary>
        /// 消息页id
        /// </summary>
        public static readonly int MsgPageId = 0x2335;
        /// <summary>
        /// 发送内容页id
        /// </summary>
        public static readonly int SendContentPageId = 0x2346;
        private IStringResource stringResource;
        public MainViewModel()
        {
            IocHelper.EnsureIocKey(this);
            stringResource = ViewModelLocatorBase.StringResources;
            ViewItems = new ObservableCollection<ViewItem>
            {
                new ViewItem(stringResource.GetString("HomeText"),(char)0xE10F,string.Empty,HomePageId),
                new ViewItem(stringResource.GetString("ContentText"),(char)0xE132,string.Empty,ContentPageId),
                new ViewItem(stringResource.GetString("MakeContentTitle"),(char)0xE11F,string.Empty,SendContentPageId),
                new ViewItem(stringResource.GetString("MsgPageText"),(char)0xE119,string.Empty,MsgPageId),
            };
            //注册消息服务
            MsgHandle.Add(ShowMsgKey, ShowMsgHandle);
        }
        public event Action<MsgShowArgs> NeedShowMsg;

        private object ShowMsgHandle(IMessageUsable sender, MessageAcceptArgs e)
        {
            if (e.Param is MsgShowArgs msa)
            {
                NeedShowMsg?.Invoke(msa);
            }
            return null;
        }
        private ViewItem currentViewItem;
        private string title;

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get => title;
            set => RaisePropertyChanged(ref title, value);
        }

        public ViewItem CurrentViewItem
        {
            get => currentViewItem;
            set
            {
                if (value != null) 
                {
                    Title = value.Title;
                }
                RaisePropertyChanged(ref currentViewItem, value);
            }
        }

        #region Commands
        public ObservableCollection<ViewItem> ViewItems { get; }
        #endregion
    }
}
