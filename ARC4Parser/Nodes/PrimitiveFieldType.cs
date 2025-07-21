namespace Aldemart.ARC4Parser.Nodes;

using Newtonsoft.Json;

public class PrimitiveFieldType:TypeNode
{
    [JsonProperty("size")]
    public int? Size { get; set; }
    [JsonProperty("decoder")]
    public string? Decoder { get; set; }
    [JsonProperty("converter")]
    public string? Converter { get; set; }
}