using System.Collections.Generic;
using System.Xml.Serialization;

namespace ACManager.Settings
{
    [XmlRoot(ElementName = "Suits")]
    public class EquipmentSettings
    {
        [XmlArray(ElementName = "IdleSuit")]
        [XmlArrayItem(ElementName = "Equipment")]
        public List<Equipment> IdleEquipment = new List<Equipment>();

        [XmlArray(ElementName = "BuffingSuit")]
        [XmlArrayItem(ElementName = "Equipment")]
        public List<Equipment> BuffingEquipment = new List<Equipment>();
    }

    public class Equipment
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public int Id;
    }
}
