using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using GlanceAadhaar.Elements;

namespace GlanceAadhaar.Helpers;

public class SessionKeyInfo : IDisposable
{
    private readonly AesGcm _aesGcm;

    private readonly byte[] _iv;
    
    private readonly byte[] _key;
    
    private readonly byte[] _tag;

    public SessionKeyInfo(string filePath) : this(new X509Certificate2(filePath))
    {
        
    }

    public SessionKeyInfo(X509Certificate2 aadhaarPublicKey)
    {
        ThrowIfNull(aadhaarPublicKey, nameof(aadhaarPublicKey));
        AadhaarPublicKey = aadhaarPublicKey;
        
        _key = new byte[32];
        _iv = new byte[AesGcm.NonceByteSizes.MaxSize];
        _tag = new byte[AesGcm.TagByteSizes.MaxSize];
        RandomNumberGenerator.Fill(_key);
        RandomNumberGenerator.Fill(_iv);
        _aesGcm = new AesGcm(_key);
    }

    private X509Certificate2 AadhaarPublicKey { get; }

    public SessionKey SessionKey => EncryptKey();

    private SessionKey EncryptKey()
    {
        var rsa = AadhaarPublicKey.GetRSAPublicKey();
        ThrowIfNull(rsa, NoPublicKeyFound);
        
        RandomNumberGenerator.Fill(_key);
        var key = Convert.ToBase64String(rsa!.Encrypt(_key, RSAEncryptionPadding.Pkcs1));
        return new SessionKey
        {
            CertificateIdentifier = DateTimeOffset.UtcNow,
            Key = key
        };
    }
    
    public byte[] Encrypt(byte[] data)
    {
        ThrowIfNull(data, nameof(data));
        var cipherText = new byte[data.Length];
        _aesGcm.Encrypt(_iv, data, cipherText, _tag);
        return cipherText;
    }
    
    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
        {
            _aesGcm.Dispose();
        }

        _disposed = true;
    }
}