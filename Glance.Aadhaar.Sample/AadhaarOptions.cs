using Glance.Aadhaar.Agency;
using Glance.Aadhaar.Device;

namespace Glance.Aadhaar.Sample;

public class AadhaarOptions
{
    public UserAgency AgencyInfo { get; set; }
    
    public DeviceInfo DeviceInfo { get; set; }

    public string UidaiEncryptionKeyPath { get; set; }
    
    public string UidaiSignatureKeyPath { get; set; }
    
    public string AuaDecryptionKeyPath { get; set; }
    
    public string AuaSignatureKeyPath { get; set; }
}