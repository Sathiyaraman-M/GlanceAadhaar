using System.Xml.Linq;

namespace Glance.Aadhaar.Security;

public interface ISigner
{
    XElement ComputeSignature(XElement element);
}