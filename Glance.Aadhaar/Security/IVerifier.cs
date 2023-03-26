using System.Xml.Linq;

namespace Glance.Aadhaar.Security;

public interface IVerifier
{
    bool VerifySignature(XElement xml);
}