using System;
using VirindiViewService;
using VirindiViewService.Controls;

namespace ACManager.Views
{
    internal class MainView : IDisposable
    {
        internal FilterCore Filter { get; set; }
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

        public MainView(FilterCore parent)
        {
            try
            {
                Filter = parent;

                VirindiViewService.XMLParsers.Decal3XMLParser parser = new VirindiViewService.XMLParsers.Decal3XMLParser();
                parser.ParseFromResource("ACManager.Views.mainView.xml", out ViewProperties Properties, out ControlGroup Controls);

                View = new HudView(Properties, Controls);

                Version = View != null ? (HudStaticText)View["Version"] : new HudStaticText();
                Version.Text = $"v{Filter.Machine.Utility.Version}";

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

                // Update the UI from settings
                PortalBotCheckBox.Checked = true;
                ExpTrackerCheckBox.Checked = true;
                AutoFellow.Checked = Filter.Machine.CurrentCharacter.AutoFellow;
                AutoRespond.Checked = Filter.Machine.CurrentCharacter.AutoRespond;
                AnnounceLogoff.Checked = Filter.Machine.CurrentCharacter.AnnounceLogoff;
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void Password_Change(object sender, EventArgs e)
        {
            try
            {
                Filter.Machine.CurrentCharacter.Password = Password.Text;
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void AutoFellow_Change(object sender, EventArgs e)
        {
            try
            {
                Filter.Machine.CurrentCharacter.AutoFellow = AutoFellow.Checked;
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void AutoRespond_Change(object sender, EventArgs e)
        {
            try
            {
                Filter.Machine.CurrentCharacter.AutoRespond = AutoRespond.Checked;
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void AnnounceLogoff_Change(object sender, EventArgs e)
        {
            try
            {
                Filter.Machine.CurrentCharacter.AnnounceLogoff = AnnounceLogoff.Checked;
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void PortalBotCheckBox_Change(object sender, EventArgs e)
        {
            try
            {
                if (PortalBotCheckBox.Checked)
                {
                    Filter.PortalBotView.View.ShowInBar = true;
                }
                else
                {
                    Filter.PortalBotView.View.ShowInBar = false;
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void ExpTrackerCheckBox_Change(object sender, EventArgs e)
        {
            try
            {
                if (ExpTrackerCheckBox.Checked)
                {
                    Filter.ExpTrackerView.View.ShowInBar = true;
                }
                else
                {
                    Filter.ExpTrackerView.View.ShowInBar = false;
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void LeadScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(LeadScarabText.Text, out _))
                {
                    LeadScarabText.Text = "0";
                }
                Filter.Machine.CurrentCharacter.LeadScarabs = int.Parse(LeadScarabText.Text);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void IronScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(IronScarabText.Text, out _))
                {
                    IronScarabText.Text = "0";
                }
                Filter.Machine.CurrentCharacter.IronScarabs = int.Parse(IronScarabText.Text);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void CopperScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(CopperScarabText.Text, out _))
                {
                    CopperScarabText.Text = "0";
                }
                Filter.Machine.CurrentCharacter.CopperScarabs = int.Parse(CopperScarabText.Text);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void SilverScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(SilverScarabText.Text, out _))
                {
                    SilverScarabText.Text = "0";
                }
                Filter.Machine.CurrentCharacter.SilverScarabs = int.Parse(SilverScarabText.Text);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void GoldScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(GoldScarabText.Text, out _))
                {
                    GoldScarabText.Text = "0";
                }
                Filter.Machine.CurrentCharacter.GoldScarabs = int.Parse(GoldScarabText.Text);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void PyrealScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(PyrealScarabText.Text, out _))
                {
                    PyrealScarabText.Text = "0";
                }
                Filter.Machine.CurrentCharacter.PyrealScarabs = int.Parse(PyrealScarabText.Text);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void PlatinumScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(PlatinumScarabText.Text, out _))
                {
                    PlatinumScarabText.Text = "0";
                }
                Filter.Machine.CurrentCharacter.PlatinumScarabs = int.Parse(PlatinumScarabText.Text);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void ManaScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(ManaScarabText.Text, out _))
                {
                    ManaScarabText.Text = "0";
                }
                Filter.Machine.CurrentCharacter.ManaScarabs = int.Parse(ManaScarabText.Text);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void TaperText_Change(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(TaperText.Text, out _))
                {
                    TaperText.Text = "0";
                }
                Filter.Machine.CurrentCharacter.Tapers = int.Parse(TaperText.Text);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Filter = null;
                    Password.Change -= Password_Change;
                    AutoFellow.Change -= AutoFellow_Change;
                    AutoRespond.Change -= AutoRespond_Change;
                    AnnounceLogoff.Change -= AnnounceLogoff_Change;
                    PortalBotCheckBox.Change -= PortalBotCheckBox_Change;
                    ExpTrackerCheckBox.Change -= ExpTrackerCheckBox_Change;
                    LeadScarabText.Change -= LeadScarabText_Change;
                    IronScarabText.Change -= IronScarabText_Change;
                    CopperScarabText.Change -= CopperScarabText_Change;
                    SilverScarabText.Change -= SilverScarabText_Change;
                    GoldScarabText.Change -= GoldScarabText_Change;
                    PyrealScarabText.Change -= PyrealScarabText_Change;
                    PlatinumScarabText.Change -= PlatinumScarabText_Change;
                    ManaScarabText.Change -= ManaScarabText_Change;
                    TaperText.Change -= TaperText_Change;
                    View?.Dispose();
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
