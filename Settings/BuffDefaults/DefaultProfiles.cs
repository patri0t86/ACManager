﻿using System.Collections.Generic;

namespace ACManager.Settings.DefaultProfiles
{
    internal class DefaultProfiles
    {
        public class Mage
        {
            public string Command = "mage";
            public List<Buff> Buffs = new List<Buff>();
            public List<int> SpellList = new List<int>()
            {
                2086,
                2060,
                2058,
                2080,
                2066,
                2090,
                2194,
                2196,
                2214,
                2226,
                2232,
                2242,
                2244,
                2248,
                2256,
                2262,
                2266,
                2270,
                2280,
                2286,
                2288,
                2292,
                2300,
                2322,
                2052,
                2148,
                2150,
                2152,
                2154,
                2156,
                2158,
                2160,
                2182,
                2184,
                2186,
                5989,
                6006,
                6023
            };

            public Mage()
            {
                foreach (int spellId in SpellList)
                {
                    Buff buff = new Buff
                    {
                        SpellId = spellId
                    };
                    Buffs.Add(buff);
                }

                //Buffs.Add(4391); // Incantation of Acid Bane
                //Buffs.Add(4393); // Incantation of Blade Bane
                //Buffs.Add(4397); // Incantation of Bludgeon Bane
                //Buffs.Add(4401); // Incantation of Flame Bane
                //Buffs.Add(4403); // Incantation of Frost Bane
                //Buffs.Add(4407); // Incantation of Impenetrability
                //Buffs.Add(4409); // Incantation of Lightning Bane
                //Buffs.Add(4412); // Incantation of Piercing Bane

            }
        }
    }
}
