using GroupeIsa.Neos.Domain.Persistence;
using PokerNeos.Domain.Persistence;
using System.Threading.Tasks;

namespace PokerNeos.PokerBase.Domain.Utils
{
    public class GameEvent : IGameEvent
    {
        private readonly IGameEventRepository _gameEventRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GameEvent(IGameEventRepository gameEventRepository, IUnitOfWork unitOfWork)
        {
            _gameEventRepository = gameEventRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task AddJoinEventAsync(int gameId, int userId)
        {
            var gameEvent = _gameEventRepository.New();
            gameEvent.GameId = gameId;
            gameEvent.UserId = userId;
            gameEvent.Event = PokerNeos.Domain.Enums.Event.HasJoined;
            _gameEventRepository.Add(gameEvent);
            await _unitOfWork.SaveAsync();
        }

        public async Task AddLeaveEventAsync(int gameId, int userId)
        {
            var gameEvent = _gameEventRepository.New();
            gameEvent.GameId = gameId;
            gameEvent.UserId = userId;
            gameEvent.Event = PokerNeos.Domain.Enums.Event.HasLeft;
            _gameEventRepository.Add(gameEvent);
            await _unitOfWork.SaveAsync();
        }

        public async Task AddStartEventAsync(int gameId, int userId)
        {
            var gameEvent = _gameEventRepository.New();
            gameEvent.GameId = gameId;
            gameEvent.UserId = userId;
            gameEvent.Event = PokerNeos.Domain.Enums.Event.HasStarted;
            _gameEventRepository.Add(gameEvent);
            await _unitOfWork.SaveAsync();
        }

        public async Task AddStopEventAsync(int gameId, int userId)
        {
            var gameEvent = _gameEventRepository.New();
            gameEvent.GameId = gameId;
            gameEvent.UserId = userId;
            gameEvent.Event = PokerNeos.Domain.Enums.Event.HasStopped;
            _gameEventRepository.Add(gameEvent);
            await _unitOfWork.SaveAsync();

        }

        public async Task AddVoteEventAsync(int gameId, int userId, string vote, int itemId)
        {
            var gameEvent = _gameEventRepository.New();
            gameEvent.GameId = gameId;
            gameEvent.UserId = userId;
            gameEvent.Event = PokerNeos.Domain.Enums.Event.HasVoted;
            gameEvent.GameItemId = itemId;
            gameEvent.Vote = vote;
            //gameEvent.GameVote = new PokerNeos.Domain.Entities.GameVote()
            //{
            //    Vote = vote,
            //    GameItemId = itemId,
            //    GameId = gameId,
            //    UserId = userId
            //};
            _gameEventRepository.Add(gameEvent);
            await _unitOfWork.SaveAsync();
        }

        public async Task AddChangeUSEventAsync(int gameId, int userId, int itemId)
        {
            var gameEvent = _gameEventRepository.New();
            gameEvent.GameId = gameId;
            gameEvent.UserId = userId;
            gameEvent.Event = PokerNeos.Domain.Enums.Event.ChangeUS;
            gameEvent.GameItemId = itemId;
            _gameEventRepository.Add(gameEvent);
            await _unitOfWork.SaveAsync();
        }

        public async Task AddShowVoteEventAsync(int gameId, int userId, int itemId)
        {
            var gameEvent = _gameEventRepository.New();
            gameEvent.GameId = gameId;
            gameEvent.UserId = userId;
            gameEvent.Event = PokerNeos.Domain.Enums.Event.ShowVote;
            gameEvent.GameItemId = itemId;
            _gameEventRepository.Add(gameEvent);
            await _unitOfWork.SaveAsync();
        }

    }
}
