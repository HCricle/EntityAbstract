using EntityAbstract.Core.Helpers;
using Mvvmlighting.Attributes;
using Mvvmlighting.Ioc;
using Shared.Core.Api.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityAbstract.Core.ViewModels
{
    [NeedInstInIoc(typeof(ISettingHelper))]
    [NeedInstInIoc(typeof(IStringResource))]
    [NeedInstInIoc(typeof(NavigationHelperBase))]
    [NeedInstInIoc(typeof(ContentNavHelperBase))]
    [NeedInstInIoc(typeof(MsgCmdHelperBase<>))]
    public abstract class ViewModelLocatorBase
    {
        [NeedKeyInIoc]
        public static readonly int ViewModelLocatorKey = 0x00233;
        [NeedKeyInIoc]
        public static readonly int ApiManagerKey = 0x00123;
        [NeedKeyInIoc]
        public static readonly int AccountSettingKey = 0x10653;
        [NeedKeyInIoc]
        public static readonly int StringReourceKey = 0x10555;
        public static IStringResource StringResources => SimpleIoc.Inst.GetInstance<IStringResource>(StringReourceKey);
        public static NavigationHelperBase NavigationHelper => SimpleIoc.Inst.GetInstance<NavigationHelperBase>(NavigationHelperBase.NavigationHelperKey);
        public static ContentNavHelperBase ContentNavHelper => SimpleIoc.Inst.GetInstance<ContentNavHelperBase>(ContentNavHelperBase.ContentNavHelperKey);
        public static ISettingHelper SettingHelper => SimpleIoc.Inst.GetInstance<ISettingHelper>(StringReourceKey);
        public ViewModelLocatorBase()
        {
            SimpleIoc.Inst.RegisterClass<WebApiManager>(ApiManagerKey);
            InitPlatformService();
            SimpleIoc.Inst.AddInstance(ViewModelLocatorKey, this);

            MainViewModel = SimpleIoc.Inst.RegisterClass<MainViewModel>(MainViewModel.MainViewModelKey);
            HomeViewModel = SimpleIoc.Inst.RegisterClass<HomeViewModel>(HomeViewModel.HomeViewModelKey);
            ContentViewModel = SimpleIoc.Inst.RegisterClass<ContentViewModel>(ContentViewModel.ContentViewModelKey);
            MsgViewModel = SimpleIoc.Inst.RegisterClass<MsgViewModel>(MsgViewModel.MsgViewModelKey);
            SendContentViewModel = SimpleIoc.Inst.RegisterClass<SendContentViewModel>(SendContentViewModel.SendContentViewModelKey);
            ContentDetailViewModel = SimpleIoc.Inst.RegisterClass<ContentDetailViewModel>(ContentDetailViewModel.ContentDetailViewModelKey);

            IocHelper.EnsureIocKey(this);
            IocHelper.EnsureIocInst(this);//确保服务已经装载
        }
        /// <summary>
        /// 初始化平台服务
        /// </summary>
        public abstract void InitPlatformService();
        public MainViewModel MainViewModel { get;  }
        public HomeViewModel HomeViewModel { get; }
        public ContentViewModel ContentViewModel { get; }
        public ContentDetailViewModel ContentDetailViewModel { get;  }
        public MsgViewModel MsgViewModel { get; }
        public SendContentViewModel SendContentViewModel { get; }
    }
}
