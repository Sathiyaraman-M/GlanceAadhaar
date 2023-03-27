using System.Xml.Linq;
using Glance.Aadhaar.Enums;
using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Resident;

public class TestFinger : IEquatable<TestFinger>, IXml
{
    private int _numberOfAttempts = 1;
    
    public Nfig Quality { get; set; } = Nfig.Excellent;
    
    public int NumberOfAttempts
    {
        get => _numberOfAttempts;
        set
        {
            if(value < 1)
                throw new ArgumentOutOfRangeException(nameof(NumberOfAttempts), OutOfRangeNumberOfAttempts);
            _numberOfAttempts = value;
        }
    }
    
    public BiometricPosition Position { get; set; }
    
    public string Data { get; set; }
    
    public bool Equals(TestFinger other)
    {
        return other != null && Position == other.Position;
    }

    public void FromXml(XElement element)
    {
        throw new NotSupportedException();
    }

    public XElement ToXml(string elementName)
    {
        if (Position is BiometricPosition.Unknown or BiometricPosition.LeftIris or BiometricPosition.RightIris)
            throw new ArgumentException(InvalidBiometricPosition, nameof(Position));
        ValidateEmptyString(Data, nameof(Data));

        var bestFinger = new XElement(elementName,
            new XAttribute("nfiq", (int)Quality),
            new XAttribute("na", NumberOfAttempts),
            new XAttribute("pos", Biometric.BiometricPositionNames[(int)Position]), Data);

        return bestFinger;
    }
    
    public override bool Equals(object obj) => obj != null && obj.GetType() == GetType() && Equals(obj as TestFinger);

    public override int GetHashCode() => Position.GetHashCode();
}