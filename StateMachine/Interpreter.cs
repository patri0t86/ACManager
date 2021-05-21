using Decal.Adapter;
using System;
using System.Text.RegularExpressions;

namespace ACManager.StateMachine
{
    public class Interpreter
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
                    switch (match.Groups["option"].Value)
                    {
                        case "start":
                            Machine.Start();
                            break;
                        case "stop":
                            Machine.Stop();
                            break;
                        case "help":
                            Debug.ToChat("Available commands:");
                            Debug.ToChat("/acm help --- Presents this menu.");
                            Debug.ToChat("/acm ( start | stop ) --- Starts or stops the bot.");
                            Debug.ToChat("/acm status --- Display basic state and statistics of the bot.");
                            Debug.ToChat("For the latest builds, please visit https://github.com/patri0t86/ACManager/releases.");
                            break;
                        case "status":
                            TimeSpan duration = DateTime.Now - Machine.MachineStarted;
                            Debug.ToChat($"Machine (v{Utility.Version}) is currently {(Machine.Enabled ? "running" : "not running")}. It is in the {Machine.CurrentState} state.");
                            if (Machine.Enabled)
                            {
                                Debug.ToChat($"It has been online for {duration.Days} days, {duration.Hours} hours, {duration.Minutes} minutes, and {duration.Seconds} seconds.");
                                Debug.ToChat($"{Machine.PortalsSummonedThisSession} portals have been requested.");
                            }
                            break;
                    }
                }
                e.Eat = true;
            }
        }
    }
}
