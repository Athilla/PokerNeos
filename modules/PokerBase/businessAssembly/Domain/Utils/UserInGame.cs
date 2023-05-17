using GroupeIsa.Neos.Domain.Persistence;
using PokerNeos.Domain.Persistence;
using System.Threading.Tasks;

namespace PokerNeos.PokerBase.Domain.Utils
{
    public class UserInGame : IUserInGame
    {
        private readonly IUserInGameRepository _userInGameRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserInGame(IUserInGameRepository userInGameRepository, IUnitOfWork unitOfWork)
        {
            _userInGameRepository = userInGameRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task AddUserInGameAsync(int gameId, int userId)
        {
            var user = await _userInGameRepository.FindAsync(userId);
            if(user == null)
            {
                var userInGame = _userInGameRepository.New();
                userInGame.GameId = gameId;
                userInGame.UserId = userId;
                _userInGameRepository.Add(userInGame);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task RemoveUserInGameAsync(int gameId, int userId)
        {
            var user = await _userInGameRepository.FindAsync(userId);
            if (user != null && user.GameId == gameId)
            {
                _userInGameRepository.Remove(user);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
