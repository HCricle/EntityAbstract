using MvvmLighting.Ioc;
using MvvmLighting.Mvvm;
using Shared.Core.Api.Models;
using Shared.Core.Api.WebApi;
using SharedEA.Core.Model.DbModels;
using SharedEA.Core.WebApi.JWT;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Shared.Core.Interactive.ViewModels
{
    /// <summary>
    /// 内容的数据模型
    /// </summary>
    public class ContentViewModel : ViewModelBase
    {
        private static uint DefaultGroupId = 1;//暂定只有一个组
        private WebApiManager webApiManager;
        private int contentPage;
        private uint seeContentId;
        private int commentPage;
        private string searchText;
        private int searchPage;
        private string commentText;
        private string sendContentTitle;
        private string sendContentContent;
        private string sendContentLabel;
        private bool hasPrevContentPage;
        private bool hasNextContentPage;
        private bool hasPrevCommentPage;
        private bool hasNextCommentPage;
        private uint operatorCommentId;
        private int totalContentCount;
        private string totalContentString;
        private string totalCommentString;
        public ContentViewModel()
        {
            webApiManager = SimpleIoc.Inst.GetInstance<WebApiManager>(ViewModelLocator.WebApiManagerKey);
            SendFiles = new List<ApiFileModel>();
            UpdateContentCommand = new RelayCommand(UpdateContent);
            UpdateCommentCommand = new RelayCommand(UpdateComment);
            SendContentCommand = new RelayCommand(SendContent);
            SearchContentCommand = new RelayCommand(SearchContent);
            SendCommentCommand = new RelayCommand(SendComment);
            DeleteCommentCommand = new RelayCommand(DeleteComment);
            DeleteContentCommand = new RelayCommand(DeleteContent);
            NextContentPageCommand = new RelayCommand(NextContentPage);
            PrevContentPageCommand = new RelayCommand(PrevContentPage);
            PrevCommentCommand = new RelayCommand(PrevCommentPage);
            NextCommentCommand = new RelayCommand(NextCommentPage);
            Contents = new ObservableCollection<Content>();
            Comments = new ObservableCollection<Comment>();
            SearchedContents = new ObservableCollection<Content>();
        }
        #region Events
        /// <summary>
        /// 内容集合改变了
        /// </summary>
        public event Action<ApiReponse> ContentChanged;
        public event Action<ApiReponse> CommentChanged;
        public event Action<ApiReponse> SendedContent;
        public event Action<ApiReponse> SearchContentsChanged;
        public event Action<ApiReponse> SendedComment;
        public event Action<ApiReponse> DeletedComment;
        public event Action<ApiReponse> DeletedContent;
        public event Action HasCommentPageChanged;
        public event Action HasContentPageChanged;
        public event Action TotalContentCountChanged;
        #endregion
        #region Can_Bind_Properties
        /// <summary>
        /// 内容页，可以不为当前页
        /// </summary>
        public int ContentPage
        {
            get => contentPage;
            set => RaisePropertyChanged(ref contentPage, value);
        }
        /// <summary>
        /// 评论页
        /// </summary>
        public int CommentPage
        {
            get => commentPage;
            set => RaisePropertyChanged(ref commentPage, value);
        }

        /// <summary>
        /// 当前或想看的内容id
        /// </summary>
        public uint SeeContentId
        {
            get => seeContentId;
            set
            {
                RaisePropertyChanged(ref seeContentId, value);
                CommentPage = 0;
            }
        }
        /// <summary>
        /// 发送内容的title
        /// </summary>
        public string SendContentTitle
        {
            get => sendContentTitle;
            set => RaisePropertyChanged(ref sendContentTitle, value);
        }
        /// <summary>
        /// 发送的内容的content
        /// </summary>
        public string SendContentContent
        {
            get => sendContentContent;
            set => RaisePropertyChanged(ref sendContentContent, value);
        }
        /// <summary>
        /// 发送的内容标签
        /// </summary>
        public string SendContentLabel
        {
            get => sendContentLabel;
            set => RaisePropertyChanged(ref sendContentLabel, value);
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
        /// 欲搜索的内容页
        /// </summary>
        public int SearchPage
        {
            get => searchPage;
            set => RaisePropertyChanged(ref searchPage, value);
        }
        /// <summary>
        /// 欲发送的评论内容
        /// </summary>
        public string CommentText
        {
            get => commentText;
            set => RaisePropertyChanged(ref commentText, value);
        }
        /// <summary>
        /// 欲操作的评论id
        /// </summary>
        public uint OperatorCommentId
        {
            get => operatorCommentId;
            set => RaisePropertyChanged(ref operatorCommentId, value);
        }
        /// <summary>
        /// 评论是否有下一页
        /// </summary>
        public bool HasNextCommentPage
        {
            get => hasNextCommentPage;
            set => RaisePropertyChanged(ref hasNextCommentPage, value);
        }

        /// <summary>
        /// 评论是否有下一页
        /// </summary>
        public bool HasPrevCommentPage
        {
            get => hasPrevCommentPage;
            set => RaisePropertyChanged(ref hasPrevCommentPage, value);
        }

        /// <summary>
        /// 内容是否有下一页
        /// </summary>
        public bool HasNextContentPage
        {
            get => hasNextContentPage;
            set => RaisePropertyChanged(ref hasNextContentPage, value);
        }

        /// <summary>
        /// 内容是否有前一页
        /// </summary>
        public bool HasPrevContentPage
        {
            get => hasPrevContentPage;
            set => RaisePropertyChanged(ref hasPrevContentPage, value);
        }
        /// <summary>
        /// 总内容数
        /// </summary>
        public int TotalContentCount
        {
            get => totalContentCount;
            set => RaisePropertyChanged(ref totalContentCount, value);
        }
        /// <summary>
        /// (当前页/总页数)
        /// </summary>
        public string TotalContentString
        {
            get => totalContentString;
            set => RaisePropertyChanged(ref totalContentString, value);
        }
        /// <summary>
        /// (当前评论页/??) 服务端没做呀
        /// </summary>
        public string TotalCommentString
        {
            get => totalCommentString;
            set => RaisePropertyChanged(ref totalCommentString, value);
        }

        /// <summary>
        /// 内容的集合，可能为空
        /// </summary>
        public ObservableCollection<Content> Contents { get;  }
        /// <summary>
        /// 评论列表，可能为空
        /// </summary>
        public ObservableCollection<Comment> Comments { get;  }
        /// <summary>
        /// 搜索到的内容
        /// </summary>
        public ObservableCollection<Content> SearchedContents { get;  }
        #endregion
        public List<ApiFileModel> SendFiles { get; }
        public RelayCommand UpdateContentCommand { get; }
        public RelayCommand UpdateCommentCommand { get; }
        public RelayCommand SendContentCommand { get;  }
        public RelayCommand SearchContentCommand { get;  }
        public RelayCommand SendCommentCommand { get;  }
        public RelayCommand DeleteCommentCommand { get;  }
        public RelayCommand DeleteContentCommand { get;  }
        public RelayCommand NextContentPageCommand { get; }
        public RelayCommand PrevContentPageCommand { get; }
        public RelayCommand PrevCommentCommand { get; }
        public RelayCommand NextCommentCommand { get; }
        public void NextContentPage()
        {
            ContentPage++;
            UpdateContent();
        }
        public void PrevContentPage()
        {
            if (contentPage>0)
            {
                ContentPage--;
                UpdateContent();
            }
        }
        public async void UpdateContent()
        {
            var res = await webApiManager.GetContentsAsync(DefaultGroupId, ContentPage);
            var ccres = await webApiManager.GetContentCountAsync();
            int ccount=0;
            try
            {
                var csc = await ccres.GetContentStreamAsync();
                ccount = Convert.ToInt32(csc);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
            }
            var cs = await res.GetForAsync<Content[]>();
            Contents.Clear();
            foreach (var item in cs)
            {
                Contents.Add(item);
            }
            HasNextContentPage = (ContentPage+1) * 6 < ccount;
            HasPrevContentPage = ContentPage > 0;
            TotalContentCount = ccount;
            TotalContentString = $"{ContentPage}/{TotalContentCount / 6}";
            TotalContentCountChanged?.Invoke();
            HasContentPageChanged?.Invoke();
            ContentChanged?.Invoke(res);

        }
        public void NextCommentPage()
        {
            CommentPage++;
            UpdateComment();
        }
        public void PrevCommentPage()
        {
            if (CommentPage>0)
            {
                CommentPage--;
                UpdateComment();
            }
        }
        public async void UpdateComment()
        {
            var res = await webApiManager.GetCommentsAsync(SeeContentId, CommentPage);
            var nextres = await webApiManager.GetCommentsAsync(SeeContentId, CommentPage + 1);
            var nextcs = await nextres.GetForAsync<Comment[]>();
            var cs = await res.GetForAsync<Comment[]>();
            Comments.Clear();
            foreach (var item in cs)
            {
                var uname = await webApiManager.GetNameAsync(item.EaUserId);
                item.EaUserId =await uname.GetContentStreamAsync();
                Comments.Add(item);
            }
            HasNextCommentPage = nextcs.Count() > 0;
            HasPrevCommentPage = CommentPage > 0;
            TotalCommentString = $"{CommentPage}/??(ps:万一有呢!)";
            HasCommentPageChanged?.Invoke();
            //评论分页没做(服务端)
            CommentChanged?.Invoke(res);
        }
        public async void SendContent()
        {
            var model = new SendContentModel(DefaultGroupId, SendContentTitle, SendContentContent??string.Empty, SendContentLabel)
            {
                FileDatas = SendFiles
            };
            var res = await webApiManager.SendContentAsync(model);
            var ss =await res.GetForAsync<Content[]>();
            SearchedContents.Clear();
            foreach (var item in ss)
            {
                SearchedContents.Add(item);
            }
            SendedContent?.Invoke(res);
        }
        public async void SearchContent()
        {
            var res = await webApiManager.SearchContentAsync(SearchText, SearchPage);
            SearchContentsChanged?.Invoke(res);
        }
        public async void SendComment()
        {
            var res = await webApiManager.SendCommentAsync(SeeContentId, CommentText);
            SendedComment?.Invoke(res);
        }
        public async void DeleteComment()
        {
            var res = await webApiManager.DeleteComment(OperatorCommentId);
            DeletedComment?.Invoke(res);
        }
        public async void DeleteContent()
        {
            var res = await webApiManager.DeleteContentAsync(SeeContentId);
            DeletedContent?.Invoke(res);
        }
        /// <summary>
        /// 获取内容详细id
        /// </summary>
        /// <returns></returns>
        public string GetContentDetailUrl()
        {
            return webApiManager.GetContentDetailUrl(SeeContentId);
        }
        

    }
}
