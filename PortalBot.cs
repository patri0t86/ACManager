using System.Collections.Generic;

using Decal.Adapter;
using Decal.Adapter.Wrappers;

namespace ACManager
{
    class PortalBot
    {
        public const string Module = "PortalBot";

        public PortalBot(PluginCore parent)
        {
            try
            {
                List<string> characterList = new List<string>();
                IndexedCollection<CharFilterIndex, int, AccountCharInfo> accountChars = CoreManager.Current.CharacterFilter.Characters;
                for (int i = 0; i < accountChars.Count; i++)
                {
                    characterList.Add(accountChars[i].Name);
                }

                characterList.Sort();

                parent.AddCharacterChoice("Select Character...");
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
