using System.Globalization;
using System.Xml.Linq;
using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Security;

public class SessionKeyInfo : IXml
{
    private const string CertificateIdentifierFormat = "yyyyMMdd";

    public SessionKeyInfo()
    {
        
    }
    
    public SessionKeyInfo(XElement element)
    {
        FromXml(element);
    }
    
    public DateTimeOffset CertificateIdentifier { get; set; }
    
    public string Key { get; set; }
    
    public void FromXml(XElement element)
    {
        ValidateNull(element, nameof(element));
        
        CertificateIdentifier = DateTimeOffset.ParseExact(element.Attribute("ci")?.Value, CertificateIdentifierFormat, CultureInfo.InvariantCulture);
        Key = element.Value;
    }

    public XElement ToXml(string elementName)
    {
        var sessionKey = new XElement(elementName, new XAttribute("ci", CertificateIdentifier.ToString(CertificateIdentifierFormat, CultureInfo.InvariantCulture)), Key);
        return sessionKey;
    }
}