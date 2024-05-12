using System.Net.WebSockets;
using Nitroterm.Backend.Database.Models.WebSockets;

namespace Nitroterm.Backend.Services;

public interface IEventService
{
    void SendEvent(WebSocketEvent wsEvent);
    void RegisterWebSocket(WebSocket ws);
}