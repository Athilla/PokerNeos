using GroupeIsa.Neos.Shared.Linq;
using PokerNeos.Application.Abstractions.DataObjects;
using PokerNeos.Application.Abstractions.Methods;
using PokerNeos.Domain.Entities;
using PokerNeos.Domain.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<List<PokerNeos.Application.Abstractions.DataObjects.ItemDO>> ExecuteAsync(int gameId)
        {
            List<GameItem> gameItems = await _gameItemRepository.GetQuery().Where(g => g.GameId == gameId).ToListAsync();
            List<ItemDO> items = new List<ItemDO>();
            foreach (GameItem gameItem in gameItems)
            {
                items.Add(new ItemDO()
                {
                    Id = gameItem.Id,
                    Name = gameItem.Name,
                    Description = gameItem.Description ?? string.Empty,
                    Link = gameItem.Link ?? string.Empty,
                    Note = gameItem.Note ?? string.Empty,
                });
            }
            return items;
        }
    }
}