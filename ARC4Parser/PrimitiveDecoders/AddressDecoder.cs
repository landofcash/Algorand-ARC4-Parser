namespace Aldemart.ARC4Parser.PrimitiveDecoders;

using Org.BouncyCastle.Crypto.Digests;
using SimpleBase;

public class AddressDecoder : IPrimitiveDecoder
{
    public bool CanDecode(string name) => name == "address";

    public DecodeResult Decode(BinaryReader reader, int? size = null)
    {
        byte[] slice = reader.ReadBytes(32);
        string address;
        try
        {
            address = AddressFromPublicKey(slice);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Bad address value: {ex.Message}");
        }
        return new DecodeResult { Value = address, Offset= 32 };
    }
    
    public static string AddressFromPublicKey(byte[] pubKey)
    {
        if (pubKey.Length != 32) 
            throw new ArgumentException("Public key must be 32 bytes", nameof(pubKey));

        // 1) SHA-512/256(pubKey) via BouncyCastle
        var digest = new Sha512tDigest(256);
        digest.BlockUpdate(pubKey, 0, pubKey.Length);
        byte[] hash256 = new byte[32];
        digest.DoFinal(hash256, 0);

        // 2) take last 4 bytes as checksum
        byte[] checksum = hash256.Skip(hash256.Length - 4).ToArray();

        // 3) concat pubKey + checksum
        byte[] addrBytes = pubKey.Concat(checksum).ToArray();

        // 4) Base32-encode (RFC 4648, no padding)
        //    using SimpleBase (add via NuGet: SimpleBase)
        return Base32.Rfc4648.Encode(addrBytes, padding: false);
    }

}