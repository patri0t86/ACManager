using ACManager.Settings;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Filters;
using System;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// State to handle buffing the buff bot itself.
    /// </summary>
    class SelfBuffing : StateBase<SelfBuffing>, IState
    {
        private bool StartedBuffing { get; set; }
        private DateTime EnteredState { get; set; }
        public void Enter(Machine machine)
        {
            EnteredState = DateTime.Now;
            if (!StartedBuffing)
            {
                ChatManager.SendTell(machine.CurrentRequest.RequesterName, "I need to buff myself, standby.");
                BuffProfile profile = Utility.GetProfile("botbuffs");
                machine.SpellsToCast.Clear();

                foreach (Buff buff in profile.Buffs)
                {
                    if (machine.Level7Self && CoreManager.Current.Filter<FileService>().SpellTable.GetById(buff.Id).Difficulty > 300)
                    {
                        machine.SpellsToCast.Add(machine.GetFallbackSpell(CoreManager.Current.Filter<FileService>().SpellTable.GetById(buff.Id), true));
                    }
                    else
                    {
                        machine.SpellsToCast.Add(CoreManager.Current.Filter<FileService>().SpellTable.GetById(buff.Id));
                    }
                }

                if (CoreManager.Current.CharacterFilter.EffectiveSkill[CharFilterSkillType.CreatureEnchantment] < 400 && !machine.Level7Self)
                {
                    machine.SpellsToCast.Insert(0, CoreManager.Current.Filter<FileService>().SpellTable.GetById(2215)); // creature 7
                    machine.SpellsToCast.Insert(0, CoreManager.Current.Filter<FileService>().SpellTable.GetById(2067)); // focus 7
                    machine.SpellsToCast.Insert(0, CoreManager.Current.Filter<FileService>().SpellTable.GetById(2091)); // self 7
                }

                StartedBuffing = true;
            }
        }

        public void Exit(Machine machine)
        {

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
                        if (CoreManager.Current.CharacterFilter.Mana < machine.ManaThreshold * CoreManager.Current.CharacterFilter.EffectiveVital[CharFilterVitalType.Mana] && CoreManager.Current.Actions.SkillTrainLevel[Decal.Adapter.Wrappers.SkillType.BaseLifeMagic] != 1)
                        {
                            machine.NextState = VitalManagement.GetInstance;
                        }
                        else
                        {
                            if (CoreManager.Current.Actions.CombatMode != CombatState.Magic)
                            {
                                if ((DateTime.Now - EnteredState).TotalSeconds > 1)
                                {
                                    ChatManager.SendTell(machine.CurrentRequest.RequesterName, "I don't have a wand setup, I'm sorry. Cancelling this request.");
                                    machine.SpellsToCast.Clear();
                                    machine.CurrentRequest.SpellsToCast.Clear();
                                }
                                else
                                {
                                    CoreManager.Current.Actions.SetCombatMode(CombatState.Magic);
                                }
                            }
                            else if (CoreManager.Current.CharacterFilter.IsSpellKnown(machine.SpellsToCast[0].Id) && machine.SpellSkillCheck(machine.SpellsToCast[0]))
                            {
                                if (Inventory.HaveComponents(machine.SpellsToCast[0]))
                                {
                                    CoreManager.Current.Actions.CastSpell(machine.SpellsToCast[0].Id, 0);
                                }
                                else
                                {
                                    ChatManager.Broadcast($"I have run out of spell components.");
                                    machine.SpellsToCast.Clear();
                                    machine.Requests.Clear();
                                }
                            }
                            else
                            {
                                Spell fallbackSpell = machine.GetFallbackSpell(machine.SpellsToCast[0], true);
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
    }
}
