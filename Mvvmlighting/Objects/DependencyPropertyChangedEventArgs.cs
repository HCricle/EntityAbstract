using System;
using System.Collections.Generic;
using System.Text;

namespace Mvvmlighting.Objects
{
    public class DependencyPropertyChangedEventArgs
    {
        public DependencyPropertyChangedEventArgs(DependencyProperty property, object oldValue, object newValue)
        {
            Property = property;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public DependencyProperty Property { get;  }
        public object OldValue { get;  }
        public object NewValue { get;  }
    }
}
