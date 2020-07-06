using ACManager.Views;
using Decal.Adapter;
using System;
using System.Collections.Generic;
using System.Timers;

namespace ACManager
{
    internal class ExpTracker : IDisposable
    {
        private FilterCore Filter { get; set; }
        internal CoreManager Core { get; set; }
        internal ExpTrackerView ExpTrackerView { get; set; }
        internal bool ViewVisible { get; set; } = true;
        private Timer CalcXpTimer { get; set; }
        private List<long> Rolling5Min { get; set; }
        private DateTime LoginTime { get; set; }
        private DateTime LastResetTime { get; set; }
        private bool DisposedValue { get; set; }
        public long XpAtReset { get; private set; }
        public long TotalXpAtLogon { get; private set; }
        public long XpEarnedSinceLogin { get; private set; }
        public long XpEarnedSinceReset { get; private set; }
        public long XpPerHourLong { get; private set; }
        public long XpLast5Long { get; private set; }
        public TimeSpan TimeLeftToLevel { get; private set; }
        public TimeSpan TimeSinceReset { get; private set; }

        public ExpTracker(FilterCore parent, CoreManager core)
        {
            Filter = parent;
            Core = core;

            LoadSettings();

            ExpTrackerView = new ExpTrackerView(this);
            Rolling5Min = new List<long>();
            LoginTime = DateTime.Now;
            StartTracking();
        }

        private void LoadSettings()
        {
            ViewVisible = Filter.Machine.Utility.GUISettings.ExpTrackerVisible;
        }

        public void ToggleView(bool isVisible)
        {
            ExpTrackerView.View.ShowInBar = isVisible;
        }

        public void Report()
        {
            string xpSinceReset = string.Format("{0:n0}", XpEarnedSinceReset);
            string timeSinceReset = string.Format("{0:D2}d {1:D2}h {2:D2}m {3:D2}s",
                TimeSinceReset.Days,
                TimeSinceReset.Hours,
                TimeSinceReset.Minutes,
                TimeSinceReset.Seconds);
            string xpPerHour = string.Format("{0:n0}", XpPerHourLong);
            string xpPer5 = string.Format("{0:n0}", XpLast5Long);
            string timeToLevel = string.Format("{0:D2}d {1:D2}h {2:D2}m {3:D2}s",
                    TimeLeftToLevel.Days,
                    TimeLeftToLevel.Hours,
                    TimeLeftToLevel.Minutes,
                    TimeLeftToLevel.Seconds);
            Debug.ToChat(string.Format("You have earned {0} XP in {1} for {2} XP/hour ({3} XP/hr in the last 5 minutes). At this rate, you'll hit your next level in {4}.",
                xpSinceReset,
                timeSinceReset,
                xpPerHour,
                xpPer5,
                timeToLevel)
            );
        }

        public void Report(string targetChat)
        {
            string xpSinceReset = string.Format("{0:n0}", XpEarnedSinceReset);
            string timeSinceReset = string.Format("{0:D2}d {1:D2}h {2:D2}m {3:D2}s",
                TimeSinceReset.Days,
                TimeSinceReset.Hours,
                TimeSinceReset.Minutes,
                TimeSinceReset.Seconds);
            string xpPerHour = string.Format("{0:n0}", XpPerHourLong);
            string xpPer5 = string.Format("{0:n0}", XpLast5Long);
            string timeToLevel = string.Format("{0:D2}d {1:D2}h {2:D2}m {3:D2}s",
                    TimeLeftToLevel.Days,
                    TimeLeftToLevel.Hours,
                    TimeLeftToLevel.Minutes,
                    TimeLeftToLevel.Seconds);
            Core.Actions.InvokeChatParser(string.Format("{0} You have earned {1} XP in {2} for {3} XP/hour ({4} XP/hr in the last 5 minutes). At this rate, you'll hit your next level in {5}.",
                targetChat,
                xpSinceReset,
                timeSinceReset,
                xpPerHour,
                xpPer5,
                timeToLevel)
            );
        }

        public void Reset()
        {
            Rolling5Min.Clear();
            XpAtReset = Core.CharacterFilter.TotalXP;
            XpLast5Long = 0;
            XpEarnedSinceReset = 0;
            TimeLeftToLevel = TimeSpan.FromSeconds(0);
            LastResetTime = DateTime.Now;
            CalcXpTimer.Close();
            CalcXpTimer = CreateTimer(1000);
        }

        private void StartTracking()
        {
            TotalXpAtLogon = XpAtReset = Core.CharacterFilter.TotalXP;
            ExpTrackerView.XpAtLogonText.Text = string.Format("{0:n0}", Core.CharacterFilter.TotalXP);
            LastResetTime = DateTime.Now;

            CalcXpTimer = CreateTimer(1000);
        }

        private Timer CreateTimer(int interval)
        {
            Timer timer = new Timer(interval);
            timer.Elapsed += UpdateXpOnInterval;
            timer.AutoReset = true;
            timer.Enabled = true;
            return timer;
        }

        private void UpdateXpOnInterval(object sender, ElapsedEventArgs e)
        {
            DateTime Now = DateTime.Now;
            TimeSinceReset = (Now - LastResetTime);
            XpEarnedSinceReset = Core.CharacterFilter.TotalXP - XpAtReset;

            #region XP Event Triggers
            XpPerHourLong = XpEarnedSinceReset / (long)TimeSinceReset.TotalSeconds * 3600;
            ExpTrackerView.XpPerHourText.Text = string.Format("{0:n0}", XpPerHourLong);

            Rolling5Min.Add(Core.CharacterFilter.TotalXP);
            if (Rolling5Min.Count > 300) Rolling5Min.RemoveAt(0);

            XpLast5Long = (Core.CharacterFilter.TotalXP - Rolling5Min[0]) / Rolling5Min.Count * 3600;
            ExpTrackerView.XpLast5Text.Text = string.Format("{0:n0}", XpLast5Long);

            ExpTrackerView.XpSinceLogonText.Text = string.Format("{0:n0}", Core.CharacterFilter.TotalXP - TotalXpAtLogon);

            ExpTrackerView.XpSinceResetText.Text = string.Format("{0:n0}", XpEarnedSinceReset);
            #endregion

            #region Time Event Triggers
            TimeSpan t = TimeSpan.FromSeconds((long)(Now - LoginTime).TotalSeconds);
            ExpTrackerView.TimeLoggedInText.Text = string.Format("{0:D2}d {1:D2}h {2:D2}m {3:d2}s", t.Days, t.Hours, t.Minutes, t.Seconds);

            t = TimeSpan.FromSeconds((long)TimeSinceReset.TotalSeconds);
            ExpTrackerView.TimeSinceResetText.Text = string.Format("{0:D2}d {1:D2}h {2:D2}m {3:d2}s", t.Days, t.Hours, t.Minutes, t.Seconds);

            TimeLeftToLevel = TimeSpan.FromSeconds((double)Core.CharacterFilter.XPToNextLevel / XpLast5Long * 3600);
            t = TimeSpan.FromSeconds((long)TimeLeftToLevel.TotalSeconds);
            ExpTrackerView.TimeToNextLevelText.Text = string.Format("{0:D2}d {1:D2}h {2:D2}m {3:d2}s", t.Days, t.Hours, t.Minutes, t.Seconds);
            #endregion
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    ExpTrackerView?.Dispose();
                    CalcXpTimer?.Close();
                }
                DisposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
