using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Slidable.Shows.Data;
using Slidable.Shows.Models;
using Slidable.Shows.Models.Live;

namespace Slidable.Shows.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ShowContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ShowContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{place}/{presenter}/{slug}")]
        public async Task<IActionResult> Latest(string place, string presenter, string slug, CancellationToken ct)
        {
            var show = await _context.Shows.SingleOrDefaultAsync(
                    s => s.Place == place && s.Presenter == presenter && s.Slug == slug, ct)
                .ConfigureAwait(false);

            if (show == null)
            {
                _logger.LogWarning($"Show not found: {place}/{presenter}/{slug}");
                return NotFound();
            }

            return RedirectToAction("Show", new {place, presenter, slug, slide = show.HighestSlideShown});
        }

        [HttpGet("{place}/{presenter}/{slug}/{slide:int}")]
        public async Task<IActionResult> Show(string place, string presenter, string slug, int slide, CancellationToken ct)
        {
            var show = await _context.Shows.SingleOrDefaultAsync(
                    s => s.Place == place && s.Presenter == presenter && s.Slug == slug, ct)
                .ConfigureAwait(false);
            if (show == null)
            {
                return NotFound();
            }

            var model = new ShowViewModel
            {
                Place = show.Place,
                Presenter = show.Presenter,
                Slug = show.Slug,
                Slide = slide,
                Title = show.Title
            };

            return View(model);
        }

        [HttpGet("{presenter}/{slug}/{number:int}/partial")]
        public async Task<ActionResult<SlidePartial>> GetSlidePartial(string place, string presenter, string slug, int number, CancellationToken ct)
        {
            var show = await _context.Shows.SingleOrDefaultAsync(
                    s => s.Place == place && s.Presenter == presenter && s.Slug == slug, ct)
                .ConfigureAwait(false);

            if (show == null || show.HighestSlideShown.GetValueOrDefault() < number) return NotFound();

            var slidePartial = new SlidePartial
            {
                SlideImageUrl = $"/slides/{place}/{presenter}/{slug}/{number}"
            };

            return slidePartial;
        }
        
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}