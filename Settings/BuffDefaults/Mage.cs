using System.Collections.Generic;

namespace ACManager.Settings.BuffDefaults
{
    public class Mage
    {
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
                2214,
                2266,
                2248,
                2286,
                2194,
                2232,
                2262,
                3512,
                2242,
                2244,
                2280,
                2256,
                2300,
                2270,
                2288,
                2292,
                2226,
                6114,
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
                    Id = spellId
                };
                Buffs.Add(buff);
            }
        }
    }
}
