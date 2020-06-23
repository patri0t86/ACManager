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
            machine.NextCharacter = null;
            machine.SpellsToCast.Clear();
            machine.Core.ChatBoxMessage -= machine.ChatManager.Current_ChatBoxMessage;
            machine.Core.Actions.SetIdleTime(1200);

            Debug.ToChat("Stopped gracefully.");
        }

        public void Exit(Machine machine)
        {
            if (machine.NextState == Idle.GetInstance)
            {
                machine.MachineStarted = DateTime.Now;
                machine.Core.Actions.SetIdleTime(Double.MaxValue);
                machine.Core.ChatBoxMessage += machine.ChatManager.Current_ChatBoxMessage;

                Debug.ToChat("Started successfully.");
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
