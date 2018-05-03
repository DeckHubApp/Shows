using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Slidable.Shows.Data;

namespace Slidable.Shows.Messaging
{
    public class ShowSlideProcessor : IHostedService
    {
        private readonly IShowSlideQueueClient _queueClient;
        private readonly ILogger<ShowSlideProcessor> _logger;
        private readonly DbContextPool<ShowContext> _contextPool;
        private readonly RedisPublisher _redis;

        [UsedImplicitly]
        public ShowSlideProcessor(IShowSlideQueueClient queueClient, ILogger<ShowSlideProcessor> logger, DbContextPool<ShowContext> contextPool, RedisPublisher redis)
        {
            _queueClient = queueClient;
            _logger = logger;
            _contextPool = contextPool;
            _redis = redis;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_queueClient.IsConnected)
            {
                _queueClient.RegisterMessageHandler(Handler);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _queueClient.CloseAsync();
        }

        private async Task Handler(ShowSlide slide, string lockToken, CancellationToken ct)
        {
            if (!slide.IsValid())
            {
                _logger.LogError("Invalid ShowSlide message received");
                await _queueClient.CompleteAsync(lockToken).ConfigureAwait(false);
            }

            try
            {
                using (var context = _contextPool.Rent())
                {
                    await context.UpdateHighestShown(slide.Place, slide.Presenter, slide.Slug, slide.Number);
                }

                await _queueClient.CompleteAsync(lockToken).ConfigureAwait(false);
                _redis.PublishSlideAvailable(slide.Place, slide.Presenter, slide.Slug, slide.Number);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating database: {ex.Message}");
            }
        }
    }
}