using Decal.Adapter.Wrappers;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// This state controls the casting of all spells.
    /// </summary>
    internal class Casting : StateBase<Casting>, IState
    {
        public void Enter(Machine machine)
        {
            machine.Fizzled = false;
            machine.CastCompleted = false;
            machine.CastStarted = false;
            if (machine.Core.Actions.CombatMode != CombatState.Magic)
            {
                machine.Core.Actions.SetCombatMode(CombatState.Magic);
            }
        }

        public void Exit(Machine machine)
        {

        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (machine.CastStarted && machine.CastCompleted && !machine.Fizzled)
                {
                    if (machine.SpellsToCast[0].Equals(157) || machine.SpellsToCast[0].Equals(2648))
                    {
                        machine.PortalsSummonedThisSession += 1;
                        if (!string.IsNullOrEmpty(machine.PortalDescription))
                        {
                            machine.ChatManager.Broadcast($"/s Portal now open to {machine.PortalDescription}. Safe journey, friend.");
                        }
                    }
                    machine.SpellsToCast.RemoveAt(0);
                    if (machine.SpellsToCast.Count == 0)
                    {
                        machine.NextState = Idle.GetInstance;
                    }
                    else
                    {
                        machine.CastCompleted = false;
                        machine.CastStarted = false;
                    }
                }
                else if (machine.CastStarted && machine.CastCompleted && machine.Fizzled)
                {
                    machine.Fizzled = false;
                    machine.CastCompleted = false;
                    machine.CastStarted = false;
                }
                else if (!machine.CastStarted)
                {
                    if (machine.Core.CharacterFilter.Mana < machine.ManaThreshold * machine.MaxVitals[CharFilterVitalType.Mana])
                    {
                        machine.NextState = VitalManagement.GetInstance;
                    }
                    else
                    {
                        machine.Core.Actions.CastSpell(machine.SpellsToCast[0], 0);
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
            return nameof(Casting);
        }
    }
}
