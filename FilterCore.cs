using ACManager.Settings;
using ACManager.StateMachine;
using ACManager.StateMachine.States;
using ACManager.Views;
using Decal.Adapter;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACManager
{
    [FriendlyName("ACManager Filter")]
    public class FilterCore : FilterBase
    {
        internal Machine Machine { get; set; }
        internal MainView MainView { get; set; }
        internal FellowshipControl FellowshipControl { get; set; }
        internal InventoryTracker InventoryTracker { get; set; }
        internal ExpTrackerView ExpTrackerView { get; set; }
        internal ExpTracker ExpTracker { get; set; }
        internal PortalBotView PortalBotView { get; set; }
        private Timer LogonTimer { get; set; }

        /// <summary>
        /// Temporary holder for list of account characters until machine can be initialized.
        /// </summary>
        public List<string> AccountCharacters { get; set; } = new List<string>();

        /// <summary>
        /// Temporary holder for total character slots allowed until machine can be initialized.
        /// </summary>
        public int TotalSlots { get; set; }

        /// <summary>
        /// Called only when the client is completely closed (exited out of the character selection screen even).
        /// </summary>
        protected override void Shutdown()
        {
            ServerDispatch -= FilterCore_ServerDispatch;
            CommandLineText -= Machine.Interpreter.Command;
            ClientDispatch -= FilterCore_ClientDispatch;
            LogonTimer.Dispose();
        }

        /// <summary>
        /// Called as soon as the client is launched from whatever launcher you're using.
        /// </summary>
        protected override void Startup()
        {
            ServerDispatch += FilterCore_ServerDispatch;
            ClientDispatch += FilterCore_ClientDispatch;

            LogonTimer = new Timer
            {
                Interval = 1000
            };
            LogonTimer.Tick += LogonTimer_Tick;
        }

        /// <summary>
        /// Parses the client messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterCore_ClientDispatch(object sender, NetworkMessageEventArgs e)
        {
            if (e.Message.Type == 0xF7B1)
            {
                if (e.Message.Value<int>("action") == 0x00A1) // Materialize character
                {
                    if (Machine == null)
                    {
                        Debug.Init(Path);
                        Machine = new Machine(Core, Path);
                        CommandLineText += Machine.Interpreter.Command;
                    }

                    if (!Machine.LoggedIn)
                    {
                        Machine.AccountCharacters = AccountCharacters;
                        Machine.TotalSlots = TotalSlots;
                        Machine.Core = Core;
                        Machine.Utility.CharacterSettings = Machine.Utility.LoadCharacterSettings();

                        Machine.CurrentCharacter = new Character
                        {
                            Name = Core.CharacterFilter.Name,
                            Account = Core.CharacterFilter.AccountName,
                            Server = Core.CharacterFilter.Server
                        };

                        if (Machine.Utility.CharacterSettings.Characters.Contains(Machine.CurrentCharacter))
                        {
                            foreach (Character character in Machine.Utility.CharacterSettings.Characters)
                            {
                                if (Machine.CurrentCharacter.Equals(character))
                                {
                                    Machine.CurrentCharacter = character;
                                    break;
                                }
                            }
                        }

                        Machine.Utility.BotSettings = Machine.Utility.LoadBotSettings();

                        if (Machine.Utility.BackCompat)
                        {
                            Machine.Utility.BackCompat = false;
                            Machine.Utility.SaveBotSettings();
                        }

                        // Instantiate views
                        MainView = new MainView(this);
                        ExpTrackerView = new ExpTrackerView(this);
                        PortalBotView = new PortalBotView(this, Core);

                        // Instantiate classes to support the views
                        ExpTracker = new ExpTracker(this, Core);
                        InventoryTracker = new InventoryTracker(this, Core);
                        FellowshipControl = new FellowshipControl(this, Core);

                        ChatBoxMessage += FellowshipControl.ChatActions;
                        ChatBoxMessage += InventoryTracker.ParseChat;
                        Core.RenderFrame += Core_RenderFrame;

                        Debug.ToChat("Running ACManager by Shem of Harvestgain.");
                        Debug.ToChat($"Currently running version {Machine.Utility.Version}. Check out the latest on the project at https://github.com/patri0t86/ACManager.");
                        Debug.ToChat($"At any time, type /acm help for more information.");

                        Machine.AdInterval = Machine.Utility.BotSettings.AdInterval;
                        Machine.Advertise = Machine.Utility.BotSettings.AdsEnabled;
                        Machine.RespondToOpenChat = Machine.Utility.BotSettings.RespondToGeneralChat;
                        Machine.Verbosity = Machine.Utility.BotSettings.Verbosity;
                        Machine.Enabled = Machine.Utility.BotSettings.BotEnabled;
                        Machine.ManaThreshold = Machine.Utility.BotSettings.ManaThreshold;
                        Machine.StaminaThreshold = Machine.Utility.BotSettings.StaminaThreshold;
                        Machine.DefaultHeading = Machine.Utility.BotSettings.DefaultHeading;
                        Machine.DesiredLandBlock = Machine.Utility.BotSettings.DesiredLandBlock;
                        Machine.DesiredBotLocationX = Machine.Utility.BotSettings.DesiredBotLocationX;
                        Machine.DesiredBotLocationY = Machine.Utility.BotSettings.DesiredBotLocationY;
                        Machine.EnablePositioning = Machine.Utility.BotSettings.BotPositioning;

                        Machine.LoggedIn = true;
                    }
                }

                if (Convert.ToInt32(e.Message.Value<int>("action")) == 0x004A) // Start casting
                {
                    Machine.CastStarted = true;
                }
            }
        }

        private void Core_RenderFrame(object sender, EventArgs e)
        {
            FellowshipControl.CheckRecruit();
            InventoryTracker.CheckComps();
        }

        /// <summary>
        /// Parses server messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterCore_ServerDispatch(object sender, NetworkMessageEventArgs e)
        {
            if (e.Message.Type == 0xF653) // End 3D Mode and return to character screen
            {
                ChatBoxMessage -= FellowshipControl.ChatActions;
                ChatBoxMessage -= InventoryTracker.ParseChat;
                Core.RenderFrame -= Core_RenderFrame;

                Machine.LoggedIn = false;

                MainView?.Dispose();
                ExpTrackerView?.Dispose();
                PortalBotView?.Dispose();

                ExpTracker?.Dispose();
                InventoryTracker = null;
                FellowshipControl = null;
                Machine.CurrentCharacter = null;
            }

            if (e.Message.Type == 0xF658) // Received the character list from the server
            {
                AccountCharacters.Clear();
                // determine total number of slots available
                TotalSlots = Convert.ToInt32(e.Message["slotCount"]);
                // number of created characters for this account
                int charCount = Convert.ToInt32(e.Message["characterCount"]);

                MessageStruct characterStruct = e.Message.Struct("characters");
                for (int i = 0; i < charCount; i++)
                {
                    AccountCharacters.Add(characterStruct.Struct(i)["name"].ToString());
                }

                // Must sort the characters alphabetically to be in the same order as displayed
                AccountCharacters.Sort();
            }

            if (e.Message.Type == 0xF746) // Login Character
            {
                LogonTimer.Stop();
            }

            if (e.Message.Type == 0xF7B0) // Game events
            {
                if (Convert.ToInt32(e.Message["event"]) == 0x01C7) // Action complete
                {
                    Machine.CastCompleted = true;
                }

                if (Convert.ToInt32(e.Message["event"]) == 0x028A) // Status messages
                {
                    if (Convert.ToInt32(e.Message[3]) == 0x0402) // Your spell fizzled.
                    {
                        Machine.Fizzled = true;
                    }
                }
            }

            if (e.Message.Type == 0xF7E1 && Machine.CurrentState == SwitchingCharacters.GetInstance) // Server Name (last server message sent when logging out)
            {
                LogonTimer.Start();
            }
        }

        /// <summary>
        /// On every timer tick, this will run the LoginNextCharacter method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogonTimer_Tick(object sender, EventArgs e)
        {
            LoginNextCharacter();
        }

        /// <summary>
        /// Controls logging in the next character. Credit to Mag for this.
        /// </summary>
        private void LoginNextCharacter()
        {
            int XPixelOffset = 121;
            int YTopOfBox = 209;
            int YBottomOfBox = 532;
            float characterNameSize = (YBottomOfBox - YTopOfBox) / (float)Machine.TotalSlots;
            int yOffset = (int)(YTopOfBox + (characterNameSize / 2) + (characterNameSize * Machine.NextCharacterIndex));

            // Select the character
            Simulate.MouseClick(Core.Decal.Hwnd, XPixelOffset, yOffset);

            // Click the Enter button
            Simulate.MouseClick(Core.Decal.Hwnd, 0x015C, 0x0185);
        }
    }
}
