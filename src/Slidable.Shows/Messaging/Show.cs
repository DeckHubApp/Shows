using System;
using MessagePack;

namespace Slidable.Shows.Messaging
{
    [MessagePackObject]
    public class ShowStart
    {
        [Key(0)]
        public string Place { get; set; }
        [Key(1)]
        public string Presenter { get; set; }
        [Key(2)]
        public string Slug { get; set; }
        [Key(3)]
        public string Title { get; set; }
        [Key(4)]
        public DateTimeOffset Time { get; set; }

        public bool IsValid() => NoNullOrWhitespace(Place, Presenter, Slug, Title);

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