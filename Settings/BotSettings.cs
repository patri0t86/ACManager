using System;
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
        public bool RespondToAllegiance = false;

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

        [XmlElement(IsNullable = false)]
        public int ComponentThreshold;

        [XmlElement(IsNullable = false)]
        public int LeadScarabThreshold;

        [XmlElement(IsNullable = false)]
        public int IronScarabThreshold;

        [XmlElement(IsNullable = false)]
        public int CopperScarabThreshold;

        [XmlElement(IsNullable = false)]
        public int SilverScarabThreshold;

        [XmlElement(IsNullable = false)]
        public int GoldScarabThreshold;

        [XmlElement(IsNullable = false)]
        public int PyrealScarabThreshold;

        [XmlElement(IsNullable = false)]
        public int PlatinumScarabThreshold;

        [XmlElement(IsNullable = false)]
        public int ManaScarabThreshold;

        [XmlElement(IsNullable = false)]
        public string BuffingCharacter;

        [XmlElement(IsNullable = false)]
        public bool StayBuffed;

        [XmlElement(IsNullable = false)]
        public bool Level7Self;

        public List<GemSetting> GemSettings = new List<GemSetting>();

        public List<Advertisement> Advertisements = new List<Advertisement>();
    }

    public class Advertisement
    {
        [XmlElement(IsNullable = false)]
        public string Message;
    }

    [XmlType(TypeName = "Gem")]
    public class GemSetting : IEquatable<GemSetting>
    {
        public string Name;
        public string Keyword;
        public double Heading;

        public bool Equals(GemSetting other)
        {
            return Name.Equals(other.Name);
        }
    }
}
