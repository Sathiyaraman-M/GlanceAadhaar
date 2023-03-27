using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Glance.Aadhaar.Agency;
using static System.Text.RegularExpressions.Regex;

namespace Glance.Aadhaar.Helper;

public static class AadhaarHelper
{
    private static readonly int[,] Multiplication =
    {
        {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
        {1, 2, 3, 4, 0, 6, 7, 8, 9, 5},
        {2, 3, 4, 0, 1, 7, 8, 9, 5, 6},
        {3, 4, 0, 1, 2, 8, 9, 5, 6, 7},
        {4, 0, 1, 2, 3, 9, 5, 6, 7, 8},
        {5, 9, 8, 7, 6, 0, 4, 3, 2, 1},
        {6, 5, 9, 8, 7, 1, 0, 4, 3, 2},
        {7, 6, 5, 9, 8, 2, 1, 0, 4, 3},
        {8, 7, 6, 5, 9, 3, 2, 1, 0, 4},
        {9, 8, 7, 6, 5, 4, 3, 2, 1, 0}
    };

    private static readonly int[,] Permutation =
    {
        {0, 1, 2, 3, 4, 5, 6, 7, 8, 9},
        {1, 5, 7, 6, 2, 8, 3, 0, 9, 4},
        {5, 8, 0, 3, 7, 9, 6, 1, 4, 2},
        {8, 9, 1, 6, 0, 4, 3, 5, 2, 7},
        {9, 4, 5, 3, 1, 2, 6, 8, 7, 0},
        {4, 2, 8, 6, 5, 7, 3, 9, 0, 1},
        {2, 7, 9, 3, 8, 0, 6, 4, 1, 5},
        {7, 0, 4, 6, 9, 1, 3, 2, 5, 8}
    };
    
    public static bool ValidateAadhaarNumber(string aadhaarNumber)
    {
        if (string.IsNullOrWhiteSpace(aadhaarNumber) || !IsMatch(aadhaarNumber, @"^\d{12}$"))
            return false;

        var digits = new List<int>(12);
        for (var i = aadhaarNumber.Length - 1; i >= 0; i--)
            digits.Add(aadhaarNumber[i] - '0');

        var checksum = 0;
        for (var i = 0; i < digits.Count; i++)
            checksum = Multiplication[checksum, Permutation[i % 8, digits[i]]];

        return checksum == 0;
    }
    public static void RemoveEmptyAttributes(this XElement element) => ValidateNull(element, nameof(element))
        .Attributes()
        .Where(a => string.IsNullOrWhiteSpace(a.Value))
        .Remove();

    public static bool ValidatePinCode(string pinCode) => !string.IsNullOrWhiteSpace(pinCode) && Regex.IsMatch(pinCode, @"^\d{6}$");
    
    public static byte[] GetBytes(this string value)
    {
        ValidateNull(value, nameof(value));

        return Encoding.UTF8.GetBytes(value);
    }
    
    public static string ToHex(this byte[] value)
    {
        ValidateNull(value, nameof(value));

        return BitConverter.ToString(value).Replace("-", string.Empty);
    }
    
    public static Uri GetAddress(this UserAgency agencyInfo, string apiName, string aadhaarNumber = null)
    {
        ValidateNull(agencyInfo, nameof(agencyInfo));
        ValidateEmptyString(apiName, nameof(apiName));
        ValidateEmptyString(agencyInfo.AuaCode, nameof(UserAgency.AuaCode));

        if (!agencyInfo.Hosts.TryGetValue(apiName, out var host))
            throw new ArgumentException(NoHostName);

        var encodedAsaLicenseKey = WebUtility.UrlEncode(agencyInfo.AsaLicenseKey) ?? string.Empty;

        return string.IsNullOrWhiteSpace(aadhaarNumber) ?
            new Uri(host, $"{agencyInfo.AuaCode}/{encodedAsaLicenseKey}") :
            new Uri(host, $"{agencyInfo.AuaCode}/{aadhaarNumber[0]}/{aadhaarNumber[1]}/{encodedAsaLicenseKey}");
    }
}