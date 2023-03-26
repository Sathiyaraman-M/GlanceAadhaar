using System.Text;
using System.Xml.Linq;
using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Resident;

public class Address : IUsed, IXml
{
    private string _pinCode;

    public Address()
    {
        
    }

    public Address(XElement element) => FromXml(element);
    
    public string CareOf { get; set; }
    
    public string House { get; set; }
    
    public string Street { get; set; }
    
    public string Landmark { get; set; }
    
    public string Locality { get; set; }
    
    public string VillageOrCity { get; set; }
    
    public string SubDistrict { get; set; }
    
    public string District { get; set; }
    
    public string State { get; set; }
    
    public string PinCode
    {
        get => _pinCode;
        set
        {
            if (!string.IsNullOrEmpty(value) && !AadhaarHelper.ValidatePinCode(value))
                throw new ArgumentException(InvalidPinCode, nameof(PinCode));
            _pinCode = value;
        }
    }
    
    public string PostOffice { get; set; }

    public bool IsUsed  => !(string.IsNullOrWhiteSpace(CareOf) &&
                             string.IsNullOrWhiteSpace(House) &&
                             string.IsNullOrWhiteSpace(Street) &&
                             string.IsNullOrWhiteSpace(Landmark) &&
                             string.IsNullOrWhiteSpace(Locality) &&
                             string.IsNullOrWhiteSpace(VillageOrCity) &&
                             string.IsNullOrWhiteSpace(SubDistrict) &&
                             string.IsNullOrWhiteSpace(District) &&
                             string.IsNullOrWhiteSpace(State) &&
                             string.IsNullOrWhiteSpace(PinCode) &&
                             string.IsNullOrWhiteSpace(PostOffice));
    public void FromXml(XElement element)
    {
        ValidateNull(element, nameof(element));
        
        CareOf = element.Element("co")?.Value;
        House = element.Element("house")?.Value;
        Street = element.Element("street")?.Value;
        Landmark = element.Element("lm")?.Value;
        Locality = element.Element("loc")?.Value;
        VillageOrCity = element.Element("vtc")?.Value;
        SubDistrict = element.Element("subdist")?.Value;
        District = element.Element("dist")?.Value;
        State = element.Element("state")?.Value;
        PinCode = element.Element("pc")?.Value;
        PostOffice = element.Element("po")?.Value;
    }

    public XElement ToXml(string elementName)
    {
        var address = new XElement(elementName,
            new XAttribute("co", CareOf ?? string.Empty),
            new XAttribute("house", House ?? string.Empty),
            new XAttribute("street", Street ?? string.Empty),
            new XAttribute("lm", Landmark ?? string.Empty),
            new XAttribute("loc", Locality ?? string.Empty),
            new XAttribute("vtc", VillageOrCity ?? string.Empty),
            new XAttribute("subdist", SubDistrict ?? string.Empty),
            new XAttribute("dist", District ?? string.Empty),
            new XAttribute("state", State ?? string.Empty),
            new XAttribute("pc", PinCode ?? string.Empty),
            new XAttribute("po", PostOffice ?? string.Empty));

        address.RemoveEmptyAttributes();

        return address;
    }
    
    public override string ToString()
    {
        var builder = new StringBuilder(250);

        // Address Line 1
        if (!string.IsNullOrWhiteSpace(CareOf))
            builder.AppendLine($"C/o {CareOf}");

        // Address Line 2
        builder.AppendLine(House);

        // Address Line 3
        if (!string.IsNullOrWhiteSpace(Street))
            builder.AppendLine(Street);

        // Address Line 4
        if (!string.IsNullOrWhiteSpace(Landmark) || !string.IsNullOrWhiteSpace(Locality))
            builder.AppendLine($"{Landmark}, {Locality}".Trim(',', ' '));

        // Address Line 5
        builder.AppendLine($"{VillageOrCity}, {District}");

        // Address Line 6
        builder.AppendLine($"{State} – {PinCode}".Trim('–', ' '));

        return builder.ToString();
    }
}