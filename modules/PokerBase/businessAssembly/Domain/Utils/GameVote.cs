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
            var gv = await _gameVoteRepository.FindAsync(gameId, itemId, userId);
            if (gv != null)
            {
                gv.Vote = vote;

            }
            else
            {
                var nv = _gameVoteRepository.New();
                nv.GameId = gameId;
                nv.UserId = userId;
                nv.Vote = vote;
                nv.GameItemId = itemId;
                _gameVoteRepository.Add(nv);
            }
            await _unitOfWork.SaveAsync();
        }
    }
}
