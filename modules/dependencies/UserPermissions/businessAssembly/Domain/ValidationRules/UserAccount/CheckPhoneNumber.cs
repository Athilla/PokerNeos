using GroupeIsa.Neos.Domain.Rules.ValidationRules;
using System.Text.RegularExpressions;
using Transversals.Business.Domain.Properties;

namespace Transversals.Business.UserPermissions.Domain.UserAccountValidationRules
{
    /// <summary>
    /// Represents Check if phone number is in correct format.
    /// </summary>
    public class CheckPhoneNumber : ValidationRule<Transversals.Business.Domain.Entities.UserAccount>, Transversals.Business.Domain.Rules.ValidationRules.UserAccount.ICheckPhoneNumber
    {
        private readonly string _regexValidation = @"^(0)[1-9]((?:[\s.-]|)\d{2}){4}$";

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckPhoneNumber"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public CheckPhoneNumber()
        {
        }

        /// <inheritdoc/>
        public override IValidationRuleResult Execute()
        {
            if (Item.PhoneNumber != null)
            {
                bool result = Regex.IsMatch(Item.PhoneNumber, _regexValidation);
                return result ? Success() : Error(Resources.UserPermissions.PhoneNumberBadFormat);
            }
            return Success();
        }
    }
}