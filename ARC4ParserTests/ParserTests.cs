namespace ARC4ParserTests
{
    using Aldemart.ARC4Parser;
    using Aldemart.ARC4Parser.Nodes;
    using Aldemart.ARC4ParserTests.Models;
    using Newtonsoft.Json;
    using System;
    using System.Buffers.Binary;
    using Aldemart.ARC4Parser.PrimitiveDecoders;
    using Microsoft.Extensions.Logging;

    [TestFixture]
    internal class ParseProductTests : BaseTests
    {
        private StructTypeNode LoadStructType(string fileName)
        {
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", fileName);
            string json = File.ReadAllText(jsonFilePath);
            StructTypeNode? structTypeNode = JsonConvert.DeserializeObject<StructTypeNode>(json);
            if (structTypeNode == null) throw new Exception("Can't deserialize json");
            Logger.LogInformation($"Kind: {structTypeNode.Kind} Name: {structTypeNode.Name}");
            return structTypeNode;
        }

        [Test]
        public void TestNodeParser()
        {
            var structTypeNode = LoadStructType("productStructNode.json");
            string base64EncodedData = "AAAAAW1YamFmUW44UWtDQjUyaWlvd0JMY2cAAAAAaHQX5QAAAAAAHl1wAAAAAAAAAAAAZDn57aMrdoS8ODz5n0n0D9frQzN/x2eW4aODv2yHiiG7AAAAAAAAAA" +
                                       "FtWGphZlFuOFFrQ0I1Mmlpb3dCTGNnAAAAAGiKo/AAAAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAlgDVAAEABAA5aHR0cHM6Ly9jZG4uYWxkZW1hcnQuY29tL2NvZGVzL2" +
                                       "1YamFmUW44UWtDQjUyaWlvd0JMY2cudHh0ADlzyywqLlHISy1XKCjKTylNLnnUsFA/Kb8itRhE6qbrFeSlA4W8SoGqEsHqSlKBTKhiLpBATmZeKgA=";
            byte[] buffer = Convert.FromBase64String(base64EncodedData);
           
            Arc4Converter converter = new Arc4Converter(Logger);
            Product product = converter.Process<Product>(structTypeNode, buffer);
            
            string expectedResult =
                """{"Version":1,"Seed":"mXjafQn8QkCB52iiowBLcg","CreatedDate":"2025-07-13T20:32:37Z","Price":1990000,"PriceToken":0,"Royalty":100,"Seller":"HH463IZLO2CLYOB47GPUT5AP27VUGM37Y5TZNYNDQO7WZB4KEG53YIXHAQ","CategoryId":1,"GroupSeed":"mXjafQn8QkCB52iiowBLcg","ExpirationDate":"2025-07-30T23:00:00Z","TotalCodes":20,"OrderCount":0,"CodesBitmap":0,"CodeManifest":{"ManifestType":1,"DataLocation":"https://cdn.aldemart.com/codes/mXjafQn8QkCB52iiowBLcg.txt"},"Details":{"Name":"First new product","ImageUrls":["/boxes/box-g.png"],"Description":"Just a new test product\nnew line"}}""";
            string result = JsonConvert.SerializeObject(product);
            Logger.LogInformation($"{result}");
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        
        [Test]
        public void TestCustomDecoders()
        {
            var structTypeNode = LoadStructType("customDecoderProductStructNode.json");
            string base64EncodedData = "AAAAAW1YamFmUW44UWtDQjUyaWlvd0JMY2cAAAAAaHQX5QAAAAAAHl1wAAAAAAAAAAAAZDn57aMrdoS8ODz5n0n0D9frQzN/x2eW4aODv2yHiiG7AAAAAAAAAA" +
                                       "FtWGphZlFuOFFrQ0I1Mmlpb3dCTGNnAAAAAGiKo/AAAAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAlgDVAAEABAA5aHR0cHM6Ly9jZG4uYWxkZW1hcnQuY29tL2NvZGVzL2" +
                                       "1YamFmUW44UWtDQjUyaWlvd0JMY2cudHh0ADlzyywqLlHISy1XKCjKTylNLnnUsFA/Kb8itRhE6qbrFeSlA4W8SoGqEsHqSlKBTKhiLpBATmZeKgA=";
            byte[] buffer = Convert.FromBase64String(base64EncodedData);
            
            Arc4Converter converter = new Arc4Converter(Logger);
            Arc4Parser parser = new Arc4Parser(Logger);
            parser.PrimitiveDecoderRegistry.RegisterDecoder(new ReversedStringDecoder());
            Product product = converter.Process<Product>(parser, structTypeNode, buffer);
            
            string expectedResult =
                """{"Version":1,"Seed":"gcLBwoii25BCkQ8nQfajXm","CreatedDate":"2025-07-13T20:32:37Z","Price":1990000,"PriceToken":0,"Royalty":100,"Seller":"HH463IZLO2CLYOB47GPUT5AP27VUGM37Y5TZNYNDQO7WZB4KEG53YIXHAQ","CategoryId":1,"GroupSeed":"bVhqYWZRbjhRa0NCNTJpaW93QkxjZw==","ExpirationDate":"2025-07-30T23:00:00Z","TotalCodes":20,"OrderCount":0,"CodesBitmap":0,"CodeManifest":{"ManifestType":1,"DataLocation":"https://cdn.aldemart.com/codes/mXjafQn8QkCB52iiowBLcg.txt"},"Details":{"Name":"First new product","ImageUrls":["/boxes/box-g.png"],"Description":"Just a new test product\nnew line"}}""";
            string result = JsonConvert.SerializeObject(product);
            Logger.LogInformation($"{result}");
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
    
    public class ReversedStringDecoder : IPrimitiveDecoder
    {
        public bool CanDecode(string name) => name == "reversed_string";

        public DecodeResult Decode(BinaryReader reader, int? size = null)
        {
            if (size.HasValue)
            {
                // Fixed-size string
                var bytes = reader.ReadBytes(size.Value);
                var original = System.Text.Encoding.UTF8.GetString(bytes);
                var str = new string(original.Reverse().ToArray());
                return new DecodeResult { Value = str, Offset = size.Value };
            }
            else
            {
                // Dynamic string with 2-byte length prefix
                int length = BinaryPrimitives.ReadUInt16BigEndian(new ReadOnlySpan<byte>(reader.ReadBytes(2)));
                var bytes = reader.ReadBytes(length);
                var original = System.Text.Encoding.UTF8.GetString(bytes);
                var str = new string(original.Reverse().ToArray());
                return new DecodeResult { Value = str, Offset = length + 2 };
            }
        }
    }
}
