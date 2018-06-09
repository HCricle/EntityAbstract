using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Server.Manager
{
    public class DefaultServiceManager : ServiceManager
    {
        private static DefaultServiceManager inst;
        public static DefaultServiceManager GetInstance()
        {
            return inst ?? (inst = new DefaultServiceManager());
        }
        private DefaultServiceManager()
        {

        }
    }
}
