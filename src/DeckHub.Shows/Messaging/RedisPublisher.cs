using JetBrains.Annotations;
using MessagePack;
using StackExchange.Redis;

namespace DeckHub.Shows.Messaging
{
    public class RedisPublisher
    {
        private readonly ConnectionMultiplexer _redis;

        [UsedImplicitly]
        public RedisPublisher(ConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public void PublishSlideAvailable(string place, string presenter, string slug, int number)
        {
            var m = new SlideAvailable
            {
                Place = place,
                Presenter = presenter,
                Slug = slug,
                Number = number
            };

            _redis.GetSubscriber().Publish("deckhub:slide-available", MessagePackSerializer.Serialize(m));
        }
    }
}