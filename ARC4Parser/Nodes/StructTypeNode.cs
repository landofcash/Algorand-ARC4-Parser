namespace Aldemart.ARC4Parser.Nodes;

using Newtonsoft.Json;

public class StructTypeNode : TypeNode
{
    [JsonProperty("fields")]
    public List<FieldNode>? Fields { get; set; }
}