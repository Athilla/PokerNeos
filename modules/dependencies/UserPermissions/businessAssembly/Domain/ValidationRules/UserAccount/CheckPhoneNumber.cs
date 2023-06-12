using GroupeIsa.Neos.Domain.Rules.ValidationRules;
using System.Text.RegularExpressions;
using Transversals.Business.Domain.Properties;

namespace Transversals.Business.UserPermissions.Domain.UserAccountValidationRules
{
    /// <summary>
    /// Represents Check if phone number is in correct format.
    /// </summary>
    public partial class CheckPhoneNumber : ValidationRule<Business.Domain.Entities.UserAccount>, Business.Domain.Rules.ValidationRules.UserAccount.ICheckPhoneNumber
    {
        [GeneratedRegex("^\\+?\\d{1,4}?[-.\\s]?\\(?\\d{1,3}?\\)?[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,4}[-.\\s]?\\d{1,9}$")]
        private static partial Regex validatePhoneNumberRegex();

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
                bool result = validatePhoneNumberRegex().IsMatch(Item.PhoneNumber);
                return result ? Success() : Error(Resources.UserPermissions.PhoneNumberBadFormat);
            }
            return Success();
        }
    }
}