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
        private bool StartedBuffCycle { get; set; }
        private Spell LastSpell { get; set; }
        private bool LostTarget { get; set; }
        private DateTime StartedTracking { get; set; }
        private int SpellCastCount { get; set; }
        
        public void Enter(Machine machine)
        {
            machine.Fizzled = false;
            machine.CastCompleted = false;
            machine.CastStarted = false;
            LostTarget = false;

            if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff) && !StartedBuffCycle)
            {
                Started = DateTime.Now;
                StartedBuffCycle = true;
                SpellCastCount = 0;

                ChatManager.Broadcast(Inventory.ReportOnLowComponents());
                TimeSpan buffTime = TimeSpan.FromSeconds(machine.CurrentRequest.SpellsToCast.Count * 4);
                ChatManager.SendTell(machine.CurrentRequest.RequesterName, $"Casting {machine.CurrentRequest.SpellsToCast.Count} buffs on you. This should take about {buffTime.Minutes} minutes and {buffTime.Seconds} seconds.");
            }
        }

        public void Exit(Machine machine)
        {
            
        }

        public void Process(Machine machine)
        {
            if (!machine.Enabled)
            {
                StartedBuffCycle = false;
                machine.NextState = Equipping.GetInstance;
                return;
            }

            if (machine.CurrentRequest.SpellsToCast.Count.Equals(0))
            {
                if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff))
                {
                    StartedBuffCycle = false;
                    TimeSpan duration = DateTime.Now - Started;
                    ChatManager.SendTell(machine.CurrentRequest.RequesterName, $"Your request is complete with {SpellCastCount} buffs, it took {duration.Minutes} minutes and {duration.Seconds} seconds.");
                    LastSpell = null;
                }

                if (machine.Requests.Count.Equals(0))
                {
                    machine.NextState = Equipping.GetInstance;
                    return;
                }

                if (machine.Requests.Peek().RequestType.Equals(RequestType.Buff))
                {
                    machine.CurrentRequest = machine.Requests.Dequeue();
                    Started = DateTime.Now;
                    StartedBuffCycle = true;
                    SpellCastCount = 0;

                    ChatManager.Broadcast(Inventory.ReportOnLowComponents());
                    TimeSpan buffTime = TimeSpan.FromSeconds(machine.CurrentRequest.SpellsToCast.Count * 4);
                    ChatManager.SendTell(machine.CurrentRequest.RequesterName, $"Casting {machine.CurrentRequest.SpellsToCast.Count} buffs on you. This should take about {buffTime.Minutes} minutes and {buffTime.Seconds} seconds.");
                }
                else
                {
                    machine.NextState = Equipping.GetInstance;
                }
                return;
            }

            if (!CoreManager.Current.Actions.CombatMode.Equals(CombatState.Magic))
            {
                CoreManager.Current.Actions.SetCombatMode(CombatState.Magic);
                return;
            }
            
            if (machine.CastStarted && machine.CastCompleted && !machine.Fizzled)
            {
                if (machine.CurrentRequest.SpellsToCast[0].Id.Equals(157) || machine.CurrentRequest.SpellsToCast[0].Id.Equals(2648))
                {
                    machine.PortalsSummonedThisSession += 1;
                    ChatManager.Broadcast($"/s Portal now open to {(string.IsNullOrEmpty(machine.CurrentRequest.Destination) ? "Portal now open." : machine.CurrentRequest.Destination)}. Safe journey, friend.");
                }
                else
                {
                    SpellCastCount += 1;
                }
                machine.CurrentRequest.SpellsToCast.RemoveAt(0);
                machine.CastCompleted = false;
                machine.CastStarted = false;
                return;
            }


            if (machine.CastStarted && machine.CastCompleted && machine.Fizzled)
            {
                machine.Fizzled = false;
                machine.CastCompleted = false;
                machine.CastStarted = false;
                return;
            }

            if (CoreManager.Current.CharacterFilter.Mana < Utility.BotSettings.ManaThreshold * CoreManager.Current.CharacterFilter.EffectiveVital[CharFilterVitalType.Mana]
                && !CoreManager.Current.Actions.SkillTrainLevel[Decal.Adapter.Wrappers.SkillType.BaseLifeMagic].Equals(1))
            {
                machine.NextState = VitalManagement.GetInstance;
                return;
            }

            foreach (string cancel in machine.CancelList)
            {
                if (machine.CurrentRequest.RequesterName.Equals(cancel))
                {
                    machine.CurrentRequest.SpellsToCast.Clear();
                    machine.CancelList.Remove(cancel);
                    return;
                }
            }

            if (machine.CastStarted)
            {
                return;
            }

            if (!(CoreManager.Current.CharacterFilter.IsSpellKnown(machine.CurrentRequest.SpellsToCast[0].Id) && machine.SpellSkillCheck(machine.CurrentRequest.SpellsToCast[0])))
            {
                Spell fallbackSpell = machine.GetFallbackSpell(machine.CurrentRequest.SpellsToCast[0]);
                machine.CurrentRequest.SpellsToCast.RemoveAt(0);
                if (fallbackSpell != null)
                {
                    machine.CurrentRequest.SpellsToCast.Insert(0, fallbackSpell);
                }
                return;
            }

            if (!Inventory.HaveComponents(machine.CurrentRequest.SpellsToCast[0]))
            {
                ChatManager.Broadcast($"I have run out of spell components.");
                machine.CurrentRequest.SpellsToCast.Clear();
                return;
            }

            if (LastSpell != null
                && LastSpell.Equals(machine.CurrentRequest.SpellsToCast[0])
                && !(machine.CurrentRequest.SpellsToCast[0].Id.Equals(157)
                || machine.CurrentRequest.SpellsToCast[0].Id.Equals(2648)))
            {
                if (!LostTarget)
                {
                    StartedTracking = DateTime.Now;
                    LostTarget = true;
                }
                else if ((DateTime.Now - StartedTracking).TotalSeconds > 5)
                {
                    machine.CurrentRequest.SpellsToCast.Clear();
                    return;
                }
            } 
            else
            {
                LastSpell = machine.CurrentRequest.SpellsToCast[0];
                LostTarget = false;
            }

            if (IsBane(machine.CurrentRequest.SpellsToCast[0]) && !IsBaneable(machine.CurrentRequest.RequesterGuid))
            {
                while(IsBane(machine.CurrentRequest.SpellsToCast[0]))
                {
                    machine.CurrentRequest.SpellsToCast.RemoveAt(0);
                }
            }

            CoreManager.Current.Actions.CastSpell(machine.CurrentRequest.SpellsToCast[0].Id, machine.CurrentRequest.RequesterGuid);
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

        private bool IsBaneable(int targetGuid)
        {
            using (WorldObjectCollection items = CoreManager.Current.WorldFilter.GetByContainer(targetGuid))
            {
                items.SetFilter(new ByObjectClassFilter(ObjectClass.Armor));
                if (items.Count > 0)
                {
                    if (!items.First.Name.Contains("Covenant"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
