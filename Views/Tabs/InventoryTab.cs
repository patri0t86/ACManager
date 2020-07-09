using ACManager.Settings;
using System;
using VirindiViewService;
using VirindiViewService.Controls;

namespace ACManager.Views.Tabs
{
    internal class InventoryTab : IDisposable
    {
        private BotManagerView Parent { get; set; }
        private HudCombo CharacterChoice { get; set; }
        private HudList InventoryList { get; set; }
        private bool disposedValue;

        public InventoryTab(BotManagerView parent)
        {
            Parent = parent;

            CharacterChoice = Parent.View != null ? (HudCombo)Parent.View["CharacterInventoryChoice"] : new HudCombo(new ControlGroup());
            CharacterChoice.Change += CharacterChoice_Change;

            InventoryList = Parent.View != null ? (HudList)Parent.View["InventoryList"] : new HudList();

            SetupInventoryColumns();
            PopulateCharacterChoice();
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
            for (int i = 0; i < Parent.Machine.AccountCharacters.Count; i++)
            {
                CharacterChoice.AddItem(Parent.Machine.AccountCharacters[i], null);
            }
        }

        private CharacterInventory GetInventory(string name)
        {
            foreach (CharacterInventory characterInventory in Parent.Machine.Utility.Inventory.CharacterInventories)
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
