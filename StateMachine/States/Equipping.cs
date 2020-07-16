using ACManager.Settings;
using ACManager.StateMachine.Queues;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// A state designed to dress and undress the character as appropriate.
    /// </summary>
    class Equipping : StateBase<Equipping>, IState
    {
        private bool FullyEquipped { get; set; }
        private bool IdleEquipped { get; set; }
        private readonly DateTime UnixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private Dictionary<int, bool> Equipment { get; set; } = new Dictionary<int, bool>();
        private Dictionary<int, bool> IdleEquipment { get; set; } = new Dictionary<int, bool>();

        public void Enter(Machine machine)
        {
            machine.Utility.EquipmentSettings = machine.Utility.LoadEquipmentSettings();

            if (Equipment.Count.Equals(0) && machine.Core.CharacterFilter.Name.Equals(machine.BuffingCharacter))
            {
                foreach (Equipment item in machine.Utility.EquipmentSettings.BuffingEquipment)
                {
                    Equipment.Add(item.Id, false);
                }
            }

            if (IdleEquipment.Count.Equals(0) && machine.Core.CharacterFilter.Name.Equals(machine.BuffingCharacter))
            {
                foreach (Equipment item in machine.Utility.EquipmentSettings.IdleEquipment)
                {
                    IdleEquipment.Add(item.Id, true);
                }
            }

            using (WorldObjectCollection wands = machine.Core.WorldFilter.GetInventory())
            {
                wands.SetFilter(new ByObjectClassFilter(ObjectClass.WandStaffOrb));
                foreach (WorldObject wand in wands)
                {
                    machine.Core.Actions.RequestId(wand.Id);
                }
            }
        }

        public void Exit(Machine machine)
        {
            if (machine.NextState == Idle.GetInstance)
            {
                Equipment.Clear();
                IdleEquipment.Clear();
            }
        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (machine.SpellsToCast.Count > 0 || !machine.IsBuffed)
                {
                    if (FullyEquipped)
                    {
                        if (!machine.IsBuffed && machine.Core.CharacterFilter.Name.Equals(machine.BuffingCharacter))
                        {
                            machine.NextState = SelfBuffing.GetInstance;
                        }
                        else
                        {
                            machine.NextState = Casting.GetInstance;
                        }
                    }
                    else if (Equipment.Count.Equals(0) || !machine.CurrentRequest.RequestType.Equals(RequestType.Buff))
                    {
                        using (WorldObjectCollection wands = machine.Core.WorldFilter.GetInventory())
                        {
                            wands.SetFilter(new ByObjectClassFilter(ObjectClass.WandStaffOrb));

                            if (wands.Count.Equals(0))
                            {
                                machine.ChatManager.Broadcast("Oops, my owner didn't give me a wand I can equip. I'm cancelling this request.");
                                machine.SpellsToCast.Clear();
                                machine.NextState = Idle.GetInstance;
                            }

                            foreach (WorldObject wand in wands)
                            {
                                if (wand.Values(LongValueKey.EquippedSlots) > 0)
                                {
                                    FullyEquipped = true;
                                    IdleEquipped = true;
                                    break;
                                }
                            }

                            if (!FullyEquipped)
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
                    else // equip the entire suit
                    {
                        foreach (KeyValuePair<int, bool> item in IdleEquipment)
                        {
                            if (machine.Core.Actions.BusyState.Equals(0))
                            {
                                if (IdleEquipment[item.Key].Equals(true))
                                {
                                    if (!Equipment.ContainsKey(item.Key))
                                    {
                                        IdleEquipment[item.Key] = false;
                                        machine.Core.Actions.MoveItem(item.Key, machine.Core.CharacterFilter.Id);
                                    }
                                    else
                                    {
                                        Equipment[item.Key] = true;
                                        IdleEquipment[item.Key] = false;
                                    }
                                }
                            }
                        }

                        foreach (KeyValuePair<int, bool> item in Equipment)
                        {
                            if (machine.Core.Actions.BusyState.Equals(0) && !item.Value)
                            {
                                machine.Core.Actions.AutoWield(item.Key);
                                Equipment[item.Key] = true;
                            }
                        }

                        FullyEquipped = true;

                        foreach (KeyValuePair<int, bool> item in Equipment)
                        {
                            if (item.Value == false)
                            {
                                FullyEquipped = false;
                                break;
                            }
                        }

                        if (FullyEquipped)
                        {
                            IdleEquipped = false;
                        }
                    }
                }
                else // done casting/buffing - remove suit
                {
                    if (FullyEquipped)
                    {
                        if (Equipment.Count > 0 && machine.CurrentRequest.RequestType.Equals(RequestType.Buff))
                        {
                            foreach (KeyValuePair<int, bool> item in Equipment)
                            {
                                if (machine.Core.Actions.BusyState.Equals(0))
                                {
                                    if (Equipment[item.Key].Equals(true))
                                    {
                                        if (!IdleEquipment.ContainsKey(item.Key))
                                        {
                                            Equipment[item.Key] = false;
                                            machine.Core.Actions.MoveItem(item.Key, machine.Core.CharacterFilter.Id);
                                        }
                                        else
                                        {
                                            Equipment[item.Key] = false;
                                            IdleEquipment[item.Key] = true;
                                        }
                                    }
                                }
                            }

                            FullyEquipped = false;

                            foreach (KeyValuePair<int, bool> item in Equipment)
                            {
                                if (item.Value == true)
                                {
                                    FullyEquipped = true;
                                    break;
                                }
                            }
                        }
                        else // no suit equipped - remove the wand
                        {
                            using (WorldObjectCollection wands = machine.Core.WorldFilter.GetInventory())
                            {
                                wands.SetFilter(new ByObjectClassFilter(ObjectClass.WandStaffOrb));
                                foreach (WorldObject wand in wands)
                                {
                                    if (wand.Values(LongValueKey.EquippedSlots) > 0)
                                    {
                                        if (machine.Core.Actions.BusyState.Equals(0))
                                        {
                                            machine.Core.Actions.MoveItem(wand.Id, machine.Core.CharacterFilter.Id);
                                            FullyEquipped = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (!IdleEquipped) // suit is unequipped
                    {
                        if (machine.Utility.EquipmentSettings.IdleEquipment.Count.Equals(0))
                        {
                            IdleEquipped = true;
                        }
                        else
                        {
                            foreach (KeyValuePair<int, bool> item in IdleEquipment)
                            {
                                if (machine.Core.Actions.BusyState.Equals(0) && !item.Value)
                                {
                                    IdleEquipment[item.Key] = true;
                                    machine.Core.Actions.AutoWield(item.Key);
                                }
                            }

                            IdleEquipped = true;

                            foreach (KeyValuePair<int, bool> item in IdleEquipment)
                            {
                                if (item.Value == false)
                                {
                                    IdleEquipped = false;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (machine.Core.Actions.BusyState.Equals(0))
                        {
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
