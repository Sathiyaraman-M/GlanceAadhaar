using System.Xml.Linq;
using Glance.Aadhaar.Constants;
using Glance.Aadhaar.Enums;

namespace Glance.Aadhaar.Helper;

public class FullAddress : IUsed, IXml
{
    private int _matchPercent = HelperConstants.MaxMatchPercent;
    private int _ilMatchPercent = HelperConstants.MaxMatchPercent;

    public FullAddress()
    {
        
    }

    public FullAddress(XElement element) => FromXml(element);
    
    public string Address { get; set; }
    
    public string IlAddress { get; set; }
    
    public MatchingStrategy Match { get; set; } = MatchingStrategy.Partial;
    
    public int MatchPercent
    {
        get => _matchPercent;
        set => _matchPercent = ValidateMatchPercent(value, nameof(MatchPercent));
    }
    
    public int IlMatchPercent
    {
        get => _ilMatchPercent;
        set => _ilMatchPercent = ValidateMatchPercent(value, nameof(IlMatchPercent));
    }
    
    public bool IsUsed => !(string.IsNullOrWhiteSpace(Address) &&
                            string.IsNullOrWhiteSpace(IlAddress));
    public void FromXml(XElement element)
    {
        ValidateNull(element, nameof(element));
        
        Address = element.Element("av")?.Value;
        IlAddress = element.Element("lav")?.Value;
        
        var value = element.Attribute("ms")?.Value;
        Match = Enum.TryParse(value, true, out MatchingStrategy match) ? match : MatchingStrategy.Exact;
        if (Match == MatchingStrategy.Partial)
        {
            MatchPercent = int.Parse(element.Attribute("mv")?.Value);
            IlMatchPercent = int.Parse(element.Attribute("lmv")?.Value);
        }
        else
        {
            MatchPercent = IlMatchPercent = HelperConstants.MaxMatchPercent;
        }
    }

    public XElement ToXml(string elementName)
    {
        var fullAddress = new XElement(elementName,
            new XAttribute("av", Address ?? string.Empty),
            new XAttribute("lav", IlAddress ?? string.Empty));
        if(Match == MatchingStrategy.Partial && !(string.IsNullOrWhiteSpace(Address) && string.IsNullOrWhiteSpace(IlAddress)))
            fullAddress.Add(new XAttribute("ms", (char)Match),
                new XAttribute("mv", MatchPercent),
                new XAttribute("lmv", IlMatchPercent));
        
        fullAddress.RemoveEmptyAttributes();
        
        return fullAddress;
    }

    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Address))
            return Address;
        return !string.IsNullOrWhiteSpace(IlAddress) ? IlAddress : base.ToString();
    }
}