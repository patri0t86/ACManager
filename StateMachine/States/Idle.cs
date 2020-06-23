using Decal.Adapter.Wrappers;
using System;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// This is the idle state, where the bot is awaiting commands.
    /// This state is entered and exited many times throughout the lifecycle. Upon entry, ensures it is in a peaceful stance.
    /// </summary>
    internal class Idle : StateBase<Idle>, IState
    {
        public void Enter(Machine machine)
        {
            machine.DecliningCommands = false;
            if (machine.Core.Actions.CombatMode != CombatState.Peace)
            {
                machine.Core.Actions.SetCombatMode(CombatState.Peace);
            }
        }

        public void Exit(Machine machine)
        {
            machine.DecliningCommands = true;
        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (machine.SpellsToCast.Count > 0 && machine.Core.CharacterFilter.Name.Equals(machine.NextCharacter)) // If there is a portal in the queue and it is on this character, enter casting state
                {
                    if ((machine.Core.Actions.Heading <= machine.NextHeading + 2 && machine.Core.Actions.Heading >= machine.NextHeading - 2) || machine.NextHeading.Equals(-1))
                    {
                        machine.NextState = Casting.GetInstance;
                    }
                    else
                    {
                        machine.Core.Actions.Heading = machine.NextHeading;
                    }
                }
                else if (machine.SpellsToCast.Count > 0 && !machine.Core.CharacterFilter.Name.Equals(machine.NextCharacter)) // If there is a portal in the queue and it is not on this character, switch to the correct character
                {
                    machine.NextState = SwitchingCharacters.GetInstance;
                }
                else if (!(machine.Core.Actions.Heading <= machine.DefaultHeading + 2 && machine.Core.Actions.Heading >= machine.DefaultHeading - 2)) // if totally idle, reset position
                {
                    machine.Core.Actions.Heading = machine.DefaultHeading;
                }
                else if (machine.Advertise && machine.Update() && DateTime.Now - machine.LastBroadcast > TimeSpan.FromMinutes(machine.AdInterval)) // Advertisement/spam timing control
                {
                    machine.LastBroadcast = DateTime.Now;
                    machine.ChatManager.Broadcast(machine.Utility.BotSettings.Advertisements[machine.RandomNumber.Next(0, machine.Utility.BotSettings.Advertisements.Count)].Message);
                }
            }
            else
            {
                machine.NextState = Stopped.GetInstance;
            }
        }

        public override string ToString()
        {
            return nameof(Idle);
        }
    }
}
