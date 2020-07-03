using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ACManager.Settings
{
    [XmlRoot("Inventories")]
    public class InventorySettings
    {
        public List<CharacterInventory> CharacterInventories = new List<CharacterInventory>();
    }

    public class CharacterInventory : IEquatable<CharacterInventory>
    {
        [XmlAttribute]
        public string Name;

        public List<Gem> Gems = new List<Gem>();
        public List<AcmComponent> Components = new List<AcmComponent>();

        public bool Equals(CharacterInventory other)
        {
            return Name.Equals(other.Name);
        }
    }

    public class Gem
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public int Quantity;
    }

    [XmlType(TypeName = "Component")]
    public class AcmComponent
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public int Quantity;
    }
}
