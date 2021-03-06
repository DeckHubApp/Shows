using System;
using System.ComponentModel.DataAnnotations;

namespace DeckHub.Shows.Data
{
    public class Show
    {
        public int Id { get; set; }

        [MaxLength(16)]
        public string Place { get; set; }

        [MaxLength(16)]
        public string Presenter { get; set; }

        [MaxLength(16)]
        public string Slug { get; set; }

        [MaxLength(256)]
        public string Title { get; set; }

        public DateTimeOffset Time { get; set; }

        public int? HighestSlideShown { get; set; }
    }
}