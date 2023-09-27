namespace GlanceAadhaar.Contracts;

internal interface IXmlElement
{
    XElement ConvertToXml(string elementName);
    
    void LoadFromXml(XElement element);
}