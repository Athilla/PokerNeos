using GroupeIsa.Neos.Domain.Rules.ValidationRules;
using System;
using Transversals.Business.Domain.Properties;

namespace Transversals.Business.UserPermissions.Domain.UserAccountValidationRules
{
    /// <summary>
    /// Represents Check if the expirationDate is valid or not.
    /// </summary>
    public class CheckExpirationDate : ValidationRule<Transversals.Business.Domain.Entities.UserAccount>, Transversals.Business.Domain.Rules.ValidationRules.UserAccount.ICheckExpirationDate
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckExpirationDate"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public CheckExpirationDate()
        {
        }

        /// <inheritdoc/>
        public override IValidationRuleResult Execute()
        {
            if (Item.ExpirationDate != null)
            {
                return Item.ExpirationDate.Value > DateTime.Now.Date ? Success() : Error(Resources.UserPermissions.ExpirationDateInvalidMessage);
            }
            return Success();
        }
    }
}