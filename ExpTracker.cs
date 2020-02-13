using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;
using System.Timers;

namespace ACManager
{
    public class ExpTracker
    {
        private PluginHost Host;
        private CoreManager Core;
        private Timer CalcXpTimer;
        private List<long> Rolling5Min;
        private DateTime LoginTime, LastResetTime;

        public EventHandler<XpEventArgs> RaiseXpPerHour;
        public EventHandler<XpEventArgs> RaiseXpLast5;
        public EventHandler<XpEventArgs> RaiseXpEarnedSinceLogon;
        public EventHandler<XpEventArgs> RaiseXpEarnedSinceReset;
        public EventHandler<XpEventArgs> RaiseTimeLoggedIn;
        public EventHandler<XpEventArgs> RaiseTimeSinceReset;
        public EventHandler<XpEventArgs> RaiseTimeToLevel;

        public long XpAtReset { get; private set; }
        public long TotalXpAtLogon { get; private set; }
        public long XpEarnedSinceLogin { get; private set; }
        public long XpEarnedSinceReset { get; private set; }
        public long XpPerHourLong { get; private set; }
        public long XpLast5Long { get; private set; }
        public TimeSpan TimeLeftToLevel { get; private set; }
        public TimeSpan TimeSinceReset { get; private set; }

        public ExpTracker(PluginHost host, CoreManager core)
        {
            Host = host;
            Core = core;
            Rolling5Min = new List<long>();
            LoginTime = DateTime.Now;
            StartTracking();
        }

        public void Report(string targetChat)
        {
            string xpSinceReset = String.Format("{0:n0}", XpEarnedSinceReset);
            string timeSinceReset = String.Format("{0:D2}d {1:D2}h {2:D2}m {3:D2}s",
                TimeSinceReset.Days,
                TimeSinceReset.Hours,
                TimeSinceReset.Minutes,
                TimeSinceReset.Seconds);
            string xpPerHour = String.Format("{0:n0}", XpPerHourLong);
            string xpPer5 = String.Format("{0:n0}", XpLast5Long);
            string timeToLevel = String.Format("{0:D2}d {1:D2}h {2:D2}m {3:D2}s",
                    TimeLeftToLevel.Days,
                    TimeLeftToLevel.Hours,
                    TimeLeftToLevel.Minutes,
                    TimeLeftToLevel.Seconds);
            Host.Actions.InvokeChatParser(String.Format("{0} You have earned {1} XP in {2} for {3} XP/hour ({4} XP in the last 5 minutes). At this rate, you'll hit your next level in {5}.",
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
            LastResetTime = DateTime.Now;
            CalcXpTimer.Close();
            CalcXpTimer = CreateTimer(1000);
        }

        private void StartTracking()
        {
            #region Do_Only_Once
            TotalXpAtLogon = XpAtReset = Core.CharacterFilter.TotalXP;
            LastResetTime = DateTime.Now;
            #endregion

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
            XpPerHourEvent(new XpEventArgs(XpPerHourLong));

            Rolling5Min.Add(Core.CharacterFilter.TotalXP);
            if (Rolling5Min.Count > 300) Rolling5Min.RemoveAt(0);

            XpLast5Long = (Core.CharacterFilter.TotalXP - Rolling5Min[0]) / Rolling5Min.Count * 3600;
            XpLast5Event(new XpEventArgs(XpLast5Long));

            XpEarnedSinceLogonEvent(new XpEventArgs(Core.CharacterFilter.TotalXP - TotalXpAtLogon));

            XpEarnedSinceResetEvent(new XpEventArgs(XpEarnedSinceReset));
            #endregion

            #region Time Event Triggers
            TimeLoggedInEvent(new XpEventArgs((long)(Now - LoginTime).TotalSeconds));

            TimeSinceResetEvent(new XpEventArgs((long)TimeSinceReset.TotalSeconds));

            TimeLeftToLevel = TimeSpan.FromSeconds((double)Core.CharacterFilter.XPToNextLevel / XpLast5Long * 3600);
            TimeToLevelEvent(new XpEventArgs((long)TimeLeftToLevel.TotalSeconds));
            #endregion
        }

        protected virtual void XpPerHourEvent(XpEventArgs e)
        {
            RaiseXpPerHour?.Invoke(this, e);
        }

        protected virtual void XpLast5Event(XpEventArgs e)
        {
            RaiseXpLast5?.Invoke(this, e);
        }

        protected virtual void XpEarnedSinceLogonEvent(XpEventArgs e)
        {
            RaiseXpEarnedSinceLogon?.Invoke(this, e);
        }

        protected virtual void XpEarnedSinceResetEvent(XpEventArgs e)
        {
            RaiseXpEarnedSinceReset?.Invoke(this, e);
        }

        protected virtual void TimeLoggedInEvent(XpEventArgs e)
        {
            RaiseTimeLoggedIn?.Invoke(this, e);
        }

        protected virtual void TimeSinceResetEvent(XpEventArgs e)
        {
            RaiseTimeSinceReset?.Invoke(this, e);
        }

        protected virtual void TimeToLevelEvent(XpEventArgs e)
        {
            RaiseTimeToLevel?.Invoke(this, e);
        }

    }

    public class XpEventArgs : EventArgs
    {
        private long value;
        public XpEventArgs(long s)
        {
            value = s;
        }
        public long Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }
}
