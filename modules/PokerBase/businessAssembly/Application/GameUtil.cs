using GroupeIsa.Neos.Shared.Linq;
using GroupeIsa.Neos.Shared.MultiTenant;
using PokerNeos.Domain.Persistence;
using System.Linq;
using System.Threading.Tasks;
using Transversals.Business.Domain.Persistence;

namespace PokerNeos.PokerBase.Application
{
    public class GameUtil : IGameUtil
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IGroupeUserAccountRepository _groupeUserAccountRepository;
        private readonly IUserInfo _user;

        public GameUtil(IUserAccountRepository userAccountRepository,
            IGroupeUserAccountRepository groupeUserAccountRepository,
            IUserInfoAccessor user)
        {
            _userAccountRepository = userAccountRepository;
            _groupeUserAccountRepository = groupeUserAccountRepository;
            _user = user.User!;
        }

        public bool CheckGroup(int groupeId)
        {
            var userEmail = _user.Email?.ToLower() ?? string.Empty;

            var user = _userAccountRepository.GetQuery().Where(u => u.Email == userEmail).SingleOrDefault();
            if (user == null)
            {
                return false;
            }

            return _groupeUserAccountRepository.GetQuery().Any(gu => gu.UserId == user.Id && gu.GroupeId == groupeId);
        }
    }
}
