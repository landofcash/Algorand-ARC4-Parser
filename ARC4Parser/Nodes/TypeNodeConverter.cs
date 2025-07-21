namespace Aldemart.ARC4Parser.Nodes;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class TypeNodeConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        // apply to TypeNode or any subclass
        return typeof(TypeNode).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        // load raw JSON
        var jo = JObject.Load(reader);
        if (jo == null)
        {
            throw new JsonSerializationException("Failed to create JObject");
        }

        var kind = (string)jo["kind"]! ?? throw new JsonSerializationException("missing kind");

        // pick target CLR type
        Type target = kind switch
        {
            "primitive" => typeof(PrimitiveFieldType),
            "struct" => typeof(StructTypeNode),
            // add other cases...
            _ => throw new NotSupportedException($"Unknown kind '{kind}'")
        };

        // create an empty instance and populate it
        var instance = Activator.CreateInstance(target)!;
        using (var subReader = jo.CreateReader())
            serializer.Populate(subReader, instance);

        return instance;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        // default serialization is fine—no loop because we're not calling ourselves
        serializer.Serialize(writer, value);
    }
    
    public override bool CanWrite => true;
}