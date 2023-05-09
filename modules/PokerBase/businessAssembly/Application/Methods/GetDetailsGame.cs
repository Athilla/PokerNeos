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
using PokerNeos.Domain.Persistence;
using PokerNeos.Domain.Properties;

namespace PokerNeos.PokerBase.Application.Methods
{
    /// <summary>
    /// Represents GetDetailsGame method.
    /// </summary>
    public class GetDetailsGame : IGetDetailsGame
    {
        private readonly IGameItemRepository _gameItemRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetDetailsGame"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public GetDetailsGame(IGameItemRepository gameItemRepository)
        {
            _gameItemRepository = gameItemRepository;
        }

        /// <inheritdoc/>
        public async Task<List<Item>> ExecuteAsync(int gameId)
        {
            List<GameItem> gameItems = await _gameItemRepository.GetQuery().Where(g => g.GameId == gameId).ToListAsync();
            List<Item> items = new List<Item>();
            foreach (GameItem gameItem in gameItems)
            {
                items.Add(new Item()
                {
                    Id = gameItem.Id,
                    Name = gameItem.Name,
                    Description = gameItem.Description,
                    Link = gameItem.Link,
                    Note = gameItem.Note,
                });
            }
            return items;
        }
    }
}