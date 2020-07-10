using Decal.Adapter.Wrappers;
using System;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// A state designed to dress and undress the character as appropriate.
    /// </summary>
    class Equipping : StateBase<Equipping>, IState
    {
        private bool IsWandEquipped { get; set; }
        private WorldObject EquippedWand { get; set; }

        private readonly DateTime UnixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime LastIDCheck { get; set; }

        public void Enter(Machine machine)
        {
            using (WorldObjectCollection wands = machine.Core.WorldFilter.GetInventory())
            {
                wands.SetFilter(new ByObjectClassFilter(ObjectClass.WandStaffOrb));
                foreach (WorldObject wand in wands)
                {
                    machine.Core.Actions.RequestId(wand.Id);
                }
            }
            LastIDCheck = DateTime.UtcNow;
        }

        public void Exit(Machine machine)
        {

        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (machine.SpellsToCast.Count > 0) // need to equip a wand
                {
                    if (IsWandEquipped)
                    {
                        machine.NextState = Casting.GetInstance;
                    }
                    else
                    {
                        using (WorldObjectCollection wands = machine.Core.WorldFilter.GetInventory())
                        {
                            wands.SetFilter(new ByObjectClassFilter(ObjectClass.WandStaffOrb));

                            if (wands.Count.Equals(0))
                            {
                                machine.ChatManager.Broadcast("Oops, my owner didn't give me a wand. I'm cancelling this request.");
                                machine.SpellsToCast.Clear();
                                machine.NextState = Idle.GetInstance;
                            }

                            foreach (WorldObject wand in wands)
                            {
                                if (wand.Values(LongValueKey.EquippedSlots) > 0)
                                {
                                    EquippedWand = wand;
                                    IsWandEquipped = true;
                                    break;
                                }
                            }

                            if (!IsWandEquipped)
                            {
                                foreach (WorldObject wand in wands)
                                {
                                    if (CanWield(machine, wand) && machine.Core.Actions.BusyState.Equals(0))
                                    {
                                        machine.Core.Actions.AutoWield(wand.Id);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else // need to take the wand off
                {
                    if (IsWandEquipped)
                    {
                        if (EquippedWand.HasIdData && !(((int)(LastIDCheck - UnixTime).TotalSeconds - EquippedWand.LastIdTime) > 1))
                        {
                            if (EquippedWand.Values(LongValueKey.EquippedSlots).Equals(0))
                            {
                                IsWandEquipped = false;
                            }
                            else
                            {
                                if (machine.Core.Actions.BusyState.Equals(0))
                                {
                                    machine.Core.Actions.MoveItem(EquippedWand.Id, machine.Core.CharacterFilter.Id);
                                }
                            }
                        }
                        else
                        {
                            LastIDCheck = DateTime.UtcNow;
                            machine.Core.Actions.RequestId(EquippedWand.Id);
                        }
                    }
                    else
                    {
                        if (machine.Core.Actions.BusyState.Equals(0))
                        {
                            EquippedWand.Dispose();
                            machine.NextState = Idle.GetInstance;
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
            return nameof(Equipping);
        }

        private bool CanWield(Machine machine, WorldObject wand)
        {
            if (wand.Values(LongValueKey.WieldReqType).Equals(0))
            {
                return true;
            }

            if (wand.Values(LongValueKey.WieldReqType).Equals(7) || wand.Values(LongValueKey.WieldReqType).Equals(2))
            {
                if (wand.Values(LongValueKey.WieldReqType).Equals(7) && !(machine.Core.CharacterFilter.Level >= wand.Values(LongValueKey.WieldReqValue)))
                {
                    return false;
                }

                if (wand.Values(LongValueKey.WieldReqType).Equals(2) && !(machine.Core.CharacterFilter.EffectiveSkill[(CharFilterSkillType)wand.Values(LongValueKey.WieldReqAttribute)] >= wand.Values(LongValueKey.WieldReqValue)))
                {
                    return false;
                }
            }

            if (wand.Values((LongValueKey)270).Equals(7) || wand.Values((LongValueKey)270).Equals(2))
            {
                if (wand.Values((LongValueKey)270).Equals(7) && !(machine.Core.CharacterFilter.Level >= wand.Values((LongValueKey)272)))
                {
                    return false;
                }

                if (wand.Values((LongValueKey)270).Equals(2) && !(machine.Core.CharacterFilter.EffectiveSkill[(CharFilterSkillType)wand.Values((LongValueKey)271)] >= wand.Values((LongValueKey)271)))
                {
                    return false;
                }
            }

            if (wand.Values((LongValueKey)273).Equals(7) || wand.Values((LongValueKey)273).Equals(2))
            {
                if (wand.Values((LongValueKey)273).Equals(7) && !(machine.Core.CharacterFilter.Level >= wand.Values((LongValueKey)275)))
                {
                    return false;
                }

                if (wand.Values((LongValueKey)273).Equals(2) && !(machine.Core.CharacterFilter.EffectiveSkill[(CharFilterSkillType)wand.Values((LongValueKey)274)] >= wand.Values((LongValueKey)274)))
                {
                    return false;
                }
            }

            if (wand.Values((LongValueKey)276).Equals(7) || wand.Values((LongValueKey)276).Equals(2))
            {
                if (wand.Values((LongValueKey)276).Equals(7) && !(machine.Core.CharacterFilter.Level >= wand.Values((LongValueKey)278)))
                {
                    return false;
                }

                if (wand.Values((LongValueKey)276).Equals(2) && !(machine.Core.CharacterFilter.EffectiveSkill[(CharFilterSkillType)wand.Values((LongValueKey)277)] >= wand.Values((LongValueKey)277)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
