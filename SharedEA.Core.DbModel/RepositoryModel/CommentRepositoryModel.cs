using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.RepositoryModel
{
    public class CommentRepositoryModel : ArrayRepository<RelComment>
    {
        public CommentRepositoryModel()
        {
        }

        public CommentRepositoryModel(int totalCount, int skip, int take, params RelComment[] ts) : base(totalCount, skip, take, ts)
        {
        }
    }
}
