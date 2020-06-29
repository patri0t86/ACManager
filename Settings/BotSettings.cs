using System.Collections.Generic;
using System.Xml.Serialization;

namespace ACManager.Settings
{
    [XmlRoot(ElementName = "Settings")]
    public class BotSettings
    {
        [XmlElement(IsNullable = false)]
        public bool BotEnabled = false;

        [XmlElement(IsNullable = false)]
        public double AdInterval = 10;

        [XmlElement(IsNullable = false)]
        public bool RespondToGeneralChat = true;

        [XmlElement(IsNullable = false)]
        public int Verbosity = 0;

        [XmlElement(IsNullable = false)]
        public double ManaThreshold = 0.5;

        [XmlElement(IsNullable = false)]
        public double StaminaThreshold = 0.5;

        [XmlElement(IsNullable = false)]
        public bool AdsEnabled = true;

        [XmlElement(IsNullable = false)]
        public double DefaultHeading;

        [XmlElement(IsNullable = false)]
        public int DesiredLandBlock;

        [XmlElement(IsNullable = false)]
        public double DesiredBotLocationX;

        [XmlElement(IsNullable = false)]
        public double DesiredBotLocationY;

        [XmlElement(IsNullable = false)]
        public bool BotPositioning = false;

        public List<Advertisement> Advertisements = new List<Advertisement>();
    }

    public class Advertisement
    {
        [XmlElement(IsNullable = false)]
        public string Message;
    }
}
