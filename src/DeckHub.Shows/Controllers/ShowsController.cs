using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DeckHub.Shows.Data;
using DeckHub.Shows.Models;
using DeckHub.Shows.Models.Live;

namespace DeckHub.Shows.Controllers
{
    [Route("")]
    public class ShowsController : Controller
    {
        private readonly IShowData _data;
        private readonly ILogger<ShowsController> _logger;

        public ShowsController(IShowData data, ILogger<ShowsController> logger)
        {
            _data = data;
            _logger = logger;
        }

        [HttpGet("{place}/{presenter}/{slug}")]
        public async Task<IActionResult> Latest(string place, string presenter, string slug, CancellationToken ct)
        {
            var show = await _data.Get(place, presenter, slug, ct).ConfigureAwait(false);

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
            var show = await _data.Get(place, presenter, slug, ct).ConfigureAwait(false);
            
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

        [HttpGet("{place}/{presenter}/{slug}/{number:int}/partial")]
        public async Task<ActionResult<SlidePartial>> GetSlidePartial(string place, string presenter, string slug, int number, CancellationToken ct)
        {
            var show = await _data.Get(place, presenter, slug, ct).ConfigureAwait(false);

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