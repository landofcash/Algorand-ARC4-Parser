namespace Aldemart.ARC4Parser;

using Aldemart.ARC4Parser.Converters;

[AttributeUsage(AttributeTargets.Property)]
public class Arc4PropertyAttribute : Attribute
{
    /// <summary>
    /// The JSON/ARC4 field name to bind to.
    /// </summary>
    public string Name { get; }

    private readonly Type? _converterType;


    public Arc4PropertyAttribute(string name, Type? converterType = null)
    {
        Name = name;
        _converterType = converterType;
    }

    /// <summary>
    /// Lazily instantiate your converter (or return null).
    /// </summary>
    public IPrimitiveConverter? Converter => _converterType == null ? null : (IPrimitiveConverter?)Activator.CreateInstance(_converterType);
}