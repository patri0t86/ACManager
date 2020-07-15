using System.Collections.Generic;

namespace ACManager.Settings.BuffDefaults
{
    public class Trades
    {
        public string Command = "trades";
        public List<string> Commands = new List<string>()
            {
                "alltrades",
                "tinker",
                "tink"
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
                2300,
                2270,
                2288,
                2292,
                2190,
                2210,
                2236,
                2196,
                2250,
                2276,
                2324,
                2184,
                2186,
                2182,
                6006,
                5989
            };
        public Trades()
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
