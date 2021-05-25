using ACManager.StateMachine.States;
using ACManager.Views;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Filters;
using System;
using System.Collections.Generic;

namespace ACManager.StateMachine
{
    /// <summary>
    /// State Machine to handle all state transitions, state of all parameters, and clock of the bot.
    /// </summary>
    public class Machine
    {
        /// <summary>
        /// Current IState of the State Machine.
        /// </summary>
        public IState CurrentState { get; set; } = Stopped.GetInstance;

        /// <summary>
        /// The next state the State Machine will transition to.
        /// </summary>
        public IState NextState { get; set; }

        /// <summary>
        /// Determines if the machine is running or not.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Determines whether a character is logged in or not.
        /// </summary>
        public bool LoggedIn { get; set; }

        /// <summary>
        /// Determines whether the character has begun casting a spell.
        /// </summary>
        public bool CastStarted { get; set; } = false;

        /// <summary>
        /// Determines whether the current cast fizzled or not.
        /// </summary>
        public bool Fizzled { get; set; } = false;

        /// <summary>
        /// Determines whether the current cast is complete or not.
        /// </summary>
        public bool CastCompleted { get; set; } = false;

        /// <summary>
        /// An instance of the ChatManager to handle all chat commands/requests.
        /// </summary>
        public ChatManager ChatManager { get; set; }


        /// <summary>
        /// Time the machine was turned on.
        /// </summary>
        public DateTime MachineStarted { get; set; }

        /// <summary>
        /// Number of portals requested this instance of the machine.
        /// </summary>
        public int PortalsSummonedThisSession { get; set; }

        /// <summary>
        /// Class to handle all command line arguments.
        /// </summary>
        public Interpreter Interpreter { get; set; }

        /// <summary>
        /// The bot management GUI in decal.
        /// </summary>
        public BotManagerView BotManagerView { get; set; }

        /// <summary>
        /// Request queue. This is to keep track of all requests as they come in.
        /// </summary>
        public Queue<Request> Requests { get; set; } = new Queue<Request>();

        /// <summary>
        /// The current request being handled.
        /// </summary>
        public Request CurrentRequest { get; set; } = new Request();

        /// <summary>
        /// List to keep track of cancelled requests as the requests are dequeued.
        /// </summary>
        public List<string> CancelList { get; set; } = new List<string>();

        /// <summary>
        /// List of objects in the character's inventory. Includes wands, armor, clothing and jewelry.
        /// </summary>
        public List<WorldObject> CharacterEquipment { get; set; } = new List<WorldObject>();

        /// <summary>
        /// Only send the Finished scanning inventory once.
        /// </summary>
        public bool FinishedScan { get; set; }

        /// <summary>
        /// Create the state machine in the StoppedState and begin processing commands on intervals (every time a frame is rendered).
        /// </summary>
        public Machine()
        {
            Interpreter = new Interpreter(this);
            ChatManager = new ChatManager(this);
        }

        public void Start()
        {
            if (!Enabled)
            {
                Enabled = true;
            }

            if (!BotManagerView.ConfigTab.BotEnabled.Checked)
            {
                BotManagerView.ConfigTab.BotEnabled.Checked = true;
            }
        }

        public void Stop()
        {
            if (Enabled)
            {
                Enabled = false;
            }

            if (BotManagerView.ConfigTab.BotEnabled.Checked)
            {
                BotManagerView.ConfigTab.BotEnabled.Checked = false;
            }
        }

        public void Login()
        {
            if (!LoggedIn)
            {
                LoggedIn = true;
                BotManagerView = new BotManagerView(this);
                Enabled = Utility.BotSettings.BotEnabled;
                IdentifyInventory();
            }
        }

        public void Logout()
        {
            if (LoggedIn)
            {
                LoggedIn = false;
                BotManagerView?.Dispose();
            }
        }

