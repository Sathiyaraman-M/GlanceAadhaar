using System.Xml.Linq;
using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Device;

public class DeviceInfo : IXml
{
    public DeviceInfo()
    {
        
    }
    
    public DeviceInfo(XElement element)
    {
        FromXml(element);
    }
    
    public string DeviceProviderId { get; set; }
    
    public string RegisteredDeviceServiceId { get; set; }
    
    public string RegisteredDeviceServiceVersion { get; set; }
    
    public string DeviceCode { get; set; }
    
    public string DeviceModelId { get; set; }
    
    public string DevicePublicKeyCertificate { get; set; }
    
    public void FromXml(XElement element)
    {
        ValidateNull(element, nameof(element));
        
        DeviceProviderId = element.Element("dpId")?.Value;
        RegisteredDeviceServiceId = element.Element("rdsId")?.Value;
        RegisteredDeviceServiceVersion = element.Element("rdsVer")?.Value;
        DeviceCode = element.Element("dc")?.Value;
        DeviceModelId = element.Element("mi")?.Value;
        DevicePublicKeyCertificate = element.Element("mc")?.Value;
    }

    public XElement ToXml(string elementName)
    {
        var deviceInfo = new XElement(elementName,
            new XAttribute("dpId", DeviceProviderId),
            new XAttribute("rdsId", RegisteredDeviceServiceId),
            new XAttribute("rdsVer", RegisteredDeviceServiceVersion),
            new XAttribute("dc", DeviceCode),
            new XAttribute("mi", DeviceModelId),
            new XAttribute("mc", DevicePublicKeyCertificate));
        
        
        return deviceInfo;
    }
}