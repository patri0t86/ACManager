using Decal.Adapter;
using Decal.Adapter.Wrappers;
using MyClasses.MetaViewWrappers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace FellowshipManager
{
    //Attaches events from core
    [WireUpBaseEvents]

    //View (UI) handling
    [MVView("FellowshipManager.mainView.xml")]
    [MVWireUpControlEvents]
    [FriendlyName("Fellowship Manager")]
    public class PluginCore : PluginBase
    {
        const string Module = "FellowshipManager";
        private readonly string PluginName = "Fellowship Manager";
        private ExpTracker ExpTracker;
        private Utility Utility;
        private string targetRecruit;
        private int targetGuid;
        private LogoffEventType LogoffType;
        private bool inFellow = false;

        public EventHandler<ConfigEventArgs> RaiseAutoFellowEvent;
        public EventHandler<ConfigEventArgs> RaiseAutoResponderEvent;
        public EventHandler<ConfigEventArgs> RaiseSecretPasswordEvent;

        [MVControlReference("SecretPassword")]
        private ITextBox SecretPasswordTextBox = null;
        [MVControlReference("AutoFellow")]
        private ICheckBox AutoFellowCheckBox = null;
        [MVControlReference("AutoRespond")]
        private ICheckBox AutoRespondCheckBox = null;
        [MVControlReference("XpAtLogon")]
        private IStaticText XpAtLogonText = null;
        [MVControlReference("XpSinceLogon")]
        private IStaticText XpSinceLogonText = null;
        [MVControlReference("XpSinceReset")]
        private IStaticText XpSinceResetText = null;
        [MVControlReference("XpPerHour")]
        private IStaticText XpPerHourText = null;
        [MVControlReference("XpLast5")]
        private IStaticText XpLast5Text = null;
        [MVControlReference("LoginTime")]
        private IStaticText TimeLoggedInText = null;
        [MVControlReference("TimeSinceReset")]
        private IStaticText TimeSinceResetText = null;
        [MVControlReference("TimeToNextLevel")]
        private IStaticText TimeToNextLevelText = null;

        protected override void Startup()
        {
            try
            {
                Utility = new Utility(this, Host, Core, PluginName);
                //Initialize the view.
                MVWireupHelper.WireupStart(this, Host);
            }
            catch (Exception ex) { Utility.LogError(ex); }
        }

        protected override void Shutdown()
        {
            try
            {
                if (LogoffType != LogoffEventType.Authorized) Crash.Notify(Utility.CharacterName);
                //Destroy the view.
                MVWireupHelper.WireupEnd(this);
            }
            catch (Exception ex) { Utility.LogError(ex); }
        }

        [BaseEvent("LoginComplete", "CharacterFilter")]
        private void LoginComplete(object sender, EventArgs e)
        {
            try
            {
                Utility.CharacterName = Core.CharacterFilter.Name;
                StartXP();
                LoadSettings();
            }
            catch (Exception ex) { Utility.LogError(ex); }
        }

        [BaseEvent("Logoff", "CharacterFilter")]
        private void CharacterFilter_Logoff(object sender, LogoffEventArgs e)
        {
            try
            {
                LogoffType = e.Type;
                Core.ChatBoxMessage -= new EventHandler<ChatTextInterceptEventArgs>(AutoFellow_ChatBoxMessage_Watcher);
                Core.ChatBoxMessage -= new EventHandler<ChatTextInterceptEventArgs>(AutoResponder_ChatBoxMessage_Watcher);
            }
            catch (Exception ex) { Utility.LogError(ex); }
        }

        private void LoadSettings()
        {
            try
            {
                Utility.LoadCharacterSettings();
                if (!Utility.SecretPassword.Equals(""))
                {
                    SecretPasswordTextBox.Text = Utility.SecretPassword;
                }
                if (Utility.AutoFellow.Equals("True"))
                {
                    AutoFellowCheckBox.Checked = true;
                    Core.ChatBoxMessage += new EventHandler<ChatTextInterceptEventArgs>(AutoFellow_ChatBoxMessage_Watcher);
                }
                if (Utility.AutoResponder.Equals("True"))
                {
                    AutoRespondCheckBox.Checked = true;
                    Core.ChatBoxMessage += new EventHandler<ChatTextInterceptEventArgs>(AutoResponder_ChatBoxMessage_Watcher);
                }
            }
            catch (Exception)
            {
            }
        }

        private void StartXP()
        {
            ExpTracker = new ExpTracker(Core);

            #region Do Only Once
            XpAtLogonText.Text = String.Format("{0:n0}", ExpTracker.TotalXpAtLogon);
            #endregion

            #region ExpTracker Subscriptions
            ExpTracker.RaiseXpPerHour += Update_XpPerHour;
            ExpTracker.RaiseXpLast5 += Update_XpLast5;
            ExpTracker.RaiseXpEarnedSinceLogon += Update_XpEarnedSinceLogon;
            ExpTracker.RaiseXpEarnedSinceReset += Update_XpEarnedSinceReset;
            ExpTracker.RaiseTimeLoggedIn += Update_TimeLoggedIn;
            ExpTracker.RaiseTimeSinceReset += Update_TimeSinceReset;
            ExpTracker.RaiseTimeToLevel += Update_TimeToLevel;
            #endregion
        }

        private void Update_XpPerHour(object sender, XpEventArgs e)
        {
            XpPerHourText.Text = String.Format("{0:n0}", e.Value);
        }

        private void Update_XpLast5(object sender, XpEventArgs e)
        {
            XpLast5Text.Text = String.Format("{0:n0}", e.Value);
        }

        private void Update_XpEarnedSinceLogon(object sender, XpEventArgs e)
        {
            XpSinceLogonText.Text = String.Format("{0:n0}", e.Value);
        }

        private void Update_XpEarnedSinceReset(object sender, XpEventArgs e)
        {
            XpSinceResetText.Text = String.Format("{0:n0}", e.Value);
        }

        private void Update_TimeLoggedIn(object sender, XpEventArgs e)
        {
            TimeSpan t = TimeSpan.FromSeconds(e.Value);
            TimeLoggedInText.Text = String.Format("{0:D2}d {1:D2}h {2:D2}m {3:d2}s", t.Days, t.Hours, t.Minutes, t.Seconds);
        }

        private void Update_TimeSinceReset(object sender, XpEventArgs e)
        {
            TimeSpan t = TimeSpan.FromSeconds(e.Value);
            TimeSinceResetText.Text = String.Format("{0:D2}d {1:D2}h {2:D2}m {3:D2}s", t.Days, t.Hours, t.Minutes, t.Seconds);
        }

        private void Update_TimeToLevel(object sender, XpEventArgs e)
        {
            TimeSpan t = TimeSpan.FromSeconds(e.Value);
            if (e.Value > 0)
            {
                TimeToNextLevelText.Text = String.Format("{0:D2}d {1:D2}h {2:D2}m {3:D2}s", t.Days, t.Hours, t.Minutes, t.Seconds);
            }
        }

        [MVControlEvent("SecretPassword", "Change")]
        void SecretPassword_Change(object sender, MVTextBoxChangeEventArgs e)
        {
            try
            {
                SecretPasswordChanged(new ConfigEventArgs(e.Text, SecretPasswordTextBox.Name, Module));
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        [MVControlEvent("AutoRespond", "Change")]
        void AutoRespond_Change(object sender, MVCheckBoxChangeEventArgs e)
        {
            try
            {
                AutoResponderChanged(new ConfigEventArgs(e.Checked.ToString(), AutoRespondCheckBox.Name, Module));
                if (e.Checked)
                {
                    Core.ChatBoxMessage += new EventHandler<ChatTextInterceptEventArgs>(AutoResponder_ChatBoxMessage_Watcher);
                    Utility.WriteToChat("To get your current component counts from another character, simply /tell 'comps' to this character.");
                }
                else
                {
                    Core.ChatBoxMessage -= new EventHandler<ChatTextInterceptEventArgs>(AutoResponder_ChatBoxMessage_Watcher);
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        void AutoResponder_ChatBoxMessage_Watcher(object sender, ChatTextInterceptEventArgs e)
        {
            string sanitizedInput = Regex.Replace(e.Text, @"[^\w:/ ']", string.Empty);
            AutoRespondParser(sanitizedInput);
        }

        void AutoRespondParser(string input)
        {
            Match match;
            string singleCompResponse = "/t {0}, I currently have {1} {2}.";
            string pluralCompResponse = "/t {0}, I currently have {1} {2}s.";

            // checking spell components
            string componentsPattern = string.Format(@"(?<guid>\d+):(?<dupleName>.+?)Tell\s(?<msg>tells)\syou\s(?<secret>.*)");

            match = new Regex(componentsPattern).Match(input);
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
                        Host.Actions.InvokeChatParser(collection.Quantity == 1 ? string.Format(singleCompResponse, name, collection.Quantity.ToString(), collection.First.Name) : string.Format(pluralCompResponse, name, collection.Quantity.ToString(), collection.First.Name));
                    }
                }
                else
                {
                    WorldObjectCollection collection = Core.WorldFilter.GetInventory();
                    collection.SetFilter(new ByNameFilter(match.Groups["secret"].Value));
                    Host.Actions.InvokeChatParser(collection.Quantity == 1 ? string.Format(singleCompResponse, name, collection.Quantity.ToString(), collection.First.Name) : string.Format(pluralCompResponse, name, collection.Quantity.ToString(), collection.First.Name));
                }

            }


        }

        [BaseEvent("ChangeFellowship", "CharacterFilter")]
        private void ChangeFellowship(object sender, ChangeFellowshipEventArgs e)
        {
            if(e.Type == FellowshipEventType.Create)
            {
                inFellow = true;
            }
            else if (e.Type == FellowshipEventType.Quit)
            {
                inFellow = false;
            }
        }

        [MVControlEvent("AutoFellow", "Change")]
        void AutoFellow_Change(object sender, MVCheckBoxChangeEventArgs e)
        {
            try
            {
                AutoFellowChanged(new ConfigEventArgs(e.Checked.ToString(), AutoFellowCheckBox.Name, Module));
                if (e.Checked)
                {
                    Core.ChatBoxMessage += new EventHandler<ChatTextInterceptEventArgs>(AutoFellow_ChatBoxMessage_Watcher);
                    Host.Actions.FellowshipSetOpen(true);
                }
                else
                {
                    Core.ChatBoxMessage -= new EventHandler<ChatTextInterceptEventArgs>(AutoFellow_ChatBoxMessage_Watcher);
                }
            }
            catch (Exception ex) { Utility.LogError(ex); }
        }

        void AutoFellow_ChatBoxMessage_Watcher(object sender, ChatTextInterceptEventArgs e)
        {
            string sanitizedInput = Regex.Replace(e.Text, @"[^\w:/ ']", string.Empty);
            AutoFellowParser(sanitizedInput);
        }

        void AutoFellowParser(string input)
        {
            Match match;

            Dictionary<string, string> regexStrings = new Dictionary<string, string> {
                ["CreatedFellow"] = @"^You have created the Fellowship",
                ["NotAccepting"] = @"(?<name>.+?) is not accepting fellowing requests",
                ["JoinedFellow"] = @"(?<name>.+?) joined the fellowship",
                ["ElseLeftFellow"] = @"(?<name>.+?) left the fellowship",
                ["RequestFellow"] = string.Format(@"(?<guid>\d+):(?<dupleName>.+?)Tell\s(?<msg>tells)\syou\s(?<secret>{0}$)", Utility.SecretPassword),
                ["AlreadyFellow"] = @"(?<name>.+?) is already in a fellowship"
            };

            foreach (var item in regexStrings)
            {
                match = new Regex(item.Value).Match(input);
                if (match.Success)
                {
                    switch (item.Key)
                    {
                        case "CreatedFellow": // You have created the Fellowship
                            Host.Actions.FellowshipSetOpen(true);
                            break;
                        case "NotAccepting": // Target is not accepting fellowship requests
                            Host.Actions.InvokeChatParser(string.Format("/t {0}, <{1}> You are not accepting fellowship requests!", match.Groups["name"].Value, PluginName));
                            break;
                        case "JoinedFellow": // Target joins the fellowship
                            break;
                        case "ElseLeftFellow": // Someone leaves the fellowship
                            break;
                        case "RequestFellow": // Someone sends you a tell, checking for secret password
                            if (!inFellow)
                            {
                                Host.Actions.InvokeChatParser(string.Format("/r I'm not currently in a fellowship."));
                            }
                            else if (match.Groups["msg"].Value.Equals("tells") && match.Groups["secret"].Value.Equals(Utility.SecretPassword))
                            {
                                targetRecruit = match.Groups["dupleName"].Value.Substring(0, match.Groups["dupleName"].Value.Length / 2);
                                targetGuid = Int32.Parse(match.Groups["guid"].Value);
                                Host.Actions.InvokeChatParser(string.Format("/t {0}, <{1}> Please stand near me, I'm going to try and recruit you into the fellowship.", targetRecruit, PluginName));
                                RecruitTarget(targetGuid);
                            }
                            break;
                        case "AlreadyFellow": // Target is already in a fellowship
                            Host.Actions.InvokeChatParser(String.Format("/t {0}, <{1}>You're already in a fellowship.", match.Groups["name"].Value, PluginName));
                            break;
                    }
                    break;
                }
            }
        }

        private void RecruitTarget(int guid)
        {
            Host.Actions.FellowshipRecruit(guid);
        }

        [MVControlEvent("XpReset", "Click")]
        void XpReset_Clicked(object sender, MVControlEventArgs e)
        {
            ExpTracker.Reset();
            XpLast5Text.Text = "0";
            XpPerHourText.Text = "0";
            XpSinceResetText.Text = "0";
            TimeToNextLevelText.Text = "";
            TimeSinceResetText.Text = String.Format("{0:D2}h {1:D2}m {2:D2}s", 0, 0, 0);
        }

        [MVControlEvent("XpFellow", "Click")]
        void XpFellow_Clicked(object sender, MVControlEventArgs e)
        {
            ReportXp("/f");
        }

        [MVControlEvent("XpAlleg", "Click")]
        void XpAlleg_Clicked(object sender, MVControlEventArgs e)
        {
            ReportXp("/a");
        }

        private void ReportXp(string targetChat)
        {
            Host.Actions.InvokeChatParser(
                String.Format("{0} You have earned {1} XP in {2} for {3} XP/hour ({4} XP in the last 5 minutes). At this rate, you'll hit your next level in {5}.",
                targetChat,
                String.Format("{0:n0}", ExpTracker.XpEarnedSinceReset),
                String.Format("{0:D2}d {1:D2}h {2:D2}m {3:D2}s",
                    ExpTracker.TimeSinceReset.Days,
                    ExpTracker.TimeSinceReset.Hours,
                    ExpTracker.TimeSinceReset.Minutes,
                    ExpTracker.TimeSinceReset.Seconds),
                String.Format("{0:n0}", ExpTracker.XpPerHourLong),
                String.Format("{0:n0}", ExpTracker.XpLast5Long),
                String.Format("{0:D2}d {1:D2}h {2:D2}m {3:D2}s",
                    ExpTracker.TimeLeftToLevel.Days,
                    ExpTracker.TimeLeftToLevel.Hours,
                    ExpTracker.TimeLeftToLevel.Minutes,
                    ExpTracker.TimeLeftToLevel.Seconds)));
        }

        //[MVControlEvent("Debug", "Click")]
        //void Debug_Clicked(object sender, MVControlEventArgs e)
        //{
        //    Utility.SaveSetting();
        //}

        protected virtual void SecretPasswordChanged(ConfigEventArgs e)
        {
            RaiseSecretPasswordEvent?.Invoke(this, e);
        }

        protected virtual void AutoFellowChanged(ConfigEventArgs e)
        {
            RaiseAutoFellowEvent?.Invoke(this, e);
        }

        protected virtual void AutoResponderChanged(ConfigEventArgs e)
        {
            RaiseAutoResponderEvent?.Invoke(this, e);
        }
    }

    public class ConfigEventArgs : EventArgs
    {
        public string Module { get; set; }
        public string Setting { get; set; }
        public string Value { get; set; }

        public ConfigEventArgs(string value, string setting, string module)
        {
            Value = value;
            Setting = setting;
            Module = module;
        }

    }
}
