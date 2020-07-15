using ACManager.Settings;
using Decal.Adapter.Wrappers;
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
        public void Enter(Machine machine)
        {
            if (!StartedBuffing)
            {
                machine.ChatManager.SendTell(machine.CurrentRequest.RequesterName, "I need to buff myself, standy.");
                BuffProfile profile = machine.Level7Self ? machine.Utility.GetProfile("botbuffs7") : machine.Utility.GetProfile("botbuffs");
                machine.SpellsToCast.Clear();
                foreach (Buff buff in profile.Buffs)
                {
                    machine.SpellsToCast.Add(buff.SpellId);
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
                        if (machine.Core.CharacterFilter.Mana < machine.ManaThreshold * machine.Core.CharacterFilter.EffectiveVital[CharFilterVitalType.Mana] && machine.Core.Actions.SkillTrainLevel[SkillType.BaseLifeMagic] != 1)
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
                                    if (machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.CreatureEnchantment] < 400 && !AddedPreBuffs)
                                    {
                                        AddedPreBuffs = true;
                                        machine.SpellsToCast.Insert(0, 2067); // focus 7
                                        machine.SpellsToCast.Insert(0, 2091); // self 7
                                        machine.SpellsToCast.Insert(0, 2215); // creature 7
                                    }
                                    else
                                    {
                                        machine.Core.Actions.CastSpell(machine.SpellsToCast[0], 0);
                                    }
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
                                int fallbackSpell = FallbackBuffCheck(machine.SpellsToCast[0]);
                                machine.SpellsToCast.RemoveAt(0);
                                if (fallbackSpell != 0)
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

        private int FallbackBuffCheck(int spellId)
        {
            List<int> selfBuffs = new List<int>()
            {
                4530, // creature self 8
                4305, // focus self 8
                4329, // willpower self 8
                4299, // endurance self 8
                4564, // item self 8
                4582, // life self 8
                4602, // mana c self 8
                4494, // mana renewal 8
                4498, // rejuv 8
                4510  // arcane enlightenment 8
            };

            if (selfBuffs.Contains(spellId))
            {
                if (spellId.Equals(4530))
                {
                    return 2215;
                }
                else if (spellId.Equals(4305))
                {
                    return 2067;
                }
                else if (spellId.Equals(4329))
                {
                    return 2091;
                }
                else if (spellId.Equals(4299))
                {
                    return 2061;
                }
                else if (spellId.Equals(4564))
                {
                    return 2249;
                }
                else if (spellId.Equals(4582))
                {
                    return 2267;
                }
                else if (spellId.Equals(4602))
                {
                    return 2287;
                }
                else if (spellId.Equals(4494))
                {
                    return 2183;
                }
                else if (spellId.Equals(4498))
                {
                    return 2187;
                }
                else if (spellId.Equals(4510))
                {
                    return 2195;
                }
            }
            return 0;
        }
    }
}
