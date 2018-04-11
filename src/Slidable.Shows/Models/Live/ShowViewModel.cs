namespace Slidable.Shows.Models.Live
{
    public class ShowViewModel
    {
        public string Place { get; set; }
        public string Presenter { get; set; }
        public string Slug { get; set; }
        public int Slide { get; set; }
        public string Title { get; set; }
    }

    public class SlidePartial
    {
        public string SlideImageUrl { get; set; }
    }

}