using System.Collections.Generic;

namespace ACManager.Settings.BuffDefaults
{
    public class XpChain
    {
        public string Command = "xpchain";
        public List<string> Commands = new List<string>()
            {
                "xp"
            };
        public List<Buff> Buffs = new List<Buff>();
        public List<int> SpellList = new List<int>()
            {
                2086,
                2080,
                2300,
                2232,
                2262
            };
        public XpChain()
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
