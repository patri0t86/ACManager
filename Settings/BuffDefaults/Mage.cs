using System.Collections.Generic;

namespace ACManager.Settings.BuffDefaults
{
    public class Mage
    {
        public string Command = "mage";
        public List<string> Commands = new List<string>()
            {
                "mage",
                "war"
            };
        public List<Buff> Buffs = new List<Buff>();
        public List<int> SpellList = new List<int>()
            {
                2086,
                2060,
                2058,
                2080,
                2066,
                2090,
                2194,
                2196,
                2214,
                2226,
                2232,
                2242,
                2244,
                2248,
                2256,
                2262,
                2266,
                2270,
                2280,
                2286,
                2288,
                2292,
                2300,
                2322,
                2052,
                2148,
                2150,
                2152,
                2154,
                2156,
                2158,
                2160,
                2182,
                2184,
                2186,
                5989,
                6006,
                6023
            };

        public Mage()
        {
            foreach (int spellId in SpellList)
            {
                Buff buff = new Buff
                {
                    SpellId = spellId
                };
                Buffs.Add(buff);
            }
        }
    }
}
