using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DeckHub.Shows.Data
{
    public interface IShowData
    {
        Task<Show> Get(string place, string presenter, string slug, CancellationToken ct = default);
    }

    public class ShowData : IShowData
    {
        private readonly ShowContext _context;

        public ShowData(ShowContext context)
        {
            _context = context;
        }

        public async Task<Show> Get(string place, string presenter, string slug, CancellationToken ct = default)
        {
            return await _context.Shows.SingleOrDefaultAsync(
                    s => s.Place == place && s.Presenter == presenter && s.Slug == slug, ct);
        }
    }
}