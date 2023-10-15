using System.Threading.Tasks;

namespace Transversals.Business.Core.Domain.Configuration.Options
{
    public interface IOptionService
    {
        Option this[string key] { get; }

        Task CreateNewOptionAsync<T>(string name, T value);
        void LoadOptions();
        bool TryFindOption(string key);
        bool TryFindOption(string key, out Option? option);
    }
}
