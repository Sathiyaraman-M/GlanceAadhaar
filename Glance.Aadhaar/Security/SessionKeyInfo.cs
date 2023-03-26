using System.Globalization;
using System.Xml.Linq;
using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Security;

public class SessionKeyInfo : IXml
{
    public static readonly string CertificateIdentifierFormat = "yyyyMMdd";
    
    public SessionKeyInfo()
    {
        
    }
    
    public SessionKeyInfo(XElement element)
    {
        FromXml(element);
    }
    
    public DateTimeOffset CertificateIdentifier { get; set; }
    
    public string Key { get; set; }
    
    public Guid KeyIdentifier { get; set; }
    
    public void FromXml(XElement element)
    {
        ValidateNull(element, nameof(element));
        
        CertificateIdentifier = DateTimeOffset.ParseExact(element.Attribute("ci").Value, CertificateIdentifierFormat, CultureInfo.InvariantCulture);
        Key = element.Value;
        var ki = element.Attribute("ki")?.Value;
        KeyIdentifier = !string.IsNullOrEmpty(ki) ? Guid.Parse(ki) : Guid.Empty;
    }

    public XElement ToXml(string elementName)
    {
        var sessionKey = new XElement(elementName, new XAttribute("ci", CertificateIdentifier.ToString(CertificateIdentifierFormat, CultureInfo.InvariantCulture)), Key);
        if (KeyIdentifier != Guid.Empty)
        {
            sessionKey.Add(new XAttribute("ki", KeyIdentifier.ToString()));
        }
        return sessionKey;
    }
}