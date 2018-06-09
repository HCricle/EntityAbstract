using EntityAbstract.Core.Helpers;
using EntityAbstract.Core.ViewModels;
using EntityAbstract.Uwp.Helpers;
using Mvvmlighting.Ioc;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.RepositoryModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace EntityAbstract.Uwp.Views
{
    public sealed partial class MsgView : Page
    {
        private MsgViewModel ViewModel => (MsgViewModel)DataContext;
        private IStringResource stringResource;
        private MsgCmdHelper msgCmdHelper;
        public MsgView()
        {
            this.InitializeComponent();
            stringResource=ViewModelLocatorBase.StringResources;
            msgCmdHelper = SimpleIoc.Inst.GetInstance<MsgCmdHelper>(MsgCmdHelper.Key);
        }

        private void LvMsgs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView lv&&lv.SelectedItem is Msg msg)
            {
                ViewModel.LocMsg = msg;
            }
            if (ViewModel.HasLocMsg)
            {
                ViewModel.LoadMsg();
            }
        }

        private void MsgGrid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ShowFlyout(sender, (fe) => e.GetCurrentPoint(fe).Properties.IsBarrelButtonPressed);
        }
        private void ShowFlyout(object sender,Func<FrameworkElement,bool> condition)
        {
            if (sender is FrameworkElement fe)
            {
                if (condition(fe))
                {
                    var fl = FlyoutBase.GetAttachedFlyout(fe);
                    if (fl != null)
                    {
                        FlyoutBase.ShowAttachedFlyout(fe);
                    }
                }
            }
        }
        private void BtnCreateMsg_Click(object sender, RoutedEventArgs e)
        {
            ShowFlyout(sender, fe => true);
        }

        private void LvFriends_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //转到会话
            if (sender is ListView lv)
            {
                if (lv.SelectedItem is Msg msg)
                {
                    ViewModel.LocMsg = msg;
                    ViewModel.LoadMsg();
                }
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem is RelFriend rf)
            {
                ViewModel.SearchUserText = rf.TargetName;
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ViewModel.SearchFriend();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe&&fe.Tag is RelFriend rf)
            {
                ViewModel.AcceptFriendApply(rf.Id);
            }
        }

        private void Flyout_Opening(object sender, object e)
        {
            ViewModel.UpdateApplyFriend();
        }

        private void LvMsgs_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (sender is ListView lv)
            {
                if (lv.SelectedItem is Msg msg)
                {
                    ViewModel.LocMsg = msg;
                    ViewModel.LoadMsg();
                }
            }

        }
        private SolidColorBrush brush = new SolidColorBrush(Colors.Red);
        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ListView lv && lv.Tag is MsgDetail md && md.SerCmds != null) 
            {
                var items=msgCmdHelper.BeginParseWithString(md);//有问题
                foreach (var item in items)
                {
                    lv.Items.Add(item);
                }
                /*
                var texts = md.Content;
                var st = new List<string>();
                for (int k = 0; k < md.SerCmds.Count; k++)
                {
                    var cmd = md.SerCmds[k];
                    var t = texts.Substring(0, cmd.InPos);
                    texts = texts.Substring(cmd.InPos, texts.Count() - cmd.InPos);
                    st.Add(t);
                }
                st.Add(texts);
                for (int i = 1; i < st.Count; i++)
                {
                    var cmd = md.SerCmds[i - 1];
                    if (cmd.Id == 0) 
                    {
                        var it = new ListViewItem();
                        try
                        {
                            var img = new Image()
                            {
                                Source = new BitmapImage(new Uri(cmd.Params.First())),
                                Stretch = Stretch.UniformToFill
                            };
                            it.Content = img;
                        }
                        catch (Exception)
                        {
                            it.Content = new TextBlock()
                            {
                                Text = stringResource.GetString("UnableLoadImg"),
                                Foreground= brush,
                                FontSize=14
                            };
                        }
                        lv.Items.Add(it);
                    }
                    lv.Items.Add(new ListViewItem() { Content = new TextBlock() { Text = st[i] } });
                }
                */
            }
            
        }
        
    }
}
