using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DeckHub.Shows.Data;
using DeckHub.Shows.Models;
using DeckHub.Shows.Models.Find;

namespace DeckHub.Shows.Controllers
{
    [PublicAPI]
    [Route("find")]
    public class FindController : Controller
    {
        private readonly ShowContext _context;
        private readonly ILogger<FindController> _logger;

        public FindController(ShowContext context, ILogger<FindController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Find([FromQuery]string tag, [FromQuery]string presenter, [FromQuery]string place, CancellationToken ct)
        {
            try
            {
                var showQuery = _context.Shows.AsQueryable();

                if (!string.IsNullOrWhiteSpace(tag))
                {
                    showQuery = showQuery.Where(s => EF.Functions.Like(s.Slug, $"%{tag}%"));
                }

                if (!string.IsNullOrWhiteSpace(presenter))
                {
                    showQuery = showQuery.Where(s => EF.Functions.Like(s.Presenter, $"%{presenter}%"));
                }

                if (!string.IsNullOrWhiteSpace(place))
                {
                    showQuery = showQuery.Where(s => EF.Functions.Like(s.Place, $"%{place}%"));
                }

                var shows = await showQuery.OrderByDescending(s => s.Time).ToListAsync(ct);

                var model = new FindByTagViewModel
                {
                    Shows = shows
                };
                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}