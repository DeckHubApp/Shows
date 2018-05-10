using Microsoft.Extensions.Configuration;

namespace DeckHub.Shows.Services
{
    public class IdentityPaths : IIdentityPaths
    {
        private readonly string _prefix;

        public IdentityPaths(IConfiguration configuration)
        {
            _prefix = configuration["Runtime:IdentityPathPrefix"];
        }

        public string Login => Adjust("/identity/Account/Login");
        public string Logout => Adjust("/identity/Account/Logout");
        public string Manage => Adjust("/identity/Account/Manage");

        private string Adjust(string path)
        {
            return string.IsNullOrWhiteSpace(_prefix)
                ? path
                : "/" + string.Join('/', _prefix.Trim('/'), path.Trim('/'));
        }
    }
}