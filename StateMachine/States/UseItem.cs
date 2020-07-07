using System;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// This class is designed to use items.
    /// </summary>
    class UseItem : StateBase<UseItem>, IState
    {
        private DateTime UseDelay;

        public void Enter(Machine machine)
        {
            UseDelay = DateTime.Now;
        }

        public void Exit(Machine machine)
        {
            machine.ItemToUse = null;
        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (machine.Inventory.GetInventoryCount(machine.ItemToUse) > 0)
                {
                    if (DateTime.Now - UseDelay > TimeSpan.FromSeconds(1)) // added a delay on use due to timing of rotation and using the item
                    {
                        if (machine.Inventory.UseItem(machine.ItemToUse))
                        {
                            machine.ChatManager.Broadcast($"Portal opened with {machine.PortalDescription}. Safe journey, friend.");
                            machine.NextState = Idle.GetInstance;
                        }                        
                    }
                }
                else
                {
                    machine.ChatManager.Broadcast($"It appears I've run out of {machine.ItemToUse}.");
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
