namespace GlanceAadhaar.Elements.PidBlock;

public class PidData : IXmlElement
{
    public ICollection<Biometric> Biometrics { get; set; } = new List<Biometric>();
    
    public XElement ConvertToXml(string elementName)
    {
        throw new NotImplementedException();
    }

    public void LoadFromXml(XElement element)
    {
        throw new NotSupportedException();
    }
}