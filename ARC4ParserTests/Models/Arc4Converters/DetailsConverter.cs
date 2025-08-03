namespace Aldemart.ARC4ParserTests.Models.Arc4Converters
{
    using Aldemart.ARC4Parser;
    using Aldemart.ARC4Parser.Converters;
    
    public class DetailsConverter : IPrimitiveConverter
    {
        private const string DETAILS_SEPARATOR = "‡";

        /// <summary>
        /// Converts the raw decompressed byte‐array into a ProductDetails instance.
        /// </summary>
        public object Convert(object decodedValue)
        {
            if (decodedValue is not byte[] rawBytes)
                throw new ArgumentException("DetailsConverter expects a byte[] as input.");

            // Decompress (stubbed)
            string rawDetails = TextCompressor.Decompress(rawBytes);

            // Split into [description, ...imageUrls, name]
            var parts = rawDetails.Split(DETAILS_SEPARATOR);
            if (parts.Length < 2)
                throw new FormatException("Invalid details format.");

            return new ProductDetails
            {
                Name = parts[0],
                ImageUrls = [..parts[1..^1]],
                Description = parts[^1]
            };
        }

        /// <summary>
        /// Packs a ProductDetails back into a compressed byte[] buffer.
        /// </summary>
        public byte[] Cook(ProductDetails details)
        {
            if (details == null)
                throw new ArgumentNullException(nameof(details));

            // Reconstruct in the same order: description, imageUrls..., name
            var ordered = new List<string> { details.Name };
            ordered.AddRange(details.ImageUrls);
            ordered.Add(details.Description);

            string rawDetails = string.Join(DETAILS_SEPARATOR, ordered);
            return TextCompressor.Compress(rawDetails);
        }
    }
}
