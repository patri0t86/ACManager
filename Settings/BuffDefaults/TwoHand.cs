using Decal.Adapter;
using Decal.Filters;
using System.Collections.Generic;

namespace ACManager.Settings.BuffDefaults
{
    public class TwoHand
    {
        public List<string> Commands = new List<string>()
            {
                "2h",
                "twohand",
                "2hand"
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
                4555,
                5826,
                5874,
                5097,
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
                5989
            };
        public TwoHand()
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
