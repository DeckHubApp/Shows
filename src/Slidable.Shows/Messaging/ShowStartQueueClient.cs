using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Slidable.Shows.Messaging
{
    [UsedImplicitly]
    public class ShowStartQueueClient : TypedQueueClient<ShowStart>, IShowStartQueueClient
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        public ShowStartQueueClient(IOptions<MessagingOptions> options, ILogger<ShowStartQueueClient> logger) : base("shows/start", options, logger)
        {
        }
    }
}