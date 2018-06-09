using EntityAbstract.Core.Helpers;
using EntityAbstract.Core.Models;
using Mvvmlighting.Ioc;
using Mvvmlighting.Mvvm;
using Shared.Core.Api.Models;
using Shared.Core.Api.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityAbstract.Core.ViewModels
{
    public class HomeViewModel : MsgDpViewModelBase
    {
        public static readonly int HomeViewModelKey = 0x00333;
        private static readonly string UserNameKey = "UName";
        private static readonly string PwdKey = "Pwd";
        private WebApiManager apiManager;
        private ISettingHelper accountHelper;
        private IStringResource appResources;
        public HomeViewModel()
        {
            apiManager = SimpleIoc.Inst.GetInstance<WebApiManager>(ViewModelLocatorBase.ApiManagerKey);
            accountHelper = SimpleIoc.Inst.GetInstance<ISettingHelper>(ViewModelLocatorBase.AccountSettingKey);
            appResources = SimpleIoc.Inst.GetInstance<IStringResource>(ViewModelLocatorBase.StringReourceKey);
            UserName = accountHelper.GetValue<string>(UserNameKey);
            Pwd = accountHelper.GetValue<string>(PwdKey);
            LoginCommand = new RelayCommand(Login);
            LogOffCommand = new RelayCommand(LogOff);
            IocHelper.EnsureIocKey(this);
#if DEBUG
            UserName = "HCricle";
            Pwd = "Asdfg123456";
#endif
        }

        private string userName;
        private string pwd;
        private bool isLogin;
        private string email;
        private bool isRemerber;
        /// <summary>
        /// 是否记住密码和账号
        /// </summary>
        public bool IsRemerber
        {
            get => isRemerber;
            set => RaisePropertyChanged(ref isRemerber, value);
        }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email
        {
            get => email;
            set => RaisePropertyChanged(ref email, value);
        }

        /// <summary>
        /// 是否已经登陆了
        /// </summary>
        public bool IsLogin
        {
            get => isLogin;
            set => RaisePropertyChanged(ref isLogin, value);
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd
        {
            get => pwd;
            set => RaisePropertyChanged(ref pwd, value);
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get => userName;
            set => RaisePropertyChanged(ref userName, value);
        }
        public RelayCommand LoginCommand { get; }
        public RelayCommand LogOffCommand { get; }
        public RelayCommand RigisterCommand { get; }
        public async void Login()
        {
            if (EnsureInfo())
            {
                try
                {
                    var ap = await apiManager.LoginAsync(new LoginModel(UserName, Pwd));
                    if (!ap.IsSucceed)
                    {
                        ShowMsg(appResources.GetString("ErrLogin"));
                        return;
                    }
                    if (IsRemerber)
                    {
                        accountHelper.SetValue(UserNameKey, UserName);
                        accountHelper.SetValue(PwdKey, Pwd);
                    }
                    ShowMsg(appResources.GetString("LoginSucceed"));
                    IsLogin = true;
                }
                catch (Exception)
                {
                    ShowMsg(appResources.GetString("NonNetwork"));
                }
            }
            
        }
        public void LogOff()
        {
            if (EnsureInfo())
            {
                IsLogin = false;
            }
        }
        private bool EnsureInfo()
        {
            if (string.IsNullOrEmpty(Pwd) || string.IsNullOrEmpty(UserName))
            {
                ShowMsg(appResources.GetString("EmptyUNameOrPwd"));
                return false;
            }
            return true;
        }
        private void ShowMsg(string msg)
        {
            Message.Send<MainViewModel>(MainViewModel.ShowMsgKey, new MsgShowArgs(1000, msg));
        }
    }
}
