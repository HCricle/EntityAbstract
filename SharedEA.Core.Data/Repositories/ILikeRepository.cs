using SharedEA.Core.DbModel.DbModels;

namespace SharedEA.Data.Repositories
{
    public interface ILikeRepository
    {

        bool LikeForContent(string userId, uint contentId);
    }
}