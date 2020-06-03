using System.Xml.Serialization;

namespace ACManager.Settings
{
    public class Advertisement
    {
        [XmlElement(IsNullable = false)]
        public string Message;
    }
}
