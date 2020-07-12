using System.Collections.Generic;

namespace ACManager.Settings.BuffDefaults
{
    public class BotBuffs
    {
        public string Command = "botbuffs";
        public List<Buff> Buffs = new List<Buff>();
        public List<int> SpellList = new List<int>()
            {
                4530,
                4305,
                4329,
                4299,
                4564,
                4582,
                4602,
                4494,
                4498
            };

        public BotBuffs()
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
