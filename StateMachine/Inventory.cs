using ACManager.Settings;
using Decal.Adapter.Wrappers;
using Decal.Filters;
using System.Collections.Generic;

namespace ACManager.StateMachine
{
    internal class Inventory
    {
        private Machine Machine { get; set; }
        internal Dictionary<string, int> SpellComponents { get; set; } = new Dictionary<string, int>();
        internal int ComponentThreshold { get; set; }
        internal int LeadScarabThreshold { get; set; }
        internal int IronScarabThreshold { get; set; }
        internal int CopperScarabThreshold { get; set; }
        internal int SilverScarabThreshold { get; set; }
        internal int GoldScarabThreshold { get; set; }
        internal int PyrealScarabThreshold { get; set; }
        internal int PlatinumScarabThreshold { get; set; }
        internal int ManaScarabThreshold { get; set; }
        internal bool IsLowOnComponents { get; set; } = false;
        
        public Inventory(Machine machine)
        {
            Machine = machine;
            ComponentThreshold = Machine.Utility.BotSettings.ComponentThreshold;
            LeadScarabThreshold = Machine.Utility.BotSettings.LeadScarabThreshold;
            IronScarabThreshold = Machine.Utility.BotSettings.IronScarabThreshold;
            CopperScarabThreshold = Machine.Utility.BotSettings.CopperScarabThreshold;
            SilverScarabThreshold = Machine.Utility.BotSettings.SilverScarabThreshold;
            GoldScarabThreshold = Machine.Utility.BotSettings.GoldScarabThreshold;
            PyrealScarabThreshold = Machine.Utility.BotSettings.PyrealScarabThreshold;
            PlatinumScarabThreshold = Machine.Utility.BotSettings.PlatinumScarabThreshold;
            ManaScarabThreshold = Machine.Utility.BotSettings.ManaScarabThreshold;
        }

        internal void GetComponentLevels()
        {
            SpellComponents.Clear();
            for (int i = 0; i < Machine.Core.Filter<FileService>().ComponentTable.Length; i++)
            {
                using (WorldObjectCollection collection = Machine.Core.WorldFilter.GetInventory())
                {
                    collection.SetFilter(new ByNameFilter(Machine.Core.Filter<FileService>().ComponentTable[i].Name));
                    if (collection.Quantity > 0) 
                    {
                        if (SpellComponents.ContainsKey(Machine.Core.Filter<FileService>().ComponentTable[i].Name))
                        {
                            SpellComponents[Machine.Core.Filter<FileService>().ComponentTable[i].Name] = collection.Quantity;
                        }
                        else if (!SpellComponents.ContainsKey(Machine.Core.Filter<FileService>().ComponentTable[i].Name))
                        {
                            SpellComponents.Add(Machine.Core.Filter<FileService>().ComponentTable[i].Name, collection.Quantity);
                        }
                    }
                }
            }
            CheckComponentThresholds();
        }

        private void CheckComponentThresholds()
        {
            IsLowOnComponents = false;
            foreach (KeyValuePair<string, int> component in SpellComponents)
            {
                if (component.Key.Contains("Scarab"))
                {
                    IsLowOnComponents = IsScarabLow(component.Key, component.Value);
                    if (IsLowOnComponents) break;
                }
                else
                {
                    IsLowOnComponents = IsComponentLow(component.Value);
                    if (IsLowOnComponents) break;
                }
            }
        }

        private bool IsComponentLow(int qty)
        {
            return qty <= ComponentThreshold;
        }

        private bool IsScarabLow(string comp, int qty)
        {
            switch (comp)
            {
                case "Lead Scarab":
                    return qty <= LeadScarabThreshold;
                case "Iron Scarab":
                    return qty <= IronScarabThreshold;
                case "Copper Scarab":
                    return qty <= CopperScarabThreshold;
                case "Silver Scarab":
                    return qty <= SilverScarabThreshold;
                case "Gold Scarab":
                    return qty <= GoldScarabThreshold;
                case "Pyreal Scarab":
                    return qty <= PyrealScarabThreshold;
                case "Platinum Scarab":
                    return qty <= PlatinumScarabThreshold;
                case "Mana Scarab":
                    return qty <= ManaScarabThreshold;
            }
            return false;
        }

        internal void UpdateInventoryFile()
        {
            // create new inventory to write to in-memory
            CharacterInventory inv = new CharacterInventory
            {
                Name = Machine.Core.CharacterFilter.Name
            };

            // go through entire components table and get quantities and add them to inventory in-memory if quantity > 0
            for (int i = 0; i < Machine.Core.Filter<FileService>().ComponentTable.Length; i++)
            {
                int quantity = GetInventoryCount(Machine.Core.Filter<FileService>().ComponentTable[i].Name);
                if (quantity > 0)
                {
                    AcmComponent component = new AcmComponent()
                    {
                        Name = Machine.Core.Filter<FileService>().ComponentTable[i].Name,
                        Quantity = quantity
                    };
                    inv.Components.Add(component);
                }
            }

            // search for gems known in the bot settings and add them to inventory
            foreach (GemSetting gem in Machine.Utility.BotSettings.GemSettings)
            {
                using(WorldObjectCollection inventory = Machine.Core.WorldFilter.GetInventory())
                {
                    int quantity = GetInventoryCount(gem.Name);
                    if (quantity > 0)
                    {
                        Gem newGem = new Gem
                        {
                            Name = gem.Name,
                            Quantity = quantity
                        };
                        inv.Gems.Add(newGem);
                    }
                }
            }

            // if the inventory file contains this character, update it, otherwise add it
            if (Machine.Utility.Inventory.CharacterInventories.Contains(inv))
            {
                for (int i = 0; i < Machine.Utility.Inventory.CharacterInventories.Count; i++)
                {
                    if (Machine.Utility.Inventory.CharacterInventories[i].Equals(inv))
                    {
                        Machine.Utility.Inventory.CharacterInventories[i] = inv;
                        break;
                    }
                }
            }
            else
            {
                Machine.Utility.Inventory.CharacterInventories.Add(inv);
            }

            // save inventory to file
            Machine.Utility.SaveInventory();
        }

        internal bool UseItem(string itemName)
        {
            using (WorldObjectCollection inventory = Machine.Core.WorldFilter.GetInventory())
            {
                inventory.SetFilter(new ByNameFilter(itemName));
                if (inventory.Quantity > 0)
                {
                    Machine.Core.Actions.UseItem(inventory.First.Id, 0);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        internal int GetInventoryCount(string itemName)
        {
            using(WorldObjectCollection inventory = Machine.Core.WorldFilter.GetInventory())
            {
                inventory.SetFilter(new ByNameFilter(itemName));
                return inventory.Quantity;
            }
        }
    }
}
