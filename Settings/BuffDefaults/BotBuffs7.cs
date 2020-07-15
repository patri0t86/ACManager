using System.Collections.Generic;

namespace ACManager.Settings.BuffDefaults
{
    public class BotBuffs7
    {
        public string Command = "botbuffs7";
        public List<Buff> Buffs = new List<Buff>();
        public List<int> SpellList = new List<int>()
            {
                2215,
                2067,
                2091,
                2061,
                2249,
                2267,
                2287,
                2183,
                2187, 
                2195
            };

        public BotBuffs7()
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
