using System.Threading.Tasks;
using Transversals.Business.Domain.Entities;

namespace PokerNeos.PokerBase.Domain.Utils
{
    public interface IUserInGame
    {
        Task AddUserInGameAsync(int gameId, UserAccount userAccount);
        Task RemoveUserInGameAsync(int gameId, int userId);
    }
}