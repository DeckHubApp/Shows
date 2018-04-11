using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slidable.Shows.Data;

namespace Slidable.Shows.Messaging
{
    public class ShowSlideService : IHostedService
    {
        private readonly ILogger<ShowSlideService> _logger;
        private readonly DbContextPool<ShowContext> _contextPool;
        private readonly QueueClient _client;

        public ShowSlideService(IOptions<MessagingOptions> options, ILogger<ShowSlideService> logger, DbContextPool<ShowContext> contextPool)
        {
            _logger = logger;
            _contextPool = contextPool;
            if (string.IsNullOrWhiteSpace(options.Value.ServiceBusConnectionString))
            {
                _logger.LogWarning("No ServiceBusConnectionString configured.");
                return;
            }

            _client = new QueueClient(options.Value.ServiceBusConnectionString, "shows/slide");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_client == null)
            {
                return Task.CompletedTask;
            }

            var handlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _client.RegisterMessageHandler(Handler, handlerOptions);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _client.CloseAsync();
        }

        private async Task Handler(Message message, CancellationToken ct)
        {
            var slide= MessagePackSerializer.Deserialize<ShowSlide>(message.Body);
            if (!slide.IsValid())
            {
                _logger.LogError("Invalid ShowSlide message received");
                await _client.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
            }

            try
            {
                using (var context = _contextPool.Rent())
                {
                    await context.UpdateHighestShown(slide.Place, slide.Presenter, slide.Slug, slide.Number);
                }

                await _client.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error updating database: {ex.Message}");
            }
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            _logger.LogError(arg.Exception, $"Exception from ServiceBus: {arg.Exception.GetType().Name} - {arg.Exception.Message}");
            return Task.CompletedTask;
        }
    }
}