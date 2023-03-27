using System.Net.Http.Headers;
using System.Xml.Linq;
using Glance.Aadhaar.Api;
using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Agency;

public class ApiClient<TRequest, TResponse> where TRequest: ApiRequest where TResponse: ApiResponse 
{
    public const string ContentType = "application/xml";
    
    public Uri Address { get; set; }
    
    public UserAgency AgencyInfo { get; set; }
    
    public TRequest Request { get; set; }
    
    public TResponse Response { get; set; }
    
    public async Task GetResponseAsync() => await GetResponseAsync(null, null);
    
    public async Task GetResponseAsync(Func<XElement, XElement> requestXmlTransformer, Func<XElement, XElement> responseXmlTransformer)
    {
        ApplyInfo();

        var requestXml = SerializeRequestXml();
        if (requestXmlTransformer != null)
            requestXml = requestXmlTransformer(requestXml);

        var responseXml = await GetResponseXmlAsync(requestXml);
        if (responseXmlTransformer != null)
            responseXml = responseXmlTransformer(responseXml);

        DeserializeResponseXml(responseXml);
    }
    
    protected virtual void ApplyInfo()
    {
        ValidateNull(Request, nameof(Request));
        ValidateNull(Response, nameof(Response));
        ValidateNull(AgencyInfo, nameof(AgencyInfo));
        
        Request.AuaLicenseKey = AgencyInfo.AuaLicenseKey;
        Request.AuaCode = AgencyInfo.AuaCode;
        Request.SubAuaCode = AgencyInfo.SubAuaCode;

        Address = AgencyInfo.GetAddress(Request.ApiName);
    }
    
    protected virtual XElement SerializeRequestXml()
    {
        return Request.ToXml();
    }
    
    protected virtual async Task<XElement> GetResponseXmlAsync(XElement requestXml)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
        using var content = new StringContent(requestXml.ToString(SaveOptions.DisableFormatting));
        using var response = (await client.PostAsync(Address, content)).EnsureSuccessStatusCode();
        return XElement.Load(await response.Content.ReadAsStreamAsync());
    }
    
    protected virtual void DeserializeResponseXml(XElement responseXml)
    {
        ValidateNull(Response, nameof(responseXml));

        Response.ErrorCode = responseXml.Attribute("err")?.Value;

        // Catch all exceptions arising from API error condition due to absence of mandatory XML elements and attributes.
        try { Response.FromXml(responseXml); }
        finally
        {
            if (!string.IsNullOrWhiteSpace(Response.ErrorCode))
                throw new ApiException(Response.ErrorCode);
        }
    }
}