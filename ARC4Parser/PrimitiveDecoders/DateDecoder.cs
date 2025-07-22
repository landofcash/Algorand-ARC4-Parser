namespace Aldemart.ARC4Parser.PrimitiveDecoders;

public class DateDecoder : IPrimitiveDecoder
{
    public bool CanDecode(string name) => name == "date";

    public DecodeResult Decode(BinaryReader reader, int? size = null)
    {
        ulong seconds = ReadUInt64BE(reader);
        if(seconds==0 || seconds>253402300799) return new DecodeResult { Value = null, Offset = 8 };
        DateTime? dt = DateTimeOffset.FromUnixTimeSeconds((long)seconds).UtcDateTime;
        return new DecodeResult { Value = dt, Offset = 8 };
    }
    
    public static ulong ReadUInt64BE(BinaryReader reader)
    {
        var bytes = reader.ReadBytes(8);
        if (bytes.Length < 8)
            throw new EndOfStreamException();

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return BitConverter.ToUInt64(bytes, 0);
    }
}