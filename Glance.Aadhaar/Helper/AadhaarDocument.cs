using Glance.Aadhaar.Enums;

namespace Glance.Aadhaar.Helper;

public class AadhaarDocument
{
    public DocumentType Type { get; set; }
    public string Content { get; set; }
}