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
            machine.CurrentRequest = new Request();
            machine.Requests.Clear();
            CoreManager.Current.ChatBoxMessage -= machine.ChatManager.Current_ChatBoxMessage;
            CoreManager.Current.Actions.SetIdleTime(1200);
            Debug.ToChat("Bot stopped successfully.");
        }

        public void Exit(Machine machine)
        {
            machine.MachineStarted = DateTime.Now;
            CoreManager.Current.Actions.SetIdleTime(double.MaxValue);
            CoreManager.Current.ChatBoxMessage += machine.ChatManager.Current_ChatBoxMessage;
            machine.CurrentRequest = new Request();
            Debug.ToChat("Bot started successfully.");
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
