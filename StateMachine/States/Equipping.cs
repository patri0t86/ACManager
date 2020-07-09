using Decal.Adapter.Wrappers;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// A state designed to dress and undress the character as appropriate.
    /// </summary>
    class Equipping : StateBase<Equipping>, IState
    {
        private bool WandEquipped { get; set; }
        private int WandId { get; set; }
        private int WandInventoryCount { get; set; }

        public void Enter(Machine machine)
        {
            using (WorldObjectCollection wands = machine.Core.WorldFilter.GetInventory())
            {
                wands.SetFilter(new ByObjectClassFilter(ObjectClass.WandStaffOrb));
                WandInventoryCount = wands.Quantity;
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
                    if (WandEquipped)
                    {
                        machine.NextState = Casting.GetInstance;
                    }
                    else if (WandInventoryCount > 0)
                    {
                        using (WorldObjectCollection wands = machine.Core.WorldFilter.GetInventory())
                        {
                            wands.SetFilter(new ByObjectClassFilter(ObjectClass.WandStaffOrb));
                            foreach (WorldObject wand in wands)
                            {
                                if (wand.Values(LongValueKey.EquippedSlots) > 0)
                                {
                                    WandId = wand.Id;
                                    WandEquipped = true;
                                    break;
                                }
                                else if (wand.HasIdData)
                                {
                                    if (CanWield(machine, wand))
                                    {
                                        machine.Core.Actions.AutoWield(wand.Id);
                                    }
                                }
                                else if (wand.LastIdTime.Equals(0))
                                {
                                    machine.Core.Actions.RequestId(wand.Id);
                                }
                            }
                        }
                    }
                    else
                    {
                        machine.ChatManager.Broadcast("Oops, my owner didn't give me a wand. I'm cancelling this request. How embarassing!");
                        machine.SpellsToCast.Clear();
                    }
                }
                else if (WandEquipped)
                {
                    machine.Core.Actions.MoveItem(WandId, machine.Core.CharacterFilter.Id);
                    WandEquipped = false;
                }
                else
                {
                    machine.NextState = Idle.GetInstance;
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
