using System;
using System.Collections.Generic;
using SharedEA.Core.DbModel.DbModels;

namespace SharedEA.Data.Repositories
{
    public interface ICommentRepository
    {
        IEnumerable<Comment> GetComment(Func<Comment, bool> func, int count = -1, int skip = 0);
        IEnumerable<Comment> GetComment(int contentId, int count = -1, int skip = 0);
        IEnumerable<Comment> GetComment(string content, int count = -1, int skip = 0);
    }
}