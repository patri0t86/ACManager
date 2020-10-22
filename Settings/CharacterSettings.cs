using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ACManager.Settings
{
    [XmlRoot(ElementName = "Settings")]
    public class CharacterSettings
    {
        public List<Character> Characters = new List<Character>();
    }

    public class Character : IEquatable<Character>
    {
        [XmlElement(IsNullable = false)]
        public string Name;

        [XmlElement(IsNullable = false)]
        public string Account;

        [XmlElement(IsNullable = false)]
        public string Server;

        [XmlElement(IsNullable = false)]
        public string Password;

        public List<Portal> Portals = new List<Portal>();

        public bool Equals(Character character)
        {
            return (Name.Equals(character.Name) && Account.Equals(character.Account) && Server.Equals(character.Server));
        }
    }

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
