using GroupeIsa.Neos.Domain.Rules.ValidationRules;
using System.Text.RegularExpressions;
using Transversals.Business.Domain.Properties;

namespace Transversals.Business.UserPermissions.Domain.UserAccountValidationRules
{
    /// <summary>
    /// Represents Check is email is in correct format.
    /// </summary>
    public class CheckEmail : ValidationRule<Transversals.Business.Domain.Entities.UserAccount>, Transversals.Business.Domain.Rules.ValidationRules.UserAccount.ICheckEmail
    {
        private readonly string _regexValidation = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckEmail"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public CheckEmail()
        {
        }

        /// <inheritdoc/>
        public override IValidationRuleResult Execute()
        {
            Item.Email = Item.Email.ToLower();
            if (!Regex.IsMatch(Item.Email, _regexValidation))
            {
                return Error(Resources.UserPermissions.EmailBadFormat);
            }
            return Success();
        }
    }
}