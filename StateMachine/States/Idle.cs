using ACManager.Settings;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Filters;
using System;
using System.Collections.Generic;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// This is the idle state, where the bot is awaiting commands.
    /// This state is entered and exited many times throughout the lifecycle. Upon entry, ensures it is in a peaceful stance.
    /// </summary>
    public class Idle : StateBase<Idle>, IState
    {
        private DateTime BuffCheck { get; set; }
        public void Enter(Machine machine)
        {
            Inventory.GetComponentLevels();
            Inventory.UpdateInventoryFile();
        }

        public void Exit(Machine machine)
        {

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
        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (CoreManager.Current.Actions.CombatMode != CombatState.Peace)
                {
                    CoreManager.Current.Actions.SetCombatMode(CombatState.Peace);
                }
                else if ((!string.IsNullOrEmpty(machine.CurrentRequest.ItemToUse) || machine.CurrentRequest.SpellsToCast.Count > 0) && !CoreManager.Current.CharacterFilter.Name.Equals(machine.CurrentRequest.Character))
                {
                    machine.NextState = SwitchingCharacters.GetInstance;
                }
                else if ((Utility.BotSettings.BotPositioning && (!machine.InPosition() || !machine.CorrectHeading())) || !machine.CorrectHeading())
                {
                    machine.NextState = Positioning.GetInstance;
                }
                else if (machine.CurrentRequest.SpellsToCast.Count > 0 && CoreManager.Current.CharacterFilter.Name.Equals(machine.CurrentRequest.Character))
                {
                    if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff) && CoreManager.Current.CharacterFilter.Name.Equals(Utility.BotSettings.BuffingCharacter))
                    {
                        machine.IsBuffed = HaveAllBuffs(machine);
                    }
                    else
                    {
                        machine.IsBuffed = true;
                    }
                    machine.NextState = Equipping.GetInstance;
                }
                else if (!string.IsNullOrEmpty(machine.CurrentRequest.ItemToUse) && CoreManager.Current.CharacterFilter.Name.Equals(machine.CurrentRequest.Character))
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
                }
                else if (Utility.BotSettings.AdsEnabled && DateTime.Now - machine.LastBroadcast > TimeSpan.FromMinutes(Utility.BotSettings.AdInterval))
                {
                    machine.LastBroadcast = DateTime.Now;
                    if (Utility.BotSettings.Advertisements.Count > 0)
                    {
                        ChatManager.Broadcast(Utility.BotSettings.Advertisements[new Random().Next(0, Utility.BotSettings.Advertisements.Count)].Message);
                    }

                    ChatManager.Broadcast(Inventory.ReportOnLowComponents());
                }
                else
                {
                    // clear the cancel list
                    if (machine.CancelList.Count > 0)
                    {
                        machine.CancelList.Clear();
                    }

                    // set the current request to a new, blank instance
                    if (!machine.CurrentRequest.RequesterName.Equals(""))
                    {
                        machine.CurrentRequest = new Request();
                    }

                    // if positioning is enabled, reset heading properly - else, set next heading to -1 or disabled
                    if (Utility.BotSettings.BotPositioning)
                    {
                        if (!machine.CurrentRequest.Heading.Equals(Utility.BotSettings.DefaultHeading))
                        {
                            machine.CurrentRequest.Heading = Utility.BotSettings.DefaultHeading;
                        }
                    }

                    // check for status of buffs on teh buffed character every 30 seconds, if currently logged into the buffing character AND keep buffs alive is enabled
                    if (Utility.BotSettings.StayBuffed && CoreManager.Current.CharacterFilter.Name.Equals(Utility.BotSettings.BuffingCharacter) && (DateTime.Now - BuffCheck).TotalSeconds > 30)
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
                BuffProfile profile = Utility.GetProfile("botbuffs");

                List<Spell> requiredBuffs = new List<Spell>();
                foreach (Buff buff in profile.Buffs)
                {
                    Spell spell = CoreManager.Current.Filter<FileService>().SpellTable.GetById(buff.Id);

                    if (Utility.BotSettings.Level7Self && spell.Difficulty > 300)
                    {
                        requiredBuffs.Add(machine.GetFallbackSpell(spell, true));
                    }
                    else
                    {
                        requiredBuffs.Add(spell);
                    }
                }

                Dictionary<int, int> enchantments = new Dictionary<int, int>();
                foreach (EnchantmentWrapper enchantment in CoreManager.Current.CharacterFilter.Enchantments)
                {
                    if (requiredBuffs.Contains(CoreManager.Current.Filter<FileService>().SpellTable.GetById(enchantment.SpellId)) && !enchantments.ContainsKey(enchantment.SpellId))
                    {
                        enchantments.Add(enchantment.SpellId, enchantment.TimeRemaining);
                    }
                }

                foreach (Spell requiredBuff in requiredBuffs)
                {
                    if (!enchantments.ContainsKey(requiredBuff.Id))
                    {
                        return false;
                    }

                    else if (enchantments[requiredBuff.Id] < 300 && !enchantments[requiredBuff.Id].Equals(-1))
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
