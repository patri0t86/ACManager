using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Filters;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACManager.StateMachine
{
    public class Inventory
    {
        private readonly CoreManager Core;
        internal Dictionary<string, int> SpellComponents { get; set; } = new Dictionary<string, int>();
        internal Dictionary<string, int> PortalGems { get; set; } = new Dictionary<string, int>();
        internal int LowTaperThreshhold { get; set; } = 100;
        internal int LowScarabThreshold { get; set; } = 10;
        internal bool IsLowOnComponents { get; set; } = false;
        

        public Inventory(CoreManager core)
        {
            Core = core;
        }

        internal void GetComponentLevels()
        {
            for (int i = 0; i < Core.Filter<FileService>().ComponentTable.Length; i++)
            {
                using (WorldObjectCollection collection = Core.WorldFilter.GetInventory())
                {
                    collection.SetFilter(new ByNameFilter(Core.Filter<FileService>().ComponentTable[i].Name));
                    if (collection.Quantity > 0) {
                        if (SpellComponents.ContainsKey(Core.Filter<FileService>().ComponentTable[i].Name))
                        {
                            SpellComponents[Core.Filter<FileService>().ComponentTable[i].Name] = collection.Quantity;

                        }
                        else if (!SpellComponents.ContainsKey(Core.Filter<FileService>().ComponentTable[i].Name))
                        {
                            SpellComponents.Add(Core.Filter<FileService>().ComponentTable[i].Name, collection.Quantity);
                        }
                    }
                }
            }
            CheckComponentThresholds();
        }

        private void CheckComponentThresholds()
        {
            foreach (KeyValuePair<string, int> component in SpellComponents)
            {
                if (component.Key.Contains("Scarab"))
                {
                    IsLowOnComponents = IsScarabLow(component.Value);
                    break;
                }
                else
                {
                    IsLowOnComponents = IsComponentLow(component.Value);
                    break;
                }
            }
            
        }

        private bool IsComponentLow(int qty)
        {
            return qty <= LowTaperThreshhold;
        }

        private bool IsScarabLow(int qty)
        {
            return qty <= LowScarabThreshold;
        }

        internal void ReportComponentLevels()
        {
            foreach (KeyValuePair<string, int> component in SpellComponents)
            {
                Debug.ToChat($"{component.Key}: {component.Value}");
            }
        }
    }
}
