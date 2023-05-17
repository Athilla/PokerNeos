using GroupeIsa.Neos.Shared.Logging;
using PokerNeos.Application.Abstractions.DataObjects;
using PokerNeos.Application.Abstractions.Methods;
using PokerNeos.Domain.Enums;
using PokerNeos.PokerBase.Domain.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly IGameInProgress _gameInProgress;
        private readonly IGameEvent _gameEvent;
        private readonly IUserInGame _userInGame;
        private readonly IGameVote _gameVote;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendEventAsync"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public SendEvent(INeosLogger<ISendEvent> logger,
                        IUserInformation userInformation,
                        IGamesInformation gamesInformation,
                        IGameInProgress gameInProgress,
                        IGameEvent gameEvent,
                        IUserInGame userInGame,
                        IGameVote gameVote)
        {
            _logger = logger;
            _userInformation = userInformation;
            _gamesInformation = gamesInformation;
            _gameInProgress = gameInProgress;
            _gameEvent = gameEvent;
            _userInGame = userInGame;
            _gameVote = gameVote;
        }

        /// <inheritdoc/>
        public async Task<GameDataDO> ExecuteAsync(PokerNeos.Application.Abstractions.DataObjects.GameEventDO gameEvent)
        {
            try
            {

                ///////////////////////////////////////////////////////////
                /// TODO
                ///  - [x] implementer un class pour recuperer l'utilisateur courant
                ///  - [x]  trouver la partie en cours
                ///      - si elle n'existe pas, la creer
                ///  - [x] ajouter l'evenement a la partie
                ///  - [] ne doit retourner qu'un bool true ou false
                ///////////////////////////////////////////////////////////
                UserData userData = await _userInformation.GetCurrentUserAsync();

                var games = await _gamesInformation.GetGamesInformationAsync(userData.GroupeIdList);
                //on verifie que la partie existe
                if (!games.Any(g => g.GameId == gameEvent.GameId))
                {
                    return new GameDataDO() { IsSuccess = false };
                }
                var currentGame = games.First(g => g.GameId == gameEvent.GameId);

                //on recupere la partie en cours
                GameDataDO gameData = await _gameInProgress.GetGameDataAsync(gameEvent.GameId);
                switch (gameEvent.Event)
                {
                    case Event.HasStarted:
                        //on rajoute l'evenement
                        await _gameEvent.AddStartEventAsync(gameEvent.GameId, userData.UserAccountId);
                        //on change le status de la partie
                        await _gameInProgress.ChangeStateAsync(gameEvent.GameId, GameState.Started);
                        //on ce positionne sur l'US courrante ? ou la premiere a d�faut
                        if (currentGame.ItemList.Count > 0 && gameData.GameInProgress.CurrentItem == null)
                        {
                            await _gameInProgress.ChangeItemAsync(gameEvent.GameId, currentGame.ItemList.First().Id);
                        }
                        break;
                    case Event.HasStopped:
                        //on rajoute l'evenement
                        await _gameEvent.AddStopEventAsync(gameEvent.GameId, userData.UserAccountId);
                        //on change le status de la partie
                        await _gameInProgress.ChangeStateAsync(gameEvent.GameId, GameState.Stopped);

                        break;
                    case Event.HasJoined:
                        //on rajoute l'evenement
                        await _gameEvent.AddJoinEventAsync(gameEvent.GameId, userData.UserAccountId);
                        //on rajoute le joueur a la partie
                        await _userInGame.AddUserInGameAsync(gameEvent.GameId, userData.UserAccountId);
                        break;
                    case Event.HasLeft:
                        //on rajoute l'evenement
                        await _gameEvent.AddLeaveEventAsync(gameEvent.GameId, userData.UserAccountId);
                        //on enleve le joueur de la partie
                        await _userInGame.RemoveUserInGameAsync(gameEvent.GameId, userData.UserAccountId);
                        break;

                    case Event.HasVoted:
                        //on rajoute l'evenement
                        await _gameEvent.AddVoteEventAsync(gameEvent.GameId, userData.UserAccountId, gameEvent.Vote.ToString(), gameEvent.ItemId);
                        //on rajoute le vote de l'utilisateur sur la l'itemId (US)
                        await _gameVote.AddGameVoteAsync(gameEvent.GameId, userData.UserAccountId, gameEvent.ItemId, gameEvent.Vote.ToString());
                        break;
                    case Event.ChangeUS:
                        //on rajoute l'evenement
                        await _gameEvent.AddChangeUSEventAsync(gameEvent.GameId, userData.UserAccountId, gameEvent.ItemId);
                        //on change l'US courrante + ShowVote a false
                        await _gameInProgress.ChangeItemAsync(gameEvent.GameId, gameEvent.ItemId);
                        break;
                    case Event.ShowVote:
                        //on rajoute l'evenement
                        await _gameEvent.AddShowVoteEventAsync(gameEvent.GameId, userData.UserAccountId, gameEvent.ItemId);
                        //ShowVote a true
                        await _gameInProgress.ShowVoteAsync(gameEvent.GameId);
                        break;

                }
                //on recupere la partie en cours
                return await _gameInProgress.GetGameDataAsync(gameEvent.GameId);
            }
            catch (Exception)
            {
                return new GameDataDO() { IsSuccess = false };
            }

        }
    }
}