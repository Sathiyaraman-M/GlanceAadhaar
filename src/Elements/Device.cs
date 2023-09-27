namespace GlanceAadhaar.Elements;

public class Device : IXmlElement
{
    public Device()
    {
        
    }

    public Device(XElement element)
    {
        LoadFromXml(element);
    }
    
    public string DeviceProviderId { get; set; }
    
    public string RegisteredDeviceServiceId { get; set; }
    
    public string RegisteredDeviceServiceVersion { get; set; }
    
    public string DeviceCode { get; set; }
    
    public string DeviceModelId { get; set; }
    
    public string DevicePublicKeyCertificate { get; set; }
    
    public XElement ConvertToXml(string elementName)
    {
        return new XElement(elementName,
            new XElement("dpId", DeviceProviderId),
            new XElement("rdsId", RegisteredDeviceServiceId),
            new XElement("rdsVer", RegisteredDeviceServiceVersion),
            new XElement("dc", DeviceCode),
            new XElement("mi", DeviceModelId),
            new XElement("mc", DevicePublicKeyCertificate));
    }

    public void LoadFromXml(XElement element)
    {
        ThrowIfNull(element, nameof(element));
        
        DeviceProviderId = element.Element("dpId")!.Value;
        RegisteredDeviceServiceId = element.Element("rdsId")!.Value;
        RegisteredDeviceServiceVersion = element.Element("rdsVer")!.Value;
        DeviceCode = element.Element("dc")!.Value;
        DeviceModelId = element.Element("mi")!.Value;
        DevicePublicKeyCertificate = element.Element("mc")!.Value;
    }
}