using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Xml;
using Decal.Adapter;
using Decal.Adapter.Wrappers;

namespace ACManager
{
    class PortalBot
    {

        public const string Module = "PortalBot";
        private PluginHost Host;
        private PluginCore Parent;
        private CoreManager Core;
        public bool PortalBotStatus;
        public IndexedCollection<CharFilterIndex, int, AccountCharInfo> accountChars;

        public PortalBot(PluginCore parent, PluginHost host, CoreManager core)
        {
            Parent = parent;
            Host = host;
            Core = core;
            try
            {
                List<string> characterList = new List<string>();
                accountChars = Core.CharacterFilter.Characters;
                for (int i = 0; i < accountChars.Count; i++)
                {
                    characterList.Add(accountChars[i].Name);
                    //parent.AddCharacterChoice(accountChars[i].Name);
                }

                characterList.Sort();

                for (int i = 0; i < characterList.Count; i++)
                {
                    parent.AddCharacterChoice(characterList[i]);
                }

            }
            catch { }
        }

        public void SetPrimaryKeyword(string characterName, string keyword)
        {
            Utility.SaveSetting(Module, characterName, "PrimaryKeyword", keyword.ToLower());
        }

        public void SetPrimaryDescription(string characterName, string description)
        {
            Utility.SaveSetting(Module, characterName, "PrimaryDescription", description);
        }

        public void SetSecondaryKeyword(string characterName, string keyword)
        {
            Utility.SaveSetting(Module, characterName, "SecondaryKeyword", keyword.ToLower());
        }

        public void SetSecondaryDescription(string characterName, string description)
        {
            Utility.SaveSetting(Module, characterName, "SecondaryDescription", description);
        }
    }
}
