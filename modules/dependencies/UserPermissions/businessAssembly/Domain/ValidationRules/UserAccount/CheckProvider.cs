using GroupeIsa.Neos.Domain.Rules.ValidationRules;
using Microsoft.Extensions.Configuration;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Properties;
using Transversals.Business.Domain.Rules.ValidationRules.UserAccount;

namespace Transversals.Business.UserPermissions.Domain.UserAccountValidationRules
{
    /// <inheritdoc/>
    public class CheckProvider : ValidationRule<UserAccount>, ICheckProvider
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckProvider"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public CheckProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc/>
        public override IValidationRuleResult Execute()
        {
            if (_configuration["AuthenticationMode"] == "AzureB2C" && !string.Equals(Item.Login, Item.Email))
            {
                return Error(Resources.UserPermissions.LoginBadFormat);
            }
            return Success();

        }
    }
}