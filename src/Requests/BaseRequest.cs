using GlanceAadhaar.Contracts;
using GlanceAadhaar.Helpers;

namespace GlanceAadhaar.Requests;

public abstract class BaseRequest : IXmlElement
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
                throw new ArgumentOutOfRangeException(nameof(SubAuaCode), OutOfRangeSubAuaCode);
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

    protected virtual XElement Serialize(string elementName)
    {
        ThrowIfNullOrEmptyString(AuaCode, nameof(AuaCode));
        ThrowIfNullOrEmptyString(AuaLicenseKey, nameof(AuaLicenseKey));
        ThrowIfNullOrEmptyString(Transaction, nameof(Transaction));
        ThrowIfNullOrEmptyString(Terminal, nameof(Terminal));

        return new XElement(elementName,
            new XAttribute("ac", AuaCode),
            new XAttribute("sa", SubAuaCode),
            new XAttribute("lk", AuaLicenseKey),
            new XAttribute("txn", Transaction),
            new XAttribute("tid", Terminal));
    }

    public void LoadFromXml(XElement element) => Deserialize(element);
    
    protected virtual void Deserialize(XElement element)
    {
        ThrowIfNull(element, nameof(element));

        AuaCode = element.Attribute("ac")?.Value;
        SubAuaCode = element.Attribute("sa")?.Value;
        AuaLicenseKey = element.Attribute("lk")?.Value;
        Transaction = element.Attribute("txn")?.Value;
        Terminal = element.Attribute("tid")?.Value;
    }
}