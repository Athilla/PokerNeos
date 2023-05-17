using System.Threading.Tasks;

namespace PokerNeos.PokerBase.Domain.Utils
{
    public interface IUserInGame
    {
        Task AddUserInGameAsync(int gameId, int userId);
        Task RemoveUserInGameAsync(int gameId, int userId);
    }
}