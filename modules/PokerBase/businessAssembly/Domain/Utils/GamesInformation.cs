using GroupeIsa.Neos.Shared.Linq;
using PokerNeos.Application.Abstractions.DataObjects;
using PokerNeos.Domain.Enums;
using PokerNeos.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerNeos.PokerBase.Domain.Utils
{
    public class GamesInformation : IGamesInformation
    {
        private readonly IGameRepository _gameRepository;

        public GamesInformation(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<List<PokerGameInformation>> GetGamesInformationAsync(List<int> groupes)
        {
            List<PokerGameInformation> games = new List<PokerGameInformation>();

            var gamesFromGroupes = await _gameRepository.GetQuery()
                                                        .Include(e => e.Groupe)
                                                        .Include(e => e.GameItemList)
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
