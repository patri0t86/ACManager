using ACManager.StateMachine;
using ACManager.Views.Tabs;
using System;
using VirindiViewService;

namespace ACManager.Views
{
    internal class BotManagerView : IDisposable
    {
        internal Machine Machine { get; set; }
        internal HudView View { get; set; }
        private ConfigTab ConfigTab { get; set; }
        private GemsTab GemsTab { get; set; }
        private PortalsTab PortalsTab { get; set; }
        private AdvertisementsTab AdvertisementsTab { get; set; }
        private InventoryTab InventoryTab { get; set; }
        private EquipmentTab EquipmentTab { get; set; }

        public BotManagerView(Machine machine)
        {
            try
            {
                Machine = machine;
                VirindiViewService.XMLParsers.Decal3XMLParser parser = new VirindiViewService.XMLParsers.Decal3XMLParser();
                parser.ParseFromResource("ACManager.Views.botManagerView.xml", out ViewProperties Properties, out ControlGroup Controls);

                View = new HudView(Properties, Controls);

                ConfigTab = new ConfigTab(this);
                PortalsTab = new PortalsTab(this);
                GemsTab = new GemsTab(this);
                AdvertisementsTab = new AdvertisementsTab(this);
                InventoryTab = new InventoryTab(this);
                EquipmentTab = new EquipmentTab(this);

                LoadSettings();
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        private void LoadSettings()
        {
            try
            {
                View.ShowInBar = Machine.Utility.GUISettings.BotConfigVisible;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ConfigTab?.Dispose();
                    GemsTab?.Dispose();
                    PortalsTab?.Dispose();
                    AdvertisementsTab?.Dispose();
                    InventoryTab?.Dispose();
                    EquipmentTab?.Dispose();
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
