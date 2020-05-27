using ACManager.Views;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace ACManager
{
    [WireUpBaseEvents]
    [FriendlyName("AC Manager")]
    public class PluginCore : PluginBase
    {
        internal const string Module = "General";
        internal const string PluginName = "AC Manager";
        internal FellowshipControl FellowshipControl { get; set; }
        internal ExpTracker ExpTracker { get; set; }
        internal InventoryTracker InventoryTracker { get; set; }
        internal MainView MainView { get; set; }
        internal PortalBotView PortalBotView { get; set; }
        internal ExpTrackerView ExpTrackerView { get; set; }
        internal bool AutoRespondEnabled { get; set; }

        protected override void Startup()
        {
            try
            {
                MainView = new MainView(this);
                ExpTrackerView = new ExpTrackerView(this);
                PortalBotView = new PortalBotView();
            }
            catch { }
        }

        protected override void Shutdown()
        {
            try
            {

            }
            catch (Exception ex) { Utility.LogError(ex); }
        }

        [BaseEvent("LoginComplete", "CharacterFilter")]
        private void LoginComplete(object sender, EventArgs e)
        {
            try
            {
                InventoryTracker = new InventoryTracker(this);
                FellowshipControl = new FellowshipControl(this);
                ExpTracker = new ExpTracker(this, Host, Core);
                LoadSettings();

                // Start listening to all chat and parsing if enabled
                Core.ChatBoxMessage += new EventHandler<ChatTextInterceptEventArgs>(ParseChat);
                Core.RenderFrame += Core_RenderFrame;
            }
            catch (Exception ex) { Utility.LogError(ex); }
        }

        [BaseEvent("Logoff", "CharacterFilter")]
        private void CharacterFilter_Logoff(object sender, LogoffEventArgs e)
        {
            try
            {
                Core.ChatBoxMessage -= new EventHandler<ChatTextInterceptEventArgs>(ParseChat);
                Core.RenderFrame -= Core_RenderFrame;
            }
            catch (Exception ex) { Utility.LogError(ex); }
        }

        private void Core_RenderFrame(object sender, EventArgs e)
        {
            InventoryTracker.CheckComps();
            FellowshipControl.CheckRecruit();
        }

        private void LoadSettings()
        {
            try
            {
                XmlNode node = Utility.LoadCharacterSettings(Module, characterName: CoreManager.Current.CharacterFilter.Name);
                if (node != null)
                {
                    XmlNodeList settingNodes = node.ChildNodes;
                    if (settingNodes.Count > 0)
                    {
                        foreach (XmlNode aNode in settingNodes)
                        {
                            switch (aNode.Name)
                            {
                                case "AutoRespond":
                                    if (aNode.InnerText.Equals("True"))
                                    {
                                        MainView.AutoRespond.Checked = true;
                                        AutoRespondEnabled = true;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        private void ParseChat(object sender, ChatTextInterceptEventArgs e)
        {
            string sanitizedInput = Regex.Replace(e.Text, @"[^\w:/ ']", string.Empty);

            FellowshipControl.ChatActions(sanitizedInput);

            if (AutoRespondEnabled)
            {
                // refactor to go to InventoryTracker
                Match match;
                string singleCompResponse = "/t {0}, I currently have {1} {2}.";
                string pluralCompResponse = "/t {0}, I currently have {1} {2}s.";
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

                // checking spell components
                string componentsPattern = string.Format(@"(?<guid>\d+):(?<dupleName>.+?)Tell\s(?<msg>tells)\syou\s(?<secret>.*)");

                match = new Regex(componentsPattern).Match(sanitizedInput);
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
                            Host.Actions.InvokeChatParser(
                                collection.Quantity == 1 ?
                                string.Format(singleCompResponse, name, collection.Quantity.ToString(), collection.First.Name) :
                                string.Format(pluralCompResponse, name, collection.Quantity.ToString(), collection.First.Name)
                            );
                        }
                    }
                    else
                    {
                        WorldObjectCollection collection = Core.WorldFilter.GetInventory();
                        collection.SetFilter(new ByNameFilter(textInfo.ToTitleCase(match.Groups["secret"].Value)));
                        if (collection.Count > 0)
                        {
                            Host.Actions.InvokeChatParser(collection.Quantity == 1 ?
                                string.Format(singleCompResponse, name, collection.Quantity.ToString(), collection.First.Name) :
                                string.Format(pluralCompResponse, name, collection.Quantity.ToString(), collection.First.Name)
                            );
                        }
                    }
                }
            }
        }
    }
}
