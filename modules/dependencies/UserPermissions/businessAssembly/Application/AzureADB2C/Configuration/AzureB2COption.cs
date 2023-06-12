using System.Diagnostics.CodeAnalysis;

namespace Transversals.Business.UserPermissions.Application.AzureADB2C.Configuration
{
    /// <summary>AzureADOptions Class</summary>
    [ExcludeFromCodeCoverage]
    public class AzureB2COption
    {
        /// <summary>Gets or sets the tenant identifier.</summary>
        /// <value>The tenant identifier.</value>
        public string? TenantId { get; set; }

        /// <summary>Gets or sets the name of the tenant.</summary>
        /// <value>The name of the tenant.</value>
        public string? TenantName { get; set; }

        /// <summary>Gets or sets the client identifier.</summary>
        /// <value>The client identifier.</value>
        public string? ClientId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? ClientSecretKey { get; set; }

        /// <summary>
        /// Gets or sets the B2C extension application client identifier.
        /// </summary>
        /// <value>
        /// The B2C extension application client identifier.
        /// </value>
        public string? B2cExtensionAppClientId { get; set; }

        /// <summary>
        /// Gets or sets the content of the email.
        /// </summary>
        /// <value>
        /// The content of the email.
        /// </value>
        public EmailContent EmailContent { get; set; } = new EmailContent();
    }

    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EmailContent
    {
        /// <summary>
        /// Gets or sets the redirect URL.
        /// </summary>
        /// <value>
        /// The redirect URL.
        /// </value>
        public string RedirectUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; } = "Bienvenue dans les MT";

        /// <summary>
        /// Gets or sets the content1.
        /// </summary>
        /// <value>
        /// The content1.
        /// </value>
        public string Content1 { get; set; } = "Vous êtes invité à vous connecter à notre application en utilisant comme identifiant votre email(<strong>{Email}</strong>) et votre mot de passe temporaire, ci-dessous, qui sera à changer à la première connexion.";
       
        /// <summary>
        /// Gets or sets the content2.
        /// </summary>
        /// <value>
        /// The content2.
        /// </value>
        public string Content2 { get; set; } = "Votre mot de passe temporaire : <strong>{Password}</strong>";
        
        /// <summary>
        /// Gets or sets the link text.
        /// </summary>
        /// <value>
        /// The link text.
        /// </value>
        public string LinkText { get; set; } = "Accéder à l'application";
    }

}
