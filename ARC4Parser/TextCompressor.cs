namespace Aldemart.ARC4Parser;

using System.IO.Compression;
using System.Text;

/// <summary>
/// Compresses and decompresses UTF-8 strings using raw DEFLATE,
/// in parity with TS pako.deflate / pako.inflate.
/// </summary>
public static class TextCompressor
{
    /// <summary>
    /// Compresses a UTF-8 string into a raw DEFLATE byte array.
    /// </summary>
    public static byte[] Compress(string text)
    {
        if (text is null) throw new ArgumentNullException(nameof(text));

        var input = Encoding.UTF8.GetBytes(text);
        using var output = new MemoryStream();
        // leaveOpen: true so we can read output after disposing the stream
        using (var ds = new DeflateStream(output, CompressionLevel.Optimal, leaveOpen: true))
        {
            ds.Write(input, 0, input.Length);
        }
        return output.ToArray();
    }

    /// <summary>
    /// Decompresses a raw DEFLATE byte array back into a UTF-8 string.
    /// </summary>
    public static string Decompress(byte[] data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));

        using var input = new MemoryStream(data);
        using var ds = new DeflateStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        ds.CopyTo(output);
        return Encoding.UTF8.GetString(output.ToArray());
    }

    /// <summary>
    /// Returns how many bytes Compress(text) would produce.
    /// </summary>
    public static int GetCompressedLength(string text)
    {
        return Compress(text).Length;
    }
}