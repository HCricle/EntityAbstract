using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Mvvmlighting.Mvvm
{
    /// <summary>
    /// 命令
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private Action<object> executeMethod;
        private Func<object, bool> canExecuteMethod;
        public RelayCommand(Action execute)
        {
#if DEBUG
            CheckParams(execute);
#endif
            executeMethod = new Action<object>(o => execute());
        }
        public RelayCommand(Action<T> execute)
        {
#if DEBUG
            CheckParams(execute);
#endif
            executeMethod = new Action<object>(o=>execute((T)(o??default(T))));

        }
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
            :this(execute)
        {
#if DEBUG
            CheckParams(canExecute);
#endif
            canExecuteMethod = new Func<object, bool>(o => canExecute((T)(o ?? default(T))));
        }
#if DEBUG
        private void CheckParams(params object[] paramObjects)
        {
            var nullParams = paramObjects.Where(o => o == null);
            if (nullParams.Count() > 0) 
            {
                throw new ArgumentException("不可为空的参数，存在为空的");
            }
        }
#endif
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (canExecuteMethod==null)
            {
                return true;
            }
            return canExecuteMethod.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            executeMethod?.Invoke(parameter);
        }
    }
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute) : base(execute)
        {
        }
    }
}
