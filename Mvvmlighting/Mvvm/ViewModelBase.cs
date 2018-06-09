using Mvvmlighting.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mvvmlighting.Mvvm
{
    /// <summary>
    /// ViewModel基类
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged, IViewModelBase,IMessageUsable
    {
        public ViewModelBase()
        {
            Message = new Message(this);
        }
        public ViewModelBase(Message message)
        {
            Message= message;
        }
        /// <summary>
        /// 消息
        /// </summary>
        public Message Message { get;  }

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性改变了
        /// </summary>
        /// <param name="propName">属性名</param>
        public void RaisePropertyChanged([CallerMemberName] string propName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        /// <summary>
        /// 属性改变，并且通知属性改变了
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <param name="propName"></param>
        public void RaisePropertyChanged<T>(ref T prop,T value, [CallerMemberName]string propName="")
        {
            prop = value;
            RaisePropertyChanged(propName);
        }
    }
}
