using System;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Slidable.Shows.Data;

namespace Slidable.Shows.Messaging
{
    public class ShowStartService : IHostedService
    {
        private readonly ILogger<ShowStartService> _logger;
        private readonly DbContextPool<ShowContext> _contextPool;
        private readonly QueueClient _client;

        public ShowStartService(IOptions<MessagingOptions> options, ILogger<ShowStartService> logger, DbContextPool<ShowContext> contextPool)
        {
            _logger = logger;
            _contextPool = contextPool;
            if (string.IsNullOrWhiteSpace(options.Value.ServiceBusConnectionString))
            {
                _logger.LogWarning("No ServiceBusConnectionString configured.");
                return;
            }

            _client = new QueueClient(options.Value.ServiceBusConnectionString, "shows/start");
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
            ShowStart showMessage;
            try
            {
                showMessage = MessagePackSerializer.Deserialize<ShowStart>(message.Body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid ShowStart message received.");
                await _client.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
                return;
            }
            
            if (!showMessage.IsValid())
            {
                _logger.LogError("Invalid ShowStart message received");
                await _client.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
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

            await _client.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            _logger.LogError(arg.Exception, $"Exception from ServiceBus: {arg.Exception}");
            return Task.CompletedTask;
        }
    }
}