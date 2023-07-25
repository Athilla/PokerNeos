using GroupeIsa.Neos.Persistence.EntityFramework;

namespace Transversals.Business.Core.Domain.Tests.UnitTests.DataBase
{

    internal class DatabasePersistenceSettings : IDatabasePersistenceSettings
    {
        public string ConnectionString { get; set; } = null!;

        public NeosDatabaseType DatabaseType { get; set; }

        public DatabaseConcurrencyAccessMode DefaultConcurrencyAccessMode { get; set; }
    }
}
