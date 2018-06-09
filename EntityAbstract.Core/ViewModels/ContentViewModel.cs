using EntityAbstract.Core.Helpers;
using Mvvmlighting.Attributes;
using Mvvmlighting.Ioc;
using Mvvmlighting.Mvvm;
using Shared.Core.Api.WebApi;
using SharedEA.Core.DbModel.RepositoryModel;
using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mvvmlighting.Args;
using EntityAbstract.Core.Models;

namespace EntityAbstract.Core.ViewModels
{
    public class ContentViewModel : MsgDpViewModelBase
    {
        private static readonly int _ParWidth=270;
        [NeedKeyInIoc]
        public static readonly int ContentViewModelKey=0x30125;

        public static readonly int RedirectToItemKey = 0x30126;
        //设置访问内容消息键
        public static readonly int SetVisitContentKey = 0x1231;
        /// <summary>
        /// 设置左半部分是否可见
        /// </summary>
        public static readonly int SetLeftPartVisibilityKey = 0x1232;
        /// <summary>
        /// 每一页多少个
        /// </summary>
        public static int PrePageCount { get; set; } = 6;
        private static uint DefaultGroup = 1;
        private WebApiManager apiManager;
        private IStringResource appResources;
        public ContentViewModel()
        {
            LocalPage = 0;
            IsNotWebConnecting = false;
            ParWidth = _ParWidth;
            apiManager = SimpleIoc.Inst.GetInstance<WebApiManager>(ViewModelLocatorBase.ApiManagerKey);
            appResources = SimpleIoc.Inst.GetInstance<IStringResource>(ViewModelLocatorBase.StringReourceKey);
            Contents = new ObservableCollection<Content>();
            IsNotWebConnecting = true;
            UpdateContentCommand = new RelayCommand(UpdateContent);
            SearchContentCommand = new RelayCommand(SearchContent);
            NextPageCommand = new RelayCommand(NextPage);
            PrevPageCommand = new RelayCommand(PrevPage);
            IocHelper.EnsureIocKey(this);
            MsgHandle.Add(SetVisitContentKey, SetVisitContent);
            MsgHandle.Add(SetLeftPartVisibilityKey, SetLeftPartVisibility);
        }
        private object SetLeftPartVisibility(IMessageUsable messageUsable, MessageAcceptArgs e)
        {
            if (e.Param is bool b)
            {
                ParWidth = b ? _ParWidth : 0;
            }
            return true;
        }
        private object SetVisitContent(IMessageUsable messageUsable, MessageAcceptArgs e)
        {
            if (e.Param is uint id)//直接发送cid
            {
                CurrentContent = Contents.Single(c => c.Id == id);
            }
            return true;
        }
        #region Can_Bind_Properties
        private int localPage;
        private int totalCount;
        private string searchText;
        private bool isNotWebConnecting;
        private string tipText;
        private bool hasPrevPage;
        private bool hasNextPage;
        private string flushText;
        private Content localContent;
        private Content currentContent;
        private int parMinWidth;

        public int ParWidth
        {
            get => parMinWidth;
            set => RaisePropertyChanged(ref parMinWidth, value);
        }

        public Content CurrentContent
        {
            get => currentContent;
            set
            {
                if (value== currentContent)
                {
                    return;
                }
                RaisePropertyChanged(ref currentContent, value);
            }
        }
        /// <summary>
        /// 当前选中的content item
        /// </summary>
        public Content LocalContent
        {
            get => localContent;
            set => RaisePropertyChanged(ref localContent, value);
        }

        /// <summary>
        /// 下拉更新的文本
        /// </summary>
        public string FlushText
        {
            get => flushText;
            set => RaisePropertyChanged(ref flushText, value);
        }

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage
        {
            get => hasNextPage;
            set => RaisePropertyChanged(ref hasNextPage, value);
        }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPrevPage
        {
            get => hasPrevPage;
            set => RaisePropertyChanged(ref hasPrevPage, value);
        }

        /// <summary>
        /// 提示的文本
        /// </summary>
        public string TipText
        {
            get => tipText;
            set => RaisePropertyChanged(ref tipText, value);
        }

        /// <summary>
        /// 是否正在于服务器连接中
        /// </summary>
        public bool IsNotWebConnecting
        {
            get => isNotWebConnecting;
            set => RaisePropertyChanged(ref isNotWebConnecting, value);
        }
        
        /// <summary>
        /// 欲搜索的内容
        /// </summary>
        public string SearchText
        {
            get => searchText;
            set => RaisePropertyChanged(ref searchText, value);
        }
        /// <summary>
        /// 当前内容页
        /// </summary>
        public int LocalPage
        {
            get => localPage;
            set => RaisePropertyChanged(ref localPage, value);
        }
        /// <summary>
        /// 内容总数
        /// </summary>
        public int TotalCount
        {
            get => totalCount;
            set => RaisePropertyChanged(ref totalCount, value);
        }

        #endregion
        public ObservableCollection<Content> Contents { get;  }

        public RelayCommand UpdateContentCommand { get; }
        public RelayCommand SearchContentCommand { get; }
        public RelayCommand NextPageCommand { get; }
        public RelayCommand PrevPageCommand { get; }
        public void NextPage()
        {
            LocalPage++;
            UpdateContent();
        }
        public void PrevPage()
        {
            LocalPage++;
            UpdateContent();
        }
        public async void SearchContent()
        {
            try
            {
                FlushText = appResources.GetString("SearchingText");
                IsNotWebConnecting = false;
                var ap = await apiManager.SearchContentAsync(SearchText,PrePageCount, LocalPage * PrePageCount, false);
                TotalCount = ap.Data.Total;
                Contents.Clear();
                foreach (var item in ap.Data.Datas)
                {
                    Contents.Add(item);
                }
                TipText = $"搜索{SearchText}共有{TotalCount}个,当前第{LocalPage}页";
            }
            catch (Exception)
            {
                ShowMsg(appResources.GetString("FlushFail"));
            }
            finally
            {
                IsNotWebConnecting = true;
                FlushText = appResources.GetString("FlushText");
            }
        }
        public async void UpdateContent()
        {
            try
            {
                FlushText = appResources.GetString("FlushingText");
                IsNotWebConnecting = false;
                var ap = await apiManager.GetContentsAsync(DefaultGroup, LocalPage * PrePageCount, PrePageCount, false);
                TotalCount = ap.Data.Total;
                Contents.Clear();
                foreach (var item in ap.Data.Datas)
                {
                    Contents.Add(item);
                }
                HasPrevPage = LocalPage > 0;
                HasNextPage = (LocalPage + 1) * PrePageCount < TotalCount;
                TipText = $"共{TotalCount}个,当前第{LocalPage}页";

            }
            catch (Exception)
            {
                ShowMsg(appResources.GetString("FlushFail"));
            }
            finally
            {
                IsNotWebConnecting = true;
                FlushText = appResources.GetString("FlushText");
            }
        }
        private void ShowMsg(string msg)
        {
            Message.Send<MainViewModel>(MainViewModel.ShowMsgKey, new MsgShowArgs(1000, msg));
        }
    }
}
