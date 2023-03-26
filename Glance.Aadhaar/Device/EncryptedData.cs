using System.Xml.Linq;
using Glance.Aadhaar.Enums;
using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Device;

public class EncryptedData : IXml
{
    public EncryptedData()
    {
        
    }

    public EncryptedData(XElement element)
    {
        FromXml(element);
    }
    
    public string Data { get; set; }

    private static EncodingType EncodingType => EncodingType.Xml;

    public void FromXml(XElement element)
    {
        Data = ValidateNull(element, nameof(element)).Value;
    }

    public XElement ToXml(string elementName)
    {
        ValidateEmptyString(Data, nameof(Data));
        
        var encryptedData = new XElement(elementName,
            new XAttribute("type", (char)EncodingType), Data);
        
        return encryptedData;
    }
}