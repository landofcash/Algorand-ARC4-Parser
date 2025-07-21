namespace Aldemart.ARC4Parser.PrimitiveDecoders;

public interface IPrimitiveDecoder
{
    public bool CanDecode(string name);
    public DecodeResult Decode(BinaryReader reader, int? size = null);
}