        /// <summary>
        /// Identifies all wearable equipment found in the character's inventory.
        /// </summary>
        public void IdentifyInventory()
        {
            CharacterEquipment.Clear();
            FinishedScan = false;
            using (WorldObjectCollection inventory = CoreManager.Current.WorldFilter.GetInventory())
            {
                foreach (WorldObject item in inventory)
                {
                    if (item.ObjectClass.Equals(ObjectClass.Armor)
                        || item.ObjectClass.Equals(ObjectClass.Jewelry)
                        || item.ObjectClass.Equals(ObjectClass.Clothing)
                        || item.ObjectClass.Equals(ObjectClass.WandStaffOrb))
                    {
                        CharacterEquipment.Add(item);
                        CoreManager.Current.Actions.RequestId(item.Id);
                    }
                }
            }
            Debug.ToChat($"Scanning {CoreManager.Current.IDQueue.ActionCount} equippable inventory items, please wait before using the Equipment manager to build suits.");
        }

        /// <summary>
        /// As items are identified, checks if they are in the character equipment to determine if all wearable items have been identified for the suit builder.
        /// </summary>
        public void WorldFilter_ChangeObject(object sender, ChangeObjectEventArgs e)
        {
            if (FinishedScan)
            {
                return;
            }

            if (e.Change.Equals(WorldChangeType.IdentReceived) && CharacterEquipment.Contains(e.Changed))
            {
                for (int i = 0; i < CharacterEquipment.Count; i++)
                {
                    if (CharacterEquipment[i].Equals(e.Changed))
                    {
                        CharacterEquipment[i] = e.Changed;
                        break;
                    }
                }

                if (CoreManager.Current.IDQueue.ActionCount <= 1)
                {
                    FinishedScan = true;
                    Debug.ToChat($"Finished scanning your inventory. You can now build suits.");
                }

            }
        }

        /// <summary>
        /// This is used for the timing of all events in the state machine. Every time a frame is rendered, 
        /// the state machine processes the current state and determines the next action or state transition. 
        /// The faster the frame rate, the faster this processes.
        /// </summary>
        public void Clock(object sender, EventArgs e)
        {
            if (LoggedIn)
            {
                if (NextState == null)
                {
                    CurrentState.Process(this);
                }
                else
                {
                    CurrentState.Exit(this);
                    CurrentState = NextState;
                    CurrentState.Enter(this);
                    NextState = null;
                }
            }
        }

        /// <summary>
        /// Determines in the bot is in the correct position in the world.
        /// </summary>
        public bool InPosition()
        {
            return CoreManager.Current.Actions.Landcell == Utility.BotSettings.DesiredLandBlock
                && Math.Abs(CoreManager.Current.Actions.LocationX - Utility.BotSettings.DesiredBotLocationX) < 1
                && Math.Abs(CoreManager.Current.Actions.LocationY - Utility.BotSettings.DesiredBotLocationY) < 1;
        }

        /// <summary>
        /// Determines if the bot is facing the correct heading for the current purpose.
        /// </summary>
        public bool CorrectHeading()
        {
            return (CoreManager.Current.Actions.Heading <= CurrentRequest.Heading + 1
                && CoreManager.Current.Actions.Heading >= CurrentRequest.Heading - 1)
                || CurrentRequest.Heading.Equals(-1);
        }

        public bool SpellSkillCheck(Spell spell)
        {
            switch (spell.School.Id)
            {
                case 2:
                    return CoreManager.Current.CharacterFilter.EffectiveSkill[CharFilterSkillType.LifeMagic] + Utility.BotSettings.SkillOverride >= spell.Difficulty + 20;
                case 3:
                    return CoreManager.Current.CharacterFilter.EffectiveSkill[CharFilterSkillType.ItemEnchantment] + Utility.BotSettings.SkillOverride >= spell.Difficulty + 20;
                case 4:
                    return CoreManager.Current.CharacterFilter.EffectiveSkill[CharFilterSkillType.CreatureEnchantment] + Utility.BotSettings.SkillOverride >= spell.Difficulty + 20;
                default:
                    // Void or War
                    return false;
            }
        }

