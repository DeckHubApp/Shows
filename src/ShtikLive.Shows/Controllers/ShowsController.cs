using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShtikLive.Shows.Data;
using ShtikLive.Shows.Models;

namespace ShtikLive.Shows.Controllers
{
    using static ResultMethods;

    [Route("shows")]
    public class ShowsController
    {
        private readonly ShowContext _context;
        private readonly ILogger<ShowsController> _logger;

        public ShowsController(ShowContext context, ILogger<ShowsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] Show show, CancellationToken ct)
        {
            try
            {
                _context.Shows.Add(show);
                await _context.SaveChangesAsync(ct).ConfigureAwait(false);
                return CreatedAtAction("Get", "Shows", new { presenter = show.Presenter, slug = show.Slug }, ShowDto.FromShow(show));

            }
            catch (System.Exception ex)
            {
                _logger.LogError(EventIds.DatabaseError, ex, ex.Message);
                throw;
            }
            
        }

        [HttpGet("{presenter}/{slug}")]
        public async Task<IActionResult> Get(string presenter, string slug, CancellationToken ct)
        {
            var show = await _context.Shows.SingleOrDefaultAsync(s => s.Presenter == presenter && s.Slug == slug, ct).ConfigureAwait(false);
            return show == null ? NotFound() : Ok(ShowDto.FromShow(show));
        }

        [HttpPut("{presenter}/{slug}")]
        public async Task<IActionResult> Set(string presenter, string slug, [FromQuery] int? highestSlideShown,
            CancellationToken ct)
        {
            if (!highestSlideShown.HasValue) return new BadRequestResult();

            var rowsUpdated = await _context.Database.ExecuteSqlCommandAsync(
                @"UPDATE ""Shows"" SET ""HighestSlideShown"" = CASE WHEN ""HighestSlideShown"" > {0} THEN ""HighestSlideShown"" ELSE {0} END
                  WHERE ""Presenter"" = {1} AND ""Slug"" = {2}",
                highestSlideShown.Value, presenter, slug
            );

            return rowsUpdated > 0 ? Accepted() : NotFound();
        }

        [HttpGet("find/by/{handle}")]
        public async Task<IActionResult> ListByPresenter(string handle, CancellationToken ct)
        {
            var shows = await _context.Shows
                .Where(s => s.Presenter == handle)
                .OrderByDescending(s => s.Time)
                .ToListAsync(ct)
                .ConfigureAwait(false);
            return Ok(shows.Select(ShowDto.FromShow).ToList());
        }

        [HttpGet("find/by/{handle}/latest")]
        public async Task<IActionResult> LatestByPresenter(string handle, CancellationToken ct)
        {
            var show = await _context.Shows
                .Where(s => s.Presenter == handle)
                .OrderByDescending(s => s.Time)
                .Take(1)
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);
            return show == null ? NotFound() : Ok(ShowDto.FromShow(show));
        }
    }
}
