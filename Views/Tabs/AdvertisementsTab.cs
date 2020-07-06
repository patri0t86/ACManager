using ACManager.Settings;
using System;
using VirindiViewService.Controls;

namespace ACManager.Views.Tabs
{
    internal class AdvertisementsTab : IDisposable
    {
        private BotManagerView Parent { get; set; }
        private HudList Advertisements { get; set; }
        private HudTextBox NewAdvertisement { get; set; }
        private HudButton AddAdvertisement { get; set; }
        private bool disposedValue;

        public AdvertisementsTab(BotManagerView parent)
        {
            Parent = parent;

            NewAdvertisement = Parent.View != null ? (HudTextBox)Parent.View["NewAdvertisementText"] : new HudTextBox();

            AddAdvertisement = Parent.View != null ? (HudButton)Parent.View["AddAdvertisement"] : new HudButton();
            AddAdvertisement.Hit += AddAdvertisement_Hit;

            Advertisements = Parent.View != null ? (HudList)Parent.View["AdvertisementList"] : new HudList();
            Advertisements.Click += Advertisements_Click;

            GetAdvertisements();
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
                        Parent.Machine.Utility.BotSettings.Advertisements.Add(advertisement);
                        Parent.Machine.Utility.SaveBotSettings();
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

                for (int i = 0; i < Parent.Machine.Utility.BotSettings.Advertisements.Count; i++)
                {
                    if (Parent.Machine.Utility.BotSettings.Advertisements[i].Message.Equals(adToDelete.Text))
                    {
                        Parent.Machine.Utility.BotSettings.Advertisements.RemoveAt(i);
                        break;
                    }
                }

                Parent.Machine.Utility.SaveBotSettings();
                adToDelete.Dispose();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void GetAdvertisements()
        {
            for (int i = 0; i < Parent.Machine.Utility.BotSettings.Advertisements.Count; i++)
            {
                HudList.HudListRowAccessor rowAccessor = Advertisements.AddRow();
                using (HudStaticText row = new HudStaticText())
                {
                    row.Text = Parent.Machine.Utility.BotSettings.Advertisements[i].Message;
                    rowAccessor[0] = row;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    AddAdvertisement.Hit -= AddAdvertisement_Hit;
                    Advertisements.Click -= Advertisements_Click;
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
