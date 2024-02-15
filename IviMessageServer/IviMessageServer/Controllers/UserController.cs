using Microsoft.AspNetCore.Mvc;
using IviMessageServer.Data_Access;
using IviMessageServer.DataModels;
using System.Text.Json;
using System.Runtime.InteropServices;
using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
namespace IviMessageServer.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UserController : ControllerBase
    {
        #region Controllers
        private readonly DataAccess da;
        private ConcurrentDictionary<WebSocket, int> connections;
        public UserController()
        {
            connections = new ConcurrentDictionary<WebSocket, int>();
            this.da = new DataAccess();
        }
        [HttpPost("adduser")]
        public IActionResult Post([FromBody] User user)
        {
            user.Id = da.AddUser(user);
            string result = JsonSerializer.Serialize(user);
            return Ok(result);
        }
        [HttpDelete("removeuser/{Id?}")]
        public IActionResult DeleteUser(int id)
        {
            da.RemoveUser(id);
            return Ok(id.ToString());
        }

        [HttpGet("ws/{id}")]
        public async Task GetConnection(int id)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var websocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await HandleConnections(websocket, id);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        #endregion Controllers

        #region SocketHandlers
        private async Task HandleConnections(WebSocket webSocket, int Id)
        {
            connections.TryAdd(webSocket, Id);
            try
            {
                var buffer = new byte[1024 * 4];
                var receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);

                string message = $"Hello Client {connections[webSocket]}";
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                while (!receiveResult.CloseStatus.HasValue)
                {
                    await webSocket.SendAsync(
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
        #endregion SocketHandlers
    }
}
