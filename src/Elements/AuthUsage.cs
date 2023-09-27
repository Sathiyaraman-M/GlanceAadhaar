using GlanceAadhaar.Elements.PidBlock;
using GlanceAadhaar.Enums;

namespace GlanceAadhaar.Elements;

public class AuthUsage : IXmlElement
{
    public AuthUsage()
    {
        
    }

    public AuthUsage(XElement element)
    {
        LoadFromXml(element);
    }
    
    public AuthType AuthUsed { get; set; }
    
    public ICollection<BiometricType> Biometrics { get; } = new HashSet<BiometricType>();
    
    public XElement ConvertToXml(string elementName)
    {
        if(AuthUsed.HasFlag(AuthType.Biometric) && !Biometrics.Any())
            throw new ArgumentException(RequiredBiometricsUsed, nameof(Biometrics));
        
        var authUsage = new XElement(elementName,
            new XAttribute("pi", AuthUsed.HasFlag(AuthType.PersonalIdentity) ? 'y' : 'n'),
            new XAttribute("pa", AuthUsed.HasFlag(AuthType.PersonalAddress) ? 'y' : 'n'),
            new XAttribute("pfa", AuthUsed.HasFlag(AuthType.PersonalFullAddress) ? 'y' : 'n'),
            new XAttribute("pin", AuthUsed.HasFlag(AuthType.Pin) ? 'y' : 'n'),
            new XAttribute("otp", AuthUsed.HasFlag(AuthType.Otp) ? 'y' : 'n'),
            new XAttribute("bio", AuthUsed.HasFlag(AuthType.Biometric) ? 'y' : 'n'));
        if(AuthUsed.HasFlag(AuthType.Biometric))
            authUsage.Add(new XAttribute("bt", string.Join(',', Biometrics.Select(x => Biometric.BiometricTypeNames[(int)x]))));
        return authUsage;
    }

    public void LoadFromXml(XElement element)
    {
        ThrowIfNull(element, nameof(element));

        AuthUsed = AuthType.None;
        Biometrics.Clear();
        
        if(element.Attribute("pi")?.Value[0] == 'y')
            AuthUsed |= AuthType.PersonalIdentity;
        if(element.Attribute("pa")?.Value[0] == 'y')
            AuthUsed |= AuthType.PersonalAddress;
        if(element.Attribute("pfa")?.Value[0] == 'y')
            AuthUsed |= AuthType.PersonalFullAddress;
        if(element.Attribute("pin")?.Value[0] == 'y')
            AuthUsed |= AuthType.Pin;
        if(element.Attribute("otp")?.Value[0] == 'y')
            AuthUsed |= AuthType.Otp;

        if (element.Attribute("bio")?.Value[0] == 'y')
        {
            AuthUsed |= AuthType.Biometric;
            foreach (var type in element.Attribute("bt")!.Value.Split(',').Select(Enum.Parse<BiometricType>))
                Biometrics.Add(type);
        }
    }
}