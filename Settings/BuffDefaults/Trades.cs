using Decal.Adapter;
using Decal.Filters;
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
                4324,
                4298,
                4296,
                4318,
                4304,
                4328,
                4509,
                3512,
                4585,
                4505,
                4525,
                4551,
                4511,
                4565,
                4591,
                4639,
                4495,
                4497,
                4493
            };
        public Trades()
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
