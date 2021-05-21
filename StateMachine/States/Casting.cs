using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Filters;
using System;
using System.Collections.Generic;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// This state controls the casting of all spells.
    /// </summary>
    public class Casting : StateBase<Casting>, IState
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

                    ChatManager.Broadcast(Inventory.ReportOnLowComponents());

                    TimeSpan buffTime = TimeSpan.FromSeconds(machine.CurrentRequest.SpellsToCast.Count * 4);
                    string message = $"Casting {machine.CurrentRequest.SpellsToCast.Count} buffs on you. This should take about {buffTime.Minutes} minutes and {buffTime.Seconds} seconds.";

                    if (machine.CurrentRequest.RequesterGuid != 0 && !CastBanes)
                    {
                        if (ContainsBanes(machine.CurrentRequest.SpellsToCast))
                        {
                            using (WorldObjectCollection items = CoreManager.Current.WorldFilter.GetByContainer(machine.CurrentRequest.RequesterGuid))
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
                    ChatManager.SendTell(machine.CurrentRequest.RequesterName, message);
                }
            }
        }

        public void Exit(Machine machine)
        {
            if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff) && machine.CurrentRequest.SpellsToCast.Count.Equals(0))
            {
                SentInitialInfo = false;
                TimeSpan duration = DateTime.Now - Started;
                if (!Cancelled)
                {
                    ChatManager.SendTell(machine.CurrentRequest.RequesterName, $"Buffing is complete with {SpellCastCount} buffs, it took {duration.Minutes} minutes and {duration.Seconds} seconds.");
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
                if (machine.CurrentRequest.SpellsToCast.Count > 0)
                {
                    if (CoreManager.Current.Actions.CombatMode != CombatState.Magic)
                    {
                        CoreManager.Current.Actions.SetCombatMode(CombatState.Magic);
                        if ((DateTime.Now - EnteredState).TotalSeconds > 1)
                        {
                            machine.CurrentRequest.SpellsToCast.Clear();
                        }
                    }
                    else if (CoreManager.Current.Actions.CombatMode == CombatState.Magic)
                    {
                        if (machine.CastStarted && machine.CastCompleted && !machine.Fizzled)
                        {
                            if (machine.CurrentRequest.SpellsToCast[0].Id.Equals(157) || machine.CurrentRequest.SpellsToCast[0].Id.Equals(2648))
                            {
                                machine.PortalsSummonedThisSession += 1;
                                if (!string.IsNullOrEmpty(machine.CurrentRequest.Destination))
                                {
                                    ChatManager.Broadcast($"/s Portal now open to {machine.CurrentRequest.Destination}. Safe journey, friend.");
                                }
                                else
                                {
                                    ChatManager.Broadcast($"/s Portal now open. Safe journey, friend.");
                                }
                            }
                            else
                            {
                                SpellCastCount += 1;
                            }
                            machine.CurrentRequest.SpellsToCast.RemoveAt(0);
                            machine.CastCompleted = false;
                            machine.CastStarted = false;

                            // Check to see if this request was cancelled.
                            foreach (string cancel in machine.CancelList)
                            {
                                if (machine.CurrentRequest.RequesterName.Equals(cancel))
                                {
                                    Cancelled = true;
                                    machine.CurrentRequest.SpellsToCast.Clear();
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
                            if (CoreManager.Current.CharacterFilter.Mana < Utility.BotSettings.ManaThreshold * CoreManager.Current.CharacterFilter.EffectiveVital[CharFilterVitalType.Mana]
                                && CoreManager.Current.Actions.SkillTrainLevel[Decal.Adapter.Wrappers.SkillType.BaseLifeMagic] != 1)
                            {
                                machine.NextState = VitalManagement.GetInstance;
                            }
                            else
                            {
                                if (CoreManager.Current.CharacterFilter.IsSpellKnown(machine.CurrentRequest.SpellsToCast[0].Id) && machine.SpellSkillCheck(machine.CurrentRequest.SpellsToCast[0]))
                                {
                                    if (Inventory.HaveComponents(machine.CurrentRequest.SpellsToCast[0]))
                                    {
                                        if (LastSpell.Equals(machine.CurrentRequest.SpellsToCast[0].Id) && !(machine.CurrentRequest.SpellsToCast[0].Id.Equals(157) || machine.CurrentRequest.SpellsToCast[0].Id.Equals(2648)))
                                        {
                                            if (!StartedTracking)
                                            {
                                                StartedTrackingTime = DateTime.Now;
                                                StartedTracking = !StartedTracking;
                                            }
                                            else if ((DateTime.Now - StartedTrackingTime).TotalSeconds > 5)
                                            {
                                                StartedTracking = !StartedTracking;
                                                machine.CurrentRequest.SpellsToCast.Clear();
                                            }
                                        }
                                        else if (StartedTracking)
                                        {
                                            StartedTracking = !StartedTracking;
                                        }

                                        LastSpell = machine.CurrentRequest.SpellsToCast[0].Id;
                                        if (IsBane(machine.CurrentRequest.SpellsToCast[0]) && CastBanes)
                                        {
                                            CoreManager.Current.Actions.CastSpell(machine.CurrentRequest.SpellsToCast[0].Id, machine.CurrentRequest.RequesterGuid);
                                        }
                                        else if (IsBane(machine.CurrentRequest.SpellsToCast[0]) && !CastBanes)
                                        {
                                            machine.CurrentRequest.SpellsToCast.RemoveAt(0);
                                        }
                                        else
                                        {
                                            CoreManager.Current.Actions.CastSpell(machine.CurrentRequest.SpellsToCast[0].Id, machine.CurrentRequest.RequesterGuid);
                                        }
                                    }
                                    else
                                    {
                                        ChatManager.Broadcast($"I have run out of spell components.");
                                        machine.CurrentRequest.SpellsToCast.Clear();
                                    }
                                }
                                else
                                {
                                    Spell fallbackSpell = machine.GetFallbackSpell(machine.CurrentRequest.SpellsToCast[0]);
                                    machine.CurrentRequest.SpellsToCast.RemoveAt(0);
                                    if (fallbackSpell != null)
                                    {
                                        machine.CurrentRequest.SpellsToCast.Insert(0, fallbackSpell);
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
