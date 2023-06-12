namespace Transversals.Business.UserPermissions.Application.AuthenticationNeos
{
    /// <summary>
    /// State type
    /// <seealso cref="UserAuthenticationResult.State"/>
    /// </summary>
    public partial class UserAuthenticationResult
    {
        /// <summary>
        /// Result state
        /// </summary>
        public enum StateType
        {
            MissingName,
            MissingEmail,
            InvalidEmail,
            InvalidConfirmationRedirectUrl,
            AlreadyCreated,
            AlreadyCreatedAndDifferentEmail,
            Succeeded,
            Failed,
        };
    }
}
