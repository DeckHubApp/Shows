using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DeckHub.Shows.Data;

namespace DeckHub.Shows.Migrate
{
    [PublicAPI]
    public class DesignTimeShowContextFactory : IDesignTimeDbContextFactory<ShowContext>
    {
        public const string LocalPostgres = "Host=localhost;Database=shows;Username=deckhub;Password=SecretSquirrel";

        public static readonly string MigrationAssemblyName =
            typeof(DesignTimeShowContextFactory).Assembly.GetName().Name;

        public ShowContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder()
                .UseNpgsql(args.FirstOrDefault() ?? LocalPostgres, b => b.MigrationsAssembly(MigrationAssemblyName));
            return new ShowContext(builder.Options);
        }
    }
}