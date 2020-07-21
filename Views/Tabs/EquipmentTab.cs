using ACManager.Settings;
using Decal.Adapter.Wrappers;
using System;
using VirindiViewService.Controls;

namespace ACManager.Views.Tabs
{
    internal class EquipmentTab : IDisposable
    {
        private BotManagerView Parent { get; set; }
        private HudList IdleEquipmentList { get; set; }
        private HudButton IdleAddEquipment { get; set; }
        private HudList BuffEquipmentList { get; set; }
        private HudButton BuffAddEquipment { get; set; }

        private bool disposedValue;

        public EquipmentTab(BotManagerView parent)
        {
            Parent = parent;

            IdleEquipmentList = Parent.View != null ? (HudList)Parent.View["IdleEquipmentList"] : new HudList();
            IdleEquipmentList.Click += IdleEquipmentList_Click;

            IdleAddEquipment = Parent.View != null ? (HudButton)Parent.View["IdleAddEquipment"] : new HudButton();
            IdleAddEquipment.Hit += IdleAddEquipment_Hit;

            BuffEquipmentList = Parent.View != null ? (HudList)Parent.View["BuffEquipmentList"] : new HudList();
            BuffEquipmentList.Click += BuffEquipmentList_Click;

            BuffAddEquipment = Parent.View != null ? (HudButton)Parent.View["BuffAddEquipment"] : new HudButton();
            BuffAddEquipment.Hit += BuffAddEquipment_Hit;

            LoadSuits();
        }

        private void LoadSuits()
        {
            try
            {
                foreach (Equipment item in Parent.Machine.Utility.EquipmentSettings.IdleEquipment)
                {
                    HudList.HudListRowAccessor row = IdleEquipmentList.AddRow();
                    using (HudStaticText newItem = new HudStaticText())
                    {
                        newItem.Text = item.Name;
                        row[0] = newItem;
                    }
                }

                foreach (Equipment item in Parent.Machine.Utility.EquipmentSettings.BuffingEquipment)
                {
                    HudList.HudListRowAccessor row = BuffEquipmentList.AddRow();
                    using (HudStaticText newItem = new HudStaticText())
                    {
                        newItem.Text = item.Name;
                        row[0] = newItem;
                    }
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void BuffAddEquipment_Hit(object sender, EventArgs e)
        {
            try
            {
                if (Parent.Machine.FinishedInitialScan)
                {
                    if (Parent.Machine.Core.CharacterFilter.Name.Equals(Parent.Machine.BuffingCharacter))
                    {
                        if (!Parent.Machine.Core.Actions.CurrentSelection.Equals(0) && Parent.Machine.Core.CharacterFilter.Name.Equals(Parent.Machine.BuffingCharacter))
                        {
                            foreach (WorldObject item in Parent.Machine.CharacterEquipment)
                            {
                                if (item.Id.Equals(Parent.Machine.Core.Actions.CurrentSelection))
                                {
                                    Equipment newEquipment = new Equipment
                                    {
                                        Id = item.Id,
                                        Name = item.Name,
                                        EquipMask = item.Values(LongValueKey.EquipableSlots),
                                        ObjectClass = item.ObjectClass.ToString()
                                    };

                                    if (!Parent.Machine.Utility.EquipmentSettings.BuffingEquipment.Contains(newEquipment))
                                    {
                                        HudList.HudListRowAccessor row = BuffEquipmentList.AddRow();
                                        using (HudStaticText control = new HudStaticText())
                                        {
                                            control.Text = item.Name;
                                            row[0] = control;
                                        }

                                        Parent.Machine.Utility.EquipmentSettings.BuffingEquipment.Add(newEquipment);
                                        Parent.Machine.Utility.SaveEquipmentSettings();
                                    }
                                    else
                                    {
                                        Debug.ToChat("This item is already in the buffing suit.");
                                    }

                                    break;
                                }
                            }
                        }
                        else
                        {
                            Debug.ToChat("Make sure you have an item selected.");
                        }
                    }
                    else
                    {
                        Debug.ToChat("You must set the buffing character to this character, or be logged into the buffing character to add equipment.");
                    }
                }
                else
                {
                    Debug.ToChat("Please wait until finished scanning your inventory.");
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void BuffEquipmentList_Click(object sender, int row, int col)
        {
            try
            {
                using (HudStaticText equipmentToDelete = (HudStaticText)BuffEquipmentList[row][0])
                {
                    BuffEquipmentList.RemoveRow(row);
                    foreach (Equipment item in Parent.Machine.Utility.EquipmentSettings.BuffingEquipment)
                    {
                        if (item.Name.Equals(equipmentToDelete.Text))
                        {
                            Parent.Machine.Utility.EquipmentSettings.BuffingEquipment.Remove(item);
                            break;
                        }
                    }
                }
                Parent.Machine.Utility.SaveEquipmentSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void IdleAddEquipment_Hit(object sender, EventArgs e)
        {
            try
            {
                if (Parent.Machine.FinishedInitialScan) 
                {
                    if (Parent.Machine.Core.CharacterFilter.Name.Equals(Parent.Machine.BuffingCharacter))
                    {
                        if (!Parent.Machine.Core.Actions.CurrentSelection.Equals(0))
                        {
                            foreach (WorldObject item in Parent.Machine.CharacterEquipment)
                            {
                                if (item.Id.Equals(Parent.Machine.Core.Actions.CurrentSelection))
                                {
                                    Equipment newEquipment = new Equipment
                                    {
                                        Id = item.Id,
                                        Name = item.Name,
                                        EquipMask = item.Values(LongValueKey.EquipableSlots),
                                        ObjectClass = item.ObjectClass.ToString()
                                    };

                                    if (!Parent.Machine.Utility.EquipmentSettings.IdleEquipment.Contains(newEquipment))
                                    {
                                        HudList.HudListRowAccessor row = IdleEquipmentList.AddRow();
                                        using (HudStaticText control = new HudStaticText())
                                        {
                                            control.Text = item.Name;
                                            row[0] = control;
                                        }
                                        Parent.Machine.Utility.EquipmentSettings.IdleEquipment.Add(newEquipment);
                                        Parent.Machine.Utility.SaveEquipmentSettings();
                                    }
                                    else
                                    {
                                        Debug.ToChat("This item is already in the idle suit.");
                                    }

                                    break;
                                }
                            }
                        }
                        else
                        {
                            Debug.ToChat("Make sure you are selecting an item in your own inventory.");
                        }
                    }
                    else
                    {
                        Debug.ToChat("You must set the buffing character to this character, or be logged into the buffing character to add equipment.");
                    }
                }
                else
                {
                    Debug.ToChat("Please wait until finished scanning your inventory.");
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void IdleEquipmentList_Click(object sender, int row, int col)
        {
            try
            {
                using (HudStaticText equipmentToDelete = (HudStaticText)IdleEquipmentList[row][0])
                {
                    IdleEquipmentList.RemoveRow(row);
                    foreach (Equipment item in Parent.Machine.Utility.EquipmentSettings.IdleEquipment)
                    {
                        if (item.Name.Equals(equipmentToDelete.Text))
                        {
                            Parent.Machine.Utility.EquipmentSettings.IdleEquipment.Remove(item);
                            break;
                        }
                    }
                }
                Parent.Machine.Utility.SaveEquipmentSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    IdleEquipmentList.Click -= IdleEquipmentList_Click;
                    IdleAddEquipment.Hit -= IdleAddEquipment_Hit;
                    BuffEquipmentList.Click -= BuffEquipmentList_Click;
                    BuffAddEquipment.Hit -= BuffAddEquipment_Hit;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
