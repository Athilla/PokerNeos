using System.Threading.Tasks;

namespace Transversals.Business.Core.Domain.Configuration.Options
{
    public interface IOption
    {
        Task<bool> UpdateValueAsync(string value);
    }
}
