namespace Aldemart.ARC4Parser.ARC4Types;

using System.Numerics;

public class ARC4Int : IConvertible
{
    private readonly BigInteger _value;

    public ARC4Int(BigInteger value)
    {
        _value = value;
    }

    public ARC4Int(string value)
    {
        _value = BigInteger.Parse(value);
    }

    public BigInteger RawValue => _value;

    public int ToNumber() => (int)_value;

    public override string ToString() => _value.ToString();

    public string ToBinaryString() => _value.ToString("B");

    public string ToBitArrayString(int? padLength = null)
    {
        var bin = ToBinaryString();
        return padLength.HasValue ? bin.PadLeft(padLength.Value, '0') : bin;
    }

    public override bool Equals(object? obj)
    {
        if (obj is ARC4Int other) return _value == other._value;
        if (obj is BigInteger bi) return _value == bi;
        return false;
    }

    public override int GetHashCode() => _value.GetHashCode();
    public TypeCode GetTypeCode()
    {
        return TypeCode.Empty;
    }

    public bool ToBoolean(IFormatProvider? provider)
    {
        return _value != 0;
    }
    public byte ToByte(IFormatProvider? provider)
    {
        return (byte)_value;
    }
    public char ToChar(IFormatProvider? provider)
    {
        return (char)(int)_value;
    }
    public DateTime ToDateTime(IFormatProvider? provider)
    {
        throw new InvalidCastException("ARC4Int cannot be converted to DateTime.");
    }
    public decimal ToDecimal(IFormatProvider? provider)
    {
        return (decimal)_value;
    }
    public double ToDouble(IFormatProvider? provider)
    {
        return (double)_value;
    }
    public short ToInt16(IFormatProvider? provider)
    {
        return (short)_value;
    }
    public int ToInt32(IFormatProvider? provider)
    {
        return (int)_value;
    }
    public long ToInt64(IFormatProvider? provider)
    {
        return (long)_value;
    }
    public sbyte ToSByte(IFormatProvider? provider)
    {
        return (sbyte)_value;
    }
    public float ToSingle(IFormatProvider? provider)
    {
        return (float)_value;
    }
    public string ToString(IFormatProvider? provider)
    {
        return _value.ToString(provider);
    }
    public object ToType(Type conversionType, IFormatProvider? provider)
    {
        if (conversionType == typeof(BigInteger))
            return _value;
        return Convert.ChangeType(_value, conversionType, provider);
    }
    public ushort ToUInt16(IFormatProvider? provider)
    {
        return (ushort)_value;
    }
    public uint ToUInt32(IFormatProvider? provider)
    {
        return (uint)_value;
    }
    public ulong ToUInt64(IFormatProvider? provider)
    {
        return (ulong)_value;
    }
}