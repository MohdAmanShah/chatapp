using IviMessageServer.DataModels;
using IviMessageServer.Repository;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using System.Collections.Concurrent;

namespace IviMessageServer.Services
{
    public class Socket : ISocket
    {
        private ConcurrentDictionary<WebSocket, int> connections;
        public Socket()
        {
            connections = new ConcurrentDictionary<WebSocket, int>();
        }
        public async Task HandleConnections(WebSocket webSocket, int Id)
        {
            connections.TryAdd(webSocket, Id);
            try
            {
                var buffer = new byte[1024 * 4];
                var receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!receiveResult.CloseStatus.HasValue)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    Payload payload = JsonSerializer.Deserialize<Payload>(message);
                    var socket = connections.FirstOrDefault(x => x.Value == payload.DestinationId).Key;
                    await socket.SendAsync(
                        new ArraySegment<byte>(messageBytes),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);

                    receiveResult = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), CancellationToken.None);
                }

                connections.TryRemove(webSocket, out int CID);
                await webSocket.CloseAsync(
                    receiveResult.CloseStatus.Value,
                    receiveResult.CloseStatusDescription,
                    CancellationToken.None);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task HandleChatClose(int[] Ids, int initiatorId)
        {
            foreach (var Id in Ids)
            {
                try
                {
                    var socket = connections.FirstOrDefault(x => x.Value == Id).Key;
                    var data = new { close = initiatorId };
                    string jsonString = JsonSerializer.Serialize(data);
                    byte[] messageBytes = Encoding.UTF8.GetBytes(jsonString);
                    await socket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch { }
            }
        }

    }
}
