using ACManager.Settings;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Filters;
using System.Collections.Generic;
using System.Text;

namespace ACManager
{
    public static class Inventory
    {
        public static Dictionary<string, int> GetComponentLevels()
        {
            Dictionary<string, int> components = new Dictionary<string, int>();
            for (int i = 0; i < CoreManager.Current.Filter<FileService>().ComponentTable.Length; i++)
            {
                string component = CoreManager.Current.Filter<FileService>().ComponentTable[i].Name;
                int qty = GetInventoryCount(component);
                if (qty > 0)
                {
                    if (components.ContainsKey(component))
                    {
                        components[component] = qty;
                    }
                    else if (!components.ContainsKey(component))
                    {
                        components.Add(component, qty);
                    }
                }
            }
            return components;
        }

        public static string ReportOnLowComponents()
        {
            List<string> lowCompsList = new List<string>();
            foreach (KeyValuePair<string, int> component in GetComponentLevels())
            {
                switch (component.Key)
                {
                    case "Lead Scarab":
                        if (component.Value <= Utility.BotSettings.LeadScarabThreshold) { lowCompsList.Add(component.Key); }
                        break;
                    case "Iron Scarab":
                        if (component.Value <= Utility.BotSettings.IronScarabThreshold) { lowCompsList.Add(component.Key); }
                        break;
                    case "Copper Scarab":
                        if (component.Value <= Utility.BotSettings.CopperScarabThreshold) { lowCompsList.Add(component.Key); }
                        break;
                    case "Silver Scarab":
                        if (component.Value <= Utility.BotSettings.SilverScarabThreshold) { lowCompsList.Add(component.Key); }
                        break;
                    case "Gold Scarab":
                        if (component.Value <= Utility.BotSettings.GoldScarabThreshold) { lowCompsList.Add(component.Key); }
                        break;
                    case "Pyreal Scarab":
                        if (component.Value <= Utility.BotSettings.PyrealScarabThreshold) { lowCompsList.Add(component.Key); }
                        break;
                    case "Platinum Scarab":
                        if (component.Value <= Utility.BotSettings.PlatinumScarabThreshold) { lowCompsList.Add(component.Key); }
                        break;
                    case "Mana Scarab":
                        if (component.Value <= Utility.BotSettings.ManaScarabThreshold) { lowCompsList.Add(component.Key); }
                        break;
                    case "Diamond Scarab":
                        break;
                    case "Prismatic Taper":
                        if (component.Value <= Utility.BotSettings.ComponentThreshold) { lowCompsList.Add(component.Key); }
                        break;
                }
            }

            if (lowCompsList.Count.Equals(0))
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("I'm low on ");
            for (int i = 0; i < lowCompsList.Count; i++)
            {
                if (!i.Equals(lowCompsList.Count - 1))
                {
                    sb.Append($"{lowCompsList[i]}s");
                    sb.Append(", ");
                }
                else
                {
                    if (lowCompsList.Count.Equals(1))
                    {
                        sb.Append($"{lowCompsList[i]}s.");
                    }
                    else
                    {
                        sb.Append($"and {lowCompsList[i]}s.");
                    }
                }
            }
            return sb.ToString();
        }

        public static void UpdateInventoryFile()
        {
            CharacterInventory inventory = new CharacterInventory
            {
                Name = CoreManager.Current.CharacterFilter.Name
            };

            foreach (KeyValuePair<string, int> comp in GetComponentLevels())
            {
                inventory.Components.Add(new AcmComponent { Name = comp.Key, Quantity = comp.Value });
            }

            foreach (GemSetting gem in Utility.BotSettings.GemSettings)
            {
                int quantity = GetInventoryCount(gem.Name);
                if (quantity > 0)
                {
                    inventory.Gems.Add(new Gem { Name = gem.Name, Quantity = quantity });
                }
            }

            // save inventory to file
            Utility.SaveInventory(inventory);
        }

        /// <summary>
        /// Checks for and returns the quantity of items in your inventory.
        /// </summary>
        /// <param name="item">Name of the item to check for.</param>
        /// <returns>The quantity of the requested item in inventory.</returns>
        public static int GetInventoryCount(string item)
        {
            using (WorldObjectCollection inventory = CoreManager.Current.WorldFilter.GetInventory())
            {
                inventory.SetFilter(new ByNameFilter(item));
                return inventory.Quantity;
            }
        }

        public static bool HaveComponents(Spell spell)
        {
            string scarabType = "";

            Dictionary<string, int> requiredComps = new Dictionary<string, int>();
            for (int i = 0; i < spell.ComponentIDs.Length; i++)
            {
                string component = CoreManager.Current.Filter<FileService>().ComponentTable.GetById(spell.ComponentIDs[i]).Name;
                if (component.Contains("Scarab"))
                {
                    scarabType = component;
                }

                if (!requiredComps.ContainsKey(component))
                {
                    requiredComps.Add(component, 1);
                }
                else
                {
                    requiredComps[component] += 1;
                }
            }

            if (HaveFociOrAugmentation(spell))
            {
                if (!(GetInventoryCount(scarabType) > requiredComps[scarabType]))
                {
                    return false;
                }

                int tapers = GetInventoryCount("Prismatic Taper");
                switch (scarabType)
                {
                    case "Lead Scarab":
                        return tapers >= 1;
                    case "Iron Scarab":
                        return tapers >= 2;
                    case "Copper Scarab":
                        return tapers >= 3;
                    case "Silver Scarab":
                        return tapers >= 4; // this is erroneously set to 4, for now, since ACEmulator is wrong, should be 3
                    case "Gold Scarab":
                        return tapers >= 4;
                    case "Pyreal Scarab":
                        return tapers >= 4;
                    case "Platinum Scarab":
                        return tapers >= 4;
                    case "Mana Scarab":
                        return tapers >= 4;
                }
            }
            else
            {
                foreach (KeyValuePair<string, int> component in requiredComps)
                {
                    if (GetInventoryCount(component.Key) < component.Value) {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool HaveFociOrAugmentation(Spell spell)
        {
            if (spell.School.Name.Equals("Creature Enchantment") &&
                (GetInventoryCount("Foci of Enchantment") > 0 || CoreManager.Current.CharacterFilter.GetCharProperty((int)Augmentations.InfusedCreature) > 0))
            {
                return true;
            }
            if (spell.School.Name.Equals("Item Enchantment") &&
                (GetInventoryCount("Foci of Artifice") > 0 || CoreManager.Current.CharacterFilter.GetCharProperty((int)Augmentations.InfusedItem) > 0))
            {
                return true;
            }
            if (spell.School.Name.Equals("Foci of Verdancy")
                && (GetInventoryCount("Life Magic") > 0 || CoreManager.Current.CharacterFilter.GetCharProperty((int)Augmentations.InfusedLife) > 0))
            {
                return true;
            }
            return false;
        }
    }
}
