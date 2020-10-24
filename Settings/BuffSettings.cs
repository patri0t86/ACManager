using System.Collections.Generic;
using System.Xml.Serialization;

namespace ACManager.Settings
{
    public class BuffProfile
    {
        [XmlArray]
        [XmlArrayItem(ElementName = "Command")]
        public List<string> Commands = new List<string>();

        public List<Buff> Buffs = new List<Buff>();
    }

    public class Buff
    {
        [XmlAttribute]
        public int SpellId;
    }
}
