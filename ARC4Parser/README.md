# Algorand ARC4 Parser

The **Algorand ARC4 Parser** is a .NET library designed to parse and convert ARC4-encoded data structures. It provides a robust and extensible framework for working with on-chain data representations, enabling seamless decoding and conversion of complex data types.

## Features

- **ARC4 Parsing**: Decode ARC4-encoded data structures into strongly-typed C# objects.
- **Customizable Converters**: Support for custom converters for specialized data types.
- **Extensive Type Support**: Includes support for primitive types, arrays, tuples, and structs.
- **Annotations**: Use `[Arc4Property]` attributes to map ARC4 fields to C# properties.
- **Test Coverage**: Comprehensive test cases to ensure reliability and correctness.

## Installation

To use this library, include the project in your .NET solution or add it as a dependency.

## Usage

### Parsing ARC4 Data

To parse ARC4 data, define your data model and annotate it with `[Arc4Property]` attributes.

You also need to create a JSON mapping file, example:

```json
{
    "kind": "struct",
    "name": "Product",
    "fields": [
        { "name": "version", "type": { "kind": "primitive", "name": "uint", "size": 32 } },
        { "name": "seed", "type": { "kind": "primitive", "name": "string", "size": 22 } },
        { "name": "createdDate", "type": { "kind": "primitive", "name": "date", "size": 64 } },
        { "name": "price", "type": { "kind": "primitive", "name": "uint", "size": 64 } },
        { "name": "priceToken", "type": { "kind": "primitive", "name": "uint", "size": 64 } },
        { "name": "royalty", "type": { "kind": "primitive", "name": "uint", "size": 16 } },
        { "name": "seller", "type": { "kind": "primitive", "name": "address" } },
        { "name": "categoryId", "type": { "kind": "primitive", "name": "uint", "size": 64 } },
        { "name": "groupSeed", "type": { "kind": "primitive", "name": "string", "size": 22 } },
        { "name": "expirationDate", "type": { "kind": "primitive", "name": "date", "size": 64 } },
        { "name": "totalCodes", "type": { "kind": "primitive", "name": "uint", "size": 32 } },
        { "name": "orderCount", "type": { "kind": "primitive", "name": "uint", "size": 32 } },
        { "name": "codesBitmap", "type": { "kind": "primitive", "name": "uint", "size": 128 } },
        {
            "name": "codeManifest",
            "type": {
                "kind": "struct",
                "name": "CodeManifest",
                "fields": [
                    { "name": "manifestType", "type": { "kind": "primitive", "name": "uint", "size": 16 } },
                    { "name": "dataLocation", "type": { "kind": "primitive", "name": "string" } }
                ]
            }
        },
        { "name": "details", "type": { "kind": "primitive", "name": "bytes", "converter": "DetailsConverter" } }
    ]
}

```
 C# class example:

```csharp
using ARC4Parser;

public class Product
{
    [Arc4Property("version")]
    public uint Version { get; set; }

    [Arc4Property("seed")]
    public string Seed { get; set; } = "";

    [Arc4Property("price")]
    public ulong Price { get; set; }

    [Arc4Property("details", typeof(DetailsConverter))]
    public ProductDetails Details { get; set; } = new();
}

public class ProductDetails
{
    public string Name { get; set; } = "";
    public List<string> ImageUrls { get; set; } = new();
    public string Description { get; set; } = "";
}
```

### Decoding Example

```csharp
string json = File.ReadAllText("productStructNode.json");
StructTypeNode? structTypeNode = JsonConvert.DeserializeObject<StructTypeNode>(json);
string base64EncodedData = "AAAAAW1YamFmUW44UWtDQjUyaWlvd0JMY2cAAAAAaHQX5QAAAAAAHl1wAAAAAAAAAAAAZDn57aMrdoS8ODz5n0n0D9frQzN/x2eW4aODv2yHiiG7AAAAAAAAAA" +
                           "FtWGphZlFuOFFrQ0I1Mmlpb3dCTGNnAAAAAGiKo/AAAAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAlgDVAAEABAA5aHR0cHM6Ly9jZG4uYWxkZW1hcnQuY29tL2NvZGVzL2" +
                           "1YamFmUW44UWtDQjUyaWlvd0JMY2cudHh0ADlzyywqLlHISy1XKCjKTylNLnnUsFA/Kb8itRhE6qbrFeSlA4W8SoGqEsHqSlKBTKhiLpBATmZeKgA=";
byte[] buffer = Convert.FromBase64String(base64EncodedData);

Arc4Converter converter = new Arc4Converter();
Product product = converter.Process<Product>(structTypeNode, buffer);
```

### Custom Converters

You can implement custom converters for specialized data types by inheriting from `Arc4Converter`.

## Test Examples

The project includes a test project (`ARC4ParserTests`) with examples of how to use the library. Below is a sample test model:

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request with your changes.

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
