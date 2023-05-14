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
using PokerNeos.Domain.Properties;
using PokerNeos.PokerBase.Domain.Utils;

namespace PokerNeos.PokerBase.Application.Methods
{
    /// <summary>
    /// Represents SendEventAsync method.
    /// </summary>
    public class SendEvent : ISendEvent
    {
        private readonly INeosLogger<ISendEvent> _logger;
        private readonly IUserInformation _userInformation;
        private readonly IGamesInformation _gamesInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendEventAsync"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public SendEvent(INeosLogger<ISendEvent> logger,
                        IUserInformation userInformation,
                        IGamesInformation gamesInformation)
        {
            _logger = logger;
            _userInformation = userInformation;
            _gamesInformation = gamesInformation;
        }

        /// <inheritdoc/>
        public async Task ExecuteAsync(GameEvent gameEvent)
        {
            ///////////////////////////////////////////////////////////
            /// TODO
            ///  - [x] implementer un class pour recuperer l'utilisateur courant
            ///  - [ ]  trouver la partie en cours
            ///      - si elle n'existe pas, la creer
            ///  - [ ] ajouter l'evenement a la partie
            ///  
            ///////////////////////////////////////////////////////////
            UserData userData = await _userInformation.GetCurrentUserAsync();
            var tt = await _gamesInformation.GetGamesInformationAsync(userData.GroupeIdList); 

        }
    }
}