using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slidable.Shows.Data;

namespace Slidable.Shows.Messaging
{
    [UsedImplicitly]
    public class ShowStartProcessor : IHostedService
    {
        private readonly IShowStartQueueClient _client;
        private readonly ILogger<ShowStartProcessor> _logger;
        private readonly DbContextPool<ShowContext> _contextPool;

        public ShowStartProcessor(IShowStartQueueClient client, ILogger<ShowStartProcessor> logger, DbContextPool<ShowContext> contextPool)
        {
            _client = client;
            _logger = logger;
            _contextPool = contextPool;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_client.IsConnected)
            {
                _client.RegisterMessageHandler(Handler);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _client.CloseAsync();
        }

        private async Task Handler(ShowStart showMessage, string lockToken, CancellationToken ct)
        {
            if (!showMessage.IsValid())
            {
                _logger.LogError("Invalid ShowStart message received");
                await _client.CompleteAsync(lockToken).ConfigureAwait(false);
                return;
            }

            var show = new Show
            {
                Place = showMessage.Place,
                Presenter = showMessage.Presenter,
                Slug = showMessage.Slug,
                HighestSlideShown = 0,
                Title = showMessage.Title,
                Time = showMessage.Time
            };

            using (var context = _contextPool.Rent())
            {
                if (!await context.Shows.AnyAsync(
                    s => s.Place == show.Place && s.Presenter == show.Presenter && s.Slug == show.Slug, ct))
                {
                    context.Shows.Add(show);
                    await context.SaveChangesAsync(ct).ConfigureAwait(false);
                }
            }

            await _client.CompleteAsync(lockToken).ConfigureAwait(false);
        }
    }
}