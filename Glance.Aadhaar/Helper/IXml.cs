using System.Xml.Linq;

namespace Glance.Aadhaar.Interfaces;

internal interface IXml
{
    void FromXml(XElement element);
    
    XElement ToXml(string elementName);
}