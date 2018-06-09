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
using EntityAbstract.Core.Models;

namespace EntityAbstract.Core.ViewModels
{
    public class ContentDetailViewModel : MsgDpViewModelBase
    {
        /// <summary>
        /// 每一页评论有多少个
        /// </summary>
        public static int PreCommentCount { get; set; } = 14;
        [NeedKeyInIoc]
        public static readonly int ContentDetailViewModelKey = 0x00231;
        private WebApiManager apiManager;
        private ContentNavHelperBase NavHelper => SimpleIoc.Inst.GetInstance<ContentNavHelperBase>(ContentNavHelperBase.ContentNavHelperKey);
        private IStringResource appResources;
        public ContentDetailViewModel()
        {
            IocHelper.EnsureIocKey(this);
            apiManager = SimpleIoc.Inst.GetInstance<WebApiManager>(ViewModelLocatorBase.ApiManagerKey);
            appResources = SimpleIoc.Inst.GetInstance<IStringResource>(ViewModelLocatorBase.StringReourceKey);
            ContentStack = new Stack<Content>();
            Comments = new ObservableCollection<RelComment>();
            GoBackCommand = new RelayCommand(GoBack);
            UpdateCommentCommand = new RelayCommand(UpdateComment);
            PrevPageCommand = new RelayCommand(PrevPage);
            NextPageCommand = new RelayCommand(NextPage);
            SendCommentCommand = new RelayCommand(SendComment);
            NavToCommentCommand = new RelayCommand(NavToComment);
            GoBackToContentCommand = new RelayCommand(GoBackToContent);
            HasPrev = false;
            PreSymbol = (char)0xE112;
        }
        public event Action ContentUriChanged;
        /// <summary>
        /// 记录打开的内容id
        /// </summary>
        private bool hasPrev;
        private Uri contentUri;
        private int localCommentPage;
        private int totalCommentCount;
        private string contentTitle;
        private bool isNonWebConnecting;
        private bool hasPrevPage;
        private bool hasNextPage;
        private Content localContent;
        private string sendCommentText;
        private char preSymbol;
        private bool isLeftVisibility = true;
        /// <summary>
        /// 那个<-键的符合
        /// </summary>
        public char PreSymbol
        {
            get => preSymbol;
            set => RaisePropertyChanged(ref preSymbol, value);
        }

        /// <summary>
        /// 准备发送的评论内容
        /// </summary>
        public string SendCommentText
        {
            get => sendCommentText;
            set => RaisePropertyChanged(ref sendCommentText, value);
        }

