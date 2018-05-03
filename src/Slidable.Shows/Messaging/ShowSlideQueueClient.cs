using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Slidable.Shows.Messaging
{
    [UsedImplicitly]
    public class ShowSlideQueueClient : TypedQueueClient<ShowSlide>, IShowSlideQueueClient
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        public ShowSlideQueueClient(IOptions<MessagingOptions> options, ILogger<ShowSlideQueueClient> logger) : base("shows/slide", options, logger)
        {
        }
    }
}