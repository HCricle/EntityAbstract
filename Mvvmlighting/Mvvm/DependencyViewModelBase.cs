using Mvvmlighting.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mvvmlighting.Mvvm
{
    /// <summary>
    /// 依赖视图模型基类
    /// </summary>
    public class DependencyViewModelBase : DependencyObject, IViewModelBase,INotifyPropertyChanged,IMessageUsable
    {
        public DependencyViewModelBase()
        {
            Message = new Message(this);
        }

        public DependencyViewModelBase(Message message)
        {
            Message = message;
        }

        public Message Message { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知属性改变了，如果是依赖属性，会自动通知的
        /// 如果使用wpf或uwp就需要调用此方法更新界面
        /// </summary>
        /// <param name="propName"></param>
        public void RaisePropertyChanged([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propName));
        }

        public void RaisePropertyChanged<T>(ref T prop, T value, [CallerMemberName] string propName = "")
        {
            prop = value;
            RaisePropertyChanged(propName);
        }
    }
}
