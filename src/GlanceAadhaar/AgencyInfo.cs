namespace GlanceAadhaar;

public class AgencyInfo
{
    public string AuaLicenseKey { get; set; }
    
    public string AsaLicenseKey { get; set; }
    
    public string AuaCode { get; set; }
    
    public string SubAuaCode { get; set; }
    
    public IDictionary<string, Uri> Hosts { get; set; }
}