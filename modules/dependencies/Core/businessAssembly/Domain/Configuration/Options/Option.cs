using GroupeIsa.Neos.Domain.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Transversals.Business.Core.Domain.Configuration.Options
{
    public class Option : IOption
    {
        private readonly Business.Domain.Entities.Option _option;
        private readonly IServiceProvider _sp;

        public string Value => _option.Value;
        public string Type => _option.Type;
        public string Name => _option.Name;
        public int Id => _option.Id;

        public Option(Business.Domain.Entities.Option option, IServiceProvider sp)
        {
            _option = option;
            _sp = sp;
        }

        /// <summary>
        /// Updates the value asynchronous.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Return True if the value was saving in database. 
        /// Return False if the new value is already the current value or if an error on saving data.
        /// </returns>
        public async Task<bool> UpdateValueAsync(string value)
        {
            if (value == _option.Value)
            {
                return false;
            }
            var _unitOfWork = _sp.GetRequiredService<IUnitOfWork>();

            _option.Value = value;
            var result = await _unitOfWork.SaveAsync();
            return result.IsSuccess;
        }
    }
}
