using GroupeIsa.Neos.Domain.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transversals.Business.Core.Domain.Exceptions;
using Transversals.Business.Domain.Persistence;

namespace Transversals.Business.Core.Domain.Configuration.Options
{
    public class OptionService : IOptionService
    {
        private readonly IOptionRepository _optionRepository;
        private readonly IServiceProvider _sp;
        private readonly List<Option> _options;
        private readonly IUnitOfWork _unitOfWork;

        public OptionService(IOptionRepository optionRepository, IServiceProvider sp)
        {
            _optionRepository = optionRepository;
            _sp = sp;
            _options = new List<Option>();
            _unitOfWork = _sp.GetRequiredService<IUnitOfWork>();
            LoadOptions();
        }

        public void LoadOptions()
        {
            _options.Clear();
            List<Business.Domain.Entities.Option> options = _optionRepository.GetQuery().ToList();
            options.ForEach(o => _options.Add(new Option(o, _sp)));
        }

        public static Business.Domain.Entities.Option CreateOptionEntity<T>(string name, T value)
        {
            return new Business.Domain.Entities.Option()
            {
                Name = name,
                Value = value?.ToString() ?? string.Empty,
                Type = typeof(T).Name,
            };
        }

        public async Task CreateNewOptionAsync<T>(string name, T value)
        {
            Business.Domain.Entities.Option newOption = CreateOptionEntity(name, value);
            _optionRepository.Add(newOption);
            await _unitOfWork.SaveAsync();
            LoadOptions();
        }

        private Option FindOption(string key)
        {
            if (_options.Any(c => c.Name == key))
            {
                return _options.First(c => c.Name == key);
            }
            throw new NullValueException("Option Not Found");
        }

        public bool TryFindOption(string key)
        {
            if (_options.Any(c => c.Name == key))
            {
                return true;
            }
            return false;
        }

        public bool TryFindOption(string key, out Option? option)
        {
            option = _options.FirstOrDefault(c => c.Name == key);
            return option != null;
        }

        public Option this[string key] => FindOption(key);
    }
}
