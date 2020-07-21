using System;
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

    public class Equipment : IEquatable<Equipment>
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public int Id;

        [XmlAttribute]
        public int EquipMask;

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Equipment);
        }

        public bool Equals(Equipment other)
        {
            return Id.Equals(other.Id);
        }
    }
}
