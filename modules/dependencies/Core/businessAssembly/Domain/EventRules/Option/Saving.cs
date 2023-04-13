using FluentResults;
using GroupeIsa.Neos.Domain.Exceptions;
using GroupeIsa.Neos.Domain.Rules.EventRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transversals.Business.Core.Domain.Factory;
using Transversals.Business.Domain.Entities;

namespace Transversals.Business.Core.Domain.OptionEventRules
{
    /// <summary>
    /// Represents Saving event rule.
    /// </summary>
    public class Saving : ISavingRule<Transversals.Business.Domain.Entities.Option>
    {
        /// <summary>
        /// The service provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        private readonly IEnumerable<IFactoryOptions> _factoryOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Saving"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="factoryOptions"></param>
        public Saving(IServiceProvider serviceProvider, IEnumerable<IFactoryOptions> factoryOptions)
        {
            _serviceProvider = serviceProvider;
            _factoryOptions = factoryOptions;
        }

        /// <inheritdoc/>
        public async Task OnSavingAsync(ISavingRuleArguments<Transversals.Business.Domain.Entities.Option> args)
        {
            Result result = Result.Ok();
            foreach (Option item in args.ModifiedItems)
            {
                //Récupération du traitement à faire selon le nom de l'option
                //On exécute le code correspondant au traitement avec la méthode Saving du IOptionSavingRules

                foreach (var factoryOptions in _factoryOptions)
                {
                    var service = factoryOptions.GetOptionSavingRules(item, _serviceProvider);
                    if(service != null)
                        result = Result.Merge(result, service.Saving(item));
                }
            }
            if (result.IsFailed)
            {
                string errorMessage = "";
                foreach (var resultError in result.Errors)
                {
                    errorMessage += resultError.Message + Environment.NewLine;
                }
                throw new BusinessException(errorMessage);
            }

            //pour empêcher le warning sur le async non utile
            await Task.CompletedTask;
        }
    }
}