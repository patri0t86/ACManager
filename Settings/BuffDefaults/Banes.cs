using System.Collections.Generic;

namespace ACManager.Settings.BuffDefaults
{
    public class Banes
    {
        public List<string> Commands = new List<string>()
            {
                "banes",
                "bane"
            };
        public List<Buff> Buffs = new List<Buff>();
        public List<int> SpellList = new List<int>()
            {
                4407,
                4412,
                4393,
                4397,
                4401,
                4403,
                4409,
                4391
            };
        public Banes()
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
