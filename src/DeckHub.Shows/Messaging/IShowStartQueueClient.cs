using System;
using System.Threading;
using System.Threading.Tasks;

namespace DeckHub.Shows.Messaging
{
    public interface IShowStartQueueClient
    {
        bool IsConnected { get; }
        void RegisterMessageHandler(Func<ShowStart, string, CancellationToken, Task> handler);
        Task CompleteAsync(string lockToken);
        Task CloseAsync();
    }
}