using ACManager.Settings;
using ACManager.StateMachine.Queues;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// This is the idle state, where the bot is awaiting commands.
    /// This state is entered and exited many times throughout the lifecycle. Upon entry, ensures it is in a peaceful stance.
    /// </summary>
    internal class Idle : StateBase<Idle>, IState
    {
        private DateTime BuffCheck { get; set; }
        public void Enter(Machine machine)
        {
            machine.Inventory.GetComponentLevels();
            machine.Inventory.UpdateInventoryFile();
            using (WorldObjectCollection inventory = machine.Core.WorldFilter.GetInventory())
            {
                inventory.SetFilter(new ByObjectClassFilter(ObjectClass.WandStaffOrb));
                foreach (WorldObject item in inventory)
                {
                    if (!item.HasIdData)
                    {
                        machine.Core.Actions.RequestId(item.Id);
                    }
                }
            }
        }

        public void Exit(Machine machine)
        {
            machine.Inventory.GetComponentLevels();
        }

        /// <summary>
        /// Order of operations:
        /// Go to peace mode, if not there
        /// Move the bot to the correct location and heading in the world depending on use/idle status
        /// Switch characters if portal/item is not on this character
        /// Equip items -> Cast spells -> Dequip items -> return
        /// Use item
        /// Broadcast advertisement / low on spell comps
        /// Set machine variables depending on status
        /// </summary>
        /// <param name="machine"></param>
        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (machine.Core.Actions.CombatMode != CombatState.Peace)
                {
                    machine.Core.Actions.SetCombatMode(CombatState.Peace);
                }
                else if ((!string.IsNullOrEmpty(machine.ItemToUse) || machine.SpellsToCast.Count > 0) && !machine.Core.CharacterFilter.Name.Equals(machine.NextCharacter))
                {
                    machine.NextState = SwitchingCharacters.GetInstance;
                }
                else if ((machine.EnablePositioning && (!machine.InPosition() || !machine.CorrectHeading())) || !machine.CorrectHeading())
                {
                    machine.NextState = Positioning.GetInstance;
                }
                else if (machine.SpellsToCast.Count > 0 && machine.Core.CharacterFilter.Name.Equals(machine.NextCharacter))
                {
                    if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff) && machine.Core.CharacterFilter.Name.Equals(machine.BuffingCharacter))
                    {
                        machine.IsBuffed = HaveAllBuffs(machine);
                    }
                    else
                    {
                        machine.IsBuffed = true;
                    }
                    machine.NextState = Equipping.GetInstance;
                }
                else if (!string.IsNullOrEmpty(machine.ItemToUse) && machine.Core.CharacterFilter.Name.Equals(machine.NextCharacter))
                {
                    machine.NextState = UseItem.GetInstance;
                }
                else if (machine.Requests.Count > 0)
                {
                    machine.CurrentRequest = machine.Requests.Dequeue();
                    foreach (string cancelled in machine.CancelList)
                    {
                        if (machine.CurrentRequest.RequesterName.Equals(cancelled))
                        {
                            machine.CancelList.Remove(cancelled);
                            return;
                        }
                    }

                    if (machine.CurrentRequest.RequestType.Equals(RequestType.Portal))
                    {
                        machine.NextCharacter = machine.CurrentRequest.Character;
                        machine.PortalDescription = machine.CurrentRequest.Destination;
                        machine.NextHeading = machine.CurrentRequest.Heading;
                        machine.SpellsToCast.AddRange(machine.CurrentRequest.SpellsToCast);
                    }
                    else if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff))
                    {
                        machine.NextCharacter = machine.CurrentRequest.Character;
                        machine.NextHeading = machine.CurrentRequest.Heading;
                        machine.SpellsToCast.AddRange(machine.CurrentRequest.SpellsToCast);
                    }
                    else if (machine.CurrentRequest.RequestType.Equals(RequestType.Gem))
                    {
                        machine.NextCharacter = machine.CurrentRequest.Character;
                        machine.PortalDescription = machine.CurrentRequest.Destination;
                        machine.NextHeading = machine.CurrentRequest.Heading;
                        machine.ItemToUse = machine.CurrentRequest.ItemToUse;
                    }
                }
                else if (machine.Advertise && machine.Update() && DateTime.Now - machine.LastBroadcast > TimeSpan.FromMinutes(machine.AdInterval))
                {
                    machine.LastBroadcast = DateTime.Now;
                    machine.Inventory.UpdateInventoryFile();
                    if (machine.Utility.BotSettings.Advertisements.Count > 0)
                    {
                        machine.ChatManager.Broadcast(machine.Utility.BotSettings.Advertisements[machine.RandomNumber.Next(0, machine.Utility.BotSettings.Advertisements.Count)].Message);
                    }
                    if (machine.Inventory.LowComponents.Count > 0)
                    {
                        machine.ChatManager.Broadcast(machine.Inventory.LowCompsReport());
                    }
                }
                else
                {
                    // clear the cancel list
                    if (machine.CancelList.Count > 0)
                    {
                        machine.CancelList.Clear();
                    }

                    // set teh current request to a new, blank instance
                    if (!machine.CurrentRequest.RequesterName.Equals(""))
                    {
                        machine.CurrentRequest = new Request();
                    }
                    
                    // if positioning is enabled, reset heading properly - else, set next heading to -1 or disabled
                    if (machine.EnablePositioning)
                    {
                        if (!machine.NextHeading.Equals(machine.DefaultHeading))
                        {
                            machine.NextHeading = machine.DefaultHeading;
                        }
                    }
                    else
                    {
                        if (!machine.NextHeading.Equals(-1))
                        {
                            machine.NextHeading = -1;
                        }
                    }

                    // check for status of buffs on teh buffed character every 30 seconds, if currently logged into the buffing character AND keep buffs alive is enabled
                    if (machine.StayBuffed && machine.Core.CharacterFilter.Name.Equals(machine.BuffingCharacter) && (DateTime.Now - BuffCheck).TotalSeconds > 30)
                    {
                        BuffCheck = DateTime.Now;
                        machine.IsBuffed = HaveAllBuffs(machine);
                        if (!machine.IsBuffed)
                        {
                            machine.NextState = Equipping.GetInstance;
                        }
                    }
                }
            }
            else
            {
                machine.NextState = Stopped.GetInstance;
            }
        }

        public override string ToString()
        {
            return nameof(Idle);
        }

        public bool HaveAllBuffs(Machine machine)
        {
            try
            {
                BuffProfile profile = machine.Level7Self ? machine.Utility.GetProfile("botbuffs7") : machine.Utility.GetProfile("botbuffs");

                List<int> requiredBuffs = new List<int>();
                foreach (Buff buff in profile.Buffs)
                {
                    requiredBuffs.Add(buff.SpellId);
                }

                Dictionary<int, int> enchantments = new Dictionary<int, int>();
                foreach (EnchantmentWrapper enchantment in machine.Core.CharacterFilter.Enchantments)
                {
                    if (requiredBuffs.Contains(enchantment.SpellId) && !enchantments.ContainsKey(enchantment.SpellId))
                    {
                        enchantments.Add(enchantment.SpellId, enchantment.TimeRemaining);
                    }
                }

                foreach (int requiredBuff in requiredBuffs)
                {
                    if (!enchantments.ContainsKey(requiredBuff))
                    {
                        return false;
                    }

                    else if (enchantments[requiredBuff] < 300 && !enchantments[requiredBuff].Equals(-1))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
                return false;
            }
        }
    }
}
