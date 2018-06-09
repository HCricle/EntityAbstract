using EntityAbstract.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace EntityAbstract.Uwp.Helpers
{
    public class AppResources:IStringResource
    {
        private ResourceLoader CurrentResourceLoader
        {
            get { return _loader ?? (_loader = ResourceLoader.GetForCurrentView("Resources")); }
        }

        private ResourceLoader _loader;
        private readonly Dictionary<string, string> ResourceCache = new Dictionary<string, string>();

        public string GetString(string key)
        {
            if (ResourceCache.TryGetValue(key, out string s))
            {
                return s;
            }
            else
            {

                s = CurrentResourceLoader.GetString(key);
                ResourceCache[key] = s;
                return s;
            }
        }
    }
}
