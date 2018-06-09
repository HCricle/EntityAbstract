using EntityAbstract.Core.Models;
using EntityAbstract.Core.ViewModels;
using EntityAbstract.Uwp.Helpers;
using EntityAbstract.Uwp.ViewModels;
using EntityAbstract.Uwp.Views;
using Mvvmlighting.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace EntityAbstract.Uwp
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;
        private Dictionary<int, Type> views = new Dictionary<int, Type>();
        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ((NavigationHelper)ViewModelLocatorBase.NavigationHelper).Frame = FrameContent;
            ViewModel.Title = ViewModelLocatorBase.StringResources.GetString("NvTitle");
            //注册消息
            ViewModel.NeedShowMsg += ViewModel_NeedShowMsg;
            //加入可导航列表
            views.Add(MainViewModel.HomePageId, typeof(HomeView));
            views.Add(MainViewModel.ContentPageId, typeof(ContentView));
            views.Add(MainViewModel.MsgPageId, typeof(MsgView));
            views.Add(MainViewModel.SendContentPageId, typeof(SendContentView));
            //注册可转页
            ViewModel.CurrentViewItem = ViewModel.ViewItems.Single(i => i.Id == MainViewModel.ContentPageId);
        }
        private async void ViewModel_NeedShowMsg(MsgShowArgs obj)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async() =>
            {
                TxbMsg.Text = string.Empty;
                MsgView.Child = null;
                TxbMsg.Visibility =MsgView.Visibility= Visibility.Collapsed;
                if (obj.Type == MsgDisplayTypes.Text)
                {
                    TxbMsg.Text = obj.Msg;
                    TxbMsg.Visibility = Visibility.Visible;
                }
                MsgContrain.Visibility = Visibility.Visible;
                await Task.Delay(obj.InvialiTime);
                MsgContrain.Visibility = Visibility.Collapsed;
            });
        }

        private void Nv_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem!=null &&args.SelectedItem is ViewItem vi)
            {
                FrameContent.Navigate(views[vi.Id],vi.ParamGetter?.Invoke());
            }
        }
    }
}
