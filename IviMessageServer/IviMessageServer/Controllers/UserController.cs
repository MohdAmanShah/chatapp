using Microsoft.AspNetCore.Mvc;
using IviMessageServer.DataModels;
using System.Text.Json;
using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;
using IviMessageServer.Repository.Interface;
using System.Collections.Generic;
using IviMessageServer.Services;
namespace IviMessageServer.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class UserController : ControllerBase
    {
        private IUnitOfWork unitOfWork;
        private ISocket socket;
        public UserController(IUnitOfWork unitOfWork, ISocket socket)
        {
            this.unitOfWork = unitOfWork;
            this.socket = socket;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Connected");
        }
        [HttpPost("add")]
        public IActionResult UserLogin([FromBody] User user)
        {
            user.Id = unitOfWork.UserRepository.AddUser(user);
            string result = JsonSerializer.Serialize(user);
            return Ok(result);
        }

        [HttpGet("GetUser/{id:int?}")]
        public IActionResult GetUser(int id)
        {
            Chat chat = unitOfWork.UserRepository.SelectRandomUser(id);
            return Ok(JsonSerializer.Serialize(chat));
        }
        [HttpDelete("deleteChat/{id:int?}")]
        public IActionResult DeleteChat(int id, [FromQuery] int RecipientId, [FromQuery] int InitiatorId)
        {
            string result = unitOfWork.UserRepository.DeleteChat(id);
            socket.HandleChatClose(new int[] { RecipientId }, InitiatorId).GetAwaiter();
            return Ok(result);
        }

        [HttpGet("ws/{id}")]
        public async Task GetConnection(int id)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var websocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await socket.HandleConnections(websocket, id);
                int[] Ids = unitOfWork.UserRepository.DeleteChats(id);
                socket.HandleChatClose(Ids, id);
                unitOfWork.UserRepository.RemoveUser(id);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
