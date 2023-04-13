using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Transversals.Business.Authentication.Application.AuthenticationNeos
{
    /// <summary>
    /// Représente le résultat de l'appel API au serveur d'authentification
    /// </summary>
    public partial class UserAuthenticationResult
    {

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public StateType State { get; set; }
    }
}
