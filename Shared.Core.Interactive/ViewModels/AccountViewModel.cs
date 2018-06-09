using MvvmLighting.Ioc;
using MvvmLighting.Mvvm;
using Shared.Core.Api.Models;
using Shared.Core.Api.WebApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Core.Interactive.ViewModels
{
    /// <summary>
    /// 账号
    /// </summary>
    public class AccountViewModel : ViewModelBase
    {
        private WebApiManager webApiManager;
        private string userName;
        private string pwd;
        private string email;
        private string errorMsg;
        public AccountViewModel()
        {
            webApiManager = SimpleIoc.Inst.GetInstance<WebApiManager>(ViewModelLocator.WebApiManagerKey);
            LoginCommand = new RelayCommand(Login);
            LogOffCommand = new RelayCommand(LogOff);
            RegisterCommand = new RelayCommand(Register);
#if DEBUG
            UserName = "HCricle";
            pwd = "Asdfg123456";
#endif
        }
        /// <summary>
        /// 是否已经登陆，其实就是查询是否存在token
        /// </summary>
        public bool IsLogin => webApiManager.HasAccountToken;
        #region Events
        public event Action<ApiReponse> Logined;
        public event Action<ApiReponse> Registered;
        public event Action<string> ErrorMsgChanged;
        #endregion
        #region Can_Bind_Properties
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg
        {
            get => errorMsg;
            set => RaisePropertyChanged(ref errorMsg, value);
        }

        /// <summary>
        /// 输入的用户名
        /// </summary>
        public string UserName
        {
            get => userName;
            set => RaisePropertyChanged(ref userName, value);
        }
        /// <summary>
        /// 输入的密码
        /// </summary>
        public string Pwd
        {
            get => pwd;
            set => RaisePropertyChanged(ref pwd, value);
        }
        /// <summary>
        /// 输入的邮箱地址
        /// </summary>
        public string Email
        {
            get => email;
            set => RaisePropertyChanged(ref email, value);
        }

        #endregion
        /// <summary>
        /// 用户名或密码是否为空
        /// </summary>
        public bool IsNameOrPwdNon => string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Pwd);
        /// <summary>
        /// 登陆命令
        /// </summary>
        public RelayCommand LoginCommand { get; }
        public RelayCommand LogOffCommand { get; }
        public RelayCommand RegisterCommand { get; }
        public async void Login()
        {
            ErrorMsg = string.Empty;
            var res =await webApiManager.LoginAsync(new LoginModel(UserName, Pwd));
            if (res!=null)
            {
                try
                {
                    ErrorMsg = await res.GetContentStreamAsync();
                }
                catch (Exception ex)
                {

                    ErrorMsg = ex.Message;
                }
            }
            ErrorMsgChanged?.Invoke(ErrorMsg);
            Logined?.Invoke(res);
        }
        public async void LogOff()
        {
            var res=await webApiManager.LogOffAsync();
            Logined?.Invoke(res);
        }
        public async void Register()
        {
            var res = await webApiManager.RegisterAsync(new RegisterModel(UserName, Email, Pwd));
            Registered?.Invoke(res);
        }
    }
}
