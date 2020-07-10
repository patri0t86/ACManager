using ACManager.StateMachine.Queues;
using Decal.Adapter.Wrappers;
using Decal.Filters;
using System;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// This state controls the casting of all spells.
    /// </summary>
    internal class Casting : StateBase<Casting>, IState
    {
        private DateTime Started { get; set; }
        private bool SentInitialInfo { get; set; }
        private bool CastBanes { get; set; }
        public void Enter(Machine machine)
        {
            machine.Fizzled = false;
            machine.CastCompleted = false;
            machine.CastStarted = false;

            if (machine.Inventory.LowComponents.Count > 0)
            {
                machine.ChatManager.Broadcast(machine.Inventory.LowCompsReport());
            }

            if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff))
            {
                if (!SentInitialInfo)
                {
                    Started = DateTime.Now;
                    SentInitialInfo = !SentInitialInfo;
                    TimeSpan buffTime = TimeSpan.FromSeconds(machine.SpellsToCast.Count * 4);
                    if (machine.SpellsToCast.Contains(5989) || machine.SpellsToCast.Contains(5997) || machine.SpellsToCast.Contains(6006) || machine.SpellsToCast.Contains(6014) || machine.SpellsToCast.Contains(6023) || machine.SpellsToCast.Contains(6031))
                    {
                        machine.ChatManager.SendTell(machine.CurrentRequest.RequesterName, $"Casting {machine.SpellsToCast.Count} buffs on you. This should take about {buffTime.Minutes} minutes and {buffTime.Seconds} seconds. Make sure you are standing near me, Item Auras are short range spells.");
                    }
                    else
                    {
                        machine.ChatManager.SendTell(machine.CurrentRequest.RequesterName, $"Casting {machine.SpellsToCast.Count} buffs on you. This should take about {buffTime.Minutes} minutes and {buffTime.Seconds} seconds.");
                    }
                }

                if (machine.CurrentRequest.RequesterGuid != 0 && !CastBanes)
                {
                    using (WorldObjectCollection items = machine.Core.WorldFilter.GetByContainer(machine.CurrentRequest.RequesterGuid))
                    {
                        items.SetFilter(new ByObjectClassFilter(ObjectClass.Armor));
                        if (items.Count > 0)
                        {
                            if (!items.First.Name.Contains("Covenant"))
                            {
                                CastBanes = true;
                            }
                            else
                            {
                                machine.ChatManager.SendTell(machine.CurrentRequest.RequesterName, "You are wearing a covenant shield, so I will be skipping banes.");
                            }
                        }
                        else
                        {
                            machine.ChatManager.SendTell(machine.CurrentRequest.RequesterName, "You are not wearing a shield, so I will be skipping banes.");
                        }
                    }
                }
            }
        }

        public void Exit(Machine machine)
        {
            if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff) && machine.SpellsToCast.Count.Equals(0))
            {
                SentInitialInfo = !SentInitialInfo;
                TimeSpan duration = DateTime.Now - Started;
                machine.ChatManager.SendTell(machine.CurrentRequest.RequesterName, $"Buffing is complete. This took {duration.Minutes} minutes and {duration.Seconds} seconds to complete.");
                CastBanes = false;
            }
        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (machine.SpellsToCast.Count > 0 && machine.CastStarted && machine.CastCompleted && !machine.Fizzled)
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
                    machine.CastCompleted = false;
                    machine.CastStarted = false;
                }
                else if (machine.SpellsToCast.Count > 0 && machine.CastStarted && machine.CastCompleted && machine.Fizzled)
                {
                    machine.Fizzled = false;
                    machine.CastCompleted = false;
                    machine.CastStarted = false;
                }
                else if (machine.SpellsToCast.Count > 0 && !machine.CastStarted)
                {
                    if (machine.Core.CharacterFilter.Mana < machine.ManaThreshold * machine.MaxVitals[CharFilterVitalType.Mana] && machine.Core.Actions.SkillTrainLevel[Decal.Adapter.Wrappers.SkillType.BaseLifeMagic] != 1)
                    {
                        machine.NextState = VitalManagement.GetInstance;
                    }
                    else
                    {
                        if (machine.Core.Actions.CombatMode != CombatState.Magic)
                        {
                            machine.Core.Actions.SetCombatMode(CombatState.Magic);
                        }
                        else if (machine.Core.CharacterFilter.IsSpellKnown(machine.SpellsToCast[0]))
                        {
                            if (machine.ComponentChecker.HaveComponents(machine.SpellsToCast[0]))
                            {
                                if (SpellIsBane(machine.SpellsToCast[0]))
                                {
                                    if (CastBanes)
                                    {
                                        machine.Core.Actions.CastSpell(machine.SpellsToCast[0], machine.CurrentRequest.RequesterGuid);
                                    }
                                    else
                                    {
                                        machine.SpellsToCast.RemoveAt(0);
                                    }
                                }
                                else
                                {
                                    machine.Core.Actions.CastSpell(machine.SpellsToCast[0], machine.CurrentRequest.RequesterGuid);
                                }
                            }
                            else
                            {
                                machine.ChatManager.Broadcast($"I have run out of spell components.");
                                machine.SpellsToCast.Clear();
                            }
                        }
                        else
                        {
                            machine.ChatManager.Broadcast($"I tried casting {machine.Core.Filter<FileService>().SpellTable.GetById(machine.SpellsToCast[0]).Name}, but I do not know it yet.");
                            machine.SpellsToCast.RemoveAt(0);
                        }
                    }
                }
                else if (machine.SpellsToCast.Count.Equals(0))
                {
                    if (machine.Core.Actions.CombatMode != CombatState.Peace)
                    {
                        machine.Core.Actions.SetCombatMode(CombatState.Peace);
                    }
                    else if (machine.Core.Actions.CombatMode == CombatState.Peace)
                    {
                        machine.NextState = Equipping.GetInstance;
                    }
                }
            }
            else
            {
                machine.NextState = Equipping.GetInstance;
            }
        }

        private bool SpellIsBane(int spellId)
        {
            return spellId.Equals(4391)
                || spellId.Equals(4393)
                || spellId.Equals(4397)
                || spellId.Equals(4401)
                || spellId.Equals(4403)
                || spellId.Equals(4407)
                || spellId.Equals(4409)
                || spellId.Equals(4412);
        }

        public override string ToString()
        {
            return nameof(Casting);
        }
    }
}
