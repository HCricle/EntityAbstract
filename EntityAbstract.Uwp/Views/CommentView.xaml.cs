using EntityAbstract.Core.ViewModels;
using EntityAbstract.Uwp.ViewModels;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.RepositoryModel;
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
    public sealed partial class CommentView : Page
    {
        public CommentView()
        {
            this.InitializeComponent();
        }
        private ContentDetailViewModel ViewModel => (ContentDetailViewModel)DataContext;

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe)
            {
                if (fe.Tag is RelComment com)
                {
                    ViewModel.DeleteComment(com.Id);
                }
            }
        }
    }
}
