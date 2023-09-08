using System.Xml.Linq;
using GlanceAadhaar.Contracts;
using GlanceAadhaar.Elements;

namespace GlanceAadhaar.Requests;

public abstract class ApiRequest : IXmlElement
{
    private string _auaCode, _subAuaCode, _auaLicenseKey;
    private TransactionIdentifier _transaction = new();

    public string AuaCode
    {
        get => _auaCode;
        set
        {
            if(value?.Length > 10)
                throw new ArgumentOutOfRangeException(nameof(AuaCode), OutOfRangeAuaCode);
            _auaCode = value;
        }
    }
    
    public string SubAuaCode
    {
        get => string.IsNullOrEmpty(_subAuaCode) ? _auaCode : _subAuaCode;
        set
        {
            if(value?.Length > 10)
                throw new ArgumentOutOfRangeException(nameof(SubAuaCode), OutOfRangeAuaCode);
            _subAuaCode = value;
        }
    }
    
    public string AuaLicenseKey
    {
        get => _auaLicenseKey;
        set
        {
            if(value?.Length > 64)
                throw new ArgumentOutOfRangeException(nameof(AuaLicenseKey), OutOfRangeAuaLicenseKey);
            _auaLicenseKey = value;
        }
    }
    
    public TransactionIdentifier Transaction
    {
        get => _transaction;
        set 
        {
            if(((string)value)?.Length > 50)
                throw new ArgumentOutOfRangeException(nameof(Transaction), OutOfRangeTransaction);
            _transaction = value;
        }
    }
    
    public string Terminal { get; set; }
    
    public XElement ConvertToXml(string elementName) => Serialize(elementName);

    public void LoadFromXml(XElement element)
    {
        throw new NotImplementedException();
    }
    
    protected virtual XElement Serialize(XName name)
    {
        ThrowIfNullOrEmptyString(AuaCode, nameof(AuaCode));
        ThrowIfNullOrEmptyString(AuaLicenseKey, nameof(AuaLicenseKey));
        ThrowIfNullOrEmptyString(Transaction, nameof(Transaction));
        ThrowIfNullOrEmptyString(Terminal, nameof(Terminal));

        return new XElement(name,
            new XAttribute("ac", AuaCode),
            new XAttribute("sa", SubAuaCode),
            new XAttribute("lk", AuaLicenseKey),
            new XAttribute("txn", Transaction),
            new XAttribute("tid", Terminal));
    }
}