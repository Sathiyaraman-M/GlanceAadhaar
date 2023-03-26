using System.Xml.Linq;
using Glance.Aadhaar.Device;
using Glance.Aadhaar.Resident;

namespace Glance.Aadhaar.Api.Auth;

public class AuthRequest : ApiRequest
{
    public const string AuthVersion = "2.5";

    public AuthRequest()
    {
        
    }

    public AuthRequest(AuthContext authContext)
    {
        
    }

    protected override string ApiName => "Auth";

    private string _userId;
    
    public string UserId
    {
        get => _userId;
        set => _userId = ValidateAadhaarNumber(value, nameof(UserId));
    }
    
    public string HasResidentConsent { get; set; }
    
    public AuthUsage Uses { get; set; }

    protected override void DeserializeXml(XElement element)
    {
        base.DeserializeXml(element);
        
        UserId = element.Element("uid")?.Value;
        HasResidentConsent = element.Element("rc")?.Value;
        Uses = new AuthUsage(element.Element("Uses"));
    }

    protected override XElement SerializeXml(XName name)
    {
        ValidateNull(Uses, nameof(Uses));
        
        var authRequest = base.SerializeXml(name.LocalName);
        authRequest.Add(new XAttribute("uid", UserId),
            new XAttribute("ver", AuthVersion),
            new XAttribute("rc", HasResidentConsent),
            Uses.ToXml("Uses"));
        
        Signer?.ComputeSignature(authRequest);
        
        return authRequest;
    }
}