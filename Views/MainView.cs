using Decal.Adapter;
using Decal.Adapter.Wrappers;
using VirindiViewService;
using VirindiViewService.Controls;

namespace ACManager.Views
{
    internal class MainView
    {
        internal PluginCore Plugin { get; set; }
        internal const string GeneralModule = "General";
        internal const string InventoryModule = "InventoryTracker";
        internal const string FellowshipModule = "FellowshipManager";
        private HudView View { get; set; }
        internal HudStaticText Version { get; set; }
        internal HudCheckBox AutoFellow {get; set; }
        internal HudCheckBox AutoRespond {get; set; }
        internal HudCheckBox LowCompLogoff { get; set; }
        internal HudCheckBox AnnounceLogoff {get; set; }
        internal HudCheckBox PortalBotCheckBox { get; set; }
        internal HudCheckBox ExpTrackerCheckBox { get; set; }
        internal HudTextBox SecretPassword { get; set; }
        internal HudTextBox LeadScarabText {get; set; }
        internal HudTextBox IronScarabText {get; set; }
        internal HudTextBox CopperScarabText {get; set; }
        internal HudTextBox SilverScarabText {get; set; }
        internal HudTextBox GoldScarabText {get; set; }
        internal HudTextBox PyrealScarabText {get; set; }
        internal HudTextBox PlatinumScarabText {get; set; }
        internal HudTextBox ManaScarabText {get; set; }
        internal HudTextBox TaperText {get; set; }

