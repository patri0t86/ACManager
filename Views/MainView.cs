using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using VirindiViewService;
using VirindiViewService.Controls;

namespace ACManager.Views
{
    internal class MainView : IDisposable
    {
        internal PluginCore Plugin { get; set; }
        internal string GeneralModule = "General";
        internal string InventoryModule = "InventoryTracker";
        internal string FellowshipModule = "FellowshipManager";
        private HudView View { get; set; }
        internal HudStaticText Version { get; set; }
        internal HudCheckBox AutoFellow { get; set; }
        internal HudCheckBox AutoRespond { get; set; }
        internal HudCheckBox LowCompLogoff { get; set; }
        internal HudCheckBox AnnounceLogoff { get; set; }
        internal HudCheckBox PortalBotCheckBox { get; set; }
        internal HudCheckBox ExpTrackerCheckBox { get; set; }
        internal HudTextBox Password { get; set; }
        internal HudTextBox LeadScarabText { get; set; }
        internal HudTextBox IronScarabText { get; set; }
        internal HudTextBox CopperScarabText { get; set; }
        internal HudTextBox SilverScarabText { get; set; }
        internal HudTextBox GoldScarabText { get; set; }
        internal HudTextBox PyrealScarabText { get; set; }
        internal HudTextBox PlatinumScarabText { get; set; }
        internal HudTextBox ManaScarabText { get; set; }
        internal HudTextBox TaperText { get; set; }

