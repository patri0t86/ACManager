using ACManager.StateMachine;
using ACManager.StateMachine.States;
using Decal.Adapter;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACManager
{
    [FriendlyName("ACManager")]
    public class FilterCore : FilterBase
    {
        public Machine Machine { get; set; }
        private Timer LogonTimer { get; set; } = new Timer();
        public static List<string> AccountCharacters { get; set; } = new List<string>();
        public static int NextCharacterIndex;
        public uint TotalSlots { get; set; }

        /// <summary>
        /// Called as soon as the client is launched.
        /// </summary>
        protected override void Startup()
        {
            ServerDispatch += FilterCore_ServerDispatch;
            ClientDispatch += FilterCore_ClientDispatch;
            LogonTimer.Interval = 1000;
            LogonTimer.Tick += LogonTimer_Tick;
            Debug.Init(Path);
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
        }

        /// <summary>
        /// Parses server messages.
        /// Protocols known at time of creation:
        /// http://skunkworks.sourceforge.net/protocol/Protocol.php
        /// https://acemulator.github.io/protocol/
        /// </summary>
        private void FilterCore_ServerDispatch(object sender, NetworkMessageEventArgs e)
        {
            switch (e.Message.Type)
            {
                case 0xf653: // End 3D Mode and return to character screen
                    Machine.Logout();
                    CommandLineText -= Machine.Interpreter.Command;
                    Core.RenderFrame -= Machine.Clock;
                    Core.WorldFilter.ChangeObject -= Machine.WorldFilter_ChangeObject;
                    break;
                case 0xf658: // Received the character list from the server
                    AccountCharacters.Clear();
                    TotalSlots = e.Message.Value<uint>("slotCount");
                    for (int i = 0; i < e.Message.Value<uint>("characterCount"); i++)
                    {
                        AccountCharacters.Add(e.Message.Struct("characters").Struct(i).Value<string>("name"));
                    }

                    AccountCharacters.Sort((a, b) => String.Compare(a, b, StringComparison.Ordinal));
                    break;
                case 0xf746: // Character login request
                    LogonTimer.Stop();
                    break;
                case 0xf7b0: // Game events                    
                    switch (e.Message.Value<uint>("event"))
                    {
                        case 0x01c7: // Ready, previous action complete
                            Machine.CastCompleted = true;
                            break;
                        case 0x028a: // Status message
                            switch (e.Message.Value<uint>("type"))
                            {
                                case 0x0402:
                                    Machine.Fizzled = true;
                                    break;
                            }
                            break;
                    }
                    break;
                case 0xf7e1:
                    if (Machine.CurrentState == SwitchingCharacters.GetInstance)
                    {
                        LogonTimer.Start();
                    }
                    break;
            }
        }

        /// <summary>
        /// Parses the client messages.
        /// </summary>
        private void FilterCore_ClientDispatch(object sender, NetworkMessageEventArgs e)
        {
            switch (e.Message.Type)
            {
                case 0xf7b1:
                    switch (e.Message.Value<uint>("action"))
                    {
                        case 0x00a1: // Character materializes (including exiting portal space)
                            if (Machine == null)
                            {
                                Utility.Init(Path);
                                Machine = new Machine();
                            }

                            Debug.ToChat($"ACManager {Utility.Version}. Check out the latest on the project at https://github.com/patri0t86/ACManager.");

                            if (!Machine.LoggedIn)
                            {
                                CommandLineText += Machine.Interpreter.Command;
                                Core.RenderFrame += Machine.Clock;
                                Core.WorldFilter.ChangeObject += Machine.WorldFilter_ChangeObject;
                                Machine.Login();
                            }
                            break;
                        case 0x004a: // Started casting
                            Machine.CastStarted = true;
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// On every timer tick, this will run the LoginNextCharacter method.
        /// </summary>
        private void LogonTimer_Tick(object sender, EventArgs e)
        {
            int XPixelOffset = 121;
            int YTopOfBox = 209;
            int YBottomOfBox = 532;
            float characterNameSize = (YBottomOfBox - YTopOfBox) / (float)TotalSlots;
            int yOffset = (int)(YTopOfBox + (characterNameSize / 2) + (characterNameSize * NextCharacterIndex));

            // Select the character
            Simulate.MouseClick(Core.Decal.Hwnd, XPixelOffset, yOffset);

            // Click the Enter button
            Simulate.MouseClick(Core.Decal.Hwnd, 0x015C, 0x0185);
        }
    }
}
