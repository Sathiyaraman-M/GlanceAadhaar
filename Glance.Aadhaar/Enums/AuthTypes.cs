namespace Glance.Aadhaar.Enums;

[Flags]
public enum AuthTypes
{
    None = 0,
    Identity = 1,
    Address = 2,
    FullAddress = 4,
    Biometric = 8,
    Pin = 16,
    Otp = 32
}