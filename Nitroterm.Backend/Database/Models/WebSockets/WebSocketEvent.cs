using System.Text.Json;

namespace Nitroterm.Backend.Database.Models.WebSockets;

public class WebSocketEvent
{
    public virtual string Kind { get; set; }
    public virtual object Data { get; set; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
    }
}

public class WebSocketEvent<T> : WebSocketEvent
{
    public T CastData
    {
        get => (T)Data;
        set => Data = value;
    }

    public WebSocketEvent(string kind, T data)
    {
        Kind = kind;
        Data = data;
    }
}