using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace EntityAbstract.Uwp.Helpers
{
    /// <summary>
    /// 对存储的帮助,setting另外
    /// </summary>
    public static class StorageHelper
    {
        /// <summary>
        /// 判断文件是否在目录中，
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName">不包含扩展名</param>
        /// <returns></returns>
        public static async Task<bool> IsFileExistAsync(StorageFolder folder,string fileName)
        {
            foreach (var item in await folder.GetFilesAsync())
            {
                if (item.DisplayName==fileName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
