using GlanceAadhaar.Requests;
using GlanceAadhaar.Responses;

namespace GlanceAadhaar.Clients;

public abstract class BaseClient<TRequest, TResponse> where TRequest : BaseRequest where TResponse : BaseResponse
{
    public Uri Endpoint { get; set; }
    
    protected abstract string XmlElementName { get; }

    public abstract Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
}