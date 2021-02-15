using Decal.Adapter.Wrappers;
using Decal.Filters;

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
                            if (machine.Core.CharacterFilter.IsSpellKnown(RevitalizeSelf.Id) && machine.SpellSkillCheck(RevitalizeSelf) && machine.ComponentChecker.HaveComponents(RevitalizeSelf.Id))
                            {
                                machine.Core.Actions.CastSpell(RevitalizeSelf.Id, 0);
                            }
                            else
                            {
                                RevitalizeSelf = machine.GetFallbackSpell(RevitalizeSelf, true);
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
            if (machine.Core.CharacterFilter.IsSpellKnown(2345) && machine.SpellSkillCheck(machine.SpellTable.GetById(2345)) && machine.ComponentChecker.HaveComponents(2345))
            {
                machine.Core.Actions.CastSpell(2345, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1681) && machine.SpellSkillCheck(machine.SpellTable.GetById(1681)) && machine.ComponentChecker.HaveComponents(1681))
            {
                machine.Core.Actions.CastSpell(1681, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1680) && machine.SpellSkillCheck(machine.SpellTable.GetById(1680)) && machine.ComponentChecker.HaveComponents(1680))
            {
                machine.Core.Actions.CastSpell(1680, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1679) && machine.SpellSkillCheck(machine.SpellTable.GetById(1679)) && machine.ComponentChecker.HaveComponents(1679))
            {
                machine.Core.Actions.CastSpell(1679, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1678) && machine.SpellSkillCheck(machine.SpellTable.GetById(1678)) && machine.ComponentChecker.HaveComponents(1678))
            {
                machine.Core.Actions.CastSpell(1678, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1677) && machine.SpellSkillCheck(machine.SpellTable.GetById(1677)) && machine.ComponentChecker.HaveComponents(1677))
            {
                machine.Core.Actions.CastSpell(1677, 0);
            }
            else if (machine.Core.CharacterFilter.IsSpellKnown(1676) && machine.SpellSkillCheck(machine.SpellTable.GetById(1676)) && machine.ComponentChecker.HaveComponents(1676))
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
