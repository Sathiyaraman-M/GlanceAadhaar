using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Glance.Aadhaar.Security;

public class SessionKey : IDisposable
{
    public static TimeSpan SynchronizedKeyTimeout = new TimeSpan(3, 45, 0);

    private readonly bool _leaveOpen;
    private readonly Aes _aes;
    private readonly DateTimeOffset? _seedCreationTime;
    private readonly RandomNumberGenerator _random;
    private readonly byte[] _seedKey;

    private byte[] _syncKey;
    
    public SessionKey(string fileName, bool isSynchronized) : this(new X509Certificate2(fileName), isSynchronized)
    {
        _leaveOpen = false;
    }

    public SessionKey(X509Certificate2 uidaiKey, bool isSynchronized)
    {
        ValidateNull(uidaiKey, nameof(uidaiKey));

        UidaiKey = uidaiKey;
        _leaveOpen = true;
        _aes = Aes.Create();
        _aes.Mode = CipherMode.ECB;
        if (!isSynchronized) return;
        IsSynchronized = true;
        _random = RandomNumberGenerator.Create();
        _seedKey = new byte[32];
        _aes.Key.CopyTo(_seedKey, 0);
        _seedCreationTime = DateTimeOffset.Now;

    }

    public SessionKeyInfo KeyInfo => EncryptKey();
    
    public X509Certificate2 UidaiKey { get; }
    
    public bool IsSynchronized { get; }
    
    public bool HasExpired => IsSynchronized && _seedCreationTime - DateTimeOffset.Now > SynchronizedKeyTimeout;
    
    public byte[] Encrypt(byte[] data)
    {
        ValidateNull(data, nameof(data));
        if (HasExpired)
            throw new InvalidOperationException(ExpiredSynchronizedKey);

        using var transform = _aes.CreateEncryptor();
        return transform.TransformFinalBlock(data, 0, data.Length);
    }
    
    private void GenerateKey()
    {
        if (IsSynchronized)
        {
            _syncKey ??= new byte[20];
            _random.GetBytes(_syncKey);
            using var transform = _aes.CreateEncryptor(_seedKey, null);
            var encryptionKey = transform.TransformFinalBlock(_syncKey, 0, _syncKey.Length);
            Array.Resize(ref encryptionKey, 32);
            _aes.Key = encryptionKey;
        }
        else
            _aes.GenerateKey();
    }
    
    private SessionKeyInfo EncryptKey()
    {
        var rsa = UidaiKey.GetRSAPublicKey();
        if (rsa == null)
            throw new ArgumentNullException(nameof(UidaiKey), NoPublicKey);

        // For normal session key, syncKey is always null.
        var key = Convert.ToBase64String(_syncKey ?? rsa.Encrypt(_aes.Key, RSAEncryptionPadding.Pkcs1));
        GenerateKey();

        return new SessionKeyInfo
        {
            CertificateIdentifier = UidaiKey.NotAfter,
            Key = key,
        };
    }

    public void Dispose() => Dispose(true);
    
    private bool _disposedValue;
    
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
        {
            _aes.Dispose();
            _random?.Dispose();
            if (!_leaveOpen)
                UidaiKey.Dispose();
            if (_seedKey != null)
                Array.Clear(_seedKey, 0, _seedKey.Length);
            if (_syncKey != null)
                Array.Clear(_syncKey, 0, _syncKey.Length);
        }
        _disposedValue = true;
    }
}