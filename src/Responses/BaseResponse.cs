namespace GlanceAadhaar.Responses;

public abstract class BaseResponse : IXmlElement 
{
    public XElement ConvertToXml(string elementName)
    {
        throw new NotImplementedException();
    }

    public void LoadFromXml(XElement element)
    {
        throw new NotImplementedException();
    }
}