using Decal.Adapter;
using Decal.Filters;
using System.Collections.Generic;

namespace ACManager.Settings.BuffDefaults
{
    public class Heavy
    {
        public List<string> Commands = new List<string>()
        {
            "heavy",
            "hw"
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
                5778,
                5802,
                4623,
                4555,
                5826,
                5850,
                5874,
                6115,
                4290,
                4471,
                4461,
                4463,
                4467,
                4465,
                4459,
                4469,
                4495,
                4497,
                4493,
                5998,
                6014,
                6031,
                6006,
                5989,
                4407,
                4412,
                4393,
                4397,
                4401,
                4403,
                4409,
                4391
            };
        public Heavy()
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
