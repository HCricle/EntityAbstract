using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Mvvmlighting.Objects.Threading
{
    public abstract class DispatcherObject
    {
        protected DispatcherObject()
        {
        }
        /// <summary>
        /// 当前线程
        /// </summary>
        //public Thread CurrentThread => Thread.CurrentThread;
    }
}
