using System.Security.Cryptography;
using Glance.Aadhaar.Api.Auth;
using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Agency;

public class AuthClient : ApiClient<AuthRequest, AuthResponse>
{
    protected override void ApplyInfo()
    {
        base.ApplyInfo();
        if (Request.AuthInfo != null)
        {
            Request.AuthInfo.UserIdHash = SHA256.HashData(Request.AadhaarNumber.GetBytes()).ToHex();
            Request.AuthInfo.AuaCodeHash = SHA256.HashData(Request.AuaCode.GetBytes()).ToHex();
            Request.AuthInfo.SubAuaCode = Request.SubAuaCode;
            Request.AuthInfo.TerminalHash = SHA256.HashData(Request.Terminal.GetBytes()).ToHex();
            Request.AuthInfo.Encode();
        }
        Address = AgencyInfo.GetAddress(Request.ApiName, Request.AadhaarNumber);
    }
}