using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nitroterm.Backend.Database.Models.WebSockets;
using Nitroterm.Backend.Services;

namespace Nitroterm.Backend.Controllers;

[ApiController]
[Route("/api/nitroterm/v1/ws")]
public class WebSocketsController : ControllerBase
{
    private IEventService _ws;
    public WebSocketsController(IEventService ws)
    {
        _ws = ws;
    }
    
    [HttpGet("feed")]
    public async Task OpenFeedWebSocket()
    {
        HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
        
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _ws.RegisterWebSocket(webSocket);

            return;
        }

        HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
}