        public MainView(PluginCore parent)
        {
            try
            {
                Plugin = parent;

                VirindiViewService.XMLParsers.Decal3XMLParser parser = new VirindiViewService.XMLParsers.Decal3XMLParser();
                parser.ParseFromResource("ACManager.Views.mainView.xml", out ViewProperties Properties, out ControlGroup Controls);

                View = new HudView(Properties, Controls);

                Version = View != null ? (HudStaticText)View["Version"] : new HudStaticText();
                Version.Text = $"v{Utility.GetVersion()}";

                SecretPassword = View != null ? (HudTextBox)View["SecretPassword"] : new HudTextBox();
                SecretPassword.Change += SecretPassword_Change;

                AutoFellow = View != null ? (HudCheckBox)View["AutoFellow"] : new HudCheckBox();
                AutoFellow.Change += AutoFellow_Change;

                AutoRespond = View != null ? (HudCheckBox)View["AutoRespond"] : new HudCheckBox();
                AutoRespond.Change += AutoRespond_Change;

                LowCompLogoff = View != null ? (HudCheckBox)View["Components"] : new HudCheckBox();
                LowCompLogoff.Change += LowCompLogoff_Change;

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
            catch { }
        }

        private void SecretPassword_Change(object sender, System.EventArgs e)
        {
            Utility.SaveSetting(FellowshipModule, CoreManager.Current.CharacterFilter.Name, SecretPassword.Name, SecretPassword.Text);
            Plugin.FellowshipControl.Password = SecretPassword.Text;
        }

        private void AutoFellow_Change(object sender, System.EventArgs e)
        {
            Utility.SaveSetting(FellowshipModule, CoreManager.Current.CharacterFilter.Name, AutoFellow.Name, AutoFellow.Checked.ToString());
            Plugin.FellowshipControl.AutoFellowEnabled = AutoFellow.Checked;
            if (AutoFellow.Checked && Plugin.FellowshipControl.FellowStatus == FellowshipEventType.Create)
            {
                CoreManager.Current.Actions.FellowshipSetOpen(true);
            }
        }

        private void AutoRespond_Change(object sender, System.EventArgs e)
        {
            Utility.SaveSetting(GeneralModule, CoreManager.Current.CharacterFilter.Name, AutoRespond.Name, AutoRespond.Checked.ToString());
            Plugin.AutoRespondEnabled = AutoRespond.Checked;
        }

        private void LowCompLogoff_Change(object sender, System.EventArgs e)
        {
            Plugin.InventoryTracker.LogoffEnabled = LowCompLogoff.Checked;
        }

        private void AnnounceLogoff_Change(object sender, System.EventArgs e)
        {
            Utility.SaveSetting(InventoryModule, CoreManager.Current.CharacterFilter.Name, AnnounceLogoff.Name, AnnounceLogoff.Checked.ToString());
            Plugin.InventoryTracker.AnnounceLogoff = AnnounceLogoff.Checked;
        }

        private void PortalBotCheckBox_Change(object sender, System.EventArgs e)
        {
            if (PortalBotCheckBox.Checked)
            {
                Plugin.PortalBotView.View.ShowInBar = true;
            } else
            {
                Plugin.PortalBotView.View.ShowInBar = false;
            }
        }

        private void ExpTrackerCheckBox_Change(object sender, System.EventArgs e)
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

        private void LeadScarabText_Change(object sender, System.EventArgs e)
        {
            if (int.TryParse(LeadScarabText.Text, out int result))
            {
                Plugin.InventoryTracker.MinLead = result;
            }
            else
            {
                Plugin.InventoryTracker.MinLead = -1;
                LeadScarabText.Text = "-1";
            }
            Utility.SaveSetting(InventoryModule, CoreManager.Current.CharacterFilter.Name, LeadScarabText.Name, LeadScarabText.Text);
        }

        private void IronScarabText_Change(object sender, System.EventArgs e)
        {
            if (int.TryParse(IronScarabText.Text, out int result))
            {
                Plugin.InventoryTracker.MinIron = result;
            }
            else
            {
                Plugin.InventoryTracker.MinIron = -1;
                IronScarabText.Text = "-1";
            }
            Utility.SaveSetting(InventoryModule, CoreManager.Current.CharacterFilter.Name, IronScarabText.Name, IronScarabText.Text);
        }

        private void CopperScarabText_Change(object sender, System.EventArgs e)
        {
            if (int.TryParse(CopperScarabText.Text, out int result))
            {
                Plugin.InventoryTracker.MinCopper = result;
            }
            else
            {
                Plugin.InventoryTracker.MinCopper = -1;
                CopperScarabText.Text = "-1";
            }
            Utility.SaveSetting(InventoryModule, CoreManager.Current.CharacterFilter.Name, CopperScarabText.Name, CopperScarabText.Text);
        }

        private void SilverScarabText_Change(object sender, System.EventArgs e)
        {
            if (int.TryParse(SilverScarabText.Text, out int result))
            {
                Plugin.InventoryTracker.MinSilver = result;
            }
            else
            {
                Plugin.InventoryTracker.MinSilver = -1;
                SilverScarabText.Text = "-1";
            }
            Utility.SaveSetting(InventoryModule, CoreManager.Current.CharacterFilter.Name, SilverScarabText.Name, SilverScarabText.Text);
        }

        private void GoldScarabText_Change(object sender, System.EventArgs e)
        {
            if (int.TryParse(GoldScarabText.Text, out int result))
            {
                Plugin.InventoryTracker.MinGold = result;
            }
            else
            {
                Plugin.InventoryTracker.MinGold = -1;
                GoldScarabText.Text = "-1";
            }
            Utility.SaveSetting(InventoryModule, CoreManager.Current.CharacterFilter.Name, GoldScarabText.Name, GoldScarabText.Text);
        }

        private void PyrealScarabText_Change(object sender, System.EventArgs e)
        {
            if (int.TryParse(PyrealScarabText.Text, out int result))
            {
                Plugin.InventoryTracker.MinPyreal = result;
            }
            else
            {
                Plugin.InventoryTracker.MinPyreal = -1;
                PyrealScarabText.Text = "-1";
            }
            Utility.SaveSetting(InventoryModule, CoreManager.Current.CharacterFilter.Name, PyrealScarabText.Name, PyrealScarabText.Text);
        }

        private void PlatinumScarabText_Change(object sender, System.EventArgs e)
        {
            if (int.TryParse(PlatinumScarabText.Text, out int result))
            {
                Plugin.InventoryTracker.MinPlatinum = result;
            }
            else
            {
                Plugin.InventoryTracker.MinPlatinum = -1;
                PlatinumScarabText.Text = "-1";
            }
            Utility.SaveSetting(InventoryModule, CoreManager.Current.CharacterFilter.Name, PlatinumScarabText.Name, PlatinumScarabText.Text);
        }

        private void ManaScarabText_Change(object sender, System.EventArgs e)
        {
            if (int.TryParse(ManaScarabText.Text, out int result))
            {
                Plugin.InventoryTracker.MinManaScarabs = result;
            }
            else
            {
                Plugin.InventoryTracker.MinManaScarabs = -1;
                ManaScarabText.Text = "-1";
            }
            Utility.SaveSetting(InventoryModule, CoreManager.Current.CharacterFilter.Name, ManaScarabText.Name, ManaScarabText.Text);
        }

        private void TaperText_Change(object sender, System.EventArgs e)
        {
            if (int.TryParse(TaperText.Text, out int result))
            {
                Plugin.InventoryTracker.MinTapers = result;
            }
            else
            {
                Plugin.InventoryTracker.MinTapers = -1;
                TaperText.Text = "-1";
            }
            Utility.SaveSetting(InventoryModule, CoreManager.Current.CharacterFilter.Name, TaperText.Name, TaperText.Text);
        }
    }
}
