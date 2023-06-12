using GroupeIsa.Neos.Shared.Linq;
using GroupeIsa.Neos.Shared.MultiTenant;
using PokerNeos.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Persistence;

namespace PokerNeos.PokerBase.Domain.Utils
{
    public class UserInformation : IUserInformation
    {
        private readonly IUserInfo _user;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IGroupeUserAccountRepository _groupeUserAccountRepository;

        public UserInformation(IUserInfoAccessor user,
                               IUserAccountRepository userAccountRepository,
                               IGroupeUserAccountRepository groupeUserAccountRepository)
        {
            _user = user.User!;
            _userAccountRepository = userAccountRepository;
            _groupeUserAccountRepository = groupeUserAccountRepository;
        }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <returns>return UserData information if user find</returns>
        public async Task<UserData> GetCurrentUserAsync()
        {
            var userAccount = GetCurrentUserInternal();
            var groupeIds = await GetCurrentUserGroupeInternalAsync(userAccount.Id);
            return new UserData
            {
                UserAccount = userAccount,
                GroupeIdList = groupeIds
            };
        }

        private UserAccount GetCurrentUserInternal()
        {
            var userEmail = _user.Email?.ToLower() ?? string.Empty;
            //recherche des comptes utilisateur existants
            UserAccount? userAccount = _userAccountRepository.GetQuery().Where(u => u.Email == userEmail).SingleOrDefault();
            if (userAccount != null
                    && userAccount.IsActive
                    && (userAccount.ExpirationDate ?? System.DateTime.Now.Date) >= System.DateTime.Now.Date)
            {
                return userAccount;
            }
            else
            {
                throw new Exception("User not found");
            }
        }

        private async Task<List<int>> GetCurrentUserGroupeInternalAsync(int userId)
        {
            List<int> groupes = new List<int>();
            var groupeUser = await _groupeUserAccountRepository.GetQuery().Where(gu => gu.UserId == userId).ToListAsync();
            foreach (var item in groupeUser)
            {
                groupes.Add(item.GroupeId);
            }
            return groupes;
        }
    }

    public class UserData
    {
        public required UserAccount UserAccount { get; set; }
        public List<int> GroupeIdList { get; set; } = new List<int>();
    }
}