        public MainView(PluginCore parent)
        {
            try
            {
                Plugin = parent;

                VirindiViewService.XMLParsers.Decal3XMLParser parser = new VirindiViewService.XMLParsers.Decal3XMLParser();
                parser.ParseFromResource("ACManager.Views.mainView.xml", out ViewProperties Properties, out ControlGroup Controls);

                View = new HudView(Properties, Controls);

                Version = View != null ? (HudStaticText)View["Version"] : new HudStaticText();
                Version.Text = $"v{Plugin.Utility.Version}";

                Password = View != null ? (HudTextBox)View["Password"] : new HudTextBox();
                Password.Change += Password_Change;

                AutoFellow = View != null ? (HudCheckBox)View["AutoFellow"] : new HudCheckBox();
                AutoFellow.Change += AutoFellow_Change;

                AutoRespond = View != null ? (HudCheckBox)View["AutoRespond"] : new HudCheckBox();
                AutoRespond.Change += AutoRespond_Change;

                LowCompLogoff = View != null ? (HudCheckBox)View["Components"] : new HudCheckBox();

                AnnounceLogoff = View != null ? (HudCheckBox)View["AnnounceLogoff"] : new HudCheckBox();
                AnnounceLogoff.Change += AnnounceLogoff_Change;

                PortalBotCheckBox = View != null ? (HudCheckBox)View["PortalBot"] : new HudCheckBox();
                PortalBotCheckBox.Change += PortalBotCheckBox_Change;

                ExpTrackerCheckBox = View != null ? (HudCheckBox)View["ExpTracker"] : new HudCheckBox();
                ExpTrackerCheckBox.Change += ExpTrackerCheckBox_Change;

                LeadScarabText = View != null ? (HudTextBox)View["LeadScarabCount"] : new HudTextBox();
                LeadScarabText.Change += LeadScarabText_Change;

                IronScarabText = View != null ? (HudTextBox)View["IronScarabCount"] : new HudTextBox();
                IronScarabText.Change += IronScarabText_Change;

                CopperScarabText = View != null ? (HudTextBox)View["CopperScarabCount"] : new HudTextBox();
                CopperScarabText.Change += CopperScarabText_Change;

                SilverScarabText = View != null ? (HudTextBox)View["SilverScarabCount"] : new HudTextBox();
                SilverScarabText.Change += SilverScarabText_Change;

                GoldScarabText = View != null ? (HudTextBox)View["GoldScarabCount"] : new HudTextBox();
                GoldScarabText.Change += GoldScarabText_Change;

                PyrealScarabText = View != null ? (HudTextBox)View["PyrealScarabCount"] : new HudTextBox();
                PyrealScarabText.Change += PyrealScarabText_Change;

                PlatinumScarabText = View != null ? (HudTextBox)View["PlatinumScarabCount"] : new HudTextBox();
                PlatinumScarabText.Change += PlatinumScarabText_Change;

                ManaScarabText = View != null ? (HudTextBox)View["ManaScarabCount"] : new HudTextBox();
                ManaScarabText.Change += ManaScarabText_Change;

                TaperText = View != null ? (HudTextBox)View["TaperCount"] : new HudTextBox();
                TaperText.Change += TaperText_Change;

            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void Password_Change(object sender, EventArgs e)
        {
            try
            {
                Plugin.CurrentCharacter.Password = Password.Text;
                Plugin.Utility.SaveSettings();
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void AutoFellow_Change(object sender, EventArgs e)
        {
            try
            {
                Plugin.CurrentCharacter.AutoFellow = AutoFellow.Checked;
                Plugin.Utility.SaveSettings();
                if (AutoFellow.Checked && Plugin.FellowshipControl.FellowStatus.Equals(FellowshipEventType.Create))
                {
                    CoreManager.Current.Actions.FellowshipSetOpen(true);
                }
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void AutoRespond_Change(object sender, EventArgs e)
        {
            try
            {
                Plugin.CurrentCharacter.AutoRespond = AutoRespond.Checked;
                Plugin.Utility.SaveSettings();
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void AnnounceLogoff_Change(object sender, EventArgs e)
        {
            try
            {
                Plugin.CurrentCharacter.AnnounceLogoff = AnnounceLogoff.Checked;
                Plugin.Utility.SaveSettings();
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void PortalBotCheckBox_Change(object sender, EventArgs e)
        {
            try
            {
                if (PortalBotCheckBox.Checked)
                {
                    Plugin.PortalBotView.View.ShowInBar = true;
                }
                else
                {
                    Plugin.PortalBotView.View.ShowInBar = false;
                }
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void ExpTrackerCheckBox_Change(object sender, EventArgs e)
        {
            try
            {
                if (ExpTrackerCheckBox.Checked)
                {
                    Plugin.ExpTrackerView.View.ShowInBar = true;
                }
                else
                {
                    Plugin.ExpTrackerView.View.ShowInBar = false;
                }
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void LeadScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(LeadScarabText.Text, out _))
                {
                    LeadScarabText.Text = "0";
                }
                Plugin.CurrentCharacter.LeadScarabs = int.Parse(LeadScarabText.Text);
                Plugin.Utility.SaveSettings();
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void IronScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(IronScarabText.Text, out _))
                {
                    IronScarabText.Text = "0";
                }
                Plugin.CurrentCharacter.IronScarabs = int.Parse(IronScarabText.Text);
                Plugin.Utility.SaveSettings();
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void CopperScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(CopperScarabText.Text, out _))
                {
                    CopperScarabText.Text = "0";
                }
                Plugin.CurrentCharacter.CopperScarabs = int.Parse(CopperScarabText.Text);
                Plugin.Utility.SaveSettings();
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void SilverScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(SilverScarabText.Text, out _))
                {
                    SilverScarabText.Text = "0";
                }
                Plugin.CurrentCharacter.SilverScarabs = int.Parse(SilverScarabText.Text);
                Plugin.Utility.SaveSettings();
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void GoldScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(GoldScarabText.Text, out _))
                {
                    GoldScarabText.Text = "0";
                }
                Plugin.CurrentCharacter.GoldScarabs = int.Parse(GoldScarabText.Text);
                Plugin.Utility.SaveSettings();
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void PyrealScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(PyrealScarabText.Text, out _))
                {
                    PyrealScarabText.Text = "0";
                }
                Plugin.CurrentCharacter.PyrealScarabs = int.Parse(PyrealScarabText.Text);
                Plugin.Utility.SaveSettings();
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void PlatinumScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(PlatinumScarabText.Text, out _))
                {
                    PlatinumScarabText.Text = "0";
                }
                Plugin.CurrentCharacter.PlatinumScarabs = int.Parse(PlatinumScarabText.Text);
                Plugin.Utility.SaveSettings();
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void ManaScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(ManaScarabText.Text, out _))
                {
                    ManaScarabText.Text = "0";
                }
                Plugin.CurrentCharacter.ManaScarabs = int.Parse(ManaScarabText.Text);
                Plugin.Utility.SaveSettings();
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void TaperText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(TaperText.Text, out _))
                {
                    TaperText.Text = "0";
                }
                Plugin.CurrentCharacter.Tapers = int.Parse(TaperText.Text);
                Plugin.Utility.SaveSettings();
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
                    GeneralModule = null;
                    InventoryModule = null;
                    FellowshipModule = null;
                    Password.Change += Password_Change;
                    AutoFellow.Change += AutoFellow_Change;
                    AutoRespond.Change += AutoRespond_Change;
                    AnnounceLogoff.Change += AnnounceLogoff_Change;
                    PortalBotCheckBox.Change += PortalBotCheckBox_Change;
                    ExpTrackerCheckBox.Change += ExpTrackerCheckBox_Change;
                    LeadScarabText.Change += LeadScarabText_Change;
                    IronScarabText.Change += IronScarabText_Change;
                    CopperScarabText.Change += CopperScarabText_Change;
                    SilverScarabText.Change += SilverScarabText_Change;
                    GoldScarabText.Change += GoldScarabText_Change;
                    PyrealScarabText.Change += PyrealScarabText_Change;
                    PlatinumScarabText.Change += PlatinumScarabText_Change;
                    ManaScarabText.Change += ManaScarabText_Change;
                    TaperText.Change += TaperText_Change;
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
