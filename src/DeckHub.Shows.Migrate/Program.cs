using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using RendleLabs.EntityFrameworkCore.MigrateHelper;

namespace DeckHub.Shows.Migrate
{
    [UsedImplicitly]
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var loggerFactory = new LoggerFactory().AddConsole((_, level) => true);
            await new MigrationHelper(loggerFactory).TryMigrate(args);
        }
    }
}
