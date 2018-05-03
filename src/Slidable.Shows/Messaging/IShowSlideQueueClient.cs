using System;
using System.Threading;
using System.Threading.Tasks;

namespace Slidable.Shows.Messaging
{
    public interface IShowSlideQueueClient
    {
        void RegisterMessageHandler(Func<ShowSlide, string, CancellationToken, Task> handler);
        Task CompleteAsync(string lockToken);
        Task CloseAsync();
        bool IsConnected { get; }
    }
}