using PokerNeos.Application.Abstractions.DataObjects;
using PokerNeos.Domain.Enums;
using System.Threading.Tasks;

namespace PokerNeos.PokerBase.Domain.Utils
{
    public interface IGameInProgress
    {
        Task ChangeItemAsync(int gameId, int itemId);
        Task ChangeStateAsync(int gameId, GameState state);
        Task<GameDataDO> GetGameDataAsync(int gameId);
        Task ShowVoteAsync(int gameId);
    }
}