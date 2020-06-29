﻿using ACManager.Settings;
using Decal.Adapter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ACManager.StateMachine
{
    /// <summary>
    /// Class to control all chat parsing while the bot is in operation. This handles tells, general chat, sending tells, and broadcasting advertisements.
    /// </summary>
    internal class ChatManager
    {
        /// <summary>
        /// A reference to the state machine instance.
        /// </summary>
        private Machine Machine { get; set; }

        /// <summary>
        /// Instantiates the ChatCommandManager and sets the internal state machine instance to this.
        /// </summary>
        /// <param name="machine"></param>
        public ChatManager(Machine machine)
        {
            Machine = machine;
        }

        /// <summary>
        /// The parser of all messages. It is subscribed to only while the bot is in operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Current_ChatBoxMessage(object sender, ChatTextInterceptEventArgs e)
        {
            string text = Regex.Replace(e.Text.ToLower(), @"[^\w:/ ']", string.Empty);

            // Create a new regex match for regular chat
            Match match = new Regex("(?<guid>\\d+):(?<dupleName>.+?)tell says (?<message>.*$)").Match(text);
            if (match.Success)
            {
                HandleChat(match);
            }

            // Create a new regex match for general tells
            match = new Regex("(?<guid>\\d+):(?<dupleName>.+?)tell tells you (?<message>.*$)").Match(text);
            if (match.Success)
            {
                HandleTell(match);
            }
        }

        /// <summary>
        /// Handles all messages sent through general chat.
        /// </summary>
        /// <param name="match"></param>
        private void HandleChat(Match match)
        {
            // The actual message
            string message = match.Groups["message"].Value;

            // The GUID of the player sending the tell
            int guid = Convert.ToInt32(match.Groups["guid"].Value);

            if (!guid.Equals(0)) // guid = 0 is said in general/trade/etc. not in local chat
            {
                Machine.CharacterMakingRequest = match.Groups["dupleName"].Value.Substring(0, match.Groups["dupleName"].Value.Length / 2);

                if ((message.Equals("whereto") || message.Equals("where to")) && Machine.Update())
                {
                    RespondWithPortals();
                }

                if (Machine.RespondToOpenChat && !Machine.DecliningCommands)
                {
                    CheckCommands(message);
                }
            }
        }

        /// <summary>
        /// Handles all direct tells.
        /// </summary>
        /// <param name="match"></param>
        private void HandleTell(Match match)
        {
            // The actual message
            string message = match.Groups["message"].Value;

            // The GUID of the player sending the tell
            int guid = Convert.ToInt32(match.Groups["guid"].Value);

            // The in-game character name sending the tell
            Machine.CharacterMakingRequest = match.Groups["dupleName"].Value.Substring(0, match.Groups["dupleName"].Value.Length / 2);

            if (!string.IsNullOrEmpty(message) && Machine.Update())
            {
                //bool commandExists = false;
                if (message.Equals("whereto") || message.Equals("where to"))
                {
                    RespondWithPortals();
                }
                else if (message.Equals("help"))
                {
                    SendTell(Machine.CharacterMakingRequest, "You can /t me 'whereto' to get a list of portals. Then /t me any keyword for me to summon the portal you request.");
                }
                else
                {
                    if (!Machine.DecliningCommands)
                    {
                        CheckCommands(message);
                    }
                    else
                    {
                        SendTell(Machine.CharacterMakingRequest, $"I'm finishing up a request for someone else. Once I'm done, please make your request again.");
                    }
                }
            }
        }

        /// <summary>
        /// Responds with all available portals.
        /// </summary>
        private void RespondWithPortals()
        {
            List<string> portalStrings = GetPortals();
            if (Machine.Verbosity > 0)
            {
                if (portalStrings.Count > 0)
                {
                    SendTell(Machine.CharacterMakingRequest, "You can /t me any keyword, and I will summon the corresponding portal.");
                    StringBuilder sb = new StringBuilder();
                    int count = 0;
                    for (int i = 0; i < portalStrings.Count; i++)
                    {
                        sb.Append(portalStrings[i]);
                        count++;
                        if (count.Equals(Machine.Verbosity + 1))
                        {
                            SendTell(Machine.CharacterMakingRequest, sb.ToString());
                            sb.Length = 0;
                            sb.Capacity = 0;
                            sb.Capacity = 16;
                            count = 0;
                        }
                        else if (count < Machine.Verbosity + 1 && !i.Equals(portalStrings.Count - 1))
                        {
                            sb.Append(", ");
                        }

                        if (i.Equals(portalStrings.Count - 1) && !string.IsNullOrEmpty(sb.ToString()))
                        {
                            SendTell(Machine.CharacterMakingRequest, sb.ToString());
                            sb.Length = 0;
                            sb.Capacity = 0;
                        }
                    }
                }
                else
                {
                    SendTell(Machine.CharacterMakingRequest, "I don't currently have any portals configured.");
                }
            }
            else
            {
                if (portalStrings.Count > 0)
                {
                    foreach (string portal in portalStrings)
                    {
                        SendTell(Machine.CharacterMakingRequest, portal);
                    }
                }
                else
                {
                    SendTell(Machine.CharacterMakingRequest, "I don't currently have any portals configured.");
                }
            }
        }

        /// <summary>
        /// Builds the list of portals to respond with.
        /// </summary>
        /// <returns>List of formatted strings to be sent to the requestor.</returns>
        private List<string> GetPortals()
        {
            List<string> portalStrings = new List<string>();
            for (int i = 0; i < Machine.Utility.CharacterSettings.Characters.Count; i++)
            {
                foreach (Portal portal in Machine.Utility.CharacterSettings.Characters[i].Portals)
                {
                    if (!string.IsNullOrEmpty(portal.Keyword))
                    {
                        portalStrings.Add($"{portal.Keyword} -> {portal.Description}{(portal.Level > 0 ? " [" + portal.Level + "+]" : "")}");
                    }
                }
            }
            return portalStrings;
        }

        /// <summary>
        /// Determines the character that the portal is on and sets the appropriate variables on the machine to take action.
        /// </summary>
        /// <param name="message"></param>
        private void CheckCommands(string message)
        {
            for (int i = 0; i < Machine.Utility.CharacterSettings.Characters.Count; i++)
            {
                foreach (Portal portal in Machine.Utility.CharacterSettings.Characters[i].Portals)
                {
                    if (portal.Keyword.Equals(message))
                    {
                        Machine.PortalDescription = portal.Description;
                        Machine.NextHeading = portal.Heading;
                        Machine.NextCharacter = Machine.Utility.CharacterSettings.Characters[i].Name;
                        if (Machine.Core.CharacterFilter.Name.Equals(Machine.Utility.CharacterSettings.Characters[i].Name)) // portal is on this character
                        {
                            if (portal.Type.Equals(PortalType.Primary))
                            {
                                Machine.SpellsToCast.Add(157);
                            }
                            else if (portal.Type.Equals(PortalType.Secondary))
                            {
                                Machine.SpellsToCast.Add(2648);
                            }
                            break;
                        }
                        else // not on this character
                        {
                            SendTell(Machine.CharacterMakingRequest, $"The requested portal to {Machine.PortalDescription} is on another character, please standby.");
                            Machine.GetNextCharacter();
                            Broadcast($"Be right back, switching to {Machine.NextCharacter} to summon {Machine.PortalDescription}.");

                            if (portal.Type.Equals(PortalType.Primary))
                            {
                                Machine.SpellsToCast.Add(157);
                            }
                            else if (portal.Type.Equals(PortalType.Secondary))
                            {
                                Machine.SpellsToCast.Add(2648);
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Send a tell to someone. 
        /// </summary>
        /// <param name="target">Character name you wish to send a tell to.</param>
        /// <param name="message">Message to send to the character.</param>
        public void SendTell(string target, string message)
        {
            Machine.Core.Actions.InvokeChatParser($"/t {target}, {message}");
        }

        /// <summary>
        /// Sends a message to local area chat.
        /// </summary>
        /// <param name="message">Message to say out in the open.</param>
        public void Broadcast(string message)
        {
            Machine.Core.Actions.InvokeChatParser(message);
        }
    }
}