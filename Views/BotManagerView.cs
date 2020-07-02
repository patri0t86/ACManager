using ACManager.Settings;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;
using VirindiViewService;
using VirindiViewService.Controls;

namespace ACManager.Views
{
    internal class BotManagerView : IDisposable
    {
        private FilterCore Filter { get; set; }
        private CoreManager Core { get; set; }
        internal HudView View { get; set; }

        #region Config Variable Declaration
        internal HudCheckBox BotEnabled { get; set; }
        internal HudButton ClearLocation { get; set; }
        internal HudButton SetLocation { get; set; }
        internal HudStaticText LocationSetpoint { get; set; }
        internal HudCheckBox RespondToGeneralChat { get; set; }
        internal HudCheckBox AdsEnabled { get; set; }
        internal HudCheckBox BotPositioning { get; set; }
        internal HudTextBox AdInterval { get; set; }
        internal HudButton SetHeading { get; set; }
        internal HudTextBox DefaultHeading { get; set; }
        internal HudHSlider Verbosity { get; set; }
        internal HudHSlider ManaThreshold { get; set; }
        internal HudHSlider StaminaThreshold { get; set; }
        internal HudStaticText ManaThresholdText { get; set; }
        internal HudStaticText StamThresholdText { get; set; }
        internal HudTextBox LeadScarabThreshold { get; set; }
        internal HudTextBox IronScarabThreshold { get; set; }
        internal HudTextBox CopperScarabThreshold { get; set; }
        internal HudTextBox SilverScarabThreshold { get; set; }
        internal HudTextBox GoldScarabThreshold { get; set; }
        internal HudTextBox PyrealScarabThreshold { get; set; }
        internal HudTextBox PlatinumScarabThreshold { get; set; }
        internal HudTextBox ManaScarabThreshold { get; set; }
        internal HudTextBox ComponentThreshold { get; set; }
        #endregion

        #region Portal Options Variable Declarations
        internal HudCombo CharacterChoice { get; set; }
        internal HudTextBox PrimaryKeyword { get; set; }
        internal HudTextBox SecondaryKeyword { get; set; }
        internal HudTextBox PrimaryDescription { get; set; }
        internal HudTextBox SecondaryDescription { get; set; }
        internal HudTextBox NewAdvertisement { get; set; }
        internal HudTextBox PrimaryHeading { get; set; }
        internal HudTextBox SecondaryHeading { get; set; }
        internal HudTextBox PrimaryLevel { get; set; }
        internal HudTextBox SecondaryLevel { get; set; }
        #endregion

        #region Advertisement Variable Declarations
        internal HudList Advertisements { get; set; }
        internal HudButton AddAdvertisement { get; set; }
        #endregion

