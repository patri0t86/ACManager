using ACManager.StateMachine.Queues;
using ACManager.StateMachine.States;
using ACManager.Views;
using Decal.Adapter;
using System;
using System.Collections.Generic;

namespace ACManager.StateMachine
{
    /// <summary>
    /// State Machine to handle all state transitions, state of all parameters, and clock of the bot.
    /// </summary>
    internal class Machine
    {
        /// <summary>
        /// Current IState of the State Machine.
        /// </summary>
        public IState CurrentState;

        /// <summary>
        /// The next state the State Machine will transition to.
        /// </summary>
        public IState NextState;

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
        public bool CastStarted { get; set; }

        /// <summary>
        /// Determines whether the current cast fizzled or not.
        /// </summary>
        public bool Fizzled { get; set; } = false;

        /// <summary>
        /// Determines whether the current cast is complete or not.
        /// </summary>
        public bool CastCompleted { get; set; } = false;

        /// <summary>
        /// List of spells to cast.
        /// </summary>
        public List<int> SpellsToCast { get; set; } = new List<int>();

        /// <summary>
        /// An instance of the ChatManager to handle all chat commands/requests.
        /// </summary>
        public ChatManager ChatManager;

        /// <summary>
        /// List of characters for the logged in account.
        /// </summary>
        public List<string> AccountCharacters { get; set; } = new List<string>();

        /// <summary>
        /// The list index of the next character to log in.
        /// </summary>
        public int NextCharacterIndex { get; set; }

        /// <summary>
        /// The next character to log in to summon the portal.
        /// </summary>
        public string NextCharacter { get; set; }

        /// <summary>
        /// The destination of the requested portal.
        /// </summary>
        public string PortalDescription { get; set; }

        /// <summary>
        /// Name of character requesting the portal.
        /// </summary>
        public string CharacterMakingRequest { get; set; }

        /// <summary>
        /// Time reference to control when advertisements are broadcast.
        /// </summary>
        public DateTime LastBroadcast { get; set; }

        /// <summary>
        /// Used to selectively decline new commands as necessary.
        /// </summary>
        public bool DecliningCommands { get; set; }

        /// <summary>
        /// Time the machine was turned on.
        /// </summary>
        public DateTime MachineStarted { get; set; }

        /// <summary>
        /// Number of portals requested this instance of the machine.
        /// </summary>
        public int PortalsSummonedThisSession { get; set; }

        /// <summary>
        /// Random number to select items with.
        /// </summary>
        public Random RandomNumber { get; set; }

        /// <summary>
        /// Class to handle all file I/O for the machine.
        /// </summary>
        public Utility Utility { get; set; }

        /// <summary>
        /// Class to handle all command line arguments.
        /// </summary>
        public Interpreter Interpreter { get; set; }

        /// <summary>
        /// Toggle this bots advertising on and off.
        /// </summary>
        public bool Advertise { get; set; } = true;

        /// <summary>
        /// Interval, in minutes, to send advertisements.
        /// </summary>
        public double AdInterval { get; set; } = 10;

        /// <summary>
        /// Threshhold to begin regaining mana.
        /// </summary>
        public double ManaThreshold { get; set; } = 0.5;

        /// <summary>
        /// Threshhold to begin regaining stamina.
        /// </summary>
        public double StaminaThreshold { get; set; } = 0.5;

        /// <summary>
        /// The heading of the next portal to summon.
        /// </summary>
        public double NextHeading { get; set; } = -1;

        /// <summary>
        /// Setting to determine if the machine will listen to portal requests from open chat.
        /// </summary>
        public bool RespondToOpenChat { get; set; } = true;

        /// <summary>
        /// Determines the output style of the 
        /// </summary>
        public int Verbosity { get; set; } = 0;

        /// <summary>
        /// CoreManager to perform actions in game.
        /// </summary>
        public CoreManager Core { get; set; }

        /// <summary>
        /// Heading to restore to when the bot is idle.
        /// </summary>
        public double DefaultHeading { get; set; }

        /// <summary>
        /// Navigation landblock to maintain.
        /// </summary>
        public int DesiredLandBlock { get; set; }

        /// <summary>
        /// X coordinate in the landblock to maintain.
        /// </summary>
        public double DesiredBotLocationX { get; set; }

        /// <summary>
        /// Y coordinate in the landblock to maintain.
        /// </summary>
        public double DesiredBotLocationY { get; set; }

        /// <summary>
        /// Enable/disable navigation.
        /// </summary>
        public bool EnablePositioning { get; set; } = false;

        /// <summary>
        /// Class to check for component status on spellcast.
        /// </summary>
        public ComponentChecker ComponentChecker { get; set; }

        /// <summary>
        /// Holds the currently logged in character's relevant inventory.
        /// </summary>
        public Inventory Inventory { get; set; }

        /// <summary>
        /// Name of the item to use next.
        /// </summary>
        public string ItemToUse { get; set; }

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
        /// Character name on the account that is capable of being a buff bot.
        /// </summary>
        public string BuffingCharacter { get; set; }

        /// <summary>
        /// Determines state of the bot being buffed for buffing.
        /// </summary>
        public bool IsBuffed { get; set; }

        /// <summary>
        /// Determines if the bot should let buffs run out or not.
        /// </summary>
        public bool StayBuffed { get; set; }

        /// <summary>
        /// Create the state machine in the StoppedState and begin processing commands on intervals (every time a frame is rendered).
        /// </summary>
        public Machine(CoreManager core, string path)
        {
            Core = core;
            Utility = new Utility(this, path);
            Interpreter = new Interpreter(this);
            ChatManager = new ChatManager(this);
            ComponentChecker = new ComponentChecker(Core);
            Inventory = new Inventory(this);
            RandomNumber = new Random();
            CurrentState = Stopped.GetInstance;
            Core.RenderFrame += Clock;
        }

        /// <summary>
        /// This is used for the timing of all events in the state machine. Every time a frame is rendered, 
        /// the state machine processes the current state and determines the next action or state transition. 
        /// The faster the frame rate, the faster this processes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clock(object sender, EventArgs e)
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
        /// Gets the updated list of portals from settings file.
        /// </summary>
        /// <returns>True if the file was parsed successfully and not null.</returns>
        public bool Update()
        {
            Utility.CharacterSettings = Utility.LoadCharacterSettings();
            return Utility.CharacterSettings != null && Utility.LoadBuffProfiles();
        }

        /// <summary>
        /// Determines the next character to log in.
        /// </summary>
        public void GetNextCharacter()
        {
            for (int i = 0; i < AccountCharacters.Count; i++)
            {
                if (AccountCharacters[i].Equals(NextCharacter))
                {
                    NextCharacterIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// Determines in the bot is in the correct position in the world.
        /// </summary>
        /// <returns></returns>
        public bool InPosition()
        {
            return Core.Actions.Landcell == DesiredLandBlock
                && Math.Abs(Core.Actions.LocationX - DesiredBotLocationX) < 1
                && Math.Abs(Core.Actions.LocationY - DesiredBotLocationY) < 1;
        }

        /// <summary>
        /// Determines if the bot is facing the correct heading for the current purpose.
        /// </summary>
        /// <returns></returns>
        public bool CorrectHeading()
        {
            return (Core.Actions.Heading <= NextHeading + 1
                && Core.Actions.Heading >= NextHeading - 1)
                || NextHeading.Equals(-1);
        }
    }
}