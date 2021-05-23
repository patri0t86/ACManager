using Decal.Adapter;
using Decal.Adapter.Wrappers;
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
            machine.CurrentRequest.ItemToUse = null;
        }

        public void Process(Machine machine)
        {
            if (!machine.Enabled)
            {
                machine.NextState = Idle.GetInstance;
                return;
            }

            if (!(Inventory.GetInventoryCount(machine.CurrentRequest.ItemToUse) > 0))
            {
                ChatManager.Broadcast($"It appears I've run out of {machine.CurrentRequest.ItemToUse}.");
                machine.NextState = Idle.GetInstance;
                return;
            }

            if (DateTime.Now - UseDelay > TimeSpan.FromSeconds(1))
            {
                CoreManager.Current.Actions.UseItem(Inventory.GetItemByName(machine.CurrentRequest.ItemToUse).Id, 0);
                ChatManager.Broadcast($"Portal opened with {machine.CurrentRequest.Destination}. Safe journey, friend.");
                machine.NextState = Idle.GetInstance;
            }
        }

        public override string ToString()
        {
            return nameof(UseItem);
        }
    }
}
