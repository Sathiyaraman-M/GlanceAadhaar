using System.Globalization;

namespace Glance.Aadhaar.Helper;

public class Transaction
{
    private static Func<string> _generator = () => DateTimeOffset.Now.ToString("yyyyMMddhhmmssfff", CultureInfo.InvariantCulture);
    
    public static Func<string> Generator
    {
        get => _generator;
        set
        {
            ValidateNull(value, nameof(Generator));
            _generator = value;
        }
    }
    
    public string Prefix { get; set; } = string.Empty;
    
    public string Value { get; set; } = Generator();
    
    public static implicit operator Transaction(string value)
    {
        var transaction = new Transaction();
        int index;
        if (value != null && (index = value.LastIndexOf(':')) != -1)
        {
            transaction.Prefix = value[..(index + 1)];
            transaction.Value = value[(index + 1)..];
        }
        else
        {
            transaction.Value = value;
        }
        
        return transaction;
    }
    
    public static implicit operator string(Transaction value)
    {
        return value?.Prefix + value?.Value;
    }

    public override string ToString() => this;
}