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
        private Dictionary<Equipment, bool> BuffingEquipment { get; set; } = new Dictionary<Equipment, bool>();
        private Dictionary<Equipment, bool> IdleEquipment { get; set; } = new Dictionary<Equipment, bool>();
        private bool BraceletEquipped { get; set; }
        private bool RingEquipped { get; set; }
        private EquipMask Ring { get; set; } = EquipMask.RightRing | EquipMask.LeftRing;
        private EquipMask Bracelet { get; set; } = EquipMask.RightBracelet | EquipMask.LeftBracelet;

        public void Enter(Machine machine)
        {
            machine.Utility.EquipmentSettings = machine.Utility.LoadEquipmentSettings();

            if (BuffingEquipment.Count.Equals(0) && machine.Core.CharacterFilter.Name.Equals(machine.BuffingCharacter))
            {
                foreach (Equipment item in machine.Utility.EquipmentSettings.BuffingEquipment)
                {
                    BuffingEquipment.Add(item, false);
                }
            }

            if (IdleEquipment.Count.Equals(0) && machine.Core.CharacterFilter.Name.Equals(machine.BuffingCharacter))
            {
                foreach (Equipment item in machine.Utility.EquipmentSettings.IdleEquipment)
                {
                    IdleEquipment.Add(item, true);
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
                BuffingEquipment.Clear();
                IdleEquipment.Clear();
                RingEquipped = false;
                BraceletEquipped = false;
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
                    else if (BuffingEquipment.Count.Equals(0) || !machine.CurrentRequest.RequestType.Equals(RequestType.Buff))
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
                        foreach (KeyValuePair<Equipment, bool> item in IdleEquipment)
                        {
                            if (machine.Core.Actions.BusyState.Equals(0))
                            {
                                if (IdleEquipment[item.Key].Equals(true))
                                {
                                    if (!BuffingEquipment.ContainsKey(item.Key))
                                    {
                                        IdleEquipment[item.Key] = false;
                                        machine.Core.Actions.MoveItem(item.Key.Id, machine.Core.CharacterFilter.Id);
                                    }
                                    else
                                    {
                                        BuffingEquipment[item.Key] = true;
                                        IdleEquipment[item.Key] = false;
                                    }
                                }
                            }
                        }                        

                        foreach (KeyValuePair<Equipment, bool> item in BuffingEquipment)
                        {
                            if (machine.Core.Actions.BusyState.Equals(0) && !item.Value)
                            {                         
                                if ((item.Key.EquipMask & (int)Ring) == (int)Ring)
                                {
                                    if (!RingEquipped)
                                    {
                                        RingEquipped = true;
                                        machine.Core.Actions.AutoWield(item.Key.Id, (int)EquipMask.RightRing, 0, 1, 1, 1);
                                    }
                                    else
                                    {
                                        machine.Core.Actions.AutoWield(item.Key.Id, (int)EquipMask.LeftRing, 0, 1, 1, 1);
                                    }
                                }
                                else if ((item.Key.EquipMask & (int)Bracelet) == (int)Bracelet)
                                {
                                    if (!BraceletEquipped)
                                    {
                                        BraceletEquipped = true;
                                        machine.Core.Actions.AutoWield(item.Key.Id, (int)EquipMask.RightBracelet, 0, 1, 1, 1);
                                    }
                                    else
                                    {
                                        machine.Core.Actions.AutoWield(item.Key.Id, (int)EquipMask.LeftBracelet, 0, 1, 1, 1);
                                    }
                                }
                                else
                                {
                                    machine.Core.Actions.AutoWield(item.Key.Id, item.Key.EquipMask, 0, 1);
                                }
                                
                                BuffingEquipment[item.Key] = true;
                            }
                        }

                        FullyEquipped = true;

                        foreach (KeyValuePair<Equipment, bool> item in BuffingEquipment)
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
                        if (BuffingEquipment.Count > 0 && machine.CurrentRequest.RequestType.Equals(RequestType.Buff))
                        {
                            foreach (KeyValuePair<Equipment, bool> item in BuffingEquipment)
                            {
                                if (machine.Core.Actions.BusyState.Equals(0))
                                {
                                    if (BuffingEquipment[item.Key].Equals(true))
                                    {
                                        if (!IdleEquipment.ContainsKey(item.Key))
                                        {
                                            BuffingEquipment[item.Key] = false;
                                            machine.Core.Actions.MoveItem(item.Key.Id, machine.Core.CharacterFilter.Id);
                                        }
                                        else
                                        {
                                            BuffingEquipment[item.Key] = false;
                                            IdleEquipment[item.Key] = true;
                                        }
                                    }
                                }
                            }

                            FullyEquipped = false;

                            foreach (KeyValuePair<Equipment, bool> item in BuffingEquipment)
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
                            foreach (KeyValuePair<Equipment, bool> item in IdleEquipment)
                            {
                                if (machine.Core.Actions.BusyState.Equals(0) && !item.Value)
                                {
                                    IdleEquipment[item.Key] = true;
                                    machine.Core.Actions.AutoWield(item.Key.Id, item.Key.EquipMask, 0, 1, 1, 1);
                                }
                            }

                            IdleEquipped = true;

                            foreach (KeyValuePair<Equipment, bool> item in IdleEquipment)
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

        [Flags]
        public enum EquipMask
        {
            Head = 0x00000001,
            ChestUnderwear = 0x00000002,
            AbdomenUnderwear = 0x00000004,
            UpperArmsUnderwear = 0x00000008,
            LowerArmsUnderwear = 0x00000010,
            Hands = 0x00000020,
            UpperLegsUnderwear = 0x00000040,
            LowerLegsUnderwear = 0x00000080,
            Feet = 0x00000100,
            Chest = 0x00000200,
            Abdomen = 0x00000400,
            UpperArms = 0x00000800,
            LowerArms = 0x00001000,
            UpperLegs = 0x00002000,
            LowerLegs = 0x00004000,
            Necklace = 0x00008000,
            RightBracelet = 0x00010000,
            LeftBracelet = 0x00020000,
            RightRing = 0x00040000,
            LeftRing = 0x00080000,
            MeleeWeapon = 0x00100000,
            Shield = 0x00200000,
            MissileWeapon = 0x00400000,
            Ammunition = 0x00800000,
            Wand = 0x01000000,
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
