using System.Xml.Linq;
using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Resident;

public class PinValue : IXml, IUsed
{
    public string Otp { get; set; }
    
    public string Pin { get; set; }
    
    public void FromXml(XElement element)
    {
        throw new NotSupportedException();
    }

    public XElement ToXml(string elementName)
    {
        var pinValue = new XElement(elementName,
            new XAttribute("otp", Otp ?? string.Empty),
            new XAttribute("pin", Pin ?? string.Empty));

        pinValue.RemoveEmptyAttributes();

        return pinValue;
    }

    public bool IsUsed => !(string.IsNullOrWhiteSpace(Otp) && string.IsNullOrWhiteSpace(Pin));
}