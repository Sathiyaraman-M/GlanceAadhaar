using System.Xml.Linq;
using Glance.Aadhaar.Enums;
using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Resident;

public class Biometric : IEquatable<Biometric>, IUsed, IXml
{
    internal static readonly string[] BiometricTypeNames = { "FIR", "FMR", "IIR", "FID" };
    internal static readonly string[] BiometricPositionNames = { "LEFT_IRIS", "RIGHT_IRIS", "LEFT_INDEX", "LEFT_LITTLE", "LEFT_MIDDLE", "LEFT_RING", "LEFT_THUMB", "RIGHT_INDEX", "RIGHT_LITTLE", "RIGHT_MIDDLE", "RIGHT_RING", "RIGHT_THUMB", "UNKNOWN" };
    
    public BiometricType Type { get; set; }
    
    public BiometricPosition Position { get; set; }
    
    public string Data { get; set; }
    
    public bool Equals(Biometric other) => other != null && Type == other.Type && Position == other.Position;

    public bool IsUsed => !string.IsNullOrWhiteSpace(Data);
    
    public void FromXml(XElement element)
    {
        throw new NotSupportedException();
    }

    public XElement ToXml(string elementName)
    {
        if ((Type == BiometricType.Iris && Position != BiometricPosition.LeftIris && Position != BiometricPosition.RightIris && Position != BiometricPosition.Unknown) ||
            (Type != BiometricType.Iris && Position is BiometricPosition.LeftIris or BiometricPosition.RightIris))
            throw new ArgumentException(InvalidBiometricPosition, nameof(Position));
        
        ValidateEmptyString(Data, nameof(Data));

        var biometric = new XElement(elementName,
            new XAttribute("type", BiometricTypeNames[(int)Type]),
            new XAttribute("posh", BiometricPositionNames[(int)Position]), Data);

        return biometric;
    }
    
    public override bool Equals(object obj) => obj != null && obj.GetType() == GetType() && Equals(obj as Biometric);
    
    public override int GetHashCode() => Type.GetHashCode() ^ Position.GetHashCode();
}