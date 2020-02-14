using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        public bool AutoFellowEnabled = false;
        private static List<NewRecruit> RecruitList;
        public FellowshipEventType FellowStatus { get; set; } = FellowshipEventType.Quit;

        public FellowshipControl(PluginCore parent, PluginHost host, CoreManager core)
        {
            Parent = parent;
            Host = host;
            Core = core;
            RecruitList = new List<NewRecruit>();
            LoadSettings();
        }

        public void ChatActions(string parsedMessage)
        {
            if (AutoFellowEnabled)
            {
                Dictionary<string, string> AutoFellowStrings = new Dictionary<string, string>
                {
                    ["CreatedFellow"] = @"^You have created the Fellowship",
                    ["NewLeader"] = string.Format(@"{0} now leads the fellowship", Utility.CharacterName),
                    ["NotAccepting"] = @"(?<name>.+?) is not accepting fellowing requests",
                    ["JoinedFellow"] = @"(?<name>.+?) joined the fellowship",
                    ["ElseLeftFellow"] = @"(?<name>.+?) left the fellowship",
                    ["RequestFellow"] = string.Format(@"(?<guid>\d+):(?<dupleName>.+?)Tell\s(?<msg>tells)\syou\s(?<secret>{0}$)", Password),
                    ["DeclinedFellow"] = @"(?<name>.+?) declines your invite",
                    ["AlreadyFellow"] = @"(?<name>.+?) is already in a fellowship",
                    ["FullFellow"] = @"^Fellowship is already full"
                };

                Match match;

                foreach (var item in AutoFellowStrings)
                {
                    match = new Regex(item.Value).Match(parsedMessage);
                    if (match.Success)
                    {
                        switch (item.Key)
                        {
                            case "CreatedFellow": // You have created the Fellowship
                                Host.Actions.FellowshipSetOpen(true);
                                break;
                            case "NewLeader": // You inherited leader
                                Host.Actions.FellowshipSetOpen(true);
                                break;
                            case "NotAccepting": // Target is not accepting fellowship requests
                                Host.Actions.InvokeChatParser(string.Format("/t {0}, You are not accepting fellowship requests!", match.Groups["name"].Value));
                                RemoveRecruit(match.Groups["name"].Value);
                                break;
                            case "JoinedFellow": // Target joins the fellowship
                                RemoveRecruit(match.Groups["name"].Value);
                                break;
                            case "ElseLeftFellow": // Someone leaves the fellowship
                                break;
                            case "FullFellow": // Fellowship is currently full
                                Host.Actions.InvokeChatParser(string.Format("/r The fellowship is already full."));
                                RemoveRecruit(match.Groups["name"].Value);
                                break;
                            case "RequestFellow": // Someone /tells you the fellowship password
                                string name = match.Groups["dupleName"].Value.Substring(0, match.Groups["dupleName"].Value.Length / 2);
                                if (FellowStatus == FellowshipEventType.Quit)
                                {
                                    Host.Actions.InvokeChatParser(string.Format("/t {0}, I'm not currently in a fellowship.", name));
                                }
                                if (match.Groups["msg"].Value.Equals("tells") && match.Groups["secret"].Value.Equals(Password))
                                {
                                    bool exists = false;
                                    for (int i = 0; i < RecruitList.Count; i++)
                                    {
                                        if (RecruitList[i].Name.Equals(name))
                                        {
                                            exists = true;
                                            Host.Actions.InvokeChatParser(string.Format("/t {0}, You have already requested a fellowship invitation.", name));
                                        }
                                    }
                                    if (!exists)
                                    {
                                        int targetGuid = int.Parse(match.Groups["guid"].Value);
                                        NewRecruit recruit = new NewRecruit(Host, name, targetGuid);
                                        RecruitList.Add(recruit);
                                    }
                                }
                                break;
                            case "DeclinedFellow": // Declines fellowship invite
                                RemoveRecruit(match.Groups["name"].Value);
                                break;
                            case "AlreadyFellow": // Target is already in a fellowship
                                Host.Actions.InvokeChatParser(String.Format("/t {0}, You're already in a fellowship.", match.Groups["name"].Value));
                                RemoveRecruit(match.Groups["name"].Value);
                                break;
                        }
                        break;
                    }
                }
            }
        }

        private void RemoveRecruit(string name)
        {
            for (int i = 0; i < RecruitList.Count; i++)
            {
                if (RecruitList[i].Name.Equals(name))
                {
                    RecruitList[i].Stop();
                    RecruitList.RemoveAt(i);
                    break;
                }
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
                                        AutoFellowEnabled = true;
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
            AutoFellowEnabled = value;
            if (value && FellowStatus == FellowshipEventType.Create)
            {
                Host.Actions.FellowshipSetOpen(true);
            }
        }

        private class NewRecruit
        {
            private PluginHost Host;
            private Timer Timer;
            public string Name { get; set; }
            private int Id, Attempts;
            public NewRecruit(PluginHost host, string target, int guid)
            {
                Host = host;
                Name = target;
                Id = guid;
                Start();
            }
            
            private void Start()
            {
                Host.Actions.InvokeChatParser(string.Format("/t {0}, Please stand near me, I'm going to try and recruit you into the fellowship.", Name));
                Timer = new Timer(1000);
                Timer.Elapsed += AttemptRecruit;
                Timer.AutoReset = true;
                Timer.Start();
            }

            private void AttemptRecruit(object sender, ElapsedEventArgs e)
            {
                Attempts++;
                if (Attempts == 10)
                {
                    Host.Actions.InvokeChatParser(string.Format("/t {0}, I haven't been able to recruit you, or you haven't accepted my invite. " +
                        "Please get closer and I'll attempt to recruit you for another 10 seconds.", Name));
                }
                if (Attempts <= 20)
                {
                    Host.Actions.FellowshipRecruit(Id);
                }
                if (Attempts >= 20)
                {
                    Host.Actions.InvokeChatParser(string.Format("/t {0}, I wasn't able to recruit you into the fellowship. Please try again.", Name));
                    Stop();
                }
            }

            public void Stop()
            { 
                Timer.Stop();
                Host = null;
                Timer = null;
                Name = null;
                Id = 0;
                Attempts = 0;
                FellowshipControl.RecruitList.Remove(this);
            }

        }

    }
}
