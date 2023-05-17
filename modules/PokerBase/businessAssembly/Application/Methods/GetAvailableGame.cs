using PokerNeos.Application.Abstractions.DataObjects;
using PokerNeos.Application.Abstractions.Methods;
using PokerNeos.PokerBase.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokerNeos.PokerBase.Application.Methods
{
    /// <summary>
    /// Represents GetAvailableGame method.
    /// </summary>
    public class GetAvailableGame : IGetAvailableGame
    {
        private readonly IGamesInformation _gamesInformation;
        private readonly IUserInformation _userInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAvailableGame"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public GetAvailableGame(IGamesInformation gamesInformation,
            IUserInformation userInformation)
        {
            _gamesInformation = gamesInformation;
            _userInformation = userInformation;
        }

        /// <inheritdoc/>
        public async Task<List<PokerNeos.Application.Abstractions.DataObjects.PokerGameInformationDO>> ExecuteAsync()
        {
            try
            {
                //Get Groupes from user
                UserData userData = await _userInformation.GetCurrentUserAsync();
                //Get games from groupes
                return await _gamesInformation.GetGamesInformationAsync(userData.GroupeIdList);
            }
            catch (Exception)
            {
                return new List<PokerGameInformationDO>();
            }

        }
    }
}