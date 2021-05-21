using ACManager.Settings;
using System;
using VirindiViewService;
using VirindiViewService.Controls;

namespace ACManager.Views.Tabs
{
    public class InventoryTab : IDisposable
    {
        private BotManagerView Parent { get; set; }
        private HudCombo CharacterChoice { get; set; }
        private HudList InventoryList { get; set; }
        private HudTextBox LeadScarabThreshold { get; set; }
        private HudTextBox IronScarabThreshold { get; set; }
        private HudTextBox CopperScarabThreshold { get; set; }
        private HudTextBox SilverScarabThreshold { get; set; }
        private HudTextBox GoldScarabThreshold { get; set; }
        private HudTextBox PyrealScarabThreshold { get; set; }
        private HudTextBox PlatinumScarabThreshold { get; set; }
        private HudTextBox ManaScarabThreshold { get; set; }
        private HudTextBox ComponentThreshold { get; set; }
        private bool disposedValue;

        public InventoryTab(BotManagerView parent)
        {
            Parent = parent;

            CharacterChoice = Parent.View != null ? (HudCombo)Parent.View["CharacterInventoryChoice"] : new HudCombo(new ControlGroup());
            CharacterChoice.Change += CharacterChoice_Change;

            InventoryList = Parent.View != null ? (HudList)Parent.View["InventoryList"] : new HudList();

            LeadScarabThreshold = Parent.View != null ? (HudTextBox)Parent.View["LeadScarabThreshold"] : new HudTextBox();
            LeadScarabThreshold.Change += LeadScarabThreshold_Change;

            IronScarabThreshold = Parent.View != null ? (HudTextBox)Parent.View["IronScarabThreshold"] : new HudTextBox();
            IronScarabThreshold.Change += IronScarabThreshold_Change;

            CopperScarabThreshold = Parent.View != null ? (HudTextBox)Parent.View["CopperScarabThreshold"] : new HudTextBox();
            CopperScarabThreshold.Change += CopperScarabThreshold_Change;

            SilverScarabThreshold = Parent.View != null ? (HudTextBox)Parent.View["SilverScarabThreshold"] : new HudTextBox();
            SilverScarabThreshold.Change += SilverScarabThreshold_Change;

            GoldScarabThreshold = Parent.View != null ? (HudTextBox)Parent.View["GoldScarabThreshold"] : new HudTextBox();
            GoldScarabThreshold.Change += GoldScarabThreshold_Change;

            PyrealScarabThreshold = Parent.View != null ? (HudTextBox)Parent.View["PyrealScarabThreshold"] : new HudTextBox();
            PyrealScarabThreshold.Change += PyrealScarabThreshold_Change;

            PlatinumScarabThreshold = Parent.View != null ? (HudTextBox)Parent.View["PlatinumScarabThreshold"] : new HudTextBox();
            PlatinumScarabThreshold.Change += PlatinumScarabThreshold_Change;

            ManaScarabThreshold = Parent.View != null ? (HudTextBox)Parent.View["ManaScarabThreshold"] : new HudTextBox();
            ManaScarabThreshold.Change += ManaScarabThreshold_Change;

            ComponentThreshold = Parent.View != null ? (HudTextBox)Parent.View["ComponentThreshold"] : new HudTextBox();
            ComponentThreshold.Change += ComponentThreshold_Change;

            SetupInventoryColumns();
            PopulateCharacterChoice();
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                LeadScarabThreshold.Text = Utility.BotSettings.LeadScarabThreshold.ToString();
                IronScarabThreshold.Text = Utility.BotSettings.IronScarabThreshold.ToString();
                CopperScarabThreshold.Text = Utility.BotSettings.CopperScarabThreshold.ToString();
                SilverScarabThreshold.Text = Utility.BotSettings.SilverScarabThreshold.ToString();
                GoldScarabThreshold.Text = Utility.BotSettings.GoldScarabThreshold.ToString();
                PyrealScarabThreshold.Text = Utility.BotSettings.PyrealScarabThreshold.ToString();
                PlatinumScarabThreshold.Text = Utility.BotSettings.PlatinumScarabThreshold.ToString();
                ManaScarabThreshold.Text = Utility.BotSettings.ManaScarabThreshold.ToString();
                ComponentThreshold.Text = Utility.BotSettings.ComponentThreshold.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void CharacterChoice_Change(object sender, System.EventArgs e)
        {
            SetupInventoryColumns();
            if (!CharacterChoice.Current.Equals(0))
            {
                using (HudStaticText selectedCharacter = (HudStaticText)CharacterChoice[CharacterChoice.Current])
                {
                    CharacterInventory inventory = GetInventory(selectedCharacter.Text);
                    if (inventory != null)
                    {
                        foreach (AcmComponent component in inventory.Components)
                        {
                            HudList.HudListRowAccessor row = InventoryList.AddRow();
                            using (HudStaticText name = new HudStaticText())
                            {
                                name.Text = component.Name;
                                row[0] = name;
                            }

                            using (HudStaticText quantity = new HudStaticText())
                            {
                                quantity.Text = component.Quantity.ToString();
                                row[1] = quantity;
                            }
                        }

                        foreach (Gem gem in inventory.Gems)
                        {
                            HudList.HudListRowAccessor row = InventoryList.AddRow();
                            using (HudStaticText name = new HudStaticText())
                            {
                                name.Text = gem.Name;
                                row[0] = name;
                            }

                            using (HudStaticText quantity = new HudStaticText())
                            {
                                quantity.Text = gem.Quantity.ToString();
                                row[1] = quantity;
                            }
                        }
                    }
                }
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
                    Utility.BotSettings.LeadScarabThreshold = result;
                }
                else
                {
                    LeadScarabThreshold.Text = "0";
                    Utility.BotSettings.LeadScarabThreshold = 0;
                }
                Utility.SaveBotSettings();
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
                    Utility.BotSettings.IronScarabThreshold = result;
                }
                else
                {
                    IronScarabThreshold.Text = "0";
                    Utility.BotSettings.IronScarabThreshold = 0;
                }
                Utility.SaveBotSettings();
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
                    Utility.BotSettings.CopperScarabThreshold = result;
                }
                else
                {
                    CopperScarabThreshold.Text = "0";
                    Utility.BotSettings.CopperScarabThreshold = 0;
                }
                Utility.SaveBotSettings();
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
                    Utility.BotSettings.PyrealScarabThreshold = result;
                }
                else
                {
                    PyrealScarabThreshold.Text = "0";
                    Utility.BotSettings.PyrealScarabThreshold = 0;
                }
                Utility.SaveBotSettings();
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
                    Utility.BotSettings.SilverScarabThreshold = result;
                }
                else
                {
                    SilverScarabThreshold.Text = "0";
                    Utility.BotSettings.SilverScarabThreshold = 0;
                }
                Utility.SaveBotSettings();
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
                    Utility.BotSettings.GoldScarabThreshold = result;
                }
                else
                {
                    GoldScarabThreshold.Text = "0";
                    Utility.BotSettings.GoldScarabThreshold = 0;
                }
                Utility.SaveBotSettings();
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
                    Utility.BotSettings.PlatinumScarabThreshold = result;
                }
                else
                {
                    PlatinumScarabThreshold.Text = "0";
                    Utility.BotSettings.PlatinumScarabThreshold = 0;
                }
                Utility.SaveBotSettings();
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
                    Utility.BotSettings.ManaScarabThreshold = result;
                }
                else
                {
                    ManaScarabThreshold.Text = "0";
                    Utility.BotSettings.ManaScarabThreshold = 0;
                }
                Utility.SaveBotSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
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
                    Utility.BotSettings.ComponentThreshold = result;
                }
                else
                {
                    ComponentThreshold.Text = "0";
                    Utility.BotSettings.ComponentThreshold = 0;
                }
                Utility.SaveBotSettings();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void SetupInventoryColumns()
        {
            try
            {
                InventoryList.ClearRows();
                HudList.HudListRowAccessor row = InventoryList.AddRow();
                using (HudStaticText text = new HudStaticText())
                {
                    text.Text = "Item";
                    row[0] = text;
                }

                using (HudStaticText text = new HudStaticText())
                {
                    text.Text = "Quantity";
                    row[1] = text;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void PopulateCharacterChoice()
        {
            CharacterChoice.AddItem("Select character...", null);
            for (int i = 0; i < FilterCore.AccountCharacters.Count; i++)
            {
                CharacterChoice.AddItem(FilterCore.AccountCharacters[i], null);
            }
        }

        private CharacterInventory GetInventory(string name)
        {
            foreach (CharacterInventory characterInventory in Utility.Inventory.CharacterInventories)
            {
                if (characterInventory.Name.Equals(name))
                {
                    return characterInventory;
                }
            }
            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CharacterChoice.Change -= CharacterChoice_Change;
                    LeadScarabThreshold.Change -= LeadScarabThreshold_Change;
                    IronScarabThreshold.Change -= IronScarabThreshold_Change;
                    CopperScarabThreshold.Change -= CopperScarabThreshold_Change;
                    SilverScarabThreshold.Change -= SilverScarabThreshold_Change;
                    GoldScarabThreshold.Change -= GoldScarabThreshold_Change;
                    PyrealScarabThreshold.Change -= PyrealScarabThreshold_Change;
                    PlatinumScarabThreshold.Change -= PlatinumScarabThreshold_Change;
                    ManaScarabThreshold.Change -= ManaScarabThreshold_Change;
                    ComponentThreshold.Change -= ComponentThreshold_Change;
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
