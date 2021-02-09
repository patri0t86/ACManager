using Decal.Adapter;
using Decal.Filters;
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
                4391,
                4393,
                4397,
                4401,
                4403,
                4407,
                4409,
                4412,
            };
        public Banes()
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
