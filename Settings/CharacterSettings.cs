using System.Collections.Generic;
using System.Xml.Serialization;

namespace ACManager.Settings
{
    [XmlRoot(ElementName = "Settings")]
    public class CharacterSettings
    {
        public List<Character> Characters = new List<Character>();
        public List<Advertisement> Advertisements = new List<Advertisement>();
    }
}
