using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ACManager.Settings
{ 
    public class Character : IEquatable<Character>
    {
        public string Name;
        public string Account;
        public string Server;
        public string Password;
        public List<Portal> Portals = new List<Portal>();
        public bool AutoFellow;
        public bool AutoRespond;
        public bool AnnounceLogoff;
        public int LeadScarabs;
        public int IronScarabs;
        public int CopperScarabs;
        public int SilverScarabs;
        public int GoldScarabs;
        public int PyrealScarabs;
        public int PlatinumScarabs;
        public int ManaScarabs;
        public int Tapers;

        public bool Equals(Character character)
        {
            return (Name.Equals(character.Name) && Account.Equals(character.Account) && Server.Equals(character.Server));
        }
    }
}
