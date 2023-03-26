using static Glance.Aadhaar.Constants.ErrorMessage;

namespace Glance.Aadhaar.Helper;

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
}