using EntityAbstract.Core.ViewModels;
using EntityAbstract.Uwp.Helpers;
using EntityAbstract.Uwp.ViewModels;
using Mvvmlighting.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ContentView : Page
    {
        private ContentViewModel ViewModel => (ContentViewModel)DataContext;
        public ContentView()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ((ContentNavHelper)ViewModelLocatorBase.ContentNavHelper).Border = DetailBorder;
            var appres = ViewModelLocatorBase.StringResources;
            ViewModel.TipText = appres.GetString("TipFlush");
            ViewModel.FlushText = appres.GetString("FlushText");
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName==nameof(ContentViewModel.LocalContent))
            {
                LvContents.SelectedItem = ViewModel.LocalContent;
            }
        }

        private ContentDetailBrowser DetailView;
        private void LvContents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e!=null &&sender is ListView lv&&lv.SelectedItem is SharedEA.Core.DbModel.DbModels.Content c)
            {
                ViewModel.LocalContent = c;
                //进入详细
                if (DetailView == null)
                {
                    DetailView = new ContentDetailBrowser();
                    DetailBorder.Child = DetailView;
                }
                DetailView.ViewModel.LoadContent(c);
            }
        }

        private void LvContents_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (sender is FrameworkElement fe)
            {
                var p = e.GetCurrentPoint(fe);
                if (p.Properties.IsLeftButtonPressed)
                {
                    var fy = FlyoutBase.GetAttachedFlyout(fe);
                    if (fy!=null)
                    {
                        FlyoutBase.ShowAttachedFlyout(fe);
                    }
                }
            }
        }

        private void LvContents_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.Y > 20) 
            {
                ViewModel.UpdateContent();
            }
        }
    }
}
