using ACManager.Settings;
using Decal.Adapter.Wrappers;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// --- DO NOT USE THIS ---
    /// This is a template to copy/paste into a new state for ease of implementation/development.
    /// </summary>
    class UseItem : StateBase<UseItem>, IState
    {
        public void Enter(Machine machine)
        {

        }

        public void Exit(Machine machine)
        {
            
        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                using (WorldObjectCollection inventory = machine.Core.WorldFilter.GetInventory())
                {
                    inventory.SetFilter(new ByNameFilter(machine.ItemToUse));
                    if (inventory.Quantity > 0)
                    {
                        machine.Core.Actions.UseItem(inventory.First.Id, 0);
                        machine.ChatManager.Broadcast($"Portal now open to {machine.PortalDescription}. Safe journey, friend.");
                        machine.ItemToUse = null;
                    }
                    else
                    {
                        machine.ChatManager.Broadcast($"It appears I've run out of {machine.ItemToUse}.");
                        machine.ItemToUse = null;
                    }
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
            return nameof(UseItem);
        }
    }
}
