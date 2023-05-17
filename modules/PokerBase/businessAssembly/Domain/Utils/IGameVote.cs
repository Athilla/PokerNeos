using System.Threading.Tasks;

namespace PokerNeos.PokerBase.Domain.Utils
{
    public interface IGameVote
    {
        Task AddGameVoteAsync(int gameId, int userId, int itemId, string vote);
    }
}