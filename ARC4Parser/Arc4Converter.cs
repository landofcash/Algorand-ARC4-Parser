namespace Aldemart.ARC4Parser
{
    using Aldemart.ARC4Parser.ARC4Types;
    using Aldemart.ARC4Parser.Nodes;
    using System.Reflection;
    using Microsoft.Extensions.Logging;

    public class Arc4Converter
    {
        private readonly ILogger _logger;

        public Arc4Converter(ILogger logger)
        {
            _logger = logger;
        }
        public T Process<T>(Arc4Parser parser, StructTypeNode typeNode, byte[] value) where T : new()
        {
            var decoded = parser.DecodeValue(typeNode, value);
            if (decoded == null || decoded.Value==null) throw new Exception("Failed to decode DecodeValue");
            return ProcessStruct<T>(typeNode, (Dictionary<string, object?>)decoded.Value);
        }
        
        public T Process<T>(StructTypeNode typeNode, byte[] value) where T : new()
        {
            Arc4Parser parser = new Arc4Parser(_logger);
            return Process<T>(parser, typeNode, value);
        }

        private object? ProcessPrimitive(string fieldName, int? fieldSize, object fieldValue)
        {
            if (fieldValue == null) return fieldValue;
            if (fieldName == "string")
            {
                return fieldValue.ToString();
            }

            if (fieldName == "uint" && fieldSize == 2)
            {
                return Convert.ToUInt16(fieldValue);
            }

            if (fieldName == "uint" && fieldSize == 4)
            {
                return Convert.ToUInt32(fieldValue);
            }

            if (fieldName == "uint" && fieldSize == 8)
            {
                return Convert.ToUInt64(fieldValue);
            }

            if (fieldName == "uint")
            {
                return ((ARC4Int)fieldValue).RawValue;
            }

            if (fieldName == "byte")
            {
                return Convert.ToByte(fieldValue);
            }

            return fieldValue;
        }


        public T ProcessStruct<T>(StructTypeNode structTypeNode, Dictionary<string, object?> decoded) where T : new()
        {
            return (T)ProcessStruct(typeof(T), structTypeNode, decoded);
        }


        private object ProcessStruct(Type modelType, StructTypeNode structTypeNode, Dictionary<string, object?> decoded)
        {
            // Create an instance of that Type
            var result = Activator.CreateInstance(modelType) ?? throw new InvalidOperationException($"Could not create {modelType.Name}");


            foreach (PropertyInfo property in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var arc4Attr = property.GetCustomAttribute<Arc4PropertyAttribute>();
                if (arc4Attr == null)
                {
                    continue;
                }

                var propertyName = arc4Attr.Name;
                if (structTypeNode.Fields == null)
                {
                    Console.WriteLine($"structTypeNode.Fields is null {structTypeNode.Name}");
                    continue;
                }
                
                var fieldNode = structTypeNode.Fields.FirstOrDefault(f => f.Name == propertyName);
                if (fieldNode == null)
                {
                    Console.WriteLine($"Can't find JSON Node for propertyName:{propertyName}");
                    continue;
                }

                Console.WriteLine($"propertyName:{propertyName} fieldNode Name:{fieldNode.Name} Type:{fieldNode.Type}");


                //skip if missing
                if (!decoded.TryGetValue(propertyName, out var raw) || raw == null)
                {
                    continue;
                }

                object decodedResult = ((DecodeResult)raw).Value!;

                //check converter
                if (arc4Attr.Converter != null)
                {
                    Console.WriteLine($"Property Name:{property.Name} Converter Type:{arc4Attr.Converter.GetType()}");
                    var converted = arc4Attr.Converter.Convert(decodedResult);
                    property.SetValue(result, converted);
                    continue;
                }

                //Primitive
                if (fieldNode.Type is PrimitiveFieldType)
                {
                    var primitiveFieldType = (PrimitiveFieldType)fieldNode.Type;
                    var prim = ProcessPrimitive(primitiveFieldType.Name!, primitiveFieldType.Size, decodedResult);
                    property.SetValue(result, prim);
                }
                //Struct → just recurse by Type
                else if (fieldNode.Type is StructTypeNode)
                {
                    var nested = ProcessStruct(property.PropertyType, (StructTypeNode)fieldNode.Type, (Dictionary<string, object?>)decodedResult);
                    property.SetValue(result, nested);
                }
                //Everything else → direct assign
                else
                {
                    property.SetValue(result, raw);
                }
            }

            return result;
        }
    }
}