using EntityAbstract.Core.Helpers;
using EntityAbstract.Core.ViewModels;
using EntityAbstract.Uwp.Helpers;
using Mvvmlighting.Attributes;
using Mvvmlighting.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace EntityAbstract.Uwp.ViewModels
{

    public class ViewModelLocator: ViewModelLocatorBase
    {

        private static readonly string AccountSettingName = "AccountSetting";
        public override void InitPlatformService()
        {
            SimpleIoc.Inst.RegisterClass<SettingHelper>(AccountSettingKey, AccountSettingName, ApplicationDataCreateDisposition.Always);
            SimpleIoc.Inst.RegisterClass<AppResources>(StringReourceKey);
            SimpleIoc.Inst.RegisterClass<NavigationHelper>(NavigationHelperBase.NavigationHelperKey);
            SimpleIoc.Inst.RegisterClass<ContentNavHelper>(ContentNavHelperBase.ContentNavHelperKey);
            SimpleIoc.Inst.RegisterClass<MsgCmdHelper>(MsgCmdHelper.Key);
            
        }
    }
}
