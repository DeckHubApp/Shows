using System;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Slidable.Shows.Messaging
{
    public abstract class TypedQueueClient<T>
    {
        private readonly string _queueName;
        private readonly ILogger _logger;
        private readonly QueueClient _client;

        protected TypedQueueClient(string queueName, IOptions<MessagingOptions> options, ILogger logger)
        {
            _queueName = queueName;
            _logger = logger;
            
            if (string.IsNullOrWhiteSpace(options.Value.ServiceBusConnectionString))
            {
                _logger.LogWarning("No ServiceBusConnectionString configured.");
                return;
            }

            _client = new QueueClient(options.Value.ServiceBusConnectionString, queueName);
            IsConnected = true;
        }
        
        public bool IsConnected { get; }

        public void RegisterMessageHandler(Func<T, string, CancellationToken, Task> handler)
        {
            var handlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            _client.RegisterMessageHandler(async (message, ct) =>
            {
                var slide = await TryDeserialize(message);
                if (slide == null)
                {
                    return;
                }
                await handler(slide, message.SystemProperties.LockToken, ct);
            }, handlerOptions);
        }

        private ValueTask<T> TryDeserialize(Message message)
        {
            try
            {
                return new ValueTask<T>(MessagePackSerializer.Deserialize<T>(message.Body));
            }
            catch (Exception e)
            {
                return new ValueTask<T>(HandleException(e, message));
            }

            async Task<T> HandleException(Exception e, Message m)
            {
                    _logger.LogError(e, $"Error parsing {typeof(T).Name} from '{_queueName}'.");
                    if (message.SystemProperties.DeliveryCount < 4)
                    {
                        await _client.AbandonAsync(message.SystemProperties.LockToken);
                    }
                    else
                    {
                        await _client.DeadLetterAsync(message.SystemProperties.LockToken, "Cannot parse.");
                    }

                return default(T);
            }
        }

        public async Task CompleteAsync(string lockToken)
        {
            await _client.CompleteAsync(lockToken).ConfigureAwait(false);
        }

        public async Task CloseAsync()
        {
            await _client.CloseAsync().ConfigureAwait(false);
        }
        
        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            _logger.LogError(arg.Exception, $"Exception from ServiceBus: {arg.Exception.GetType().Name} - {arg.Exception.Message}");
            return Task.CompletedTask;
        }
    }
}