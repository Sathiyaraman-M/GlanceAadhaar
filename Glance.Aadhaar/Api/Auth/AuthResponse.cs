using System.Xml.Linq;
using Glance.Aadhaar.Constants;
using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Api.Auth;

public class AuthResponse : ApiResponse
{
    public bool IsAuthenticationPassed { get; set; }
    
    public string ActionCode { get; set; }
    
    public AuthInfo AuthInfo { get; set; }

    protected override void DeserializeXml(XElement element)
    {
        base.DeserializeXml(element);
        IsAuthenticationPassed = element.Element("ret")?.Value[0] == HelperConstants.Yes;
        ActionCode = element.Element("actn")?.Value;
        AuthInfo = new AuthInfo { InfoValue = element.Element("info")?.Value };
    }

    protected override XElement SerializeXml(string elementName)
    {
        var authResponse = base.SerializeXml(elementName);
        authResponse.Add(new XAttribute("ret", IsAuthenticationPassed ? HelperConstants.Yes : HelperConstants.No), 
            new XAttribute("info", AuthInfo.InfoValue));
        if(!string.IsNullOrWhiteSpace(ActionCode))
            authResponse.Add(new XAttribute("actn", ActionCode ?? string.Empty));
        return authResponse;
    }
}