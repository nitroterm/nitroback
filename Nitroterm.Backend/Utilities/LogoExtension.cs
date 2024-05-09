using Microsoft.OpenApi;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace Nitroterm.Backend.Utilities;

public class LogoExtension(string url, string backgroundColor, string altText) : IOpenApiExtension
{
    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        writer.WriteStartObject();
        writer.WriteProperty("url", url);
        writer.WriteProperty("backgroundColor", backgroundColor);
        writer.WriteProperty("altText", altText);
        writer.WriteEndObject();
    }
}