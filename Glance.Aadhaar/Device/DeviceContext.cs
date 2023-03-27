using System.Globalization;
using System.Xml.Linq;
using Glance.Aadhaar.Constants;
using Glance.Aadhaar.Helper;
using Glance.Aadhaar.Security;

namespace Glance.Aadhaar.Device;

public abstract class DeviceContext : IXml
{
    private string _aadhaarNumber;
    
    public DeviceContext()
    {
        
    }

    public DeviceContext(XElement element)
    {
        FromXml(element);
    }
    
    public abstract string ApiName { get; }
    
    public string AadhaarNumber
    {
        get => _aadhaarNumber;
        set => _aadhaarNumber = ValidateAadhaarNumber(value, nameof(AadhaarNumber));
    }
    
    public string Terminal { get; set; } = HelperConstants.PublicTerminal;
    
    public DateTimeOffset Timestamp { get; set; }
    
    public DeviceInfo DeviceInfo { get; set; }
    
    public SessionKeyInfo KeyInfo { get; set; }
    
    public EncryptedData Data { get; set; }
    
    public string Hmac { get; set; }

    public void FromXml(XElement element) => DeserializeXml(element);

    public XElement ToXml(string elementName) => SerializeXml(elementName);
    
    public XElement ToXml() => ToXml(ApiName);
    
    protected virtual void DeserializeXml(XElement element)
    {
        ValidateNull(element, nameof(element));

        AadhaarNumber = element.Attribute("uid")?.Value;
        Terminal = element.Attribute("tid")?.Value;
        Timestamp = DateTimeOffset.ParseExact(element.Attribute("ts")?.Value, HelperConstants.TimestampFormat, CultureInfo.InvariantCulture);

        var xml = element.Element("Meta");
        DeviceInfo = xml != null ? new DeviceInfo(xml) : null;

        xml = element.Element("Skey");
        KeyInfo = xml != null ? new SessionKeyInfo(xml) : null;

        xml = element.Element("Data");
        Data = xml != null ? new EncryptedData(xml) : null;

        xml = element.Element("Hmac");
        Hmac = xml?.Value;
    }
    
    protected virtual XElement SerializeXml(string elementName)
    {
        var deviceContext = new XElement(elementName,
            new XAttribute("uid", AadhaarNumber ?? string.Empty),
            new XAttribute("tid", Terminal),
            new XAttribute("ts", Timestamp.ToString(HelperConstants.TimestampFormat, CultureInfo.InvariantCulture)));

        if (DeviceInfo != null)
            deviceContext.Add(DeviceInfo.ToXml("Meta"));
        if (KeyInfo != null)
            deviceContext.Add(DeviceInfo.ToXml("Skey"));
        if (Data != null)
            deviceContext.Add(DeviceInfo.ToXml("Data"));
        if (!string.IsNullOrWhiteSpace(Hmac))
            deviceContext.Add(new XElement("Hmac", Hmac));

        return deviceContext;
    }
}