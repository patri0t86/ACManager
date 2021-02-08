﻿using System.Collections.Generic;

namespace ACManager.Settings.BuffDefaults
{
    public class BotBuffs7
    {
        public List<string> Commands = new List<string>()
        {
            "botbuffs7"
        };
        public List<Buff> Buffs = new List<Buff>();
        public List<int> SpellList = new List<int>()
            {
                2215,
                2067,
                2091,
                2061,
                2249,
                2267,
                2287,
                2183,
                2187,
                2195,
                2117
            };

        public BotBuffs7()
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
