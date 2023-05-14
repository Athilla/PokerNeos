using System.Threading.Tasks;

namespace PokerNeos.PokerBase.Domain.Utils
{
    public interface IUserInformation
    {
        Task<UserData> GetCurrentUserAsync();
    }
}