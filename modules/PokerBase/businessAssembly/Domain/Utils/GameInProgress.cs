using GroupeIsa.Neos.Domain.Persistence;
using GroupeIsa.Neos.Shared.Linq;
using PokerNeos.Application.Abstractions.DataObjects;
using PokerNeos.Domain.Enums;
using PokerNeos.Domain.Persistence;
using System.Linq;
using System.Threading.Tasks;

namespace PokerNeos.PokerBase.Domain.Utils
{
    public class GameInProgress : IGameInProgress
    {
        private readonly IGameInProgressRepository _gameInProgressRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GameInProgress(IGameInProgressRepository gameInProgressRepository,
            IUnitOfWork unitOfWork)
        {
            _gameInProgressRepository = gameInProgressRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Gets the game data asynchronous.
        /// </summary>
        /// <param name="gameId">The game identifier.</param>
        /// <returns>return an GameData <see cref="GameData"/></returns>
        public async Task<GameDataDO> GetGameDataAsync(int gameId)
        {
            GameDataDO gameData = new GameDataDO();
            var game = _gameInProgressRepository.GetQuery()
                .Include(e => e.UserList)
                .Include(e => e.EventList)
                .FirstOrDefault(g => g.GameId == gameId);
            if (game == null)
            {
                var gip = _gameInProgressRepository.New();
                gip.GameId = gameId;
                gip.CurrentItem = null;
                _gameInProgressRepository.Add(gip);
                await _unitOfWork.SaveAsync();
                game = gip;
            }
            gameData.GameInProgress = game;
            gameData.IsSuccess = true;
            
            return gameData;
        }

        public async Task ShowVoteAsync(int gameId)
        {
            var game = await _gameInProgressRepository.FindAsync(gameId);
            if (game != null)
            {
                game.ShowVote = true;
                await _unitOfWork.SaveAsync();
            }
        }
        public async Task ChangeItemAsync(int gameId, int itemId)
        {
            var game = await _gameInProgressRepository.FindAsync(gameId);
            if (game != null)
            {
                game.ShowVote = false;
                game.CurrentItem = itemId;
                await _unitOfWork.SaveAsync();
            }
        }
        public async Task ChangeStateAsync(int gameId, GameState state)
        {
            var game = await _gameInProgressRepository.FindAsync(gameId);
            if (game != null)
            {
                game.State = state;
                await _unitOfWork.SaveAsync();
            }
        }

    }
}
