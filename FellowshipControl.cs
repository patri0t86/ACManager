using System;
using System.Timers;
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
        private Timer InviteTimer;
        private int RecruitAttempts = 0;
        private int targetGUID = 0;
        public string targetName;
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
            targetName = "";
            targetGUID = 0;
            RecruitAttempts = 0;
            if (FellowStatus == FellowshipEventType.Quit)
            {
                Host.Actions.InvokeChatParser(string.Format("/t {0}, I'm not currently in a fellowship.", name));
            }
            else
            {
                Host.Actions.InvokeChatParser(string.Format("/t {0}, Please stand near me, I'm going to try and recruit you into the fellowship.", name));
                targetName = name;
                targetGUID = GUID;
                StartRecruiting();
            }
        }

        private void StartRecruiting()
        {
            InviteTimer = new Timer(1000);
            InviteTimer.Elapsed += Recruit;
            InviteTimer.AutoReset = true;
            InviteTimer.Start();
        }

        private void Recruit(object sender, ElapsedEventArgs e)
        {
            RecruitAttempts++;
            if (RecruitAttempts == 10)
            {
                Host.Actions.InvokeChatParser(string.Format("/t {0}, I haven't been able to recruit you, or you haven't accepted my invite. Please get closer and I'll attempt to recruit you for another 10 seconds.", targetName));
            }
            if (RecruitAttempts <= 20) {
                Host.Actions.FellowshipRecruit(targetGUID);
            }
            if (RecruitAttempts >= 20)
            {
                Host.Actions.InvokeChatParser(string.Format("/t {0}, I wasn't able to recruit you into the fellowship. Please try again.", targetName));
                InviteTimer.Stop();
            }
        }

        public void StopTimer()
        {
            InviteTimer.Stop();
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
