using System;
using VirindiViewService;
using VirindiViewService.Controls;

namespace ACManager.Views.Tabs
{
    internal class ConfigTab : IDisposable
    {
        private BotManagerView Parent { get; set; }
        private HudCheckBox BotEnabled { get; set; }
        private HudButton ClearLocation { get; set; }
        private HudButton SetLocation { get; set; }
        private HudStaticText LocationSetpoint { get; set; }
        private HudCheckBox RespondToGeneralChat { get; set; }
        private HudCheckBox RespondToAllegianceChat { get; set; }
        private HudCheckBox AdsEnabled { get; set; }
        private HudCheckBox BotPositioning { get; set; }
        private HudTextBox AdInterval { get; set; }
        private HudButton SetHeading { get; set; }
        private HudTextBox DefaultHeading { get; set; }
        private HudHSlider Verbosity { get; set; }
        private HudHSlider ManaThreshold { get; set; }
        private HudHSlider StaminaThreshold { get; set; }
        private HudStaticText ManaThresholdText { get; set; }
        private HudStaticText StamThresholdText { get; set; }
        private HudCombo BuffingCharacterChoice { get; set; }
        private HudCheckBox StayBuffed { get; set; }
        private HudCheckBox Level7Self { get; set; }
        private HudStaticText Version { get; set; }
        private bool disposedValue;

