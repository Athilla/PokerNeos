using GroupeIsa.Neos.Domain.Rules.ValidationRules;
using Transversals.Business.Domain.Properties;

namespace Transversals.Business.UserPermissions.Domain.PermissionValidationRules
{
    /// <summary>
    /// Represents Check if the access are correctly setted with the right AccesType.
    /// </summary>
    public class CheckAccessPermission : ValidationRule<Transversals.Business.Domain.Entities.Permission>, Transversals.Business.Domain.Rules.ValidationRules.Permission.ICheckAccessPermission
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckAccessPermission"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public CheckAccessPermission()
        {
        }

        /// <inheritdoc/>
        public override IValidationRuleResult Execute()
        {
            if (Item.FunctionType == Business.Domain.Enums.FunctionType.CRUD)
            {
                if (Item.HasAccess)
                {
                    return Error(Resources.UserPermissions.PermissionErrorMessageOnBools);
                }
            }
            else
            {
                if (Item.HasReadOnlyAccess || Item.HasCreationAccess || Item.HasUpdateAccess || Item.HasDeleteAccess)
                {
                    return Error(Resources.UserPermissions.PermissionErrorMessageOnBools);
                }
            }
            return Success();
        }
    }
}