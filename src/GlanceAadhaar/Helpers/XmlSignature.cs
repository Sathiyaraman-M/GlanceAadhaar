using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace GlanceAadhaar.Helpers;

public static class XmlSignature
{
    public static XElement Sign(XElement element, X509Certificate2 certificate)
    {
        ThrowIfNull(element, nameof(element));
        ThrowIfNull(certificate, nameof(certificate));
        
        if (!certificate.HasPrivateKey)
        {
            throw new CryptographicException(NoPrivateKeyFound);
        }
        
        using var rsa = certificate.GetRSAPrivateKey();
        var document = new XmlDocument { PreserveWhitespace = false };
        using var reader = element.CreateReader();
        document.Load(reader);
        
        var signedXml = new SignedXml(document) { SigningKey = rsa };
        var reference = new Reference { Uri = string.Empty };
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        signedXml.AddReference(reference);
        
        var keyInfo = new KeyInfo();
        var clause = new KeyInfoX509Data(certificate);
        clause.AddSubjectName(certificate.Subject);
        keyInfo.AddClause(clause);
        signedXml.KeyInfo = keyInfo;
        
        signedXml.ComputeSignature();
        var xmlDigitalSignature = signedXml.GetXml();
        var xDocument = new XDocument();
        using var writer = xDocument.CreateWriter();
        xmlDigitalSignature.WriteTo(writer);
        element.Add(xDocument.Root);
        
        return element;
    }

    public static bool Verify(XElement element, X509Certificate2 certificate)
    {
        ThrowIfNull(element, nameof(element));
        ThrowIfNull(certificate, nameof(certificate));
        
        var document = new XmlDocument { PreserveWhitespace = false };
        using var reader = element.CreateReader();
        document.Load(reader);
        
        var signatureNodes = document.GetElementsByTagName("Signature");
        if (signatureNodes.Count == 0)
        {
            throw new CryptographicException(NoSignatureFound);
        }
        
        var signedXml = new SignedXml(document);
        signedXml.LoadXml((XmlElement)signatureNodes[0]);
        
        return signedXml.CheckSignature(certificate, true);
    }
}