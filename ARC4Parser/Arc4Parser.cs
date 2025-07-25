﻿namespace Aldemart.ARC4Parser;

using System.Buffers.Binary;
using ARC4Parser.Nodes;
using ARC4Parser.PrimitiveDecoders;

public class PrimitiveDecoderRegistry
{
    private readonly List<IPrimitiveDecoder> Decoders = new();
    public void RegisterDecoder(IPrimitiveDecoder decoder) => Decoders.Add(decoder);
    public IPrimitiveDecoder? GetDecoder(string typeName) => Decoders.FirstOrDefault(d => d.CanDecode(typeName));
}

public static class Arc4Parser
{
    public static bool DEBUG_PARSER = true;
    public static readonly PrimitiveDecoderRegistry PrimitiveDecoderRegistry = new();

    static Arc4Parser()
    {
        List<IPrimitiveDecoder> defaultPrimitiveDecoders =
        [
            new UintDecoder(),
            new StringDecoder(),
            new DateDecoder(),
            new AddressDecoder(),
            new BytesDecoder()
        ];

        foreach (var decoder in defaultPrimitiveDecoders)
        {
            PrimitiveDecoderRegistry.RegisterDecoder(decoder);
        }
    }

    public static DecodeResult DecodeValue(TypeNode type, byte[] buffer, int offset = 0)
    {
        //offset is the pointer on the data.
        if (type is PrimitiveFieldType primitiveType)
        {
            return DecodePrimitive(primitiveType, buffer, offset);
        }

        if (type is ArrayTypeNode arrayType)
        {
            if (arrayType.Length.HasValue)
            {
                return DecodeStaticArray(arrayType.ElementType!, arrayType.Length.Value, buffer, offset);
            }
            else
            {
                return DecodeDynamicArray(arrayType.ElementType!, buffer, offset);
            }
        }

        if (type is TupleTypeNode tupleType)
        {
            return DecodeContainer(tupleType.Components!, buffer, offset);
        }

        if (type is StructTypeNode structType)
        {
            return DecodeStruct(structType, buffer, offset);
        }

        throw new NotSupportedException($"Unsupported type: {type.GetType().Name}");
    }

    private static DecodeResult DecodePrimitive(PrimitiveFieldType primitiveFieldType, byte[] buffer, int offset)
    {
        var decoder = PrimitiveDecoderRegistry.GetDecoder(primitiveFieldType.Name!);
        if (decoder == null)
        {
            throw new NotSupportedException($"Unsupported primitive type: {primitiveFieldType.Name}");
        }
        using var memoryStream = new MemoryStream(buffer);
        using var reader = new BinaryReader(memoryStream);
        memoryStream.Seek(offset, SeekOrigin.Begin);
        var result = decoder.Decode(reader, primitiveFieldType.Size);

        if (DEBUG_PARSER)
        {
            Console.WriteLine(
                $"Primitive: {primitiveFieldType.Name}{(primitiveFieldType.Size.HasValue ? $"({primitiveFieldType.Size})" : "")} = {result.Value}, length: {result.Offset} bytes");
        }

        return result;
    }

    private static DecodeResult DecodeStaticArray(TypeNode elementType, int length, byte[] buffer, int offset)
    {
        if (DEBUG_PARSER)
        {
            Console.WriteLine($"StaticArray[{length}] starting at offset {offset}");
        }

        var array = new List<object>();
        var currentOffset = offset;
        for (int i = 0; i < length; i++)
        {
            var result = DecodeValue(elementType, buffer, currentOffset);
            array.Add(result.Value!);
            currentOffset = result.Offset;
        }

        if (DEBUG_PARSER)
        {
            Console.WriteLine($"StaticArray end, total length: {currentOffset - offset} bytes");
        }

        return new DecodeResult { Value = array, Offset = currentOffset };
    }

    private static DecodeResult DecodeDynamicArray(TypeNode elementType, byte[] buffer, int offset)
    {
        var length = BitConverter.ToUInt16(buffer, offset);
        if (DEBUG_PARSER)
        {
            Console.WriteLine($"DynamicArray with {length} elements starting at offset {offset}");
        }

        var array = new List<object>();
        var currentOffset = offset + 2;
        for (int i = 0; i < length; i++)
        {
            var result = DecodeValue(elementType, buffer, currentOffset);
            array.Add(result.Value!);
            currentOffset = result.Offset;
        }

        if (DEBUG_PARSER)
        {
            Console.WriteLine($"DynamicArray end, total length: {currentOffset - offset} bytes (including 2-byte prefix)");
        }

        return new DecodeResult { Value = array, Offset = currentOffset };
    }

