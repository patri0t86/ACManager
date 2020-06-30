using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Filters;
using System.Collections.Generic;

namespace ACManager
{
    internal class ComponentChecker
    {
        private readonly CoreManager Core;

        public ComponentChecker(CoreManager core)
        {
            Core = core;
        }

        internal bool HaveComponents(int spellId)
        {
            bool haveOriginalComponents = true;
            bool haveScarabs = true;
            string scarabType = "";
            SpellComponentIDs comps = RequiredComponents(spellId);
            Dictionary<string, int> compCounts = ComponentsPerSpell(comps);

            foreach (KeyValuePair<string,int> compRequired in compCounts)
            {
                if (compRequired.Key.Contains("Scarab"))
                {
                    scarabType = compRequired.Key;
                }

                if (!(QuantityInInventory(compRequired.Key) >= compRequired.Value))
                {
                    if (compRequired.Key.Contains("Scarab"))
                    {
                        haveScarabs = false;
                    }
                    haveOriginalComponents = false;
                    break;
                }
            }

            if (haveOriginalComponents)
            {
                return true;
            }
            else if (haveScarabs && HaveTapers(scarabType) && (HaveFoci(spellId) || HaveAugmentation())) // need to check for augmentation as well!!
            {
                return true;
            }
            return false;
        }

        private bool HaveTapers(string scarab)
        {
            string taper = "Prismatic Taper";
            switch (scarab)
            {
                case "Lead Scarab":
                    return QuantityInInventory(taper) >= 1;
                case "Iron Scarab":
                    return QuantityInInventory(taper) >= 2;
                case "Copper Scarab":
                    return QuantityInInventory(taper) >= 3;
                case "Silver Scarab":
                    return QuantityInInventory(taper) >= 4; // this is erroneously set to 4, for now, since the ACEmulator is wrong, should be 3
                case "Gold Scarab":
                    return QuantityInInventory(taper) >= 4;
                case "Pyreal Scarab":
                    return QuantityInInventory(taper) >= 4;
                case "Platinum Scarab":
                    return QuantityInInventory(taper) >= 4;
                case "Mana Scarab":
                    return QuantityInInventory(taper) >= 4;
            }
            return false;
        }

        private int QuantityInInventory(string compName)
        {
            using (WorldObjectCollection inventory = Core.WorldFilter.GetInventory())
            {
                inventory.SetFilter(new ByNameFilter(compName));
                return inventory.Quantity;
            }
        }

        private Dictionary<string, int> ComponentsPerSpell(SpellComponentIDs comps)
        {
            Dictionary<string, int> compDict = new Dictionary<string, int>();
            for (int i = 0; i < comps.Length; i++)
            {
                if (!compDict.ContainsKey(GetComponent(comps[i]).Name))
                {
                    compDict.Add(GetComponent(comps[i]).Name, 1);
                }
                else
                {
                    compDict[GetComponent(comps[i]).Name] += 1;
                }
            }
            return compDict;
        }

        internal bool HaveFoci(int spellId)
        {
            string school = SpellSchool(spellId).Name;
            using (WorldObjectCollection inventory = Core.WorldFilter.GetInventory())
            {
                inventory.SetFilter(new ByObjectClassFilter(ObjectClass.Foci));
                foreach (WorldObject item in inventory)
                {
                    if (item.Name.Equals("Foci of Enchantment") && school.Equals("Creature Enchantment"))
                    {
                        return true;
                    }
                    else if (item.Name.Equals("Foci of Artifice") && school.Equals("Item Enchantment"))
                    {
                        return true;
                    }
                    else if (item.Name.Equals("Foci of Verdancy") && school.Equals("Life Magic"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool HaveAugmentation()
        {
            // need to find out how to get the augmentation status of the character
            return false;
        }

        private SpellSchool SpellSchool(int spellId)
        {
            return Core.Filter<FileService>().SpellTable.GetById(spellId).School;
        }

        private SpellComponentIDs RequiredComponents(int spellId)
        { 
            return Core.Filter<FileService>().SpellTable.GetById(spellId).ComponentIDs;
        }

        private Component GetComponent(int componentId)
        {
            return Core.Filter<FileService>().ComponentTable.GetById(componentId);
        }

    }
}
