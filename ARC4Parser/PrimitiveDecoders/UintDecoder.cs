namespace Aldemart.ARC4Parser.PrimitiveDecoders;

using System.Numerics;
using ARC4Parser.ARC4Types;

public class UintDecoder : IPrimitiveDecoder
{
    public bool CanDecode(string name) => name == "uint" || name == "byte";

    public DecodeResult Decode(BinaryReader reader, int? size = null)
    {
        if (!size.HasValue)
            throw new InvalidOperationException("Size required for uint decoding.");

        int byteLen = size.Value;
        byte[] buf = reader.ReadBytes(byteLen);
        var v = BigInteger.Zero;
        foreach (var b in buf)
        {
            v = (v << 8) + b;
        }

        return new DecodeResult
        {
            Value = new ARC4Int(v),
            Offset = byteLen
        };
    }
}