        public ConfigTab(BotManagerView botManagerView)
        {
            Parent = botManagerView;

            BotEnabled = Parent.View != null ? (HudCheckBox)Parent.View["Bot"] : new HudCheckBox();
            BotEnabled.Change += BotEnabled_Change;

            ClearLocation = Parent.View != null ? (HudButton)Parent.View["ClearLocation"] : new HudButton();
            ClearLocation.Hit += ClearLocation_Hit;

            LocationSetpoint = Parent.View != null ? (HudStaticText)Parent.View["LocationSetpoint"] : new HudStaticText();
            LocationSetpoint.TextAlignment = WriteTextFormats.Center;

            SetLocation = Parent.View != null ? (HudButton)Parent.View["SetLocation"] : new HudButton();
            SetLocation.Hit += SetLocation_Hit;

            RespondToGeneralChat = Parent.View != null ? (HudCheckBox)Parent.View["GeneralChatResponse"] : new HudCheckBox();
            RespondToGeneralChat.Change += RespondToGeneralChat_Change;

            RespondToAllegianceChat = Parent.View != null ? (HudCheckBox)Parent.View["AllegianceResponse"] : new HudCheckBox();
            RespondToAllegianceChat.Change += RespondToAllegianceChat_Change;

            AdsEnabled = Parent.View != null ? (HudCheckBox)Parent.View["AdsEnabled"] : new HudCheckBox();
            AdsEnabled.Change += AdsEnabled_Change;

            BotPositioning = Parent.View != null ? (HudCheckBox)Parent.View["BotPositioning"] : new HudCheckBox();
            BotPositioning.Change += BotPositioning_Change;

            AdInterval = Parent.View != null ? (HudTextBox)Parent.View["AdInterval"] : new HudTextBox();
            AdInterval.Change += AdInterval_Change;

            SetHeading = Parent.View != null ? (HudButton)Parent.View["SetHeading"] : new HudButton();
            SetHeading.Hit += SetHeading_Hit;

            DefaultHeading = Parent.View != null ? (HudTextBox)Parent.View["DefaultHeading"] : new HudTextBox();
            DefaultHeading.Change += DefaultHeading_Change;

            Verbosity = Parent.View != null ? (HudHSlider)Parent.View["Verbosity"] : new HudHSlider();
            Verbosity.Changed += Verbosity_Changed;

            ManaThreshold = Parent.View != null ? (HudHSlider)Parent.View["ManaThresh"] : new HudHSlider();
            ManaThreshold.Changed += ManaThreshhold_Changed;

            StaminaThreshold = Parent.View != null ? (HudHSlider)Parent.View["StamThresh"] : new HudHSlider();
            StaminaThreshold.Changed += StaminaThreshhold_Changed;

            ManaThresholdText = Parent.View != null ? (HudStaticText)Parent.View["ManaThreshText"] : new HudStaticText();
            StamThresholdText = Parent.View != null ? (HudStaticText)Parent.View["StamThreshText"] : new HudStaticText();

            BuffingCharacterChoice = Parent.View != null ? (HudCombo)Parent.View["BuffingCharacterChoice"] : new HudCombo(new ControlGroup());
            BuffingCharacterChoice.Change += BuffingCharacterChoice_Change;

            StayBuffed = Parent.View != null ? (HudCheckBox)Parent.View["StayBuffed"] : new HudCheckBox();
            StayBuffed.Change += StayBuffed_Change;

            Level7Self = Parent.View != null ? (HudCheckBox)Parent.View["Level7Self"] : new HudCheckBox();
            Level7Self.Change += Level7Self_Change;

            Version = Parent.View != null ? (HudStaticText)Parent.View["Version"] : new HudStaticText();
            Version.Text = $"V{Parent.Machine.Utility.Version}";

            PopulateCharacterChoice();
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                BotEnabled.Checked = Parent.Machine.Utility.BotSettings.BotEnabled;
                RespondToGeneralChat.Checked = Parent.Machine.Utility.BotSettings.RespondToGeneralChat;
                RespondToAllegianceChat.Checked = Parent.Machine.Utility.BotSettings.RespondToAllegiance;
                Verbosity.Position = Parent.Machine.Utility.BotSettings.Verbosity;
                AdInterval.Text = Parent.Machine.Utility.BotSettings.AdInterval >= 5 ? Parent.Machine.Utility.BotSettings.AdInterval.ToString() : 5.ToString();
                ManaThreshold.Position = (int)(Parent.Machine.Utility.BotSettings.ManaThreshold * 100);
                StaminaThreshold.Position = (int)(Parent.Machine.Utility.BotSettings.StaminaThreshold * 100);
                ManaThresholdText.Text = ManaThreshold.Position.ToString() + "%";
                StamThresholdText.Text = StaminaThreshold.Position.ToString() + "%";
                AdsEnabled.Checked = Parent.Machine.Utility.BotSettings.AdsEnabled;
                BotPositioning.Checked = Parent.Machine.Utility.BotSettings.BotPositioning;
                DefaultHeading.Text = Parent.Machine.Utility.BotSettings.DefaultHeading.ToString();
                LocationSetpoint.Text = !Parent.Machine.Utility.BotSettings.DesiredLandBlock.Equals(0) ? $"{Parent.Machine.Utility.BotSettings.DesiredLandBlock.ToString("X").Substring(0, 4)} - X: { Math.Round(Parent.Machine.Utility.BotSettings.DesiredBotLocationX, 2)} Y: {Math.Round(Parent.Machine.Utility.BotSettings.DesiredBotLocationY, 2)}" : "No location set";
                StayBuffed.Checked = Parent.Machine.Utility.BotSettings.StayBuffed;
                Level7Self.Checked = Parent.Machine.Utility.BotSettings.Level7Self;

                if (string.IsNullOrEmpty(Parent.Machine.Utility.BotSettings.BuffingCharacter))
                {
                    BuffingCharacterChoice.Current = 0;
                }
                else
                {
                    for (int i = 0; i < Parent.Machine.AccountCharacters.Count; i++)
                    {
                        if (Parent.Machine.AccountCharacters[i].Equals(Parent.Machine.Utility.BotSettings.BuffingCharacter))
                        {
                            BuffingCharacterChoice.Current = i + 1;
                            break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void BotEnabled_Change(object sender, EventArgs e)
        {
            try
            {
                Parent.Machine.Enabled = Parent.Machine.Utility.BotSettings.BotEnabled = BotEnabled.Checked;
                Parent.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void ClearLocation_Hit(object sender, EventArgs e)
        {
            try
            {
                Parent.Machine.DesiredLandBlock = Parent.Machine.Utility.BotSettings.DesiredLandBlock = 0;
                Parent.Machine.DesiredBotLocationX = Parent.Machine.Utility.BotSettings.DesiredBotLocationX = 0;
                Parent.Machine.DesiredBotLocationY = Parent.Machine.Utility.BotSettings.DesiredBotLocationY = 0;
                LocationSetpoint.Text = "No location set";
                Parent.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void SetLocation_Hit(object sender, EventArgs e)
        {
            try
            {
                Parent.Machine.DesiredLandBlock = Parent.Machine.Utility.BotSettings.DesiredLandBlock = Parent.Machine.Core.Actions.Landcell;
                Parent.Machine.DesiredBotLocationX = Parent.Machine.Utility.BotSettings.DesiredBotLocationX = Parent.Machine.Core.Actions.LocationX;
                Parent.Machine.DesiredBotLocationY = Parent.Machine.Utility.BotSettings.DesiredBotLocationY = Parent.Machine.Core.Actions.LocationY;
                LocationSetpoint.Text = $"{Parent.Machine.DesiredLandBlock.ToString("X").Substring(0, 4)} - X: { Math.Round(Parent.Machine.DesiredBotLocationX, 2)} Y: {Math.Round(Parent.Machine.DesiredBotLocationY, 2)}";
                Parent.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void RespondToGeneralChat_Change(object sender, EventArgs e)
        {
            try
            {
                Parent.Machine.RespondToOpenChat = Parent.Machine.Utility.BotSettings.RespondToGeneralChat = RespondToGeneralChat.Checked;
                Parent.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"The bot will {(Parent.Machine.Utility.BotSettings.RespondToGeneralChat ? "now" : "no longer")} summon portals based on keyword requests said in local chat.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void RespondToAllegianceChat_Change(object sender, EventArgs e)
        {
            try
            {
                Parent.Machine.RespondToAllegiance = Parent.Machine.Utility.BotSettings.RespondToAllegiance = RespondToAllegianceChat.Checked;
                Parent.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"The bot will {(Parent.Machine.Utility.BotSettings.RespondToAllegiance ? "now" : "no longer")} summon portals based on keyword requests said in allegiance chat.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void AdsEnabled_Change(object sender, EventArgs e)
        {
            try
            {
                Parent.Machine.Advertise = Parent.Machine.Utility.BotSettings.AdsEnabled = AdsEnabled.Checked;
                Parent.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"The bot will {(Parent.Machine.Utility.BotSettings.AdsEnabled ? "now" : "no longer")} broadcast advertisements.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void BotPositioning_Change(object sender, EventArgs e)
        {
            try
            {
                Parent.Machine.EnablePositioning = Parent.Machine.Utility.BotSettings.BotPositioning = BotPositioning.Checked;
                Parent.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"The bot will {(Parent.Machine.EnablePositioning ? "now" : "no longer")} try to automatically position itself to the set navigation point.");
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void AdInterval_Change(object sender, EventArgs e)
        {
            try
            {
                if (double.TryParse(AdInterval.Text, out double result))
                {
                    if (result <= 5)
                    {
                        Parent.Machine.AdInterval = Parent.Machine.Utility.BotSettings.AdInterval = 5;
                        AdInterval.Text = 5.ToString();
                    }
                    else
                    {
                        Parent.Machine.AdInterval = Parent.Machine.Utility.BotSettings.AdInterval = result;
                    }
                }
                else
                {
                    Parent.Machine.AdInterval = Parent.Machine.Utility.BotSettings.AdInterval = 10;
                    AdInterval.Text = 10.ToString();
                }
                Parent.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"The bot will now broadcast an advertisement every {Parent.Machine.Utility.BotSettings.AdInterval} minutes.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void SetHeading_Hit(object sender, EventArgs e)
        {
            try
            {
                Parent.Machine.DefaultHeading = Parent.Machine.Utility.BotSettings.DefaultHeading = Math.Round(Parent.Machine.Core.Actions.Heading, 0);
                DefaultHeading.Text = Math.Round(Parent.Machine.Core.Actions.Heading, 0).ToString();
                Parent.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void DefaultHeading_Change(object sender, EventArgs e)
        {
            try
            {
                if (double.TryParse(DefaultHeading.Text, out double result))
                {
                    if (result >= 360)
                    {
                        DefaultHeading.Text = "0";
                        result = 0;
                    }
                    else if (result <= -1)
                    {
                        result = 0;
                    }
                    Parent.Machine.DefaultHeading = Parent.Machine.Utility.BotSettings.DefaultHeading = result;
                }
                else
                {
                    DefaultHeading.Text = "0";
                    Parent.Machine.DefaultHeading = Parent.Machine.Utility.BotSettings.DefaultHeading = 0;
                }
                Parent.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void Verbosity_Changed(int min, int max, int pos)
        {
            try
            {
                Parent.Machine.Verbosity = Parent.Machine.Utility.BotSettings.Verbosity = Verbosity.Position;
                Parent.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"The bot will now respond with {Parent.Machine.Utility.BotSettings.Verbosity + 1} portals per response line to 'whereto' commands. Adjust this lower if portals are being truncated due to max character limits. " +
                    $"Adjust higher if bot is being server muted.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void ManaThreshhold_Changed(int min, int max, int pos)
        {
            try
            {
                Parent.Machine.ManaThreshold = Parent.Machine.Utility.BotSettings.ManaThreshold = (double)ManaThreshold.Position / 100;
                ManaThresholdText.Text = $"{ManaThreshold.Position}%";
                Parent.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"The bot will now recover mana at {ManaThreshold.Position}%.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void StaminaThreshhold_Changed(int min, int max, int pos)
        {
            try
            {
                Parent.Machine.StaminaThreshold = Parent.Machine.Utility.BotSettings.StaminaThreshold = (double)StaminaThreshold.Position / 100;
                StamThresholdText.Text = $"{StaminaThreshold.Position}%";
                Parent.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"The bot will now recover stamina at {StaminaThreshold.Position}%.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void BuffingCharacterChoice_Change(object sender, EventArgs e)
        {
            try
            {
                if (!BuffingCharacterChoice.Current.Equals(0))
                {
                    using (HudStaticText selectedCharacter = (HudStaticText)BuffingCharacterChoice[BuffingCharacterChoice.Current])
                    {
                        Parent.Machine.BuffingCharacter = Parent.Machine.Utility.BotSettings.BuffingCharacter = selectedCharacter.Text;
                        Debug.ToChat($"The buff bot feature is now enabled using {selectedCharacter.Text}.");
                    }
                }
                else
                {
                    Parent.Machine.BuffingCharacter = Parent.Machine.Utility.BotSettings.BuffingCharacter = "";
                    Debug.ToChat("The buff bot feature is now disabled.");
                }
                Parent.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void StayBuffed_Change(object sender, EventArgs e)
        {
            try
            {
                Parent.Machine.StayBuffed = Parent.Machine.Utility.BotSettings.StayBuffed = StayBuffed.Checked;
                Parent.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"{(StayBuffed.Checked ? "The bot will now ensure self buffs will never run out, even when idle." : "The bot will now let self buffs run out, and only self buff when required.")}");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void Level7Self_Change(object sender, EventArgs e)
        {
            try
            {
                Parent.Machine.Level7Self = Parent.Machine.Utility.BotSettings.Level7Self = Level7Self.Checked;
                Parent.Machine.Utility.SaveBotSettings();
                Debug.ToChat(Level7Self.Checked ? "The bot will now buff the buffing character with level 7 spells only, no level 8 spells are used for self spells. Use this if level 8 self spells are not known." : "The bot will now buff the buffing character with level 8 spells only, with level 7 fallback if a spell is not known. Use this if level 8 self spells are known.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void PopulateCharacterChoice()
        {
            BuffingCharacterChoice.AddItem("None", null);
            for (int i = 0; i < Parent.Machine.AccountCharacters.Count; i++)
            {
                BuffingCharacterChoice.AddItem(Parent.Machine.AccountCharacters[i], null);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    BotEnabled.Change -= BotEnabled_Change;
                    ClearLocation.Hit -= ClearLocation_Hit;
                    SetLocation.Hit -= SetHeading_Hit;
                    RespondToGeneralChat.Change -= RespondToGeneralChat_Change;
                    RespondToAllegianceChat.Change -= RespondToAllegianceChat_Change;
                    AdsEnabled.Change -= AdsEnabled_Change;
                    BotPositioning.Change -= BotPositioning_Change;
                    AdInterval.Change -= AdInterval_Change;
                    SetHeading.Hit -= SetHeading_Hit;
                    DefaultHeading.Change -= DefaultHeading_Change;
                    Verbosity.Changed -= Verbosity_Changed;
                    ManaThreshold.Changed -= ManaThreshhold_Changed;
                    StaminaThreshold.Changed -= StaminaThreshhold_Changed;
                    BuffingCharacterChoice.Change -= BuffingCharacterChoice_Change;
                    StayBuffed.Change -= StayBuffed_Change;
                    Level7Self.Change -= Level7Self_Change;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
