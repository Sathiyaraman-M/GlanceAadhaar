using Glance.Aadhaar.Agency;
using Glance.Aadhaar.Api.Auth;
using Glance.Aadhaar.Device;
using Glance.Aadhaar.Enums;
using Glance.Aadhaar.Resident;
using Glance.Aadhaar.Security;

namespace Glance.Aadhaar.Sample;

public static class Auth
{
    private const string BiometricData = "Rk1SACAyMAAAAADkAAgAyQFnAMUAxQEAAAARIQBqAGsgPgCIAG0fRwC2AG2dSQBVAIUjPABuALShMgCxAL0jMAByAM6lPgCmAN2kQQBwAN8qNAB1AN8mPADJAOcgOQA8AOorNABoAOomOQC+AO2fMQDFAPqlSgCvAP8lRQB8AQuhPABwAQ4fMgB7ASqcRADAAS4iNwCkATMeMwCFATYeNwBLATYwMQBWATcoMQCkATecMQBEATwyMgBJAUciQQCkAU8cNQB9AVQWNgCEAVUVRACoAVgYOgBBAV69NgCsAWeYNwAA";
    
    public static AadhaarOptions Options { get; set; }
    
    public static ISigner Signer { get; set; }
    
    public static IVerifier Verifier { get; set; }

    public static async Task AuthenticateAsync()
    {
        var personalInfo = new PersonalInfo
        {
            AadhaarNumber = "999999990019",
            Demographic = new Demographic
            {
                Identity = new Identity
                {
                    Name = "Shivshankar Choudhury",
                    DateOfBirth = new DateTime(1968, 5, 13, 0, 0, 0),
                    Gender = Gender.Male,
                    Phone = "2810806979",
                    Email = "sschoudhury@dummyemail.com"
                },
                Address = new Address
                {
                    Street = "12 Maulana Azad Marg",
                    State = "New Delhi",
                    PinCode = "110002"
                }
            }
        };
        
        personalInfo.Biometrics.Add(new Biometric
        {
            Type = BiometricType.Minutiae,
            Position = BiometricPosition.LeftIndex,
            Data = BiometricData
        });
        
        var deviceContext = new AuthContext
        {
            HasResidentConsent = true, // Should not be hardcoded in production environment.
            DeviceInfo = Options.DeviceInfo.Create()
        };
        
        using var sessionKey = new SessionKey(Options.UidaiEncryptionKeyPath, false);
        await deviceContext.EncryptAsync(personalInfo, sessionKey);

        var apiClient = new AuthClient()
        {
            AgencyInfo = Options.AgencyInfo,
            Request = new AuthRequest(deviceContext) { Signer = Signer },
            Response = new AuthResponse { Verifier = Verifier }
        };
        await apiClient.GetResponseAsync();
        
        Console.WriteLine(string.IsNullOrEmpty(apiClient.Response.ErrorCode)
            ? $"Is the user authentic: {apiClient.Response.IsAuthenticationPassed}"
            : $"Error Code: {apiClient.Response.ErrorCode}");
        
        apiClient.Response.AuthInfo.Decode();
        var mismatch = string.Join(", ", apiClient.Response.AuthInfo.GetMismatch());
        if (!string.IsNullOrEmpty(mismatch))
            Console.WriteLine($"Mismatch Attributes: {mismatch}");
    }
}