        public Spell GetFallbackSpell(Spell spell, bool IsUntargetted = false)
        {
            Spell fallback = null;
            for (int i = 1; i < CoreManager.Current.Filter<FileService>().SpellTable.Length; i++)
            {
                if (CoreManager.Current.Filter<FileService>().SpellTable[i].Family.Equals(spell.Family) &&
                    CoreManager.Current.Filter<FileService>().SpellTable[i].Difficulty < spell.Difficulty &&
                    CoreManager.Current.Filter<FileService>().SpellTable[i].IsUntargetted.Equals(IsUntargetted) &&
                    !CoreManager.Current.Filter<FileService>().SpellTable[i].IsFellowship &&
                    (CoreManager.Current.Filter<FileService>().SpellTable[i].Duration >= 1800 && CoreManager.Current.Filter<FileService>().SpellTable[i].Duration < spell.Duration || CoreManager.Current.Filter<FileService>().SpellTable[i].Duration == -1))
                {
                    if (fallback == null || CoreManager.Current.Filter<FileService>().SpellTable[i].Difficulty > fallback.Difficulty)
                    {
                        fallback = CoreManager.Current.Filter<FileService>().SpellTable[i];
                    }
                }
            }

            return fallback;
        }

        public void AddToQueue(Request newRequest)
        {
            if (Requests.Contains(newRequest))
            {
                ChatManager.SendTell(newRequest.RequesterName, $"You already have a {newRequest.RequestType} request in.");
            }
            else if (CurrentRequest.Equals(newRequest))
            {
                ChatManager.SendTell(newRequest.RequesterName, $"I'm already helping you with this type of request.");
            }
            else
            {
                Requests.Enqueue(newRequest);

                // Give an estimated wait time
                TimeSpan waitTime;
                int seconds = 0;
                string lastCharacter = CoreManager.Current.CharacterFilter.Name;

                int currentRequest = CurrentRequest.SpellsToCast.Count * 4;

                // add the current request time in
                seconds += currentRequest;

                foreach (Request request in Requests)
                {
                    seconds += request.SpellsToCast.Count * 4;

                    if (!request.Character.Equals(lastCharacter))
                    {
                        lastCharacter = request.Character;
                        seconds += 17;
                    }
                }

                string estimatedWait = "";

                if (Requests.Count > 1)
                {
                    seconds += Requests.Count * 5;
                    waitTime = TimeSpan.FromSeconds(seconds);
                    estimatedWait = $"I should be able to get to your request in about {(waitTime.Minutes > 0 ? $"{waitTime.Minutes} minutes and " : string.Empty)}{waitTime.Seconds} seconds.";
                }
                else if (!string.IsNullOrEmpty(CurrentRequest.RequesterName))
                {
                    seconds = currentRequest + 5;
                    waitTime = TimeSpan.FromSeconds(seconds);
                    estimatedWait = $"I should be able to get to your request in about {(waitTime.Minutes > 0 ? $"{waitTime.Minutes} minutes and " : string.Empty)}{waitTime.Seconds} seconds.";
                }

                if (Requests.Count.Equals(1) && string.IsNullOrEmpty(CurrentRequest.RequesterName))
                {
                    ChatManager.SendTell(newRequest.RequesterName, "I have received your request and will help you now. Say 'cancel' at any time to cancel this request.");
                }
                else if (Requests.Count.Equals(1) && !string.IsNullOrEmpty(CurrentRequest.RequesterName))
                {
                    ChatManager.SendTell(newRequest.RequesterName, $"I have received your request. You are next in line. Say 'cancel' at any time to cancel this request. {(!string.IsNullOrEmpty(estimatedWait) ? estimatedWait : string.Empty)}");
                }
                else
                {
                    ChatManager.SendTell(newRequest.RequesterName, $"I have received your request. There are currently {Requests.Count} requests ahead of you, including the person I'm currently helping. Say 'cancel' at any time to cancel this request. {(!string.IsNullOrEmpty(estimatedWait) ? estimatedWait : string.Empty)}");
                }
            }
        }

        public void CancelRequest(string name)
        {
            if (CurrentRequest.RequesterName.Equals(name))
            {
                CancelList.Add(name);
                ChatManager.SendTell(name, "Your current request is now cancelled. If you had another request in the queue then your spot is maintained.");
            }
            else
            {
                foreach (Request request in Requests)
                {
                    if (request.RequesterName.Equals(name))
                    {
                        CancelList.Add(name);
                        ChatManager.SendTell(name, "I will remove your next request from my queue. If you wish to remove all requests, say 'cancel' for each request you have put in.");
                        return;
                    }
                }
                ChatManager.SendTell(name, "You don't have any requests in at this time.");
            }
        }
    }
}
