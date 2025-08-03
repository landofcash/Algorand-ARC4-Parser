namespace Aldemart.ARC4Parser.PrimitiveDecoders;

using System.Buffers.Binary;

public class Base64Decoder : IPrimitiveDecoder
{
    public bool CanDecode(string name) => name == "base64";

    public DecodeResult Decode(BinaryReader reader, int? size = null)
    {
        if (size.HasValue)
        {
            // Fixed-size string
            var bytes = reader.ReadBytes(size.Value);
            var str = Convert.ToBase64String(bytes);
            return new DecodeResult { Value = str, Offset = size.Value };
        }
        else
        {
            // Dynamic string with 2-byte length prefix
            int length = BinaryPrimitives.ReadUInt16BigEndian(new ReadOnlySpan<byte>(reader.ReadBytes(2)));
            var bytes = reader.ReadBytes(length);
            var str = Convert.ToBase64String(bytes);
            return new DecodeResult { Value = str, Offset = length + 2 };
        }
    }
}