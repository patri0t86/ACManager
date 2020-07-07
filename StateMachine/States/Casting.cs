using Decal.Adapter.Wrappers;
using Decal.Filters;

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
            if (machine.Inventory.LowComponents.Count > 0)
            {
                machine.ChatManager.Broadcast(machine.Inventory.LowCompsReport());
            }
        }

        public void Exit(Machine machine)
        {

        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (machine.Core.Actions.CombatMode != CombatState.Magic)
                {
                    machine.Core.Actions.SetCombatMode(CombatState.Magic);
                }
                else if (machine.CastStarted && machine.CastCompleted && !machine.Fizzled)
                {
                    if (machine.SpellsToCast[0].Equals(157) || machine.SpellsToCast[0].Equals(2648))
                    {
                        machine.PortalsSummonedThisSession += 1;
                        if (!string.IsNullOrEmpty(machine.PortalDescription))
                        {
                            machine.ChatManager.Broadcast($"/s Portal now open to {machine.PortalDescription}. Safe journey, friend.");
                        }
                        else
                        {
                            machine.ChatManager.Broadcast($"/s Portal now open. Safe journey, friend.");
                        }
                    }
                    machine.SpellsToCast.RemoveAt(0);
                    if (machine.SpellsToCast.Count.Equals(0))
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
                    if (machine.Core.CharacterFilter.Mana < machine.ManaThreshold * machine.MaxVitals[CharFilterVitalType.Mana] && machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] > 50)
                    {
                        machine.NextState = VitalManagement.GetInstance;
                    }
                    else
                    {
                        if (machine.Core.CharacterFilter.IsSpellKnown(machine.SpellsToCast[0]))
                        {
                            if (machine.ComponentChecker.HaveComponents(machine.SpellsToCast[0]))
                            {
                                machine.Core.Actions.CastSpell(machine.SpellsToCast[0], 0);
                            }
                            else
                            {
                                machine.ChatManager.Broadcast($"I have run out of spell components.");
                                machine.SpellsToCast.Clear();
                                machine.NextState = Idle.GetInstance;
                            }
                        }
                        else
                        {
                            Debug.ToChat($"You do not know the spell {machine.Core.Filter<FileService>().SpellTable.GetById(machine.SpellsToCast[0]).Name}. Removing from current casting session.");
                            machine.ChatManager.Broadcast($"I tried casting {machine.Core.Filter<FileService>().SpellTable.GetById(machine.SpellsToCast[0]).Name}, but I do not know it yet.");
                            machine.SpellsToCast.RemoveAt(0);
                            if (machine.SpellsToCast.Count.Equals(0))
                            {
                                machine.NextState = Idle.GetInstance;
                            }
                        }
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
