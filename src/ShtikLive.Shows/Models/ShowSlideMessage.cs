namespace ShtikLive.Shows.Models
{
    public class ShowSlideMessage
    {
        public string MessageType { get; set; } = "slideshown";
        public string Presenter { get; set; }
        public string Slug { get; set; }
        public int Slide { get; set; }
    }
}