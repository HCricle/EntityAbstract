using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedEA.Core.DbModel.DbModels;

namespace SharedEA.Data.Repositories
{
    public interface IContentRepository
    {
        Task<IEnumerable<Content>> GetContentAsync(Func<Content, bool> func, int count = -1, int skip = 0, bool isAutoFill = false);
        Task<IEnumerable<Content>> GetContentAsync(int groupId, int count = -1, int skip = 0, bool isAutoFill = false);
        Task<IEnumerable<Content>> GetContentAsync(string title, bool isBlurry = false, int count = -1, int skip = 0, bool isAutoFill = false);
    }
}