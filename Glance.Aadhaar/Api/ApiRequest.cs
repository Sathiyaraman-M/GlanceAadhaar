using System.Xml.Linq;
using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Api;

public abstract class ApiRequest : IXml
{
    private string _auaCode;
    private string _subAuaCode;
    private string _auaLicenseKey;
    private Transaction _transaction = new();
    
    public string Terminal { get; set; }
    
    public abstract string ApiName { get; }

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
        get => !string.IsNullOrWhiteSpace(_subAuaCode) ? _subAuaCode : _auaCode;
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
    
    public Transaction Transaction
    {
        get => _transaction;
        set
        {
            if(((string)value)?.Length > 50)
                throw new ArgumentOutOfRangeException(nameof(Transaction), OutOfRangeTransaction);
            _transaction = value;
        }
    }

    public void FromXml(XElement element) => DeserializeXml(element);

    public XElement ToXml(string elementName) => SerializeXml(elementName);

    public XElement ToXml() => ToXml(ApiName);

    protected virtual void DeserializeXml(XElement element)
    {
        ValidateNull(element, nameof(element));

        Terminal = element.Attribute("tid")?.Value;
        AuaCode = element.Attribute("ac")?.Value;
        SubAuaCode = element.Attribute("sa")?.Value;
        Transaction = element.Attribute("txn")?.Value;
        AuaLicenseKey = element.Attribute("lk")?.Value;
        
    }
    
    protected virtual XElement SerializeXml(XName name)
    {
        ValidateEmptyString(Terminal, nameof(Terminal));
        ValidateEmptyString(AuaCode, nameof(AuaCode));
        ValidateEmptyString(Transaction, nameof(Transaction));
        ValidateEmptyString(AuaLicenseKey, nameof(AuaLicenseKey));

        return new XElement(name,
            new XAttribute("tid", Terminal),
            new XAttribute("ac", AuaCode),
            new XAttribute("sa", SubAuaCode),
            new XAttribute("txn", Transaction),
            new XAttribute("lk", AuaLicenseKey));
    }
}