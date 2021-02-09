using Decal.Adapter;
using Decal.Filters;
using System.Collections.Generic;

namespace ACManager.Settings.BuffDefaults
{
    public class Mage
    {
        public List<string> Commands = new List<string>()
        {
            "mage",
            "war"
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
                4529,
                4581,
                4563,
                4601,
                4509,
                4547,
                4577,
                3512,
                4557,
                4559,
                4595,
                4571,
                4615,
                4585,
                4603,
                4607,
                4541,
                6115,
                4637,
                4290,
                4459,
                4461,
                4463,
                4465,
                4467,
                4469,
                4471,
                4493,
                4495,
                4497,
                5989,
                6006,
                6023
            };

        public Mage()
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
