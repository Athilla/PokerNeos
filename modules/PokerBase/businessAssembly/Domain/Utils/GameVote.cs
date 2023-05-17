using GroupeIsa.Neos.Domain.Persistence;
using PokerNeos.Domain.Persistence;
using System.Threading.Tasks;

namespace PokerNeos.PokerBase.Domain.Utils
{
    public class GameVote : IGameVote
    {
        private readonly IGameVoteRepository _gameVoteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GameVote(IGameVoteRepository gameVoteRepository, IUnitOfWork unitOfWork)
        {
            _gameVoteRepository = gameVoteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task AddGameVoteAsync(int gameId, int userId, int itemId, string vote)
        {
            var gv = _gameVoteRepository.New();
            gv.GameId = gameId;
            gv.UserId = userId;
            gv.Vote = vote;
            gv.GameItemId = itemId;
            _gameVoteRepository.Add(gv);
            await _unitOfWork.SaveAsync();
        }
    }
}
