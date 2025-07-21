namespace Aldemart.ARC4Parser.Nodes;

public class ArrayTypeNode : TypeNode
{
    public TypeNode? ElementType { get; set; }
    public int? Length { get; set; }
}