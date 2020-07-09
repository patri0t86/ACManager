using ACManager.StateMachine.Queues;
using System;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// This state handles logging a character out, and if the correct character is logged in, resuming requested operation.
    /// </summary>
    internal class SwitchingCharacters : StateBase<SwitchingCharacters>, IState
    {
        private DateTime AttempedLogoff = DateTime.MinValue;

        public void Enter(Machine machine)
        {
            machine.GetNextCharacter();
        }

        public void Exit(Machine machine)
        {

        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (machine.Core.CharacterFilter.Name.Equals(machine.NextCharacter))
                {
                    machine.NextState = Idle.GetInstance;
                }
                else
                {
                    if (DateTime.Now - AttempedLogoff > TimeSpan.FromMilliseconds(10000))
                    {
                        if (machine.CurrentRequest.RequestType.Equals(RequestType.Portal))
                        {
                            machine.ChatManager.Broadcast($"Be right back, switching to {machine.NextCharacter} to summon{(string.IsNullOrEmpty(machine.PortalDescription) ? "" : $" {machine.PortalDescription}")}.");
                        }
                        else if (machine.CurrentRequest.RequestType.Equals(RequestType.Gem))
                        {
                            machine.ChatManager.Broadcast($"Be right back, switching to {machine.NextCharacter} to use {machine.PortalDescription}.");
                        }
                        AttempedLogoff = DateTime.Now;
                        machine.Core.Actions.Logout();
                    }
                }
            }
            else
            {
                machine.NextState = Idle.GetInstance;
            }
        }

        public override string ToString()
        {
            return nameof(SwitchingCharacters);
        }
    }
}
