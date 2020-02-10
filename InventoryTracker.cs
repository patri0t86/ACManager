﻿using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Timers;

namespace FellowshipManager
{
    class InventoryTracker
    {
        private PluginHost Host;
        private CoreManager Core;
        private Utility Utility;
        private Timer ComponentTimer;

        public bool LogoffEnabled { get; set; }

        private string[] Comps = {
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

        public int MinLead { get; set; }
        public int MinIron { get; set; }
        public int MinCopper { get; set; }
        public int MinSilver { get; set; }
        public int MinGold { get; set; }
        public int MinPyreal { get; set; }
        public int MinPlatinum { get; set; }
        public int MinManaScarabs { get; set; }
        public int MinTapers { get; set; }

        public int CurLead { get; set; }
        public int CurIron { get; set; }
        public int CurCopper { get; set; }
        public int CurSilver { get; set; }
        public int CurGold { get; set; }
        public int CurPyreal { get; set; }
        public int CurPlatinum { get; set; }
        public int CurManaScarabs { get; set; }
        public int CurTapers { get; set; }


        public InventoryTracker(PluginHost host, CoreManager core, Utility utility)
        {
            Host = host;
            Core = core;
            Utility = utility;
            StartWatcher();
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
                        CurLead = collection.Count;
                        break;
                    case "Iron Scarab":
                        CurIron = collection.Count;
                        break;
                    case "Copper Scarab":
                        CurCopper = collection.Count;
                        break;
                    case "Silver Scarab":
                        CurSilver = collection.Count;
                        break;
                    case "Gold Scarab":
                        CurGold = collection.Count;
                        break;
                    case "Pyreal Scarab":
                        CurPyreal = collection.Count;
                        break;
                    case "Platinum Scarab":
                        CurPlatinum = collection.Count;
                        break;
                    case "Mana Scarab":
                        CurManaScarabs = collection.Count;
                        break;
                    case "Prismatic Taper":
                        CurTapers = collection.Count;
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
            string message = String.Format("Logging off for low component count: {0}", comp);
            Utility.WriteToChat(message);
            Core.Actions.Logout();
        }
    }
}