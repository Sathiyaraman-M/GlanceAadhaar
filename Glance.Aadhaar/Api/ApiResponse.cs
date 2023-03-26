using System.Globalization;
using System.Security.Cryptography;
using System.Xml.Linq;
using Glance.Aadhaar.Interfaces;
using static Glance.Aadhaar.Helper.ExceptionHelper;
using static Glance.Aadhaar.Constants.ErrorMessage;

namespace Glance.Aadhaar.Api;

public class ApiResponse : IXml
{
    public string ResponseCode { get; set; }
    
    public Transaction Transaction { get; set; }
    
    public DateTimeOffset TimeStamp { get; set; }
    
    public string ErrorCode { get; set; }
    
    public IVerifier Verifier { get; set; }
    
    public void FromXml(XElement element) => DeserializeXml(element);

    public XElement ToXml(string elementName) => SerializeXml(elementName);

    protected virtual void DeserializeXml(XElement element)
    {
        ValidateNull(element, nameof(element));
        
        if(Verifier.VerifySignature(element) == false)
            throw new CryptographicException(InvalidSignature);
        
        ResponseCode = element.Element("code")?.Value;
        Transaction = element.Element("txn")?.Value;
        TimeStamp = DateTimeOffset.Parse(element.Element("ts").Value, CultureInfo.InvariantCulture);
        ErrorCode = element.Element("err")?.Value;
    }
    
    protected virtual XElement SerializeXml(string elementName)
    {
        ValidateEmptyString(ResponseCode, nameof(ResponseCode));
        ValidateEmptyString(Transaction, nameof(Transaction));
        
        var apiResponse = new XElement(elementName,
            new XAttribute("code", ResponseCode),
            new XAttribute("txn", Transaction),
            new XAttribute("ts", TimeStamp),
            new XAttribute("err", ErrorCode ?? string.Empty));
        
        return apiResponse;
    }
}