using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ACManager.Settings
{
    public enum PortalType
    {
        Primary,
        Secondary
    }

    public class Portal
    {
        public PortalType Type;
        public string Keyword;
        public string Description;
    }
}
