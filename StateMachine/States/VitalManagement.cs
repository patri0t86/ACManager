using Decal.Adapter.Wrappers;

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
            if (machine.Core.Actions.CombatMode != CombatState.Magic)
            {
                machine.Core.Actions.SetCombatMode(CombatState.Magic);
            }
        }

        public void Exit(Machine machine)
        {

        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (machine.Core.CharacterFilter.Mana >= machine.ManaThreshold * machine.MaxVitals[CharFilterVitalType.Mana])
                {
                    if (machine.CastStarted && machine.CastCompleted)
                    {
                        machine.NextState = Casting.GetInstance;
                    }
                }
                else if (machine.Core.CharacterFilter.Stamina < machine.StaminaThreshold * machine.MaxVitals[CharFilterVitalType.Stamina])
                {
                    if (machine.CastStarted && machine.CastCompleted)
                    {
                        machine.CastCompleted = false;
                        machine.CastStarted = false;
                        if (machine.SpellBook.Contains(2083) && machine.Skills[CharFilterSkillType.LifeMagic] >= 350)
                        {
                            machine.Core.Actions.CastSpell(2083, 0);
                        }
                        else if (machine.SpellBook.Contains(1182) && machine.Skills[CharFilterSkillType.LifeMagic] >= 300)
                        {
                            machine.Core.Actions.CastSpell(1182, 0);
                        }
                        else if (machine.SpellBook.Contains(1181) && machine.Skills[CharFilterSkillType.LifeMagic] >= 250)
                        {
                            machine.Core.Actions.CastSpell(1181, 0);
                        }
                        else if (machine.SpellBook.Contains(1180) && machine.Skills[CharFilterSkillType.LifeMagic] >= 200)
                        {
                            machine.Core.Actions.CastSpell(1180, 0);
                        }
                        else if (machine.SpellBook.Contains(1179) && machine.Skills[CharFilterSkillType.LifeMagic] >= 150)
                        {
                            machine.Core.Actions.CastSpell(1179, 0);
                        }
                        else if (machine.SpellBook.Contains(1178) && machine.Skills[CharFilterSkillType.LifeMagic] >= 100)
                        {
                            machine.Core.Actions.CastSpell(1178, 0);
                        }
                        else if (machine.SpellBook.Contains(1177) && machine.Skills[CharFilterSkillType.LifeMagic] >= 50)
                        {
                            machine.Core.Actions.CastSpell(1177, 0);
                        }
                    }
                    else if (!machine.CastStarted)
                    {
                        if (machine.SpellBook.Contains(2083) && machine.Skills[CharFilterSkillType.LifeMagic] >= 350)
                        {
                            machine.Core.Actions.CastSpell(2083, 0);
                        }
                        else if (machine.SpellBook.Contains(1182) && machine.Skills[CharFilterSkillType.LifeMagic] >= 300)
                        {
                            machine.Core.Actions.CastSpell(1182, 0);
                        }
                        else if (machine.SpellBook.Contains(1181) && machine.Skills[CharFilterSkillType.LifeMagic] >= 250)
                        {
                            machine.Core.Actions.CastSpell(1181, 0);
                        }
                        else if (machine.SpellBook.Contains(1180) && machine.Skills[CharFilterSkillType.LifeMagic] >= 200)
                        {
                            machine.Core.Actions.CastSpell(1180, 0);
                        }
                        else if (machine.SpellBook.Contains(1179) && machine.Skills[CharFilterSkillType.LifeMagic] >= 150)
                        {
                            machine.Core.Actions.CastSpell(1179, 0);
                        }
                        else if (machine.SpellBook.Contains(1178) && machine.Skills[CharFilterSkillType.LifeMagic] >= 100)
                        {
                            machine.Core.Actions.CastSpell(1178, 0);
                        }
                        else if (machine.SpellBook.Contains(1177) && machine.Skills[CharFilterSkillType.LifeMagic] >= 50)
                        {
                            machine.Core.Actions.CastSpell(1177, 0);
                        }
                    }
                }
                else
                {
                    if (machine.CastStarted && machine.CastCompleted)
                    {
                        machine.CastCompleted = false;
                        machine.CastStarted = false;
                        if (machine.SpellBook.Contains(2345) && machine.Skills[CharFilterSkillType.LifeMagic] >= 350)
                        {
                            machine.Core.Actions.CastSpell(2345, 0);
                        }
                        else if (machine.SpellBook.Contains(1681) && machine.Skills[CharFilterSkillType.LifeMagic] >= 300)
                        {
                            machine.Core.Actions.CastSpell(1681, 0);
                        }
                        else if (machine.SpellBook.Contains(1680) && machine.Skills[CharFilterSkillType.LifeMagic] >= 250)
                        {
                            machine.Core.Actions.CastSpell(1680, 0);
                        }
                        else if (machine.SpellBook.Contains(1679) && machine.Skills[CharFilterSkillType.LifeMagic] >= 200)
                        {
                            machine.Core.Actions.CastSpell(1679, 0);
                        }
                        else if (machine.SpellBook.Contains(1678) && machine.Skills[CharFilterSkillType.LifeMagic] >= 150)
                        {
                            machine.Core.Actions.CastSpell(1678, 0);
                        }
                        else if (machine.SpellBook.Contains(1677) && machine.Skills[CharFilterSkillType.LifeMagic] >= 100)
                        {
                            machine.Core.Actions.CastSpell(1677, 0);
                        }
                        else if (machine.SpellBook.Contains(1676) && machine.Skills[CharFilterSkillType.LifeMagic] >= 50)
                        {
                            machine.Core.Actions.CastSpell(1676, 0);
                        }
                    }
                    else if (!machine.CastStarted)
                    {
                        if (machine.SpellBook.Contains(2345) && machine.Skills[CharFilterSkillType.LifeMagic] >= 350)
                        {
                            machine.Core.Actions.CastSpell(2345, 0);
                        }
                        else if (machine.SpellBook.Contains(1681) && machine.Skills[CharFilterSkillType.LifeMagic] >= 300)
                        {
                            machine.Core.Actions.CastSpell(1681, 0);
                        }
                        else if (machine.SpellBook.Contains(1680) && machine.Skills[CharFilterSkillType.LifeMagic] >= 250)
                        {
                            machine.Core.Actions.CastSpell(1680, 0);
                        }
                        else if (machine.SpellBook.Contains(1679) && machine.Skills[CharFilterSkillType.LifeMagic] >= 200)
                        {
                            machine.Core.Actions.CastSpell(1679, 0);
                        }
                        else if (machine.SpellBook.Contains(1678) && machine.Skills[CharFilterSkillType.LifeMagic] >= 150)
                        {
                            machine.Core.Actions.CastSpell(1678, 0);
                        }
                        else if (machine.SpellBook.Contains(1677) && machine.Skills[CharFilterSkillType.LifeMagic] >= 100)
                        {
                            machine.Core.Actions.CastSpell(1677, 0);
                        }
                        else if (machine.SpellBook.Contains(1676) && machine.Skills[CharFilterSkillType.LifeMagic] >= 50)
                        {
                            machine.Core.Actions.CastSpell(1676, 0);
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
            return nameof(VitalManagement);
        }
    }
}
