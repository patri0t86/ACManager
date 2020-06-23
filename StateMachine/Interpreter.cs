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
                Match match = new Regex("((?<option>(help|status))|(?<option>(bot|ads|respond)) ((?<value>(enable|disable|all|tells))|(?<modifier>(interval|verbosity)) (?<value>[0-9\\.]*)))$").Match(command);
                if (match.Success)
                {
                    string option = match.Groups["option"].Value;
                    string modifier = match.Groups["modifier"].Value;
                    string value = match.Groups["value"].Value;

                    if (option.Equals("bot"))
                    {
                        if (value.Equals("enable"))
                        {
                            if (!Machine.Enabled)
                            {
                                Machine.ChatManager.Broadcast($"/me is running the ACManager Bot {Machine.Utility.Version}. Whisper /help to get started.");
                                Machine.LastBroadcast = DateTime.Now;
                                Machine.Enabled = true;
                                Debug.ToChat("Starting machine...");
                            }
                            else
                            {
                                Debug.ToChat("Machine is already running.");
                            }
                        }
                        else if (value.Equals("disable"))
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
                    }
                    else if (option.Equals("ads"))
                    {
                        if (string.IsNullOrEmpty(modifier))
                        {
                            if (value.Equals("enable"))
                            {
                                if (!Machine.Advertise.Equals(true))
                                {
                                    Machine.Advertise = true;
                                    Debug.ToChat("The machine will now broadcast advertisements.");
                                }
                                else
                                {
                                    Debug.ToChat("Advertisements are already enabled.");
                                }
                            }
                            else if (value.Equals("disable"))
                            {
                                if (!Machine.Advertise.Equals(false))
                                {
                                    Machine.Advertise = false;
                                    Debug.ToChat("The machine will no longer broadcast advertisements.");
                                }
                                else
                                {
                                    Debug.ToChat("Advertisements are already disabled.");
                                }
                            }
                        }
                        else if (modifier.Equals("interval"))
                        {
                            if (double.TryParse(value, out double result))
                            {
                                if (result <= 5)
                                {
                                    Machine.AdInterval = 5;
                                }
                                else
                                {
                                    Machine.AdInterval = result;
                                }
                            }
                            else
                            {
                                Machine.AdInterval = 5;
                            }
                            Debug.ToChat($"New advertising interval is {Machine.AdInterval} minutes.");
                        }
                    }
                    else if (option.Equals("respond"))
                    {
                        if (string.IsNullOrEmpty(modifier))
                        {
                            if (value.Equals("all"))
                            {
                                if (!Machine.RespondToOpenChat)
                                {
                                    Machine.RespondToOpenChat = true;
                                    Debug.ToChat("Now responding to general chat requests.");
                                }
                                else
                                {
                                    Debug.ToChat("Already responding to general chat requests.");
                                }
                            }
                            else if (value.Equals("tells"))
                            {
                                if (Machine.RespondToOpenChat)
                                {
                                    Machine.RespondToOpenChat = false;
                                    Debug.ToChat("No longer responding to general chat requests.");
                                }
                                else
                                {
                                    Debug.ToChat("Already disregarding general chat requests.");
                                }
                            }
                        }
                        else if (modifier.Equals("verbosity"))
                        {
                            if (int.TryParse(value, out int result))
                            {
                                if (result <= 0)
                                {
                                    Machine.Verbosity = 0;
                                }
                                else if (result >= 5)
                                {
                                    Machine.Verbosity = 5;
                                }
                                else
                                {
                                    Machine.Verbosity = result;
                                }
                            }
                            Debug.ToChat($"Verbosity now set to {Machine.Verbosity}.");
                        }
                    }
                    else if (option.Equals("help"))
                    {
                        Debug.ToChat("Available commands:");
                        Debug.ToChat("/acm help --- Presents this menu.");
                        Debug.ToChat("/acm bot ( enable | disable ) --- Starts or stops the bot.");
                        Debug.ToChat("/acm ads ( enable | disable | interval #.# ) --- Enables/Disables advertising. Interval sets the advertising interval in minutes and can be an integer, or decimal value. Minimum of 5 minutes.");
                        Debug.ToChat("/acm respond ( all | tells | verbosity # ) --- Tells the bot to take action based on open chat commands, or tells only. Verbosity is from 0 to 5, adjust this if getting server squelched. Verbosity 0 is one portal per text line.");
                        Debug.ToChat("/acm status --- Shows the current status of the bot with statistics.");
                    }
                    else if (option.Equals("status"))
                    {
                        TimeSpan duration = DateTime.Now - Machine.MachineStarted;
                        Debug.ToChat($"Machine (v{Machine.Utility.Version}) is currently {(Machine.Enabled ? "running" : "not running")}. It is in the {Machine.CurrentState} state.");
                        if (Machine.Enabled)
                        {
                            Debug.ToChat($"It has been online for {duration.Days} days, {duration.Hours} hours, {duration.Minutes} minutes, and {duration.Seconds} seconds.");
                            Debug.ToChat($"Advertisements are currently {(Machine.Advertise ? "enabled" + " on a " + Machine.AdInterval.ToString() + " minute interval" : "disabled")}.");
                            Debug.ToChat($"Verbosity level is set to {Machine.Verbosity}.");
                            Debug.ToChat($"{Machine.PortalsSummonedThisSession} portals have been requested.");
                        }
                    }
                }
                e.Eat = true;
            }
        }
    }
}
