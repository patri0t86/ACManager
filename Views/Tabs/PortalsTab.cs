using ACManager.Settings;
using System;
using VirindiViewService;
using VirindiViewService.Controls;

namespace ACManager.Views.Tabs
{
    internal class PortalsTab : IDisposable
    {
        private BotManagerView Parent { get; set; }
        private HudCombo CharacterChoice { get; set; }
        private HudTextBox PrimaryKeyword { get; set; }
        private HudTextBox SecondaryKeyword { get; set; }
        private HudTextBox PrimaryDescription { get; set; }
        private HudTextBox SecondaryDescription { get; set; }
        private HudTextBox PrimaryHeading { get; set; }
        private HudTextBox SecondaryHeading { get; set; }
        private HudTextBox PrimaryLevel { get; set; }
        private HudTextBox SecondaryLevel { get; set; }
        private bool disposedValue;

        public PortalsTab(BotManagerView parent)
        {
            Parent = parent;
            PrimaryKeyword = Parent.View != null ? (HudTextBox)Parent.View["PrimaryKeyword"] : new HudTextBox();
            PrimaryKeyword.Change += PrimaryKeyword_Change;

            SecondaryKeyword = Parent.View != null ? (HudTextBox)Parent.View["SecondaryKeyword"] : new HudTextBox();
            SecondaryKeyword.Change += SecondaryKeyword_Change;

            PrimaryDescription = Parent.View != null ? (HudTextBox)Parent.View["PrimaryDescription"] : new HudTextBox();
            PrimaryDescription.Change += PrimaryDescription_Change;

            SecondaryDescription = Parent.View != null ? (HudTextBox)Parent.View["SecondaryDescription"] : new HudTextBox();
            SecondaryDescription.Change += SecondaryDescription_Change;

            PrimaryHeading = Parent.View != null ? (HudTextBox)Parent.View["PrimaryHeading"] : new HudTextBox();
            PrimaryHeading.Change += PrimaryHeading_Change;

            SecondaryHeading = Parent.View != null ? (HudTextBox)Parent.View["SecondaryHeading"] : new HudTextBox();
            SecondaryHeading.Change += SecondaryHeading_Change;

            PrimaryLevel = Parent.View != null ? (HudTextBox)Parent.View["PrimaryLevel"] : new HudTextBox();
            PrimaryLevel.Change += PrimaryLevel_Change;

            SecondaryLevel = Parent.View != null ? (HudTextBox)Parent.View["SecondaryLevel"] : new HudTextBox();
            SecondaryLevel.Change += SecondaryLevel_Change;

            CharacterChoice = Parent.View != null ? (HudCombo)Parent.View["CharacterChoice"] : new HudCombo(new ControlGroup());
            CharacterChoice.Change += CharacterChoice_Change;

            PopulateCharacterChoice();
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

        private Character CharacterExistsOrNew(string name)
        {
            Character newCharacter = new Character
            {
                Name = name,
                Account = Parent.Machine.Core.CharacterFilter.AccountName,
                Server = Parent.Machine.Core.CharacterFilter.Server
            };

            if (Parent.Machine.Utility.CharacterSettings.Characters.Contains(newCharacter))
            {
                foreach (Character character in Parent.Machine.Utility.CharacterSettings.Characters)
                {
                    if (newCharacter.Equals(character))
                    {
                        return character;
                    }
                }
            }
            return newCharacter;
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

                    if (character.Portals.Count > 0)
                    {
                        foreach (Portal portal in character.Portals)
                        {
                            if (portal.Type.Equals(PortalType.Primary))
                            {
                                PrimaryKeyword.Text = !string.IsNullOrEmpty(portal.Keyword) ? portal.Keyword : "";
                                PrimaryDescription.Text = !string.IsNullOrEmpty(portal.Description) ? portal.Description : "";
                                PrimaryHeading.Text = portal.Heading.ToString();
                                PrimaryLevel.Text = portal.Level.ToString();
                            }
                            else
                            {
                                SecondaryKeyword.Text = !string.IsNullOrEmpty(portal.Keyword) ? portal.Keyword : "";
                                SecondaryDescription.Text = !string.IsNullOrEmpty(portal.Description) ? portal.Description : "";
                                SecondaryHeading.Text = portal.Heading.ToString();
                                SecondaryLevel.Text = portal.Level.ToString();
                            }
                        }
                    }
                    else
                    {
                        ClearUI();
                    }

                    selectedCharacter.Dispose();
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void PopulateCharacterChoice()
        {
            CharacterChoice.AddItem("Select character...", null);
            for (int i = 0; i < Parent.Machine.AccountCharacters.Count; i++)
            {
                CharacterChoice.AddItem(Parent.Machine.AccountCharacters[i], null);
            }
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

            if (Parent.Machine.Utility.CharacterSettings.Characters.Contains(character))
            {
                for (int i = 0; i < Parent.Machine.Utility.CharacterSettings.Characters.Count; i++)
                {
                    if (Parent.Machine.Utility.CharacterSettings.Characters[i].Equals(character))
                    {
                        Parent.Machine.Utility.CharacterSettings.Characters[i] = character;
                        break;
                    }
                }
            }
            else
            {
                Parent.Machine.Utility.CharacterSettings.Characters.Add(character);
            }

            Parent.Machine.Utility.SaveCharacterSettings();
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

            if (Parent.Machine.Utility.CharacterSettings.Characters.Contains(character))
            {
                for (int i = 0; i < Parent.Machine.Utility.CharacterSettings.Characters.Count; i++)
                {
                    if (Parent.Machine.Utility.CharacterSettings.Characters[i].Equals(character))
                    {
                        Parent.Machine.Utility.CharacterSettings.Characters[i] = character;
                        break;
                    }
                }
            }
            else
            {
                Parent.Machine.Utility.CharacterSettings.Characters.Add(character);
            }

            Parent.Machine.Utility.SaveCharacterSettings();
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

            if (Parent.Machine.Utility.CharacterSettings.Characters.Contains(character))
            {
                for (int i = 0; i < Parent.Machine.Utility.CharacterSettings.Characters.Count; i++)
                {
                    if (Parent.Machine.Utility.CharacterSettings.Characters[i].Equals(character))
                    {
                        Parent.Machine.Utility.CharacterSettings.Characters[i] = character;
                        break;
                    }
                }
            }
            else
            {
                Parent.Machine.Utility.CharacterSettings.Characters.Add(character);
            }

            Parent.Machine.Utility.SaveCharacterSettings();
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

            if (Parent.Machine.Utility.CharacterSettings.Characters.Contains(character))
            {
                for (int i = 0; i < Parent.Machine.Utility.CharacterSettings.Characters.Count; i++)
                {
                    if (Parent.Machine.Utility.CharacterSettings.Characters[i].Equals(character))
                    {
                        Parent.Machine.Utility.CharacterSettings.Characters[i] = character;
                        break;
                    }
                }
            }
            else
            {
                Parent.Machine.Utility.CharacterSettings.Characters.Add(character);
            }
            Parent.Machine.Utility.SaveCharacterSettings();
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CharacterChoice.Change -= CharacterChoice_Change;
                    PrimaryKeyword.Change -= PrimaryKeyword_Change;
                    SecondaryKeyword.Change -= SecondaryKeyword_Change;
                    PrimaryDescription.Change -= PrimaryDescription_Change;
                    SecondaryDescription.Change -= SecondaryDescription_Change;
                    PrimaryHeading.Change -= PrimaryHeading_Change;
                    SecondaryHeading.Change -= SecondaryHeading_Change;
                    PrimaryLevel.Change -= PrimaryLevel_Change;
                    SecondaryLevel.Change -= SecondaryLevel_Change;
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
