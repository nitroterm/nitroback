using System.Net.WebSockets;
using System.Text;
using Nitroterm.Backend.Database.Models.WebSockets;

namespace Nitroterm.Backend.Services;

public class WebSocketsService : IEventService
{
    public List<WebSocket> WebSockets { get; set; } = [];
    
    public async void SendEvent(WebSocketEvent wsEvent)
    {
        List<WebSocket> wsToDelete = [];
        string json = wsEvent.ToJson();
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        foreach (WebSocket ws in WebSockets)
        {
            if (ws.CloseStatus.HasValue)
            {
                wsToDelete.Add(ws);
                continue;
            }

            // Send the event to websockets
            try
            {
                await ws.SendAsync(jsonBytes, WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage,
                    CancellationToken.None);
            }
            catch (Exception e)
            {
                wsToDelete.Add(ws);
            }
        }

        WebSockets.RemoveAll(wsToDelete.Contains);
    }

    public void RegisterWebSocket(WebSocket ws)
    {
        WebSockets.Add(ws);
    }

    private static async Task ProcessWebSocket(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}