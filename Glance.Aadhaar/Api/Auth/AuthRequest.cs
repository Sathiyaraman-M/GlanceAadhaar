﻿using System.Xml.Linq;
using Glance.Aadhaar.Constants;
using Glance.Aadhaar.Device;
using Glance.Aadhaar.Helper;
using Glance.Aadhaar.Resident;
using Glance.Aadhaar.Security;

namespace Glance.Aadhaar.Api.Auth;

public class AuthRequest : ApiRequest
{
    public const string AuthVersion = "2.5";

    public AuthRequest()
    {
        
    }

    public AuthRequest(AuthContext authContext)
    {
        ValidateNull(authContext, nameof(authContext));

        AadhaarNumber = authContext.AadhaarNumber;
        HasResidentConsent = authContext.HasResidentConsent;
        Data = authContext.Data;
        DeviceInfo = authContext.DeviceInfo;
        Hmac = authContext.Hmac;
        SessionKeyInfo = authContext.KeyInfo;
        Terminal = authContext.Terminal;
        Uses = authContext.Uses;
        AuthInfo = authContext.AuthInfo;
    }

    public override string ApiName => "Auth";

    private string _aadhaarNumber;
    
    public string AadhaarNumber
    {
        get => _aadhaarNumber;
        set => _aadhaarNumber = ValidateAadhaarNumber(value, nameof(AadhaarNumber));
    }
    
    public bool HasResidentConsent { get; set; }
    
    public AuthUsage Uses { get; set; }
    
    public DeviceInfo DeviceInfo { get; set; }
    
    public SessionKeyInfo SessionKeyInfo { get; set; }
    
    public EncryptedData Data { get; set; }
    
    public string Hmac { get; set; }
    
    public AuthInfo AuthInfo { get; set; }

    protected override void DeserializeXml(XElement element)
    {
        base.DeserializeXml(element);
        
        AadhaarNumber = element.Element("uid")?.Value;
        HasResidentConsent = element.Element("rc")?.Value[0] == HelperConstants.YesUpperCase;
        DeviceInfo = new DeviceInfo(element.Element("Devices"));
        Uses = new AuthUsage(element.Element("Uses"));
        SessionKeyInfo = new SessionKeyInfo(element.Element("Skey"));
        Data = new EncryptedData(element.Element("Data"));
        Hmac = element.Element("Hmac")?.Value;
    }

    protected override XElement SerializeXml(XName name)
    {
        ValidateNull(Uses, nameof(Uses));
        
        var authRequest = base.SerializeXml(name.LocalName);
        authRequest.Add(new XAttribute("uid", AadhaarNumber),
            new XAttribute("ver", AuthVersion),
            new XAttribute("rc", HasResidentConsent ? HelperConstants.YesUpperCase : HelperConstants.NoUpperCase),
            Uses.ToXml("Uses"),
            DeviceInfo.ToXml("Devices"),
            SessionKeyInfo.ToXml("Skey"),
            Data.ToXml("Data"),
            new XElement("Hmac", Hmac));
        
        Signer?.ComputeSignature(authRequest);
        
        return authRequest;
    }
}