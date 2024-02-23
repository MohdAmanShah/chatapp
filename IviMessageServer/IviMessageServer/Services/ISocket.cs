using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace IviMessageServer.Services
{
    public interface ISocket
    {
        Task HandleConnections(WebSocket webSocket, int Id);
        Task HandleChatClose(int[] Ids,int InitiatorId);
    }
}
