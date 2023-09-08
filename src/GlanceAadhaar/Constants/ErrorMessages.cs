namespace GlanceAadhaar.Constants;

internal static class ErrorMessages
{
    // Invalid
    public const string InvalidAadhaarNumber = "Aadhaar number is invalid.";
    public const string InvalidBiometricPosition = "Biometric position is invalid.";
    public const string InvalidHeader = "Header data is invalid.";
    public const string InvalidHmac = "Data hash is wrong.";
    public const string InvalidPinCode = "Pin Code must be 6 digits.";
    public const string InvalidSignature = "Digital signature verification failed.";
    public const string InvalidTransactionPrefix = "Transaction prefix is invalid.";

    // Out of range
    public const string OutOfRangeAge = "Age must be within 0 - 150.";
    public const string OutOfRangeAuaCode = "AUA or Sub-AUA code must be within 10 characters.";
    public const string OutOfRangeAuaLicenseKey = "AUA license key must be within 64 characters.";
    public const string OutOfRangeDeviceCode = "Device code length exceeded.";
    public const string OutOfRangeLatitude = "Latitude must be within -90 - +90.";
    public const string OutOfRangeLongitude = "Longitude must be within -180 - +180.";
    public const string OutOfRangeAltitude = "Altitude must be within -999 to +999";
    public const string OutOfRangeMatchPercent = "Match percentage must be within 1 - 100.";
    public const string OutOfRangeName = "Name must be within 60 characters.";
    public const string OutOfRangeNumberOfAttempts = "Minimum attempts must be greater than 1.";
    public const string OutOfRangeTransaction = "Transaction identifier must be within 50 characters.";

    // Required
    public const string RequiredBiometricOrOtp = "Biometric or OTP authentication required.";
    public const string RequiredBiometricsUsed = "Biometrics used need to be specified.";
    public const string RequiredConsent = "Resident consent required.";
    public const string RequiredIndianLanguage = "Indian language type used required.";
    public const string RequiredNonEmptyString = "String cannot be null, empty or whitespaces.";
    public const string RequiredSomeData = "At least one resident data needs to be specified.";

    // Not Found
    public const string NoHostName = "Host name not found.";
    public const string NoPrivateKey = "No private key found.";
    public const string NoPublicKey = "No public key found.";
    public const string NoSignature = "No signature found.";

    // XOR
    public const string XorAddresses = "Address & FullAddress cannot be used in same transaction.";
    public const string XorFirFmr = "Fingerprint and Minutiae cannot be used in same transaction.";

    // Miscellaneous
    public const string NotSupportedXmlSignature = "XML signing not supported in platform.";
    public const string ExpiredSynchronizedKey = "Synchronized key expired.";
}