using System.Globalization;
using System.Xml.Linq;
using Glance.Aadhaar.Enums;
using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Resident;

public class Demographic : IXml
{
    public Demographic()
    {
        
    }

    public Demographic(XElement element)
    {
        FromXml(element);
    }
    
    public IndianLanguage? LanguageUsed { get; set; }
    
    public Identity Identity { get; set; }
    
    public Address Address { get; set; }
    
    public Address IlAddress { get; set; }
    
    public FullAddress FullAddress { get; set; }
    
    public void FromXml(XElement element)
    {
        ValidateNull(element, nameof(element));
        
        var xml = element.Element("Pa");
        Address = xml != null ? new Address(xml) : null;
        
        xml = element.Element("Pi");
        Identity = xml != null ? new Identity(xml) : null;
        
        xml = element.Element("Pfa");
        FullAddress = xml != null ? new FullAddress(xml) : null;
        
        var lang = element.Attribute("lang")?.Value;
        LanguageUsed = lang != null ? (IndianLanguage?) int.Parse(lang) : null;
    }

    public XElement ToXml(string elementName)
    {
        var isIlUsed = !(string.IsNullOrWhiteSpace(Identity?.IlName) && string.IsNullOrWhiteSpace(FullAddress?.IlAddress));

        if (LanguageUsed == null && isIlUsed)
            throw new ArgumentException(RequiredIndianLanguage, nameof(LanguageUsed));
        if (Address != null && FullAddress != null)
            throw new ArgumentException(XorAddresses);

        var demographic = new XElement(elementName);
        if (isIlUsed)
            demographic.Add(new XAttribute("lang", ((int)LanguageUsed).ToString("D2", CultureInfo.InvariantCulture)));
        if (Identity != null)
            demographic.Add(Identity.ToXml("Pi"));
        if (Address != null)
            demographic.Add(Address.ToXml("Pa"));
        if (FullAddress != null)
            demographic.Add(FullAddress.ToXml("Pfa"));

        return demographic;
    }
}