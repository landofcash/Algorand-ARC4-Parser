namespace Aldemart.ARC4Parser.PrimitiveDecoders;

public class BoolDecoder : IPrimitiveDecoder
{
    public bool CanDecode(string name) => name == "bool";

    public DecodeResult Decode(BinaryReader reader, int? size = null)
    {
        byte b = reader.ReadByte();
        bool val = (b & 0x80) != 0;
        return new DecodeResult { Value = val, Offset = 1 };
    }
}