using System.Xml.Serialization;

namespace ACManager.Settings
{
    [XmlRoot(ElementName = "Settings")]
    public class GUISettings
    {
        [XmlElement(IsNullable = false)]
        public bool ExpTrackerVisible = true;
        [XmlElement(IsNullable = false)]
        public bool BotConfigVisible = true;
    }
}
