namespace Aldemart.ARC4Parser.PrimitiveDecoders;

public class DateDecoder : IPrimitiveDecoder
{
    public bool CanDecode(string name) => name == "date";

    public DecodeResult Decode(BinaryReader reader, int? size = null)
    {
        ulong seconds = ReadUInt64BE(reader);
        DateTime? dt = seconds == 0 ? (DateTime?)null : DateTimeOffset.FromUnixTimeSeconds((long)seconds).UtcDateTime;
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