using VirindiViewService;
using VirindiViewService.Controls;
using System;

namespace ACManager.Views
{
    internal class ExpTrackerView : IDisposable
    {
        internal PluginCore Plugin { get; set; }
        internal HudView View { get; set; }
        internal HudStaticText XpAtLogonText { get; set; }
        internal HudStaticText XpSinceLogonText {get; set; }
        internal HudStaticText XpSinceResetText {get; set; }
        internal HudStaticText XpPerHourText {get;set;}
        internal HudStaticText XpLast5Text {get;set;}
        internal HudStaticText TimeLoggedInText {get;set;}
        internal HudStaticText TimeSinceResetText {get;set;}
        internal HudStaticText TimeToNextLevelText {get;set;}
        internal HudButton XpReset {get;set;}
        internal HudButton XpFellow {get;set;}
        internal HudButton XpAlleg {get;set;}
        internal HudButton XpLocal { get; set; }

        public ExpTrackerView(PluginCore parent)
        {
            try
            {
                Plugin = parent;

                VirindiViewService.XMLParsers.Decal3XMLParser parser = new VirindiViewService.XMLParsers.Decal3XMLParser();
                parser.ParseFromResource("ACManager.Views.expTrackerView.xml", out ViewProperties Properties, out ControlGroup Controls);

                View = new HudView(Properties, Controls);
                View.ShowInBar = false;

                XpAtLogonText = View != null ? (HudStaticText)View["XpAtLogon"] : new HudStaticText();
                XpSinceLogonText = View != null ? (HudStaticText)View["XpSinceLogon"] : new HudStaticText();
                XpSinceResetText = View != null ? (HudStaticText)View["XpSinceReset"] : new HudStaticText();
                XpPerHourText = View != null ? (HudStaticText)View["XpPerHour"] : new HudStaticText();
                XpLast5Text = View != null ? (HudStaticText)View["XpLast5"] : new HudStaticText();
                TimeLoggedInText = View != null ? (HudStaticText)View["LoginTime"] : new HudStaticText();
                TimeSinceResetText = View != null ? (HudStaticText)View["TimeSinceReset"] : new HudStaticText();
                TimeToNextLevelText = View != null ? (HudStaticText)View["TimeToNextLevel"] : new HudStaticText();

                XpReset = View != null ? (HudButton)View["XpReset"] : new HudButton();
                XpReset.Hit += XpReset_Hit;

                XpFellow = View != null ? (HudButton)View["XpFellow"] : new HudButton();
                XpFellow.Hit += XpFellow_Hit;

                XpAlleg = View != null ? (HudButton)View["XpAlleg"] : new HudButton();
                XpAlleg.Hit += XpAlleg_Hit;

                XpLocal = View != null ? (HudButton)View["XpLocal"] : new HudButton();
                XpLocal.Hit += XpLocal_Hit;
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void XpReset_Hit(object sender, EventArgs e)
        {
            try
            {
                Plugin.ExpTracker.Reset();
                XpLast5Text.Text = "0";
                XpPerHourText.Text = "0";
                XpSinceResetText.Text = "0";
                TimeToNextLevelText.Text = string.Format("{0:D2}d {1:D2}h {2:D2}m {3:D2}s", 0, 0, 0, 0);
                TimeSinceResetText.Text = string.Format("{0:D2}d {1:D2}h {2:D2}m {3:D2}s", 0, 0, 0, 0);
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void XpFellow_Hit(object sender, EventArgs e)
        {
            try
            {
                Plugin.ExpTracker.Report("/f");
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void XpAlleg_Hit(object sender, EventArgs e)
        {
            try
            {
                Plugin.ExpTracker.Report("/a");
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void XpLocal_Hit(object sender, EventArgs e)
        {
            try
            {
                Plugin.ExpTracker.Report();
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Plugin = null;
                    XpReset.Hit -= XpReset_Hit;
                    XpFellow.Hit -= XpFellow_Hit;
                    XpAlleg.Hit -= XpAlleg_Hit;
                    XpLocal.Hit -= XpLocal_Hit;
                    if (View != null) View.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
