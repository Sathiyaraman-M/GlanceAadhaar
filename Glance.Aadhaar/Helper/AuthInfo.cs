using System.Collections;
using System.Globalization;
using Glance.Aadhaar.Enums;
using Glance.Aadhaar.Resident;

namespace Glance.Aadhaar.Helper;

public class AuthInfo
{
    private const string InfoVersion = "04";

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
    
    public string UserIdHash { get; set; }
    
    public UserIdType? UserIdType { get; set; }
    
    public string DemographicHash { get; set; }
    
    public string UsageData { get; set; }
    
    public DateTimeOffset Timestamp { get; set; }
    
    public string AuaCodeHash { get; set; }
    
    public string SubAuaCode { get; set; }
    
    public string TerminalHash { get; set; }

    public string Encode()
    {
        var infoArray = new string[31];
        for (var i = 0; i < infoArray.Length; i++)
            infoArray[i] = string.Empty;
        infoArray[0] = UserIdHash ?? string.Empty;
        infoArray[1] = UserIdType.ToString();
        infoArray[2] = DemographicHash ?? string.Empty;
        infoArray[3] = UsageData ?? string.Empty;
        infoArray[5] = Timestamp.ToString(TimestampFormat, CultureInfo.InvariantCulture);
        infoArray[12] = AuaCodeHash ?? string.Empty;
        infoArray[13] = SubAuaCode ?? string.Empty;
        infoArray[24] = TerminalHash ?? string.Empty;
        
        InfoValue = $"{InfoVersion}{{{string.Join(",", infoArray)}}}";
        return InfoValue;
    }

    public AuthInfo Decode()
    {
        ValidateEmptyString(InfoValue, nameof(InfoValue));
        
        var start = InfoValue.IndexOf('{');
        var end = InfoValue.LastIndexOf('}');
        if (start != -1 && end != -1)
        {
            var infoArray = InfoValue.Substring(start + 1, end - (start + 1)).Split(',');

            UserIdHash = infoArray[0];
            UserIdType = (UserIdType) Enum.Parse(typeof(UserIdType), infoArray[1]);
            DemographicHash = infoArray[2];
            UsageData = infoArray[3];
            Timestamp = DateTimeOffset.ParseExact(infoArray[5], TimestampFormat, CultureInfo.InvariantCulture);
            AuaCodeHash = infoArray[12];
            SubAuaCode = infoArray[13];
            TerminalHash = infoArray[24];
        }

        return this;
    }
    
    public string[] Validate(AuthInfo authInfo)
    {
        ValidateNull(authInfo, nameof(authInfo));

        var errorProperties = new List<string>(6);
        if (!UserIdHash.Equals(authInfo.UserIdHash, StringComparison.OrdinalIgnoreCase))
            errorProperties.Add(nameof(UserIdHash));
        if(!UserIdType.Equals(authInfo.UserIdType))
            errorProperties.Add(nameof(UserIdType));
        if (!DemographicHash.Equals(authInfo.DemographicHash, StringComparison.OrdinalIgnoreCase))
            errorProperties.Add(nameof(DemographicHash));
        if (Timestamp != authInfo.Timestamp)
            errorProperties.Add(nameof(Timestamp));
        if (!AuaCodeHash.Equals(authInfo.AuaCodeHash, StringComparison.OrdinalIgnoreCase))
            errorProperties.Add(nameof(AuaCodeHash));
        if (!SubAuaCode.Equals(authInfo.SubAuaCode))
            errorProperties.Add(nameof(SubAuaCode));
        if (!TerminalHash.Equals(authInfo.TerminalHash, StringComparison.OrdinalIgnoreCase))
            errorProperties.Add(nameof(TerminalHash));

        return errorProperties.ToArray();
    }
    
    public BitArray GetUsageData()
    {
        ValidateEmptyString(UsageData, nameof(UsageData));

        var values = Convert.ToString(Convert.ToInt64(UsageData, 16), 2)
            .PadLeft(60, '0')
            .Select(b => b == '1')
            .ToArray();

        return new BitArray(values);
    }
    
    public IEnumerable<string> GetMismatch()
    {
        var bitArray = GetUsageData();

        return Usage.Value.Where(u => bitArray[u.UseLocation] && !bitArray[u.MatchLocation])
            .Select(u => u.Name)
            .OrderBy(u => u);
    }
}