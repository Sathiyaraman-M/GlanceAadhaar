using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Linq;

namespace Glance.Aadhaar.Security;

public class XmlSignature : ISigner, IVerifier, IDisposable
{
    public X509Certificate2 Signer { get; set; }
    
    public X509Certificate2 Verifier { get; set; }
    
    public XElement ComputeSignature(XElement xml)
    {
        ValidateNull(xml, nameof(xml));
        ValidateNull(Signer, nameof(Signer));
        if (!Signer.HasPrivateKey)
            throw new CryptographicException(NoPrivateKey);

        var signedXml = new SignedXml(GetXmlDocument(xml)) { SigningKey = Signer.GetRSAPrivateKey() };

        // Add Reference.
        var reference = new Reference(string.Empty);
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        signedXml.AddReference(reference);

        // Add Key Info.
        var keyInfo = new KeyInfo();
        var clause = new KeyInfoX509Data(Signer);
        clause.AddSubjectName(Signer.Subject);
        keyInfo.AddClause(clause);
        signedXml.KeyInfo = keyInfo;

        // Compute Signature.
        signedXml.ComputeSignature();
        var signatureXml = GetXDocument(signedXml.GetXml()).Root;
        xml.Add(signatureXml);

        return signatureXml;
    }

    public bool VerifySignature(XElement xml)
    {
        ValidateNull(xml, nameof(xml));
        ValidateNull(Verifier, nameof(Verifier));

        var document = GetXmlDocument(xml);
        var nodeList = document.GetElementsByTagName("Signature");
        if (nodeList.Count == 0)
            throw new CryptographicException(NoSignature);
        var signedXml = new SignedXml(document);
        signedXml.LoadXml((XmlElement)nodeList[0]);

        return signedXml.CheckSignature(Verifier.PublicKey.GetRSAPublicKey());
    }
    
    private static XDocument GetXDocument(XmlNode node)
    {
        var document = new XDocument();
        using var writer = document.CreateWriter();
        node.WriteTo(writer);
        return document;
    }

    private static XmlDocument GetXmlDocument(XNode node)
    {
        var document = new XmlDocument { PreserveWhitespace = false };
        using var reader = node.CreateReader();
        document.Load(reader);
        return document;
    }
    
    private bool _disposedValue;
    
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
        {
            Signer?.Dispose();
            Verifier?.Dispose();
        }
        _disposedValue = true;
    }
    
    public void Dispose() => Dispose(true);
}