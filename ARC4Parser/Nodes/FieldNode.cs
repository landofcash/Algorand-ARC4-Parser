namespace Aldemart.ARC4Parser.Nodes;

using Newtonsoft.Json;

public class FieldNode
{
    [JsonProperty("name")]
    public string? Name { get; set; }
    [JsonProperty("type")]
    public TypeNode? Type { get; set; }
}