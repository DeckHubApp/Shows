using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShtikLive.Shows.Data;
using ShtikLive.Shows.Models;
using StackExchange.Redis;

namespace ShtikLive.Shows.Controllers
{
    using static ResultMethods;

    [Route("slides")]
    public class SlidesController
    {
        private readonly ShowContext _context;
        private readonly ConnectionMultiplexer _redis;

        public SlidesController(ShowContext context, ConnectionMultiplexer redis)
        {
            _context = context;
            _redis = redis;
        }

        [HttpGet("{presenter}/{slug}/{number:int}")]
        public async Task<IActionResult> GetSlide(string presenter, string slug, int number, CancellationToken ct)
        {
            var slide = await _context.Slides
                .SingleOrDefaultAsync(s => s.Show.Presenter == presenter && s.Show.Slug == slug && s.Number == number, ct)
                .ConfigureAwait(false);
            return slide == null ? NotFound() : Ok(SlideDto.FromSlide(presenter, slug, slide));
        }

        [HttpGet("{presenter}/{slug}/latest")]
        public async Task<IActionResult> GetLatestSlide(string presenter, string slug, CancellationToken ct)
        {
            var slide = await _context.Slides
                .Where(s => s.Show.Presenter == presenter && s.Show.Slug == slug && s.HasBeenShown)
                .OrderByDescending(s => s.Number)
                .Take(1)
                .SingleOrDefaultAsync(ct)
                .ConfigureAwait(false);
            return slide == null ? NotFound() : Ok(SlideDto.FromSlide(presenter, slug, slide));
        }

        [HttpPut("show/{presenter}/{slug}/{number}")]
        public async Task<IActionResult> ShowSlide(string presenter, string slug, int number, CancellationToken ct)
        {
            var slide = await _context.Slides
                .SingleOrDefaultAsync(s => s.Show.Presenter == presenter && s.Show.Slug == slug && s.Number == number, ct)
                .ConfigureAwait(false);

            if (slide == null) return NotFound();

            if (!slide.HasBeenShown)
            {
                slide.HasBeenShown = true;
                await _context.SaveChangesAsync(ct);
            }

            await PublishMessage(presenter, slug, number).ConfigureAwait(false);

            return Accepted();
        }

        private Task PublishMessage(string presenter, string slug, int number)
        {
            var message = new ShowSlideMessage
            {
                Presenter = presenter,
                Slug = slug,
                Slide = number
            };
            return _redis.GetDatabase().PublishAsync("shtik:messaging", JsonConvert.SerializeObject(message));
        }
    }
}