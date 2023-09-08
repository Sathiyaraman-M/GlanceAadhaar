using System.Xml.Linq;
using GlanceAadhaar.Contracts;

namespace GlanceAadhaar.Responses;

public abstract class ApiResponse : IXmlElement 
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