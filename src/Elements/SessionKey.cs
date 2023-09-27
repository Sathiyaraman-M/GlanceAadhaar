using System.Globalization;

namespace GlanceAadhaar.Elements;

public class SessionKey : IXmlElement
{
    public SessionKey()
    {
        
    }

    public SessionKey(XElement element)
    {
        LoadFromXml(element);
    }
    
    public DateTimeOffset CertificateIdentifier { get; set; }
    
    public string Key { get; set; }
    
    public XElement ConvertToXml(string elementName)
    {
        return new XElement(elementName,
            new XAttribute("ci", CertificateIdentifier.ToString("yyyyMMdd", CultureInfo.InvariantCulture)), Key);
    }

    public void LoadFromXml(XElement element)
    {
        ThrowIfNull(element, nameof(element));
        CertificateIdentifier = DateTimeOffset.ParseExact(element.Attribute("ci")!.Value, "yyyyMMdd", CultureInfo.InvariantCulture);
        Key = element.Value;
    }
}