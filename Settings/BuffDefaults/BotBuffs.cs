using Decal.Adapter;
using Decal.Filters;
using System.Collections.Generic;

namespace ACManager.Settings.BuffDefaults
{
    public class BotBuffs
    {
        public List<string> Commands = new List<string>()
        {
            "botbuffs"
        };
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
                4498,
                4510,
                4418
            };

        public BotBuffs()
        {
            foreach (int spellId in SpellList)
            {
                Buffs.Add(new Buff
                {
                    Id = spellId,
                    Name = CoreManager.Current.Filter<FileService>().SpellTable.GetById(spellId).Name
                });
            }
        }
    }
}
