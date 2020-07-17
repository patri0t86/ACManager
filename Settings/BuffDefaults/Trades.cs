using System.Collections.Generic;

namespace ACManager.Settings.BuffDefaults
{
    public class Trades
    {
        public List<string> Commands = new List<string>()
            {
                "trades",
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
                2194,
                3512,
                2270,
                2190,
                2210,
                2236,
                2196,
                2250,
                2276,
                2324,
                2184,
                2186,
                2182
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
