using Decal.Adapter;
using System;
using VirindiViewService;
using VirindiViewService.Controls;

namespace ACManager.Views.Tabs
{
    public class ConfigTab : IDisposable
    {
        private BotManagerView Parent { get; set; }
        public HudCheckBox BotEnabled { get; set; }
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
        private HudTextBox SkillOverride { get; set; }
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

            SkillOverride = Parent.View != null ? (HudTextBox)Parent.View["SkillOverride"] : new HudTextBox();
            SkillOverride.Change += SkillOverride_Change;

            Version = Parent.View != null ? (HudStaticText)Parent.View["Version"] : new HudStaticText();
            Version.Text = $"V{Utility.Version}";

            PopulateCharacterChoice();
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                BotEnabled.Checked = Utility.BotSettings.BotEnabled;
                RespondToGeneralChat.Checked = Utility.BotSettings.RespondToGeneralChat;
                RespondToAllegianceChat.Checked = Utility.BotSettings.RespondToAllegiance;
                Verbosity.Position = Utility.BotSettings.Verbosity;
                AdInterval.Text = Utility.BotSettings.AdInterval >= 5 ? Utility.BotSettings.AdInterval.ToString() : 5.ToString();
                ManaThreshold.Position = (int)(Utility.BotSettings.ManaThreshold * 100);
                StaminaThreshold.Position = (int)(Utility.BotSettings.StaminaThreshold * 100);
                ManaThresholdText.Text = ManaThreshold.Position.ToString() + "%";
                StamThresholdText.Text = StaminaThreshold.Position.ToString() + "%";
                AdsEnabled.Checked = Utility.BotSettings.AdsEnabled;
                BotPositioning.Checked = Utility.BotSettings.BotPositioning;
                DefaultHeading.Text = Utility.BotSettings.DefaultHeading.ToString();
                LocationSetpoint.Text = !Utility.BotSettings.DesiredLandBlock.Equals(0) ? $"{Utility.BotSettings.DesiredLandBlock.ToString("X").Substring(0, 4)} - X: { Math.Round(Utility.BotSettings.DesiredBotLocationX, 2)} Y: {Math.Round(Utility.BotSettings.DesiredBotLocationY, 2)}" : "No location set";
                StayBuffed.Checked = Utility.BotSettings.StayBuffed;
                Level7Self.Checked = Utility.BotSettings.Level7Self;
                SkillOverride.Text = Utility.BotSettings.SkillOverride.ToString();

