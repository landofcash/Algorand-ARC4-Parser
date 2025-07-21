namespace Aldemart.ARC4ParserTests.Models
{
    using System.Numerics;
    using ARC4Parser;
    using ARC4ParserTests.Models.Arc4Converters;


    /// <summary>
    /// Represents a product as stored on-chain, with all primitive types inlined.
    /// </summary>
    public class Product
    {
        /// <summary>4 B: product data structure version</summary>
        [Arc4Property("version")]
        public uint Version { get; set; }

        /// <summary>22 B: Product Seed (compact GUID), encoded as hex/base64</summary>
        [Arc4Property("seed")]
        public string Seed { get; set; } = "";

        /// <summary>8 B: product creation timestamp (nullable Unix epoch)</summary>
        [Arc4Property("createdDate")]
        public DateTime? CreatedDate { get; set; }

        /// <summary>8 B: price in micro-tokens</summary>
        [Arc4Property("price")]
        public ulong Price { get; set; }

        /// <summary>8 B: ASA id, or 0 for ALGO</summary>
        [Arc4Property("priceToken")]
        public ulong PriceToken { get; set; }

        /// <summary>2 B: royalty % in basis-points (bps)</summary>
        [Arc4Property("royalty")]
        public ushort Royalty { get; set; }

        /// <summary>32 B: seller Algorand address</summary>
        [Arc4Property("seller")]
        public string Seller { get; set; } = "";

        /// <summary>8 B: category identifier</summary>
        [Arc4Property("categoryId")]
        public ulong CategoryId { get; set; }

        /// <summary>22 B: group seed; equals `seed` for the main product</summary>
        [Arc4Property("groupSeed")]
        public string GroupSeed { get; set; } = "";

        /// <summary>8 B: expiration timestamp (nullable Unix epoch)</summary>
        [Arc4Property("expirationDate")]
        public DateTime? ExpirationDate { get; set; }

        /// <summary>4 B: total codes available</summary>
        [Arc4Property("totalCodes")]
        public uint TotalCodes { get; set; }

        /// <summary>4 B: number of codes sold</summary>
        [Arc4Property("orderCount")]
        public uint OrderCount { get; set; }

        /// <summary>16 B: bitmap of code availability (0 = available, 1 = reserved/sold)</summary>
        [Arc4Property("codesBitmap")]
        public BigInteger CodesBitmap { get; set; }

        /// <summary>Variable-length metadata & URI/CID</summary>
        [Arc4Property("codeManifest")]
        public CodeManifest? CodeManifest { get; set; }

        /// <summary>Variable-length product description/details</summary>
        [Arc4Property("details", typeof(DetailsConverter))]
        public ProductDetails Details { get; set; } = new();
    }
    
    
    /// <summary>
    /// Variable-length product description/details.
    /// </summary>
    public class ProductDetails
    {
        /// <summary>Name of the product.</summary>
        public string Name { get; set; } = "";

        /// <summary>List of image URLs.</summary>
        public List<string> ImageUrls { get; set; } = [""];

        /// <summary>Product description.</summary>
        public string Description { get; set; } = "";
    }
}
