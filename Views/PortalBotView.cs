using ACManager.Settings;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;
using VirindiViewService;
using VirindiViewService.Controls;

namespace ACManager.Views
{
    internal class PortalBotView : IDisposable
    {
        internal const string Module = "PortalBot";
        internal PluginCore Plugin { get; set; }
        internal HudView View { get; set; }
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
        internal HudList Advertisements { get; set; }
        internal HudButton AddAdvertisement { get; set; }

        public PortalBotView(PluginCore parent)
        {
            try
            {
                Plugin = parent;

                VirindiViewService.XMLParsers.Decal3XMLParser parser = new VirindiViewService.XMLParsers.Decal3XMLParser();
                parser.ParseFromResource("ACManager.Views.portalBotView.xml", out ViewProperties Properties, out ControlGroup Controls);

                View = new HudView(Properties, Controls);
                View.ShowInBar = false;

                PrimaryKeyword = View != null ? (HudTextBox)View["PrimaryKeyword"] : new HudTextBox();
                PrimaryKeyword.Change += PrimaryKeyword_Change;

                SecondaryKeyword = View != null ? (HudTextBox)View["SecondaryKeyword"] : new HudTextBox();
                SecondaryKeyword.Change += SecondaryKeyword_Change;

                PrimaryDescription = View != null ? (HudTextBox)View["PrimaryDescription"] : new HudTextBox();
                PrimaryDescription.Change += PrimaryDescription_Change;

                SecondaryDescription = View != null ? (HudTextBox)View["SecondaryDescription"] : new HudTextBox();
                SecondaryDescription.Change += SecondaryDescription_Change;

                NewAdvertisement = View != null ? (HudTextBox)View["NewAdvertisementText"] : new HudTextBox();

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

                Advertisements = View != null ? (HudList)View["AdvertisementList"] : new HudList();
                Advertisements.Click += Advertisements_Click;

                CharacterChoice = View != null ? (HudCombo)View["CharacterChoice"] : new HudCombo(new ControlGroup());
                CharacterChoice.Change += CharacterChoice_Change;

                GetCharacters();
                GetAdvertisements();
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
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
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
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
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
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
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
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
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
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
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
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
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
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
                Plugin.Utility.LogError(ex);
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
                Plugin.Utility.LogError(ex);
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

                        Plugin.Utility.AllSettings.Advertisements.Add(advertisement);
                        Plugin.Utility.SaveSettings();
                        advertisement = null;
                    }
                }
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void Advertisements_Click(object sender, int row, int col)
        {
            try
            {
                HudStaticText adToDelete = (HudStaticText)Advertisements[row][0];
                Advertisements.RemoveRow(row);

                for (int i = 0; i < Plugin.Utility.AllSettings.Advertisements.Count; i++)
                {
                    if (Plugin.Utility.AllSettings.Advertisements[i].Message.Equals(adToDelete.Text))
                    {
                        Plugin.Utility.AllSettings.Advertisements.RemoveAt(i);
                        break;
                    }
                }

                Plugin.Utility.SaveSettings();
                adToDelete.Dispose();
            }
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
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
            catch (Exception ex) { Plugin.Utility.LogError(ex); }
        }

        private void GetCharacters()
        {
            List<string> characterList = new List<string>();
            IndexedCollection<CharFilterIndex, int, AccountCharInfo> accountChars = CoreManager.Current.CharacterFilter.Characters;
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
            for (int i = 0; i < Plugin.Utility.AllSettings.Advertisements.Count; i++)
            {
                Plugin.Utility.WriteToChat(Plugin.Utility.AllSettings.Advertisements[i].Message);
                HudList.HudListRowAccessor rowAccessor = Advertisements.AddRow();
                row = new HudStaticText
                {
                    Text = Plugin.Utility.AllSettings.Advertisements[i].Message
                };
                rowAccessor[0] = row;
            }
        }

        private Character CharacterExistsOrNew(string name)
        {
            Character character = new Character
            {
                Name = name,
                Account = CoreManager.Current.CharacterFilter.AccountName,
                Server = CoreManager.Current.CharacterFilter.Server
            };

            if (Plugin.Utility.AllSettings.Characters.Contains(character))
            {
                foreach (Character ch in Plugin.Utility.AllSettings.Characters)
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

            if (Plugin.Utility.AllSettings.Characters.Contains(character))
            {
                for (int i = 0; i < Plugin.Utility.AllSettings.Characters.Count; i++)
                {
                    if (Plugin.Utility.AllSettings.Characters[i].Equals(character))
                    {
                        Plugin.Utility.AllSettings.Characters[i] = character;
                        break;
                    }
                }
            }
            else
            {
                Plugin.Utility.AllSettings.Characters.Add(character);
            }

            Plugin.Utility.SaveSettings();
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

            if (Plugin.Utility.AllSettings.Characters.Contains(character))
            {
                for (int i = 0; i < Plugin.Utility.AllSettings.Characters.Count; i++)
                {
                    if (Plugin.Utility.AllSettings.Characters[i].Equals(character))
                    {
                        Plugin.Utility.AllSettings.Characters[i] = character;
                        break;
                    }
                }
            }
            else
            {
                Plugin.Utility.AllSettings.Characters.Add(character);
            }

            Plugin.Utility.SaveSettings();
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

            if (Plugin.Utility.AllSettings.Characters.Contains(character))
            {
                for (int i = 0; i < Plugin.Utility.AllSettings.Characters.Count; i++)
                {
                    if (Plugin.Utility.AllSettings.Characters[i].Equals(character))
                    {
                        Plugin.Utility.AllSettings.Characters[i] = character;
                        break;
                    }
                }
            }
            else
            {
                Plugin.Utility.AllSettings.Characters.Add(character);
            }

            Plugin.Utility.SaveSettings();
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

            if (Plugin.Utility.AllSettings.Characters.Contains(character))
            {
                for (int i = 0; i < Plugin.Utility.AllSettings.Characters.Count; i++)
                {
                    if (Plugin.Utility.AllSettings.Characters[i].Equals(character))
                    {
                        Plugin.Utility.AllSettings.Characters[i] = character;
                        break;
                    }
                }
            }
            else
            {
                Plugin.Utility.AllSettings.Characters.Add(character);
            }
            Plugin.Utility.SaveSettings();
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
                    PrimaryKeyword.Change -= PrimaryKeyword_Change;
                    SecondaryKeyword.Change -= SecondaryKeyword_Change;
                    PrimaryDescription.Change -= PrimaryDescription_Change;
                    SecondaryDescription.Change -= SecondaryDescription_Change;
                    PrimaryHeading.Change -= PrimaryHeading_Change;
                    SecondaryHeading.Change -= SecondaryHeading_Change;
                    PrimaryLevel.Change -= PrimaryLevel_Change;
                    SecondaryLevel.Change -= SecondaryLevel_Change;
                    AddAdvertisement.Hit -= AddAdvertisement_Hit;
                    Advertisements.Click -= Advertisements_Click;
                    CharacterChoice.Change -= CharacterChoice_Change;
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
