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
using PokerNeos.Application.Abstractions.DataObjects;
using PokerNeos.Application.Abstractions.EntityViews;
using PokerNeos.Application.Abstractions.Methods;
using PokerNeos.Application.Abstractions.Notifications;
using PokerNeos.Domain.Entities;
using PokerNeos.Domain.Enums;
using PokerNeos.Domain.Properties;
using PokerNeos.PokerBase.Domain.Utils;

namespace PokerNeos.PokerBase.Application.Methods
{
    /// <summary>
    /// Represents GetLastGameInformation method.
    /// </summary>
    public class GetLastGameInformation : IGetLastGameInformation
    {
        private readonly IUserInformation _userInformation;
        private readonly IGamesInformation _gamesInformation;
        private readonly IGameInProgress _gameInProgress;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetLastGameInformation"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public GetLastGameInformation(IUserInformation userInformation,
                        IGamesInformation gamesInformation,
                        IGameInProgress gameInProgress)
        {
            _userInformation = userInformation;
            _gamesInformation = gamesInformation;
            _gameInProgress = gameInProgress;
        }
        /// <inheritdoc/>
        public async Task<GameDataDO> ExecuteAsync(int gameId)
        {
            try
            {

                UserData userData = await _userInformation.GetCurrentUserAsync();

                var games = await _gamesInformation.GetGamesInformationAsync(userData.GroupeIdList);
                //on verifie que la partie existe
                if (!games.Any(g => g.GameId == gameId))
                {
                    return new GameDataDO() { IsSuccess = false };
                }
                //on recupere la partie en cours
                return await _gameInProgress.GetGameDataAsync(gameId);
            }
            catch (Exception)
            {
                return new GameDataDO() { IsSuccess = false };
            }
        }
    }
}