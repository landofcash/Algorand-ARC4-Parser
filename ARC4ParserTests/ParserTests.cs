namespace ARC4ParserTests
{
    using Newtonsoft.Json;
    using System;
    using Aldemart.ARC4Parser;
    using Aldemart.ARC4Parser.Nodes;
    using Aldemart.ARC4ParserTests.Models;


    [TestFixture]
    internal class ParseProductTests
    {
        [Test]
        public void TestNodeParser()
        {
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "productStructNode.json");
            string json = File.ReadAllText(jsonFilePath);
            StructTypeNode? structTypeNode = JsonConvert.DeserializeObject<StructTypeNode>(json);
            if (structTypeNode == null) throw new Exception("Can't deserialize json");
            Console.WriteLine($"Kind: {structTypeNode.Kind} Name: {structTypeNode.Name}");
            
            string base64EncodedData = "AAAAAW1YamFmUW44UWtDQjUyaWlvd0JMY2cAAAAAaHQX5QAAAAAAHl1wAAAAAAAAAAAAZDn57aMrdoS8ODz5n0n0D9frQzN/x2eW4aODv2yHiiG7AAAAAAAAAA" +
                                       "FtWGphZlFuOFFrQ0I1Mmlpb3dCTGNnAAAAAGiKo/AAAAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAlgDVAAEABAA5aHR0cHM6Ly9jZG4uYWxkZW1hcnQuY29tL2NvZGVzL2" +
                                       "1YamFmUW44UWtDQjUyaWlvd0JMY2cudHh0ADlzyywqLlHISy1XKCjKTylNLnnUsFA/Kb8itRhE6qbrFeSlA4W8SoGqEsHqSlKBTKhiLpBATmZeKgA=";
            byte[] buffer = Convert.FromBase64String(base64EncodedData);

            Arc4Converter converter = new Arc4Converter();
            Product product = converter.Process<Product>(structTypeNode, buffer);
            
            string expectedResult =
                """{"Version":1,"Seed":"mXjafQn8QkCB52iiowBLcg","CreatedDate":"2025-07-13T20:32:37Z","Price":1990000,"PriceToken":0,"Royalty":100,"Seller":"HH463IZLO2CLYOB47GPUT5AP27VUGM37Y5TZNYNDQO7WZB4KEG53YIXHAQ","CategoryId":1,"GroupSeed":"mXjafQn8QkCB52iiowBLcg","ExpirationDate":"2025-07-30T23:00:00Z","TotalCodes":20,"OrderCount":0,"CodesBitmap":0,"CodeManifest":{"ManifestType":1,"DataLocation":"https://cdn.aldemart.com/codes/mXjafQn8QkCB52iiowBLcg.txt"},"Details":{"Name":"First new product","ImageUrls":["/boxes/box-g.png"],"Description":"Just a new test product\nnew line"}}""";
            string result = JsonConvert.SerializeObject(product);
            Console.WriteLine($"{result}");
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
