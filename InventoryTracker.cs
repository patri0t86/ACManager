using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Filters;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ACManager
{
    internal class InventoryTracker : IDisposable
    {
        private FilterCore Filter { get; set; }
        private CoreManager Core { get; set; }
        private Timer Timer { get; set; }
        private bool LoggingOut { get; set; }
        private bool disposedValue;
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
            Filter = parent;
            Core = core;
            Timer = new Timer
            {
                Interval = 5000
            };
            Timer.Tick += Timer_Tick;
        }

        internal void StartTimer()
        {
            Timer.Start();
        }

        internal void StopTimer()
        {
            Timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            GetCompCounts();
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
            StopTimer();
            string message = $"Logging off for low component count: {comp}";
            if (Filter.MainView.AnnounceLogoff.Checked)
            {
                Core.Actions.InvokeChatParser($"/f {message}");
            }
            Debug.ToChat(message);
            if (!LoggingOut)
            {
                Core.Actions.Logout();
            }
            LoggingOut = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Timer?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
