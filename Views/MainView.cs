using ACManager.Settings;
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

                LoadSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void LowCompLogoff_Change(object sender, EventArgs e)
        {
            if (LowCompLogoff.Checked)
            {
                Filter.InventoryTracker.StartTimer();
            }
            else
            {
                Filter.InventoryTracker.StopTimer();
            }
        }

        private void LoadSettings()
        {
            PortalBotCheckBox.Checked = Filter.Machine.Utility.GUISettings.BotConfigVisible;
            ExpTrackerCheckBox.Checked = Filter.Machine.Utility.GUISettings.ExpTrackerVisible;

            Character character = Filter.Machine.Utility.GetCurrentCharacter();

            AutoFellow.Checked = character.AutoFellow;
            AutoRespond.Checked = character.AutoRespond;
            AnnounceLogoff.Checked = character.AnnounceLogoff;
            Password.Text = string.IsNullOrEmpty(character.Password) ? "xp" : character.Password;

            LeadScarabText.Text = character.LeadScarabs.ToString();
            IronScarabText.Text = character.IronScarabs.ToString();
            CopperScarabText.Text = character.CopperScarabs.ToString();
            SilverScarabText.Text = character.SilverScarabs.ToString();
            GoldScarabText.Text = character.GoldScarabs.ToString();
            PyrealScarabText.Text = character.PyrealScarabs.ToString();
            PlatinumScarabText.Text = character.PlatinumScarabs.ToString();
            ManaScarabText.Text = character.ManaScarabs.ToString();
            TaperText.Text = character.Tapers.ToString();
            AnnounceLogoff.Checked = character.AnnounceLogoff;
        }

        private Character CharacterExistsOrNew()
        {
            Character newCharacter = new Character
            {
                Name = Filter.Machine.Core.CharacterFilter.Name,
                Account = Filter.Machine.Core.CharacterFilter.AccountName,
                Server = Filter.Machine.Core.CharacterFilter.Server
            };

            if (Filter.Machine.Utility.CharacterSettings.Characters.Contains(newCharacter))
            {
                foreach (Character character in Filter.Machine.Utility.CharacterSettings.Characters)
                {
                    if (newCharacter.Equals(character))
                    {
                        return character;
                    }
                }
            }
            return newCharacter;
        }

        private void Password_Change(object sender, EventArgs e)
        {
            try
            {
                Character character = CharacterExistsOrNew();
                character.Password = Password.Text;
                Filter.Machine.Utility.UpdateSettingsWithCharacter(character);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void AutoFellow_Change(object sender, EventArgs e)
        {
            try
            {
                Character character = CharacterExistsOrNew();
                character.AutoFellow = AutoFellow.Checked;
                Filter.Machine.Utility.UpdateSettingsWithCharacter(character);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void AutoRespond_Change(object sender, EventArgs e)
        {
            try
            {
                Character character = CharacterExistsOrNew();
                character.AutoRespond = AutoRespond.Checked;
                Filter.Machine.Utility.UpdateSettingsWithCharacter(character);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void AnnounceLogoff_Change(object sender, EventArgs e)
        {
            try
            {
                Character character = CharacterExistsOrNew();
                character.AnnounceLogoff = AnnounceLogoff.Checked;
                Filter.Machine.Utility.UpdateSettingsWithCharacter(character);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void PortalBotCheckBox_Change(object sender, EventArgs e)
        {
            try
            {
                Filter.Machine.BotManagerView.View.ShowInBar = Filter.Machine.Utility.GUISettings.BotConfigVisible = PortalBotCheckBox.Checked;
                Filter.Machine.Utility.SaveGUISettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void ExpTrackerCheckBox_Change(object sender, EventArgs e)
        {
            try
            {
                Filter.ExpTracker.ToggleView(ExpTrackerCheckBox.Checked);
                Filter.Machine.Utility.GUISettings.ExpTrackerVisible = ExpTrackerCheckBox.Checked;
                Filter.Machine.Utility.SaveGUISettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void LeadScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                Character character = CharacterExistsOrNew();
                if (int.TryParse(LeadScarabText.Text, out int result))
                {
                    character.LeadScarabs = result;
                }
                else
                {
                    LeadScarabText.Text = "0";
                    character.LeadScarabs = 0;
                }

                Filter.Machine.Utility.UpdateSettingsWithCharacter(character);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void IronScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                Character character = CharacterExistsOrNew();
                if (int.TryParse(IronScarabText.Text, out int result))
                {
                    character.IronScarabs = result;
                }
                else
                {
                    IronScarabText.Text = "0";
                    character.IronScarabs = 0;
                }

                Filter.Machine.Utility.UpdateSettingsWithCharacter(character);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void CopperScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                Character character = CharacterExistsOrNew();
                if (int.TryParse(CopperScarabText.Text, out int result))
                {
                    character.CopperScarabs = result;
                }
                else
                {
                    CopperScarabText.Text = "0";
                    character.CopperScarabs = 0;
                }

                Filter.Machine.Utility.UpdateSettingsWithCharacter(character);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void SilverScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                Character character = CharacterExistsOrNew();
                if (int.TryParse(SilverScarabText.Text, out int result))
                {
                    character.SilverScarabs = result;
                }
                else
                {
                    SilverScarabText.Text = "0";
                    character.SilverScarabs = 0;
                }

                Filter.Machine.Utility.UpdateSettingsWithCharacter(character);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void GoldScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                Character character = CharacterExistsOrNew();
                if (int.TryParse(GoldScarabText.Text, out int result))
                {
                    character.GoldScarabs = result;
                }
                else
                {
                    GoldScarabText.Text = "0";
                    character.GoldScarabs = 0;
                }

                Filter.Machine.Utility.UpdateSettingsWithCharacter(character);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void PyrealScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                Character character = CharacterExistsOrNew();
                if (int.TryParse(PyrealScarabText.Text, out int result))
                {
                    character.PyrealScarabs = result;
                }
                else
                {
                    PyrealScarabText.Text = "0";
                    character.PyrealScarabs = 0;
                }

                Filter.Machine.Utility.UpdateSettingsWithCharacter(character);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void PlatinumScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                Character character = CharacterExistsOrNew();
                if (int.TryParse(PlatinumScarabText.Text, out int result))
                {
                    character.PlatinumScarabs = result;
                }
                else
                {
                    PlatinumScarabText.Text = "0";
                    character.PlatinumScarabs = 0;
                }

                Filter.Machine.Utility.UpdateSettingsWithCharacter(character);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void ManaScarabText_Change(object sender, EventArgs e)
        {
            try
            {
                Character character = CharacterExistsOrNew();
                if (int.TryParse(ManaScarabText.Text, out int result))
                {
                    character.ManaScarabs = result;
                }
                else
                {
                    ManaScarabText.Text = "0";
                    character.ManaScarabs = 0;
                }

                Filter.Machine.Utility.UpdateSettingsWithCharacter(character);
                Filter.Machine.Utility.SaveCharacterSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void TaperText_Change(object sender, EventArgs e)
        {
            try
            {
                Character character = CharacterExistsOrNew();
                if (int.TryParse(TaperText.Text, out int result))
                {
                    character.Tapers = result;
                }
                else
                {
                    TaperText.Text = "0";
                    character.Tapers = 0;
                }

                Filter.Machine.Utility.UpdateSettingsWithCharacter(character);
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
