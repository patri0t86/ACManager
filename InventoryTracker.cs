using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Timers;
using System.Xml;

namespace ACManager
{
    class InventoryTracker
    {
        private const string Module = "InventoryTracker";
        private PluginCore Parent;
        private PluginHost Host;
        private CoreManager Core;
        private Timer ComponentTimer;
        private readonly string[] Comps = {
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
        internal int MinLead { get; set; } = -1;
        internal int MinIron {get;set;} = -1;
        internal int MinCopper {get;set;} = -1;
        internal int MinSilver {get;set;} = -1;
        internal int MinGold {get;set;} = -1;
        internal int MinPyreal {get;set;} = -1;
        internal int MinPlatinum {get;set;} = -1;
        internal int MinManaScarabs {get;set;} = -1;
        internal int MinTapers {get;set;} = -1;
        internal bool LogoffEnabled { get; set; }
        internal bool AnnounceLogoff { get; set; }
        internal int CurLead { get; set; }
        internal int CurIron { get; set; }
        internal int CurCopper { get; set; }
        internal int CurSilver { get; set; }
        internal int CurGold { get; set; }
        internal int CurPyreal { get; set; }
        internal int CurPlatinum { get; set; }
        internal int CurManaScarabs { get; set; }
        internal int CurTapers { get; set; }

        public InventoryTracker(PluginCore parent, PluginHost host, CoreManager core)
        {
            Parent = parent;
            Host = host;
            Core = core;
            LoadSettings();
            StartWatcher();
        }

        private void LoadSettings()
        {
            try
            {
                XmlNode node = Utility.LoadCharacterSettings(Module, characterName:Utility.CharacterName);
                if (node != null)
                {
                    XmlNodeList settingNodes = node.ChildNodes;
                    if (settingNodes.Count > 0)
                    {
                        foreach (XmlNode aNode in settingNodes)
                        {
                            switch (aNode.Name)
                            {
                                case "LeadScarabCount":
                                    MinLead = int.Parse(aNode.InnerText);
                                    Parent.MainView.LeadScarabText.Text = aNode.InnerText;
                                    break;
                                case "IronScarabCount":
                                    MinIron = int.Parse(aNode.InnerText);
                                    Parent.MainView.IronScarabText.Text = aNode.InnerText;
                                    break;
                                case "CopperScarabCount":
                                    MinCopper = int.Parse(aNode.InnerText);
                                    Parent.MainView.CopperScarabText.Text = aNode.InnerText;
                                    break;
                                case "SilverScarabCount":
                                    MinSilver = int.Parse(aNode.InnerText);
                                    Parent.MainView.SilverScarabText.Text = aNode.InnerText;
                                    break;
                                case "GoldScarabCount":
                                    MinGold = int.Parse(aNode.InnerText);
                                    Parent.MainView.GoldScarabText.Text = aNode.InnerText;
                                    break;
                                case "PyrealScarabCount":
                                    MinPyreal = int.Parse(aNode.InnerText);
                                    Parent.MainView.PyrealScarabText.Text = aNode.InnerText;
                                    break;
                                case "PlatinumScarabCount":
                                    MinPlatinum = int.Parse(aNode.InnerText);
                                    Parent.MainView.PlatinumScarabText.Text = aNode.InnerText;
                                    break;
                                case "ManaScarabCount":
                                    MinManaScarabs = int.Parse(aNode.InnerText);
                                    Parent.MainView.ManaScarabText.Text = aNode.InnerText;
                                    break;
                                case "TaperCount":
                                    MinTapers = int.Parse(aNode.InnerText);
                                    Parent.MainView.TaperText.Text = aNode.InnerText;
                                    break;
                                case "AnnounceLogoff":
                                    if (aNode.InnerText.Equals("True")) 
                                    { 
                                        Parent.MainView.AnnounceLogoff.Checked = true;
                                        AnnounceLogoff = true;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void CheckComps(object sender, ElapsedEventArgs e)
        {
            GetCompCounts();
            if (LogoffEnabled) MinCompsCheck();
        }

        private void GetCompCounts()
        {
            foreach (string comp in Comps)
            {
                WorldObjectCollection collection = Core.WorldFilter.GetInventory();
                collection.SetFilter(new ByNameFilter(comp));
                switch (comp)
                {
                    case "Lead Scarab":
                        CurLead = collection.Quantity;
                        break;
                    case "Iron Scarab":
                        CurIron = collection.Quantity;
                        break;
                    case "Copper Scarab":
                        CurCopper = collection.Quantity;
                        break;
                    case "Silver Scarab":
                        CurSilver = collection.Quantity;
                        break;
                    case "Gold Scarab":
                        CurGold = collection.Quantity;
                        break;
                    case "Pyreal Scarab":
                        CurPyreal = collection.Quantity;
                        break;
                    case "Platinum Scarab":
                        CurPlatinum = collection.Quantity;
                        break;
                    case "Mana Scarab":
                        CurManaScarabs = collection.Quantity;
                        break;
                    case "Prismatic Taper":
                        CurTapers = collection.Quantity;
                        break;
                }
                collection = null;
            }
        }

        private void MinCompsCheck()
        {
            if (CurLead < MinLead)
            {
                Logout("Lead Scarabs");
            }

            if (CurIron < MinIron)
            {
                Logout("Iron Scarabs");
            }

            if (CurCopper < MinCopper)
            {
                Logout("Copper Scarabs");
            }

            if (CurSilver < MinSilver)
            {
                Logout("Silver Scarabs");
            }

            if (CurGold < MinGold)
            {
                Logout("Gold Scarabs");
            }

            if (CurPyreal < MinPyreal)
            {
                Logout("Pyreal Scarabs");
            }

            if (CurPlatinum < MinPlatinum)
            {
                Logout("Platinum Scarabs");
            }

            if (CurManaScarabs < MinManaScarabs)
            {
                Logout("Mana Scarabs");
            }

            if (CurTapers < MinTapers)
            {
                Logout("Prismatic Tapers");
            }
        }

        private void StartWatcher()
        {
            ComponentTimer = new Timer(1000);
            ComponentTimer.AutoReset = true;
            ComponentTimer.Elapsed += CheckComps;
            ComponentTimer.Start();
        }

        private void Logout(string comp)
        {
            ComponentTimer.Stop();
            string message = String.Format("/f Logging off for low component count: {0}", comp);
            if(AnnounceLogoff)
            {
                Host.Actions.InvokeChatParser(message);
            }
            Core.Actions.Logout();
        }
    }
}
