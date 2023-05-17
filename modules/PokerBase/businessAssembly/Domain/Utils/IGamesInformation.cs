using PokerNeos.Application.Abstractions.DataObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokerNeos.PokerBase.Domain.Utils
{
    public interface IGamesInformation
    {
        Task<List<PokerGameInformationDO>> GetGamesInformationAsync(List<int> groupes);
    }
}