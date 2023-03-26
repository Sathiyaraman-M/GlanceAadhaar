using System.Globalization;
using System.Xml.Linq;
using Glance.Aadhaar.Constants;
using Glance.Aadhaar.Enums;

namespace Glance.Aadhaar.Helper;

public class Identity : IUsed, IXml
{
    private const string BirthDateFormat = "ddMMyyyy";

    private const string BirthYearFormat = "yyyy";

    private int _age;
    private int _ilNameMatchPercent = HelperConstants.MaxMatchPercent;
    private int _nameMatchPercent = HelperConstants.MaxMatchPercent;
    
    public Identity()
    {
        
    }
    
    public Identity(XElement element) => FromXml(element);
    
    public string Name { get; set; }
    
    public string IlName { get; set; }
    
    public Gender? Gender { get; set; }
    
    public DateTimeOffset? DateOfBirth { get; set; }
    
    public int Age
    {
        get => _age;
        set
        {
            if (value is < 0 or > 150)
                throw new ArgumentOutOfRangeException(nameof(Age), OutOfRangeAge);
            _age = value;
        }
    }
    
    public string Email { get; set; }
    
    public string Phone { get; set; }
    
    public MatchingStrategy Match { get; set; } = MatchingStrategy.Exact;
    
    public int NameMatchPercent
    {
        get => _nameMatchPercent;
        set => _nameMatchPercent = ValidateMatchPercent(value, nameof(NameMatchPercent));
    }
    
    public int IlNameMatchPercent
    {
        get => _ilNameMatchPercent;
        set => _ilNameMatchPercent = ValidateMatchPercent(value, nameof(IlNameMatchPercent));
    }
    
    public DateOfBirthType? DobType { get; set; }
    
    public bool VerifyOnlyBirthYear { get; set; }

    public bool IsUsed => !(string.IsNullOrWhiteSpace(Name) &&
                            string.IsNullOrWhiteSpace(IlName) &&
                            string.IsNullOrWhiteSpace(Phone) &&
                            string.IsNullOrWhiteSpace(Email) &&
                            Gender == null &&
                            DateOfBirth == null &&
                            Age == 0);
    public void FromXml(XElement element)
    {
        ValidateNull(element, nameof(element));
        
        Name = element.Attribute("name")?.Value;
        IlName = element.Attribute("lname")?.Value;
        Phone = element.Attribute("phone")?.Value;
        Email = element.Attribute("email")?.Value;
        Gender = (Gender?)element.Attribute("gender")?.Value[0];
        
        var value = element.Attribute("dob")?.Value;
        if (value != null)
        {
            if (DateTimeOffset.TryParseExact(value, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob))
                DateOfBirth = dob;
            else
            {
                VerifyOnlyBirthYear = value.Length == BirthYearFormat.Length;
                DateOfBirth = DateTimeOffset.ParseExact(value, VerifyOnlyBirthYear ? BirthYearFormat : BirthDateFormat, CultureInfo.InvariantCulture);
            }
        }
        else
        {
            VerifyOnlyBirthYear = false;
            DateOfBirth = null;
        }
        
        var age = element.Attribute("age")?.Value;
        Age = age != null ? int.Parse(age) : 0;
        
        var match = element.Attribute("match")?.Value;
        Match = match != null ? (MatchingStrategy)match[0] : MatchingStrategy.Exact;
        if (Match == MatchingStrategy.Partial)
        {
            NameMatchPercent = int.Parse(element.Attribute("mv")?.Value);
            IlNameMatchPercent = int.Parse(element.Attribute("lmv")?.Value);
        }
        else
        {
            NameMatchPercent = IlNameMatchPercent = HelperConstants.MaxMatchPercent;
        }
        DobType = (DateOfBirthType?)element.Attribute("dobt")?.Value[0];
    }

    public XElement ToXml(string elementName)
    {
        var identity = new XElement(elementName,
            new XAttribute("name", Name ?? string.Empty),
            new XAttribute("lname", IlName ?? string.Empty),
            new XAttribute("phone", Phone ?? string.Empty),
            new XAttribute("email", Email ?? string.Empty));
        if(Gender != null)
            identity.Add(new XAttribute("gender", (char)Gender));
        if (DateOfBirth != null)
            identity.Add(new XAttribute("dob", DateOfBirth.Value.ToString(VerifyOnlyBirthYear ? BirthYearFormat : BirthDateFormat, CultureInfo.InvariantCulture)));
        if (Age != 0)
            identity.Add(new XAttribute("age", Age));

        if (Match == MatchingStrategy.Partial && !(string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(IlName)))
        {
            identity.Add(new XAttribute("ms", (char)Match),
                new XAttribute("mv", NameMatchPercent),
                new XAttribute("lmv", IlNameMatchPercent));
        }
        if (DobType != null && (DateOfBirth != null || Age != 0))
            identity.Add(new XAttribute("dobt", (char)DobType));

        identity.RemoveEmptyAttributes();

        return identity;
    }
}