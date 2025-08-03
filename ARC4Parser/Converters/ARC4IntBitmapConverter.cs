namespace Aldemart.ARC4Parser.Converters;

using Aldemart.ARC4Parser.ARC4Types;

/// <summary>
/// Converts a decoded ARC4 integer into a bitmap string.
/// 7 ==> "111"
/// 6 ==> "110"
/// </summary>
public class ARC4IntBitmapConverter : IPrimitiveConverter
{
    public object Convert(object decodedValue)
    {
        var arc4Int = decodedValue as ARC4Int;
        if(arc4Int == null) throw new ArgumentException("ARC4IntBitmapConverter expects an ARC4Int as input.");
        return arc4Int.ToBitArrayString();
    }
}