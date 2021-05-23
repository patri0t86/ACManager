using Decal.Adapter;
using System;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// This state handles logging a character out, and if the correct character is logged in, resuming requested operation.
    /// </summary>
    public class SwitchingCharacters : StateBase<SwitchingCharacters>, IState
    {
        private DateTime LoggedOff;
        public void Enter(Machine machine)
        {
            for (int i = 0; i < FilterCore.AccountCharacters.Count; i++)
            {
                if (FilterCore.AccountCharacters[i].Equals(machine.CurrentRequest.Character))
                {
                    FilterCore.NextCharacterIndex = i;
                    break;
                }
            }

            if (machine.CurrentRequest.RequestType.Equals(RequestType.Portal))
            {
                ChatManager.Broadcast($"Be right back, switching to {machine.CurrentRequest.Character} to summon{(string.IsNullOrEmpty(machine.CurrentRequest.Destination) ? "" : $" {machine.CurrentRequest.Destination}")}.");
            }
            else if (machine.CurrentRequest.RequestType.Equals(RequestType.Gem))
            {
                ChatManager.Broadcast($"Be right back, switching to {machine.CurrentRequest.Character} to use {machine.CurrentRequest.Destination}.");
            }
            else if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff))
            {
                ChatManager.Broadcast($"Be right back, switching to {machine.CurrentRequest.Character} to buff {machine.CurrentRequest.RequesterName}.");
            }
        }

        public void Exit(Machine machine)
        {

        }

        public void Process(Machine machine)
        {
            if (!machine.Enabled)
            {
                machine.NextState = Idle.GetInstance;
                return;
            }

            if (CoreManager.Current.CharacterFilter.Name.Equals(machine.CurrentRequest.Character))
            {
                machine.NextState = Idle.GetInstance;
                return;
            }

            if (DateTime.Now - LoggedOff > TimeSpan.FromSeconds(10))
            {
                LoggedOff = DateTime.Now;
                CoreManager.Current.Actions.Logout();
            }
        }

        public override string ToString()
        {
            return nameof(SwitchingCharacters);
        }
    }
}
