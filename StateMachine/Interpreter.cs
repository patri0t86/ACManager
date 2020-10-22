using Decal.Adapter;
using System;
using System.Text.RegularExpressions;

namespace ACManager.StateMachine
{
    class Interpreter
    {
        private Machine Machine { get; set; }

        public Interpreter(Machine machine)
        {
            Machine = machine;
        }

        /// <summary>
        /// This parses command line arguments sent from the chat box. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Command(object sender, ChatParserInterceptEventArgs e)
        {
            string command = e.Text.ToLower();
            if (command.StartsWith("/acm"))
            {
                Match match = new Regex("(?<option>(help|status|start|stop))$").Match(command);
                if (match.Success)
                {
                    string option = match.Groups["option"].Value;
                    string value = match.Groups["value"].Value;

                    if (option.Equals("start"))
                    {
                        if (!Machine.Enabled)
                        {
                            Machine.Enabled = true;
                            Debug.ToChat("Starting machine...");
                        }
                        else
                        {
                            Debug.ToChat("Machine is already running.");
                        }
                    }
                    else if (option.Equals("stop"))
                    {
                        if (Machine.Enabled)
                        {
                            Machine.Enabled = false;
                            Debug.ToChat("Stopping machine...");
                        }
                        else
                        {
                            Debug.ToChat("Machine is already stopped.");
                        }
                    }
                    else if (option.Equals("help"))
                    {
                        Debug.ToChat("Available commands:");
                        Debug.ToChat("/acm help --- Presents this menu.");
                        Debug.ToChat("/acm ( start | stop ) --- Starts or stops the bot.");
                        Debug.ToChat("/acm status --- Display basic state and statistics of the bot.");
                        Debug.ToChat("For the latest builds, please visit https://github.com/patri0t86/ACManager/releases.");
                    }
                    else if (option.Equals("status"))
                    {
                        TimeSpan duration = DateTime.Now - Machine.MachineStarted;
                        Debug.ToChat($"Machine (v{Machine.Utility.Version}) is currently {(Machine.Enabled ? "running" : "not running")}. It is in the {Machine.CurrentState} state.");
                        if (Machine.Enabled)
                        {
                            Debug.ToChat($"It has been online for {duration.Days} days, {duration.Hours} hours, {duration.Minutes} minutes, and {duration.Seconds} seconds.");
                            Debug.ToChat($"{Machine.PortalsSummonedThisSession} portals have been requested.");
                        }
                    }
                }
                e.Eat = true;
            }
        }
    }
}
