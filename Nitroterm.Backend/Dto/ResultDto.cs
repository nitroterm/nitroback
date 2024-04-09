using System.Text.Json.Serialization;

namespace Nitroterm.Backend.Dto;

public abstract class ResultDto
{
    [JsonIgnore]
    public abstract bool Success { get; }
}

public class ResultDto<T> : ResultDto
{
    public override bool Success => true;
    public T? Data { get; set; }

    public ResultDto()
    {
        Data = default;
    }

    public ResultDto(T? data)
    {
        Data = data;
    }
}

public class ErrorResultDto : ResultDto
{
    public override bool Success => false;
    public string Slug { get; set; }
    public string Message { get; set; }

    public ErrorResultDto(string slug, string message)
    {
        Slug = slug;
        Message = message;
    }
}