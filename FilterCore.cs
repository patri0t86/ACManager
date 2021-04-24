using ACManager.StateMachine;
using ACManager.StateMachine.States;
using ACManager.Views;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACManager
{
    [FriendlyName("ACManager")]
    public class FilterCore : FilterBase
    {
        internal Machine Machine { get; set; }
        private Timer LogonTimer { get; set; } = new Timer();
        public List<string> AccountCharacters { get; set; } = new List<string>();
        public int TotalSlots { get; set; }

        /// <summary>
        /// Called as soon as the client is launched.
        /// </summary>
        protected override void Startup()
        {
            ServerDispatch += FilterCore_ServerDispatch;
            ClientDispatch += FilterCore_ClientDispatch;
            LogonTimer.Interval = 1000;
            LogonTimer.Tick += LogonTimer_Tick;
        }

        /// <summary>
        /// Called only when the client is completely closed (exited out of the character selection screen even).
        /// </summary>
        protected override void Shutdown()
        {
            ServerDispatch -= FilterCore_ServerDispatch;
            CommandLineText -= Machine.Interpreter.Command;
            ClientDispatch -= FilterCore_ClientDispatch;
            LogonTimer.Tick -= LogonTimer_Tick;
            LogonTimer?.Dispose();
            Machine.BotManagerView?.Dispose();
            Machine = null;
        }

        /// <summary>
        /// Parses server messages.
        /// Protocols known at time of creation:
        /// http://skunkworks.sourceforge.net/protocol/Protocol.php
        /// https://acemulator.github.io/protocol/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterCore_ServerDispatch(object sender, NetworkMessageEventArgs e)
        {
            // End 3D Mode and return to character screen
            if (e.Message.Type.Equals(0xF653))
            {
                Machine.LoggedIn = false;
                CommandLineText -= Machine.Interpreter.Command;
                Core.RenderFrame -= Machine.Clock;
                Core.WorldFilter.ChangeObject -= Machine.WorldFilter_ChangeObject;
                Machine.BotManagerView?.Dispose();
            }

            // Received the character list from the server
            if (e.Message.Type.Equals(0xF658))
            {
                AccountCharacters.Clear();
                TotalSlots = Convert.ToInt32(e.Message["slotCount"]);
                int charCount = Convert.ToInt32(e.Message["characterCount"]);

                MessageStruct characterStruct = e.Message.Struct("characters");
                for (int i = 0; i < charCount; i++)
                {
                    AccountCharacters.Add(characterStruct.Struct(i)["name"].ToString());
                }

                // Must sort the characters ordinally to be in the same order as displayed
                AccountCharacters.Sort((a, b) => String.Compare(a, b, StringComparison.Ordinal));
            }

            // Login Character
            if (e.Message.Type.Equals(0xF746))
            {
                LogonTimer.Stop();
            }

            // Game events
            if (e.Message.Type.Equals(0xF7B0))
            {
                // Action complete
                if (Convert.ToInt32(e.Message["event"]).Equals(0x01C7))
                {
                    Machine.CastCompleted = true;
                }

                // Status messages
                if (Convert.ToInt32(e.Message["event"]).Equals(0x028A))
                {
                    // Your spell fizzled.
                    if (Convert.ToInt32(e.Message[3]).Equals(0x0402))
                    {
                        Machine.Fizzled = true;
                    }
                }
            }

            // Server Name (last server message sent when logging out)
            if (e.Message.Type.Equals(0xF7E1) && Machine.CurrentState == SwitchingCharacters.GetInstance)
            {
                LogonTimer.Start();
            }
        }

        /// <summary>
        /// Parses the client messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterCore_ClientDispatch(object sender, NetworkMessageEventArgs e)
        {
            if (e.Message.Type.Equals(0xF7B1))
            {
                // Materialize character (including any portal taken)
                if (e.Message.Value<int>("action").Equals(0x00A1))
                {
                    if (Machine == null)
                    {
                        Debug.Init(Path);
                        Machine = new Machine(Core, Path);
                    }

                    if (!Machine.LoggedIn)
                    {
                        CommandLineText += Machine.Interpreter.Command;
                        Machine.AccountCharacters = AccountCharacters;
                        Machine.Core = Core;

                        // Instantiate views
                        Machine.BotManagerView = new BotManagerView(Machine);

                        Machine.CharacterEquipment.Clear();
                        Machine.FinishedInitialScan = false;
                        Machine.Enabled = Machine.Utility.BotSettings.BotEnabled;
                        Machine.LoggedIn = true;
                        Core.RenderFrame += Machine.Clock;
                        Core.WorldFilter.ChangeObject += Machine.WorldFilter_ChangeObject;

                        Debug.ToChat($"Running ACManager {Machine.Utility.Version} by Shem of Harvestgain (now Coldeve). Check out the latest on the project at https://github.com/patri0t86/ACManager.");
                        Debug.ToChat("Scanning inventory, please wait before using the Equipment manager to build suits...");

                        using (WorldObjectCollection inventory = Core.WorldFilter.GetInventory())
                        {
                            foreach (WorldObject item in inventory)
                            {
                                if (item.ObjectClass.Equals(ObjectClass.Armor)
                                    || item.ObjectClass.Equals(ObjectClass.Jewelry)
                                    || item.ObjectClass.Equals(ObjectClass.Clothing)
                                    || item.ObjectClass.Equals(ObjectClass.WandStaffOrb))
                                {
                                    Machine.CharacterEquipment.Add(item);
                                    Core.Actions.RequestId(item.Id);
                                }
                            }
                        }
                    }
                }

                // Start casting
                if (Convert.ToInt32(e.Message.Value<int>("action")).Equals(0x004A))
                {
                    Machine.CastStarted = true;
                }
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
            float characterNameSize = (YBottomOfBox - YTopOfBox) / (float)TotalSlots;
            int yOffset = (int)(YTopOfBox + (characterNameSize / 2) + (characterNameSize * Machine.NextCharacterIndex));

            // Select the character
            Simulate.MouseClick(Core.Decal.Hwnd, XPixelOffset, yOffset);

            // Click the Enter button
            Simulate.MouseClick(Core.Decal.Hwnd, 0x015C, 0x0185);
        }
    }
}
