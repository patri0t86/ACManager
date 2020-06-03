using System.Xml.Serialization;

namespace ACManager.Settings
{
    public enum PortalType
    {
        Primary,
        Secondary
    }

    public class Portal
    {
        [XmlElement(IsNullable = false)]
        public PortalType Type;
        [XmlElement(IsNullable = false)]
        public string Keyword;
        [XmlElement(IsNullable = false)]
        public string Description;
        [XmlElement(IsNullable = false)]
        public double Heading;
        [XmlElement(IsNullable = false)]
        public int Level;
    }
}
