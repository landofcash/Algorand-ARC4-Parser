using System.Buffers.Binary;

namespace Aldemart.ARC4Parser.PrimitiveDecoders;

public class BytesDecoder : IPrimitiveDecoder
{
    public bool CanDecode(string name) => name == "bytes";

    public DecodeResult Decode(BinaryReader reader, int? size = null)
    {
        if (size.HasValue)
        {
            var data = reader.ReadBytes(size.Value);
            return new DecodeResult { Value = data, Offset = size.Value };
        }
        else
        {
            ushort length = BinaryPrimitives.ReadUInt16BigEndian(reader.ReadBytes(2));
            var data = reader.ReadBytes(length);
            return new DecodeResult { Value = data, Offset = length + 2 };
        }
    }
}