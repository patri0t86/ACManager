using Decal.Adapter.Wrappers;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// A state designed to dress and undress the character as appropriate.
    /// </summary>
    class Equipping : StateBase<Equipping>, IState
    {
        private bool WandEquipped { get; set; }
        private int WandId { get; set; }

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
                // make it do this eventually:
                // IF THE PLAYER IS THE BUFFING CHARACTER:
                //      if spellstocast > 0, determine suit to wear, wear it, enter casting state
                //      if spellstocast == 0, done casting, put on idle suit, return to idle state
                // ELSE:
                //      if spellstocast > 0, find a wand and equip it, enter casting state
                //      if spellstocast == 0, done casting, take wand off, return to idle state

                if (machine.SpellsToCast.Count > 0)
                {
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
                                    WandEquipped = true;
                                    WandId = wand.Id;
                                    break;
                                }

                                if (!WandEquipped)
                                {
                                    machine.Core.Actions.AutoWield(wand.Id);
                                    WandEquipped = true;
                                    WandId = wand.Id;
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
                else if (WandEquipped)
                {
                    machine.Core.Actions.MoveItem(WandId, machine.Core.CharacterFilter.Id);
                    WandEquipped = false;
                }
                else
                {
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
            return nameof(Equipping);
        }
    }
}
