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
        private DateTime LastBroadcast;
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
            if (!machine.Enabled)
            {
                machine.NextState = Stopped.GetInstance;
                return;
            }

            if (!CoreManager.Current.Actions.CombatMode.Equals(CombatState.Peace))
            {
                CoreManager.Current.Actions.SetCombatMode(CombatState.Peace);
                return;
            }

            if (!CoreManager.Current.CharacterFilter.Name.Equals(machine.CurrentRequest.Character))
            {
                machine.NextState = SwitchingCharacters.GetInstance;
                return;
            }

            if ((Utility.BotSettings.BotPositioning && (!machine.InPosition() || !machine.CorrectHeading())) || !machine.CorrectHeading())
            {
                machine.NextState = Positioning.GetInstance;
                return;
            }

            if (!machine.CurrentRequest.SpellsToCast.Count.Equals(0))
            {
                machine.NextState = Equipping.GetInstance;
                return;
            }

            if (!string.IsNullOrEmpty(machine.CurrentRequest.ItemToUse))
            {
                machine.NextState = UseItem.GetInstance;
                return;
            }

            if (machine.CurrentRequest.RequestType.Equals(RequestType.Tinker) && !machine.CurrentRequest.IsFinished) {
                machine.NextState = Trading.GetInstance;
                return;
            }

            if (machine.Requests.Count > 0)
            {
                while(machine.Requests.Peek().IsCancelled)
                {
                    machine.Requests.Dequeue();
                }

                machine.CurrentRequest = machine.Requests.Dequeue();
                return; 
            }

            if (Utility.BotSettings.AdsEnabled && DateTime.Now - LastBroadcast > TimeSpan.FromMinutes(Utility.BotSettings.AdInterval))
            {
                LastBroadcast = DateTime.Now;

                if (!Utility.BotSettings.Advertisements.Count.Equals(0))
                {
                    ChatManager.Broadcast(Utility.BotSettings.Advertisements[new Random().Next(0, Utility.BotSettings.Advertisements.Count)].Message);
                } 
                else
                {
                    ChatManager.Broadcast($"/me is running ACManager v{Utility.Version} found at https://github.com/patri0t86/ACManager. Whisper 'help' to get started.");
                }

                ChatManager.Broadcast(Inventory.ReportOnLowComponents());
            }

            // set the current request to a new, blank instance
            if (!string.IsNullOrEmpty(machine.CurrentRequest.RequesterName))
            {
                machine.CurrentRequest = new Request();
            }

            // if positioning is enabled, set heading properly - else, set next heading to -1 (disabled)
            if (Utility.BotSettings.BotPositioning)
            {
                if (!machine.CurrentRequest.Heading.Equals(Utility.BotSettings.DefaultHeading))
                {
                    machine.CurrentRequest.Heading = Utility.BotSettings.DefaultHeading;
                }
            }
            else if (!machine.CurrentRequest.Heading.Equals(-1))
            {
                machine.CurrentRequest.Heading = -1;
            }

            // check for status of buffs on the buffing character every 30 seconds, if currently logged into the buffing character AND keep buffs alive is enabled
            if (Utility.BotSettings.StayBuffed && CoreManager.Current.CharacterFilter.Name.Equals(Utility.BotSettings.BuffingCharacter) && (DateTime.Now - BuffCheck).TotalSeconds > 30)
            {
                BuffCheck = DateTime.Now;
                if (!Casting.HaveSelfBuffs(machine))
                {
                    Request request = new Request
                    { 
                        RequestType = RequestType.SelfBuff
                    };
                    request.SpellsToCast.AddRange(Casting.GetSelfBuffs(machine));
                    machine.Requests.Enqueue(request);
                }
            }
        }

        public override string ToString()
        {
            return nameof(Idle);
        }
    }
}
