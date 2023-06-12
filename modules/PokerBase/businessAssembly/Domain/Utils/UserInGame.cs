using GroupeIsa.Neos.Domain.Persistence;
using PokerNeos.Domain.Persistence;
using System.Threading.Tasks;
using Transversals.Business.Domain.Entities;

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

        public async Task AddUserInGameAsync(int gameId, UserAccount userAccount)
        {
            var user = await _userInGameRepository.FindAsync(userAccount.Id);
            if (user == null)
            {
                var userInGame = _userInGameRepository.New();
                userInGame.GameId = gameId;
                userInGame.UserId = userAccount.Id;
                userInGame.DefaultUserName = userAccount.FirstName + " " + userAccount.LastName;
                _userInGameRepository.Add(userInGame);
                userInGame.IsOnline = true;
            }
            else
            {
                user.IsOnline = false;
            }
            await _unitOfWork.SaveAsync();
        }

        public async Task RemoveUserInGameAsync(int gameId, int userId)
        {
            var user = await _userInGameRepository.FindAsync(userId);
            if (user != null && user.GameId == gameId)
            {
                user.IsOnline = false;
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
