using System.Xml.Linq;
using Glance.Aadhaar.Constants;
using Glance.Aadhaar.Enums;
using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Resident;

public class AuthUsage : IXml
{
    public AuthUsage()
    {
        
    }

    public AuthUsage(XElement element) => FromXml(element);
    
    public AuthTypes AuthUsed { get; set; }
    
    public ICollection<BiometricType> Biometrics { get; } = new HashSet<BiometricType>();

    public void FromXml(XElement element)
    {
        ValidateNull(element, nameof(element));
        
        AuthUsed = AuthTypes.None;
        Biometrics.Clear();
        
        if (element.Attribute("pi")?.Value[0] == HelperConstants.Yes)
            AuthUsed |= AuthTypes.Identity;
        if (element.Attribute("pa")?.Value[0] == HelperConstants.Yes)
            AuthUsed |= AuthTypes.Address;
        if (element.Attribute("pfa")?.Value[0] == HelperConstants.Yes)
            AuthUsed |= AuthTypes.FullAddress;
        if (element.Attribute("otp")?.Value[0] == HelperConstants.Yes)
            AuthUsed |= AuthTypes.Otp;
        if(element.Attribute("pin")?.Value[0] == HelperConstants.Yes)
            AuthUsed |= AuthTypes.Pin;
        if (element.Attribute("bio")?.Value[0] == HelperConstants.Yes)
        {
            AuthUsed |= AuthTypes.Biometric;
            foreach (var type in element.Attribute("bt").Value.Split(',').Select(x => (BiometricType) Enum.Parse(typeof(BiometricType), x)))
                Biometrics.Add(type);
        }
    }

    public XElement ToXml(string elementName)
    {
        if(AuthUsed.HasFlag(AuthTypes.Biometric) && !Biometrics.Any())
            throw new ArgumentException(RequiredBiometricsUsed, nameof(Biometrics));
        
        var authUsage = new XElement(elementName,
            new XAttribute("pi", AuthUsed.HasFlag(AuthTypes.Identity) ? HelperConstants.Yes : HelperConstants.No),
            new XAttribute("pa", AuthUsed.HasFlag(AuthTypes.Address) ? HelperConstants.Yes : HelperConstants.No),
            new XAttribute("pfa", AuthUsed.HasFlag(AuthTypes.FullAddress) ? HelperConstants.Yes : HelperConstants.No),
            new XAttribute("bio", AuthUsed.HasFlag(AuthTypes.Biometric) ? HelperConstants.Yes : HelperConstants.No),
            new XAttribute("otp", AuthUsed.HasFlag(AuthTypes.Otp) ? HelperConstants.Yes : HelperConstants.No),
            new XAttribute("pin", AuthUsed.HasFlag(AuthTypes.Pin) ? HelperConstants.Yes : HelperConstants.No));
        if(AuthUsed.HasFlag(AuthTypes.Biometric))
            authUsage.Add(new XAttribute("bt", string.Join(",", Biometrics.Select(x =>Biometric.BiometricTypeNames[(int)x]))));
        
        return authUsage;
    }
}