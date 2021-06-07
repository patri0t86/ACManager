using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// Tinkering
    /// </summary>
    class Trading : StateBase<Trading>, IState
    {
        private int Tradee;
        private DateTime TimeOfLastAction;
        private int AddedItem;
        private WorldObject CreatedItem;
        private readonly List<WorldObject> ItemsToTinker = new List<WorldObject>();
        private bool EndSession;
        private bool TradeAccepted;
        private Rectangle WindowRect = new Rectangle();

        public void Enter(Machine machine)
        {
            CoreManager.Current.WorldFilter.EnterTrade += WorldFilter_EnterTrade;
            CoreManager.Current.WorldFilter.AddTradeItem += WorldFilter_AddTradeItem;
            CoreManager.Current.WorldFilter.AcceptTrade += WorldFilter_AcceptTrade;
            CoreManager.Current.WorldFilter.EndTrade += WorldFilter_EndTrade;
            CoreManager.Current.WorldFilter.ResetTrade += WorldFilter_ResetTrade;
            CoreManager.Current.WorldFilter.CreateObject += WorldFilter_CreateObject;
            CoreManager.Current.Actions.TradeEnd();
            ChatManager.SendTell(machine.CurrentRequest.RequesterName, 
                "Please open a trade with me, place your item to be tinkered along with the salvage to use. " +
                "When all items are in the window, accept the trade.");
            TimeOfLastAction = DateTime.Now;
            Simulate.GetWindowRect(new HandleRef(this, CoreManager.Current.Decal.Hwnd), out Simulate.RECT rct);
            WindowRect.Width = rct.Right - rct.Left - 6;
            WindowRect.Height = rct.Bottom - rct.Top - 29;
        }

        public void Exit(Machine machine)
        {
            CoreManager.Current.WorldFilter.EnterTrade -= WorldFilter_EnterTrade;
            CoreManager.Current.WorldFilter.AddTradeItem -= WorldFilter_AddTradeItem;
            CoreManager.Current.WorldFilter.AcceptTrade -= WorldFilter_AcceptTrade;
            CoreManager.Current.WorldFilter.EndTrade -= WorldFilter_EndTrade;
            CoreManager.Current.WorldFilter.ResetTrade -= WorldFilter_ResetTrade;
            CoreManager.Current.WorldFilter.CreateObject -= WorldFilter_CreateObject; 
            CoreManager.Current.Actions.TradeEnd();
            ItemsToTinker.Clear();
        }

        public void Process(Machine machine)
        {
            if (!machine.Enabled || machine.CurrentRequest.IsCancelled || machine.CurrentRequest.IsFinished)
            {
                machine.CurrentRequest.IsFinished = true;
                machine.NextState = Idle.GetInstance;
                return;
            }

            if ((DateTime.Now - TimeOfLastAction).TotalSeconds > 60 && !machine.Requests.Count.Equals(0))
            {
                machine.CurrentRequest.IsFinished = true;
                ChatManager.SendTell(machine.CurrentRequest.RequesterName, "I'm sorry, too much time has passed since you performed an action. I'm cancelling this request.");
                return;
            }

            if (!Tradee.Equals(machine.CurrentRequest.RequesterGuid))
            {
                CoreManager.Current.Actions.TradeEnd();
                Tradee = machine.CurrentRequest.RequesterGuid;
                return;
            }

            if (EndSession)
            {
                EndSession = false;
                CoreManager.Current.Actions.TradeEnd();
                machine.CurrentRequest.IsFinished = true;
                ChatManager.SendTell(machine.CurrentRequest.RequesterName, "You have ended the trade, so I'm ending this request. If this was in error, make another request.");
                return;
            }

            if (TradeAccepted)
            {
                TradeAccepted = false;
                CoreManager.Current.Actions.TradeAccept();
                // save items to disk
                // go to tinkering state
            }

            // go into tinkering - should trading be a separate State from tinkering?

            // below here handles adding items to the trade and verifying compatability

            if (CreatedItem == null || AddedItem.Equals(0))
            {
                return;
            }

            if (CreatedItem.Id.Equals(AddedItem) && !Salvage(CreatedItem) && !TinkerTarget(CreatedItem))
            {
                ChatManager.SendTell(machine.CurrentRequest.RequesterName, $"{CreatedItem.Name} is not a tinkering related item.");
                CoreManager.Current.Actions.TradeReset();
                ItemsToTinker.Clear();
                CreatedItem = null;
                AddedItem = 0;
                return;
            }

            if (ItemsToTinker.Count.Equals(0))
            {
                Debug.ToChat($"Added {CreatedItem.Name}");
                ItemsToTinker.Add(CreatedItem);
            }
            else
            {
                if (Salvage(ItemsToTinker[0]) 
                    && Salvage(CreatedItem) 
                    && ItemsToTinker[0].Values(LongValueKey.UsageMask).Equals(CreatedItem.Values(LongValueKey.UsageMask)))
                {
                    ItemsToTinker.Add(CreatedItem);
                } 
                else if (Salvage(ItemsToTinker[0]) 
                    && TinkerTarget(CreatedItem)
                    && ((ItemCategory)ItemsToTinker[0].Values(LongValueKey.UsageMask) & (ItemCategory)CreatedItem.Category) != ItemCategory.None)
                {
                    ItemsToTinker.Add(CreatedItem);
                }
                else if (TinkerTarget(ItemsToTinker[0]) 
                    && Salvage(CreatedItem)
                    && ((ItemCategory)ItemsToTinker[0].Category & (ItemCategory)CreatedItem.Values(LongValueKey.UsageMask)) != ItemCategory.None)
                {
                    ItemsToTinker.Add(CreatedItem);
                }
                else
                {
                    ItemsToTinker.Clear();
                    CoreManager.Current.Actions.TradeReset();
                    ChatManager.SendTell(machine.CurrentRequest.RequesterName, "Please place only one tinker target (weapon/wand/armor/etc) in the trade window at a time. " +
                        "Also ensure you are placing only compatible salvage into the trade window.");
                }
            }

            CreatedItem = null;
            AddedItem = 0;
        }

        private void WorldFilter_EnterTrade(object sender, EnterTradeEventArgs e)
        {
            Tradee = e.TradeeId;
        }

        private void WorldFilter_AddTradeItem(object sender, AddTradeItemEventArgs e)
        {
            Debug.ToChat($"From WorldFilter.AddTradeItem: {e.ItemId} {e.SideId}");
            AddedItem = e.ItemId;
            TimeOfLastAction = DateTime.Now;
        }

        private void WorldFilter_AcceptTrade(object sender, AcceptTradeEventArgs e)
        {
            Debug.ToChat($"From WorldFilter.AcceptTrade: {e.TargetId}");
            TradeAccepted = true;
            TimeOfLastAction = DateTime.Now;
        }

        private void WorldFilter_EndTrade(object sender, EndTradeEventArgs e)
        {
            Debug.ToChat($"From WorldFilter.EndTrade: {e.ReasonId}");
            EndSession = true;
        }

        private void WorldFilter_ResetTrade(object sender, ResetTradeEventArgs e)
        {
            Debug.ToChat($"From WorldFilter.ResetTrade: {e.TraderId}");
        }

        private void WorldFilter_CreateObject(object sender, CreateObjectEventArgs e)
        {
            Debug.ToChat($"From WorldFilter.CreateObject: {e.New.Name} ({e.New.Id})");
            Debug.ToChat($"Category = {e.New.Category}");
            Debug.ToChat($"WS = {e.New.Values(DoubleValueKey.SalvageWorkmanship)}");
            Debug.ToChat($"Usable on = {e.New.Values(LongValueKey.UsageMask)}");
            CreatedItem = e.New;
        }

        private void ClickYes()
        {
            Simulate.MouseClick(CoreManager.Current.Decal.Hwnd, WindowRect.Width / 2 - 80, WindowRect.Height / 2 + 25);
        }

        private void ClickNo()
        {
            Simulate.MouseClick(CoreManager.Current.Decal.Hwnd, WindowRect.Width / 2 + 80, WindowRect.Height / 2 + 25);
        }

        private bool Salvage(WorldObject wo)
        {
            return ((ItemCategory)wo.Category & ItemCategory.Salvage) != ItemCategory.None;
        }

        private bool TinkerTarget(WorldObject wo)
        {
            ItemCategory item = (ItemCategory)wo.Category;
            return (item & ItemCategory.Weapon) != ItemCategory.None
                || (item & ItemCategory.Armor) != ItemCategory.None
                || (item & ItemCategory.Clothing) != ItemCategory.None
                || (item & ItemCategory.Missile) != ItemCategory.None
                || (item & ItemCategory.Wand) != ItemCategory.None;
        }

        [Flags]
        private enum ItemCategory
        {
            None = 0x00000000,
            Weapon = 0x00000001,
            Armor = 0x00000002,
            Clothing = 0x00000004,
            Missile = 0x00000100,
            Wand = 0x00008000,
            Salvage = 0x40000000
        }

        public override string ToString()
        {
            return nameof(Trading);
        }
    }
}
