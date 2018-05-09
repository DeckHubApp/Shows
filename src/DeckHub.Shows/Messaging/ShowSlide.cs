using MessagePack;

namespace DeckHub.Shows.Messaging
{
    [MessagePackObject]
    public class ShowSlide
    {
        [Key(0)]
        public string Place { get; set; }
        [Key(1)]
        public string Presenter { get; set; }
        [Key(2)]
        public string Slug { get; set; }
        [Key(3)]
        public int Number { get; set; }

        public bool IsValid() => NoNullOrWhitespace(Place, Presenter, Slug);

        private static bool NoNullOrWhitespace(params string[] values)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < values.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(values[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}