    private static DecodeResult DecodeContainer(List<TypeNode> components, byte[] buffer, int offset)
    {
        if (DEBUG_PARSER)
        {
            Console.WriteLine($"Container with {components.Count} elements starting at offset {offset}");
        }
        
        var values = new List<object>();
        var currentOffset = offset;
        foreach (var component in components)
        {
            bool dynamicComponent = IsDynamic(component);
            if (dynamicComponent)
            {
                int pointerValue = BitConverter.ToUInt16(buffer, currentOffset);
                int dataOffset = offset + pointerValue;
                if (DEBUG_PARSER)
                {
                    Console.WriteLine($"Dynamic Container, Data offset {dataOffset}");
                }
                var result = DecodeValue(component, buffer, dataOffset);
                values.Add(result.Value!);
                currentOffset += 2;
            }
            else
            {
                var result = DecodeValue(component, buffer, currentOffset);
                values.Add(result.Value!);
                currentOffset = result.Offset;
            }
        }

        if (DEBUG_PARSER)
        {
            Console.WriteLine($"Container end, total length: {currentOffset - offset} bytes");
        }

        return new DecodeResult { Value = values, Offset = currentOffset };
    }

    private static DecodeResult DecodeStruct(StructTypeNode s, byte[] buffer, int offset)
    {
        // Determine if this struct is dynamic (contains any dynamic fields)
        bool dynamicStruct = IsDynamic(s);
        if (DEBUG_PARSER)
        {
            Console.WriteLine($"Struct {s.Name} Dynamic:{dynamicStruct} with {s.Fields?.Count} fields starting at offset {offset}");
        }
        // Remember start of the head region
        int headStart = offset;
        int currentOffset = offset;
        var values = new List<object>();
        if (s.Fields == null)
        {
            throw new ArgumentNullException(nameof(s.Fields));
        }
        // Decode each field in the head
        foreach (var field in s.Fields)
        {
            if (field.Type == null) throw new ApplicationException($"field.Type is null for field: {field.Name}");
            if (DEBUG_PARSER)
            {
                Console.WriteLine($"Decoding field: {field.Name} type:{field.Type.Name} {field.Type.Kind}");
            }
            if (IsDynamic(field.Type))
            {
                // Dynamic field: read a 2-byte big-endian pointer into the tail
                ushort pointer = BinaryPrimitives.ReadUInt16BigEndian(new ReadOnlySpan<byte>(buffer, currentOffset, 2));
                int dataOffset = headStart + pointer;
                if (DEBUG_PARSER)
                {
                    Console.WriteLine($"Dynamic field pointer at {currentOffset} points to offset: {dataOffset}");
                }
                // Decode from the tail without advancing the head
                var value = DecodeValue(field.Type, buffer, dataOffset);
                values.Add(value);
                currentOffset += 2;
            }
            else
            {
                // Static field: decode in place, advancing the head
                var value = DecodeValue(field.Type, buffer, currentOffset);
                values.Add(value);
                currentOffset += value.Offset;
            }
        }
        
        // 2) Map decoded values back to field names
        var result = new Dictionary<string, object>();
        for (int i = 0; i < s.Fields.Count; i++)
        {
            result[s.Fields[i].Name!] = values[i];
        }

        // 3) Advance the global offset:
        //    - For dynamic structs, the outer pointer is 2 bytes
        //    - Otherwise, jump to end of head
        offset = dynamicStruct ? headStart + 2 : currentOffset;
        if (DEBUG_PARSER)
        {
            Console.WriteLine($"Struct end, total length: {offset-headStart} bytes, offset:{offset}");
        }

        return new DecodeResult
        {
            Value = result,
            Offset = offset
        };
    }

    /// <summary>
    /// Determines whether the given type node represents a dynamic (variable‐length) type,
    /// mirroring the ARC-4 TypeNode logic.
    /// </summary>
    private static bool IsDynamic(TypeNode t)
    {
        switch (t.Kind)
        {
            case "primitive":
            {
                var p = (PrimitiveFieldType)t;
                // string without a fixed size is dynamic
                if (p.Name == "string" && !p.Size.HasValue) return true;
                // bytes without a fixed size are dynamic
                if (p.Name == "bytes" && !p.Size.HasValue) return true;
                // all other primitives are static
                return false;
            }

            case "array":
            {
                var a = (ArrayTypeNode)t;
                // dynamic array (no fixed Length)
                if (!a.Length.HasValue) return true;
                // static array is dynamic if its element type is
                if (a.ElementType == null) throw new ApplicationException($"Array is not defined for {a.Name}");
                return IsDynamic(a.ElementType);
            }

            case "tuple":
            {
                var tpl = (TupleTypeNode)t;
                if (tpl.Components == null) throw new ApplicationException($"Tuple Components is not defined for {tpl.Name}");
                // dynamic if any component is dynamic
                return tpl.Components.Any(IsDynamic);
            }

            case "struct":
            {
                var s = (StructTypeNode)t;
                if (s.Fields == null) throw new ApplicationException($"Struct fields is not defined for {s.Name}");
                // dynamic if any field’s type is dynamic
                return s.Fields.Any(f => IsDynamic(f.Type!));
            }

            default:
                return false;
        }
    }
}