        /// <summary>
        /// 当前的内容
        /// </summary>
        public Content LocalContent
        {
            get => localContent;
            set => RaisePropertyChanged(ref localContent, value);
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
        /// 是否不在与服务器通信中
        /// </summary>
        public bool IsNonWebConnecting
        {
            get => isNonWebConnecting;
            set => RaisePropertyChanged(ref isNonWebConnecting, value);
        }

        /// <summary>
        /// 当前内容的标题
        /// </summary>
        public string ContentTitle
        {
            get => contentTitle;
            set => RaisePropertyChanged(ref contentTitle, value);
        }


        /// <summary>
        /// 评论一共有多少个
        /// </summary>
        public int TotalCommentCount
        {
            get => totalCommentCount;
            set => RaisePropertyChanged(ref totalCommentCount, value);
        }
        /// <summary>
        /// 当前评论页
        /// </summary>
        public int LocalCommentPage
        {
            get => localCommentPage;
            set => RaisePropertyChanged(ref localCommentPage, value);
        }

        /// <summary>
        /// 内容uri
        /// </summary>
        public Uri ContentUri
        {
            get => contentUri;
            set
            {
                RaisePropertyChanged(ref contentUri, value);
                ContentUriChanged?.Invoke();
            }
        }


        /// <summary>
        /// 是否有上一条记录
        /// </summary>
        public bool HasPrev
        {
            get => hasPrev;
            set => RaisePropertyChanged(ref hasPrev, value);
        }
        public Stack<Content> ContentStack { get; }
        public ObservableCollection<RelComment> Comments { get; }
        public RelayCommand GoBackCommand { get; }
        public RelayCommand UpdateCommentCommand { get; }
        public RelayCommand PrevPageCommand { get; }
        public RelayCommand NextPageCommand { get; }
        public RelayCommand SendCommentCommand { get; }
        public RelayCommand NavToCommentCommand { get; }
        public RelayCommand GoBackToContentCommand { get; }

        public event Action GoBackToContentAction;
        public event Action NavToCommentAcion;
        public void GoBackToContent()
        {
            GoBackToContentAction?.Invoke();
            //NavHelper.Border.Child = new ContentDetailBrowser();
        }
        public void NavToComment()
        {
            NavToCommentAcion?.Invoke();
            //NavHelper.Border.Child = new CommentView();
            UpdateComment();
        }
        public void LoadContent(Content c)
        {
            Load(c, true);
        }
        private void Load(Content c, bool push = true)
        {
            try
            {
                LocalContent = c;
                IsNonWebConnecting = false;
                ContentTitle = LocalContent.Title;
                if (push)
                {
                    ContentStack.Push(c);//历史
                }
                UpdateContentStack();//更新内容栈
                LocalCommentPage = 0;
                ContentUri = new Uri(apiManager.GetContentDetailUrl(c.Id));
                UpdateComment();//更新评论
            }
            catch (Exception)
            {
                ShowMsg(appResources.GetString("FlushFail"));
            }
        }
        // ContentView的左半部分是否可见
        public void GoBack()
        {
            /*
            if (HasPrev)
            {
                ContentStack.Pop();
                var data = ContentStack.Pop();
                Load(data,false);
                RedirectTo(data.Id);
                UpdateContentStack();
            }
            */
            isLeftVisibility = !isLeftVisibility;
            if (isLeftVisibility)
            {
                PreSymbol = (char)0xE112 ;
            }
            else
            {
                PreSymbol = (char)0xE111;
            }
            SetParVisibility(isLeftVisibility);
        }
        public void PrevPage()
        {
            LocalCommentPage--;
            UpdateComment();
        }
        public void NextPage()
        {
            LocalCommentPage++;
            UpdateComment();
        }
        public async void DeleteComment(uint cid)
        {
            if (!apiManager.HasAccountToken)
            {
                ShowMsg(appResources.GetString("NonLogin"));
                return;
            }
            var res = await apiManager.DeleteCommentAsync(cid);
            ShowMsg(appResources.GetString(res.Reponse.IsSucceed ? "DelCommOk" : "DelCommErr"));
            UpdateComment();
        }
        public async void UpdateComment()
        {
            if (LocalContent != null)
            {
                Comments.Clear();
                IsNonWebConnecting = false;
                var ap = await apiManager.GetCommentsAsync(LocalContent.Id, PreCommentCount, LocalCommentPage * PreCommentCount);
                if (!ap.Reponse.IsSucceed)
                {
                    ShowMsg(appResources.GetString("FlushFail"));
                    return;
                }
                TotalCommentCount = ap.Data.Total;
                foreach (var item in ap.Data.Datas)
                {
                    if (item.EaUserId==apiManager.User?.Id)
                    {
                        item.IsSelf = true;
                    }
                    else
                    {
                        item.IsSelf = false;
                    }
                    Comments.Add(item);
                }
                IsNonWebConnecting = true;
                HasPrevPage = LocalCommentPage > 0;
                HasNextPage = (LocalCommentPage + 1) * PreCommentCount < TotalCommentCount;
            }
        }
        public async void SendComment()
        {
            if (!apiManager.HasAccountToken)
            {
                ShowMsg(appResources.GetString("NonLogin"));
                return;

            }
            if (string.IsNullOrEmpty(SendCommentText))
            {
                ShowMsg(appResources.GetString("CommentTextEmpty"));
                return;
            }
            if (SendCommentText.Length<8)
            {
                ShowMsg(appResources.GetString("CommLengthErr"));
                return;
            }
            if (LocalContent!=null)
            {
                try
                {
                    var ap = await apiManager.SendCommentAsync(LocalContent.Id, SendCommentText);
                    if (!ap.Reponse.IsSucceed||!ap.Data)
                    {
                        ShowMsg(appResources.GetString("CommentSendErr"));
                    }
                    else
                    {
                        UpdateComment();
                        ShowMsg(appResources.GetString("SendCommOk"));
                        SendCommentText = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message);
                }
            }
        }
        //更新内容栈关联信息
        private void UpdateContentStack()
        {
            HasPrev = ContentStack.Count > 1;
        }
        private void ShowMsg(string msg)
        {
            Message.Send<MainViewModel>(MainViewModel.ShowMsgKey, new MsgShowArgs(1000, msg));
        }
        private object RedirectTo(uint cid)
        {
            return Message.Send<ContentViewModel>(ContentViewModel.SetVisitContentKey, cid);
        }
        private object SetParVisibility(bool isVisibility)
        {
            return Message.Send<ContentViewModel>(ContentViewModel.SetLeftPartVisibilityKey, isVisibility);
        }
    }
}