                if (string.IsNullOrEmpty(Utility.BotSettings.BuffingCharacter))
                {
                    BuffingCharacterChoice.Current = 0;
                }
                else
                {
                    for (int i = 0; i < FilterCore.AccountCharacters.Count; i++)
                    {
                        if (FilterCore.AccountCharacters[i].Equals(Utility.BotSettings.BuffingCharacter))
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
                Parent.Machine.Enabled = Utility.BotSettings.BotEnabled = BotEnabled.Checked;
                Utility.SaveBotSettings();
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
                Parent.Machine.DesiredLandBlock = Utility.BotSettings.DesiredLandBlock = 0;
                Parent.Machine.DesiredBotLocationX = Utility.BotSettings.DesiredBotLocationX = 0;
                Parent.Machine.DesiredBotLocationY = Utility.BotSettings.DesiredBotLocationY = 0;
                LocationSetpoint.Text = "No location set";
                Utility.SaveBotSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void SetLocation_Hit(object sender, EventArgs e)
        {
            try
            {
                Parent.Machine.DesiredLandBlock = Utility.BotSettings.DesiredLandBlock = CoreManager.Current.Actions.Landcell;
                Parent.Machine.DesiredBotLocationX = Utility.BotSettings.DesiredBotLocationX = CoreManager.Current.Actions.LocationX;
                Parent.Machine.DesiredBotLocationY = Utility.BotSettings.DesiredBotLocationY = CoreManager.Current.Actions.LocationY;
                LocationSetpoint.Text = $"{Parent.Machine.DesiredLandBlock.ToString("X").Substring(0, 4)} - X: { Math.Round(Parent.Machine.DesiredBotLocationX, 2)} Y: {Math.Round(Parent.Machine.DesiredBotLocationY, 2)}";
                Utility.SaveBotSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void RespondToGeneralChat_Change(object sender, EventArgs e)
        {
            try
            {
                Parent.Machine.RespondToOpenChat = Utility.BotSettings.RespondToGeneralChat = RespondToGeneralChat.Checked;
                Utility.SaveBotSettings();
                Debug.ToChat($"The bot will {(Utility.BotSettings.RespondToGeneralChat ? "now" : "no longer")} summon portals based on keyword requests said in local chat.");
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
                Parent.Machine.RespondToAllegiance = Utility.BotSettings.RespondToAllegiance = RespondToAllegianceChat.Checked;
                Utility.SaveBotSettings();
                Debug.ToChat($"The bot will {(Utility.BotSettings.RespondToAllegiance ? "now" : "no longer")} summon portals based on keyword requests said in allegiance chat.");
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
                Parent.Machine.Advertise = Utility.BotSettings.AdsEnabled = AdsEnabled.Checked;
                Utility.SaveBotSettings();
                Debug.ToChat($"The bot will {(Utility.BotSettings.AdsEnabled ? "now" : "no longer")} broadcast advertisements.");
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
                Parent.Machine.EnablePositioning = Utility.BotSettings.BotPositioning = BotPositioning.Checked;
                Utility.SaveBotSettings();
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
                        Parent.Machine.AdInterval = Utility.BotSettings.AdInterval = 5;
                        AdInterval.Text = 5.ToString();
                    }
                    else
                    {
                        Parent.Machine.AdInterval = Utility.BotSettings.AdInterval = result;
                    }
                }
                else
                {
                    Parent.Machine.AdInterval = Utility.BotSettings.AdInterval = 10;
                    AdInterval.Text = 10.ToString();
                }
                Utility.SaveBotSettings();
                Debug.ToChat($"The bot will now broadcast an advertisement every {Utility.BotSettings.AdInterval} minutes.");
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
                Parent.Machine.DefaultHeading = Utility.BotSettings.DefaultHeading = Math.Round(CoreManager.Current.Actions.Heading, 0);
                DefaultHeading.Text = Math.Round(CoreManager.Current.Actions.Heading, 0).ToString();
                Utility.SaveBotSettings();
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
                    Parent.Machine.DefaultHeading = Utility.BotSettings.DefaultHeading = result;
                }
                else
                {
                    DefaultHeading.Text = "0";
                    Parent.Machine.DefaultHeading = Utility.BotSettings.DefaultHeading = 0;
                }
                Utility.SaveBotSettings();
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
                Parent.Machine.Verbosity = Utility.BotSettings.Verbosity = Verbosity.Position;
                Utility.SaveBotSettings();
                Debug.ToChat($"The bot will now respond with {Utility.BotSettings.Verbosity + 1} portals per response line to 'whereto' commands. Adjust this lower if portals are being truncated due to max character limits. " +
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
                Parent.Machine.ManaThreshold = Utility.BotSettings.ManaThreshold = (double)ManaThreshold.Position / 100;
                ManaThresholdText.Text = $"{ManaThreshold.Position}%";
                Utility.SaveBotSettings();
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
                Parent.Machine.StaminaThreshold = Utility.BotSettings.StaminaThreshold = (double)StaminaThreshold.Position / 100;
                StamThresholdText.Text = $"{StaminaThreshold.Position}%";
                Utility.SaveBotSettings();
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
                        Parent.Machine.BuffingCharacter = Utility.BotSettings.BuffingCharacter = selectedCharacter.Text;
                        Debug.ToChat($"The buff bot feature is now enabled using {selectedCharacter.Text}.");
                    }
                }
                else
                {
                    Parent.Machine.BuffingCharacter = Utility.BotSettings.BuffingCharacter = "";
                    Debug.ToChat("The buff bot feature is now disabled.");
                }
                Utility.SaveBotSettings();
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
                Parent.Machine.StayBuffed = Utility.BotSettings.StayBuffed = StayBuffed.Checked;
                Utility.SaveBotSettings();
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
                Parent.Machine.Level7Self = Utility.BotSettings.Level7Self = Level7Self.Checked;
                Utility.SaveBotSettings();
                Debug.ToChat(Level7Self.Checked ? "The bot will now buff the buffing character with level 7 spells only, no level 8 spells are used for self spells. Use this if level 8 self spells are not known." : "The bot will now buff the buffing character with level 8 spells only, with level 7 fallback if a spell is not known. Use this if level 8 self spells are known.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void SkillOverride_Change(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(SkillOverride.Text, out int result))
                {
                    Parent.Machine.SkillOverride = Utility.BotSettings.SkillOverride = result;
                }
                else
                {
                    Parent.Machine.SkillOverride = Utility.BotSettings.SkillOverride = 0;
                    SkillOverride.Text = 0.ToString();
                }
                Utility.SaveBotSettings();
                Debug.ToChat($"Your magic casting abilities are now modified by {Utility.BotSettings.SkillOverride} when calculating skill.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void PopulateCharacterChoice()
        {
            BuffingCharacterChoice.AddItem("None", null);
            for (int i = 0; i < FilterCore.AccountCharacters.Count; i++)
            {
                BuffingCharacterChoice.AddItem(FilterCore.AccountCharacters[i], null);
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
                    SkillOverride.Change -= SkillOverride_Change;
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
