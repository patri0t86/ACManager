using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Filters;
using System;
using System.Text.RegularExpressions;

namespace ACManager
{
    internal class InventoryTracker
    {
        private FilterCore Filter { get; set; }
        private CoreManager Core { get; set; }
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

        public InventoryTracker(FilterCore parent, CoreManager core)
        {
            try
            {
                Filter = parent;
                Core = core;
                Filter.MainView.LeadScarabText.Text = Filter.Machine.CurrentCharacter.LeadScarabs.ToString();
                Filter.MainView.IronScarabText.Text = Filter.Machine.CurrentCharacter.IronScarabs.ToString();
                Filter.MainView.CopperScarabText.Text = Filter.Machine.CurrentCharacter.CopperScarabs.ToString();
                Filter.MainView.SilverScarabText.Text = Filter.Machine.CurrentCharacter.SilverScarabs.ToString();
                Filter.MainView.GoldScarabText.Text = Filter.Machine.CurrentCharacter.GoldScarabs.ToString();
                Filter.MainView.PyrealScarabText.Text = Filter.Machine.CurrentCharacter.PyrealScarabs.ToString();
                Filter.MainView.PlatinumScarabText.Text = Filter.Machine.CurrentCharacter.PlatinumScarabs.ToString();
                Filter.MainView.ManaScarabText.Text = Filter.Machine.CurrentCharacter.ManaScarabs.ToString();
                Filter.MainView.TaperText.Text = Filter.Machine.CurrentCharacter.Tapers.ToString();
                Filter.MainView.AnnounceLogoff.Checked = Filter.Machine.CurrentCharacter.AnnounceLogoff;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public void ParseChat(object sender, ChatTextInterceptEventArgs e)
        {
            if (Filter.MainView.AutoRespond.Checked)
            {
                string sanitizedInput = Regex.Replace(e.Text.ToLower(), @"[^\w:/ ']", string.Empty);
                Match match = new Regex("(?<guid>\\d+):(?<dupleName>.+?)tell tells you (?<message>.*$)").Match(sanitizedInput);
                string message = match.Groups["message"].Value;
                int guid = Convert.ToInt32(match.Groups["guid"].Value);
                string name = match.Groups["dupleName"].Value.Substring(0, match.Groups["dupleName"].Value.Length / 2);
                if (match.Success)
                {
                    if (message.Equals("comps"))
                    {
                        for (int i = 0; i < Core.Filter<FileService>().ComponentTable.Length; i++)
                        {
                            using (WorldObjectCollection collection = Core.WorldFilter.GetInventory())
                            {
                                collection.SetFilter(new ByNameFilter(Core.Filter<FileService>().ComponentTable[i].Name));
                                if (collection.Quantity.Equals(0))
                                {
                                    continue;
                                }
                                else if (collection.Quantity.Equals(1))
                                {
                                    Filter.Machine.ChatManager.SendTell(name, $"I have {collection.Quantity} {collection.First.Name}.");
                                }
                                else
                                {
                                    Filter.Machine.ChatManager.SendTell(name, $"I have {collection.Quantity} {collection.First.Name}{(collection.Quantity > 1 ? "s" : "")}.");
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CheckComps()
        {
            if (Filter.MainView.LowCompLogoff.Checked)
            {
                GetCompCounts();
            }
        }

        private void GetCompCounts()
        {
            foreach (string comp in Comps)
            {
                using (WorldObjectCollection collection = Core.WorldFilter.GetInventory())
                {
                    collection.SetFilter(new ByNameFilter(comp));
                    switch (comp)
                    {
                        case "Lead Scarab":
                            if (collection.Quantity < int.Parse(Filter.MainView.LeadScarabText.Text))
                            {
                                Logout("Lead Scarab");
                            }
                            break;
                        case "Iron Scarab":
                            if (collection.Quantity < int.Parse(Filter.MainView.IronScarabText.Text))
                            {
                                Logout("Iron Scarab");
                            }
                            break;
                        case "Copper Scarab":
                            if (collection.Quantity < int.Parse(Filter.MainView.CopperScarabText.Text))
                            {
                                Logout("Copper Scarab");
                            }
                            break;
                        case "Silver Scarab":
                            if (collection.Quantity < int.Parse(Filter.MainView.SilverScarabText.Text))
                            {
                                Logout("Silver Scarab");
                            };
                            break;
                        case "Gold Scarab":
                            if (collection.Quantity < int.Parse(Filter.MainView.GoldScarabText.Text))
                            {
                                Logout("Gold Scarab");
                            }
                            break;
                        case "Pyreal Scarab":
                            if (collection.Quantity < int.Parse(Filter.MainView.PyrealScarabText.Text))
                            {
                                Logout("Pyreal Scarab");
                            }
                            break;
                        case "Platinum Scarab":
                            if (collection.Quantity < int.Parse(Filter.MainView.PlatinumScarabText.Text))
                            {
                                Logout("Platinum Scarab");
                            }
                            break;
                        case "Mana Scarab":
                            if (collection.Quantity < int.Parse(Filter.MainView.ManaScarabText.Text))
                            {
                                Logout("Mana Scarab");
                            }
                            break;
                        case "Prismatic Taper":
                            if (collection.Quantity < int.Parse(Filter.MainView.TaperText.Text))
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
            Filter.MainView.LowCompLogoff.Checked = false;
            string message = $"Logging off for low component count: {comp}";
            if (Filter.MainView.AnnounceLogoff.Checked)
            {
                Core.Actions.InvokeChatParser($"/f {message}");
            }
            Debug.ToChat(message);
            Core.Actions.Logout();
        }
    }
}
