using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ACManager.Settings
{
    [XmlRoot(ElementName = "Settings")]
    public class AllSettings
    {
        public List<Character> Characters = new List<Character>();
        public List<Advertisement> Advertisements = new List<Advertisement>();
    }
}
