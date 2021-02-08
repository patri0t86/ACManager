﻿using System.Collections.Generic;

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
                2086,
                2060,
                2058,
                2080,
                2066,
                2090,
                2214,
                2266,
                2248,
                2286,
                2194,
                2232,
                2262,
                3512,
                2242,
                2244,
                2280,
                2256,
                2300,
                2270,
                2288,
                2292,
                2226,
                5777,
                2240,
                5825,
                5873,
                5097,
                2052,
                2160,
                2150,
                2152,
                2156,
                2154,
                2148,
                2158,
                2184,
                2186,
                2182,
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
                Buff buff = new Buff
                {
                    Id = spellId
                };
                Buffs.Add(buff);
            }
        }
    }
}
