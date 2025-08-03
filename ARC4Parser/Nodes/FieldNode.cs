namespace Aldemart.ARC4Parser.Nodes;

using System.Text.Json.Serialization;

public class FieldNode
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("type")]
    public TypeNode? Type { get; set; }
}