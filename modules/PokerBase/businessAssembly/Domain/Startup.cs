using Microsoft.Extensions.DependencyInjection;
using PokerNeos.PokerBase.Domain.Utils;
using System.Diagnostics.CodeAnalysis;

namespace PokerNeos.PokerBase.Domain
{
    /// <summary>
    /// Represents the assembly startup.
    /// </summary>
    /// <remarks>
    /// This class is automatically instanciated when the assembly is loaded.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public static class Startup
    {
        /// <summary>
        /// Configures services for dependency injection.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <remarks>
        /// This method is automatically called when the assembly is loaded.
        /// </remarks>
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUserInformation, UserInformation>();
            services.AddScoped<IGamesInformation, GamesInformation>();
            services.AddScoped<IGameInProgress, GameInProgress>();
            services.AddScoped<IGameEvent, GameEvent>();
            services.AddScoped<IUserInGame, UserInGame>();
            services.AddScoped<IGameVote, GameVote>();
        }

    }
}