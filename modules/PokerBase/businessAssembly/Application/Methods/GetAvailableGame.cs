using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GroupeIsa.Neos.Shared;
using GroupeIsa.Neos.Shared.Linq;
using GroupeIsa.Neos.Shared.Logging;
using GroupeIsa.Neos.Shared.Metadata;
using GroupeIsa.Neos.Shared.MultiTenant;
using PokerNeos.Application.Abstractions.DataObjects;
using PokerNeos.Application.Abstractions.EntityViews;
using PokerNeos.Application.Abstractions.Methods;
using PokerNeos.Application.Abstractions.Notifications;
using PokerNeos.Domain.Enums;
using PokerNeos.Domain.Persistence;
using PokerNeos.Domain.Properties;
using Transversals.Business.Domain.Persistence;

namespace PokerNeos.PokerBase.Application.Methods
{
    /// <summary>
    /// Represents GetAvailableGame method.
    /// </summary>
    public class GetAvailableGame : IGetAvailableGame
    {
        private readonly INeosLogger<IGetAvailableGame> _logger;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IGroupeUserAccountRepository _groupeUserAccountRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IUserInfo _user;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAvailableGame"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public GetAvailableGame(INeosLogger<IGetAvailableGame> logger,
            IUserAccountRepository userAccountRepository,
            IGroupeUserAccountRepository groupeUserAccountRepository,
            IUserInfoAccessor user,
            IGameRepository gameRepository)
        {
            _logger = logger;
            _userAccountRepository = userAccountRepository;
            _groupeUserAccountRepository = groupeUserAccountRepository;
            _gameRepository = gameRepository; 
            _user = user.User!;
        }

        /// <inheritdoc/>
        public async Task<List<PokerGameInformation>> ExecuteAsync()
        {
            try
            {
                //Get Groupes from user
                var groupeIds = await GetGroupesFromCurentUser();
                //Get games from groupes
                return await GetGamesInformation(groupeIds);
            }
            catch (Exception)
            {
                return new List<PokerGameInformation>();
            }

        }

        private async Task<List<int>> GetGroupesFromCurentUser() 
        { 
            List<int> groupes = new List<int>();
            var userEmail = _user.Email?.ToLower() ?? string.Empty;

            var user = _userAccountRepository.GetQuery().Where(u => u.Email == userEmail).SingleOrDefault();
            if(user == null)
            {
                return groupes;
            }

            var groupeUser = await _groupeUserAccountRepository.GetQuery().Where(gu => gu.UserId == user.Id).ToListAsync();
            foreach (var item in groupeUser)
            {
                groupes.Add(item.GroupeId);
            }
            return groupes;
        }
        private async Task<List<PokerGameInformation>> GetGamesInformation(List<int> groupes)
        {
            List<PokerGameInformation> games = new List<PokerGameInformation>();

            var gamesFromGroupes = await _gameRepository.GetQuery()
                                                        .Include(e=> e.Groupe)
                                                        .Include(e=> e.GameItemList)
                                                        .Where(g => groupes.Contains(g.GroupeId) && g.State == GameState.Started).ToListAsync();
            foreach (var game in gamesFromGroupes)
            {
                var pgi = new PokerGameInformation
                {
                    GroupeName = game.Groupe.Name,
                    RomName = game.Name,
                    GameId = game.Id,
                    ItemsCount = 0
                };
                foreach (var item in game.GameItemList)
                {
                    pgi.ItemList.Add(new Item
                    {
                        Description = item.Description ?? string.Empty,
                        Name = item.Name,
                        Id = item.Id,
                        Link = item.Link ?? string.Empty,
                        Note = item.Note ?? string.Empty
                    });
                    pgi.ItemsCount++;
                }
                games.Add(pgi);
            }
            return games;
        }
    }
}