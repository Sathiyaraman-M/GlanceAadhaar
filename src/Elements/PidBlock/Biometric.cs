using GlanceAadhaar.Enums;

namespace GlanceAadhaar.Elements.PidBlock;

public class Biometric : IXmlElement, IEquatable<Biometric>
{
    internal static string[] BiometricTypeNames => new[] { "FMR", "FIR", "IIR", "FID" };

    internal static string[] BiometricPositionNames => new[] { "LEFT_IRIS", "RIGHT_IRIS", "LEFT_INDEX", "LEFT_LITTLE", "LEFT_MIDDLE", "LEFT_RING", "LEFT_THUMB", "RIGHT_INDEX", "RIGHT_LITTLE", "RIGHT_MIDDLE", "RIGHT_RING", "RIGHT_THUMB", "FACE", "UNKNOWN" };
    
    public BiometricType Type { get; set; }
    
    public BiometricPosition Position { get; set; }
    
    public string Data { get; set; }
    
    public bool IsUsed => !string.IsNullOrWhiteSpace(Data);
    
    public XElement ConvertToXml(string elementName)
    {
        if ((Type == BiometricType.Iris && Position != BiometricPosition.LeftIris && Position != BiometricPosition.RightIris && Position != BiometricPosition.Unknown) ||
            (Type != BiometricType.Iris && Position is BiometricPosition.LeftIris or BiometricPosition.RightIris))
            throw new ArgumentException(InvalidBiometricPosition, nameof(Position));
        
        ThrowIfNullOrEmptyString(Data, nameof(Data));

        var biometric = new XElement(elementName,
            new XAttribute("type", BiometricTypeNames[(int)Type]),
            new XAttribute("posh", BiometricPositionNames[(int)Position]), Data);

        return biometric;
    }

    public void LoadFromXml(XElement element)
    {
        throw new NotSupportedException();
    }

    public bool Equals(Biometric other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type && Position == other.Position;
    }

    public override bool Equals(object obj)
    {
        return obj != null && obj.GetType() == GetType() && Equals(obj as Biometric);
    }

    public override int GetHashCode()
    {
        return Type.GetHashCode() ^ Position.GetHashCode();
    }
}