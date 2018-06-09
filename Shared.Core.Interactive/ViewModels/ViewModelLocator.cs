using MvvmLighting.Ioc;
using Shared.Core.Api.WebApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Core.Interactive.ViewModels
{
    public class ViewModelLocator
    {
        public static string WebApiManagerKey = "WebApiManagerKey";
        public static string ViewModelLocatorKey = "ViewModelLocatorKey";
        public ViewModelLocator()
        {
            SimpleIoc.Inst.AddInstance(ViewModelLocatorKey, this);
            SimpleIoc.Inst.RegisterClass<WebApiManager>(WebApiManagerKey);
            AccountViewModel = new AccountViewModel();
            ContentViewModel = new ContentViewModel();
            MakeContentViewModel = new MakeContentViewModel();
            MakeCommentViewModel = new MakeCommentViewModel();
            MainViewModel = new MainViewModel();
        }
        public AccountViewModel AccountViewModel { get;  }
        public ContentViewModel ContentViewModel { get;  }
        public MakeContentViewModel MakeContentViewModel { get;  }
        public MakeCommentViewModel MakeCommentViewModel { get;  }
        public MainViewModel MainViewModel { get;  }
    }
}
