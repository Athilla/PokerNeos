using GroupeIsa.Neos.Application.Rules.EventRules;
using GroupeIsa.Neos.Shared.Logging;
using PokerNeos.Application.Abstractions.EntityViews;
using PokerNeos.Application.Abstractions.Persistence;
using PokerNeos.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokerNeos.PokerBase.Application.EventRules.ShowGameView
{
    /// <summary>
    /// Represents Retrieving event rule.
    /// </summary>
    public class Retrieving : IRetrievingRule<PokerNeos.Application.Abstractions.EntityViews.IShowGameView>
    {
        private readonly INeosLogger<IRetrievingRule<PokerNeos.Application.Abstractions.EntityViews.IShowGameView>> _logger;
        private readonly IGameRepository _gameRepository;
        private readonly IGameUtil _gameUtil;
        private readonly IGameViewRepository _gameViewRepository;
        private readonly IShowGameViewRepository _showGameViewRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="Retrieving"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public Retrieving(INeosLogger<IRetrievingRule<PokerNeos.Application.Abstractions.EntityViews.IShowGameView>> logger,
            IGameRepository gameRepository,
            IGameUtil gameUtil,
            IGameViewRepository gameViewRepository,
            IShowGameViewRepository showGameViewRepository)
        {
            _logger = logger;
            _gameRepository = gameRepository;
            _gameUtil = gameUtil;
            _gameViewRepository = gameViewRepository;
            _showGameViewRepository = showGameViewRepository;
        }

        /// <inheritdoc/>
        public async Task OnRetrievingAsync(IRetrievingRuleArguments<PokerNeos.Application.Abstractions.EntityViews.IShowGameView> args)
        {

            // on recupere les informations de la partie
            int key = args.Parameters.GetGameId();
            Console.WriteLine($"ID récupérer : {key}");
            _logger.LogError($"ID récupérer : {key}");

            IGameView? games = await _gameViewRepository.FindAsync(key);
            IGameView game = await _gameViewRepository.GetAsync(key);
            if (game == null) { return; }

            //on verifie que le joueur est bien dans la partie
            if (!_gameUtil.CheckGroup(game.GroupeId)) { return; }
            IShowGameView showGameView = _showGameViewRepository.New();
            showGameView.GameId = key;
            showGameView.Game = game;

            args.SetItems(new List<IShowGameView>() { showGameView }, 1);

            //IShowGameView showGameView = _showGameViewRepository.New();
            //showGameView.GameId = 99;

            //args.SetItems(new List<IShowGameView>() { showGameView }, 1);
        }
    }
}