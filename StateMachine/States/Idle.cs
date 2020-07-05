using Decal.Adapter.Wrappers;
using System;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// This is the idle state, where the bot is awaiting commands.
    /// This state is entered and exited many times throughout the lifecycle. Upon entry, ensures it is in a peaceful stance.
    /// </summary>
    internal class Idle : StateBase<Idle>, IState
    {
        private bool wandEquipped = false;
        private int wandId;

        public void Enter(Machine machine)
        {
            machine.DecliningCommands = false;
            machine.Inventory.GetComponentLevels();
            machine.Inventory.UpdateInventoryFile();
        }

        public void Exit(Machine machine)
        {
            machine.DecliningCommands = true;
            machine.Inventory.GetComponentLevels();
        }

        /// <summary>
        /// Order of operations:
        /// Go to peace mode
        /// Move the bot to the correct location in the world
        /// Switch characters if portal is not on this character
        /// Cast summon portal from this character / turn character to the correct heading / equip wand
        /// If a wand is equipped, unequip it
        /// Use gem if required
        /// Turn character to the default heading if truly idle
        /// Broadcast advertisement / low on spell comps
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
                else if (!(machine.Core.Actions.Landcell == machine.DesiredLandBlock && Math.Abs(machine.Core.Actions.LocationX - machine.DesiredBotLocationX) < 1 && Math.Abs(machine.Core.Actions.LocationY - machine.DesiredBotLocationY) < 1) && machine.EnablePositioning) // If bot is not in the correct spot, get there
                {
                    if (machine.DesiredLandBlock.Equals(0) && machine.DesiredBotLocationX.Equals(0) && machine.DesiredBotLocationY.Equals(0)) // no location settings, use current location
                    {
                        machine.DesiredLandBlock = machine.Core.Actions.Landcell;
                        machine.DesiredBotLocationX = machine.Core.Actions.LocationX;
                        machine.DesiredBotLocationY = machine.Core.Actions.LocationY;
                        Debug.ToChat("Bot location set to current location since one was not set previously.");
                    }
                    else // location settings were set - reposition the bot
                    {
                        machine.NextState = Positioning.GetInstance;
                    }
                }
                else if ((!string.IsNullOrEmpty(machine.ItemToUse) || machine.SpellsToCast.Count > 0) && !machine.Core.CharacterFilter.Name.Equals(machine.NextCharacter)) // If there is a portal in the queue and it is not on this character, switch to the correct character
                {
                    machine.NextState = SwitchingCharacters.GetInstance;
                }
                else if (machine.SpellsToCast.Count > 0 && machine.Core.CharacterFilter.Name.Equals(machine.NextCharacter)) // If there is a portal in the queue and it is on this character, enter casting state
                {
                    if ((machine.Core.Actions.Heading <= machine.NextHeading + 2 && machine.Core.Actions.Heading >= machine.NextHeading - 2) || machine.NextHeading.Equals(-1))
                    {
                        // todo make this an actual state to get dressed
                        using (WorldObjectCollection wands = machine.Core.WorldFilter.GetInventory()) // get any wand in the inventory and equip it
                        {
                            wands.SetFilter(new ByObjectClassFilter(ObjectClass.WandStaffOrb));

                            if (wands.Quantity > 0)
                            {
                                foreach (WorldObject wand in wands)
                                {
                                    if (wand.Values(LongValueKey.EquippedSlots) > 0)
                                    {
                                        machine.NextState = Casting.GetInstance;
                                        wandEquipped = true;
                                        wandId = wand.Id;
                                        break;
                                    }

                                    if (!wandEquipped)
                                    {
                                        machine.Core.Actions.AutoWield(wand.Id);
                                        wandEquipped = true;
                                        wandId = wand.Id;
                                    }
                                }
                            }
                            else
                            {
                                machine.ChatManager.Broadcast("Oops, my owner didn't give me a wand. Cancelling this request.");
                                machine.SpellsToCast.Clear();
                            }
                        }
                    }
                    else
                    {
                        machine.Core.Actions.Heading = machine.NextHeading;
                    }
                }
                else if (wandEquipped)
                {
                    machine.Core.Actions.MoveItem(wandId, machine.Core.CharacterFilter.Id);
                    wandEquipped = false;
                }
                else if (!string.IsNullOrEmpty(machine.ItemToUse) && machine.Core.CharacterFilter.Name.Equals(machine.NextCharacter))
                {
                    if (machine.Inventory.GetInventoryCount(machine.ItemToUse) > 0)
                    {
                        if (machine.Core.Actions.Heading <= machine.NextHeading + 1 && machine.Core.Actions.Heading >= machine.NextHeading - 1)
                        {
                            machine.NextState = UseItem.GetInstance;
                        }
                        else
                        {
                            machine.Core.Actions.Heading = machine.NextHeading;
                        }
                    } 
                    else
                    {
                        machine.ChatManager.Broadcast($"It appears I've run out of {machine.ItemToUse}.");
                        machine.ItemToUse = null;
                    }
                }
                else if (!(machine.Core.Actions.Heading <= machine.DefaultHeading + 2 && machine.Core.Actions.Heading >= machine.DefaultHeading - 2)) // if totally idle, reset heading
                {
                    machine.Core.Actions.Heading = machine.DefaultHeading;
                }
                else if (machine.Advertise && machine.Update() && DateTime.Now - machine.LastBroadcast > TimeSpan.FromMinutes(machine.AdInterval)) // Advertisement/spam timing control
                {
                    machine.LastBroadcast = DateTime.Now;
                    machine.Inventory.UpdateInventoryFile();
                    if (machine.Utility.BotSettings.Advertisements.Count > 0)
                    {
                        machine.ChatManager.Broadcast(machine.Utility.BotSettings.Advertisements[machine.RandomNumber.Next(0, machine.Utility.BotSettings.Advertisements.Count)].Message);
                    }
                    if (machine.Inventory.IsLowOnComponents)
                    {
                        machine.ChatManager.Broadcast($"I'm low on spell components, please '/t {machine.Core.CharacterFilter.Name}, comps' to see what I'm low on.");
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
    }
}
