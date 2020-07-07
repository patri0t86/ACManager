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
                else if ((machine.EnablePositioning && (!machine.InPosition() || !machine.CorrectHeading())) || !machine.CorrectHeading())
                {
                    machine.NextState = Positioning.GetInstance;
                }
                else if ((!string.IsNullOrEmpty(machine.ItemToUse) || machine.SpellsToCast.Count > 0) && !machine.Core.CharacterFilter.Name.Equals(machine.NextCharacter))
                {
                    machine.NextState = SwitchingCharacters.GetInstance;
                }
                else if (machine.SpellsToCast.Count > 0 && machine.Core.CharacterFilter.Name.Equals(machine.NextCharacter))
                {
                    // equipping state?
                    using (WorldObjectCollection wands = machine.Core.WorldFilter.GetInventory())
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
                else if (wandEquipped)
                {
                    // equipping state - unequip on exit of state?
                    machine.Core.Actions.MoveItem(wandId, machine.Core.CharacterFilter.Id);
                    wandEquipped = false;
                }
                else if (!string.IsNullOrEmpty(machine.ItemToUse) && machine.Core.CharacterFilter.Name.Equals(machine.NextCharacter))
                {
                     machine.NextState = UseItem.GetInstance;
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
