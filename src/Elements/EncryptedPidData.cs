namespace GlanceAadhaar.Elements;

public class EncryptedPidData : IXmlElement
{
    public EncryptedPidData()
    {
        
    }

    public EncryptedPidData(XElement element)
    {
        LoadFromXml(element);
    }
    
    public string Data { get; set; }
    
    public XElement ConvertToXml(string elementName)
    {
        ThrowIfNullOrEmptyString(Data, nameof(Data));
        
        return new XElement(elementName, new XAttribute("type",'X'), Data);
    }

    public void LoadFromXml(XElement element)
    {
        ThrowIfNull(element, nameof(element));
        ThrowIfNullOrEmptyString(element.Value, nameof(element.Value));
        Data = element.Value;
    }
}