using PokerNeos.Application.Abstractions.Methods;
using PokerNeos.Application.Abstractions.Notifications;
using System.Threading.Tasks;

namespace PokerNeos.PokerBase.Application.Methods
{
    /// <summary>
    /// Represents BroadcastMessage method.
    /// </summary>
    public class BroadcastMessage : IBroadcastMessage
    {

        private readonly INotificationRegistry _notificationRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcastMessage"/> class.
        /// </summary>
        /// <param name="notificationRegistry">The notification registry.</param>
        public BroadcastMessage(INotificationRegistry notificationRegistry)
        {
            _notificationRegistry = notificationRegistry;
        }

        /// <inheritdoc/>
        public async Task ExecuteAsync()
        {
            throw new GroupeIsa.Neos.Domain.Exceptions.BusinessException("Error");
        }
    }
}