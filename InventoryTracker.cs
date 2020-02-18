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
        private int MinLead = -1;
        private int MinIron = -1;
        private int MinCopper = -1;
        private int MinSilver = -1;
        private int MinGold = -1;
        private int MinPyreal = -1;
        private int MinPlatinum = -1;
        private int MinManaScarabs = -1;
        private int MinTapers = -1;
        public bool LogoffEnabled { get; set; }
        public bool AnnounceLogoff { get; set; }
        public int CurLead { get; set; }
        public int CurIron { get; set; }
        public int CurCopper { get; set; }
        public int CurSilver { get; set; }
        public int CurGold { get; set; }
        public int CurPyreal { get; set; }
        public int CurPlatinum { get; set; }
        public int CurManaScarabs { get; set; }
        public int CurTapers { get; set; }

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
                XmlNode node = Utility.LoadCharacterSettings(Module);
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
                                    Parent.SetLeadCount(aNode.InnerText);
                                    break;
                                case "IronScarabCount":
                                    MinIron = int.Parse(aNode.InnerText);
                                    Parent.SetIronCount(aNode.InnerText);
                                    break;
                                case "CopperScarabCount":
                                    MinCopper = int.Parse(aNode.InnerText);
                                    Parent.SetCopperCount(aNode.InnerText);
                                    break;
                                case "SilverScarabCount":
                                    MinSilver = int.Parse(aNode.InnerText);
                                    Parent.SetSilverCount(aNode.InnerText);
                                    break;
                                case "GoldScarabCount":
                                    MinGold = int.Parse(aNode.InnerText);
                                    Parent.SetGoldCount(aNode.InnerText);
                                    break;
                                case "PyrealScarabCount":
                                    MinPyreal = int.Parse(aNode.InnerText);
                                    Parent.SetPyrealCount(aNode.InnerText);
                                    break;
                                case "PlatinumScarabCount":
                                    MinPlatinum = int.Parse(aNode.InnerText);
                                    Parent.SetPlatinumCount(aNode.InnerText);
                                    break;
                                case "ManaScarabCount":
                                    MinManaScarabs = int.Parse(aNode.InnerText);
                                    Parent.SetManaCount(aNode.InnerText);
                                    break;
                                case "TaperCount":
                                    MinTapers = int.Parse(aNode.InnerText);
                                    Parent.SetTaperCount(aNode.InnerText);
                                    break;
                                case "AnnounceLogoff":
                                    if (aNode.InnerText.Equals("True")) 
                                    { 
                                        AnnounceLogoff = true;
                                        Parent.SetAnnounceCheckBox(AnnounceLogoff);
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

        public void SetMinLead(string setting, string value)
        {
            MinLead = int.Parse(value);
            Utility.SaveSetting(Module, setting, value);
        }

        public void SetMinIron(string setting, string value)
        {
            MinIron = int.Parse(value);
            Utility.SaveSetting(Module, setting, value);
        }

        public void SetMinCopper(string setting, string value)
        {
            MinCopper = int.Parse(value);
            Utility.SaveSetting(Module, setting, value);
        }

        public void SetMinSilver(string setting, string value)
        {
            MinSilver = int.Parse(value);
            Utility.SaveSetting(Module, setting, value);
        }

        public void SetMinGold(string setting, string value)
        {
            MinGold = int.Parse(value);
            Utility.SaveSetting(Module, setting, value);
        }

        public void SetMinPyreal(string setting, string value)
        {
            MinPyreal = int.Parse(value);
            Utility.SaveSetting(Module, setting, value);
        }

        public void SetMinPlatinum(string setting, string value)
        {
            MinPlatinum = int.Parse(value);
            Utility.SaveSetting(Module, setting, value);
        }

        public void SetMinMana(string setting, string value)
        {
            MinManaScarabs = int.Parse(value);
            Utility.SaveSetting(Module, setting, value);
        }

        public void SetMinTapers(string setting, string value)
        {
            MinTapers = int.Parse(value);
            Utility.SaveSetting(Module, setting, value);
        }

        public void SetAnnounce(string setting, bool value)
        {
            AnnounceLogoff = value;
            Utility.SaveSetting(Module, setting, value.ToString());
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
            if (CurLead <= MinLead)
            {
                Logout("Lead Scarabs");
            }

            if (CurIron <= MinIron)
            {
                Logout("Iron Scarabs");
            }

            if (CurCopper <= MinCopper)
            {
                Logout("Copper Scarabs");
            }

            if (CurSilver <= MinSilver)
            {
                Logout("Silver Scarabs");
            }

            if (CurGold <= MinGold)
            {
                Logout("Gold Scarabs");
            }

            if (CurPyreal <= MinPyreal)
            {
                Logout("Pyreal Scarabs");
            }

            if (CurPlatinum <= MinPlatinum)
            {
                Logout("Platinum Scarabs");
            }

            if (CurManaScarabs <= MinManaScarabs)
            {
                Logout("Mana Scarabs");
            }

            if (CurTapers <= MinTapers)
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
