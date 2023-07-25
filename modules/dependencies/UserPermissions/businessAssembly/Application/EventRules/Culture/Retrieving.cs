using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GroupeIsa.Neos.Application;
using GroupeIsa.Neos.Application.Rules.EventRules;
using GroupeIsa.Neos.Shared.Extensions;
using Transversals.Business.Application.Abstractions.EntityViews;
using Transversals.Business.Application.Abstractions.Persistence;

namespace Transversals.Business.UserPermissions.Application.EventRules.Culture
{
    /// <summary>
    /// Represents Retrieving event rule.
    /// </summary>
    public class Retrieving : IRetrievingRule<Transversals.Business.Application.Abstractions.EntityViews.ICulture>
    {
        private readonly IApplicationInfo _applicationInfo;
        private readonly ICultureRepository _cultureRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="Retrieving"/> class.
        /// </summary>
        public Retrieving(IApplicationInfo applicationInfo, ICultureRepository cultureRepository)
        {
            _applicationInfo = applicationInfo;
            _cultureRepository = cultureRepository;
        }

        /// <inheritdoc/>
        public Task OnRetrievingAsync(IRetrievingRuleArguments<Transversals.Business.Application.Abstractions.EntityViews.ICulture> args)
        {
            IEnumerable<ICulture> cultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture && c.TwoLetterISOLanguageName != "iv" && // ignore neutral culture like "en" only and invariant culture "iv"
                !int.TryParse(new RegionInfo(c.Name).Name, out _) && // Ingore culture like "en-001" 
                _applicationInfo.Languages.Any(l => l.Equals(c.TwoLetterISOLanguageName, StringComparison.InvariantCultureIgnoreCase)))
                .Select(c => CreateCultureFromCultureInfo(c));
            args.SetItemsSource(cultures);
            return Task.CompletedTask;
        }

        private ICulture CreateCultureFromCultureInfo(CultureInfo cultureInfo)
        {
            ICulture culture = _cultureRepository.New();
            culture.Code = cultureInfo.Name;
            culture.Name = cultureInfo.DisplayName.ToCapitalizeFirstLetter();
            return culture;
        }
    }

}