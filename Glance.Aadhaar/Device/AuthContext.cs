using System.Security.Cryptography;
using System.Xml.Linq;
using Glance.Aadhaar.Constants;
using Glance.Aadhaar.Helper;
using Glance.Aadhaar.Resident;
using Glance.Aadhaar.Security;

namespace Glance.Aadhaar.Device;

public class AuthContext : DeviceContext
{
    public AuthContext()
    {
        
    }

    public AuthContext(XElement element) => FromXml(element);

    public override string ApiName => "Auth";
    
    public bool HasResidentConsent { get; set; }
    
    public AuthUsage Uses { get; set; }
    
    public AuthInfo AuthInfo { get; set; }

    public virtual void Encrypt(PersonalInfo data, SessionKey key)
    {
        ValidateNull(data, nameof(data));
        ValidateNull(key, nameof(key));
        if (!HasResidentConsent)
            throw new ArgumentException(RequiredConsent, nameof(HasResidentConsent));

        // Create Pid bytes.
        var pidXml = data.ToXml("Pid").ToString(SaveOptions.DisableFormatting);
        var pidBytes = pidXml.GetBytes();
        var encryptedPid = key.Encrypt(pidBytes);
        var encryptedPidHash = key.Encrypt(SHA256.HashData(encryptedPid));
        KeyInfo = key.KeyInfo;

        AadhaarNumber = data.AadhaarNumber;
        Timestamp = data.TimeStamp;
        Uses = data.Uses;
        Data = new EncryptedData { Data = Convert.ToBase64String(encryptedPid) };
        Hmac = Convert.ToBase64String(encryptedPidHash);

        AuthInfo ??= new AuthInfo();
        AuthInfo.Timestamp = data.TimeStamp;

        const string demoStart = "<Demo", demoEnd = "</Demo";
        var startOfDemoStart = pidXml.LastIndexOf(demoStart, StringComparison.Ordinal);
        if (startOfDemoStart >= 0)
        {
            var startOfDemoEnd = pidXml.IndexOf(demoEnd, startOfDemoStart + demoStart.Length, StringComparison.Ordinal);
            var realEnd = pidXml.IndexOf(">", startOfDemoEnd + demoEnd.Length, StringComparison.Ordinal);
            var demoXml = pidXml.Substring(startOfDemoStart, realEnd - startOfDemoStart + 1);
            var demoBytes = demoXml.GetBytes();
            var demoHash = SHA256.HashData(demoBytes);
            AuthInfo.DemographicHash = demoHash.ToHex();
            Array.Clear(demoBytes, 0, demoBytes.Length);
        }
        Array.Clear(pidBytes, 0, pidBytes.Length);
    }
    
    public virtual async Task EncryptAsync(PersonalInfo data, SessionKey key) => await Task.Run(() => Encrypt(data, key));
    
    protected override void DeserializeXml(XElement element)
    {
        base.DeserializeXml(element);
        HasResidentConsent = element.Attribute("rc")?.Value[0] == HelperConstants.YesUpperCase;
        Uses = new AuthUsage(element.Element("Uses"));

        var info = element.Attribute("info")?.Value;
        AuthInfo = info != null ? new AuthInfo { InfoValue = info } : null;
    }
    
    protected override XElement SerializeXml(string name)
    {
        ValidateNull(Uses, nameof(Uses));

        var authContext = base.SerializeXml(name);
        authContext.Add(new XAttribute("rc", HelperConstants.YesUpperCase),
            Uses.ToXml("Uses"));

        if (AuthInfo != null)
            authContext.Add(new XAttribute("info", AuthInfo.Encode()));

        return authContext;
    }
}