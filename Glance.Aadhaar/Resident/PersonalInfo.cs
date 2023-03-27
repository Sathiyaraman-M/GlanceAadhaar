using System.Globalization;
using System.Xml.Linq;
using Glance.Aadhaar.Constants;
using Glance.Aadhaar.Enums;
using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Resident;

public class PersonalInfo : IXml
{
    private const string PidVersion = "2.0";

    private string _aadhaarNumber;
    
    public string AadhaarNumber
    {
        get => _aadhaarNumber;
        set => _aadhaarNumber = ValidateAadhaarNumber(value, nameof(AadhaarNumber));
    }
    
    public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.Now;
    
    public Demographic Demographic { get; set; }
    
    public ICollection<Biometric> Biometrics { get; } = new HashSet<Biometric>();
    
    public PinValue PinValue { get; set; }
    
    public byte[] Photo { get; set; }

    public AuthUsage Uses
    {
        get
        {
            var uses = new AuthUsage{ AuthUsed = AuthTypesUsed };
            var biometricTypes = Biometrics.Select(b => b.Type).Distinct();
            foreach (var biometricType in biometricTypes)
                uses.Biometrics.Add(biometricType);
            return uses;
        }
    }
    
    private AuthTypes AuthTypesUsed
    {
        get
        {
            var authTypes = AuthTypes.None;
            if (Demographic != null)
            {
                if (Demographic.Identity?.IsUsed == true)
                    authTypes |= AuthTypes.Identity;
                if (Demographic.Address?.IsUsed == true)
                    authTypes |= AuthTypes.Address;
                if (Demographic.FullAddress?.IsUsed == true)
                    authTypes |= AuthTypes.FullAddress;
            }
            if (Biometrics.Count > 0 && Biometrics.Any(b => b.IsUsed))
                authTypes |= AuthTypes.Biometric;
            if (PinValue == null) return authTypes;
            if (!string.IsNullOrWhiteSpace(PinValue.Pin))
                authTypes |= AuthTypes.Pin;
            if (!string.IsNullOrWhiteSpace(PinValue.Otp))
                authTypes |= AuthTypes.Otp;
            return authTypes;
        }
    }
    
    public void FromXml(XElement element)
    {
        throw new NotSupportedException();
    }

    public XElement ToXml(string elementName)
    {
        if (Uses.AuthUsed == AuthTypes.None)
            throw new ArgumentException(RequiredSomeData);
        if (Biometrics.Any(b => b.Type == BiometricType.Fingerprint) && Biometrics.Any(b => b.Type == BiometricType.Minutiae))
            throw new ArgumentException(XorFirFmr, nameof(Biometrics));

        var personalInfo = new XElement(elementName,
            new XAttribute("ts", TimeStamp.ToString(HelperConstants.TimestampFormat, CultureInfo.InvariantCulture)),
            new XAttribute("ver", PidVersion),
            new XAttribute("wadh", string.Empty));
        if (Demographic != null)
            personalInfo.Add(Demographic.ToXml("Demo"));
        if (Biometrics.Count > 0)
        {
            var biometrics = new XElement("Bios");
            foreach (var biometric in Biometrics)
                biometrics.Add(biometric.ToXml("Bio"));
            personalInfo.Add(biometrics);
        }
        if (PinValue != null)
            personalInfo.Add(PinValue.ToXml("Pv"));

        return personalInfo;
    }
}