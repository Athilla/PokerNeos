using GroupeIsa.Neos.Domain.Rules.ValidationRules;
using Transversals.Business.Domain.Properties;

namespace Transversals.Business.UserPermissions.Domain.PermissionValidationRules
{
    /// <summary>
    /// Represents Check that Read Access is true if Create/Update/Delete/.
    /// </summary>
    public class CheckIfReadMustBeTrue : ValidationRule<Transversals.Business.Domain.Entities.Permission>, Transversals.Business.Domain.Rules.ValidationRules.Permission.ICheckIfReadMustBeTrue
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckIfReadMustBeTrue"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public CheckIfReadMustBeTrue()
        {
        }

        /// <inheritdoc/>
        public override IValidationRuleResult Execute()
        {
            if ((Item.HasCreationAccess || Item.HasUpdateAccess || Item.HasDeleteAccess) && !Item.HasReadOnlyAccess)
            {
                return Error(Resources.UserPermissions.ErrorReadAccessIsNotGiven);
            }
            return Success();
        }
    }
}