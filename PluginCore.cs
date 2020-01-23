using System;
using System.Text.RegularExpressions;
using System.Timers;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using MyClasses.MetaViewWrappers;

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
        private readonly string PluginName = "Fellowship Manager";
        private ExpTracker ExpTracker;
        private Utility Utility;
        private string targetRecruit;
        private int targetGuid;
        private bool properLogoff = false;

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
                if(!properLogoff) Crash.Notify(Core.CharacterFilter.Name);
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
                StartXP();
                Utility.LoadSettingsFromFile();
                LoadSettings();
            }
            catch (Exception ex) { Utility.LogError(ex); }
        }

        [BaseEvent("Logoff", "CharacterFilter")]
        private void CharacterFilter_Logoff(object sender, LogoffEventArgs e)
        {
            try
            {
                properLogoff = true;
                Core.ChatBoxMessage -= new EventHandler<ChatTextInterceptEventArgs>(AutoFellow_ChatBoxMessage_Watcher);
                Core.ChatBoxMessage -= new EventHandler<ChatTextInterceptEventArgs>(AutoResponder_ChatBoxMessage_Watcher);
                SecretPasswordChanged(new ConfigEventArgs(SecretPasswordTextBox.Text));
                AutoResponderChanged(new ConfigEventArgs(AutoRespondCheckBox.Checked.ToString()));
                AutoFellowChanged(new ConfigEventArgs(AutoFellowCheckBox.Checked.ToString()));
                Utility.SaveSettings();
            }
            catch (Exception ex) { Utility.LogError(ex); }
        }

        private void LoadSettings()
        {
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
            TimeLoggedInText.Text = String.Format("{0:D2}h {1:D2}m {2:d2}s", t.Hours, t.Minutes, t.Seconds);
        }

        private void Update_TimeSinceReset(object sender, XpEventArgs e)
        {
            TimeSpan t = TimeSpan.FromSeconds(e.Value);
            TimeSinceResetText.Text = String.Format("{0:D2}h {1:D2}m {2:D2}s", t.Hours, t.Minutes, t.Seconds);
        }

        private void Update_TimeToLevel(object sender, XpEventArgs e)
        {
            TimeSpan t = TimeSpan.FromSeconds(e.Value);
            if (e.Value > 0)
            {
                TimeToNextLevelText.Text = String.Format("{0:D2}h {1:D2}m {2:D2}s", t.Hours, t.Minutes, t.Seconds);
            }
        }

        [MVControlEvent("SecretPassword", "Change")]
        void SecretPassword_Change(object sender, MVTextBoxChangeEventArgs e)
        {
            try
            {
                SecretPasswordChanged(new ConfigEventArgs(SecretPasswordTextBox.Text));
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
                AutoResponderChanged(new ConfigEventArgs(e.Checked.ToString()));
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
            string componentsPattern = string.Format(@"(?<guid>\d+):(?<dupleName>.+?)Tell\s(?<msg>tells).+?(?<secret>comps)");

            match = new Regex(componentsPattern).Match(input);
            if (match.Success && match.Groups["secret"].Value.Equals("comps"))
            {
                string[] s = {
                    "Platinum Scarab",
                    "Mana Scarab",
                    "Prismatic Taper"
                };
                string name = match.Groups["dupleName"].Value.Substring(0, match.Groups["dupleName"].Value.Length / 2);
                foreach (string comp in s)
                {
                    WorldObjectCollection collection = Core.WorldFilter.GetInventory();
                    collection.SetFilter(new ByNameFilter(comp));
                    if (collection.Quantity == 0) continue;
                    Host.Actions.InvokeChatParser(collection.Quantity == 1 ? string.Format(singleCompResponse, name, collection.Quantity.ToString(), collection.First.Name) : string.Format(pluralCompResponse, name, collection.Quantity.ToString(), collection.First.Name));
                }
            }
        }

        [MVControlEvent("AutoFellow", "Change")]
        void AutoFellow_Change(object sender, MVCheckBoxChangeEventArgs e)
        {
            try
            {
                AutoFellowChanged(new ConfigEventArgs(e.Checked.ToString()));
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
            Regex regex;
            Match match;

            // make the regex patterns an array to iterate over?

            // Not accepting fellowship requests
            string notAcceptingPattern = @"(?<name>.+?) is not accepting fellowing requests";
            regex = new Regex(notAcceptingPattern);
            match = regex.Match(input);
            if (match.Success)
            {
                Host.Actions.InvokeChatParser(string.Format("/t {0}, <{1}> You are not accepting fellowship requests!", match.Groups["name"].Value, PluginName));
                return;
            }

            // Someone joins the fellowship
            string joinedFellowshipPattern = @"(?<name>.+?) joined the fellowship";
            regex = new Regex(joinedFellowshipPattern);
            match = regex.Match(input);
            if (match.Success)
            {
                // do something when someone joins if you want to
            }

            // Someone leaves the fellowship
            string leftFellowshipPattern = @"(?<name>.+?) left the fellowship";
            regex = new Regex(leftFellowshipPattern);
            match = regex.Match(input);
            if (match.Success)
            {
                // do something when someone leaves if you want to
            }

            // Someone sends you a tell, checking for secret password
            string fellowshipPattern = string.Format(@"(?<guid>\d+):(?<dupleName>.+?)Tell\s(?<msg>tells).+?(?<secret>{0})", Utility.SecretPassword);
            regex = new Regex(fellowshipPattern);
            match = regex.Match(input);
            if (match.Success)
            {
                if (match.Groups["msg"].Value.Equals("tells") && match.Groups["secret"].Value.Equals(Utility.SecretPassword))
                {
                    targetRecruit = match.Groups["dupleName"].Value.Substring(0, match.Groups["dupleName"].Value.Length / 2);
                    targetGuid = Int32.Parse(match.Groups["guid"].Value);
                    Host.Actions.InvokeChatParser(string.Format("/t {0}, <{1}> Please stand near me, I'm going to try and recruit you into the fellowship.", targetRecruit, PluginName));
                    RecruitTarget(targetGuid);
                    return;
                }
            }

            // Testerstwo is already in a fellowship
            string alreadyFellowedPattern = @"(?<name>.+?) is already in a fellowship";
            regex = new Regex(alreadyFellowedPattern);
            match = regex.Match(input);
            if (match.Success)
            {
                Host.Actions.InvokeChatParser(String.Format("/t {0}, <{1}>You're already in a fellowship.", match.Groups["name"].Value, PluginName));
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
                String.Format("{0}h {1}m {2}s",
                    String.Format("{0:00}", ExpTracker.TimeSinceReset.Hours),
                    String.Format("{0:00}", ExpTracker.TimeSinceReset.Minutes),
                    String.Format("{0:00}", ExpTracker.TimeSinceReset.Seconds)),
                String.Format("{0:n0}", ExpTracker.XpPerHourLong),
                String.Format("{0:n0}", ExpTracker.XpLast5Long),
                String.Format("{0:D2}h {1:D2}m {2:D2}s",
                    ExpTracker.TimeLeftToLevel.Hours,
                    ExpTracker.TimeLeftToLevel.Minutes,
                    ExpTracker.TimeLeftToLevel.Seconds)));
        }

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
        public ConfigEventArgs(string s)
        {
            Value = s;
        }
        public string Value { get; set; }
    }
}
