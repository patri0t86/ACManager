using Decal.Adapter;
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
                RevitalizeSelf = CoreManager.Current.Filter<FileService>().SpellTable.GetById(4321);
            }
        }

        public void Exit(Machine machine)
        {

        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (CoreManager.Current.Actions.CombatMode != CombatState.Magic)
                {
                    CoreManager.Current.Actions.SetCombatMode(CombatState.Magic);
                }
                else if (CoreManager.Current.Actions.CombatMode == CombatState.Magic)
                {
                    if (machine.CastStarted && machine.CastCompleted)
                    {
                        if (CoreManager.Current.CharacterFilter.Mana >= Utility.BotSettings.ManaThreshold * CoreManager.Current.CharacterFilter.EffectiveVital[CharFilterVitalType.Mana])
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
                        if (CoreManager.Current.CharacterFilter.Stamina < Utility.BotSettings.StaminaThreshold * CoreManager.Current.CharacterFilter.EffectiveVital[CharFilterVitalType.Stamina])
                        {
                            if (CoreManager.Current.CharacterFilter.IsSpellKnown(RevitalizeSelf.Id) && machine.SpellSkillCheck(RevitalizeSelf) && Inventory.HaveComponents(RevitalizeSelf))
                            {
                                CoreManager.Current.Actions.CastSpell(RevitalizeSelf.Id, 0);
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
            if (CoreManager.Current.CharacterFilter.IsSpellKnown(2345) && machine.SpellSkillCheck(CoreManager.Current.Filter<FileService>().SpellTable.GetById(2345)) && Inventory.HaveComponents(CoreManager.Current.Filter<FileService>().SpellTable.GetById(2345)))
            {
                CoreManager.Current.Actions.CastSpell(2345, 0);
            }
            else if (CoreManager.Current.CharacterFilter.IsSpellKnown(1681) && machine.SpellSkillCheck(CoreManager.Current.Filter<FileService>().SpellTable.GetById(1681)) && Inventory.HaveComponents(CoreManager.Current.Filter<FileService>().SpellTable.GetById(1681)))
            {
                CoreManager.Current.Actions.CastSpell(1681, 0);
            }
            else if (CoreManager.Current.CharacterFilter.IsSpellKnown(1680) && machine.SpellSkillCheck(CoreManager.Current.Filter<FileService>().SpellTable.GetById(1680)) && Inventory.HaveComponents(CoreManager.Current.Filter<FileService>().SpellTable.GetById(1680)))
            {
                CoreManager.Current.Actions.CastSpell(1680, 0);
            }
            else if (CoreManager.Current.CharacterFilter.IsSpellKnown(1679) && machine.SpellSkillCheck(CoreManager.Current.Filter<FileService>().SpellTable.GetById(1679)) && Inventory.HaveComponents(CoreManager.Current.Filter<FileService>().SpellTable.GetById(1679)))
            {
                CoreManager.Current.Actions.CastSpell(1679, 0);
            }
            else if (CoreManager.Current.CharacterFilter.IsSpellKnown(1678) && machine.SpellSkillCheck(CoreManager.Current.Filter<FileService>().SpellTable.GetById(1678)) && Inventory.HaveComponents(CoreManager.Current.Filter<FileService>().SpellTable.GetById(1678)))
            {
                CoreManager.Current.Actions.CastSpell(1678, 0);
            }
            else if (CoreManager.Current.CharacterFilter.IsSpellKnown(1677) && machine.SpellSkillCheck(CoreManager.Current.Filter<FileService>().SpellTable.GetById(1677)) && Inventory.HaveComponents(CoreManager.Current.Filter<FileService>().SpellTable.GetById(1677)))
            {
                CoreManager.Current.Actions.CastSpell(1677, 0);
            }
            else if (CoreManager.Current.CharacterFilter.IsSpellKnown(1676) && machine.SpellSkillCheck(CoreManager.Current.Filter<FileService>().SpellTable.GetById(1676)) && Inventory.HaveComponents(CoreManager.Current.Filter<FileService>().SpellTable.GetById(1676)))
            {
                CoreManager.Current.Actions.CastSpell(1676, 0);
            }
        }

        public override string ToString()
        {
            return nameof(VitalManagement);
        }
    }
}
