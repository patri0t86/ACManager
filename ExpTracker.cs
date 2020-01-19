using System;
using System.Timers;
using System.Collections.Generic;

using Decal.Adapter;

namespace FellowshipManager.XPTracker
{
    public class ExpTracker
    {
        private CoreManager Core;
        private Timer CalcXpTimer;
        private List<long> Rolling5Min;
        private DateTime Now, LoginTime, LastResetTime;

        public long XpAtReset { get; private set; }
        public long TotalXpAtLogon { get; private set; }
        public long XpEarnedSinceLogin { get; private set; }
        public long XpEarnedSinceReset { get; private set; }
        public long XpPerHourLong { get; private set; }
        public long XpLast5Long { get; private set; }
        public TimeSpan TimeLeftToLevel { get; private set; }
        public TimeSpan TimeLoggedIn { get; private set; }
        public TimeSpan TimeSinceReset { get; private set; }

        public ExpTracker (CoreManager core)
        {
            Core = core;
            Rolling5Min = new List<long>();
            LoginTime = DateTime.Now;
            StartTracking();
        }

        public void Reset()
        {
            Rolling5Min.Clear();
            XpAtReset = Core.CharacterFilter.TotalXP;
            XpPerHourLong = 0;
            XpLast5Long = 0;
            XpEarnedSinceReset = 0;
            LastResetTime = DateTime.Now;
            CalcXpTimer.Close();
            CalcXpTimer = CreateTimer(1000);
        }

        private void StartTracking()
        {
            TotalXpAtLogon = XpAtReset = Core.CharacterFilter.TotalXP;
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
            Now = DateTime.Now;
            TimeLoggedIn = Now - LoginTime;
            TimeSinceReset = Now - LastResetTime;
            XpEarnedSinceLogin = Core.CharacterFilter.TotalXP - TotalXpAtLogon;
            XpEarnedSinceReset = Core.CharacterFilter.TotalXP - XpAtReset;
            Rolling5Min.Add(Core.CharacterFilter.TotalXP);
            if (Rolling5Min.Count > 300) Rolling5Min.RemoveAt(0);
            XpPerHourLong = XpPerHour();
            XpLast5Long = XpLast5();
            TimeLeftToLevel = TimeToLevel();
        }

        private long XpPerHour()
        {
            try
            {
                return XpEarnedSinceReset / (long)TimeSinceReset.TotalSeconds * 3600;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        private long XpLast5()
        {
            try
            {
                return (Core.CharacterFilter.TotalXP - Rolling5Min[0]) / Rolling5Min.Count * 3600;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private TimeSpan TimeToLevel()
        {
            try
            {
                return TimeSpan.FromSeconds((double)Core.CharacterFilter.XPToNextLevel / XpLast5Long * 3600);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
