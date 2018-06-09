using System.Runtime.CompilerServices;
using Mvvmlighting.Mvvm;

namespace Mvvmlighting.Mvvm
{
    public interface IViewModelBase
    {
        void RaisePropertyChanged([CallerMemberName] string propName = "");
        void RaisePropertyChanged<T>(ref T prop, T value, [CallerMemberName] string propName = "");
    }
}