using ACManager.Settings;
using Decal.Adapter.Wrappers;
using Decal.Filters;
using System;
using System.Collections.Generic;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// State to handle buffing the buff bot itself.
    /// </summary>
    class SelfBuffing : StateBase<SelfBuffing>, IState
    {
        private bool StartedBuffing { get; set; }
        private bool AddedPreBuffs { get; set; }
        private DateTime EnteredState { get; set; }
        public void Enter(Machine machine)
        {
            EnteredState = DateTime.Now;
            if (!StartedBuffing)
            {
                machine.ChatManager.SendTell(machine.CurrentRequest.RequesterName, "I need to buff myself, standby.");
                BuffProfile profile = machine.Utility.GetProfile("botbuffs");
                machine.SpellsToCast.Clear();
                foreach (Buff buff in profile.Buffs)
                {
                    machine.SpellsToCast.Add(machine.SpellTable.GetById(buff.Id));
                }
                StartedBuffing = true;
            }
        }

        public void Exit(Machine machine)
        {
            AddedPreBuffs = false;
        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (machine.SpellsToCast.Count > 0)
                {
                    if (machine.CastStarted && machine.CastCompleted && !machine.Fizzled)
                    {
                        machine.SpellsToCast.RemoveAt(0);
                        machine.CastCompleted = false;
                        machine.CastStarted = false;
                    }
                    else if (machine.CastStarted && machine.CastCompleted && machine.Fizzled)
                    {
                        machine.Fizzled = false;
                        machine.CastCompleted = false;
                        machine.CastStarted = false;
                    }
                    else if (!machine.CastStarted)
                    {
                        if (machine.Core.CharacterFilter.Mana < machine.ManaThreshold * machine.Core.CharacterFilter.EffectiveVital[CharFilterVitalType.Mana] && machine.Core.Actions.SkillTrainLevel[Decal.Adapter.Wrappers.SkillType.BaseLifeMagic] != 1)
                        {
                            machine.NextState = VitalManagement.GetInstance;
                        }
                        else
                        {
                            if (machine.Core.Actions.CombatMode != CombatState.Magic)
                            {
                                if ((DateTime.Now - EnteredState).TotalSeconds > 1)
                                {
                                    machine.ChatManager.SendTell(machine.CurrentRequest.RequesterName, "I don't have a wand setup, I'm sorry. Cancelling this request.");
                                    machine.SpellsToCast.Clear();
                                    machine.CurrentRequest.SpellsToCast.Clear();
                                }
                                else
                                {
                                    machine.Core.Actions.SetCombatMode(CombatState.Magic);
                                }
                            }
                            else if (machine.Core.CharacterFilter.IsSpellKnown(machine.SpellsToCast[0].Id))
                            {
                                if (machine.ComponentChecker.HaveComponents(machine.SpellsToCast[0].Id))
                                {
                                    //if (machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.CreatureEnchantment] < 400 && !AddedPreBuffs && !machine.Level7Self)
                                    //{
                                    //    AddedPreBuffs = true;
                                    //    machine.SpellsToCast.Insert(0, machine.SpellTable.GetById(2067)); // focus 7
                                    //    machine.SpellsToCast.Insert(0, machine.SpellTable.GetById(2091)); // self 7
                                    //    machine.SpellsToCast.Insert(0, machine.SpellTable.GetById(2215)); // creature 7
                                    //}
                                    //else
                                    //{
                                        if (machine.Level7Self && machine.SpellsToCast[0].Difficulty > 300)
                                        {
                                            Spell fallbackSpell = FallbackBuffCheck(machine.SpellTable, machine.SpellsToCast[0]);
                                            machine.SpellsToCast.RemoveAt(0);
                                            if (fallbackSpell != null)
                                            {
                                                machine.SpellsToCast.Insert(0, fallbackSpell);
                                            }
                                        }
                                        else
                                        {
                                            machine.Core.Actions.CastSpell(machine.SpellsToCast[0].Id, 0);
                                        }                                        
                                    //}
                                }
                                else
                                {
                                    machine.ChatManager.Broadcast($"I have run out of spell components.");
                                    machine.SpellsToCast.Clear();
                                    machine.Requests.Clear();
                                }
                            }
                            else
                            {
                                Spell fallbackSpell = FallbackBuffCheck(machine.SpellTable, machine.SpellsToCast[0]);
                                machine.SpellsToCast.RemoveAt(0);
                                if (fallbackSpell != null)
                                {
                                    machine.SpellsToCast.Insert(0, fallbackSpell);
                                }
                            }
                        }
                    }
                }
                else
                {
                    StartedBuffing = false;
                    machine.IsBuffed = true;
                    machine.SpellsToCast.AddRange(machine.CurrentRequest.SpellsToCast);
                    machine.NextState = Equipping.GetInstance;
                }
            }
            else
            {
                machine.NextState = Idle.GetInstance;
            }
        }

        public override string ToString()
        {
            return nameof(SelfBuffing);
        }

        private Spell FallbackBuffCheck(SpellTable spellTable, Spell spell)
        {
            List<Spell> spellFamily = new List<Spell>();
            for (int i = 1; i < spellTable.Length; i++)
            {
                if (spellTable[i].Family.Equals(spell.Family) &&
                    spellTable[i].Difficulty < spell.Difficulty &&
                    spellTable[i].IsUntargetted &&
                    !spellTable[i].IsFellowship &&
                    spellTable[i].Duration >= 1800 &&
                    spellTable[i].Duration < spell.Duration)
                {
                    spellFamily.Add(spellTable[i]);
                }
            }

            int maxDiff = 0;
            Spell fallback = null;

            foreach (Spell sp in spellFamily)
            {
                if (sp.Difficulty > maxDiff)
                {
                    fallback = sp;
                    maxDiff = sp.Difficulty;
                }
            }
            return fallback;
        }
    }
}
