using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;

namespace ACManager
{
    internal class InventoryTracker
    {
        private PluginCore Plugin { get; set; }
        private string[] Comps { get; set; } = {
                        "Lead Scarab",
                        "Iron Scarab",
                        "Copper Scarab",
                        "Silver Scarab",
                        "Gold Scarab",
                        "Pyreal Scarab",
                        "Platinum Scarab",
                        "Mana Scarab",
                        "Prismatic Taper"
                        };

        public InventoryTracker(PluginCore parent)
        {
            try
            {
                Plugin = parent;
                Plugin.MainView.LeadScarabText.Text = Plugin.CurrentCharacter.LeadScarabs.ToString();
                Plugin.MainView.IronScarabText.Text = Plugin.CurrentCharacter.IronScarabs.ToString();
                Plugin.MainView.CopperScarabText.Text = Plugin.CurrentCharacter.CopperScarabs.ToString();
                Plugin.MainView.SilverScarabText.Text = Plugin.CurrentCharacter.SilverScarabs.ToString();
                Plugin.MainView.GoldScarabText.Text = Plugin.CurrentCharacter.GoldScarabs.ToString();
                Plugin.MainView.PyrealScarabText.Text = Plugin.CurrentCharacter.PyrealScarabs.ToString();
                Plugin.MainView.PlatinumScarabText.Text = Plugin.CurrentCharacter.PlatinumScarabs.ToString();
                Plugin.MainView.ManaScarabText.Text = Plugin.CurrentCharacter.ManaScarabs.ToString();
                Plugin.MainView.TaperText.Text = Plugin.CurrentCharacter.Tapers.ToString();
                Plugin.MainView.AnnounceLogoff.Checked = Plugin.CurrentCharacter.AnnounceLogoff;
            }
            catch (Exception ex)
            {
                Plugin.Utility.LogError(ex);
            }
        }

        public void CheckComps()
        {
            if (Plugin.MainView.LowCompLogoff.Checked)
            {
                GetCompCounts();
            }
        }

        private void GetCompCounts()
        {
            foreach (string comp in Comps)
            {
                using (WorldObjectCollection collection = CoreManager.Current.WorldFilter.GetInventory())
                {
                    collection.SetFilter(new ByNameFilter(comp));
                    switch (comp)
                    {
                        case "Lead Scarab":
                            if (collection.Quantity < int.Parse(Plugin.MainView.LeadScarabText.Text))
                            {
                                Logout("Lead Scarab");
                            }
                            break;
                        case "Iron Scarab":
                            if (collection.Quantity < int.Parse(Plugin.MainView.IronScarabText.Text))
                            {
                                Logout("Iron Scarab");
                            }
                            break;
                        case "Copper Scarab":
                            if (collection.Quantity < int.Parse(Plugin.MainView.CopperScarabText.Text))
                            {
                                Logout("Copper Scarab");
                            }
                            break;
                        case "Silver Scarab":
                            if (collection.Quantity < int.Parse(Plugin.MainView.SilverScarabText.Text))
                            {
                                Logout("Silver Scarab");
                            };
                            break;
                        case "Gold Scarab":
                            if (collection.Quantity < int.Parse(Plugin.MainView.GoldScarabText.Text))
                            {
                                Logout("Gold Scarab");
                            }
                            break;
                        case "Pyreal Scarab":
                            if (collection.Quantity < int.Parse(Plugin.MainView.PyrealScarabText.Text))
                            {
                                Logout("Pyreal Scarab");
                            }
                            break;
                        case "Platinum Scarab":
                            if (collection.Quantity < int.Parse(Plugin.MainView.PlatinumScarabText.Text))
                            {
                                Logout("Platinum Scarab");
                            }
                            break;
                        case "Mana Scarab":
                            if (collection.Quantity < int.Parse(Plugin.MainView.ManaScarabText.Text))
                            {
                                Logout("Mana Scarab");
                            }
                            break;
                        case "Prismatic Taper":
                            if (collection.Quantity < int.Parse(Plugin.MainView.TaperText.Text))
                            {
                                Logout("Prismatic Taper");
                            }
                            break;
                    }
                }
            }
        }

        private void Logout(string comp)
        {
            Plugin.MainView.LowCompLogoff.Checked = false;
            string message = $"Logging off for low component count: {comp}";
            if (Plugin.MainView.AnnounceLogoff.Checked)
            {
                CoreManager.Current.Actions.InvokeChatParser($"/f {message}");
            }
            Plugin.Utility.WriteToChat(message);
            CoreManager.Current.Actions.Logout();
        }
    }
}
