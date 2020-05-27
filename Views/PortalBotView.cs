using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;
using System.Xml;
using VirindiViewService;
using VirindiViewService.Controls;

namespace ACManager.Views
{
    internal class PortalBotView
    {
        internal const string Module = "PortalBot";
        internal HudView View { get; set; }
        internal HudCombo CharacterChoice { get; set; }
        internal HudTextBox PrimaryKeyword { get; set; }
        internal HudTextBox SecondaryKeyword { get; set; }
        internal HudTextBox PrimaryDescription { get; set; }
        internal HudTextBox SecondaryDescription { get; set; }
        internal HudTextBox NewAdvertisement { get; set; }
        internal HudList Advertisements { get; set; }
        internal HudButton AddAdvertisement { get; set; }

        public PortalBotView()
        {
            try
            {
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

                AddAdvertisement = View != null ? (HudButton)View["AddAdvertisement"] : new HudButton();
                AddAdvertisement.Hit += AddAdvertisement_Hit;

                Advertisements = View != null ? (HudList)View["AdvertisementList"] : new HudList();
                Advertisements.Click += Advertisements_Click;

                CharacterChoice = View != null ? (HudCombo)View["CharacterChoice"] : new HudCombo(new ControlGroup());
                CharacterChoice.Change += CharacterChoice_Change;

                GetCharacters();
                GetAdvertisements();
            }
            catch { }
        }

        private void PrimaryKeyword_Change(object sender, EventArgs e)
        {
            HudStaticText selectedCharacter = (HudStaticText)CharacterChoice[CharacterChoice.Current];
            Utility.SaveSetting(Module, selectedCharacter.Text, "PrimaryKeyword", PrimaryKeyword.Text.ToLower());
        }

        private void SecondaryKeyword_Change(object sender, EventArgs e)
        {
            HudStaticText selectedCharacter = (HudStaticText)CharacterChoice[CharacterChoice.Current];
            Utility.SaveSetting(Module, selectedCharacter.Text, "SecondaryKeyword", SecondaryKeyword.Text.ToLower());
        }

        private void PrimaryDescription_Change(object sender, EventArgs e)
        {
            HudStaticText selectedCharacter = (HudStaticText)CharacterChoice[CharacterChoice.Current];
            Utility.SaveSetting(Module, selectedCharacter.Text, "PrimaryDescription", PrimaryDescription.Text);
        }

        private void SecondaryDescription_Change(object sender, EventArgs e)
        {
            HudStaticText selectedCharacter = (HudStaticText)CharacterChoice[CharacterChoice.Current];
            Utility.SaveSetting(Module, selectedCharacter.Text, "SecondaryDescription", SecondaryDescription.Text);
        }

        private void AddAdvertisement_Hit(object sender, EventArgs e)
        {
            HudList.HudListRowAccessor row = Advertisements.AddRow();
            HudStaticText control = new HudStaticText();
            control.Text = NewAdvertisement.Text;
            row[0] = control;
            Utility.SaveSetting("PortalBot", characterName: "BotGlobal", $"Ad{DateTime.Now.Ticks}", NewAdvertisement.Text);
            NewAdvertisement.Text = "";
        }

        private void Advertisements_Click(object sender, int row, int col)
        {
            HudList.HudListRowAccessor rowToDelete = Advertisements[row];
            HudStaticText item = (HudStaticText)rowToDelete[row];
            Utility.DeleteSetting("PortalBot", "BotGlobal", item.Text);
            Advertisements.RemoveRow(row);
        }

        private void CharacterChoice_Change(object sender, EventArgs e)
        {
            if (CharacterChoice.Current == 0)
            {
                PrimaryKeyword.Text = "";
                SecondaryKeyword.Text = "";
                PrimaryDescription.Text = "";
                SecondaryDescription.Text = "";
                return;
            }

            HudStaticText selectedCharacter = (HudStaticText)CharacterChoice[CharacterChoice.Current];
            if (selectedCharacter.Text.Contains(" "))
            {
                selectedCharacter.Text = selectedCharacter.Text.Replace(" ", "_");
            }
            PrimaryKeyword.Text = "";
            SecondaryKeyword.Text = "";
            PrimaryDescription.Text = "";
            SecondaryDescription.Text = "";

            XmlNode node = Utility.LoadCharacterSettings(Module, portal: true);
            if (node != null)
            {
                XmlNodeList charNodes = node.ChildNodes;
                if (charNodes.Count > 0)
                {
                    for (int i = 0; i < charNodes.Count; i++)
                    {
                        if (charNodes[i].Name == selectedCharacter.Text)
                        {
                            foreach (XmlNode aNode in charNodes[i])
                            {
                                if (aNode.Name == "PrimaryKeyword")
                                {
                                    PrimaryKeyword.Text = aNode.InnerText;
                                }
                                if (aNode.Name == "SecondaryKeyword")
                                {
                                    SecondaryKeyword.Text = aNode.InnerText;
                                }
                                if (aNode.Name == "PrimaryDescription")
                                {
                                    PrimaryDescription.Text = aNode.InnerText;
                                }
                                if (aNode.Name == "SecondaryDescription")
                                {
                                    SecondaryDescription.Text = aNode.InnerText;
                                }
                            }
                            break;
                        }
                    }
                }
            }
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
            List<string> ads = Utility.GetAdvertisements();
            if (ads != null)
            {
                HudStaticText row;
                for (int i = 0; i < ads.Count; i++)
                {
                    HudList.HudListRowAccessor rowAccessor = Advertisements.AddRow();
                    row = new HudStaticText();
                    row.Text = ads[i];
                    rowAccessor[0] = row;
                }
            }
        }
    }
}
