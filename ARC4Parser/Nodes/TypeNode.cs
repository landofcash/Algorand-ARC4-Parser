namespace Aldemart.ARC4Parser.Nodes;

using Newtonsoft.Json;

[JsonConverter(typeof(TypeNodeConverter))]
public class TypeNode
{
    [JsonProperty("kind")]
    public string? Kind { get; set; }
    [JsonProperty("name")]
    public string? Name { get; set; }
}