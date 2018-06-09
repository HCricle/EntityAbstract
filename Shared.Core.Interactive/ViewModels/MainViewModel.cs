using MvvmLighting.Mvvm;
using Shared.Core.Interactive.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Shared.Core.Interactive.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
        }
        #region Can_Bind_Properties

        /// <summary>
        /// 可使用的视图
        /// </summary>
        public ObservableCollection<ViewItem> UseViews { get; set; }
        /// <summary>
        /// 选项的视图
        /// </summary>
        public ObservableCollection<ViewItem> OptionViews { get; set; }
        #endregion
    }
}
