using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedEA.Core.DbModel.DbModels;

namespace SharedEA.Data.Repositories
{
    public interface IGroupRepository
    {
        Task<IEnumerable<ContentGroup>> GetGroupAsync(int count = -1, int skip = 0, bool isAutoFill = false);
        Task<IEnumerable<ContentGroup>> GetGroupAsync(bool isEnable, int count = -1, int skip = 0, bool isAutoFill = false);
        Task<IEnumerable<ContentGroup>> GetGroupAsync(Func<ContentGroup, bool> func, int count = -1, int skip = 0, bool isAutoFill = false);
        Task<IEnumerable<ContentGroup>> GetGroupAsync(string name, bool isBlurry = false, int count = -1, int skip = 0, bool isAutoFill = false);
    }
}