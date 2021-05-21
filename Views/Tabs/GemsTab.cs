using ACManager.Settings;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using VirindiViewService.Controls;

namespace ACManager.Views.Tabs
{
    public class GemsTab : IDisposable
    {
        private BotManagerView Parent { get; set; }
        private HudList Gems { get; set; }
        private HudTextBox GemKeyword { get; set; }
        private HudTextBox GemHeading { get; set; }
        private HudButton AddGem { get; set; }
        private bool disposedValue;

        public GemsTab(BotManagerView parent)
        {
            Parent = parent;

            Gems = Parent.View != null ? (HudList)Parent.View["GemList"] : new HudList();
            Gems.Click += Gems_Click;

            GemKeyword = Parent.View != null ? (HudTextBox)Parent.View["GemKeyword"] : new HudTextBox();

            GemHeading = Parent.View != null ? (HudTextBox)Parent.View["GemHeading"] : new HudTextBox();
            GemHeading.Change += GemHeading_Change;

            AddGem = Parent.View != null ? (HudButton)Parent.View["AddGem"] : new HudButton();
            AddGem.Hit += AddGem_Hit;

            GetGems();
        }

        private void Gems_Click(object sender, int row, int col)
        {
            try
            {
                if (!row.Equals(0))
                {
                    HudStaticText gemToDelete = (HudStaticText)Gems[row][0];
                    Gems.RemoveRow(row);

                    for (int i = 0; i < Utility.BotSettings.GemSettings.Count; i++)
                    {
                        if (Utility.BotSettings.GemSettings[i].Name.Equals(gemToDelete.Text))
                        {
                            Utility.BotSettings.GemSettings.RemoveAt(i);
                            break;
                        }
                    }

                    Utility.SaveBotSettings();
                    gemToDelete.Dispose();
                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void GetGems()
        {
            SetupGemsList();
            foreach (GemSetting gemSetting in Utility.BotSettings.GemSettings)
            {
                HudList.HudListRowAccessor row = Gems.AddRow();
                using (HudStaticText text = new HudStaticText())
                {
                    text.Text = gemSetting.Name;
                    row[0] = text;
                }

                using (HudStaticText text = new HudStaticText())
                {
                    text.Text = gemSetting.Keyword;
                    row[1] = text;
                }

                using (HudStaticText text = new HudStaticText())
                {
                    text.Text = gemSetting.Heading.ToString();
                    row[2] = text;
                }
            }
        }

        private void AddGem_Hit(object sender, EventArgs e)
        {
            try
            {
                if (CoreManager.Current.Actions.CurrentSelection != 0 && CoreManager.Current.WorldFilter[CoreManager.Current.Actions.CurrentSelection] != null)
                {
                    WorldObject worldObject = CoreManager.Current.WorldFilter[CoreManager.Current.Actions.CurrentSelection];
                    if (worldObject.ObjectClass.Equals(ObjectClass.Gem))
                    {
                        if (!string.IsNullOrEmpty(GemKeyword.Text))
                        {
                            if (!string.IsNullOrEmpty(GemHeading.Text))
                            {
                                GemSetting gemSetting = new GemSetting
                                {
                                    Name = CoreManager.Current.WorldFilter[CoreManager.Current.Actions.CurrentSelection].Name,
                                    Keyword = GemKeyword.Text,
                                    Heading = double.Parse(GemHeading.Text)
                                };

                                if (Utility.BotSettings.GemSettings.Contains(gemSetting))
                                {
                                    for (int i = 0; i < Utility.BotSettings.GemSettings.Count; i++)
                                    {
                                        if (Utility.BotSettings.GemSettings[i].Equals(gemSetting))
                                        {
                                            Utility.BotSettings.GemSettings[i] = gemSetting;
                                        }
                                    }

                                    Gems.ClearRows();
                                    GetGems();
                                }
                                else
                                {
                                    Utility.BotSettings.GemSettings.Add(gemSetting);

                                    HudList.HudListRowAccessor row = Gems.AddRow();
                                    using (HudStaticText text = new HudStaticText())
                                    {
                                        text.Text = CoreManager.Current.WorldFilter[CoreManager.Current.Actions.CurrentSelection].Name;
                                        row[0] = text;
                                    }

                                    using (HudStaticText text = new HudStaticText())
                                    {
                                        text.Text = GemKeyword.Text;
                                        row[1] = text;
                                    }
                                    using (HudStaticText text = new HudStaticText())
                                    {
                                        text.Text = GemHeading.Text;
                                        row[2] = text;
                                    }
                                }

                                Utility.SaveBotSettings();

                                GemKeyword.Text = "";
                                GemHeading.Text = "";
                            }
                            else
                            {
                                Debug.ToChat("Please enter a heading.");
                            }
                        }
                        else
                        {
                            Debug.ToChat("Please enter a keyword.");

                        }
                    }
                    else
                    {
                        Debug.ToChat("That is not recognized as a gem. Please select a portal gem.");

                    }
                }
                else
                {
                    Debug.ToChat("Please select a portal gem.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void GemHeading_Change(object sender, EventArgs e)
        {
            try
            {
                if (double.TryParse(GemHeading.Text, out double result))
                {
                    if (result >= 360)
                    {
                        GemHeading.Text = "0";
                        result = 0;
                    }
                    else if (result <= -1)
                    {
                        result = 0;
                    }
                    GemHeading.Text = result.ToString();
                }
                else
                {
                    GemHeading.Text = "0";
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void SetupGemsList()
        {
            try
            {
                HudList.HudListRowAccessor row = Gems.AddRow();
                using (HudStaticText text = new HudStaticText())
                {
                    text.Text = "Gem";
                    row[0] = text;
                }

                using (HudStaticText text = new HudStaticText())
                {
                    text.Text = "Keyword";
                    row[1] = text;
                }
                using (HudStaticText text = new HudStaticText())
                {
                    text.Text = "Heading";
                    row[2] = text;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Gems.Click -= Gems_Click;
                    GemHeading.Change -= GemHeading_Change;
                    AddGem.Hit -= AddGem_Hit;
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
