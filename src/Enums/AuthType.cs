namespace GlanceAadhaar.Enums;

[Flags]
public enum AuthType
{
    None = 0,
    PersonalIdentity = 1,
    PersonalAddress = 2,
    PersonalFullAddress = 4,
    Biometric = 8,
    Pin = 16,
    Otp = 32,
}