        public BotManagerView(FilterCore parent, CoreManager core)
        {
            try
            {
                Filter = parent;
                Core = core;
                VirindiViewService.XMLParsers.Decal3XMLParser parser = new VirindiViewService.XMLParsers.Decal3XMLParser();
                parser.ParseFromResource("ACManager.Views.botManagerView.xml", out ViewProperties Properties, out ControlGroup Controls);

                View = new HudView(Properties, Controls);

                #region Config
                BotEnabled = View != null ? (HudCheckBox)View["Bot"] : new HudCheckBox();
                BotEnabled.Change += BotEnabled_Change;

                ClearLocation = View != null ? (HudButton)View["ClearLocation"] : new HudButton();
                ClearLocation.Hit += ClearLocation_Hit;

                LocationSetpoint = View != null ? (HudStaticText)View["LocationSetpoint"] : new HudStaticText();
                LocationSetpoint.TextAlignment = VirindiViewService.WriteTextFormats.Center;

                SetLocation = View != null ? (HudButton)View["SetLocation"] : new HudButton();
                SetLocation.Hit += SetLocation_Hit;

                RespondToGeneralChat = View != null ? (HudCheckBox)View["GeneralChatResponse"] : new HudCheckBox();
                RespondToGeneralChat.Change += RespondToGeneralChat_Change;

                AdsEnabled = View != null ? (HudCheckBox)View["AdsEnabled"] : new HudCheckBox();
                AdsEnabled.Change += AdsEnabled_Change;

                BotPositioning = View != null ? (HudCheckBox)View["BotPositioning"] : new HudCheckBox();
                BotPositioning.Change += BotPositioning_Change;

                AdInterval = View != null ? (HudTextBox)View["AdInterval"] : new HudTextBox();
                AdInterval.Change += AdInterval_Change;

                SetHeading = View != null ? (HudButton)View["SetHeading"] : new HudButton();
                SetHeading.Hit += SetHeading_Hit;

                DefaultHeading = View != null ? (HudTextBox)View["DefaultHeading"] : new HudTextBox();
                DefaultHeading.Change += DefaultHeading_Change;

                Verbosity = View != null ? (HudHSlider)View["Verbosity"] : new HudHSlider();
                Verbosity.Changed += Verbosity_Changed;

                ManaThreshold = View != null ? (HudHSlider)View["ManaThresh"] : new HudHSlider();
                ManaThreshold.Changed += ManaThreshhold_Changed;

                StaminaThreshold = View != null ? (HudHSlider)View["StamThresh"] : new HudHSlider();
                StaminaThreshold.Changed += StaminaThreshhold_Changed;

                ManaThresholdText = View != null ? (HudStaticText)View["ManaThreshText"] : new HudStaticText();
                StamThresholdText = View != null ? (HudStaticText)View["StamThreshText"] : new HudStaticText();

                LeadScarabThreshold = View != null ? (HudTextBox)View["LeadScarabThreshold"] : new HudTextBox();
                LeadScarabThreshold.Change += LeadScarabThreshold_Change;

                IronScarabThreshold = View != null ? (HudTextBox)View["IronScarabThreshold"] : new HudTextBox();
                IronScarabThreshold.Change += IronScarabThreshold_Change;

                CopperScarabThreshold = View != null ? (HudTextBox)View["CopperScarabThreshold"] : new HudTextBox();
                CopperScarabThreshold.Change += CopperScarabThreshold_Change;

                SilverScarabThreshold = View != null ? (HudTextBox)View["SilverScarabThreshold"] : new HudTextBox();
                SilverScarabThreshold.Change += SilverScarabThreshold_Change;

                GoldScarabThreshold = View != null ? (HudTextBox)View["GoldScarabThreshold"] : new HudTextBox();
                GoldScarabThreshold.Change += GoldScarabThreshold_Change;

                PyrealScarabThreshold = View != null ? (HudTextBox)View["PyrealScarabThreshold"] : new HudTextBox();
                PyrealScarabThreshold.Change += PyrealScarabThreshold_Change;

                PlatinumScarabThreshold = View != null ? (HudTextBox)View["PlatinumScarabThreshold"] : new HudTextBox();
                PlatinumScarabThreshold.Change += PlatinumScarabThreshold_Change;

                ManaScarabThreshold = View != null ? (HudTextBox)View["ManaScarabThreshold"] : new HudTextBox();
                ManaScarabThreshold.Change += ManaScarabThreshold_Change;

                ComponentThreshold = View != null ? (HudTextBox)View["ComponentThreshold"] : new HudTextBox();
                ComponentThreshold.Change += ComponentThreshold_Change;

                #endregion

                #region Portal Settings
                PrimaryKeyword = View != null ? (HudTextBox)View["PrimaryKeyword"] : new HudTextBox();
                PrimaryKeyword.Change += PrimaryKeyword_Change;

                SecondaryKeyword = View != null ? (HudTextBox)View["SecondaryKeyword"] : new HudTextBox();
                SecondaryKeyword.Change += SecondaryKeyword_Change;

                PrimaryDescription = View != null ? (HudTextBox)View["PrimaryDescription"] : new HudTextBox();
                PrimaryDescription.Change += PrimaryDescription_Change;

                SecondaryDescription = View != null ? (HudTextBox)View["SecondaryDescription"] : new HudTextBox();
                SecondaryDescription.Change += SecondaryDescription_Change;

                PrimaryHeading = View != null ? (HudTextBox)View["PrimaryHeading"] : new HudTextBox();
                PrimaryHeading.Change += PrimaryHeading_Change;

                SecondaryHeading = View != null ? (HudTextBox)View["SecondaryHeading"] : new HudTextBox();
                SecondaryHeading.Change += SecondaryHeading_Change;

                PrimaryLevel = View != null ? (HudTextBox)View["PrimaryLevel"] : new HudTextBox();
                PrimaryLevel.Change += PrimaryLevel_Change;

                SecondaryLevel = View != null ? (HudTextBox)View["SecondaryLevel"] : new HudTextBox();
                SecondaryLevel.Change += SecondaryLevel_Change;

                AddAdvertisement = View != null ? (HudButton)View["AddAdvertisement"] : new HudButton();
                AddAdvertisement.Hit += AddAdvertisement_Hit;

                CharacterChoice = View != null ? (HudCombo)View["CharacterChoice"] : new HudCombo(new ControlGroup());
                CharacterChoice.Change += CharacterChoice_Change;
                #endregion

                #region Advertisements
                NewAdvertisement = View != null ? (HudTextBox)View["NewAdvertisementText"] : new HudTextBox();

                Advertisements = View != null ? (HudList)View["AdvertisementList"] : new HudList();
                Advertisements.Click += Advertisements_Click;
                #endregion

                GetCharacters();
                GetAdvertisements();

                LoadSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void ComponentThreshold_Change(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(ComponentThreshold.Text, out int result))
                {
                    if (result < 0)
                    {
                        result = 0;
                    }
                    Filter.Machine.Inventory.ComponentThreshold = Filter.Machine.Utility.BotSettings.ComponentThreshold = result;
                }
                else
                {
                    ComponentThreshold.Text = "0";
                    Filter.Machine.Inventory.ComponentThreshold = Filter.Machine.Utility.BotSettings.ComponentThreshold = 0;
                }
                Filter.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void ManaScarabThreshold_Change(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(ManaScarabThreshold.Text, out int result))
                {
                    if (result < 0)
                    {
                        result = 0;
                    }
                    Filter.Machine.Inventory.ManaScarabThreshold = Filter.Machine.Utility.BotSettings.ManaScarabThreshold = result;
                }
                else
                {
                    ManaScarabThreshold.Text = "0";
                    Filter.Machine.Inventory.ManaScarabThreshold = Filter.Machine.Utility.BotSettings.ManaScarabThreshold = 0;
                }
                Filter.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void PlatinumScarabThreshold_Change(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(PlatinumScarabThreshold.Text, out int result))
                {
                    if (result < 0)
                    {
                        result = 0;
                    }
                    Filter.Machine.Inventory.PlatinumScarabThreshold = Filter.Machine.Utility.BotSettings.PlatinumScarabThreshold = result;
                }
                else
                {
                    PlatinumScarabThreshold.Text = "0";
                    Filter.Machine.Inventory.PlatinumScarabThreshold = Filter.Machine.Utility.BotSettings.PlatinumScarabThreshold = 0;
                }
                Filter.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void PyrealScarabThreshold_Change(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(PyrealScarabThreshold.Text, out int result))
                {
                    if (result < 0)
                    {
                        result = 0;
                    }
                    Filter.Machine.Inventory.PyrealScarabThreshold = Filter.Machine.Utility.BotSettings.PyrealScarabThreshold = result;
                }
                else
                {
                    PyrealScarabThreshold.Text = "0";
                    Filter.Machine.Inventory.PyrealScarabThreshold = Filter.Machine.Utility.BotSettings.PyrealScarabThreshold = 0;
                }
                Filter.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void GoldScarabThreshold_Change(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(GoldScarabThreshold.Text, out int result))
                {
                    if (result < 0)
                    {
                        result = 0;
                    }
                    Filter.Machine.Inventory.GoldScarabThreshold = Filter.Machine.Utility.BotSettings.GoldScarabThreshold = result;
                }
                else
                {
                    GoldScarabThreshold.Text = "0";
                    Filter.Machine.Inventory.GoldScarabThreshold = Filter.Machine.Utility.BotSettings.GoldScarabThreshold = 0;
                }
                Filter.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void SilverScarabThreshold_Change(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(SilverScarabThreshold.Text, out int result))
                {
                    if (result < 0)
                    {
                        result = 0;
                    }
                    Filter.Machine.Inventory.SilverScarabThreshold = Filter.Machine.Utility.BotSettings.SilverScarabThreshold = result;
                }
                else
                {
                    SilverScarabThreshold.Text = "0";
                    Filter.Machine.Inventory.SilverScarabThreshold = Filter.Machine.Utility.BotSettings.SilverScarabThreshold = 0;
                }
                Filter.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void CopperScarabThreshold_Change(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(CopperScarabThreshold.Text, out int result))
                {
                    if (result < 0)
                    {
                        result = 0;
                    }
                    Filter.Machine.Inventory.CopperScarabThreshold = Filter.Machine.Utility.BotSettings.CopperScarabThreshold = result;
                }
                else
                {
                    CopperScarabThreshold.Text = "0";
                    Filter.Machine.Inventory.CopperScarabThreshold = Filter.Machine.Utility.BotSettings.CopperScarabThreshold = 0;
                }
                Filter.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void IronScarabThreshold_Change(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(IronScarabThreshold.Text, out int result))
                {
                    if (result < 0)
                    {
                        result = 0;
                    }
                    Filter.Machine.Inventory.IronScarabThreshold = Filter.Machine.Utility.BotSettings.IronScarabThreshold = result;
                }
                else
                {
                    IronScarabThreshold.Text = "0";
                    Filter.Machine.Inventory.IronScarabThreshold = Filter.Machine.Utility.BotSettings.IronScarabThreshold = 0;
                }
                Filter.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void LeadScarabThreshold_Change(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(LeadScarabThreshold.Text, out int result))
                {
                    if (result < 0)
                    {
                        result = 0;
                    }
                    Filter.Machine.Inventory.LeadScarabThreshold = Filter.Machine.Utility.BotSettings.LeadScarabThreshold = result;
                }
                else
                {
                    LeadScarabThreshold.Text = "0";
                    Filter.Machine.Inventory.LeadScarabThreshold = Filter.Machine.Utility.BotSettings.LeadScarabThreshold = 0;
                }
                Filter.Machine.Utility.SaveBotSettings();
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
                Filter.Machine.DesiredLandBlock = Filter.Machine.Utility.BotSettings.DesiredLandBlock = 0;
                Filter.Machine.DesiredBotLocationX = Filter.Machine.Utility.BotSettings.DesiredBotLocationX = 0;
                Filter.Machine.DesiredBotLocationY = Filter.Machine.Utility.BotSettings.DesiredBotLocationY = 0;
                LocationSetpoint.Text = "No location set";
                Filter.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void BotPositioning_Change(object sender, EventArgs e)
        {
            try
            {
                Filter.Machine.EnablePositioning = Filter.Machine.Utility.BotSettings.BotPositioning = BotPositioning.Checked;
                Filter.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"The bot will {(Filter.Machine.EnablePositioning ? "now" : "no longer")} try to automatically position itself to the set navigation point.");
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void SetLocation_Hit(object sender, EventArgs e)
        {
            try
            {
                Filter.Machine.DesiredLandBlock = Filter.Machine.Utility.BotSettings.DesiredLandBlock = Filter.Machine.Core.Actions.Landcell;
                Filter.Machine.DesiredBotLocationX = Filter.Machine.Utility.BotSettings.DesiredBotLocationX = Filter.Machine.Core.Actions.LocationX;
                Filter.Machine.DesiredBotLocationY = Filter.Machine.Utility.BotSettings.DesiredBotLocationY = Filter.Machine.Core.Actions.LocationY;
                LocationSetpoint.Text = $"{Filter.Machine.DesiredLandBlock.ToString("X").Substring(0, 4)} - X: { Math.Round(Filter.Machine.DesiredBotLocationX, 2)} Y: {Math.Round(Filter.Machine.DesiredBotLocationY, 2)}";
                Filter.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void SetHeading_Hit(object sender, EventArgs e)
        {
            try
            {
                Filter.Machine.DefaultHeading = Filter.Machine.Utility.BotSettings.DefaultHeading = Math.Round(Filter.Machine.Core.Actions.Heading, 0);
                DefaultHeading.Text = Math.Round(Filter.Machine.Core.Actions.Heading, 0).ToString();
                Filter.Machine.Utility.SaveBotSettings();
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
                    Filter.Machine.DefaultHeading = Filter.Machine.Utility.BotSettings.DefaultHeading = result;
                }
                else
                {
                    DefaultHeading.Text = "0";
                    Filter.Machine.DefaultHeading = Filter.Machine.Utility.BotSettings.DefaultHeading = 0;
                }
                Filter.Machine.Utility.SaveBotSettings();
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
                Filter.Machine.Advertise = Filter.Machine.Utility.BotSettings.AdsEnabled = AdsEnabled.Checked;
                Filter.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"The bot will {(Filter.Machine.Utility.BotSettings.AdsEnabled ? "now" : "no longer")} broadcast advertisements.");
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
                Filter.Machine.StaminaThreshold = Filter.Machine.Utility.BotSettings.StaminaThreshold = (double)StaminaThreshold.Position / 100;
                StamThresholdText.Text = $"{StaminaThreshold.Position}%";
                Filter.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"The bot will now recover stamina at {StaminaThreshold.Position}%.");
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
                Filter.Machine.ManaThreshold = Filter.Machine.Utility.BotSettings.ManaThreshold = (double)ManaThreshold.Position / 100;
                ManaThresholdText.Text = $"{ManaThreshold.Position}%";
                Filter.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"The bot will now recover mana at {ManaThreshold.Position}%.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void LoadSettings()
        {
            try
            {
                BotEnabled.Checked = Filter.Machine.Utility.BotSettings.BotEnabled;
                View.ShowInBar = Filter.Machine.Utility.GUISettings.BotConfigVisible;
                RespondToGeneralChat.Checked = Filter.Machine.Utility.BotSettings.RespondToGeneralChat;
                Verbosity.Position = Filter.Machine.Utility.BotSettings.Verbosity;
                AdInterval.Text = Filter.Machine.Utility.BotSettings.AdInterval >= 5 ? Filter.Machine.Utility.BotSettings.AdInterval.ToString() : 5.ToString();
                ManaThreshold.Position = (int)(Filter.Machine.Utility.BotSettings.ManaThreshold * 100);
                StaminaThreshold.Position = (int)(Filter.Machine.Utility.BotSettings.StaminaThreshold * 100);
                ManaThresholdText.Text = $"{ManaThreshold.Position}%";
                StamThresholdText.Text = $"{StaminaThreshold.Position}%";
                AdsEnabled.Checked = Filter.Machine.Utility.BotSettings.AdsEnabled;
                BotPositioning.Checked = Filter.Machine.Utility.BotSettings.BotPositioning;
                DefaultHeading.Text = Filter.Machine.Utility.BotSettings.DefaultHeading.ToString();
                LocationSetpoint.Text = Filter.Machine.Utility.BotSettings.GetType().GetProperty("DesiredLandBlock") != null ? $"{Filter.Machine.Utility.BotSettings.DesiredLandBlock.ToString("X").Substring(0,4)} - X: { Math.Round(Filter.Machine.Utility.BotSettings.DesiredBotLocationX, 2)} Y: {Math.Round(Filter.Machine.Utility.BotSettings.DesiredBotLocationY, 2)}" : "No location set";
                LeadScarabThreshold.Text = Filter.Machine.Utility.BotSettings.LeadScarabThreshold.ToString();
                IronScarabThreshold.Text = Filter.Machine.Utility.BotSettings.IronScarabThreshold.ToString();
                CopperScarabThreshold.Text = Filter.Machine.Utility.BotSettings.CopperScarabThreshold.ToString();
                SilverScarabThreshold.Text = Filter.Machine.Utility.BotSettings.SilverScarabThreshold.ToString();
                GoldScarabThreshold.Text = Filter.Machine.Utility.BotSettings.GoldScarabThreshold.ToString();
                PyrealScarabThreshold.Text = Filter.Machine.Utility.BotSettings.PyrealScarabThreshold.ToString();
                PlatinumScarabThreshold.Text = Filter.Machine.Utility.BotSettings.PlatinumScarabThreshold.ToString();
                ManaScarabThreshold.Text = Filter.Machine.Utility.BotSettings.ManaScarabThreshold.ToString();
                ComponentThreshold.Text = Filter.Machine.Utility.BotSettings.ComponentThreshold.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void RespondToGeneralChat_Change(object sender, EventArgs e)
        {
            try
            {
                Filter.Machine.RespondToOpenChat = Filter.Machine.Utility.BotSettings.RespondToGeneralChat = RespondToGeneralChat.Checked;
                Filter.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"The bot will {(Filter.Machine.Utility.BotSettings.RespondToGeneralChat ? "now" : "no longer")} summon portals based on keyword requests said in local chat.");
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
                Filter.Machine.Verbosity = Filter.Machine.Utility.BotSettings.Verbosity = Verbosity.Position;
                Filter.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"The bot will now respond with {Filter.Machine.Utility.BotSettings.Verbosity + 1} portals per response line to 'whereto' commands. Adjust this lower if portals are being truncated due to max character limits. " +
                    $"Adjust higher if bot is being server muted.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void AdInterval_Change(object sender, EventArgs e)
        {
            try
            {
                if (double.TryParse(AdInterval.Text, out double result))
                {
                    if (result <= 5)
                    {
                        Filter.Machine.AdInterval = Filter.Machine.Utility.BotSettings.AdInterval = 5;
                        AdInterval.Text = 5.ToString();
                    }
                    else
                    {
                        Filter.Machine.AdInterval = Filter.Machine.Utility.BotSettings.AdInterval = result;
                    }
                }
                else
                {
                    Filter.Machine.AdInterval = Filter.Machine.Utility.BotSettings.AdInterval = 10;
                    AdInterval.Text = 10.ToString();
                }
                Filter.Machine.Utility.SaveBotSettings();
                Debug.ToChat($"The bot will now broadcast an advertisement every {Filter.Machine.Utility.BotSettings.AdInterval} minutes.");
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
                Filter.Machine.Enabled = Filter.Machine.Utility.BotSettings.BotEnabled = BotEnabled.Checked;
                if (Filter.Machine.Utility.BotSettings.BotEnabled)
                {
                    Filter.Machine.ChatManager.Broadcast($"/me is running ACManager Bot {Filter.Machine.Utility.Version}. Whisper /help to get started.");
                    Filter.Machine.LastBroadcast = DateTime.Now;
                }
                Filter.Machine.Utility.SaveBotSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void PrimaryLevel_Change(object sender, EventArgs e)
        {
            try
            {
                if (!CharacterChoice.Current.Equals(0))
                {
                    using (HudStaticText selectedCharacter = (HudStaticText)CharacterChoice[CharacterChoice.Current])
                    {
                        Character character = CharacterExistsOrNew(selectedCharacter.Text);
                        if (int.TryParse(PrimaryLevel.Text, out int result))
                        {
                            if (result >= 275)
                            {
                                result = 275;
                                UpdatePortalLevel(character, PortalType.Primary, result);
                                PrimaryLevel.Text = result.ToString();
                            }
                            else if (result < 0)
                            {
                                result = 0;
                                UpdatePortalLevel(character, PortalType.Primary, result);
                                PrimaryLevel.Text = result.ToString();
                            }
                            else
                            {
                                UpdatePortalLevel(character, PortalType.Primary, result);
                            }
                        }
                        else
                        {
                            PrimaryLevel.Text = $"{default(int)}";
                            UpdatePortalLevel(character, PortalType.Primary, default);
                        }
                    }
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void SecondaryLevel_Change(object sender, EventArgs e)
        {
            try
            {
                if (!CharacterChoice.Current.Equals(0))
                {
                    using (HudStaticText selectedCharacter = (HudStaticText)CharacterChoice[CharacterChoice.Current])
                    {
                        Character character = CharacterExistsOrNew(selectedCharacter.Text);
                        if (int.TryParse(SecondaryLevel.Text, out int result))
                        {
                            if (result >= 275)
                            {
                                result = 275;
                                UpdatePortalLevel(character, PortalType.Secondary, result);
                                SecondaryLevel.Text = result.ToString();
                            }
                            else if (result < 0)
                            {
                                result = 0;
                                UpdatePortalLevel(character, PortalType.Secondary, result);
                                SecondaryLevel.Text = result.ToString();
                            }
                            else
                            {
                                UpdatePortalLevel(character, PortalType.Secondary, result);
                            }
                        }
                        else
                        {
                            SecondaryLevel.Text = $"{default(int)}";
                            UpdatePortalLevel(character, PortalType.Secondary, default);
                        }
                    }
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void PrimaryKeyword_Change(object sender, EventArgs e)
        {
            try
            {
                if (!CharacterChoice.Current.Equals(0))
                {
                    HudStaticText selectedCharacter = (HudStaticText)CharacterChoice[CharacterChoice.Current];
                    Character character = CharacterExistsOrNew(selectedCharacter.Text);
                    UpdatePortalKeyword(character, PortalType.Primary, PrimaryKeyword.Text.ToLower());
                    selectedCharacter.Dispose();
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void SecondaryKeyword_Change(object sender, EventArgs e)
        {
            try
            {
                if (!CharacterChoice.Current.Equals(0))
                {
                    HudStaticText selectedCharacter = (HudStaticText)CharacterChoice[CharacterChoice.Current];
                    Character character = CharacterExistsOrNew(selectedCharacter.Text);
                    UpdatePortalKeyword(character, PortalType.Secondary, SecondaryKeyword.Text.ToLower());
                    selectedCharacter.Dispose();
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void PrimaryDescription_Change(object sender, EventArgs e)
        {
            try
            {
                if (!CharacterChoice.Current.Equals(0))
                {
                    HudStaticText selectedCharacter = (HudStaticText)CharacterChoice[CharacterChoice.Current];
                    Character character = CharacterExistsOrNew(selectedCharacter.Text);
                    UpdatePortalDescription(character, PortalType.Primary, PrimaryDescription.Text);
                    selectedCharacter.Dispose();
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void SecondaryDescription_Change(object sender, EventArgs e)
        {
            try
            {
                if (!CharacterChoice.Current.Equals(0))
                {
                    HudStaticText selectedCharacter = (HudStaticText)CharacterChoice[CharacterChoice.Current];
                    Character character = CharacterExistsOrNew(selectedCharacter.Text);
                    UpdatePortalDescription(character, PortalType.Secondary, SecondaryDescription.Text);
                    selectedCharacter.Dispose();
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void PrimaryHeading_Change(object sender, EventArgs e)
        {
            try
            {
                if (!CharacterChoice.Current.Equals(0))
                {
                    HudStaticText selectedCharacter = (HudStaticText)CharacterChoice[CharacterChoice.Current];
                    Character character = CharacterExistsOrNew(selectedCharacter.Text);
                    if (double.TryParse(PrimaryHeading.Text, out double result))
                    {
                        if (result >= 360)
                        {
                            result = 0;
                            UpdatePortalHeading(character, PortalType.Primary, result);
                            PrimaryHeading.Text = result.ToString();
                        }
                        else if (result <= -1)
                        {
                            result = -1;
                            UpdatePortalHeading(character, PortalType.Primary, result);
                            PrimaryHeading.Text = result.ToString();
                        }
                        else
                        {
                            UpdatePortalHeading(character, PortalType.Primary, result);
                        }
                    }
                    else
                    {
                        PrimaryHeading.Text = $"{default(double)}";
                        UpdatePortalHeading(character, PortalType.Primary, default);
                    }
                    selectedCharacter.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void SecondaryHeading_Change(object sender, EventArgs e)
        {
            try
            {
                if (!CharacterChoice.Current.Equals(0))
                {
                    HudStaticText selectedCharacter = (HudStaticText)CharacterChoice[CharacterChoice.Current];
                    Character character = CharacterExistsOrNew(selectedCharacter.Text);
                    if (double.TryParse(SecondaryHeading.Text, out double result))
                    {
                        if (result >= 360)
                        {
                            result = 0;
                            UpdatePortalHeading(character, PortalType.Secondary, result);
                            SecondaryHeading.Text = result.ToString();
                        }
                        else if (result <= -1)
                        {
                            result = -1;
                            UpdatePortalHeading(character, PortalType.Secondary, result);
                            SecondaryHeading.Text = result.ToString();
                        }
                        else
                        {
                            UpdatePortalHeading(character, PortalType.Secondary, result);
                        }
                    }
                    else
                    {
                        SecondaryHeading.Text = $"{default(double)}";
                        UpdatePortalHeading(character, PortalType.Secondary, default);
                    }
                    selectedCharacter.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void AddAdvertisement_Hit(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(NewAdvertisement.Text))
                {
                    HudList.HudListRowAccessor row = Advertisements.AddRow();
                    using (HudStaticText control = new HudStaticText())
                    {
                        string ad = NewAdvertisement.Text;
                        control.Text = ad;
                        row[0] = control;
                        NewAdvertisement.Text = "";

                        Advertisement advertisement = new Advertisement
                        {
                            Message = ad
                        };
                        Filter.Machine.Utility.BotSettings.Advertisements.Add(advertisement);
                        Filter.Machine.Utility.SaveBotSettings();
                    }
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void Advertisements_Click(object sender, int row, int col)
        {
            try
            {
                HudStaticText adToDelete = (HudStaticText)Advertisements[row][0];
                Advertisements.RemoveRow(row);

                for (int i = 0; i < Filter.Machine.Utility.BotSettings.Advertisements.Count; i++)
                {
                    if (Filter.Machine.Utility.BotSettings.Advertisements[i].Message.Equals(adToDelete.Text))
                    {
                        Filter.Machine.Utility.BotSettings.Advertisements.RemoveAt(i);
                        break;
                    }
                }

                Filter.Machine.Utility.SaveBotSettings();
                adToDelete.Dispose();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        public void ClearUI()
        {
            PrimaryKeyword.Text = "";
            SecondaryKeyword.Text = "";
            PrimaryDescription.Text = "";
            SecondaryDescription.Text = "";
            PrimaryHeading.Text = "";
            SecondaryHeading.Text = "";
            PrimaryLevel.Text = "";
            SecondaryLevel.Text = "";
        }

        private void CharacterChoice_Change(object sender, EventArgs e)
        {
            try
            {
                ClearUI();
                if (!CharacterChoice.Current.Equals(0))
                {
                    HudStaticText selectedCharacter = (HudStaticText)CharacterChoice[CharacterChoice.Current];
                    Character character = CharacterExistsOrNew(selectedCharacter.Text);

                    foreach (Portal portal in character.Portals)
                    {
                        if (portal.Type.Equals(PortalType.Primary))
                        {
                            PrimaryKeyword.Text = portal.Keyword;
                            PrimaryDescription.Text = portal.Description;
                            PrimaryHeading.Text = portal.Heading.ToString();
                            PrimaryLevel.Text = portal.Level.ToString();
                        }
                        else
                        {
                            SecondaryKeyword.Text = portal.Keyword;
                            SecondaryDescription.Text = portal.Description;
                            SecondaryHeading.Text = portal.Heading.ToString();
                            SecondaryLevel.Text = portal.Level.ToString();
                        }
                    }

                    selectedCharacter.Dispose();
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void GetCharacters()
        {
            List<string> characterList = new List<string>();
            IndexedCollection<CharFilterIndex, int, AccountCharInfo> accountChars = Core.CharacterFilter.Characters;
            for (int i = 0; i < accountChars.Count; i++)
            {
                characterList.Add(accountChars[i].Name);
            }

            characterList.Sort();

            CharacterChoice.AddItem("Select character...", null);
            for (int i = 0; i < characterList.Count; i++)
            {
                CharacterChoice.AddItem(characterList[i], null);
            }
        }

        private void GetAdvertisements()
        {
            HudStaticText row;
            for (int i = 0; i < Filter.Machine.Utility.BotSettings.Advertisements.Count; i++)
            {
                HudList.HudListRowAccessor rowAccessor = Advertisements.AddRow();
                row = new HudStaticText
                {
                    Text = Filter.Machine.Utility.BotSettings.Advertisements[i].Message
                };
                rowAccessor[0] = row;
            }
        }

        private Character CharacterExistsOrNew(string name)
        {
            Character character = new Character
            {
                Name = name,
                Account = Core.CharacterFilter.AccountName,
                Server = Core.CharacterFilter.Server
            };

            if (Filter.Machine.Utility.CharacterSettings.Characters.Contains(character))
            {
                foreach (Character ch in Filter.Machine.Utility.CharacterSettings.Characters)
                {
                    if (character.Equals(ch))
                    {
                        return ch;
                    }
                }
            }
            return character;
        }

        private void UpdatePortalKeyword(Character character, PortalType type, string value)
        {
            Portal portal = null;
            for (int i = 0; i < character.Portals.Count; i++)
            {
                if (character.Portals[i].Type.Equals(type))
                {
                    portal = character.Portals[i];
                    character.Portals.RemoveAt(i);
                    break;
                }
            }

            if (portal != null)
            {
                portal.Keyword = value;
            }
            else
            {
                portal = new Portal
                {
                    Type = type,
                    Keyword = value
                };
            }
            character.Portals.Add(portal);

            if (Filter.Machine.Utility.CharacterSettings.Characters.Contains(character))
            {
                for (int i = 0; i < Filter.Machine.Utility.CharacterSettings.Characters.Count; i++)
                {
                    if (Filter.Machine.Utility.CharacterSettings.Characters[i].Equals(character))
                    {
                        Filter.Machine.Utility.CharacterSettings.Characters[i] = character;
                        break;
                    }
                }
            }
            else
            {
                Filter.Machine.Utility.CharacterSettings.Characters.Add(character);
            }

            Filter.Machine.Utility.SaveCharacterSettings();
        }

        private void UpdatePortalDescription(Character character, PortalType type, string value)
        {
            Portal portal = null;
            for (int i = 0; i < character.Portals.Count; i++)
            {
                if (character.Portals[i].Type.Equals(type))
                {
                    portal = character.Portals[i];
                    character.Portals.RemoveAt(i);
                    break;
                }
            }

            if (portal != null)
            {
                portal.Description = value;
            }
            else
            {
                portal = new Portal
                {
                    Type = type,
                    Description = value
                };
            }
            character.Portals.Add(portal);

            if (Filter.Machine.Utility.CharacterSettings.Characters.Contains(character))
            {
                for (int i = 0; i < Filter.Machine.Utility.CharacterSettings.Characters.Count; i++)
                {
                    if (Filter.Machine.Utility.CharacterSettings.Characters[i].Equals(character))
                    {
                        Filter.Machine.Utility.CharacterSettings.Characters[i] = character;
                        break;
                    }
                }
            }
            else
            {
                Filter.Machine.Utility.CharacterSettings.Characters.Add(character);
            }

            Filter.Machine.Utility.SaveCharacterSettings();
        }

        private void UpdatePortalHeading(Character character, PortalType type, double value)
        {
            Portal portal = null;
            for (int i = 0; i < character.Portals.Count; i++)
            {
                if (character.Portals[i].Type.Equals(type))
                {
                    portal = character.Portals[i];
                    character.Portals.RemoveAt(i);
                    break;
                }
            }

            if (portal != null)
            {
                portal.Heading = value;
            }
            else
            {
                portal = new Portal
                {
                    Type = type,
                    Heading = value
                };
            }
            character.Portals.Add(portal);

            if (Filter.Machine.Utility.CharacterSettings.Characters.Contains(character))
            {
                for (int i = 0; i < Filter.Machine.Utility.CharacterSettings.Characters.Count; i++)
                {
                    if (Filter.Machine.Utility.CharacterSettings.Characters[i].Equals(character))
                    {
                        Filter.Machine.Utility.CharacterSettings.Characters[i] = character;
                        break;
                    }
                }
            }
            else
            {
                Filter.Machine.Utility.CharacterSettings.Characters.Add(character);
            }

            Filter.Machine.Utility.SaveCharacterSettings();
        }

        private void UpdatePortalLevel(Character character, PortalType type, int value)
        {
            Portal portal = null;
            for (int i = 0; i < character.Portals.Count; i++)
            {
                if (character.Portals[i].Type.Equals(type))
                {
                    portal = character.Portals[i];
                    character.Portals.RemoveAt(i);
                    break;
                }
            }

            if (portal != null)
            {
                portal.Level = value;
            }
            else
            {
                portal = new Portal
                {
                    Type = type,
                    Level = value
                };
            }
            character.Portals.Add(portal);

            if (Filter.Machine.Utility.CharacterSettings.Characters.Contains(character))
            {
                for (int i = 0; i < Filter.Machine.Utility.CharacterSettings.Characters.Count; i++)
                {
                    if (Filter.Machine.Utility.CharacterSettings.Characters[i].Equals(character))
                    {
                        Filter.Machine.Utility.CharacterSettings.Characters[i] = character;
                        break;
                    }
                }
            }
            else
            {
                Filter.Machine.Utility.CharacterSettings.Characters.Add(character);
            }
            Filter.Machine.Utility.SaveCharacterSettings();
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

                    #region Remove Config EventHandlers
                    BotEnabled.Change -= BotEnabled_Change;
                    ClearLocation.Hit -= ClearLocation_Hit;
                    SetLocation.Hit -= SetHeading_Hit;
                    RespondToGeneralChat.Change -= RespondToGeneralChat_Change;
                    AdsEnabled.Change -= AdsEnabled_Change;
                    BotPositioning.Change -= BotPositioning_Change;
                    AdInterval.Change -= AdInterval_Change;
                    SetHeading.Hit -= SetHeading_Hit;
                    DefaultHeading.Change -= DefaultHeading_Change;
                    Verbosity.Changed -= Verbosity_Changed;
                    ManaThreshold.Changed -= ManaThreshhold_Changed;
                    StaminaThreshold.Changed -= StaminaThreshhold_Changed;
                    LeadScarabThreshold.Change -= LeadScarabThreshold_Change;
                    IronScarabThreshold.Change -= IronScarabThreshold_Change;
                    CopperScarabThreshold.Change -= CopperScarabThreshold_Change;
                    SilverScarabThreshold.Change -= SilverScarabThreshold_Change;
                    GoldScarabThreshold.Change -= GoldScarabThreshold_Change;
                    PyrealScarabThreshold.Change -= PyrealScarabThreshold_Change;
                    PlatinumScarabThreshold.Change -= PlatinumScarabThreshold_Change;
                    ManaScarabThreshold.Change -= ManaScarabThreshold_Change;
                    ComponentThreshold.Change -= ComponentThreshold_Change;
                    #endregion

                    #region Remove Portal EventHandlers
                    CharacterChoice.Change -= CharacterChoice_Change;
                    PrimaryKeyword.Change -= PrimaryKeyword_Change;
                    SecondaryKeyword.Change -= SecondaryKeyword_Change;
                    PrimaryDescription.Change -= PrimaryDescription_Change;
                    SecondaryDescription.Change -= SecondaryDescription_Change;
                    PrimaryHeading.Change -= PrimaryHeading_Change;
                    SecondaryHeading.Change -= SecondaryHeading_Change;
                    PrimaryLevel.Change -= PrimaryLevel_Change;
                    SecondaryLevel.Change -= SecondaryLevel_Change;
                    #endregion

                    #region Remove Advertisement EventHandlers
                    AddAdvertisement.Hit -= AddAdvertisement_Hit;
                    Advertisements.Click -= Advertisements_Click;
                    #endregion

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
