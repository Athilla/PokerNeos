using GroupeIsa.Neos.Migration.Commands;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Transversals.Business.UserPermissions.Application.Migration
{
    /// <summary>
    /// Command removing potential duplicates of user permissions in the database.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Should be tested by integration test instead of unit tests")]
    public class RemoveUserPermissionDuplicatesCommand : ICommand
    {
        public void Execute(FluentMigrator.Migration migration, ICommandExecutionArgs args)
        {
            switch (args.DatabaseType)
            {
                case GroupeIsa.Neos.Shared.Persistence.DatabaseType.Oracle:
                    migration.Execute.Sql("DELETE \"Permission\" WHERE rowid NOT IN (SELECT MIN(rowid) FROM \"Permission\" GROUP BY \"TenantId\", \"UserRoleId\", \"FunctionName\")");
                    break;
                case GroupeIsa.Neos.Shared.Persistence.DatabaseType.PostgreSQL:
                    migration.Execute.Sql("delete from public.\"Permission\" p0 using public.\"Permission\" p1 where p0.\"Id\" < p1.\"Id\" and p0.\"TenantId\" = p1.\"TenantId\" and p0.\"UserRoleId\" = p1.\"UserRoleId\" and p0.\"FunctionName\" = p1.\"FunctionName\"");
                    break;
                case GroupeIsa.Neos.Shared.Persistence.DatabaseType.SqlServer:
                    migration.Execute.Sql("DELETE FROM Permission WHERE Id NOT IN (SELECT MIN(Id) FROM Permission GROUP BY TenantId, UserRoleId, FunctionName)");
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
