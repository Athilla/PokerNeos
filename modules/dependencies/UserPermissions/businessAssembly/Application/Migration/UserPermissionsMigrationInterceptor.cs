using GroupeIsa.Neos.Migration.Commands;
using GroupeIsa.Neos.Migration.Interceptor;
using GroupeIsa.Neos.Shared.Logging;
using System.Threading.Tasks;

namespace Transversals.Business.UserPermissions.Application.Migration
{
    public class UserPermissionsMigrationInterceptor : IMigrationInterceptor
    {
        private readonly INeosLogger<UserPermissionsMigrationInterceptor> _logger;

        public UserPermissionsMigrationInterceptor(INeosLogger<UserPermissionsMigrationInterceptor> logger)
        {
            _logger = logger;
        }

        public Task<ICommandList> OnCommandsCreatedAsync(CommandsCreatedEventData data, ICommandList commandList)
        {
            // Check if the unique index of Permission table is already created.
            // If not, remove all potential duplicates before creating the index.
            if (!data.ExistingSchema.Indexes.Exists(i => i.Name == "IDX_Unique_Permission_FunctionName_UserRoleId"))
            {
                _logger.LogInformation(Properties.Resources.UserPermissionsMigrationInterceptor_RemovingDuplicates);
                commandList.AddBeforeContraintsCreation(new RemoveUserPermissionDuplicatesCommand());
            }

            return Task.FromResult(commandList);
        }
    }
}
