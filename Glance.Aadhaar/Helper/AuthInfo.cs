using Glance.Aadhaar.Enums;
using Glance.Aadhaar.Resident;

namespace Glance.Aadhaar.Helper;

public class AuthInfo
{
    public const string InfoVersion = "04";

    private const string TimestampFormat = "yyyyMMddHHmmss";

    private static readonly Lazy<UsageMatcher[]> Usage = new (() => new[]
    {
        // 2nd, 9th Hexadecimal Digit
        new UsageMatcher {UseLocation = 4, MatchLocation = 32, Name = nameof(Identity.Name)},
        new UsageMatcher {UseLocation = 5, MatchLocation = 33, Name = nameof(Identity.IlName)},
        new UsageMatcher {UseLocation = 6, MatchLocation = 34, Name = nameof(Identity.Gender)},
        new UsageMatcher {UseLocation = 7, MatchLocation = 35, Name = nameof(Identity.DateOfBirth)},

        // 3rd, 10th Hexadecimal Digit
        new UsageMatcher {UseLocation = 8, MatchLocation = 36, Name = nameof(Identity.Phone)},
        new UsageMatcher {UseLocation = 9, MatchLocation = 37, Name = nameof(Identity.Email)},
        new UsageMatcher {UseLocation = 10, MatchLocation = 38, Name = nameof(Identity.Age)},
        new UsageMatcher {UseLocation = 11, MatchLocation = 39, Name = nameof(Address.CareOf)},

        // 4th, 11th Hexadecimal Digit
        new UsageMatcher {UseLocation = 12, MatchLocation = 40, Name = nameof(Address.House)},
        new UsageMatcher {UseLocation = 13, MatchLocation = 41, Name = nameof(Address.Street)},
        new UsageMatcher {UseLocation = 14, MatchLocation = 42, Name = nameof(Address.Landmark)},
        new UsageMatcher {UseLocation = 15, MatchLocation = 43, Name = nameof(Address.Locality)},

        // 5th, 12th Hexadecimal Digit
        new UsageMatcher {UseLocation = 16, MatchLocation = 44, Name = nameof(Address.VillageOrCity)},
        new UsageMatcher {UseLocation = 17, MatchLocation = 45, Name = nameof(Address.District)},
        new UsageMatcher {UseLocation = 18, MatchLocation = 46, Name = nameof(Address.State)},
        new UsageMatcher {UseLocation = 19, MatchLocation = 47, Name = nameof(Address.PinCode)},

        // 6th, 13th Hexadecimal Digit
        new UsageMatcher {UseLocation = 20, MatchLocation = 48, Name = nameof(FullAddress.Address)},
        new UsageMatcher {UseLocation = 21, MatchLocation = 49, Name = nameof(FullAddress.IlAddress)},
        new UsageMatcher {UseLocation = 22, MatchLocation = 50, Name = nameof(BiometricType.Minutiae)},
        new UsageMatcher {UseLocation = 23, MatchLocation = 50, Name = nameof(BiometricType.Fingerprint)},

        // 7th, 13th Hexadecimal Digit
        new UsageMatcher {UseLocation = 24, MatchLocation = 51, Name = nameof(BiometricType.Iris)},

        // 8th, 14th Hexadecimal Digit
        new UsageMatcher {UseLocation = 28, MatchLocation = 52, Name = nameof(Address.PostOffice)},
        new UsageMatcher {UseLocation = 29, MatchLocation = 53, Name = nameof(Address.SubDistrict)},
        new UsageMatcher {UseLocation = 30, MatchLocation = 54, Name = nameof(Identity.DobType)}
    });

    private class UsageMatcher
    {
        public int MatchLocation, UseLocation;
        public string Name;
    }
    
    public string InfoValue { get; set; }
    
    public string AadhaarNumberHash { get; set; }
    
    public string DemographicHash { get; set; }
    
    public string UsageData { get; set; }
    
    public DateTimeOffset Timestamp { get; set; }
    
    public string AuaCodeHash { get; set; }
    
    public string SubAuaCode { get; set; }
    
    public string TerminalHash { get; set; }
}