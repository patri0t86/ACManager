using System.Collections.Generic;
using System.Xml.Serialization;

namespace ACManager.Settings
{
    public class BuffProfile
    {
        [XmlAttribute]
        public string Command;

        public List<Buff> Buffs = new List<Buff>();
    }

    public class Buff
    {
        [XmlAttribute]
        public int SpellId;
    }
}
