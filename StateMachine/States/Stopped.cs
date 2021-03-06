﻿using System;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// This state starts and stops the core functionality of the bot as it is started and stopped.
    /// Everything is implemented here since the Idle state may be entered many times throughout the bot lifecycle.
    /// </summary>
    class Stopped : StateBase<Stopped>, IState
    {
        public void Enter(Machine machine)
        {
            machine.NextCharacter = null;
            machine.SpellsToCast.Clear();
            machine.Requests.Clear();
            machine.ItemToUse = null;
            machine.Core.ChatBoxMessage -= machine.ChatManager.Current_ChatBoxMessage;
            machine.Core.Actions.SetIdleTime(1200);

            Debug.ToChat("Stopped gracefully.");
        }

        public void Exit(Machine machine)
        {
            if (machine.NextState == Idle.GetInstance)
            {
                machine.MachineStarted = DateTime.Now;
                machine.LastBroadcast = DateTime.Now;
                machine.Core.Actions.SetIdleTime(Double.MaxValue);
                machine.Core.ChatBoxMessage += machine.ChatManager.Current_ChatBoxMessage;

                machine.AdInterval = machine.Utility.BotSettings.AdInterval >= 5 ? machine.Utility.BotSettings.AdInterval : 5;
                machine.Advertise = machine.Utility.BotSettings.AdsEnabled;
                machine.RespondToOpenChat = machine.Utility.BotSettings.RespondToGeneralChat;
                machine.RespondToAllegiance = machine.Utility.BotSettings.RespondToAllegiance;
                machine.Verbosity = machine.Utility.BotSettings.Verbosity;
                machine.ManaThreshold = machine.Utility.BotSettings.ManaThreshold;
                machine.StaminaThreshold = machine.Utility.BotSettings.StaminaThreshold;
                machine.DefaultHeading = machine.Utility.BotSettings.DefaultHeading;
                machine.DesiredLandBlock = machine.Utility.BotSettings.DesiredLandBlock;
                machine.DesiredBotLocationX = machine.Utility.BotSettings.DesiredBotLocationX;
                machine.DesiredBotLocationY = machine.Utility.BotSettings.DesiredBotLocationY;
                machine.EnablePositioning = machine.Utility.BotSettings.BotPositioning;
                machine.BuffingCharacter = machine.Utility.BotSettings.BuffingCharacter;
                machine.StayBuffed = machine.Utility.BotSettings.StayBuffed;
                machine.Level7Self = machine.Utility.BotSettings.Level7Self;
                machine.SkillOverride = machine.Utility.BotSettings.SkillOverride;

                Debug.ToChat("Started successfully.");
                machine.ChatManager.Broadcast($"/me is running ACManager {machine.Utility.Version} found at https://github.com/patri0t86/ACManager. Whisper 'help' to get started.");
            }
        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                machine.NextState = Idle.GetInstance;
            }
        }

        public override string ToString()
        {
            return nameof(Stopped);
        }
    }
}
