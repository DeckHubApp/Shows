using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ShtikLive.Shows.Data;

namespace ShtikLive.Shows.Models
{
    public class ShowDto
    {
        [MaxLength(16)]
        public string Presenter { get; set; }

        [MaxLength(256)]
        public string Slug { get; set; }

        [MaxLength(256)]
        public string Title { get; set; }

        public DateTimeOffset Time { get; set; }

        [MaxLength(256)]
        public string Place { get; set; }

        public int? HighestSlideShown { get; set; }

        public static ShowDto FromShow(Show show)
        {
            return new ShowDto
            {
                Presenter = show.Presenter,
                Slug = show.Slug,
                Title = show.Title,
                Time = show.Time,
                Place = show.Place,
                HighestSlideShown = show.HighestSlideShown
            };
        }
    }
}