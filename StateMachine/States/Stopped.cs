using Decal.Adapter;
using System;

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
            //machine.NextCharacter = null;
            machine.SpellsToCast.Clear();
            machine.Requests.Clear();
            //machine.ItemToUse = null;
            CoreManager.Current.ChatBoxMessage -= machine.ChatManager.Current_ChatBoxMessage;
            CoreManager.Current.Actions.SetIdleTime(1200);

            Debug.ToChat("Stopped gracefully.");
        }

        public void Exit(Machine machine)
        {
            if (machine.NextState == Idle.GetInstance)
            {
                machine.MachineStarted = DateTime.Now;
                machine.LastBroadcast = DateTime.Now;
                CoreManager.Current.Actions.SetIdleTime(Double.MaxValue);
                CoreManager.Current.ChatBoxMessage += machine.ChatManager.Current_ChatBoxMessage;

                machine.AdInterval = Utility.BotSettings.AdInterval >= 5 ? Utility.BotSettings.AdInterval : 5;
                machine.Advertise = Utility.BotSettings.AdsEnabled;
                machine.RespondToOpenChat = Utility.BotSettings.RespondToGeneralChat;
                machine.RespondToAllegiance = Utility.BotSettings.RespondToAllegiance;
                machine.Verbosity = Utility.BotSettings.Verbosity;
                machine.ManaThreshold = Utility.BotSettings.ManaThreshold;
                machine.StaminaThreshold = Utility.BotSettings.StaminaThreshold;
                machine.DefaultHeading = Utility.BotSettings.DefaultHeading;
                machine.DesiredLandBlock = Utility.BotSettings.DesiredLandBlock;
                machine.DesiredBotLocationX = Utility.BotSettings.DesiredBotLocationX;
                machine.DesiredBotLocationY = Utility.BotSettings.DesiredBotLocationY;
                machine.EnablePositioning = Utility.BotSettings.BotPositioning;
                machine.BuffingCharacter = Utility.BotSettings.BuffingCharacter;
                machine.StayBuffed = Utility.BotSettings.StayBuffed;
                machine.Level7Self = Utility.BotSettings.Level7Self;
                machine.SkillOverride = Utility.BotSettings.SkillOverride;

                Debug.ToChat("Started successfully.");
                ChatManager.Broadcast($"/me is running ACManager {Utility.Version} found at https://github.com/patri0t86/ACManager. Whisper 'help' to get started.");
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
