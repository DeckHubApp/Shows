using JetBrains.Annotations;
using MessagePack;

namespace DeckHub.Shows.Messaging
{
    [MessagePackObject]
    public class SlideAvailable
    {
        [Key(0)]
        public string Place { get; set; }
        [Key(1)]
        public string Presenter { get; [UsedImplicitly] set; }
        [Key(2)]
        public string Slug { get; [UsedImplicitly] set; }
        [Key(3)]
        public int Number { get; [UsedImplicitly] set; }
    }
}