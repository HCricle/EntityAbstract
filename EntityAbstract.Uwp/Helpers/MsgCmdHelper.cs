using EntityAbstract.Core.Args;
using EntityAbstract.Core.Helpers;
using EntityAbstract.Uwp.ViewModels;
using Mvvmlighting.Ioc;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace EntityAbstract.Uwp.Helpers
{
    public class MsgCmdHelper : MsgCmdHelperBase<ListViewItem>
    {
        private int imgId;
        private int textId;
        private SolidColorBrush brush = new SolidColorBrush(Colors.Red);
        private AppResources stringResources;
        public int TextId => textId;
        public int ImgId => imgId;
        /// <summary>
        /// 是否开启解析错误显示解析错误的文本框
        /// </summary>
        public bool EnableErrText { get; set; }
        public MsgCmdHelper()
        {
            stringResources = SimpleIoc.Inst.GetInstance<AppResources>(ViewModelLocator.StringReourceKey);
            CreateGroups();
        }
        public event Action<MsgCmdParseArgs<ListViewItem>,Exception> ParseErr;
        protected override ListViewItem GetUi()
        {
            return new ListViewItem();
        }
        /// <summary>
        /// 这个会根据你给出的消息，按照命令位置插入
        /// </summary>
        /// <param name="msgDetail"></param>
        /// <returns></returns>
        public ListViewItem[] BeginParseWithString(MsgDetail msgDetail)
        {
            var texts = msgDetail.Content;
            var st = new List<string>();
            //分块
            for (int k = 0; k < msgDetail.SerCmds.Count; k++)
            {
                var cmd = msgDetail.SerCmds[k];
                var t = texts.Substring(0, cmd.InPos);
                texts = texts.Substring(cmd.InPos, texts.Count() - cmd.InPos);
                st.Add(t);
            }
            st.Add(texts);
            var res = new ListViewItem[msgDetail.SerCmds.Count+st.Count];
            res[0] = Parse(new MsgCmd(TextId, 0, new List<string> { st[0] }));
            //显示
            for (int i = 1; i < st.Count; i++)
            {
                res[i] = Parse(msgDetail.SerCmds[i - 1]);
                res[i+1]=Parse(new MsgCmd(TextId, 0, new List<string> { st[i] }));
            }
            return res;

        }
        private void CreateGroups()
        {
            CreateImgParseGroup();
            CreateTextParseGroup();
        }
        private void CreateTextParseGroup()
        {
            textId = CreateGroup(new MsgCmdParseEvent<ListViewItem>(args =>
              {
                  try
                  {
                      //参数 文字,字号，颜色(R/G/B),
                      var tb = new TextBlock();
                      var param = args.ParseObject.Params;
                      if (param!=null)
                      {
                          tb.Text = param[0];
                          if (IsStrColHasAndNotEmpty(param, 1))
                          {
                              tb.FontSize = double.Parse(param[1]);
                          }
                          if (IsStrColHasAndNotEmpty(param, 2))
                          {
                              tb.Foreground = new SolidColorBrush(ParseColor(param[2]));
                          }
                          args.UiElement.Content = tb;
                      }
                  }
                  catch (Exception ex)
                  {
                      HandleException(args,ex);
                  }
                  return args.UiElement;
              }));
        }
        private void CreateImgParseGroup()
        {
            imgId = base.CreateGroup(new MsgCmdParseEvent<ListViewItem>(args =>
            {
                try
                {

                    //参数 uri,宽，高，透明度
                    var img = new Image();
                    var param = args.ParseObject.Params;
                    if (param!=null)
                    {
                        img.Source = new BitmapImage(new Uri(param[0]));
                        if (IsStrColHasAndNotEmpty(param, 1))
                        {
                            img.Width = int.Parse(param[1]);
                        }
                        if (IsStrColHasAndNotEmpty(param, 2))
                        {
                            img.Height = int.Parse(param[2]);
                        }
                        if (IsStrColHasAndNotEmpty(param, 3))
                        {
                            img.Opacity = double.Parse(param[3]);
                        }
                        args.UiElement.Content = img;
                    }
                    
                }
                catch (Exception ex)
                {
                    HandleException(args,ex);
                }
                return args.UiElement;
            }));

        }
        private void HandleException(MsgCmdParseArgs<ListViewItem> args,Exception ex)
        {
            if (EnableErrText)
            {
                args.UiElement.Content = new TextBlock()
                {
                    Text = stringResources.GetString("UnableLoadImg"),
                    Foreground = brush,
                    FontSize = 14
                };
            }
            ParseErr?.Invoke(args, ex);
        }
        private Color ParseColor(string value)
        {
            var c = value.Split('/');
            if (c.Count()!=3)
            {
                return Colors.Black;
            }
            var r = byte.Parse(c.ElementAt(0));
            var g = byte.Parse(c.ElementAt(1));
            var b = byte.Parse(c.ElementAt(2));
            return Color.FromArgb(255, r, g, b);
        }
        private bool IsStrColHasAndNotEmpty(List<string> vs,int count)
        {
            return vs.Count > count && vs[count].Trim() != string.Empty;
        }
    }
}
