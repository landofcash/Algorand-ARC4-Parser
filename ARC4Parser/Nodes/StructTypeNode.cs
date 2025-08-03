namespace Aldemart.ARC4Parser.Nodes;

using System.Text.Json.Serialization;

public class StructTypeNode : TypeNode
{
    [JsonPropertyName("fields")]
    public List<FieldNode>? Fields { get; set; }
}