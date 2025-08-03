namespace Aldemart.ARC4Parser.Nodes;

using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(PrimitiveFieldType),      "primitive")]
[JsonDerivedType(typeof(StructTypeNode),          "struct")]
public class TypeNode
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}