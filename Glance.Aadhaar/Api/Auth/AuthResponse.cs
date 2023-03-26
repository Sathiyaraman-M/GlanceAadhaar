using System.Xml.Linq;
using Glance.Aadhaar.Constants;

namespace Glance.Aadhaar.Api.Auth;

public class AuthResponse : ApiResponse
{
    public bool IsAuthenticationPassed { get; set; }
    
    public string ActionCode { get; set; }

    protected override void DeserializeXml(XElement element)
    {
        base.DeserializeXml(element);
        IsAuthenticationPassed = element.Element("ret")?.Value[0] == HelperConstants.Yes;
        ActionCode = element.Element("actn")?.Value;
    }
}