using Decal.Adapter;
using Decal.Filters;
using System.Collections.Generic;

namespace ACManager.Settings.BuffDefaults
{
    public class XpChain
    {
        public List<string> Commands = new List<string>()
            {
                "xp"
            };
        public List<Buff> Buffs = new List<Buff>();
        public List<int> SpellList = new List<int>()
            {
                4547,
                4577
            };
        public XpChain()
        {
            foreach (int spellId in SpellList)
            {
                Buff buff = new Buff
                {
                    Id = spellId,
                    Name = CoreManager.Current.Filter<FileService>().SpellTable.GetById(spellId).Name
                };
                Buffs.Add(buff);
            }
        }
    }
}
