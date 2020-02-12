using System;
using System.Xml;
using Decal.Adapter;
using Decal.Adapter.Wrappers;

namespace ACManager
{
    public class FellowshipControl
    {
        private PluginCore Parent;
        private PluginHost Host;
        private CoreManager Core;
        private const string Module = "FellowshipManager";
        public string Password = "XP";
        public FellowshipEventType FellowStatus { get; set; } = FellowshipEventType.Quit;

        public FellowshipControl(PluginCore parent, PluginHost host, CoreManager core)
        {
            Parent = parent;
            Host = host;
            Core = core;
            LoadSettings();
        }

        public void InviteRequested(int GUID, string name)
        {
            if (FellowStatus == FellowshipEventType.Quit)
            {
                Host.Actions.InvokeChatParser(string.Format("/t {0}, I'm not currently in a fellowship.", name));
            }
            else if (FellowStatus == FellowshipEventType.Create)
            {
                Host.Actions.InvokeChatParser(string.Format("/t {0}, Please stand near me, I'm going to try and recruit you into the fellowship.", name));
                Host.Actions.FellowshipRecruit(GUID);
            }
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
                                case "SecretPassword":
                                    if (!string.IsNullOrEmpty(aNode.InnerText))
                                    {
                                        Password = aNode.InnerText;
                                        Parent.SetPassword(Password);
                                    }
                                    break;
                                case "AutoFellow":
                                    if (aNode.InnerText.Equals("True"))
                                    {
                                        Parent.SetAutoFellow(true);
                                        ChatParser.AutoFellow = true;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Utility.LogError(ex); }
        }

        public void SetPassword(string setting, string value)
        {
            Utility.SaveSetting(Module, setting, value);
            Password = value;
        }

        public void SetAutoFellow(string setting, bool value)
        {
            Utility.SaveSetting(Module, setting, value.ToString());
            ChatParser.AutoFellow = value;
            if (value && FellowStatus == FellowshipEventType.Create)
            {
                Host.Actions.FellowshipSetOpen(true);
            }
        }
    }
}
