using System.Text.Json.Serialization;

namespace Aldemart.ARC4Parser.Nodes;

public class PrimitiveFieldType:TypeNode
{
    [JsonPropertyName("size")]
    public int? Size { get; set; }
    [JsonPropertyName("decoder")]
    public string? Decoder { get; set; }
    [JsonPropertyName("converter")]
    public string? Converter { get; set; }
}