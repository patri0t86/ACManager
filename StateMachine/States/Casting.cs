using ACManager.StateMachine.Queues;
using Decal.Adapter.Wrappers;
using Decal.Filters;
using System;
using System.Collections.Generic;

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
        private int LastSpell { get; set; }
        private bool StartedTracking { get; set; }
        private DateTime StartedTrackingTime { get; set; }
        private int SpellCastCount { get; set; }
        private DateTime EnteredState { get; set; }
        private bool Cancelled { get; set; }

        public void Enter(Machine machine)
        {
            machine.Fizzled = false;
            machine.CastCompleted = false;
            machine.CastStarted = false;
            EnteredState = DateTime.Now;

            if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff))
            {
                if (!SentInitialInfo)
                {
                    Started = DateTime.Now;
                    SentInitialInfo = true;
                    SpellCastCount = 0;

                    if (machine.Inventory.LowComponents.Count > 0)
                    {
                        machine.ChatManager.Broadcast(machine.Inventory.LowCompsReport());
                    }

                    TimeSpan buffTime = TimeSpan.FromSeconds(machine.SpellsToCast.Count * 4);
                    string message = $"Casting {machine.SpellsToCast.Count} buffs on you. This should take about {buffTime.Minutes} minutes and {buffTime.Seconds} seconds.";

                    if (machine.CurrentRequest.RequesterGuid != 0 && !CastBanes)
                    {
                        if (ContainsBanes(machine.SpellsToCast))
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
                                        message += " You are wearing a covenant shield, so I will be skipping banes.";
                                    }
                                }
                                else
                                {
                                    message += " You are not wearing a shield, so I will be skipping banes.";
                                }
                            }
                        }
                    }
                    machine.ChatManager.SendTell(machine.CurrentRequest.RequesterName, message);
                }
            }
        }

        public void Exit(Machine machine)
        {
            if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff) && machine.SpellsToCast.Count.Equals(0))
            {
                SentInitialInfo = false;
                TimeSpan duration = DateTime.Now - Started;
                if (!Cancelled)
                {
                    machine.ChatManager.SendTell(machine.CurrentRequest.RequesterName, $"Buffing is complete with {SpellCastCount} buffs, it took {duration.Minutes} minutes and {duration.Seconds} seconds.");
                }
                CastBanes = false;
                LastSpell = 0;
            }
            Cancelled = false;
        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (machine.SpellsToCast.Count > 0)
                {
                    if (machine.Core.Actions.CombatMode != CombatState.Magic)
                    {
                        machine.Core.Actions.SetCombatMode(CombatState.Magic);
                        if ((DateTime.Now - EnteredState).TotalSeconds > 1)
                        {
                            machine.SpellsToCast.Clear();
                        }
                    }
                    else if (machine.Core.Actions.CombatMode == CombatState.Magic)
                    {
                        if (machine.CastStarted && machine.CastCompleted && !machine.Fizzled)
                        {
                            if (machine.SpellsToCast[0].Id.Equals(157) || machine.SpellsToCast[0].Id.Equals(2648))
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
                            else
                            {
                                SpellCastCount += 1;
                            }
                            machine.SpellsToCast.RemoveAt(0);
                            machine.CastCompleted = false;
                            machine.CastStarted = false;

                            // Check to see if this request was cancelled.
                            foreach (string cancel in machine.CancelList)
                            {
                                if (machine.CurrentRequest.RequesterName.Equals(cancel))
                                {
                                    Cancelled = true;
                                    machine.SpellsToCast.Clear();
                                    machine.CancelList.Remove(cancel);
                                    break;
                                }
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
                            if (machine.Core.CharacterFilter.Mana < machine.ManaThreshold * machine.Core.CharacterFilter.EffectiveVital[CharFilterVitalType.Mana]
                                && machine.Core.Actions.SkillTrainLevel[Decal.Adapter.Wrappers.SkillType.BaseLifeMagic] != 1)
                            {
                                machine.NextState = VitalManagement.GetInstance;
                            }
                            else
                            {
                                if (machine.Core.CharacterFilter.IsSpellKnown(machine.SpellsToCast[0].Id) && machine.SpellSkillCheck(machine.SpellsToCast[0]))
                                {
                                    if (machine.ComponentChecker.HaveComponents(machine.SpellsToCast[0].Id))
                                    {
                                        if (LastSpell.Equals(machine.SpellsToCast[0]) && !(machine.SpellsToCast[0].Equals(157) || machine.SpellsToCast[0].Equals(2648)))
                                        {
                                            if (!StartedTracking)
                                            {
                                                StartedTrackingTime = DateTime.Now;
                                                StartedTracking = !StartedTracking;
                                            }
                                            else if ((DateTime.Now - StartedTrackingTime).TotalSeconds > 5)
                                            {
                                                StartedTracking = !StartedTracking;
                                                machine.SpellsToCast.Clear();
                                            }
                                        }
                                        else if (StartedTracking)
                                        {
                                            StartedTracking = !StartedTracking;
                                        }

                                        LastSpell = machine.SpellsToCast[0].Id;

                                        if (IsBane(machine.SpellsToCast[0]) && CastBanes)
                                        {
                                            machine.Core.Actions.CastSpell(machine.SpellsToCast[0].Id, machine.CurrentRequest.RequesterGuid);
                                        }
                                        else if (IsBane(machine.SpellsToCast[0]) && !CastBanes)
                                        {
                                            machine.SpellsToCast.RemoveAt(0);
                                        }
                                        else
                                        {
                                            machine.Core.Actions.CastSpell(machine.SpellsToCast[0].Id, machine.CurrentRequest.RequesterGuid);
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
                                    Spell fallbackSpell = machine.GetFallbackSpell(machine.SpellsToCast[0]);
                                    machine.SpellsToCast.RemoveAt(0);
                                    if (fallbackSpell != null)
                                    {
                                        machine.SpellsToCast.Insert(0, fallbackSpell);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    machine.NextState = Equipping.GetInstance;
                }
            }
            else
            {
                SentInitialInfo = false;
                machine.NextState = Equipping.GetInstance;
            }
        }

        public override string ToString()
        {
            return nameof(Casting);
        }

        private bool IsBane(Spell spell)
        {
            return spell.Family.Equals(160)
                || spell.Family.Equals(162)
                || spell.Family.Equals(164)
                || spell.Family.Equals(166)
                || spell.Family.Equals(168)
                || spell.Family.Equals(170)
                || spell.Family.Equals(172)
                || spell.Family.Equals(174);
        }

        private bool ContainsBanes(List<Spell> spells)
        {
            foreach (Spell spell in spells)
            {
                if (IsBane(spell))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
