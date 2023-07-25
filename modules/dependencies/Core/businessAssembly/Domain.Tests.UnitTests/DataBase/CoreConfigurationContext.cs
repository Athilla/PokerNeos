using DemoMT.AllModules.Persistence.Configurations;
using GroupeIsa.Neos.Persistence.EntityFramework;
using GroupeIsa.Neos.Shared.Metadata;
using GroupeIsa.Neos.Shared.MultiTenant;
using Microsoft.EntityFrameworkCore;

namespace Transversals.Business.Core.Domain.Tests.UnitTests.DataBase
{
    public class CoreConfigurationContext : NeosDatabaseContext
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="persistenceSettings">Persistence settings.</param>
        public CoreConfigurationContext(INeosTenantInfoAccessor neosTenantInfoAccessor, IUserInfoAccessor userInfoAccessor, IApplicationContext applicationContext, DbContextOptions options)
            : this(neosTenantInfoAccessor, userInfoAccessor, applicationContext, options, string.Empty, NeosDatabaseType.Unknown)
        {
        }
        protected CoreConfigurationContext(INeosTenantInfoAccessor neosTenantInfoAccessor, IUserInfoAccessor userInfoAccessor, IApplicationContext applicationContext, DbContextOptions options, string connectionString, NeosDatabaseType databaseType)
    : base(neosTenantInfoAccessor, userInfoAccessor, applicationContext, options, GetDatabaseSettings(connectionString, databaseType))
        {
        }
        /// <summary>
        /// Gets or sets Counter.
        /// </summary>
        /// <value>
        /// Counter.
        /// </value>
        public DbSet<Business.Domain.Entities.Counter> Counter { get; set; } = null!;
        public DbSet<Business.Domain.Entities.Option> Option { get; set; } = null!;

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new CounterConfiguration(Settings));
            modelBuilder.ApplyConfiguration(new OptionConfiguration(Settings));
        }
 
        private static IDatabasePersistenceSettings GetDatabaseSettings(string connectionString, NeosDatabaseType databaseType)
        {
            DatabasePersistenceSettings settings = new DatabasePersistenceSettings
            {
                ConnectionString = connectionString,
                DatabaseType = databaseType,
            };

            if (databaseType is NeosDatabaseType.Oracle or NeosDatabaseType.Unknown)
            {
                settings.DefaultConcurrencyAccessMode = DatabaseConcurrencyAccessMode.Disabled;
            }

            return settings;
        }
    }
}
