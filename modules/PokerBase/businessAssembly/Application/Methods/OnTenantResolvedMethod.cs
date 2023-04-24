using GroupeIsa.Neos.Application;
using GroupeIsa.Neos.Shared.Logging;
using PokerNeos.Application.Abstractions.Methods;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokerNeos.PokerBase.Application.Methods
{
    /// <summary>
    /// Represents OnTenantResolvedMethod method.
    /// </summary>
    public class OnTenantResolvedMethod : IOnTenantResolvedMethod
    {
        private readonly INeosLogger<IOnTenantResolvedMethod> _logger;
        private readonly IApplicationInfo _applicationInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnTenantResolvedMethod"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        public OnTenantResolvedMethod(INeosLogger<IOnTenantResolvedMethod> logger, IApplicationInfo applicationInfo)
        {
            _logger = logger;
            _applicationInfo = applicationInfo;
        }

        /// <inheritdoc/>
        public async Task<OnTenantResolvedResult> ExecuteAsync()
        {
            return new OnAuthenticatedResult(true, new Dictionary<string, object>() { }, theme: _applicationInfo.Themes.FirstOrDefault(t => t == "default"));
        }
    }
}