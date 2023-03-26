using System.Xml.Linq;

namespace Glance.Aadhaar.Helper;

internal interface IXml
{
    void FromXml(XElement element);
    
    XElement ToXml(string elementName);
}