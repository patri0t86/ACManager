using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Globalization;
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
            string message = Regex.Replace(e.Text, @"[^\w:/ ']", string.Empty);

            if (Filter.MainView.AutoRespond.Checked)
            {
                // refactor to go to InventoryTracker
                Match match;
                string singleCompResponse = "/t {0}, I currently have {1} {2}.";
                string pluralCompResponse = "/t {0}, I currently have {1} {2}s.";
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

                // checking spell components
                string componentsPattern = string.Format(@"(?<guid>\d+):(?<dupleName>.+?)Tell\s(?<msg>tells)\syou\s(?<secret>.*)");

                match = new Regex(componentsPattern).Match(message);
                if (match.Success)
                {
                    string name = match.Groups["dupleName"].Value.Substring(0, match.Groups["dupleName"].Value.Length / 2);
                    if (match.Groups["secret"].Value.Equals("comps"))
                    {
                        string[] comps = {
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
                        foreach (string comp in comps)
                        {
                            WorldObjectCollection collection = Core.WorldFilter.GetInventory();
                            collection.SetFilter(new ByNameFilter(comp));
                            if (collection.Quantity == 0) continue;
                            Core.Actions.InvokeChatParser(
                                collection.Quantity == 1 ?
                                string.Format(singleCompResponse, name, collection.Quantity.ToString(), collection.First.Name) :
                                string.Format(pluralCompResponse, name, collection.Quantity.ToString(), collection.First.Name)
                            );
                            collection.Dispose();
                        }
                    }
                    else
                    {
                        WorldObjectCollection collection = Core.WorldFilter.GetInventory();
                        collection.SetFilter(new ByNameFilter(textInfo.ToTitleCase(match.Groups["secret"].Value)));
                        if (collection.Count > 0)
                        {
                            Core.Actions.InvokeChatParser(collection.Quantity == 1 ?
                                string.Format(singleCompResponse, name, collection.Quantity.ToString(), collection.First.Name) :
                                string.Format(pluralCompResponse, name, collection.Quantity.ToString(), collection.First.Name)
                            );
                        }
                        collection.Dispose();
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
