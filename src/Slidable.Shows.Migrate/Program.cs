using System.Threading.Tasks;
using JetBrains.Annotations;
using RendleLabs.EntityFrameworkCore.MigrateHelper;

namespace Slidable.Shows.Migrate
{
    [UsedImplicitly]
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await new MigrationHelper().TryMigrate(args);
        }
    }
}
