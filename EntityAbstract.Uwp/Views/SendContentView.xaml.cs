using EntityAbstract.Core.ViewModels;
using EntityAbstract.Uwp.Helpers;
using SharedEA.Core.WebApi.JWT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EntityAbstract.Uwp.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SendContentView : Page
    {
        private static readonly string TextFormatterSettings = "txtFormatter";
        private SendContentViewModel ViewModel => (SendContentViewModel)DataContext;
        public SendContentView()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe&&fe.Tag is ApiFileModel afm)
            {
                ViewModel.RemoveFiles(afm);
            }
        }

        private async void BtnOpenFiles_Click(object sender, RoutedEventArgs e)
        {
            var fp = new FileOpenPicker();
            var f = await fp.PickMultipleFilesAsync();
            if (f != null)
            {
                var fs = f.Select(file => new ApiFileModel(file.Name, file.Path)).ToArray();
                ViewModel.AddFiles(fs);
            }
        }
    }
}
