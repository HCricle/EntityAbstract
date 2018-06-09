using EntityAbstract.Core.ViewModels;
using EntityAbstract.Uwp.ViewModels;
using SharedEA.Core.DbModel.DbModels;
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

namespace EntityAbstract.Uwp.Views
{
    /// <summary>
    /// 内容详细页
    /// </summary>
    public sealed partial class ContentDetailBrowser : UserControl
    {
        public ContentDetailViewModel ViewModel => (ContentDetailViewModel)DataContext;
        public ContentDetailBrowser()
        {
            this.InitializeComponent();
            //TODO:把评论移到另一个页面，接收可运行命令
        }

        private void ViewModel_ContentUriChanged()
        {
            if (ViewModel.ContentUri!=null)
            {
                Wv.Source = ViewModel.ContentUri;
            }
            
        }

        public void BrowserContent(Content c)
        {
        }

        private void Wv_UnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
        {
            
        }

        private void Wv_ScriptNotify(object sender, NotifyEventArgs e)
        {

        }

        private void Wv_UnsafeContentWarningDisplaying(WebView sender, object args)
        {

        }

        private void Wv_UnsupportedUriSchemeIdentified(WebView sender, WebViewUnsupportedUriSchemeIdentifiedEventArgs args)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.GoBackToContentAction += ViewModel_GoBackToContentAction;
            ViewModel.NavToCommentAcion += ViewModel_NavToCommentAcion;
            ViewModel.ContentUriChanged += ViewModel_ContentUriChanged;
        }

        private void ViewModel_NavToCommentAcion()
        {
            ViewModelLocatorBase.ContentNavHelper.SetContent(new CommentView());
        }

        private void ViewModel_GoBackToContentAction()
        {
            ViewModelLocatorBase.ContentNavHelper.SetContent(new ContentDetailBrowser());
        }

        private void Wv_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            Pbr.Visibility =  Visibility.Visible;
        }

        private void Wv_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            Pbr.Visibility = Visibility.Collapsed;
        }
    }
}
