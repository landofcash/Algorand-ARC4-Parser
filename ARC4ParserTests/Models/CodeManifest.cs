namespace Aldemart.ARC4ParserTests.Models;

using ARC4Parser;

/// <summary>
/// Describes a code manifest, including storage type and location.
/// </summary>
public class CodeManifest
{
    /// <summary>
    /// 2 B: manifest type enum (1=CDN, 2=IPFS, 3=BOX, …)
    /// </summary>
    [Arc4Property("manifestType")]
    public ushort ManifestType { get; set; }

    /// <summary>
    /// Variable-length: CDN/IPFS URL or box name (10–100 B)
    /// </summary>
    [Arc4Property("dataLocation")]
    public string? DataLocation { get; set; }
}