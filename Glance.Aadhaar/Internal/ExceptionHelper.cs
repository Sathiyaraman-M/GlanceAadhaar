using Glance.Aadhaar.Helper;

namespace Glance.Aadhaar.Internal;

internal static class ExceptionHelper
{
    public static T ValidateNull<T>(T argument, string argumentName) where T : class
    {
        if (argument == null)
            throw new ArgumentNullException(argumentName);

        return argument;
    }
    
    public static int ValidateMatchPercent(int percent, string argumentName)
    {
        if (percent is < 1 or > 100)
            throw new ArgumentOutOfRangeException(argumentName, OutOfRangeMatchPercent);

        return percent;
    }
    
    public static string ValidateEmptyString(string argument, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(argument))
            throw new ArgumentException(RequiredNonEmptyString, argumentName);

        return argument;
    }
    
    public static string ValidateAadhaarNumber(string aadhaarNumber, string argumentName)
    {
        if (!string.IsNullOrEmpty(aadhaarNumber) && !AadhaarHelper.ValidateAadhaarNumber(aadhaarNumber))
            throw new ArgumentException(InvalidAadhaarNumber, argumentName);

        return aadhaarNumber;
    }
}