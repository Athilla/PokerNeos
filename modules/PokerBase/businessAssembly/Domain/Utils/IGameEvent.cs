using System.Threading.Tasks;

namespace PokerNeos.PokerBase.Domain.Utils
{
    public interface IGameEvent
    {
        Task AddChangeUSEventAsync(int gameId, int userId, int itemId);
        Task AddJoinEventAsync(int gameId, int userId);
        Task AddLeaveEventAsync(int gameId, int userId);
        Task AddShowVoteEventAsync(int gameId, int userId, int itemId);
        Task AddStartEventAsync(int gameId, int userId);
        Task AddStopEventAsync(int gameId, int userId);
        Task AddVoteEventAsync(int gameId, int userId, string vote, int itemId);
    }
}