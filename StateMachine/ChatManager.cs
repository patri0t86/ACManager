using ACManager.Settings;
using ACManager.StateMachine.Queues;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Filters;
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
        private Machine Machine { get; set; }
        private string CharacterMakingRequest { get; set; }

        /// <summary>
        /// Instantiates the ChatManager and sets the internal state machine instance to this.
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
                CharacterMakingRequest = match.Groups["dupleName"].Value.Substring(0, match.Groups["dupleName"].Value.Length / 2);

                if ((message.Equals("whereto") || message.Equals("where to")) && Machine.Update())
                {
                    RespondWithPortals();
                }

                if (message.Equals("profiles") && !string.IsNullOrEmpty(Machine.BuffingCharacter))
                {
                    RespondWithProfiles();
                }

                if (Machine.RespondToOpenChat)
                {
                    CheckCommands(guid, message);
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
            CharacterMakingRequest = match.Groups["dupleName"].Value.Substring(0, match.Groups["dupleName"].Value.Length / 2);

            if (!string.IsNullOrEmpty(message) && Machine.Update())
            {
                if (message.Equals("whereto") || message.Equals("where to"))
                {
                    RespondWithPortals();
                }
                else if (message.Equals("help"))
                {
                    SendTell(CharacterMakingRequest, "My list of commands are: profiles, whereto, and comps.");
                }
                else if (message.Equals("profiles"))
                {
                    RespondWithProfiles();
                }
                else if (message.Equals("comps"))
                {
                    Dictionary<string, int> components = new Dictionary<string, int>();
                    for (int i = 0; i < Machine.Core.Filter<FileService>().ComponentTable.Length; i++)
                    {
                        using (WorldObjectCollection collection = Machine.Core.WorldFilter.GetInventory())
                        {
                            collection.SetFilter(new ByNameFilter(Machine.Core.Filter<FileService>().ComponentTable[i].Name));
                            if (collection.Quantity > 0)
                            {
                                if (collection.First.Name.Contains("Scarab"))
                                {
                                    switch (collection.First.Name)
                                    {
                                        case "Lead Scarab":
                                            components.Add(collection.First.Name, collection.Quantity);
                                            break;
                                        case "Iron Scarab":
                                            components.Add(collection.First.Name, collection.Quantity);
                                            break;
                                        case "Copper Scarab":
                                            components.Add(collection.First.Name, collection.Quantity);
                                            break;
                                        case "Silver Scarab":
                                            components.Add(collection.First.Name, collection.Quantity);
                                            break;
                                        case "Gold Scarab":
                                            components.Add(collection.First.Name, collection.Quantity);
                                            break;
                                        case "Pyreal Scarab":
                                            components.Add(collection.First.Name, collection.Quantity);
                                            break;
                                        case "Platinum Scarab":
                                            components.Add(collection.First.Name, collection.Quantity);
                                            break;
                                        case "Mana Scarab":
                                            components.Add(collection.First.Name, collection.Quantity);
                                            break;
                                    }
                                }
                                else
                                {
                                    components.Add(collection.First.Name, collection.Quantity);
                                }
                            }
                        }
                    }

                    if (components.Count > 0)
                    {
                        string[] keys = new string[components.Count];
                        int[] values = new int[components.Count];
                        components.Keys.CopyTo(keys, 0);
                        components.Values.CopyTo(values, 0);
                        StringBuilder sb = new StringBuilder();
                        sb.Append("I currently have ");
                        for (int i = 0; i < components.Count; i++)
                        {
                            if (i.Equals(components.Count - 1) && components.Count > 1)
                            {
                                sb.Append($"and ");
                            }
                            sb.Append($"{values[i]} {keys[i]}{(values[i] > 1 ? "s" : "")}");
                            if (i < components.Count - 1)
                            {
                                sb.Append($", ");
                            }
                            if (i.Equals(components.Count - 1))
                            {
                                sb.Append(".");
                            }
                        }
                        SendTell(CharacterMakingRequest, sb.ToString());
                    }
                    else
                    {
                        SendTell(CharacterMakingRequest, "I don't have any components.");
                    }
                }
                else
                {
                    CheckCommands(guid, message, true);
                }
            }
        }

        /// <summary>
        /// Responds with all available portals.
        /// </summary>
        private void RespondWithPortals()
        {
            List<string> responeStrings = new List<string>();
            responeStrings.AddRange(GetPortals());
            responeStrings.AddRange(GetGems());
            if (responeStrings.Count > 0)
            {
                SendTell(CharacterMakingRequest, "You can /t me any keyword, and I will summon the corresponding portal.");
                if (Machine.Verbosity > 0)
                {
                    RespondWithVerbosity(responeStrings);
                }
                else
                {
                    foreach (string response in responeStrings)
                    {
                        SendTell(CharacterMakingRequest, response);
                    }
                }
            }
            else
            {
                SendTell(CharacterMakingRequest, "I don't currently have any portals configured.");
            }
        }

        private void RespondWithVerbosity(List<string> stringList)
        {

            StringBuilder sb = new StringBuilder();
            int count = 0;
            for (int i = 0; i < stringList.Count; i++)
            {
                sb.Append(stringList[i]);
                count++;
                if (count.Equals(Machine.Verbosity + 1))
                {
                    SendTell(CharacterMakingRequest, sb.ToString());
                    sb.Length = 0;
                    sb.Capacity = 0;
                    sb.Capacity = 16;
                    count = 0;
                }
                else if (count < Machine.Verbosity + 1 && !i.Equals(stringList.Count - 1))
                {
                    sb.Append(", ");
                }

                if (i.Equals(stringList.Count - 1) && !string.IsNullOrEmpty(sb.ToString()))
                {
                    SendTell(CharacterMakingRequest, sb.ToString());
                    sb.Length = 0;
                    sb.Capacity = 0;
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
                        portalStrings.Add($"{portal.Keyword} -> {(string.IsNullOrEmpty(portal.Description) ? "No description" : portal.Description)}{(portal.Level > 0 ? " [" + portal.Level + "+]" : "")}");
                    }
                }
            }
            return portalStrings;
        }

        private List<string> GetGems()
        {
            List<string> gemStrings = new List<string>();
            foreach (GemSetting gemSetting in Machine.Utility.BotSettings.GemSettings)
            {
                if (!string.IsNullOrEmpty(gemSetting.Keyword))
                {
                    gemStrings.Add($"{gemSetting.Keyword} -> {gemSetting.Name}");
                }
            }
            return gemStrings;
        }

        private void RespondWithProfiles()
        {
            if (!string.IsNullOrEmpty(Machine.BuffingCharacter))
            {
                List<string> profiles = new List<string>();
                foreach (BuffProfile profile in Machine.Utility.BuffProfiles)
                {
                    foreach (string command in profile.Commands)
                    {
                        profiles.Add(command);
                    }
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("My profile commands are: ");
                for (int i = 0; i < profiles.Count; i++)
                {
                    if (!profiles[i].Equals("botbuffs") && !profiles[i].Equals("botbuffs7"))
                    {
                        if (!i.Equals(profiles.Count - 1))
                        {
                            sb.Append($"{profiles[i]}");
                            sb.Append(", ");
                        }
                        else
                        {
                            if (profiles.Count.Equals(1))
                            {
                                sb.Append($"{profiles[i]}.");
                            }
                            else
                            {
                                sb.Append($"and {profiles[i]}.");
                            }
                        }
                    }
                }

                SendTell(CharacterMakingRequest, sb.ToString());
            }
            else
            {
                SendTell(CharacterMakingRequest, "I am not currently acting as a buff bot.");
            }
        }

        /// <summary>
        /// Determines the character that the portal is on and sets the appropriate variables on the machine to take action.
        /// </summary>
        /// <param name="message"></param>
        private void CheckCommands(int guid, string message, bool fromTell = false)
        {
            Machine.Inventory.UpdateInventoryFile();

            // checking for portal keywords
            for (int i = 0; i < Machine.Utility.CharacterSettings.Characters.Count; i++)
            {
                foreach (Portal portal in Machine.Utility.CharacterSettings.Characters[i].Portals)
                {
                    if (portal.Keyword.Equals(message))
                    {
                        Request newRequest = new Request
                        {
                            RequestType = RequestType.Portal,
                            RequesterName = CharacterMakingRequest,
                            Destination = portal.Description,
                            Heading = portal.Heading,
                            Character = Machine.Utility.CharacterSettings.Characters[i].Name
                        };

                        if (Machine.Core.CharacterFilter.Name.Equals(Machine.Utility.CharacterSettings.Characters[i].Name))
                        {
                            if (portal.Type.Equals(PortalType.Primary))
                            {
                                newRequest.SpellsToCast.Add(157);
                            }
                            else if (portal.Type.Equals(PortalType.Secondary))
                            {
                                newRequest.SpellsToCast.Add(2648);
                            }
                        }
                        else
                        {
                            if (portal.Type.Equals(PortalType.Primary))
                            {
                                newRequest.SpellsToCast.Add(157);
                            }
                            else if (portal.Type.Equals(PortalType.Secondary))
                            {
                                newRequest.SpellsToCast.Add(2648);
                            }
                        }
                        AddToQueue(newRequest);
                    }
                }
            }

            // checking for gem keywords
            foreach (GemSetting gemSetting in Machine.Utility.BotSettings.GemSettings)
            {
                if (gemSetting.Keyword.Equals(message))
                {
                    Request newRequest = new Request
                    {
                        RequestType = RequestType.Gem,
                        RequesterName = CharacterMakingRequest,
                        Destination = gemSetting.Name,
                        Heading = gemSetting.Heading,
                        ItemToUse = gemSetting.Name
                    };

                    if (Machine.Inventory.GetInventoryCount(gemSetting.Name) > 0)
                    {
                        newRequest.Character = Machine.Core.CharacterFilter.Name;
                        AddToQueue(newRequest);
                    }
                    else
                    {
                        for (int i = 0; i < Machine.Utility.Inventory.CharacterInventories.Count; i++)
                        {
                            for (int j = 0; j < Machine.Utility.Inventory.CharacterInventories[i].Gems.Count; j++)
                            {
                                if (Machine.Utility.Inventory.CharacterInventories[i].Gems[j].Name.Equals(gemSetting.Name) && Machine.Utility.Inventory.CharacterInventories[i].Gems[j].Quantity > 0)
                                {
                                    newRequest.Character = Machine.Utility.Inventory.CharacterInventories[i].Name;
                                    AddToQueue(newRequest);
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(Machine.BuffingCharacter) && fromTell)
            {
                if (!message.Equals("botbuffs") && !message.Equals("botbuffs7"))
                {
                    foreach (BuffProfile profile in Machine.Utility.BuffProfiles)
                    {
                        if (profile.Commands.Contains(message))
                        {
                            Request newRequest = new Request
                            {
                                RequestType = RequestType.Buff,
                                Character = Machine.BuffingCharacter,
                                RequesterName = CharacterMakingRequest,
                                RequesterGuid = guid
                            };
                            foreach (Buff buff in profile.Buffs)
                            {
                                newRequest.SpellsToCast.Add(buff.SpellId);
                            }
                            AddToQueue(newRequest);
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
            if (!string.IsNullOrEmpty(target) && !string.IsNullOrEmpty(message))
            {
                Machine.Core.Actions.InvokeChatParser($"/t {target}, {message}");
            }
        }

        /// <summary>
        /// Sends a message to local area chat.
        /// </summary>
        /// <param name="message">Message to say out in the open.</param>
        public void Broadcast(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Machine.Core.Actions.InvokeChatParser(message);
            }
        }

        public void AddToQueue(Request newRequest)
        {
            if (Machine.Requests.Contains(newRequest))
            {
                SendTell(CharacterMakingRequest, $"You already have a {newRequest.RequestType} request in.");
            }
            else if (Machine.CurrentRequest.Equals(newRequest))
            {
                SendTell(CharacterMakingRequest, $"I'm already helping you, please be patient.");
            }
            else
            {
                Machine.Requests.Enqueue(newRequest);

                // Give an estimated wait time
                TimeSpan waitTime;
                int seconds = 0;
                string lastCharacter = Machine.Core.CharacterFilter.Name;

                int currentRequest = Machine.SpellsToCast.Count * 4;

                // add the current request time in
                seconds += currentRequest;

                foreach (Request request in Machine.Requests)
                {
                    seconds += request.SpellsToCast.Count * 4;

                    if (!request.Character.Equals(lastCharacter))
                    {
                        lastCharacter = request.Character;
                        seconds += 17;
                    }
                }

                string estimatedWait = "";

                if (Machine.Requests.Count > 1)
                {
                    seconds += Machine.Requests.Count * 5;
                    waitTime = TimeSpan.FromSeconds(seconds);
                    estimatedWait = $" I should be able to get to your request in {waitTime.Minutes} minutes and {waitTime.Seconds} seconds, or less.";
                }
                else if (!string.IsNullOrEmpty(Machine.CurrentRequest.RequesterName))
                {
                    seconds = currentRequest + 5;
                    waitTime = TimeSpan.FromSeconds(seconds);
                    estimatedWait = $" I should be able to get to your request in {waitTime.Minutes} minutes and {waitTime.Seconds} seconds, or less.";
                }

                if (Machine.Requests.Count.Equals(1) && string.IsNullOrEmpty(Machine.CurrentRequest.RequesterName))
                {
                    SendTell(CharacterMakingRequest, "I have received your request and will help you now.");
                }
                else if (Machine.Requests.Count.Equals(1) && !string.IsNullOrEmpty(Machine.CurrentRequest.RequesterName))
                {
                    SendTell(CharacterMakingRequest, $"I have received your request. There is currently 1 request in the queue ahead of you.{(!string.IsNullOrEmpty(estimatedWait) ? estimatedWait : "")}");
                }
                else
                {
                    SendTell(CharacterMakingRequest, $"I have received your request. There are currently {Machine.Requests.Count} requests in the queue ahead of you, including the person I'm currently helping.{(!string.IsNullOrEmpty(estimatedWait) ? estimatedWait : "")}");
                }
            }
        }
    }
}
