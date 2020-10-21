using Decal.Adapter.Wrappers;
using System.ComponentModel;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// State to handle recovering health/stamina/mana.
    /// </summary>
    class VitalManagement : StateBase<VitalManagement>, IState
    {
        public void Enter(Machine machine)
        {
            machine.CastStarted = false;
            machine.CastCompleted = false;
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
                            RegainStamina(machine);
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

        private void RegainStamina(Machine machine)
        {
            if (machine.Core.CharacterFilter.IsSpellKnown(2083) && machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] >= 350)
            {
                machine.Core.Actions.CastSpell(2083, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1182) && machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] >= 300)
            {
                machine.Core.Actions.CastSpell(1182, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1181) && machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] >= 250)
            {
                machine.Core.Actions.CastSpell(1181, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1180) && machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] >= 200)
            {
                machine.Core.Actions.CastSpell(1180, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1179) && machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] >= 150)
            {
                machine.Core.Actions.CastSpell(1179, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1178) && machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] >= 100)
            {
                machine.Core.Actions.CastSpell(1178, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1177) && machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] >= 50)
            {
                machine.Core.Actions.CastSpell(1177, 0);
            }
        }

        private void RegainMana(Machine machine)
        {
            if (machine.Core.CharacterFilter.IsSpellKnown(2345) && machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] >= 350)
            {
                machine.Core.Actions.CastSpell(2345, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1681) && machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] >= 300)
            {
                machine.Core.Actions.CastSpell(1681, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1680) && machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] >= 250)
            {
                machine.Core.Actions.CastSpell(1680, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1679) && machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] >= 200)
            {
                machine.Core.Actions.CastSpell(1679, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1678) && machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] >= 150)
            {
                machine.Core.Actions.CastSpell(1678, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1677) && machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] >= 100)
            {
                machine.Core.Actions.CastSpell(1677, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1676) && machine.Core.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] >= 50)
            {
                machine.Core.Actions.CastSpell(1676, 0);
            }
        }

        public override string ToString()
        {
            return nameof(VitalManagement);
        }
    }
}
