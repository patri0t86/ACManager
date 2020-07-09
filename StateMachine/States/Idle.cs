using ACManager.StateMachine.Queues;
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
                    machine.NextState = Equipping.GetInstance;
                }
                else if (!string.IsNullOrEmpty(machine.ItemToUse) && machine.Core.CharacterFilter.Name.Equals(machine.NextCharacter))
                {
                    machine.NextState = UseItem.GetInstance;
                }
                else if (machine.Requests.Count > 0)
                {
                    machine.CurrentRequest = machine.Requests.Dequeue();
                    if (machine.CurrentRequest.RequestType.Equals(RequestType.Portal))
                    {
                        machine.NextCharacter = machine.CurrentRequest.Character;
                        machine.PortalDescription = machine.CurrentRequest.Destination;
                        machine.NextHeading = machine.CurrentRequest.Heading;
                        machine.SpellsToCast = machine.CurrentRequest.SpellsToCast;
                    }
                    else if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff))
                    {
                        machine.NextCharacter = machine.CurrentRequest.Character;
                        machine.NextHeading = machine.CurrentRequest.Heading;
                        machine.SpellsToCast = machine.CurrentRequest.SpellsToCast;
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
                    machine.CurrentRequest = new Request();
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
