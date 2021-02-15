using Decal.Adapter.Wrappers;
using Decal.Filters;
using System.Collections.Generic;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// State to handle recovering health/stamina/mana.
    /// </summary>
    class VitalManagement : StateBase<VitalManagement>, IState
    {
        private Spell RevitalizeSelf = null;

        public void Enter(Machine machine)
        {
            machine.CastStarted = false;
            machine.CastCompleted = false;

            if (RevitalizeSelf == null)
            {
                RevitalizeSelf = machine.SpellTable.GetById(4321);
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
                else if (machine.Core.Actions.CombatMode == CombatState.Magic)
                {
                    if (machine.CastStarted && machine.CastCompleted)
                    {
                        if (machine.Core.CharacterFilter.Mana >= machine.ManaThreshold * machine.Core.CharacterFilter.EffectiveVital[CharFilterVitalType.Mana])
                        {
                            if (machine.IsBuffed)
                            {
                                machine.NextState = Casting.GetInstance;
                            }
                            else
                            {
                                machine.NextState = SelfBuffing.GetInstance;
                            }
                        }
                        machine.CastStarted = false;
                        machine.CastCompleted = false;
                    }
                    else if (!machine.CastStarted)
                    {
                        if (machine.Core.CharacterFilter.Stamina < machine.StaminaThreshold * machine.Core.CharacterFilter.EffectiveVital[CharFilterVitalType.Stamina])
                        {
                            if (machine.Core.CharacterFilter.IsSpellKnown(RevitalizeSelf.Id) && SkillCheck(machine, RevitalizeSelf))
                            {
                                machine.Core.Actions.CastSpell(RevitalizeSelf.Id, 0);
                            }
                            else
                            {
                                FallbackCheck(machine.SpellTable, RevitalizeSelf);
                            }
                        }
                        else
                        {
                            RegainMana(machine);
                        }
                    }
                }
            }
            else
            {
                machine.NextState = Idle.GetInstance;
            }
        }

        private void RegainMana(Machine machine)
        {
            if (machine.Core.CharacterFilter.IsSpellKnown(2345) && SkillCheck(machine, machine.SpellTable.GetById(2345)))
            {
                machine.Core.Actions.CastSpell(2345, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1681) && SkillCheck(machine, machine.SpellTable.GetById(1681)))
            {
                machine.Core.Actions.CastSpell(1681, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1680) && SkillCheck(machine, machine.SpellTable.GetById(1680)))
            {
                machine.Core.Actions.CastSpell(1680, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1679) && SkillCheck(machine, machine.SpellTable.GetById(1679)))
            {
                machine.Core.Actions.CastSpell(1679, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1678) && SkillCheck(machine, machine.SpellTable.GetById(1678)))
            {
                machine.Core.Actions.CastSpell(1678, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1677) && SkillCheck(machine, machine.SpellTable.GetById(1677)))
            {
                machine.Core.Actions.CastSpell(1677, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1676) && SkillCheck(machine, machine.SpellTable.GetById(1676)))
            {
                machine.Core.Actions.CastSpell(1676, 0);
            }
        }

        public override string ToString()
        {
            return nameof(VitalManagement);
        }

        private void FallbackCheck(SpellTable spellTable, Spell spell)
        {
            List<Spell> spellFamily = new List<Spell>();
            for (int i = 1; i < spellTable.Length; i++)
            {
                if (spellTable[i].Family.Equals(spell.Family) &&
                    spellTable[i].Difficulty < spell.Difficulty &&
                    spellTable[i].IsUntargetted &&
                    !spellTable[i].IsFellowship &&
                    spellTable[i].Duration == -1)
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

            if (fallback.Family.Equals(RevitalizeSelf.Family))
            {
                RevitalizeSelf = fallback;
            }
        }

        private bool SkillCheck(Machine machine, Spell spell)
        {
            return machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] + machine.SkillOverride >= spell.Difficulty + 20;
        }
    }
}
