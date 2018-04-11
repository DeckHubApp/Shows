using Microsoft.Extensions.Configuration;

namespace Slidable.Shows.Services
{
    public class IdentityPaths : IIdentityPaths
    {
        private readonly string _prefix;

        public IdentityPaths(IConfiguration configuration)
        {
            _prefix = configuration["Runtime:IdentityPathPrefix"];
        }

        public string Login => Adjust("/Identity/Account/Login");
        public string Logout => Adjust("/Identity/Account/Logout");
        public string Manage => Adjust("/Identity/Account/Manage");

        private string Adjust(string path)
        {
            return string.IsNullOrWhiteSpace(_prefix)
                ? path
                : "/" + string.Join('/', _prefix.Trim('/'), path.Trim('/'));
        }
    }
}