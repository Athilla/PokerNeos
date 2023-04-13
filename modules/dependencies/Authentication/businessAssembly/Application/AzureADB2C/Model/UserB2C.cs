using System.Diagnostics.CodeAnalysis;

namespace Transversals.Business.Authentication.Application.AzureADB2C.Model
{
    /// <summary>
    /// User B2C
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserB2C
    {
        /// <summary>Gets or sets the first name.</summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>Gets or sets the last name.</summary>
        /// <value>The last name.</value>
        public string LastName { get; set; } = string.Empty;

        /// <summary>Gets or sets the email.</summary>
        /// <value>The email.</value>
        public string Email { get; set; } = string.Empty;

    }
}
