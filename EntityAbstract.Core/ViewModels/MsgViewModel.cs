using EntityAbstract.Core.Helpers;
using EntityAbstract.Core.Models;
using Mvvmlighting.Ioc;
using Mvvmlighting.Mvvm;
using Newtonsoft.Json;
using Shared.Core.Api.WebApi;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.Models;
using SharedEA.Core.DbModel.RepositoryModel;
using SharedEA.Core.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace EntityAbstract.Core.ViewModels
{
    /// <summary>
    /// 消息响应模型
    /// </summary>
    public class MsgViewModel : MsgDpViewModelBase
    {
        public static readonly int MsgViewModelKey = 0x10987;
        private static readonly int PreMsgCount = 12;
        private static readonly int PreMsgDetailCount = 12;
        private static readonly int PreApplyCount = 6;
        /// <summary>
        /// 到时搞进设置里
        /// 是否将消息详细倒序显示
        /// </summary>
        private static readonly bool IsDescForMsgDetail = true;
        private WebApiManager apiManager;
        private IStringResource stringResource;
        public MsgViewModel()
        {
            PreWidth = 270;
            apiManager = SimpleIoc.Inst.GetInstance<WebApiManager>(ViewModelLocatorBase.ApiManagerKey);
            stringResource = SimpleIoc.Inst.GetInstance<IStringResource>(ViewModelLocatorBase.StringReourceKey);

            Msgs = new ObservableCollection<Msg>();
            MsgDetails = new ObservableCollection<MsgDetail>();
            Friends = new ObservableCollection<RelFriend>();
            SearchedFriends = new ObservableCollection<RelFriend>();
            ApplyFriends = new ObservableCollection<RelFriend>();

            ApplyFriendCommand = new RelayCommand(ApplyFriend);
            UpdateFriendCommand = new RelayCommand(UpdateFriend);
            SendCommand = new RelayCommand(Send);
            UpdateMsgCommand = new RelayCommand(UpdateMsg);
            LoadMsgCommand = new RelayCommand(LoadMsg);
            NextMsgPageCommand = new RelayCommand(NextMsgPage);
            PrevMsgPageCommand = new RelayCommand(PrevMsgPage);
            DeleteMsgCommand = new RelayCommand(DeleteMsg);
            PrevMsgDetailPageCommand = new RelayCommand(PrevMsgDetailPage);
            NextMsgDetailPageCommand = new RelayCommand(NextMsgDetailPage);
            ApplyFriendCommand = new RelayCommand(ApplyFriend);
            ApplyPageNextCommand = new RelayCommand(ApplyPageNext);
            ApplyPagePrevCommand = new RelayCommand(ApplyPagePrev);
            MsgTitle = stringResource.GetString("MsgTitleNull");
        }
        private int preWidth;
        private int locMsgPage;
        private int msgCount;
        private bool msgCanGoPrev;
        private bool msgCanGoNext;
        private Msg locMsg;
        private string msgDetailText;
        private int msgDetailCount;
        private int locMsgDetailPage;
        private bool msgDetailCanGoPrev;
        private bool msgDetailCanGoNext;
        private string msgText;
        private bool hasLocMsg;
        private string sendText;
        private string searchText;
        private string msgTitle;
        private string searchUserText;
        private bool searchNotFriend;
        private bool showFriendWithoutHasMsg;
        private int locApplyPage;
        private bool applyHasPrev;
        private bool applyHasNext;
        private int applyCount;
        private string applyDetalText;
        private bool hasFriendApply;

        /// <summary>
        /// 是否存在朋友申请
        /// </summary>
        public bool HasFriendApply
        {
            get => hasFriendApply;
            set => RaisePropertyChanged(ref hasFriendApply, value);
        }

        /// <summary>
        /// 申请的页数详细信息
        /// </summary>
        public string ApplyDetalText
        {
            get => applyDetalText;
            set => RaisePropertyChanged(ref applyDetalText, value);
        }

        /// <summary>
        /// 申请的总数
        /// </summary>
        public int ApplyCount
        {
            get => applyCount;
            set => RaisePropertyChanged(ref applyCount, value);
        }

        /// <summary>
        /// 申请页是否有下一页
        /// </summary>
        public bool ApplyHasNext
        {
            get => applyHasNext;
            set => RaisePropertyChanged(ref applyHasNext, value);
        }

        /// <summary>
        /// 申请页是否有上一页
        /// </summary>
        public bool ApplyHasPrev
        {
            get => applyHasPrev;
            set => RaisePropertyChanged(ref applyHasPrev, value);
        }

        /// <summary>
        /// 当前申请页
        /// </summary>
        public int LocApplyPage
        {
            get => locApplyPage;
            set => RaisePropertyChanged(ref locApplyPage, value);
        }

        /// <summary>
        /// 显示朋友的时候是否不要显示含有会话的
        /// </summary>
        public bool ShowFriendWithoutHasMsg
        {
            get => showFriendWithoutHasMsg;
            set
            {
                RaisePropertyChanged(ref showFriendWithoutHasMsg, value);
                UpdateFriend();
            }
        }

        /// <summary>
        /// 搜索栏里是朋友？
        /// </summary>
        public bool SearchNotFriend
        {
            get => searchNotFriend;
            set => RaisePropertyChanged(ref searchNotFriend, value);
        }
        /// <summary>
        /// 搜索的用户，如果存在
        /// </summary>
        public string SearchUserText
        {
            get => searchUserText;
            set
            {
                RaisePropertyChanged(ref searchUserText, value);
                //SearchFriend();性能问题
            }
        }

        /// <summary>
        /// 当前消息的标题，暂时是朋友名字
        /// </summary>
        public string MsgTitle
        {
            get => msgTitle;
            set => RaisePropertyChanged(ref msgTitle, value);
        }

        /// <summary>
        /// 欲搜索的会话
        /// </summary>
        public string SearchText
        {
            get => searchText;
            set
            {
                RaisePropertyChanged(ref searchText, value);
                SearchFriend();
            }
        }

        /// <summary>
        /// 想发送的内容
        /// </summary>
        public string SendText
        {
            get => sendText;
            set => RaisePropertyChanged(ref sendText, value);
        }
        /// <summary>
        /// 是否存在选中消息
        /// </summary>
        public bool HasLocMsg
        {
            get => hasLocMsg;
            set
            {
                RaisePropertyChanged(ref hasLocMsg, value);
                HasLocMsgChanged?.Invoke();
            }
        }

        /// <summary>
        /// 消息详细的详细信息
        /// </summary>
        public string MsgDetailText
        {
            get => msgDetailText;
            set => RaisePropertyChanged(ref msgDetailText, value);
        }

        /// <summary>
        /// 消息详细是否能翻下一页
        /// </summary>
        public bool MsgDetailCanGoNext
        {
            get => msgDetailCanGoNext;
            set => RaisePropertyChanged(ref msgDetailCanGoNext, value);
        }
        /// <summary>
        /// 消息详细是否能翻上一页
        /// </summary>
        public bool MsgDetailCanGoPrev
        {
            get => msgDetailCanGoPrev;
            set => RaisePropertyChanged(ref msgDetailCanGoPrev, value);
        }
        /// <summary>
        /// 当前消息详细的页数
        /// </summary>
        public int LocMsgDetailPage
        {
            get => locMsgDetailPage;
            set => RaisePropertyChanged(ref locMsgDetailPage, value);
        }

        /// <summary>
        /// 当前选中的会话的详细总数
        /// </summary>
        public int MsgDetailCount
        {
            get => msgDetailCount;
            set => RaisePropertyChanged(ref msgDetailCount, value);
        }

        /// <summary>
        /// 当前选中的会话
        /// </summary>
        public Msg LocMsg
        {
            get => locMsg;
            set
            {
                HasLocMsg = value != null;
                if (value!=locMsg)
                {
                    RaisePropertyChanged(ref locMsg, value);
                    LoadMsg();
                }
                MsgTitle = HasLocMsg ? LocMsg.TargetName : string.Empty;
            }
        }

        /// <summary>
        /// 消息表示数据的详细
        /// </summary>
        public string MsgText
        {
            get => msgText;
            set => RaisePropertyChanged(ref msgText, value);
        }

        /// <summary>
        /// 会话是否能翻下一页
        /// </summary>
        public bool MsgCanGoNext
        {
            get => msgCanGoNext;
            set => RaisePropertyChanged(ref msgCanGoNext, value);
        }

        /// <summary>
        /// 会话是否能翻前一页
        /// </summary>
        public bool MsgCanGoPrev
        {
            get => msgCanGoPrev;
            set => RaisePropertyChanged(ref msgCanGoPrev, value);
        }

        /// <summary>
        /// 会话总数
        /// </summary>
        public int MsgCount
        {
            get => msgCount;
            set => RaisePropertyChanged(ref msgCount, value);
        }

        /// <summary>
        /// 当前会话第几页
        /// </summary>
        public int LocMsgPage
        {
            get => locMsgPage;
            set => RaisePropertyChanged(ref locMsgPage, value);
        }
        /// <summary>
        /// 会话栏宽度
        /// </summary>
        public int PreWidth
        {
            get => preWidth;
            set => RaisePropertyChanged(ref preWidth, value);
        }
        /// <summary>
        /// 会话集合
        /// </summary>
        public ObservableCollection<Msg> Msgs { get;  }
        /// <summary>
        /// 当前选中的会话的详细集合
        /// </summary>
        public ObservableCollection<MsgDetail> MsgDetails { get; }
        /// <summary>
        /// 朋友集合
        /// </summary>
        public ObservableCollection<RelFriend> Friends { get;  }
        /// <summary>
        /// 搜索出来的朋友集合
        /// </summary>
        public ObservableCollection<RelFriend> SearchedFriends { get; }
        /// <summary>
        /// 申请朋友的列表
        /// </summary>
        public ObservableCollection<RelFriend> ApplyFriends { get; }
        /// <summary>
        /// 是否存在当前会话改变了
        /// </summary>
        public event Action HasLocMsgChanged;

        public RelayCommand SendCommand { get; }
        public RelayCommand UpdateMsgCommand { get; }
        public RelayCommand LoadMsgCommand { get;  }
        public RelayCommand DeleteMsgCommand { get; }
        public RelayCommand NextMsgPageCommand { get; }
        public RelayCommand PrevMsgPageCommand { get; }
        public RelayCommand NextMsgDetailPageCommand { get; }
        public RelayCommand PrevMsgDetailPageCommand { get; }
        public RelayCommand UpdateFriendCommand { get; }
        public RelayCommand ApplyFriendCommand { get; }
        public RelayCommand ApplyPagePrevCommand { get; }
        public RelayCommand ApplyPageNextCommand { get; }
        public async void BeginMsg(string fid)
        {
            if (EnsureLogin())
            {
                var mpm = await apiManager.CreatMsgAsync(fid);
                if (!mpm.Reponse.IsSucceed||mpm.Data)
                {
                    ShowMsg("BeginMsgErr", true);
                    return;
                }
                ShowMsg("BeginMsgOk", true);
            }
        }
        public async void AcceptFriendApply(uint fid)//这个应该不能用命令吧
        {
            if (EnsureLogin())
            {
                var fpm = await apiManager.AcceptFriendApplyAsync(fid);
                if (!fpm.Reponse.IsSucceed||fpm.Data)
                {
                    ShowMsg("ApplyAcceptErr", true);
                    return;
                }
                ShowMsg("ApplyAcceptOk", true);
                UpdateFriend();
                UpdateApplyFriend();
            }
        }
        public void ApplyPagePrev()
        {
            if (LocApplyPage>0)
            {
                LocApplyPage--;
                UpdateApplyFriend();
            }
        }
        public void ApplyPageNext()
        {
            LocApplyPage++;
            UpdateApplyFriend();
        }
        public async void UpdateApplyFriend()
        {
            if (EnsureLogin())
            {
                var fpm = await apiManager.GetApplyFriendsAsync(LocApplyPage * PreApplyCount, PreApplyCount);
                if (!fpm.Reponse.IsSucceed)
                {
                    ShowMsg("FlushApplyFriendErr", true);
                    return;
                }
                if (fpm.Data?.Datas!=null)
                {
                    ApplyFriends.Clear();
                    foreach (var item in fpm.Data.Datas)
                    {
                        ApplyFriends.Add(item);
                    }
                    ApplyCount = fpm.Data.Total;
                    HasFriendApply = ApplyCount > 0;
                    ApplyHasPrev = LocApplyPage > 0;
                    ApplyHasNext = (LocApplyPage + 1) * PreApplyCount < ApplyCount;
                    ApplyDetalText = $"{LocApplyPage}/{ApplyCount/PreApplyCount})";
                }
            }
        }
        public async void SearchFriend()
        {
            if (!string.IsNullOrEmpty(SearchUserText)&& EnsureLogin())
            {
                UpdateFriend();
                SearchedFriends.Clear();
                var fs = await apiManager.SearchUserAsync(SearchUserText, 0, 10);
                if (!fs.Reponse.IsSucceed)
                {
                    return;
                }
                if (fs.Data.Datas!=null)
                {
                    foreach (var item in fs.Data.Datas)
                    {
                        SearchedFriends.Add(item);
                    }
                    
                    SearchNotFriend = !Friends.Any(f => f.TargetName == SearchUserText);
                }

            }
        }
        //申请好友
        public async void ApplyFriend()
        {
            if (EnsureLogin())
            {
                try
                {
                    var fpm = await apiManager.GetUserByNameAsync(SearchUserText);
                    if (!fpm.Reponse.IsSucceed)
                    {
                        throw new Exception();
                    }
                    var res = await apiManager.ApplyFriendAsync(fpm.Data.Id);
                    if (!res.Reponse.IsSucceed||!res.Data)
                    {
                        throw new Exception();
                    }
                    ShowMsg("ApplyFriendOk", true);
                }
                catch (Exception)
                {
                    ShowMsg("ApplyFriendErr", true);
                }
            }
        }
        public async void UpdateFriend()
        {
            if (EnsureLogin())
            {
                UpdateMsg();
                var fpm = await apiManager.GetFriendsAsync(0, 999);
                if (!fpm.Reponse.IsSucceed)
                {
                    ShowMsg("UpdateFirendErr", true);
                    return;
                }
                if (fpm.Data!=null)
                {
                    Friends.Clear();
                    foreach (var item in fpm.Data.Datas)
                    {
                        if (!ShowFriendWithoutHasMsg || !Msgs.Any(m => m.Target == item.Target))
                        {
                            var user = await apiManager.GetUserByIdAsync(item.Target);
                            item.TargetName = user.Data?.UserName;
                            Friends.Add(item);
                        }
                    }
                }
            }
        }
        public void NextMsgDetailPage()
        {
            LocMsgDetailPage++;
            LoadMsg();
        }
        public void PrevMsgDetailPage()
        {
            LocMsgDetailPage--;
            LoadMsg();
        }
        public void NextMsgPage()
        {
            LocMsgPage++;
            UpdateMsg();
        }
        public void PrevMsgPage()
        {
            LocMsgPage--;
            UpdateMsg();
        }
        public async void Send()
        {
            //TEST：
            //var bpm = await apiManager.SendMsgAsync(new MsgSendModel(0, SendText, SendText));
            if (EnsureLogin())
            {
                if (LocMsg==null)
                {
                    return;
                }
                if (string.IsNullOrEmpty(SendText))
                {
                    ShowMsg("NonSendText", true);
                    return;
                }
                if (SendText.Length < 7) 
                {
                    ShowMsg("SendMinErr", true);
                    return;
                }
                var bpm = await apiManager.SendMsgAsync(new MsgSendModel(LocMsg.Id, SendText, SendText));
                if (!bpm.Reponse.IsSucceed)
                {
                    ShowMsg("SendMsgErr", true);
                    return;
                }
                LoadMsg();
                SendText = string.Empty;
            }
            
        }
        public async void DeleteMsg()
        {
            if (EnsureLogin())
            {
                if (LocMsg == null)
                {
                    ShowMsg("SelectMsgNull", true);
                    return;
                }
                var dpm = await apiManager.DeleteMsgAsync(LocMsg.Id);
                if (!dpm.Reponse.IsSucceed)
                {
                    ShowMsg("DelMsgErr",true);
                    return;
                }
                LocMsg = null;
                MsgDetails.Clear();
            }
            
        }
        
        //加载当前选中的会话的详细
        public async void LoadMsg()
        {
            if (LocMsg!=null)
            {
                if (EnsureLogin())
                {
                    var mpm = await apiManager.GetMsgDetailAsync(LocMsg.Id, LocMsgDetailPage * PreMsgDetailCount, PreMsgDetailCount, IsDescForMsgDetail);
                    if (!mpm.Reponse.IsSucceed)
                    {
                        ShowMsg(stringResource.GetString("FlushMsgDetailErr"));
                        return;
                    }
                    MsgDetails.Clear();
                    if (mpm.Data?.Datas!=null)
                    {
                        foreach (var item in mpm.Data.Datas)
                        {
                            item.UserName = LocMsg.TargetName;
                            MsgDetails.Add(item);
                        }
                        MsgDetailCount = mpm.Data.Total;
                        MsgDetailCanGoPrev = LocMsgDetailPage > 0;
                        MsgDetailCanGoNext = (LocMsgDetailPage + 1) * PreMsgDetailCount < MsgDetailCount;
                        MsgDetailText = $"{LocMsg.TargetName}({LocMsgDetailPage}/{(MsgDetailCount / PreMsgDetailCount) })";

                    }
                }
                
            }
        }
        public async void UpdateMsg()
        {
            if (EnsureLogin())
            {
                var mpm = await apiManager.GetMsgAsync(LocMsgPage * PreMsgCount, PreMsgCount);
                if (!mpm.Reponse.IsSucceed)
                {
                    ShowMsg(stringResource.GetString("FlushMsgErr"));
                    return;
                }
                Msgs.Clear();
                if (mpm.Data!=null)
                {
                    foreach (var item in mpm.Data.Datas)
                    {
                        var rpm = await apiManager.GetUserByIdAsync(item.Target);
                        if (rpm.Reponse.IsSucceed)
                        {
                            item.TargetName = rpm.Data.UserName;
                        }
                        Msgs.Add(item);
                    }
                    MsgCount = mpm.Data.Total;
                    MsgCanGoPrev = LocMsgPage > 0;
                    MsgCanGoNext = (LocMsgPage + 1) * PreMsgCount < MsgCount;
                    MsgText = $"{LocMsgPage + 1}/{(MsgCount / PreMsgCount) + 1}";
                }
                
            }
            
        }
        private bool EnsureLogin()
        {
            if (!apiManager.HasAccountToken)
            {
                ShowMsg(stringResource.GetString("NonLogin"));
                return false;
            }
            return true;
        }
        private void ShowMsg(string msg,bool autoMsg=false)
        {
            if (autoMsg)
            {
                msg = stringResource.GetString(msg);
            }
            Message.Send<MainViewModel>(MainViewModel.ShowMsgKey, new MsgShowArgs(1000, msg));
        }
    }
}